# CSharpier in Visual Studio Extension: DOTNET_ROOT Conflict Issue

## Problem Background

When integrating the csharpier command-line tool into a Visual Studio VisualStudio.Extensibility extension, the tool runs normally in CMD/PowerShell but fails when called from within the VS extension.

### Environment

- Visual Studio 2022 (18.0 Insiders)
- CSharpier 1.1.2 (global tool installed via `dotnet tool install -g csharpier`)
- Extension based on Out-of-Process model (.NET 8)
- Operating System: Windows

## Problem Manifestation

### Error Message

```
You must install or update .NET to run this application.

App: C:\Users\haeer\.dotnet\tools\csharpier.exe
Architecture: x64
Framework: 'Microsoft.NETCore.App', version '10.0.0-preview.6.25358.103' (x64)
.NET location: C:\Program Files\Microsoft Visual Studio\18\Insiders\dotnet\net8.0\runtime

The following frameworks were found:
  8.0.21 at [C:\Program Files\Microsoft Visual Studio\18\Insiders\dotnet\net8.0\runtime\shared\Microsoft.NETCore.App]
```

### Behavior Differences

| Environment | Result | Reason |
|-------------|--------|--------|
| CMD/PowerShell | ✅ Works | Uses system default .NET runtime |
| VS Extension | ❌ Fails | Uses VS built-in .NET runtime |

## Investigation Process

### Initial Hypothesis: Path or Working Directory Issue

At first, suspected it was a working directory issue because:
- The error message contained "目录名称无效" (invalid directory name)
- Extension code used `Path.GetDirectoryName(filePath)` to get the working directory

Created test demo `DemoCSharpEnv.cs` to verify:
- Test 1: No WorkingDirectory set → ✅ Success
- Test 2: Valid existing WorkingDirectory → ✅ Success
- Test 3: Non-existent WorkingDirectory → ❌ Exactly reproduces the error

Conclusion: Working directory exists, not the root cause.

### Key Discovery: Environment Variable Differences

Created diagnostic command that outputs all relevant information. Key findings:

**Environment Variable Comparison:**

```
CMD Environment:
PATH: C:\Program Files\dotnet\
      C:\Users\haeer\.dotnet\tools
DOTNET_ROOT: (not set or points to user-installed .NET)

VS Extension Environment:
PATH: C:\Program Files\dotnet\
      C:\Users\haeer\.dotnet\tools
DOTNET_ROOT: C:\Program Files\Microsoft Visual Studio\18\Insiders\dotnet\net8.0\runtime
```

The difference is clear: **Visual Studio forcibly sets the DOTNET_ROOT environment variable**.

## Root Cause

1. **Visual Studio sets DOTNET_ROOT**: VS sets `DOTNET_ROOT` to its built-in .NET runtime path when launching, used to run the VS extension host process
2. **Child process inherits environment variables**: When the extension uses `Process.Start()` to launch csharpier, it inherits VS's environment variables
3. **Runtime version mismatch**: The installed csharpier was compiled with .NET 10.0 preview, but VS's DOTNET_ROOT only points to .NET 8.0
4. **.NET runtime selection logic**: When DOTNET_ROOT is set, the .NET runtime loader prioritizes that location to find runtime files, ignoring other locations

### Why CMD Works

CMD/PowerShell doesn't set DOTNET_ROOT, so .NET's runtime loader follows normal search order:
1. Application directory
2. Global installation path (typically `C:\Program Files\dotnet\`)
3. User tools path (`%USERPROFILE%\.dotnet\tools`)

This allows it to find the correct runtime version.

## Solution

### Implementation

Remove DOTNET_ROOT environment variable when starting the csharpier process, allowing it to use the system's default .NET runtime.

**Code modification (in `CSharpFormatService.cs`):**

```csharp
var processInfo = new ProcessStartInfo
{
    FileName = "csharpier",
    Arguments = arguments,
    WorkingDirectory = workingDirectory,
    UseShellExecute = false,
    RedirectStandardInput = true,
    RedirectStandardOutput = true,
    RedirectStandardError = true,
    CreateNoWindow = true
};

// Fix: Remove VS-set DOTNET_ROOT to avoid runtime version conflicts
// VS sets DOTNET_ROOT to its built-in .NET runtime (e.g., net8.0)
// while csharpier may require a different runtime version
// Removing it lets csharpier use the system default .NET runtime
processInfo.Environment.Remove("DOTNET_ROOT");
processInfo.Environment.Remove("DOTNET_ROOT(x86)");

using var process = Process.Start(processInfo);
```

### Verification Method

Created diagnostic command for testing:
- Test 1: Use current environment (with VS's DOTNET_ROOT) → ❌ Fails
- Test 2: Remove DOTNET_ROOT → ✅ Success

Test results confirm the fix is effective.

## Alternative Solutions

### Option 1: Reinstall Compatible CSharpier Version

```bash
# Uninstall current version
dotnet tool uninstall -g csharpier

# Install version compatible with .NET 8.0
dotnet tool install -g csharpier --version 0.28.2
```

Downside: Requires user manual intervention, not ideal for distributed extensions.

### Option 2: Use Full Path + Set DOTNET_ROOT

```csharp
processInfo.FileName = "C:\\Users\\haeer\\.dotnet\\tools\\csharpier.exe";
processInfo.Environment["DOTNET_ROOT"] = "C:\\Program Files\\dotnet\\";
```

Downside: Hard to get user's actual .NET installation path, poor cross-machine compatibility.

### Option 3: Remove DOTNET_ROOT (Recommended)

Simplest and most reliable, suitable for all scenarios.

## Applicable Scenarios

This issue applies to any VS extension that needs to call external .NET tools, such as:
- Code formatters (csharpier, dotnet-format)
- Code analyzers
- Build tools
- Test runners
- Any .NET global tool

## Related Documentation

- [.NET Runtime Environment Variables](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-environment-variables)
- [ProcessStartInfo.Environment Property](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.processstartinfo.environment)
- [CSharpier CLI Documentation](https://csharpier.com/docs/CLI)
- [Visual Studio Extensibility: Out-of-Process Extensions](https://learn.microsoft.com/en-us/visualstudio/extensibility/visualstudio.extensibility/)

## Summary

When calling external .NET tools from a VS extension:
1. Be aware that VS sets the `DOTNET_ROOT` environment variable
2. If the tool requires a different .NET runtime version than VS's built-in version, conflicts may occur
3. Solution: Use `processInfo.Environment.Remove("DOTNET_ROOT")` to clear the environment variable
4. This allows external tools to use the system's default .NET runtime for proper execution

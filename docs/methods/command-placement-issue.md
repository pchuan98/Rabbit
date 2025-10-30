# Command Placement Control Issue

## Problem Background

1. Based on Out-of-Process extension model
2. Want to have our own menu group with flexible control over position and placement
3. Want to maintain the .NET 8 target framework for the main program

## Desired Outcome

- Add menu under the file editor for specified format files
- At the top of the right-click context menu, in a separate group

## Attempted Solutions

### Solution 1: Pure Out-of-Process Mode

This method directly uses:

```cs
public static CommandPlacement VsctParent(Guid guid, uint id, ushort priority);
```

Utilizing the `vsshlids.h` file to manually specify GUID and ID information.

Encountered many issues, here are the main problems:

1. Unclear which GUIDs match which IDs, documentation is very scarce and AI assistance often gets it wrong
2. Even when finding the correct GUID and ID, menu position is uncontrollable and cannot be placed in the desired location

### Solution 2: Hybrid Mode (In-Process Hosting)

- [in-proc-extensions](https://learn.microsoft.com/en-us/visualstudio/extensibility/visualstudio.extensibility/get-started/in-proc-extensions?view=vs-2022)

Microsoft does provide this solution, but it requires specifying the .NET Framework 4.8 framework and requires the entire extension to be programmed in In-Proc mode.

*If using it, use the latest solution and target framework :)*

## Final Solution: Create Two Projects

### Key Discovery

During extension testing, because the project [Xavalon/XamlStyler](https://github.com/Xavalon/XamlStyler) achieves very high placement in the XAML file right-click menu, I wondered if I could add my Command to its Group and ID.

Here is part of its VSCT file code:

```xml
<CommandPlacement guid="GuidXamlStylerMenuSet" id="CommandIDFormatXamlFile" priority="0x0100">
    <Parent guid="GuidXamlStylerMenuSet" id="GroupIDXamlStylerMenu" />
</CommandPlacement>
```

My implementation in the C# code was:

```
CommandPlacement.VsctParent(
    new Guid("83fc41d5-eacb-4fa8-aaa3-9a9bdd5f6407"), // GuidXamlStylerMenuSet (XAML Styler's GUID)
    0x1020, // GroupIDXamlStylerMenu (XAML Styler's main menu group)
    priority: 0x0001 // High priority, placed at the front
),
```

It turned out to work correctly, proving that you can indeed directly use third-party extension Groups and IDs to add your own Commands.

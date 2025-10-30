using System.Diagnostics;

namespace Rabbit.ConsoleExample;

/// <summary>
/// 测试环境变量对 csharpier 执行的影响
/// </summary>
internal static class DemoCSharpEnv
{
    private const string TestCode = """
                                    namespace Test;
                                    public class Foo{public void Bar(){}}
                                    """;

    public static async Task Exec()
    {
        Console.WriteLine("=== 环境变量测试 ===\n");

        // 测试1: 不设置 WorkingDirectory (当前环境)
        await TestWithoutWorkingDirectory();

        // 测试2: 设置 WorkingDirectory (模拟 VS 扩展)
        await TestWithWorkingDirectory();

        // 测试3: 打印环境变量差异
        PrintEnvironmentInfo();

        // 测试4: 显式清除/设置环境变量
        await TestWithModifiedEnvironment();
    }

    private static async Task TestWithoutWorkingDirectory()
    {
        Console.WriteLine("=== 测试1: 不设置 WorkingDirectory ===");
        Console.WriteLine($"当前目录: {Directory.GetCurrentDirectory()}");

        var (output, errors, exitCode) = await RunCSharpier(
            TestCode,
            workingDirectory: null);

        Console.WriteLine($"退出码: {exitCode}");
        if (exitCode != 0)
        {
            Console.WriteLine($"错误信息: {errors}");
        }
        else
        {
            Console.WriteLine("成功！");
        }
        Console.WriteLine();
    }

    private static async Task TestWithWorkingDirectory()
    {
        Console.WriteLine("=== 测试2: 设置 WorkingDirectory ===");

        // 测试2a: 使用存在的目录
        var existingDir = Path.GetDirectoryName(typeof(DemoCSharpEnv).Assembly.Location)!;
        Console.WriteLine($"测试2a - 存在的目录: {existingDir}");
        Console.WriteLine($"目录是否存在: {Directory.Exists(existingDir)}");

        var (output1, errors1, exitCode1) = await RunCSharpier(
            TestCode,
            workingDirectory: existingDir);

        Console.WriteLine($"退出码: {exitCode1}");
        if (exitCode1 != 0)
        {
            Console.WriteLine($"错误信息: {errors1}");
        }
        else
        {
            Console.WriteLine("成功！");
        }

        // 测试2b: 使用不存在的目录（模拟 VS 扩展可能的问题）
        Console.WriteLine("\n测试2b - 不存在的目录:");
        var nonExistentDir = @"D:\NonExistent\Path\That\Does\Not\Exist";
        Console.WriteLine($"目录: {nonExistentDir}");
        Console.WriteLine($"目录是否存在: {Directory.Exists(nonExistentDir)}");

        try
        {
            var (output2, errors2, exitCode2) = await RunCSharpier(
                TestCode,
                workingDirectory: nonExistentDir);

            Console.WriteLine($"退出码: {exitCode2}");
            Console.WriteLine($"错误信息: {errors2}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 捕获到异常: {ex.GetType().Name}");
            Console.WriteLine($"   消息: {ex.Message}");
        }

        Console.WriteLine();
    }

    private static void PrintEnvironmentInfo()
    {
        Console.WriteLine("=== 测试3: 环境信息 ===");

        // 关键环境变量
        var envVars = new[]
        {
            "PATH",
            "DOTNET_ROOT",
            "DOTNET_ROOT(x86)",
            "DOTNET_MULTILEVEL_LOOKUP",
            "DOTNET_CLI_TELEMETRY_OPTOUT",
            "DOTNET_SKIP_FIRST_TIME_EXPERIENCE"
        };

        foreach (var varName in envVars)
        {
            var value = Environment.GetEnvironmentVariable(varName);
            if (!string.IsNullOrEmpty(value))
            {
                Console.WriteLine($"{varName}:");
                if (varName == "PATH")
                {
                    // PATH 太长，只显示相关部分
                    var paths = value.Split(';');
                    var dotnetPaths = paths.Where(p => p.Contains("dotnet", StringComparison.OrdinalIgnoreCase));
                    foreach (var path in dotnetPaths)
                    {
                        Console.WriteLine($"  - {path}");
                    }
                }
                else
                {
                    Console.WriteLine($"  {value}");
                }
            }
        }

        // 检查 csharpier 的位置
        Console.WriteLine("\ncsharpier 路径:");
        var whereResult = ExecuteCommand("where", "csharpier");
        Console.WriteLine($"  {whereResult}");

        Console.WriteLine();
    }

    private static async Task TestWithModifiedEnvironment()
    {
        Console.WriteLine("=== 测试4: 显式设置环境变量 ===");

        var (output, errors, exitCode) = await RunCSharpierWithCustomEnv(TestCode);

        Console.WriteLine($"退出码: {exitCode}");
        if (exitCode != 0)
        {
            Console.WriteLine($"错误信息: {errors}");
        }
        else
        {
            Console.WriteLine("成功！");
        }
        Console.WriteLine();
    }

    private static async Task<(string output, string errors, int exitCode)> RunCSharpier(
        string code,
        string? workingDirectory)
    {
        var processInfo = new ProcessStartInfo
        {
            FileName = "csharpier",
            Arguments = "format --write-stdout",
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        // 只在提供时设置 WorkingDirectory
        if (workingDirectory != null)
        {
            processInfo.WorkingDirectory = workingDirectory;
        }

        using var process = Process.Start(processInfo);
        if (process == null)
        {
            return (string.Empty, "无法启动进程", -1);
        }

        await process.StandardInput.WriteAsync(code);
        await process.StandardInput.FlushAsync();
        process.StandardInput.Close();

        var output = await process.StandardOutput.ReadToEndAsync();
        var errors = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        return (output, errors, process.ExitCode);
    }

    private static async Task<(string output, string errors, int exitCode)> RunCSharpierWithCustomEnv(
        string code)
    {
        var processInfo = new ProcessStartInfo
        {
            FileName = "csharpier",
            Arguments = "format --write-stdout",
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        // 显式复制环境变量
        processInfo.Environment.Clear();
        foreach (var key in Environment.GetEnvironmentVariables().Keys)
        {
            var keyStr = key.ToString();
            var value = Environment.GetEnvironmentVariable(keyStr);
            if (keyStr != null && value != null)
            {
                processInfo.Environment[keyStr] = value;
            }
        }

        Console.WriteLine("已复制所有环境变量到子进程");

        using var process = Process.Start(processInfo);
        if (process == null)
        {
            return (string.Empty, "无法启动进程", -1);
        }

        await process.StandardInput.WriteAsync(code);
        await process.StandardInput.FlushAsync();
        process.StandardInput.Close();

        var output = await process.StandardOutput.ReadToEndAsync();
        var errors = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        return (output, errors, process.ExitCode);
    }

    private static string ExecuteCommand(string fileName, string arguments)
    {
        try
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            using var process = Process.Start(processInfo);
            if (process == null) return "无法执行命令";

            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return output.Trim();
        }
        catch (Exception ex)
        {
            return $"错误: {ex.Message}";
        }
    }
}

using System.Diagnostics;

namespace Rabbit.ConsoleExample;

internal static class DemoAxaml
{
    private const string XamlString
        = """
          <Window xmlns="https://github.com/avaloniaui"
                  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                  xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
                  xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
                  mc:Ignorable="d" d:DesignWidth="800" d:DesignHeight="450"
                  x:Class="AvaloniaApplication27.MainWindow"
                  Title="AvaloniaApplication27">
              Welcome to Avalonia!
          </Window>
          """;

    public static async Task Exec()
    {
        var tempFile1 = await CreateTempFileWithXaml(XamlString);
        var tempFile2 = await CreateTempFileWithXaml(XamlString);

        try
        {
            Console.WriteLine("=== XAML 格式化测试开始 ===");

            // 测试1: 正常 XAML 格式化
            var stopwatch1 = await TestNormalFormatting(tempFile1);

            // 测试2: 错误 XAML 格式化
            var brokenXaml = CreateBrokenXaml();
            await File.WriteAllTextAsync(tempFile2, brokenXaml);
            var stopwatch2 = await TestBrokenFormatting(tempFile2, brokenXaml);

            // 测试3: 检测模式
            var stopwatch3 = await TestCheckMode(tempFile2);

            // 显示结果对比
            ShowPerformanceComparison(stopwatch1, stopwatch2, stopwatch3);
        }
        finally
        {
            CleanupTempFiles(tempFile1, tempFile2);
        }
    }

    private static async Task<string> CreateTempFileWithXaml(string xamlContent)
    {
        var tempFile = Path.GetTempFileName() + ".axaml";
        await File.WriteAllTextAsync(tempFile, xamlContent);
        Console.WriteLine($"创建临时文件: {tempFile}");
        return tempFile;
    }

    private static async Task<Stopwatch> TestNormalFormatting(string filePath)
    {
        Console.WriteLine("\n=== 测试1: 正常 XAML 格式化 ===");
        Console.WriteLine("原始 XAML 内容:");
        Console.WriteLine(XamlString);

        var stopwatch = Stopwatch.StartNew();
        var (output, errors) = await RunXstylerAsync(filePath, "--write-to-stdout");
        stopwatch.Stop();

        Console.WriteLine($"格式化完成！耗时: {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine("\n格式化后的结果:");
        Console.WriteLine(output);

        if (!string.IsNullOrEmpty(errors))
        {
            Console.WriteLine("\n处理信息:");
            Console.WriteLine(errors);
        }

        return stopwatch;
    }

    private static string CreateBrokenXaml()
    {
        return XamlString.Replace("</Window>", "");
    }

    private static async Task<Stopwatch> TestBrokenFormatting(string filePath, string brokenXaml)
    {
        Console.WriteLine("\n=== 测试2: 错误 XAML 格式化 ===");
        Console.WriteLine("错误的 XAML 内容:");
        Console.WriteLine(brokenXaml);

        var stopwatch = Stopwatch.StartNew();
        var (output, errors) = await RunXstylerAsync(filePath, "--write-to-stdout");
        stopwatch.Stop();

        Console.WriteLine($"错误格式化完成！耗时: {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine("\n错误 XAML 格式化结果:");
        Console.WriteLine("格式化输出:");
        Console.WriteLine(output);

        if (!string.IsNullOrEmpty(errors))
        {
            Console.WriteLine("\n处理信息:");
            Console.WriteLine(errors);
        }

        return stopwatch;
    }

    private static async Task<Stopwatch> TestCheckMode(string filePath)
    {
        Console.WriteLine("\n=== 测试3: 检测模式 ===");

        var stopwatch = Stopwatch.StartNew();
        var (output, errors) = await RunXstylerAsync(filePath, "-p -l Minimal");
        stopwatch.Stop();

        Console.WriteLine($"检测完成！耗时: {stopwatch.ElapsedMilliseconds}ms");

        return stopwatch;
    }

    private static async Task<(string output, string errors)> RunXstylerAsync(string filePath, string arguments)
    {
        var processInfo = new ProcessStartInfo
        {
            FileName = "xstyler",
            Arguments = $"-f \"{filePath}\" {arguments}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using var process = Process.Start(processInfo);
        if (process == null) return (string.Empty, string.Empty);

        var output = await process.StandardOutput.ReadToEndAsync();
        var errors = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        return (output, errors);
    }

    private static void ShowPerformanceComparison(Stopwatch normalTime, Stopwatch brokenTime, Stopwatch checkTime)
    {
        Console.WriteLine("\n=== 时间对比总结 ===");
        Console.WriteLine($"正常 XAML 格式化耗时: {normalTime.ElapsedMilliseconds}ms");
        Console.WriteLine($"错误 XAML 格式化耗时: {brokenTime.ElapsedMilliseconds}ms");
        Console.WriteLine($"检测模式耗时: {checkTime.ElapsedMilliseconds}ms");

        var timeDiff1 = brokenTime.ElapsedMilliseconds - normalTime.ElapsedMilliseconds;
        var timeDiff2 = checkTime.ElapsedMilliseconds - normalTime.ElapsedMilliseconds;

        Console.WriteLine($"\n格式化错误vs正常 差异: {timeDiff1}ms");
        Console.WriteLine($"检测模式vs正常 差异: {timeDiff2}ms");

        var savedTime = brokenTime.ElapsedMilliseconds - checkTime.ElapsedMilliseconds;
        Console.WriteLine($"优化效果: 节省了 {savedTime}ms ({(double)savedTime / brokenTime.ElapsedMilliseconds * 100:F1}%)");
        Console.WriteLine("====================");
    }

    private static void CleanupTempFiles(params string[] files)
    {
        foreach (var file in files)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }
        Console.WriteLine("临时文件已清理完成。");
    }
}

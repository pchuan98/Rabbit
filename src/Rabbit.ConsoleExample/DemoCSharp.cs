using System.Diagnostics;

namespace Rabbit.ConsoleExample;

internal static class DemoCSharp
{
    private const string CSharpCode
        = """
          namespace MyApp;
          public class Calculator{
          public int Add(int a,int b){return a+b;}
          public int Subtract(int a,int b)
          {
          return a-b;
          }
          public int Multiply(int a,int b){
          return a*b;}
          }
          """;

    public static async Task Exec()
    {
        Console.WriteLine("=== C# 格式化测试开始 ===");

        // 测试1: 正常 C# 格式化 (stdin/stdout)
        var stopwatch1 = await TestNormalFormatting();

        // 测试2: 错误 C# 格式化
        var brokenCode = CreateBrokenCode();
        var stopwatch2 = await TestBrokenFormatting(brokenCode);

        // 测试3: 使用 --skip-validation 选项
        var stopwatch3 = await TestSkipValidation();

        // 测试4: 使用文件路径 (--stdin-path)
        var stopwatch4 = await TestWithStdinPath();

        // 显示结果对比
        ShowPerformanceComparison(stopwatch1, stopwatch2, stopwatch3, stopwatch4);
    }

    private static async Task<Stopwatch> TestNormalFormatting()
    {
        Console.WriteLine("\n=== 测试1: 正常 C# 格式化 (stdin/stdout) ===");
        Console.WriteLine("原始 C# 代码:");
        Console.WriteLine(CSharpCode);

        var stopwatch = Stopwatch.StartNew();
        var (output, errors, exitCode) = await RunCSharpierAsync(CSharpCode, "--write-stdout");
        stopwatch.Stop();

        Console.WriteLine($"格式化完成！耗时: {stopwatch.ElapsedMilliseconds}ms, 退出码: {exitCode}");
        Console.WriteLine("\n格式化后的结果:");
        Console.WriteLine(output);

        if (!string.IsNullOrEmpty(errors))
        {
            Console.WriteLine("\n处理信息:");
            Console.WriteLine(errors);
        }

        return stopwatch;
    }

    private static string CreateBrokenCode()
    {
        return """
               namespace MyApp;
               public class Calculator{
               public int Add(int a,int b){return a+b;
               // 缺少闭合括号
               """;
    }

    private static async Task<Stopwatch> TestBrokenFormatting(string brokenCode)
    {
        Console.WriteLine("\n=== 测试2: 错误 C# 格式化 ===");
        Console.WriteLine("错误的 C# 代码:");
        Console.WriteLine(brokenCode);

        var stopwatch = Stopwatch.StartNew();
        var (output, errors, exitCode) = await RunCSharpierAsync(
            brokenCode,
            "--write-stdout --compilation-errors-as-warnings");
        stopwatch.Stop();

        Console.WriteLine($"错误格式化完成！耗时: {stopwatch.ElapsedMilliseconds}ms, 退出码: {exitCode}");
        Console.WriteLine("\n格式化输出:");
        Console.WriteLine(output);

        if (!string.IsNullOrEmpty(errors))
        {
            Console.WriteLine("\n处理信息:");
            Console.WriteLine(errors);
        }

        return stopwatch;
    }

    private static async Task<Stopwatch> TestSkipValidation()
    {
        Console.WriteLine("\n=== 测试3: 使用 --skip-validation 选项 ===");

        var stopwatch = Stopwatch.StartNew();
        var (output, errors, exitCode) = await RunCSharpierAsync(
            CSharpCode,
            "--write-stdout --skip-validation");
        stopwatch.Stop();

        Console.WriteLine($"格式化完成（跳过验证）！耗时: {stopwatch.ElapsedMilliseconds}ms, 退出码: {exitCode}");

        if (!string.IsNullOrEmpty(errors))
        {
            Console.WriteLine("处理信息:");
            Console.WriteLine(errors);
        }

        return stopwatch;
    }

    private static async Task<Stopwatch> TestWithStdinPath()
    {
        Console.WriteLine("\n=== 测试4: 使用 --stdin-path 参数 ===");

        // 模拟一个真实的文件路径
        var fakePath = Path.Combine(Directory.GetCurrentDirectory(), "Calculator.cs");
        Console.WriteLine($"模拟文件路径: {fakePath}");

        var stopwatch = Stopwatch.StartNew();
        var (output, errors, exitCode) = await RunCSharpierAsync(
            CSharpCode,
            $"--write-stdout --stdin-path \"{fakePath}\"");
        stopwatch.Stop();

        Console.WriteLine($"格式化完成（带路径）！耗时: {stopwatch.ElapsedMilliseconds}ms, 退出码: {exitCode}");

        if (!string.IsNullOrEmpty(errors))
        {
            Console.WriteLine("处理信息:");
            Console.WriteLine(errors);
        }

        return stopwatch;
    }

    private static async Task<(string output, string errors, int exitCode)> RunCSharpierAsync(
        string code,
        string arguments)
    {
        var processInfo = new ProcessStartInfo
        {
            FileName = "csharpier",
            Arguments = $"format {arguments}",
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using var process = Process.Start(processInfo);
        if (process == null)
        {
            return (string.Empty, "无法启动 csharpier 进程", -1);
        }

        // 将代码写入 stdin
        await process.StandardInput.WriteAsync(code);
        await process.StandardInput.FlushAsync();
        process.StandardInput.Close();

        // 读取输出
        var output = await process.StandardOutput.ReadToEndAsync();
        var errors = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        return (output, errors, process.ExitCode);
    }

    private static void ShowPerformanceComparison(
        Stopwatch normal,
        Stopwatch broken,
        Stopwatch skipValidation,
        Stopwatch withPath)
    {
        Console.WriteLine("\n=== 时间对比总结 ===");
        Console.WriteLine($"正常 C# 格式化耗时:           {normal.ElapsedMilliseconds}ms");
        Console.WriteLine($"错误 C# 格式化耗时:           {broken.ElapsedMilliseconds}ms");
        Console.WriteLine($"跳过验证格式化耗时:           {skipValidation.ElapsedMilliseconds}ms");
        Console.WriteLine($"带 stdin-path 格式化耗时:     {withPath.ElapsedMilliseconds}ms");

        var savedBySkipValidation = normal.ElapsedMilliseconds - skipValidation.ElapsedMilliseconds;
        var percentSaved = normal.ElapsedMilliseconds > 0
            ? (double)savedBySkipValidation / normal.ElapsedMilliseconds * 100
            : 0;

        Console.WriteLine($"\n使用 --skip-validation 节省: {savedBySkipValidation}ms ({percentSaved:F1}%)");
        Console.WriteLine("====================");
    }
}

using System.Diagnostics;

namespace Rabbit.Formator.Csharp;

/// <summary>
/// C# 格式化服务，使用 csharpier 命令行工具进行格式化
/// </summary>
internal class CSharpFormatService
{
    /// <summary>
    /// 格式化 C# 内容（使用 stdin/stdout）
    /// </summary>
    /// <param name="content">C# 内容</param>
    /// <param name="workingDirectory">工作目录（csharpier 将在此目录查找配置文件）</param>
    /// <param name="stdinPath">文件路径（用于解析 options 和 ignore 文件）</param>
    /// <param name="config">格式化配置选项 (CSharpierOptions，可选)</param>
    /// <returns>格式化后的 C# 字符串</returns>
    /// <exception cref="InvalidOperationException">当格式化失败时抛出</exception>
    public async Task<string> FormatAsync(
        string content,
        string workingDirectory,
        string? stdinPath = null,
        CSharpierOptions? config = null)
    {
        // 解析配置选项
        var options = config ?? new CSharpierOptions();

        // 如果提供了 stdinPath，设置到选项中
        if (!string.IsNullOrEmpty(stdinPath))
        {
            options.StdinPath = stdinPath;
        }

        // 构建命令行参数
        var arguments = options.ToString();

        // 执行 csharpier 命令
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

        // 修复：清除 VS 设置的 DOTNET_ROOT，避免运行时版本冲突
        // VS 会将 DOTNET_ROOT 设置为其内置的 .NET 运行时（如 net8.0）
        // 而 csharpier 可能需要不同版本的运行时
        // 清除后让 csharpier 使用系统默认的 .NET 运行时
        processInfo.Environment.Remove("DOTNET_ROOT");
        processInfo.Environment.Remove("DOTNET_ROOT(x86)");

        using var process = Process.Start(processInfo);
        if (process == null)
        {
            throw new InvalidOperationException("无法启动 csharpier 进程");
        }

        // 将内容写入 stdin 并立即刷新
        await process.StandardInput.WriteAsync(content);
        await process.StandardInput.FlushAsync();
        process.StandardInput.Close();

        // 读取输出
        var output = await process.StandardOutput.ReadToEndAsync();
        var errors = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        // 检查是否有错误
        if (process.ExitCode != 0)
        {
            var errorMessage = string.IsNullOrEmpty(errors)
                ? $"csharpier 执行失败，退出码: {process.ExitCode}"
                : $"csharpier 执行失败: {errors}";
            throw new InvalidOperationException(errorMessage);
        }

        // 返回格式化后的内容
        return output;
    }
}

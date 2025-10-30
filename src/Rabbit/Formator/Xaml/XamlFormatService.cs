using System.Diagnostics;

namespace Rabbit.Formator.Xaml;

/// <summary>
/// XAML 格式化服务，使用 xstyler 命令行工具进行格式化
/// </summary>
internal class XamlFormatService
{
    /// <summary>
    /// 格式化 XAML 内容
    /// </summary>
    /// <param name="content">XAML 内容</param>
    /// <param name="config">格式化配置选项 (XamlStylerOptions)</param>
    /// <returns>格式化后的 XAML 字符串</returns>
    /// <exception cref="InvalidOperationException">当格式化失败时抛出</exception>
    public async Task<string> FormatAsync(string content, object? config)
    {
        // 创建临时文件
        var tempFile = Path.GetTempFileName();
        var tempFileWithExt = Path.ChangeExtension(tempFile, ".axaml");

        try
        {
            // 将内容写入临时文件
            await File.WriteAllTextAsync(tempFileWithExt, content);

            // 解析配置选项
            var options = config as XamlStylerOptions ?? new XamlStylerOptions();

            // 设置文件路径
            options.File = tempFileWithExt;

            // 构建命令行参数
            var arguments = options.ToString();

            // 执行 xstyler 命令
            var processInfo = new ProcessStartInfo
            {
                FileName = "xstyler",
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = Process.Start(processInfo);
            if (process == null)
            {
                throw new InvalidOperationException("无法启动 xstyler 进程");
            }

            // 读取输出
            var output = await process.StandardOutput.ReadToEndAsync();
            var errors = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            // 检查是否有错误
            if (process.ExitCode != 0)
            {
                var errorMessage = string.IsNullOrEmpty(errors)
                    ? $"xstyler 执行失败，退出码: {process.ExitCode}"
                    : $"xstyler 执行失败: {errors}";
                throw new InvalidOperationException(errorMessage);
            }

            // 返回格式化后的内容
            return output;
        }
        finally
        {
            // 清理临时文件
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
            if (File.Exists(tempFileWithExt))
            {
                File.Delete(tempFileWithExt);
            }
        }
    }
}

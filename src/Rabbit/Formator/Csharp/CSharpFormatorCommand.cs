using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Microsoft.VisualStudio.Extensibility.Editor;
using Rabbit.Constants;
using Rabbit.Utils;

namespace Rabbit.Formator.Csharp;

[VisualStudioContribution]
internal class CSharpFormatorCommand(LogService log) : Command
{
    private static readonly CSharpFormatService FormatService = new();

    public override CommandConfiguration CommandConfiguration => new("%RabbitCommandString.Format%")
    {
        Placements =
        [
            CommandPlacement.VsctParent(RabbitContainer.Root, RabbitContainer.EditorContextMenuGroup, priority: 0x0002),
        ],
        Icon = new(ImageMoniker.KnownValues.FormatDocument, IconSettings.IconAndText),
        VisibleWhen = ActivationUtil.CSharpFileType
    };

    public override async Task ExecuteCommandAsync(
        IClientContext context, CancellationToken cancellationToken)
    {
        using var view = await context.GetActiveTextViewAsync(cancellationToken);
        if (view is null) return;

        try
        {
            // 获取文档 URI 和路径
            var documentUri = view.Document.Uri;
            var filePath = documentUri.LocalPath;

            if (string.IsNullOrEmpty(filePath)) return;

            // 确定工作目录：使用文件所在目录
            // csharpier 会自动从此目录向上查找 .csharpierrc 或 .editorconfig
            var workingDirectory = Path.GetDirectoryName(filePath) ?? Directory.GetCurrentDirectory();

            // 获取当前文档内容
            var currentContent = view.Document.Text.CopyToString();

            // 使用默认配置进行格式化
            // - workingDirectory: 用于查找项目配置文件
            // - stdinPath: 告诉 csharpier 文件路径，用于解析 ignore 规则
            var config = new CSharpierOptions();
            var formattedContent = await FormatService.FormatAsync(
                currentContent,
                workingDirectory,
                filePath,
                config);

            // 替换编辑器内容
            await Extensibility.Editor().EditAsync(batch =>
            {
                var editor = view.Document.AsEditable(batch);
                var documentRange = view.Document.Text;
                editor.Replace(documentRange, formattedContent);
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            // 格式化失败时显示错误提示
            await log.DebugAsync($"C# 格式化失败: {ex.Message}", cancellationToken);
        }
    }
}

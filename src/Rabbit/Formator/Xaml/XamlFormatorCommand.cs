using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Microsoft.VisualStudio.Extensibility.Editor;
using Rabbit.Constants;
using Rabbit.Utils;

namespace Rabbit.Formator.Xaml;

[VisualStudioContribution]
internal class XamlFormatorCommand(LogService log) : Command
{
    private static readonly XamlFormatService FormatService = new();

    public override CommandConfiguration CommandConfiguration => new("%RabbitCommandString.Format%")
    {
        Placements =
        [
            CommandPlacement.VsctParent(RabbitContainer.Root, RabbitContainer.EditorContextMenuGroup, priority: 0x0001),
        ],
        Icon = new(ImageMoniker.KnownValues.FormatDocument, IconSettings.IconAndText),
        VisibleWhen = ActivationConstraint.Or(
            ActivationUtil.XamlFileType,
            ActivationUtil.AxamlFileType)
    };

    public override async Task ExecuteCommandAsync(
        IClientContext context, CancellationToken cancellationToken)
    {
        using var view = await context.GetActiveTextViewAsync(cancellationToken);
        if (view is null) return;

        try
        {
            // 获取当前文档内容
            var currentContent = view.Document.Text.CopyToString();

            // 使用默认配置进行格式化
            var config = new XamlStylerOptions();
            var formattedContent = await FormatService.FormatAsync(currentContent, config);

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
            await log.DebugAsync($"XAML 格式化失败: {ex.Message}", cancellationToken);
        }
    }
}

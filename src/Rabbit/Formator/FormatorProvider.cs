using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Rabbit.Constants;

namespace Rabbit.Formator;

[VisualStudioContribution]
internal class FormatorProvider(LogService log) : Command
{
    public override CommandConfiguration CommandConfiguration => new("%Rabbit.Command.Format%")
    {
        Placements =
        [
            // 使用 RabbitContainer 提供的编辑器右键菜单组
            CommandPlacement.VsctParent(RabbitContainer.Root,RabbitContainer.EditorContextMenuGroup, priority: 0x0001),
        ],
        Icon = new(ImageMoniker.KnownValues.FormatDocument, IconSettings.IconAndText),
        TooltipText = "Format the selected code"
    };

    public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
    {
        await log.Error("hello");
    }
}

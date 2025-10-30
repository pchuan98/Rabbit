using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Microsoft.VisualStudio.Extensibility.Shell;

namespace Rabbit.LearnExtension.Task3;

[VisualStudioContribution]
internal class ShowSimpleToolWindowCommand : Command
{
    public override CommandConfiguration CommandConfiguration => new("%Rabbit.LearnExtension.ShowSimpleToolCommand%")
    {
        Placements = [CommandPlacement.KnownPlacements.ToolsMenu],
        Icon = new(ImageMoniker.KnownValues.ToolWindow, IconSettings.IconAndText),
        TooltipText = "显示简单工具窗口",
    };

    public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
    {
        await Extensibility.Shell().ShowToolWindowAsync<SimpleToolWindow>(activate: true, cancellationToken);
    }
}

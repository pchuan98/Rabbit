using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Microsoft.VisualStudio.Extensibility.Shell;

namespace Rabbit.LearnExtension.Task1;

[VisualStudioContribution]
internal class SimpleCommand : Command
{
    public override CommandConfiguration CommandConfiguration => new("%Rabbit.LearnExtension.SimpleCommand%")
    {
        Placements = [CommandPlacement.KnownPlacements.ToolsMenu],
        Icon = new(ImageMoniker.KnownValues.Extension, IconSettings.IconAndText),
        TooltipText = "%Rabbit.LearnExtension.SimpleCommand.Tooltip%",
    };

    public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
    {
        await Extensibility
            .Shell()
            .ShowPromptAsync("Hello from Simple Taks", PromptOptions.OK, cancellationToken);
    }
}

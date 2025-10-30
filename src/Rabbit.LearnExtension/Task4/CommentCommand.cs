using System.Reflection.Metadata.Ecma335;
using System.Threading;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Microsoft.VisualStudio.Extensibility.Editor;
using Microsoft.VisualStudio.Extensibility.Shell;
using Microsoft.VisualStudio.Shell;
using Newtonsoft.Json.Linq;

namespace Rabbit.LearnExtension.Task4;

[VisualStudioContribution]
internal class CommentCommand : Command
{
    public override CommandConfiguration CommandConfiguration => new("%Rabbit.LearnExtension.CommentCommand%")
    {
        Placements = [CommandPlacement.KnownPlacements.ExtensionsMenu],
        Icon = new(ImageMoniker.KnownValues.Comment, IconSettings.IconAndText),
    };

    public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
    {
        using var view = await context.GetActiveTextViewAsync(cancellationToken);

        if (view is null)  // 代表打开了任何可编辑的东西
            return;

        var selection = view.Selection.Extent;

        var start = view.Document.GetLineNumberFromPosition(selection.Start);
        var end = view.Document.GetLineNumberFromPosition(selection.End);

        //await Extensibility.Shell().ShowPromptAsync(
        //    $"{end - start + 1}",
        //    PromptOptions.OK, cancellationToken);

        //-----------------------------------------------------------------------

        // to upper
        await Extensibility.Editor().EditAsync(batch =>
        {
            var editor = view.Document.AsEditable(batch);

            for (var i = start; i <= end; i++)
            {
                var line = view.Document.Lines[i];
                
                editor.Insert(line.Text.Start, "//");
            }
        }, cancellationToken);
    }
}

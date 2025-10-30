using System.Diagnostics;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Shell;

namespace Rabbit;

internal class LogService(VisualStudioExtensibility extensibility)
{
    private const string Header = "Rabbit";

    public async Task DebugAsync(string msg, CancellationToken? token)
    {
        token ??= CancellationToken.None;

        await extensibility.Shell().ShowPromptAsync(msg, PromptOptions.OK, (CancellationToken)token);
        Debug.WriteLine(msg);
    }
}

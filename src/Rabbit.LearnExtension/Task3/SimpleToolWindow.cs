using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.ToolWindows;
using Microsoft.VisualStudio.RpcContracts.RemoteUI;

namespace Rabbit.LearnExtension.Task3;

[VisualStudioContribution]
internal class SimpleToolWindow : ToolWindow
{
    private SimpleToolData? _data;

    public override ToolWindowConfiguration ToolWindowConfiguration => new()
    {
        Placement = ToolWindowPlacement.DocumentWell
    };

    public override Task InitializeAsync(CancellationToken cancellationToken)
    {
        _data = new SimpleToolData(Extensibility);
        return Task.CompletedTask;
    }

    public override Task<IRemoteUserControl> GetContentAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IRemoteUserControl>(new SimpleToolControl(_data));
    }
}

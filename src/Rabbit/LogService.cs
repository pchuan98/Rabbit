using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.ProjectSystem.Query.Framework.Actions;
using Debug = System.Diagnostics.Debug;

namespace Rabbit;

internal class LogService(VisualStudioExtensibility extensibility)
{
    private const string ErrorTitle = "Rabbit Error";

    public async Task Error(string msg)
    {
        Debug.WriteLine("hello");
    }
}

using System.Runtime.Serialization;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.UI;

namespace Rabbit.LearnExtension.Task3;

[DataContract]
internal class SimpleToolData : NotifyPropertyChangedObject
{
    private readonly VisualStudioExtensibility _extensibility;
    private readonly AsyncCommand _updateCommand;

    public SimpleToolData(VisualStudioExtensibility extensibility)
    {
        _extensibility = extensibility;

        _updateCommand = new AsyncCommand(async (parameter, cancellationToken) =>
        {
            Message = DateTime.Now.ToString("O");
            await Task.CompletedTask;
        });
    }

    [DataMember]
    public string Message
    {
        get => field;
        set => SetProperty(ref field, value);
    } = "";

    [DataMember]
    public AsyncCommand UpdateCommand => _updateCommand;
}

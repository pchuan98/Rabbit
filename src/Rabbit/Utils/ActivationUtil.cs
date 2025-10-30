using Microsoft.VisualStudio.Extensibility;

namespace Rabbit.Utils;

public static class ActivationUtil
{
    #region FileType

    private static ActivationConstraint FileTypeFilter(string type)
        => ActivationConstraint.ClientContext(ClientContextKey.Shell.ActiveEditorContentType, type);

    public static ActivationConstraint CSharpFileType => ActivationConstraint.ClientContext(ClientContextKey.Shell.ActiveEditorFileName, ".*.cs$");

    public static ActivationConstraint XamlFileType => ActivationConstraint.ClientContext(ClientContextKey.Shell.ActiveEditorFileName, ".*.xaml$");

    public static ActivationConstraint AxamlFileType => ActivationConstraint.ClientContext(ClientContextKey.Shell.ActiveEditorFileName, ".*.axaml$");

    #endregion
}

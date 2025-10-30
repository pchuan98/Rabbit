using Microsoft.VisualStudio.Extensibility.UI;

namespace Rabbit.LearnExtension.Task3;

internal class SimpleToolControl : RemoteUserControl
{
    public SimpleToolControl(SimpleToolData dataContext)
          : base(dataContext)
    {
        // base(dataContext) 会将 dataContext 设置为这个控件的 DataContext
        // XAML 中的绑定就是绑定到这个 DataContext
    }
}

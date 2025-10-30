# 任务 11：创建对话框

## 学习目标

掌握如何创建自定义对话框，使用 XAML 设计界面并实现数据绑定。

## 核心概念

### 1. 创建继承自 RemoteUserControl 的控件类

**要实现什么**：定义对话框的代码类，继承 `RemoteUserControl`

**结果**：在构造函数中设置数据上下文和添加资源字典

### 2. 设计 XAML 数据模板

**要实现什么**：创建 `.xaml` 文件定义对话框的 UI 布局

**结果**：使用 WPF 控件（TextBlock、Button 等）设计界面

### 3. 添加嵌入式资源字典

**要实现什么**：创建 `.xaml` 资源字典定义样式和资源

**结果**：调用 `ResourceDictionaries.AddEmbeddedResource()` 加载资源

### 4. 使用动态资源引用

**要实现什么**：在 XAML 中使用 `{DynamicResource key}` 引用资源字典中的资源

**结果**：可引用字符串、样式、颜色等

### 5. 使用 Shell().ShowDialogAsync 显示对话框

**要实现什么**：创建控件实例，调用 `Shell().ShowDialogAsync(control)` 显示为模态对话框

**结果**：对话框会阻塞直到用户关闭

### 6. 实现对话框的数据绑定

**要实现什么**：在数据上下文类中定义属性，使用 `{Binding PropertyName}` 在 XAML 中绑定

**结果**：数据上下文需继承 `NotifyPropertyChangedObject`

### 7. 处理对话框的确认和取消

**要实现什么**：在控件中添加按钮，绑定到命令或事件处理

**结果**：通过修改数据上下文传递用户输入的结果

## 知识点详解

### 1. 创建数据上下文类

```csharp
using Microsoft.VisualStudio.Extensibility.UI;
using System.Runtime.Serialization;

[DataContract]
internal class MyDialogData : NotifyPropertyChangedObject
{
    private string? _userName;
    private string? _email;

    [DataMember]
    public string? UserName
    {
        get => _userName;
        set => SetProperty(ref _userName, value);
    }

    [DataMember]
    public string? Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }
}
```

**关键点：**
- 继承 `NotifyPropertyChangedObject` 支持属性变更通知
- 使用 `[DataContract]` 和 `[DataMember]` 标记可序列化属性
- 使用 `SetProperty` 自动触发 `PropertyChanged` 事件

### 2. 创建 RemoteUserControl 类

```csharp
internal class MyDialogControl : RemoteUserControl
{
    public MyDialogControl(MyDialogData dataContext)
        : base(dataContext)
    {
        // 添加嵌入式资源字典（如果有）
        this.ResourceDictionaries.AddEmbeddedResource("MyExtension.Resources.DialogResources");
    }
}
```

### 3. 创建 XAML 数据模板

创建 `MyDialogControl.xaml` 文件：

```xml
<DataTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
              xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
              xmlns:vs="http://schemas.microsoft.com/visualstudio/extensibility/2022/xaml"
              xmlns:styles="clr-namespace:Microsoft.VisualStudio.Shell;assembly=Microsoft.VisualStudio.Shell.15.0"
              xmlns:colors="clr-namespace:Microsoft.VisualStudio.PlatformUI;assembly=Microsoft.VisualStudio.Shell.15.0">
    <Grid Margin="20">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <!-- 标题 -->
        <TextBlock Grid.Row="0"
                   Text="用户信息"
                   FontSize="16"
                   FontWeight="Bold"
                   Margin="0,0,0,20"/>

        <!-- 用户名输入 -->
        <StackPanel Grid.Row="1" Margin="0,0,0,10">
            <TextBlock Text="用户名：" Margin="0,0,0,5"/>
            <TextBox Text="{Binding UserName, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"
                     MinWidth="250"/>
        </StackPanel>

        <!-- 邮箱输入 -->
        <StackPanel Grid.Row="2" Margin="0,0,0,20">
            <TextBlock Text="邮箱：" Margin="0,0,0,5"/>
            <TextBox Text="{Binding Email, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"
                     MinWidth="250"/>
        </StackPanel>

        <!-- 按钮 -->
        <StackPanel Grid.Row="3" Orientation="Horizontal" HorizontalAlignment="Right">
            <Button Content="确定"
                    IsDefault="True"
                    MinWidth="80"
                    Margin="0,0,10,0"
                    Command="{Binding CloseCommand}"/>
            <Button Content="取消"
                    IsCancel="True"
                    MinWidth="80"/>
        </StackPanel>
    </Grid>
</DataTemplate>
```

### 4. 显示对话框

```csharp
[VisualStudioContribution]
internal class ShowDialogCommand : Command
{
    public override CommandConfiguration CommandConfiguration => new("%ShowDialog.DisplayName%")
    {
        Placements = [CommandPlacement.KnownPlacements.ToolsMenu],
    };

    public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
    {
        // 创建数据上下文
        var dialogData = new MyDialogData
        {
            UserName = "默认用户",
            Email = "user@example.com"
        };

        // 创建对话框控件
        #pragma warning disable CA2000 // Dispose objects before losing scope
        var control = new MyDialogControl(dialogData);
        #pragma warning restore CA2000

        // 显示对话框（模态）
        await this.Extensibility.Shell().ShowDialogAsync(control, cancellationToken);

        // 对话框关闭后，可以读取用户输入
        if (!string.IsNullOrEmpty(dialogData.UserName))
        {
            await this.Extensibility.Shell().ShowPromptAsync(
                $"用户: {dialogData.UserName}, 邮箱: {dialogData.Email}",
                PromptOptions.OK,
                cancellationToken);
        }
    }
}
```

## 使用资源字典

### 1. 创建资源字典

创建 `DialogResources.xaml` 文件：

```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

    <!-- 字符串资源 -->
    <System:String x:Key="dialogTitle">用户信息</System:String>
    <System:String x:Key="userNameLabel">用户名：</System:String>

    <!-- 样式资源 -->
    <Style x:Key="TitleTextStyle" TargetType="TextBlock">
        <Setter Property="FontSize" Value="16"/>
        <Setter Property="FontWeight" Value="Bold"/>
        <Setter Property="Margin" Value="0,0,0,20"/>
    </Style>
</ResourceDictionary>
```

### 2. 配置为嵌入式资源

在 `.csproj` 文件中：

```xml
<ItemGroup>
  <EmbeddedResource Include="Resources\DialogResources.xaml" />
  <Page Remove="Resources\DialogResources.xaml" />
</ItemGroup>
```

### 3. 在 XAML 中使用资源

```xml
<TextBlock Text="{DynamicResource dialogTitle}"
           Style="{DynamicResource TitleTextStyle}"/>
```

## 完整示例：配置对话框

```csharp
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Microsoft.VisualStudio.Extensibility.UI;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace MyExtension;

// 数据上下文
[DataContract]
internal class ConfigDialogData : NotifyPropertyChangedObject
{
    private bool _enableFeature;
    private int _timeout = 30;

    [DataMember]
    public bool EnableFeature
    {
        get => _enableFeature;
        set => SetProperty(ref _enableFeature, value);
    }

    [DataMember]
    public int Timeout
    {
        get => _timeout;
        set => SetProperty(ref _timeout, value);
    }
}

// 对话框控件
internal class ConfigDialog : RemoteUserControl
{
    public ConfigDialog(ConfigDialogData dataContext)
        : base(dataContext)
    {
    }
}

// 命令
[VisualStudioContribution]
internal class ShowConfigCommand : Command
{
    public override CommandConfiguration CommandConfiguration => new("%Config.DisplayName%")
    {
        Placements = [CommandPlacement.KnownPlacements.ToolsMenu],
        Icon = new(ImageMoniker.KnownValues.Settings, IconSettings.IconAndText),
    };

    public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
    {
        var data = new ConfigDialogData
        {
            EnableFeature = true,
            Timeout = 30
        };

        #pragma warning disable CA2000
        var dialog = new ConfigDialog(data);
        #pragma warning restore CA2000

        await this.Extensibility.Shell().ShowDialogAsync(dialog, cancellationToken);

        // 保存配置...
    }
}
```

## 常见问题

### Q: 如何关闭对话框？
A: 对话框中的按钮设置 `IsDefault="True"` 或 `IsCancel="True"`，点击会自动关闭。

### Q: 如何实现自定义关闭逻辑？
A: 在数据上下文中定义 `ICommand`，在 XAML 中绑定到按钮的 `Command` 属性。

### Q: 对话框会阻塞 UI 吗？
A: `ShowDialogAsync` 是模态对话框，会阻塞直到用户关闭。

### Q: 如何验证用户输入？
A: 在数据上下文的属性 setter 中添加验证逻辑。

## 参考资料

- llm.md 相关代码片段：
  - 第 498-514 行：显示对话框
  - 第 518-534 行：初始化数据和创建 UI
  - 第 727-738 行：配置嵌入式资源
  - 第 1176-1191 行：添加资源字典
  - 第 1394-1408 行：定义 XAML 模板
  - 第 1688-1696 行：引用动态资源

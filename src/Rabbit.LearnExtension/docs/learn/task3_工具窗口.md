# 任务 3：创建工具窗口

## 学习目标

掌握如何创建和配置 Visual Studio 工具窗口，实现数据绑定和 UI 交互。

## 核心概念

### 1. ToolWindow 类

工具窗口的基类，所有自定义工具窗口都需要继承此类。

```csharp
[VisualStudioContribution]
public class MyToolWindow : ToolWindow
{
    // 工具窗口实现
}
```

### 2. 工具窗口的三个核心部分

1. **ToolWindow 类** - 工具窗口的逻辑和配置
2. **数据模型类** - 提供数据绑定的数据源
3. **RemoteUserControl** - 定义 UI 和 XAML

## 知识点详解

### 1. 创建工具窗口类

```csharp
[VisualStudioContribution]
public class MyToolWindow : ToolWindow
{
    private MyToolWindowData? dataContext;

    public MyToolWindow(VisualStudioExtensibility extensibility)
        : base(extensibility)
    {
        this.Title = "我的工具窗口";
    }

    public override ToolWindowConfiguration ToolWindowConfiguration => new()
    {
        Placement = ToolWindowPlacement.DocumentWell,
    };

    public override Task InitializeAsync(CancellationToken cancellationToken)
    {
        this.dataContext = new MyToolWindowData(this.Extensibility);
        return Task.CompletedTask;
    }

    public override Task<IRemoteUserControl> GetContentAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IRemoteUserControl>(new MyToolWindowControl(this.dataContext));
    }
}
```

**关键方法说明：**

- **构造函数**：设置工具窗口的标题
- **ToolWindowConfiguration**：配置窗口的放置位置
- **InitializeAsync**：初始化数据上下文
- **GetContentAsync**：创建并返回 UI 控件

### 2. 配置工具窗口

#### 窗口放置位置 (Placement)

```csharp
public override ToolWindowConfiguration ToolWindowConfiguration => new()
{
    Placement = ToolWindowPlacement.DocumentWell,  // 文档区域
};
```

常用放置位置：
- `DocumentWell` - 文档区域（与代码编辑器同一区域）
- `Floating` - 浮动窗口
- `DockedToDocumentWell` - 停靠到文档区域

#### 添加工具栏

```csharp
[VisualStudioContribution]
private static ToolbarConfiguration Toolbar => new("%MyToolWindow.Toolbar.DisplayName%")
{
    Children = [ToolbarChild.Command<MyToolbarCommand>()],
};

public override ToolWindowConfiguration ToolWindowConfiguration => new()
{
    Placement = ToolWindowPlacement.DocumentWell,
    Toolbar = new ToolWindowToolbar(Toolbar),
};
```

### 3. 创建数据模型类

数据模型类需要：
- 添加 `[DataContract]` 特性（用于序列化）
- 继承 `NotifyPropertyChangedObject`（支持属性更改通知）
- 使用 `[DataMember]` 标记需要序列化的属性

```csharp
[DataContract]
internal class MyToolWindowData : NotifyPropertyChangedObject
{
    private readonly VisualStudioExtensibility extensibility;
    private string _message = "Hello from Tool Window!";
    private int _counter = 0;

    public MyToolWindowData(VisualStudioExtensibility extensibility)
    {
        this.extensibility = extensibility;
    }

    [DataMember]
    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    [DataMember]
    public int Counter
    {
        get => _counter;
        set => SetProperty(ref _counter, value);
    }

    [DataMember]
    public AsyncCommand IncrementCommand => new(async (parameter, cancellationToken) =>
    {
        Counter++;
        await Task.CompletedTask;
    });
}
```

**关键点：**
- 使用 `SetProperty` 方法更新属性，自动触发 UI 更新
- 命令使用 `AsyncCommand` 类型
- `extensibility` 字段不需要 `[DataMember]` 特性

### 4. 创建 RemoteUserControl

```csharp
internal class MyToolWindowControl : RemoteUserControl
{
    public MyToolWindowControl(object? dataContext)
        : base(dataContext)
    {
    }
}
```

**XAML 数据模板** (`MyToolWindowControl.xaml`):

```xml
<DataTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
              xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
              xmlns:vs="http://schemas.microsoft.com/visualstudio/extensibility/2022/xaml"
              xmlns:styles="clr-namespace:Microsoft.VisualStudio.Shell;assembly=Microsoft.VisualStudio.Shell.15.0"
              xmlns:colors="clr-namespace:Microsoft.VisualStudio.PlatformUI;assembly=Microsoft.VisualStudio.Shell.15.0">

    <Grid>
        <StackPanel Margin="10">
            <TextBlock Text="{Binding Message}"
                       FontSize="16"
                       Margin="0,0,0,10"/>

            <TextBlock Text="{Binding Counter, StringFormat='计数器: {0}'}"
                       Margin="0,0,0,10"/>

            <Button Content="增加计数"
                    Command="{Binding IncrementCommand}"
                    Width="100"/>
        </StackPanel>
    </Grid>
</DataTemplate>
```

### 5. 创建显示工具窗口的命令

```csharp
[VisualStudioContribution]
public class MyToolWindowCommand : Command
{
    public override CommandConfiguration CommandConfiguration => new("%MyToolWindowCommand.DisplayName%")
    {
        Placements = [CommandPlacement.KnownPlacements.ToolsMenu],
        Icon = new(ImageMoniker.KnownValues.ToolWindow, IconSettings.IconAndText),
    };

    public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
    {
        await this.Extensibility.Shell().ShowToolWindowAsync<MyToolWindow>(
            activate: true,
            cancellationToken);
    }
}
```

**ShowToolWindowAsync 参数：**
- 泛型参数 `<MyToolWindow>` - 指定要显示的工具窗口类型
- `activate: true` - 显示后激活窗口（获得焦点）

## 完整示例

### 文件结构
```
MyToolWindow/
├── MyToolWindow.cs              (工具窗口类)
├── MyToolWindowData.cs          (数据模型)
├── MyToolWindowControl.cs       (UI 控件)
├── MyToolWindowControl.xaml     (XAML 模板)
└── MyToolWindowCommand.cs       (显示命令)
```

### MyToolWindow.cs
```csharp
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.ToolWindows;
using System.Threading;
using System.Threading.Tasks;

namespace MyExtension;

[VisualStudioContribution]
public class MyToolWindow : ToolWindow
{
    private MyToolWindowData? dataContext;

    public MyToolWindow(VisualStudioExtensibility extensibility)
        : base(extensibility)
    {
        this.Title = "我的工具窗口";
    }

    public override ToolWindowConfiguration ToolWindowConfiguration => new()
    {
        Placement = ToolWindowPlacement.DocumentWell,
    };

    public override Task InitializeAsync(CancellationToken cancellationToken)
    {
        this.dataContext = new MyToolWindowData(this.Extensibility);
        return Task.CompletedTask;
    }

    public override Task<IRemoteUserControl> GetContentAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IRemoteUserControl>(new MyToolWindowControl(this.dataContext));
    }
}
```

### MyToolWindowData.cs
```csharp
using Microsoft.VisualStudio.Extensibility;
using System.Runtime.Serialization;

namespace MyExtension;

[DataContract]
internal class MyToolWindowData : NotifyPropertyChangedObject
{
    private readonly VisualStudioExtensibility extensibility;
    private string _message = "欢迎使用工具窗口！";
    private int _counter = 0;

    public MyToolWindowData(VisualStudioExtensibility extensibility)
    {
        this.extensibility = extensibility;
    }

    [DataMember]
    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    [DataMember]
    public int Counter
    {
        get => _counter;
        set => SetProperty(ref _counter, value);
    }

    [DataMember]
    public AsyncCommand IncrementCommand => new(async (parameter, cancellationToken) =>
    {
        Counter++;
        Message = $"你已经点击了 {Counter} 次！";
        await Task.CompletedTask;
    });

    [DataMember]
    public AsyncCommand ResetCommand => new(async (parameter, cancellationToken) =>
    {
        Counter = 0;
        Message = "计数器已重置";
        await Task.CompletedTask;
    });
}
```

## 实践步骤

1. **创建工具窗口类**
   - 继承 `ToolWindow`
   - 添加 `[VisualStudioContribution]` 特性
   - 配置标题和放置位置

2. **创建数据模型**
   - 继承 `NotifyPropertyChangedObject`
   - 添加 `[DataContract]` 特性
   - 定义属性和命令

3. **创建 UI 控件**
   - 创建 `RemoteUserControl` 子类
   - 创建 XAML 数据模板
   - 实现数据绑定

4. **创建显示命令**
   - 创建命令类
   - 使用 `ShowToolWindowAsync` 显示窗口

5. **测试工具窗口**
   - 运行扩展
   - 点击命令打开工具窗口
   - 测试 UI 交互和数据绑定

## 数据绑定要点

### 属性绑定
```xml
<TextBlock Text="{Binding PropertyName}" />
```

### 命令绑定
```xml
<Button Content="点击" Command="{Binding CommandName}" />
```

### 格式化绑定
```xml
<TextBlock Text="{Binding Counter, StringFormat='计数: {0}'}" />
```

### 双向绑定
```xml
<TextBox Text="{Binding Message, Mode=TwoWay}" />
```

## 常见问题

### Q: 工具窗口关闭后数据会丢失吗？
A: 不会，工具窗口隐藏时数据会保留，再次打开时恢复。

### Q: 如何在工具窗口中访问 Visual Studio 服务？
A: 通过 `VisualStudioExtensibility` 对象访问各种服务：
```csharp
this.extensibility.Shell()
this.extensibility.Editor()
this.extensibility.Workspaces()
```

### Q: 属性更改后 UI 没有更新？
A: 检查：
- 属性是否使用 `SetProperty` 方法更新
- 数据模型是否继承 `NotifyPropertyChangedObject`
- 属性是否添加了 `[DataMember]` 特性

### Q: 可以在工具窗口中使用用户控件吗？
A: 可以，但需要是 WPF 用户控件，并且数据绑定需要遵循远程 UI 的规则。

## 进阶主题

- 工具窗口工具栏配置
- 工具窗口状态持久化
- 多实例工具窗口
- 工具窗口与编辑器联动

## 参考资料

- 源代码示例：`ToolWindowSample`
- llm.md 相关代码片段：
  - 第 483-494 行：显示工具窗口命令
  - 第 518-534 行：InitializeAsync 和 GetContentAsync
  - 第 1089-1103 行：XAML 数据模板结构
  - 第 1549-1559 行：数据模型类定义
  - 第 1630-1642 行：工具窗口构造函数

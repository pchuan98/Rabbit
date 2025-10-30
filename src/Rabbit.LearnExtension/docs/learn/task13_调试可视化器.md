# 任务 13：调试可视化器

## 学习目标

掌握如何创建调试可视化器，在调试时为特定类型的对象提供自定义可视化界面。

## 核心概念

### 1. 创建继承自 DebuggerVisualizerProvider 的类

**要实现什么**：定义调试可视化器类，继承 `DebuggerVisualizerProvider`

**结果**：用于在调试时为特定类型的对象提供自定义可视化界面

### 2. 配置 DebuggerVisualizerProviderConfiguration

**要实现什么**：设置可视化器的显示名称和目标类型（如 `typeof(Match)`）

**结果**：指定这个可视化器用于哪种类型的对象

### 3. 实现可视化器对象源（Object Source）

**要实现什么**：创建单独的 netstandard2.0 项目，继承 `VisualizerObjectSource`

**结果**：在被调试进程中运行，负责提取和序列化对象数据

### 4. 实现 CreateVisualizerAsync 创建 UI

**要实现什么**：返回 `RemoteUserControl` 显示可视化内容

**结果**：接收 `VisualizerTarget` 参数用于请求调试对象数据

### 5. 使用 RequestDataAsync 获取调试对象数据

**要实现什么**：通过 `visualizerTarget.ObjectSource.RequestDataAsync<T>()` 从对象源获取数据

**结果**：数据会在被调试进程中提取并传输过来

### 6. 序列化和反序列化调试数据

**要实现什么**：对象源中实现 `GetData` 方法，将对象序列化为可传输的数据

**结果**：可视化器端反序列化数据用于显示

### 7. 打包对象源 DLL 到扩展

**要实现什么**：将对象源 DLL 作为 Content 包含在 VSIX 中

**结果**：配置 `VisualizerObjectSourceType` 引用对象源类型

## 架构说明

调试可视化器由两部分组成：

1. **对象源（Object Source）** - 运行在被调试进程中
   - netstandard2.0 项目
   - 继承 `VisualizerObjectSource`
   - 提取和序列化对象数据

2. **可视化器（Visualizer）** - 运行在 VS 进程中
   - 主扩展项目的一部分
   - 继承 `DebuggerVisualizerProvider`
   - 显示 UI 和处理交互

## 完整示例：Regex Match 可视化器

### 1. 对象源项目

创建 `RegexMatchObjectSource.csproj`（netstandard2.0）：

```csharp
using Microsoft.VisualStudio.DebuggerVisualizers;
using System.IO;
using System.Text.RegularExpressions;

public class RegexMatchObjectSource : VisualizerObjectSource
{
    public override void GetData(object target, Stream outgoingData)
    {
        if (target is Match match)
        {
            // 提取 Match 数据
            var data = new MatchData
            {
                Value = match.Value,
                Index = match.Index,
                Length = match.Length,
                Success = match.Success
            };

            // 序列化数据
            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(MatchData));
            serializer.WriteObject(outgoingData, data);
        }
    }
}

[System.Runtime.Serialization.DataContract]
public class MatchData
{
    [System.Runtime.Serialization.DataMember]
    public string? Value { get; set; }

    [System.Runtime.Serialization.DataMember]
    public int Index { get; set; }

    [System.Runtime.Serialization.DataMember]
    public int Length { get; set; }

    [System.Runtime.Serialization.DataMember]
    public bool Success { get; set; }
}
```

### 2. 可视化器提供者

```csharp
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.DebuggerVisualizers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

[VisualStudioContribution]
internal class RegexMatchVisualizerProvider : DebuggerVisualizerProvider
{
    public RegexMatchVisualizerProvider(VisualStudioExtensibility extensibility)
        : base(extensibility)
    {
    }

    public override DebuggerVisualizerProviderConfiguration DebuggerVisualizerProviderConfiguration => new(
        "Regex Match 可视化器",
        typeof(Match))
    {
        VisualizerObjectSourceType = new(typeof(RegexMatchObjectSource)),
    };

    public override Task<IRemoteUserControl> CreateVisualizerAsync(
        VisualizerTarget visualizerTarget,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<IRemoteUserControl>(
            new RegexMatchVisualizerUserControl(visualizerTarget));
    }
}
```

### 3. 可视化器 UI

```csharp
internal class RegexMatchVisualizerUserControl : RemoteUserControl
{
    private readonly VisualizerTarget visualizerTarget;

    public RegexMatchVisualizerUserControl(VisualizerTarget visualizerTarget)
        : base(dataContext: null)
    {
        this.visualizerTarget = visualizerTarget;
    }

    public override async Task ControlLoadedAsync(CancellationToken cancellationToken)
    {
        // 从对象源请求数据
        var matchData = await this.visualizerTarget.ObjectSource
            .RequestDataAsync<MatchData>(jsonSerializer: null, cancellationToken);

        // 更新数据上下文
        this.DataContext = matchData;
    }
}
```

### 4. XAML 布局

```xml
<DataTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
    <StackPanel Margin="10">
        <TextBlock Text="{Binding Value, StringFormat='匹配值: {0}'}"
                   FontWeight="Bold" FontSize="14"/>
        <TextBlock Text="{Binding Index, StringFormat='位置: {0}'}"/>
        <TextBlock Text="{Binding Length, StringFormat='长度: {0}'}"/>
        <TextBlock Text="{Binding Success, StringFormat='成功: {0}'}"/>
    </StackPanel>
</DataTemplate>
```

### 5. 打包对象源 DLL

在主项目的 `.csproj` 中：

```xml
<ItemGroup>
  <!-- 包含对象源 DLL -->
  <Content Include="..\..\..\bin\ObjectSource\$(Configuration)\netstandard2.0\RegexMatchObjectSource.dll"
           Link="netstandard2.0\RegexMatchObjectSource.dll">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </Content>
</ItemGroup>

<ItemGroup>
  <!-- 添加项目引用确保构建顺序，但不引用程序集 -->
  <ProjectReference Include="..\ObjectSource\RegexMatchObjectSource.csproj"
                    ReferenceOutputAssembly="false" />
</ItemGroup>
```

## 使用可视化器

1. **设置断点**
2. **运行调试**
3. **查看变量**：将鼠标悬停在 Match 类型的变量上
4. **点击放大镜图标**：选择你的可视化器
5. **查看自定义界面**

## 常见问题

### Q: 为什么对象源必须是 netstandard2.0？
A: 因为它需要加载到被调试进程中，该进程可能运行在 .NET Framework 或 .NET Core 上。

### Q: 如何处理不可序列化的类型？
A: 在对象源中提取可序列化的数据，避免直接传输不可序列化的对象。

### Q: 可视化器会影响调试性能吗？
A: 只有在用户打开可视化器时才会执行，不会影响正常调试。

## 参考资料

- llm.md 相关代码片段：
  - 第 307-320 行：使用反射访问属性
  - 第 1157-1172 行：简化实现
  - 第 1476-1489 行：定义提供者
  - 第 1673-1684 行：创建用户控件
  - 第 1728-1743 行：打包 DLL
  - 第 1986-1997 行：按类型引用对象源

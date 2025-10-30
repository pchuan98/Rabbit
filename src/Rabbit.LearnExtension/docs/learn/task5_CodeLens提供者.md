# 任务 5：实现 CodeLens 提供者

## 学习目标

掌握如何创建 CodeLens 提供者，在代码编辑器中显示内联信息和交互式元素。

## 核心概念

### 1. CodeLens 是什么？

CodeLens 是在代码元素（类、方法等）上方显示的内联信息，可以显示引用计数、单元测试状态等信息。

### 2. 理解 ICodeLensProvider 接口

**要实现什么**：实现 `ICodeLensProvider` 接口，让你的类能够为代码元素（方法、类等）提供 CodeLens

**结果**：CodeLens 是代码上方显示的信息提示（如引用计数）

### 3. 配置 CodeLensProviderConfiguration

**要实现什么**：设置 CodeLens 提供者的显示名称和优先级

**结果**：优先级决定了多个 CodeLens 的显示顺序

### 4. 实现 TryCreateCodeLensAsync 方法

**要实现什么**：判断是否为某个代码元素创建 CodeLens

**结果**：接收 `CodeElement` 参数（代表方法、类等），返回 `CodeLens` 实例或 null

### 5. 创建可点击的 CodeLens

**要实现什么**：继承 `ClickableCodeLens` 类，实现 `ExecuteAsync` 方法处理点击事件

**结果**：用户点击 CodeLens 时会触发你的逻辑

### 6. 实现 GetLabelAsync 提供标签文本

**要实现什么**：返回 `CodeLensLabel` 对象，设置显示的文本和工具提示

**结果**：这是 CodeLens 显示在代码上方的文字内容

### 7. 实现 GetVisualizationAsync 显示自定义 UI

**要实现什么**：返回自定义的 `RemoteUserControl`，点击 CodeLens 后会显示这个 UI

**结果**：可以展示复杂的信息面板

### 8. 处理 CodeLens 刷新和失效

**要实现什么**：调用 `Invalidate()` 方法触发 CodeLens 重新计算和刷新

**结果**：当数据变化时使用

## 知识点详解

### 1. 创建 CodeLens 提供者

```csharp
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Editor;

[VisualStudioContribution]
internal class MyCodeLensProvider : ExtensionPart, ICodeLensProvider
{
    /// <summary>
    /// 配置 CodeLens 提供者
    /// </summary>
    public CodeLensProviderConfiguration CodeLensProviderConfiguration =>
        new("我的 CodeLens 提供者")
        {
            Priority = 500,
        };

    /// <summary>
    /// 配置文本视图扩展（指定适用的文档类型）
    /// </summary>
    public TextViewExtensionConfiguration TextViewExtensionConfiguration => new()
    {
        AppliesTo = new[]
        {
            DocumentFilter.FromDocumentType(DocumentType.KnownValues.Code),
        },
    };

    /// <summary>
    /// 尝试为代码元素创建 CodeLens
    /// </summary>
    public Task<CodeLens?> TryCreateCodeLensAsync(
        CodeElement codeElement,
        CodeElementContext codeElementContext,
        CancellationToken token)
    {
        // 仅为方法创建 CodeLens
        if (codeElement.Kind == CodeElementKind.KnownValues.Method)
        {
            return Task.FromResult<CodeLens?>(
                new MyCodeLens(codeElement, this.Extensibility));
        }

        return Task.FromResult<CodeLens?>(null);
    }
}
```

**关键配置：**
- **CodeLensProviderConfiguration** - 设置显示名称和优先级
- **TextViewExtensionConfiguration** - 指定适用的文档类型
- **TryCreateCodeLensAsync** - 决定是否为特定代码元素创建 CodeLens

### 2. Priority（优先级）

优先级决定了当多个 CodeLens 提供者都适用时的显示顺序。数值越大，优先级越高，越靠前显示。

```csharp
Priority = 500  // 中等优先级
```

### 3. 代码元素类型 (CodeElementKind)

常用的代码元素类型：
- `Method` - 方法
- `Type` - 类型（类、接口等）
- `Property` - 属性
- `Field` - 字段

检查代码元素类型：
```csharp
if (codeElement.Kind == CodeElementKind.KnownValues.Method)
{
    // 这是一个方法
}

// 检查是否是类型（包括类、接口、结构等）
if (codeElement.Kind.IsOfKind(CodeElementKind.KnownValues.Type))
{
    // 这是一个类型
}
```

### 4. 创建可点击的 CodeLens

```csharp
internal class ClickableCodeLens : CodeLens
{
    private int clickCount = 0;

    public ClickableCodeLens(CodeElement codeElement, VisualStudioExtensibility extensibility)
        : base(codeElement, extensibility)
    {
    }

    /// <summary>
    /// 提供 CodeLens 标签（显示的文本）
    /// </summary>
    public override Task<CodeLensLabel> GetLabelAsync(
        CodeElementContext codeElementContext,
        CancellationToken token)
    {
        return Task.FromResult(new CodeLensLabel()
        {
            Text = this.clickCount == 0 ? "点击我" : $"已点击 {this.clickCount} 次",
            Tooltip = "可点击的 CodeLens 示例",
        });
    }

    /// <summary>
    /// 处理 CodeLens 被点击
    /// </summary>
    public override Task ExecuteAsync(
        CodeElementContext codeElementContext,
        IClientContext clientContext,
        CancellationToken cancelToken)
    {
        this.clickCount++;
        this.Invalidate(); // 刷新 CodeLens 显示
        return Task.CompletedTask;
    }
}
```

**关键方法：**
- **GetLabelAsync** - 返回要显示的标签
- **ExecuteAsync** - 处理点击事件
- **Invalidate** - 触发 CodeLens 刷新

### 5. 创建带可视化界面的 CodeLens

```csharp
internal class WordCountCodeLens : CodeLens
{
    private readonly WordCountData wordCountData = new();
    private readonly CodeElementContext codeElementContext;

    public WordCountCodeLens(
        CodeElement codeElement,
        CodeElementContext codeElementContext,
        VisualStudioExtensibility extensibility,
        ICodeLensProvider provider)
        : base(codeElement, extensibility)
    {
        this.codeElementContext = codeElementContext;
    }

    /// <summary>
    /// 提供标签
    /// </summary>
    public override Task<CodeLensLabel> GetLabelAsync(
        CodeElementContext codeElementContext,
        CancellationToken token)
    {
        this.wordCountData.WordCount = CountWords(codeElementContext.Range);

        return Task.FromResult(new CodeLensLabel()
        {
            Text = $"单词数: {this.wordCountData.WordCount}",
            Tooltip = "此代码元素中的单词数量",
        });
    }

    /// <summary>
    /// 提供可视化界面（点击后显示）
    /// </summary>
    public override Task<IRemoteUserControl> GetVisualizationAsync(
        CodeElementContext codeElementContext,
        IClientContext clientContext,
        CancellationToken token)
    {
        return Task.FromResult<IRemoteUserControl>(
            new WordCountCodeLensVisual(this.wordCountData));
    }

    private int CountWords(TextRange range)
    {
        string text = range.CopyToString();
        return text.Split(new[] { ' ', '\t', '\n', '\r' },
            StringSplitOptions.RemoveEmptyEntries).Length;
    }
}
```

### 6. CodeLens 可视化界面

可视化界面是一个 `RemoteUserControl`：

```csharp
internal class WordCountCodeLensVisual : RemoteUserControl
{
    public WordCountCodeLensVisual(WordCountData dataContext)
        : base(dataContext)
    {
    }
}
```

XAML 数据模板：
```xml
<DataTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
              xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <StackPanel Margin="10">
        <TextBlock Text="{Binding WordCount, StringFormat='总单词数: {0}'}"
                   FontSize="14"
                   FontWeight="Bold"/>
        <TextBlock Text="此代码元素中包含的单词数量统计"
                   Margin="0,5,0,0"
                   FontSize="12"/>
    </StackPanel>
</DataTemplate>
```

## 完整示例：引用计数 CodeLens

```csharp
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Editor;
using System.Threading;
using System.Threading.Tasks;

namespace MyExtension;

[VisualStudioContribution]
internal class ReferenceCountCodeLensProvider : ExtensionPart, ICodeLensProvider
{
    public CodeLensProviderConfiguration CodeLensProviderConfiguration =>
        new("引用计数 CodeLens")
        {
            Priority = 500,
        };

    public TextViewExtensionConfiguration TextViewExtensionConfiguration => new()
    {
        AppliesTo = new[]
        {
            DocumentFilter.FromDocumentType(DocumentType.KnownValues.Code),
        },
    };

    public Task<CodeLens?> TryCreateCodeLensAsync(
        CodeElement codeElement,
        CodeElementContext codeElementContext,
        CancellationToken token)
    {
        // 为方法和类型创建 CodeLens
        if (codeElement.Kind == CodeElementKind.KnownValues.Method ||
            codeElement.Kind.IsOfKind(CodeElementKind.KnownValues.Type))
        {
            return Task.FromResult<CodeLens?>(
                new ReferenceCountCodeLens(codeElement, this.Extensibility));
        }

        return Task.FromResult<CodeLens?>(null);
    }
}

internal class ReferenceCountCodeLens : CodeLens
{
    private int referenceCount = 0;
    private bool isCalculated = false;

    public ReferenceCountCodeLens(
        CodeElement codeElement,
        VisualStudioExtensibility extensibility)
        : base(codeElement, extensibility)
    {
    }

    public override async Task<CodeLensLabel> GetLabelAsync(
        CodeElementContext codeElementContext,
        CancellationToken token)
    {
        if (!isCalculated)
        {
            // 模拟计算引用（实际应该调用语言服务）
            referenceCount = new Random().Next(0, 100);
            isCalculated = true;
        }

        string text = referenceCount == 0
            ? "无引用"
            : referenceCount == 1
                ? "1 个引用"
                : $"{referenceCount} 个引用";

        return new CodeLensLabel()
        {
            Text = text,
            Tooltip = "点击查看引用详情",
        };
    }

    public override async Task ExecuteAsync(
        CodeElementContext codeElementContext,
        IClientContext clientContext,
        CancellationToken cancelToken)
    {
        // 点击后显示引用详情（简化示例）
        await this.Extensibility.Shell().ShowPromptAsync(
            $"找到 {referenceCount} 个引用",
            PromptOptions.OK,
            cancelToken);
    }
}
```

## 实践步骤

1. **创建提供者类**
   - 实现 `ICodeLensProvider` 接口
   - 添加 `[VisualStudioContribution]` 特性
   - 配置提供者和文本视图扩展

2. **实现 TryCreateCodeLensAsync**
   - 检查代码元素类型
   - 为合适的元素创建 CodeLens 实例
   - 返回 null 表示不创建

3. **创建 CodeLens 类**
   - 继承 `CodeLens` 基类
   - 实现 `GetLabelAsync` 提供显示文本
   - 可选实现 `ExecuteAsync` 处理点击
   - 可选实现 `GetVisualizationAsync` 提供详细界面

4. **测试 CodeLens**
   - 打开支持的代码文件
   - 查看方法或类上方的 CodeLens
   - 测试点击交互

## CodeElement 信息

可以从 `CodeElement` 获取的信息：
```csharp
string name = codeElement.Name;              // 元素名称
CodeElementKind kind = codeElement.Kind;     // 元素类型
```

## CodeElementContext 信息

可以从 `CodeElementContext` 获取的信息：
```csharp
TextRange range = codeElementContext.Range;  // 代码范围
string text = range.CopyToString();          // 代码文本
```

## 刷新 CodeLens

当数据变化需要更新显示时：
```csharp
this.Invalidate();  // 触发 GetLabelAsync 重新调用
```

## 常见问题

### Q: CodeLens 不显示？
A: 检查：
- 文档类型是否匹配 `TextViewExtensionConfiguration`
- `TryCreateCodeLensAsync` 是否返回了有效实例
- Visual Studio 的 CodeLens 功能是否启用

### Q: 如何异步加载数据？
A: 在 `GetLabelAsync` 中可以执行异步操作，但要注意性能影响。

### Q: 可以为同一个元素创建多个 CodeLens 吗？
A: 可以，不同的提供者可以为同一元素创建多个 CodeLens。

### Q: 如何访问语言服务（如查找引用）？
A: 通过 `Extensibility` 对象访问相应的服务，但这需要额外的 API 支持。

## 性能注意事项

- `GetLabelAsync` 会频繁调用，应该尽快返回
- 耗时操作应该异步执行并缓存结果
- 使用 `Invalidate` 时要谨慎，避免频繁刷新

## 进阶主题

- 与语言服务集成
- 缓存和性能优化
- 多文档类型支持
- 动态更新 CodeLens

## 参考资料

- 源代码示例：`CodeLensSample`
- llm.md 相关代码片段：
  - 第 952-968 行：创建可点击 CodeLens
  - 第 988-1000 行：配置 CodeLens 提供者
  - 第 1107-1119 行：配置显示和优先级
  - 第 1123-1139 行：创建 WordCountCodeLens
  - 第 1284-1306 行：实现点击行为
  - 第 1492-1513 行：提供标签和可视化

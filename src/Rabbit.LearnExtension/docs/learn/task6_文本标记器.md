# 任务 6：实现文本标记器（Tagger）

## 学习目标

掌握如何创建文本标记器，为编辑器中的特定文本添加视觉效果（如高亮、下划线等）。

## 核心概念

### 1. Tagger 是什么？

Tagger（标记器）用于标识文档中的特定文本范围，并为其应用视觉样式，如：
- 语法高亮
- 错误波浪线
- 查找结果高亮
- 自定义标记

### 2. 创建 ITextViewTaggerProvider<T> 实现

**要实现什么**：实现 `ITextViewTaggerProvider<T>` 接口，为文本视图提供标记器

**结果**：T 是标记类型（如 `TextMarkerTag` 用于高亮）

### 3. 配置 TextViewExtensionConfiguration 指定适用的文档类型

**要实现什么**：通过 `AppliesTo` 属性设置 Tagger 应用的文件类型

**结果**：使用 `DocumentFilter` 指定文件扩展名或文档类型

### 4. 实现 CreateTaggerAsync 方法

**要实现什么**：创建并返回你的 Tagger 实例

**结果**：每个打开的文本视图会调用一次这个方法

### 5. 创建 TextViewTagger<T> 子类

**要实现什么**：继承 `TextViewTagger<T>`，实现具体的标记逻辑

**结果**：这是标记器的核心类

### 6. 实现 CreateTagsAsync 处理文档范围

**要实现什么**：为指定的文档范围创建标记

**结果**：接收 `ITextDocumentSnapshot` 和要处理的文本范围列表，返回带位置信息的标记列表

### 7. 使用 TextMarkerTag 高亮文本

**要实现什么**：创建 `TextMarkerTag` 实例指定高亮样式（如 FindHighlight）

**结果**：通过 `TaggedTrackingTextRange` 关联标记和文本位置

### 8. 处理文本视图变化事件

**要实现什么**：实现 `TextViewChangedAsync` 方法响应文本编辑

**结果**：获取编辑范围，更新受影响区域的标记

## 知识点详解

### 1. 创建标记器提供者

```csharp
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Editor;

[VisualStudioContribution]
internal class MarkdownTextMarkerTaggerProvider : ExtensionPart, ITextViewTaggerProvider<TextMarkerTag>
{
    /// <summary>
    /// 配置文本视图扩展（指定适用的文档类型）
    /// </summary>
    public TextViewExtensionConfiguration TextViewExtensionConfiguration => new()
    {
        AppliesTo = [DocumentFilter.FromDocumentType("vs-markdown")],
    };

    /// <summary>
    /// 创建标记器实例
    /// </summary>
    public Task<TextViewTagger<TextMarkerTag>> CreateTaggerAsync(
        ITextViewSnapshot textView,
        CancellationToken cancellationToken)
    {
        var tagger = new MarkdownTextMarkerTagger(this, textView.Document.Uri);
        return Task.FromResult<TextViewTagger<TextMarkerTag>>(tagger);
    }
}
```

**关键点：**
- 泛型参数 `<TextMarkerTag>` 指定标签类型
- `TextViewExtensionConfiguration` 指定适用的文档类型
- `CreateTaggerAsync` 为每个匹配的文本视图创建标记器实例

### 2. 文档类型过滤器

#### 按文档类型过滤

```csharp
AppliesTo = [DocumentFilter.FromDocumentType("vs-markdown")]
```

常用文档类型：
- `"vs-markdown"` - Markdown 文件
- `"csharp"` - C# 文件
- `"xml"` - XML 文件

#### 按文件模式过滤

```csharp
AppliesTo = [DocumentFilter.FromGlobPattern("**/*.md", true)]
```

### 3. 创建标记器类

```csharp
internal class MarkdownTextMarkerTagger : TextViewTagger<TextMarkerTag>
{
    public MarkdownTextMarkerTagger(ExtensionPart extension, Uri documentUri)
        : base(extension, documentUri)
    {
    }

    /// <summary>
    /// 文本视图打开时调用
    /// </summary>
    public override Task TextViewOpenedAsync(
        ITextViewSnapshot textView,
        CancellationToken cancellationToken)
    {
        // 初始化时为整个文档创建标记
        var wholeDocument = new TextRange(textView.Document, 0, textView.Document.Length);
        return this.CreateTagsAsync(textView.Document, new[] { wholeDocument });
    }

    /// <summary>
    /// 文本视图变化时调用
    /// </summary>
    public async Task TextViewChangedAsync(
        ITextViewSnapshot textView,
        IReadOnlyList<TextEdit> edits,
        CancellationToken cancellationToken)
    {
        // 获取所有之前请求的范围
        var allRequestedRanges = await this.GetAllRequestedRangesAsync(
            textView.Document,
            cancellationToken);

        // 只为编辑影响的范围创建标记
        await this.CreateTagsAsync(
            textView.Document,
            allRequestedRanges.Intersect(
                edits.Select(e =>
                    EnsureNotEmpty(
                        e.Range.TranslateTo(
                            textView.Document,
                            TextRangeTrackingMode.ExtendForwardAndBackward)))));
    }

    private static TextRange EnsureNotEmpty(TextRange range)
    {
        return range.Length > 0
            ? range
            : new TextRange(range.Document, range.Start, 1);
    }
}
```

**关键生命周期方法：**
- **TextViewOpenedAsync** - 文本视图首次打开时调用
- **TextViewChangedAsync** - 文本内容变化时调用
- **TextViewClosedAsync** - 文本视图关闭时调用

### 4. 创建标记

```csharp
private async Task CreateTagsAsync(
    ITextDocumentSnapshot document,
    IEnumerable<TextRange> requestedRanges)
{
    List<TaggedTrackingTextRange<TextMarkerTag>> tags = new();
    List<TextRange> ranges = new();

    foreach (var lineNumber in requestedRanges.SelectMany(r =>
    {
        // 将请求的范围转换为行号
        var startLine = r.Document.GetLineNumberFromPosition(r.Start);
        var endLine = r.Document.GetLineNumberFromPosition(r.End);
        return Enumerable.Range(startLine, endLine - startLine + 1);
    }).Distinct()) // 使用 Distinct 避免处理同一行多次
    {
        var line = document.Lines[lineNumber];

        // 检查行是否以 '#' 开头（Markdown 标题）
        if (line.Text.StartsWith("#"))
        {
            int len = line.Text.Length;
            if (len > 0)
            {
                // 创建标记（使用 FindHighlight 标记类型）
                tags.Add(new(
                    new(document, line.Text.Start, len, TextRangeTrackingMode.ExtendForwardAndBackward),
                    new("MarkerFormatDefinition/FindHighlight")));
            }
        }

        // 添加已处理的范围
        ranges.Add(new(document, line.TextIncludingLineBreak.Start, line.TextIncludingLineBreak.Length));
    }

    // 更新标记
    await this.UpdateTagsAsync(ranges, tags, CancellationToken.None);
}
```

**重要概念：**

1. **TaggedTrackingTextRange<T>** - 带标签的跟踪文本范围
   - 第一个参数：文本范围（包含文档、位置、长度、跟踪模式）
   - 第二个参数：标签实例

2. **TextRangeTrackingMode** - 文本范围跟踪模式
   - `ExtendForwardAndBackward` - 向前向后扩展
   - `ExtendForward` - 仅向前扩展
   - `ExtendBackward` - 仅向后扩展

3. **UpdateTagsAsync** - 更新标记
   - 第一个参数：已处理的范围列表
   - 第二个参数：创建的标记列表
   - 即使范围没有标记也要添加到列表（清除旧标记）

### 5. 内置标记类型

```csharp
// 查找高亮
new TextMarkerTag("MarkerFormatDefinition/FindHighlight")

// 错误标记
new TextMarkerTag("MarkerFormatDefinition/ErrorSquiggle")

// 警告标记
new TextMarkerTag("MarkerFormatDefinition/WarningSquiggle")
```

## 完整示例：Markdown 标题高亮

```csharp
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyExtension;

/// <summary>
/// 标记器提供者：为 Markdown 文件提供标题高亮
/// </summary>
[VisualStudioContribution]
internal class MarkdownHeadingTaggerProvider : ExtensionPart, ITextViewTaggerProvider<TextMarkerTag>
{
    public TextViewExtensionConfiguration TextViewExtensionConfiguration => new()
    {
        AppliesTo =
        [
            DocumentFilter.FromGlobPattern("**/*.md", true),
        ],
    };

    public Task<TextViewTagger<TextMarkerTag>> CreateTaggerAsync(
        ITextViewSnapshot textView,
        CancellationToken cancellationToken)
    {
        var tagger = new MarkdownHeadingTagger(this, textView.Document.Uri);
        return Task.FromResult<TextViewTagger<TextMarkerTag>>(tagger);
    }
}

/// <summary>
/// 标记器：高亮 Markdown 标题行（以 # 开头的行）
/// </summary>
internal class MarkdownHeadingTagger : TextViewTagger<TextMarkerTag>
{
    public MarkdownHeadingTagger(ExtensionPart extension, Uri documentUri)
        : base(extension, documentUri)
    {
    }

    public override Task TextViewOpenedAsync(
        ITextViewSnapshot textView,
        CancellationToken cancellationToken)
    {
        // 文本视图打开时，为整个文档创建标记
        var wholeDocument = new TextRange(textView.Document, 0, textView.Document.Length);
        return this.CreateTagsAsync(textView.Document, new[] { wholeDocument });
    }

    public async Task TextViewChangedAsync(
        ITextViewSnapshot textView,
        IReadOnlyList<TextEdit> edits,
        CancellationToken cancellationToken)
    {
        // 文本变化时，只为编辑影响的范围更新标记
        var allRequestedRanges = await this.GetAllRequestedRangesAsync(
            textView.Document,
            cancellationToken);

        await this.CreateTagsAsync(
            textView.Document,
            allRequestedRanges.Intersect(
                edits.Select(e =>
                    EnsureNotEmpty(
                        e.Range.TranslateTo(
                            textView.Document,
                            TextRangeTrackingMode.ExtendForwardAndBackward)))));
    }

    private async Task CreateTagsAsync(
        ITextDocumentSnapshot document,
        IEnumerable<TextRange> requestedRanges)
    {
        List<TaggedTrackingTextRange<TextMarkerTag>> tags = new();
        List<TextRange> ranges = new();

        foreach (var lineNumber in requestedRanges.SelectMany(r =>
        {
            var startLine = r.Document.GetLineNumberFromPosition(r.Start);
            var endLine = r.Document.GetLineNumberFromPosition(r.End);
            return Enumerable.Range(startLine, endLine - startLine + 1);
        }).Distinct())
        {
            var line = document.Lines[lineNumber];

            // 检查是否是标题行
            if (line.Text.StartsWith("#"))
            {
                int len = line.Text.Length;
                if (len > 0)
                {
                    // 创建高亮标记
                    tags.Add(new(
                        new(document, line.Text.Start, len, TextRangeTrackingMode.ExtendForwardAndBackward),
                        new("MarkerFormatDefinition/FindHighlight")));
                }
            }

            // 添加已处理的范围（包括换行符）
            ranges.Add(new(
                document,
                line.TextIncludingLineBreak.Start,
                line.TextIncludingLineBreak.Length));
        }

        // 更新标记
        await this.UpdateTagsAsync(ranges, tags, CancellationToken.None);
    }

    private static TextRange EnsureNotEmpty(TextRange range)
    {
        return range.Length > 0
            ? range
            : new TextRange(range.Document, range.Start, 1);
    }
}
```

## 实践步骤

1. **创建提供者类**
   - 实现 `ITextViewTaggerProvider<TextMarkerTag>`
   - 添加 `[VisualStudioContribution]` 特性
   - 配置文本视图扩展

2. **创建标记器类**
   - 继承 `TextViewTagger<TextMarkerTag>`
   - 实现 `TextViewOpenedAsync`
   - 实现 `TextViewChangedAsync`

3. **实现标记逻辑**
   - 解析请求的文本范围
   - 识别需要标记的文本
   - 创建标记实例
   - 调用 `UpdateTagsAsync`

4. **测试标记器**
   - 创建或打开匹配的文档
   - 验证标记是否正确显示
   - 测试编辑时标记是否正确更新

## 文档行操作

### 获取行信息

```csharp
var line = document.Lines[lineNumber];
string lineText = line.Text;                          // 行文本（不含换行符）
TextRange lineRange = line.TextIncludingLineBreak;   // 行范围（含换行符）
int startPosition = line.Text.Start;                 // 行起始位置
int length = line.Text.Length;                       // 行长度
```

### 从位置获取行号

```csharp
int lineNumber = document.GetLineNumberFromPosition(position);
```

## 范围操作

### 创建文本范围

```csharp
// 创建范围：起始位置 + 长度
var range = new TextRange(document, start, length);

// 创建跟踪范围：范围 + 跟踪模式
var trackingRange = new TextRange(
    document,
    start,
    length,
    TextRangeTrackingMode.ExtendForwardAndBackward);
```

### 范围转换

```csharp
// 将范围转换到新文档快照
var newRange = oldRange.TranslateTo(
    newDocument,
    TextRangeTrackingMode.ExtendForwardAndBackward);
```

## 性能优化

### 1. 使用 Distinct 避免重复处理

```csharp
foreach (var lineNumber in lineNumbers.Distinct())
{
    // 处理行
}
```

### 2. 只处理受影响的范围

```csharp
// 获取编辑影响的范围
var affectedRanges = allRequestedRanges.Intersect(editedRanges);
```

### 3. 缓存计算结果

对于复杂的解析，可以缓存结果避免重复计算。

## 常见问题

### Q: 标记没有显示？
A: 检查：
- 文档类型是否匹配 `TextViewExtensionConfiguration`
- 是否调用了 `UpdateTagsAsync`
- 标记类型字符串是否正确

### Q: 如何创建自定义标记样式？
A: 目前 VisualStudio.Extensibility 不支持定义新的标记类型，需要使用内置标记类型。

### Q: 编辑时标记消失？
A: 确保：
- 实现了 `TextViewChangedAsync`
- 使用了正确的 `TextRangeTrackingMode`
- 调用了 `UpdateTagsAsync` 更新标记

### Q: 如何处理多行标记？
A: 创建跨越多行的 `TextRange`：
```csharp
var multiLineRange = new TextRange(document, startPosition, endPosition - startPosition);
```

## 进阶主题

- 多种标记类型混合使用
- 性能优化和增量更新
- 与语言服务集成
- 自定义标记格式定义

## 参考资料

- 源代码示例：`TaggersSample`
- llm.md 相关代码片段：
  - 第 211-254 行：创建 TextMarker 标记
  - 第 1259-1280 行：定义标记器提供者
  - 第 1965-1982 行：处理文本视图变化

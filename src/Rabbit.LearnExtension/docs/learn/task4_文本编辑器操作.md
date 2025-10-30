# 任务 4：文本编辑器操作

## 学习目标

掌握如何在 Visual Studio 扩展中操作文本编辑器，包括读取和修改文档内容。

## 核心概念

### 1. 获取活动文本视图

**要实现什么**：通过 `context.GetActiveTextViewAsync()` 获取当前编辑器

**结果**：返回的 `textView` 对象包含了文档内容、光标位置、选择区域等所有信息

```csharp
using var textView = await context.GetActiveTextViewAsync(cancellationToken);
```

### 2. 读取文档内容和选择区域

**要实现什么**：从 `textView.Document` 读取文本内容，从 `textView.Selection` 获取用户选中的区域

**结果**：可以得到选中文本的起止位置和内容

```csharp
var document = await textView.GetTextDocumentAsync(cancellationToken);
var selections = textView.Selections;
```

### 3. 使用 Editor().EditAsync 编辑文档

**要实现什么**：所有文档修改必须在 `Extensibility.Editor().EditAsync()` 的批处理（batch）中进行

**结果**：这样可以保证编辑操作的原子性和撤销/重做功能正常

```csharp
await this.Extensibility.Editor().EditAsync(
    batch =>
    {
        var editor = textView.Document.AsEditable(batch);
        // 执行编辑操作
    },
    cancellationToken);
```

### 4. 处理多个选择区域

**要实现什么**：VS 支持多光标编辑，`textView.Selections` 是一个列表

**结果**：需要遍历所有选择区域分别处理

### 5. 撤销/重做兼容

**要实现什么**：在 `EditAsync` 批处理中的所有修改会被自动包装为一个撤销单元

**结果**：用户按 Ctrl+Z 可以一次性撤销整个批处理的操作

## 知识点详解

### 1. 获取文本视图和文档

```csharp
public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
{
    // 获取活动文本视图
    using var textView = await context.GetActiveTextViewAsync(cancellationToken);

    if (textView == null)
    {
        await this.Extensibility.Shell().ShowPromptAsync(
            "没有打开的文本编辑器",
            PromptOptions.OK,
            cancellationToken);
        return;
    }

    // 获取文档快照
    var document = await textView.GetTextDocumentAsync(cancellationToken);
}
```

### 2. 读取文档内容

#### 获取选择的文本

```csharp
var document = await textView.GetTextDocumentAsync(cancellationToken);

// 获取所有选择区域
var selections = textView.Selections;

foreach (var selection in selections)
{
    if (selection.IsEmpty)
    {
        continue; // 跳过空选择
    }

    // 获取选择的文本
    string selectedText = selection.Extent.CopyToString();
}
```

#### 获取当前行

```csharp
// 获取当前行号
var lineNumber = document.GetLineNumberFromPosition(textView.Selection.Extent.Start);

// 获取行内容
var line = document.Lines[lineNumber];
string lineText = line.Text;
```

### 3. 编辑文档

所有编辑操作都必须在 `EditAsync` 的批处理中进行。

```csharp
await this.Extensibility.Editor().EditAsync(
    batch =>
    {
        var editor = textView.Document.AsEditable(batch);

        // 在这里执行编辑操作
        // editor.Replace(...)
        // editor.Insert(...)
        // editor.Delete(...)
    },
    cancellationToken);
```

#### 替换文本

```csharp
await this.Extensibility.Editor().EditAsync(
    batch =>
    {
        var editor = document.AsEditable(batch);

        // 替换选择的文本
        editor.Replace(textView.Selection.Extent, "新文本");
    },
    cancellationToken);
```

#### 插入文本

```csharp
await this.Extensibility.Editor().EditAsync(
    batch =>
    {
        var editor = document.AsEditable(batch);

        // 在光标位置插入
        var position = textView.Selection.Extent.Start;
        editor.Insert(position, "插入的文本");
    },
    cancellationToken);
```

#### 删除文本

```csharp
await this.Extensibility.Editor().EditAsync(
    batch =>
    {
        var editor = document.AsEditable(batch);

        // 删除选择的文本
        editor.Delete(textView.Selection.Extent);
    },
    cancellationToken);
```

### 4. 处理多个选择区域

Visual Studio 支持多光标编辑，需要处理多个选择区域。

```csharp
await this.Extensibility.Editor().EditAsync(
    batch =>
    {
        var editor = textView.Document.AsEditable(batch);
        var selections = textView.Selections;

        for (int i = 0; i < selections.Count; i++)
        {
            var selection = selections[i];
            if (selection.IsEmpty)
            {
                continue;
            }

            string originalText = selection.Extent.CopyToString();
            string newText = ProcessText(originalText);
            editor.Replace(selection.Extent, newText);
        }
    },
    cancellationToken);
```

## 完整示例：插入 GUID 命令

```csharp
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyExtension;

[VisualStudioContribution]
internal class InsertGuidCommand : Command
{
    public override CommandConfiguration CommandConfiguration => new("%InsertGuidCommand.DisplayName%")
    {
        Placements = [CommandPlacement.KnownPlacements.ExtensionsMenu],
        Icon = new(ImageMoniker.KnownValues.OfficeWebExtension, IconSettings.IconAndText),
        VisibleWhen = ActivationConstraint.ClientContext(
            ClientContextKey.Shell.ActiveEditorContentType, ".+"),
    };

    public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
    {
        // 获取活动文本视图
        using var textView = await context.GetActiveTextViewAsync(cancellationToken);

        if (textView == null)
        {
            return;
        }

        // 生成新的 GUID
        string newGuid = Guid.NewGuid().ToString();

        // 获取文档
        var document = await textView.GetTextDocumentAsync(cancellationToken);

        // 执行编辑
        await this.Extensibility.Editor().EditAsync(
            batch =>
            {
                var editor = document.AsEditable(batch);

                // 替换当前选择（如果有）或在光标位置插入
                editor.Replace(textView.Selection.Extent, newGuid);
            },
            cancellationToken);
    }
}
```

## 完整示例：Base64 编码/解码命令

```csharp
[VisualStudioContribution]
internal class EncodeDecodeBase64Command : Command
{
    public override CommandConfiguration CommandConfiguration => new("%EncodeDecodeBase64.DisplayName%")
    {
        Placements = [CommandPlacement.KnownPlacements.ExtensionsMenu],
        Icon = new(ImageMoniker.KnownValues.ConvertPartition, IconSettings.IconAndText),
        VisibleWhen = ActivationConstraint.SolutionState(SolutionState.FullyLoaded),
        EnabledWhen = ActivationConstraint.ClientContext(
            ClientContextKey.Shell.ActiveEditorContentType, "csharp"),
    };

    public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
    {
        using var textView = await context.GetActiveTextViewAsync(cancellationToken);

        if (textView == null)
        {
            return;
        }

        var document = await textView.GetTextDocumentAsync(cancellationToken);

        await this.Extensibility.Editor().EditAsync(
            batch =>
            {
                var editor = textView.Document.AsEditable(batch);
                var selections = textView.Selections;

                for (int i = 0; i < selections.Count; i++)
                {
                    var selection = selections[i];
                    if (selection.IsEmpty)
                    {
                        continue;
                    }

                    string originalText = selection.Extent.CopyToString();
                    string newText = this.EncodeOrDecode(originalText);
                    editor.Replace(selection.Extent, newText);
                }
            },
            cancellationToken);
    }

    private string EncodeOrDecode(string text)
    {
        try
        {
            // 尝试解码（如果是 Base64）
            byte[] data = Convert.FromBase64String(text);
            return System.Text.Encoding.UTF8.GetString(data);
        }
        catch
        {
            // 否则编码
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }
    }
}
```

## 实践步骤

1. **创建编辑器命令**
   - 创建继承自 `Command` 的类
   - 配置 `VisibleWhen` 确保有活动编辑器时才显示

2. **获取文本视图**
   - 使用 `context.GetActiveTextViewAsync` 获取活动视图
   - 检查是否为 null

3. **读取文档内容**
   - 使用 `GetTextDocumentAsync` 获取文档
   - 访问 `textView.Selections` 获取选择

4. **执行编辑操作**
   - 使用 `Editor().EditAsync` 创建编辑批处理
   - 在批处理中调用 `AsEditable` 获取编辑器
   - 执行 Replace、Insert 或 Delete 操作

5. **测试命令**
   - 打开一个文本文件
   - 选择一些文本
   - 运行命令验证编辑效果

## 重要概念

### 文本范围 (TextRange)

表示文档中的一段文本：
```csharp
var range = new TextRange(document, start, length);
```

### 文本扩展范围 (Extent)

选择的实际文本范围：
```csharp
var extent = selection.Extent;
int start = extent.Start;
int end = extent.End;
int length = extent.Length;
```

### 跟踪模式 (TextRangeTrackingMode)

控制编辑时范围如何更新：
- `Default` - 默认行为
- `ExtendForwardAndBackward` - 向前向后扩展
- `ExtendForward` - 向前扩展
- `ExtendBackward` - 向后扩展

## 激活约束

### 仅在编辑器打开时显示命令

```csharp
VisibleWhen = ActivationConstraint.ClientContext(
    ClientContextKey.Shell.ActiveEditorContentType, ".+")
```

### 仅对特定语言启用

```csharp
EnabledWhen = ActivationConstraint.ClientContext(
    ClientContextKey.Shell.ActiveEditorContentType, "csharp")
```

## 常见问题

### Q: 如何获取整个文档的文本？
A: 遍历所有行或创建覆盖整个文档的范围。

### Q: 编辑操作会自动添加到撤销栈吗？
A: 是的，通过 `EditAsync` 执行的操作会自动支持撤销/重做。

### Q: 可以在 EditAsync 外部修改文档吗？
A: 不可以，所有编辑必须在 `EditAsync` 的批处理中进行。

### Q: 如何处理编辑失败？
A: `EditAsync` 会抛出异常，应该使用 try-catch 处理。

## 进阶主题

- 获取编辑器选项（如制表符设置）
- 监听文本变化事件
- 使用文本标记器高亮文本（Task 6）
- 处理异步编辑操作

## 参考资料

- 源代码示例：
  - `InsertGuid`
  - `EncodeDecodeBase64`
- llm.md 相关代码片段：
  - 第 75-83 行：获取活动文本视图
  - 第 101-116 行：准备编辑文档
  - 第 181-195 行：替换文本
  - 第 324-345 行：处理多个选择区域

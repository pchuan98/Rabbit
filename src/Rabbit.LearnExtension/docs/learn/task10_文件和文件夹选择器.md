# 任务 10：文件和文件夹选择器

## 学习目标

掌握如何在 Visual Studio 扩展中使用文件和文件夹选择对话框，让用户选择文件路径。

## 核心概念

### 1. 使用 ShowOpenFileDialogAsync 选择单个文件

**要实现什么**：调用 `Shell().ShowOpenFileDialogAsync()` 显示文件选择对话框

**结果**：返回选中的文件路径字符串，用户取消返回 null

### 2. 使用 ShowOpenMultipleFilesDialogAsync 选择多个文件

**要实现什么**：允许用户选择多个文件

**结果**：返回 `IReadOnlyList<string>` 包含所有选中文件的路径

### 3. 使用 ShowOpenFolderDialogAsync 选择文件夹

**要实现什么**：显示文件夹选择对话框

**结果**：返回选中的文件夹路径，用户可以浏览并选择目录

### 4. 使用 ShowSaveAsFileDialogAsync 保存文件

**要实现什么**：显示保存文件对话框

**结果**：返回用户指定的保存路径，可设置默认文件名和扩展名

### 5. 配置 FileDialogOptions（过滤器、初始文件名等）

**要实现什么**：创建 `FileDialogOptions` 设置 `InitialFileName`、`Filters`（文件类型过滤）等

**结果**：控制对话框的初始状态和行为

### 6. 处理用户取消操作

**要实现什么**：所有对话框方法在用户取消时返回 null

**结果**：需要检查返回值是否为 null 再继续处理

## 知识点详解

### 1. 选择单个文件

```csharp
public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
{
    var options = new FileDialogOptions()
    {
        InitialFileName = "config.json",
        Filters = new DialogFilters(
            new("JSON Files", "*.json"),
            new("All Files", "*.*")
        )
        {
            DefaultFilterIndex = 0,
        },
    };

    string? filePath = await this.Extensibility.Shell()
        .ShowOpenFileDialogAsync(options, cancellationToken);

    if (filePath != null)
    {
        // 用户选择了文件
        await this.Extensibility.Shell().ShowPromptAsync(
            $"选择的文件: {filePath}",
            PromptOptions.OK,
            cancellationToken);
    }
}
```

### 2. 选择多个文件

```csharp
var options = new FileDialogOptions()
{
    Filters = new DialogFilters(
        new("C# Files", "*.cs"),
        new("Text Files", "*.txt", "*.log")
    ),
};

IReadOnlyList<string>? filePaths = await this.Extensibility.Shell()
    .ShowOpenMultipleFilesDialogAsync(options, cancellationToken);

if (filePaths != null)
{
    foreach (var path in filePaths)
    {
        // 处理每个文件
    }
}
```

### 3. 选择文件夹

```csharp
var options = new FolderDialogOptions();

string? folderPath = await this.Extensibility.Shell()
    .ShowOpenFolderDialogAsync(options, cancellationToken);

if (folderPath != null)
{
    // 用户选择了文件夹
}
```

### 4. 保存文件对话框

```csharp
var options = new FileDialogOptions()
{
    Title = "保存导出文件",
    InitialFileName = "export.txt",
};

string? filePath = await this.Extensibility.Shell()
    .ShowSaveAsFileDialogAsync(options, cancellationToken);

if (filePath != null)
{
    // 保存文件到 filePath
    await File.WriteAllTextAsync(filePath, "内容", cancellationToken);
}
```

## 文件类型过滤器

### 单个扩展名

```csharp
new DialogFilter("C# Files", "*.cs")
```

### 多个扩展名

```csharp
new DialogFilter("Code Files", "*.cs", "*.vb", "*.fs")
```

### 设置默认过滤器

```csharp
Filters = new DialogFilters(
    new("JSON Files", "*.json"),
    new("XML Files", "*.xml"),
    new("All Files", "*.*")
)
{
    DefaultFilterIndex = 0,  // 默认选择第一个（JSON Files）
}
```

## 完整示例

```csharp
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MyExtension;

[VisualStudioContribution]
internal class SelectFileCommand : Command
{
    public override CommandConfiguration CommandConfiguration => new("%SelectFile.DisplayName%")
    {
        Placements = [CommandPlacement.KnownPlacements.ToolsMenu],
    };

    public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
    {
        // 1. 选择源文件
        var openOptions = new FileDialogOptions()
        {
            InitialFileName = "source.txt",
            Filters = new DialogFilters(
                new("Text Files", "*.txt"),
                new("All Files", "*.*")
            ),
        };

        string? sourcePath = await this.Extensibility.Shell()
            .ShowOpenFileDialogAsync(openOptions, cancellationToken);

        if (sourcePath == null)
        {
            return;  // 用户取消
        }

        // 2. 选择目标位置
        var saveOptions = new FileDialogOptions()
        {
            Title = "保存到",
            InitialFileName = Path.GetFileName(sourcePath),
        };

        string? destPath = await this.Extensibility.Shell()
            .ShowSaveAsFileDialogAsync(saveOptions, cancellationToken);

        if (destPath == null)
        {
            return;  // 用户取消
        }

        // 3. 复制文件
        File.Copy(sourcePath, destPath, overwrite: true);

        await this.Extensibility.Shell().ShowPromptAsync(
            "文件已成功复制！",
            PromptOptions.OK,
            cancellationToken);
    }
}
```

## 常见问题

### Q: 如何设置对话框的初始目录？
A: 当前 API 不直接支持设置初始目录，但可以通过 `InitialFileName` 包含路径来实现。

### Q: 用户取消时会返回什么？
A: 返回 `null`，必须检查返回值。

### Q: 如何验证用户选择的文件是否存在？
A: 使用 `File.Exists(path)` 检查文件是否存在。

## 参考资料

- llm.md 相关代码片段：
  - 第 590-606 行：选择多个文件
  - 第 638-658 行：选择单个文件
  - 第 763-772 行：选择文件夹
  - 第 792-806 行：保存文件

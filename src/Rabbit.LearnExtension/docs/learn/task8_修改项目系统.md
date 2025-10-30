# 任务 8：修改项目系统

## 学习目标

掌握如何使用项目查询 API 修改项目系统，包括添加/删除文件、重命名项目、配置解决方案等。

## 核心概念

### 1. 使用 UpdateProjectsAsync 修改项目

**要实现什么**：调用 `UpdateProjectsAsync` 传入选择器和修改操作

**结果**：第一个参数选择要修改的项目，第二个参数定义修改动作

```csharp
await querySpace.UpdateProjectsAsync(
    project => /* 选择器 */,
    project => /* 操作 */,
    cancellationToken);
```

### 2. 创建新文件到项目

**要实现什么**：使用 `project.AddFile("filename.txt")` 在项目中创建新文件

**结果**：文件会自动添加到项目并保存

### 3. 从项目中删除文件

**要实现什么**：使用 `project.DeleteFile("filepath")` 从项目中删除文件

**结果**：会同时从磁盘和项目文件中移除

### 4. 重命名项目

**要实现什么**：使用 `AsUpdatable().Rename("NewName")` 重命名项目

**结果**：项目文件和引用会自动更新

### 5. 添加/删除解决方案配置

**要实现什么**：使用 `UpdateSolutionAsync` 调用 `AddSolutionConfiguration` 或 `DeleteSolutionConfiguration`

**结果**：管理 Debug/Release 等配置

### 6. 设置启动项目

**要实现什么**：使用 `solution.SetStartupProjects(projectPath)` 设置解决方案的启动项目

**结果**：可以设置多个启动项目

### 7. 构建项目和解决方案

**要实现什么**：调用 `project.BuildAsync()` 或 `solution.BuildAsync()` 触发构建

**结果**：等待构建完成并获取结果

## 知识点详解

### 1. 添加文件到项目

```csharp
await querySpace.UpdateProjectsAsync(
    project => project.Where(p => p.Name == "MyProject"),
    project => project.AddFile("NewFile.cs"),
    cancellationToken);
```

**AddFile 参数**：
- 文件名（相对于项目根目录）
- 可以包含子目录路径，如 `"Folder\\NewFile.cs"`

### 2. 从另一个位置复制文件

```csharp
string sourceFilePath = @"C:\Temp\Template.cs";
string destinationFileName = "CopiedFile.cs";

await querySpace.UpdateProjectsAsync(
    project => project.Where(p => p.Name == "MyProject"),
    project => project.AddFileFromCopy(sourceFilePath, destinationFileName),
    cancellationToken);
```

### 3. 删除文件（通过移动实现）

项目查询 API 目前不直接支持删除文件，需要通过两步操作：
1. 将文件复制到目标项目
2. 从源项目中移除

```csharp
// 第一步：复制文件到新项目
await querySpace.UpdateProjectsAsync(
    project => project.Where(p => p.Name == "TargetProject"),
    project => project.AddFileFromCopy(sourceFilePath, destinationFileName),
    cancellationToken);

// 第二步：需要其他 API 从源项目移除
```

### 4. 重命名项目

```csharp
await querySpace.Projects
    .Where(p => p.Name == "OldName")
    .AsUpdatable()
    .Rename("NewName")
    .ExecuteAsync(cancellationToken);
```

### 5. 构建项目

```csharp
// 构建单个项目
var result = await querySpace.QueryProjectsAsync(
    project => project.Where(p => p.Name == "MyProject")
                     .With(p => p.Name),
    cancellationToken);

await foreach (var project in result)
{
    await project.BuildAsync(cancellationToken);
}

// 或直接构建整个解决方案
await querySpace.Solutions.BuildAsync(cancellationToken);
```

### 6. 保存解决方案

```csharp
await querySpace.Solutions.SaveAsync(cancellationToken);
```

## 解决方案级别操作

### 1. 添加解决方案配置

```csharp
string solutionName = "MySolution";
string newConfigName = "CustomDebug";
string baseConfigName = "Debug";
bool propagate = false;

await querySpace.UpdateSolutionAsync(
    solution => solution.Where(s => s.BaseName == solutionName),
    solution => solution.AddSolutionConfiguration(
        newConfigName,
        baseConfigName,
        propagate),
    cancellationToken);
```

**参数说明**：
- **newConfigName** - 新配置名称
- **baseConfigName** - 基于的现有配置
- **propagate** - 是否传播到项目

### 2. 删除解决方案配置

```csharp
await querySpace.UpdateSolutionAsync(
    solution => solution.Where(s => s.BaseName == solutionName),
    solution => solution.DeleteSolutionConfiguration("CustomDebug"),
    cancellationToken);
```

### 3. 设置启动项目

```csharp
string projectPath1 = @"C:\Projects\Project1\Project1.csproj";
string projectPath2 = @"C:\Projects\Project2\Project2.csproj";

await querySpace.UpdateSolutionAsync(
    solution => solution.Where(s => s.BaseName == solutionName),
    solution => solution.SetStartupProjects(projectPath1, projectPath2),
    cancellationToken);
```

### 4. 卸载项目

```csharp
string projectPath = @"C:\Projects\MyProject\MyProject.csproj";

await querySpace.UpdateSolutionAsync(
    solution => solution.Where(s => s.BaseName == solutionName),
    solution => solution.UnloadProject(projectPath),
    cancellationToken);
```

### 5. 重新加载项目

```csharp
await querySpace.UpdateSolutionAsync(
    solution => solution.Where(s => s.BaseName == solutionName),
    solution => solution.ReloadProject(projectPath),
    cancellationToken);
```

## 完整示例：项目文件管理器

```csharp
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace MyExtension;

[VisualStudioContribution]
internal class ProjectFileManagerCommand : Command
{
    public override CommandConfiguration CommandConfiguration => new("%ProjectFileManager.DisplayName%")
    {
        Placements = [CommandPlacement.KnownPlacements.ToolsMenu],
    };

    public override async Task ExecuteCommandAsync(
        IClientContext context,
        CancellationToken cancellationToken)
    {
        var querySpace = this.Extensibility.Workspaces();
        var shell = this.Extensibility.Shell();

        // 1. 获取项目名称
        string? projectName = await shell.ShowPromptAsync(
            "请输入项目名称：",
            InputPromptOptions.Default,
            cancellationToken);

        if (string.IsNullOrEmpty(projectName))
        {
            return;
        }

        // 2. 选择操作
        var operation = await shell.ShowPromptAsync(
            "选择要执行的操作：",
            new PromptOptions<string>
            {
                Choices =
                {
                    { "添加新文件", "AddFile" },
                    { "重命名项目", "Rename" },
                    { "构建项目", "Build" },
                },
            },
            cancellationToken);

        // 3. 执行操作
        switch (operation)
        {
            case "AddFile":
                await AddFileToProjectAsync(
                    querySpace,
                    shell,
                    projectName,
                    cancellationToken);
                break;

            case "Rename":
                await RenameProjectAsync(
                    querySpace,
                    shell,
                    projectName,
                    cancellationToken);
                break;

            case "Build":
                await BuildProjectAsync(
                    querySpace,
                    shell,
                    projectName,
                    cancellationToken);
                break;
        }
    }

    private async Task AddFileToProjectAsync(
        WorkspacesExtensibility querySpace,
        ShellExtensibility shell,
        string projectName,
        CancellationToken cancellationToken)
    {
        // 获取文件名
        string? fileName = await shell.ShowPromptAsync(
            "输入新文件名（如 NewClass.cs）：",
            InputPromptOptions.Default,
            cancellationToken);

        if (string.IsNullOrEmpty(fileName))
        {
            return;
        }

        try
        {
            // 添加文件到项目
            await querySpace.UpdateProjectsAsync(
                project => project.Where(p => p.Name == projectName),
                project => project.AddFile(fileName),
                cancellationToken);

            await shell.ShowPromptAsync(
                $"文件 '{fileName}' 已成功添加到项目 '{projectName}'",
                PromptOptions.OK,
                cancellationToken);
        }
        catch (Exception ex)
        {
            await shell.ShowPromptAsync(
                $"添加文件失败: {ex.Message}",
                PromptOptions.ErrorConfirm,
                cancellationToken);
        }
    }

    private async Task RenameProjectAsync(
        WorkspacesExtensibility querySpace,
        ShellExtensibility shell,
        string projectName,
        CancellationToken cancellationToken)
    {
        // 获取新名称
        string? newName = await shell.ShowPromptAsync(
            "输入新项目名称：",
            InputPromptOptions.Default,
            cancellationToken);

        if (string.IsNullOrEmpty(newName))
        {
            return;
        }

        try
        {
            // 重命名项目
            await querySpace.Projects
                .Where(p => p.Name == projectName)
                .AsUpdatable()
                .Rename(newName)
                .ExecuteAsync(cancellationToken);

            await shell.ShowPromptAsync(
                $"项目已重命名: '{projectName}' -> '{newName}'",
                PromptOptions.OK,
                cancellationToken);
        }
        catch (Exception ex)
        {
            await shell.ShowPromptAsync(
                $"重命名失败: {ex.Message}",
                PromptOptions.ErrorConfirm,
                cancellationToken);
        }
    }

    private async Task BuildProjectAsync(
        WorkspacesExtensibility querySpace,
        ShellExtensibility shell,
        string projectName,
        CancellationToken cancellationToken)
    {
        try
        {
            // 查询并构建项目
            var result = await querySpace.QueryProjectsAsync(
                project => project.Where(p => p.Name == projectName)
                                 .With(p => p.Name),
                cancellationToken);

            bool found = false;
            await foreach (var project in result)
            {
                found = true;
                await project.BuildAsync(cancellationToken);
            }

            if (found)
            {
                await shell.ShowPromptAsync(
                    $"项目 '{projectName}' 构建已启动",
                    PromptOptions.OK,
                    cancellationToken);
            }
            else
            {
                await shell.ShowPromptAsync(
                    $"未找到项目 '{projectName}'",
                    PromptOptions.ErrorConfirm,
                    cancellationToken);
            }
        }
        catch (Exception ex)
        {
            await shell.ShowPromptAsync(
                $"构建失败: {ex.Message}",
                PromptOptions.ErrorConfirm,
                cancellationToken);
        }
    }
}
```

## 实践步骤

1. **准备修改操作**
   - 确定要修改的项目/解决方案
   - 准备必要的参数（文件名、配置名等）

2. **选择目标**
   - 使用 `Where` 过滤目标项目/解决方案
   - 确保选择器准确

3. **执行更新**
   - 调用相应的更新方法
   - 使用 `AsUpdatable` 使查询结果可更新

4. **错误处理**
   - 使用 try-catch 捕获异常
   - 向用户报告错误

5. **验证结果**
   - 检查操作是否成功
   - 必要时刷新解决方案资源管理器

## 常用操作总结

| 操作 | 方法 | 级别 |
|------|------|------|
| 添加文件 | `AddFile` | 项目 |
| 复制文件 | `AddFileFromCopy` | 项目 |
| 重命名项目 | `Rename` | 项目 |
| 构建项目 | `BuildAsync` | 项目 |
| 构建解决方案 | `BuildAsync` | 解决方案 |
| 保存解决方案 | `SaveAsync` | 解决方案 |
| 添加配置 | `AddSolutionConfiguration` | 解决方案 |
| 删除配置 | `DeleteSolutionConfiguration` | 解决方案 |
| 设置启动项目 | `SetStartupProjects` | 解决方案 |
| 卸载项目 | `UnloadProject` | 解决方案 |
| 重新加载项目 | `ReloadProject` | 解决方案 |

## 注意事项

### 1. 权限和限制

某些操作可能失败，原因包括：
- 项目被其他进程锁定
- 权限不足
- 项目不存在

### 2. 异步操作

所有修改操作都是异步的，需要使用 `await`。

### 3. 保存更改

某些更改可能需要显式保存：
```csharp
await querySpace.Solutions.SaveAsync(cancellationToken);
```

## 常见问题

### Q: 如何知道操作是否成功？
A: 大多数操作成功时不抛出异常，使用 try-catch 捕获失败情况。

### Q: 可以批量添加多个文件吗？
A: 可以，但需要多次调用 `AddFile`。

### Q: 修改后需要刷新 UI 吗？
A: Visual Studio 会自动更新解决方案资源管理器，通常不需要手动刷新。

### Q: 如何回滚操作？
A: 项目查询 API 不提供事务支持，需要手动实现回滚逻辑。

## 进阶主题

- 批量项目操作
- 项目依赖管理
- 自定义构建配置
- 与源代码管理集成

## 参考资料

- 源代码示例：`VSProjectQueryAPISample`
- llm.md 相关代码片段：
  - 第 137-149 行：创建文件
  - 第 375-386 行：设置启动项目
  - 第 610-619 行：构建解决方案
  - 第 886-897 行：添加解决方案配置
  - 第 1051-1059 行：保存解决方案
  - 第 1077-1085 行：构建项目
  - 第 1412-1423 行：删除解决方案配置
  - 第 1461-1472 行：卸载项目
  - 第 1533-1545 行：重命名项目
  - 第 1700-1711 行：重新加载项目
  - 第 1760-1771 行：添加文件到项目

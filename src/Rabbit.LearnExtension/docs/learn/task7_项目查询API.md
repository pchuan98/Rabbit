# 任务 7：使用项目查询 API

## 学习目标

掌握如何使用 Visual Studio 项目查询 API 来读取项目、解决方案和配置信息。

## 核心概念

### 1. 获取 WorkspacesExtensibility 对象

**要实现什么**：通过 `Extensibility.Workspaces()` 获取工作区扩展对象

**结果**：这是访问项目系统查询 API 的入口点

```csharp
WorkspacesExtensibility querySpace = this.Extensibility.Workspaces();
```

### 2. 使用 QueryProjectsAsync 查询项目信息

**要实现什么**：调用 `QueryProjectsAsync` 并传入查询表达式

**结果**：返回异步枚举器，可遍历查询结果

### 3. 使用 With 选择需要的属性

**要实现什么**：通过 `With(p => p.Name)` 等语法选择要查询的项目属性

**结果**：只会加载你指定的属性，提高性能

### 4. 查询项目文件和配置信息

**要实现什么**：使用 `With(p => p.Files.With(f => f.Path))` 查询项目中的文件

**结果**：可以链式查询嵌套属性（如配置的输出路径）

### 5. 使用 Where 过滤项目

**要实现什么**：通过 `Where(p => p.Name == "MyProject")` 过滤符合条件的项目

**结果**：减少返回的结果数量

### 6. 查询解决方案级别信息

**要实现什么**：使用 `querySpace.Solutions` 查询解决方案信息（如解决方案名称、配置等）

**结果**：获取解决方案级别的数据

## 知识点详解

### 1. 基本项目查询

```csharp
var querySpace = this.Extensibility.Workspaces();

var result = await querySpace.QueryProjectsAsync(
    project => project.With(p => p.Name)
                     .With(p => p.Path),
    cancellationToken);

await foreach (var project in result)
{
    string name = project.Value.Name;
    string path = project.Value.Path;
}
```

**With 方法**：指定要查询的属性
- 只有使用 `With` 指定的属性才会被填充
- 可以链式调用多个 `With`

### 2. 查询项目文件

```csharp
var result = await querySpace.QueryProjectsAsync(
    project => project.With(project => project.Name)
                     .With(project => project.Path)
                     .With(project => project.Files
                         .With(file => file.FileName)
                         .With(file => file.Path)
                         .With(file => file.ItemType)),
    cancellationToken);

await foreach (var project in result)
{
    foreach (var file in project.Value.Files)
    {
        string fileName = file.FileName;
        string filePath = file.Path;
        string itemType = file.ItemType;
    }
}
```

### 3. 查询项目配置

```csharp
var result = await querySpace.QueryProjectsAsync(
    project => project.With(p => p.Name)
                     .With(p => p.ActiveConfigurations
                         .With(c => c.Name)
                         .With(c => c.OutputGroups
                             .With(g => g.Name))),
    cancellationToken);

await foreach (var project in result)
{
    foreach (var config in project.Value.ActiveConfigurations)
    {
        string configName = config.Name;

        foreach (var group in config.OutputGroups)
        {
            string groupName = group.Name;
        }
    }
}
```

### 4. 过滤项目

使用 `Where` 方法过滤项目：

```csharp
var result = await querySpace.QueryProjectsAsync(
    project => project.Where(p => p.Name == "MyProject")
                     .With(p => p.Name)
                     .With(p => p.Path),
    cancellationToken);
```

### 5. 分页查询

使用 `Skip` 限制结果数量：

```csharp
var result = await querySpace.QueryProjectsAsync(
    project => project.With(p => p.Name)
                     .Skip(1),  // 跳过第一个项目
    cancellationToken);
```

### 6. 查询解决方案

```csharp
var result = await querySpace.Solutions
    .With(s => s.Name)
    .With(s => s.Path)
    .ExecuteQueryAsync(cancellationToken);

await foreach (var solution in result)
{
    string name = solution.Value.Name;
    string path = solution.Value.Path;
}
```

### 7. 按 ID 查询模式

对于已知的项目，可以使用 `AsQueryable()` 进行后续查询：

```csharp
// 第一步：获取基本信息
var result = await querySpace.QueryProjectsAsync(
    project => project.With(p => p.Name)
                     .With(p => p.ActiveConfigurations
                         .With(c => c.Name)),
    cancellationToken);

// 第二步：对特定项目进行详细查询
await foreach (var project in result)
{
    foreach (var config in project.Value.ActiveConfigurations)
    {
        // 使用 AsQueryable 获取更多详情
        var detailedConfig = await config.AsQueryable()
            .With(c => c.OutputGroups
                .With(g => g.Name)
                .With(g => g.Description))
            .ExecuteQueryAsync();
    }
}
```

## 完整示例：项目浏览器

```csharp
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyExtension;

[VisualStudioContribution]
internal class ProjectBrowserCommand : Command
{
    public override CommandConfiguration CommandConfiguration => new("%ProjectBrowser.DisplayName%")
    {
        Placements = [CommandPlacement.KnownPlacements.ToolsMenu],
    };

    public override async Task ExecuteCommandAsync(
        IClientContext context,
        CancellationToken cancellationToken)
    {
        var querySpace = this.Extensibility.Workspaces();
        var message = new StringBuilder();

        // 查询解决方案信息
        var solutionResult = await querySpace.Solutions
            .With(s => s.Name)
            .With(s => s.Path)
            .ExecuteQueryAsync(cancellationToken);

        await foreach (var solution in solutionResult)
        {
            message.AppendLine($"解决方案: {solution.Value.Name}");
            message.AppendLine($"路径: {solution.Value.Path}");
            message.AppendLine();
        }

        // 查询所有项目
        var projectResult = await querySpace.QueryProjectsAsync(
            project => project.With(p => p.Name)
                             .With(p => p.Path)
                             .With(p => p.Guid)
                             .With(p => p.ActiveConfigurations
                                 .With(c => c.Name))
                             .With(p => p.Files
                                 .With(f => f.FileName)),
            cancellationToken);

        int projectCount = 0;
        int fileCount = 0;

        await foreach (var project in projectResult)
        {
            projectCount++;
            message.AppendLine($"项目 {projectCount}: {project.Value.Name}");
            message.AppendLine($"  路径: {project.Value.Path}");
            message.AppendLine($"  GUID: {project.Value.Guid}");

            // 活动配置
            foreach (var config in project.Value.ActiveConfigurations)
            {
                message.AppendLine($"  活动配置: {config.Name}");
            }

            // 文件数量
            int projectFileCount = 0;
            foreach (var file in project.Value.Files)
            {
                projectFileCount++;
                fileCount++;
            }
            message.AppendLine($"  文件数: {projectFileCount}");
            message.AppendLine();
        }

        message.AppendLine($"总计: {projectCount} 个项目, {fileCount} 个文件");

        // 显示结果
        await this.Extensibility.Shell().ShowPromptAsync(
            message.ToString(),
            PromptOptions.OK,
            cancellationToken);
    }
}
```

## 常用查询操作

### 查询项目 GUID

```csharp
var result = await querySpace.QueryProjectsAsync(
    project => project.With(p => p.Name)
                     .With(p => p.Guid),
    cancellationToken);
```

### 查询项目引用

```csharp
var result = await querySpace.QueryProjectsAsync(
    project => project.With(p => p.Name)
                     .With(p => p.References
                         .With(r => r.Name)),
    cancellationToken);
```

### 查询文件详细信息

```csharp
var result = await querySpace.Projects
    .With(project => project.Files
        .With(file => file.ItemType)
        .With(file => file.ItemName)
        .With(file => file.Path)
        .With(file => file.LinkPath)
        .With(file => file.VisualPath))
    .ExecuteQueryAsync();
```

## 实践步骤

1. **获取 WorkspacesExtensibility**
   ```csharp
   var querySpace = this.Extensibility.Workspaces();
   ```

2. **构建查询**
   - 使用 `QueryProjectsAsync` 或 `Solutions.ExecuteQueryAsync`
   - 使用 `With` 指定要查询的属性
   - 使用 `Where` 过滤结果

3. **执行查询**
   - 查询返回异步可枚举结果
   - 使用 `await foreach` 遍历结果

4. **访问查询结果**
   - 通过 `.Value` 访问实际数据
   - 访问嵌套属性（如文件、配置）

5. **显示结果**
   - 格式化查询结果
   - 使用提示框或工具窗口显示

## 性能建议

### 1. 只查询需要的属性

❌ 不好：
```csharp
project => project  // 查询所有属性（慢）
```

✅ 好：
```csharp
project => project.With(p => p.Name)  // 只查询名称
                 .With(p => p.Path)   // 和路径
```

### 2. 使用过滤器减少结果

```csharp
project => project.Where(p => p.Name.Contains("Test"))
                 .With(p => p.Name)
```

### 3. 按需查询详细信息

使用"按 ID 查询"模式，先查询基本信息，再按需查询详情。

## 常见问题

### Q: 查询结果为空？
A: 检查：
- 是否打开了解决方案
- 查询条件是否过于严格
- 是否使用了正确的属性名

### Q: 某些属性值为 null？
A: 确保使用 `With` 方法指定了该属性，未指定的属性不会被填充。

### Q: 如何查询所有解决方案配置？
A:
```csharp
var result = await querySpace.Solutions
    .With(s => s.Configurations
        .With(c => c.Name))
    .ExecuteQueryAsync();
```

### Q: 性能优化建议？
A:
- 只查询需要的属性
- 使用过滤器
- 考虑缓存结果
- 使用分页（Skip）

## 进阶主题

- 修改项目系统（Task 8）
- 复杂查询优化
- 项目系统事件监听
- 与其他服务集成

## 参考资料

- 源代码示例：`VSProjectQueryAPISample`
- llm.md 相关代码片段：
  - 第 36-54 行：执行项目查询
  - 第 58-71 行：查询输出组
  - 第 422-430 行：获取 WorkspacesExtensibility
  - 第 610-619 行：调用构建操作
  - 第 623-634 行：按 ID 查询模式
  - 第 1310-1334 行：按 ID 进行后续查询
  - 第 1338-1350 行：查询项目信息

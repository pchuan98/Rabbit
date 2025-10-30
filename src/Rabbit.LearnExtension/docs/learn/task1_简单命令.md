# 任务 1：创建第一个简单命令

## 学习目标

掌握 Visual Studio 扩展中最基础的功能——创建和注册命令，了解扩展的基本结构。

## 核心概念

### 1. Command 类

所有命令都需要继承自 `Command` 基类，这是 VisualStudio.Extensibility 框架提供的核心抽象类。

```csharp
[VisualStudioContribution]
internal class MyFirstCommand : Command
{
    // 命令实现
}
```

### 2. [VisualStudioContribution] 特性

这个特性用于将类注册到 Visual Studio 扩展系统中。没有这个特性，Visual Studio 无法发现你的命令。

**作用：**
- 自动注册组件到扩展框架
- 使用类的完整类型名作为唯一标识符
- 在编译时生成必要的元数据

### 3. CommandConfiguration 属性

通过重写 `CommandConfiguration` 属性来配置命令的外观和行为。

```csharp
public override CommandConfiguration CommandConfiguration => new("%MyCommand.DisplayName%")
{
    Placements = [CommandPlacement.KnownPlacements.ToolsMenu],
    Icon = new(ImageMoniker.KnownValues.Extension, IconSettings.IconAndText),
    TooltipText = "%MyCommand.ToolTip%",
};
```

**配置项说明：**
- **DisplayName**：命令在菜单中显示的名称（支持本地化字符串）
- **Placements**：命令在 UI 中的位置（如工具菜单、扩展菜单等）
- **Icon**：命令的图标（使用 `ImageMoniker.KnownValues` 中的预定义图标）
- **IconSettings**：图标显示方式（仅图标、仅文本、图标和文本）
- **TooltipText**：鼠标悬停时显示的提示文本

### 4. ExecuteCommandAsync 方法

这是命令的核心逻辑所在，当用户点击命令时会执行这个方法。

```csharp
public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
{
    // 命令执行逻辑
    await this.Extensibility.Shell().ShowPromptAsync(
        "Hello from my first command!",
        PromptOptions.OK,
        cancellationToken);
}
```

**参数说明：**
- **context**：客户端上下文，提供命令执行时的环境信息
- **cancellationToken**：用于取消异步操作的令牌

## 知识点详解

### 菜单位置 (Placements)

常用的预定义位置：
- `CommandPlacement.KnownPlacements.ToolsMenu` - 工具菜单
- `CommandPlacement.KnownPlacements.ExtensionsMenu` - 扩展菜单
- `CommandPlacement.KnownPlacements.ViewOtherWindowsMenu` - 视图 > 其他窗口菜单

### 图标 (Icons)

使用预定义图标：
```csharp
Icon = new(ImageMoniker.KnownValues.Extension, IconSettings.IconAndText)
```

常用图标：
- `Extension` - 扩展图标
- `ToolWindow` - 工具窗口图标
- `Dialog` - 对话框图标
- `Settings` - 设置图标

### 本地化字符串

使用 `%` 包围的字符串会从资源文件中查找：
```csharp
DisplayName = "%MyCommand.DisplayName%"
```

对应的资源文件（`string-resources.json`）：
```json
{
    "MyCommand.DisplayName": "我的第一个命令"
}
```

## 完整示例

```csharp
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace MyExtension;

[VisualStudioContribution]
internal class MyFirstCommand : Command
{
    public override CommandConfiguration CommandConfiguration => new("%MyFirstCommand.DisplayName%")
    {
        Placements = [CommandPlacement.KnownPlacements.ToolsMenu],
        Icon = new(ImageMoniker.KnownValues.Extension, IconSettings.IconAndText),
        TooltipText = "%MyFirstCommand.ToolTip%",
    };

    public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
    {
        await this.Extensibility.Shell().ShowPromptAsync(
            "这是我的第一个 Visual Studio 命令！",
            PromptOptions.OK,
            cancellationToken);
    }
}
```

## 实践步骤

1. **创建命令类**
   - 继承 `Command` 基类
   - 添加 `[VisualStudioContribution]` 特性

2. **配置命令属性**
   - 重写 `CommandConfiguration` 属性
   - 设置显示名称、图标、位置

3. **实现命令逻辑**
   - 重写 `ExecuteCommandAsync` 方法
   - 添加简单的提示框显示

4. **测试命令**
   - 构建并运行扩展
   - 在实验实例中查找并点击命令
   - 验证提示框是否正确显示

## 常见问题

### Q: 命令没有出现在菜单中？
A: 检查以下几点：
- 是否添加了 `[VisualStudioContribution]` 特性
- `Placements` 配置是否正确
- 扩展是否正确部署到实验实例

### Q: 如何在不同位置显示同一个命令？
A: 在 `Placements` 数组中添加多个位置：
```csharp
Placements = [
    CommandPlacement.KnownPlacements.ToolsMenu,
    CommandPlacement.KnownPlacements.ExtensionsMenu
]
```

### Q: 可以使用自定义图标吗？
A: 可以，但建议优先使用 `ImageMoniker.KnownValues` 中的预定义图标，以保持与 Visual Studio 的一致性。

## 进阶主题

- 命令的启用/禁用条件（`EnabledWhen`）
- 命令的可见性条件（`VisibleWhen`）
- 快捷键配置（`Shortcuts`）
- 命令参数传递

## 参考资料

- 源代码示例：`SimpleRemoteCommandSample`
- llm.md 相关代码片段：第 1004-1016 行

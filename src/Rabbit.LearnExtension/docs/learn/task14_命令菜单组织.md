# 任务 14：命令菜单组织

## 学习目标

掌握如何组织和管理命令的菜单结构，包括创建自定义菜单、工具栏和配置命令位置。

## 核心概念

### 1. 使用 MenuConfiguration 创建自定义菜单

**要实现什么**：定义静态 `MenuConfiguration` 属性并标记 `[VisualStudioContribution]`

**结果**：创建新的菜单（如 Extensions 下的子菜单）

### 2. 使用 ToolbarConfiguration 创建工具栏

**要实现什么**：类似菜单配置，创建 `ToolbarConfiguration` 定义工具栏

**结果**：可以是独立工具栏或工具窗口的工具栏

### 3. 配置命令放置位置（Placements）

**要实现什么**：在 `CommandConfiguration` 的 `Placements` 中指定命令位置

**结果**：可以放在预定义位置（`KnownPlacements.ToolsMenu`）或自定义菜单

### 4. 使用 CommandPlacement.VsctParent 添加到现有菜单

**要实现什么**：通过 GUID 和 ID 引用 VS 内置菜单项

**结果**：可以将命令添加到解决方案资源管理器右键菜单等位置

### 5. 组织菜单子项（命令和分隔符）

**要实现什么**：在菜单配置的 `Children` 中添加 `MenuChild.Command<YourCommand>()` 和 `MenuChild.Separator`

**结果**：组织菜单结构

### 6. 配置工具窗口工具栏

**要实现什么**：在 `ToolWindowConfiguration` 中设置 `Toolbar` 属性

**结果**：工具窗口顶部会显示配置的工具栏

### 7. 设置命令优先级

**要实现什么**：在 `CommandPlacement` 中设置 `priority` 参数

**结果**：控制同一位置多个命令的显示顺序

## 知识点详解

### 1. 创建自定义菜单

```csharp
public static class ExtensionMenus
{
    [VisualStudioContribution]
    public static MenuConfiguration MyMenu => new("%MyExtension.Menu.DisplayName%")
    {
        Placements = [CommandPlacement.KnownPlacements.ExtensionsMenu],
        Children =
        [
            MenuChild.Command<Command1>(),
            MenuChild.Command<Command2>(),
            MenuChild.Separator,
            MenuChild.Command<Command3>(),
        ],
    };
}
```

### 2. 创建工具栏

```csharp
[VisualStudioContribution]
public static ToolbarConfiguration MyToolbar => new("%MyExtension.Toolbar.DisplayName%")
{
    Children =
    [
        ToolbarChild.Command<SaveCommand>(),
        ToolbarChild.Command<LoadCommand>(),
    ],
};
```

### 3. 将命令添加到自定义菜单

```csharp
[VisualStudioContribution]
internal class MyCommand : Command
{
    public override CommandConfiguration CommandConfiguration => new("%MyCommand.DisplayName%")
    {
        Placements = [new CommandPlacement(ExtensionMenus.MyMenu)],
    };
}
```

### 4. 添加到 VS 内置菜单（使用 VSCT GUID）

```csharp
public override CommandConfiguration CommandConfiguration => new("%MyCommand.DisplayName%")
{
    Placements =
    [
        // 添加到解决方案资源管理器的项目右键菜单
        CommandPlacement.VsctParent(
            new Guid("{d309f791-903f-11d0-9efc-00a0c911004f}"),
            id: 518,
            priority: 0),

        // 添加到文件右键菜单
        CommandPlacement.VsctParent(
            new Guid("{d309f791-903f-11d0-9efc-00a0c911004f}"),
            id: 521,
            priority: 0),
    ],
};
```

### 5. 工具窗口工具栏

```csharp
[VisualStudioContribution]
public class MyToolWindow : ToolWindow
{
    [VisualStudioContribution]
    private static ToolbarConfiguration Toolbar => new("%MyToolWindow.Toolbar.DisplayName%")
    {
        Children = [ToolbarChild.Command<RefreshCommand>()],
    };

    public override ToolWindowConfiguration ToolWindowConfiguration => new()
    {
        Placement = ToolWindowPlacement.DocumentWell,
        Toolbar = new ToolWindowToolbar(Toolbar),
    };
}
```

## 常用 VS 菜单 GUID 和 ID

### 解决方案资源管理器

```csharp
// 项目右键菜单
new Guid("{d309f791-903f-11d0-9efc-00a0c911004f}"), id: 518

// 文件右键菜单
new Guid("{d309f791-903f-11d0-9efc-00a0c911004f}"), id: 521

// 解决方案右键菜单
new Guid("{d309f791-903f-11d0-9efc-00a0c911004f}"), id: 537
```

## 完整示例

```csharp
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;

namespace MyExtension;

// 定义菜单结构
public static class ExtensionCommands
{
    [VisualStudioContribution]
    public static MenuConfiguration MainMenu => new("%Extension.MainMenu.DisplayName%")
    {
        Placements = [CommandPlacement.KnownPlacements.ExtensionsMenu.WithPriority(0x01)],
        Children =
        [
            MenuChild.Command<ProcessCommand>(),
            MenuChild.Command<AnalyzeCommand>(),
            MenuChild.Separator,
            MenuChild.Command<SettingsCommand>(),
        ],
    };

    [VisualStudioContribution]
    public static ToolbarConfiguration MainToolbar => new("%Extension.Toolbar.DisplayName%")
    {
        Children =
        [
            ToolbarChild.Command<ProcessCommand>(),
            ToolbarChild.Command<AnalyzeCommand>(),
        ],
    };
}

// 命令1
[VisualStudioContribution]
internal class ProcessCommand : Command
{
    public override CommandConfiguration CommandConfiguration => new("%Process.DisplayName%")
    {
        Icon = new(ImageMoniker.KnownValues.Play, IconSettings.IconAndText),
        Shortcuts = [new CommandShortcutConfiguration(ModifierKey.Control, Key.P)],
    };

    public override Task ExecuteCommandAsync(IClientContext context, CancellationToken ct)
    {
        // 实现...
        return Task.CompletedTask;
    }
}

// 命令2
[VisualStudioContribution]
internal class AnalyzeCommand : Command
{
    public override CommandConfiguration CommandConfiguration => new("%Analyze.DisplayName%")
    {
        Icon = new(ImageMoniker.KnownValues.Analyze, IconSettings.IconAndText),
    };

    public override Task ExecuteCommandAsync(IClientContext context, CancellationToken ct)
    {
        // 实现...
        return Task.CompletedTask;
    }
}

// 命令3 - 也添加到右键菜单
[VisualStudioContribution]
internal class SettingsCommand : Command
{
    public override CommandConfiguration CommandConfiguration => new("%Settings.DisplayName%")
    {
        Placements =
        [
            // 添加到项目右键菜单
            CommandPlacement.VsctParent(
                new Guid("{d309f791-903f-11d0-9efc-00a0c911004f}"),
                id: 518,
                priority: 0),
        ],
        Icon = new(ImageMoniker.KnownValues.Settings, IconSettings.IconAndText),
    };

    public override Task ExecuteCommandAsync(IClientContext context, CancellationToken ct)
    {
        // 实现...
        return Task.CompletedTask;
    }
}
```

## 常见问题

### Q: 如何查找 VS 内置菜单的 GUID 和 ID？
A: 参考 Visual Studio SDK 的 VSCT 文件或使用工具如 VSCommands。

### Q: 命令会自动添加到菜单吗？
A: 不会，需要在 `MenuConfiguration.Children` 中明确添加。

### Q: 优先级如何影响显示顺序？
A: 数值越小，越靠前显示。

### Q: 分隔符会自动优化吗？
A: VS 会自动移除冗余的分隔符（开头、结尾、连续的分隔符）。

## 参考资料

- llm.md 相关代码片段：
  - 第 932-948 行：定义工具栏
  - 第 972-984 行：配置工具窗口工具栏
  - 第 1143-1154 行：定义命令类
  - 第 1244-1255 行：引用工具栏配置
  - 第 1354-1375 行：配置命令放置位置
  - 第 1584-1612 行：定义菜单结构

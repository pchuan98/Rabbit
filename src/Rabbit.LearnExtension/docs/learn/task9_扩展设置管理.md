# 任务 9：扩展设置管理

## 学习目标

掌握如何为 Visual Studio 扩展创建和管理用户设置，包括定义设置、读取设置值、监听设置变化和编程方式更新设置。

## 核心概念

### 1. 定义 SettingCategory 和 Setting 类

**要实现什么**：创建静态属性定义设置分类和具体设置项

**结果**：使用 `Setting.Boolean`、`Setting.String` 等类型定义不同类型的设置

```csharp
[VisualStudioContribution]
internal static SettingCategory MyCategory { get; } = new("myCategory", "我的设置")
{
    Description = "示例设置类别",
    GenerateObserverClass = true,
};

[VisualStudioContribution]
internal static Setting.Boolean MySetting { get; } = new(
    "mySetting", "我的布尔设置", MyCategory, defaultValue: true);
```

### 2. 使用 [VisualStudioContribution] 注册设置

**要实现什么**：在设置类和分类上添加 `[VisualStudioContribution]`

**结果**：VS 会自动在工具-选项对话框中显示这些设置

### 3. 实现设置观察者（Observer）模式

**要实现什么**：设置 `GenerateObserverClass = true`，构建时自动生成观察者类

**结果**：注入观察者类到你的组件中监听设置变化

### 4. 订阅设置更改事件

**要实现什么**：订阅观察者的 `Changed` 事件

**结果**：用户修改设置时会触发此事件，参数包含新的设置快照

### 5. 读取设置当前值

**要实现什么**：通过 `settingsSnapshot.YourSetting.ValueOrDefault(defaultValue)` 读取设置值

**结果**：提供默认值防止设置不存在时出错

### 6. 编程方式写入设置

**要实现什么**：调用 `Extensibility.Settings().WriteAsync()` 在批处理中修改设置

**结果**：可同时修改多个设置

### 7. 处理用户覆盖的设置值

**要实现什么**：用户可在 `extensibility.settings.json` 中覆盖设置

**结果**：读取时会自动使用覆盖后的值

## 知识点详解

### 1. 定义设置类别

```csharp
using Microsoft.VisualStudio.Extensibility.Settings;

public static class SettingDefinitions
{
    [VisualStudioContribution]
    internal static SettingCategory SettingsSampleCategory { get; } =
        new("settingsSample", "%SettingsSample.Category.DisplayName%")
        {
            Description = "%SettingsSample.Category.Description%",
            GenerateObserverClass = true,
        };
}
```

**关键属性：**
- **ID**（第一个参数）：设置类别的唯一标识符
  - 必须以小写字母开头
  - 只能包含字母、数字和句点
- **DisplayName**：在设置 UI 中显示的名称
- **Description**：类别描述
- **GenerateObserverClass**：是否生成观察者类（用于监听设置变化）

### 2. 定义不同类型的设置

#### 布尔设置

```csharp
[VisualStudioContribution]
internal static Setting.Boolean AutoUpdateSetting { get; } = new(
    "autoUpdate",
    "%SettingsSample.AutoUpdate.DisplayName%",
    SettingsSampleCategory,
    defaultValue: true)
{
    Description = "%SettingsSample.AutoUpdate.Description%",
};
```

#### 整数设置

```csharp
[VisualStudioContribution]
internal static Setting.Int32 TextLengthSetting { get; } = new(
    "textLength",
    "%SettingsSample.TextLength.DisplayName%",
    SettingsSampleCategory,
    defaultValue: 100)
{
    Description = "%SettingsSample.TextLength.Description%",
};
```

#### 字符串设置

```csharp
[VisualStudioContribution]
internal static Setting.String DefaultThemeSetting { get; } = new(
    "defaultTheme",
    "默认主题",
    SettingsSampleCategory,
    defaultValue: "Light")
{
    Description = "应用程序的默认主题",
};
```

### 3. 设置 ID 命名规则

**有效的设置 ID：**
- ✅ `settingsSample` - 以小写字母开头
- ✅ `mySettings` - 驼峰命名
- ✅ `app.settings.general` - 包含句点

**无效的设置 ID：**
- ❌ `SettingsSample` - 以大写字母开头
- ❌ `my-settings` - 包含连字符
- ❌ `my_settings` - 包含下划线

### 4. 设置的完整 ID

设置的完整 ID 由类别 ID 和设置 ID 组成：

```
{categoryId}.{settingId}
```

例如：
```
settingsSample.autoUpdate
settingsSample.textLength
```

### 5. 使用设置观察者监听变化

当 `GenerateObserverClass = true` 时，会自动生成观察者类：

```csharp
// 注入观察者
public MyToolWindowData(
    VisualStudioExtensibility extensibility,
    SettingsSampleCategoryObserver settingsObserver)
{
    this.extensibility = extensibility;
    this.settingsObserver = settingsObserver;

    // 订阅变化事件
    settingsObserver.Changed += this.SettingsObserver_ChangedAsync;
}

// 处理变化
private Task SettingsObserver_ChangedAsync(
    Settings.SettingsSampleCategorySnapshot settingsSnapshot)
{
    // 读取设置值
    bool autoUpdate = settingsSnapshot.AutoUpdateSetting.ValueOrDefault(defaultValue: true);
    int textLength = settingsSnapshot.TextLengthSetting.ValueOrDefault(defaultValue: 100);

    // 更新 UI 或应用逻辑
    this.AutoUpdate = autoUpdate;
    this.MaxLength = textLength;

    return Task.CompletedTask;
}
```

**重要：** `Changed` 事件在订阅时至少会被调用一次，提供当前设置值。

### 6. 读取设置当前值

如果不想只依赖事件，可以主动读取设置：

```csharp
var settingsSnapshot = await this.settingsObserver.GetSnapshotAsync(cancellationToken);
bool autoUpdate = settingsSnapshot.AutoUpdateSetting.ValueOrDefault(defaultValue: true);
```

### 7. 使用 ValueOrDefault

推荐使用 `ValueOrDefault` 而不是直接访问 `Value`：

```csharp
// ✅ 推荐：安全，提供默认值
bool value = settingsSnapshot.AutoUpdateSetting.ValueOrDefault(defaultValue: true);

// ❌ 不推荐：可能抛出异常
bool value = settingsSnapshot.AutoUpdateSetting.Value;
```

### 8. 编程方式写入设置

```csharp
await this.extensibility.Settings().WriteAsync(
    batch =>
    {
        batch.WriteSetting(SettingDefinitions.AutoUpdateSetting, false);
        batch.WriteSetting(SettingDefinitions.TextLengthSetting, 150);
    },
    description: "更新应用设置",
    CancellationToken.None);
```

**WriteAsync 参数：**
- **batch** - 批处理操作（可以一次更新多个设置）
- **description** - 操作描述（用于撤销/重做）
- **cancellationToken** - 取消令牌

### 9. 用户覆盖设置

用户可以通过 `extensibility.settings.json` 文件覆盖设置：

```json
{
  "settingsSample.textLength": 150,
  "settingsSample.autoUpdate": false
}
```

## 完整示例：主题设置管理

```csharp
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Settings;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace MyExtension;

// 1. 定义设置
public static class ThemeSettings
{
    [VisualStudioContribution]
    internal static SettingCategory Category { get; } = new(
        "themeSettings",
        "主题设置")
    {
        Description = "控制应用程序主题的设置",
        GenerateObserverClass = true,
    };

    [VisualStudioContribution]
    internal static Setting.Boolean DarkModeEnabled { get; } = new(
        "darkMode",
        "启用深色模式",
        Category,
        defaultValue: false)
    {
        Description = "是否使用深色主题",
    };

    [VisualStudioContribution]
    internal static Setting.Int32 FontSize { get; } = new(
        "fontSize",
        "字体大小",
        Category,
        defaultValue: 14)
    {
        Description = "界面字体大小",
    };

    [VisualStudioContribution]
    internal static Setting.String AccentColor { get; } = new(
        "accentColor",
        "强调色",
        Category,
        defaultValue: "Blue")
    {
        Description = "界面强调色",
    };
}

// 2. 工具窗口数据类（使用设置）
[DataContract]
internal class ThemeToolWindowData : NotifyPropertyChangedObject
{
    private readonly VisualStudioExtensibility extensibility;
    private readonly ThemeSettingsCategoryObserver settingsObserver;

    private bool _darkModeEnabled;
    private int _fontSize;
    private string _accentColor = "Blue";

    public ThemeToolWindowData(
        VisualStudioExtensibility extensibility,
        ThemeSettingsCategoryObserver settingsObserver)
    {
        this.extensibility = extensibility;
        this.settingsObserver = settingsObserver;

        // 订阅设置变化
        settingsObserver.Changed += this.OnSettingsChangedAsync;
    }

    [DataMember]
    public bool DarkModeEnabled
    {
        get => _darkModeEnabled;
        set => SetProperty(ref _darkModeEnabled, value);
    }

    [DataMember]
    public int FontSize
    {
        get => _fontSize;
        set => SetProperty(ref _fontSize, value);
    }

    [DataMember]
    public string AccentColor
    {
        get => _accentColor;
        set => SetProperty(ref _accentColor, value);
    }

    [DataMember]
    public AsyncCommand ToggleDarkModeCommand => new(async (_, cancellationToken) =>
    {
        await this.extensibility.Settings().WriteAsync(
            batch =>
            {
                batch.WriteSetting(ThemeSettings.DarkModeEnabled, !DarkModeEnabled);
            },
            description: "切换深色模式",
            cancellationToken);
    });

    [DataMember]
    public AsyncCommand IncreaseFontSizeCommand => new(async (_, cancellationToken) =>
    {
        await this.extensibility.Settings().WriteAsync(
            batch =>
            {
                batch.WriteSetting(ThemeSettings.FontSize, FontSize + 1);
            },
            description: "增加字体大小",
            cancellationToken);
    });

    private Task OnSettingsChangedAsync(Settings.ThemeSettingsCategorySnapshot snapshot)
    {
        DarkModeEnabled = snapshot.DarkModeEnabled.ValueOrDefault(defaultValue: false);
        FontSize = snapshot.FontSize.ValueOrDefault(defaultValue: 14);
        AccentColor = snapshot.AccentColor.ValueOrDefault(defaultValue: "Blue");

        return Task.CompletedTask;
    }
}
```

## 实践步骤

1. **定义设置类别**
   - 创建 `SettingCategory` 静态属性
   - 添加 `[VisualStudioContribution]` 特性
   - 设置 `GenerateObserverClass = true`

2. **定义设置项**
   - 创建 `Setting.Boolean`, `Setting.Int32` 等静态属性
   - 添加 `[VisualStudioContribution]` 特性
   - 关联到设置类别
   - 提供默认值

3. **注入观察者**
   - 在构造函数中接收观察者参数
   - 订阅 `Changed` 事件

4. **处理设置变化**
   - 实现事件处理方法
   - 使用 `ValueOrDefault` 读取设置值
   - 更新应用程序状态

5. **写入设置**
   - 使用 `Settings().WriteAsync` 更新设置
   - 在批处理中可以更新多个设置

6. **测试设置**
   - 通过 UI 修改设置
   - 验证应用是否正确响应
   - 测试默认值是否正确

## 设置类型总结

| C# 类型 | Setting 类型 | 示例默认值 |
|---------|--------------|-----------|
| `bool` | `Setting.Boolean` | `true` |
| `int` | `Setting.Int32` | `100` |
| `string` | `Setting.String` | `"default"` |

## 本地化设置

使用资源字符串进行本地化：

```csharp
[VisualStudioContribution]
internal static SettingCategory Category { get; } =
    new("myCategory", "%MyExtension.Settings.Category.DisplayName%")
    {
        Description = "%MyExtension.Settings.Category.Description%",
    };
```

在 `string-resources.json` 中：
```json
{
    "MyExtension.Settings.Category.DisplayName": "我的设置",
    "MyExtension.Settings.Category.Description": "控制扩展行为的设置"
}
```

## 常见问题

### Q: 设置不显示在 Visual Studio 设置中？
A: 检查：
- 是否添加了 `[VisualStudioContribution]` 特性
- 设置 ID 是否符合命名规则
- 扩展是否正确部署

### Q: 观察者类未生成？
A: 确保 `GenerateObserverClass = true` 并重新构建项目。

### Q: 如何在没有观察者的情况下读取设置？
A: 可以直接使用设置服务，但推荐使用观察者模式。

### Q: 设置何时会自动保存？
A: 使用 `WriteAsync` 写入的设置会立即持久化。

## 进阶主题

- 设置验证
- 设置迁移
- 设置导入/导出
- 设置搜索和过滤

## 参考资料

- 源代码示例：`SettingsSample`
- llm.md 相关代码片段：
  - 第 166-177 行：用户覆盖设置示例
  - 第 349-357 行：有效的设置 ID
  - 第 434-453 行：处理设置变化
  - 第 538-557 行：定义设置
  - 第 662-670 行：读取设置快照
  - 第 674-688 行：写入设置

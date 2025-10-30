# 任务 2：使用用户提示和对话框

## 学习目标

掌握如何在 Visual Studio 扩展中与用户进行交互，包括显示各种类型的提示框和获取用户输入。

## 核心概念

### 1. Shell 服务

通过 `Extensibility.Shell()` 获取 Shell 扩展服务，这是与 Visual Studio UI 交互的入口点。

```csharp
var shell = this.Extensibility.Shell();
```

### 2. ShowPromptAsync 方法

这是显示用户提示的核心方法，支持多种提示类型。

```csharp
bool result = await shell.ShowPromptAsync(
    "提示消息",
    PromptOptions.OKCancel,
    cancellationToken);
```

## 知识点详解

### 1. 确认提示框 (OK/Cancel)

最常用的提示类型，让用户确认或取消操作。

```csharp
if (!await shell.ShowPromptAsync(
    "确定要继续执行此操作吗？",
    PromptOptions.OKCancel,
    cancellationToken))
{
    return; // 用户点击了取消
}

// 用户点击了确定，继续执行
```

**返回值：**
- `true` - 用户点击了"确定"
- `false` - 用户点击了"取消"或关闭了对话框

### 2. 错误确认提示

显示带有错误图标的单按钮提示框。

```csharp
await shell.ShowPromptAsync(
    "项目名称为必填项，无法继续配置流程。",
    PromptOptions.ErrorConfirm with { Title = "错误" },
    cancellationToken);
```

**PromptOptions.ErrorConfirm 特点：**
- 只有一个"确定"按钮
- 显示错误图标
- 可以自定义标题

### 3. 输入提示框

获取用户的文本输入。

```csharp
string? projectName = await shell.ShowPromptAsync(
    "请输入项目名称：",
    InputPromptOptions.Default with { Title = "项目配置" },
    cancellationToken);

if (string.IsNullOrEmpty(projectName))
{
    // 用户取消或未输入
    return;
}
```

**InputPromptOptions 配置：**
- **Title** - 对话框标题
- **DefaultText** - 默认文本
- **Icon** - 对话框图标

### 4. 带默认值的输入提示

```csharp
string? feedback = await shell.ShowPromptAsync(
    $"感谢您配置 {projectName}，您有什么反馈吗？",
    new InputPromptOptions
    {
        DefaultText = "运行正常。",
        Icon = ImageMoniker.KnownValues.Feedback,
        Title = "反馈",
    },
    cancellationToken);
```

### 5. 自定义选项提示

让用户从多个选项中选择一个。

```csharp
var themeResult = await shell.ShowPromptAsync(
    "应该使用哪个主题生成输出？",
    new PromptOptions<TokenThemeResult>
    {
        Choices =
        {
            { "Solarized 最棒", TokenThemeResult.Solarized },
            { "OneDark 最好", TokenThemeResult.OneDark },
            { "GruvBox 很酷", TokenThemeResult.GruvBox },
        },
        DismissedReturns = TokenThemeResult.None,
        DefaultChoiceIndex = 2,
    },
    cancellationToken);
```

**PromptOptions<T> 配置：**
- **Choices** - 选项字典（显示文本 -> 返回值）
- **DismissedReturns** - 用户取消时的返回值
- **DefaultChoiceIndex** - 默认选中的选项索引

### 6. 自定义标题和图标

```csharp
bool confirmConfiguration = await shell.ShowPromptAsync(
    $"所选系统 ({selectedSystem}) 可能需要额外的资源。您要继续吗？",
    PromptOptions.OKCancel with
    {
        Title = "系统配置确认",
        Icon = ImageMoniker.KnownValues.StatusSecurityWarning,
    },
    cancellationToken);
```

### 7. 设置默认按钮

将"取消"设为默认按钮（适合危险操作）。

```csharp
if (!await shell.ShowPromptAsync(
    "此操作无法撤销，确定要继续吗？",
    PromptOptions.OKCancel.WithCancelAsDefault(),
    cancellationToken))
{
    return;
}
```

## 完整示例

```csharp
[VisualStudioContribution]
internal class UserPromptCommand : Command
{
    private const string Title = "用户提示示例";

    public override CommandConfiguration CommandConfiguration => new("%UserPromptCommand.DisplayName%")
    {
        TooltipText = "%UserPromptCommand.ToolTip%",
        Placements = [CommandPlacement.KnownPlacements.ToolsMenu],
    };

    public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
    {
        var shell = this.Extensibility.Shell();

        // 步骤 1：获取项目名称
        string? projectName = await shell.ShowPromptAsync(
            "请输入要配置的项目名称：",
            InputPromptOptions.Default with { Title = Title },
            cancellationToken);

        if (string.IsNullOrEmpty(projectName))
        {
            await shell.ShowPromptAsync(
                "项目名称为必填项，退出配置流程。",
                PromptOptions.ErrorConfirm with { Title = Title },
                cancellationToken);
            return;
        }

        // 步骤 2：选择系统类型
        var systemType = await shell.ShowPromptAsync(
            "请选择系统类型：",
            new PromptOptions<string>
            {
                Choices =
                {
                    { "生产系统", "Production" },
                    { "测试系统", "Test" },
                    { "开发系统", "Development" },
                },
                DismissedReturns = "Development",
                DefaultChoiceIndex = 0,
            },
            cancellationToken);

        // 步骤 3：确认配置
        bool confirmConfiguration = await shell.ShowPromptAsync(
            $"将为项目 '{projectName}' 配置 {systemType} 环境。确定继续？",
            PromptOptions.OKCancel with
            {
                Title = Title,
                Icon = ImageMoniker.KnownValues.StatusSecurityWarning,
            },
            cancellationToken);

        if (!confirmConfiguration)
        {
            return;
        }

        // 步骤 4：获取反馈
        string? feedback = await shell.ShowPromptAsync(
            $"感谢您配置 {projectName}，您有什么反馈吗？",
            new InputPromptOptions
            {
                DefaultText = "配置成功。",
                Icon = ImageMoniker.KnownValues.Feedback,
                Title = Title,
            },
            cancellationToken);

        await shell.ShowPromptAsync(
            $"配置完成！\n项目：{projectName}\n系统：{systemType}\n反馈：{feedback}",
            PromptOptions.OK,
            cancellationToken);
    }
}
```

## 实践步骤

1. **创建命令类**
   - 创建继承自 `Command` 的类
   - 添加 `[VisualStudioContribution]` 特性

2. **实现简单提示**
   - 使用 `PromptOptions.OK` 显示信息提示
   - 使用 `PromptOptions.OKCancel` 获取确认

3. **添加输入功能**
   - 使用 `InputPromptOptions` 获取用户输入
   - 验证输入是否为空

4. **自定义提示外观**
   - 设置自定义标题
   - 添加适当的图标
   - 配置默认按钮

5. **创建选项提示**
   - 使用 `PromptOptions<T>` 提供多个选项
   - 处理用户选择结果

## 常见 PromptOptions

| 选项 | 说明 | 按钮 |
|------|------|------|
| `PromptOptions.OK` | 信息提示 | 确定 |
| `PromptOptions.OKCancel` | 确认提示 | 确定、取消 |
| `PromptOptions.ErrorConfirm` | 错误提示 | 确定 |
| `InputPromptOptions.Default` | 输入提示 | 确定、取消 |

## 常用图标

```csharp
ImageMoniker.KnownValues.Feedback          // 反馈图标
ImageMoniker.KnownValues.StatusSecurityWarning  // 警告图标
ImageMoniker.KnownValues.StatusInformation // 信息图标
ImageMoniker.KnownValues.StatusError       // 错误图标
```

## 常见问题

### Q: 如何判断用户是否取消了操作？
A: 对于确认提示，返回 `false` 表示取消；对于输入提示，返回 `null` 或空字符串表示取消。

### Q: 可以创建多按钮的自定义提示吗？
A: 可以使用 `PromptOptions<T>` 创建多选项提示，每个选项相当于一个按钮。

### Q: 提示框支持多行文本吗？
A: 支持，在消息字符串中使用 `\n` 即可换行。

## 进阶主题

- 创建完全自定义的对话框（Task 11）
- 使用进度报告功能
- 异步提示的生命周期管理

## 参考资料

- 源代码示例：`UserPromptSample`
- llm.md 相关代码片段：
  - 第 13-33 行：错误确认提示
  - 第 457-479 行：自定义选项提示
  - 第 560-572 行：OK/Cancel 提示
  - 第 855-866 行：输入提示

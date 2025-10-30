# 任务 15：文档选择器和激活约束

## 学习目标

掌握如何精确控制扩展功能的应用范围和命令的可见性/启用条件。

## 核心概念

### 1. 使用 DocumentFilter.FromGlobPattern 匹配文件模式

**要实现什么**：使用 glob 模式（如 `**/*.cs`、`**/tests/*.cs`）匹配文件路径

**结果**：可以精确控制扩展应用于哪些文件

### 2. 使用 DocumentFilter.FromDocumentType 匹配文档类型

**要实现什么**：按文档类型（如 `"csharp"`、`"vs-markdown"`）匹配

**结果**：适用于特定语言的扩展功能

### 3. 配置 TextViewExtensionConfiguration.AppliesTo

**要实现什么**：设置 `AppliesTo` 属性为 `DocumentFilter` 列表

**结果**：限定 Tagger、CodeLens 等文本视图扩展的应用范围

### 4. 使用 VisibleWhen 控制命令可见性

**要实现什么**：在 `CommandConfiguration` 中设置 `VisibleWhen` 激活约束

**结果**：命令只在满足条件时出现在菜单中

### 5. 使用 EnabledWhen 控制命令启用状态

**要实现什么**：设置 `EnabledWhen` 约束

**结果**：命令可见但只在满足条件时可点击（非灰色状态）

### 6. 使用 ActivationConstraint.SolutionState 检查解决方案状态

**要实现什么**：检查解决方案状态（如 `SolutionState.FullyLoaded`）

**结果**：确保命令只在解决方案加载完成后可用

### 7. 使用 ActivationConstraint.ClientContext 检查编辑器状态

**要实现什么**：检查客户端上下文（如当前编辑器的内容类型）

**结果**：例如限定命令只在编辑 C# 文件时启用

## 知识点详解

### 1. Glob 模式匹配

```csharp
public TextViewExtensionConfiguration TextViewExtensionConfiguration => new()
{
    AppliesTo =
    [
        // 匹配所有 C# 文件
        DocumentFilter.FromGlobPattern("**/*.cs", relativePath: false),

        // 仅匹配 tests 文件夹中的 C# 文件
        DocumentFilter.FromGlobPattern("**/tests/*.cs", relativePath: false),

        // 匹配项目相对路径
        DocumentFilter.FromGlobPattern("src/**/*.cs", relativePath: true),
    ],
};
```

**Glob 模式语法：**
- `*` - 匹配任意字符（不包括路径分隔符）
- `**` - 匹配任意目录层级
- `?` - 匹配单个字符
- `[abc]` - 匹配方括号中的任一字符

### 2. 按文档类型匹配

```csharp
AppliesTo =
[
    DocumentFilter.FromDocumentType("csharp"),
    DocumentFilter.FromDocumentType("vs-markdown"),
    DocumentFilter.FromDocumentType(DocumentType.KnownValues.Code),
]
```

### 3. 命令激活约束

```csharp
public override CommandConfiguration CommandConfiguration => new("%MyCommand.DisplayName%")
{
    // 仅在有活动编辑器时可见
    VisibleWhen = ActivationConstraint.ClientContext(
        ClientContextKey.Shell.ActiveEditorContentType, ".+"),

    // 仅在编辑 C# 文件时启用
    EnabledWhen = ActivationConstraint.ClientContext(
        ClientContextKey.Shell.ActiveEditorContentType, "csharp"),
};
```

### 4. 解决方案状态约束

```csharp
public override CommandConfiguration CommandConfiguration => new("%MyCommand.DisplayName%")
{
    // 仅在解决方案完全加载后可见
    VisibleWhen = ActivationConstraint.SolutionState(SolutionState.FullyLoaded),

    // 或：仅在有解决方案时可见
    VisibleWhen = ActivationConstraint.SolutionState(SolutionState.Exists),
};
```

### 5. 组合多个约束

```csharp
public override CommandConfiguration CommandConfiguration => new("%MyCommand.DisplayName%")
{
    // 解决方案已加载 AND 编辑器打开 C# 文件
    EnabledWhen = ActivationConstraint.And(
        ActivationConstraint.SolutionState(SolutionState.FullyLoaded),
        ActivationConstraint.ClientContext(
            ClientContextKey.Shell.ActiveEditorContentType, "csharp")),
};
```

## 完整示例：特定文件的扩展

### 仅应用于测试文件的 Tagger

```csharp
[VisualStudioContribution]
internal class TestFileTaggerProvider : ExtensionPart, ITextViewTaggerProvider<TextMarkerTag>
{
    public TextViewExtensionConfiguration TextViewExtensionConfiguration => new()
    {
        AppliesTo =
        [
            // 仅应用于 tests 文件夹中的 C# 文件
            DocumentFilter.FromGlobPattern("**/tests/*.cs", relativePath: false),
        ],
    };

    public Task<TextViewTagger<TextMarkerTag>> CreateTaggerAsync(
        ITextViewSnapshot textView,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<TextViewTagger<TextMarkerTag>>(
            new TestFileTagger(this, textView.Document.Uri));
    }
}
```

### 智能启用的命令

```csharp
[VisualStudioContribution]
internal class RunTestCommand : Command
{
    public override CommandConfiguration CommandConfiguration => new("%RunTest.DisplayName%")
    {
        Placements = [CommandPlacement.KnownPlacements.ExtensionsMenu],

        // 仅在解决方案完全加载时可见
        VisibleWhen = ActivationConstraint.SolutionState(SolutionState.FullyLoaded),

        // 仅在编辑 C# 测试文件时启用
        EnabledWhen = ActivationConstraint.And(
            ActivationConstraint.ClientContext(
                ClientContextKey.Shell.ActiveEditorContentType, "csharp"),
            ActivationConstraint.ClientContext(
                ClientContextKey.Shell.ActiveEditorFileName, ".*Test\\.cs$")),
    };

    public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken ct)
    {
        // 运行测试...
    }
}
```

## ClientContext 键

### Shell 相关
- `ClientContextKey.Shell.ActiveEditorContentType` - 编辑器内容类型
- `ClientContextKey.Shell.ActiveEditorFileName` - 当前文件名
- `ClientContextKey.Shell.ActiveEditorFullPath` - 当前文件完整路径

### 示例

```csharp
// 仅对 .config 文件启用
EnabledWhen = ActivationConstraint.ClientContext(
    ClientContextKey.Shell.ActiveEditorFileName, ".*\\.config$")

// 仅对项目根目录的文件启用
EnabledWhen = ActivationConstraint.ClientContext(
    ClientContextKey.Shell.ActiveEditorFullPath, ".*\\\\[^\\\\]+$")
```

## SolutionState 枚举

- `SolutionState.Exists` - 有解决方案打开
- `SolutionState.FullyLoaded` - 解决方案完全加载
- `SolutionState.Empty` - 没有解决方案

## 约束组合

```csharp
// AND 组合
ActivationConstraint.And(constraint1, constraint2)

// OR 组合
ActivationConstraint.Or(constraint1, constraint2)

// NOT 否定
ActivationConstraint.Not(constraint1)
```

## 常见问题

### Q: VisibleWhen 和 EnabledWhen 有什么区别？
A: `VisibleWhen` 控制命令是否出现在菜单，`EnabledWhen` 控制命令是否可点击。

### Q: 如何调试激活约束？
A: 使用 VS 日志查看约束评估结果，或临时移除约束测试。

### Q: Glob 模式区分大小写吗？
A: 在 Windows 上不区分，在 Linux/Mac 上区分。

### Q: 可以自定义 ClientContext 键吗？
A: 不可以，只能使用预定义的键。

## 参考资料

- llm.md 相关代码片段：
  - 第 1036-1047 行：配置命令属性和约束
  - 第 1442-1457 行：配置文本视图扩展
  - 第 1870-1884 行：配置 Markdown 文件监听
  - 第 1914-1929 行：配置测试文件选择器

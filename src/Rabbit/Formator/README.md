# Rabbit Formator

代码格式化功能模块，支持多种语言的格式化。

## 架构设计

每种语言的格式化工具独立维护，避免过度抽象。

```
Formator/
├── Xaml/               # XAML 格式化
│   ├── XamlStylerOptions.cs
│   ├── XamlFormatService.cs
│   └── XamlFormatorCommand.cs
├── Csharp/             # C# 格式化
│   ├── CSharpierOptions.cs
│   ├── CSharpFormatService.cs
│   └── CSharpFormatorCommand.cs
└── README.md
```

## 设计原则

### 职责分离

- **Service 层**：纯格式化逻辑，调用命令行工具
- **Command 层**：处理 VS 扩展集成（文件路径、工作目录、错误处理等）

### 独立性

每个格式化工具独立实现，互不依赖：
- 不使用共享接口
- 不使用工厂模式
- 每个工具根据自己的特性选择最佳实现方式

## XAML 格式化

### 工具：xstyler

**特点**：
- 需要文件路径才能工作
- 使用临时文件方案

**使用方式**：
```bash
xstyler -f "file.xaml" --write-to-stdout
```

### 实现说明

- 创建临时文件写入内容
- 调用 xstyler 处理临时文件
- 从 stdout 读取格式化结果
- 清理临时文件

## C# 格式化

### 工具：csharpier

**特点**：
- 支持 stdin/stdout
- 需要项目上下文（查找 .csharpierrc、.editorconfig）
- 使用 `--stdin-path` 参数告知文件路径

**使用方式**：
```bash
echo "code" | csharpier format --write-stdout --stdin-path "File.cs"
```

### 实现说明

**Service 层**：
```csharp
FormatAsync(
    string content,          // 代码内容
    string workingDirectory, // 工作目录（查找配置文件）
    string? stdinPath,       // 文件路径（解析 ignore 规则）
    CSharpierOptions? config // 配置选项
)
```

**Command 层**：
- 从 Visual Studio 获取当前文件路径
- 提取文件所在目录作为工作目录
- 调用 Service 进行格式化
- 将结果替换回编辑器

### 工作原理

1. **WorkingDirectory**：csharpier 从此目录向上查找配置文件
2. **--stdin-path**：告诉 csharpier 文件路径，用于：
   - 解析项目配置（.csharpierrc）
   - 应用 ignore 规则（.csharpierignore）
   - 确定文件类型

## 添加新的格式化工具

1. 创建新文件夹：`Formator/NewLanguage/`
2. 创建配置类：`NewLanguageOptions.cs`
3. 创建服务类：`NewLanguageFormatService.cs`
4. 创建命令类：`NewLanguageFormatorCommand.cs`
5. 根据工具特性选择实现方式（临时文件 vs stdin/stdout）

## 注意事项

### C# 格式化

- **不要**在临时目录创建文件（会导致找不到项目配置）
- **必须**设置正确的 WorkingDirectory
- **建议**传递 stdin-path 参数以获得完整功能

### XAML 格式化

- **必须**创建临时文件（xstyler 要求）
- **不需要**项目上下文
- 清理临时文件时忽略错误

## 相关文档

- [CSharpier CLI](https://csharpier.com/docs/CLI)
- [XamlStyler](https://github.com/Xavalon/XamlStyler)
- [belav/csharpier](https://github.com/belav/csharpier)
- [Xavalon/XamlStyler](https://github.com/Xavalon/XamlStyler)

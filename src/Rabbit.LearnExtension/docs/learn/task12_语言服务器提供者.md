# 任务 12：语言服务器提供者

## 学习目标

掌握如何集成外部语言服务器（LSP - Language Server Protocol），为自定义语言提供智能感知等功能。

## 核心概念

### 1. 创建继承自 LanguageServerProvider 的类

**要实现什么**：定义语言服务器提供者类，继承 `LanguageServerProvider`

**结果**：这是集成 LSP（Language Server Protocol）的入口

### 2. 配置 LanguageServerProviderConfiguration

**要实现什么**：重写配置属性，设置显示名称和适用的文档类型

**结果**：指定这个语言服务器为哪些文件类型提供服务

### 3. 定义文档类型

**要实现什么**：使用 `DocumentFilter` 指定文档类型（如 `.rs` 文件）

**结果**：只有匹配的文件会使用这个语言服务器

### 4. 实现 CreateServerConnectionAsync 启动语言服务器进程

**要实现什么**：创建并启动外部语言服务器进程（如 rust-analyzer.exe）

**结果**：配置进程的启动信息和工作目录

### 5. 创建 DuplexPipe 连接进程流

**要实现什么**：使用进程的标准输入输出流创建 `DuplexPipe`

**结果**：这是 VS 和语言服务器通信的通道（使用 LSP 协议）

### 6. 处理服务器初始化结果

**要实现什么**：重写 `OnServerInitializationResultAsync` 检查初始化状态

**结果**：可以记录日志或根据结果决定后续行为

### 7. 处理初始化失败场景

**要实现什么**：当初始化失败时设置 `Enabled = false` 禁用提供者

**结果**：防止 VS 反复尝试启动失败的服务器

## 知识点详解

### 1. 定义自定义文档类型

```csharp
[VisualStudioContribution]
internal static DocumentType RustDocumentType => new("rust")
{
    FileExtensions = new[] { ".rs" },
    BaseDocumentType = DocumentType.KnownValues.Code,
};
```

### 2. 创建语言服务器提供者

```csharp
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.LanguageServer;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

[VisualStudioContribution]
internal class RustLanguageServerProvider : LanguageServerProvider
{
    public RustLanguageServerProvider(VisualStudioExtensibility extensibility)
        : base(extensibility)
    {
    }

    public override LanguageServerProviderConfiguration LanguageServerProviderConfiguration => new(
        "%RustLsp.DisplayName%",
        new[]
        {
            DocumentFilter.FromDocumentType(RustDocumentType)
        });

    public override Task<IDuplexPipe?> CreateServerConnectionAsync(CancellationToken cancellationToken)
    {
        // 获取语言服务器可执行文件路径
        string serverPath = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
            "rust-analyzer.exe");

        if (!File.Exists(serverPath))
        {
            return Task.FromResult<IDuplexPipe?>(null);
        }

        // 配置进程启动信息
        var startInfo = new ProcessStartInfo
        {
            FileName = serverPath,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        // 启动进程
        var process = new Process { StartInfo = startInfo };

        if (process.Start())
        {
            // 创建双工管道连接进程流
            return Task.FromResult<IDuplexPipe?>(new DuplexPipe(
                PipeReader.Create(process.StandardOutput.BaseStream),
                PipeWriter.Create(process.StandardInput.BaseStream)));
        }

        return Task.FromResult<IDuplexPipe?>(null);
    }

    public override Task OnServerInitializationResultAsync(
        ServerInitializationResult serverInitializationResult,
        LanguageServerInitializationFailureInfo? initializationFailureInfo,
        CancellationToken cancellationToken)
    {
        if (serverInitializationResult == ServerInitializationResult.Failed)
        {
            // 记录失败信息并禁用提供者
            // this.Logger.LogError("Language server initialization failed");
            this.Enabled = false;
        }

        return base.OnServerInitializationResultAsync(
            serverInitializationResult,
            initializationFailureInfo,
            cancellationToken);
    }
}
```

## LSP 协议简介

语言服务器协议（LSP）是微软开发的协议，用于编辑器和语言服务器之间的通信。

**主要功能：**
- 代码补全（IntelliSense）
- 转到定义
- 查找引用
- 重命名符号
- 诊断错误和警告
- 代码格式化

## 打包语言服务器

### 1. 包含服务器可执行文件

在 `.csproj` 中添加：

```xml
<ItemGroup>
  <Content Include="LanguageServer\rust-analyzer.exe">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </Content>
</ItemGroup>
```

### 2. 配置为 VSIX 内容

```xml
<ItemGroup>
  <Content Include="LanguageServer\**\*">
    <IncludeInVSIX>true</IncludeInVSIX>
  </Content>
</ItemGroup>
```

## 字符串资源

在 `string-resources.json` 中定义：

```json
{
    "RustLsp.DisplayName": "Rust Language Server"
}
```

## 测试步骤

1. **准备语言服务器**
   - 下载对应语言的 LSP 服务器（如 rust-analyzer）
   - 放到项目的 LanguageServer 文件夹

2. **配置项目**
   - 定义文档类型
   - 创建提供者类
   - 配置文件包含

3. **测试功能**
   - 打开匹配的文件类型
   - 验证智能感知是否工作
   - 检查错误提示

## 常用语言服务器

- **Rust**: rust-analyzer
- **Python**: pyright, pylsp
- **JavaScript/TypeScript**: typescript-language-server
- **Go**: gopls
- **C/C++**: clangd

## 常见问题

### Q: 如何调试语言服务器？
A: 可以在 `CreateServerConnectionAsync` 中添加日志，查看进程启动情况。

### Q: 服务器进程何时终止？
A: 当文本视图关闭或 VS 退出时，VS 会自动终止服务器进程。

### Q: 如何传递启动参数？
A: 在 `ProcessStartInfo.Arguments` 中设置命令行参数。

### Q: 支持哪些 LSP 版本？
A: Visual Studio 支持 LSP 3.x 版本。

## 参考资料

- LSP 规范: https://microsoft.github.io/language-server-protocol/
- llm.md 相关代码片段：
  - 第 390-418 行：创建语言服务器连接
  - 第 422-430 行：设置查询 API
  - 第 576-586 行：定义提供者类
  - 第 742-759 行：处理初始化结果
  - 第 843-851 行：定义本地化字符串
  - 第 901-914 行：配置语言服务器

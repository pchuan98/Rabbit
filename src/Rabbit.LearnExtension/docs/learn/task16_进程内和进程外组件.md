# 任务 16：进程内和进程外组件

## 学习目标

掌握如何创建进程内和进程外组件，以及如何通过 Brokered Service 在它们之间通信。

## 核心概念

### 1. 理解 In-Proc 和 Out-of-Proc 组件的区别

**要实现什么**：In-Proc 组件运行在 VS 主进程内（devenv.exe），Out-of-Proc 组件运行在独立的 ServiceHub 进程

**结果**：Out-of-Proc 更安全但需要跨进程通信

### 2. 创建 Brokered Service 接口

**要实现什么**：定义服务接口（如 `IMyService`），使用 `ServiceJsonRpcDescriptor` 配置 JSON-RPC 服务描述符

**结果**：接口方法返回 `Task` 支持异步

### 3. 在进程内提供服务（ProfferBrokeredService）

**要实现什么**：在 `InitializeServices` 中调用 `serviceCollection.ProfferBrokeredService()`

**结果**：注册服务实现，让其他组件可以发现和使用

### 4. 在进程外消费服务（GetProxyAsync）

**要实现什么**：通过 `ServiceBroker.GetProxyAsync<T>()` 获取服务代理

**结果**：使用代理调用服务方法，Service Broker 处理跨进程通信

### 5. 配置项目引用以打包进 VSIX

**要实现什么**：在主项目中添加 Out-of-Proc 项目引用，设置 `ReferenceOutputAssembly=false` 和 `IncludeInVSIX=true`

**结果**：确保两个组件都打包进扩展

### 6. 处理服务代理的生命周期

**要实现什么**：使用 try-finally 模式，在 finally 块中 `(proxy as IDisposable)?.Dispose()`

**结果**：及时释放代理连接资源

### 7. 在组件之间共享接口定义

**要实现什么**：将接口定义放在共享文件或单独项目中

**结果**：In-Proc 和 Out-of-Proc 项目都引用相同的接口，保证契约一致

## 架构说明

### In-Proc 组件（net8.0-windows）
- 运行在 VS 主进程
- 可以直接访问 VS API
- 崩溃会导致 VS 崩溃
- 性能更好（无跨进程开销）

### Out-of-Proc 组件（net8.0-windows）
- 运行在独立的 ServiceHub 进程
- 隔离性好，崩溃不影响 VS
- 适合耗时或不稳定的操作
- 通过 Brokered Service 通信

## 完整示例

### 1. 定义共享接口

```csharp
using Microsoft.ServiceHub.Framework;
using System.Threading;
using System.Threading.Tasks;

// 服务接口
public interface ICalculatorService
{
    Task<int> AddAsync(int a, int b, CancellationToken cancellationToken);
    Task<int> MultiplyAsync(int a, int b, CancellationToken cancellationToken);
}

// 服务配置
public static class CalculatorServiceConfiguration
{
    public static readonly ServiceRpcDescriptor Descriptor = new ServiceJsonRpcDescriptor(
        new ServiceMoniker("MyExtension.CalculatorService", new Version(1, 0)),
        clientInterface: null,
        ServiceJsonRpcDescriptor.Formatters.UTF8,
        ServiceJsonRpcDescriptor.MessageDelimiters.HttpLikeHeaders,
        multiplexingStreamOptions: null);
}
```

### 2. Out-of-Proc 实现（ServiceHub 进程）

创建 `OutOfProcComponent` 项目（net8.0-windows）：

```csharp
using Microsoft.VisualStudio.Extensibility;

[VisualStudioContribution]
public class CalculatorExtension : Extension
{
    public CalculatorExtension(ExtensionCore extensibilityObject)
        : base(extensibilityObject)
    {
    }

    protected override void InitializeServices(IServiceCollection serviceCollection)
    {
        base.InitializeServices(serviceCollection);

        // 注册 Brokered Service
        serviceCollection.ProfferBrokeredService(
            CalculatorServiceConfiguration.Descriptor,
            factory: (moniker, options, serviceBroker, ct) =>
                new ValueTask<object?>(new CalculatorService()));
    }
}

// 服务实现
internal class CalculatorService : ICalculatorService
{
    public Task<int> AddAsync(int a, int b, CancellationToken cancellationToken)
    {
        return Task.FromResult(a + b);
    }

    public Task<int> MultiplyAsync(int a, int b, CancellationToken cancellationToken)
    {
        return Task.FromResult(a * b);
    }
}
```

### 3. In-Proc 消费（VS 主进程）

在主扩展项目中：

```csharp
[VisualStudioContribution]
internal class UseCalculatorCommand : Command
{
    public override CommandConfiguration CommandConfiguration => new("%UseCalculator.DisplayName%")
    {
        Placements = [CommandPlacement.KnownPlacements.ToolsMenu],
    };

    public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
    {
        ICalculatorService? calculator = null;

        try
        {
            // 获取服务代理
            calculator = await this.Extensibility.ServiceBroker.GetProxyAsync<ICalculatorService>(
                CalculatorServiceConfiguration.Descriptor,
                cancellationToken);

            if (calculator == null)
            {
                await this.Extensibility.Shell().ShowPromptAsync(
                    "无法连接到计算服务",
                    PromptOptions.OK,
                    cancellationToken);
                return;
            }

            // 调用服务方法
            int sum = await calculator.AddAsync(10, 20, cancellationToken);
            int product = await calculator.MultiplyAsync(5, 6, cancellationToken);

            await this.Extensibility.Shell().ShowPromptAsync(
                $"10 + 20 = {sum}\n5 × 6 = {product}",
                PromptOptions.OK,
                cancellationToken);
        }
        finally
        {
            // 释放代理
            (calculator as IDisposable)?.Dispose();
        }
    }
}
```

### 4. 共享接口文件

在主项目的 `.csproj` 中包含接口：

```xml
<ItemGroup>
  <!-- 包含共享接口文件 -->
  <Compile Include="..\Shared\ICalculatorService.cs" Link="Services\ICalculatorService.cs" />
</ItemGroup>
```

### 5. 配置项目引用

在主项目的 `.csproj` 中：

```xml
<ItemGroup>
  <!-- 引用 Out-of-Proc 项目但不引用程序集 -->
  <ProjectReference Include="..\OutOfProcComponent\OutOfProcComponent.csproj">
    <ReferenceOutputAssembly>false</ReferenceOutputAssembly>
    <SetTargetFramework>TargetFramework=net8.0-windows</SetTargetFramework>
    <IncludeInVSIX>true</IncludeInVSIX>
    <IncludeOutputGroupsInVSIX>ExtensionFilesOutputGroup</IncludeOutputGroupsInVSIX>
  </ProjectReference>
</ItemGroup>
```

## Brokered Service 描述符

### ServiceRpcDescriptor 属性

```csharp
new ServiceJsonRpcDescriptor(
    serviceMoniker: new ServiceMoniker("服务名称", new Version(1, 0)),
    clientInterface: null,  // 或指定客户端接口
    formatter: ServiceJsonRpcDescriptor.Formatters.UTF8,
    messageDelimiters: ServiceJsonRpcDescriptor.MessageDelimiters.HttpLikeHeaders,
    multiplexingStreamOptions: null);
```

### 版本管理

服务名称相同但版本不同的服务被视为不同的服务。这允许服务演进而不破坏现有客户端。

## 生命周期管理

### 服务实例

- 每次 `GetProxyAsync` 调用可能创建新的服务实例
- 服务实现应该是无状态的或线程安全的

### 代理释放

```csharp
try
{
    var service = await ServiceBroker.GetProxyAsync<IMyService>(...);
    await service.DoWorkAsync();
}
finally
{
    (service as IDisposable)?.Dispose();
}
```

## 常见问题

### Q: 什么时候使用 Out-of-Proc 组件？
A: 当操作耗时、可能崩溃、或需要隔离时使用。

### Q: 如何调试 Out-of-Proc 组件？
A: 在 VS 中附加到 ServiceHub 进程，或使用 VS 实验实例调试。

### Q: 可以传递复杂对象吗？
A: 可以，但对象必须是可序列化的（JSON-RPC 序列化）。

### Q: 如何处理服务不可用？
A: `GetProxyAsync` 可能返回 null，需要检查并处理。

### Q: 多个客户端可以同时使用服务吗？
A: 可以，每个客户端会获得独立的代理。

## 最佳实践

1. **接口设计**
   - 所有方法返回 `Task` 支持异步
   - 接受 `CancellationToken` 参数
   - 参数和返回值类型要可序列化

2. **错误处理**
   - 检查代理是否为 null
   - 捕获 RPC 异常
   - 在 finally 中释放代理

3. **性能考虑**
   - 避免频繁跨进程调用
   - 批量处理数据
   - 考虑缓存

4. **版本管理**
   - 使用语义版本号
   - 向后兼容的更改增加次版本号
   - 破坏性更改增加主版本号

## 参考资料

- llm.md 相关代码片段：
  - 第 1565-1579 行：注入 VS SDK 服务
  - 第 1616-1625 行：包含共享接口
  - 第 1794-1808 行：提供 In-Proc 服务
  - 第 1812-1825 行：引用 Out-of-Proc 项目
  - 第 1888-1910 行：消费 Out-of-Proc 服务

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Extensibility;

namespace Rabbit;

/// <summary>
/// Extension entrypoint for the VisualStudio.Extensibility extension.
/// </summary>
[VisualStudioContribution]
internal class ExtensionEntrypoint : Extension
{
    /// <inheritdoc/>
    public override ExtensionConfiguration ExtensionConfiguration => new()
    {
        Metadata = new(
                id: "Rabbit.bd3f4a73-420f-4e44-be06-d883acf8d29c",
                version: this.ExtensionAssemblyVersion,
                publisherName: "pchuan",
                displayName: "Rabbit",
                description: "This a simple extension of VS2026"),
    };

    /// <inheritdoc />
    protected override void InitializeServices(IServiceCollection serviceCollection)
    {
        base.InitializeServices(serviceCollection);

        // 注册服务 - 推荐使用 Singleton 生命周期
        // Singleton: 服务在整个扩展生命周期中只创建一次，所有组件共享同一个实例
        serviceCollection.AddSingleton<LogService>();

        // 生命周期说明：
        // - AddSingleton: 整个扩展生命周期只创建一次（推荐用于大多数 VS 扩展服务）
        // - AddTransient: 每次请求都创建新实例
        // - AddScoped: 作用域生命周期（绑定到组件生命周期）
    }
}

# Visual Studio 插件开发学习任务清单

## 第一阶段：基础入门

- [x] 任务 1：创建第一个简单命令

**Stories：**
- 理解 `[VisualStudioContribution]` 特性的作用
- 创建一个继承自 `Command` 的类
- 配置命令的显示名称和图标
- 实现 `ExecuteCommandAsync` 方法
- 在工具菜单中显示命令
- 测试命令的执行

- [x] 任务 2：使用用户提示和对话框

**Stories：**
- 使用 `Shell().ShowPromptAsync` 显示确认对话框
- 实现 OK/Cancel 提示
- 显示带图标和自定义标题的提示
- 创建输入提示框获取用户输入
- 显示自定义选项提示
- 处理用户取消操作

- [x] 任务 3：创建工具窗口

**Stories：**
- 创建继承自 `ToolWindow` 的类
- 配置工具窗口的标题和放置位置
- 创建数据模型类（继承 `NotifyPropertyChangedObject`）
- 创建 XAML 数据模板定义 UI
- 实现 `InitializeAsync` 和 `GetContentAsync`
- 创建命令来显示工具窗口
- 实现数据绑定和属性更改通知

## 第二阶段：编辑器集成

### - [ ] 任务 4：文本编辑器操作

**Stories：**
- 获取当前活动文本视图（`GetActiveTextViewAsync`）
  > 通过 `context.GetActiveTextViewAsync()` 获取当前编辑器。返回的 `textView` 对象包含了文档内容、光标位置、选择区域等所有信息

- 读取文档内容和选择区域
  > 从 `textView.Document` 读取文本内容，从 `textView.Selection` 获取用户选中的区域。可以得到选中文本的起止位置和内容

- 使用 `Editor().EditAsync` 编辑文档
  > 所有文档修改必须在 `Extensibility.Editor().EditAsync()` 的批处理（batch）中进行。这样可以保证编辑操作的原子性和撤销/重做功能正常

- 替换选定文本

- 插入新文本到光标位置

- 处理多个选择区域
  > VS 支持多光标编辑，`textView.Selections` 是一个列表。需要遍历所有选择区域分别处理

- 实现撤销/重做兼容的编辑
  > 在 `EditAsync` 批处理中的所有修改会被自动包装为一个撤销单元。用户按 Ctrl+Z 可以一次性撤销整个批处理的操作

### - [ ] 任务 5：实现 CodeLens 提供者

**Stories：**
- 理解 `ICodeLensProvider` 接口
  > 实现 `ICodeLensProvider` 接口，让你的类能够为代码元素（方法、类等）提供 CodeLens。CodeLens 是代码上方显示的信息提示（如引用计数）

- 配置 `CodeLensProviderConfiguration`
  > 设置 CodeLens 提供者的显示名称和优先级。优先级决定了多个 CodeLens 的显示顺序

- 实现 `TryCreateCodeLensAsync` 方法
  > 判断是否为某个代码元素创建 CodeLens。接收 `CodeElement` 参数（代表方法、类等），返回 `CodeLens` 实例或 null

- 创建可点击的 CodeLens
  > 继承 `ClickableCodeLens` 类，实现 `ExecuteAsync` 方法处理点击事件。用户点击 CodeLens 时会触发你的逻辑

- 实现 `GetLabelAsync` 提供标签文本
  > 返回 `CodeLensLabel` 对象，设置显示的文本和工具提示。这是 CodeLens 显示在代码上方的文字内容

- 实现 `GetVisualizationAsync` 显示自定义 UI
  > 返回自定义的 `RemoteUserControl`，点击 CodeLens 后会显示这个 UI。可以展示复杂的信息面板

- 处理 CodeLens 刷新和失效
  > 调用 `Invalidate()` 方法触发 CodeLens 重新计算和刷新。当数据变化时使用

### - [ ] 任务 6：实现文本标记器（Tagger）

**Stories：**
- 创建 `ITextViewTaggerProvider<T>` 实现
  > 实现 `ITextViewTaggerProvider<T>` 接口，为文本视图提供标记器。T 是标记类型（如 `TextMarkerTag` 用于高亮）

- 配置 `TextViewExtensionConfiguration` 指定适用的文档类型
  > 通过 `AppliesTo` 属性设置 Tagger 应用的文件类型。使用 `DocumentFilter` 指定文件扩展名或文档类型

- 实现 `CreateTaggerAsync` 方法
  > 创建并返回你的 Tagger 实例。每个打开的文本视图会调用一次这个方法

- 创建 `TextViewTagger<T>` 子类
  > 继承 `TextViewTagger<T>`，实现具体的标记逻辑。这是标记器的核心类

- 实现 `CreateTagsAsync` 处理文档范围
  > 为指定的文档范围创建标记。接收 `ITextDocumentSnapshot` 和要处理的文本范围列表，返回带位置信息的标记列表

- 使用 `TextMarkerTag` 高亮文本
  > 创建 `TextMarkerTag` 实例指定高亮样式（如 FindHighlight）。通过 `TaggedTrackingTextRange` 关联标记和文本位置

- 处理文本视图变化事件
  > 实现 `TextViewChangedAsync` 方法响应文本编辑。获取编辑范围，更新受影响区域的标记

## 第三阶段：项目系统交互

### - [ ] 任务 7：使用项目查询 API

**Stories：**
- 获取 `WorkspacesExtensibility` 对象
  > 通过 `Extensibility.Workspaces()` 获取工作区扩展对象。这是访问项目系统查询 API 的入口点

- 使用 `QueryProjectsAsync` 查询项目信息
  > 调用 `QueryProjectsAsync` 并传入查询表达式。返回异步枚举器，可遍历查询结果

- 使用 `With` 选择需要的属性
  > 通过 `With(p => p.Name)` 等语法选择要查询的项目属性。只会加载你指定的属性，提高性能

- 查询项目文件和配置信息
  > 使用 `With(p => p.Files.With(f => f.Path))` 查询项目中的文件。可以链式查询嵌套属性（如配置的输出路径）

- 使用 `Where` 过滤项目
  > 通过 `Where(p => p.Name == "MyProject")` 过滤符合条件的项目。减少返回的结果数量

- 查询解决方案级别信息
  > 使用 `querySpace.Solutions` 查询解决方案信息（如解决方案名称、配置等）

### - [ ] 任务 8：修改项目系统

**Stories：**
- 使用 `UpdateProjectsAsync` 修改项目
  > 调用 `UpdateProjectsAsync` 传入选择器和修改操作。第一个参数选择要修改的项目，第二个参数定义修改动作

- 创建新文件到项目
  > 使用 `project.AddFile("filename.txt")` 在项目中创建新文件。文件会自动添加到项目并保存

- 从项目中删除文件
  > 使用 `project.DeleteFile("filepath")` 从项目中删除文件。会同时从磁盘和项目文件中移除

- 重命名项目
  > 使用 `AsUpdatable().Rename("NewName")` 重命名项目。项目文件和引用会自动更新

- 添加/删除解决方案配置
  > 使用 `UpdateSolutionAsync` 调用 `AddSolutionConfiguration` 或 `DeleteSolutionConfiguration`。管理 Debug/Release 等配置

- 设置启动项目
  > 使用 `solution.SetStartupProjects(projectPath)` 设置解决方案的启动项目。可以设置多个启动项目

- 构建项目和解决方案
  > 调用 `project.BuildAsync()` 或 `solution.BuildAsync()` 触发构建。等待构建完成并获取结果

## 第四阶段：高级功能

### - [ ] 任务 9：扩展设置管理

**Stories：**
- 定义 `SettingCategory` 和 `Setting` 类
  > 创建静态属性定义设置分类和具体设置项。使用 `Setting.Boolean`、`Setting.String` 等类型定义不同类型的设置

- 使用 `[VisualStudioContribution]` 注册设置
  > 在设置类和分类上添加 `[VisualStudioContribution]`。VS 会自动在工具-选项对话框中显示这些设置

- 实现设置观察者（Observer）模式
  > 设置 `GenerateObserverClass = true`，构建时自动生成观察者类。注入观察者类到你的组件中监听设置变化

- 订阅设置更改事件
  > 订阅观察者的 `Changed` 事件。用户修改设置时会触发此事件，参数包含新的设置快照

- 读取设置当前值
  > 通过 `settingsSnapshot.YourSetting.ValueOrDefault(defaultValue)` 读取设置值。提供默认值防止设置不存在时出错

- 编程方式写入设置
  > 调用 `Extensibility.Settings().WriteAsync()` 在批处理中修改设置。可同时修改多个设置

- 处理用户覆盖的设置值
  > 用户可在 `extensibility.settings.json` 中覆盖设置。读取时会自动使用覆盖后的值

### - [ ] 任务 10：文件和文件夹选择器

**Stories：**
- 使用 `ShowOpenFileDialogAsync` 选择单个文件
  > 调用 `Shell().ShowOpenFileDialogAsync()` 显示文件选择对话框。返回选中的文件路径字符串，用户取消返回 null

- 使用 `ShowOpenMultipleFilesDialogAsync` 选择多个文件
  > 允许用户选择多个文件。返回 `IReadOnlyList<string>` 包含所有选中文件的路径

- 使用 `ShowOpenFolderDialogAsync` 选择文件夹
  > 显示文件夹选择对话框。返回选中的文件夹路径，用户可以浏览并选择目录

- 使用 `ShowSaveAsFileDialogAsync` 保存文件
  > 显示保存文件对话框。返回用户指定的保存路径，可设置默认文件名和扩展名

- 配置 `FileDialogOptions`（过滤器、初始文件名等）
  > 创建 `FileDialogOptions` 设置 `InitialFileName`、`Filters`（文件类型过滤）等。控制对话框的初始状态和行为

- 处理用户取消操作
  > 所有对话框方法在用户取消时返回 null。需要检查返回值是否为 null 再继续处理

### - [ ] 任务 11：创建对话框

**Stories：**
- 创建继承自 `RemoteUserControl` 的控件类
  > 定义对话框的代码类，继承 `RemoteUserControl`。在构造函数中设置数据上下文和添加资源字典

- 设计 XAML 数据模板
  > 创建 `.xaml` 文件定义对话框的 UI 布局。使用 WPF 控件（TextBlock、Button 等）设计界面

- 添加嵌入式资源字典
  > 创建 `.xaml` 资源字典定义样式和资源。调用 `ResourceDictionaries.AddEmbeddedResource()` 加载资源

- 使用动态资源引用
  > 在 XAML 中使用 `{DynamicResource key}` 引用资源字典中的资源（如字符串、样式、颜色等）

- 使用 `Shell().ShowDialogAsync` 显示对话框
  > 创建控件实例，调用 `Shell().ShowDialogAsync(control)` 显示为模态对话框。对话框会阻塞直到用户关闭

- 实现对话框的数据绑定
  > 在数据上下文类中定义属性，使用 `{Binding PropertyName}` 在 XAML 中绑定。数据上下文需继承 `NotifyPropertyChangedObject`

- 处理对话框的确认和取消
  > 在控件中添加按钮，绑定到命令或事件处理。通过修改数据上下文传递用户输入的结果

### - [ ] 任务 12：语言服务器提供者

**Stories：**
- 创建继承自 `LanguageServerProvider` 的类
  > 定义语言服务器提供者类，继承 `LanguageServerProvider`。这是集成 LSP（Language Server Protocol）的入口

- 配置 `LanguageServerProviderConfiguration`
  > 重写配置属性，设置显示名称和适用的文档类型。指定这个语言服务器为哪些文件类型提供服务

- 定义文档类型
  > 使用 `DocumentFilter` 指定文档类型（如 `.rs` 文件）。只有匹配的文件会使用这个语言服务器

- 实现 `CreateServerConnectionAsync` 启动语言服务器进程
  > 创建并启动外部语言服务器进程（如 rust-analyzer.exe）。配置进程的启动信息和工作目录

- 创建 `DuplexPipe` 连接进程流
  > 使用进程的标准输入输出流创建 `DuplexPipe`。这是 VS 和语言服务器通信的通道（使用 LSP 协议）

- 处理服务器初始化结果
  > 重写 `OnServerInitializationResultAsync` 检查初始化状态。可以记录日志或根据结果决定后续行为

- 处理初始化失败场景
  > 当初始化失败时设置 `Enabled = false` 禁用提供者。防止 VS 反复尝试启动失败的服务器

## 第五阶段：综合实践

### - [ ] 任务 13：调试可视化器

**Stories：**
- 创建继承自 `DebuggerVisualizerProvider` 的类
  > 定义调试可视化器类，继承 `DebuggerVisualizerProvider`。用于在调试时为特定类型的对象提供自定义可视化界面

- 配置 `DebuggerVisualizerProviderConfiguration`
  > 设置可视化器的显示名称和目标类型（如 `typeof(Match)`）。指定这个可视化器用于哪种类型的对象

- 实现可视化器对象源（Object Source）
  > 创建单独的 netstandard2.0 项目，继承 `VisualizerObjectSource`。在被调试进程中运行，负责提取和序列化对象数据

- 实现 `CreateVisualizerAsync` 创建 UI
  > 返回 `RemoteUserControl` 显示可视化内容。接收 `VisualizerTarget` 参数用于请求调试对象数据

- 使用 `RequestDataAsync` 获取调试对象数据
  > 通过 `visualizerTarget.ObjectSource.RequestDataAsync<T>()` 从对象源获取数据。数据会在被调试进程中提取并传输过来

- 序列化和反序列化调试数据
  > 对象源中实现 `GetData` 方法，将对象序列化为可传输的数据。可视化器端反序列化数据用于显示

- 打包对象源 DLL 到扩展
  > 将对象源 DLL 作为 Content 包含在 VSIX 中。配置 `VisualizerObjectSourceType` 引用对象源类型

### - [ ] 任务 14：命令菜单组织

**Stories：**
- 使用 `MenuConfiguration` 创建自定义菜单
  > 定义静态 `MenuConfiguration` 属性并标记 `[VisualStudioContribution]`。创建新的菜单（如 Extensions 下的子菜单）

- 使用 `ToolbarConfiguration` 创建工具栏
  > 类似菜单配置，创建 `ToolbarConfiguration` 定义工具栏。可以是独立工具栏或工具窗口的工具栏

- 配置命令放置位置（`Placements`）
  > 在 `CommandConfiguration` 的 `Placements` 中指定命令位置。可以放在预定义位置（`KnownPlacements.ToolsMenu`）或自定义菜单

- 使用 `CommandPlacement.VsctParent` 添加到现有菜单
  > 通过 GUID 和 ID 引用 VS 内置菜单项。可以将命令添加到解决方案资源管理器右键菜单等位置

- 组织菜单子项（命令和分隔符）
  > 在菜单配置的 `Children` 中添加 `MenuChild.Command<YourCommand>()` 和 `MenuChild.Separator`。组织菜单结构

- 配置工具窗口工具栏
  > 在 `ToolWindowConfiguration` 中设置 `Toolbar` 属性。工具窗口顶部会显示配置的工具栏

- 设置命令优先级
  > 在 `CommandPlacement` 中设置 `priority` 参数。控制同一位置多个命令的显示顺序

### - [ ] 任务 15：文档选择器和激活约束

**Stories：**
- 使用 `DocumentFilter.FromGlobPattern` 匹配文件模式
  > 使用 glob 模式（如 `**/*.cs`、`**/tests/*.cs`）匹配文件路径。可以精确控制扩展应用于哪些文件

- 使用 `DocumentFilter.FromDocumentType` 匹配文档类型
  > 按文档类型（如 `"csharp"`、`"vs-markdown"`）匹配。适用于特定语言的扩展功能

- 配置 `TextViewExtensionConfiguration.AppliesTo`
  > 设置 `AppliesTo` 属性为 `DocumentFilter` 列表。限定 Tagger、CodeLens 等文本视图扩展的应用范围

- 使用 `VisibleWhen` 控制命令可见性
  > 在 `CommandConfiguration` 中设置 `VisibleWhen` 激活约束。命令只在满足条件时出现在菜单中

- 使用 `EnabledWhen` 控制命令启用状态
  > 设置 `EnabledWhen` 约束。命令可见但只在满足条件时可点击（非灰色状态）

- 使用 `ActivationConstraint.SolutionState` 检查解决方案状态
  > 检查解决方案状态（如 `SolutionState.FullyLoaded`）。确保命令只在解决方案加载完成后可用

- 使用 `ActivationConstraint.ClientContext` 检查编辑器状态
  > 检查客户端上下文（如当前编辑器的内容类型）。例如限定命令只在编辑 C# 文件时启用

### - [ ] 任务 16：进程内和进程外组件

**Stories：**
- 理解 In-Proc 和 Out-of-Proc 组件的区别
  > In-Proc 组件运行在 VS 主进程内（devenv.exe），Out-of-Proc 组件运行在独立的 ServiceHub 进程。Out-of-Proc 更安全但需要跨进程通信

- 创建 Brokered Service 接口
  > 定义服务接口（如 `IMyService`），使用 `ServiceJsonRpcDescriptor` 配置 JSON-RPC 服务描述符。接口方法返回 `Task` 支持异步

- 在进程内提供服务（`ProfferBrokeredService`）
  > 在 `InitializeServices` 中调用 `serviceCollection.ProfferBrokeredService()`。注册服务实现，让其他组件可以发现和使用

- 在进程外消费服务（`GetProxyAsync`）
  > 通过 `ServiceBroker.GetProxyAsync<T>()` 获取服务代理。使用代理调用服务方法，Service Broker 处理跨进程通信

- 配置项目引用以打包进 VSIX
  > 在主项目中添加 Out-of-Proc 项目引用，设置 `ReferenceOutputAssembly=false` 和 `IncludeInVSIX=true`。确保两个组件都打包进扩展

- 处理服务代理的生命周期
  > 使用 try-finally 模式，在 finally 块中 `(proxy as IDisposable)?.Dispose()`。及时释放代理连接资源

- 在组件之间共享接口定义
  > 将接口定义放在共享文件或单独项目中。In-Proc 和 Out-of-Proc 项目都引用相同的接口，保证契约一致

## 学习建议

1. **按顺序完成任务**：从简单到复杂，逐步掌握各个知识点
2. **动手实践**：每个任务都创建实际的代码示例
3. **参考官方示例**：查看 `llm.md` 中的代码片段
4. **测试验证**：确保每个功能都能正常工作
5. **记录笔记**：在 `learn` 文件夹中记录学习心得

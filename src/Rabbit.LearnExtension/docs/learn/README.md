# Visual Studio 插件开发学习文档

这个文件夹包含了 Visual Studio 插件开发的详细学习资料，每个文件对应 `todo.md` 中的一个学习任务。

## 文档索引

### 第一阶段：基础入门

1. **[task1_简单命令.md](task1_简单命令.md)** - 创建第一个简单命令
   - Command 类基础
   - [VisualStudioContribution] 特性
   - CommandConfiguration 配置
   - ExecuteCommandAsync 实现

2. **[task2_用户提示和对话框.md](task2_用户提示和对话框.md)** - 用户交互
   - Shell 服务使用
   - ShowPromptAsync 方法
   - 各种提示框类型
   - 输入提示和自定义选项

3. **[task3_工具窗口.md](task3_工具窗口.md)** - 工具窗口开发
   - ToolWindow 类
   - 数据模型和数据绑定
   - RemoteUserControl 和 XAML
   - 窗口配置和工具栏

### 第二阶段：编辑器集成

4. **[task4_文本编辑器操作.md](task4_文本编辑器操作.md)** - 文本编辑器操作
   - 获取活动文本视图
   - 读取文档内容
   - 执行编辑操作
   - 处理多个选择区域

5. **[task5_CodeLens提供者.md](task5_CodeLens提供者.md)** - CodeLens 提供者
   - ICodeLensProvider 接口
   - 创建可点击的 CodeLens
   - 提供可视化界面
   - 代码元素类型识别

6. **[task6_文本标记器.md](task6_文本标记器.md)** - 文本标记器（Tagger）
   - ITextViewTaggerProvider 接口
   - TextViewTagger 实现
   - 创建和更新标记
   - 文档变化处理

### 第三阶段：项目系统交互

7. **[task7_项目查询API.md](task7_项目查询API.md)** - 项目查询 API
   - WorkspacesExtensibility 使用
   - 查询项目和解决方案
   - With 方法和属性选择
   - 过滤和分页查询

8. **[task8_修改项目系统.md](task8_修改项目系统.md)** - 修改项目系统
   - UpdateProjectsAsync 使用
   - 添加和删除文件
   - 重命名项目
   - 解决方案配置管理
   - 构建和保存操作

### 第四阶段：高级功能

9. **[task9_扩展设置管理.md](task9_扩展设置管理.md)** - 扩展设置管理
   - SettingCategory 和 Setting 定义
   - 设置观察者模式
   - 读取和写入设置
   - 设置本地化

10. **task10_文件和文件夹选择器.md** - 文件和文件夹选择器（待创建）
    - ShowOpenFileDialogAsync
    - ShowSaveAsFileDialogAsync
    - ShowOpenFolderDialogAsync
    - FileDialogOptions 配置

11. **task11_创建对话框.md** - 创建对话框（待创建）
    - RemoteUserControl
    - XAML 数据模板
    - 资源字典
    - ShowDialogAsync

12. **task12_语言服务器提供者.md** - 语言服务器提供者（待创建）
    - LanguageServerProvider
    - CreateServerConnectionAsync
    - DuplexPipe
    - 服务器初始化

## 学习建议

### 学习顺序

按照任务编号顺序学习，从简单到复杂：
1. 先掌握基础命令和 UI 交互（Task 1-3）
2. 再学习编辑器集成（Task 4-6）
3. 然后学习项目系统操作（Task 7-8）
4. 最后学习高级功能（Task 9-16）

### 实践方法

1. **阅读文档** - 仔细阅读每个任务的学习文档
2. **理解概念** - 理解核心概念和知识点
3. **运行示例** - 运行完整示例代码
4. **动手实践** - 自己编写代码实现功能
5. **解决问题** - 遇到问题查看"常见问题"部分
6. **扩展学习** - 探索"进阶主题"

### 代码示例

每个文档都包含：
- 📝 核心概念说明
- 💡 知识点详解
- 📦 完整可运行示例
- ✅ 实践步骤指导
- ❓ 常见问题解答
- 🚀 进阶主题参考

### 参考资源

- **llm.md** - 包含所有代码片段的源文档
- **官方示例** - Microsoft 官方提供的示例代码
- **API 文档** - Visual Studio Extensibility API 文档

## 进度跟踪

在 `docs/todo.md` 中跟踪你的学习进度：
- 使用 `- [ ]` 标记待完成任务
- 完成后改为 `- [x]`
- 记录遇到的问题和解决方案

## 快速查找

### 按功能查找

- **命令开发**: Task 1, Task 14
- **UI 开发**: Task 2, Task 3, Task 10, Task 11
- **编辑器操作**: Task 4, Task 5, Task 6
- **项目系统**: Task 7, Task 8
- **设置管理**: Task 9
- **语言服务**: Task 12
- **高级主题**: Task 13, Task 15, Task 16

### 按难度查找

- **入门级** (⭐): Task 1, Task 2
- **初级** (⭐⭐): Task 3, Task 4, Task 10
- **中级** (⭐⭐⭐): Task 5, Task 6, Task 7, Task 8, Task 9, Task 11
- **高级** (⭐⭐⭐⭐): Task 12, Task 13, Task 14, Task 15, Task 16

## 更新记录

- 2025-10-25: 创建学习文档结构，完成 Task 1-9 的详细文档

## 反馈和改进

如果在学习过程中发现问题或有改进建议，请：
1. 在文档中添加注释
2. 记录到 `todo.md` 的 stories 中
3. 与团队分享经验

祝学习愉快！ 🎉

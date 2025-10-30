using System.Threading;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Microsoft.VisualStudio.Extensibility.Shell;
using Microsoft.VisualStudio.ProjectSystem.Query.Metadata;
using Microsoft.VisualStudio.RpcContracts.Notifications;
using Microsoft.VisualStudio.Text.Editor;

namespace Rabbit.LearnExtension.Task2;

[VisualStudioContribution]
internal class DialogCommand : Command
{
    public override CommandConfiguration CommandConfiguration => new("%Rabbit.LearnExtension.DialogCommand%")
    {
        Placements = [CommandPlacement.KnownPlacements.ToolsMenu],
        Icon = new(ImageMoniker.KnownValues.Extension, IconSettings.IconAndText),
        TooltipText = "%Rabbit.LearnExtension.DialogCommand%",
    };

    public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
    {
        // 1. dialog
        await Extensibility.Shell().ShowDialogAsync(new DialogControl(null), cancellationToken);
        await Extensibility.Shell().ShowDialogAsync(new DialogControl(null), "Custom Dialog Title", cancellationToken);
        await Extensibility.Shell().ShowDialogAsync(new DialogControl(null), DialogOption.OKCancel, cancellationToken);

        var dialogResult = await Extensibility.Shell().ShowDialogAsync(new DialogControl(null), "dialog 4", DialogOption.OKCancel, cancellationToken);
        switch (dialogResult)
        {
            case DialogResult.OK:
                break;
            case DialogResult.Close:
                break;
            default:
                break;
        }

        // 2.
        await ShowPromptWorkflowAsync(cancellationToken);
    }

    public enum ProjectType
    {
        WebApi,
        Console,
        ClassLibrary
    }

    internal async Task ShowPromptWorkflowAsync(CancellationToken cancellationToken)
    {
        var shell = Extensibility.Shell();

        // ========== 完整工作流示例：创建项目配置 ==========

        // 1️⃣ 确认对话框（PromptOptions.OKCancel）- 返回 bool
        bool shouldContinue = await shell.ShowPromptAsync(
            "是否要创建新的项目配置？",
            PromptOptions.OKCancel,
            cancellationToken);

        if (!shouldContinue)
        {
            await shell.ShowPromptAsync("操作已取消", PromptOptions.OK, cancellationToken);
            return;
        }

        // 2️⃣ 文本输入框（InputPromptOptions）- 获取项目名称
        string? projectName = await shell.ShowPromptAsync(
            "请输入项目名称：",
            new InputPromptOptions
            {
                DefaultText = "MyProject",
                Title = "新建项目 - 步骤 1/2"
            },
            cancellationToken);

        if (projectName == null) // 用户取消
        {
            await shell.ShowPromptAsync("操作已取消", PromptOptions.OK, cancellationToken);
            return;
        }

        // 3️⃣ 自定义选项（PromptOptions<TResult>）- 选择项目类型
        ProjectType selectedType = await shell.ShowPromptAsync(
            $"为项目 '{projectName}' 选择类型：",
            new PromptOptions<ProjectType>
            {
                Choices =
                {
                    { "Web API 项目", ProjectType.WebApi },
                    { "控制台应用", ProjectType.Console },
                    { "类库", ProjectType.ClassLibrary },
                },
                DismissedReturns = ProjectType.Console,
                DefaultChoiceIndex = 0,
                Title = "新建项目 - 步骤 2/2"
            },
            cancellationToken);

        // 4️⃣ 信息提示（PromptOptions.OK）- 显示创建结果
        string typeText = selectedType switch
        {
            ProjectType.WebApi => "Web API",
            ProjectType.Console => "控制台",
            ProjectType.ClassLibrary => "类库",
            _ => "未知"
        };

        await shell.ShowPromptAsync(
            $"✅ 配置完成！\n项目名称: {projectName}\n项目类型: {typeText}",
            PromptOptions.OK,
            cancellationToken);
    }
}

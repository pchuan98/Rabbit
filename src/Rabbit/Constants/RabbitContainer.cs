namespace Rabbit.Constants;

/// <summary>
/// 对项目 RabbitContainer 常量封装
/// </summary>
public static class RabbitContainer
{
    public static Guid Root => new("e85d182a-f11b-4b4f-8fdc-cdfafe7b0d25");

    public static uint EditorContextMenuGroup => 0x0001;
    public static uint FileItemContextMenuGroup = 0x0002;
    public static uint FolderContextMenuGroup = 0x0003;
    public static uint ProjectContextMenuGroup = 0x0004;
    public static uint SolutionContextMenuGroup = 0x0005;
}

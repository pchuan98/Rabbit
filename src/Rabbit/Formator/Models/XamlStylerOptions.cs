using System.Text;

namespace Rabbit.Formator.Models;

/// <summary>
/// XamlStyler 命令行工具的配置选项模型
/// </summary>
public class XamlStylerOptions
{
    #region 基本操作参数

    /// <summary>
    /// 要处理的 XAML 文件（支持逗号分隔的文件列表）
    /// 对应参数: -f, --file
    /// </summary>
    public string? File { get; set; }

    /// <summary>
    /// 要处理 XAML 文件的目录
    /// 对应参数: -d, --directory
    /// </summary>
    public string? Directory { get; set; }

    /// <summary>
    /// 包含 XAML Styler 设置配置的 JSON 文件
    /// 对应参数: -c, --config
    /// </summary>
    public string? Config { get; set; }

    /// <summary>
    /// 忽略 XAML 文件类型检查并处理所有文件
    /// 对应参数: -i, --ignore
    /// </summary>
    public bool? Ignore { get; set; }

    /// <summary>
    /// 递归处理指定的目录
    /// 对应参数: -r, --recursive
    /// </summary>
    public bool? Recursive { get; set; }

    /// <summary>
    /// 检查文件是否遵循正确的格式，而不进行任何修改。如果文件未通过检查，则返回错误状态
    /// 对应参数: -p, --passive
    /// </summary>
    public bool? Passive { get; set; }

    /// <summary>
    /// 将格式化结果输出到 stdout 而不是修改文件。在此模式下，日志打印到 stderr。必须指定一个文件。不能与 --passive 结合使用
    /// 对应参数: --write-to-stdout
    /// </summary>
    public bool? WriteToStdout { get; set; }

    /// <summary>
    /// 日志级别，按详细程度递增顺序: None, Minimal, Default, Verbose, Debug
    /// 对应参数: -l, --loglevel
    /// </summary>
    public string? LogLevel { get; set; }

    #endregion

    #region 缩进设置

    /// <summary>
    /// 覆盖: 缩进大小
    /// 对应参数: --indent-size
    /// </summary>
    public int? IndentSize { get; set; }

    /// <summary>
    /// 覆盖: 使用制表符缩进
    /// 对应参数: --indent-tabs
    /// </summary>
    public bool? IndentTabs { get; set; }

    #endregion

    #region 属性格式化设置

    /// <summary>
    /// 覆盖: 属性容差（在达到此值之前，属性将保持在同一行）
    /// 对应参数: --attributes-tolerance
    /// </summary>
    public int? AttributesTolerance { get; set; }

    /// <summary>
    /// 覆盖: 将第一个属性保持在同一行
    /// 对应参数: --attributes-same-line
    /// </summary>
    public bool? AttributesSameLine { get; set; }

    /// <summary>
    /// 覆盖: 每行最大属性字符数
    /// 对应参数: --attributes-max-chars
    /// </summary>
    public int? AttributesMaxChars { get; set; }

    /// <summary>
    /// 覆盖: 每行最大属性数
    /// 对应参数: --attributes-max
    /// </summary>
    public int? AttributesMax { get; set; }

    /// <summary>
    /// 覆盖: 不换行的元素列表
    /// 对应参数: --no-newline-elements
    /// </summary>
    public string? NoNewlineElements { get; set; }

    /// <summary>
    /// 覆盖: 将属性排序规则组放在单独的行上
    /// 对应参数: --attributes-order-groups-newline
    /// </summary>
    public bool? AttributesOrderGroupsNewline { get; set; }

    /// <summary>
    /// 覆盖: 属性缩进量
    /// 对应参数: --attributes-indentation
    /// </summary>
    public int? AttributesIndentation { get; set; }

    /// <summary>
    /// 覆盖: 属性缩进样式
    /// 对应参数: --attributes-indentation-style
    /// </summary>
    public string? AttributesIndentationStyle { get; set; }

    /// <summary>
    /// 覆盖: 移除设计时引用
    /// 对应参数: --remove-design-references
    /// </summary>
    public bool? RemoveDesignReferences { get; set; }

    /// <summary>
    /// 覆盖: 启用属性重新排序
    /// 对应参数: --attributes-reorder
    /// </summary>
    public bool? AttributesReorder { get; set; }

    /// <summary>
    /// 覆盖: 第一行属性
    /// 对应参数: --attributes-first-line
    /// </summary>
    public string? AttributesFirstLine { get; set; }

    /// <summary>
    /// 覆盖: 按名称对属性排序
    /// 对应参数: --attributes-order-name
    /// </summary>
    public bool? AttributesOrderName { get; set; }

    #endregion

    #region 元素格式化设置

    /// <summary>
    /// 覆盖: 将结束括号放在新行上
    /// 对应参数: --ending-bracket-newline
    /// </summary>
    public bool? EndingBracketNewline { get; set; }

    /// <summary>
    /// 覆盖: 移除空元素的结束标签
    /// 对应参数: --remove-empty-ending-tag
    /// </summary>
    public bool? RemoveEmptyEndingTag { get; set; }

    /// <summary>
    /// 覆盖: 在闭合斜杠前添加空格
    /// 对应参数: --space-before-closing-slash
    /// </summary>
    public bool? SpaceBeforeClosingSlash { get; set; }

    /// <summary>
    /// 覆盖: 根元素换行规则
    /// 对应参数: --root-line-break
    /// </summary>
    public string? RootLineBreak { get; set; }

    #endregion

    #region 元素重新排序设置

    /// <summary>
    /// 覆盖: 重新排序 VisualStateManager
    /// 对应参数: --reorder-vsm
    /// </summary>
    public bool? ReorderVsm { get; set; }

    /// <summary>
    /// 覆盖: 重新排序 Grid 子元素
    /// 对应参数: --reorder-grid-children
    /// </summary>
    public bool? ReorderGridChildren { get; set; }

    /// <summary>
    /// 覆盖: 重新排序 Canvas 子元素
    /// 对应参数: --reorder-canvas-children
    /// </summary>
    public bool? ReorderCanvasChildren { get; set; }

    /// <summary>
    /// 覆盖: 重新排序 Setter
    /// 对应参数: --reorder-setters
    /// </summary>
    public bool? ReorderSetters { get; set; }

    #endregion

    #region 标记扩展和厚度设置

    /// <summary>
    /// 覆盖: 格式化标记扩展
    /// 对应参数: --format-markup-extension
    /// </summary>
    public bool? FormatMarkupExtension { get; set; }

    /// <summary>
    /// 覆盖: 不换行的标记扩展
    /// 对应参数: --no-newline-markup-extensions
    /// </summary>
    public string? NoNewlineMarkupExtensions { get; set; }

    /// <summary>
    /// 覆盖: 厚度样式
    /// 对应参数: --thickness-style
    /// </summary>
    public string? ThicknessStyle { get; set; }

    /// <summary>
    /// 覆盖: 厚度属性
    /// 对应参数: --thickness-attributes
    /// </summary>
    public string? ThicknessAttributes { get; set; }

    #endregion

    #region 注释设置

    /// <summary>
    /// 覆盖: 注释空格数
    /// 对应参数: --comment-spaces
    /// </summary>
    public int? CommentSpaces { get; set; }

    #endregion

    #region 方法

    /// <summary>
    /// 将配置选项转换为命令行参数字符串
    /// </summary>
    /// <returns>格式化的命令行参数字符串</returns>
    public override string ToString()
    {
        var sb = new StringBuilder(512);

        // 基本操作参数
        if (File != null) sb.Append("-f \"").Append(File).Append("\" ");
        if (Directory != null) sb.Append("-d \"").Append(Directory).Append("\" ");
        if (Config != null) sb.Append("-c \"").Append(Config).Append("\" ");
        if (Ignore == true) sb.Append("-i ");
        if (Recursive == true) sb.Append("-r ");
        if (Passive == true) sb.Append("-p ");
        if (WriteToStdout == true) sb.Append("--write-to-stdout ");
        if (LogLevel != null) sb.Append("-l ").Append(LogLevel).Append(' ');

        // 缩进设置
        if (IndentSize != null) sb.Append("--indent-size ").Append(IndentSize).Append(' ');
        if (IndentTabs == true) sb.Append("--indent-tabs ");

        // 属性格式化设置
        if (AttributesTolerance != null) sb.Append("--attributes-tolerance ").Append(AttributesTolerance).Append(' ');
        if (AttributesSameLine == true) sb.Append("--attributes-same-line ");
        if (AttributesMaxChars != null) sb.Append("--attributes-max-chars ").Append(AttributesMaxChars).Append(' ');
        if (AttributesMax != null) sb.Append("--attributes-max ").Append(AttributesMax).Append(' ');
        if (NoNewlineElements != null) sb.Append("--no-newline-elements \"").Append(NoNewlineElements).Append("\" ");
        if (AttributesOrderGroupsNewline == true) sb.Append("--attributes-order-groups-newline ");
        if (AttributesIndentation != null) sb.Append("--attributes-indentation ").Append(AttributesIndentation).Append(' ');
        if (AttributesIndentationStyle != null) sb.Append("--attributes-indentation-style ").Append(AttributesIndentationStyle).Append(' ');
        if (RemoveDesignReferences == true) sb.Append("--remove-design-references ");
        if (AttributesReorder == true) sb.Append("--attributes-reorder ");
        if (AttributesFirstLine != null) sb.Append("--attributes-first-line ").Append(AttributesFirstLine).Append(' ');
        if (AttributesOrderName == true) sb.Append("--attributes-order-name ");

        // 元素格式化设置
        if (EndingBracketNewline == true) sb.Append("--ending-bracket-newline ");
        if (RemoveEmptyEndingTag == true) sb.Append("--remove-empty-ending-tag ");
        if (SpaceBeforeClosingSlash == true) sb.Append("--space-before-closing-slash ");
        if (RootLineBreak != null) sb.Append("--root-line-break ").Append(RootLineBreak).Append(' ');

        // 元素重新排序设置
        if (ReorderVsm == true) sb.Append("--reorder-vsm ");
        if (ReorderGridChildren == true) sb.Append("--reorder-grid-children ");
        if (ReorderCanvasChildren == true) sb.Append("--reorder-canvas-children ");
        if (ReorderSetters == true) sb.Append("--reorder-setters ");

        // 标记扩展和厚度设置
        if (FormatMarkupExtension == true) sb.Append("--format-markup-extension ");
        if (NoNewlineMarkupExtensions != null) sb.Append("--no-newline-markup-extensions \"").Append(NoNewlineMarkupExtensions).Append("\" ");
        if (ThicknessStyle != null) sb.Append("--thickness-style ").Append(ThicknessStyle).Append(' ');
        if (ThicknessAttributes != null) sb.Append("--thickness-attributes \"").Append(ThicknessAttributes).Append("\" ");

        // 注释设置
        if (CommentSpaces != null) sb.Append("--comment-spaces ").Append(CommentSpaces).Append(' ');

        // 移除末尾空格
        if (sb.Length > 0 && sb[^1] == ' ')
            sb.Length--;

        return sb.ToString();
    }

    #endregion
}

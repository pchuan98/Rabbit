namespace Rabbit.Formator.Services;

/// <summary>
/// 代码格式化配置
/// </summary>
internal interface IFormatService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="input">输入字符串</param>
    /// <param name="config">每次format需要的配置文件</param>
    /// <returns></returns>
    public Task<string> FormatAsync(string input, object? config);
}

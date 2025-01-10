namespace MbUtils.TextSearch.Business;

public class AppConfig
{
    public int ParallelTasks { get; init; } = 1;
    public string OutputFilePath { get; init; } = string.Empty;
    public bool IsUtf8 { get; init; } = true;
    public int BufferSize { get; init; } = 1 * 1024 * 1024;
    public string Strategy { get; init; } = "Regex";
}
namespace TinyVgCore.DrawCommands;

/// <summary>
/// Command that represents the end of a file
/// </summary>
public class TvgCmdEndOfDocument : TvgCommand
{
    /// <summary>
    /// Create a default end-of-document command
    /// </summary>
    public TvgCmdEndOfDocument()
    {
        CommandType = 0;
        PrimaryStyleType = 0;
    }
}
using TinyVgCore.BasicTypes;

namespace TinyVgCore.DrawCommands;

/// <summary>
/// General file command. These are mostly drawing actions
/// </summary>
public abstract class TvgCommand
{
    /// <summary>
    /// Type of draw command
    /// </summary>
    public TvgCommandType CommandType { get; set; }

    /// <summary>
    /// Primary fill style
    /// </summary>
    public TvgFillStyleType PrimaryStyleType { get; set; }
}
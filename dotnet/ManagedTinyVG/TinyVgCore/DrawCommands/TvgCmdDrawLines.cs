using System.Collections.Generic;
using TinyVgCore.BasicTypes;

namespace TinyVgCore.DrawCommands;

/// <summary>
/// Draws a set of lines
/// </summary>
public class TvgCmdDrawLines : TvgCommand
{
    /// <summary>
    /// Create an empty command
    /// </summary>
    public TvgCmdDrawLines()
    {
        LineStyle = TvgStyle.Default;
        CommandType = TvgCommandType.DrawLines;
    }
    
    /// <summary>
    /// The number of lines in the set. This value is offset by 1
    /// </summary>
    public ulong LineCount { get; set; }
    
    /// <summary>
    /// Line style
    /// </summary>
    public TvgStyle LineStyle { get; set; }

    /// <summary>
    /// Width of line
    /// </summary>
    public double LineWidth { get; set; }
    
    /// <summary>
    /// List of lines
    /// </summary>
    public List<TvgLine> Lines { get; } = new();
}
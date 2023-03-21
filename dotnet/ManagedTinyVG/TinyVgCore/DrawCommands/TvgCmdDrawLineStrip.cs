using System.Collections.Generic;
using TinyVgCore.BasicTypes;

namespace TinyVgCore.DrawCommands;

/// <summary>
/// Draws a list of consecutive lines
/// </summary>
public class TvgCmdDrawLineStrip : TvgCommand
{
    /// <summary>
    /// Create an empty polygon
    /// </summary>
    public TvgCmdDrawLineStrip()
    {
        LineStyle = TvgStyle.Default;
        CommandType = TvgCommandType.DrawLineStrip;
    }
        
    /// <summary>
    /// The number of points in the polygon. This value is offset by 1
    /// </summary>
    public ulong PointCount { get; set; }

    /// <summary>
    /// List of points in the polygon
    /// </summary>
    public List<TvgPoint> Points { get; } = new();

    /// <summary>
    /// Line style
    /// </summary>
    public TvgStyle LineStyle { get; set; }

    /// <summary>
    /// Line width
    /// </summary>
    public double LineWidth { get; set; }
}
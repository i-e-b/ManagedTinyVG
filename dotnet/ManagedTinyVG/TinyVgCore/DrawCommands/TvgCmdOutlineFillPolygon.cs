using System.Collections.Generic;
using TinyVgCore.BasicTypes;

namespace TinyVgCore.DrawCommands;

/// <summary>
/// Fills a polygon and draws an outline at the same time
/// </summary>
public class TvgCmdOutlineFillPolygon : TvgCommand
{
    /// <summary>
    /// Create an empty command
    /// </summary>
    public TvgCmdOutlineFillPolygon()
    {
        FillStyle = TvgStyle.Default;
        LineStyle = TvgStyle.Default;
        CommandType = TvgCommandType.OutlineFillPolygon;
    }

    /// <summary>
    /// The number of points in the polygon. This value is offset by 1
    /// </summary>
    public ulong SegmentCount { get; set; }

    /// <summary>
    /// The set of points of this polygon
    /// </summary>
    public List<TvgPoint> Points { get; } = new();

    /// <summary>
    /// Fill style
    /// </summary>
    public TvgStyle FillStyle { get; set; }

    /// <summary>
    /// Line style
    /// </summary>
    public TvgStyle LineStyle { get; set; }

    /// <summary>
    /// Width of line
    /// </summary>
    public double LineWidth { get; set; }
}
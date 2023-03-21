using System.Collections.Generic;
using TinyVgCore.BasicTypes;

namespace TinyVgCore.DrawCommands;

/// <summary>
/// Fills a polygon with N points.
/// </summary>
public class TvgCmdFillPolygon:TvgCommand
{
    /// <summary>
    /// Create an empty polygon
    /// </summary>
    public TvgCmdFillPolygon()
    {
        Style = TvgStyle.Default;
        CommandType = TvgCommandType.FillPolygon;
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
    /// Fill style
    /// </summary>
    public TvgStyle Style { get; set; }
}
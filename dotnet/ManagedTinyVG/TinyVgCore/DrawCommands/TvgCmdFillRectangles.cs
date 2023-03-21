using System.Collections.Generic;
using TinyVgCore.BasicTypes;

namespace TinyVgCore.DrawCommands;

/// <summary>
/// Fills a list of rectangles.
/// </summary>
public class TvgCmdFillRectangles : TvgCommand
{
    /// <summary>
    /// Create an empty rectangle
    /// </summary>
    public TvgCmdFillRectangles()
    {
        Style = TvgStyle.Default;
        CommandType = TvgCommandType.FillRectangles;
    }

    /// <summary>
    /// The number of points in the polygon. This value is offset by 1
    /// </summary>
    public ulong RectangleCount { get; set; }

    /// <summary>
    /// List of points in the polygon
    /// </summary>
    public List<TvgRectangle> Rectangles { get; } = new();

    /// <summary>
    /// Fill style
    /// </summary>
    public TvgStyle Style { get; set; }
}
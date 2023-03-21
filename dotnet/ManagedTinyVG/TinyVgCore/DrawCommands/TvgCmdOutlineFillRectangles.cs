using System.Collections.Generic;
using TinyVgCore.BasicTypes;

namespace TinyVgCore.DrawCommands;

/// <summary>
/// Fills a list of rectangles.
/// </summary>
public class TvgCmdOutlineFillRectangles : TvgCommand
{
    /// <summary>
    /// Create an empty command
    /// </summary>
    public TvgCmdOutlineFillRectangles()
    {
        FillStyle = TvgStyle.Default;
        LineStyle = TvgStyle.Default;
        CommandType = TvgCommandType.OutlineFillRectangles;
    }

    /// <summary>
    /// The number of rectangles in the set. This value is offset by 1
    /// </summary>
    public ulong RectangleCount { get; set; }

    /// <summary>
    /// List of rectangles
    /// </summary>
    public List<TvgRectangle> Rectangles { get; } = new();

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
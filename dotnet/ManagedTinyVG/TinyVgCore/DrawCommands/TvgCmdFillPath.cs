using System.Collections.Generic;
using TinyVgCore.BasicTypes;
using TinyVgCore.Paths;

namespace TinyVgCore.DrawCommands;

/// <summary>
/// Fills a path. 
/// </summary>
public class TvgCmdFillPath : TvgCommand
{
    /// <summary>
    /// Create a new command
    /// </summary>
    public TvgCmdFillPath()
    {
        CommandType = TvgCommandType.FillPath;
        FillStyle = new TvgStyle();
        Path = new();
    }

    /// <summary>
    /// The number of segments in the path. This value is offset by 1.
    /// </summary>
    public ulong SegmentCount { get; set; }

    /// <summary>
    /// A path with SegmentCount segments.
    /// </summary>
    public List<TvgPathSegment> Path { get; set; }
    
    /// <summary>
    /// Fill style
    /// </summary>
    public TvgStyle FillStyle { get; set; }
}
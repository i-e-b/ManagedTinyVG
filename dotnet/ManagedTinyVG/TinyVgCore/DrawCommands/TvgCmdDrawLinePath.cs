﻿using System.Collections.Generic;
using TinyVgCore.BasicTypes;
using TinyVgCore.Paths;

namespace TinyVgCore.DrawCommands;

/// <summary>
/// Draws a path. 
/// </summary>
public class TvgCmdDrawLinePath : TvgCommand
{
    /// <summary>
    /// Create a new command
    /// </summary>
    public TvgCmdDrawLinePath()
    {
        CommandType = TvgCommandType.DrawLinePath;
        LineStyle = new TvgStyle();
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
    /// Line style
    /// </summary>
    public TvgStyle LineStyle { get; set; }

    /// <summary>
    /// Width of line
    /// </summary>
    public double LineWidth { get; set; }
}
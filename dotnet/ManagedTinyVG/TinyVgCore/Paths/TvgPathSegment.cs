using System.Collections.Generic;
using TinyVgCore.BasicTypes;

namespace TinyVgCore.Paths;

/// <summary>
/// Path segment command
/// </summary>
public class TvgPathSegment
{
    /// <summary>
    /// Create new segment
    /// </summary>
    public TvgPathSegment()
    {
        StartPoint = new TvgPoint();
        Commands = new();
    }

    /// <summary>
    /// Pen-down location for segment (all segments start
    /// with an implicit "Move To")
    /// </summary>
    public TvgPoint StartPoint { get; set; }

    /// <summary>
    /// Line width at this segment. Copied from
    /// containing path, or the last command change.
    /// </summary>
    public double LineWidth { get; set; }

    /// <summary>
    /// List of commands that comprise this segment
    /// </summary>
    public List<TvgPathSegmentCommand> Commands { get; }
}
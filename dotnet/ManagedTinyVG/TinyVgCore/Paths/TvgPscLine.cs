using TinyVgCore.BasicTypes;

namespace TinyVgCore.Paths;

/// <summary>
/// PathSegmentCommand for a line
/// </summary>
public class TvgPscLine : TvgPathSegmentCommand
{
    /// <summary>
    /// Create segment command
    /// </summary>
    public TvgPscLine()
    {
        Position = new();
    }
    /// <summary>
    /// The end point of the line.
    /// </summary>
    public TvgPoint Position { get; set; }
}
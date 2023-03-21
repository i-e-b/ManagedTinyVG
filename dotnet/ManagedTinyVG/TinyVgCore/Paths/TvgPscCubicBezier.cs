using TinyVgCore.BasicTypes;

namespace TinyVgCore.Paths;

/// <summary>
/// PathSegmentCommand for a cubic bezier instruction that draws a Bézier curve with two control points.
/// </summary>
public class TvgPscCubicBezier : TvgPathSegmentCommand
{
    /// <summary>
    /// Create segment command
    /// </summary>
    public TvgPscCubicBezier()
    {
        Control0 = new();
        Control1 = new();
        EndPoint = new();
    }
    
    /// <summary>
    /// The first control point
    /// </summary>
    public TvgPoint Control0 { get; set; }
    
    /// <summary>
    /// The second control point
    /// </summary>
    public TvgPoint Control1 { get; set; }
    
    /// <summary>
    /// The end point of the line.
    /// </summary>
    public TvgPoint EndPoint { get; set; }
}
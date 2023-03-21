using TinyVgCore.BasicTypes;

namespace TinyVgCore.Paths;

/// <summary>
/// PathSegmentCommand for a quadratic bezier instruction that draws a Bézier curve with one control point.
/// </summary>
public class TvgPscQuadraticBezier : TvgPathSegmentCommand
{
    /// <summary>
    /// Create segment command
    /// </summary>
    public TvgPscQuadraticBezier()
    {
        Control0 = new();
        EndPoint = new();
    }
    
    /// <summary>
    /// The control point
    /// </summary>
    public TvgPoint Control0 { get; set; }
    
    /// <summary>
    /// The end point of the line.
    /// </summary>
    public TvgPoint EndPoint { get; set; }
}
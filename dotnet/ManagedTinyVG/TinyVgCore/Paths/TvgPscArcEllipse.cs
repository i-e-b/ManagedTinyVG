using TinyVgCore.BasicTypes;

namespace TinyVgCore.Paths;

/// <summary>
/// PathSegmentCommand for an ellipse segment between the current and the target point
/// </summary>
public class TvgPscArcEllipse : TvgPathSegmentCommand
{
    
    /// <summary>
    /// Create segment command
    /// </summary>
    public TvgPscArcEllipse()
    {
        Target = new();
    }
    
    /// <summary>
    /// When true the larger circle segment is drawn.
    /// </summary>
    public bool LargeArc { get; set; }

    /// <summary>
    /// If SweepLeft is true, the circle segment will make a left turn, otherwise it will make a right turn. This means
    /// that if we go from the current point to target, a rotation to the movement direction is necessary to either
    /// the left or the right.
    /// </summary>
    public bool SweepLeft { get; set; }

    /// <summary>
    /// The radius of the ellipse in horizontal direction
    /// </summary>
    public double RadiusX { get; set; }

    /// <summary>
    /// The radius of the ellipse in vertical direction
    /// </summary>
    public double RadiusY { get; set; }
    
    /// <summary>
    /// The rotation of the ellipse in mathematical negative direction, in degrees
    /// </summary>
    public double Rotation { get; set; }
    
    /// <summary>
    /// The end point of ellipse circle segment
    /// </summary>
    public TvgPoint Target { get; set; }
}
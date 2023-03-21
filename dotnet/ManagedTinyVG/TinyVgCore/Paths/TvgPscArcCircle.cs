using TinyVgCore.BasicTypes;

namespace TinyVgCore.Paths;

/// <summary>
/// PathSegmentCommand for a circle segment between the current and the target point
/// </summary>
public class TvgPscArcCircle : TvgPathSegmentCommand
{
    /// <summary>
    /// Create segment command
    /// </summary>
    public TvgPscArcCircle()
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
    /// The radius of the circle
    /// </summary>
    public double Radius { get; set; }

    /// <summary>
    /// The end point of the circle segment
    /// </summary>
    public TvgPoint Target { get; set; }
}
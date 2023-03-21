namespace TinyVgCore.Paths;

/// <summary>
/// Instruction type in a path
/// </summary>
public enum TvgPathInstruction
{
    /// <summary>
    /// A straight line is drawn from the current point to a new point
    /// </summary>
    Line = 0,
    
    /// <summary>
    ///  A straight horizontal line is drawn from the current point to a new x coordinate
    /// </summary>
    HorizontalLine = 1,
    
    /// <summary>
    /// A straight vertical line is drawn from the current point to a new y coordinate
    /// </summary>
    VerticalLine = 2,
    
    /// <summary>
    /// A cubic Bézier curve is drawn from the current point to a new point
    /// </summary>
    CubicBezier = 3,
    
    /// <summary>
    /// A circle segment is drawn from current point to a new point
    /// </summary>
    ArcCircle = 4,
    
    /// <summary>
    /// An ellipse segment is drawn from current point to a new point
    /// </summary>
    ArcEllipse = 5,
    
    /// <summary>
    /// The path is closed, and a straight line is drawn to the starting point
    /// </summary>
    ClosePath = 6,
    
    /// <summary>
    /// A quadratic Bézier curve is drawn from the current point to a new point
    /// </summary>
    QuadraticBezier = 7
}
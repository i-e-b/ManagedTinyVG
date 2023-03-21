namespace TinyVgCore.BasicTypes;

/// <summary>
/// A scaled rectangle in a Tvg document
/// </summary>
public class TvgRectangle
{
    /// <summary>
    /// Horizontal distance of the left side to the origin
    /// </summary>
    public double X { get; set; }

    /// <summary>
    /// Vertical distance of the upper side to the origin
    /// </summary>
    public double Y { get; set; }

    /// <summary>
    /// Horizontal extent of the rectangle
    /// </summary>
    public double Width { get; set; }

    /// <summary>
    /// Vertical extent of the rectangle origin
    /// </summary>
    public double Height { get; set; }
}
namespace TinyVgCore.BasicTypes;

/// <summary>
/// A scaled line in a Tvg document
/// </summary>
public class TvgLine
{
    /// <summary>
    /// Create a new line
    /// </summary>
    public TvgLine()
    {
        Start = new TvgPoint();
        End = new TvgPoint();
    }
    
    /// <summary>
    /// Start point of the line
    /// </summary>
    public TvgPoint Start { get; set; }
    
    /// <summary>
    /// End point of the line
    /// </summary>
    public TvgPoint End { get; set; }
}
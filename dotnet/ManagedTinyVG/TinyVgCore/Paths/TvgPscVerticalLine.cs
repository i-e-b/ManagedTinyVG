namespace TinyVgCore.Paths;

/// <summary>
/// PathSegmentCommand for a line
/// </summary>
internal class TvgPscVerticalLine : TvgPathSegmentCommand
{
    /// <summary>
    /// The new y coordinate
    /// </summary>
    public double Y { get; set; }
}
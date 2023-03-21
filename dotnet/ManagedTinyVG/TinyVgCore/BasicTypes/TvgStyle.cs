namespace TinyVgCore.BasicTypes;

/// <summary>
/// Fill data
/// </summary>
public class TvgStyle
{
    /// <summary>
    /// Default style, using flat fill and color 0
    /// </summary>
    public static TvgStyle Default => new();
        
    /// <summary>
    /// Fill type. If <see cref="TvgFillStyleType.Flat"/>, then only Color0
    /// is populated. Otherwise all fields are populated
    /// </summary>
    public TvgFillStyleType FillType { get; set; }

    /// <summary>
    /// Color palette index of the primary color
    /// </summary>
    public ulong Color0 { get; set; }
        
    /// <summary>
    /// Color palette index of the secondary color
    /// </summary>
    public ulong Color1 { get; set; }

    /// <summary>
    /// Primary gradient point, if a gradient fill
    /// </summary>
    public TvgPoint? Point0 { get; set; }
        
    /// <summary>
    /// Secondary gradient point, if a gradient fill
    /// </summary>
    public TvgPoint? Point1 { get; set; }
}
namespace TinyVgCore;

/// <summary>
/// The coordinate range defines how many bits a Unit value uses
/// </summary>
public enum TvgCoordinateRange
{
    /// <summary>
    /// Each Unit takes up 16 bits
    /// </summary>
    Default = 0,
        
    /// <summary>
    /// Each Unit takes up 8 bits
    /// </summary>
    Reduced = 1,
        
    /// <summary>
    /// Each Unit takes up 32 bit
    /// </summary>
    Enhanced = 2
}
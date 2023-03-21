namespace TinyVgCore.BasicTypes;

/// <summary>
/// Fill style for commands
/// </summary>
public enum TvgFillStyleType : byte
{
    /// <summary> Flat fill of a single color (by palette index) </summary>
    Flat = 0,
        
    /// <summary> Linear fill by a pair of points and a pair of palette indexes </summary>
    Linear = 1,
        
    /// <summary> Radial fill by a pair of points and a pair of palette indexes </summary>
    Radial = 2,
        
    /// <summary> Unknown fill value </summary>
    Unknown = 3
}
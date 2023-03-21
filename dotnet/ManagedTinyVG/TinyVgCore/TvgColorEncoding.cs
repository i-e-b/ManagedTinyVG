namespace TinyVgCore;

/// <summary>
/// Defines the type of color information that is used in the color table
/// </summary>
public enum TvgColorEncoding:int
{
    /// <summary>
    /// Each color is a 4-tuple (red, green, blue, alpha) of bytes with the color
    /// channels encoded in sRGB and the alpha as linear alpha.
    /// </summary>
    Rgba8888 = 0,
        
    /// <summary>
    /// Each color is encoded as a 3-tuple (red, green, blue) with 16 bit per color.
    /// While red and blue both use 5 bit, the green channel uses 6 bit. red uses
    /// bit range 0...4, green bits 5...10 and blue bits 11...15. This color also uses
    /// the sRGB color space.
    /// </summary>
    Rgb565 = 1,
        
    /// <summary>
    /// Each color is a 4-tuple (red, green ,blue, alpha) of binary32 IEEE 754
    /// floating point value with the color channels encoded in scRGB and the
    /// alpha as linear alpha. A color value of 1.0 is full intensity, while a value of
    /// 0.0 is zero intensity
    /// </summary>
    RgbaF32 = 2,
        
    /// <summary>
    /// The custom color encoding is defined undefined. The information how these
    /// colors are encoded must be implemented via external means.
    /// </summary>
    Custom = 3
}
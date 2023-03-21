namespace TinyVgCore.DrawCommands;

/// <summary>
/// Drawing command type
/// </summary>
public enum TvgCommandType:byte
{
    /// <summary>
    /// This command determines the end of file.
    /// <p></p>
    /// Every byte after this command is considered not part of the TinyVG data and can be used for other
    /// purposes like metadata or similar.
    /// </summary>
    EndOfDocument = 0,

    /// <summary>
    /// This command fills an N-gon
    /// </summary>
    FillPolygon = 1,
        
    /// <summary>
    /// This command fills a set of rectangles
    /// </summary>
    FillRectangles = 2,
        
    /// <summary>
    /// This command fills a free-form path
    /// </summary>
    FillPath = 3,

    /// <summary>
    /// This command draws a set of lines
    /// </summary>
    DrawLines = 4,
        
    /// <summary>
    /// This command draws the outline of a polygon
    /// </summary>
    DrawLineLoop = 5,
        
    /// <summary>
    /// This command draws a list of end-to-end lines
    /// </summary>
    DrawLineStrip = 6,
        
    /// <summary>
    /// This command draws a free-form path
    /// </summary>
    DrawLinePath = 7,

    /// <summary>
    /// This command draws a filled polygon with an outline
    /// </summary>
    OutlineFillPolygon = 8,
        
    /// <summary>
    /// This command draws several filled rectangles with an outline
    /// </summary>
    OutlineFillRectangles = 9,
        
    /// <summary>
    /// This command combines the fill and draw line path command into one
    /// </summary>
    OutlineFillPath = 10,
}
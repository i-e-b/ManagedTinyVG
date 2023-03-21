namespace TinyVgCore.Helpers;

/// <summary>
/// Writes bits and bytes to an output
/// </summary>
public interface IBitwiseWriter
{
    /// <summary>
    /// Write the current pending output byte (if any)
    /// </summary>
    void Flush();

    /// <summary>
    /// Write a single bit value to the stream
    /// </summary>
    void WriteBit(bool value);

    /// <summary>
    /// Write a single bit value to the stream
    /// </summary>
    void WriteBit(int value);

    /// <summary>
    /// Write 8 bits to the stream. These might not be aligned to a byte boundary
    /// </summary>
    void WriteByteUnaligned(byte value);

    /// <summary>
    /// Write 8 bits to the stream. These will be aligned to a byte boundary. Extra zero bits may be inserted to force alignment
    /// </summary>
    void WriteByteAligned(byte value);

    /// <summary>
    /// Seek underlying stream to start
    /// </summary>
    void Rewind();
}
namespace TinyVgCore.Helpers;

/// <summary>
/// Read bits and bytes from a stream or enumerator
/// </summary>
public interface IBitwiseReader
{
    /// <summary>
    /// Read a single bit value from the stream.
    /// Returns 1 or 0. Will return all zeros during run-out.
    /// </summary>
    int ReadBit();

    /// <summary>
    /// Read a single bit value from the stream.
    /// Returns true if data can be read. Does not include run-out
    /// </summary>
    bool TryReadBit(out int b);

    /// <summary>
    /// Read 8 bits from the stream. These might not be aligned to a byte boundary
    /// </summary>
    /// <returns></returns>
    byte ReadByteUnaligned();

    /// <summary>
    /// Returns true if all real data has been consumed.
    /// Ignores run-out data 
    /// </summary>
    bool IsEmpty();

    /// <summary>
    /// Returns true if bits can be read, including run-out bits
    /// </summary>
    bool CanRead();
}
using System;
using System.Collections.Generic;

namespace TinyVgCore;

/// <summary>
/// A bitwise wrapper around a byte stream. Also provides run-out
/// </summary>
public class BitwiseByteWrapper : IBitwiseReader, IBitwiseWriter
{
    private readonly IEnumerable<byte> _original;
    private readonly IEnumerator<byte> _enumerator;
    private List<byte>? _output = null;
    private int _runOutBits;

    private bool _inRunOut;
    private byte _readMask, _writeMask;
    private int _nextOut, _currentIn;
    private bool _more;

    /// <summary>
    /// Wrap a stream to read and write individual bits
    /// </summary>
    /// <param name="original">Stream that is the source/sink for data</param>
    /// <param name="runOutBits">number of zero bits to make readable after the end of the stream</param>
    public BitwiseByteWrapper(IEnumerable<byte> original, int runOutBits)
    {
        _original = original ?? throw new Exception("Must not wrap a null enumerable");
        _enumerator = original.GetEnumerator();
        _more = _enumerator.MoveNext();
        _runOutBits = runOutBits;

        _inRunOut = false;
        _readMask = 1;
        _writeMask = 0x80;
        _nextOut = 0;
        _currentIn = 0;
    }

    /// <summary>
    /// Write the current pending output byte (if any)
    /// </summary>
    public void Flush() {
        if (_writeMask == 0x80) return; // no pending byte
        WriteByte((byte)_nextOut);
        _writeMask = 0x80;
        _nextOut = 0;
    }

    private void WriteByte(byte nextOut)
    {
        _output ??= new();
        _output.Add(nextOut);
    }

    /// <summary>
    /// Write a single bit value to the stream
    /// </summary>
    public void WriteBit(bool value){
        if (value) _nextOut |= _writeMask;
        _writeMask >>= 1;

        if (_writeMask == 0)
        {
            WriteByte((byte)_nextOut);
            _writeMask = 0x80;
            _nextOut = 0;
        }
    }
        
    /// <summary>
    /// Write a single bit value to the stream
    /// </summary>
    public void WriteBit(int value){
        if (value != 0) _nextOut |= _writeMask;
        _writeMask >>= 1;

        if (_writeMask == 0)
        {
            WriteByte((byte)_nextOut);
            _writeMask = 0x80;
            _nextOut = 0;
        }
    }

    /// <summary>
    /// Read a single bit value from the stream.
    /// Returns 1 or 0. Will return all zeros during run-out.
    /// </summary>
    public int ReadBit()
    {
        if (_inRunOut)
        {
            if (_runOutBits-- > 0) return 0;
            throw new Exception("End of input stream");
        }

        if (_readMask == 1)
        {
            _currentIn = ReadByte();
            if (_currentIn < 0)
            {
                _inRunOut = true;
                if (_runOutBits-- > 0) return 0;
                throw new Exception("End of input stream");
            }
            _readMask = 0x80;
        }
        else
        {
            _readMask >>= 1;
        }
        return ((_currentIn & _readMask) != 0) ? 1 : 0;
    }
        
    /// <summary>
    /// Read a single bit value from the stream.
    /// Returns true if data can be read. Does not include run-out
    /// </summary>
    public bool TryReadBit(out int b)
    {
        b=0;
        if (_inRunOut) { return false; }
        if (_readMask == 1)
        {
            _currentIn = ReadByte();
            if (_currentIn < 0) { _inRunOut = true; return false; }
            _readMask = 0x80;
        }
        else
        {
            _readMask >>= 1;
        }
        b=((_currentIn & _readMask) != 0) ? 1 : 0;
        return true;
    }

    private int ReadByte()
    {
        if (!_more) return -1;
        var v = _enumerator.Current;
        _more = _enumerator.MoveNext();
        return v;
    }

    /// <summary>
    /// Read 8 bits from the stream. These might not be aligned to a byte boundary
    /// </summary>
    /// <returns></returns>
    public byte ReadByteUnaligned() {
        byte b = 0;
        for (int i = 0x80; i != 0; i >>= 1)
        {
            if (!TryReadBit(out var v)) break;
            b |= (byte)(i * v);
        }
        return b;
    }

    /// <summary>
    /// Write 8 bits to the stream. These might not be aligned to a byte boundary
    /// </summary>
    public void WriteByteUnaligned(byte value) {
        for (int i = 0x80; i != 0; i >>= 1)
        {
            WriteBit((value & i) != 0);
        }
    }
        
    /// <summary>
    /// Write 8 bits to the stream. These will be aligned to a byte boundary. Extra zero bits may be inserted to force alignment
    /// </summary>
    public void WriteByteAligned(byte value) {
        Flush();
        WriteByte(value);
    }

    /// <summary>
    /// Seek underlying stream to start
    /// </summary>
    public void Rewind()
    {
        if (_output is not null) _output.Clear();
        
        _inRunOut = false;
        _readMask = 1;
        _writeMask = 0x80;
        _nextOut = 0;
        _currentIn = 0;
    }

    /// <summary>
    /// Returns true if all real data has been consumed.
    /// Ignores run-out data 
    /// </summary>
    public bool IsEmpty()
    {
        return _inRunOut;
    }

    /// <summary>
    /// Returns true if bits can be read, including run-out bits
    /// </summary>
    public bool CanRead()
    {
        return _more || _runOutBits > 0;
    }
}
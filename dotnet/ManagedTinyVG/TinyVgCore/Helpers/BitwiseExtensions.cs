using System.Runtime.InteropServices;

namespace TinyVgCore.Helpers;

/// <summary>
/// Extensions to IBitwiseReader
/// </summary>
public static class BitwiseExtensions
{
    /// <summary>
    /// Read an integer from an arbitrary number of bits, less than 8
    /// </summary>
    public static int TinyInt(this IBitwiseReader rdr, int bits)
    {
        var accum = 0;
        for (int i = 0; i < bits; i++)
        {
            accum <<= 1;
            accum |= rdr.ReadBit();
        }
        return accum;
    }

    /// <summary>
    /// Read an unsigned 8 bit integer
    /// </summary>
    public static uint Uint8(this IBitwiseReader rdr)
    {
        return rdr.ReadByteUnaligned();
    }

    /// <summary>
    /// Read an unsigned 16 bit integer
    /// </summary>
    public static uint Uint16(this IBitwiseReader rdr)
    {
        uint a = rdr.ReadByteUnaligned();
        a |= ((uint)rdr.ReadByteUnaligned()) << 8;
        return a;
    }

    /// <summary>
    /// Read an unsigned 32 bit integer
    /// </summary>
    public static uint Uint32(this IBitwiseReader rdr)
    {
        uint a = rdr.ReadByteUnaligned();
        a |= ((uint)rdr.ReadByteUnaligned()) << 8;
        a |= ((uint)rdr.ReadByteUnaligned()) << 16;
        a |= ((uint)rdr.ReadByteUnaligned()) << 24;
        return a;
    }

    /// <summary>
    /// Read a float
    /// </summary>
    public static float Float32(this IBitwiseReader rdr)
    {
        uint a = rdr.ReadByteUnaligned();
        a |= ((uint)rdr.ReadByteUnaligned()) << 8;
        a |= ((uint)rdr.ReadByteUnaligned()) << 16;
        a |= ((uint)rdr.ReadByteUnaligned()) << 24;
            
        return FloatToInt.Convert(a);
    }
        
    /// <summary>
    /// This craziness reduces buffer noise over <c>BitConverter.ToSingle(..., 0);</c>
    /// and doesn't require the 'unsafe' flag like <c>*(float*)(&amp;value);</c>
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    struct FloatToInt 
    {
        [FieldOffset(0)]private float f;
        [FieldOffset(0)]private uint i;
            
        public static float Convert(uint value)
        {
            return new FloatToInt { i = value }.f;
        }
    }

    /// <summary>
    /// Read a variable length unsigned integer
    /// </summary>
    public static ulong VarUint(this IBitwiseReader rdr)
    {
        var shift = 0;
        ulong result = 0;
        while (shift < 64)
        {
            var b = rdr.ReadByteUnaligned();
            result |= ((ulong)(b & 0x7F)) << (shift);
            if ((b & 0x80) == 0) break;
            shift += 7;
        }

        return result;
    }
}
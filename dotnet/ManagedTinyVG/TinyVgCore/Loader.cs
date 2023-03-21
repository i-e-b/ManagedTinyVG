using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace TinyVgCore
{
    /// <summary>
    /// Reads TVG binary files into <see cref="TvgDocument"/>
    /// </summary>
    public static class TvgLoad
    {

        /// <summary>
        /// Convert bytes into a <see cref="TvgDocument"/>
        /// </summary>
        public static TvgDocument FromByteArray(IEnumerable<byte> array)
        {
            return FromBitwiseReader(new BitwiseByteWrapper(array, 8));
        }
        
        /// <summary>
        /// Convert bytes into a <see cref="TvgDocument"/>
        /// </summary>
        public static TvgDocument FromStream(Stream stream)
        {
            return FromBitwiseReader(new BitwiseStreamWrapper(stream, 8));
        }

        private static TvgDocument FromBitwiseReader(IBitwiseReader reader)
        {
            var doc = new TvgDocument();
            
            // expect 'rV' at start
            var magic1 = reader.ReadByteUnaligned();
            var magic2 = reader.ReadByteUnaligned();
            if (magic1 != 0x72 || magic2 != 0x56) throw new Exception("Not a valid TinyVg document");
            
            var version = reader.ReadByteUnaligned();
            if (version != 1) throw new Exception("Later version than this loader can process");
            
            ReadHeader(reader, doc);
            ReadColorTable(reader, doc);

            return doc;
        }

        private static void ReadColorTable(IBitwiseReader reader, TvgDocument doc)
        {
            var colorCount = reader.VarUint();
            doc.CreateColorTable(colorCount);

            switch (doc.ColorEncoding)
            {
                case TvgColorEncoding.Rgba8888:
                    ReadColorTableRgba8888(colorCount, reader, doc);
                    break;
                case TvgColorEncoding.Rgb565:
                    ReadColorTableRgb565(colorCount, reader, doc);
                    break;
                case TvgColorEncoding.RgbaF32:
                    ReadColorTableRgbaF32(colorCount, reader, doc);
                    break;
                case TvgColorEncoding.Custom:
                default:
                    throw new Exception("Could not interpret color encoding");
            }
        }

        private static void ReadHeader(IBitwiseReader reader, TvgDocument doc)
        {
            // Note: the spec seems to have this backwards, and so does the source.
            // I think this might be an oddity of Zig 'packed struct'
            // Seems like bitwise packing is done LSB first in Zig, where my reader does MSB first
            doc.CoordinateRange = (TvgCoordinateRange)reader.TinyInt(2);
            doc.ColorEncoding = (TvgColorEncoding)reader.TinyInt(2);
            doc.FractionBits = reader.TinyInt(4);

            switch (doc.CoordinateRange)
            {
                case TvgCoordinateRange.Default:
                    doc.Width = reader.Uint16();
                    doc.Height = reader.Uint16();
                    break;
                case TvgCoordinateRange.Reduced:
                    doc.Width = reader.Uint8();
                    doc.Height = reader.Uint8();
                    break;
                case TvgCoordinateRange.Enhanced:
                    doc.Width = reader.Uint32();
                    doc.Height = reader.Uint32();
                    break;
                default:
                    throw new Exception("Could not interpret coordinate range");
            }
        }

        private static void ReadColorTableRgba8888(ulong colorCount, IBitwiseReader reader, TvgDocument doc)
        {
            for (ulong i = 0; i < colorCount; i++)
            {
                var color = TvgColor.FromRgba8888(red: reader.ReadByteUnaligned(), green: reader.ReadByteUnaligned(),
                    blue: reader.ReadByteUnaligned(), alpha: reader.ReadByteUnaligned());
                
                doc.SetColor(i, color);
            }
        }
        
        private static void ReadColorTableRgb565(ulong colorCount, IBitwiseReader reader, TvgDocument doc)
        {
            for (ulong i = 0; i < colorCount; i++)
            {
                var color = TvgColor.FromRgbaRgb565(left: reader.ReadByteUnaligned(), right: reader.ReadByteUnaligned());
                doc.SetColor(i, color);
            }
        }
        
        private static void ReadColorTableRgbaF32(ulong colorCount, IBitwiseReader reader, TvgDocument doc)
        {
            for (ulong i = 0; i < colorCount; i++)
            {
                var color = new TvgColor{
                    Red = reader.Float32(),
                    Green = reader.Float32(),
                    Blue = reader.Float32(),
                    Alpha = reader.Float32()
                };
                doc.SetColor(i, color);
            }
        }
    }

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
}
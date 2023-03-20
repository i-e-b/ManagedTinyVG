using System;
using System.Collections.Generic;
using System.IO;

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
            if (magic1 != 0x72 || magic2 != 0x56) throw new Exception("File is not a valid TinyVg document");
            
            return doc;
        }
    }
}
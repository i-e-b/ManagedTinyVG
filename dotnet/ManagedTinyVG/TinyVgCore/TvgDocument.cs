using System;
using System.Collections.Generic;
using TinyVgCore.BasicTypes;
using TinyVgCore.DrawCommands;

namespace TinyVgCore
{
    /// <summary>
    /// Representation of a TVG document
    /// </summary>
    public class TvgDocument
    {
        private readonly List<TvgColor> _colorTable;
        private readonly List<TvgCommand> _drawCommands;

        /// <summary>
        /// Create a new blank document
        /// </summary>
        public TvgDocument()
        {
            _drawCommands = new();
            _colorTable = new List<TvgColor>();
        }

        /// <summary>
        /// Defines the number of fraction bits in a Unit value
        /// </summary>
        public int FractionBits { get; set; }

        /// <summary>
        /// Defines the type of color information that is used in the color table
        /// </summary>
        public TvgColorEncoding ColorEncoding { get; set; }

        /// <summary>
        /// The coordinate range defines how many bits a Unit value uses
        /// </summary>
        public TvgCoordinateRange CoordinateRange { get; set; }

        /// <summary>
        ///  Encodes the maximum width of the output file in display units.
        /// A value of 0 indicates that the image has the maximum possible
        /// width. The size of this field depends on the coordinate range field.
        /// </summary>
        public uint Width { get; set; }
        
        /// <summary>
        /// Encodes the maximum height of the output file in display units.
        /// A value of 0 indicates that the image has the maximum possible
        /// height. The size of this field depends on the coordinate range field.
        /// </summary>
        public uint Height { get; set; }

        /// <summary>
        /// Initialise the document's color table with a fixed number of
        /// entries. All entries will be initially set to zero-value (black)
        /// </summary>
        public void CreateColorTable(ulong colorCount)
        {
            if (_colorTable.Count > 0) throw new Exception("Color table has already been initialised");
            for (ulong i = 0; i < colorCount; i++)
            {
                _colorTable.Add(TvgColor.Black);
            }
        }

        /// <summary>
        /// Change an existing color in the color table
        /// </summary>
        public void SetColor(ulong index, TvgColor color)
        {
            if (index >= (ulong)_colorTable.Count) throw new Exception($"{nameof(SetColor)} called on index outside of color palette");
            _colorTable[(int)index] = color;
        }

        /// <summary>
        /// Add a command to the drawing list
        /// </summary>
        public void AddCommand(TvgCommand tvgCommand)
        {
            _drawCommands.Add(tvgCommand);
        }
    }

    /// <summary>
    /// A standardised color. All values are in the range 0..1
    /// </summary>
    public class TvgColor
    {
        /// <summary>Red component 0..1</summary>
        public double Red { get; set; }
        /// <summary>Green component 0..1</summary>
        public double Green { get; set; }
        /// <summary>Blue component 0..1</summary>
        public double Blue { get; set; }
        /// <summary>Alpha component 0..1</summary>
        public double Alpha { get; set; }

        /// <summary>
        /// Solid black color
        /// </summary>
        public static TvgColor Black => new TvgColor(alpha:1.0, red:0.0, green:0.0, blue:0.0);

        /// <summary>
        /// Create a new color from components
        /// </summary>
        public TvgColor(double alpha, double red, double green, double blue)
        {
            Alpha = alpha;
            Red = red;
            Green = green;
            Blue = blue;
        }

        /// <summary>
        /// Create a new color. Values default to transparent black.
        /// </summary>
        public TvgColor() { }

        /// <summary>
        /// Convert RGBA 8888 byte values to a standardised color
        /// </summary>
        public static TvgColor FromRgba8888(byte red, byte green, byte blue, byte alpha)
        {
            return new TvgColor
            {
                Red = red / 255.0,
                Green = green / 255.0,
                Blue = blue / 255.0,
                Alpha = alpha / 255.0
            };
        }

        /// <summary>
        /// Convert RGB 565 byte values to a standardised color
        /// </summary>
        public static TvgColor FromRgbaRgb565(byte left, byte right)
        {
            var r = (left >> 3) & 0x1F;
            var g = ((left & 0x07) << 3) | ((right >> 5) & 0x07);
            var b = right & 0x1F;
            
            return new TvgColor
            {
                Red = r / 31.0,
                Green = g / 63.0,
                Blue = b / 31.0,
                Alpha = 1.0
            };
        }
    }
}
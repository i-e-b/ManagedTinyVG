using System;
using System.Collections.Generic;
using System.IO;
using TinyVgCore.BasicTypes;
using TinyVgCore.DrawCommands;
using TinyVgCore.Helpers;
using TinyVgCore.Paths;

namespace TinyVgCore;

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
        return FromBitwiseReader(new BitwiseByteWrapper(array, 512));
    }

    /// <summary>
    /// Convert bytes into a <see cref="TvgDocument"/>
    /// </summary>
    public static TvgDocument FromStream(Stream stream)
    {
        return FromBitwiseReader(new BitwiseStreamWrapper(stream, 512));
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

        TvgCommand lastCommand = new TvgCmdNothing();
        var scale = 1.0 / (1 << doc.FractionBits);
        while (lastCommand is not TvgCmdEndOfDocument) // the run-out zeros will cause this eventually
        {
            lastCommand = ReadCommand(reader, scale, doc.CoordinateRange);
            doc.AddCommand(lastCommand);
        }

        return doc;
    }

    private static TvgCommand ReadCommand(IBitwiseReader reader, double scale, TvgCoordinateRange unitSize)
    {
        var style = (TvgFillStyleType)reader.TinyInt(2);
        var cmd = (TvgCommandType)reader.TinyInt(6);

        return cmd switch
        {
            TvgCommandType.EndOfDocument => new TvgCmdEndOfDocument(),
            TvgCommandType.DrawLines => ReadCmdDrawLines(reader, style, scale, unitSize),
            TvgCommandType.DrawLineLoop => ReadCmdDrawLineLoop(reader, style, scale, unitSize),
            TvgCommandType.DrawLineStrip => ReadCmdDrawLineStrip(reader, style, scale, unitSize),
            TvgCommandType.FillPolygon => ReadCmdFillPoly(reader, style, scale, unitSize),
            TvgCommandType.OutlineFillPolygon => ReadCmdOutlineFillPolygon(reader, style, scale, unitSize),
            TvgCommandType.FillRectangles => ReadCmdFillRects(reader, style, scale, unitSize),
            TvgCommandType.OutlineFillRectangles => ReadCmdOutlineFillRectangles(reader, style, scale, unitSize),
            TvgCommandType.DrawLinePath => ReadCmdDrawLinePath(reader, style, scale, unitSize),
            TvgCommandType.FillPath => ReadCmdFillPath(reader, style, scale, unitSize),
            TvgCommandType.OutlineFillPath => ReadCmdOutlineFillPath(reader, style, scale, unitSize),
            _ => throw new Exception($"Unknown command {(int)cmd}")
        };
    }
    
    private static TvgCommand ReadCmdOutlineFillPath(IBitwiseReader reader, TvgFillStyleType fillStyleType, double scale, TvgCoordinateRange unitSize)
    {
        var lineStyleType = (TvgFillStyleType)reader.TinyInt(2);
        var segCount = reader.TinyInt(6)+1;
        
        var cmd = new TvgCmdOutlineFillPath {
            SegmentCount = (ulong)segCount,
            FillStyle = ReadStyle(fillStyleType, reader, scale, unitSize),
            LineStyle = ReadStyle(lineStyleType, reader, scale, unitSize),
            LineWidth = ReadUnit(reader, scale, unitSize),
        };

        // read segments' command counts
        var commandCounts = new List<ulong>();
        var segmentCount = (int)cmd.SegmentCount;
        for (int i = 0; i < segmentCount; i++)
        {
            commandCounts.Add(reader.VarUint() + 1);
        }
        
        var lineWidth = cmd.LineWidth;
        for (int i = 0; i < segmentCount; i++)
        {
            cmd.Path.Add(ReadSegment(reader, ref lineWidth, scale, unitSize, commandCount: commandCounts[i]));
        }
        
        return cmd;
    }

    private static TvgCommand ReadCmdFillPath(IBitwiseReader reader, TvgFillStyleType style, double scale, TvgCoordinateRange unitSize)
    {
        var cmd = new TvgCmdFillPath {
            SegmentCount = reader.VarUint() + 1,
            FillStyle = ReadStyle(style, reader, scale, unitSize),
        };

        // read segments' command counts
        var commandCounts = new List<ulong>();
        var segmentCount = (int)cmd.SegmentCount;
        for (int i = 0; i < segmentCount; i++)
        {
            commandCounts.Add(reader.VarUint() + 1);
        }
        
        var lineWidth = 0.0;
        for (int i = 0; i < segmentCount; i++)
        {
            cmd.Path.Add(ReadSegment(reader, ref lineWidth, scale, unitSize, commandCount: commandCounts[i]));
        }
        
        return cmd;
    }

    private static TvgCommand ReadCmdDrawLinePath(IBitwiseReader reader, TvgFillStyleType style, double scale, TvgCoordinateRange unitSize)
    {
        var cmd = new TvgCmdDrawLinePath {
            SegmentCount = reader.VarUint() + 1,
            LineStyle = ReadStyle(style, reader, scale, unitSize),
            LineWidth = ReadUnit(reader, scale, unitSize),
        };

        // read segments' command counts
        var commandCounts = new List<ulong>();
        var segmentCount = (int)cmd.SegmentCount;
        for (int i = 0; i < segmentCount; i++)
        {
            commandCounts.Add(reader.VarUint() + 1);
        }
        
        var lineWidth = cmd.LineWidth;
        for (int i = 0; i < segmentCount; i++)
        {
            cmd.Path.Add(ReadSegment(reader, ref lineWidth, scale, unitSize, commandCount: commandCounts[i]));
        }
        
        return cmd;
    }

    private static TvgPathSegment ReadSegment(IBitwiseReader reader, ref double lineWidth, double scale, TvgCoordinateRange unitSize, ulong commandCount)
    {
        var seg = new TvgPathSegment();
        
        var startPoint = ReadPoint(reader, scale, unitSize);
        
        seg.StartPoint = startPoint;
        seg.LineWidth = lineWidth;

        var numCommands = (int)commandCount;
        for (int i = 0; i < numCommands; i++)
        {
            var p2 = reader.TinyInt(3);
            var hasLineWidth = reader.TinyInt(1);
            var p1 = reader.TinyInt(1);
            var instruction = (TvgPathInstruction)reader.TinyInt(3);
            if (p1 != 0 || p2 != 0) throw new Exception("Path tag padding was non-zero");

            if (hasLineWidth == 1)
            {
                lineWidth = ReadUnit(reader, scale, unitSize);
            }

            seg.Commands.Add(ReadPathSegmentCommand(instruction, reader, scale, unitSize));
        }
        
        return seg;
    }

    private static TvgPathSegmentCommand ReadPathSegmentCommand(TvgPathInstruction instruction,IBitwiseReader reader, double scale, TvgCoordinateRange unitSize)
    {
        switch (instruction)
        {
            case TvgPathInstruction.Line:
                return new TvgPscLine{
                    Position = ReadPoint(reader, scale, unitSize)
                };
                
            case TvgPathInstruction.HorizontalLine:
                return new TvgPscHorizontalLine{
                    X = ReadUnit(reader, scale, unitSize)
                };
            
            case TvgPathInstruction.VerticalLine:
                return new TvgPscVerticalLine{
                    Y = ReadUnit(reader, scale, unitSize)
                };
            
            case TvgPathInstruction.CubicBezier:
                return new TvgPscCubicBezier{
                    Control0 = ReadPoint(reader, scale, unitSize),
                    Control1 = ReadPoint(reader, scale, unitSize),
                    EndPoint = ReadPoint(reader, scale, unitSize),
                };

            case TvgPathInstruction.ArcCircle:
            {
                var padding = reader.TinyInt(6); // should be zero
                if (padding != 0) throw new Exception("Arc circle padding not zero");
                
                var sweepLeft = reader.TinyInt(1) == 1;
                var largeArc = reader.TinyInt(1) == 1;
                return new TvgPscArcCircle
                {
                    LargeArc = largeArc,
                    SweepLeft = sweepLeft,
                    Radius = ReadUnit(reader, scale, unitSize),
                    Target = ReadPoint(reader, scale, unitSize)
                };
            }
            case TvgPathInstruction.ArcEllipse:
            {
                var padding = reader.TinyInt(6); // should be zero
                if (padding != 0) throw new Exception("Arc circle padding not zero");
                
                var sweepLeft = reader.TinyInt(1) == 1;
                var largeArc = reader.TinyInt(1) == 1;
                return new TvgPscArcEllipse
                {
                    LargeArc = largeArc,
                    SweepLeft = sweepLeft,
                    RadiusX = ReadUnit(reader, scale, unitSize),
                    RadiusY = ReadUnit(reader, scale, unitSize),
                    Rotation = ReadUnit(reader, scale, unitSize),
                    Target = ReadPoint(reader, scale, unitSize)
                };
            }
            case TvgPathInstruction.ClosePath:return new TvgPscClosePath();
            
            case TvgPathInstruction.QuadraticBezier:
                return new TvgPscQuadraticBezier{
                    Control0 = ReadPoint(reader, scale, unitSize),
                    EndPoint = ReadPoint(reader, scale, unitSize),
                };
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static TvgCommand ReadCmdDrawLineLoop(IBitwiseReader reader, TvgFillStyleType styleType, double scale, TvgCoordinateRange unitSize)
    {
        var cmd = new TvgCmdDrawLineLoop
        {
            PrimaryStyleType = styleType,
            PointCount = reader.VarUint() + 1,
            LineStyle = ReadStyle(styleType, reader, scale, unitSize),
            LineWidth = ReadUnit(reader, scale, unitSize)
        };

        var points = (int)cmd.PointCount;
        for (var i = 0; i < points; i++)
        {
            cmd.Points.Add(ReadPoint(reader, scale, unitSize));
        }
        
        return cmd;
    }
    
    private static TvgCommand ReadCmdDrawLineStrip(IBitwiseReader reader, TvgFillStyleType styleType, double scale, TvgCoordinateRange unitSize)
    {
        var cmd = new TvgCmdDrawLineStrip
        {
            PrimaryStyleType = styleType,
            PointCount = reader.VarUint() + 1,
            LineStyle = ReadStyle(styleType, reader, scale, unitSize),
            LineWidth = ReadUnit(reader, scale, unitSize)
        };

        var points = (int)cmd.PointCount;
        for (var i = 0; i < points; i++)
        {
            cmd.Points.Add(ReadPoint(reader, scale, unitSize));
        }
        
        return cmd;
    }

    private static TvgCommand ReadCmdDrawLines(IBitwiseReader reader, TvgFillStyleType styleType, double scale, TvgCoordinateRange unitSize)
    {
        var cmd = new TvgCmdDrawLines{
            LineCount = reader.VarUint() + 1,
            LineStyle = ReadStyle(styleType, reader, scale, unitSize),
            LineWidth = ReadUnit(reader, scale, unitSize)
        };

        var lines = (int)cmd.LineCount;
        for (var i = 0; i < lines; i++)
        {
            cmd.Lines.Add(ReadLine(reader, scale, unitSize));
        }
        
        return cmd;
    }

    private static TvgCommand ReadCmdOutlineFillRectangles(IBitwiseReader reader, TvgFillStyleType primaryStyleType, double scale, TvgCoordinateRange unitSize)
    {
        var cmd = new TvgCmdOutlineFillRectangles();
            
        var secondaryStyleType = (TvgFillStyleType)reader.TinyInt(2);
        cmd.RectangleCount = (ulong)(reader.TinyInt(6) + 1);

        cmd.FillStyle = ReadStyle(primaryStyleType, reader, scale, unitSize);
        cmd.LineStyle = ReadStyle(secondaryStyleType, reader, scale, unitSize);
        cmd.LineWidth = ReadUnit(reader, scale, unitSize);

        var rects = (int)cmd.RectangleCount;
        for (var i = 0; i < rects; i++)
        {
            cmd.Rectangles.Add(ReadRect(reader, scale, unitSize));
        }

        return cmd;
    }

    private static TvgCommand ReadCmdOutlineFillPolygon(IBitwiseReader reader, TvgFillStyleType primaryStyleType, double scale, TvgCoordinateRange unitSize)
    {
        var cmd = new TvgCmdOutlineFillPolygon();
            
        var secondaryStyleType = (TvgFillStyleType)reader.TinyInt(2);
        cmd.SegmentCount = (ulong)(reader.TinyInt(6) + 1);

        cmd.FillStyle = ReadStyle(primaryStyleType, reader, scale, unitSize);
        cmd.LineStyle = ReadStyle(secondaryStyleType, reader, scale, unitSize);
        cmd.LineWidth = ReadUnit(reader, scale, unitSize);

        var rects = (int)cmd.SegmentCount;
        for (var i = 0; i < rects; i++)
        {
            cmd.Points.Add(ReadPoint(reader, scale, unitSize));
        }

        return cmd;
    }
    
    private static TvgCommand ReadCmdFillPoly(IBitwiseReader reader, TvgFillStyleType styleType, double scale, TvgCoordinateRange unitSize)
    {
        var cmd = new TvgCmdFillPolygon
        {
            PrimaryStyleType = styleType,
            PointCount = reader.VarUint() + 1,
            Style = ReadStyle(styleType, reader, scale, unitSize)
        };

        var points = (int)cmd.PointCount;
        for (var i = 0; i < points; i++)
        {
            cmd.Points.Add(ReadPoint(reader, scale, unitSize));
        }

        return cmd;
    }

    private static TvgCommand ReadCmdFillRects(IBitwiseReader reader, TvgFillStyleType styleType, double scale, TvgCoordinateRange unitSize)
    {
        var cmd = new TvgCmdFillRectangles
        {
            PrimaryStyleType = styleType,
            RectangleCount = reader.VarUint() + 1,
            Style = ReadStyle(styleType, reader, scale, unitSize)
        };

        var rects = (int)cmd.RectangleCount;
        for (var i = 0; i < rects; i++)
        {
            cmd.Rectangles.Add(ReadRect(reader, scale, unitSize));
        }

        return cmd;
    }

    private static double ReadUnit(IBitwiseReader reader, double scale, TvgCoordinateRange unitSize)
    {
        return unitSize switch
        {
            TvgCoordinateRange.Reduced => reader.Uint8() * scale,
            TvgCoordinateRange.Default => reader.Uint16() * scale,
            TvgCoordinateRange.Enhanced => reader.Uint32() * scale,
            _ => throw new Exception($"Unknown unit size {(int)unitSize}")
        };
    }

    private static TvgPoint ReadPoint(IBitwiseReader reader, double scale, TvgCoordinateRange unitSize)
    {
        return new TvgPoint{
            X = ReadUnit(reader, scale, unitSize),
            Y = ReadUnit(reader, scale, unitSize)
        };
    }
    
    private static TvgLine ReadLine(IBitwiseReader reader, double scale, TvgCoordinateRange unitSize)
    {
        return new TvgLine{
            Start = ReadPoint(reader, scale, unitSize),
            End = ReadPoint(reader, scale, unitSize)
        };
    }

    private static TvgRectangle ReadRect(IBitwiseReader reader, double scale, TvgCoordinateRange unitSize)
    {
        return new TvgRectangle{
            X = ReadUnit(reader, scale, unitSize),
            Y = ReadUnit(reader, scale, unitSize),
            Width = ReadUnit(reader, scale, unitSize),
            Height = ReadUnit(reader, scale, unitSize)
        };
    }

    private static TvgStyle ReadStyle(TvgFillStyleType type, IBitwiseReader reader, double scale, TvgCoordinateRange unitSize)
    {
        switch (type)
        {
            case TvgFillStyleType.Flat:
                return new TvgStyle
                {
                    FillType = type,
                    Color0 = reader.VarUint()
                };

            case TvgFillStyleType.Linear:
            case TvgFillStyleType.Radial:
                return new TvgStyle
                {
                    FillType = type,
                    Point0 = ReadPoint(reader, scale, unitSize),
                    Point1 = ReadPoint(reader, scale, unitSize),
                    Color0 = reader.VarUint(),
                    Color1 = reader.VarUint()
                };

            case TvgFillStyleType.Unknown:
            default:
                throw new Exception($"Unknown style {(int)type}");
        }
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
            var color = new TvgColor
            {
                Red = reader.Float32(),
                Green = reader.Float32(),
                Blue = reader.Float32(),
                Alpha = reader.Float32()
            };
            doc.SetColor(i, color);
        }
    }
}
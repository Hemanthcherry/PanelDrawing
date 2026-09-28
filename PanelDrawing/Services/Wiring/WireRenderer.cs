using PanelDrawing.Core.Constants;
using PanelDrawing.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PanelDrawing.Services.P2
{
    public static class WireRenderer
    {
        private const double CTypeOffset = 80;
        private const double SourceLabelOffset = 48;
        private const double DestinationLabelOffset = 25;
        private const double EndSymbolOffset = 16;
        private const double LabelToCoreOffset = 10;
        private const double CoreToLengthOffset = 2;

        public static double GetX3Value(string FromOrientation, string ToOrientation, double X1)
        {
            double bend;

            if (FromOrientation.Equals("R", StringComparison.OrdinalIgnoreCase) || ToOrientation.Equals("R", StringComparison.OrdinalIgnoreCase))
            {
                bend = CTypeOffset + PanelConstants.WiringOffset;
                return X1 - bend;
            }
            else if (string.IsNullOrWhiteSpace(FromOrientation) && string.IsNullOrWhiteSpace(ToOrientation) && X1 > CTypeOffset + PanelConstants.MarginX)
            {
                bend = CTypeOffset + PanelConstants.WiringOffset;
                return X1 - bend;
            }
            else
            {
                bend = CTypeOffset - PanelConstants.WiringOffset;
                return X1 + bend;
            }
        }

        public static void DrawWireEndSymbol(TextWriter writer, double x, double y, int direction, string wireSymbol, string wireType, string groupId)
        {
            double baseX = x + (EndSymbolOffset * direction);

            writer.WriteLine(FormattableString.Invariant($"ADD I2 {wireSymbol} :R0 {baseX},{y};"));
            writer.WriteLine("TESTDIS;");
            writer.WriteLine(FormattableString.Invariant($"MOD N250 {baseX + 2},{y + 1.5} 0,0 :L254 STOR_MID :E'{wireType}' JU; NOP;"));
            writer.WriteLine("TESTDIS_OFF;");
            writer.WriteLine(FormattableString.Invariant($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {baseX},{y} {baseX + 1.5},{y + 0.5};NOP;"));

            if (wireType.Equals("SS") || wireType.Equals("TP") ||
                wireType.Equals("STP") || wireType.Equals("COAX") ||
                wireType.Equals("TRIAX") || wireType.Equals("QUADRAX"))
            {
                writer.WriteLine(FormattableString.Invariant($"MOD N253 {baseX},{y} 0,0 STOR_MID :E'{groupId}' JU;NOP;"));
            }
        }

        // Centralized N54/N56/N252/N59/N254 label block shared by straight, C-type, and Z-type wires.
        // lengthExtra distinguishes the straight-wire layout (length aligned with core, lengthExtra=0)
        // from the C/Z-type layout (length offset 2 past core, lengthExtra=2).
        private static void WriteWireLabelBlock(TextWriter writer, double wireX, double wireY, double labelX, double coreY, double lengthExtra,
            string iWireCode, string iWireGauge, string wireTypeCoreNumber, string wireLength)
        {
            double gaugeX = labelX + CoreToLengthOffset;
            double coreX = labelX + LabelToCoreOffset;
            double lengthX = coreX + lengthExtra;

            writer.WriteLine(FormattableString.Invariant($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {wireX},{wireY} '{iWireCode}' {labelX},{wireY} ;NOP;"));
            writer.WriteLine(FormattableString.Invariant($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {wireX},{wireY} '#{iWireGauge}' {gaugeX},{wireY} ;"));
            writer.WriteLine(FormattableString.Invariant($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {wireX},{wireY} '{wireTypeCoreNumber}' {coreX},{coreY} ;"));
            writer.WriteLine(FormattableString.Invariant($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 {wireX},{wireY} '{wireLength}' {lengthX},{wireY} ;"));
            writer.WriteLine(FormattableString.Invariant($"ADD N254 :R0 :T3017 :AC L154 {wireX},{wireY} '' {lengthX},{wireY} ;"));
        }

        public static void DrawStraightWire(double X1, double Y1, double X2, double Y2, TextWriter writer, string iWireCode, string iWireGauge, string wire_Type_Core_Number, string wire_Length,
            string wire_symbol, string wire_Type, string groupId)
        {
            writer.WriteLine(FormattableString.Invariant($"{X1},{Y1} {X2},{Y2}"));

            // write wire data in middle of the wire
            writer.WriteLine(";;NOP;;");
            WriteWireLabelBlock(writer, X1, Y1, (X1 + X2) / 2, Y1 - 1, lengthExtra: 0, iWireCode, iWireGauge, wire_Type_Core_Number, wire_Length);

            // Source end symbols
            if (!PanelConstants.lst_WireCodes_Info_Processed.Contains(iWireCode))
            {
                DrawWireEndSymbol(writer, X1, Y1, +1, wire_symbol, wire_Type, groupId); // source
                writer.WriteLine(":GRI");
                DrawWireEndSymbol(writer, X2, Y2, -1, wire_symbol, wire_Type, groupId); // destination
            }
        }

        public static void Draw_C_TypeWire(double X1, double Y1, double X2, double Y2, double X3, double Y3, double X4, double Y4, string iWireCode, string iWireGauge, string wire_Type_Core_Number,
            string wire_Length, string FromOrientation, string ToOrientation, TextWriter writer, string wire_symbol, string wire_Type, string groupId)
        {
            writer.WriteLine(FormattableString.Invariant($"{X1},{Y1}"));
            writer.WriteLine(FormattableString.Invariant($"{X3},{Y3}"));
            writer.WriteLine(FormattableString.Invariant($"{X4},{Y4}"));
            writer.WriteLine(FormattableString.Invariant($"{X2},{Y2}"));
            writer.WriteLine(";;NOP;;");

            if (FromOrientation.Equals("R", StringComparison.OrdinalIgnoreCase) || ToOrientation.Equals("R", StringComparison.OrdinalIgnoreCase))
            {
                WriteWireLabelBlock(writer, X1, Y1, X1 - SourceLabelOffset, Y1 + 1, lengthExtra: 2, iWireCode, iWireGauge, wire_Type_Core_Number, wire_Length); // Source
                WriteWireLabelBlock(writer, X2 - DestinationLabelOffset, Y2, X2 - DestinationLabelOffset * 2, Y2 - 1, lengthExtra: 2, iWireCode, iWireGauge, wire_Type_Core_Number, wire_Length); // Destination
            }
            else if (string.IsNullOrWhiteSpace(FromOrientation) && string.IsNullOrWhiteSpace(ToOrientation) && X1 > CTypeOffset + PanelConstants.MarginX)
            {
                WriteWireLabelBlock(writer, X1, Y1, X1 - SourceLabelOffset, Y1 + 1, lengthExtra: 2, iWireCode, iWireGauge, wire_Type_Core_Number, wire_Length); // Source
                WriteWireLabelBlock(writer, X2 - DestinationLabelOffset, Y2, X2 - DestinationLabelOffset * 2, Y2 - 1, lengthExtra: 2, iWireCode, iWireGauge, wire_Type_Core_Number, wire_Length); // Destination
            }
            else if (FromOrientation.Equals("L", StringComparison.OrdinalIgnoreCase) || ToOrientation.Equals("L", StringComparison.OrdinalIgnoreCase))
            {
                WriteWireLabelBlock(writer, X1, Y1, X1 + SourceLabelOffset, Y1 - 1, lengthExtra: 2, iWireCode, iWireGauge, wire_Type_Core_Number, wire_Length); // Source
                WriteWireLabelBlock(writer, X2 + DestinationLabelOffset, Y2, X2 + DestinationLabelOffset * 2, Y2 - 1, lengthExtra: 2, iWireCode, iWireGauge, wire_Type_Core_Number, wire_Length); // Destination
            }

            if (!PanelConstants.lst_WireCodes_Info_Processed.Contains(iWireCode))
            {
                int Direction = FromOrientation.Equals("R", StringComparison.OrdinalIgnoreCase) || ToOrientation.Equals("R", StringComparison.OrdinalIgnoreCase) ? -1 : +1;

                if (string.IsNullOrWhiteSpace(FromOrientation) && string.IsNullOrWhiteSpace(ToOrientation) && X1 > CTypeOffset + PanelConstants.MarginX)
                {
                    Direction = -1;
                }
                DrawWireEndSymbol(writer, X1, Y1, Direction, wire_symbol, wire_Type, groupId); //source
                DrawWireEndSymbol(writer, X2, Y2, Direction, wire_symbol, wire_Type, groupId); // destination
            }
        }

        public static void Draw_Z_TypeWire(double X1, double Y1, double X2, double Y2, double X3, double Y3, double X4, double Y4, string iWireCode, string iWireGauge,
            string wire_Type_Core_Number, string wire_Length, TextWriter writer, string wire_symbol, string wire_Type, string groupId)
        {
            writer.WriteLine(FormattableString.Invariant($"{X1},{Y1}"));
            writer.WriteLine(FormattableString.Invariant($"{X3},{Y3}"));
            writer.WriteLine(FormattableString.Invariant($"{X4},{Y4}"));
            writer.WriteLine(FormattableString.Invariant($"{X2},{Y2}"));

            writer.WriteLine(";;NOP;;");
            WriteWireLabelBlock(writer, X1, Y1, X1 + SourceLabelOffset, Y1 - 1, lengthExtra: 2, iWireCode, iWireGauge, wire_Type_Core_Number, wire_Length); // Source
            WriteWireLabelBlock(writer, X2 - DestinationLabelOffset, Y2, X2 - DestinationLabelOffset * 2, Y2 - 1, lengthExtra: 2, iWireCode, iWireGauge, wire_Type_Core_Number, wire_Length); // Destination

            // Source end symbols
            if (!PanelConstants.lst_WireCodes_Info_Processed.Contains(iWireCode))
            {
                DrawWireEndSymbol(writer, X1, Y1, +1, wire_symbol, wire_Type, groupId); // source
                writer.WriteLine(":GRI");
                DrawWireEndSymbol(writer, X2, Y2, -1, wire_symbol, wire_Type, groupId); //destination
            }
        }

        public static double GetUniqueX3(double x3)
        {
            const double step = 2.0;
            const double tolerance = 2;

            while (PanelConstants.UsedX3Values.Any(v => Math.Abs(v - x3) <= tolerance))
            {
                x3 += step;
            }

            return x3;
        }

        public static void DrawWire(TextWriter writer, double X1, double Y1, double X2, double Y2, string iWireCode, string iWireGauge, string c1, string c2, string iF_Type, string iT_Type,
            string groupId, string wire_Length, string wire_Type, string wire_Type_Core_Number, string FromOrientation, string ToOrientation)
        {
            double X3 = 0, Y3 = 0, X4 = 0, Y4 = 0;

            ApplicationLogger.Info($"P2 Routing started for wire {iWireCode} " + $"from {c1} to {c2}");

            bool isCType = (X1 == X2) || Math.Abs(X1 - X2) < 50;

            string routeType = (Y1 == Y2) ? "STRAIGHT" : isCType ? "C-TYPE" : "Z-TYPE";

            ApplicationLogger.Info($"Wire {iWireCode} routing type decided as {routeType}");

            if (isCType) // C-Line
            {
                X3 = GetX3Value(FromOrientation, ToOrientation, X1);
            }
            else  // Z-line
            {
                if (Y1 <= Y2)
                {
                    X3 = (X1 + X2) / 2 + PanelConstants.WiringOffset;
                }
                else
                {
                    X3 = (X1 + X2) / 2 - PanelConstants.WiringOffset;
                }
            }
            X3 = GetUniqueX3(X3);
            PanelConstants.UsedX3Values.Add(X3);
            X4 = X3;

            Y3 = Y1;
            Y4 = Y2;

            string wire_symbol = PanelConstants.GetWireSymbol(wire_Type);
            try
            {
                writer.WriteLine("GRI 0.5, 2;");
                writer.WriteLine("ADD L154 :W0");

                if (Y1 == Y2) // Straight Line wiring if both source and destination components are in same horizontal location
                {
                    DrawStraightWire(X1, Y1, X2, Y2, writer, iWireCode, iWireGauge, wire_Type_Core_Number, wire_Length, wire_symbol, wire_Type, groupId);
                }
                else if (isCType) // C type wiring if Source and destination components are in same horizontal location with 50 margin
                {
                    Draw_C_TypeWire(X1, Y1, X2, Y2, X3, Y3, X4, Y4, iWireCode, iWireGauge, wire_Type_Core_Number, wire_Length, FromOrientation, ToOrientation, writer, wire_symbol, wire_Type, groupId);
                }
                else // Z shaped wire routing
                {
                    Draw_Z_TypeWire(X1, Y1, X2, Y2, X3, Y3, X4, Y4, iWireCode, iWireGauge, wire_Type_Core_Number, wire_Length, writer, wire_symbol, wire_Type, groupId);
                }

                PanelConstants.WiringOffset += 4;

                if (!PanelConstants.lst_WireCodes_Info_Processed.Contains(iWireCode))
                    PanelConstants.lst_WireCodes_Info_Processed.Add(iWireCode);

                ApplicationLogger.Info($"Wire {iWireCode} routed successfully " + $"from {c1} to {c2}");
            }
            catch (Exception ex)
            {
                ApplicationLogger.Error($"Wire {iWireCode} routing failed from {c1} to {c2}", ex);
                throw;
            }
        }
    }
}

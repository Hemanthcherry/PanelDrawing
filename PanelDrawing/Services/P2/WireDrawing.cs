using PanelDrawing.CommonOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;

namespace PanelDrawing.Services.P2
{
    public static class WireDrawing
    {      
        public static double GetX3Value(string FromOrientation, string ToOrientation, double X1)
        {
            double Offset = 80;
            double bend = 0;

            if (FromOrientation.Equals("R", StringComparison.OrdinalIgnoreCase) || ToOrientation.Equals("R", StringComparison.OrdinalIgnoreCase))
            {
                bend = Offset + Constants.WiringOffset;
                return X1 - bend;
            }
            else
            {
                bend = Offset - Constants.WiringOffset;
                return X1 + bend;
            }
        }

        public static void DrawWireEndSymbol(StreamWriter writer,double x,double y,int direction, string wireSymbol,string wireType,string groupId)
        {
            double baseX = x + (16 * direction);

            writer.WriteLine($"ADD I2 {wireSymbol} :R0 {baseX},{y};");
            writer.WriteLine($"TESTDIS;");
            writer.WriteLine($"MOD N250 {baseX + 2 },{y + 1.5} 0,0 :L254 STOR_MID :E'{wireType}' JU; NOP;");
            writer.WriteLine($"TESTDIS_OFF;");
            writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {baseX},{y} {baseX + 1.5},{y + 0.5};NOP;");

            if (wireType.Equals("SS") || wireType.Equals("TP") ||
                wireType.Equals("STP") || wireType.Equals("COAX") ||
                wireType.Equals("TRIAX") || wireType.Equals("QUADRAX"))
            {
                writer.WriteLine($"MOD N253 {baseX},{y} 0,0 STOR_MID :E'{groupId}' JU;NOP;");
            }
        }

        public static void DrawStraightWire(double X1, double Y1, double X2, double Y2, StreamWriter writer, string iWireCode, string iWireGauge, string wire_Type_Core_Number, string wire_Length,
            string wire_symbol, string wire_Type, string groupId)
        {
            writer.WriteLine($"{X1},{Y1} {X2},{Y2}");

            // write wire data in middle of the wire
            writer.WriteLine($";;NOP;;");
            writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {X1},{Y1} '{iWireCode}' {(X1 + X2) / 2},{Y1} ;NOP;");
            writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {X1},{Y1} '#{iWireGauge}' {(X1 + X2) / 2 + 2},{Y1} ;");
            writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {X1},{Y1} '{wire_Type_Core_Number}' {(X1 + X2) / 2 + 10},{Y1 - 1} ;");
            writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 {X1},{Y1} '{wire_Length}' {(X1 + X2) / 2 + 10},{Y1} ;");
            writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {X1},{Y1} '' {(X1 + X2) / 2 + 10},{Y1} ;");

            // Source end symbols
            if (!Constants.lst_WireCodes_Info_Processed.Contains(iWireCode))
            {
                DrawWireEndSymbol(writer, X1, Y1, +1, wire_symbol, wire_Type, groupId); // source
                writer.WriteLine($":GRI");
                DrawWireEndSymbol(writer, X2, Y2, -1, wire_symbol, wire_Type, groupId); // destination
            }            
        }

        public static void Draw_C_TypeWire(double X1, double Y1, double X2, double Y2, double X3, double Y3, double X4, double Y4, string iWireCode, string iWireGauge, string wire_Type_Core_Number,
            string wire_Length, string FromOrientation, string ToOrientation, StreamWriter writer, string wire_symbol, string wire_Type, string groupId)
        {
            writer.WriteLine($"{X1},{Y1}");
            writer.WriteLine($"{X3},{Y3}");
            writer.WriteLine($"{X4},{Y4}");
            writer.WriteLine($"{X2},{Y2}");

            if (FromOrientation.Equals("R", StringComparison.OrdinalIgnoreCase) || ToOrientation.Equals("R", StringComparison.OrdinalIgnoreCase))
            {
                //Source
                writer.WriteLine($";;NOP;;");
                writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {X1},{Y1} '{iWireCode}' {X1 - 48},{Y1} ;NOP;");
                writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {X1},{Y1} '#{iWireGauge}' {X1 - 48 + 2},{Y1} ;");
                writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {X1},{Y1} '{wire_Type_Core_Number}' {X1 - 48 + 2 + 8},{Y1 + 1} ;");
                writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 {X1},{Y1} '{wire_Length}' {X1 - 48 + 2 + 8 + 2},{Y1} ;");
                writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {X1},{Y1} '' {X1 - 48 + 2 + 8 + 2},{Y1} ;");

                //Destination
                writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {X2 - 25},{Y2} '{iWireCode}' {X2 - 25 * 2},{Y2} ;NOP;");
                writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {X2 - 25},{Y2} '#{iWireGauge}' {X2 - 25 * 2 + 2},{Y2} ;");
                writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {X2 - 25},{Y2} '{wire_Type_Core_Number}' {X2 - 25 * 2 + 2 + 8},{Y2 - 1} ;");
                writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 {X2 - 25},{Y2} '{wire_Length}' {X2 - 25 * 2 + 2 + 8 + 2},{Y2} ;");
                writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {X2 - 25},{Y2} '' {X2 - 25 * 2 + 2 + 8 + 2},{Y2} ;");
            }
            else if (FromOrientation.Equals("L", StringComparison.OrdinalIgnoreCase) || ToOrientation.Equals("L", StringComparison.OrdinalIgnoreCase))
            {
                //Source
                writer.WriteLine($";;NOP;;");
                writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {X1},{Y1} '{iWireCode}' {X1 + 48},{Y1} ;NOP;");
                writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {X1},{Y1} '#{iWireGauge}' {X1 + 48 + 2},{Y1} ;");
                writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {X1},{Y1} '{wire_Type_Core_Number}' {X1 + 48 + 2 + 8},{Y1 - 1} ;");
                writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 {X1},{Y1} '{wire_Length}' {X1 + 48 + 2 + 8 + 2},{Y1} ;");
                writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {X1},{Y1} '' {X1 + 48 + 2 + 8 + 2},{Y1} ;");

                //Destination
                writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {X2 + 25},{Y2} '{iWireCode}' {X2 + 25 * 2},{Y2} ;NOP;");
                writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {X2 + 25},{Y2} '#{iWireGauge}' {X2 + 25 * 2 + 2},{Y2} ;");
                writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {X2 + 25},{Y2} '{wire_Type_Core_Number}' {X2 + 25 * 2 + 2 + 8},{Y2 - 1} ;");
                writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 {X2 + 25},{Y2} '{wire_Length}' {X2 + 25 * 2 + 2 + 8 + 2},{Y2} ;");
                writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {X2 + 25},{Y2} '' {X2 + 25 * 2 + 2 + 8 + 2},{Y2} ;");
            }

            if (!Constants.lst_WireCodes_Info_Processed.Contains(iWireCode))
            {
                int Direction = FromOrientation.Equals("R", StringComparison.OrdinalIgnoreCase) || ToOrientation.Equals("R", StringComparison.OrdinalIgnoreCase) ? -1 : +1;

                DrawWireEndSymbol(writer, X1, Y1, Direction, wire_symbol, wire_Type, groupId); //source
                DrawWireEndSymbol(writer, X2, Y2, Direction, wire_symbol, wire_Type, groupId); // destination
            }
        }

        public static void Draw_Z_TypeWire(double X1, double Y1, double X2, double Y2, double X3, double Y3, double X4, double Y4, string iWireCode, string iWireGauge,
            string wire_Type_Core_Number, string wire_Length, StreamWriter writer, string wire_symbol, string wire_Type, string groupId)
        {
            writer.WriteLine($"{X1},{Y1}");
            writer.WriteLine($"{X3},{Y3}");
            writer.WriteLine($"{X4},{Y4}");
            writer.WriteLine($"{X2},{Y2}");

            //Source
            writer.WriteLine($";;NOP;;");
            writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {X1},{Y1} '{iWireCode}' {X1 + 48},{Y1} ;NOP;");
            writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {X1},{Y1} '#{iWireGauge}' {X1 + 48 + 2},{Y1} ;");
            writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {X1},{Y1} '{wire_Type_Core_Number}' {X1 + 48 + 2 + 8},{Y1 - 1} ;");
            writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 {X1},{Y1} '{wire_Length}' {X1 + 48 + 2 + 8 + 2},{Y1} ;");
            writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {X1},{Y1} '' {X1 + 48 + 2 + 8 + 2},{Y1} ;");

            //Destination
            writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {X2 - 25},{Y2} '{iWireCode}' {X2 - 25 * 2},{Y2} ;NOP;");
            writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {X2 - 25},{Y2} '#{iWireGauge}' {X2 - 25 * 2 + 2},{Y2} ;");
            writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {X2 - 25},{Y2} '{wire_Type_Core_Number}' {X2 - 25 * 2 + 2 + 8},{Y2 - 1} ;");
            writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 {X2 - 25},{Y2} '{wire_Length}' {X2 - 25 * 2 + 2 + 8 + 2},{Y2} ;");
            writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {X2 - 25},{Y2} '' {X2 - 25 * 2 + 2 + 8 + 2},{Y2} ;");

            // Source end symbols
            if (!Constants.lst_WireCodes_Info_Processed.Contains(iWireCode))
            {
                DrawWireEndSymbol(writer, X1, Y1, +1, wire_symbol, wire_Type, groupId); // source
                writer.WriteLine($":GRI");
                DrawWireEndSymbol(writer, X2, Y2, -1, wire_symbol, wire_Type, groupId); //destination
            }  
        }

        public static void DrawWire(double X1, double Y1, double X2, double Y2, string iWireCode, string iWireGauge, string c1, string c2, string iF_Type, string iT_Type,
            string groupId, string wire_Length, string wire_Type, string wire_Type_Core_Number, string FromOrientation, string ToOrientation)
        {
            double  X3 = 0, Y3 = 0, X4 = 0, Y4 = 0;

            bool isCType = (X1 == X2) || Math.Abs(X1 - X2) < 50;

            if (isCType) // C-Line 
            {
                X3 = GetX3Value(FromOrientation, ToOrientation, X1);
            }
            else   // Straight & Z-line
            {             
                X3 = (X1 + X2) / 2 + 10 - Constants.WiringOffset;
            }
            X4 = X3;

            Y3 = Y1;
            Y4 = Y2;

            string wire_symbol = Constants.GetWireSymbol(wire_Type);

            using (var writer = File.AppendText(Constants.el_ExecFilePath))
            {
                writer.WriteLine($"GRI 0.5, 2;");
                writer.WriteLine($"ADD L154 :W0");

                if (Y1 == Y2) // Straight Line wiring if both source and destination components are in same horizontal location
                {
                    DrawStraightWire(X1, Y1, X2, Y2, writer, iWireCode, iWireGauge,wire_Type_Core_Number, wire_Length, wire_symbol, wire_Type, groupId);
                }
                else if (isCType) // C type wiring if Source and destination components are in same horizontal location with 50 margin
                {
                    Draw_C_TypeWire(X1, Y1, X2, Y2, X3, Y3, X4, Y4, iWireCode, iWireGauge, wire_Type_Core_Number, wire_Length, FromOrientation, ToOrientation, writer, wire_symbol, wire_Type, groupId);
                }                
                else // Z shaped wire routing
                {
                    Draw_Z_TypeWire(X1, Y1, X2, Y2, X3, Y3, X4, Y4, iWireCode, iWireGauge, wire_Type_Core_Number, wire_Length, writer, wire_symbol, wire_Type, groupId);
                }
              
                //writer.WriteLine($":GRI");
                //writer.WriteLine($"pm_files_sav;;");
                //writer.WriteLine($"GRI ELECTRE_GRID_STH;");

                Constants.WiringOffset++;

                if (!Constants.lst_WireCodes_Info_Processed.Contains(iWireCode))
                    Constants.lst_WireCodes_Info_Processed.Add(iWireCode);
            }
        }

        #region // Old code
        #region old Code commented on Jan 9th, 2025
        //public static void Simple4PointConnection_ZLine_Optimized(double p1x, double p1y, double p2x, double p2y, string iWireCode, string iWireGauge, string c1, string c2, string iF_Type, 
        //    string iT_Type, string groupId, string wire_Length, string wire_Type, string wire_Type_Core_Number, string FromOrientation, string ToOrientation)
        //{
        //    double X1 = 0, Y1 = 0, X2 = 0, Y2 = 0, X3 = 0, Y3 = 0, X4 = 0, Y4 = 0;
        //    X1 = p1x;
        //    Y1 = p1y;
        //    X2 = p2x;
        //    Y2 = p2y;

        //    bool isCType = (X1 == X2) || Math.Abs(X1 - X2) < 50;

        //    if (isCType)
        //    {
        //        double Offset = 80;

        //        double bend = 0;

        //        if (FromOrientation.Equals("L", StringComparison.OrdinalIgnoreCase) || ToOrientation.Equals("L", StringComparison.OrdinalIgnoreCase))
        //        {
        //            bend = Offset - Constants.WiringOffset;
        //            X3 = X1 + bend;
        //        }
        //        else if (FromOrientation.Equals("R", StringComparison.OrdinalIgnoreCase) || ToOrientation.Equals("R", StringComparison.OrdinalIgnoreCase))
        //        {
        //            bend = Offset + Constants.WiringOffset;
        //            X3 = X1 - bend;
        //        }
        //        else
        //        {
        //            bend = Offset - Constants.WiringOffset;
        //            X3 = X1 + bend;
        //        }
        //    }
        //    else
        //    {
        //        // Existing Z-line logic
        //        X3 = (X1 + X2) / 2 + 10 - Constants.WiringOffset;
        //    }
        //    X4 = X3;

        //    Y3 = Y1;
        //    Y4 = Y2;

        //    string wire_symbol = Constants.GetWireSymbol(wire_Type);

        //    using (var writer = File.AppendText(Constants.el_ExecFilePath))
        //    {
        //        writer.WriteLine($"GRI 0.5, 2;");
        //        writer.WriteLine($"ADD L154 :W0");

        //        if (Y1 == Y2)
        //        {
        //            writer.WriteLine($"ADD L154 :W0 {X1},{Y1} {X2},{Y2} ;;NOP;;");
        //        }
        //        else
        //        {
        //            writer.WriteLine($"{X1},{Y1}");
        //            writer.WriteLine($"{X3},{Y3}");
        //            writer.WriteLine($"{X4},{Y4}");
        //            writer.WriteLine($"{X2},{Y2}");
        //        }

        //        if (Y1 == Y2)
        //        {
        //            // middle
        //            writer.WriteLine($";;NOP;;");
        //            writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {X1},{Y1} '{iWireCode}' {(X1 + X2) / 2},{Y1} ;NOP;");
        //            writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {X1},{Y1} '#{iWireGauge}' {(X1 + X2) / 2 + 2},{Y1} ;");
        //            writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {X1},{Y1} '{wire_Type_Core_Number}' {(X1 + X2) / 2 + 10},{Y1 - 1} ;");
        //            writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 {X1},{Y1} '{wire_Length}' {(X1 + X2) / 2 + 10},{Y1} ;");
        //            writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {X1},{Y1} '' {(X1 + X2) / 2 + 10},{Y1} ;");
        //        }
        //        else if (isCType && (FromOrientation.Equals("R", StringComparison.OrdinalIgnoreCase) || ToOrientation.Equals("R", StringComparison.OrdinalIgnoreCase)))
        //        {
        //            //Source
        //            writer.WriteLine($";;NOP;;");
        //            writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {X1},{Y1} '{iWireCode}' {X1 - 48},{Y1} ;NOP;");
        //            writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {X1},{Y1} '#{iWireGauge}' {X1 - 48 + 2},{Y1} ;");
        //            writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {X1},{Y1} '{wire_Type_Core_Number}' {X1 - 48 + 2 + 8},{Y1 + 1} ;");
        //            writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 {X1},{Y1} '{wire_Length}' {X1 - 48 + 2 + 8 + 2},{Y1} ;");
        //            writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {X1},{Y1} '' {X1 - 48 + 2 + 8 + 2},{Y1} ;");

        //            //Destination
        //            writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {X2 - 25},{Y2} '{iWireCode}' {X2 - 25 * 2},{Y2} ;NOP;");
        //            writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {X2 - 25},{Y2} '#{iWireGauge}' {X2 - 25 * 2 + 2},{Y2} ;");
        //            writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {X2 - 25},{Y2} '{wire_Type_Core_Number}' {X2 - 25 * 2 + 2 + 8},{Y2 - 1} ;");
        //            writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 {X2 - 25},{Y2} '{wire_Length}' {X2 - 25 * 2 + 2 + 8 + 2},{Y2} ;");
        //            writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {X2 - 25},{Y2} '' {X2 - 25 * 2 + 2 + 8 + 2},{Y2} ;");
        //        }
        //        else if (isCType && (FromOrientation.Equals("L", StringComparison.OrdinalIgnoreCase) || ToOrientation.Equals("L", StringComparison.OrdinalIgnoreCase)))
        //        {
        //            //Source
        //            writer.WriteLine($";;NOP;;");
        //            writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {X1},{Y1} '{iWireCode}' {X1 + 48},{Y1} ;NOP;");
        //            writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {X1},{Y1} '#{iWireGauge}' {X1 + 48 + 2},{Y1} ;");
        //            writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {X1},{Y1} '{wire_Type_Core_Number}' {X1 + 48 + 2 + 8},{Y1 - 1} ;");
        //            writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 {X1},{Y1} '{wire_Length}' {X1 + 48 + 2 + 8 + 2},{Y1} ;");
        //            writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {X1},{Y1} '' {X1 + 48 + 2 + 8 + 2},{Y1} ;");

        //            //Destination
        //            writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {X2 + 25},{Y2} '{iWireCode}' {X2 + 25 * 2},{Y2} ;NOP;");
        //            writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {X2 + 25},{Y2} '#{iWireGauge}' {X2 + 25 * 2 + 2},{Y2} ;");
        //            writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {X2 + 25},{Y2} '{wire_Type_Core_Number}' {X2 + 25 * 2 + 2 + 8},{Y2 - 1} ;");
        //            writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 {X2 + 25},{Y2} '{wire_Length}' {X2 + 25 * 2 + 2 + 8 + 2},{Y2} ;");
        //            writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {X2 + 25},{Y2} '' {X2 + 25 * 2 + 2 + 8 + 2},{Y2} ;");
        //        }
        //        else
        //        {
        //            //Source
        //            writer.WriteLine($";;NOP;;");
        //            writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {X1},{Y1} '{iWireCode}' {X1 + 48},{Y1} ;NOP;");
        //            writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {X1},{Y1} '#{iWireGauge}' {X1 + 48 + 2},{Y1} ;");
        //            writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {X1},{Y1} '{wire_Type_Core_Number}' {X1 + 48 + 2 + 8},{Y1 - 1} ;");
        //            writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 {X1},{Y1} '{wire_Length}' {X1 + 48 + 2 + 8 + 2},{Y1} ;");
        //            writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {X1},{Y1} '' {X1 + 48 + 2 + 8 + 2},{Y1} ;");

        //            //Destination
        //            writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {X2 - 25},{Y2} '{iWireCode}' {X2 - 25 * 2},{Y2} ;NOP;");
        //            writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {X2 - 25},{Y2} '#{iWireGauge}' {X2 - 25 * 2 + 2},{Y2} ;");
        //            writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {X2 - 25},{Y2} '{wire_Type_Core_Number}' {X2 - 25 * 2 + 2 + 8},{Y2 - 1} ;");
        //            writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 {X2 - 25},{Y2} '{wire_Length}' {X2 - 25 * 2 + 2 + 8 + 2},{Y2} ;");
        //            writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {X2 - 25},{Y2} '' {X2 - 25 * 2 + 2 + 8 + 2},{Y2} ;");
        //        }

        //        if (!Constants.lst_WireCodes_Info_Processed.Contains(iWireCode))
        //        {
        //            if (isCType && (FromOrientation.Equals("R", StringComparison.OrdinalIgnoreCase) || ToOrientation.Equals("R", StringComparison.OrdinalIgnoreCase)))
        //            {
        //                writer.WriteLine($"ADD I2 {wire_symbol} :R0 {X1 - 16},{Y1};");
        //                writer.WriteLine($"TESTDIS;");
        //                writer.WriteLine($"MOD N250 {X1 - 16 + 2},{Y1 + 1.5} 0,0 :L254 STOR_MID :E'{wire_Type}' JU; NOP;");//wire type display
        //                writer.WriteLine($"TESTDIS_OFF;");
        //                writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {X1 - 16},{Y1} {X1 - 16 + 1.5},{Y1 + 0.5};NOP;");

        //                if (wire_Type.Equals("SS") || wire_Type.Equals("TP") || wire_Type.Equals("STP") || wire_Type.Equals("COAX") || wire_Type.Equals("TRIAX") || wire_Type.Equals("QUADRAX"))
        //                {
        //                    writer.WriteLine($"MOD N253 {X1 - 16},{Y1} 0,0 STOR_MID :E'{groupId}' JU;NOP;");
        //                }
        //            }
        //            else if (isCType && (FromOrientation.Equals("L", StringComparison.OrdinalIgnoreCase) || ToOrientation.Equals("L", StringComparison.OrdinalIgnoreCase)))
        //            {
        //                writer.WriteLine($"ADD I2 {wire_symbol} :R0 {X1 + 16},{Y1};");
        //                writer.WriteLine($"TESTDIS;");
        //                writer.WriteLine($"MOD N250 {X1 + 16 - 2},{Y1 + 1.5} 0,0 :L254 STOR_MID :E'{wire_Type}' JU; NOP;");//wire type display
        //                writer.WriteLine($"TESTDIS_OFF;");
        //                writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {X1 + 16},{Y1} {X1 + 16 + 1.5},{Y1 + 0.5};NOP;");

        //                if (wire_Type.Equals("SS") || wire_Type.Equals("TP") || wire_Type.Equals("STP") || wire_Type.Equals("COAX") || wire_Type.Equals("TRIAX") || wire_Type.Equals("QUADRAX"))
        //                {
        //                    writer.WriteLine($"MOD N253 {X1 + 16},{Y1} 0,0 STOR_MID :E'{groupId}' JU;NOP;");
        //                }
        //            }
        //            else
        //            {
        //                writer.WriteLine($"ADD I2 {wire_symbol} :R0 {X1 + 16},{Y1};");
        //                writer.WriteLine($"TESTDIS;");
        //                writer.WriteLine($"MOD N250 {X1 + 16 + 2},{Y1 + 1.5} 0,0 :L254 STOR_MID :E'{wire_Type}' JU; NOP;");//wire type display
        //                writer.WriteLine($"TESTDIS_OFF;");
        //                writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {X1 + 16},{Y1} {X1 + 16 + 1.5},{Y1 + 0.5};NOP;");

        //                if (wire_Type.Equals("SS") || wire_Type.Equals("TP") || wire_Type.Equals("STP") || wire_Type.Equals("COAX") || wire_Type.Equals("TRIAX") || wire_Type.Equals("QUADRAX"))
        //                {
        //                    writer.WriteLine($"MOD N253 {X1 + 16},{Y1} 0,0 STOR_MID :E'{groupId}' JU;NOP;");
        //                }
        //            }
        //        }
        //        writer.WriteLine($":GRI");
        //        #region//Destination end symbol added
        //        if (!Constants.lst_WireCodes_Info_Processed.Contains(iWireCode))
        //        {
        //            if (isCType && (FromOrientation.Equals("R", StringComparison.OrdinalIgnoreCase) || ToOrientation.Equals("R", StringComparison.OrdinalIgnoreCase)))
        //            {
        //                writer.WriteLine($"ADD I2 {wire_symbol} :R0 {X2 - 16},{Y2};");
        //                writer.WriteLine($"TESTDIS;");
        //                writer.WriteLine($"MOD N250 {X2 - 16 - 2},{Y2 + 1.5} 0,0 :L254 STOR_MID :E'{wire_Type}' JU; NOP;");//wire type display
        //                writer.WriteLine($"TESTDIS_OFF;");
        //                writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {X2 - 16},{Y2} {X2 - 16 + 1.5},{Y2 + 0.5};NOP;");

        //                if (wire_Type.Equals("SS") || wire_Type.Equals("TP") || wire_Type.Equals("STP") || wire_Type.Equals("COAX") || wire_Type.Equals("TRIAX") || wire_Type.Equals("QUADRAX"))
        //                {
        //                    writer.WriteLine($"MOD N253 {X2 - 16},{Y2} 0,0 STOR_MID :E'{groupId}' JU; NOP;");
        //                }
        //            }
        //            else if (isCType && (FromOrientation.Equals("L", StringComparison.OrdinalIgnoreCase) || ToOrientation.Equals("L", StringComparison.OrdinalIgnoreCase)))
        //            {
        //                writer.WriteLine($"ADD I2 {wire_symbol} :R0 {X2 + 16},{Y2};");
        //                writer.WriteLine($"TESTDIS;");
        //                writer.WriteLine($"MOD N250 {X2 + 16 + 2},{Y2 + 1.5} 0,0 :L254 STOR_MID :E'{wire_Type}' JU; NOP;");//wire type display
        //                writer.WriteLine($"TESTDIS_OFF;");
        //                writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {X2 + 16},{Y2} {X2 + 16 + 1.5},{Y2 + 0.5};NOP;");

        //                if (wire_Type.Equals("SS") || wire_Type.Equals("TP") || wire_Type.Equals("STP") || wire_Type.Equals("COAX") || wire_Type.Equals("TRIAX") || wire_Type.Equals("QUADRAX"))
        //                {
        //                    writer.WriteLine($"MOD N253 {X2 + 16},{Y2} 0,0 STOR_MID :E'{groupId}' JU; NOP;");
        //                }
        //            }
        //            else
        //            {
        //                writer.WriteLine($"ADD I2 {wire_symbol} :R0 {X2 - 16},{Y2};");
        //                writer.WriteLine($"TESTDIS;");
        //                writer.WriteLine($"MOD N250 {X2 - 16 - 2},{Y2 + 1.5} 0,0 :L254 STOR_MID :E'{wire_Type}' JU; NOP;");//wire type display
        //                writer.WriteLine($"TESTDIS_OFF;");
        //                writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {X2 - 16},{Y2} {X2 - 16 + 1.5},{Y2 + 0.5};NOP;");

        //                if (wire_Type.Equals("SS") || wire_Type.Equals("TP") || wire_Type.Equals("STP") || wire_Type.Equals("COAX") || wire_Type.Equals("TRIAX") || wire_Type.Equals("QUADRAX"))
        //                {
        //                    writer.WriteLine($"MOD N253 {X2 - 16},{Y2} 0,0 STOR_MID :E'{groupId}' JU; NOP;");
        //                }
        //            }

        //        }
        //        #endregion
        //        writer.WriteLine($":GRI");
        //        writer.WriteLine($"pm_files_sav;;");
        //        writer.WriteLine($"GRI ELECTRE_GRID_STH;");

        //        Constants.WiringOffset++;

        //        if (!Constants.lst_WireCodes_Info_Processed.Contains(iWireCode))
        //            Constants.lst_WireCodes_Info_Processed.Add(iWireCode);
        //    }
        //}
        #endregion

        #region old code commented on Jan, 12
        //public static void DrawStraightLineConnection(double p1x, double p1y, double p2x, double p2y, string iWireCode, string iWireGauge, string c1, string c2,
        //    string iF_Type, string iT_Type, string groupId, string wire_Length, string wire_Type, string wire_Type_Core_Number)
        //{
        //    // MASTER LISTS (you can add more types any time)
        //    var monoTypes = new List<string> { "86A9S", "86A9SS", "S", "S0", "S00", "SS", "COAX", "TRIAX" };
        //    var tpTypes = new List<string> { "TP", "QUADRAX" };
        //    var stpTypes = new List<string> { "STP" };

        //    // Decide which block to execute
        //    if (monoTypes.Contains(wire_Type))
        //    {
        //        double X3 = (p1x + p2x) / 2;
        //        string mono_wire_symbol = string.Empty;

        //        if (wire_Type.Equals("COAX"))
        //            mono_wire_symbol = "sth_coax";
        //        else if (wire_Type.Equals("SS"))
        //            mono_wire_symbol = "sth_ss";
        //        else if (wire_Type.Equals("COAX"))
        //            mono_wire_symbol = "sth_coax";
        //        //else if (wire_Type.Equals("86A9S") || wire_Type.Equals("86A9SS") || wire_Type.Equals("S") || wire_Type.Equals("S0") || wire_Type.Equals("S00"))
        //        //    mono_wire_symbol = "sth_s";
        //        else
        //            mono_wire_symbol = "sth_s";

        //        using (var writer = File.AppendText(Constants.el_ExecFilePath))
        //        {
        //            writer.WriteLine($"GRI 2.0, 2;");
        //            writer.WriteLine($"ADD L154 :W0 {p1x},{p1y} {p2x},{p2y} ;;NOP;;");
        //            writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {p1x},{p1y} '{iWireCode}' {p1x * 2},{p1y} ;NOP;");
        //            writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {p1x},{p1y} '#{iWireGauge}' {p1x * 2 + 2},{p1y} ;");
        //            writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {p1x},{p1y} '{wire_Type_Core_Number}' {p1x * 2 + 2 + 8},{p1y - 1} ;");
        //            writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 {p1x},{p1y} '{wire_Length}' {p1x * 2 + 2 + 8},{p1y} ;");
        //            writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {p1x},{p1y} '' {p1x * 2 + 2 + 8},{p1y} ;");
        //            writer.WriteLine($"ADD L154 :W0");
        //            writer.WriteLine($"{p1x},{p1y - 4}");
        //            writer.WriteLine($"{p2x},{p2y - 4}");
        //            writer.WriteLine($";;NOP;;");
        //            writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {p1x},{p1y - 4} '{iWireCode}' {X3},{p2y - 4} ;NOP;");
        //            writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {p1x},{p1y - 4} '#{iWireGauge}' {X3 + 2},{p2y - 4} ;");
        //            writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {p1x},{p2y - 4} '{wire_Type_Core_Number}' {X3 + 2 + 8},{p2y - 4 - 1} ;");
        //            writer.WriteLine($"ADD N59 :R0 :D :J2 :F2 :T3009 :AC L154 {p1x},{p2y - 4} '{wire_Length}' {X3 + 2 + 8},{p2y - 4} ;");
        //            writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {p1x},{p2y - 4} '' {X3 + 2 + 8},{p2y - 4} ;");
        //            writer.WriteLine($"ADD I2 {mono_wire_symbol} :R0 {p1x + 16},{p1y};");
        //            writer.WriteLine($"TESTDIS;");
        //            writer.WriteLine($"TESTDIS_OFF;");

        //            writer.WriteLine($":RAW");
        //            writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {p1x + 16},{p1y} {p1x + 16 - 1.5},{p1y + 0.5};NOP;");
        //            if (wire_Type.Equals("SS"))
        //                writer.WriteLine($"MOD N253 {p1x + 16},{p1y} 0,0 STOR_MID :E'{groupId}' JU;NOP;");
        //            writer.WriteLine($":GRI");

        //            writer.WriteLine($"ADD I2 {mono_wire_symbol} :R0 {p2x - 16},{p2y};");
        //            writer.WriteLine($"TESTDIS;");
        //            writer.WriteLine($"TESTDIS_OFF;");

        //            writer.WriteLine($":RAW");
        //            writer.WriteLine($"ADD N58 :J7: T3003: D: F1: R0 '{groupId}' :AC I2 {p2x - 16},{p2y} {p2x - 16 - 1.5},{p1y + 0.5}; NOP;");
        //            if (wire_Type.Equals("SS") || wire_Type.Equals("COAX") || wire_Type.Equals("TRIAX"))
        //                writer.WriteLine($"MOD N253 {p2x - 16},{p2y} 0,0 STOR_MID :E'{iWireCode}' JU; NOP;");
        //            writer.WriteLine($":GRI");

        //            writer.WriteLine($"pm_files_sav;;");
        //            writer.WriteLine($"GRI ELECTRE_GRID_STH;");
        //        }
        //        return;
        //    }

        //    // TP ORIGINAL BLOCK
        //    else if (tpTypes.Contains(wire_Type))
        //    {
        //        double X3 = (p1x + p2x) / 2;
        //        string twisted_symbol = wire_Type.Equals("TP") ? "sth_tp4" : "sth_quadrax4";
        //        int coreNum = Convert.ToInt16(wire_Type_Core_Number);

        //        using (var writer = File.AppendText(Constants.el_ExecFilePath))
        //        {
        //            writer.WriteLine($"GRI 2.0, 2;");
        //            writer.WriteLine($"ADD L154 :W0 {p1x},{p1y} {p2x},{p2y} ;;NOP;;");
        //            writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {p1x},{p1y} '{iWireCode}' {p1x * 2},{p1y} ;NOP;");
        //            writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {p1x},{p1y} '#{iWireGauge}' {p1x * 2 + 2},{p1y} ;");
        //            writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {p1x},{p1y} '{wire_Type_Core_Number}' {p1x * 2 + 2 + 8},{p1y - 1} ;");
        //            writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 {p1x},{p1y} '{wire_Length}' {p1x * 2 + 2 + 8},{p1y} ;");
        //            writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {p1x},{p1y} '' {p1x * 2 + 2 + 8},{p1y} ;");
        //            writer.WriteLine($"ADD L154 :W0");
        //            writer.WriteLine($"{p1x},{p1y - 4}");
        //            writer.WriteLine($"{p2x},{p2y - 4}");
        //            writer.WriteLine($";;NOP;;");
        //            writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {p2x},{p2y - 4} '{iWireCode}' {X3},{p2y - 4} ;NOP;");
        //            writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {p2x},{p2y - 4} '#{iWireGauge}' {X3 + 2},{p2y - 4} ;");
        //            writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {p2x},{p2y - 4} '{wire_Type_Core_Number + 1}' {X3 + 2 + 8},{p2y - 4 - 1} ;");
        //            writer.WriteLine($"ADD N59 :R0 :D :J2 :F2 :T3009 :AC L154 {p1x},{p2y - 4} '{wire_Length}' {X3 + 2 + 8},{p2y - 4} ;");
        //            writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {p1x},{p2y - 4} '' {X3 + 2 + 8},{p2y - 4} ;");
        //            writer.WriteLine($"ADD I2 {twisted_symbol} :R0 {p1x + 16},{p1y};");
        //            writer.WriteLine($"TESTDIS;");
        //            writer.WriteLine($"MOD N250 {p1x + 16},{p1y} 0,0 :L254 STOR_MID :E'{wire_Type}' JU;NOP;");
        //            writer.WriteLine($"MOD N250 {p1x + 16},{p1y - 4} 0,0 :L254 STOR_MID :E'{wire_Type}' JU;NOP;");
        //            writer.WriteLine($"TESTDIS_OFF;");
        //            writer.WriteLine($":RAW");
        //            writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {p1x + 16},{p1y} {p1x + 16 - 1.5},{p1y + 0.5};NOP;");
        //            writer.WriteLine($"MOD N253 {p1x + 16},{p1y} 0,0 STOR_MID :E'{groupId}' JU;NOP;");
        //            writer.WriteLine($":GRI");
        //            writer.WriteLine($"ADD I2 {twisted_symbol} :R0 {p2x - 16},{p2y};");
        //            writer.WriteLine($"TESTDIS;");
        //            writer.WriteLine($"MOD N250 {p2x - 16},{p2y} 0,0 :L254 STOR_MID :E'{wire_Type}' JU;NOP;");
        //            writer.WriteLine($"MOD N250 {p2x - 16},{p2y - 4} 0,0 :L254 STOR_MID :E'{wire_Type}' JU;NOP;");
        //            writer.WriteLine($"TESTDIS_OFF;");
        //            writer.WriteLine($":RAW");
        //            writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {p2x - 16},{p2y} {p2x - 16 - 1.5},{p2y + 0.5};NOP;");
        //            writer.WriteLine($"MOD N253 {p2x - 16},{p2y} 0,0 STOR_MID :E'{groupId}' JU; NOP;");
        //            writer.WriteLine($":GRI");
        //            writer.WriteLine($"pm_files_sav;;");
        //            writer.WriteLine($"GRI ELECTRE_GRID_STH;");
        //        }
        //        return;
        //    }

        //    // STP ORIGINAL BLOCK
        //    else if (stpTypes.Contains(wire_Type))
        //    {
        //        double X3 = (p1x + p2x) / 2;
        //        int coreNum = Convert.ToInt16(wire_Type_Core_Number);

        //        using (var writer = File.AppendText(Constants.el_ExecFilePath))
        //        {
        //            writer.WriteLine($"GRI 2.0, 2;");
        //            writer.WriteLine($"ADD L154 :W0 {p1x},{p1y} {p2x},{p2y} ;;NOP;;");
        //            writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {p1x},{p1y} '{iWireCode}' {p1x * 2},{p1y} ;NOP;");
        //            writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {p1x},{p1y} '#{iWireGauge}' {p1x * 2 + 2},{p1y} ;");
        //            writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {p1x},{p1y} '{wire_Type_Core_Number}' {p1x * 2 + 2 + 8},{p1y - 1} ;");
        //            writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 {p1x},{p1y} '{wire_Length}' {p1x * 2 + 2 + 8},{p1y} ;");
        //            writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {p1x},{p1y} '' {p1x * 2 + 2 + 8},{p1y} ;");
        //            writer.WriteLine($"ADD L154 :W0");
        //            writer.WriteLine($"{p1x},{p1y - 4}");
        //            writer.WriteLine($"{p2x},{p2y - 4}");
        //            writer.WriteLine($";;NOP;;");
        //            writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {p1x},{p1y - 4} '{iWireCode}' {X3},{p2y - 4} ;NOP;");
        //            writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {p1x},{p1y - 4} '#{iWireGauge}' {X3 + 2},{p2y - 4} ;");
        //            writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {p1x},{p2y - 4} '{wire_Type_Core_Number + 1}' {X3 + 2 + 8},{p2y - 4 - 1} ;");
        //            writer.WriteLine($"ADD N59 :R0 :D :J2 :F2 :T3009 :AC L154 {p1x},{p2y - 4} '{wire_Length}' {X3 + 2 + 8},{p2y - 4} ;");
        //            writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {p1x},{p2y - 4} '' {X3 + 2 + 8},{p2y - 4} ;");
        //            writer.WriteLine($"ADD I2 sth_stp4 :R0 {p1x + 16},{p1y};");
        //            writer.WriteLine($"TESTDIS;");
        //            writer.WriteLine($"MOD N250 {p1x + 16},{p1y} 0,0 :L254 STOR_MID :E'{wire_Type}' JU;NOP;");
        //            writer.WriteLine($"MOD N250 {p1x + 16},{p1y - 4} 0,0 :L254 STOR_MID :E'{wire_Type}' JU;NOP;");
        //            writer.WriteLine($"TESTDIS_OFF;");
        //            writer.WriteLine($":RAW");
        //            writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {p1x + 16},{p1y} {p1x + 16 - 1.5},{p1y + 0.5};NOP;");
        //            writer.WriteLine($"MOD N253 {p1x + 16},{p1y} 0,0 STOR_MID :E'{groupId}' JU;NOP;");
        //            writer.WriteLine($":GRI");
        //            writer.WriteLine($"ADD I2 sth_stp4 :R0 {p2x - 16},{p2y};");
        //            writer.WriteLine($"TESTDIS;");
        //            writer.WriteLine($"MOD N250 {p2x - 16},{p2y} 0,0 :L254 STOR_MID :E'{wire_Type}' JU;NOP;");
        //            writer.WriteLine($"MOD N250 {p2x - 16},{p2y - 4} 0,0 :L254 STOR_MID :E'{wire_Type}' JU;NOP;");
        //            writer.WriteLine($"TESTDIS_OFF;");
        //            writer.WriteLine($":RAW");
        //            writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {p2x - 16},{p2y} {p2x - 16 - 1.5},{p2y + 0.5};NOP;");
        //            writer.WriteLine($"MOD N253 {p2x - 16},{p2y} 0,0 STOR_MID :E'{groupId}' JU; NOP;");
        //            writer.WriteLine($":GRI");
        //            writer.WriteLine($"pm_files_sav;;");
        //            writer.WriteLine($"GRI ELECTRE_GRID_STH;");
        //        }
        //        return;
        //    }
        //    else
        //    {
        //        double X3 = (p1x + p2x) / 2;
        //        string mono_wire_symbol = "sth_s";

        //        using (var writer = File.AppendText(Constants.el_ExecFilePath))
        //        {
        //            writer.WriteLine($"GRI 2.0, 2;");
        //            writer.WriteLine($"ADD L154 :W0 {p1x},{p1y} {p2x},{p2y} ;;NOP;;");
        //            writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {p1x},{p1y} '{iWireCode}' {p1x * 2},{p1y} ;NOP;");
        //            writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {p1x},{p1y} '#{iWireGauge}' {p1x * 2 + 2},{p1y} ;");
        //            writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {p1x},{p1y} '{wire_Type_Core_Number}' {p1x * 2 + 2 + 8},{p1y - 1} ;");
        //            writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 {p1x},{p1y} '{wire_Length}' {p1x * 2 + 2 + 8},{p1y} ;");
        //            writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {p1x},{p1y} '' {p1x * 2 + 2 + 8},{p1y} ;");
        //            writer.WriteLine($"ADD L154 :W0");
        //            writer.WriteLine($"{p1x},{p1y - 4}");
        //            writer.WriteLine($"{p2x},{p2y - 4}");
        //            writer.WriteLine($";;NOP;;");
        //            writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {p1x},{p1y - 4} '{iWireCode}' {X3},{p2y - 4} ;NOP;");
        //            writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {p1x},{p1y - 4} '#{iWireGauge}' {X3 + 2},{p2y - 4} ;");
        //            writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {p1x},{p2y - 4} '{wire_Type_Core_Number}' {X3 + 2 + 8},{p2y - 4 - 1} ;");
        //            writer.WriteLine($"ADD N59 :R0 :D :J2 :F2 :T3009 :AC L154 {p1x},{p2y - 4} '{wire_Length}' {X3 + 2 + 8},{p2y - 4} ;");
        //            writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {p1x},{p2y - 4} '' {X3 + 2 + 8},{p2y - 4} ;");
        //            writer.WriteLine($"ADD I2 {mono_wire_symbol} :R0 {p1x + 16},{p1y};");
        //            writer.WriteLine($"TESTDIS;");
        //            writer.WriteLine($"TESTDIS_OFF;");

        //            writer.WriteLine($":RAW");
        //            writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {p1x + 16},{p1y} {p1x + 16 - 1.5},{p1y + 0.5};NOP;");
        //            if (wire_Type.Equals("SS"))
        //                writer.WriteLine($"MOD N253 {p1x + 16},{p1y} 0,0 STOR_MID :E'{groupId}' JU;NOP;");
        //            writer.WriteLine($":GRI");

        //            writer.WriteLine($"ADD I2 {mono_wire_symbol} :R0 {p2x - 16},{p2y};");
        //            writer.WriteLine($"TESTDIS;");
        //            writer.WriteLine($"TESTDIS_OFF;");

        //            writer.WriteLine($":RAW");
        //            writer.WriteLine($"ADD N58 :J7: T3003: D: F1: R0 '{groupId}' :AC I2 {p2x - 16},{p2y} {p2x - 16 - 1.5},{p1y + 0.5}; NOP;");
        //            if (wire_Type.Equals("SS") || wire_Type.Equals("COAX") || wire_Type.Equals("TRIAX"))
        //                writer.WriteLine($"MOD N253 {p2x - 16},{p2y} 0,0 STOR_MID :E'{iWireCode}' JU; NOP;");
        //            writer.WriteLine($":GRI");

        //            writer.WriteLine($"pm_files_sav;;");
        //            writer.WriteLine($"GRI ELECTRE_GRID_STH;");
        //        }
        //        return;
        //    }
        //}

        //public static void Simple2PointConnection_Mono_StraightLine(double p1x, double p1y, double p2x, double p2y, string iWireCode, string iWireGauge, string c1, string c2, string iF_Type, string iT_Type, string groupId, string wire_Length, string wire_Type, string wire_Type_Core_Number)
        //{
        //    double X3 = (p1x + p2x) / 2;
        //    string mono_wire_symbol = string.Empty;
        //    //if (wire_Type.Equals("86A9S") || wire_Type.Equals("86A9SS") || wire_Type.Equals("S") || wire_Type.Equals("S0") || wire_Type.Equals("S00"))
        //    //{ mono_wire_symbol = "sth_s"; }
        //    if (wire_Type.Equals("SS")) { mono_wire_symbol = "sth_ss"; }
        //    else if (wire_Type.Equals("COAX")) { mono_wire_symbol = "sth_coax"; }
        //    else if (wire_Type.Equals("TRIAX")) { mono_wire_symbol = "sth_triax"; }
        //    else { mono_wire_symbol = "sth_s"; }

        //    using (var writer = File.AppendText(Constants.el_ExecFilePath))
        //    {
        //        writer.WriteLine($"GRI 2.0, 2;");
        //        writer.WriteLine($"ADD L154 :W0 {p1x},{p1y} {p2x},{p2y} ;;NOP;;");
        //        writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {p1x},{p1y} '{iWireCode}' {p1x * 2},{p1y} ;NOP;");
        //        writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {p1x},{p1y} '#{iWireGauge}' {p1x * 2 + 2},{p1y} ;");
        //        if (!string.IsNullOrEmpty(wire_Type_Core_Number))
        //        {
        //            writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {p1x},{p1y} '{wire_Type_Core_Number}' {p1x * 2 + 2 + 8},{p1y - 1} ;");
        //        }
        //        writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 {p1x},{p1y} '{wire_Length}' {p1x * 2 + 2 + 8},{p1y} ;");
        //        writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {p1x},{p1y} '' {p1x * 2 + 2 + 8},{p1y} ;");

        //        //writer.WriteLine($"ADD L154 :W0");
        //        //writer.WriteLine($"{p1x},{p1y - 4}");
        //        //writer.WriteLine($"{p2x},{p2y - 4}");
        //        //writer.WriteLine($";;NOP;;");
        //        //writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {p1x},{p1y - 4} '{iWireCode}' {X3},{p2y - 4} ;NOP;");
        //        //writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {p1x},{p1y - 4} '#{iWireGauge}' {X3 + 2},{p2y - 4} ;");
        //        //writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {p1x},{p2y - 4} '{wire_Type_Core_Number}' {X3 + 2 + 8},{p2y - 4 - 1} ;");
        //        //writer.WriteLine($"ADD N59 :R0 :D :J2 :F2 :T3009 :AC L154 {p1x},{p2y - 4} '{wire_Length}' {X3 + 2 + 8},{p2y - 4} ;");
        //        //writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {p1x},{p2y - 4} '' {X3 + 2 + 8},{p2y - 4} ;");

        //        writer.WriteLine($"ADD I2 {mono_wire_symbol} :R0 {p1x + 16},{p1y};");
        //        writer.WriteLine($"TESTDIS;");
        //        writer.WriteLine($"TESTDIS_OFF;");
        //        writer.WriteLine($":RAW");
        //        writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {p1x + 16},{p1y} {p1x + 16 - 1.5},{p1y + 0.5};NOP;");
        //        if (wire_Type.Equals("SS"))
        //        {
        //            writer.WriteLine($"MOD N253 {p1x + 16},{p1y} 0,0 STOR_MID :E'{groupId}' JU;NOP;");
        //        }
        //        writer.WriteLine($":GRI");
        //        writer.WriteLine($"ADD I2 {mono_wire_symbol} :R0 {p2x - 16},{p2y};");
        //        writer.WriteLine($"TESTDIS;");
        //        writer.WriteLine($"TESTDIS_OFF;");
        //        writer.WriteLine($":RAW");
        //        writer.WriteLine($"ADD N58 :J7: T3003: D: F1: R0 '{groupId}' :AC I2 {p2x - 16},{p2y} {p2x - 16 - 1.5},{p1y + 0.5}; NOP;");
        //        if (wire_Type.Equals("SS") || wire_Type.Equals("COAX") || wire_Type.Equals("TRIAX"))
        //        {
        //            writer.WriteLine($"MOD N253 {p2x - 16},{p2y} 0,0 STOR_MID :E'{iWireCode}' JU; NOP;");
        //        }
        //        writer.WriteLine($":GRI");
        //        writer.WriteLine($"pm_files_sav;;");
        //        writer.WriteLine($"GRI ELECTRE_GRID_STH;");
        //    }
        //}

        //public static void Simple2PointConnection_TP_StraightLine(double p1x, double p1y, double p2x, double p2y, string iWireCode, string iWireGauge, string c1, string c2, string iF_Type, string iT_Type, string groupId, string wire_Length, string wire_Type, string wire_Type_Core_Num)
        //{
        //    string twisted_wire_symbol = string.Empty;
        //    if (wire_Type.Equals("TP")) { twisted_wire_symbol = "sth_tp4"; }
        //    else if (wire_Type.Equals("QUADRAX")) { twisted_wire_symbol = "sth_quadrax4"; }
        //    double X3 = (p1x + p2x) / 2;
        //    //int wire_Type_Core_Number = Convert.ToInt16(wire_Type_Core_Num);
        //    using (var writer = File.AppendText(Constants.el_ExecFilePath))
        //    {
        //        writer.WriteLine($"GRI 2.0, 2;");
        //        writer.WriteLine($"ADD L154 :W0 {p1x},{p1y} {p2x},{p2y} ;;NOP;;");
        //        writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {p1x},{p1y} '{iWireCode}' {p1x * 2},{p1y} ;NOP;");
        //        writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {p1x},{p1y} '#{iWireGauge}' {p1x * 2 + 2},{p1y} ;");
        //        if (!string.IsNullOrEmpty(wire_Type_Core_Num))
        //        {
        //            writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {p1x},{p1y} '{wire_Type_Core_Num}' {p1x * 2 + 2 + 8},{p1y - 1} ;");
        //        }
        //        writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 {p1x},{p1y} '{wire_Length}' {p1x * 2 + 2 + 8},{p1y} ;");
        //        writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {p1x},{p1y} '' {p1x * 2 + 2 + 8},{p1y} ;");

        //        //writer.WriteLine($"ADD L154 :W0");
        //        //writer.WriteLine($"{p1x},{p1y - 4}");
        //        //writer.WriteLine($"{p2x},{p2y - 4}");
        //        //writer.WriteLine($";;NOP;;");
        //        //writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {p2x},{p2y - 4} '{iWireCode}' {X3},{p2y - 4} ;NOP;");
        //        //writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {p2x},{p2y - 4} '#{iWireGauge}' {X3 + 2},{p2y - 4} ;");
        //        //writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {p2x},{p2y - 4} '{wire_Type_Core_Num + 1}' {X3 + 2 + 8},{p2y - 4 - 1} ;");
        //        //writer.WriteLine($"ADD N59 :R0 :D :J2 :F2 :T3009 :AC L154 {p1x},{p2y - 4} '{wire_Length}' {X3 + 2 + 8},{p2y - 4} ;");
        //        //writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {p1x},{p2y - 4} '' {X3 + 2 + 8},{p2y - 4} ;");

        //        writer.WriteLine($"ADD I2 {twisted_wire_symbol} :R0 {p1x + 16},{p1y};");
        //        writer.WriteLine($"TESTDIS;");
        //        writer.WriteLine($"MOD N250 {p1x + 16},{p1y} 0,0 :L254 STOR_MID :E'{wire_Type}' JU;NOP;");
        //        writer.WriteLine($"MOD N250 {p1x + 16},{p1y - 4} 0,0 :L254 STOR_MID :E'{wire_Type}' JU;NOP;");
        //        writer.WriteLine($"TESTDIS_OFF;");
        //        writer.WriteLine($":RAW");
        //        writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {p1x + 16},{p1y} {p1x + 16 - 1.5},{p1y + 0.5};NOP;");
        //        writer.WriteLine($"MOD N253 {p1x + 16},{p1y} 0,0 STOR_MID :E'{groupId}' JU;NOP;");
        //        writer.WriteLine($":GRI");
        //        writer.WriteLine($"ADD I2 {twisted_wire_symbol} :R0 {p2x - 16},{p2y};");
        //        writer.WriteLine($"TESTDIS;");
        //        writer.WriteLine($"MOD N250 {p2x - 16},{p2y} 0,0 :L254 STOR_MID :E'{wire_Type}' JU;NOP;");
        //        writer.WriteLine($"MOD N250 {p2x - 16},{p2y - 4} 0,0 :L254 STOR_MID :E'{wire_Type}' JU;NOP;");
        //        writer.WriteLine($"TESTDIS_OFF;");
        //        writer.WriteLine($":RAW");
        //        writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {p2x - 16},{p2y} {p2x - 16 - 1.5},{p2y + 0.5};NOP;");
        //        writer.WriteLine($"MOD N253 {p2x - 16},{p2y} 0,0 STOR_MID :E'{groupId}' JU; NOP;");
        //        writer.WriteLine($":GRI");
        //        writer.WriteLine($"pm_files_sav;;");
        //        writer.WriteLine($"GRI ELECTRE_GRID_STH;");
        //    }
        //}

        //public static void Simple2PointConnection_STP_StraightLine(double p1x, double p1y, double p2x, double p2y, string iWireCode, string iWireGauge, string c1, string c2, string iF_Type, string iT_Type, string groupId, string wire_Length, string wire_Type, string wire_Type_Core_Num)
        //{
        //    double X3 = (p1x + p2x) / 2;
        //    //int wire_Type_Core_Number = Convert.ToInt16(wire_Type_Core_Num);
        //    using (var writer = File.AppendText(Constants.el_ExecFilePath))
        //    {
        //        writer.WriteLine($"GRI 2.0, 2;");
        //        writer.WriteLine($"ADD L154 :W0 {p1x},{p1y} {p2x},{p2y} ;;NOP;;");
        //        writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {p1x},{p1y} '{iWireCode}' {p1x * 2},{p1y} ;NOP;");
        //        writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {p1x},{p1y} '#{iWireGauge}' {p1x * 2 + 2},{p1y} ;");
        //        if (!string.IsNullOrEmpty(wire_Type_Core_Num))
        //        {
        //            writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {p1x},{p1y} '{wire_Type_Core_Num}' {p1x * 2 + 2 + 8},{p1y - 1} ;");
        //        }
        //        writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 {p1x},{p1y} '{wire_Length}' {p1x * 2 + 2 + 8},{p1y} ;");
        //        writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {p1x},{p1y} '' {p1x * 2 + 2 + 8},{p1y} ;");

        //        //writer.WriteLine($"ADD L154 :W0 {p1x},{ p1y - 4} {p2x},{p2y - 4} ;;NOP;;");
        //        //writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {p1x},{p1y - 4} '{iWireCode}' {X3},{p2y - 4} ;NOP;");
        //        //writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {p1x},{p1y - 4} '#{iWireGauge}' {X3 + 2},{p2y - 4} ;");
        //        //writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {p1x},{p2y - 4} '{wire_Type_Core_Number + 1}' {X3 + 2 + 8},{p2y - 4 - 1} ;");
        //        //writer.WriteLine($"ADD N59 :R0 :D :J2 :F2 :T3009 :AC L154 {p1x},{p2y - 4} '{wire_Length}' {X3 + 2 + 8},{p2y - 4} ;");
        //        //writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {p1x},{p2y - 4} '' {X3 + 2 + 8},{p2y - 4} ;");

        //        writer.WriteLine($"ADD I2 sth_stp4 :R0 {p1x + 16},{p1y};");
        //        writer.WriteLine($"TESTDIS;");
        //        writer.WriteLine($"MOD N250 {p1x + 16},{p1y} 0,0 :L254 STOR_MID :E'{wire_Type}' JU;NOP;");
        //        writer.WriteLine($"MOD N250 {p1x + 16},{p1y - 4} 0,0 :L254 STOR_MID :E'{wire_Type}' JU;NOP;");
        //        writer.WriteLine($"TESTDIS_OFF;");
        //        writer.WriteLine($":RAW");
        //        writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {p1x + 16},{p1y} {p1x + 16 - 1.5},{p1y + 0.5};NOP;");
        //        writer.WriteLine($"MOD N253 {p1x + 16},{p1y} 0,0 STOR_MID :E'{groupId}' JU;NOP;");
        //        writer.WriteLine($":GRI");
        //        writer.WriteLine($"ADD I2 sth_stp4 :R0 {p2x - 16},{p2y};");
        //        writer.WriteLine($"TESTDIS;");
        //        writer.WriteLine($"MOD N250 {p2x - 16},{p2y} 0,0 :L254 STOR_MID :E'{wire_Type}' JU;NOP;");
        //        writer.WriteLine($"MOD N250 {p2x - 16},{p2y - 4} 0,0 :L254 STOR_MID :E'{wire_Type}' JU;NOP;");
        //        writer.WriteLine($"TESTDIS_OFF;");
        //        writer.WriteLine($":RAW");
        //        writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {p2x - 16},{p2y} {p2x - 16 - 1.5},{p2y + 0.5};NOP;");
        //        writer.WriteLine($"MOD N253 {p2x - 16},{p2y} 0,0 STOR_MID :E'{groupId}' JU; NOP;");
        //        writer.WriteLine($":GRI");
        //        writer.WriteLine($"pm_files_sav;;");
        //        writer.WriteLine($"GRI ELECTRE_GRID_STH;");
        //    }
        //}

        #region //old method
        //public static void Simple4PointConnection_ZLine_Optimized(double p1x, double p1y, double p2x, double p2y, string iWireCode, string iWireGauge, string c1, string c2, string iF_Type, string iT_Type, string groupId, string wire_Length, string wire_Type, string wire_Type_Core_Number, string FromOrientation, string ToOrientation)
        //{
        //    double X1 = 0, Y1 = 0, X2 = 0, Y2 = 0, X3 = 0, Y3 = 0, X4 = 0, Y4 = 0;
        //    X1 = p1x;
        //    Y1 = p1y;
        //    X2 = p2x;
        //    Y2 = p2y;

        //    // Existing Z-line logic
        //    X3 = (X1 + X2) / 2 + 10 - Constants.WiringOffset;
        //    X4 = X3;

        //    Y3 = Y1;
        //    Y4 = Y2;           

        //    string wire_symbol = Constants.GetWireSymbol(wire_Type);

        //    using (var writer = File.AppendText(Constants.el_ExecFilePath))
        //    {
        //        writer.WriteLine($"GRI 0.5, 2;");
        //        writer.WriteLine($"ADD L154 :W0");

        //        if (Y1 == Y2)
        //        {
        //            writer.WriteLine($"ADD L154 :W0 {X1},{Y1} {X2},{Y2} ;;NOP;;");
        //        }
        //        else
        //        {
        //            writer.WriteLine($"{X1},{Y1}");
        //            writer.WriteLine($"{X3},{Y3}");
        //            writer.WriteLine($"{X4},{Y4}");
        //            writer.WriteLine($"{X2},{Y2}");
        //        }

        //        if (Y1 == Y2)
        //        {
        //            // Wire codes in middle
        //            writer.WriteLine($";;NOP;;");
        //            writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {X1},{Y1} '{iWireCode}' {(X1 + X2) / 2},{Y1} ;NOP;");
        //            writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {X1},{Y1} '#{iWireGauge}' {(X1 + X2) / 2 + 2},{Y1} ;");
        //            writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {X1},{Y1} '{wire_Type_Core_Number}' {(X1 + X2) / 2 + 10},{Y1 - 1} ;");
        //            writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 {X1},{Y1} '{wire_Length}' {(X1 + X2) / 2 + 10},{Y1} ;");
        //            writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {X1},{Y1} '' {(X1 + X2) / 2 + 10},{Y1} ;");
        //        }
        //        else
        //        {
        //            //Source
        //            writer.WriteLine($";;NOP;;");
        //            writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {X1},{Y1} '{iWireCode}' {X1 + 48},{Y1} ;NOP;");
        //            writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {X1},{Y1} '#{iWireGauge}' {X1 + 48 + 2},{Y1} ;");
        //            writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {X1},{Y1} '{wire_Type_Core_Number}' {X1 + 48 + 2 + 8},{Y1 - 1} ;");
        //            writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 {X1},{Y1} '{wire_Length}' {X1 + 48 + 2 + 8},{Y1} ;");
        //            writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {X1},{Y1} '' {X1 + 48 + 2 + 8},{Y1} ;");

        //            //Destination
        //            writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {X2 - 25},{Y2} '{iWireCode}' {X2 - 25 * 2},{Y2} ;NOP;");
        //            writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {X2 - 25},{Y2} '#{iWireGauge}' {X2 - 25 * 2 + 2},{Y2} ;");
        //            writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {X2 - 25},{Y2} '{wire_Type_Core_Number}' {X2 - 25 * 2 + 2 + 8},{Y2 - 1} ;");
        //            writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 {X2 - 25},{Y2} '{wire_Length}' {X2 - 25 * 2 + 2 + 8},{Y2} ;");
        //            writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {X2 - 25},{Y2} '' {X2 - 25 * 2 + 2 + 8},{Y2} ;");
        //        }

        //         /*         writer.WriteLine($"{X1},{Y1}");
        //        writer.WriteLine($"{X3},{Y3}");
        //        writer.WriteLine($"{X4},{Y4}");
        //        writer.WriteLine($"{X2},{Y2}");

        //        //Source
        //        writer.WriteLine($";;NOP;;");
        //        writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {X1},{Y1} '{iWireCode}' {X1 + 48},{Y1} ;NOP;");
        //        writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {X1},{Y1} '#{iWireGauge}' {X1 + 48 + 2},{Y1} ;");
        //        writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {X1},{Y1} '{wire_Type_Core_Number}' {X1 + 48 + 2 + 8},{Y1 - 1} ;");
        //        writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 {X1},{Y1} '{wire_Length}' {X1 + 48 + 2 + 8},{Y1} ;");
        //        writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {X1},{Y1} '' {X1 + 48 + 2 + 8},{Y1} ;");

        //        //Destination
        //        writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {X2 - 25},{Y2} '{iWireCode}' {X2 - 25 * 2},{Y2} ;NOP;");
        //        writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {X2 - 25},{Y2} '#{iWireGauge}' {X2 - 25 * 2 + 2},{Y2} ;");
        //        writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {X2 - 25},{Y2} '{wire_Type_Core_Number}' {X2 - 25 * 2 + 2 + 8},{Y2 - 1} ;");
        //        writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 {X2 - 25},{Y2} '{wire_Length}' {X2 - 25 * 2 + 2 + 8},{Y2} ;");
        //        writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {X2 - 25},{Y2} '' {X2 - 25 * 2 + 2 + 8},{Y2} ;");*/

        //        if (!Constants.lst_WireCodes_Info_Processed.Contains(iWireCode))
        //        {
        //            writer.WriteLine($"ADD I2 {wire_symbol} :R0 {X1 + 16},{Y1};");
        //            writer.WriteLine($"TESTDIS;");
        //            writer.WriteLine($"MOD N250 {X1 + 16 - 2},{Y1 + 1.5} 0,0 :L254 STOR_MID :E'{wire_Type}' JU; NOP;");//wire type display
        //            writer.WriteLine($"TESTDIS_OFF;");
        //            writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {X1 + 16},{Y1} {X1 + 16 - 1.5},{Y1 + 0.5};NOP;");

        //            if (wire_Type.Equals("SS") || wire_Type.Equals("TP") || wire_Type.Equals("STP") || wire_Type.Equals("COAX") || wire_Type.Equals("TRIAX") || wire_Type.Equals("QUADRAX"))
        //            {
        //                writer.WriteLine($"MOD N253 {X1 + 16},{Y1} 0,0 STOR_MID :E'{groupId}' JU;NOP;");
        //            }
        //        }
        //        writer.WriteLine($":GRI");
        //        #region//Destination end symbol added
        //        if (!Constants.lst_WireCodes_Info_Processed.Contains(iWireCode))
        //        {
        //            writer.WriteLine($"ADD I2 {wire_symbol} :R0 {X2 - 16},{Y2};");
        //            writer.WriteLine($"TESTDIS;");
        //            writer.WriteLine($"MOD N250 {X2 - 16 - 2},{Y2 + 1.5} 0,0 :L254 STOR_MID :E'{wire_Type}' JU; NOP;");//wire type display
        //            writer.WriteLine($"TESTDIS_OFF;");
        //            writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {X2 - 16},{Y2} {X2 - 16 - 1.5},{Y2 + 0.5};NOP;");

        //            if (wire_Type.Equals("SS") || wire_Type.Equals("TP") || wire_Type.Equals("STP") || wire_Type.Equals("COAX") || wire_Type.Equals("TRIAX") || wire_Type.Equals("QUADRAX"))
        //            {
        //                writer.WriteLine($"MOD N253 {X2 - 16},{Y2} 0,0 STOR_MID :E'{groupId}' JU; NOP;");
        //            }
        //        }
        //        #endregion
        //        writer.WriteLine($":GRI");
        //        writer.WriteLine($"pm_files_sav;;");
        //        writer.WriteLine($"GRI ELECTRE_GRID_STH;");

        //        Constants.WiringOffset++;

        //        if (!Constants.lst_WireCodes_Info_Processed.Contains(iWireCode))
        //            Constants.lst_WireCodes_Info_Processed.Add(iWireCode);
        //    }
        //}
        #endregion

        #endregion
        #endregion
    }
}

using PanelDrawing.CommonOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanelDrawing.Services.P1
{
    public class EQUDoubleConnector
    {
        public static void GraLine(double X0,double Y0,int iNumberOfPins,string CompName,string[] pinList,string partNumber,string associatedPNs,string equipmentBoxName,string baseConnectorName)
        {
            // Skip if already drawn
            if (Constants.processedItems.Contains(baseConnectorName))
                return;

            int pinStep = -4;
            int totalPins = pinList.Length;

            // 1. Read orientation info
            string info =
                Constants.dataExtractionListBelow.FirstOrDefault(x =>string.Equals(x.ConnectorName, CompName, StringComparison.OrdinalIgnoreCase))
                ?.SymbolName?.Trim() ?? "";

            // 2. Base orientation
            string baseOrientation = info switch
            {
                "cont_sth_md" => "R",
                "cont_sth_mg" => "L",
                "cont_sth_half_l" => "R",
                "cont_sth_half_r" => "L",
                _ => ""
            };

            if (string.IsNullOrEmpty(baseOrientation))
                return;

            bool isHalfL = info == "cont_sth_half_l";
            bool isHalfR = info == "cont_sth_half_r";

            //using (var writer = File.AppendText(Constants.el_ExecFilePath))
            //{
            //    writer.WriteLine($"$$ EQU Double Start");
            //}

            //  SEGMENT 1 (HEADER)
            string o1 = isHalfL ? "R" : isHalfR ? "L" : baseOrientation;
            EquSeg1(X0, Y0, Constants.el_ExecFilePath, o1);

            //  SEGMENT 2 (PINS)
            string o2 = isHalfL ? "L" : isHalfR ? "R" : baseOrientation;

            for (int i = 0; i < pinList.Length; i++)
                EquSeg2(X0, Y0 + pinStep * i, pinList[i], Constants.el_ExecFilePath, o2, baseConnectorName, CompName, info);

            //  SEGMENT 3 (VERTICAL BODY)
            string o3 = isHalfL ? "R" : isHalfR ? "L" : baseOrientation;

            EquSeg3(X0, Y0, pinStep * iNumberOfPins, Constants.el_ExecFilePath, o3);

            //  SEGMENT 4 (RECTANGLE FRAME)
            string o4 = isHalfL ? "L" : isHalfR ? "R" : baseOrientation;

            EquSeg4(X0,Y0 - Math.Abs(pinStep) * totalPins - 6,X0,Y0,CompName,Constants.el_ExecFilePath,partNumber,o4,associatedPNs,baseConnectorName,info);

            //  SEGMENT 5 (FOOTER)
            string o5 = isHalfL ? "R" : isHalfR ? "L" : baseOrientation;

            EquSeg5(X0, Y0 + pinStep * iNumberOfPins, Constants.el_ExecFilePath, o5);

            //  EQUIPMENT BOX
            if (!string.IsNullOrWhiteSpace(equipmentBoxName) && equipmentBoxName != "+" && !equipmentBoxName.Equals("LOC", StringComparison.OrdinalIgnoreCase))
            {
                EquipmentBoxSeg(X0,Y0 - Math.Abs(pinStep) * totalPins,X0,Y0,equipmentBoxName,o4,Constants.el_ExecFilePath);
            }
            //using (var writer = File.AppendText(Constants.el_ExecFilePath))
            //{
            //    writer.WriteLine($"$$ EQU Double end");
            //}

            // Mark connector as processed
            Constants.processedItems.Add(baseConnectorName);
        }


        public static void EquSeg1(double X0, double Y0, string filePath, string strDISOrientation)
        {
            using (var writer = File.AppendText(filePath))
            {
                if (strDISOrientation.Equals("L"))
                {
                    writer.WriteLine($"ADD I1 cont_sth_1c_new_t :MY {X0},{Y0};");
                }
                else
                {
                    writer.WriteLine($"ADD I1 cont_sth_1c_new_t {X0 + 20},{Y0};");
                }
            }
        }

        public static void EquSeg2(double x,double y,string pinNumber,string filePath,string orientation,string baseConnectorName,string fullConnectorName,string tempOrientation)
        {
            using (var writer = File.AppendText(filePath))
            {
                bool isLeft = orientation.Equals("L", StringComparison.OrdinalIgnoreCase);
                bool isRight = orientation.Equals("R", StringComparison.OrdinalIgnoreCase);

                bool isHalfLeft = tempOrientation.Equals("cont_sth_half_l", StringComparison.OrdinalIgnoreCase);
                bool isHalfRight = tempOrientation.Equals("cont_sth_half_r", StringComparison.OrdinalIgnoreCase);

                // Find sibling connector (A/B or J1/J2 type)
                var pairedConnector =
                    Constants.listComponentsWithPartNumber
                    .Where(n => !string.IsNullOrEmpty(n)
                                && n.Contains(baseConnectorName)
                                && !n.Equals(fullConnectorName, StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();

                string siblingConnector = "";

                if (!string.IsNullOrEmpty(pairedConnector))
                {
                    siblingConnector =
                        Constants.dataExtractionListAbove.Where(row => row.ConnectorName.Equals(pairedConnector, StringComparison.OrdinalIgnoreCase) &&
                            //!string.IsNullOrEmpty(row.CoreNumber) &&
                            row.Panel.Equals(Constants.textPanelPartName, StringComparison.OrdinalIgnoreCase))
                        .Select(row => row.ConnectorName)
                        .FirstOrDefault();
                }
                bool hasSibling = !string.IsNullOrEmpty(siblingConnector);
                // LEFT SIDE
                if (isLeft)
                {
                    if (!isHalfLeft)
                    {
                        writer.WriteLine($"ADD Cont_sth_mg_hal {x},{y};");
                        writer.WriteLine($"MOD N51 {x + 0.8},{y} 0,0 :E '{pinNumber}'; NOP;");
                        writer.WriteLine($"Add N254 'EQU' :F1.0 :R0 :AC I0 {x},{y} :T4320 {x},{y - 2};NOP;");
                    }
                    else
                    {
                        writer.WriteLine($"ADD Cont_sth_mg {x + 10},{y};");
                        writer.WriteLine($"MOD N254 {x + 13},{y} 0,0 :E '{pinNumber}'; NOP;");
                        writer.WriteLine($"Add N254 'EQU' :F1.0 :R0 :AC I0 {x + 10},{y - 2} :T4320 {x + 10},{y - 2};NOP;");
                    }

                    // Sibling connector drawn on left side
                    if (hasSibling)
                    {
                        writer.WriteLine($"ADD Cont_sth_mg {x + 10},{y};");
                        writer.WriteLine($"MOD N254 {x + 7},{y} 0,0 :E '{pinNumber}'; NOP;");
                        writer.WriteLine($"Add N254 'EQU' :F1.0 :R0 :AC I0 {x + 10},{y} :T4320 {x},{y - 2};NOP;");
                    }
                }

                // RIGHT SIDE
                if (isRight)
                {
                    if (!isHalfRight)
                    {
                        writer.WriteLine($"ADD Cont_sth_md {x + 20},{y};");
                        writer.WriteLine($"MOD N51 {x + 11 - 2.5},{y} 0,0 :E '{pinNumber}'; NOP;");
                        writer.WriteLine($"Add N254 'EQU' :F1.0 :R0 :AC I0 {x + 20},{y - 2} :T4320 {x + 20},{y - 2};NOP;");
                    }
                    else
                    {
                        writer.WriteLine($"ADD Cont_sth_mg {x + 10},{y};");
                        writer.WriteLine($"MOD N254 {x + 13},{y} 0,0 :E '{pinNumber}'; NOP;");
                        writer.WriteLine($"Add N254 'EQU' :F1.0 :R0 :AC I0 {x + 10},{y - 2} :T4320 {x + 10},{y - 2};NOP;");
                    }

                    // Sibling connector drawn on right side
                    if (hasSibling)
                    {
                        writer.WriteLine($"ADD Cont_sth_mg {x + 10},{y};");
                        writer.WriteLine($"MOD N254 {x + 13},{y} 0,0 :E '{pinNumber}'; NOP;");
                        writer.WriteLine($"Add N254 'EQU' :F1.0 :R0 :AC I0 {x + 10},{y - 2} :T4320 {x + 10},{y - 2};NOP;");
                    }
                }
            }
        }

        public static void EquSeg3(double X0, double Y0, double L, string filePath, string strOri)
        {
            using (var writer = File.AppendText(filePath))
            {
                if (strOri.Equals("L"))
                {
                    writer.WriteLine($"ADD L214 {X0},{Y0} {X0},{Y0 + L}; ; ; ; NOP;");
                    writer.WriteLine($"ADD L214 {X0 + 7.5 - 0.1 + 0.01 + 0.002},{Y0} {X0 + 7.5 - 0.1 + 0.01 + 0.002},{Y0 + L}; ; ; ; NOP;");
                    writer.WriteLine($"ADD L214 {X0 + 10},{Y0} {X0 + 10},{Y0 + L}; ; ; ; NOP;");
                }
                if (strOri.Equals("R"))
                {
                    writer.WriteLine($"ADD L214 {X0 + 10},{Y0} {X0 + 10},{Y0 + L}; ; ; ; NOP;");
                    writer.WriteLine($"ADD L214 {X0 + 7.9 + 5 - 0.3 - 0.01 - 0.002},{Y0} {X0 + 7.9 + 5 - 0.3 - 0.01 - 0.002},{Y0 + L}; ; ; ; NOP;");
                    writer.WriteLine($"ADD L214 {X0 + 10 + 10},{Y0} {X0 + 10 + 10},{Y0 + L}; ; ; ; NOP;");
                }
            }
        }

        public static void EquSeg4(double LL_x,double LL_y,double UR_x,double UR_y,string compName,string filePath,string partNumber,string orientation,string associatedPNs,string baseConnectorName,string tempOri)
        {
            using (var writer = File.AppendText(filePath))
            {
                var w = writer;

                int widthSpaceIncrease = 0;

                // VB6 behavior: cont_sth_md shifts X
                if (tempOri.Equals("cont_sth_md", StringComparison.OrdinalIgnoreCase))
                    widthSpaceIncrease = 8;

                bool isLeft = orientation.Equals("L", StringComparison.OrdinalIgnoreCase);
                bool isRight = orientation.Equals("R", StringComparison.OrdinalIgnoreCase);

                //  CASE 1: LEFT ORIENTATION
                if (isLeft)
                {
                    int halfOffset = tempOri.Equals("cont_sth_half_l", StringComparison.OrdinalIgnoreCase) ? 0 : 4;

                    // MAIN RECTANGLE
                    w.WriteLine($"ADD R254 {LL_x + 8 - widthSpaceIncrease + 2.5 },{LL_y} {UR_x + 12 - widthSpaceIncrease + 2.5},{UR_y + 8};");

                    // HEADER LABEL RECTANGLE
                    w.WriteLine($"ADD N53 :T1001 :F3.0 :D :J7 :AC R254 {LL_x - 4 - 3 + 10},{LL_y - 30 - 48 + 8 + 8 + 2} '{compName}' {UR_x + 8 + 10},{UR_y + 4 - 6 - 12 + 8 + 8 + 2};");

                    // COMPONENT NAME
                    w.WriteLine($"ADD N253 '{compName}' {LL_x + 2 + 10},{LL_y + 2} :F1.0 :T1001 :D;;NOP;");

                    // PART NUMBER
                    w.WriteLine($"ADD N52 '{partNumber}' {LL_x + 3 + 10},{LL_y} :F1.0;;NOP;");

                    // ASSOCIATED PART NUMBERS
                    if (!string.IsNullOrWhiteSpace(associatedPNs))
                    {
                        int dy = 0;
                        foreach (string pn in associatedPNs.Split(';'))
                        {
                            dy += 4;
                            w.WriteLine($"ADD N52 '{pn}' {LL_x + 5 + 10},{LL_y - dy} :F1.0;;NOP;");
                        }
                    }

                    // N-MARKER
                    w.WriteLine($"ADD N255 'N' {LL_x + 12 + 10},{LL_y - 2} :F1.0 :T4326 :D;;NOP;");

                    //  OPPOSITE CONNECTOR (LEFT SIDE)

                    // Find sibling connector (like J1/J2, A/B, etc.)
                    string siblingName =
                        Constants.listComponentsWithPartNumber
                        .Where(x => !string.IsNullOrEmpty(x) &&
                                    x.Contains(baseConnectorName) &&
                                    !x.Equals(compName, StringComparison.OrdinalIgnoreCase))
                        .FirstOrDefault();

                    if (!string.IsNullOrEmpty(siblingName))
                    {
                        // Get sibling part number from DATAEXTRACTION LIST
                        string siblingPN =
                            Constants.dataExtractionListAbove
                            .Where(row =>
                                row.ConnectorName.Equals(siblingName, StringComparison.OrdinalIgnoreCase) &&
                                !string.IsNullOrEmpty(row.CoreNumber) &&
                                row.Panel.Equals(Constants.textPanelPartName, StringComparison.OrdinalIgnoreCase))
                            .Select(row => row.CoreNumber)
                            .FirstOrDefault();

                        //if (!string.IsNullOrEmpty(siblingPN))
                       // {
                            // Draw sibling's box
                            w.WriteLine($"ADD R254  {LL_x - 4 + 8 + 2 + 3},{LL_y} {UR_x + 4 + 8 + 2 + 3},{UR_y + 4 + 4} ;");

                            // Header
                            w.WriteLine($"ADD N53 :T1001 :F3.0 :D :J7 :AC R254 {LL_x - 4 - 3 + 20 + 15},{LL_y - 30 - 48 + 8 + 8 + 2} '{siblingName}' {UR_x + 4 + 8 - 4 - 2 + 20 + 15},{UR_y + 4 - 6 - 12 + 8 + 8 + 2};");

                            // Name
                            w.WriteLine($"ADD N253 '{siblingName}' {LL_x + 9},{LL_y + 3} :F1.0 :T1001 :D;;NOP;");

                            // PN
                            w.WriteLine($"ADD N52 '{siblingPN}' {LL_x + 10},{LL_y} :F1.0;;NOP;");

                            // N
                            w.WriteLine($"ADD N255 'N' {LL_x + 10},{LL_y - 3} :F1.0 :T4326 :D;;NOP;");
                        //}
                    }

                    return;
                }

                //  CASE 2: RIGHT ORIENTATION
                if (isRight)
                {
                    // MAIN RECTANGLE
                    w.WriteLine($"ADD R254  {LL_x - 4 + 8 + 2 + widthSpaceIncrease + 2.5},{LL_y} {UR_x + 4 + 8 + 2 + widthSpaceIncrease + 2.5},{UR_y + 4 + 4} ;");

                    // HEADER LABEL
                    w.WriteLine($"ADD N53 :T1001 :F3.0 :D :J7 :AC R254 {LL_x - 4 - 3 + 20 + 10},{LL_y - 30 - 48 + 8 + 8 + 2} '{compName}' {UR_x + 4 + 8 - 4 - 2 + 20 + 10},{UR_y + 4 - 6 - 12 + 8 + 8 + 2};");

                    // NAME
                    w.WriteLine($"ADD N253 '{compName}' {LL_x + 11 + 10},{LL_y + 3} :F1.0 :T1001 :D;;NOP;");

                    // PN
                    w.WriteLine($"ADD N52 '{partNumber}' {LL_x + 11 + 10},{LL_y} :F1.0;;NOP;");

                    // N-marker
                    w.WriteLine($"ADD N255 'N' {LL_x + 11 + 10},{LL_y - 3} :F1.0 :T4326 :D;;NOP;");

                    //  OPPOSITE CONNECTOR — RIGHT

                    string sibling = Constants.listComponentsWithPartNumber
                        .Where(x => !string.IsNullOrEmpty(x) && x.Contains(baseConnectorName) &&
                                    !x.Equals(compName, StringComparison.OrdinalIgnoreCase))
                        .FirstOrDefault();

                    if (!string.IsNullOrEmpty(sibling))
                    {
                        string siblingPN =
                            Constants.dataExtractionListAbove
                            .Where(row =>
                                row.ConnectorName.Equals(sibling, StringComparison.OrdinalIgnoreCase) &&
                                !string.IsNullOrEmpty(row.CoreNumber) &&
                                row.Panel.Equals(Constants.textPanelPartName, StringComparison.OrdinalIgnoreCase))
                            .Select(row => row.CoreNumber)
                            .FirstOrDefault();

                        //if (!string.IsNullOrEmpty(siblingPN))
                        //{
                            // Draw sibling box
                            w.WriteLine($"ADD R254  {LL_x - 4 + 8 + 2},{LL_y} {UR_x + 4 + 8 + 2},{UR_y + 4 + 4} ;");

                            // Header
                            w.WriteLine($"ADD N53 :T1001 :F3.0 :D :J7 :AC R254 {LL_x - 4 - 3},{LL_y - 30 - 48 + 8 + 8 + 2} '{sibling}' {UR_x + 4 + 8 - 4 - 2},{UR_y + 4 - 6 - 12 + 8 + 8 + 2};");

                            // Name
                            w.WriteLine($"ADD N253 '{sibling}' {LL_x - 6},{LL_y + 3} :F1.0 :T1001 :D;;NOP;");

                            // PN
                            w.WriteLine($"ADD N52 '{siblingPN}' {LL_x - 20},{LL_y} :F1.0;;NOP;");

                            // N mark
                            w.WriteLine($"ADD N255 'N' {LL_x},{LL_y - 3} :F1.0 :T4326 :D;;NOP;");
                       // }
                    }
                }
            }
        }

        public static void EquSeg5(double X0, double Y0, string filePath, string strOri)
        {
            using (var writer = File.AppendText(filePath))
            {
                if (strOri.Equals("L"))
                {
                    writer.WriteLine($"ADD I1 cont_sth_lastc_new_t :R0 :MY {X0},{Y0 + 2};");
                    writer.WriteLine("$$ DisSeg - End...");
                }
                if (strOri.Equals("R"))
                {
                    writer.WriteLine($"ADD I1 cont_sth_lastc_new_t {X0 + 20},{Y0 + 2};");
                    writer.WriteLine("$$ DisSeg - End...");
                }
            }
        }

        public static void EquipmentBoxSeg(double LL_x, double LL_y, double UR_x, double UR_y, string iEquName, string Ori, string filePath)
        {
            double w = 6;   // Width
            double O = 8;   // Offset


            using (var writer = File.AppendText(filePath))
            {
                if (Ori == "R")
                {
                    writer.WriteLine($"ADD R252 {LL_x},{LL_y - 18} {UR_x + 30},{UR_y + 12};;;NOP;");
                    writer.WriteLine($"ADD N202 '{iEquName}' :T1001 :F3.0 :D :AC R252 {LL_x - 18},{LL_y - 8} {UR_x + 4},{UR_y + 12};;NOP;");
                }
                else if (Ori == "L")
                {
                    writer.WriteLine($"ADD R252 {LL_x - 10},{LL_y - 18} {UR_x + 30 - 10},{UR_y + 12};;;NOP;");
                    writer.WriteLine($"ADD N202 '{iEquName}' :T1001 :F3.0 :D :AC R252 {LL_x - 18},{LL_y - 8} {UR_x + 4},{UR_y + 12};;NOP;");
                    //writer.WriteLine($"ADD R252 {LL_x - 2.5},{LL_y - O} {UR_x + w},{UR_y + O * 2};;;NOP;");
                    //writer.WriteLine($"ADD N202 '{iEquName}' :T1001 :F3.0 :D :AC R252 {LL_x},{LL_y - O} {LL_x + 1},{UR_y + O * 2};;NOP;");
                }

            }
        }
    }
}

using PanelDrawing.CommonOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PanelDrawing.Services.P1
{
    public class GraLine_DIS
    {
        public static void GraLine(double X0, double Y0, int iNumberOfPins, string CompName, string[] arrpinsCollection, string partnumber, string assopns, string DisEquipmentBoxRefName)
        {
            // Break connector base name (removes last 2 chars)
            string baseName = CompName.Length > 2 ? CompName[..^2] : CompName;

            // If already processed → exit
            if (Constants.processedItems.Contains(baseName))
                return;

            // Orientation lookup result
            string info = "";

            // 1. Look up exact connector orientation in BELOW list
            var match = Constants.dataExtractionListBelow.FirstOrDefault(x =>
                    string.Equals(x.ConnectorName?.Trim(), CompName.Trim(), StringComparison.OrdinalIgnoreCase));

            if (match != null)
                info = match.SymbolName?.Trim() ?? "";

            // 2. If type "receptacle", swap M/F and search again
            if (info.Equals("contact_sth_receptacle", StringComparison.OrdinalIgnoreCase))
            {
                string swapped = CompName;
                if (swapped.EndsWith("_F", StringComparison.OrdinalIgnoreCase))
                    swapped = swapped[..^2] + "_M";
                else if (swapped.EndsWith("_M", StringComparison.OrdinalIgnoreCase))
                    swapped = swapped[..^2] + "_F";

                var match2 = Constants.dataExtractionListBelow.FirstOrDefault(x =>
                    string.Equals(x.ConnectorName?.Trim(), swapped.Trim(), StringComparison.OrdinalIgnoreCase));

                if (match2 != null)
                    info = match2.SymbolName?.Trim() ?? "";

                // shift X left by 8 (VB6 logic)
                X0 -= 8;
            }

            // 3. Convert info → orientation
            string orientation = info switch
            {
                "cont_sth_md" => "R",
                "cont_sth_mg" => "L",
                _ => ""
            };

            if (string.IsNullOrEmpty(orientation))
                return; // no valid orientation → cannot draw

            int incre = -4;
            int T = arrpinsCollection.Length;

            // Draw DIS Header
            DisSeg1(X0, Y0, Constants.el_ExecFilePath, orientation);

            // Draw each pin segment
            for (int i = 0; i < arrpinsCollection.Length; i++)
            {
                DisSeg2(X0, Y0 + incre * i, arrpinsCollection[i], Constants.el_ExecFilePath, orientation, baseName, CompName);
            }

            // Draw vertical spine
            DisSeg3(X0, Y0, incre * iNumberOfPins, Constants.el_ExecFilePath, orientation);

            // Draw rectangle (DisSeg4)
            DisSeg4(X0, Y0 - Math.Abs(incre) * T - 6, X0, Y0, CompName, Constants.el_ExecFilePath, orientation, partnumber, assopns, baseName);

            // Draw footer
            DisSeg5(X0, Y0 + incre * iNumberOfPins, Constants.el_ExecFilePath, orientation);

            // Equipment Box
            if (!string.IsNullOrWhiteSpace(DisEquipmentBoxRefName) && DisEquipmentBoxRefName != "+" && !DisEquipmentBoxRefName.Equals("LOC", StringComparison.OrdinalIgnoreCase))
            {
                DISSegEquipmentBox(X0, Y0 - Math.Abs(incre) * T, X0, Y0, DisEquipmentBoxRefName, orientation, Constants.el_ExecFilePath);
            }

            // Mark as processed
            Constants.processedItems.Add(baseName);
        }

        public static void DisSeg1(double X0, double Y0, string filePath, string strDISOrientation)
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

        public static void DisSeg2(double X0, double Y0, string N, string filePath, string Ori, string baseConnectorName, string fullConnectorName)
        {
            using (var writer = File.AppendText(filePath))
            {
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
                           // !string.IsNullOrEmpty(row.CoreNumber) &&
                            row.Panel.Equals(Constants.textPanelPartName, StringComparison.OrdinalIgnoreCase))
                        .Select(row => row.ConnectorName)
                        .FirstOrDefault();
                }
                bool hasSibling = !string.IsNullOrEmpty(siblingConnector);

                if (Ori.Equals("L"))
                {
                    writer.WriteLine($"ADD Cont_sth_mg_hal {X0},{Y0};");//oRIG
                    writer.WriteLine($"MOD N51 {X0 + 0.8},{Y0} 0,0 :E '{N}'; NOP;");
                    writer.WriteLine($"Add N254 'DIS' :F1.0 :R0 :AC I0 {X0},{Y0} :T4320 {X0},{Y0 - 2};NOP;");

                    if (hasSibling)
                    {
                        writer.WriteLine($"ADD contact_sth_receptacle {X0 + 10},{Y0};");
                        writer.WriteLine($"MOD N254 {X0 + 11 - 2.5},{Y0} 0,0 :E '{N}'; NOP;");
                        writer.WriteLine($"Add N254 'DIS' :F1.0 :R0 :AC I0 {X0 + 10},{Y0 - 2} :T4320 {X0 + 10},{Y0 - 2};NOP;");
                    }
                }
                else if (Ori.Equals("R"))//Right Side Pins
                {
                    writer.WriteLine($"ADD contact_sth_receptacle {X0 + 10},{Y0};");
                    writer.WriteLine($"MOD N254 {X0 + 11 - 2.5},{Y0} 0,0 :E '{N}'; NOP;");
                    writer.WriteLine($"Add N254 'DIS' :F1.0 :R0 :AC I0 {X0 + 10},{Y0 - 2} :T4320 {X0 + 10},{Y0 - 2};NOP;");

                    if (hasSibling)
                    {
                        writer.WriteLine($"ADD Cont_sth_mg_hal {X0},{Y0};");//oRIG
                        writer.WriteLine($"MOD N51 {X0 + 0.8},{Y0} 0,0 :E '{N}'; NOP;");
                        writer.WriteLine($"Add N254 'DIS' :F1.0 :R0 :AC I0 {X0},{Y0} :T4320 {X0},{Y0 - 2};NOP;");
                    }
                }               
            }
        }

        #region //Commented DisSeg2 on Dec 8, 2025
        //public static void DisSeg2_OLD(double X0, double Y0, string N, string filePath, string Ori)
        //{
        //    using (var writer = File.AppendText(filePath))
        //    {
        //        if (Ori.Equals("L"))
        //        {
        //            writer.WriteLine($"ADD Cont_sth_mg_hal {X0},{Y0};");//oRIG
        //            writer.WriteLine($"MOD N51 {X0 + 0.8},{Y0} 0,0 :E '{N}'; NOP;");
        //            writer.WriteLine($"Add N254 'DIS' :F1.0 :R0 :AC I0 {X0},{Y0} :T4320 {X0},{Y0 - 2};NOP;");
        //        }
        //        if (Ori.Equals("R"))//Right Side Pins
        //        {
        //            writer.WriteLine($"ADD contact_sth_receptacle {X0 + 10},{Y0};");
        //            writer.WriteLine($"MOD N254 {X0 + 11 - 2.5},{Y0} 0,0 :E '{N}'; NOP;");
        //            writer.WriteLine($"Add N254 'DIS' :F1.0 :R0 :AC I0 {X0 + 10},{Y0 - 2} :T4320 {X0 + 10},{Y0 - 2};NOP;");
        //        }
        //    }
        //}
        #endregion

        public static void DisSeg3(double X0, double Y0, double L, string filePath, string strOri)
        {
            using (var writer = File.AppendText(filePath))
            {
                if (strOri.Equals("L"))
                {
                    writer.WriteLine($"ADD L214 {X0},{Y0} {X0},{Y0 + L}; ; ; ; NOP;");
                    writer.WriteLine($"ADD L214 {X0 + 7.5 - 0.1 + 0.05},{Y0} {X0 + 7.5 - 0.1 + 0.05},{Y0 + L}; ; ; ; NOP;");
                    // writer.WriteLine($"ADD L214 {X0 + 7.5 - 0.1 + 0.01 + 0.002},{Y0} {X0 + 7.5 - 0.1 + 0.01 + 0.002},{Y0 + L}; ; ; ; NOP;");
                    writer.WriteLine($"ADD L214 {X0 + 10},{Y0} {X0 + 10},{Y0 + L}; ; ; ; NOP;");
                }
                if (strOri.Equals("R"))
                {
                    writer.WriteLine($"ADD L214 {X0 + 10},{Y0} {X0 + 10},{Y0 + L}; ; ; ; NOP;");
                    //writer.WriteLine($"ADD L214 {X0 + 7.9 + 5 - 0.3 - 0.01 - 0.002},{Y0} {X0 + 7.9 + 5 - 0.3 - 0.01 - 0.002},{Y0 + L}; ; ; ; NOP;");
                    writer.WriteLine($"ADD L214 {X0 + 7.9 + 5 - 0.3 - 0.05},{Y0} {X0 + 7.9 + 5 - 0.3 - 0.05},{Y0 + L}; ; ; ; NOP;");
                    writer.WriteLine($"ADD L214 {X0 + 10 + 10},{Y0} {X0 + 10 + 10},{Y0 + L}; ; ; ; NOP;");
                }
            }
        }

        #region //Commented DisSeg4 on Dec 8, 2025
        //public static void DisSeg4_OLD(double LL_x, double LL_y, double UR_x, double UR_y, string CompNmae, string filePath, string Ori, string partnumber, string assoPNs)
        //{
        //    //double w = 6;   // Width
        //    // double O = 4;   // Offset
        //    using (var writer = File.AppendText(filePath))
        //    {
        //        if (Ori.Equals("L"))
        //        {
        //            writer.WriteLine($"ADD R254  {LL_x - 4},{LL_y} {UR_x + 4},{UR_y + 4 + 4} ;");
        //            writer.WriteLine($"ADD N53 :T1001 :F3.0 :D :J7 :AC R254 {LL_x - 4 - 3},{LL_y - 30 - 48 + 8 + 8 + 2} '{CompNmae}' {UR_x + 4 - 8},{UR_y + 4 - 6 - 12 + 8 + 8 + 2};");
        //            writer.WriteLine($"ADD N253 '{CompNmae}' {LL_x + 2},{LL_y + 3} :F1.0 :T1001 :D;;NOP;");
        //            writer.WriteLine($"ADD N52 '{partnumber}' {LL_x + 2},{LL_y} :F1.0;;NOP;");
        //            writer.WriteLine($"MOD N52 {LL_x + 2},{LL_y} 0,0 :E '{partnumber}';NOP;");
        //            //Included Assosiate PartNumbers for Break Connectors Family
        //            if (!string.IsNullOrEmpty(assoPNs))
        //            {
        //                int decre = 0;
        //                string[] arrassopns = assoPNs.Split(";");
        //                for (int assopns = 0; assopns <= arrassopns.Length - 1; assopns++)
        //                {
        //                    string associatePartNumber = arrassopns[assopns];
        //                    if (partnumber != associatePartNumber)
        //                    {
        //                        decre = decre + 4;
        //                        writer.WriteLine($"ADD N52 '{arrassopns[assopns]}' {LL_x + 2 + 3},{LL_y - decre} :F1.0;;NOP;");
        //                    }
        //                }
        //            }
        //            writer.WriteLine($"ADD N255 'N' {LL_x + 2},{LL_y - 3} :F1.0 :T4326 :D;;NOP;");
        //        }
        //        if (Ori.Equals("R"))
        //        {
        //            writer.WriteLine($"ADD R254  {LL_x - 4 + 10},{LL_y} {UR_x + 4 + 10},{UR_y + 4 + 4} ;");
        //            writer.WriteLine($"ADD N53 :T1001 :F3.0 :D :J7 :AC R254 {LL_x - 4 - 3},{LL_y - 30 - 48 + 8 + 8 + 2} '{CompNmae}' {UR_x + 4 + 8 - 4 - 2},{UR_y + 4 - 6 - 12 + 8 + 8 + 2};");
        //            writer.WriteLine($"ADD N253 '{CompNmae}' {LL_x + 2 + 9},{LL_y + 3} :F1.0 :T1001 :D;;NOP;");
        //            writer.WriteLine($"ADD N52 '{partnumber}' {LL_x + 2 + 9},{LL_y} :F1.0;;NOP;");
        //            writer.WriteLine($"MOD N52 {LL_x + 2 + 9},{LL_y} 0,0 :E '{partnumber}';NOP;");
        //            writer.WriteLine($"ADD N255 'N' {LL_x + 2 + 9},{LL_y - 3} :F1.0 :T4326 :D;;NOP;");
        //        }
        //    }
        //}
        #endregion

        public static void DisSeg4(double LL_x, double LL_y, double UR_x, double UR_y, string CompName, string filePath, string Ori, string partnumber, string assoPNs,string BaseName)
        {
            //double w = 6;   // Width
           // double O = 4;   // Offset
            using (var writer = File.AppendText(filePath))
            {
                bool isLeft = Ori.Equals("L", StringComparison.OrdinalIgnoreCase);
                bool isRight = Ori.Equals("R", StringComparison.OrdinalIgnoreCase);

                string siblingName = Constants.panelComponentProperties
                    .Where(x => x.ComponentName != null &&
                                x.ComponentName.StartsWith(BaseName) &&
                                !x.ComponentName.Equals(CompName, StringComparison.OrdinalIgnoreCase))
                    .Select(x => x.ComponentName)
                    .FirstOrDefault();

                string siblingPN = null;

                if (!string.IsNullOrEmpty(siblingName))
                {
                    siblingPN =
                         Constants.dataExtractionListAbove
                         .Where(row =>
                             row.ConnectorName.Equals(siblingName, StringComparison.OrdinalIgnoreCase) &&
                             !string.IsNullOrEmpty(row.CoreNumber) &&
                             row.Panel.Equals(Constants.textPanelPartName, StringComparison.OrdinalIgnoreCase))
                         .Select(row => row.CoreNumber)
                         .FirstOrDefault();
                }

                if (isLeft)
                {
                    if (string.IsNullOrEmpty(siblingName))
                    {                        
                        writer.WriteLine($"ADD R254  {LL_x - 4},{LL_y} {UR_x + 4},{UR_y + 8} ;"); // 480 80
                        writer.WriteLine($"ADD N53 :T1001 :F3.0 :D :J7 :AC R254 {LL_x - 2},{UR_y + 4} '{CompName}' {LL_x - 2},{UR_y + 4};");
                        writer.WriteLine($"ADD N253 '{CompName}' {LL_x - 15 },{LL_y + 2} :F1.0 :T1001 :D;;NOP;");
                        writer.WriteLine($"ADD N52 '{partnumber}' {LL_x - 15},{LL_y - 1} :F1.0;;NOP;");
                        writer.WriteLine($"MOD N52 {LL_x -  15},{LL_y - 1} 0,0 :E '{partnumber}';NOP;");
                        writer.WriteLine($"ADD N255 'N' {LL_x - 10},{LL_y - 4} :F1.0 :T4326 :D;;NOP;");
                    }
                    else
                    {
                        writer.WriteLine($"ADD R254  {LL_x - 4},{LL_y} {UR_x + 4},{UR_y + 8} ;");
                        writer.WriteLine($"ADD N53 :T1001 :F3.0 :D :J7 :AC R254 {LL_x + 38},{UR_y + 4} '{CompName}' {LL_x + 38},{UR_y + 4};");
                        writer.WriteLine($"ADD N253 '{CompName}' {LL_x + 10},{LL_y + 2} :F1.0 :T1001 :D;;NOP;");
                        writer.WriteLine($"ADD N52 '{partnumber}' {LL_x + 10},{LL_y - 1} :F1.0;;NOP;");
                        writer.WriteLine($"MOD N52 {LL_x + 10},{LL_y - 1} 0,0 :E '{partnumber}';NOP;");
                        writer.WriteLine($"ADD N255 'N' {LL_x + 10},{LL_y - 4} :F1.0 :T4326 :D;;NOP;");

                        if (!string.IsNullOrEmpty(siblingName))
                        {
                            writer.WriteLine($"ADD R254  {LL_x + 6},{LL_y} {UR_x + 14},{UR_y + 8} ;");
                            writer.WriteLine($"ADD N53 :T1001 :F3.0 :D :J7 :AC R254 {LL_x - 7},{UR_y + 4} '{siblingName}' {LL_x - 7},{UR_y + 4};");
                            writer.WriteLine($"ADD N253 '{siblingName}' {LL_x - 18},{LL_y + 2} :F1.0 :T1001 :D;;NOP;");
                            writer.WriteLine($"ADD N52 '{siblingPN}' {LL_x - 20},{LL_y - 1} :F1.0;;NOP;");
                            writer.WriteLine($"MOD N52 {LL_x - 20},{LL_y - 1} 0,0 :E '{siblingPN}';NOP;");
                            writer.WriteLine($"ADD N255 'N' {LL_x - 2},{LL_y - 4} :F1.0 :T4326 :D;;NOP;");
                        }
                    }


                    //Included Assosiate PartNumbers for Break Connectors Family
                    if (!string.IsNullOrEmpty(assoPNs))
                    {
                        int decre = 0;
                        string[] arrassopns = assoPNs.Split(";");
                        for (int assopns = 0; assopns <= arrassopns.Length - 1; assopns++)
                        {
                            string associatePartNumber = arrassopns[assopns];
                            if (partnumber != associatePartNumber)
                            {
                                decre = decre + 4;
                                writer.WriteLine($"ADD N52 '{arrassopns[assopns]}' {LL_x + 2 + 3},{LL_y - decre} :F1.0;;NOP;");
                            }
                        }
                    }             
                }
                if (isRight)
                {
                    if (string.IsNullOrEmpty(siblingName))
                    {
                        writer.WriteLine($"ADD R254  {LL_x - 4},{LL_y} {UR_x + 4},{UR_y + 8} ;");
                        writer.WriteLine($"ADD N53 :T1001 :F3.0 :D :J7 :AC R254 {LL_x + 38},{UR_y + 4} '{CompName}' {LL_x + 38},{UR_y + 4};");
                        writer.WriteLine($"ADD N253 '{CompName}' {LL_x + 10},{LL_y + 2} :F1.0 :T1001 :D;;NOP;");
                        writer.WriteLine($"ADD N52 '{partnumber}' {LL_x + 10},{LL_y - 1} :F1.0;;NOP;");
                        writer.WriteLine($"MOD N52 {LL_x + 10},{LL_y - 1} 0,0 :E '{partnumber}';NOP;");
                        writer.WriteLine($"ADD N255 'N' {LL_x + 10},{LL_y - 4} :F1.0 :T4326 :D;;NOP;");
                    }
                    else
                    {
                        writer.WriteLine($"ADD R254  {LL_x + 6},{LL_y} {UR_x + 14},{UR_y + 8} ;");
                        writer.WriteLine($"ADD N53 :T1001 :F3.0 :D :J7 :AC R254 {LL_x - 7},{UR_y + 4}'{CompName}' {LL_x - 7},{UR_y + 4};");
                        writer.WriteLine($"ADD N253 '{CompName}' {LL_x - 18},{LL_y + 2} :F1.0 :T1001 :D;;NOP;");
                        writer.WriteLine($"ADD N52 '{partnumber}'  {LL_x - 20},{LL_y - 1} :F1.0;;NOP;");
                        writer.WriteLine($"MOD N52  {LL_x - 20},{LL_y - 1} 0,0 :E '{partnumber}';NOP;");
                        writer.WriteLine($"ADD N255 'N' {LL_x - 2},{LL_y - 4} :F1.0 :T4326 :D;;NOP;");

                        if (!string.IsNullOrEmpty(siblingName))
                        {
                            writer.WriteLine($"ADD R254  {LL_x - 4},{LL_y} {UR_x + 4},{UR_y + 8} ;");
                            writer.WriteLine($"ADD N53 :T1001 :F3.0 :D :J7 :AC R254 {LL_x + 38},{UR_y + 4} '{siblingName}' {LL_x + 38},{UR_y + 4};");
                            writer.WriteLine($"ADD N253 '{siblingName}' {LL_x + 10},{LL_y + 2} :F1.0 :T1001 :D;;NOP;");
                            writer.WriteLine($"ADD N52 '{siblingPN}' {LL_x + 10},{LL_y - 1} :F1.0;;NOP;");
                            writer.WriteLine($"MOD N52 {LL_x + 10},{LL_y - 1} 0,0 :E '{siblingPN}';NOP;");
                            writer.WriteLine($"ADD N255 'N' {LL_x + 10},{LL_y - 4} :F1.0 :T4326 :D;;NOP;");
                        }
                    }
                    //Included Assosiate PartNumbers for Break Connectors Family
                    if (!string.IsNullOrEmpty(assoPNs))
                    {
                        int decre = 0;
                        string[] arrassopns = assoPNs.Split(";");
                        for (int assopns = 0; assopns <= arrassopns.Length - 1; assopns++)
                        {
                            string associatePartNumber = arrassopns[assopns];
                            if (partnumber != associatePartNumber)
                            {
                                decre = decre + 4;
                                writer.WriteLine($"ADD N52 '{arrassopns[assopns]}' {LL_x + 2 + 3},{LL_y - decre} :F1.0;;NOP;");
                            }
                        }
                    }
                    
                }
            }
        }
        public static void DisSeg5(double X0, double Y0, string filePath, string strOri)
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
        public static void DISSegEquipmentBox(double LL_x, double LL_y, double UR_x, double UR_y, string iEquName, string Ori, string filePath)
        {
           // double w = 6;   // Width
           // double O = 8;   // Offset

            using (var writer = File.AppendText(filePath))
            {
                if (Ori == "R")
                {
                    writer.WriteLine($"ADD R252 {LL_x},{LL_y - 18} {UR_x + 30},{UR_y + 12};;;NOP;");
                    writer.WriteLine($"ADD N202 '{iEquName}' :T1001 :F3.0 :D :AC R252 {LL_x - 18},{LL_y - 8} {UR_x + 4},{UR_y + 12};;NOP;");
                }
                else if (Ori == "L")
                {
                    writer.WriteLine($"ADD R252 {LL_x},{LL_y + 18} {UR_x - 30},{UR_y - 12};;;NOP;");
                    writer.WriteLine($"ADD N202 '{iEquName}' :T1001 :F3.0 :D :AC R252 {LL_x - 18},{LL_y - 8} {UR_x + 4},{UR_y + 12};;NOP;");
                }

            }
        }
    }
}

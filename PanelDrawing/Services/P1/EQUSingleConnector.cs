using Panel_Drawing.Forms;
using PanelDrawing.CommonOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanelDrawing.Services.P1
{
    public class EQUSingleConnector
    {
        public static void DrawEquSymbolUsedPins(double X0, double Y0, string iNumberOfMaxPins, string iEquName, string side, string iSamplePinNumber,
             string iPartNumber, /*frmPanelOri f,*/ string assoPNs, string EquBoxExist, string LoomsExist)
        {
            List<string> arrPins = new List<string>();
            int incre_y = -4;
            int I = 0;            
            string IsFullConnector = string.Empty;

            string Ori = side == "LEFT" ? "L" : "R";

            // parse maximum pins
            int numberOfPins = 0;
            if (string.IsNullOrWhiteSpace(iNumberOfMaxPins) || !int.TryParse(iNumberOfMaxPins.Trim(), out numberOfPins))
            {
                numberOfPins = 0; // fallback (same as your old code)
            }

            int l = 0;
            arrPins = Constants.listPinsOfEqu;

            // The VB6 code used "T - 1" checks repeatedly; replicate those semantics
            if (arrPins.Count - 1 < numberOfPins)
            {
                IsFullConnector = "no";
            }
            else if (arrPins.Count == numberOfPins)
            {
                IsFullConnector = "yes";
            }

            // keep same call so existing seg methods will handle file writing
            EquSeg1(X0, Y0, Ori, "Full", iEquName, Constants.el_ExecFilePath);

            // iterate pins with original indexing (1 .. T-1)
            for (int i = 0; i < arrPins.Count; i++)
            {
                double yPin = Y0 + incre_y * (I - 1) + l - 4;
                // call pin segment writer
                EquSeg2(X0, yPin, arrPins[i].ToString(), Ori, Constants.el_ExecFilePath);
                l = l - 4;
            }
            
            // vertical lines
            EquSeg3(X0 - 7.5, Y0, incre_y * (arrPins.Count), Ori, Constants.el_ExecFilePath);

            Y0 -= 4;

            // Rectangle (LLx, LLy, URx, URy) — original call
            EquSeg4(X0, Y0 - Math.Abs(incre_y) * arrPins.Count, X0, Y0, iEquName, Ori, Constants.el_ExecFilePath);

            // Footer — preserve original signature and pass IsFullConnector
            EquSeg5(X0, Y0 + incre_y * (arrPins.Count - 1), Ori, "Full", iEquName, Constants.el_ExecFilePath, IsFullConnector);

            // change Y0 so partnumber and equipment box placed correctly (same as original)
            Y0 = Y0 - Math.Abs(incre_y) * arrPins.Count;
            EquSegPartNumber(X0, Y0 - 10, iEquName, iPartNumber, Constants.el_ExecFilePath, assoPNs);

            // Equipment box if present
            if (!string.IsNullOrWhiteSpace(EquBoxExist) && !string.Equals(EquBoxExist, "+", StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(EquBoxExist, "LOC", StringComparison.OrdinalIgnoreCase))
            {
                SegEquipmentBox(X0 - 8, Y0 + 4 - Math.Abs(incre_y) * arrPins.Count, X0 + 16, Y0 + 20, EquBoxExist, Ori, Constants.el_ExecFilePath);
            }

            Y0 = Y0 - 50; // as original

            // end left connector

            // RIGHT CONNECTOR (grid column 1)
            //var rightCell = f.grdOriInfo[1, 1].Value;
            //if (rightCell != null && !string.IsNullOrEmpty(rightCell.ToString()))
            //if(side == "RIGHT")
            //{
            //    Ori = "R";
            //    int r = 0;
            //    arrPins = Constants.listPinsOfEqu;
            //    T = arrPins.Count;

            //    if (T - 1 < numberOfPins)
            //    {
            //        IsFullConnector = "no";
            //    }
            //    else if (T == numberOfPins)
            //    {
            //        IsFullConnector = "yes";
            //    }

            //    // Header
            //    EquSeg1(X0, Y0, Ori, "Full", iEquName, Constants.el_ExecFilePath);

            //    // iterate pins (1..T-1) preserve original arithmetic
            //    for (int i0 = 0; i0 <= T; i0++)
            //    {
            //        double yPin = Y0 + incre_y * (I - 1) + r - 4;
            //        EquSeg2(X0, yPin, arrPins[i0], Ori, Constants.el_ExecFilePath);
            //        r = r - 4;
            //    }

            //    // vertical lines
            //    EquSeg3(X0 - 7.5, Y0, incre_y * (T - 1), Ori, Constants.el_ExecFilePath);

            //    // rectangle
            //    EquSeg4(X0, Y0 - Math.Abs(incre_y) * T, X0, Y0, iEquName, Ori, Constants.el_ExecFilePath);

            //    // footer and partnumber
            //    EquSeg5(X0, Y0 + incre_y * (T - 1), Ori, "Full", iEquName, Constants.el_ExecFilePath, IsFullConnector);

            //    Y0 = Y0 - Math.Abs(incre_y) * T;
            //    EquSegPartNumber(X0, Y0 - 10, iEquName, iPartNumber, Constants.el_ExecFilePath, assoPNs);

            //    if (!string.IsNullOrWhiteSpace(EquBoxExist) && !string.Equals(EquBoxExist, "+", StringComparison.OrdinalIgnoreCase)
            //             && !string.Equals(EquBoxExist, "LOC", StringComparison.OrdinalIgnoreCase))
            //    {
            //        SegEquipmentBox(X0 - 8, Y0 + 4 - Math.Abs(incre_y) * T, X0 + 16, Y0 + 20, EquBoxExist, Ori, Constants.el_ExecFilePath);
            //    }

            //    Y0 = Y0 - 50;
            //} // end right connector

            #region//Top and bottom Connecter
            //if (!string.IsNullOrEmpty((string)f.grdOriInfo[1,2].Value))
            //{
            //    arrPins = f.grdOriInfo[1,2].Value.ToString().Split(",");
            //    T = arrPins.Length; 
            //    Ori = "T";
            //    if (T - 1 < Convert.ToInt16(iNumberOfPins))
            //    {
            //        IsFullConnector = "no";
            //    }
            //    else if (T == Convert.ToInt16(iNumberOfPins))
            //    {
            //        IsFullConnector = "yes";
            //    }
            //    int b = 0;
            //    //X0 = 100;
            //    //Y0 = 150;
            //    for (int I1 = 1; I1 <= T-1; I1++)
            //    {
            //        modOPCommand.EquSeg2(X0 + b, Y0, arrPins[I1], Ori, Constants.el_ExecFilePath);
            //        b = 4;
            //    }

            //    // Reordered EquSeg1 as EQU was not getting mapped properly to pin in Top and Bottom
            //    modOPCommand.EquSeg1(X0, Y0, Ori, "Full", iEquName, Constants.el_ExecFilePath);//


            //    modOPCommand.EquSeg3(X0, Y0, Math.Abs(incre_x * (T - 1)), Ori, Constants.el_ExecFilePath);//Line Closer
            //    modOPCommand.EquSeg4(X0, Y0, X0 + (Math.Abs(incre_x) * (T - 1)), Y0, iEquName, Ori, Constants.el_ExecFilePath);
            //    modOPCommand.EquSeg5(X0 + incre_x * (T - 1), Y0, Ori, "Full", iEquName, Constants.el_ExecFilePath, IsFullConnector);
            //    modOPCommand.EquSegPartNumber(X0, Y0 - 10, iEquName, iPartNumber, Constants.el_ExecFilePath,assoPNs);

            //    Y0 = Y0 - 60;
            //}

            //#endregion
            //#region//Bottom Connecter
            //if (!string.IsNullOrEmpty((string)f.grdOriInfo[1, 3].Value))
            //{
            //    arrPins = f.grdOriInfo[1, 3].Value.ToString().Split(",");
            //    T = arrPins.Length;
            //    Ori = "B";
            //    int a = 0;
            //    if (T - 1 < Convert.ToInt16(iNumberOfPins))
            //    {
            //        IsFullConnector = "no";
            //    }
            //    else if (T == Convert.ToInt16(iNumberOfPins))
            //    {
            //        IsFullConnector = "yes";
            //    }
            //    //X0 = 100;
            //    //Y0 = 102;
            //    for (int I11 = 1; I11 <= T-1; I11++)
            //    {
            //        modOPCommand.EquSeg2(X0 + a, Y0, arrPins[I11], Ori, Constants.el_ExecFilePath);//Adding Pins
            //        //using (writer = File.AppendText(Constants.el_ExecFilePath))
            //        //{
            //        //    //writer.WriteLine($"Add N254 'EQU' :F1.0 :R90 :AC I0 {X0}, {Y0} :T4320 {X0}, {Y0};NOP;");
            //        //}
            //        //modOPCommand.EquSeg2(X0 + a + incre_x * (I - 1), Y0, arrPins[I11], Ori, Constants.el_ExecFilePath);//Adding Pins
            //        //X0 = X0 + 4;
            //        a = 4;
            //        //modOPCommand.EquSeg2(X0 + incre_x * (I - 1), Y0, arrPins[I11], Ori, Constants.el_ExecFilePath);//Adding Pins
            //    }//3 line

            //    // Reordered EquSeg1 as EQU was not getting mapped properly to pin in Top and Bottom
            //    modOPCommand.EquSeg1(X0, Y0, Ori, "Full", iEquName, Constants.el_ExecFilePath);// 2 lines
            //    modOPCommand.EquSeg3(X0, Y0, Math.Abs(incre_x * (T - 1)), Ori, Constants.el_ExecFilePath);//2 lines
            //    modOPCommand.EquSeg4(X0, Y0, X0 + (Math.Abs(incre_x) * (T - 1)), Y0, iEquName, Ori, Constants.el_ExecFilePath);//2 lines
            //    modOPCommand.EquSeg5(X0, Y0, Ori, "Full", iEquName, Constants.el_ExecFilePath, IsFullConnector);//1 line
            //    //modOPCommand.EquSeg5(X0 + incre_x * (T - 1), Y0, Ori, "Full", iEquName, Constants.el_ExecFilePath);//1 line
            //    modOPCommand.EquSegPartNumber(X0, Y0, iEquName, iPartNumber, Constants.el_ExecFilePath, assoPNs);//2 lines

            //    //// Reordered EquSeg1 as EQU was not getting mapped properly to pin in Top and Bottom
            //    //modOPCommand.EquSeg1(X0, Y0, Ori, "Full", iEquName, Constants.el_ExecFilePath);// 1 line
            //    //modOPCommand.EquSeg3(X0, Y0 - 7.5, Math.Abs(incre_x * (T - 1)), Ori, Constants.el_ExecFilePath);//2 lines
            //    //modOPCommand.EquSeg4(X0, Y0, X0 + (Math.Abs(incre_x) * (T - 1)), Y0, iEquName, Ori, Constants.el_ExecFilePath);//2 lines
            //    //modOPCommand.EquSeg5(X0 + incre_x * (T - 1), Y0, Ori, "Full", iEquName, Constants.el_ExecFilePath);//1 line
            //    //modOPCommand.EquSegPartNumber(X0, Y0 - 10, iEquName, iPartNumber, Constants.el_ExecFilePath);//2 lines
            //    //Y0 = Y0 - 60;
            //}
            #endregion
        }


        public static void EquSeg1(double X0, double Y0, string iOrientation, string iCRepSymbol, string iEquName, string filePath)
        {
            using (var writer = File.AppendText(filePath))
            {
                string Symb = string.Empty;
                if (iCRepSymbol == "Full")
                {
                    if (iEquName.ToUpper().Contains("_F"))
                    {
                        Symb = "cont_sth_1_new_fixed_t";
                    }
                    else
                    {
                        Symb = "cont_sth_1_new_t";
                    }
                }
                else if (iCRepSymbol == "BOTop")
                {
                    Symb = "cont_sth_1_new_b";
                }

                if (iOrientation == "R")
                {
                    writer.WriteLine($"ADD I1 {Symb} :MY {X0},{Y0};;NOP;");
                }
                else if (iOrientation == "L")
                {
                    writer.WriteLine($"ADD I1 {Symb} {X0},{Y0};;NOP;");
                }
                //else if (iOrientation == "B")
                //{
                //    writer.WriteLine($"ADD I1 {Symb} :R90 {X0},{Y0};;NOP;");
                //}
                //else if (iOrientation == "T")
                //{
                //    writer.WriteLine($"ADD I1 {Symb} :R90 {X0},{Y0 + 7.5};;NOP;");
                //}
            }
        }

        public static void EquSeg2(double X0, double Y0, string N, string iOrientation, string filePath)
        {
            string Symb;
            double PinNumber_x, PinNumber_y;
            int Rotation;

            if (iOrientation == "R")
            {
                Symb = "contact_sth_ml";
                PinNumber_x = X0;//+ 5;
                PinNumber_y = Y0 - 1;
                Rotation = 0;
            }
            else if (iOrientation == "L")
            {
                Symb = "contact_sth_mr";
                PinNumber_x = X0 - 3;
                PinNumber_y = Y0 - 2;
                Rotation = 0;
            }
            //else if (iOrientation == "B")
            //{
            //    Symb = "contact_sth_mr";
            //    PinNumber_x = X0;
            //    PinNumber_y = Y0;
            //    Rotation = 90;
            //}
            //else if (iOrientation == "T")
            //{
            //    Symb = "contact_sth_ml";
            //    PinNumber_x = X0 - 1;
            //    PinNumber_y = Y0 + 5;
            //    Rotation = 90;
            //}
            else
            {
                Symb = "contact_sth_ml";
                PinNumber_x = X0;
                PinNumber_y = Y0;
                Rotation = 0;
            }


            using (var writer = File.AppendText(filePath))
            {
                writer.WriteLine($"ADD {Symb} :R{Rotation} {X0}, {Y0}; ");
                writer.WriteLine($"MOD N51 {PinNumber_x}, {PinNumber_y} 0,0 :E'{N}';");
                writer.WriteLine($"Add N254 'EQU' :F1.0 :R{Rotation} :AC I0 {PinNumber_x}, {PinNumber_y} :T4320 {PinNumber_x}, {PinNumber_y};NOP;");
            }
        }

        public static void EquSeg3(double X0, double Y0, double iLength, string iOrientation, string filePath)
        {
            double w = 7.5;

            using (var writer = File.AppendText(filePath))
            {
                writer.WriteLine($"GRID 0.5,2;");
                if (iOrientation == "R")
                {
                    writer.WriteLine($"ADD L214 {X0 + w * 2},{Y0} {X0 + w * 2},{Y0 + iLength}; ; ; ; NOP;");
                    writer.WriteLine($"ADD L214 {X0 + w},{Y0} {X0 + w},{Y0 + iLength}; ; ; ; NOP;");
                }
                else if (iOrientation == "L")
                {
                    writer.WriteLine($"ADD L214 {X0},{Y0} {X0},{Y0 + iLength}; ; ; ; NOP;");
                    writer.WriteLine($"ADD L214 {X0 + w},{Y0} {X0 + w},{Y0 + iLength}; ; ; ; NOP;");
                }
                //else if (iOrientation == "B")
                //{
                //    writer.WriteLine($"ADD L214 {X0},{Y0} {X0 + 8},{Y0}; ; ; ; NOP;");
                //    writer.WriteLine($"ADD L214 {X0},{Y0 - w} {X0 + 8},{Y0 - w}; ; ; ; NOP;");
                //}
                //else if (iOrientation == "T")
                //{
                //    writer.WriteLine($"ADD L214 {X0},{Y0} {X0 + iLength},{Y0}; ; ; ; NOP;");
                //    writer.WriteLine($"ADD L214 {X0},{Y0 + w} {X0 + iLength},{Y0 + w}; ; ; ; NOP;");
                //}
                writer.WriteLine($"GRID 0.5,2;");
            }
        }

        public static void EquSeg4(double LL_x, double LL_y, double UR_x, double UR_y, string iEquName, string iOrientation, string filePath)
        {
            double w = 6;   // Width
            double O = 4;   // Offset


            using (var writer = File.AppendText(filePath))
            {
                if (iOrientation == "R")
                {
                    writer.WriteLine($"ADD R254 {LL_x - w},{LL_y - O} {UR_x + 2},{UR_y + O * 2};;;NOP;");
                    writer.WriteLine($"ADD N53 '{iEquName}' :T1001 :F3.0 :D :AC R254 {LL_x - w},{LL_y + 1} {LL_x - 5},{UR_y + O * 2 + 1};;NOP;");
                }
                else if (iOrientation == "L")
                {
                    writer.WriteLine($"ADD R254 {LL_x - 2.5},{LL_y - O} {UR_x + w},{UR_y + O * 2};;;NOP;");
                    writer.WriteLine($"ADD N53 '{iEquName}' :T1001 :F3.0 :D :AC R254 {LL_x},{LL_y + 1} {LL_x + 1},{UR_y + O * 2 + 1};;NOP;");
                }
                //else if (iOrientation == "B")
                //{
                //    writer.WriteLine($"ADD R254 {LL_x - 8},{LL_y - 2} {LL_x + 16},{LL_y + 4};;;NOP;");
                //    writer.WriteLine($"ADD N53 '{iEquName}' :T1001 :F3.0 :R0 :D :AC R254 {LL_x - 8},{LL_y - 2} {LL_x - 1},{LL_y + 6};;NOP;");
                //}
                //else if (iOrientation == "T")
                //{
                //    writer.WriteLine($"ADD R254 {LL_x - 2 * O},{LL_y - w} {UR_x + 2 * O},{UR_y};;;NOP;");
                //    writer.WriteLine($"ADD N53 '{iEquName}' :T1001 :F3.0 :R0 :D :AC R254 {LL_x - 2 * O},{LL_y - w} {LL_x - 1},{UR_y + 2 * O + 4};;NOP;");
                //}
            }
        }

        public static void EquSeg5(double X0, double Y0, string iOrientation, string iCRepSymbol, string iEquName, string filePath, string Isfull)
        {
            string Symb = "";

            if (iCRepSymbol == "BOBot")
            {
                Symb = "cont_sth_last_new_t";
            }
            else if (iCRepSymbol == "Full")
            {
                if (iEquName.ToUpper().Contains("_F"))
                {
                    Symb = "cont_sth_last_new_fixed_b";
                }
                else
                {
                    if (Isfull.Equals("no"))
                    {

                        Symb = "cont_sth_last_new_t";//partial Symbol for footer
                    }
                    else
                    {
                        //Symb = "cont_sth_last_new_b"; //Orig
                        Symb = "cont_sth_last_new_b";//Full Symbol for footer
                    }
                }
            }

            using (var writer = File.AppendText(filePath))
            {
                if (iOrientation == "R")
                {
                    writer.WriteLine($"ADD I1 {Symb} :MY {X0},{Y0 + 2};;NOP;");
                }
                else if (iOrientation == "L")
                {
                    writer.WriteLine($"ADD I1 {Symb} {X0},{Y0 + 2};;NOP;");
                }
                //else if (iOrientation == "B")
                //{
                //    writer.WriteLine($"ADD I1 {Symb} :R90 {X0 + 8},{Y0};;NOP;");
                //}
                //else if (iOrientation == "T")
                //{
                //    writer.WriteLine($"ADD I1 {Symb} :R90 {X0},{Y0 + 7.5};;NOP;");
                //}
            }
        }

        public static void EquSegPartNumber(double iX0, double iY0, string iCompName, string iPartNumber, string filePath, string assoPNs)
        {
            using (var writer = File.AppendText(filePath))
            {
                writer.WriteLine($"ADD I1 info_sth_cmd {iX0},{iY0 + 4 + 4}; ; NOP;");
                writer.WriteLine($"MOD N2 {iX0},{iY0 + 4 + 4 - 2} {iX0},{iY0 + 4 + 4 - 2} :L253 :E'{iCompName}'; ; NOP;");
                writer.WriteLine($"MOD N52 {iX0},{iY0 + 4 + 4 - 4} {iX0},{iY0 + 4 + 4 - 4} :E'{iPartNumber}'; ; NOP;");
                if (!string.IsNullOrEmpty(assoPNs))
                {
                    int decre = 0;
                    string[] arrassopns = assoPNs.Split(";");
                    for (int assopns = 0; assopns <= arrassopns.Length - 1; assopns++)
                    {
                        string associatePartNumber = arrassopns[assopns];
                        if (associatePartNumber != iPartNumber)
                        {
                            decre = decre + 2;
                            //info_sth_cmd_HAL
                            //writer.WriteLine($"ADD I1 info_sth_cmd_HAL {iX0},{iY0 + 4 + 4 - 1 - decre}; ; NOP;");
                            writer.WriteLine($"ADD I1 info_sth_cmd {iX0},{iY0 + 4 + 4 - 1 - decre}; ; NOP;");
                            writer.WriteLine($"MOD N52 {iX0},{iY0 + 4 + 4 - 1 - decre} {iX0 - 1},{iY0 + 4 + 4 - 1 - decre} :E'{arrassopns[assopns]}'; ; NOP;");
                        }
                    }
                }
            }
        }

        public static void SegEquipmentBox(double LL_x, double LL_y, double UR_x, double UR_y, string iEquName, string iOrientation, string filePath)
        {
            double w = 6;   // Width
            double O = 4;   // Offset

            using (var writer = File.AppendText(filePath))
            {
                if (iOrientation == "R")
                {
                    writer.WriteLine($"ADD R252 {LL_x - w},{LL_y - O} {UR_x + 2},{UR_y + O * 2};;;NOP;");
                    writer.WriteLine($"ADD N202 '{iEquName}' :T1001 :F3.0 :D :AC R252 {LL_x - w},{LL_y - O} {LL_x - 5},{UR_y + O * 2};;NOP;");
                }
                else if (iOrientation == "L")
                {
                    writer.WriteLine($"ADD R252 {LL_x - 2.5},{LL_y - O} {UR_x + w},{UR_y + O * 2};;;NOP;");
                    writer.WriteLine($"ADD N202 '{iEquName}' :T1001 :F3.0 :D :AC R252 {LL_x},{LL_y - O} {LL_x + 1},{UR_y + O * 2};;NOP;");
                }
                //else if (iOrientation == "B")
                //{
                //    writer.WriteLine($"ADD R252 {LL_x - 8},{LL_y - 2} {LL_x + 16},{LL_y + 4};;;NOP;");
                //    writer.WriteLine($"ADD N202 '{iEquName}' :T1001 :F3.0 :R0 :D :AC R252 {LL_x - 8},{LL_y - 2} {LL_x - 1},{LL_y + 6};;NOP;");
                //}
                //else if (iOrientation == "T")
                //{
                //    writer.WriteLine($"ADD R252 {LL_x - 2 * O},{LL_y - w} {UR_x + 2 * O},{UR_y};;;NOP;");
                //    writer.WriteLine($"ADD N202 '{iEquName}' :T1001 :F3.0 :R0 :D :AC R252 {LL_x - 2 * O},{LL_y - w} {LL_x - 1},{UR_y + 2 * O + 4};;NOP;");
                //}
            }
        }
    }
}

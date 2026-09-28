using Microsoft.VisualBasic.Logging;
using PanelDrawing.Core.Constants;
using PanelDrawing.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace PanelDrawing.Core.Utilities
{
    public class CommandProcessor
    {
        public static void NewSheetWithTB(string el_Execfilpath, string pnlnum, string strnum, string SheetTemplateName)
        {
            using (var writer = File.AppendText(el_Execfilpath))
            {
                writer.WriteLine("EDI " + SheetTemplateName + "; SAV (CHR(34)+" + PanelConstants.quotationMark + "" + PanelConstants.Elec_Proj_Schem_Folder_Path + "\\" + pnlnum + "" + PanelConstants.quotationMark + "+CHR(34));");
                writer.WriteLine($"EDI (CHR(34)+{PanelConstants.quotationMark}{pnlnum}{PanelConstants.quotationMark}+CHR(34));");
                writer.WriteLine($"MOD_TAG 2012 '{pnlnum}';");
                writer.WriteLine($"MOD_TAG 2011 '{strnum}';");
                writer.WriteLine($"MOD_TAG 2013 '{pnlnum}';");

                writer.WriteLine("PRT_NAM;");
                writer.WriteLine("GRID 0.5,2;");
                writer.WriteLine("REMOVE :A;");
            }
        }

        public static void AddSymbol(string iSymbolname, string iConnectorNameForComment, double X0, double Y0, string iPartNumber, string el_Execfilpath)  //'Symbolname is Macroname
        {
            //using (StreamWriter writer = File.AppendText(el_Execfilpath))
            using (var writer = File.AppendText(el_Execfilpath))
            {
                writer.WriteLine("ADD I2 " + iSymbolname + " " + X0 + "," + Y0 + ";NOP;");
                if (iSymbolname.StartsWith("macro"))
                {
                    writer.WriteLine("SMA I2 " + iSymbolname + " " + X0 + "," + Y0 + ";");
                }
            }
        }

        public static void AddSymbolSPL_OOTB(string refName, double X0, double Y0, string iPartNumber, string el_Execfilpath)  //'Symbolname is Macroname
        {
            //using (StreamWriter writer = File.AppendText(el_Execfilpath))
            using (var writer = File.AppendText(el_Execfilpath))
            {
                //writer.WriteLine($"ADD I0 ");
                //writer.WriteLine($"ADD I0 sth_splice ");
                writer.WriteLine($"ADD I0 sth_splice {X0},{Y0};");
                writer.WriteLine($"MOD N53 {X0},{Y0 + 4} 0,0 :E '{refName}';NOP;");
                writer.WriteLine($"MOD N254 {X0 + 2.5},{Y0 - 5} 0,0 :E '{iPartNumber}';NOP;");
            }
        }

        public static void AddSymbolGND(string refName, double X0, double Y0, string iPartNumber, string el_Execfilpath)  //'Symbolname is Macroname
        {
            //using (StreamWriter writer = File.AppendText(el_Execfilpath))
            using (var writer = File.AppendText(el_Execfilpath))
            {
                //writer.WriteLine($"ADD I0 ");
                //writer.WriteLine($"ADD I0 sth_ground_1 ");
                writer.WriteLine($"ADD I0 sth_ground_1 {X0},{Y0};");
                writer.WriteLine($"MOD N53 {X0},{Y0 + 4} 0,0 :E '{refName}';NOP;");
            }
        }

        public static void AddSymbolREL_OOTB(string refName, double X0, double Y0, string iPartNumber, string el_Execfilpath, List<string> Pins)  //'Symbolname is Macroname
        {
            int stepCount = (Pins.Count-2)/3;
            //using (StreamWriter writer = File.AppendText(el_Execfilpath))
            using (var writer = File.AppendText(el_Execfilpath))
            {
                writer.WriteLine($"ADD I0 sth_relay_first_t {X0},{Y0};;;NOP;;"); //202,260
                double tempY0 = Y0 - 2; //254
                Char[] chars = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L' };
                for (int i = 0; i < stepCount; i++) //count = 3
                {
                    tempY0 = tempY0 - 4; //250 // 246

                    writer.WriteLine($"ADD I0 cont_sth_relay_down_t {X0},{tempY0};;;NOP;;"); // 202,254
                    writer.WriteLine($"ADD L212 {X0},{tempY0 - 1.5} {X0},{tempY0 - 3};;;NOP;;;");
                    writer.WriteLine($"MOD N51 {X0 - 9},{tempY0 - 1} {X0 - 9},{tempY0 - 1} :E '{chars[i]}2' {X0 - 9},{tempY0 - 1} ;NOP;;;");
                    writer.WriteLine($"MOD N51 {X0 + 5.5},{tempY0 - 1} {X0 + 5.5},{tempY0 - 1} :E '{chars[i]}1' {X0 + 5.5},{tempY0 - 1} ;NOP;;;");
                    writer.WriteLine($"MOD N51 {X0 + 5.5},{tempY0 + 1.5} {X0 + 5.5},{tempY0 + 1.5} :E '{chars[i]}3' {X0 + 5.5},{tempY0 + 1.5} ;NOP;;;");
                    writer.WriteLine($"MOD N253 {X0 + 10},{tempY0 - 1.5} {X0 + 10},{tempY0 - 1.5} :E '{refName}'  ;NOP;;;");
                                       
                }

                writer.WriteLine($"ADD I0 cont_sth_relay_coil_t {X0},{tempY0 - 6};;;NOP;;");
                writer.WriteLine($"MOD N253 {X0 + 10},{tempY0 - 7.5} {X0 + 10},{tempY0 - 7.5} :E '{refName}'  ;NOP;;;");

                writer.WriteLine($"ADD I0 sth_relay_last_new_t {X0},{tempY0 - 10};;;NOP;;");
                writer.WriteLine($":RAW");
                writer.WriteLine($"ADD L214 {X0 - 10},{Y0} {X0 - 10},{tempY0 - 10};;;NOP;;");
                writer.WriteLine($"ADD L214 {X0 + 10},{Y0} {X0 + 10},{tempY0 - 10};;;NOP;;");

                writer.WriteLine($":GRI");
                writer.WriteLine($"ADD I0 info_sth_relay_base {X0 + 6},{tempY0 - 16};");
                writer.WriteLine($"MOD N53 {X0 + 6},{tempY0 - 16} {X0 + 6},{tempY0 - 16} :E '{refName}' {X0 + 6},{tempY0 - 16} ;");
                writer.WriteLine($"MOD N52 {X0 + 6.5},{tempY0 - 16} {X0 + 6.5},{tempY0 - 16} :E '{iPartNumber}' {X0 + 6.5},{tempY0 - 16} ;");

                writer.WriteLine($"ADD I0 info_sth_relay {X0 - 10},{tempY0 - 16}; ");
                writer.WriteLine($"MOD N53 {X0 - 10},{tempY0 - 16} {X0 - 10},{tempY0 - 16} STOR_MID :E 'RR' {X0 - 10},{tempY0 - 16} JU ;");
                writer.WriteLine($"MOD N10 {X0 - 10},{tempY0 - 16} {X0 - 10},{tempY0 - 16} STOR_MID :E 'D' {X0 - 10},{tempY0 - 16} JU");
            }
        } 

        public static void AddOOTB_TER_SymbolForTerminal(string iConnectorNameForComment, double X0, double Y0, string iPartNumber, string el_Execfilpath,string IROT, List<string> arrPin,string temp_TERTBK_Shunt_info)  //'Symbolname is Macroname
        {
            int incre = 0;
            //using (StreamWriter writer = File.AppendText(el_Execfilpath))
            using (var writer = File.AppendText(el_Execfilpath))
            {
                //writer.WriteLine($"ADD I1 sth_tb_head_t :R{IROT} {X0},{Y0};");//header
                for (int d = 0; d <= arrPin.Count - 1; d++)
                {
                    writer.WriteLine($"ADD I0 sth_tb_body2 :R{IROT} {X0},{Y0 - incre};");
                    writer.WriteLine($"MOD N51 {X0},{Y0 - incre} 0,0 :E'{arrPin[d]}';");

                    #region//Add shunt values to the terminal pins if its exist
                    if (!string.IsNullOrEmpty(temp_TERTBK_Shunt_info))
                    {
                        //if(Constants.strDEBelowTERTBK_Shunt_List)
                        var arr_temp_TERTBK_Shunt_dup = temp_TERTBK_Shunt_info.Split(";");
                        var arr_temp_TERTBK_Shunt = arr_temp_TERTBK_Shunt_dup.Distinct().ToArray();
                        for (int shuntpincount = 0; shuntpincount <= arr_temp_TERTBK_Shunt.Length - 1; shuntpincount++)
                        {
                            var tempItem = arr_temp_TERTBK_Shunt[shuntpincount].Split(',');
                            if (tempItem[1].Equals(arrPin[d]))
                            {
                                //var tempPininfo = 
                                if (tempItem[2].Equals("L")) writer.WriteLine($"Add N254 '{tempItem[4]}' :F1.0 :R0 :AC I0 {X0 - 10},{Y0 - incre} :T4328 {X0 - 10},{Y0 - incre};NOP;");//left pin
                                if (tempItem[2].Equals("R")) writer.WriteLine($"Add N254 '{tempItem[4]}' :F1.0 :R0 :AC I0 {X0 + 7},{Y0 - incre} :T4328 {X0 + 7},{Y0 - incre};NOP;");//right pin
                            }
                        }
                    }
                    #endregion
                    incre = incre + 4;
                }
                
                writer.WriteLine($"ADD I1 sth_tb_foot_new_t :R{IROT} {X0},{Y0 - arrPin.Count * 4};");//footer
                writer.WriteLine($"ADD L101 {X0-6},{Y0} {X0-6},{Y0 - arrPin.Count * 4};;;;NOP;");
                writer.WriteLine($"ADD L101 {X0 + 6},{Y0} {X0 + 6},{Y0 - arrPin.Count * 4};;;;NOP;");
                writer.WriteLine($"ADD R254  {X0 - 6},{Y0 - 12 - (arrPin.Count) * 4} {X0 + 6},{Y0 + 6};;;NOP;");
                writer.WriteLine($"ADD N53 :T1001 :F4.0 :R0 :D :J4 :AC R254 {X0},{Y0 + 6} '{iConnectorNameForComment}' {X0},{Y0 + 6+2};");
                writer.WriteLine($"ADD info_sth_cmd {X0-4},{Y0 -11 -3 - arrPin.Count * 4};");
                writer.WriteLine($"MOD N2 {X0-4},{Y0 - 13 -4 - arrPin.Count * 4} 0,0 STOR_MID :F1.0:L253 :E'{iConnectorNameForComment}' JU ;");
                writer.WriteLine($"MOD N52 {X0-4},{Y0 - 15 -4 - arrPin.Count * 4} 0,0 STOR_MID :E'{iPartNumber}' JU ;");
                writer.WriteLine($"TESTDIS_OFF;;");
                writer.WriteLine($"pm_files_sav;");
                writer.WriteLine($";;NOP;");
            }
        }

        public static LibraryCatalog? FindCatalogByPart(string partNumber)
        {
            var res = PanelConstants.libCatalogList
                .FirstOrDefault(x =>
                    (x.RefInternal ?? "").Contains(partNumber) ||
                    (x.MandatoryAccessory1 ?? "").Contains(partNumber) //||
                   // (x.MandatoryAccessory2 ?? "").Contains(partNumber)
                );
            return res;
        }

        public static string GetPrimaryPartNumber(List<string> assocParts)
        {
            foreach (var pn in assocParts)
            {
                var catalog = FindCatalogByPart(pn);

                if (catalog != null && string.IsNullOrEmpty(catalog.MandatoryAccessory1))
                    return pn;
            }

            return assocParts.FirstOrDefault() ?? "";
        }    

        public static void AddSymbolAttributes(double iX0, double iY0, string refname, string loc, string PartNumber, string filePath, string comptype)
        {
            using (var writer = File.AppendText(filePath))
            {
                writer.WriteLine($"MOD N53 {iX0},{iY0 + 2} 0,0 :E '{refname}';NOP;");//Red Colour Attribute
                writer.WriteLine($"MOD N202 {iX0},{iY0 - 4} 0,0 :E '{loc}';NOP;");//Pink Colour Attribute
                writer.WriteLine($"MOD N52 {iX0},{iY0+5} 0,0 :E '{PartNumber}';NOP;");
                writer.WriteLine($"SHOW #E;");
            }
        }

        public static void AddCBSymbolAttributes(double iX0, double iY0,string refname,string loc,string PartNumber,string volt,string filePath,string comptype)
        {
            using (var writer = File.AppendText(filePath))
            {
                #region commented
                //writer.WriteLine($"ADD N53 '{refname}' {iX0},{iY0} :F1.0 :T1001 :D;;NOP;");
                //writer.WriteLine($"ADD N202 '{loc}' {iX0},{iY0 - 4} :F1.0 :T2002 :D;;NOP;");
                //writer.WriteLine($"ADD N52 '{PartNumber}' {iX0},{iY0 - 4 - 4} :F1.0 :T1004 :D;;NOP;");
                //writer.WriteLine($"ADD N254 '{volt}' {iX0},{iY0 - 4 - 4-4} :F1.0 :T4343 :D;;NOP;");
                //writer.WriteLine($"ADD N55 '{NoMeggerOrYesMegger}' {iX0},{iY0 - 4 - 4-4-4} :F1.0 :T5001 :D;;NOP;");

                //writer.WriteLine($"MOD N53 '{refname}' {iX0},{iY0} :F1.0 :T1001 :D;;NOP;");
                //writer.WriteLine($"MOD N202 '{loc}' {iX0},{iY0 - 4} :F1.0 :T2002 :D;;NOP;");
                //writer.WriteLine($"MOD N52 '{PartNumber}' {iX0},{iY0 - 4 - 4} :F1.0 :T1004 :D;;NOP;");
                //writer.WriteLine($"MOD N254 '{volt}' {iX0},{iY0 - 4 - 4 - 4} :F1.0 :T4343 :D;;NOP;");
                //writer.WriteLine($"MOD N55 '{NoMeggerOrYesMegger}' {iX0},{iY0 - 4 - 4 - 4 - 4} :F1.0 :T5001 :D;;NOP;");

                //writer.WriteLine($"MOD N53 {iX0},{iY0} 0,0 :E '{refname};'NOP;");
                //writer.WriteLine($"MOD N202 {iX0},{iY0 - 4} 0,0 :E '{loc};'NOP;");
                //writer.WriteLine($"MOD N52 {iX0},{iY0 - 4 - 4} 0,0 :E '{PartNumber};'NOP;");
                //writer.WriteLine($"MOD N254 {iX0},{iY0 - 4 - 4 - 4} 0,0 :E '{volt};'NOP;");
                //writer.WriteLine($"MOD N55 {iX0},{iY0 - 4 - 4 - 4 - 4} 0,0 :E '{NoMeggerOrYesMegger}';NOP;");
                #endregion

                writer.WriteLine($"MOD N53 {iX0},{iY0+2} 0,0 :E '{refname}';NOP;");
                writer.WriteLine($"MOD N202 {iX0},{iY0 - 4} 0,0 :E '{loc}';NOP;");
                writer.WriteLine($"MOD N52 {iX0},{iY0 - 4 - 4} 0,0 :E '{PartNumber}';NOP;");
                if (!comptype.Equals("SCB"))
                {
                    writer.WriteLine($"MOD N254 {iX0},{iY0 - 4 - 4 - 4} 0,0 :E '{volt}';NOP;");
                   // writer.WriteLine($"MOD N55 {iX0},{iY0 - 4 - 4 - 4 - 4} 0,0 :E '{NoMeggerOrYesMegger}';NOP;");
                }

                writer.WriteLine($"SHOW #E;");
            }
        }
    }
}


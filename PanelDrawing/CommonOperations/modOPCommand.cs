using Microsoft.VisualBasic.Logging;
using PanelDrawing.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace PanelDrawing.CommonOperations
{
    public class modOPCommand
    {
        //'Puting into seperate Panel Project
        public static void NewSheetWithTB(string el_Execfilpath, string pnlnum, string strnum, string SheetTemplateName)
        {
            //FileStream fileStream = new FileStream(el_ExecfilePath, FileMode.OpenOrCreate, FileAccess.Write);
            using (var writer = File.AppendText(el_Execfilpath))
            {
                //// Save the new drawing using the specified project path
                //EDI mytemp_new_a4; SAV(CHR(34) + 'C:\ELECTRE\electre_projects\PANEL_DR03\Schem\CCCCC_12345' + CHR(34));
                writer.WriteLine("EDI " + SheetTemplateName + "; SAV (CHR(34)+" + Constants.quotationMark + "" + Constants.Elec_Proj_Schem_Folder_Path + "\\" + pnlnum + "" + Constants.quotationMark + "+CHR(34));");
                //Open the new drawing
               // writer.WriteLine("NEW_OPEN_DRAWING " + Constants.quotationMark + pnlnum + Constants.quotationMark + ";"); //'Open the new drawing
                writer.WriteLine($"EDI {pnlnum}");
                writer.WriteLine("MOD_TAG 2012 '" + pnlnum + "'; ");// 'Modify the titleblock attributes
                writer.WriteLine("MOD_TAG 2011 '" + strnum + "'; ");
                writer.WriteLine("MOD_TAG 2013 '" + pnlnum + "'; ");
                writer.WriteLine("PRT_NAM; ");
                writer.WriteLine("GRID 0.5,2;");
                writer.WriteLine("REMOVE :A; ");// 'Shut the template to avoid sharing issue
            }
            //FileStream fileStream = new FileStream(el_ExecfilePath,File.);
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

        public static void AddSymbolSPL(string refName, double X0, double Y0, string iPartNumber, string el_Execfilpath)  //'Symbolname is Macroname
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

        public static List<PanelComponentProperties> GatherPanelComponentProperties1(string panelName)
        {
            // Clear previous results
            Constants.panelComponentProperties.Clear();

            // STEP 1: Get all components belonging to panel & having part number
            var panelComponents =
                Constants.dataExtractionListAbove
                .Where(x => x.Panel == panelName &&
                            Constants.listComponentsWithPartNumber.Contains(x.ConnectorName))
                .Select(x => x.ConnectorName)
                .Distinct()
                .ToList();

            foreach (var compName in panelComponents)
            {
                // STEP 2: Get all DEAbove rows for this component
                var compDEAbove = Constants.dataExtractionListAbove
                    .Where(x => x.ConnectorName == compName && x.Panel == panelName)
                    .ToList();

                // STEP 3: Get associated part numbers
                var associatedPNs = compDEAbove
                    .Select(x => x.CoreNumber)     // CoreNumber = PartNumber
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Distinct()
                    .ToList();

                // STEP 4: Primary part number
                string primaryPN = modOPCommand.GetPrimaryPartNumber(associatedPNs);

                // STEP 5: Get extra properties DEBelow
                var firstDEBelow = Constants.dataExtractionListBelow
                    .FirstOrDefault(x => x.ConnectorName == compName &&
                                         x.Panel == panelName);

                string compType = firstDEBelow?.ComponentType ?? "";
                string samplePin = firstDEBelow?.PinNumber ?? "";
                string cbTypeName = firstDEBelow?.Tag4 ?? "";
                string cbVoltage = firstDEBelow?.Tag5 ?? "";

                // STEP 6: Shunts (for TBK/TER)
                string shuntList = string.Join(";",
                    Constants.dataExtractionListBelow
                    .Where(x => x.ConnectorName == compName &&
                                !string.IsNullOrEmpty(x.Shunt))
                    .Select(x => $"{x.ConnectorName},{x.PinNumber},{x.FunctionalDesignation},{x.ComponentType},{x.Shunt}")
                );

                // STEP 7: Get info from Library Catalog
                var lib = Constants.libCatalogList
                    .FirstOrDefault(x =>
                        (x.RefInternal ?? "") == primaryPN ||
                        (x.MandatoryAccessory1 ?? "") == primaryPN ||
                        (x.MandatoryAccessory2 ?? "") == primaryPN
                    );

                string macroName = lib?.Symbol2D ?? "";
                string maxPins = lib?.MaxPins ?? "";
                string accessory = lib?.MandatoryAccessory1 ?? "";
                string acc2 = lib?.MandatoryAccessory2 ?? "";

                // STEP 8: Equipment Box & Looms
                string equipInfo = compDEAbove.FirstOrDefault()?.EquipmentName ?? "";
                string loomsInfo = compDEAbove.FirstOrDefault()?.BundleName ?? "";

                // STEP 9: Construct modern object
                var compProps = new PanelComponentProperties
                {
                    ComponentName = compName,
                    ComponentType = compType,
                    MacroName = macroName,
                    MaxPin = maxPins,
                    PartNumber = primaryPN,
                    Accessory = $"{accessory};{acc2}",
                    SamplePin = samplePin,
                    GroupId = firstDEBelow?.Group ?? "",
                    WireLength = firstDEBelow?.Length ?? "",
                    WireType = firstDEBelow?.CableType ?? "",
                    CBTypeName = cbTypeName,
                    CBVoltage = cbVoltage,
                    AssociatedPartNumbers = string.Join(";", associatedPNs),
                    EquipmentBox = equipInfo,
                    Looms = loomsInfo,
                    ShuntList = shuntList
                };

                Constants.panelComponentProperties.Add(compProps);
            }

            return Constants.panelComponentProperties;
        }

        public static void AddOOTB_TER_SymbolForTerminal(string iConnectorNameForComment, double X0, double Y0, string iPartNumber, string el_Execfilpath,string IROT, string[] arrPin,string temp_TERTBK_Shunt_info)  //'Symbolname is Macroname
        {
            int incre = 0;
            //using (StreamWriter writer = File.AppendText(el_Execfilpath))
            using (var writer = File.AppendText(el_Execfilpath))
            {
                //writer.WriteLine($"ADD I1 sth_tb_head_t :R{IROT} {X0},{Y0};");//header
                for (int d = 0; d <= arrPin.Length - 1; d++)
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
                
                writer.WriteLine($"ADD I1 sth_tb_foot_new_t :R{IROT} {X0},{Y0 - arrPin.Length * 4};");//footer
                writer.WriteLine($"ADD L101 {X0-6},{Y0} {X0-6},{Y0 - arrPin.Length * 4};;;;NOP;");
                writer.WriteLine($"ADD L101 {X0 + 6},{Y0} {X0 + 6},{Y0 - arrPin.Length * 4};;;;NOP;");
                writer.WriteLine($"ADD R254  {X0 - 6},{Y0 - 12 - (arrPin.Length) * 4} {X0 + 6},{Y0 + 6};;;NOP;");
                writer.WriteLine($"ADD N53 :T1001 :F4.0 :R0 :D :J4 :AC R254 {X0},{Y0 + 6} '{iConnectorNameForComment}' {X0},{Y0 + 6+2};");
                writer.WriteLine($"ADD info_sth_cmd {X0-4},{Y0 -11 -3 - arrPin.Length * 4};");
                writer.WriteLine($"MOD N2 {X0-4},{Y0 - 13 -4 - arrPin.Length * 4} 0,0 STOR_MID :F1.0:L253 :E'{iConnectorNameForComment}' JU ;");
                writer.WriteLine($"MOD N52 {X0-4},{Y0 - 15 -4 - arrPin.Length * 4} 0,0 STOR_MID :E'{iPartNumber}' JU ;");
                writer.WriteLine($"TESTDIS_OFF;;");
                writer.WriteLine($"pm_files_sav;");
                writer.WriteLine($";;NOP;");
            }
        }

        public static int RowOfFoundStringInColOfMDarray(string CompPN, string[,] Temp_arrTableOfLibCatalog, int colnum)
        {
            int rownumFound = 0;
            for (int rownum = 0; rownum <= Temp_arrTableOfLibCatalog.GetLength(0) - 1; rownum++)
            {
                //string strsub = CompPN.Substring(0).Trim();
                if (Temp_arrTableOfLibCatalog[rownum, colnum].Trim().Contains(CompPN.Trim()))
                {
                    rownumFound = rownum;
                    break;
                }
               /* else
                {
                    rownumFound = 0;
                }*/
            }
            return rownumFound;
        }

        //RowOfFoundStringInColOfMDarray
        public static LibraryCatalog FindCatalogByPart(string partNumber)
        {
            var res = Constants.libCatalogList
                .FirstOrDefault(x =>
                    (x.RefInternal ?? "").Contains(partNumber) ||
                    (x.MandatoryAccessory1 ?? "").Contains(partNumber) //||
                   // (x.MandatoryAccessory2 ?? "").Contains(partNumber)
                );
            return res;
        }


        #region //Commented old RowOfCountFoundStringInColOfMDarray code on 18 november, 2025 and written GetAssociatedPartNumbers
        //public static string RowOfCountFoundStringInColOfMDarray(string strSerch, string[,] Temp_arr, int colnum1, int colnum2,int colnum3)
        //{
        //    Constants.strAssosiatePNinfo = string.Empty; 
        //    string strPNs=string.Empty;
        //    //var gh = Temp_arr[1]
        //    for (int rownum = 0; rownum <= Temp_arr.GetLength(0) - 1; rownum++)
        //    {
        //        if (Temp_arr[rownum, colnum1].Trim().Equals(strSerch.Trim()) && !string.IsNullOrEmpty(Temp_arr[rownum, colnum2]) && Temp_arr[rownum, colnum3].Trim().Equals(Constants.textPanelPartName.Trim()))
        //        {
        //            strPNs = Temp_arr[rownum, colnum2].Trim();
        //            Constants.strAssosiatePNinfo = string.Concat(Constants.strAssosiatePNinfo,";", strPNs);
        //        }
        //    }
        //    return Constants.strAssosiatePNinfo.Substring(1, Constants.strAssosiatePNinfo.Length-1);
        //}
        #endregion

        public static List<string> GetAssociatedPartNumbers(string connector, string panel)
        {
            var res = Constants.dataExtractionListAbove
                .Where(x => x.ConnectorName == connector &&
                            x.Panel == panel &&
                            !string.IsNullOrEmpty(x.CoreNumber))  // CoreNumber = PartNumber
                .Select(x => x.CoreNumber)
                .Distinct()
                .ToList();

            return res;
        }

        //FindPartNumberFromAssosiatedPartnumbersIfDefined
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

        public static int RowOfFoundStringsInColOfMDarray(string str1,string str2,string str3, string[,] Temp_arr, int colnum1, int colnum2,int colnum3)
        {
            int rownumFound = 0;
            string[] arrtempWireCode = str3.Split('/');
            for (int rownum = 0; rownum <= Temp_arr.GetLength(0) - 1; rownum++)
            {
                //string strsub = CompPN.Substring(0).Trim();
                if (Temp_arr[rownum, colnum1].Trim().Equals(str1.Trim()) && Temp_arr[rownum, colnum3].Trim().Equals(arrtempWireCode[0].Trim()) && Temp_arr[rownum, colnum2].Trim().Equals(str2.Trim()))
                {
                    rownumFound = rownum;
                    break;
                }
               /* else
                {
                    rownumFound = 0;
                }*/
            }
            return rownumFound;
        }      

        public static void AddSWTSymbolAttributes(double iX0, double iY0,string PartNumber, string filePath,string refname)
        {
            using (var writer = File.AppendText(filePath))
            {
                writer.WriteLine($"MOD N53 {iX0 + 4},{iY0} 0,0 :E '{refname}';NOP;");
                writer.WriteLine($"MOD N2 {iX0 + 4},{iY0} 0,0 :E '{refname}';NOP;");
                writer.WriteLine($"MOD N52 {iX0+4},{iY0 - 1} 0,0 :E '{PartNumber}';NOP;");
            }
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

        public static void AddDISSymbolAttributes(double iX0, double iY0, string refname, string loc, string PartNumber, string filePath)
        {
            using (var writer = File.AppendText(filePath))
            {
                //writer.WriteLine($"MOD N53 {iX0},{iY0 + 2} 0,0 :E '{refname}';NOP;");
                //writer.WriteLine($"MOD N202 {iX0},{iY0 - 4} 0,0 :E '{loc}';NOP;");
                writer.WriteLine($"MOD N52 {iX0},{iY0 - 4 - 4} 0,0 :E '{PartNumber}';NOP;");
                writer.WriteLine($"SHOW #E;");
            }
        }

        public static void SegLooms(double iX0, double iY0, string iCompName, string iPartNumber, string filePath, string LoomRefName)
        {
            using (var writer = File.AppendText(filePath))
            {
                //writer.WriteLine($"ADD R203  76.5,206.5 92,229.5 ;");
            }
        }
        public static void CBSeg2(double X0, double Y0, string N, string filePath, int decreRight)
        {
            using (var writer = File.AppendText(filePath))
            {
                //MOD N51 115,255 0,0 :E '1111'; NOP;
                writer.WriteLine($"MOD N51 {X0},{Y0} 0,0 :E '{N}'; NOP;");
                #region
                //writer.WriteLine($"MOD N51 {X0 + 12},{Y0} 0,0 :E '{N}'; NOP;");

                //writer.WriteLine($"Add N51 'TCB' :F1.0 :R0 :AC I0 {X0}, {Y0} :T1003 {X0 - 2}, {Y0+4};NOP;");
                //writer.WriteLine($"Add N51 '{N}' :F1.0 :R0 :AC I0 {X0},{Y0} :T1003 {X0-2},{Y0+4};NOP;");//ORIG
                //writer.WriteLine($"MOD N51 {X0},{Y0} 0,0 :E '{N}'; NOP;");
                //writer.WriteLine($"ADD N51 '{N}' {X0},{Y0} :F1.0 :T4326 :D;;NOP;");
                //writer.WriteLine($"ADD N51 '{N}' {X0 + 12},{Y0} :F1.0 :T4326 :D;;NOP;");
                //writer.WriteLine($"MOD N51 '{N}' {X0},{Y0} :F1.0 :T4326 :D;;NOP;");
                //writer.WriteLine($"MOD N51 '{N}' {X0 + 12},{Y0} :F1.0 :T4326 :D;;NOP;");
                #endregion
            }
        }

        #region//DIS Connector************************

        #region //Commented old DisSeg1 code on 18 november, 2025
        //public static void DisSeg1(double X0, double Y0, string filePath,string strDISOrientation)
        //{
        //    using (writer = File.AppendText(filePath))
        //    {
        //        //writer.WriteLine("$$ DisSeg - Start...");
        //        //writer.WriteLine("LET IROT 0;");
        //        //writer.WriteLine($"ADD I1 cont_sth_1c_new_t :R (IROT)  :MY {X0},{Y0};");

        //        //writer.WriteLine("$$ DisSeg - Start...");
        //        //writer.WriteLine("LET IROT 0;");
        //        //macro_sth_cou_ci2
        //        //writer.WriteLine($"ADD I1 macro_sth_cou_ci2 :MY {X0},{Y0};");
        //        if (strDISOrientation.Equals("L"))
        //        {
        //            //cont_sth_mg_hal
        //            writer.WriteLine($"ADD I1 cont_sth_1c_new_t :MY {X0},{Y0};");
        //        }
        //        else 
        //        {
        //            writer.WriteLine($"ADD I1 cont_sth_1c_new_t {X0 + 20},{Y0};");
        //        }
        //    }
        //}
        #endregion

        #region //Commented old DisSeg2 code on 18 november, 2025
        //public static void DisSeg2(double X0, double Y0, string N, string filePath,string Ori)
        //{
        //    using (writer = File.AppendText(filePath))
        //    {
        //        //Cont_sth_md  - Left Ori,Cont_sth_mg - Right Ori rest of the thing recipticle
        //        //double lineLength = 30;
        //        //Left Side Pins
        //        if (Ori.Equals("L"))
        //        {
        //            //writer.WriteLine($"ADD Cont_sth_mg {X0},{Y0};");
        //            writer.WriteLine($"ADD Cont_sth_mg_hal {X0},{Y0};");//oRIG
        //            //writer.WriteLine($"ADD Cont_sth_md :MY {X0},{Y0};");//oRIG
        //            //writer.WriteLine($"Add N51 '{N}' :F1.0 :R0 :AC I0 {X0 + 0.8},{Y0} :T1003 {X0 + 0.8},{Y0};NOP;");//ORIG

        //            writer.WriteLine($"MOD N51 {X0 + 0.8},{Y0} 0,0 :E '{N}'; NOP;");
        //            writer.WriteLine($"Add N254 'DIS' :F1.0 :R0 :AC I0 {X0},{Y0} :T4320 {X0},{Y0 - 2};NOP;");
        //        }
        //        #region
        //        //writer.WriteLine($"SMA macro_sth_cou_ci2 {X0},{Y0};");
        //        //writer.WriteLine($"Add N254 'DIS' :F1.0 :R(IROT) :AC I0 {X0},{Y0} :T4320 {X0},{Y0 - 2};NOP;");
        //        //writer.WriteLine($"Add N254 'DIS' :F1.0 :R(IROT) :AC I0 {X0 + 10},{Y0} :T4320 {X0 + 7},{Y0 - 2};NOP;");
        //        #endregion
        //        if (Ori.Equals("R"))//Right Side Pins
        //        {
        //            //writer.WriteLine($"ADD contact_sth_receptacle {X0 + 10},{Y0};");//orig
        //            writer.WriteLine($"ADD contact_sth_receptacle {X0+10},{Y0};");
        //            //MOD N254 218.5,272 0,0 :E 'A'; NOP;
        //            //writer.WriteLine($"Add N254 '{N}' :F1.0 :R0 :AC I0 {X0 + 11 - 2.5},{Y0} :T1003 {X0 + 11 - 2.5},{Y0};NOP;");//orig
        //            writer.WriteLine($"MOD N254 {X0 + 11 - 2.5},{Y0} 0,0 :E '{N}'; NOP;");
        //            writer.WriteLine($"Add N254 'DIS' :F1.0 :R0 :AC I0 {X0 + 10},{Y0 - 2} :T4320 {X0 + 10},{Y0 - 2};NOP;");
        //        }
        //            #region
        //            //writer.WriteLine($"Add N254 'DIS' :F1.0 :R0 :AC I0 {X0 + 10},{Y0} :T4320 {X0 + 7},{Y0 - 2};NOP;");
        //            //// writer.WriteLine($"MOD N51 {X0 - 2-4},{Y0 - 1} {X0 - 2-4},{Y0 - 1} :E'{N}';NOP;");//orig
        //            ////writer.WriteLine($"MOD N254 {X0 + 10 + 6},{Y0} {X0 + 10 + 6},{Y0} :E'{N}';NOP;");//Orig



        //            //writer.WriteLine($"ADD macro_sth_cou_ci2 {X0},{Y0};");
        //            //writer.WriteLine($"SMA macro_sth_cou_ci2 {X0},{Y0};");
        //            ////writer.WriteLine($"Add R254 {X0+200},{Y0+200} {X0+200},{Y0 - 2+200}; ; ;NOP;");
        //            ////writer.WriteLine($"ADD N53 :T1001: F3.0 :R0: D: J7: AC R254 {X0},{Y0} '1002DEL_M' 189,252; NOP;");
        //            ////writer.WriteLine($"Add R254 {X0+10+200},{Y0+200} {X0+7+200},{Y0 - 2+200};");
        //            //////ADD R254 364,244 372,276; ; ; NOP;
        //            //////ADD N53 'C1' :T1001: F3.0 :D: AC R254 364,244 365,276; ; NOP;
        //            ////writer.WriteLine($"ADD N53 :T1001: F3.0 :R0: D: AC R254 200,194 '1002DEL_F' 209,252;");

        //            //writer.WriteLine($"Add R254 'DIS' :F1.0 :R0 :AC I0 {X0 + 10},{Y0} :T4320 {X0 + 7},{Y0 - 2};NOP;");


        //            //writer.WriteLine($"MOD N51 {X0},{Y0 - 1} {X0},{Y0 - 1} :E'{N}';NOP;");
        //            ////writer.WriteLine($"MOD N51 {X0-2},{Y0 - 1} {X0-2},{Y0 - 1} :E'{N}';NOP;");//orig
        //            ////writer.WriteLine($"MOD N254 {X0 + 10+6},{Y0} {X0 + 10+6},{Y0} :E'{N}';NOP;");//Orig
        //            //writer.WriteLine($"MOD N254 {X0 + 7.5},{Y0 - 1} {X0 + 7.5},{Y0 - 1} :E'{N}';NOP;");//Orig
        //            #endregion
        //    }
        //}
        #endregion

        #region //Commented old DisSeg4 code on 18 november, 2025
        //public static void DisSeg4(double LL_x, double LL_y, double UR_x, double UR_y, string CompNmae, string filePath,string Ori,string partnumber,string assoPNs)
        //{
        //    double w = 6;   // Width
        //    double O = 4;   // Offset
        //    //public static void EquSeg4(double LL_x, double LL_y, double UR_x, double UR_y, string iEquName, string iOrientation, string filePath)
        //    using (writer = File.AppendText(filePath))
        //    {
        //        if (Ori.Equals("L"))
        //        {
        //            #region
        //            writer.WriteLine($"ADD R254  {LL_x - 4},{LL_y} {UR_x + 4},{UR_y + 4 + 4} ;");
        //            writer.WriteLine($"ADD N53 :T1001 :F3.0 :D :J7 :AC R254 {LL_x - 4 - 3},{LL_y - 30 - 48 + 8 + 8 + 2} '{CompNmae}' {UR_x + 4 - 8},{UR_y + 4 - 6 - 12 + 8 + 8 + 2};");
        //            #region//temp comment
        //            ////writer.WriteLine($"ADD info_sth_id :R0 {LL_x - 4},{LL_y - 2};");
        //            ////writer.WriteLine($"SMA info_sth_id {LL_x - 4},{LL_y - 2};");
        //            ////writer.WriteLine($"MOD N253 {LL_x-4},{LL_y - 4} 0,0 :E '{CompNmae}';NOP;");
        //            ////writer.WriteLine($"MOD N252 {LL_x - 4},{LL_y - 6} 0,0 :E '{partnumber}';NOP;");

        //            ////writer.WriteLine($"SHOW #E;");
        //            #endregion
        //            writer.WriteLine($"ADD N253 '{CompNmae}' {LL_x +2},{LL_y +3} :F1.0 :T1001 :D;;NOP;");
        //            writer.WriteLine($"ADD N52 '{partnumber}' {LL_x + 2},{LL_y} :F1.0;;NOP;");
        //            writer.WriteLine($"MOD N52 {LL_x + 2},{LL_y} 0,0 :E '{partnumber}';NOP;");
        //            #region//Included Assosiate PartNumbers for Break Connectors Family
        //            if (!string.IsNullOrEmpty(assoPNs))
        //            {
        //                int decre = 0;
        //                string[] arrassopns = assoPNs.Split(";");
        //                for (int assopns = 0; assopns <= arrassopns.Length - 1; assopns++)
        //                {
        //                    decre = decre + 4;
        //                    writer.WriteLine($"ADD N52 '{arrassopns[assopns]}' {LL_x + 2 + 3},{LL_y - decre} :F1.0;;NOP;");
        //                }
        //            }
        //            #endregion
        //            writer.WriteLine($"ADD N255 'N' {LL_x + 2},{LL_y - 3} :F1.0 :T4326 :D;;NOP;");
        //            #endregion
        //        }
        //        if (Ori.Equals("R"))
        //        {
        //            //if (CompNmae.EndsWith("_F")) CompNmae = CompNmae.Replace("_F", "_M");
        //            //else if (CompNmae.EndsWith("_M")) CompNmae = CompNmae.Replace("_M", "_F");
        //            writer.WriteLine($"ADD R254  {LL_x - 4 + 8 + 2},{LL_y} {UR_x + 4 + 8 + 2},{UR_y + 4 + 4} ;");
        //            writer.WriteLine($"ADD N53 :T1001 :F3.0 :D :J7 :AC R254 {LL_x - 4 - 3},{LL_y - 30 - 48 + 8 + 8 + 2} '{CompNmae}' {UR_x + 4 + 8 - 4 - 2},{UR_y + 4 - 6 - 12 + 8 + 8 + 2};");
        //            //writer.WriteLine($"ADD N53 :T1001 :F3.0 :D :J7 :AC R254 {LL_x - 4 - 3 + 36 - 4 - 2},{LL_y - 30 - 48 + 8 + 8 + 2} '{CompNmae}' {UR_x + 4 + 8 + 36 - 4 - 2},{UR_y + 4 - 6 - 12 + 8 + 8 + 2};");
        //            ////ADD info_sth_ci :R0 204 196;
        //            #region
        //            ////writer.WriteLine($"ADD info_sth_id :R0 {LL_x - 4 + 8 + 2},{LL_y - 30};");
        //            ////writer.WriteLine($"SMA info_sth_id {LL_x - 4 + 8 + 2},{LL_y - 30};");
        //            ////writer.WriteLine($"MOD N253 {LL_x - 4 + 8 + 2},{LL_y - 30} 0,0 :E '{CompNmae}';NOP;");
        //            ////writer.WriteLine($"MOD N252 {LL_x - 4 + 8 + 2},{LL_y - 30} 0,0 :E '{partnumber}';NOP;");
        //            ////writer.WriteLine($"SHOW #E;");
        //            #endregion
        //            writer.WriteLine($"ADD N253 '{CompNmae}' {LL_x + 2+9},{LL_y + 3} :F1.0 :T1001 :D;;NOP;");
        //            writer.WriteLine($"ADD N52 '{partnumber}' {LL_x + 2+9},{LL_y} :F1.0;;NOP;");
        //            writer.WriteLine($"MOD N52 {LL_x + 2 + 9},{LL_y} 0,0 :E '{partnumber}';NOP;");
        //            writer.WriteLine($"ADD N255 'N' {LL_x + 2+9},{LL_y - 3} :F1.0 :T4326 :D;;NOP;");
        //        }
        //    }
        //}
        #endregion

        #region //Commented old DisSeg3 code on 18 november, 2025
        //public static void DisSeg3(double X0, double Y0, double L, string filePath,string strOri)
        //{
        //    using (writer = File.AppendText(filePath))
        //    {
        //        if (strOri.Equals("L"))
        //        {
        //            writer.WriteLine($"ADD L214 {X0},{Y0} {X0},{Y0 + L}; ; ; ; NOP;");
        //            writer.WriteLine($"ADD L214 {X0 + 7.5-0.1+0.01+0.002},{Y0} {X0 + 7.5-0.1+0.01+0.002},{Y0 + L}; ; ; ; NOP;");
        //            writer.WriteLine($"ADD L214 {X0 + 10},{Y0} {X0 + 10},{Y0 + L}; ; ; ; NOP;");
        //        }
        //        if (strOri.Equals("R"))
        //        {
        //            writer.WriteLine($"ADD L214 {X0+10},{Y0} {X0+10},{Y0 + L}; ; ; ; NOP;");
        //            writer.WriteLine($"ADD L214 {X0 + 7.9+5-0.3-0.01-0.002},{Y0} {X0 + 7.9+5-0.3-0.01-0.002},{Y0 + L}; ; ; ; NOP;");
        //            writer.WriteLine($"ADD L214 {X0 + 10+10},{Y0} {X0 + 10+10},{Y0 + L}; ; ; ; NOP;");
        //        }
        //    }
        //}

        #endregion

        #region //Commented old DisSeg5 code on 18 november, 2025
        //public static void DisSeg5(double X0, double Y0,string filePath,string strOri)
        //{
        //    using (writer = File.AppendText(filePath))
        //    {
        //        //writer.WriteLine($"ADD I1 cont_sth_lastc_new_b :R (IROT)  :MY {X0},{Y0};");
        //        //writer.WriteLine("$$ DisSeg - End...");
        //        if (strOri.Equals("L"))
        //        {
        //            writer.WriteLine($"ADD I1 cont_sth_lastc_new_t :R0 :MY {X0},{Y0 + 2};");
        //            writer.WriteLine("$$ DisSeg - End...");
        //        }
        //        if (strOri.Equals("R"))
        //        {
        //            writer.WriteLine($"ADD I1 cont_sth_lastc_new_t {X0+20},{Y0 + 2};");
        //            writer.WriteLine("$$ DisSeg - End...");
        //        }
        //    }
        //}
        #endregion

        #region //Commented old DISSegEquipmentBox code on 18 november, 2025
        //public static void DISSegEquipmentBox(double LL_x, double LL_y, double UR_x, double UR_y, string iEquName,string Ori, string filePath)
        //{
        //    double w = 6;   // Width
        //    double O = 8;   // Offset


        //    using (writer = File.AppendText(filePath))
        //    {
        //        if (Ori == "R")
        //        {
        //            writer.WriteLine($"ADD R252 {LL_x},{LL_y - 18} {UR_x + 30},{UR_y +12};;;NOP;");
        //            writer.WriteLine($"ADD N202 '{iEquName}' :T1001 :F3.0 :D :AC R252 {LL_x - 18},{LL_y - 8} {UR_x + 4},{UR_y + 12};;NOP;");
        //        }
        //        else if (Ori == "L")
        //        {
        //            writer.WriteLine($"ADD R252 {LL_x},{LL_y + 18} {UR_x - 30},{UR_y - 12};;;NOP;");
        //            writer.WriteLine($"ADD N202 '{iEquName}' :T1001 :F3.0 :D :AC R252 {LL_x - 18},{LL_y - 8} {UR_x + 4},{UR_y + 12};;NOP;");
        //            //writer.WriteLine($"ADD R252 {LL_x - 2.5},{LL_y - O} {UR_x + w},{UR_y + O * 2};;;NOP;");
        //            //writer.WriteLine($"ADD N202 '{iEquName}' :T1001 :F3.0 :D :AC R252 {LL_x},{LL_y - O} {LL_x + 1},{UR_y + O * 2};;NOP;");
        //        }

        //    }
        //}
        #endregion

        #endregion

        #region//EQU - Double Type Connector************************

        #region //Commented old EquDTCSeg1 code on 18 november, 2025
        //public static void EquDTCSeg1(double X0, double Y0, string filePath, string strDISOrientation)
        //{
        //    using (writer = File.AppendText(filePath))
        //    {
        //        //writer.WriteLine("$$ DisSeg - Start...");
        //        //writer.WriteLine("LET IROT 0;");
        //        //writer.WriteLine($"ADD I1 cont_sth_1c_new_t :R (IROT)  :MY {X0},{Y0};");

        //        //writer.WriteLine("$$ DisSeg - Start...");
        //        //writer.WriteLine("LET IROT 0;");
        //        //macro_sth_cou_ci2
        //        //writer.WriteLine($"ADD I1 macro_sth_cou_ci2 :MY {X0},{Y0};");
        //        //writer.WriteLine($"ADD I1 cont_sth_1c_new_t {X0},{Y0};");//cont_sth_1c_new_t
        //        //writer.WriteLine($"ADD I1 cont_sth_1c_new_t :MY {X0},{Y0};");//cont_sth_1c_new_t
        //        if (strDISOrientation.Equals("L"))
        //        {
        //            //cont_sth_mg_hal
        //            writer.WriteLine($"ADD I1 cont_sth_1c_new_t :MY {X0},{Y0};");
        //        }
        //        else
        //        {
        //            writer.WriteLine($"ADD I1 cont_sth_1c_new_t {X0 + 20},{Y0};");
        //        }
        //    }
        //}
        #endregion

        #region //Commented old EquDTCSeg2 code on 18 november, 2025
        //public static void EquDTCSeg2(double X0, double Y0, string N, string filePath, string Ori,string BC_Comp_Nmae,string CompNmae,string tempOri)
        //{
        //    using (writer = File.AppendText(filePath))
        //    {
        //        //Cont_sth_md  - Left Ori,Cont_sth_mg - Right Ori rest of the thing recipticle
        //        //double lineLength = 30;
        //        //Left Side Pins
        //        string appoCOmpNmaeLeft = string.Empty;
        //        string appoCOmpNmaeRight = string.Empty;
        //        if (Ori.Equals("L"))
        //        {

        //            if (!tempOri.Equals("cont_sth_half_l"))
        //            {
        //                //writer.WriteLine($"ADD Cont_sth_mg {X0},{Y0};");
        //                writer.WriteLine($"ADD Cont_sth_mg_hal {X0},{Y0};");//oRIG
        //                writer.WriteLine($"MOD N51 {X0 + 0.8},{Y0} 0,0 :E '{N}'; NOP;");
        //                writer.WriteLine($"Add N254 'EQU' :F1.0 :R0 :AC I0 {X0},{Y0} :T4320 {X0},{Y0 - 2};NOP;");
        //            }
        //            else 
        //            {

        //                writer.WriteLine($"ADD Cont_sth_mg {X0 + 10},{Y0};");//oRIG
        //                writer.WriteLine($"MOD N254 {X0 + 10+ 3},{Y0} 0,0 :E '{N}'; NOP;");
        //                writer.WriteLine($"Add N254 'EQU' :F1.0 :R0 :AC I0 {X0 + 10},{Y0-2} :T4320 {X0 + 10},{Y0-2};NOP;");
        //            }
        //            //string appoCOmpNmaePNLeft = string.Empty;
        //            var appoCOmpNmae = (from item in Constants.arrComponentsWithPartNumber
        //                                where !string.IsNullOrEmpty(item) && item.Contains(BC_Comp_Nmae) && !item.Equals(CompNmae)
        //                                select item).ToArray();
        //            for (int comDEabove = 0; comDEabove <= Constants.arrDEabove.GetLength(0) - 1; comDEabove++)
        //            {
        //                if (Constants.arrDEabove[comDEabove, Constants.colDE_Connector - 1].Equals(appoCOmpNmae[0]) && Constants.arrDEabove[comDEabove, Constants.colDE_PartNumber - 1] != "" && Constants.arrDEabove[comDEabove, Constants.colDE_Panels - 1].Equals(Constants.textPanelPartName))
        //                {
        //                    appoCOmpNmaeLeft = Constants.arrDEabove[comDEabove, Constants.colDE_Connector - 1];
        //                    break;
        //                }
        //            }

        //            if (appoCOmpNmae[0].Equals(appoCOmpNmaeLeft) && !string.IsNullOrEmpty(appoCOmpNmaeLeft))
        //            { 
        //                writer.WriteLine($"ADD Cont_sth_mg {X0 + 10},{Y0};");//oRIG
        //                writer.WriteLine($"MOD N254 {X0 + 10 - 3},{Y0} 0,0 :E '{N}'; NOP;");
        //                writer.WriteLine($"Add N254 'EQU' :F1.0 :R0 :AC I0 {X0 + 10},{Y0} :T4320 {X0},{Y0 - 2};NOP;");
        //            }
        //            //writer.WriteLine($"ADD Cont_sth_md :MY {X0},{Y0};");//oRIG
        //            //writer.WriteLine($"Add N51 '{N}' :F1.0 :R0 :AC I0 {X0 + 0.8},{Y0} :T1003 {X0 + 0.8},{Y0};NOP;");//ORIG
        //        }
        //        if (Ori.Equals("R"))//Right Side Pins
        //        {
        //            if (!tempOri.Equals("cont_sth_half_r"))
        //            {
        //                //writer.WriteLine($"ADD contact_sth_receptacle {X0 + 10},{Y0};");//orig
        //                //writer.WriteLine($"Add N254 '{N}' :F1.0 :R0 :AC I0 {X0 + 11 - 2.5},{Y0} :T1003 {X0 + 11 - 2.5},{Y0};NOP;");//orig
        //                writer.WriteLine($"ADD Cont_sth_md {X0 + 20},{Y0};");
        //                writer.WriteLine($"MOD N51 {X0 + 11 - 2.5},{Y0} 0,0 :E '{N}'; NOP;");
        //                writer.WriteLine($"Add N254 'EQU' :F1.0 :R0 :AC I0 {X0 + 20},{Y0 - 2} :T4320 {X0 + 20},{Y0 - 2};NOP;");
        //            }
        //            else 
        //            {
        //                writer.WriteLine($"ADD Cont_sth_mg {X0 - 10 + 20},{Y0};");
        //                writer.WriteLine($"MOD N254 {X0 - 10 + 20 + 3},{Y0} 0,0 :E '{N}'; NOP;");
        //                writer.WriteLine($"Add N254 'EQU' :F1.0 :R0 :AC I0 {X0 - 10 + 20},{Y0 - 2} :T4320 {X0 - 10 + 20},{Y0 - 2};NOP;");
        //            }
        //            var appoCOmpNmae = (from item in Constants.arrComponentsWithPartNumber
        //                                where !string.IsNullOrEmpty(item) && item.Contains(BC_Comp_Nmae) && !item.Equals(CompNmae)
        //                                select item).ToArray();
        //            for (int comDEabove = 0; comDEabove <= Constants.arrDEabove.GetLength(0) - 1; comDEabove++)
        //            {
        //                if (Constants.arrDEabove[comDEabove, Constants.colDE_Connector - 1].Equals(appoCOmpNmae[0]) && Constants.arrDEabove[comDEabove, Constants.colDE_PartNumber - 1] != "" && Constants.arrDEabove[comDEabove, Constants.colDE_Panels - 1].Equals(Constants.textPanelPartName))
        //                {
        //                    appoCOmpNmaeRight = Constants.arrDEabove[comDEabove, Constants.colDE_Connector - 1];
        //                    break;
        //                }
        //            }
        //            if (appoCOmpNmae[0].Equals(appoCOmpNmaeRight) && !string.IsNullOrEmpty(appoCOmpNmaeRight))
        //            {
        //                writer.WriteLine($"ADD Cont_sth_mg {X0 - 10 + 20},{Y0};");
        //                writer.WriteLine($"MOD N254 {X0 - 10 + 20 + 3},{Y0} 0,0 :E '{N}'; NOP;");
        //                writer.WriteLine($"Add N254 'EQU' :F1.0 :R0 :AC I0 {X0 - 10 + 20},{Y0 - 2} :T4320 {X0 - 10 + 20},{Y0 - 2};NOP;");
        //            }
        //        }
        //    }
        //}
        #endregion

        #region //Commented old EquDTCSeg4 code on 18 november, 2025
        //public static void EquDTCSeg4(double LL_x, double LL_y, double UR_x, double UR_y, string CompNmae, string filePath,string partnumber,string Ori, string assoPNs,string BC_Comp_Nmae,string tempstr)
        //{
        //    int widthspaceincrese = 0;
        //    int widthincresecont_sth_half_l = 0;
        //    using (writer = File.AppendText(filePath))
        //    {
        //        if (Ori.Equals("L"))
        //        {
        //            if (tempstr.Equals("cont_sth_md")) widthspaceincrese = 8;
        //            if (tempstr.Equals("cont_sth_half_l")) widthincresecont_sth_half_l = 0;
        //            writer.WriteLine($"ADD R254  {LL_x + 4+4- widthspaceincrese},{LL_y} {UR_x + 8+4- widthspaceincrese},{UR_y + 4 + 4} ;");
        //            writer.WriteLine($"ADD N53 :T1001 :F3.0 :D :J7 :AC R254 {LL_x - 4 - 3},{LL_y - 30 - 48 + 8 + 8 + 2} '{CompNmae}' {UR_x + 8},{UR_y + 4 - 6 - 12 + 8 + 8 + 2};");
        //            #region//temp comment
        //            ////writer.WriteLine($"ADD info_sth_id :R0 {LL_x - 4},{LL_y - 2};");
        //            ////writer.WriteLine($"SMA info_sth_id {LL_x - 4},{LL_y - 2};");
        //            ////writer.WriteLine($"MOD N253 {LL_x-4},{LL_y - 4} 0,0 :E '{CompNmae}';NOP;");
        //            ////writer.WriteLine($"MOD N252 {LL_x - 4},{LL_y - 6} 0,0 :E '{partnumber}';NOP;");

        //            ////writer.WriteLine($"SHOW #E;");
        //            #endregion

        //            writer.WriteLine($"ADD N253 '{CompNmae}' {LL_x + 2},{LL_y + 3 - 1} :F1.0 :T1001 :D;;NOP;");
        //            writer.WriteLine($"ADD N52 '{partnumber}' {LL_x + 2 + 1},{LL_y} :F1.0;;NOP;");
        //            #region//Included Assosiate PartNumbers for Double Connectors Family
        //            if (!string.IsNullOrEmpty(assoPNs))
        //            {
        //                int decre = 0;
        //                string[] arrassopns = assoPNs.Split(";");
        //                for (int assopns = 0; assopns <= arrassopns.Length - 1; assopns++)
        //                {
        //                    decre = decre + 4;
        //                    writer.WriteLine($"ADD N52 '{arrassopns[assopns]}' {LL_x + 2 + 3},{LL_y - decre} :F1.0;;NOP;");
        //                }
        //            }
        //            #endregion
        //            writer.WriteLine($"ADD N255 'N' {LL_x + 2+10},{LL_y - 3+1} :F1.0 :T4326 :D;;NOP;");
        //            //Opposite Component Display for Double Connector
        //            if (Constants.arrComponentsWithPartNumber.Contains(CompNmae))
        //            {
        //                string appoCOmpNmaePNLeft = string.Empty;
        //                var appoCOmpNmae = (from item in Constants.arrComponentsWithPartNumber
        //                                    where !string.IsNullOrEmpty(item) && item.Contains(BC_Comp_Nmae) && !item.Equals(CompNmae)
        //                                    select item).ToArray();
        //                //Get part Number for OppComp
        //                for (int comDEabove = 0; comDEabove <= Constants.arrDEabove.GetLength(0) - 1; comDEabove++)
        //                {
        //                    if (Constants.arrDEabove[comDEabove, Constants.colDE_Connector - 1].Equals(appoCOmpNmae[0]) && Constants.arrDEabove[comDEabove, Constants.colDE_PartNumber - 1] != "" && Constants.arrDEabove[comDEabove, Constants.colDE_Panels - 1].Equals(Constants.textPanelPartName))
        //                    {
        //                        appoCOmpNmaePNLeft = Constants.arrDEabove[comDEabove, Constants.colDE_PartNumber - 1];
        //                        break;
        //                    }
        //                }

        //                if (appoCOmpNmae.Length == 1 && !string.IsNullOrEmpty(appoCOmpNmaePNLeft))
        //                {
        //                    writer.WriteLine($"ADD R254  {LL_x - 4 + 8 + 2},{LL_y} {UR_x + 4 + 8 + 2},{UR_y + 4 + 4} ;");
        //                    writer.WriteLine($"ADD N53 :T1001 :F3.0 :D :J7 :AC R254 {LL_x - 4 - 3+20+15},{LL_y - 30 - 48 + 8 + 8 + 2} '{appoCOmpNmae[0]}' {UR_x + 4 + 8 - 4 - 2+20+15},{UR_y + 4 - 6 - 12 + 8 + 8 + 2};");
        //                    writer.WriteLine($"ADD N253 '{appoCOmpNmae[0]}' {LL_x - 6+15},{LL_y + 3} :F1.0 :T1001 :D;;NOP;");
        //                    writer.WriteLine($"ADD N52 '{appoCOmpNmaePNLeft}' {LL_x - 20+30},{LL_y} :F1.0;;NOP;");
        //                    writer.WriteLine($"ADD N255 'N' {LL_x+10},{LL_y - 3} :F1.0 :T4326 :D;;NOP;");
        //                }
        //            }

        //        }
        //        if (Ori.Equals("R"))
        //        {                    
        //            if (tempstr.Equals("cont_sth_md")) widthspaceincrese = 8; //|| tempstr.Equals("")) //tempstr.Equals("cont_sth_md")
        //            writer.WriteLine($"ADD R254  {LL_x - 4 + 8 + 2+ widthspaceincrese},{LL_y} {UR_x + 4 + 8 + 2+ widthspaceincrese},{UR_y + 4 + 4} ;");
        //            writer.WriteLine($"ADD N53 :T1001 :F3.0 :D :J7 :AC R254 {LL_x - 4 - 3+20},{LL_y - 30 - 48 + 8 + 8 + 2} '{CompNmae}' {UR_x + 4 + 8 - 4 - 2+20},{UR_y + 4 - 6 - 12 + 8 + 8 + 2};");
        //            writer.WriteLine($"ADD N253 '{CompNmae}' {LL_x + 2 + 9+2-2},{LL_y + 3} :F1.0 :T1001 :D;;NOP;");
        //            writer.WriteLine($"ADD N52 '{partnumber}' {LL_x + 2 + 9},{LL_y} :F1.0;;NOP;");
        //            writer.WriteLine($"ADD N255 'N' {LL_x + 2 + 9},{LL_y - 3} :F1.0 :T4326 :D;;NOP;");
        //            //Opposite Component Display for Double Connector
        //            if (Constants.arrComponentsWithPartNumber.Contains(CompNmae))
        //            {
        //                string appoCOmpNmaePNRight = string.Empty;
        //                var appoCOmpNmae = (from item in Constants.arrComponentsWithPartNumber
        //                                    where !string.IsNullOrEmpty(item) && item.Contains(BC_Comp_Nmae) && !item.Equals(CompNmae)
        //                                    select item).ToArray();
        //                //Get part Number for OppComp
        //                for (int comDEabove = 0; comDEabove <= Constants.arrDEabove.GetLength(0) - 1; comDEabove++)
        //                { 
        //                    if (Constants.arrDEabove[comDEabove, Constants.colDE_Connector - 1].Equals(appoCOmpNmae[0]) && Constants.arrDEabove[comDEabove, Constants.colDE_PartNumber - 1] != "" && Constants.arrDEabove[comDEabove, Constants.colDE_Panels - 1].Equals(Constants.textPanelPartName))
        //                    {
        //                        appoCOmpNmaePNRight = Constants.arrDEabove[comDEabove, Constants.colDE_PartNumber - 1];
        //                        break;
        //                    }
        //                }

        //                if (appoCOmpNmae.Length == 1 && !string.IsNullOrEmpty(appoCOmpNmaePNRight))
        //                {
        //                    writer.WriteLine($"ADD R254  {LL_x - 4 + 8 + 2},{LL_y} {UR_x + 4 + 8 + 2},{UR_y + 4 + 4} ;");
        //                    writer.WriteLine($"ADD N53 :T1001 :F3.0 :D :J7 :AC R254 {LL_x - 4 - 3},{LL_y - 30 - 48 + 8 + 8 + 2} '{appoCOmpNmae[0]}' {UR_x + 4 + 8 - 4 - 2},{UR_y + 4 - 6 - 12 + 8 + 8 + 2};");
        //                    writer.WriteLine($"ADD N253 '{appoCOmpNmae[0]}' {LL_x-6},{LL_y + 3} :F1.0 :T1001 :D;;NOP;");
        //                    writer.WriteLine($"ADD N52 '{appoCOmpNmaePNRight}' {LL_x-20},{LL_y} :F1.0;;NOP;");
        //                    writer.WriteLine($"ADD N255 'N' {LL_x},{LL_y - 3} :F1.0 :T4326 :D;;NOP;");
        //                }
        //            }
        //        }
        //    }
        //}
        #endregion

        #region //Commented old EquDTCSeg3 code on 18 november, 2025
        //public static void EquDTCSeg3(double X0, double Y0, double L, string filePath,string strOri)
        //{
        //    using (writer = File.AppendText(filePath))
        //    {
        //        if (strOri.Equals("L"))
        //        {
        //            writer.WriteLine($"ADD L214 {X0},{Y0} {X0},{Y0 + L}; ; ; ; NOP;");
        //            writer.WriteLine($"ADD L214 {X0 + 7.5 - 0.1 + 0.01 + 0.002},{Y0} {X0 + 7.5 - 0.1 + 0.01 + 0.002},{Y0 + L}; ; ; ; NOP;");
        //            writer.WriteLine($"ADD L214 {X0 + 10},{Y0} {X0 + 10},{Y0 + L}; ; ; ; NOP;");
        //        }
        //        if (strOri.Equals("R"))
        //        {
        //            writer.WriteLine($"ADD L214 {X0 + 10},{Y0} {X0 + 10},{Y0 + L}; ; ; ; NOP;");
        //            writer.WriteLine($"ADD L214 {X0 + 7.9 + 5 - 0.3 - 0.01 - 0.002},{Y0} {X0 + 7.9 + 5 - 0.3 - 0.01 - 0.002},{Y0 + L}; ; ; ; NOP;");
        //            writer.WriteLine($"ADD L214 {X0 + 10 + 10},{Y0} {X0 + 10 + 10},{Y0 + L}; ; ; ; NOP;");
        //        }
        //    }
        //}
        #endregion

        #region //Commented old EquDTCSeg5 code on 18 november, 2025
        //public static void EquDTCSeg5(double X0, double Y0, string filePath,string strOri)
        //{
        //    using (writer = File.AppendText(filePath))
        //    {
        //        if (strOri.Equals("L"))
        //        {
        //            writer.WriteLine($"ADD I1 cont_sth_lastc_new_t :R0 :MY {X0},{Y0 + 2};");
        //            writer.WriteLine("$$ DisSeg - End...");
        //        }
        //        if (strOri.Equals("R"))
        //        {
        //            writer.WriteLine($"ADD I1 cont_sth_lastc_new_t {X0 + 20},{Y0 + 2};");
        //            writer.WriteLine("$$ DisSeg - End...");
        //        }
        //    }
        //}
        #endregion

        #region //Commented old DTCSegEquipmentBox code on 18 november, 2025
        //public static void DTCSegEquipmentBox(double LL_x, double LL_y, double UR_x, double UR_y, string iEquName, string Ori, string filePath)
        //{
        //    double w = 6;   // Width
        //    double O = 8;   // Offset


        //    using (writer = File.AppendText(filePath))
        //    {
        //        if (Ori == "R")
        //        {
        //            writer.WriteLine($"ADD R252 {LL_x},{LL_y - 18} {UR_x + 30},{UR_y + 12};;;NOP;");
        //            writer.WriteLine($"ADD N202 '{iEquName}' :T1001 :F3.0 :D :AC R252 {LL_x - 18},{LL_y - 8} {UR_x + 4},{UR_y + 12};;NOP;");
        //        }
        //        else if (Ori == "L")
        //        {
        //            writer.WriteLine($"ADD R252 {LL_x-10},{LL_y - 18} {UR_x + 30-10},{UR_y + 12};;;NOP;");
        //            writer.WriteLine($"ADD N202 '{iEquName}' :T1001 :F3.0 :D :AC R252 {LL_x - 18},{LL_y - 8} {UR_x + 4},{UR_y + 12};;NOP;");
        //            //writer.WriteLine($"ADD R252 {LL_x - 2.5},{LL_y - O} {UR_x + w},{UR_y + O * 2};;;NOP;");
        //            //writer.WriteLine($"ADD N202 '{iEquName}' :T1001 :F3.0 :D :AC R252 {LL_x},{LL_y - O} {LL_x + 1},{UR_y + O * 2};;NOP;");
        //        }

        //    }
        //}
        #endregion

        #endregion//************************

        #region//EQU - Simple/Single Type Connector
        //public static void EquSeg1(double X0, double Y0, string iOrientation, string iCRepSymbol, string iEquName, string filePath)
        //{
        //    using (writer = File.AppendText(filePath))
        //    {
        //        string Symb = string.Empty;
        //        //writer.WriteLine($"$$ {iEquName} - Start");
        //        if (iCRepSymbol == "Full")
        //        {
        //            if (iEquName.ToUpper().Contains("_F"))
        //            {
        //                Symb = "cont_sth_1_new_fixed_t";
        //            }
        //            else
        //            {
        //                Symb = "cont_sth_1_new_t";
        //            }
        //        }
        //        else if (iCRepSymbol == "BOTop")
        //        {
        //            Symb = "cont_sth_1_new_b";
        //        }

        //        if (iOrientation == "R")
        //        {
        //            writer.WriteLine($"ADD I1 {Symb} :MY {X0},{Y0};;NOP;");
        //        }
        //        else if (iOrientation == "L")
        //        {
        //            writer.WriteLine($"ADD I1 {Symb} {X0},{Y0};;NOP;");
        //        }
        //        else if (iOrientation == "B")
        //        {
        //            writer.WriteLine($"ADD I1 {Symb} :R90 {X0},{Y0};;NOP;");
        //        }
        //        else if (iOrientation == "T")
        //        {
        //            writer.WriteLine($"ADD I1 {Symb} :R90 {X0},{Y0 + 7.5};;NOP;");
        //        }
        //    }
        //}

        //public static void EquSeg2(double X0, double Y0, string N, string iOrientation, string filePath)
        //{
        //    double iLengthOfLine = 30;
        //    string Symb;
        //    double PinNumber_x, PinNumber_y;
        //    int Rotation;

        //    if (iOrientation == "R")
        //    {
        //        Symb = "contact_sth_ml";
        //        PinNumber_x = X0;//+ 5;
        //        PinNumber_y = Y0 - 1;
        //        Rotation = 0;
        //    }
        //    else if (iOrientation == "L")
        //    {
        //        Symb = "contact_sth_mr";
        //        PinNumber_x = X0 - 3;
        //        //PinNumber_x = X0 - 5;//orig
        //        PinNumber_y = Y0 - 2;
        //        Rotation = 0;
        //    }
        //    else if (iOrientation == "B")
        //    {
        //        Symb = "contact_sth_mr";
        //        PinNumber_x = X0;
        //        //PinNumber_x = X0 - 1;
        //        //PinNumber_y = Y0 - 5;
        //        PinNumber_y = Y0;
        //        Rotation = 90;
        //    }
        //    else if (iOrientation == "T")
        //    {
        //        Symb = "contact_sth_ml";
        //        PinNumber_x = X0 - 1;
        //        PinNumber_y = Y0 + 5;
        //        Rotation = 90;
        //    }
        //    else
        //    {
        //        // Default fallback
        //        Symb = "contact_sth_ml";
        //        PinNumber_x = X0;
        //        PinNumber_y = Y0;
        //        Rotation = 0;
        //    }


        //    using (writer = File.AppendText(filePath))
        //    {
        //        writer.WriteLine($"ADD {Symb} :R{Rotation} {X0}, {Y0}; ");
        //        writer.WriteLine($"MOD N51 {PinNumber_x}, {PinNumber_y} 0,0 :E'{N}';");
        //        //writer.WriteLine($"MOD N51 {PinNumber_x}, {PinNumber_y} {PinNumber_x}, {PinNumber_y} :E'{N}';NOP;");
        //        //writer.WriteLine($"Add N254 'EQU' :F1.0 :R{Rotation} :AC I0 {X0 - 11.5}, {Y0} :T4320 {X0}, {Y0};NOP;");
        //        //writer.WriteLine($"Add N254 'EQU' :F1.0 :R{Rotation} :AC I0 {X0 + 8}, {Y0} :T4320 {X0 - 3}, {Y0 - 2};NOP;");
        //        //writer.WriteLine($"Add N254 'EQU' :F1.0 :R{Rotation} :AC I0 {X0 + 8}, {Y0} :T4320 {X0 - 3}, {Y0 - 2};NOP;");
        //        writer.WriteLine($"Add N254 'EQU' :F1.0 :R{Rotation} :AC I0 {PinNumber_x}, {PinNumber_y} :T4320 {PinNumber_x}, {PinNumber_y};NOP;");
        //        //writer.WriteLine($"Add N254 'EQU' :F1.0 :R{Rotation} :AC I0 {X0}, {Y0} :T4320 {X0}, {Y0};NOP;");
        //        //writer.WriteLine($"Add N254 'EQU' :F1.0 :R{Rotation} :AC I0 {X0 - 4}, {Y0 - 2} :T4320 {X0 - 4}, {Y0 - 2};NOP;");
        //    }
        //}

        //public static void EquSeg3(double X0, double Y0, double iLength, string iOrientation, string filePath)
        //{
        //    //double w = 5.5;
        //    double w = 7.5;

        //    using (writer = File.AppendText(filePath))
        //    {
        //        writer.WriteLine($"GRID 0.5,2;");
        //        if (iOrientation == "R")
        //        {
        //            writer.WriteLine($"ADD L214 {X0 + w * 2},{Y0} {X0 + w * 2},{Y0 + iLength}; ; ; ; NOP;");
        //            writer.WriteLine($"ADD L214 {X0 + w},{Y0} {X0 + w},{Y0 + iLength}; ; ; ; NOP;");
        //        }
        //        else if (iOrientation == "L")
        //        {
        //            writer.WriteLine($"ADD L214 {X0},{Y0} {X0},{Y0 + iLength}; ; ; ; NOP;");
        //            writer.WriteLine($"ADD L214 {X0 + w},{Y0} {X0 + w},{Y0 + iLength}; ; ; ; NOP;");
        //        }
        //        else if (iOrientation == "B")
        //        {
        //            writer.WriteLine($"ADD L214 {X0},{Y0} {X0 + 8},{Y0}; ; ; ; NOP;");
        //            writer.WriteLine($"ADD L214 {X0},{Y0 - w} {X0 + 8},{Y0 - w}; ; ; ; NOP;");
        //            //writer.WriteLine($"ADD L214 {X0},{Y0 + w} {X0 + iLength},{Y0 + w}; ; ; ; NOP;");
        //            //writer.WriteLine($"ADD L214 {X0},{Y0 - w} {X0 + iLength},{Y0 - w}; ; ; ; NOP;");

        //        }
        //        else if (iOrientation == "T")
        //        {
        //            writer.WriteLine($"ADD L214 {X0},{Y0} {X0 + iLength},{Y0}; ; ; ; NOP;");
        //            writer.WriteLine($"ADD L214 {X0},{Y0 + w} {X0 + iLength},{Y0 + w}; ; ; ; NOP;");
        //        }
        //        writer.WriteLine($"GRID 0.5,2;");
        //        // Optional line for debugging or future use
        //        // writer.WriteLine($"ADD L214 {X0 + 8},{Y0} {X0 + 10},{Y0 + iLength}; ; ; ; NOP;");
        //    }
        //}

        //public static void EquSeg4(double LL_x, double LL_y, double UR_x, double UR_y, string iEquName, string iOrientation, string filePath)
        //{
        //    double w = 6;   // Width
        //    double O = 4;   // Offset


        //    using (writer = File.AppendText(filePath))
        //    {
        //        #region
        //        //if (iOrientation == "R")
        //        //{
        //        //    writer.WriteLine($"ADD R254 {LL_x - w},{LL_y - O} {UR_x},{UR_y + O * 2};;;NOP;");
        //        //    writer.WriteLine($"ADD N53 '{iEquName}' :T1001 :F3.0 :D :AC R254 {LL_x - w},{LL_y - O} {LL_x - 5},{UR_y + O * 2};;NOP;");
        //        //}
        //        //else if (iOrientation == "L")
        //        //{
        //        //    writer.WriteLine($"ADD R254 {LL_x - 11.5},{LL_y - O} {UR_x + w},{UR_y + O * 2};;;NOP;");
        //        //    writer.WriteLine($"ADD N53 '{iEquName}' :T1001 :F3.0 :D :AC R254 {LL_x},{LL_y - O} {LL_x + 1},{UR_y + O * 2};;NOP;");
        //        //}
        //        //else if (iOrientation == "B")
        //        //{
        //        //    writer.WriteLine($"ADD R254 {LL_x - 8},{LL_y - 2} {LL_x + 16},{LL_y + 4};;;NOP;");
        //        //    writer.WriteLine($"ADD N53 '{iEquName}' :T1001 :F3.0 :R0 :D :AC R254 {LL_x - 8},{LL_y - 2} {LL_x - 1},{LL_y + 6};;NOP;");
        //        //    //writer.WriteLine($"ADD R254 {LL_x - 2 * O},{LL_y} {UR_x + 2 * O},{UR_y + w};;;NOP;");
        //        //    //writer.WriteLine($"ADD N53 '{iEquName}' :T1001 :F3.0 :R0 :D :AC R254 {LL_x - 2 * O},{LL_y} {LL_x - 1},{UR_y + 2 * O};;NOP;");
        //        //}
        //        //else if (iOrientation == "T")
        //        //{
        //        //    writer.WriteLine($"ADD R254 {LL_x - 2 * O},{LL_y - w} {UR_x + 2 * O},{UR_y};;;NOP;");
        //        //    writer.WriteLine($"ADD N53 '{iEquName}' :T1001 :F3.0 :R0 :D :AC R254 {LL_x - 2 * O},{LL_y - w} {LL_x - 1},{UR_y + 2 * O + 4};;NOP;");
        //        //}
        //        #endregion
        //        if (iOrientation == "R")
        //        {
        //            writer.WriteLine($"ADD R254 {LL_x - w},{LL_y - O} {UR_x + 2},{UR_y + O * 2};;;NOP;");
        //            //writer.WriteLine($"ADD R254 {LL_x - w},{LL_y - O} {UR_x},{UR_y + O * 2};;;NOP;");//orig
        //            writer.WriteLine($"ADD N53 '{iEquName}' :T1001 :F3.0 :D :AC R254 {LL_x - w},{LL_y - O} {LL_x - 5},{UR_y + O * 2};;NOP;");
        //        }
        //        else if (iOrientation == "L")
        //        {
        //            writer.WriteLine($"ADD R254 {LL_x - 2.5},{LL_y - O} {UR_x + w},{UR_y + O * 2};;;NOP;");
        //            writer.WriteLine($"ADD N53 '{iEquName}' :T1001 :F3.0 :D :AC R254 {LL_x},{LL_y - O} {LL_x + 1},{UR_y + O * 2};;NOP;");
        //        }
        //        else if (iOrientation == "B")
        //        {
        //            writer.WriteLine($"ADD R254 {LL_x - 8},{LL_y - 2} {LL_x + 16},{LL_y + 4};;;NOP;");
        //            writer.WriteLine($"ADD N53 '{iEquName}' :T1001 :F3.0 :R0 :D :AC R254 {LL_x - 8},{LL_y - 2} {LL_x - 1},{LL_y + 6};;NOP;");
        //            //writer.WriteLine($"ADD R254 {LL_x - 2 * O},{LL_y} {UR_x + 2 * O},{UR_y + w};;;NOP;");
        //            //writer.WriteLine($"ADD N53 '{iEquName}' :T1001 :F3.0 :R0 :D :AC R254 {LL_x - 2 * O},{LL_y} {LL_x - 1},{UR_y + 2 * O};;NOP;");
        //        }
        //        else if (iOrientation == "T")
        //        {
        //            writer.WriteLine($"ADD R254 {LL_x - 2 * O},{LL_y - w} {UR_x + 2 * O},{UR_y};;;NOP;");
        //            writer.WriteLine($"ADD N53 '{iEquName}' :T1001 :F3.0 :R0 :D :AC R254 {LL_x - 2 * O},{LL_y - w} {LL_x - 1},{UR_y + 2 * O + 4};;NOP;");
        //        }
        //        // Optional: Other label styles from original VB6 comments could be added here.
        //    }
        //}

        //public static void EquSeg5(double X0, double Y0, string iOrientation, string iCRepSymbol, string iEquName, string filePath, string Isfull)
        //{
        //    string Symb = "";

        //    if (iCRepSymbol == "BOBot")
        //    {
        //        Symb = "cont_sth_last_new_t";
        //        //cont_sth_last_new_t
        //    }
        //    else if (iCRepSymbol == "Full")
        //    {
        //        if (iEquName.ToUpper().Contains("_F"))
        //        {
        //            Symb = "cont_sth_last_new_fixed_b";
        //        }
        //        else
        //        {
        //            if (Isfull.Equals("no"))
        //            {

        //                Symb = "cont_sth_last_new_t";//partial Symbol for footer
        //            }
        //            else
        //            {
        //                //Symb = "cont_sth_last_new_b"; //Orig
        //                Symb = "cont_sth_last_new_b";//Full Symbol for footer
        //            }
        //        }
        //    }

        //    using (writer = File.AppendText(filePath))
        //    {
        //        if (iOrientation == "R")
        //        {
        //            writer.WriteLine($"ADD I1 {Symb} :MY {X0},{Y0 + 2};;NOP;");
        //        }
        //        else if (iOrientation == "L")
        //        {
        //            writer.WriteLine($"ADD I1 {Symb} {X0},{Y0 + 2};;NOP;");
        //            //writer.WriteLine($"ADD I1 {Symb} :MY {X0 - 7.5},{Y0};;NOP;");//Orig
        //        }
        //        else if (iOrientation == "B")
        //        {
        //            writer.WriteLine($"ADD I1 {Symb} :R90 {X0 + 8},{Y0};;NOP;");
        //            //writer.WriteLine($"ADD I1 {Symb} :R90 :MY {X0},{Y0 - 7.5};;NOP;");
        //        }
        //        else if (iOrientation == "T")
        //        {
        //            writer.WriteLine($"ADD I1 {Symb} :R90 {X0},{Y0 + 7.5};;NOP;");
        //        }
        //    }
        //}

        //public static void EquSegPartNumber(double iX0, double iY0, string iCompName, string iPartNumber, string filePath, string assoPNs)
        //{
        //    using (writer = File.AppendText(filePath))
        //    {
        //        //ADD I1 info_sth_cmd 70,126; ; NOP;
        //        //MOD N2 70,124 70,124 :L253: E'Con1'; ; NOP;
        //        //MOD N52 70,120 70,120 :E'D38999-24FC35SA'; ; NOP;
        //        #region// Commented Original  Code for adding assosiated text for respective part numbers
        //        //writer.WriteLine($"ADD N253 '{iCompName}' {iX0},{iY0 + 4 + 4} :F1.0 :T1001 :D;;NOP;");
        //        //writer.WriteLine($"ADD N52 '{iPartNumber}' {iX0},{iY0 + 4} :F1.0;;NOP;");//ORIG

        //        #region// Commented Included Assosiate PartNumbers for Connectors Family
        //        //if (!string.IsNullOrEmpty(assoPNs))
        //        //{
        //        //    int decre = 0;
        //        //    string[] arrassopns = assoPNs.Split(";");
        //        //    for (int assopns = 0; assopns <= arrassopns.Length - 1; assopns++)
        //        //    {
        //        //        decre = decre + 4;

        //        //        //writer.WriteLine($"ADD I1 info_sth_cmd_HAL {iX0 + 3},{iY0 + 4 - decre}; ; NOP;");
        //        //        //writer.WriteLine($"MOD N52 :T1004 :F1.0 :R0 :D :J4 :AC R254 {iX0 + 3},{iY0 + 4 - decre} '{arrassopns[assopns]}' {iX0},{iY0 + 4 - decre};");
        //        //    }
        //        //}
        //        #endregion
        //        ////writer.WriteLine($"ADD N255 'N' {iX0},{iY0} :F1.0 :T4326 :D;;NOP;");
        //        #endregion//Original  Code for adding assosiated text for respective part numbers

        //        #region//NATHALI code

        //        //writer.WriteLine($"ADD I1 info_sth_cmd {iX0},{iY0}; ; NOP;");
        //        //writer.WriteLine($"MOD N2 {iX0},{iY0 - 2} {iX0},{iY0 - 2} :L253 :E'{iCompName}'; ; NOP;");

        //        //#region//Included Assosiate PartNumbers for Connectors Family
        //        //writer.WriteLine($"ADD N52 '{iPartNumber}' {iX0},{iY0 + 4} :F1.0;;NOP;");
        //        writer.WriteLine($"ADD I1 info_sth_cmd {iX0},{iY0 + 4 + 4}; ; NOP;");
        //        writer.WriteLine($"MOD N2 {iX0},{iY0 + 4 + 4 - 2} {iX0},{iY0 + 4 + 4 - 2} :L253 :E'{iCompName}'; ; NOP;");
        //        writer.WriteLine($"MOD N52 {iX0},{iY0 + 4 + 4 - 4} {iX0},{iY0 + 4 + 4 - 4} :E'{iPartNumber}'; ; NOP;");
        //        if (!string.IsNullOrEmpty(assoPNs))
        //        {
        //            int decre = 0;
        //            string[] arrassopns = assoPNs.Split(";");
        //            for (int assopns = 0; assopns <= arrassopns.Length - 1; assopns++)
        //            {
        //                decre = decre + 2;
        //                //info_sth_cmd_HAL
        //                writer.WriteLine($"ADD I1 info_sth_cmd_HAL {iX0},{iY0 + 4 + 4 - 1 - decre}; ; NOP;");
        //                writer.WriteLine($"MOD N52 {iX0},{iY0 + 4 + 4 - 1 - decre} {iX0 - 1},{iY0 + 4 + 4 - 1 - decre} :E'{arrassopns[assopns]}'; ; NOP;");
        //            }
        //        }
        //        #endregion

        //    }
        //}

        //public static void SegEquipmentBox(double LL_x, double LL_y, double UR_x, double UR_y, string iEquName, string iOrientation, string filePath)
        //{
        //    double w = 6;   // Width
        //    double O = 4;   // Offset


        //    using (writer = File.AppendText(filePath))
        //    {
        //        if (iOrientation == "R")
        //        {
        //            writer.WriteLine($"ADD R252 {LL_x - w},{LL_y - O} {UR_x + 2},{UR_y + O * 2};;;NOP;");
        //            //writer.WriteLine($"ADD R254 {LL_x - w},{LL_y - O} {UR_x},{UR_y + O * 2};;;NOP;");//orig
        //            writer.WriteLine($"ADD N202 '{iEquName}' :T1001 :F3.0 :D :AC R252 {LL_x - w},{LL_y - O} {LL_x - 5},{UR_y + O * 2};;NOP;");
        //        }
        //        else if (iOrientation == "L")
        //        {
        //            writer.WriteLine($"ADD R252 {LL_x - 2.5},{LL_y - O} {UR_x + w},{UR_y + O * 2};;;NOP;");
        //            writer.WriteLine($"ADD N202 '{iEquName}' :T1001 :F3.0 :D :AC R252 {LL_x},{LL_y - O} {LL_x + 1},{UR_y + O * 2};;NOP;");
        //        }
        //        else if (iOrientation == "B")
        //        {
        //            writer.WriteLine($"ADD R252 {LL_x - 8},{LL_y - 2} {LL_x + 16},{LL_y + 4};;;NOP;");
        //            writer.WriteLine($"ADD N202 '{iEquName}' :T1001 :F3.0 :R0 :D :AC R252 {LL_x - 8},{LL_y - 2} {LL_x - 1},{LL_y + 6};;NOP;");
        //            //writer.WriteLine($"ADD R254 {LL_x - 2 * O},{LL_y} {UR_x + 2 * O},{UR_y + w};;;NOP;");
        //            //writer.WriteLine($"ADD N53 '{iEquName}' :T1001 :F3.0 :R0 :D :AC R254 {LL_x - 2 * O},{LL_y} {LL_x - 1},{UR_y + 2 * O};;NOP;");
        //        }
        //        else if (iOrientation == "T")
        //        {
        //            writer.WriteLine($"ADD R252 {LL_x - 2 * O},{LL_y - w} {UR_x + 2 * O},{UR_y};;;NOP;");
        //            writer.WriteLine($"ADD N202 '{iEquName}' :T1001 :F3.0 :R0 :D :AC R252 {LL_x - 2 * O},{LL_y - w} {LL_x - 1},{UR_y + 2 * O + 4};;NOP;");
        //        }
        //        // Optional: Other label styles from original VB6 comments could be added here.
        //    }
        //}
        #endregion

        public static void DrawNewHpLine_Length(double iStartX, double iStartY, double iLength, int iWireCode, int iWireGauge, int iWireLength,string filepath)
        {
            // Draw Horizontal Positive line
            string sMidCoord = $"{iStartX + iLength / 2},{iStartY}";

            // Write output to file
            using (var writer = File.AppendText(filepath))
            {
                writer.WriteLine($"ADD I2 sth_cable_id_t {sMidCoord};NOP;");
                writer.WriteLine($"MOD N154 {sMidCoord} {sMidCoord} :L54 STOR_MID :E'{iWireCode}' JU ;NOP;");
                writer.WriteLine($"MOD N55 {sMidCoord} {sMidCoord} :E'#{iWireGauge}';NOP;");
            }
        }
        
        public static void Simple2PointConnection(double p1x, double p1y, double p2x, double p2y, string wireCode, string wireGauge)
        {
            double X1 = p1x, Y1 = p1y;
            double X2 = p2x, Y2 = p2y;

            #region commented
            //string command = $"ADD L154 :W0.0 :FILL {X1},{Y1} {X2},{Y2};;;;NOP";
            //            GRI 2.0,2;
            //            SHOW #E;
            //TESTDIS;
            //            ADD L154 72,148 348,148; ; NOP; ;
            //            ADD N54 :R0: J8: F2: D: T1002: AC L154 72,148 'WIRENB' 210,148; NOP;
            //            ADD N56 :R0: S10: D: J2: F2: T3006: AC L154 72,148 '#' 212,148;
            //            ADD N59 :R0: D: J2: F2: T3009: AC L154 72,148 'LENGTH' 218,148;
            //            ADD I2 sth_s: R0 88,148;
            //            MOD N250 88,148 0,0 :L254 STOR_MID :E'CABLE_TYPE' JU; NOP;
            //            TESTDIS_OFF;
            //            ADD N58 :J7: T3003: D: F1: R0 'GROUPE' :AC I2 88,148 86.5,148.5; NOP;
            //            ADD I2 sth_s: R0 332,148;
            //            TESTDIS;
            //            MOD N250 332,148 0,0 :L254 STOR_MID :E'CABLE_TYPE' JU; NOP;
            //            TESTDIS_OFF;
            //            ADD N58 :J7: T3003: D: F1: R0 'GROUPE' :AC I2 332,148 330.5,148.5; NOP;
            //            pm_files_sav; ;
            //            GRI 2,2;


            //Console.WriteLine(command); // Replace with file writing if needed
            // If writing to a file instead of console:
            #endregion

            using (var writer = File.AppendText(Constants.el_ExecFilePath))
            {
                //string command = $"ADD L154 :W0.0 :FILL {X1},{Y1} {X2},{Y2};;;;NOP";
                writer.WriteLine("GRI 2.0,2;");
                writer.WriteLine("SHOW #E;");
                writer.WriteLine("TESTDIS;");

                writer.WriteLine($"ADD L154 {X1},{Y1} {X2},{Y2} ;;NOP;;");
                //writer.WriteLine($"ADD L154 72,148 348,148; ; NOP;;");
                double X3 = (X1 + X2) / 2;
                writer.WriteLine($"ADD N54 :R0 :J8 :F2 :D :T1002 :AC L154 {X1},{Y1} 'WIRENB' {X3},{Y1}; NOP;");
                //writer.WriteLine("ADD N54 :R0: J8: F2: D: T1002: AC L154 72,148 'WIRENB' 210,148; NOP;");

                writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {X1},{Y1} '#{wireGauge}' {X3+2},{Y1};");
                //writer.WriteLine("ADD N56 :R0: S10: D: J2: F2: T3006: AC L154 72,148 '#' 212,148;");

                writer.WriteLine($"ADD N59 :R0 :D :J2 :F2 :T3009 :AC L154 {X1},{Y1} 'LENGTH' {X3+8},{Y1};");
                //writer.WriteLine("ADD N59 :R0: D: J2: F2: T3009: AC L154 72,148 'LENGTH' 218,148;");

                writer.WriteLine($"ADD I2 sth_s :R0 {X1+16},{Y1};");
                //writer.WriteLine("ADD I2 sth_s: R0 88,148;");

                writer.WriteLine($"MOD N250 {X1 + 16},{Y2} 0,0 :L254 STOR_MID :E'{wireCode}' JU;NOP;");
                //writer.WriteLine("MOD N250 88,148 0,0 :L254 STOR_MID :E'CABLE_TYPE' JU; NOP;");

                writer.WriteLine("TESTDIS_OFF;");

                writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 'GROUPE' :AC I2 {X1 + 16},{Y1} {X1+16-1.5},{Y1}; NOP;");
                //writer.WriteLine("ADD N58 :J7: T3003: D: F1: R0 'GROUPE' :AC I2 88,148 86.5,148.5; NOP;");

                writer.WriteLine($"ADD I2 sth_s :R0 {X2-16},{Y2};");
                //writer.WriteLine("ADD I2 sth_s: R0 332,148;");

                writer.WriteLine("TESTDIS;");

                writer.WriteLine($"MOD N250 {X2 - 16},{Y2} 0,0 :L254 STOR_MID :E'{wireCode}' JU;NOP;");
                //writer.WriteLine("MOD N250 332,148 0,0 :L254 STOR_MID :E'CABLE_TYPE' JU; NOP;");

                writer.WriteLine("TESTDIS_OFF;");

                writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 'GROUPE' :AC I2 {X2 - 16},{Y2} {X2-16-1.5},{Y2};NOP;");
                //writer.WriteLine("ADD N58 :J7: T3003: D: F1: R0 'GROUPE' :AC I2 332,148 330.5,148.5; NOP;");
                //modOPCommand.El_Exec_Footer_Connection_Commands();
                writer.WriteLine($":GRI");
                //writer.WriteLine($"pm_view_win1_recall_n 9 ;");
                //writer.WriteLine($"set_actual_layer LAYER_ORIG ;");
                //writer.WriteLine($"pm_view_redraw; TESTDIS_OFF ;");
                //writer.WriteLine($"pm_view_grid_on;");
                writer.WriteLine($"pm_files_sav;;");
                writer.WriteLine($"GRI ELECTRE_GRID_STH;");
                //writer.WriteLine($"UNDO_END2;");
                //writer.WriteLine($"UNDO :E;");
                //writer.WriteLine("pm_files_sav ;;");
                //GRI 2,2;
                //writer.WriteLine(command);
            }
        }

        public static void Simple3PointConnection(double p1x, double p1y, double p2x, double p2y,string wireCode, string wireGauge, string c1, string c2,string fType, string tType,string fOri, string tOri)
        {
            double X1 = p1x, Y1 = p1y;
            double X3 = p2x, Y3 = p2y;
            double X2 = 0, Y2 = 0;

            if (fType == "EQU")
            {
                X2 = X1;
                Y2 = Y3;
            }
            else if (tType == "EQU")
            {
                X2 = X3;
                Y2 = Y1;
            }

            // Construct the command string
            string command = $"ADD L154 :W0.0 :FILL {X1},{Y1} {X2},{Y2} {X3},{Y3};;;;NOP";

            // Output to debug and optionally file
            //Console.WriteLine("3Point " + command);

            // If writing to a file instead of console:
            
            using (var writer = File.AppendText(Constants.el_ExecFilePath))

            {
                writer.WriteLine(command);
            }

            // Insert label mid-way between Y2 and Y3 (for vertical placement)
            double labelX = X2;
            double labelY = Y2 + (Y3 - Y2) / 2;
            InsertWireCode90(labelX, labelY, wireCode, wireGauge);
        }

        private static void InsertWireCode90(double iX, double iY, string wireCode, string wireGauge)
        {
            const double iLength = 5.0; // Adjust this value based on original VB6 scope or declaration
            string sMidCoord = $"{iX + iLength / 2},{iY}";

            using (var writer = File.AppendText(Constants.el_ExecFilePath))
            {
                //writer.WriteLine($"ADD L154 {iX},{iY} {iX + iLength},{iY};NOP;");
                writer.WriteLine($"Add N54 :D :F1 '{wireCode}' :R90 :J7 :AC L154 {sMidCoord} {iX - 1},{iY};NOP;");
                writer.WriteLine($"Add N55 :D :F1 '#{wireGauge}' :R90 :J1 :AC L154 {sMidCoord} {iX - 1},{iY};NOP;");
            }

            //Console.WriteLine($"Add N54 :D :F1 '{wireCode}' :R90 :J7 :AC L154 {sMidCoord} {iX - 1},{iY};NOP;");
            //Console.WriteLine($"Add N55 :D :F1 '#{wireGauge}' :R90 :J1 :AC L154 {sMidCoord} {iX - 1},{iY};NOP;");
        }

        public static void Simple4PointConnection_STP_ZLine(double p1x, double p1y, double p2x, double p2y, string iWireCode, string iWireGauge, string c1, string c2, string iF_Type, string iT_Type, string groupId, string wire_Length, string wire_Type, string wire_Type_Core_Num)
        {
            //double p12x, p12y, p23x, p23y;
            double X1=0,Y1=0,X2=0,Y2=0,X3=0, Y3=0, X4=0, Y4=0;
            X1 = p1x;
            Y1= p1y;
            X2 = p2x;
            Y2= p2y;
            X3 = (X1 + X2) / 2;
            X4 = X3;
            Y3 = Y1;
            Y4 = Y2;
            int wire_Type_Core_Number = Convert.ToInt16(wire_Type_Core_Num);
            //p12x = (p1x + p2x) / 2; //p1x+150;
            //p12y = p1y;//
            //p23x = p12x;
            //p23y= p12y-60;
            using (var writer = File.AppendText(Constants.el_ExecFilePath))
            {
                //writer.WriteLine($"ADD L154 {iX},{iY} {iX + iLength},{iY};NOP;");
                //writer.WriteLine(
                writer.WriteLine($"GRI 2.0, 2;");
                writer.WriteLine($"ADD L154 :W0");
                
                writer.WriteLine($"{X1},{Y1}");
                //writer.WriteLine($"74,238");
                writer.WriteLine($"{X3},{Y3}");
                //writer.WriteLine($"230,238");
                writer.WriteLine($"{X4},{Y4}");
                //writer.WriteLine($"230,172");
                writer.WriteLine($"{X2},{Y2}");
                //writer.WriteLine($"386,172");
                writer.WriteLine($";;NOP;;");
                writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {X1},{Y1} '{iWireCode}' {X1*2},{Y1} ;NOP;");
                //writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 74,238 'STP__02' 152,238 ;NOP;");
                writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {X1},{Y1} '#{iWireGauge}' {X1 * 2+2},{Y1} ;");
                //writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 74,238 '#14' 154,238 ;");
                writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {X1},{Y1} '{wire_Type_Core_Number}' {X1 * 2 + 2+8},{Y1-1} ;");
                //writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 74,238 '1' 162,237 ;");
                writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 {X1},{Y1} '{wire_Length}' {X1 * 2 + 2 + 8},{Y1} ;");
                //writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 74,238 'LLL' 162,238 ;");
                writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {X1},{Y1} '' {X1 * 2 + 2 + 8},{Y1} ;");
                //writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 74,238 '' 162,238 ;");
                writer.WriteLine($"ADD L154 :W0");
                writer.WriteLine($"{X1},{Y1-4}");
                //writer.WriteLine($"74,234");
                writer.WriteLine($"{X3-4},{Y3-4}");
                //writer.WriteLine($"226,234");
                writer.WriteLine($"{X4 - 4},{Y2 - 4}");
                //writer.WriteLine($"226,168");
                writer.WriteLine($"{X2},{Y2-4}");
                //writer.WriteLine($"386,168");
                writer.WriteLine($";;NOP;;");
                writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {X3-4},{Y2 - 4} '{iWireCode}' {X3 - 4+80},{Y2 - 4} ;NOP;");
                //writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 226,168 'STP__02' 306,168 ;NOP;");
                writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {X3 - 4},{Y2 - 4} '#{iWireGauge}' {X3 - 4 + 80+2},{Y2 - 4} ;");
                //writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 226,168 '#14' 308,168 ;");
                writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {X3 - 4},{Y2 - 4} '{wire_Type_Core_Number + 1}' {X3 - 4 + 80 + 2+8},{Y2 - 4-1} ;");
                //writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 226,168 '2' 316,167 ;");
                writer.WriteLine($"ADD N59 :R0 :D :J2 :F2 :T3009 :AC L154 {X3 - 4},{Y2 - 4} '{wire_Length}' {X3 - 4 + 80 + 2 + 8},{Y2 - 4} ;");
                //writer.WriteLine($"ADD N59 :R0 :D :J2 :F2 :T3009 :AC L154 226,168 'LLL' 316,168 ;");
                writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {X3 - 4},{Y2 - 4} '' {X3 - 4 + 80 + 2 + 8},{Y2 - 4} ;");
                //writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 226,168 '' 316,168 ;");
                writer.WriteLine($"ADD I2 sth_stp4 :R0 {X1+16},{Y1};");
                //writer.WriteLine($"ADD I2 sth_stp4 :R0 90,238;");
                writer.WriteLine($"TESTDIS;");
                writer.WriteLine($"MOD N250 {X1+16},{Y1} 0,0 :L254 STOR_MID :E'{wire_Type}' JU;NOP;");
                //writer.WriteLine($"MOD N250 90,238 0,0 :L254 STOR_MID :E'STP' JU;NOP;");
                writer.WriteLine($"MOD N250 {X1 + 16},{Y1-4} 0,0 :L254 STOR_MID :E'{wire_Type}' JU;NOP;");
                //writer.WriteLine($"MOD N250 90,234 0,0 :L254 STOR_MID :E'STP' JU;NOP;");
                writer.WriteLine($"TESTDIS_OFF;");
                writer.WriteLine($":RAW");
                writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {X1 + 16},{Y1} {X1 + 16-1.5},{Y1+0.5};NOP;");
                //writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '004' :AC I2 90,238 88.5,238.5;NOP;");
                writer.WriteLine($"MOD N253 {X1 + 16},{Y1} 0,0 STOR_MID :E'{groupId}' JU;NOP;");
                //writer.WriteLine($"MOD N253 90,238 0,0 STOR_MID :E'004' JU;NOP;");
                writer.WriteLine($":GRI");
                writer.WriteLine($"ADD I2 sth_stp4 :R0 {X2-16},{Y2};");
                //writer.WriteLine($"ADD I2 sth_stp4 :R0 370,172;");
                writer.WriteLine($"TESTDIS;");
                writer.WriteLine($"MOD N250 {X2 - 16},{Y2} 0,0 :L254 STOR_MID :E'{wire_Type}' JU;NOP;");
                //writer.WriteLine($"MOD N250 370,172 0,0 :L254 STOR_MID :E'STP' JU;NOP;");
                writer.WriteLine($"MOD N250 {X2 - 16},{Y2-4} 0,0 :L254 STOR_MID :E'{wire_Type}' JU;NOP;");
                //writer.WriteLine($"MOD N250 370,168 0,0 :L254 STOR_MID :E'STP' JU;NOP;");
                writer.WriteLine($"TESTDIS_OFF;");
                writer.WriteLine($":RAW");
                writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {X2 - 16},{Y2} {X2 - 16-1.5},{Y2+0.5};NOP;");
                //writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '004' :AC I2 370,172 368.5,172.5;NOP;");
                writer.WriteLine($"MOD N253 {X2 - 16},{Y2} 0,0 STOR_MID :E'{groupId}' JU; NOP;");
                //writer.WriteLine($"MOD N253 370,172 0,0 STOR_MID: E'004' JU; NOP;");
                //modOPCommand.El_Exec_Footer_Connection_Commands();
                writer.WriteLine($":GRI");
                //writer.WriteLine($"pm_view_win1_recall_n 9 ;");
                //writer.WriteLine($"set_actual_layer LAYER_ORIG ;");
                //writer.WriteLine($"pm_view_redraw; TESTDIS_OFF ;");
                //writer.WriteLine($"pm_view_grid_on;");
                writer.WriteLine($"pm_files_sav;;");
                writer.WriteLine($"GRI ELECTRE_GRID_STH;");
                //writer.WriteLine($"UNDO_END2;");
                //writer.WriteLine($"UNDO :E;");
                //MessageBox.Show("Simple4PointConnection_STP_ZLine Completed");
            }
        }

        public static void Simple4PointConnection_TP_ZLine(double p1x, double p1y, double p2x, double p2y, string iWireCode, string iWireGauge, string c1, string c2, string iF_Type, string iT_Type, string groupId, string wire_Length, string wire_Type, string wire_Type_Core_Num)
        {
            //double p12x, p12y, p23x, p23y;
            double X1 = 0, Y1 = 0, X2 = 0, Y2 = 0, X3 = 0, Y3 = 0, X4 = 0, Y4 = 0;string twisted_wire_symbol = string.Empty;
            X1 = p1x;
            Y1 = p1y;
            X2 = p2x;
            Y2 = p2y;
            X3 = (X1 + X2) / 2;
            X4 = X3;
            Y3 = Y1;
            Y4 = Y2;
            int wire_Type_Core_Number = Convert.ToInt16(wire_Type_Core_Num);
            if (wire_Type.Equals("TP")) { twisted_wire_symbol = "sth_tp4"; }
            else if (wire_Type.Equals("QUADRAX")){ twisted_wire_symbol = "sth_quadrax4"; }
            //)//Quadrax,sth_quadrax4,TP,sth_tp4
            //p12x = (p1x + p2x) / 2; //p1x+150;
            //p12y = p1y;//
            //p23x = p12x;
            //p23y= p12y-60;
            using (var writer = File.AppendText(Constants.el_ExecFilePath))
            {
                //writer.WriteLine($"ADD L154 {iX},{iY} {iX + iLength},{iY};NOP;");
                //writer.WriteLine(
                writer.WriteLine($"GRI 2.0, 2;");
                writer.WriteLine($"ADD L154 :W0");

                writer.WriteLine($"{X1},{Y1}");
                //writer.WriteLine($"74,238");
                writer.WriteLine($"{X3},{Y3}");
                //writer.WriteLine($"230,238");
                writer.WriteLine($"{X4},{Y4}");
                //writer.WriteLine($"230,172");
                writer.WriteLine($"{X2},{Y2}");
                //writer.WriteLine($"386,172");
                writer.WriteLine($";;NOP;;");
                writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {X1},{Y1} '{iWireCode}' {X1 * 2},{Y1} ;NOP;");
                //writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 74,238 'STP__02' 152,238 ;NOP;");
                writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {X1},{Y1} '#{iWireGauge}' {X1 * 2 + 2},{Y1} ;");
                //writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 74,238 '#14' 154,238 ;");
                writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {X1},{Y1} '{wire_Type_Core_Number}' {X1 * 2 + 2 + 8},{Y1 - 1} ;");
                //writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 74,238 '1' 162,237 ;");
                writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 {X1},{Y1} '{wire_Length}' {X1 * 2 + 2 + 8},{Y1} ;");
                //writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 74,238 'LLL' 162,238 ;");
                writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {X1},{Y1} '' {X1 * 2 + 2 + 8},{Y1} ;");
                //writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 74,238 '' 162,238 ;");
                writer.WriteLine($"ADD L154 :W0");
                writer.WriteLine($"{X1},{Y1 - 4}");
                //writer.WriteLine($"74,234");
                writer.WriteLine($"{X3 - 4},{Y3 - 4}");
                //writer.WriteLine($"226,234");
                writer.WriteLine($"{X4 - 4},{Y2 - 4}");
                //writer.WriteLine($"226,168");
                writer.WriteLine($"{X2},{Y2 - 4}");
                //writer.WriteLine($"386,168");
                writer.WriteLine($";;NOP;;");
                writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {X3 - 4},{Y2 - 4} '{iWireCode}' {X3 - 4 + 80},{Y2 - 4} ;NOP;");
                //writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 226,168 'STP__02' 306,168 ;NOP;");
                writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {X3 - 4},{Y2 - 4} '#{iWireGauge}' {X3 - 4 + 80 + 2},{Y2 - 4} ;");
                //writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 226,168 '#14' 308,168 ;");
                writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {X3 - 4},{Y2 - 4} '{wire_Type_Core_Number + 1}' {X3 - 4 + 80 + 2 + 8},{Y2 - 4 - 1} ;");
                //writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 226,168 '2' 316,167 ;");
                writer.WriteLine($"ADD N59 :R0 :D :J2 :F2 :T3009 :AC L154 {X3 - 4},{Y2 - 4} '{wire_Length}' {X3 - 4 + 80 + 2 + 8},{Y2 - 4} ;");
                //writer.WriteLine($"ADD N59 :R0 :D :J2 :F2 :T3009 :AC L154 226,168 'LLL' 316,168 ;");
                writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {X3 - 4},{Y2 - 4} '' {X3 - 4 + 80 + 2 + 8},{Y2 - 4} ;");
                //writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 226,168 '' 316,168 ;");
                writer.WriteLine($"ADD I2 {twisted_wire_symbol} :R0 {X1 + 16},{Y1};");
                //writer.WriteLine($"ADD I2 sth_stp4 :R0 90,238;");
                writer.WriteLine($"TESTDIS;");
                writer.WriteLine($"MOD N250 {X1 + 16},{Y1} 0,0 :L254 STOR_MID :E'{wire_Type}' JU;NOP;");
                //writer.WriteLine($"MOD N250 90,238 0,0 :L254 STOR_MID :E'STP' JU;NOP;");
                writer.WriteLine($"MOD N250 {X1 + 16},{Y1 - 4} 0,0 :L254 STOR_MID :E'{wire_Type}' JU;NOP;");
                //writer.WriteLine($"MOD N250 90,234 0,0 :L254 STOR_MID :E'STP' JU;NOP;");
                writer.WriteLine($"TESTDIS_OFF;");
                writer.WriteLine($":RAW");
                writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {X1 + 16},{Y1} {X1 + 16 - 1.5},{Y1 + 0.5};NOP;");
                //writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '004' :AC I2 90,238 88.5,238.5;NOP;");
                writer.WriteLine($"MOD N253 {X1 + 16},{Y1} 0,0 STOR_MID :E'{groupId}' JU;NOP;");
                //writer.WriteLine($"MOD N253 90,238 0,0 STOR_MID :E'004' JU;NOP;");
                writer.WriteLine($":GRI");
                writer.WriteLine($"ADD I2 {twisted_wire_symbol} :R0 {X2 - 16},{Y2};");
                //writer.WriteLine($"ADD I2 sth_stp4 :R0 370,172;");
                writer.WriteLine($"TESTDIS;");
                writer.WriteLine($"MOD N250 {X2 - 16},{Y2} 0,0 :L254 STOR_MID :E'{wire_Type}' JU;NOP;");
                //writer.WriteLine($"MOD N250 370,172 0,0 :L254 STOR_MID :E'STP' JU;NOP;");
                writer.WriteLine($"MOD N250 {X2 - 16},{Y2 - 4} 0,0 :L254 STOR_MID :E'{wire_Type}' JU;NOP;");
                //writer.WriteLine($"MOD N250 370,168 0,0 :L254 STOR_MID :E'STP' JU;NOP;");
                writer.WriteLine($"TESTDIS_OFF;");
                writer.WriteLine($":RAW");
                writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {X2 - 16},{Y2} {X2 - 16 - 1.5},{Y2 + 0.5};NOP;");
                //writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '004' :AC I2 370,172 368.5,172.5;NOP;");
                writer.WriteLine($"MOD N253 {X2 - 16},{Y2} 0,0 STOR_MID :E'{groupId}' JU; NOP;");
                //writer.WriteLine($"MOD N253 370,172 0,0 STOR_MID: E'004' JU; NOP;");
                //modOPCommand.El_Exec_Footer_Connection_Commands();
                writer.WriteLine($":GRI");
                //writer.WriteLine($"pm_view_win1_recall_n 9 ;");
                //writer.WriteLine($"set_actual_layer LAYER_ORIG ;");
                //writer.WriteLine($"pm_view_redraw; TESTDIS_OFF ;");
                //writer.WriteLine($"pm_view_grid_on;");
                writer.WriteLine($"pm_files_sav;;");
                writer.WriteLine($"GRI ELECTRE_GRID_STH;");
                //writer.WriteLine($"UNDO_END2;");
                //writer.WriteLine($"UNDO :E;");
                //MessageBox.Show("Simple4PointConnection_TP_ZLine Completed");
            }

        }

        #region //Commented old Simple4PointConnection_ZLine_Optimized code on 19 november, 2025
        //public static void Simple4PointConnection_ZLine_Optimized(double p1x, double p1y, double p2x, double p2y, string iWireCode, string iWireGauge, string c1, string c2, string iF_Type, string iT_Type, string groupId, string wire_Length, string wire_Type, string wire_Type_Core_Number)
        //{
        //    //double p12x, p12y, p23x, p23y;
        //    double X1 = 0, Y1 = 0, X2 = 0, Y2 = 0, X3 = 0, Y3 = 0, X4 = 0, Y4 = 0;
        //    //Constants.Remove_Wire_OverLap_Add_Count = Constants.Remove_Wire_OverLap_Add_Count + 2;
        //    X1 = p1x;
        //    Y1 = p1y;
        //    X2 = p2x;
        //    Y2 = p2y;
        //    X3 = (X1 + X2) / 2;
        //    //X3 = ((X1 + X2) / 2) + Constants.Remove_Wire_OverLap_Add_Count;
        //    X4 = X3;
        //    Y3 = Y1;
        //    Y4 = Y2;
        //    string wire_symbol = string.Empty;
        //    #region//Symbols added for each wire type
        //    if (wire_Type.Equals("86A9S") || wire_Type.Equals("86A9US") || wire_Type.Equals("86A9SS") || wire_Type.Equals("S") || wire_Type.Equals("S0") || wire_Type.Equals("S00"))
        //    { wire_symbol = "sth_s"; }
        //    else if (wire_Type.Equals("SS")) { wire_symbol = "sth_ss"; }
        //    else if (wire_Type.Equals("TP")) { wire_symbol = "sth_tp4"; }
        //    else if (wire_Type.Equals("STP")) { wire_symbol = "sth_stp4"; }
        //    else if (wire_Type.Equals("COAX")) { wire_symbol = "sth_coax"; }
        //    else if (wire_Type.Equals("TRIAX")) { wire_symbol = "sth_triax"; }
        //    else if (wire_Type.Equals("QUADRAX")) { wire_symbol = "sth_quadrax4"; }
        //    else { }
        //    #endregion
        //    using (writer = File.AppendText(Constants.el_ExecFilePath))
        //    {
        //        //writer.WriteLine($"ADD L154 {iX},{iY} {iX + iLength},{iY};NOP;");
        //        //writer.WriteLine(
        //        writer.WriteLine($"GRI 0.5, 2;");
        //        writer.WriteLine($"ADD L154 :W0");

        //        writer.WriteLine($"{X1},{Y1}");
        //        //writer.WriteLine($"74,238");
        //        writer.WriteLine($"{X3},{Y3}");
        //        //writer.WriteLine($"230,238");
        //        writer.WriteLine($"{X4},{Y4}");
        //        //writer.WriteLine($"230,172");
        //        writer.WriteLine($"{X2},{Y2}");
        //        //writer.WriteLine($"386,172");
        //        writer.WriteLine($";;NOP;;");
        //        writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {X1},{Y1} '{iWireCode}' {X1 + 48},{Y1} ;NOP;");
        //        //writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 74,238 'STP__02' 152,238 ;NOP;");
        //        writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {X1},{Y1} '#{iWireGauge}' {X1 + 48 + 2},{Y1} ;");
        //        //writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 74,238 '#14' 154,238 ;");
        //        writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {X1},{Y1} '{wire_Type_Core_Number}' {X1 + 48 + 2 + 8},{Y1 - 1} ;");
        //        //writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 74,238 '1' 162,237 ;");
        //        writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 {X1},{Y1} '{wire_Length}' {X1 + 48 + 2 + 8},{Y1} ;");
        //        //writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 74,238 'LLL' 162,238 ;");
        //        writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {X1},{Y1} '' {X1 + 48 + 2 + 8},{Y1} ;");
        //        //writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 74,238 '' 162,238 ;");
        //        #region//Extra Z line Removed 
        //        //writer.WriteLine($"ADD L154 :W0");
        //        //writer.WriteLine($"{X1},{Y1 - 4}");
        //        ////writer.WriteLine($"74,234");
        //        //writer.WriteLine($"{X3 - 4},{Y3 - 4}");
        //        ////writer.WriteLine($"226,234");
        //        //writer.WriteLine($"{X4 - 4},{Y2 - 4}");
        //        ////writer.WriteLine($"226,168");
        //        //writer.WriteLine($"{X2},{Y2 - 4}");
        //        ////writer.WriteLine($"386,168");
        //        //writer.WriteLine($";;NOP;;");
        //        //writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {X3 - 4},{Y2 - 4} '{iWireCode}' {X3 - 4 + 80},{Y2 - 4} ;NOP;");
        //        //writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 226,168 'STP__02' 306,168 ;NOP;");
        //        //writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {X3 - 4},{Y2 - 4} '#{iWireGauge}' {X3 - 4 + 80 + 2},{Y2 - 4} ;");
        //        //writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 226,168 '#14' 308,168 ;");
        //        //writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {X3 - 4},{Y2 - 4} '{groupId}' {X3 - 4 + 80 + 2 + 8},{Y2 - 4 - 1} ;");
        //        //writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 226,168 '2' 316,167 ;");
        //        //writer.WriteLine($"ADD N59 :R0 :D :J2 :F2 :T3009 :AC L154 {X3 - 4},{Y2 - 4} '{wire_Length}' {X3 - 4 + 80 + 2 + 8},{Y2 - 4} ;");
        //        //writer.WriteLine($"ADD N59 :R0 :D :J2 :F2 :T3009 :AC L154 226,168 'LLL' 316,168 ;");
        //        //writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {X3 - 4},{Y2 - 4} '' {X3 - 4 + 80 + 2 + 8},{Y2 - 4} ;");
        //        #endregion
        //        #region//Destination end
        //        writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {X2 - 25},{Y2} '{iWireCode}' {X2 - 25 * 2},{Y2} ;NOP;");
        //        //writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 74,238 'STP__02' 152,238 ;NOP;");
        //        writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {X2 - 25},{Y2} '#{iWireGauge}' {X2 - 25 * 2 + 2},{Y2} ;");
        //        //writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 74,238 '#14' 154,238 ;");
        //        writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {X2 - 25},{Y2} '{wire_Type_Core_Number}' {X2 - 25 * 2 + 2 + 8},{Y2 - 1} ;");
        //        //writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 74,238 '1' 162,237 ;");
        //        writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 {X2 - 25},{Y2} '{wire_Length}' {X2 - 25 * 2 + 2 + 8},{Y2} ;");
        //        //writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 74,238 'LLL' 162,238 ;");
        //        writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {X2 - 25},{Y2} '' {X2 - 25 * 2 + 2 + 8},{Y2} ;");

        //        #endregion
        //        #region//Nathali instructions executed for wire type hidden attributes
        //        //ADD I2 sth_s: R0 132,772;
        //        //TESTDIS;
        //        //MOD N250 130,773.5 0,0 :L254 STOR_MID :E'86A9S' JU; NOP;
        //        //TESTDIS_OFF;
        //        //ADD N58 :J7: T3003: D: F1: R0 '001' :AC I2 132,772 130.5,772.5; NOP;
        //        #endregion
        //        if (!Constants.lst_WireCodes_Info_Processed.Contains(iWireCode))
        //        {
        //            writer.WriteLine($"ADD I2 {wire_symbol} :R0 {X1 + 16},{Y1};");
        //            writer.WriteLine($"TESTDIS;");
        //            writer.WriteLine($"MOD N250 {X1 + 16-2},{Y1+1.5} 0,0 :L254 STOR_MID :E'{wire_Type}' JU; NOP;");//wire type display
        //            writer.WriteLine($"TESTDIS_OFF;");
        //            writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {X1 + 16},{Y1} {X1 + 16 - 1.5},{Y1 + 0.5};NOP;");
        //            //writer.WriteLine($"ADD N254 :J7 :T3007 :D :F1 :R0 '{wire_Type}' :AC I2 {X1 + 16},{Y1} {X1 + 16 - 1.5+16},{Y1 + 0.5};NOP;");//wire type display

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
        //            writer.WriteLine($"MOD N250 {X2 - 16-2},{Y2+1.5} 0,0 :L254 STOR_MID :E'{wire_Type}' JU; NOP;");//wire type display
        //            writer.WriteLine($"TESTDIS_OFF;");
        //            writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {X2 - 16},{Y2} {X2 - 16 - 1.5},{Y2 + 0.5};NOP;");
        //            //writer.WriteLine($"ADD N254 :J7 :T3007 :D :F1 :R0 '{wire_Type}' :AC I2 {X2 - 16},{Y2} {X2 - 16 - 1.5 - 16},{Y2 + 0.5};NOP;");//wire type display

        //            if (wire_Type.Equals("SS") || wire_Type.Equals("TP") || wire_Type.Equals("STP") || wire_Type.Equals("COAX") || wire_Type.Equals("TRIAX") || wire_Type.Equals("QUADRAX"))
        //            {
        //                writer.WriteLine($"MOD N253 {X2 - 16},{Y2} 0,0 STOR_MID :E'{groupId}' JU; NOP;");
        //            }
        //        }
        //        ////writer.WriteLine($"MOD N253 370,172 0,0 STOR_MID: E'004' JU; NOP;");
        //        #endregion
        //        //modOPCommand.El_Exec_Footer_Connection_Commands();
        //        writer.WriteLine($":GRI");
        //        writer.WriteLine($"pm_files_sav;;");
        //        writer.WriteLine($"GRI ELECTRE_GRID_STH;");

        //        if (!Constants.lst_WireCodes_Info_Processed.Contains(iWireCode))
        //            Constants.lst_WireCodes_Info_Processed.Add(iWireCode);
        //    }

        //}

        //*****************************************************************************
        #endregion
        public static void Simple4PointConnection_Mono_ZLine(double p1x, double p1y, double p2x, double p2y, string iWireCode, string iWireGauge, string c1, string c2, string iF_Type, string iT_Type, string groupId, string wire_Length, string wire_Type, string wire_Type_Core_Number)
        {
            //double p12x, p12y, p23x, p23y;
            double X1 = 0, Y1 = 0, X2 = 0, Y2 = 0, X3 = 0, Y3 = 0, X4 = 0, Y4 = 0;
            X1 = p1x;
            Y1 = p1y;
            X2 = p2x;
            Y2 = p2y;
            X3 = (X1 + X2) / 2;
            X4 = X3;
            Y3 = Y1;
            Y4 = Y2;
            string mono_wire_symbol = string.Empty;
            if (wire_Type.Equals("86A9S") || wire_Type.Equals("86A9SS") || wire_Type.Equals("S") || wire_Type.Equals("S0") || wire_Type.Equals("S00"))
            { mono_wire_symbol = "sth_s"; }
            else if(wire_Type.Equals("SS")) { mono_wire_symbol = "sth_ss"; }
            else if (wire_Type.Equals("COAX")) { mono_wire_symbol = "sth_coax"; }
            else if (wire_Type.Equals("TRIAX")) { mono_wire_symbol = "sth_triax"; }
            //coax,sth_coax,triax,sth_triax
            //p12x = (p1x + p2x) / 2; //p1x+150;
            //p12y = p1y;//
            //p23x = p12x;
            //p23y= p12y-60;
            using (var writer = File.AppendText(Constants.el_ExecFilePath))
            {
                //writer.WriteLine($"ADD L154 {iX},{iY} {iX + iLength},{iY};NOP;");
                //writer.WriteLine(
                writer.WriteLine($"GRI 2.0, 2;");
                writer.WriteLine($"ADD L154 :W0");

                writer.WriteLine($"{X1},{Y1}");
                //writer.WriteLine($"74,238");
                writer.WriteLine($"{X3},{Y3}");
                //writer.WriteLine($"230,238");
                writer.WriteLine($"{X4},{Y4}");
                //writer.WriteLine($"230,172");
                writer.WriteLine($"{X2},{Y2}");
                //writer.WriteLine($"386,172");
                writer.WriteLine($";;NOP;;");
                writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {X1},{Y1} '{iWireCode}' {X1 * 2},{Y1} ;NOP;");
                //writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 74,238 'STP__02' 152,238 ;NOP;");
                writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {X1},{Y1} '#{iWireGauge}' {X1 * 2 + 2},{Y1} ;");
                //writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 74,238 '#14' 154,238 ;");
                writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {X1},{Y1} '{wire_Type_Core_Number}' {X1 * 2 + 2 + 8},{Y1 - 1} ;");
                //writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 74,238 '1' 162,237 ;");
                writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 {X1},{Y1} '{wire_Length}' {X1 * 2 + 2 + 8},{Y1} ;");
                //writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 74,238 'LLL' 162,238 ;");
                writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {X1},{Y1} '' {X1 * 2 + 2 + 8},{Y1} ;");
                //writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 74,238 '' 162,238 ;");
                #region//Extra Z line Removed 
                //writer.WriteLine($"ADD L154 :W0");
                //writer.WriteLine($"{X1},{Y1 - 4}");
                ////writer.WriteLine($"74,234");
                //writer.WriteLine($"{X3 - 4},{Y3 - 4}");
                ////writer.WriteLine($"226,234");
                //writer.WriteLine($"{X4 - 4},{Y2 - 4}");
                ////writer.WriteLine($"226,168");
                //writer.WriteLine($"{X2},{Y2 - 4}");
                ////writer.WriteLine($"386,168");
                //writer.WriteLine($";;NOP;;");
                //writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {X3 - 4},{Y2 - 4} '{iWireCode}' {X3 - 4 + 80},{Y2 - 4} ;NOP;");
                ////writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 226,168 'STP__02' 306,168 ;NOP;");
                //writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {X3 - 4},{Y2 - 4} '#{iWireGauge}' {X3 - 4 + 80 + 2},{Y2 - 4} ;");
                ////writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 226,168 '#14' 308,168 ;");
                //writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {X3 - 4},{Y2 - 4} '{groupId}' {X3 - 4 + 80 + 2 + 8},{Y2 - 4 - 1} ;");
                ////writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 226,168 '2' 316,167 ;");
                //writer.WriteLine($"ADD N59 :R0 :D :J2 :F2 :T3009 :AC L154 {X3 - 4},{Y2 - 4} '{wire_Length}' {X3 - 4 + 80 + 2 + 8},{Y2 - 4} ;");
                ////writer.WriteLine($"ADD N59 :R0 :D :J2 :F2 :T3009 :AC L154 226,168 'LLL' 316,168 ;");
                //writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {X3 - 4},{Y2 - 4} '' {X3 - 4 + 80 + 2 + 8},{Y2 - 4} ;");
                #endregion
                //writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 226,168 '' 316,168 ;");
                writer.WriteLine($"ADD I2 {mono_wire_symbol} :R0 {X1 + 16},{Y1};");
                writer.WriteLine($"TESTDIS;");
                //writer.WriteLine($"ADD I2 sth_s :R0 {X1 + 16},{Y1};");
                //writer.WriteLine($"ADD I2 sth_stp4 :R0 90,238;");
                //writer.WriteLine($"MOD N250 {X1 + 16},{Y1} 0,0 :L254 STOR_MID :E'SS' JU;NOP;");
                //writer.WriteLine($"MOD N250 90,238 0,0 :L254 STOR_MID :E'STP' JU;NOP;");
                //writer.WriteLine($"MOD N250 {X1 + 16},{Y1 - 4} 0,0 :L254 STOR_MID :E'SS' JU;NOP;");
                //writer.WriteLine($"MOD N250 90,234 0,0 :L254 STOR_MID :E'STP' JU;NOP;");
                //writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {X1 + 16},{Y1} {X1 + 16 - 1.5},{Y1 + 0.5};NOP;");
                //writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '004' :AC I2 90,238 88.5,238.5;NOP;");
                writer.WriteLine($"TESTDIS_OFF;");
                writer.WriteLine($":RAW");
                writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {X1 + 16},{Y1} {X1 + 16 - 1.5},{Y1 + 0.5};NOP;");
                if (wire_Type.Equals("SS") || wire_Type.Equals("COAX") || wire_Type.Equals("TRIAX"))
                {
                    writer.WriteLine($"MOD N253 {X1 + 16},{Y1} 0,0 STOR_MID :E'{groupId}' JU;NOP;");
                }
                //writer.WriteLine($"MOD N253 90,238 0,0 STOR_MID :E'004' JU;NOP;");
                writer.WriteLine($":GRI");
                #region//Extra Z line symbol removed
                //writer.WriteLine($"ADD I2 {mono_wire_symbol} :R0 {X2 - 16},{Y2};");
                ////writer.WriteLine($"ADD I2 sth_stp4 :R0 370,172;");
                //writer.WriteLine($"TESTDIS;");
                ////writer.WriteLine($"ADD I2 sth_s :R0 {X2 - 16},{Y2};");
                //////writer.WriteLine($"ADD I2 sth_stp4 :R0 370,172;");
                ////writer.WriteLine($"MOD N250 {X2 - 16},{Y2} 0,0 :L254 STOR_MID :E'SS' JU;NOP;");
                ////writer.WriteLine($"MOD N250 370,172 0,0 :L254 STOR_MID :E'STP' JU;NOP;");
                ////writer.WriteLine($"MOD N250 {X2 - 16},{Y2 - 4} 0,0 :L254 STOR_MID :E'SS' JU;NOP;");
                ////writer.WriteLine($"MOD N250 370,168 0,0 :L254 STOR_MID :E'STP' JU;NOP;");
                ////writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {X2 - 16},{Y2} {X2 - 16 - 1.5},{Y2 + 0.5};NOP;");
                ////writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '004' :AC I2 370,172 368.5,172.5;NOP;");
                //writer.WriteLine($"TESTDIS_OFF;");
                //writer.WriteLine($":RAW");
                //writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {X2 - 16},{Y2} {X2 - 16 - 1.5},{Y2 + 0.5};NOP;");
                //if (wire_Type.Equals("SS"))
                //{
                //    writer.WriteLine($"MOD N253 {X2 - 16},{Y2} 0,0 STOR_MID :E'{groupId}' JU; NOP;");
                //}
                //writer.WriteLine($"MOD N253 370,172 0,0 STOR_MID: E'004' JU; NOP;");
                #endregion
                //modOPCommand.El_Exec_Footer_Connection_Commands();
                writer.WriteLine($":GRI");
                //writer.WriteLine($"pm_view_win1_recall_n 9 ;");
                //writer.WriteLine($"set_actual_layer LAYER_ORIG ;");
                //writer.WriteLine($"pm_view_redraw; TESTDIS_OFF ;");
                //writer.WriteLine($"pm_view_grid_on;");
                writer.WriteLine($"pm_files_sav;;");
                writer.WriteLine($"GRI ELECTRE_GRID_STH;");
                //writer.WriteLine($"UNDO_END2;");
                //writer.WriteLine($"UNDO :E;");
                //MessageBox.Show("Simple4PointConnection_Mono_ZLine Completed");
            }

        }

        #region //Commented old Simple2PointConnection_TP_StraightLine code on 19 november, 2025
        //public static void Simple2PointConnection_STP_StraightLine(double p1x, double p1y, double p2x, double p2y, string iWireCode, string iWireGauge, string c1, string c2, string iF_Type, string iT_Type, string groupId, string wire_Length, string wire_Type, string wire_Type_Core_Num)
        //{
        //    //double p12x, p12y, p23x, p23y;
        //    //p12x = p1x + 150;
        //    //p12y = p1y;
        //    //p23x = p12x;
        //    //p23y = p12y - 60;
        //    double X3 = (p1x + p2x) / 2;
        //    int wire_Type_Core_Number = Convert.ToInt16(wire_Type_Core_Num);
        //    using (writer = File.AppendText(Constants.el_ExecFilePath))
        //    {
        //        //writer.WriteLine($"ADD L154 {iX},{iY} {iX + iLength},{iY};NOP;");
        //        //writer.WriteLine(
        //        writer.WriteLine($"GRI 2.0, 2;");
        //        writer.WriteLine($"ADD L154 :W0 {p1x},{p1y} {p2x},{p2y} ;;NOP;;");
        //        writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {p1x},{p1y} '{iWireCode}' {p1x * 2},{p1y} ;NOP;");
        //        //writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 74,238 'STP__02' 152,238 ;NOP;");
        //        writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {p1x},{p1y} '#{iWireGauge}' {p1x * 2 + 2},{p1y} ;");
        //        //writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 74,238 '#14' 154,238 ;");
        //        writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {p1x},{p1y} '{wire_Type_Core_Number}' {p1x * 2 + 2 + 8},{p1y - 1} ;");
        //        //writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 74,238 '1' 162,237 ;");
        //        writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 {p1x},{p1y} '{wire_Length}' {p1x * 2 + 2 + 8},{p1y} ;");
        //        //writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 74,238 'LLL' 162,238 ;");
        //        writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {p1x},{p1y} '' {p1x * 2 + 2 + 8},{p1y} ;");
        //        //writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 74,238 '' 162,238 ;");

        //        writer.WriteLine($"ADD L154 :W0");
        //        writer.WriteLine($"{p1x},{p1y - 4}");
        //        writer.WriteLine($"{p2x},{p2y - 4}");
        //        //writer.WriteLine($"386,168");
        //        writer.WriteLine($";;NOP;;");
        //        writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {p1x},{p1y - 4} '{iWireCode}' {X3},{p2y - 4} ;NOP;");
        //        //writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 226,168 'STP__02' 306,168 ;NOP;");
        //        writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {p1x},{p1y - 4} '#{iWireGauge}' {X3 + 2},{p2y - 4} ;");
        //        //writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 226,168 '#14' 308,168 ;");
        //        writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {p1x},{p2y - 4} '{wire_Type_Core_Number +1}' {X3 + 2 + 8},{p2y - 4 - 1} ;");
        //        //writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 226,168 '2' 316,167 ;");
        //        writer.WriteLine($"ADD N59 :R0 :D :J2 :F2 :T3009 :AC L154 {p1x},{p2y - 4} '{wire_Length}' {X3 + 2 + 8},{p2y - 4} ;");
        //        //writer.WriteLine($"ADD N59 :R0 :D :J2 :F2 :T3009 :AC L154 226,168 'LLL' 316,168 ;");
        //        writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {p1x},{p2y - 4} '' {X3 + 2 + 8},{p2y - 4} ;");
        //        //writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 226,168 '' 316,168 ;");
        //        writer.WriteLine($"ADD I2 sth_stp4 :R0 {p1x + 16},{p1y};");
        //        //writer.WriteLine($"ADD I2 sth_stp4 :R0 90,238;");
        //        writer.WriteLine($"TESTDIS;");
        //        writer.WriteLine($"MOD N250 {p1x + 16},{p1y} 0,0 :L254 STOR_MID :E'{wire_Type}' JU;NOP;");
        //        //writer.WriteLine($"MOD N250 90,238 0,0 :L254 STOR_MID :E'STP' JU;NOP;");
        //        writer.WriteLine($"MOD N250 {p1x + 16},{p1y - 4} 0,0 :L254 STOR_MID :E'{wire_Type}' JU;NOP;");
        //        //writer.WriteLine($"MOD N250 90,234 0,0 :L254 STOR_MID :E'STP' JU;NOP;");
        //        writer.WriteLine($"TESTDIS_OFF;");
        //        writer.WriteLine($":RAW");
        //        writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {p1x + 16},{p1y} {p1x + 16 - 1.5},{p1y + 0.5};NOP;");
        //        //writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '004' :AC I2 90,238 88.5,238.5;NOP;");
        //        writer.WriteLine($"MOD N253 {p1x + 16},{p1y} 0,0 STOR_MID :E'{groupId}' JU;NOP;");
        //        //writer.WriteLine($"MOD N253 90,238 0,0 STOR_MID :E'004' JU;NOP;");
        //        writer.WriteLine($":GRI");
        //        writer.WriteLine($"ADD I2 sth_stp4 :R0 {p2x - 16},{p2y};");
        //        //writer.WriteLine($"ADD I2 sth_stp4 :R0 370,172;");
        //        writer.WriteLine($"TESTDIS;");
        //        writer.WriteLine($"MOD N250 {p2x - 16},{p2y} 0,0 :L254 STOR_MID :E'{wire_Type}' JU;NOP;");
        //        //writer.WriteLine($"MOD N250 370,172 0,0 :L254 STOR_MID :E'STP' JU;NOP;");
        //        writer.WriteLine($"MOD N250 {p2x - 16},{p2y - 4} 0,0 :L254 STOR_MID :E'{wire_Type}' JU;NOP;");
        //        //writer.WriteLine($"MOD N250 370,168 0,0 :L254 STOR_MID :E'STP' JU;NOP;");
        //        writer.WriteLine($"TESTDIS_OFF;");
        //        writer.WriteLine($":RAW");
        //        writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {p2x - 16},{p2y} {p2x - 16 - 1.5},{p2y + 0.5};NOP;");
        //        //writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '004' :AC I2 370,172 368.5,172.5;NOP;");
        //        writer.WriteLine($"MOD N253 {p2x - 16},{p2y} 0,0 STOR_MID :E'{groupId}' JU; NOP;");
        //        //writer.WriteLine($"MOD N253 370,172 0,0 STOR_MID: E'004' JU; NOP;");
        //        writer.WriteLine($":GRI");
        //        //writer.WriteLine($"pm_view_win1_recall_n 9 ;");
        //        //writer.WriteLine($"set_actual_layer LAYER_ORIG ;");
        //        //writer.WriteLine($"pm_view_redraw; TESTDIS_OFF ;");
        //        //writer.WriteLine($"pm_view_grid_on;");
        //        writer.WriteLine($"pm_files_sav;;");
        //        writer.WriteLine($"GRI ELECTRE_GRID_STH;");
        //        //writer.WriteLine($"UNDO_END2;");
        //        //writer.WriteLine($"UNDO :E;");
        //        //MessageBox.Show("Simple2PointConnection_STP_StraightLine Completed");
        //    }

        //}
        #endregion

        #region //Commented old Simple2PointConnection_TP_StraightLine code on 19 november, 2025
        //public static void Simple2PointConnection_TP_StraightLine(double p1x, double p1y, double p2x, double p2y, string iWireCode, string iWireGauge, string c1, string c2, string iF_Type, string iT_Type,string groupId, string wire_Length, string wire_Type, string wire_Type_Core_Num)
        //{
        //    //double p12x, p12y, p23x, p23y;
        //    //p12x = p1x + 150;
        //    //p12y = p1y;
        //    //p23x = p12x;
        //    //p23y = p12y - 60;
        //    string twisted_wire_symbol = string.Empty;
        //    if (wire_Type.Equals("TP")) { twisted_wire_symbol = "sth_tp4"; }
        //    else if (wire_Type.Equals("QUADRAX")) { twisted_wire_symbol = "sth_quadrax4"; }
        //    //)//Quadrax,sth_quadrax4,TP,sth_tp4
        //    double X3 = (p1x+p2x)/2;
        //    int wire_Type_Core_Number = Convert.ToInt16(wire_Type_Core_Num);
        //    using (writer = File.AppendText(Constants.el_ExecFilePath))
        //    {
        //        //writer.WriteLine($"ADD L154 {iX},{iY} {iX + iLength},{iY};NOP;");
        //        //writer.WriteLine(
        //        writer.WriteLine($"GRI 2.0, 2;");
        //        writer.WriteLine($"ADD L154 :W0 {p1x},{p1y} {p2x},{p2y} ;;NOP;;");
        //        #region
        //        //writer.WriteLine($"{p1x},{p1y}");
        //        //writer.WriteLine($"74,238");
        //        //writer.WriteLine($"{p12x},{p12y}");
        //        //writer.WriteLine($"230,238");
        //        //writer.WriteLine($"{p23x},{p2y}");
        //        //writer.WriteLine($"230,172");
        //        //writer.WriteLine($"{p2x},{p2y}");
        //        //writer.WriteLine($"386,172");
        //        //writer.WriteLine($";;NOP;;");
        //        #endregion
        //        writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {p1x},{p1y} '{iWireCode}' {p1x * 2},{p1y} ;NOP;");
        //        //writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 74,238 'STP__02' 152,238 ;NOP;");
        //        writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {p1x},{p1y} '#{iWireGauge}' {p1x * 2 + 2},{p1y} ;");
        //        //writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 74,238 '#14' 154,238 ;");
        //        writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {p1x},{p1y} '{wire_Type_Core_Number}' {p1x * 2 + 2 + 8},{p1y - 1} ;");
        //        //writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 74,238 '1' 162,237 ;");
        //        writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 {p1x},{p1y} '{wire_Length}' {p1x * 2 + 2 + 8},{p1y} ;");
        //        //writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 74,238 'LLL' 162,238 ;");
        //        writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {p1x},{p1y} '' {p1x * 2 + 2 + 8},{p1y} ;");
        //        //writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 74,238 '' 162,238 ;");
        //        writer.WriteLine($"ADD L154 :W0");
        //        writer.WriteLine($"{p1x},{p1y - 4}");
        //        //writer.WriteLine($"74,234");
        //        //writer.WriteLine($"{p12x - 4},{p12y - 4}");
        //        //writer.WriteLine($"226,234");
        //        //writer.WriteLine($"{p23x - 4},{p2y - 4}");
        //        //writer.WriteLine($"226,168");
        //        writer.WriteLine($"{p2x},{p2y - 4}");
        //        //writer.WriteLine($"386,168");
        //        writer.WriteLine($";;NOP;;");
        //        writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {p2x},{p2y - 4} '{iWireCode}' {X3},{p2y - 4} ;NOP;");
        //        //writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 226,168 'STP__02' 306,168 ;NOP;");
        //        writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {p2x},{p2y - 4} '#{iWireGauge}' {X3 + 2},{p2y - 4} ;");
        //        //writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 226,168 '#14' 308,168 ;");
        //        writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {p2x},{p2y - 4} '{wire_Type_Core_Number + 1}' {X3 +2 + 8},{p2y - 4 - 1} ;");
        //        //writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 226,168 '2' 316,167 ;");
        //        writer.WriteLine($"ADD N59 :R0 :D :J2 :F2 :T3009 :AC L154 {p1x},{p2y - 4} '{wire_Length}' {X3 + 2 + 8},{p2y - 4} ;");
        //        //writer.WriteLine($"ADD N59 :R0 :D :J2 :F2 :T3009 :AC L154 226,168 'LLL' 316,168 ;");
        //        writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {p1x},{p2y - 4} '' {X3 + 2 + 8},{p2y - 4} ;");
        //        //writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 226,168 '' 316,168 ;");
        //        writer.WriteLine($"ADD I2 {twisted_wire_symbol} :R0 {p1x + 16},{p1y};");
        //        //writer.WriteLine($"ADD I2 sth_stp4 :R0 90,238;");
        //        writer.WriteLine($"TESTDIS;");
        //        writer.WriteLine($"MOD N250 {p1x + 16},{p1y} 0,0 :L254 STOR_MID :E'{wire_Type}' JU;NOP;");
        //        //writer.WriteLine($"MOD N250 90,238 0,0 :L254 STOR_MID :E'STP' JU;NOP;");
        //        writer.WriteLine($"MOD N250 {p1x + 16},{p1y - 4} 0,0 :L254 STOR_MID :E'{wire_Type}' JU;NOP;");
        //        //writer.WriteLine($"MOD N250 90,234 0,0 :L254 STOR_MID :E'STP' JU;NOP;");
        //        writer.WriteLine($"TESTDIS_OFF;");
        //        writer.WriteLine($":RAW");
        //        writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {p1x + 16},{p1y} {p1x + 16 - 1.5},{p1y + 0.5};NOP;");
        //        //writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '004' :AC I2 90,238 88.5,238.5;NOP;");
        //        writer.WriteLine($"MOD N253 {p1x + 16},{p1y} 0,0 STOR_MID :E'{groupId}' JU;NOP;");
        //        //writer.WriteLine($"MOD N253 90,238 0,0 STOR_MID :E'004' JU;NOP;");
        //        writer.WriteLine($":GRI");
        //        writer.WriteLine($"ADD I2 {twisted_wire_symbol} :R0 {p2x - 16},{p2y};");
        //        //writer.WriteLine($"ADD I2 sth_stp4 :R0 370,172;");
        //        writer.WriteLine($"TESTDIS;");
        //        writer.WriteLine($"MOD N250 {p2x - 16},{p2y} 0,0 :L254 STOR_MID :E'{wire_Type}' JU;NOP;");
        //        //writer.WriteLine($"MOD N250 370,172 0,0 :L254 STOR_MID :E'STP' JU;NOP;");
        //        writer.WriteLine($"MOD N250 {p2x - 16},{p2y - 4} 0,0 :L254 STOR_MID :E'{wire_Type}' JU;NOP;");
        //        //writer.WriteLine($"MOD N250 370,168 0,0 :L254 STOR_MID :E'STP' JU;NOP;");
        //        writer.WriteLine($"TESTDIS_OFF;");
        //        writer.WriteLine($":RAW");
        //        writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {p2x - 16},{p2y} {p2x - 16 - 1.5},{p2y + 0.5};NOP;");
        //        //writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '004' :AC I2 370,172 368.5,172.5;NOP;");
        //        writer.WriteLine($"MOD N253 {p2x - 16},{p2y} 0,0 STOR_MID :E'{groupId}' JU; NOP;");
        //        //writer.WriteLine($"MOD N253 370,172 0,0 STOR_MID: E'004' JU; NOP;");
        //        writer.WriteLine($":GRI");
        //        //writer.WriteLine($"pm_view_win1_recall_n 9 ;");
        //        //writer.WriteLine($"set_actual_layer LAYER_ORIG ;");
        //        //writer.WriteLine($"pm_view_redraw; TESTDIS_OFF ;");
        //        //writer.WriteLine($"pm_view_grid_on;");
        //        writer.WriteLine($"pm_files_sav;;");
        //        writer.WriteLine($"GRI ELECTRE_GRID_STH;");
        //        //writer.WriteLine($"UNDO_END2;");
        //        //writer.WriteLine($"UNDO :E;");
        //        //MessageBox.Show("Simple2PointConnection_TP_StraightLine Completed");
        //    }

        //}
        #endregion

        #region //Commented old Simple2PointConnection_Mono_StraightLine code on 18 november, 2025
        //public static void Simple2PointConnection_Mono_StraightLine(double p1x, double p1y, double p2x, double p2y, string iWireCode, string iWireGauge, string c1, string c2, string iF_Type, string iT_Type, string groupId, string wire_Length,string wire_Type,string wire_Type_Core_Number)
        //{
        //    //double p12x, p12y, p23x, p23y;
        //    //p12x = p1x + 150;
        //    //p12y = p1y;
        //    //p23x = p12x;
        //    //p23y = p12y - 60;
        //    double X3 = (p1x + p2x) / 2;
        //    string mono_wire_symbol = string.Empty;
        //    if (wire_Type.Equals("86A9S") || wire_Type.Equals("86A9SS") || wire_Type.Equals("S") || wire_Type.Equals("S0") || wire_Type.Equals("S00"))
        //    { mono_wire_symbol = "sth_s"; }
        //    else if (wire_Type.Equals("SS")) { mono_wire_symbol = "sth_ss"; }
        //    else if (wire_Type.Equals("COAX")) { mono_wire_symbol = "sth_coax"; }
        //    else if (wire_Type.Equals("TRIAX")) { mono_wire_symbol = "sth_triax"; }
        //    using (writer = File.AppendText(Constants.el_ExecFilePath))
        //    {
        //        //writer.WriteLine($"ADD L154 {iX},{iY} {iX + iLength},{iY};NOP;");
        //        //writer.WriteLine(
        //        writer.WriteLine($"GRI 2.0, 2;");
        //        writer.WriteLine($"ADD L154 :W0 {p1x},{p1y} {p2x},{p2y} ;;NOP;;");
        //        #region
        //        //writer.WriteLine($"{p1x},{p1y}");
        //        //writer.WriteLine($"74,238");
        //        //writer.WriteLine($"{p12x},{p12y}");
        //        //writer.WriteLine($"230,238");
        //        //writer.WriteLine($"{p23x},{p2y}");
        //        //writer.WriteLine($"230,172");
        //        //writer.WriteLine($"{p2x},{p2y}");
        //        //writer.WriteLine($"386,172");
        //        //writer.WriteLine($";;NOP;;");
        //        #endregion
        //        writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {p1x},{p1y} '{iWireCode}' {p1x * 2},{p1y} ;NOP;");
        //        //writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 74,238 'STP__02' 152,238 ;NOP;");
        //        writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {p1x},{p1y} '#{iWireGauge}' {p1x * 2 + 2},{p1y} ;");
        //        //writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 74,238 '#14' 154,238 ;");
        //        writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {p1x},{p1y} '{wire_Type_Core_Number}' {p1x * 2 + 2 + 8},{p1y - 1} ;");
        //        //writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 74,238 '1' 162,237 ;");
        //        writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 {p1x},{p1y} '{wire_Length}' {p1x * 2 + 2 + 8},{p1y} ;");
        //        //writer.WriteLine($"ADD N59 :R0  :D :J2 :F2 :T3009 :AC L154 74,238 'LLL' 162,238 ;");
        //        writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {p1x},{p1y} '' {p1x * 2 + 2 + 8},{p1y} ;");
        //        //writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 74,238 '' 162,238 ;");
        //        writer.WriteLine($"ADD L154 :W0");
        //        writer.WriteLine($"{p1x},{p1y - 4}");
        //        //writer.WriteLine($"74,234");
        //        //writer.WriteLine($"{p12x - 4},{p12y - 4}");
        //        //writer.WriteLine($"226,234");
        //        //writer.WriteLine($"{p23x - 4},{p2y - 4}");
        //        //writer.WriteLine($"226,168");
        //        writer.WriteLine($"{p2x},{p2y - 4}");
        //        //writer.WriteLine($"386,168");
        //        writer.WriteLine($";;NOP;;");
        //        writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 {p1x},{p1y - 4} '{iWireCode}' {X3},{p2y - 4} ;NOP;");
        //        //writer.WriteLine($"ADD N54  :R0 :J8 :F2 :D :T1002 :AC L154 226,168 'STP__02' 306,168 ;NOP;");
        //        writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 {p1x},{p1y - 4} '#{iWireGauge}' {X3 + 2},{p2y - 4} ;");
        //        //writer.WriteLine($"ADD N56 :R0 :S10 :D :J2 :F2 :T3006 :AC L154 226,168 '#14' 308,168 ;");
        //        writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 {p1x},{p2y - 4} '{wire_Type_Core_Number}' {X3 + 2 + 8},{p2y - 4 - 1} ;");
        //        //writer.WriteLine($"ADD N252 :R0 :D :J3 :F1 :T4005 :AC L154 226,168 '2' 316,167 ;");
        //        writer.WriteLine($"ADD N59 :R0 :D :J2 :F2 :T3009 :AC L154 {p1x},{p2y - 4} '{wire_Length}' {X3 + 2 + 8},{p2y - 4} ;");
        //        //writer.WriteLine($"ADD N59 :R0 :D :J2 :F2 :T3009 :AC L154 226,168 'LLL' 316,168 ;");
        //        writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 {p1x},{p2y - 4} '' {X3 + 2 + 8},{p2y - 4} ;");
        //        //writer.WriteLine($"ADD N254 :R0 :T3017 :AC L154 226,168 '' 316,168 ;");
        //        writer.WriteLine($"ADD I2 {mono_wire_symbol} :R0 {p1x + 16},{p1y};");
        //        ////writer.WriteLine($"ADD I2 sth_stp4 :R0 90,238;");
        //        writer.WriteLine($"TESTDIS;");
        //        //writer.WriteLine($"ADD I2 sth_s :R0 {p1x + 16},{p1y};");
        //        //writer.WriteLine($"ADD I2 sth_stp4 :R0 90,238;");
        //        //writer.WriteLine($"MOD N250 {p1x + 16},{p1y} 0,0 :L254 STOR_MID :E'S' JU;NOP;");
        //        //writer.WriteLine($"MOD N250 90,238 0,0 :L254 STOR_MID :E'STP' JU;NOP;");
        //        //writer.WriteLine($"ADD N58 :J7: T3003: D: F1: R0 '{groupId}' :AC I2 {p1x + 16},{p1y} {p1x + 16 - 1.5},{p1y + 0.5}; NOP;");
        //        //writer.WriteLine($"MOD N250 {p1x + 16},{p1y - 4} 0,0 :L254 STOR_MID :E'S' JU;NOP;");
        //        //writer.WriteLine($"MOD N250 90,234 0,0 :L254 STOR_MID :E'STP' JU;NOP;");
        //        writer.WriteLine($"TESTDIS_OFF;");
        //        writer.WriteLine($":RAW");
        //        writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '{groupId}' :AC I2 {p1x + 16},{p1y} {p1x + 16 - 1.5},{p1y + 0.5};NOP;");
        //        //writer.WriteLine($"ADD N58 :J7 :T3003 :D :F1 :R0 '004' :AC I2 90,238 88.5,238.5;NOP;");
        //        if (wire_Type.Equals("SS"))
        //        {
        //            writer.WriteLine($"MOD N253 {p1x + 16},{p1y} 0,0 STOR_MID :E'{groupId}' JU;NOP;");
        //        }
        //        //writer.WriteLine($"MOD N253 90,238 0,0 STOR_MID :E'004' JU;NOP;");
        //        writer.WriteLine($":GRI");
        //        //writer.WriteLine($"TESTDIS;");
        //        writer.WriteLine($"ADD I2 {mono_wire_symbol} :R0 {p2x - 16},{p2y};");
        //        //writer.WriteLine($"ADD I2 sth_stp4 :R0 370,172;");
        //        writer.WriteLine($"TESTDIS;");
        //        //writer.WriteLine($"MOD N250 {p2x - 16},{p2y} 0,0 :L254 STOR_MID :E'S' JU;NOP;");
        //        //writer.WriteLine($"MOD N250 370,172 0,0 :L254 STOR_MID :E'STP' JU;NOP;");
        //        //writer.WriteLine($"MOD N250 {p2x - 16},{p2y - 4} 0,0 :L254 STOR_MID :E'S' JU;NOP;");
        //        //writer.WriteLine($"MOD N250 370,168 0,0 :L254 STOR_MID :E'STP' JU;NOP;");
        //        //writer.WriteLine($"ADD N58 :J7: T3003: D: F1: R0 '{groupId}' :AC I2 {p2x - 16},{p2y} {p2x - 16 - 1.5},{p1y + 0.5}; NOP;");
        //        //ADD N58 :J7: T3003: D: F1: R0 '001' :AC I2 280,256 278.5,256.5
        //        writer.WriteLine($"TESTDIS_OFF;");
        //        writer.WriteLine($":RAW");
        //        writer.WriteLine($"ADD N58 :J7: T3003: D: F1: R0 '{groupId}' :AC I2 {p2x - 16},{p2y} {p2x - 16 - 1.5},{p1y + 0.5}; NOP;");
        //        if (wire_Type.Equals("SS") || wire_Type.Equals("COAX") || wire_Type.Equals("TRIAX"))
        //        {
        //            writer.WriteLine($"MOD N253 {p2x - 16},{p2y} 0,0 STOR_MID :E'{iWireCode}' JU; NOP;");
        //        }
        //        //writer.WriteLine($"MOD N253 370,172 0,0 STOR_MID   :E'004' JU; NOP;");
        //        writer.WriteLine($":GRI");
        //        //writer.WriteLine($"pm_view_win1_recall_n 9 ;");
        //        //writer.WriteLine($"set_actual_layer LAYER_ORIG ;");
        //        //writer.WriteLine($"pm_view_redraw; TESTDIS_OFF ;");
        //        //writer.WriteLine($"pm_view_grid_on;");
        //        writer.WriteLine($"pm_files_sav;;");
        //        writer.WriteLine($"GRI ELECTRE_GRID_STH;");
        //        //writer.WriteLine($"UNDO_END2;");
        //        //writer.WriteLine($"UNDO :E;");
        //        //MessageBox.Show("Simple2PointConnection_Mono_StraightLine Completed");
        //    }

        //}
        #endregion

        public static void Simple4PointConnection(double p1x, double p1y, double p2x, double p2y, string iWireCode, string iWireGauge, string c1, string c2, string iF_Type, string iT_Type)
        {
            string Obj1Type, Obj2Type;
            string Obj1Name, Obj2Name;
            string sObj1WirePoints = "", sObj2WirePoints = "";

            double X1, Y1, X2, Y2, X3, Y3, X4, y4;
            double X14, y14;

            if (p1x < p2x)
            {
                X1 = p1x; Y1 = p1y;
                X4 = p2x; y4 = p2y;
                Obj1Type = iF_Type; Obj1Name = c1;
                Obj2Type = iT_Type; Obj2Name = c2;
            }
            else
            {
                X1 = p2x; Y1 = p2y;
                X4 = p1x; y4 = p1y;
                Obj1Type = iT_Type; Obj1Name = c2;
                Obj2Type = iF_Type; Obj2Name = c1;
            }

            X14 = Math.Abs(X1 - X4);
            y14 = Math.Abs(Y1 - y4);

            X2 = X1 + X14 / 2;
            Y2 = Y1;

            int ValueOfOffset_Wire = 5;
            //if(!string.IsNullOrEmpty(Constants.arrCompNameForWireOffset))
            Constants.arrCompNameForWireOffset = new string[0];
            int EquIndex = Constants.arrCompNameForWireOffset.Length.Equals("0")?0: modStandard.RowOfFoundStringIn1Darray(X2.ToString(), Constants.arrCompNameForWireOffset);

            Console.WriteLine($"{X2}   {iWireCode}");

            if (EquIndex == 0)
            {
                Array.Resize(ref Constants.arrCompNameForWireOffset, Constants.arrCompNameForWireOffset.Length + 1);
                Array.Resize(ref Constants.arrCountForWireOffset, Constants.arrCompNameForWireOffset.Length);
                Constants.arrCompNameForWireOffset[^1] = X2.ToString();
                Constants.arrCountForWireOffset[^1] = 1.ToString();
            }
            else
            {
                Constants.arrCountForWireOffset[EquIndex] += 1;
                X2 += (Convert.ToInt16(Constants.arrCountForWireOffset[EquIndex]) - 1) * Constants.WireSpacing_X;
            }

            X3 = X2;

            Y3 = Y1 < y4 ? Math.Abs(Y1 + y14) : Math.Abs(Y1 - y14);

            int F = 0, FoundY = 0;

            if (y4 == -22)
            {
                // Debug breakpoint
            }

            if (Obj1Type == "SPL" || Obj2Type == "SPL" || Obj1Type == "TBK" || Obj2Type == "TBK")
            {
                int Offset1 = 1;
                switch (Obj1Type)
                {
                    case "TBK":
                    case "SPL":
                        int RowValue1 = Check_SPL_TBK_WireOverlapAndAppend(Obj1Name, Y1);
                        if (RowValue1 != 0)
                        {
                            F = Convert.ToInt16(Constants.arrSlantSPL[RowValue1, Constants.SlantSPL_Count]);
                        }

                        if (y4 > Y1)
                        {
                            Console.WriteLine("obj1 - y4>y1");
                            sObj1WirePoints = $"{X1},{Y1} {X1 + Offset1 * (RowValue1 == 0 ? 1 : F)},{Y1 + Offset1 * (RowValue1 == 0 ? 1 : F)} {X2},{Y2 + Offset1 * (RowValue1 == 0 ? 1 : F)}";
                        }
                        else if (y4 < Y1)
                        {
                            Console.WriteLine("obj1 - y4<y1");
                            sObj1WirePoints = $"{X1},{Y1} {X1 + Offset1 * (RowValue1 == 0 ? 1 : F)},{Y1 - Offset1 * (RowValue1 == 0 ? 1 : F)} {X2},{Y2 - Offset1 * (RowValue1 == 0 ? 1 : F)}";
                        }
                        break;

                    default:
                        sObj1WirePoints = $"{X1},{Y1} {X2},{Y2}";
                        break;
                }

                switch (Obj2Type)
                {
                    case "TBK":
                    case "SPL":
                        int RowValue2 = Check_SPL_TBK_WireOverlapAndAppend(Obj2Name, y4);
                        if (RowValue2 != 0)
                        {
                            F = Convert.ToInt16(Constants.arrSlantSPL[RowValue2, Constants.SlantSPL_Count]);
                        }

                        if (y4 > Y1)
                        {
                            Console.WriteLine("obj2 - y4>y1");
                            sObj2WirePoints = $"{X3},{Y3 - Offset1 * (RowValue2 == 0 ? 1 : F)} {X4 - Offset1 * (RowValue2 == 0 ? 1 : F)},{y4 - Offset1 * (RowValue2 == 0 ? 1 : F)} {X4},{y4}";
                        }
                        else if (y4 < Y1)
                        {
                            Console.WriteLine("obj2 - y4<y1");
                            sObj2WirePoints = $"{X3},{Y3 + Offset1 * (RowValue2 == 0 ? 1 : F)} {X4 - Offset1 * (RowValue2 == 0 ? 1 : F)},{y4 + Offset1 * (RowValue2 == 0 ? 1 : F)} {X4},{y4}";
                        }
                        break;

                    default:
                        sObj2WirePoints = $"{X3},{Y3} {X4},{y4}";
                        break;
                }

                //Console.WriteLine($"ADD L154 :W0.0 :FILL {sObj1WirePoints} {sObj2WirePoints};;;;NOP");
                File.AppendAllText(Constants.el_ExecFilePath, $"ADD L151 :W0.0 :FILL {sObj1WirePoints} {sObj2WirePoints};;;;NOP\n");
            }

            else
            {
                //string command = $"ADD L154 :W0.0 :FILL {X1},{Y1} {X2},{Y2} {X3},{Y3};;;;NOP";
                File.AppendAllText(Constants.el_ExecFilePath, $"ADD L154 :W0.0 :FILL {X1},{Y1} {X2},{Y2} {X3},{Y3} {X4},{y4};;;;NOP\n");
            }

            InsertWireCode90(X2, Y2 + (Y3 - Y2) / 2, iWireCode, iWireGauge);
        }

        public static int Check_SPL_TBK_WireOverlapAndAppend(string iName, double iY)
        {
            for (int r = 1; r <= Constants.SlantSPL_RowCounter; r++)
            {
                if (Constants.arrSlantSPL[r, Constants.SlantSPL_Yvalue].ToString() == iY.ToString() &&
                    Constants.arrSlantSPL[r, Constants.SlantSPL_Name].ToString() == iName)
                {
                    Constants.arrSlantSPL[r, Constants.SlantSPL_Count] = Convert.ToInt16(Constants.arrSlantSPL[r, Constants.SlantSPL_Count]) + 1.ToString();
                    return r;
                }
            }

            // If not found, append a new row
            Constants.SlantSPL_RowCounter++;
            Constants.arrSlantSPL[Constants.SlantSPL_RowCounter, Constants.SlantSPL_Name] = iName;
            Constants.arrSlantSPL[Constants.SlantSPL_RowCounter, Constants.SlantSPL_Yvalue] = iY.ToString();
            Constants.arrSlantSPL[Constants.SlantSPL_RowCounter, Constants.SlantSPL_Count] = 1.ToString();

            return 0;
        }

    }
}


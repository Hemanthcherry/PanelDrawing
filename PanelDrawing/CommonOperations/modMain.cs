using Panel_Drawing.Forms;
using PanelDrawing.Logs;
using PanelDrawing.Objects;
using PanelDrawing.Services.P1;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Text.RegularExpressions;

namespace PanelDrawing.CommonOperations
{
    public class modMain
    {
        public static void InitiateOutPutFile(string el_ExecfilePath)
        {
            try
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();

                if (!File.Exists(el_ExecfilePath))
                {
                    File.Create(el_ExecfilePath).Close();
                }
                File.WriteAllText(el_ExecfilePath, string.Empty);

                //if (File.Exists(el_ExecfilePath)) { File.Delete(el_ExecfilePath); }
                //File.Create(el_ExecfilePath).Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured {ex.Message}");
            }
        }
        
        public static void ReadLibCatalog()
        {
            Constants.libCatalogList = DataReader.LibraryCatalogReader(Constants.sComponent_Catalog_File);
            // Constants.arrTableOfLibCatalog = ExcelOperations.ConvertCSVDataInto2DArray(Constants.sComponent_Catalog_File,StringComparer.OrdinalIgnoreCase);//ReadLibCatalog
        }       

        public static (double X, double Y) GetNextEquPosition(string side, double compWidth, double compHeight)
        {
            // Validate
            if (!side.Equals("LEFT", StringComparison.OrdinalIgnoreCase) &&
                !side.Equals("RIGHT", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("EQU position must be LEFT or RIGHT");

            double posX;
            double posY;

            // ⚡ X-Coordinate (Left/Right)
            if (side.Equals("LEFT", StringComparison.OrdinalIgnoreCase))
            {
                if (Constants.CursorY_EQU_Left - compHeight < Constants.MarginYEQU)
                {
                    Constants.CursorY_EQU_Left = Constants.SheetHeight - Constants.MarginYEQU;

                    Constants.CursorX_EQU_Left += Constants.ComponentSpacing_X_EQU;
                }
                posX = Constants.CursorX_EQU_Left;

                // ⚡ Y-Coordinate (Top → Bottom)
                posY = Constants.CursorY_EQU_Left;

                // Move Y downward for next EQU component
                Constants.CursorY_EQU_Left = Constants.CursorY_EQU_Left - compHeight - Constants.ComponentSpacingY;
            }
            else
            {
                if (Constants.CursorY_EQU_Right - compHeight < Constants.MarginY)
                {
                    Constants.CursorY_EQU_Right = Constants.SheetHeight - Constants.MarginYEQU;

                    Constants.CursorX_EQU_Right -= Constants.ComponentSpacing_X_EQU;
                }
                // Margin from right side
                posX = Constants.CursorX_EQU_Right;

                // ⚡ Y-Coordinate (Top → Bottom)
                posY = Constants.CursorY_EQU_Right;

                // Move Y downward for next EQU component
                Constants.CursorY_EQU_Right = Constants.CursorY_EQU_Right - compHeight - Constants.ComponentSpacingY;
            }

            //// ⚡ Y-Coordinate (Top → Bottom)
            //posY = Constants.CursorY_EQU;

            //// Move Y downward for next EQU component
            //Constants.CursorY_EQU = Constants.CursorY_EQU  - compHeight - Constants.ComponentSpacingY;

            if (Constants.CursorX_EQU_Left >= Constants.CursorX) // = added on Jan 14
            {
                Constants.CursorX = Constants.CursorX_EQU_Left + compWidth;

                Constants.CursorX_EQU_DIS = Constants.CursorX_EQU_Left + 80;
            }

            posX = Math.Round(posX);
            if(posX % 2 != 0)
            {
                posX = posX + 1;
            }

            posY = Math.Round(posY);
            if(posY % 2 != 0)
            {
                posY = posY + 1;
            }

            return (posX, posY);
        }

        public static (double X, double Y) GetNextComponentPosition(double compWidth, double compHeight, string compType)
        {
            // If next component X exceeds sheet width → go to new row

            if (compType =="DIS" || compType == "EQU")
            {
                if(Constants.CursorY_EQU_DIS - compHeight < Constants.MarginYEQU)
                {
                    Constants.CursorY_EQU_DIS = Constants.SheetHeight - Constants.MarginYEQU;

                    Constants.CursorX_EQU_DIS += Constants.ComponentSpacingX + Constants.ColumnWidth;

                    Constants.ColumnWidth = 0;
                }

                double posX = Constants.CursorX_EQU_DIS;
                // Current component position
                if (compType == "DIS") 
                {
                    posX = Constants.CursorX_EQU_DIS + 30;
                }
                
                double posY = Constants.CursorY_EQU_DIS; //- compHeight;

                Constants.CursorY_EQU_DIS = Constants.CursorY_EQU_DIS - compHeight - Constants.ComponentSpacingY;

                // Update widest component in this column
                if (compWidth > Constants.ColumnWidth)
                {
                    Constants.ColumnWidth = compWidth;
                }
            
                if (Constants.CursorX_EQU_DIS >= Constants.CursorX)
                {
                    Constants.CursorX = Constants.CursorX_EQU_DIS + compWidth;
                }

                posX = Math.Round(posX);
                if (posX % 2 != 0)
                {
                    posX = posX + 1;
                }

                posY = Math.Round(posY);
                if (posY % 2 != 0)
                {
                    posY = posY + 1;
                }

                return (posX, posY);
            }
            else
            {               
                //if (Constants.CursorX + compWidth > Constants.SheetWidth - Constants.MarginX)
                if (Constants.CursorX + compWidth > Constants.CursorX_EQU_Right - Constants.MarginX)
                {
                    // Move to NEXT ROW
                    // Constants.CursorX = Constants.MarginX;
                   Constants.CursorX = Constants.CursorX_EQU_DIS + 80; // commented on Jan 14th

                   // Constants.CursorX = Constants.CursorX_EQU_Left + 40;

                    Constants.CursorY += Constants.RowHeight + Constants.ComponentSpacingY;

                    // Reset Row Height
                    Constants.RowHeight = 0;
                }

                // Current component position
                double posX = Constants.CursorX;
                double posY = Constants.CursorY; //- compHeight;

                // Move X cursor to right for next component
                Constants.CursorX += compWidth + Constants.ComponentSpacingX;

                // Update tallest component in this row
                if (compHeight > Constants.RowHeight)
                    Constants.RowHeight = compHeight;

                posX = Math.Round(posX);
                if (posX % 2 != 0)
                {
                    posX = posX + 1;
                }

                posY = Math.Round(posY);
                if (posY % 2 != 0)
                {
                    posY = posY + 1;
                }

                return (posX, posY);
            }            
        }

        public static void InitializeLayout()
        {
            // X starts at left margin
            Constants.CursorX = Constants.MarginX;

            Constants.CursorX_EQU_DIS = /*Constants.MarginX + */120;  //180 commented on Jan 14

            Constants.CursorX_EQU_Left = Constants.MarginXEQU;

            Constants.CursorX_EQU_Right = Constants.SheetWidth - Constants.MarginXEQU;

            // Y starts at TOP of the sheet
            //Constants.CursorY = Constants.SheetHeight - Constants.MarginY;

            // Y starts at Bottom of the sheet
            Constants.CursorY = Constants.MarginY;

            Constants.CursorY_EQU = Constants.SheetHeight - Constants.MarginYEQU;
            Constants.CursorY_EQU_Right = Constants.SheetHeight - Constants.MarginYEQU;
            Constants.CursorY_EQU_Left = Constants.SheetHeight - Constants.MarginYEQU;

            Constants.CursorY_EQU_DIS = Constants.SheetHeight - Constants.MarginYEQU ;

            // Track tallest component in the current row
            Constants.RowHeight = 0;

            Constants.ColumnWidth = 0;
        }

        public static (double Width, double Height) GetComponentDefaultSize(string compType)
        {
            return compType.ToUpper() switch
            {
                "SPL" => (50, 50),
                "SCB" => (30, 30),
                "TCB" => (50, 50),
                "TBK" => (80, 140),
                "TER" => (80, 140),
                "SWT" => (80, 140),
                "EQU" => (100, 200),
                "DIS" => (100, 200),
                "MSW" => (60, 80),
                "REL" => (80, 140),
                "IND" => (60, 80),
                "BUS" => (80, 80),
                "ANT" => (80, 80),
                "DD" => (30, 30),
                "FUS" => (80, 80),
                "SNR" => (60, 80),
                "GND" => (40, 50),
                "ERM" => (60, 80),
                "LMP" => (60, 80),
                "TRK" => (60, 80),
                "NEW" => (60, 80),
                "RES" => (30, 30),
                "CAP" => (30, 30),
                "IDT" => (60, 80),
                "POT" => (60, 80),
                _ => (60, 100) // fallback
            };
        }

        public static void InitiateStep1()
        {
            AppLog.Info($"P1 Components Placement Started...........");
            InitializeLayout();

            Constants.processedItems = new List<string>();
            Constants.remainingItems = new List<string>();

            string doubleSidePattern = @"(_J([1-9]|1[0-9]|2[0-4])|_[a-hj-np-z])$";

            var components = Constants.panelComponentProperties
                .OrderBy(c =>
                {
                    // PRIORITY 0 & 1: EQU Components
                    if (c.ComponentType == "EQU")
                    {
                        bool isDoubleSide = Regex.IsMatch(c.ComponentName ?? "", doubleSidePattern, RegexOptions.IgnoreCase);
                        return isDoubleSide ? 1 : 0; // 0 = Single, 1 = Double
                    }

                    // PRIORITY 2: DIS Components
                    if (c.ComponentType == "DIS") return 2;

                    // PRIORITY 3: Everything else
                    return 3;
                })
                .ThenBy(c => c.ComponentType) // Group remaining types together
                .ThenBy(c => c.ComponentName) // Ensure alphabetical order within groups
                .ToList();

            foreach (var comp in components)
            {
                string CompDwgName = comp.ComponentName?.Trim() ?? "";
                string CompType = comp.ComponentType?.Trim().ToUpper() ?? "";
                string MacroName = comp.MacroName?.Trim() ?? "";
                string CompPN = comp.PartNumber?.Trim() ?? "";
                string CompAcc = comp.Accessory?.Trim() ?? "";
                string CompMaxPin = comp.MaxPin?.Trim() ?? "";
                string SampleEquPinNumber = comp.SamplePin?.Trim() ?? "";
                string GroupId = comp.GroupId?.Trim() ?? "";
                string Wire_Length = comp.WireLength?.Trim() ?? "";
                string Wire_Type = comp.WireType?.Trim() ?? "";
                string CBType_Name = comp.CBTypeName?.Trim() ?? "";
                string CBVoltage = comp.CBVoltage?.Trim() ?? "";
                string EquipBox = comp.EquipmentBox?.Trim() ?? "";
                string Looms = comp.Looms?.Trim() ?? "";
                string AssocPNs = comp.AssociatedPartNumbers?.Trim() ?? "";
                string ShuntInfo = comp.ShuntList?.Trim() ?? "";

                (double defW, double defH) = GetComponentDefaultSize(CompType);

                double CompWidth = comp.CompWidth == 0 ? defW : comp.CompWidth;
                double CompHeight = comp.CompHeight == 0 ? defH : comp.CompHeight;

                double CompXdist = 0.0;
                double CompYdist = 0.0;
                //frmPanelOri frmpanelori = new frmPanelOri();

                AppLog.Info($"Component {CompDwgName} ({CompType}) started placement");
                try
                {
                    switch (CompType)
                    {
                        case "SPL":
                            if (string.IsNullOrEmpty(MacroName))
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(30, 30, CompType);
                                modOPCommand.AddSymbolSPL_OOTB(CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                                AppLog.Info($"OOTB Component {CompDwgName} ({CompType}) placed at X={CompXdist}, Y={CompYdist}");
                            }
                            else
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, CompHeight, CompType);

                                modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                                // modOPCommand.AddSWTSymbolAttributes(CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath, CompDwgName);
                                modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                                AppLog.Info($"Custom Component {CompDwgName} ({CompType}) with macro {MacroName} placed at X={CompXdist}, Y={CompYdist}");
                            }
                            continue;

                        case "TBK":
                        /*(CompXdist, CompYdist) = GetNextComponentPosition(80, 100);

                        modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                        modOPCommand.AddSWTSymbolAttributes(CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath, CompDwgName);
                        continue;*/

                        case "TER":
                            var tbkPins = modStandard.PinsOfConnector(CompDwgName);
                            if (tbkPins.Count == 0)
                            {
                                MessageBox.Show($"{CompDwgName} Pins Not Available");
                                continue;
                            }
                            if (string.IsNullOrEmpty(MacroName))
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, CompHeight, CompType);

                                modOPCommand.AddOOTB_TER_SymbolForTerminal(CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath, "0", tbkPins, ShuntInfo);
                                AppLog.Info($"OOTB Component {CompDwgName} ({CompType}) placed at X={CompXdist}, Y={CompYdist}");
                            }
                            else
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, CompHeight, CompType);

                                modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                                modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                                AppLog.Info($"Custom Component {CompDwgName} ({CompType}) with macro {MacroName} placed at X={CompXdist}, Y={CompYdist}");
                                //modOPCommand.AddSWTSymbolAttributes(CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath, CompDwgName);
                            }
                            continue;

                        case "EQU":
                            var equPins = modStandard.PinsOfConnector(CompDwgName);
                            Constants.listPinsOfEqu = equPins;

                            int NoOfPins = equPins.Count;
                            Constants.txtEquName = CompDwgName;

                            // Update panelDetailsList
                            foreach (var row in Constants.panelDetailsList.Where(x => x.FromConnector == CompDwgName))
                            {
                                row.Usage = SampleEquPinNumber;
                                row.FromType = CompType;
                                row.GroupId = GroupId;
                                row.WireLength = Wire_Length;
                                row.WireType = Wire_Type;
                            }

                            //bool isDouble = CompDwgName.Contains("_J") ||
                            //    Regex.IsMatch(CompDwgName.Last().ToString(), "[a-hj-np-zA-HJ-NP-Z]");
                            bool isDouble =
                                Regex.IsMatch(CompDwgName, @"_[Jj](?:[1-9]|1\d|2[0-4])$") ||
                                Regex.IsMatch(CompDwgName, @"_[A-HJ-NP-Za-hj-np-z]$");


                            //(CompXdist, CompYdist) = GetNextComponentPosition(80, 100);
                            int CompHeightEQU = (NoOfPins * 4) + 20 + 20;

                            if (isDouble)
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, CompHeightEQU, CompType);

                                string root = CompDwgName.Split('_')[0];

                                if (!Constants.remainingItems.Contains(root))
                                {
                                    EQUDoubleConnector.GraLine(CompXdist, CompYdist, Constants.listPinsOfEqu.Count, CompDwgName, Constants.listPinsOfEqu, CompPN, AssocPNs, EquipBox, root);

                                    Constants.remainingItems.Add(root);
                                }
                            }
                            else
                            {
                                //frmpanelori.lblEquName.Text = CompDwgName;
                                // frmpanelori.ShowDialog();
                                //var side = frmpanelori.SelectedSide;
                                var side = comp.SymbolName.Trim().Contains("contact_sth_mr") ? "LEFT" : "RIGHT";

                                // Get position based on LEFT / RIGHT
                                //(CompXdist, CompYdist) = GetNextEquPosition(side,40,compHeight: Constants.arrPinsOfEqu.Length * 10);
                                (CompXdist, CompYdist) = GetNextEquPosition(side, 40, CompHeightEQU);

                                EQUSingleConnector.DrawEquSymbolUsedPins(CompXdist, CompYdist, CompMaxPin, CompDwgName, side, SampleEquPinNumber, CompPN, /*frmpanelori,*/ AssocPNs, EquipBox, Looms);
                                //frmpanelori.Close();
                            }
                            AppLog.Info($"OOTB Component {CompDwgName} ({CompType}) placed at X={CompXdist}, Y={CompYdist}");
                            continue;

                        case "DIS":
                            var disPins = modStandard.PinsOfConnector(CompDwgName);
                            if (disPins.Count == 0)
                            {
                                MessageBox.Show($"{CompDwgName} Pins Not Available");
                                continue;
                            }
                            int yCoordinate_DIS = (disPins.Count * 4) + 12 + 18;

                            foreach (var pd in Constants.panelDetailsList.Where(x => x.FromConnector == CompDwgName))
                            {
                                pd.Usage = SampleEquPinNumber;
                                pd.FromType = CompType;
                            }

                            string baseName = CompDwgName.Length > 2 ? CompDwgName[..^2] : CompDwgName;

                            // If already processed → exit
                            if (Constants.processedItems.Contains(baseName))
                                continue;

                            (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, yCoordinate_DIS, CompType);

                            GraLine_DIS.GraLine(CompXdist, CompYdist, disPins.Count, CompDwgName, disPins, CompPN, AssocPNs, EquipBox);
                            AppLog.Info($"OOTB Component {CompDwgName} ({CompType}) placed at X={CompXdist}, Y={CompYdist}"); 
                            continue;

                        case "SWT":
                            if (!string.IsNullOrEmpty(MacroName))
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, CompHeight, CompType);

                                modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                                modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                                AppLog.Info($"Custom Component {CompDwgName} ({CompType}) with macro {MacroName} placed at X={CompXdist}, Y={CompYdist}");
                            }
                            //modOPCommand.AddSWTSymbolAttributes(CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath, CompDwgName);
                            continue;

                        case "MSW":
                            if (!string.IsNullOrEmpty(MacroName))
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, CompHeight, CompType);
                                modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                                modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                                AppLog.Info($"Custom Component {CompDwgName} ({CompType}) with macro {MacroName} placed at X={CompXdist}, Y={CompYdist}");
                            }
                            continue;

                        case "GND" or "GROUND":
                            if (string.IsNullOrEmpty(MacroName))
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(20, 20, CompType);
                                modOPCommand.AddSymbolGND(CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                                AppLog.Info($"OOTB Component {CompDwgName} ({CompType}) placed at X={CompXdist}, Y={CompYdist}");
                            }
                            else
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, CompHeight, CompType);
                                modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                                modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                                AppLog.Info($"Custom Component {CompDwgName} ({CompType}) with macro {MacroName} placed at X={CompXdist}, Y={CompYdist}");
                            }
                            continue;

                        case "SCB":
                            if (!string.IsNullOrEmpty(MacroName))
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, CompHeight, CompType);
                                modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                                modOPCommand.AddCBSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, CBVoltage, Constants.el_ExecFilePath, CompType);
                                AppLog.Info($"Custom Component {CompDwgName} ({CompType}) with macro {MacroName} placed at X={CompXdist}, Y={CompYdist}");
                            }
                            continue;

                        case "TCB":
                            if (!string.IsNullOrEmpty(MacroName))
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, CompHeight, CompType);
                                modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                                modOPCommand.AddCBSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, CBVoltage, Constants.el_ExecFilePath, CompType);
                                AppLog.Info($"Custom Component {CompDwgName} ({CompType}) with macro {MacroName} placed at X={CompXdist}, Y={CompYdist}");
                            }
                            continue;

                        case "REL":
                            var relPins = modStandard.GetPinsofOOTBRelay(CompDwgName);
                            int yCoordinateREL = (relPins.Count * 5) + 40;

                            //(CompXdist, CompYdist) = GetNextComponentPosition(60, yCoordinateREL);
                            //modOPCommand.AddSymbolREL_OOTB(CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath, relPins);

                            if (string.IsNullOrEmpty(MacroName))
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, yCoordinateREL, CompType);
                                modOPCommand.AddSymbolREL_OOTB(CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath, relPins);
                                AppLog.Info($"OOTB Component {CompDwgName} ({CompType}) placed at X={CompXdist}, Y={CompYdist}");
                            }
                            else
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, CompHeight, CompType);
                                modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                                modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                                AppLog.Info($"Custom Component {CompDwgName} ({CompType}) with macro {MacroName} placed at X={CompXdist}, Y={CompYdist}");
                            }
                            continue;

                        case "IND":
                            if (!string.IsNullOrEmpty(MacroName))
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, CompHeight, CompType);
                                modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                                modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                                AppLog.Info($"Custom Component {CompDwgName} ({CompType}) with macro {MacroName} placed at X={CompXdist}, Y={CompYdist}");
                            }
                            continue;

                        case "BUS":
                            if (!string.IsNullOrEmpty(MacroName))
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, CompHeight, CompType);
                                modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                                modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                                AppLog.Info($"Custom Component {CompDwgName} ({CompType}) with macro {MacroName} placed at X={CompXdist}, Y={CompYdist}");
                            }
                            continue;

                        case "ANT":
                            if (!string.IsNullOrEmpty(MacroName))
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, CompHeight, CompType);
                                modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                                modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                                AppLog.Info($"Custom Component {CompDwgName} ({CompType}) with macro {MacroName} placed at X={CompXdist}, Y={CompYdist}");
                            }
                            continue;

                        case "DD":
                            if (!string.IsNullOrEmpty(MacroName))
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, CompHeight, CompType);
                                modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                                modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                                AppLog.Info($"Custom Component {CompDwgName} ({CompType}) with macro {MacroName} placed at X={CompXdist}, Y={CompYdist}");
                            }
                            continue;

                        case "FUS":
                            if (!string.IsNullOrEmpty(MacroName))
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, CompHeight, CompType);
                                modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                                modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                                AppLog.Info($"Custom Component {CompDwgName} ({CompType}) with macro {MacroName} placed at X={CompXdist}, Y={CompYdist}");
                            }
                            continue;

                        //case "CNT":
                        //    modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                        //    modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                        //    break;

                        case "SNR":
                            if (!string.IsNullOrEmpty(MacroName))
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, CompHeight, CompType);
                                modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                                modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                                AppLog.Info($"Custom Component {CompDwgName} ({CompType}) with macro {MacroName} placed at X={CompXdist}, Y={CompYdist}");
                            }
                            continue;

                        //case "ML":
                        //    modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                        //    modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                        //    break;

                        case "EM" or "ERM":
                            if (!string.IsNullOrEmpty(MacroName))
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, CompHeight, CompType);
                                modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                                modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                                AppLog.Info($"Custom Component {CompDwgName} ({CompType}) with macro {MacroName} placed at X={CompXdist}, Y={CompYdist}");
                            }
                            continue;

                        case "LMP":
                            if (!string.IsNullOrEmpty(MacroName))
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, CompHeight, CompType);
                                modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                                modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                                AppLog.Info($"Custom Component {CompDwgName} ({CompType}) with macro {MacroName} placed at X={CompXdist}, Y={CompYdist}");
                            }
                            continue;

                        //case "M1" or "M2":
                        //    modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                        //    modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                        //    break;

                        case "TRK":
                            if (!string.IsNullOrEmpty(MacroName))
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, CompHeight, CompType);
                                modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                                modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                                AppLog.Info($"Custom Component {CompDwgName} ({CompType}) with macro {MacroName} placed at X={CompXdist}, Y={CompYdist}");
                            }
                            continue;

                        case "NEW":
                            if (!string.IsNullOrEmpty(MacroName))
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, CompHeight, CompType);
                                modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                                modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                                AppLog.Info($"Custom Component {CompDwgName} ({CompType}) with macro {MacroName} placed at X={CompXdist}, Y={CompYdist}");
                            }
                            continue;

                        case "RES":
                            if (!string.IsNullOrEmpty(MacroName))
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, CompHeight, CompType);
                                modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                                modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                                AppLog.Info($"Custom Component {CompDwgName} ({CompType}) with macro {MacroName} placed at X={CompXdist}, Y={CompYdist}");
                            }
                            continue;

                        case "CAP":
                            if (!string.IsNullOrEmpty(MacroName))
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, CompHeight, CompType);
                                modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                                modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                                AppLog.Info($"Custom Component {CompDwgName} ({CompType}) with macro {MacroName} placed at X={CompXdist}, Y={CompYdist}");
                            }
                            continue;

                        case "IDT":
                            if (!string.IsNullOrEmpty(MacroName))
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, CompHeight, CompType);
                                modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                                modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                                AppLog.Info($"Custom Component {CompDwgName} ({CompType}) with macro {MacroName} placed at X={CompXdist}, Y={CompYdist}");
                            }
                            continue;

                        case "POT":
                            if (!string.IsNullOrEmpty(MacroName))
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, CompHeight, CompType);
                                modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                                modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                                AppLog.Info($"Custom Component {CompDwgName} ({CompType}) with macro {MacroName} placed at X={CompXdist}, Y={CompYdist}");
                            }
                            continue;

                        default:
                            if (!string.IsNullOrEmpty(MacroName))
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, CompHeight, CompType);
                                modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                                modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                                AppLog.Info($"Custom Component {CompDwgName} ({CompType}) with macro {MacroName} placed at X={CompXdist}, Y={CompYdist}");
                            }
                            else
                            {
                                AppLog.Warn($"No Macro defined for Component {CompDwgName} ({CompType}). Skipping placement.");
                            }
                            continue;
                    }
                }
                catch (Exception ex)
                {
                    AppLog.Error($"Component {CompDwgName} ({CompType}) placement failed", ex);
                    continue;
                }
            }

            AppLog.Info($"P1 Components Placement Completed............");
        }

        public static void UpdatePanelDetailsTextFile(List<PanelDetailsRow> panelDetailsList)
        {
            if (panelDetailsList == null || panelDetailsList.Count == 0)
                return;

            // Prepare file name
            Constants.PanelDetailsFullFileName =
                Path.Combine(Constants.Electre_Temp_Folder_Path,
                             $"{Constants.textPanelPartNumber}-{Constants.panelDetailsTextFileName}");

            List<string> outputLines = new List<string>();

            foreach (var pd in panelDetailsList)
            {
                string[] rowValues =
                {
                pd.FromConnector?.Trim() ?? "",
                pd.FromPin?.Trim() ?? "",
                pd.ToConnector?.Trim() ?? "",
                pd.ToPin?.Trim() ?? "",
                pd.WireCode?.Trim() ?? "",
                pd.PinX?.Trim() ?? "",
                pd.PinY?.Trim() ?? "",
                pd.TPinX?.Trim() ?? "",
                pd.TPinY?.Trim() ?? "",
                pd.Usage?.Trim() ?? "",
                pd.FromType?.Trim() ?? "",
                pd.ToType?.Trim() ?? "",
                pd.FromOrientation?.Trim() ?? "",
                pd.ToOrientation?.Trim() ?? "",
                pd.GroupId?.Trim() ?? "",
                pd.WireLength?.Trim() ?? "",
                pd.WireType?.Trim() ?? "",
                pd.WireTypeNumber?.Trim() ?? ""
               };
                // Join all fields into one CSV row
                outputLines.Add(string.Join(",", rowValues));
            }

            // Save lines to file
            File.WriteAllLines(Constants.PanelDetailsFullFileName, outputLines);

            // Store in constants for later use
           // Constants.arrUpdatedPanelDetails = outputLines.ToArray();
        }

        public static void ComponentsCreatedTextFileCreation()
        {
            //Constants.
            //Constants.arrComponentsCreated[0] = string.Concat(Constants.textPanelPartNumber,Constants.textPanelPartName, lstSelectedItemTemplateName,);
            File.WriteAllLines(string.Concat(Constants.Electre_Temp_Folder_Path, Constants.ComponentsCreatedTextFileName), Constants.arrComponentsCreated);
            File.WriteAllLines(string.Concat(Constants.Electre_Temp_Folder_Path, Constants.NewSheetDetails), Constants.arrComponentsCreated);
        }

        public static void ExitOutputFile(string filepath)
        {
            using (var writer = File.AppendText(filepath))
            {
                writer.WriteLine("GRI ELECTRE_GRID_STH;");
                writer.WriteLine("FOPEN (TYC+FEXEC+TYC);");
                writer.WriteLine("FWRITE (TYC+';;'+TYC) ;");
                writer.WriteLine("FCLOSE;");
                writer.WriteLine("NOP;");
                writer.WriteLine(";");
                // writer.Close();
            }
        }

        public static List<PanelComponentProperties> GatherPanelComponentProperties(string panelName)
        {
           // GatherPanelComponentProperties(panelName);
            // Clear previous results
            Constants.panelComponentProperties.Clear();

            // STEP 1: Get all components belonging to panel & having part number
            var panelComponents = Constants.dataExtractionList
              //  Constants.dataExtractionListAbove
                .Where(x => x.Panel == panelName &&
                            Constants.listComponentsWithPartNumber.Contains(x.ConnectorName))
                .Select(x => x.ConnectorName)
                .Distinct()
                .ToList();

            var GNDcomponents = Constants.dataExtractionList.
                Where(x =>  x.Panel == panelName && (x.ComponentType == "GND" || x.ComponentType == "GROUND")).Select(x => x.ConnectorName).Distinct().ToList();

            panelComponents.AddRange(GNDcomponents);

            panelComponents = panelComponents.Distinct().ToList();

            foreach (var compName in panelComponents)
            {
                // STEP 2: Get all DEAbove rows for this component
                //var compDEAbove = Constants.dataExtractionListAbove  // commented this line on Dec,1 2025
                var compDEAbove = Constants.dataExtractionList
                    .Where(x => x.ConnectorName == compName && x.Panel == panelName)
                    .ToList();

                // STEP 3: Get associated part numbers
                var associatedPNs = compDEAbove
                    .Select(x => x.CoreNumber)     // CoreNumber = PartNumber
                    .Where(x => !string.IsNullOrEmpty(x)
                            && (!int.TryParse(x, out int coreNum) || coreNum < 1 || coreNum > 18)) // added extra line on Dec,1 2025
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
                string cbTypeName = firstDEBelow?.EquipmentName ?? "";
                string cbVoltage = firstDEBelow?.Voltage ?? "";
                string symbolName = firstDEBelow?.SymbolName ?? "";

                // STEP 6: Shunts (for TBK/TER)
                var shuntList = "";
                if (compType == "TBK" || compType == "TER")
                {
                    shuntList = string.Join(";",
                            Constants.dataExtractionListBelow
                            .Where(x => x.ConnectorName == compName &&
                                        !string.IsNullOrEmpty(x.Shunt))
                            .Select(x => $"{x.ConnectorName},{x.PinNumber},{x.FunctionalDesignation},{x.ComponentType},{x.Shunt}"));
                }             

                // STEP 7: Get info from Library Catalog
                var lib = Constants.libCatalogList
                    .FirstOrDefault(x =>
                        (x.RefInternal ?? "") == primaryPN ||
                        (x.MandatoryAccessory1 ?? "") == primaryPN //||
                       // (x.MandatoryAccessory2 ?? "") == primaryPN
                    );

                string macroName = lib?.Symbol2D ?? "";
                string maxPins = lib?.MaxPins ?? "";
                string accessory = lib?.MandatoryAccessory1 ?? "";

                double CompWidth = lib?.CompWidth ?? 0.0;
                double CompHeight = lib?.CompHeight ?? 0.0;
                //string acc2 = lib?.MandatoryAccessory2 ?? "";

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
                    Accessory = $"{accessory}",
                    SamplePin = samplePin,
                    GroupId = firstDEBelow?.Group ?? "",
                    WireLength = firstDEBelow?.Length ?? "",
                    WireType = firstDEBelow?.CableType ?? "",
                    CBTypeName = cbTypeName,
                    SymbolName = symbolName,
                    CBVoltage = cbVoltage,
                    AssociatedPartNumbers = string.Join(";", associatedPNs),
                    EquipmentBox = equipInfo,
                    Looms = loomsInfo,
                    ShuntList = shuntList,
                    CompWidth = CompWidth,
                    CompHeight = CompHeight
                };

                Constants.panelComponentProperties.Add(compProps);
            }        
            
            return Constants.panelComponentProperties;
        }

        #region // Commented on Jan 27, 2026    
        #region //Commented old UpdatePanelDetailsTextFile code on 18 november, 2025
        /* public static void UpdatePanelDetailsTextFile(string[,] arrPD)
         {            
             string[] strarrPDValue = new string[arrPD.GetLength(1)];

             Constants.PanelDetailsFullFileName = string.Concat(Constants.Electre_Temp_Folder_Path, Constants.textPanelPartNumber, Constants.dashMark, Constants.panelDetailsTextFileName);
             Constants.arrUpdatedPanelDetails = new string[arrPD.GetLength(0)];

             for (int i = 0; i < arrPD.GetLength(0); i++)
             {
                 for (int j = 0; j < arrPD.GetLength(1); j++)
                 {
                     strarrPDValue[j]= arrPD[i, j].ToString();
                     //if (string.IsNullOrEmpty(strarrPDValue[9])) { strarrPDValue[9] = 1.ToString(); }
                 }
                 Constants.strPDlines = string.Join(",", strarrPDValue);
                 Constants.arrUpdatedPanelDetails[i] = Constants.strPDlines;
             }
             File.WriteAllLines(Constants.PanelDetailsFullFileName, Constants.arrUpdatedPanelDetails);
         }*/
        #endregion

        #region //GatherPanelComponentProperties_backup code
        /* public static void GatherPanelComponentProperties_backup(string panelName)
         {
             #region//no of components from arrDEabove  
             int NumberOfPanelComponents = 0;
             string strComponentName = string.Empty;
             int nocomps = (Constants.arrComponentsWithPartNumber).Count(x => !string.IsNullOrEmpty(x));//Remove null from array
             for (int pancom = 0; pancom <= nocomps - 1; pancom++)
             {
                 strComponentName = Constants.arrComponentsWithPartNumber[pancom];
                 for (int comDEabove = 0; comDEabove <= Constants.arrDEabove.GetLength(0) - 1; comDEabove++)
                 {
                     if (Constants.arrDEabove[comDEabove, Constants.colDE_Connector-1].Equals(strComponentName) && Constants.arrDEabove[comDEabove, Constants.colDE_Panels - 1].Equals(panelName))
                         //if (Constants.arrDEabove[comDEabove, Constants.colDE_Connector - 1].Equals(strComponentName) && Constants.arrDEabove[comDEabove, Constants.colDE_Info - 1].Equals(Constants.info) && Constants.arrDEabove[comDEabove, Constants.colDE_Panels - 1].Equals(panellName))
                     {
                         NumberOfPanelComponents = NumberOfPanelComponents + 1;
                         Debug.Print($" NumberOfPanelComponents {NumberOfPanelComponents.ToString()} - {strComponentName}");
                         break;
                     }
                 }
             }
             #endregion

             #region
             int PanelComp = 0;
             //Commented on Nov 13,2025 due to index outside bounds array getting in
             //Constants.arrPanelComponentsProperties = new string[NumberOfPanelComponents, Constants.arrPanelComponentsPropertiesMaxCols];
             Constants.arrPanelComponentsProperties = new string[nocomps, Constants.arrPanelComponentsPropertiesMaxCols];
             bool IsInPanel = false;
             string CompPN = string.Empty;
             string CompType = string.Empty;
             string CBType_Name = string.Empty;
             string CBType_Voltage = string.Empty;
             //colDE_CBType_Voltage
             string SampleEquPinNumber= string.Empty;
             string GroupId = string.Empty;
             string Wire_Length = string.Empty;
             string Wire_Type = string.Empty;
             string Equipment_Box_Info = string.Empty;
             string Looms_Info = string.Empty;
             #endregion
             //string TBKTER_Shunt_Value = string.Empty;
             var finalPnlComps = Constants.arrComponentsWithPartNumber.Distinct().ToArray();
             Constants.arrComponentsWithPartNumber = finalPnlComps.ToArray();
             //Define Component Properties for each component which are inside panel
             for (int rPanCOmpPros = 0; rPanCOmpPros <= nocomps - 1; rPanCOmpPros++)
             {
                 #region//Check for The particular component is inside/belongs to the panel or not 
                 if(string.IsNullOrEmpty(Constants.arrComponentsWithPartNumber[rPanCOmpPros])) continue;
                 strComponentName = Constants.arrComponentsWithPartNumber[rPanCOmpPros];
                 for (int comDEabove = 0; comDEabove <= Constants.arrDEabove.GetLength(0) - 1; comDEabove++)
                 {
                     if (Constants.arrDEabove[comDEabove, Constants.colDE_Connector - 1].Equals(strComponentName) && Constants.arrDEabove[comDEabove, Constants.colDE_Panels - 1].Equals(panelName))
                   //if (Constants.arrDEabove[comDEabove, Constants.colDE_Connector - 1].Equals(strComponentName) && Constants.arrDEabove[comDEabove, Constants.colDE_Info - 1].Equals(Constants.info) && Constants.arrDEabove[comDEabove, Constants.colDE_Panels - 1].Equals(panellName))
                     {
                         IsInPanel = true;
                         //NumberOfPanelComponents = NumberOfPanelComponents + 1;
                         //Debug.Print($" NumberOfPanelComponents {NumberOfPanelComponents.ToString()} - {strComponentName}");
                         break;
                     }
                     *//*else
                     {
                         IsInPanel = false;
                     }*//*
                 }
                 #endregion

                 if (IsInPanel == false) continue;
                 int rownum = Constants.temp_rownum;

                 #region //Get Part Number from arrDEabove 2D array if the component is in same panel or exist based on IsInPanel is true or false
                 for (int comDEabove = 0; comDEabove <= Constants.arrDEabove.GetLength(0) - 1; comDEabove++)
                 {
                     //Adding properties for Part Numbers
                     if (Constants.arrDEabove[comDEabove, Constants.colDE_Connector - 1].Equals(strComponentName) && Constants.arrDEabove[comDEabove, Constants.colDE_PartNumber - 1] != "" && Constants.arrDEabove[comDEabove, Constants.colDE_Panels - 1].Equals(panelName))
                     //if (Constants.arrDEabove[comDEabove, Constants.colDE_Connector - 1].Equals(strComponentName) && Constants.arrDEabove[comDEabove, Constants.colDE_Info - 1].Equals(Constants.info) && Constants.arrDEabove[comDEabove, Constants.colDE_PartNumber - 1] != "" && Constants.arrDEabove[comDEabove, Constants.colDE_Panels - 1].Equals(panellName))
                     {
                         #region //The below code search for whether Component Part Number is primary part number for relavent component or assosiate part number,
                         string sd = modOPCommand.RowOfCountFoundStringInColOfMDarray(strComponentName, Constants.arrDEabove, Constants.colDE_Connector - 1, Constants.colDE_PartNumber - 1, Constants.colDE_Panels - 1);
                         string[] arrPns = sd.Split(";");
                         arrPns = arrPns.Distinct().ToArray();
                         if (arrPns.Length > 1)
                         {
                             Constants.strDEPanelaboveAssosiatePNList = string.Empty;
                             CompPN = modMain.FindPartNumberFromAssosiatedPartnumbersIfDefined(arrPns);
                             var removePNfromAssoPNList = arrPns.ToList();
                             removePNfromAssoPNList.Remove(CompPN);
                             Constants.strDEPanelaboveAssosiatePNList = string.Join(";", removePNfromAssoPNList.ToArray());
                             Equipment_Box_Info = Constants.arrDEabove[comDEabove, Constants.colDE_EquipmentBox - 1];
                             Looms_Info = Constants.arrDEabove[comDEabove, Constants.colDE_Looms - 1];
                         }
                         else
                         {
                             CompPN = Constants.arrDEabove[comDEabove, Constants.colDE_PartNumber - 1];
                             Equipment_Box_Info = Constants.arrDEabove[comDEabove, Constants.colDE_EquipmentBox - 1];
                             Looms_Info = Constants.arrDEabove[comDEabove, Constants.colDE_Looms - 1];
                         }
                         #endregion
                         break;
                     }                    
                 }
                     #endregion

                 #region// from arrDBBelow get  CompType,SampleEquPinNumber
                 for (int comDEbelow = 0; comDEbelow <= Constants.arrDEbelow.GetLength(0) - 1; comDEbelow++)
                 {
                      if (Constants.arrDEbelow[comDEbelow, Constants.colDE_Connector - 1].Equals(strComponentName)  
                        && (Constants.arrDEbelow[comDEbelow, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.cont) || Constants.arrDEbelow[comDEbelow, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.ind) || Constants.arrDEbelow[comDEbelow, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.ter) || Constants.arrDEbelow[comDEbelow, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.tbk) || Constants.arrDEbelow[comDEbelow, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.bus) || Constants.arrDEbelow[comDEbelow, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.cnt) || Constants.arrDEbelow[comDEbelow, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.ant)  || Constants.arrDEbelow[comDEbelow, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.dd) || Constants.arrDEbelow[comDEbelow, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.snr) || Constants.arrDEbelow[comDEbelow, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.rel) || Constants.arrDEbelow[comDEbelow, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.spl) || Constants.arrDEbelow[comDEbelow, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.cb_s) || Constants.arrDEbelow[comDEbelow, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.cb_t)) 
                         && Constants.arrDEbelow[comDEbelow, Constants.colDE_Panels - 1].Equals(panelName)) //&& Constants.arrDEbelow[comDEbelow, Constants.colDE_PartNumber - 1] != "")
                      {
                         CompType = Constants.arrDEbelow[comDEbelow, Constants.colDE_Type - 1];
                         CBType_Name = Constants.arrDEbelow[comDEbelow, Constants.colDE_CBType_Name - 1];
                         CBType_Voltage = Constants.arrDEbelow[comDEbelow, Constants.colDE_CBType_Voltage - 1];
                         SampleEquPinNumber = Constants.arrDEbelow[comDEbelow, Constants.colDE_PinNumber - 1];

                         #region//Adding shunt values for Terminal Block - TER,TBK COMPONENT TYPE
                         if (CompType.Equals("TBK") || CompType.Equals("TER"))
                         {                            
                             for (int shuntDEbelow = 0; shuntDEbelow <= Constants.arrDEbelow.GetLength(0) - 1; shuntDEbelow++)
                             {
                                 if (Constants.arrDEbelow[comDEbelow, Constants.colDE_Connector - 1].Equals(strComponentName) && !string.IsNullOrEmpty(Constants.arrDEbelow[shuntDEbelow, Constants.colDE_TBKTER_Shunt_Value - 1]))
                                 {
                                     if (string.IsNullOrEmpty(Constants.strDEBelowTERTBK_Shunt_List))
                                     {
                                         Constants.strDEBelowTERTBK_Shunt_List = string.Concat(Constants.arrDEbelow[shuntDEbelow, Constants.colDE_Connector - 1], ",", Constants.arrDEbelow[shuntDEbelow, 7], ",", Constants.arrDEbelow[shuntDEbelow, 8], ",", Constants.arrDEbelow[shuntDEbelow, 11], ",", Constants.arrDEbelow[shuntDEbelow, Constants.colDE_TBKTER_Shunt_Value - 1]);
                                     }
                                     else
                                     {
                                         Constants.strDEBelowTERTBK_Shunt_List = string.Concat(Constants.strDEBelowTERTBK_Shunt_List, ";", string.Concat(Constants.arrDEbelow[shuntDEbelow, Constants.colDE_Connector - 1], ",", Constants.arrDEbelow[shuntDEbelow, 7], ",", Constants.arrDEbelow[shuntDEbelow, 8], ",", Constants.arrDEbelow[shuntDEbelow, 11], ",", Constants.arrDEbelow[shuntDEbelow, Constants.colDE_TBKTER_Shunt_Value - 1]));
                                     } 
                                 }
                             }
                             for (int shuntDEbelow = 0; shuntDEbelow <= Constants.arrDEabove.GetLength(0) - 1; shuntDEbelow++)
                             {
                                 if (Constants.arrDEabove[comDEbelow, Constants.colDE_Connector - 1].Equals(strComponentName) && !string.IsNullOrEmpty(Constants.arrDEabove[shuntDEbelow, Constants.colDE_TBKTER_Shunt_Value - 1]))
                                 {
                                     if (string.IsNullOrEmpty(Constants.strDEBelowTERTBK_Shunt_List))
                                     {
                                         Constants.strDEBelowTERTBK_Shunt_List = string.Concat(Constants.arrDEabove[shuntDEbelow, Constants.colDE_Connector - 1], ",", Constants.arrDEabove[shuntDEbelow, 7], ",", Constants.arrDEabove[shuntDEbelow, 8], ",", Constants.arrDEbelow[shuntDEbelow, 11], ",", Constants.arrDEabove[shuntDEbelow, Constants.colDE_TBKTER_Shunt_Value - 1]);
                                     }
                                     else
                                     {
                                         Constants.strDEBelowTERTBK_Shunt_List = string.Concat(Constants.strDEBelowTERTBK_Shunt_List, ";", string.Concat(Constants.arrDEabove[shuntDEbelow, Constants.colDE_Connector - 1], ",", Constants.arrDEabove[shuntDEbelow, 7], ",", Constants.arrDEabove[shuntDEbelow, 8], ",", Constants.arrDEabove[shuntDEbelow, 11], ",", Constants.arrDEabove[shuntDEbelow, Constants.colDE_TBKTER_Shunt_Value - 1]));
                                     }
                                 }
                             }
                         }
                         #endregion
                         break;
                      }
                 }
                 #endregion

                 string CompAcc = string.Empty;
                 string MacroName = string.Empty;
                 string CompMaxPin = string.Empty;
                 string CompDwgName = string.Empty;
                 CompDwgName = strComponentName;
                 //To check in accessory column first
                 rownum = modOPCommand.RowOfFoundStringInColOfMDarray(CompPN, Constants.arrTableOfLibCatalog, Constants.colLibCatalog_Accessory-1);
                 //If its not there in accessory then in internal part number column
                 if (rownum == 0) rownum = modOPCommand.RowOfFoundStringInColOfMDarray(CompPN, Constants.arrTableOfLibCatalog, Constants.colLibCatalog_InternalPartNumber-1);
                 CompPN = Constants.arrTableOfLibCatalog[rownum, Constants.colLibCatalog_InternalPartNumber-1];
                 CompAcc = Constants.arrTableOfLibCatalog[rownum, Constants.colLibCatalog_Accessory-1];
                 MacroName = Constants.arrTableOfLibCatalog[rownum, Constants.colLibCatalog_Macro-1];
                 CompMaxPin = Constants.arrTableOfLibCatalog[rownum, Constants.ColLibCatalog_MaxPin-1];

                 Constants.arrPanelComponentsProperties[PanelComp, 0] = CompDwgName;
                 Constants.arrPanelComponentsProperties[PanelComp, 1] = CompType;
                 Constants.arrPanelComponentsProperties[PanelComp, 2] = MacroName;
                 Constants.arrPanelComponentsProperties[PanelComp, 3] = CompMaxPin;
                 Constants.arrPanelComponentsProperties[PanelComp, 4] = CompPN;
                 Constants.arrPanelComponentsProperties[PanelComp, 5] = CompAcc;
                 Constants.arrPanelComponentsProperties[PanelComp, 6] = SampleEquPinNumber;
                 Constants.arrPanelComponentsProperties[PanelComp, 7] = GroupId;
                 Constants.arrPanelComponentsProperties[PanelComp, 8] = Wire_Length;
                 Constants.arrPanelComponentsProperties[PanelComp, 9] = Wire_Type;
                 Constants.arrPanelComponentsProperties[PanelComp, 10] = CBType_Name;
                 Constants.arrPanelComponentsProperties[PanelComp, 11] = CBType_Voltage;
                 Constants.arrPanelComponentsProperties[PanelComp, 12] = Constants.strDEPanelaboveAssosiatePNList;
                 Constants.arrPanelComponentsProperties[PanelComp, 13] = Equipment_Box_Info;
                 Constants.arrPanelComponentsProperties[PanelComp, 14] = Looms_Info;
                 Constants.arrPanelComponentsProperties[PanelComp, 15] = Constants.strDEBelowTERTBK_Shunt_List;

                 PanelComp = PanelComp + 1;
                 Constants.strDEPanelaboveAssosiatePNList = string.Empty;
                 Constants.strDEBelowTERTBK_Shunt_List = string.Empty;
             }
         }*/
        #endregion

        #region //Commented old ReadMyData code on 19 november, 2025
        //public static void ReadMyData(string sMyData_File)
        //{
        //    try
        //    {
        //        Constants.arr_My_Data = ExcelOperations.ConvertCSVDataInto2DArray(sMyData_File, StringComparer.OrdinalIgnoreCase); //Parsing of My_Data file
        //        modMain.ReadPanelDetails(); // Parsing of Panel Details file
        //        AssignOrientation();//Assign AssignOrientation from My_Data file to PD array
        //        CoordinatesToPDPin();//Assign CoordinatesToPDPin from My_Data file to PD array
        //        //AssignOrientation();//Assign AssignOrientation from My_Data file to PD array
        //        DrawWireLine();//Generate Output Command file that is "el_exec" file
        //        //Parsing of My_Data file & PD file,Assign CoordinatesToPDPin from My_Data file to PD array,Assign AssignOrientation from My_Data file to PD array,DrawWireLine
        //    }
        //    catch (Exception ex)
        //    {
        //        // Optional: Log or display error
        //        MessageBox.Show(ex.Message);
        //        return;
        //        //Console.WriteLine("Error: " + ex.Message);
        //    }
        //}
        #endregion

        #region //Commented old CoordinatesToPDPin code on 19 november, 2025
        //public static void CoordinatesToPDPin()
        //{
        //    for (int k = 0; k <= Constants.arrPanelDetails.GetLength(0)-1; k++)
        //    {
        //        //if (!Constants.arrPanelDetails[k, Constants.colPD_Usage-1].Equals("1"))
        //       //if (!Constants.arrPanelDetails[k, Constants.colPD_Usage - 1].Equals("1") && Constants.arrPanelDetails[k, Constants.colPD_F_Ori - 1].Equals("L"))
        //           // {
        //            bool Tdetails = false;
        //            bool Fdetails = false;

        //            for (int n = 0; n <= Constants.arr_My_Data.GetLength(0)-1; n++)
        //            {
        //                if (!Tdetails &&
        //                    Constants.arr_My_Data[n, Constants.colMD_Connector - 1] == Constants.arrPanelDetails[k, Constants.colPD_T_Connector - 1] &&
        //                    Constants.arr_My_Data[n, Constants.colMD_PinNumber - 1] == Constants.arrPanelDetails[k, Constants.colPD_T_Pin - 1] &&
        //                    //string.Equals(Constants.arr_My_Data[n, Constants.colMD_SheetName - 1], "Panel_Drawing", StringComparison.OrdinalIgnoreCase))
        //                    string.Equals(Constants.arr_My_Data[n, Constants.colMD_SheetName - 1], Constants.textPanelPartNumber, StringComparison.OrdinalIgnoreCase))
        //                {
        //                    Constants.arrPanelDetails[k, Constants.colPD_T_PinX-1] = Constants.arr_My_Data[n, Constants.colMD_PinX-1];
        //                    Constants.arrPanelDetails[k, Constants.colPD_T_PinY-1] = Constants.arr_My_Data[n, Constants.colMD_PinY-1];
        //                    Constants.arrPanelDetails[k, Constants.colPD_T_Type-1] = Constants.arr_My_Data[n, Constants.colMD_Type-1];

        //                    int rownumTo = modOPCommand.RowOfFoundStringInColOfMDarray(Constants.arr_My_Data[n, Constants.colMD_PinNumber - 1], Constants.arrPanelDetails, Constants.colPD_F_Pin - 1);
        //                    Constants.arrPanelDetails[rownumTo, Constants.colPD_Usage - 1] = Constants.arr_My_Data[n, 14];
        //                    Constants.arrPanelDetails[k, Constants.colPD_Usage - 1] = Constants.arr_My_Data[n, 14];

        //                    Tdetails = true;
        //                }

        //                if (!Fdetails &&
        //                    Constants.arr_My_Data[n, Constants.colMD_Connector-1] == Constants.arrPanelDetails[k, Constants.colPD_F_Connector - 1] &&
        //                    Constants.arr_My_Data[n, Constants.colMD_PinNumber - 1] == Constants.arrPanelDetails[k, Constants.colPD_F_Pin - 1] &&
        //                    //string.Equals(Constants.arr_My_Data[n, Constants.colMD_SheetName - 1], "Panel_Drawing", StringComparison.OrdinalIgnoreCase))
        //                    string.Equals(Constants.arr_My_Data[n, Constants.colMD_SheetName - 1], Constants.textPanelPartNumber, StringComparison.OrdinalIgnoreCase))
        //                    //Panel_Drawing
        //                {
        //                    Constants.arrPanelDetails[k, Constants.colPD_F_PinX-1] = Constants.arr_My_Data[n, Constants.colMD_PinX-1];
        //                    Constants.arrPanelDetails[k, Constants.colPD_F_PinY-1] = Constants.arr_My_Data[n, Constants.colMD_PinY-1];
        //                    Constants.arrPanelDetails[k, Constants.colPD_F_Type-1] = Constants.arr_My_Data[n, Constants.colMD_Type-1];

        //                    int rownumFrom = modOPCommand.RowOfFoundStringInColOfMDarray(Constants.arr_My_Data[n, Constants.colMD_PinNumber - 1], Constants.arrPanelDetails, Constants.colPD_T_Pin - 1);
        //                    Constants.arrPanelDetails[rownumFrom, Constants.colPD_Usage - 1] = Constants.arr_My_Data[n, 14];
        //                    Constants.arrPanelDetails[k, Constants.colPD_Usage - 1] = Constants.arr_My_Data[n,14];

        //                    Fdetails = true;
        //                }

        //                if (Tdetails && Fdetails)
        //                {
        //                    break;
        //                }
        //            }

        //            if (Tdetails && Fdetails)
        //            {
        //                Constants.arr_My_Data[k, Constants.colPD_Usage - 1] = "1";

        //                //Call function with reversed F and T arguments
        //                DeactivateRepetitiveConnections(
        //                    Constants.arr_My_Data[k, Constants.colPD_T_Connector - 1],
        //                    Constants.arr_My_Data[k, Constants.colPD_T_Pin - 1],
        //                    Constants.arr_My_Data[k, Constants.colPD_F_Connector - 1],
        //                    Constants.arr_My_Data[k, Constants.colPD_F_Pin - 1]
        //                );
        //            }
        //        //}

        //        #region//Update PD with GroupId,Wire Length,Wire Type
        //        int PDrownum = modOPCommand.RowOfFoundStringsInColOfMDarray(Constants.arrPanelDetails[k, Constants.colPD_F_Connector - 1], Constants.arrPanelDetails[k, Constants.colPD_F_Pin - 1], Constants.arrPanelDetails[k, Constants.colPD_WireCode - 1] , Constants.arrDEbelow, Constants.colDE_Connector - 1, Constants.colDE_PinNumber - 1, Constants.colDE_WireCode-1);
        //        if (PDrownum > -1)
        //        {
        //            Constants.arrPanelDetails[k, Constants.colPD_F_GroupId - 1] = Constants.arrDEbelow[PDrownum, Constants.colDE_GroupId - 1];
        //            Constants.arrPanelDetails[k, Constants.colPD_F_Wire_Length - 1] = Constants.arrDEbelow[PDrownum, Constants.colDE_Wire_Length - 1];
        //            Constants.arrPanelDetails[k, Constants.colPD_F_Wire_Type - 1] = Constants.arrDEbelow[PDrownum, Constants.colDE_Wire_Type - 1];
        //            Constants.arrPanelDetails[k, Constants.colPD_F_Wire_Type_Number - 1] = Constants.arrDEbelow[PDrownum, Constants.colDE_PartNumber - 1];
        //        }
        //        #endregion
        //    }
        //}
        #endregion

        #region //Commented old DeactivateRepetitiveConnections code on 19 november, 2025
        //public static void DeactivateRepetitiveConnections(string FC, string FP, string TC, string TP)
        //{
        //    for (int k = 0; k <= Constants.arrPanelDetails.GetLength(0)-1; k++)
        //    {
        //        if (Constants.arr_My_Data[k, Constants.colPD_F_Connector-1] == FC &&
        //            Constants.arr_My_Data[k, Constants.colPD_F_Pin-1] == FP &&
        //            Constants.arr_My_Data[k, Constants.colPD_T_Connector - 1] == TC &&
        //            Constants.arr_My_Data[k, Constants.colPD_T_Pin-1] == TP)
        //        {
        //            Constants.arr_My_Data[k, Constants.colPD_Usage - 1] = "2";
        //            break;
        //        }
        //    }
        //}
        #endregion

        #region //Commented old AssignOrientation code on 19 november, 2025
        //public static void AssignOrientation()
        //{
        //    List<string> OriAssignedEQU = new List<string>();

        //    for (int k = 0; k <= Constants.arrPanelDetails.GetLength(0)-1; k++)
        //    {
        //        //if (Constants.arrPanelDetails[k, Constants.colPD_Usage-1] == "1")
        //        //{
        //            string c1 = Constants.arrPanelDetails[k, Constants.colPD_F_Connector - 1];
        //            string c2 = Constants.arrPanelDetails[k, Constants.colPD_T_Connector-1];
        //            string F_Type = Constants.arrPanelDetails[k, Constants.colPD_F_Type - 1];
        //            string T_Type = Constants.arrPanelDetails[k, Constants.colPD_T_Type-1];

        //        if (F_Type == "EQU")
        //        {
        //            Constants.txtEquName = c1;
        //        }

        //        if (modStandard.SearchAndAppend(Constants.txtEquName, OriAssignedEQU))
        //        {
        //            Constants.sPanelEQUori_File = string.Concat(Constants.Electre_Temp_Folder_Path, Constants.textPanelPartNumber, " " + "- " + Constants.txtEquName, " - PanelEquOri.txt");//
        //            if (!modStandard.ValidateFileSelection(Constants.sPanelEQUori_File))
        //                {
        //                    Console.WriteLine($"{Constants.sPanelEQUori_File} - PanelOri file is missing");
        //                    // Optionally continue or break here
        //                    continue;
        //                }
        //                else
        //                {
        //                    try
        //                    {
        //                        foreach (string line in File.ReadLines(Constants.sPanelEQUori_File))
        //                        {
        //                            string[] arrtemp1 = line.Split(';');
        //                            if (arrtemp1.Length < 3) continue;

        //                            string pinNumber = arrtemp1[1];
        //                            string orientation = arrtemp1[2];

        //                            // Assign to F_Ori
        //                            for (int s = 0; s <= Constants.arrPanelDetails.GetLength(0)-1; s++)
        //                            {
        //                            //if (Constants.arrPanelDetails[s, Constants.colPD_F_Connector-1] == Constants.txtEquName &&
        //                            //    Constants.arrPanelDetails[s, Constants.colPD_F_Pin-1] == pinNumber)
        //                            if (Constants.arrPanelDetails[s, Constants.colPD_F_Connector - 1].Equals(Constants.txtEquName) && //== Constants.txtEquName &&
        //                                Constants.arrPanelDetails[s, Constants.colPD_F_Pin - 1].Equals(pinNumber))
        //                            {
        //                                Constants.arrPanelDetails[s, Constants.colPD_F_Ori - 1] = orientation;
        //                                    break;
        //                                }
        //                            }

        //                            // Assign to T_Ori
        //                            for (int t = 0; t <= Constants.arrPanelDetails.GetLength(0)-1; t++)
        //                            {
        //                                if (Constants.arrPanelDetails[t, Constants.colPD_T_Connector-1] == Constants.txtEquName &&
        //                                    Constants.arrPanelDetails[t, Constants.colPD_T_Pin-1] == pinNumber)
        //                                {
        //                                Constants.arrPanelDetails[t, Constants.colPD_T_Ori - 1] = orientation;
        //                                    break;
        //                                }
        //                            }
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        Console.WriteLine($"Error reading file: {ex.Message}");
        //                    }
        //                }
        //            }
        //        //}
        //    }
        //}
        #endregion

        #region //Commented old DrawWireLine code on 19 november, 2025
        //public static void DrawWireLine()
        //{
        //    double X1=0, Y1=0, X2=0, Y2=0;
        //    string c1 = string.Empty, c2 = string.Empty;
        //    string F_Type = string.Empty, T_Type = string.Empty;
        //    string F_Ori = string.Empty, T_Ori = string.Empty;
        //    string WireCode = string.Empty,GroupId = string.Empty,Wire_Length = string.Empty, Wire_Type = string.Empty, Wire_Type_Number = string.Empty, strPin = string.Empty;
        //    Constants.lst_WireCodes_Info_Processed = new List<string>();
        //    // Write to file or console as in VB6: Print #1, "GRID 1,1 ;"
        //    // Assuming output is redirected to a file or console here:
        //    //Console.WriteLine("GRID 2.0,2 ;");
        //    using (writer = File.AppendText(Constants.el_ExecFilePath))
        //    {
        //        //writer.WriteLine("GRID 2.0,2 ;");//, Constants.arrPanelDetails.GetLength(0)));
        //    }

        //    //int totalcount = Constants.arrPanelDetails.GetLength(0);
        //    //int totalcount = Constants.arrPanelDetails.GetLength(0) / 2;
        //    //var f = Constants.arrPanelDetails.
        //    //for (int k = 0; k <= 1; k++)
        //    for (int k = 0; k <= Constants.arrPanelDetails.GetLength(0) - 1; k++)
        //    {
        //        //if (!Constants.arrPanelDetails[k, Constants.colPD_Usage-1].Equals(strPin))
        //        //{
        //            if (string.IsNullOrEmpty(Constants.arrPanelDetails[k, Constants.colPD_F_PinX - 1])) continue;
        //            if (string.IsNullOrEmpty(Constants.arrPanelDetails[k, Constants.colPD_F_PinY - 1])) continue;
        //            if (string.IsNullOrEmpty(Constants.arrPanelDetails[k, Constants.colPD_T_PinX - 1])) continue;
        //            if (string.IsNullOrEmpty(Constants.arrPanelDetails[k, Constants.colPD_T_PinY - 1])) continue;
        //        X1 = Convert.ToDouble(Constants.arrPanelDetails[k, Constants.colPD_F_PinX - 1]);
        //        //Convert.ToDouble(string.IsNullOrEmpty(Constants.arrPanelDetails[k, Constants.colPD_F_PinX-1])?0:Constants.arrPanelDetails[k, Constants.colPD_F_PinX - 1]);
        //        Y1 = Convert.ToDouble(Constants.arrPanelDetails[k, Constants.colPD_F_PinY - 1]); 
        //        //Convert.ToDouble(string.IsNullOrEmpty(Constants.arrPanelDetails[k, Constants.colPD_F_PinY-1])?0:Constants.arrPanelDetails[k, Constants.colPD_F_PinY - 1]);
        //            X2 = Convert.ToDouble(Constants.arrPanelDetails[k, Constants.colPD_T_PinX - 1]);
        //        //Convert.ToDouble(string.IsNullOrEmpty(Constants.arrPanelDetails[k, Constants.colPD_T_PinX-1])?0:Constants.arrPanelDetails[k, Constants.colPD_T_PinX - 1]);
        //        Y2 = Convert.ToDouble(Constants.arrPanelDetails[k, Constants.colPD_T_PinY - 1]);
        //        //Convert.ToDouble(string.IsNullOrEmpty(Constants.arrPanelDetails[k, Constants.colPD_T_PinY-1])?0:Constants.arrPanelDetails[k, Constants.colPD_T_PinY - 1]);
        //                                                                                        //Y1 = Convert.ToDouble(Constants.arrPanelDetails[k, Constants.colPD_F_PinY-1]);
        //                                                                                        //X2 = Convert.ToDouble(Constants.arrPanelDetails[k, Constants.colPD_T_PinX - 1]);
        //                                                                                        //Y2 = Convert.ToDouble(Constants.arrPanelDetails[k, Constants.colPD_T_PinY-1]);
        //            c1 = Constants.arrPanelDetails[k, Constants.colPD_F_Connector-1];
        //            c2 = Constants.arrPanelDetails[k, Constants.colPD_T_Connector - 1];
        //            F_Type = Constants.arrPanelDetails[k, Constants.colPD_F_Type-1];
        //            T_Type = Constants.arrPanelDetails[k, Constants.colPD_T_Type - 1];
        //            WireCode = Constants.arrPanelDetails[k, Constants.colPD_WireCode-1];
        //            F_Ori = Constants.arrPanelDetails[k, Constants.colPD_F_Ori - 1];
        //            T_Ori = Constants.arrPanelDetails[k, Constants.colPD_T_Ori-1];
        //            strPin = Constants.arrPanelDetails[k, Constants.colPD_Usage - 1];
        //            GroupId = Constants.arrPanelDetails[k, Constants.colPD_F_GroupId-1];
        //            Wire_Length = Constants.arrPanelDetails[k, Constants.colPD_F_Wire_Length - 1];
        //            Wire_Type = Constants.arrPanelDetails[k, Constants.colPD_F_Wire_Type - 1];
        //            Wire_Type_Number = Constants.arrPanelDetails[k, Constants.colPD_F_Wire_Type_Number - 1];

        //            if (k == 33)
        //            {
        //                Console.WriteLine("J1 check");
        //            }
        //        if (X1 < X2)
        //        {
        //            ConnectionRequired(X1, Y1, X2, Y2, c1, c2, WireCode, F_Type, T_Type, F_Ori, T_Ori, GroupId, Wire_Length, Wire_Type, Wire_Type_Number);
        //        }
        //    }
        //    //}
        //}
        #endregion

        #region //Commented old ConnectionRequired code on 19 november, 2025
        //private static void ConnectionRequired(double p1x, double p1y, double p2x, double p2y,string c1, string c2, string wCode, string iF_Type, string iT_Type,string iF_Ori, string iT_Ori,string groupId,string wire_Length,string wire_Type, string wire_Type_Core_Number)
        //{
        //    string gauge = string.Empty;
        //    string wire_code = string.Empty;
        //    string wire_code_with_gauge = string.Empty;

        //    // Split WCode into code and gauge
        //    var arrTemp = wCode.Split('/');
        //    wire_code = arrTemp[0];
        //    gauge = arrTemp.Length > 1 ? arrTemp[1] : "";
        //    wire_code_with_gauge = string.Concat(wire_code, "/", gauge);
        //    //wire_code_with_gauge = string.Concat(arrTemp[0], "/", arrTemp[1]);
        //    //Constants.lst_WireCodes_Info.Add();
        //    //if (!Constants.lst_WireCodes_Info_Processed.Contains(wire_code))
        //    //    Constants.lst_WireCodes_Info_Processed.Add(wire_code);
        //    //else { //return;
        //    //       }
        //    // Check if it's a straight line
        //    if (p1y == p2y)
        //        //if (p1x == p2x || p1y == p2y)
        //    {
        //        if (c1 == c2 && p1x == p2x)
        //        {
        //            // Do nothing - overlapping wire scenario
        //        }
        //        else
        //        {
        //            //if (wire_Type.Equals("86A9S") || wire_Type.Equals("86A9SS") || wire_Type.Equals("S") || wire_Type.Equals("S0") || wire_Type.Equals("S00") || wire_code.StartsWith("SS"))
        //            if (wire_Type.Equals("86A9S") || wire_Type.Equals("86A9SS") || wire_Type.Equals("S") || wire_Type.Equals("S0") || wire_Type.Equals("S00") || wire_code.StartsWith("SS") || wire_Type.Equals("COAX") || wire_Type.Equals("TRIAX"))//coax,sth_coax,triax,sth_triax
        //                modOPCommand.Simple2PointConnection_Mono_StraightLine(p1x, p1y, p2x, p2y, wire_code, gauge, c1, c2, iF_Type, iT_Type, groupId,wire_Length, wire_Type, wire_Type_Core_Number);
        //            else if (wire_Type.Equals("TP") || wire_Type.Equals("QUADRAX"))//Quadrax,sth_quadrax4
        //                modOPCommand.Simple2PointConnection_TP_StraightLine(p1x, p1y, p2x, p2y, wire_code, gauge, c1, c2, iF_Type, iT_Type, groupId, wire_Length, wire_Type, wire_Type_Core_Number);
        //            else if (wire_Type.Equals("STP"))
        //                modOPCommand.Simple2PointConnection_STP_StraightLine(p1x, p1y, p2x, p2y, wire_code, gauge, c1, c2, iF_Type, iT_Type, groupId, wire_Length, wire_Type, wire_Type_Core_Number);
        //            else { }

        //        }
        //    }
        //    else
        //    {
        //        if ((iF_Ori == "T" || iF_Ori == "B") || (iT_Ori == "T" || iT_Ori == "B"))
        //        {
        //            //modOPCommand.Simple3PointConnection(p1x + 4, p1y, p2x + 4, p2y, wire_code, gauge, c1, c2, iF_Type, iT_Type, iF_Ori, iT_Ori);
        //        }
        //        else//Z LINE
        //        {
        //            modOPCommand.Simple4PointConnection_ZLine_Optimized(p1x, p1y, p2x, p2y, wire_code, gauge, c1, c2, iF_Type, iT_Type, groupId, wire_Length, wire_Type, wire_Type_Core_Number);
        //            #region//B4 Code Optimized
        //            ////if (wire_code.StartsWith("S") || wire_code.StartsWith("SS"))
        //            //if (wire_Type.Equals("86A9S") || wire_Type.Equals("86A9SS") || wire_Type.Equals("S") || wire_Type.Equals("S0") || wire_Type.Equals("S00") || wire_code.StartsWith("SS") || wire_Type.Equals("COAX") || wire_Type.Equals("TRIAX"))//coax,sth_coax,triax,sth_triax
        //            //    modOPCommand.Simple4PointConnection_Mono_ZLine(p1x, p1y, p2x, p2y, wire_code, gauge, c1, c2, iF_Type, iT_Type, groupId, wire_Length, wire_Type, wire_Type_Core_Number);
        //            //else if (wire_Type.Equals("TP") || wire_Type.Equals("QUADRAX"))//Quadrax,sth_quadrax4
        //            //    modOPCommand.Simple4PointConnection_TP_ZLine(p1x, p1y, p2x, p2y, wire_code, gauge, c1, c2, iF_Type, iT_Type, groupId, wire_Length, wire_Type, wire_Type_Core_Number);
        //            //else if (wire_Type.Equals("STP"))
        //            //    modOPCommand.Simple4PointConnection_STP_ZLine(p1x, p1y, p2x, p2y, wire_code, gauge, c1, c2, iF_Type, iT_Type, groupId, wire_Length, wire_Type, wire_Type_Core_Number);
        //            //else { }
        //            #endregion
        //        }
        //    }
        //}
        #endregion

        //public static (double X, double Y) GetNextComponentPosition(double compWidth, double compHeight)
        //{
        //    // Wrap to new row if needed
        //    if (Constants.CursorX + compWidth > Constants.SheetWidthTemp - Constants.MarginX)
        //    {
        //        Constants.CursorX = Constants.MarginX;
        //        Constants.CursorY += Constants.RowHeight + Constants.ComponentSpacingY;
        //        Constants.RowHeight = 0;
        //    }

        //    double posX = Constants.CursorX;
        //    double posY = Constants.CursorY;

        //    // Move horizontal cursor
        //    Constants.CursorX += compWidth + Constants.ComponentSpacingX;

        //    // Track tallest in row
        //    if (compHeight > Constants.RowHeight)
        //        Constants.RowHeight = compHeight;

        //    return (posX, posY);
        //}


        //public static void InitializeLayout()
        //{
        //    Constants.CursorX = Constants.MarginX;
        //    Constants.CursorY = Constants.MarginY;

        //    Constants.RowHeight = 0;

        //    Constants.SheetWidthTemp = Constants.SheetWidth;
        //    Constants.SheetHeightTemp = Constants.SheetHeight;
        //}


        //**********************************************************
        #region//OLD GraLine - BackUp
        //public static void GraLine(double X0, double Y0, int iNumberOfPins,string CompName, string[] arrpinsCollection,string partnumber)
        //{
        //    //if (string.IsNullOrEmpty(iNumberOfPins.ToString()))
        //    //    iNumberOfPins = 0;
        //    //iNumberOfPins = arrpinsCollection.Length;
        //    string BC_Comp_Nmae = string.Empty;//Break Connector Component Name with out Male or Female
        //    BC_Comp_Nmae = CompName.Substring(0, CompName.Length - 2);
        //    //Constants.remainingItems.Contains(BC_Comp_Nmae)
        //    int incre = -4;
        //    int T = 0;
        //    T = arrpinsCollection.Length;
        //    if (!Constants.remainingItems.Contains(CompName) && BC_Comp_Nmae.Equals(CompName.Substring(0, CompName.Length - 2)))
        //    {
        //        Constants.remainingItems.Add(CompName);

        //        //if (CompName.EndsWith("_F") || CompName.EndsWith("_M"))
        //        //{
        //        modOPCommand.DrawNewHpLine_Length(100.0, 200.0, 100, 0, 123, 22, Constants.el_ExecFilePath);
        //        modOPCommand.DrawNewHpLine_Length(100.0, 200.0, 100, 0, 123, 22, Constants.el_ExecFilePath);


        //        // X0 = 266;
        //        // Y0 = 190;

        //        modOPCommand.DisSeg1(X0, Y0, Constants.el_ExecFilePath);//Hearder

        //        //int incre = -4;
        //        //int T = 0;
        //        T = arrpinsCollection.Length*2;
        //        for (int N = 0; N <= arrpinsCollection.Length - 1; N++)//Orig for loop
        //        {
        //            modOPCommand.DisSeg2(X0, Y0 + incre * N, arrpinsCollection[N], Constants.el_ExecFilePath,"L");
        //        }
        //        //modOPCommand.DisSeg2(X0, Y0 + incre, "N", Constants.el_ExecFilePath);
        //        modOPCommand.DisSeg3(X0, Y0, incre * (iNumberOfPins), Constants.el_ExecFilePath); // vertical lines 

        //        //modOPCommand.DisSeg4(X0, Y0 - (Math.Abs(incre) * (T)), X0, Y0 + (incre*T), CompName, Constants.el_ExecFilePath);//Rectongler
        //        modOPCommand.DisSeg4(X0, Y0 - (Math.Abs(incre) * (T)), X0, Y0, CompName, Constants.el_ExecFilePath,"L", partnumber);//Rectongler
        //        //modOPCommand.DisSeg4(X0, Y0 + incre * iNumberOfPins, CompName, Constants.el_ExecFilePath);//Rectongles
        //        //X0, Y0 - (Math.Abs(incre_y) * (T)), X0, Y0, iEquName, Ori, Constants.el_ExecFilePath);//Rectongler
        //        modOPCommand.DisSeg5(X0, Y0 + incre * iNumberOfPins, Constants.el_ExecFilePath);//footer
        //                                                                                        //}
        //    }
        //    else 
        //    {
        //        for (int N = 0; N <= arrpinsCollection.Length - 1; N++)//Orig for loop
        //        {
        //            modOPCommand.DisSeg2(X0, Y0 + incre * N, arrpinsCollection[N], Constants.el_ExecFilePath,"R");
        //        }
        //        modOPCommand.DisSeg4(X0, Y0 - (Math.Abs(incre) * (T)), X0, Y0, CompName, Constants.el_ExecFilePath,"R", partnumber);//Rectongler
        //    }
        //    #region
        //    //if (CompName.EndsWith("_F"))
        //    //{
        //    //    modOPCommand.DrawNewHpLine_Length(100.0, 200.0, 100, 0, 123, 22, Constants.el_ExecFilePath);
        //    //    modOPCommand.DrawNewHpLine_Length(100.0, 200.0, 100, 0, 123, 22, Constants.el_ExecFilePath);

        //    //    int incre = -6;
        //    //    // X0 = 266;
        //    //    // Y0 = 190;

        //    //    modOPCommand.DisSeg1(X0, Y0, Constants.el_ExecFilePath);//Hearder
        //    //    modOPCommand.DisSeg5(X0, Y0 + incre * iNumberOfPins, Constants.el_ExecFilePath);//footer
        //    //    modOPCommand.DisSeg3(X0, Y0, incre * (iNumberOfPins), Constants.el_ExecFilePath); // vertical lines 
        //    //    modOPCommand.DisSeg4(X0, Y0, CompName, Constants.el_ExecFilePath);//Rectongle

        //    //    for (int N = 0; N <= arrpinsCollection.Length - 1; N++)//Orig for loop
        //    //    {
        //    //        modOPCommand.DisSeg2(X0, Y0 + incre * N, arrpinsCollection[N], Constants.el_ExecFilePath);
        //    //    }
        //    //    //modOPCommand.DisSeg2(X0, Y0 + incre, "N", Constants.el_ExecFilePath);
        //    //    //modOPCommand.DisSeg3(X0, Y0, incre * (iNumberOfPins), Constants.el_ExecFilePath); // vertical lines 

        //    //    //modOPCommand.DisSeg4(X0, Y0, CompName, Constants.el_ExecFilePath);//Rectongle

        //    //    //modOPCommand.DisSeg5(X0, Y0 + incre * iNumberOfPins, Constants.el_ExecFilePath);//footer
        //    //}
        //    #endregion
        //}
        #endregion

        #region//Commented on 18 november, 2025 => New GraLine for Break Connector - DIS comp type
        //public static void GraLine(double X0, double Y0, int iNumberOfPins, string CompName, string[] arrpinsCollection, string partnumber,string assopns,string DisEqupmentBoxrefname)
        //{
        //    string BC_Comp_Nmae = string.Empty;//Break Connector Component Name with out Male or Female

        //    BC_Comp_Nmae = CompName.Substring(0, CompName.Length - 2);
        //    if (Constants.processedItems.Count > 0 && Constants.processedItems.Contains(BC_Comp_Nmae)) return;
        //    int incre = -4;
        //    int T = 0;
        //    string tempstr = string.Empty, strDISOrientation = string.Empty, strTempCompName = string.Empty;
        //    T = arrpinsCollection.Length;
        //    for (int df = 0; df <= Constants.arrDEbelow.GetLength(0)-1; df++)//cont_sth_mg - Left Orientation  cont_sth_md - Right Orientation
        //    {
        //        if(CompName.Equals(Constants.arrDEbelow[df, Constants.colDE_Connector - 1])) tempstr = Constants.arrDEbelow[df, Constants.colDE_Info - 1].Trim();
        //        if (tempstr.Equals("cont_sth_md") || tempstr.Equals("cont_sth_mg")) break;
        //    }

        //    if (tempstr.Equals("contact_sth_receptacle"))
        //    {
        //        strTempCompName = CompName;
        //        if (strTempCompName.EndsWith("_F")) strTempCompName = strTempCompName.Replace("_F", "_M");
        //        else if (strTempCompName.EndsWith("_M")) strTempCompName = strTempCompName.Replace("_M", "_F");
        //        for (int df = 0; df <= Constants.arrDEbelow.GetLength(0) - 1; df++)//cont_sth_mg - Left Orientation  cont_sth_md - Right Orientation
        //        {
        //            if (strTempCompName.Equals(Constants.arrDEbelow[df, Constants.colDE_Connector - 1])) tempstr = Constants.arrDEbelow[df, Constants.colDE_Info - 1].Trim();
        //            if (tempstr.Equals("cont_sth_md") || tempstr.Equals("cont_sth_mg")) break;
        //        }
        //        X0 = X0 - 8;// Constants.CB_Cur_Height_Incre_Count
        //    }
        //    if (tempstr.Equals("cont_sth_md"))
        //    {
        //        strDISOrientation = "R";
        //    }
        //    else if (tempstr.Equals("cont_sth_mg"))
        //    {
        //        strDISOrientation = "L";
        //    }

        //    if (string.IsNullOrEmpty(strDISOrientation))

        //    { 
        //        return; 
        //    }
        //    //cont_sth_md - Left Orientation   cont_sth_mg - Right Ori 
        //    if (!Constants.processedItems.Contains(BC_Comp_Nmae) && BC_Comp_Nmae.Equals(CompName.Substring(0, CompName.Length - 2)))
        //    {
        //        modOPCommand.DisSeg1(X0, Y0, Constants.el_ExecFilePath, strDISOrientation);//Hearder

        //        T = arrpinsCollection.Length;
        //        for (int N = 0; N <= arrpinsCollection.Length - 1; N++)//Orig for loop
        //        {
        //            modOPCommand.DisSeg2(X0, Y0 + incre * N,arrpinsCollection[N], Constants.el_ExecFilePath, strDISOrientation);
        //        }
        //        //modOPCommand.DisSeg2(X0, Y0 + incre, "N", Constants.el_ExecFilePath);
        //        modOPCommand.DisSeg3(X0, Y0, incre * (iNumberOfPins), Constants.el_ExecFilePath, strDISOrientation); // vertical lines 
        //        //modOPCommand.DisSeg4(X0, Y0 - (Math.Abs(incre) * (T)), X0, Y0 + (incre*T), CompName, Constants.el_ExecFilePath);//Rectongler
        //        modOPCommand.DisSeg4(X0, Y0 - (Math.Abs(incre) * (T))-6, X0, Y0, CompName, Constants.el_ExecFilePath, strDISOrientation, partnumber, assopns);//Rectongler
        //        //modOPCommand.DisSeg4(X0, Y0 + incre * iNumberOfPins, CompName, Constants.el_ExecFilePath);//Rectongles
        //        //X0, Y0 - (Math.Abs(incre_y) * (T)), X0, Y0, iEquName, Ori, Constants.el_ExecFilePath);//Rectongler
        //        modOPCommand.DisSeg5(X0, Y0 + incre * iNumberOfPins, Constants.el_ExecFilePath, strDISOrientation);//footer
        //        if (DisEqupmentBoxrefname.Equals("+") || DisEqupmentBoxrefname.Equals("Loc") || DisEqupmentBoxrefname.Equals("LOC") || string.IsNullOrEmpty(DisEqupmentBoxrefname))
        //        {
        //            //modOPCommand.SegEquipmentBox(X0 - 20, Y0 - (Math.Abs(incre_y) * (T)), X0 + 20, Y0, EquBoxExist, Ori, Constants.el_ExecFilePath);//Equipment Box
        //        }
        //        else
        //        {
        //            modOPCommand.DISSegEquipmentBox(X0, Y0 - (Math.Abs(incre) * (T)), X0, Y0, DisEqupmentBoxrefname, strDISOrientation, Constants.el_ExecFilePath);//Equipment Box }
        //        }

        //        Constants.processedItems.Add(BC_Comp_Nmae);
        //    }


        //}
        #region commented old code
        //if (CompName.EndsWith("_F"))
        //{
        //    modOPCommand.DrawNewHpLine_Length(100.0, 200.0, 100, 0, 123, 22, Constants.el_ExecFilePath);
        //    modOPCommand.DrawNewHpLine_Length(100.0, 200.0, 100, 0, 123, 22, Constants.el_ExecFilePath);

        //    int incre = -6;
        //    // X0 = 266;
        //    // Y0 = 190;

        //    modOPCommand.DisSeg1(X0, Y0, Constants.el_ExecFilePath);//Hearder
        //    modOPCommand.DisSeg5(X0, Y0 + incre * iNumberOfPins, Constants.el_ExecFilePath);//footer
        //    modOPCommand.DisSeg3(X0, Y0, incre * (iNumberOfPins), Constants.el_ExecFilePath); // vertical lines 
        //    modOPCommand.DisSeg4(X0, Y0, CompName, Constants.el_ExecFilePath);//Rectongle

        //    for (int N = 0; N <= arrpinsCollection.Length - 1; N++)//Orig for loop
        //    {
        //        modOPCommand.DisSeg2(X0, Y0 + incre * N, arrpinsCollection[N], Constants.el_ExecFilePath);
        //    }
        //    //modOPCommand.DisSeg2(X0, Y0 + incre, "N", Constants.el_ExecFilePath);
        //    //modOPCommand.DisSeg3(X0, Y0, incre * (iNumberOfPins), Constants.el_ExecFilePath); // vertical lines 

        //    //modOPCommand.DisSeg4(X0, Y0, CompName, Constants.el_ExecFilePath);//Rectongle

        //    //modOPCommand.DisSeg5(X0, Y0 + incre * iNumberOfPins, Constants.el_ExecFilePath);//footer
        //}
        #endregion
        #endregion

        //******************************************************************

        #region //Commented old GraLine_DTC(Double type) code on 18 november
        //public static void GraLine_DTC(double X0, double Y0, int iNumberOfPins, string CompName, string[] arrpinsCollection, string partnumber, string assopns, string DTCEqupmentBoxrefname, string BC_Comp_Nmae)//DTC - Double Type Connector
        //{
        //    //string BC_Comp_Nmae = string.Empty;//Break Connector Component Name with out Male or Female

        //    //BC_Comp_Nmae = CompName.Substring(0, CompName.Length - 3);
        //    if (Constants.processedItems.Count > 0 && Constants.processedItems.Contains(BC_Comp_Nmae)) return;
        //    int incre = -4;
        //    int T = 0;
        //    string tempstr = string.Empty, strDISOrientation = string.Empty, strTempCompName = string.Empty;
        //    T = arrpinsCollection.Length;
        //    for (int df = 0; df <= Constants.arrDEbelow.GetLength(0) - 1; df++)//cont_sth_mg - Left Orientation  cont_sth_md - Right Orientation
        //    {
        //        if (CompName.Equals(Constants.arrDEbelow[df, Constants.colDE_Connector - 1])) tempstr = Constants.arrDEbelow[df, Constants.colDE_Info - 1].Trim();
        //        if (tempstr.Equals("cont_sth_md") || tempstr.Equals("cont_sth_mg")) break;
        //    }

        //    if (tempstr.Equals("cont_sth_mg"))
        //    {
        //        strDISOrientation = "L";
        //    }
        //    else if (tempstr.Equals("cont_sth_md"))
        //    {
        //        strDISOrientation = "R";
        //    }

        //    if (string.IsNullOrEmpty(strDISOrientation))
        //    {
        //        if (tempstr.Equals("cont_sth_half_l")) strDISOrientation = "R";
        //        if (tempstr.Equals("cont_sth_half_r")) strDISOrientation = "L";

        //    }
        //    //cont_sth_md - Left Orientation   cont_sth_mg - Right Ori
        //    if (!Constants.processedItems.Contains(BC_Comp_Nmae))
        //    {
        //        if (tempstr.Equals("cont_sth_half_l")) strDISOrientation = "R";
        //        if (tempstr.Equals("cont_sth_half_r")) strDISOrientation = "L";
        //        modOPCommand.EquDTCSeg1(X0, Y0, Constants.el_ExecFilePath, strDISOrientation);//Hearder
        //        if (tempstr.Equals("cont_sth_half_l")) strDISOrientation = "L";
        //        if (tempstr.Equals("cont_sth_half_r")) strDISOrientation = "R";
        //        T = arrpinsCollection.Length;
        //        for (int N = 0; N <= arrpinsCollection.Length - 1; N++)//Orig for loop
        //        {
        //            modOPCommand.EquDTCSeg2(X0, Y0 + incre * N, arrpinsCollection[N], Constants.el_ExecFilePath, strDISOrientation, BC_Comp_Nmae, CompName, tempstr);
        //        }
        //        //modOPCommand.DisSeg2(X0, Y0 + incre, "N", Constants.el_ExecFilePath);
        //        if (tempstr.Equals("cont_sth_half_l")) strDISOrientation = "R";
        //        if (tempstr.Equals("cont_sth_half_r")) strDISOrientation = "L";
        //        modOPCommand.EquDTCSeg3(X0, Y0, incre * (iNumberOfPins), Constants.el_ExecFilePath, strDISOrientation); // vertical lines 
        //        //modOPCommand.DisSeg4(X0, Y0 - (Math.Abs(incre) * (T)), X0, Y0 + (incre*T), CompName, Constants.el_ExecFilePath);//Rectongler
        //        if (tempstr.Equals("cont_sth_half_l")) strDISOrientation = "L";
        //        if (tempstr.Equals("cont_sth_half_r")) strDISOrientation = "R";
        //        modOPCommand.EquDTCSeg4(X0, Y0 - (Math.Abs(incre) * (T)) - 6, X0, Y0, CompName, Constants.el_ExecFilePath, partnumber, strDISOrientation, assopns, BC_Comp_Nmae, tempstr);//Rectongler
        //        //modOPCommand.DisSeg4(X0, Y0 + incre * iNumberOfPins, CompName, Constants.el_ExecFilePath);//Rectongles
        //        //X0, Y0 - (Math.Abs(incre_y) * (T)), X0, Y0, iEquName, Ori, Constants.el_ExecFilePath);//Rectongler
        //        if (tempstr.Equals("cont_sth_half_l")) strDISOrientation = "R";
        //        if (tempstr.Equals("cont_sth_half_r")) strDISOrientation = "L";
        //        modOPCommand.EquDTCSeg5(X0, Y0 + incre * iNumberOfPins, Constants.el_ExecFilePath, strDISOrientation);//footer
        //        if (DTCEqupmentBoxrefname.Equals("+") || DTCEqupmentBoxrefname.Equals("Loc") || DTCEqupmentBoxrefname.Equals("LOC") || string.IsNullOrEmpty(DTCEqupmentBoxrefname))
        //        {
        //            //modOPCommand.SegEquipmentBox(X0 - 20, Y0 - (Math.Abs(incre_y) * (T)), X0 + 20, Y0, EquBoxExist, Ori, Constants.el_ExecFilePath);//Equipment Box
        //        }
        //        else
        //        {
        //            modOPCommand.DTCSegEquipmentBox(X0, Y0 - (Math.Abs(incre) * (T)), X0, Y0, DTCEqupmentBoxrefname, strDISOrientation, Constants.el_ExecFilePath);//Equipment Box }
        //        }

        //        Constants.processedItems.Add(BC_Comp_Nmae);
        //    }
        //}
        #endregion

        #region //Commented old DrawEquSymbolUsedPins code on 18 november, 2025
        //public static void DrawEquSymbolUsedPins(int X0, int Y0, string iNumberOfMaxPins, string iEquName ,string iSamplePinNumber,string iPartNumber,frmPanelOri f,string assoPNs,string EquBoxExist,string LoomsExist)
        //{
        //    int Counter = 0;
        //    int[] arrUsedPinsInConnector, arrPinssorted;
        //    int L = 0;
        //    string[] arrPins;
        //    int TotalUsedPinsInConnector =0; //TotalUsedPinsInConnector
        //    int incre_x = 4;
        //    int incre_y = -4;
        //    int I = 0;
        //    //frmPanelOri frmpanelori = new frmPanelOri();
        //    string Ori = string.Empty;
        //    string IsFullConnector = string.Empty;
        //    int T = 0;// arrPins.Length;
        //    if (f.grdOriInfo.RowCount == 0) return;
        //    int numberOfPins = 0;
        //    if (string.IsNullOrEmpty(iNumberOfMaxPins) || !int.TryParse(iNumberOfMaxPins, out numberOfPins))
        //    {
        //        // Handle invalid iNumberOfPins (set default value or log the error)
        //        numberOfPins = 0; // Default value or return early if necessary
        //    }
        //    //arrUsedPinsInConnector = new
        //    #region//Left Connecter
        //    if (!string.IsNullOrEmpty((string)f.grdOriInfo[1, 0].Value))
        //    {
        //        Ori = "L";
        //        int l = 0;

        //        var arrPinsTempLeft = f.grdOriInfo[1, 0].Value.ToString().Split(",");
        //        arrPins = arrPinsTempLeft.Distinct().ToArray();
        //        #region//Pins Array sorting
        //        //arrUsedPinsInConnector = new int[arrPins.Length];
        //        //for (int sort = 1; sort <= arrUsedPinsInConnector.Length - 1; sort++)
        //        //{
        //        //    arrUsedPinsInConnector[sort] = Convert.ToInt16(arrPins[sort]);
        //        //}
        //        //    //arrUsedPinsInConnector = //arrPins.OrderBy(x=>Convert.ToInt16(x)).ToArray();
        //        //T = arrUsedPinsInConnector.Length;
        //        //arrPinssorted = new int[T];
        //        //arrPinssorted = arrUsedPinsInConnector.OrderBy(x => x).ToArray();
        //        #endregion
        //        T = arrPins.Length;
        //        if (T-1 < numberOfPins)
        //        {
        //            IsFullConnector = "no";
        //        }
        //        else if(T == numberOfPins)
        //        { 
        //            IsFullConnector = "yes";
        //        }

        //        #region // Coordinate settings
        //        X0 = 40;
        //        if (T - 1 <= 2) Constants.Simple_EQU_Left_Cur_Height_Incre_Count = T * 4;
        //        Y0 = 65 + Constants.Sample_Equ_Left_Cur_Height_Count;
        //        Constants.Sample_Equ_Left_Cur_Height_Count = Constants.Sample_Equ_Left_Cur_Height_Count + (T * 4 + 8 + 4*4) + Constants.Simple_EQU_Left_Cur_Height_Incre_Count;//41
        //        #endregion

        //        modOPCommand.EquSeg1(X0, Y0, Ori, "Full", iEquName,Constants.el_ExecFilePath);//Header
        //        for (int i = 1; i <= T - 1; i++)
        //        {
        //            //l = l - 4;
        //            modOPCommand.EquSeg2(X0, (Y0 + incre_y * (I - 1)) + l-4, arrPins[i].ToString(), Ori, Constants.el_ExecFilePath); //'since the first has to start from 0;//Pins
        //            //modOPCommand.EquSeg2(X0, (Y0 + incre_y * (I - 1)) + l - 4, arrPinssorted[i].ToString(), Ori, Constants.el_ExecFilePath); //'since the first has to start from 0;//Pins
        //            l = l - 4;
        //        }
        //        modOPCommand.EquSeg3(X0 - 7.5, Y0, incre_y * (T - 1), Ori, Constants.el_ExecFilePath);//Lefy and Right lines
        //        modOPCommand.EquSeg4(X0, Y0 - (Math.Abs(incre_y) * (T)), X0, Y0, iEquName, Ori, Constants.el_ExecFilePath);//Rectongler
        //        modOPCommand.EquSeg5(X0, Y0 + incre_y * (T - 1), Ori, "Full", iEquName, Constants.el_ExecFilePath, IsFullConnector);//Footer
        //        //'change the "Y0" value so that L,R,T,B can come in different position
        //        //'as rectangle(EquSeg4) LL-y is the lowest point, passing it to "y0"
        //        Y0 = Y0 - (Math.Abs(incre_y) * (T));
        //        modOPCommand.EquSegPartNumber(X0, Y0 -10, iEquName, iPartNumber, Constants.el_ExecFilePath, assoPNs);//PN and comp namew
        //        if (EquBoxExist.Equals("+") || EquBoxExist.Equals("Loc") || EquBoxExist.Equals("LOC") || string.IsNullOrEmpty(EquBoxExist))
        //        {
        //            //modOPCommand.SegEquipmentBox(X0 - 20, Y0 - (Math.Abs(incre_y) * (T)), X0 + 20, Y0, EquBoxExist, Ori, Constants.el_ExecFilePath);//Equipment Box
        //        }
        //        else
        //        {
        //            modOPCommand.SegEquipmentBox(X0 - 8, Y0 + 4 - (Math.Abs(incre_y) * (T)), X0 + 16, Y0 + 20, EquBoxExist, Ori, Constants.el_ExecFilePath);//Equipment Box }
        //        }
        //            Y0 = Y0 - 50;
        //    }
        //    #endregion
        //    #region//Right Connecter
        //    if (!string.IsNullOrEmpty((string)f.grdOriInfo[1, 1].Value))
        //    {
        //        Ori = "R";
        //        int r = 0;
        //        int DecreHeight = 0;
        //        arrPins = f.grdOriInfo[1, 1].Value.ToString().Split(",");
        //        var arrPinsTempRight = f.grdOriInfo[1, 1].Value.ToString().Split(",");
        //        arrPins = arrPinsTempRight.Distinct().ToArray();

        //        T = arrPins.Length;
        //        if (T - 1 < numberOfPins)
        //        {
        //            IsFullConnector = "no";
        //        }
        //        else if (T == numberOfPins)
        //        {
        //            IsFullConnector = "yes";
        //        }                

        //        #region // Coordinate settings
        //        X0 = Constants.SheetWidth - 30;
        //        if (T - 1 <= 2) Constants.Simple_EQU_Right_Cur_Height_Decre_Count = T * 4;
        //        Y0 = Constants.SheetHeight - Constants.Sample_Equ_Right_Cur_Height_Count - 24;
        //        Constants.Sample_Equ_Right_Cur_Height_Count = Constants.Sample_Equ_Right_Cur_Height_Count + (T * 4 + 8 + 4 * 4) + Constants.Simple_EQU_Right_Cur_Height_Decre_Count;//41
        //        #endregion

        //        modOPCommand.EquSeg1(X0, Y0, Ori, "Full", iEquName, Constants.el_ExecFilePath);
        //        for (int i0 = 1; i0 <= T - 1; i0++)
        //        {
        //            //r = r - 4;
        //            //modOPCommand.EquSeg2(X0 + r, Y0, arrPins[i0], Ori, Constants.el_ExecFilePath); //'since the first has to start from 0
        //            modOPCommand.EquSeg2(X0, Y0 + (incre_y * (I - 1)) +r-4, arrPins[i0], Ori, Constants.el_ExecFilePath); //'since the first has to start from 0
        //            r = r - 4;
        //            //modOPCommand.EquSeg2(X0, Y0 + incre_y * (I - 1), arrPins[i0], Ori, Constants.el_ExecFilePath); //'since the first has to start from 0
        //        }//3 lines
        //        modOPCommand.EquSeg3(X0 - 7.5, Y0, incre_y * (T - 1), Ori, Constants.el_ExecFilePath);//1 line 
        //        modOPCommand.EquSeg4(X0, Y0 - (Math.Abs(incre_y) * (T)), X0, Y0, iEquName, Ori, Constants.el_ExecFilePath);
        //        modOPCommand.EquSeg5(X0, Y0 + incre_y * (T - 1), Ori, "Full", iEquName, Constants.el_ExecFilePath, IsFullConnector);
        //        //'change the "Y0" value so that L,R,T,B can come in different position
        //        //'as rectangle(EquSeg4) LL-y is the lowest point, passing it to "y0"
        //        Y0 = Y0 - (Math.Abs(incre_y) * (T));
        //        modOPCommand.EquSegPartNumber(X0, Y0 - 10, iEquName, iPartNumber, Constants.el_ExecFilePath, assoPNs);
        //        if (EquBoxExist.Equals("+") || EquBoxExist.Equals("Loc") || EquBoxExist.Equals("LOC") || string.IsNullOrEmpty(EquBoxExist))
        //        {
        //            //modOPCommand.SegEquipmentBox(X0 - 20, Y0 - (Math.Abs(incre_y) * (T)), X0 + 20, Y0, EquBoxExist, Ori, Constants.el_ExecFilePath);//Equipment Box
        //        }
        //        else
        //        {
        //            modOPCommand.SegEquipmentBox(X0 - 8, Y0 + 4 - (Math.Abs(incre_y) * (T)), X0 + 16, Y0+20, EquBoxExist, Ori, Constants.el_ExecFilePath);//Equipment Box }
        //        }
        //        Y0 = Y0 - 50;
        //    }
        //    #endregion

        //    #region//Top Connecter
        //    //if (!string.IsNullOrEmpty((string)f.grdOriInfo[1,2].Value))
        //    //{
        //    //    arrPins = f.grdOriInfo[1,2].Value.ToString().Split(",");
        //    //    T = arrPins.Length; 
        //    //    Ori = "T";
        //    //    if (T - 1 < Convert.ToInt16(iNumberOfPins))
        //    //    {
        //    //        IsFullConnector = "no";
        //    //    }
        //    //    else if (T == Convert.ToInt16(iNumberOfPins))
        //    //    {
        //    //        IsFullConnector = "yes";
        //    //    }
        //    //    int b = 0;
        //    //    //X0 = 100;
        //    //    //Y0 = 150;
        //    //    for (int I1 = 1; I1 <= T-1; I1++)
        //    //    {
        //    //        modOPCommand.EquSeg2(X0 + b, Y0, arrPins[I1], Ori, Constants.el_ExecFilePath);
        //    //        b = 4;
        //    //    }

        //    //    // Reordered EquSeg1 as EQU was not getting mapped properly to pin in Top and Bottom
        //    //    modOPCommand.EquSeg1(X0, Y0, Ori, "Full", iEquName, Constants.el_ExecFilePath);//


        //    //    modOPCommand.EquSeg3(X0, Y0, Math.Abs(incre_x * (T - 1)), Ori, Constants.el_ExecFilePath);//Line Closer
        //    //    modOPCommand.EquSeg4(X0, Y0, X0 + (Math.Abs(incre_x) * (T - 1)), Y0, iEquName, Ori, Constants.el_ExecFilePath);
        //    //    modOPCommand.EquSeg5(X0 + incre_x * (T - 1), Y0, Ori, "Full", iEquName, Constants.el_ExecFilePath, IsFullConnector);
        //    //    modOPCommand.EquSegPartNumber(X0, Y0 - 10, iEquName, iPartNumber, Constants.el_ExecFilePath,assoPNs);

        //    //    Y0 = Y0 - 60;
        //    //}

        //    //#endregion
        //    //#region//Bottom Connecter
        //    //if (!string.IsNullOrEmpty((string)f.grdOriInfo[1, 3].Value))
        //    //{
        //    //    arrPins = f.grdOriInfo[1, 3].Value.ToString().Split(",");
        //    //    T = arrPins.Length;
        //    //    Ori = "B";
        //    //    int a = 0;
        //    //    if (T - 1 < Convert.ToInt16(iNumberOfPins))
        //    //    {
        //    //        IsFullConnector = "no";
        //    //    }
        //    //    else if (T == Convert.ToInt16(iNumberOfPins))
        //    //    {
        //    //        IsFullConnector = "yes";
        //    //    }
        //    //    //X0 = 100;
        //    //    //Y0 = 102;
        //    //    for (int I11 = 1; I11 <= T-1; I11++)
        //    //    {
        //    //        modOPCommand.EquSeg2(X0 + a, Y0, arrPins[I11], Ori, Constants.el_ExecFilePath);//Adding Pins
        //    //        //using (writer = File.AppendText(Constants.el_ExecFilePath))
        //    //        //{
        //    //        //    //writer.WriteLine($"Add N254 'EQU' :F1.0 :R90 :AC I0 {X0}, {Y0} :T4320 {X0}, {Y0};NOP;");
        //    //        //}
        //    //        //modOPCommand.EquSeg2(X0 + a + incre_x * (I - 1), Y0, arrPins[I11], Ori, Constants.el_ExecFilePath);//Adding Pins
        //    //        //X0 = X0 + 4;
        //    //        a = 4;
        //    //        //modOPCommand.EquSeg2(X0 + incre_x * (I - 1), Y0, arrPins[I11], Ori, Constants.el_ExecFilePath);//Adding Pins
        //    //    }//3 line

        //    //    // Reordered EquSeg1 as EQU was not getting mapped properly to pin in Top and Bottom
        //    //    modOPCommand.EquSeg1(X0, Y0, Ori, "Full", iEquName, Constants.el_ExecFilePath);// 2 lines
        //    //    modOPCommand.EquSeg3(X0, Y0, Math.Abs(incre_x * (T - 1)), Ori, Constants.el_ExecFilePath);//2 lines
        //    //    modOPCommand.EquSeg4(X0, Y0, X0 + (Math.Abs(incre_x) * (T - 1)), Y0, iEquName, Ori, Constants.el_ExecFilePath);//2 lines
        //    //    modOPCommand.EquSeg5(X0, Y0, Ori, "Full", iEquName, Constants.el_ExecFilePath, IsFullConnector);//1 line
        //    //    //modOPCommand.EquSeg5(X0 + incre_x * (T - 1), Y0, Ori, "Full", iEquName, Constants.el_ExecFilePath);//1 line
        //    //    modOPCommand.EquSegPartNumber(X0, Y0, iEquName, iPartNumber, Constants.el_ExecFilePath, assoPNs);//2 lines

        //    //    //// Reordered EquSeg1 as EQU was not getting mapped properly to pin in Top and Bottom
        //    //    //modOPCommand.EquSeg1(X0, Y0, Ori, "Full", iEquName, Constants.el_ExecFilePath);// 1 line
        //    //    //modOPCommand.EquSeg3(X0, Y0 - 7.5, Math.Abs(incre_x * (T - 1)), Ori, Constants.el_ExecFilePath);//2 lines
        //    //    //modOPCommand.EquSeg4(X0, Y0, X0 + (Math.Abs(incre_x) * (T - 1)), Y0, iEquName, Ori, Constants.el_ExecFilePath);//2 lines
        //    //    //modOPCommand.EquSeg5(X0 + incre_x * (T - 1), Y0, Ori, "Full", iEquName, Constants.el_ExecFilePath);//1 line
        //    //    //modOPCommand.EquSegPartNumber(X0, Y0 - 10, iEquName, iPartNumber, Constants.el_ExecFilePath);//2 lines
        //    //    //Y0 = Y0 - 60;
        //    //}
        //    #endregion
        //}
        #endregion

        /*public static void ReadPanelDetails_Backup()
        {
            //Note : Reserve Place Holders into "arrPanelDetails" is required for column 6 -14 as per old VB6 code but not sure why(Lets wait for future development)
            Constants.arrPanelDetails = TextOperations.ConvertTextFileDataInto2DArray(Constants.PanelDetails_FilePath);//(string.Concat(Constants.Electre_Temp_Folder_Path, Constants.textPanelPartNumber, Constants.dashMark, Constants.panelDetailsTextFileName));
        }
*/

        //public static void ReadPanelDetails()
        //{
        //    Constants.panelDetailsList.Clear();

        //    var lines = File.ReadLines(Constants.PanelDetails_FilePath)
        //        .Where(l => !string.IsNullOrWhiteSpace(l))
        //        .ToList();

        //    foreach (var line in lines)
        //    {
        //        var parts = line.Split(',').Select(x => x.Trim()).ToArray();

        //        var row = new PanelDetailsRow
        //        {
        //            FromConnector = parts.ElementAtOrDefault(0),
        //            FromPin = parts.ElementAtOrDefault(1),
        //            ToConnector = parts.ElementAtOrDefault(2),
        //            ToPin = parts.ElementAtOrDefault(3),
        //            WireCode = parts.ElementAtOrDefault(4),
        //            PinX = parts.ElementAtOrDefault(5),
        //            PinY = parts.ElementAtOrDefault(6),
        //            TPinX = parts.ElementAtOrDefault(7),
        //            TPinY = parts.ElementAtOrDefault(8),
        //            Usage = parts.ElementAtOrDefault(9),
        //            FromType = parts.ElementAtOrDefault(10),
        //            ToType = parts.ElementAtOrDefault(11),
        //            FromOrientation = parts.ElementAtOrDefault(12),
        //            ToOrientation = parts.ElementAtOrDefault(13),
        //            GroupId = parts.ElementAtOrDefault(14),
        //            WireLength = parts.ElementAtOrDefault(15),
        //            WireType = parts.ElementAtOrDefault(16),
        //            WireTypeNumber = parts.ElementAtOrDefault(17),
        //        };

        //        Constants.panelDetailsList.Add(row);
        //    }

        //    Constants.panelDetailsList = TextOperations.SortPanelDetails(Constants.panelDetailsList);
        //}

        //public static string FindPartNumberFromAssosiatedPartnumbersIfDefined(string[] arrCompPN)
        //{
        //    int colCFound, colDFound, colAssoFound, colBYFound;
        //    string strPN = string.Empty;
        //    for (int pn = 0; pn <= arrCompPN.Length - 1; pn++)
        //    {
        //        colCFound = modOPCommand.RowOfFoundStringInColOfMDarray(arrCompPN[pn], Constants.arrTableOfLibCatalog, Constants.colLibCatalog_InternalPartNumber - 1);
        //        colDFound = modOPCommand.RowOfFoundStringInColOfMDarray(arrCompPN[pn], Constants.arrTableOfLibCatalog, Constants.colLibCatalog_InternalPartNumber - 1);
        //        colAssoFound = modOPCommand.RowOfFoundStringInColOfMDarray(arrCompPN[pn], Constants.arrTableOfLibCatalog, Constants.colLibCatalog_Accessory - 1);
        //        if (colCFound > 0 && colDFound > 0 && colAssoFound.Equals(0))
        //        {
        //            strPN = arrCompPN[pn];
        //            break;
        //        }
        //    }
        //    return strPN;
        //}

        #region // GatherPanelComponentProperties old code backup - Nov 17,2025
        /* public static void GatherPanelComponentProperties(string panelName)
         {
             #region//no of components from arrDEabove  
             int NumberOfPanelComponents = 0;
             string strComponentName = string.Empty;
             // int nocomps = (Constants.arrComponentsWithPartNumber).Count(x => !string.IsNullOrEmpty(x));//Remove null from array
             int nocomps = Constants.listComponentsWithPartNumber.Count;//Changed on Nov 17,2025
             for (int pancom = 0; pancom <= nocomps - 1; pancom++)
             {
                 strComponentName = Constants.arrComponentsWithPartNumber[pancom];
                 for (int comDEabove = 0; comDEabove <= Constants.arrDEabove.GetLength(0) - 1; comDEabove++)
                 {
                     if (Constants.arrDEabove[comDEabove, Constants.colDE_Connector - 1].Equals(strComponentName) && Constants.arrDEabove[comDEabove, Constants.colDE_Panels - 1].Equals(panelName))
                     //if (Constants.arrDEabove[comDEabove, Constants.colDE_Connector - 1].Equals(strComponentName) && Constants.arrDEabove[comDEabove, Constants.colDE_Info - 1].Equals(Constants.info) && Constants.arrDEabove[comDEabove, Constants.colDE_Panels - 1].Equals(panellName))
                     {
                         NumberOfPanelComponents = NumberOfPanelComponents + 1;
                         Debug.Print($" NumberOfPanelComponents {NumberOfPanelComponents.ToString()} - {strComponentName}");
                         break;
                     }
                 }
             }
             #endregion

             int PanelComp = 0;
             //Commented on Nov 13,2025 due to index outside bounds array getting in
             //Constants.arrPanelComponentsProperties = new string[NumberOfPanelComponents, Constants.arrPanelComponentsPropertiesMaxCols];
             Constants.arrPanelComponentsProperties = new string[nocomps, Constants.arrPanelComponentsPropertiesMaxCols];
             bool IsInPanel = false;
             string CompPN = string.Empty;
             string CompType = string.Empty;
             string CBType_Name = string.Empty;
             string CBType_Voltage = string.Empty;
             //colDE_CBType_Voltage
             string SampleEquPinNumber = string.Empty;
             string GroupId = string.Empty;
             string Wire_Length = string.Empty;
             string Wire_Type = string.Empty;
             string Equipment_Box_Info = string.Empty;
             string Looms_Info = string.Empty;
             //string TBKTER_Shunt_Value = string.Empty;
             var finalPnlComps = Constants.arrComponentsWithPartNumber.Distinct().ToArray();
             Constants.arrComponentsWithPartNumber = finalPnlComps.ToArray();
             //Define Component Properties for each component which are inside panel
             for (int rPanCOmpPros = 0; rPanCOmpPros <= nocomps - 1; rPanCOmpPros++)
             {
                 #region//Check for The particular component is inside/belongs to the panel or not 
                 if (string.IsNullOrEmpty(Constants.arrComponentsWithPartNumber[rPanCOmpPros])) continue;
                 strComponentName = Constants.arrComponentsWithPartNumber[rPanCOmpPros];
                 for (int comDEabove = 0; comDEabove <= Constants.arrDEabove.GetLength(0) - 1; comDEabove++)
                 {
                     if (Constants.arrDEabove[comDEabove, Constants.colDE_Connector - 1].Equals(strComponentName) && Constants.arrDEabove[comDEabove, Constants.colDE_Panels - 1].Equals(panelName))
                     //if (Constants.arrDEabove[comDEabove, Constants.colDE_Connector - 1].Equals(strComponentName) && Constants.arrDEabove[comDEabove, Constants.colDE_Info - 1].Equals(Constants.info) && Constants.arrDEabove[comDEabove, Constants.colDE_Panels - 1].Equals(panellName))
                     {
                         IsInPanel = true;
                         //NumberOfPanelComponents = NumberOfPanelComponents + 1;
                         //Debug.Print($" NumberOfPanelComponents {NumberOfPanelComponents.ToString()} - {strComponentName}");
                         break;
                     }
                     *//*else
                     {
                         IsInPanel = false;
                     }*//*
                 }
                 #endregion

                 if (IsInPanel == false) continue;
                 int rownum = Constants.temp_rownum;

                 #region //Get Part Number from arrDEabove 2D array if the component is in same panel or exist based on IsInPanel is true or false
                 for (int comDEabove = 0; comDEabove <= Constants.arrDEabove.GetLength(0) - 1; comDEabove++)
                 {
                     //Adding properties for Part Numbers
                     if (Constants.arrDEabove[comDEabove, Constants.colDE_Connector - 1].Equals(strComponentName) && Constants.arrDEabove[comDEabove, Constants.colDE_PartNumber - 1] != "" && Constants.arrDEabove[comDEabove, Constants.colDE_Panels - 1].Equals(panelName))
                     //if (Constants.arrDEabove[comDEabove, Constants.colDE_Connector - 1].Equals(strComponentName) && Constants.arrDEabove[comDEabove, Constants.colDE_Info - 1].Equals(Constants.info) && Constants.arrDEabove[comDEabove, Constants.colDE_PartNumber - 1] != "" && Constants.arrDEabove[comDEabove, Constants.colDE_Panels - 1].Equals(panellName))
                     {
                         #region //The below code search for whether Component Part Number is primary part number for relavent component or assosiate part number,
                         string sd = modOPCommand.RowOfCountFoundStringInColOfMDarray(strComponentName, Constants.arrDEabove, Constants.colDE_Connector - 1, Constants.colDE_PartNumber - 1, Constants.colDE_Panels - 1);
                         string[] arrPns = sd.Split(";");
                         arrPns = arrPns.Distinct().ToArray();
                         if (arrPns.Length > 1)
                         {
                             Constants.strDEPanelaboveAssosiatePNList = string.Empty;
                             CompPN = modMain.FindPartNumberFromAssosiatedPartnumbersIfDefined(arrPns);
                             var removePNfromAssoPNList = arrPns.ToList();
                             removePNfromAssoPNList.Remove(CompPN);
                             Constants.strDEPanelaboveAssosiatePNList = string.Join(";", removePNfromAssoPNList.ToArray());
                             Equipment_Box_Info = Constants.arrDEabove[comDEabove, Constants.colDE_EquipmentBox - 1];
                             Looms_Info = Constants.arrDEabove[comDEabove, Constants.colDE_Looms - 1];
                         }
                         else
                         {
                             CompPN = Constants.arrDEabove[comDEabove, Constants.colDE_PartNumber - 1];
                             Equipment_Box_Info = Constants.arrDEabove[comDEabove, Constants.colDE_EquipmentBox - 1];
                             Looms_Info = Constants.arrDEabove[comDEabove, Constants.colDE_Looms - 1];
                         }
                         #endregion
                         break;
                     }
                 }
                 #endregion

                 #region// from arrDBBelow get  CompType,SampleEquPinNumber
                 for (int comDEbelow = 0; comDEbelow <= Constants.arrDEbelow.GetLength(0) - 1; comDEbelow++)
                 {
                     if (Constants.arrDEbelow[comDEbelow, Constants.colDE_Connector - 1].Equals(strComponentName)
                        && (Constants.arrDEbelow[comDEbelow, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.cont) || Constants.arrDEbelow[comDEbelow, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.ind) || Constants.arrDEbelow[comDEbelow, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.ter) || Constants.arrDEbelow[comDEbelow, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.tbk) || Constants.arrDEbelow[comDEbelow, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.bus) || Constants.arrDEbelow[comDEbelow, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.cnt) || Constants.arrDEbelow[comDEbelow, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.ant) || Constants.arrDEbelow[comDEbelow, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.dd) || Constants.arrDEbelow[comDEbelow, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.snr) || Constants.arrDEbelow[comDEbelow, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.rel) || Constants.arrDEbelow[comDEbelow, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.spl) || Constants.arrDEbelow[comDEbelow, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.cb_s) || Constants.arrDEbelow[comDEbelow, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.cb_t))
                         && Constants.arrDEbelow[comDEbelow, Constants.colDE_Panels - 1].Equals(panelName)) //&& Constants.arrDEbelow[comDEbelow, Constants.colDE_PartNumber - 1] != "")
                     {
                         CompType = Constants.arrDEbelow[comDEbelow, Constants.colDE_Type - 1];
                         CBType_Name = Constants.arrDEbelow[comDEbelow, Constants.colDE_CBType_Name - 1];
                         CBType_Voltage = Constants.arrDEbelow[comDEbelow, Constants.colDE_CBType_Voltage - 1];
                         SampleEquPinNumber = Constants.arrDEbelow[comDEbelow, Constants.colDE_PinNumber - 1];

                         #region//Adding shunt values for Terminal Block - TER,TBK COMPONENT TYPE
                         if (CompType.Equals("TBK") || CompType.Equals("TER"))
                         {
                             for (int shuntDEbelow = 0; shuntDEbelow <= Constants.arrDEbelow.GetLength(0) - 1; shuntDEbelow++)
                             {
                                 if (Constants.arrDEbelow[comDEbelow, Constants.colDE_Connector - 1].Equals(strComponentName) && !string.IsNullOrEmpty(Constants.arrDEbelow[shuntDEbelow, Constants.colDE_TBKTER_Shunt_Value - 1]))
                                 {
                                     if (string.IsNullOrEmpty(Constants.strDEBelowTERTBK_Shunt_List))
                                     {
                                         Constants.strDEBelowTERTBK_Shunt_List = string.Concat(Constants.arrDEbelow[shuntDEbelow, Constants.colDE_Connector - 1], ",", Constants.arrDEbelow[shuntDEbelow, 7], ",", Constants.arrDEbelow[shuntDEbelow, 8], ",", Constants.arrDEbelow[shuntDEbelow, 11], ",", Constants.arrDEbelow[shuntDEbelow, Constants.colDE_TBKTER_Shunt_Value - 1]);
                                     }
                                     else
                                     {
                                         Constants.strDEBelowTERTBK_Shunt_List = string.Concat(Constants.strDEBelowTERTBK_Shunt_List, ";", string.Concat(Constants.arrDEbelow[shuntDEbelow, Constants.colDE_Connector - 1], ",", Constants.arrDEbelow[shuntDEbelow, 7], ",", Constants.arrDEbelow[shuntDEbelow, 8], ",", Constants.arrDEbelow[shuntDEbelow, 11], ",", Constants.arrDEbelow[shuntDEbelow, Constants.colDE_TBKTER_Shunt_Value - 1]));
                                     }
                                 }
                             }
                             for (int shuntDEbelow = 0; shuntDEbelow <= Constants.arrDEabove.GetLength(0) - 1; shuntDEbelow++)
                             {
                                 if (Constants.arrDEabove[comDEbelow, Constants.colDE_Connector - 1].Equals(strComponentName) && !string.IsNullOrEmpty(Constants.arrDEabove[shuntDEbelow, Constants.colDE_TBKTER_Shunt_Value - 1]))
                                 {
                                     if (string.IsNullOrEmpty(Constants.strDEBelowTERTBK_Shunt_List))
                                     {
                                         Constants.strDEBelowTERTBK_Shunt_List = string.Concat(Constants.arrDEabove[shuntDEbelow, Constants.colDE_Connector - 1], ",", Constants.arrDEabove[shuntDEbelow, 7], ",", Constants.arrDEabove[shuntDEbelow, 8], ",", Constants.arrDEbelow[shuntDEbelow, 11], ",", Constants.arrDEabove[shuntDEbelow, Constants.colDE_TBKTER_Shunt_Value - 1]);
                                     }
                                     else
                                     {
                                         Constants.strDEBelowTERTBK_Shunt_List = string.Concat(Constants.strDEBelowTERTBK_Shunt_List, ";", string.Concat(Constants.arrDEabove[shuntDEbelow, Constants.colDE_Connector - 1], ",", Constants.arrDEabove[shuntDEbelow, 7], ",", Constants.arrDEabove[shuntDEbelow, 8], ",", Constants.arrDEabove[shuntDEbelow, 11], ",", Constants.arrDEabove[shuntDEbelow, Constants.colDE_TBKTER_Shunt_Value - 1]));
                                     }
                                 }
                             }
                         }
                         #endregion
                         break;
                     }
                 }
                 #endregion

                 string CompAcc = string.Empty;
                 string MacroName = string.Empty;
                 string CompMaxPin = string.Empty;
                 string CompDwgName = string.Empty;
                 CompDwgName = strComponentName;
                 //To check in accessory column first
                 rownum = modOPCommand.RowOfFoundStringInColOfMDarray(CompPN, Constants.arrTableOfLibCatalog, Constants.colLibCatalog_Accessory - 1);
                 //If its not there in accessory then in internal part number column
                 if (rownum == 0) rownum = modOPCommand.RowOfFoundStringInColOfMDarray(CompPN, Constants.arrTableOfLibCatalog, Constants.colLibCatalog_InternalPartNumber - 1);
                 CompPN = Constants.arrTableOfLibCatalog[rownum, Constants.colLibCatalog_InternalPartNumber - 1];
                 CompAcc = Constants.arrTableOfLibCatalog[rownum, Constants.colLibCatalog_Accessory - 1];
                 MacroName = Constants.arrTableOfLibCatalog[rownum, Constants.colLibCatalog_Macro - 1];
                 CompMaxPin = Constants.arrTableOfLibCatalog[rownum, Constants.ColLibCatalog_MaxPin - 1];

                 Constants.arrPanelComponentsProperties[PanelComp, 0] = CompDwgName;
                 Constants.arrPanelComponentsProperties[PanelComp, 1] = CompType;
                 Constants.arrPanelComponentsProperties[PanelComp, 2] = MacroName;
                 Constants.arrPanelComponentsProperties[PanelComp, 3] = CompMaxPin;
                 Constants.arrPanelComponentsProperties[PanelComp, 4] = CompPN;
                 Constants.arrPanelComponentsProperties[PanelComp, 5] = CompAcc;
                 Constants.arrPanelComponentsProperties[PanelComp, 6] = SampleEquPinNumber;
                 Constants.arrPanelComponentsProperties[PanelComp, 7] = GroupId;
                 Constants.arrPanelComponentsProperties[PanelComp, 8] = Wire_Length;
                 Constants.arrPanelComponentsProperties[PanelComp, 9] = Wire_Type;
                 Constants.arrPanelComponentsProperties[PanelComp, 10] = CBType_Name;
                 Constants.arrPanelComponentsProperties[PanelComp, 11] = CBType_Voltage;
                 Constants.arrPanelComponentsProperties[PanelComp, 12] = Constants.strDEPanelaboveAssosiatePNList;
                 Constants.arrPanelComponentsProperties[PanelComp, 13] = Equipment_Box_Info;
                 Constants.arrPanelComponentsProperties[PanelComp, 14] = Looms_Info;
                 Constants.arrPanelComponentsProperties[PanelComp, 15] = Constants.strDEBelowTERTBK_Shunt_List;

                 PanelComp = PanelComp + 1;
                 Constants.strDEPanelaboveAssosiatePNList = string.Empty;
                 Constants.strDEBelowTERTBK_Shunt_List = string.Empty;
             }
         }*/
        #endregion

        #region // Commented old InitiateStep1 code on 18 november, 2025
        /*  public static void InitiateStep1()
        {
            int Q = 0;
            int CompXdist = 50;
            int CompYdist =50;//changed 50 to 100
            int Xlimit;
            int Ylimit;
            int Xcol = 1;
            int Xcounter = 1;
            int Ycounter = 1;
            int secXcol = 1;
            int secXcounter = 0;
            int secYcounter = 0;
            int Noofpin = 1;int spacing =4;//(spacing may be 4 to 6)
            Constants.processedItems = new List<string>();
            Constants.remainingItems = new List<string>();
            Xlimit = (Constants.SheetWidth - 10) / CompXdist; //' -10 to compensate on the title block
            Ylimit = (Constants.SheetHeight - 10) / CompYdist;
            for (int Q1 = 0; Q1 <= Constants.arrPanelComponentsProperties.GetLength(0) - 1; Q1++)
            {
                string CompDwgName = string.Empty;
                string CompType = string.Empty;// As String
                string CBType_Name = string.Empty;// As String
                string CBType_Voltage = string.Empty;// As String
                string MacroName = string.Empty;// As String
                string CompPN = string.Empty;// As String
                string CompAcc = string.Empty;// As String
                string CompMaxPin = string.Empty;// As String
                string SampleEquPinNumber = string.Empty;// As String
                string GroupId = string.Empty;// As String
                string Wire_Length = string.Empty;// As String
                string Wire_Type = string.Empty;// As String
                string Assosiate_PartNumbers = string.Empty;// As String
                string Equipment_Box_Info = string.Empty;// As String
                string Looms_Info = string.Empty;// As String
                string TERTBK_Shunt_Info = string.Empty;// As String
                CompXdist = 50;
                CompYdist = 50;

                CompDwgName = Constants.arrPanelComponentsProperties[Q1, 0];
                CompType = Constants.arrPanelComponentsProperties[Q1, 1];
                MacroName = Constants.arrPanelComponentsProperties[Q1, 2];
                CompMaxPin = Constants.arrPanelComponentsProperties[Q1, 3];
                CompPN = Constants.arrPanelComponentsProperties[Q1, 4];
                CompAcc = Constants.arrPanelComponentsProperties[Q1, 5];
                SampleEquPinNumber = Constants.arrPanelComponentsProperties[Q1, 6];
                GroupId = Constants.arrPanelComponentsProperties[Q1, 7];
                Wire_Length = Constants.arrPanelComponentsProperties[Q1, 8];
                Wire_Type = Constants.arrPanelComponentsProperties[Q1, 9];
                CBType_Name = Constants.arrPanelComponentsProperties[Q1, 10];
                CBType_Voltage = Constants.arrPanelComponentsProperties[Q1, 11];
                Assosiate_PartNumbers = Constants.arrPanelComponentsProperties[Q1, 12];
                Equipment_Box_Info = Constants.arrPanelComponentsProperties[Q1, 13];
                Looms_Info = Constants.arrPanelComponentsProperties[Q1, 14];
                TERTBK_Shunt_Info = Constants.arrPanelComponentsProperties[Q1, 15];
                //if (MacroName != "")
                //{
                if (CompType.ToUpper() == "SPL")
                {
                    if (secYcounter < Ylimit - 1)
                    {
                        secYcounter = secYcounter + 1;
                    }
                    else
                    {
                        secYcounter = 1;
                        secXcol = secXcol + 1;
                    }
                    CompXdist = (Constants.SheetWidth / 8);
                    CompYdist = (Constants.SheetHeight / 8);
                    Constants.SPL_Cur_Height_Incre_Count = Constants.SPL_Cur_Height_Incre_Count + 10 - 1;
                    CompYdist = CompYdist - Constants.SPL_Cur_Height_Incre_Count * 8;//264,230,202Constants.SheetHeight - (Constants.Cur_Height_Count * 50);
                    Constants.SPL_Cur_Width_Incre_Count = Constants.SPL_Cur_Width_Incre_Count + 30;
                    CompXdist = CompXdist + Constants.SPL_Cur_Width_Incre_Count;
                    modOPCommand.AddSymbolSPL(CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                }
                else 
                {
                    Xcounter = Xcounter + 1;
                    Ycounter = Ycounter + 1;
                    frmPanelOri frmpanelori = new frmPanelOri();
                    switch (CompType.ToUpper()) 
                    {
                        case "TCB" or "SCB" ://Circut Breakers
                            
                            #region//CB coordinate settings
                            int T = 0;
                            if (CompType.Equals("TCB")) { T = 3; }
                            else if (CompType.Equals("SCB")) { T = 1; }
                            CompXdist = (Constants.SheetWidth / 8);
                            CompYdist = Constants.SheetHeight - Constants.CB_Cur_Height_Count - 40;
                            Constants.CB_Cur_Height_Count = Constants.CB_Cur_Height_Count + (T * 4 + 4 * 6);
                            #endregion
                            modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                            Constants.arrPinsOfEqu = modStandard.PinsOfConnectorToArray(CompDwgName, Constants.arrPanelDetails, 0, 1);
                            //cbpinsCollection.Length * 4;
                            
                            string[] cbpinsTempCollection = new string[T];
                            cbpinsTempCollection = (from i in Constants.arrPinsOfEqu
                                                    where (!string.IsNullOrEmpty(i))
                                                    select i).ToArray();
                            var cbpinsCollection = cbpinsTempCollection.Distinct().ToArray();
                            List<string> termsList = new List<string>();
                            for (int runs = 0; runs <= cbpinsCollection.Length-1; runs++)
                            {
                                termsList.Add(cbpinsCollection[runs]);
                            }
                            if (termsList.Count < T)
                            {
                                for (int r = termsList.Count; r < T; r++)
                                {
                                    termsList.Add("*");
                                }
                            }
                            // You can convert it back to an array if you would like to
                            string[] stetemp = termsList.ToArray();
                            //cbpinsTempCollection = Constants.arrPinsOfEqu;
                            
                            if (cbpinsCollection.Length == 0)
                            {
                                MessageBox.Show($"{CompDwgName} Pins Information Not Avaialble,Pls Check Once");
                                continue;
                            }
                                                        
                            //string[] stetemp = new string[T];
                            //for (int N1 = 0; N1 <= cbpinsCollection.Length - 1; N1++)//Orig for loop
                            //{
                            //    stetemp[N1] = string.IsNullOrEmpty(cbpinsCollection[N1])?"*": cbpinsCollection[N1];
                            //}
                            int incre = 0;
                            for (int N = 0; N <= stetemp.Length - 1; N++)//Orig for loop
                            {
                                //decre = decre - 2;
                                //modOPCommand.CBSeg2(CompXdist - 2, CompYdist+ incre, stetemp[N], Constants.el_ExecFilePath, N);//orig
                                //modOPCommand.CBSeg2(CompXdist - 2, CompYdist - incre, stetemp[N], Constants.el_ExecFilePath, N);
                                incre = incre + 6;
                            }
                            incre = 0;
                            for (int N = 0; N <= stetemp.Length - 1; N++)//Orig for loop
                            {
                                //decre = decre - 2;
                                //modOPCommand.CBSeg2(CompXdist+10, CompYdist + incre, stetemp[N], Constants.el_ExecFilePath, N);//orig
                                //modOPCommand.CBSeg2(CompXdist - 2, CompYdist - incre, stetemp[N], Constants.el_ExecFilePath, N);
                                incre = incre + 6;
                            }
                            //MOD N51 115,255 0,0 :E '1111'; NOP;
                            modOPCommand.AddCBSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, CBType_Voltage, "Nomegger", Constants.el_ExecFilePath, CompType);
                            break;
                        case "TBK" or "TER" ://Terminal Blocks - TER,Junction Module - TBK
                                                 
                            Constants.arrPinsOfEqu = modStandard.PinsOfConnectorToArray(CompDwgName, Constants.arrPanelDetails, 0, 1);
                            string[] tbkpinsCollectionter = new string[Constants.arrPinsOfEqu.Length];
                            tbkpinsCollectionter = (from i in Constants.arrPinsOfEqu
                                                    where (!string.IsNullOrEmpty(i))
                                                    select i).Distinct().ToArray();
                            
                            if (tbkpinsCollectionter.Length == 0)
                            {
                                MessageBox.Show($"{CompDwgName} Pins Information Not Avaialble,Pls Check Once");
                                continue;
                            }
                            
                            #region // Coordinate settings
                            CompXdist = (Constants.SheetWidth / 2);
                            if (tbkpinsCollectionter.Length - 1 <= 2) Constants.TBK_Height_Temp_Decre_Count = tbkpinsCollectionter.Length * 4;
                            CompYdist = Constants.SheetHeight - Constants.TBK_Cur_Height_Decre_Count - 24;
                            Constants.TBK_Cur_Height_Decre_Count = Constants.TBK_Cur_Height_Decre_Count + (tbkpinsCollectionter.Length * 4 + 8 + 4 * 4) + Constants.TBK_Height_Temp_Decre_Count;//41
                            #endregion
                            
                            modOPCommand.AddOOTB_TER_SymbolForTerminal(CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath, "0", tbkpinsCollectionter, TERTBK_Shunt_Info);
                            break;
                        case "SWT"://Switches

                            #region//Coordinate settings
                            CompXdist = (Constants.SheetWidth / 6) + Constants.SWT_Cur_Width_Incre_Count;
                            Constants.SWT_Cur_Width_Incre_Count = Constants.SWT_Cur_Width_Incre_Count + 50;
                            Constants.SWT_Cur_Height_Decre_Count = Constants.SWT_Cur_Height_Decre_Count + 10 - 1;
                            CompYdist = Constants.SheetHeight - Constants.SWT_Cur_Height_Decre_Count * 8;
                            #endregion

                            modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                            modOPCommand.AddSWTSymbolAttributes(CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath, CompDwgName);
                            break;
                        case "EQU":
                            string pattern = "[a-hj-np-zA-HJ-NP-Z]";
                            Constants.arrPinsOfEqu = modStandard.PinsOfConnectorToArray(CompDwgName, Constants.arrPanelDetails, 0, 1);
                            string[] dtcpinstempCollection = new string[Constants.arrPinsOfEqu.Length];
                            dtcpinstempCollection = (from i in Constants.arrPinsOfEqu
                                                    where (!string.IsNullOrEmpty(i))
                                                    select i).ToArray();
                            var dtcpinsCollection = dtcpinstempCollection.Distinct().ToArray();
                            Noofpin = Constants.arrPinsOfEqu.Length;
                            Ycounter = (Noofpin * spacing) + 2 * 4 + 4;
                            Xcounter = 17;
                            Constants.txtEquName = CompDwgName;
                            frmpanelori.lblEquName.Text = CompDwgName;

                            for (int PDRowUpdateEQU = 0; PDRowUpdateEQU <= Constants.arrPanelDetails.GetLength(0) - 1; PDRowUpdateEQU++)
                            {
                                if (Constants.arrPanelDetails[PDRowUpdateEQU, 0].Equals(CompDwgName))
                                {
                                    Constants.arrPanelDetails[PDRowUpdateEQU, Constants.colPD_Usage - 1] = SampleEquPinNumber;
                                    Constants.arrPanelDetails[PDRowUpdateEQU, Constants.colPD_F_Type - 1] = CompType;
                                    Constants.arrPanelDetails[PDRowUpdateEQU, Constants.colPD_F_GroupId] = GroupId;
                                    Constants.arrPanelDetails[PDRowUpdateEQU, Constants.colPD_F_Wire_Length - 2] = Wire_Length;
                                    Constants.arrPanelDetails[PDRowUpdateEQU, Constants.colPD_F_Wire_Type - 1] = Wire_Type;
                                }
                            }                            
                            if (CompDwgName.Contains("_J") || Regex.IsMatch(CompDwgName.Substring(CompDwgName.Length-1), pattern))//Double type connectors
                            {
                                #region // Coordinate settings
                                CompXdist = (Constants.SheetWidth /3);
                                if (dtcpinsCollection.Length - 1 <= 2) Constants.DTC_Cur_Height_Decre_Count = dtcpinsCollection.Length * 4;
                                CompYdist = (Constants.SheetHeight/2) - Constants.DTC_Cur_Height_Count;
                                Constants.DTC_Cur_Height_Count = Constants.DTC_Cur_Height_Count + (dtcpinsCollection.Length * 4 + 8 + 4 * 4) + Constants.DTC_Cur_Height_Decre_Count+10;//41
                                #endregion

                                int startIndex = CompDwgName.IndexOf("_");
                                string BC_Comp_Nmae_DTC = string.Empty;//Double Connector Component Name with out a(a to z with out i,o) or _J1(1 to 24)
                                    
                                if (Constants.remainingItems.Contains(BC_Comp_Nmae_DTC)) continue;
                                BC_Comp_Nmae_DTC = CompDwgName.Substring(0, startIndex);
                                modMain.GraLine_DTC(CompXdist, CompYdist, dtcpinsCollection.Length, CompDwgName, dtcpinsCollection, CompPN, Assosiate_PartNumbers, Equipment_Box_Info, BC_Comp_Nmae_DTC); //'CompMaxPin)
                                if (!Constants.remainingItems.Contains(BC_Comp_Nmae_DTC)) Constants.remainingItems.Add(BC_Comp_Nmae_DTC);
                            }
                            else
                            {
                                frmpanelori.ShowDialog();
                                modMain.DrawEquSymbolUsedPins(CompXdist, CompYdist, CompMaxPin, CompDwgName, SampleEquPinNumber, CompPN, frmpanelori, Assosiate_PartNumbers, Equipment_Box_Info, Looms_Info);
                                frmpanelori.Close();
                            }
                            break;
                        case "DIS"://Break Connectors
                            Constants.arrPinsOfEqu = modStandard.PinsOfConnectorToArray(CompDwgName, Constants.arrPanelDetails, 0, 1);
                            string[] dispinstempCollection = new string[Constants.arrPinsOfEqu.Length];
                            dispinstempCollection = (from i in Constants.arrPinsOfEqu
                                                    where (!string.IsNullOrEmpty(i))
                                                    select i).ToArray();
                            var dispinsCollection = dispinstempCollection.Distinct().ToArray();
                            if (dispinsCollection.Length == 0)
                            {
                                MessageBox.Show($"{CompDwgName} Pins Information Not Avaialble,Pls Check Once");
                                continue;
                            }
                            
                            for (int PDRowUpdateDIS = 0; PDRowUpdateDIS <= Constants.arrPanelDetails.GetLength(0) - 1; PDRowUpdateDIS++)
                            {
                                if (Constants.arrPanelDetails[PDRowUpdateDIS, 0].Equals(CompDwgName))
                                {
                                    Constants.arrPanelDetails[PDRowUpdateDIS, Constants.colPD_Usage - 1] = SampleEquPinNumber;
                                    Constants.arrPanelDetails[PDRowUpdateDIS, Constants.colPD_F_Type - 1] = CompType;
                                }
                            }
                            
                            #region // Coordinate settings
                            CompXdist = (Constants.SheetWidth/3);
                            if (dispinsCollection.Length - 1 <= 2) Constants.DIS_Cur_Height_Decre_Count = dispinsCollection.Length * 4;
                            CompYdist = Constants.SheetHeight - Constants.DIS_Cur_Height_Count - 24;
                            Constants.DIS_Cur_Height_Count = Constants.DIS_Cur_Height_Count + (dispinsCollection.Length * 4 + 8 + 4 * 4) + Constants.DIS_Cur_Height_Decre_Count;//41
                            #endregion

                            modMain.GraLine(CompXdist, CompYdist, dispinsCollection.Length, CompDwgName, dispinsCollection, CompPN, Assosiate_PartNumbers, Equipment_Box_Info); //'CompMaxPin)

                            break;
                        case "MSW" or "REL":
                            #region//Coordinate settings
                            CompXdist = (Constants.SheetWidth / 2) + Constants.RELMSW_Cur_Width_Incre_Count;
                            Constants.RELMSW_Cur_Width_Incre_Count = Constants.RELMSW_Cur_Width_Incre_Count + 50;
                            Constants.RELMSW_Cur_Height_Decre_Count = Constants.RELMSW_Cur_Height_Decre_Count + 10 - 1;
                            CompYdist = (Constants.SheetHeight/2) - Constants.RELMSW_Cur_Height_Decre_Count * 8;
                            #endregion

                            modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                            //modOPCommand.AddSWTSymbolAttributes(CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                            //modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                            break;
                        case "IND":
                            CompXdist = (Constants.SheetWidth / 6) + Constants.IND_Cur_Width_Incre_Count;
                            Constants.IND_Cur_Width_Incre_Count = Constants.IND_Cur_Width_Incre_Count + 70;
                            Constants.IND_Cur_Height_Decre_Count = Constants.IND_Cur_Height_Decre_Count + 10 - 1;
                            CompYdist = Constants.SheetHeight - Constants.SWT_Cur_Height_Decre_Count * 8 + 10;

                            modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                            modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                            break;
                        case "BUS":
                            CompXdist = (Constants.SheetWidth / 6) + Constants.BUS_Cur_Width_Incre_Count;
                            Constants.BUS_Cur_Width_Incre_Count = Constants.BUS_Cur_Width_Incre_Count + 80;
                            Constants.BUS_Cur_Height_Decre_Count = Constants.BUS_Cur_Height_Decre_Count + 10 - 1;
                            CompYdist = Constants.SheetHeight - Constants.BUS_Cur_Height_Decre_Count * 8 + 30;
                            modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                            modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                            break;
                        case "ANT":
                            CompXdist = (Constants.SheetWidth / 6) + Constants.ANT_Cur_Width_Incre_Count;
                            Constants.ANT_Cur_Width_Incre_Count = Constants.ANT_Cur_Width_Incre_Count + 140;
                            Constants.ANT_Cur_Height_Decre_Count = Constants.ANT_Cur_Height_Decre_Count + 10 - 1;
                            CompYdist = Constants.SheetHeight - Constants.ANT_Cur_Height_Decre_Count * 8 + 50;
                            modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                            modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                            break;
                        case "DD":
                            CompXdist = (Constants.SheetWidth / 6) + Constants.DD_Cur_Width_Incre_Count;
                            Constants.DD_Cur_Width_Incre_Count = Constants.DD_Cur_Width_Incre_Count + 300;
                            Constants.DD_Cur_Height_Decre_Count = Constants.DD_Cur_Height_Decre_Count + 10 - 1+50;
                            CompYdist = Constants.SheetHeight - Constants.DD_Cur_Height_Decre_Count * 8;
                            modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                            modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                            break;
                        case "FUS":
                            CompXdist = (Constants.SheetWidth / 6) + Constants.FUS_Cur_Width_Incre_Count;
                            Constants.FUS_Cur_Width_Incre_Count = Constants.FUS_Cur_Width_Incre_Count + 400;
                            Constants.FUS_Cur_Height_Decre_Count = Constants.FUS_Cur_Height_Decre_Count + 10 - 1;
                            CompYdist = Constants.SheetHeight - Constants.FUS_Cur_Height_Decre_Count * 8 + 100;
                            modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                            modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                            break;
                        case "CNT":
                            modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                            modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                            break;
                        case "SNR":
                            modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                            modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                            break;
                        case "GROUND":
                            modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                            modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                            break;
                        case "ML":
                            modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                            modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                            break;
                        case "ERM" or "EM":
                            CompXdist = (Constants.SheetWidth / 6) + Constants.ERM_Cur_Width_Incre_Count;
                            Constants.ERM_Cur_Width_Incre_Count = Constants.ERM_Cur_Width_Incre_Count + 280;
                            Constants.ERM_Cur_Height_Decre_Count = Constants.ERM_Cur_Height_Decre_Count + 10 - 1;
                            CompYdist = Constants.SheetHeight - Constants.ERM_Cur_Height_Decre_Count * 8 + 150;
                            modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                            modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                            break;
                        case "LMP":
                            modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                            modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                            break;
                        case "M1" or "M2":
                            modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                            modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                            break;
                        case "TRK":
                            modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                            modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                            break;
                        case "NEW":
                            modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                            modOPCommand.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, Constants.el_ExecFilePath, CompType);
                            break;
                        default :
                            break;
                }                    
            }
                
                if (CompType.Equals("EQU"))
                {
                    CompXdist = CompXdist + Xcounter;
                    CompYdist = CompYdist + Ycounter - 40;
                }
            }
        }*/
        #endregion
        #endregion

    }
}


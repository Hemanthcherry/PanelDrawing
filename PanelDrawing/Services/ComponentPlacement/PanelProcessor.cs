using Panel_Drawing.Forms;
using PanelDrawing.Core.Constants;
using PanelDrawing.Core.Sorting;
using PanelDrawing.Core.Utilities;
using PanelDrawing.Logging;
using PanelDrawing.Models;
using PanelDrawing.Services.Connectors;
using PanelDrawing.Services.Infrastructure;
using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Numerics;
using System.Text.RegularExpressions;

namespace PanelDrawing.Services.ComponentPlacement
{
    public class PanelProcessor
    {           
        public static (double X, double Y) GetNextEquPosition(string side, double compWidth, double compHeight)
        {
            // Validate
            if (!side.Equals("L", StringComparison.OrdinalIgnoreCase) &&
                !side.Equals("R", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("EQU position must be LEFT or RIGHT");

            double posX;
            double posY;

            // ⚡ X-Coordinate (Left/Right)
            if (side.Equals("L", StringComparison.OrdinalIgnoreCase))
            {
                if (PanelConstants.CursorY_EQU_Left - compHeight < PanelConstants.MarginYEQU)
                {
                    PanelConstants.CursorY_EQU_Left = PanelConstants.SheetHeight - PanelConstants.MarginYEQU;

                    PanelConstants.CursorX_EQU_Left += PanelConstants.ComponentSpacing_X_EQU;
                }
                posX = PanelConstants.CursorX_EQU_Left;

                // ⚡ Y-Coordinate (Top → Bottom)
                posY = PanelConstants.CursorY_EQU_Left;

                // Move Y downward for next EQU component
                PanelConstants.CursorY_EQU_Left = PanelConstants.CursorY_EQU_Left - compHeight - PanelConstants.ComponentSpacingY;
            }
            else
            {
                if (PanelConstants.CursorY_EQU_Right - compHeight < PanelConstants.MarginY)
                {
                    PanelConstants.CursorY_EQU_Right = PanelConstants.SheetHeight - PanelConstants.MarginYEQU;

                    PanelConstants.CursorX_EQU_Right -= PanelConstants.ComponentSpacing_X_EQU;
                }
                // Margin from right side
                posX = PanelConstants.CursorX_EQU_Right;

                // ⚡ Y-Coordinate (Top → Bottom)
                posY = PanelConstants.CursorY_EQU_Right;

                // Move Y downward for next EQU component
                PanelConstants.CursorY_EQU_Right = PanelConstants.CursorY_EQU_Right - compHeight - PanelConstants.ComponentSpacingY;
            }

            //// ⚡ Y-Coordinate (Top → Bottom)
            //posY = Constants.CursorY_EQU;

            //// Move Y downward for next EQU component
            //Constants.CursorY_EQU = Constants.CursorY_EQU  - compHeight - Constants.ComponentSpacingY;

            if (PanelConstants.CursorX_EQU_Left + compWidth >= PanelConstants.CursorX) // = added on Jan 14
            {
                PanelConstants.CursorX = PanelConstants.CursorX_EQU_Left + 200;

                PanelConstants.CursorX_DIS = PanelConstants.CursorX_EQU_Left + 100;
            }

            posX = getEvenPosition(posX);
            posY = getEvenPosition(posY);

            return (posX, posY);
        }

        // An EQU connector name ending in _J1-_J24 or a single letter (A-H, J-N, P-Z; I/O excluded to
        // avoid confusion with 1/0) denotes the second side of a double-sided EQU connector.
        private static bool IsDoubleSidedEquConnector(string componentName) =>
            Regex.IsMatch(componentName, @"_[Jj](?:[1-9]|1\d|2[0-4])$") ||
            Regex.IsMatch(componentName, @"_[A-HJ-NP-Za-hj-np-z]$");

        private static double getEvenPosition(double pos)
        {
            pos = Math.Round(pos);
            if (pos % 2 != 0)
            {
                pos = pos + 1;
            }

            return pos;
        }

        public static (double X, double Y) GetNextComponentPosition(double compWidth, double compHeight, string compType)
        {
            // If next component X exceeds sheet width → go to new row

            if (compType =="DIS" || compType == "TBK" || compType == "TER" || compType == "REL")
            {
                if(PanelConstants.CursorY_DIS - compHeight < PanelConstants.MarginYEQU)
                {
                    PanelConstants.CursorY_DIS = PanelConstants.SheetHeight - PanelConstants.MarginYEQU;

                    PanelConstants.CursorX_DIS += PanelConstants.ComponentSpacingX + PanelConstants.ColumnWidth;

                    PanelConstants.ColumnWidth = 0;
                }

                double posX = PanelConstants.CursorX_DIS;
                // Current component position
                //if (compType == "DIS") 
                //{
                    posX = PanelConstants.CursorX_DIS + 30;
                //}
                
                double posY = PanelConstants.CursorY_DIS; //- compHeight;

                PanelConstants.CursorY_DIS = PanelConstants.CursorY_DIS - compHeight - PanelConstants.ComponentSpacingY;

                // Update widest component in this column
                if (compWidth > PanelConstants.ColumnWidth)
                {
                    PanelConstants.ColumnWidth = compWidth;
                }
            
                if (PanelConstants.CursorX_DIS >= PanelConstants.CursorX)
                {
                    PanelConstants.CursorX = PanelConstants.CursorX_DIS + compWidth;
                }

                posX = getEvenPosition(posX);
                posY = getEvenPosition(posY);

                return (posX, posY);
            }
            else
            {
                if (PanelConstants.CursorY + compHeight > PanelConstants.SheetHeight - PanelConstants.MarginYEQU)
                {
                    // Move to NEXT Column
                    // Constants.CursorX = Constants.MarginX;
                    PanelConstants.CursorY = PanelConstants.MarginY;

                    // Constants.CursorX = Constants.CursorX_EQU_Left + 40;

                    PanelConstants.CursorX += PanelConstants.ColumnWidth + PanelConstants.ComponentSpacingX;

                    // Reset Row Height
                    PanelConstants.ColumnWidth = 0;
                }

                // Current component position
                double posX = PanelConstants.CursorX;
                double posY = PanelConstants.CursorY; //- compHeight;

                // Move X cursor to right for next component
                PanelConstants.CursorY += compHeight + PanelConstants.ComponentSpacingY;

                // Update tallest component in this row
                if (compHeight > PanelConstants.ColumnWidth)
                    PanelConstants.ColumnWidth = compWidth;

                posX = getEvenPosition(posX);
                posY = getEvenPosition(posY);

                return (posX, posY);

                //if (PanelConstants.CursorX + compWidth > PanelConstants.CursorX_EQU_Right - PanelConstants.MarginX)
                //{
                //    // Move to NEXT ROW
                //    // Constants.CursorX = Constants.MarginX;
                //   PanelConstants.CursorX = PanelConstants.CursorX_DIS + 80; // commented on Jan 14th

                //   // Constants.CursorX = Constants.CursorX_EQU_Left + 40;

                //    PanelConstants.CursorY += PanelConstants.RowHeight + PanelConstants.ComponentSpacingY;

                //    // Reset Row Height
                //    PanelConstants.RowHeight = 0;
                //}

                //// Current component position
                //double posX = PanelConstants.CursorX;
                //double posY = PanelConstants.CursorY; //- compHeight;

                //// Move X cursor to right for next component
                //PanelConstants.CursorX += compWidth + PanelConstants.ComponentSpacingX;

                //// Update tallest component in this row
                //if (compHeight > PanelConstants.RowHeight)
                //    PanelConstants.RowHeight = compHeight;

                //posX = getEvenPosition(posX);
                //posY = getEvenPosition(posY);

                //return (posX, posY);
            }            
        }

        public static void InitializeLayout()
        {
            // X starts at left margin
            PanelConstants.CursorX = PanelConstants.MarginX;

            PanelConstants.CursorX_DIS = /*Constants.MarginX + */120;  //180 commented on Jan 14

            PanelConstants.CursorX_EQU_Left = PanelConstants.MarginXEQU;

            PanelConstants.CursorX_EQU_Right = PanelConstants.SheetWidth - PanelConstants.MarginXEQU;

            // Y starts at TOP of the sheet
            //Constants.CursorY = Constants.SheetHeight - Constants.MarginY;

            // Y starts at Bottom of the sheet
            PanelConstants.CursorY = PanelConstants.MarginY;

            PanelConstants.CursorY_EQU = PanelConstants.SheetHeight - PanelConstants.MarginYEQU;
            PanelConstants.CursorY_EQU_Right = PanelConstants.SheetHeight - PanelConstants.MarginYEQU;
            PanelConstants.CursorY_EQU_Left = PanelConstants.SheetHeight - PanelConstants.MarginYEQU;

            PanelConstants.CursorY_DIS = PanelConstants.SheetHeight - PanelConstants.MarginYEQU ;

            // Track tallest component in the current row
            PanelConstants.RowHeight = 0;

            PanelConstants.ColumnWidth = 0;
        }

        public static (double Width, double Height) GetComponentDefaultSize(string compType)
        {
            return compType.ToUpper() switch
            {
                "SPL" => (50, 50),
                "SCB" => (30, 30),
                "TCB" => (50, 50),
                "TBK" => (80, 100),
                "TER" => (80, 100),
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

        // Shared placement logic for component types that only need AddSymbol + AddSymbolAttributes.
        private static void PlaceGenericSymbol(string compType, string compDwgName, string macroName, string compPN, string cbTypeName,
            double compWidth, double compHeight, string missingMacroMsg)
        {
            if (!string.IsNullOrEmpty(macroName))
            {
                (double x, double y) = GetNextComponentPosition(compWidth, compHeight, compType);
                CommandProcessor.AddSymbol(macroName, compDwgName, x, y, compPN, PanelConstants.el_ExecFilePath);
                CommandProcessor.AddSymbolAttributes(x, y, compDwgName, cbTypeName, compPN, PanelConstants.el_ExecFilePath, compType);
                ApplicationLogger.Info($"Custom Component {compDwgName} ({compType}) with macro {macroName} placed at X={x}, Y={y}");
            }
            else
            {
                ApplicationLogger.Warn($"Macro or Part Number missing for component '{compDwgName}' ({compType}) Please verify the part number(Y) col in 'data_extraction.csv' and the macro in library file.Skipping Placement");
                MessageBox.Show(missingMacroMsg, "Macro / Part Number Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Same as PlaceGenericSymbol but for circuit breaker types (SCB/TCB) which also carry a voltage rating.
        private static void PlaceCircuitBreakerSymbol(string compType, string compDwgName, string macroName, string compPN, string cbTypeName, string cbVoltage,
            double compWidth, double compHeight, string missingMacroMsg)
        {
            if (!string.IsNullOrEmpty(macroName))
            {
                (double x, double y) = GetNextComponentPosition(compWidth, compHeight, compType);
                CommandProcessor.AddSymbol(macroName, compDwgName, x, y, compPN, PanelConstants.el_ExecFilePath);
                CommandProcessor.AddCBSymbolAttributes(x, y, compDwgName, cbTypeName, compPN, cbVoltage, PanelConstants.el_ExecFilePath, compType);
                ApplicationLogger.Info($"Custom Component {compDwgName} ({compType}) with macro {macroName} placed at X={x}, Y={y}");
            }
            else
            {
                ApplicationLogger.Warn($"Macro or Part Number missing for component '{compDwgName}' ({compType}) Please verify the part number(Y) col in 'data_extraction.csv' and the macro in library file.Skipping Placement");
                MessageBox.Show(missingMacroMsg, "Macro / Part Number Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public static void InitiateStep1()
        {
            ApplicationLogger.Info($"P1 Components Placement Started...........");
            InitializeLayout();

            PanelConstants.processedItems = new List<string>();
            PanelConstants.remainingItems = new List<string>();

            var components = PanelConstants.panelComponentProperties
                .OrderBy(c =>
                {
                    // PRIORITY 0 & 1: EQU Components
                    if (c.ComponentType == "EQU")
                    {
                        bool isDoubleSide = IsDoubleSidedEquConnector(c.ComponentName ?? "");
                        return isDoubleSide ? 1 : 0; // 0 = Single, 1 = Double
                    }

                    // PRIORITY 2: DIS Components
                    if (c.ComponentType == "DIS") return 2;

                    if (c.ComponentType == "TBK") return 3;

                    if (c.ComponentType == "REL") return 4;


                    // PRIORITY 3: Everything else
                    return 5;
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

                string msg = $"Macro or Part Number missing for component '{CompDwgName}' ({CompType}).\n" +
                                   $"Part Number : {CompPN}\n" +
                                   $"Please verify the part number (Y) col in 'data_extraction.csv' and the macro in library file. " +
                                   $"Symbol placement skipped.";

                ApplicationLogger.Info($"Component {CompDwgName} ({CompType}) started placement");
                try
                {
                    // Populating panel details with the below panelComponentProperties data for the P2 action
                    foreach (var row in PanelConstants.panelDetailsList)
                    {
                        if (row.FromConnector == CompDwgName)
                        {
                            row.Usage = SampleEquPinNumber;
                            row.FromType = CompType;
                            row.GroupId = GroupId;
                            row.WireLength = Wire_Length;
                            row.WireType = Wire_Type;
                        }

                        if (row.ToConnector == CompDwgName)
                        {
                            row.ToType = CompType;
                        }
                    }
                    switch (CompType)
                    {
                        case "SPL":
                            if (string.IsNullOrEmpty(MacroName))
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(30, 30, CompType);
                                CommandProcessor.AddSymbolSPL_OOTB(CompDwgName, CompXdist, CompYdist, CompPN, PanelConstants.el_ExecFilePath);
                                ApplicationLogger.Info($"OOTB Component {CompDwgName} ({CompType}) placed at X={CompXdist}, Y={CompYdist}");
                            }
                            else
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, CompHeight, CompType);

                                CommandProcessor.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, PanelConstants.el_ExecFilePath);
                                // modOPCommand.AddSWTSymbolAttributes(CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath, CompDwgName);
                                CommandProcessor.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, PanelConstants.el_ExecFilePath, CompType);
                                ApplicationLogger.Info($"Custom Component {CompDwgName} ({CompType}) with macro {MacroName} placed at X={CompXdist}, Y={CompYdist}");
                            }
                            continue;

                        case "TBK":
                        /*(CompXdist, CompYdist) = GetNextComponentPosition(80, 100);

                        modOPCommand.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath);
                        modOPCommand.AddSWTSymbolAttributes(CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath, CompDwgName);
                        continue;*/

                        case "TER":
                            var tbkPins = ConnectorPinService.getPinsOfComponent(CompDwgName);
                            if (tbkPins.Count == 0)
                            {
                                MessageBox.Show($"{CompDwgName} Pins Not Available, skipping symbol placement");
                                continue;
                            }
                            if (string.IsNullOrEmpty(MacroName))
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, CompHeight, CompType);

                                CommandProcessor.AddOOTB_TER_SymbolForTerminal(CompDwgName, CompXdist, CompYdist, CompPN, PanelConstants.el_ExecFilePath, "0", tbkPins, ShuntInfo);
                                ApplicationLogger.Info($"OOTB Component {CompDwgName} ({CompType}) placed at X={CompXdist}, Y={CompYdist}");
                            }
                            else
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, CompHeight, "TER_CUST");

                                CommandProcessor.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, PanelConstants.el_ExecFilePath);
                                CommandProcessor.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, PanelConstants.el_ExecFilePath, CompType);
                                ApplicationLogger.Info($"Custom Component {CompDwgName} ({CompType}) with macro {MacroName} placed at X={CompXdist}, Y={CompYdist}");
                            }
                            continue;

                        case "EQU":
                            var equPins = ConnectorPinService.getPinsOfComponent(CompDwgName);
                            PanelConstants.listPinsOfEqu = equPins;

                            int NoOfPins = equPins.Count;
                            PanelConstants.txtEquName = CompDwgName;

                            bool isDouble = IsDoubleSidedEquConnector(CompDwgName);

                            //(CompXdist, CompYdist) = GetNextComponentPosition(80, 100);
                            int CompHeightEQU = (NoOfPins * 4) + 20 + 20;

                            string fileName = $"{PanelConstants.textPanelPartNumber} - {PanelConstants.txtEquName} - PanelEquOri.txt";

                            PanelConstants.sPanelEQUori_File = Path.Combine(PanelConstants.Electre_Temp_Folder_Path, fileName);

                            if (isDouble)
                            {
                                string info =
                                   PanelConstants.dataExtractionListBelow.FirstOrDefault(x => string.Equals(x.ConnectorName, CompDwgName, StringComparison.OrdinalIgnoreCase))
                                   ?.SymbolName?.Trim() ?? "";

                                // 2. Base orientation
                                string Ori = info switch
                                {
                                    "cont_sth_md" => "L",
                                    "cont_sth_mg" => "R",
                                    "cont_sth_half_l" => "R",
                                    "cont_sth_half_r" => "L",
                                    _ => ""
                                };

                                using (StreamWriter writer = new StreamWriter(PanelConstants.sPanelEQUori_File))
                                {
                                    PanelSortingService.Export_PanelEquOri(writer, equPins, Ori);
                                }

                                (CompXdist, CompYdist) = GetNextEquPosition(Ori, 60, CompHeightEQU);
                                //(CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, CompHeightEQU, CompType);

                                string root = CompDwgName.Split('_')[0];

                                if (!PanelConstants.remainingItems.Contains(root))
                                {
                                    DoubleConnectorPlacementService.DrawDoubleConnectorSymbol(CompXdist, CompYdist, PanelConstants.listPinsOfEqu.Count, CompDwgName, PanelConstants.listPinsOfEqu, CompPN, AssocPNs, EquipBox, root);

                                    PanelConstants.remainingItems.Add(root);
                                }
                            }
                            else
                            {
                                //frmpanelori.lblEquName.Text = CompDwgName;
                                //frmpanelori.ShowDialog();
                                //var side = frmpanelori.SelectedSide;
                              
                                var Ori = (comp.SymbolName ?? "").Trim().Contains("contact_sth_mr") ? "L" : "R";

                                using (StreamWriter writer = new StreamWriter(PanelConstants.sPanelEQUori_File))
                                {
                                    PanelSortingService.Export_PanelEquOri(writer, equPins, Ori);
                                }

                                // Get position based on LEFT / RIGHT
                                //(CompXdist, CompYdist) = GetNextEquPosition(side,40,compHeight: Constants.arrPinsOfEqu.Length * 10);
                                (CompXdist, CompYdist) = GetNextEquPosition(Ori, 60, CompHeightEQU);

                                SingleConnectorPlacementService.DrawSingleConnectorSymbol(CompXdist, CompYdist, CompMaxPin, CompDwgName, Ori, SampleEquPinNumber, CompPN, /*frmpanelori,*/ AssocPNs, EquipBox, Looms);
                                //frmpanelori.Close();
                            }
                            ApplicationLogger.Info($"OOTB Component {CompDwgName} ({CompType}) placed at X={CompXdist}, Y={CompYdist}");
                            continue;

                        case "DIS":
                            var disPins = ConnectorPinService.getPinsOfComponent(CompDwgName);
                            if (disPins.Count == 0)
                            {
                                MessageBox.Show($"{CompDwgName} Pins Not Available");
                                continue;
                            }
                            int yCoordinate_DIS = (disPins.Count * 4) + 12 + 18;

                            string baseName = CompDwgName.Length > 2 ? CompDwgName[..^2] : CompDwgName;

                            // If already processed → exit
                            if (PanelConstants.processedItems.Contains(baseName))
                                continue;

                            (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, yCoordinate_DIS, CompType);

                            BreakConnectorPlacementService.DrawBreakConnectorSymbol(CompXdist, CompYdist, disPins.Count, CompDwgName, disPins, CompPN, AssocPNs, EquipBox);
                            ApplicationLogger.Info($"OOTB Component {CompDwgName} ({CompType}) placed at X={CompXdist}, Y={CompYdist}"); 
                            continue;


                        case "REL":
                            var relPins = ConnectorPinService.GetPinsofOOTBRelay(CompDwgName);
                            int yCoordinateREL = (relPins.Count * 5) + 40;

                            //(CompXdist, CompYdist) = GetNextComponentPosition(60, yCoordinateREL);
                            //modOPCommand.AddSymbolREL_OOTB(CompDwgName, CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath, relPins);

                            if (string.IsNullOrEmpty(MacroName))
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, yCoordinateREL, CompType);
                                CommandProcessor.AddSymbolREL_OOTB(CompDwgName, CompXdist, CompYdist, CompPN, PanelConstants.el_ExecFilePath, relPins);
                                ApplicationLogger.Info($"OOTB Component {CompDwgName} ({CompType}) placed at X={CompXdist}, Y={CompYdist}");
                            }
                            else
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, CompHeight, "REL_CUST");
                                CommandProcessor.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, PanelConstants.el_ExecFilePath);
                                CommandProcessor.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, PanelConstants.el_ExecFilePath, CompType);
                                ApplicationLogger.Info($"Custom Component {CompDwgName} ({CompType}) with macro {MacroName} placed at X={CompXdist}, Y={CompYdist}");
                            }
                            continue;

                        case "SWT":
                            if (!string.IsNullOrEmpty(MacroName))
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, CompHeight, CompType);

                                CommandProcessor.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, PanelConstants.el_ExecFilePath);
                                CommandProcessor.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, PanelConstants.el_ExecFilePath, CompType);
                                ApplicationLogger.Info($"Custom Component {CompDwgName} ({CompType}) with macro {MacroName} placed at X={CompXdist}, Y={CompYdist}");
                            }
                            else
                            {
                                ApplicationLogger.Warn($"Macro or Part Number missing for component '{CompDwgName}' ({CompType}) Please verify the part number(Y) col in 'data_extraction.csv' and the macro in library file.Skipping Placement");

                                MessageBox.Show(
                                    msg,
                                    "Macro / Part Number Missing",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                            }
                            //modOPCommand.AddSWTSymbolAttributes(CompXdist, CompYdist, CompPN, Constants.el_ExecFilePath, CompDwgName);
                            continue;

                        case "MSW":
                            if (!string.IsNullOrEmpty(MacroName))
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, CompHeight, CompType);
                                CommandProcessor.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, PanelConstants.el_ExecFilePath);
                                CommandProcessor.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, PanelConstants.el_ExecFilePath, CompType);
                                ApplicationLogger.Info($"Custom Component {CompDwgName} ({CompType}) with macro {MacroName} placed at X={CompXdist}, Y={CompYdist}");
                            }
                            else
                            {
                                ApplicationLogger.Warn($"Macro or Part Number missing for component '{CompDwgName}' ({CompType}) Please verify the part number(Y) col in 'data_extraction.csv' and the macro in library file.Skipping Placement");

                                MessageBox.Show(
                                    msg,
                                    "Macro / Part Number Missing",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                            }
                            continue;

                        case "GND" or "GROUND":
                            if (string.IsNullOrEmpty(MacroName))
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(20, 20, CompType);
                                CommandProcessor.AddSymbolGND(CompDwgName, CompXdist, CompYdist, CompPN, PanelConstants.el_ExecFilePath);
                                ApplicationLogger.Info($"OOTB Component {CompDwgName} ({CompType}) placed at X={CompXdist}, Y={CompYdist}");
                            }
                            else
                            {
                                (CompXdist, CompYdist) = GetNextComponentPosition(CompWidth, CompHeight, CompType);
                                CommandProcessor.AddSymbol(MacroName, CompDwgName, CompXdist, CompYdist, CompPN, PanelConstants.el_ExecFilePath);
                                CommandProcessor.AddSymbolAttributes(CompXdist, CompYdist, CompDwgName, CBType_Name, CompPN, PanelConstants.el_ExecFilePath, CompType);
                                ApplicationLogger.Info($"Custom Component {CompDwgName} ({CompType}) with macro {MacroName} placed at X={CompXdist}, Y={CompYdist}");
                            }
                            continue;

                        case "SCB":
                        case "TCB":
                            PlaceCircuitBreakerSymbol(CompType, CompDwgName, MacroName, CompPN, CBType_Name, CBVoltage, CompWidth, CompHeight, msg);
                            continue;

                        // IND, BUS, ANT, DD, FUS, SNR, EM/ERM, LMP, TRK, NEW, RES, CAP, IDT, POT and any
                        // other component type share the same "place with macro, else warn" placement logic.
                        case "IND":
                        case "BUS":
                        case "ANT":
                        case "DD":
                        case "FUS":
                        case "SNR":
                        case "EM" or "ERM":
                        case "LMP":
                        case "TRK":
                        case "NEW":
                        case "RES":
                        case "CAP":
                        case "IDT":
                        case "POT":
                        default:
                            PlaceGenericSymbol(CompType, CompDwgName, MacroName, CompPN, CBType_Name, CompWidth, CompHeight, msg);
                            continue;
                    }
                }
                catch (Exception ex)
                {
                    ApplicationLogger.Error($"Component {CompDwgName} ({CompType}) placement failed", ex);
                    continue;
                }
            }

            ApplicationLogger.Info($"P1 Components Placement Completed............");
        }


    }
}


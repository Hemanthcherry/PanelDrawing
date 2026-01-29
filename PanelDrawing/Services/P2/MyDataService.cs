using PanelDrawing.CommonOperations;
using PanelDrawing.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanelDrawing.Services.P2
{
    public static class MyDataService
    {
        public static void AssignOrientation()
        {
            List<string> processedEquList = new List<string>();

            foreach (var row in Constants.panelDetailsList)
            {
                string fromConnector = row.FromConnector;
                string toConnector = row.ToConnector;
                string fromType = row.FromType;
                string toType = row.ToType;

                if (fromType == "EQU")
                {
                    Constants.txtEquName = fromConnector;
                }

                if (modStandard.SearchAndAppend(Constants.txtEquName, processedEquList))
                {
                    string filePath =  $"{Constants.Electre_Temp_Folder_Path}{Constants.textPanelPartNumber} - {Constants.txtEquName} - PanelEquOri.txt";

                    Constants.sPanelEQUori_File = filePath;

                    if (!modStandard.ValidateFileSelection(filePath))
                    {
                        Console.WriteLine($"{filePath} - PanelOri file is missing");
                        continue;
                    }

                    try
                    {
                        foreach (var line in File.ReadLines(filePath))
                        {
                            string[] parts = line.Split(';');
                            if (parts.Length < 3) continue;

                            string pinNumber = parts[1];
                            string orientation = parts[2];

                            // Assign orientation to From side
                            var matchFrom = Constants.panelDetailsList
                                .Where(x => x.FromConnector == Constants.txtEquName &&
                                            x.FromPin == pinNumber);

                            foreach (var r in matchFrom)
                                r.FromOrientation = orientation;

                            // Assign orientation to To side
                            var matchTo = Constants.panelDetailsList
                                .Where(x => x.ToConnector == Constants.txtEquName &&
                                            x.ToPin == pinNumber);

                            foreach (var r in matchTo)
                                r.ToOrientation = orientation;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading file: {ex.Message}");
                    }
                }
            }
        }

        public static void CoordinatesToPDPin()
        {
            foreach (var pd in Constants.panelDetailsList)
            {
                bool Tdetails = false;
                bool Fdetails = false;

                foreach (var md in Constants.MyDataList) // MyDataList = list of ElectreMyDataRow
                {
                    //   MATCH T (TO side)
                    if (!Tdetails &&
                        md.ConnectorName == pd.ToConnector &&
                        md.PinNumber == pd.ToPin && string.Equals(md.Column22, Constants.textPanelPartNumber, StringComparison.OrdinalIgnoreCase))
                    {
                        if (md.ComponentType == "TBK" && !string.IsNullOrEmpty(pd.FromOrientation) && pd.FromOrientation != md.Orientation &&
                        Constants.MyDataList.Count(x => x.ConnectorName == md.ConnectorName && x.PinNumber == md.PinNumber && x.Column22 == Constants.textPanelPartNumber) > 1)
                        {
                            continue;
                        }
                        pd.TPinX = md.PinX;
                        pd.TPinY = md.PinY;
                        pd.ToType = md.ComponentType;
                        pd.Usage = md.Usage;

                        // update Usage for matching F_Pin row
                        var rowTo = Constants.panelDetailsList.FirstOrDefault(x => x.FromPin == md.PinNumber);

                        if (rowTo != null)
                            rowTo.Usage = md.Usage;

                        Tdetails = true;
                    }

                    //   MATCH F (FROM side)
                    if (!Fdetails &&
                        md.ConnectorName == pd.FromConnector &&
                        md.PinNumber == pd.FromPin && string.Equals(md.Column22, Constants.textPanelPartNumber, StringComparison.OrdinalIgnoreCase))
                    {
                        if (md.ComponentType == "TBK" && !string.IsNullOrEmpty(pd.ToOrientation) && pd.ToOrientation != md.Orientation &&
                        Constants.MyDataList.Count(x => x.ConnectorName == md.ConnectorName && x.PinNumber == md.PinNumber && x.Column22 == Constants.textPanelPartNumber) > 1)
                        {
                            continue;
                        }
                        pd.PinX = md.PinX;
                        pd.PinY = md.PinY;
                        pd.FromType = md.ComponentType;
                        pd.Usage = md.Usage;

                        // update Usage for matching T_Pin row
                        var rowFrom = Constants.panelDetailsList.FirstOrDefault(x => x.ToPin == md.PinNumber);

                        if (rowFrom != null)
                            rowFrom.Usage = md.Usage;

                        Fdetails = true;
                    }

                    if (Tdetails && Fdetails)
                        break;
                }
                // DEACTIVATE REPEATED CONNECTIONS
                if (Tdetails && Fdetails)
                {
                    pd.Usage = "1";

                    DeactivateRepetitiveConnections(pd.ToConnector,pd.ToPin, pd.FromConnector, pd.FromPin);
                }

                //   UPDATE GroupId / Wire Length / WireType
                string baseWireCode = (pd.WireCode ?? "").Split('/')[0];

                var rowDE = Constants.dataExtractionListBelow
                    .FirstOrDefault(x =>
                        string.Equals(x.ConnectorName?.Trim(), pd.FromConnector?.Trim(), StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(x.PinNumber?.Trim(), pd.FromPin?.Trim(), StringComparison.OrdinalIgnoreCase) &&
                        string.Equals((x.WireNumber ?? "").Trim(), baseWireCode, StringComparison.OrdinalIgnoreCase)
                    );

                if (rowDE != null)
                {
                    pd.GroupId = rowDE.Group;
                    pd.WireLength = rowDE.Length;
                    pd.WireType = rowDE.CableType;
                    var corenumber = !string.IsNullOrEmpty(rowDE.CoreNumber) && rowDE.CoreNumber.Length <=2 ? rowDE.CoreNumber : string.Empty;
                    //pd.WireTypeNumber = rowDE.CoreNumber;
                    pd.WireTypeNumber = corenumber;
                }
            }
        }

        private static void DeactivateRepetitiveConnections(string FC, string FP, string TC, string TP)
        {
            foreach (var md in Constants.MyDataList)
            {
                if (md.ConnectorName == FC &&      // FromConnector
                    md.PinNumber == FP &&      // FromPin
                    md.ConnectorName == TC &&      // ToConnector  (NOTE: original code reused same array!)
                    md.PinNumber == TP)        // ToPin
                {
                    md.Usage = "2";       // Usage column
                    break;
                }
            }
        }       
    }
}

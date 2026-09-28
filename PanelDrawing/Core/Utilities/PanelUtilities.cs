using PanelDrawing.Core.Constants;
using PanelDrawing.Models;
using System.Diagnostics;

namespace PanelDrawing.Core.Utilities
{
    public class PanelUtilities
    {     
        public static  void CloseVbsFiles(string VbsFilePath)
        { 
            // Start the VBScript file
            Process vbScriptProcess = new Process();
            vbScriptProcess.StartInfo.FileName = "cscript";
            vbScriptProcess.StartInfo.Arguments = "//B //Nologo "+""+ VbsFilePath;
            vbScriptProcess.StartInfo.UseShellExecute = false;
            vbScriptProcess.Start();

            // Close the process if still running
            if (!vbScriptProcess.HasExited)
            {
                vbScriptProcess.Kill();
            }

            vbScriptProcess.Dispose();
        }

        public static bool IsFileExist(string filepath)
        {
            if(!File.Exists(filepath)) 
                return false;
            else return true;
        }

        public static void  CreateTemp_El_Exec_File(string filepath)
        {
            PanelConstants.el_Exec_Temp_FilePath = string.Concat(@"C:\Users\", Environment.GetEnvironmentVariable("USERNAME", EnvironmentVariableTarget.Machine), @"\", filepath);//@"C:\Users\Admin\el_exec";
            if (File.Exists(PanelConstants.el_Exec_Temp_FilePath)) File.Delete(PanelConstants.el_Exec_Temp_FilePath);
            File.Copy(PanelConstants.el_ExecFilePath, PanelConstants.el_Exec_Temp_FilePath);
        }

        public static string[] ConvertListInto1DArray(List<string> lstlist)
        {
            string[] arrtemp = new string[lstlist[0].Split(',').Count() * lstlist.Count];
            int intTempCount = 0;
            for (int i = 0; i <= lstlist.Count - 1; i++)
            {
                for (int j = 0; j <= lstlist[i].Split(',').Count() - 1; j++)
                {
                    string[] temparr = lstlist[i].Split(',');
                    arrtemp[intTempCount] = temparr[j];//lstlist[i].Split(',').ToString();
                    intTempCount++;
                }
            }
            return arrtemp;
        }

        public static List<string> ConvertTextFileIntoList(string txtFileName)
        {
            List<string> lstlineInfo = new List<string>();
            foreach (string line in File.ReadLines(txtFileName))
            {
                lstlineInfo.Add(line);
            }
            return lstlineInfo;
        }

        public static void UpdatePanelDetailsTextFile(List<PanelDetailsRow> panelDetailsList)
        {
            if (panelDetailsList == null || panelDetailsList.Count == 0)
                return;

            // Prepare file name
            PanelConstants.PanelDetailsFullFileName =
                Path.Combine(PanelConstants.Electre_Temp_Folder_Path,
                             $"{PanelConstants.textPanelPartNumber}-{PanelConstants.panelDetailsTextFileName}");

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
            File.WriteAllLines(PanelConstants.PanelDetailsFullFileName, outputLines);
        }

        public static void ComponentsCreatedTextFileCreation()
        {
            string filePath = Path.Combine(PanelConstants.Electre_Temp_Folder_Path, PanelConstants.ComponentsCreatedTextFileName);
            File.WriteAllLines(filePath, PanelConstants.arrComponentsCreated ?? Array.Empty<string>());
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
            }
        }

        public static List<PanelComponentProperties> GatherPanelComponentProperties(string panelName)
        {
            // GatherPanelComponentProperties(panelName);
            // Clear previous results
            PanelConstants.panelComponentProperties.Clear();

            // STEP 1: Get all components belonging to panel & having part number
            var panelComponents = PanelConstants.dataExtractionList
                //  Constants.dataExtractionListAbove
                .Where(x => x.Panel == panelName &&
                            PanelConstants.listComponentsWithPartNumber.Contains(x.ConnectorName ?? string.Empty))
                .Select(x => x.ConnectorName)
                .Distinct()
                .ToList();

            // Get the ground components in the panel
            var GNDcomponents = PanelConstants.dataExtractionList.
                Where(x => x.Panel == panelName && (x.ComponentType == "GND" || x.ComponentType == "GROUND")).Select(x => x.ConnectorName).Distinct().ToList();

            panelComponents.AddRange(GNDcomponents);

            panelComponents = panelComponents.Distinct().ToList();

            foreach (var compName in panelComponents)
            {
                // STEP 2: Get all DE rows for this component
                var compDE = PanelConstants.dataExtractionList
                    .Where(x => x.ConnectorName == compName && x.Panel == panelName)
                    .ToList();

                // STEP 3: Get associated part numbers
                var associatedPNs = compDE
                    .Select(x => x.CoreNumber)     // CoreNumber = PartNumber
                    .Where(x => !string.IsNullOrEmpty(x)
                            && (!int.TryParse(x, out int coreNum) || coreNum < 1 || coreNum > 18)) // corenumber and part number is in same column, so get only part number as corenumber is 1 to 18
                    .Select(x => x!)
                    .Distinct()
                    .ToList();

                // STEP 4: Primary part number
                string primaryPN = CommandProcessor.GetPrimaryPartNumber(associatedPNs);

                // STEP 5: Get extra properties DEBelow
                var firstDEBelow = PanelConstants.dataExtractionListBelow
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
                            PanelConstants.dataExtractionListBelow
                            .Where(x => x.ConnectorName == compName &&
                                        !string.IsNullOrEmpty(x.Shunt))
                            .Select(x => $"{x.ConnectorName},{x.PinNumber},{x.FunctionalDesignation},{x.ComponentType},{x.Shunt}"));
                }

                // STEP 7: Get info from Library Catalog
                var lib = PanelConstants.libCatalogList
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
                string equipInfo = compDE.FirstOrDefault()?.EquipmentName ?? "";
                string loomsInfo = compDE.FirstOrDefault()?.BundleName ?? "";

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

                PanelConstants.panelComponentProperties.Add(compProps);
            }

            return PanelConstants.panelComponentProperties;
        }

        public static void InitiateOutPutFile(string el_ExecfilePath)
        {
            try
            {
                File.WriteAllText(el_ExecfilePath, string.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

    }
}


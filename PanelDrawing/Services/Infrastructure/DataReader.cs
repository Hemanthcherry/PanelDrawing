using PanelDrawing.Core.Constants;
using PanelDrawing.Core.Sorting;
using PanelDrawing.Models;

namespace PanelDrawing.Services.Infrastructure
{
    public static class DataReader
    {
        public static void DataExtractionReader(string filePath)
        {
            // 🧹 Always clear old data before loading new
            PanelConstants.dataExtractionList.Clear();
            PanelConstants.dataExtractionListAbove.Clear();
            PanelConstants.dataExtractionListBelow.Clear();
            PanelConstants.listPanels.Clear();
            PanelConstants.listComponents.Clear();
            PanelConstants.listComponentsWithPartNumber.Clear();
            PanelConstants.listComponentsWitOuthPartNumber.Clear();

            // Validate file existence
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Data extraction file not found at: {filePath}");
            try
            {
                foreach (var line in File.ReadLines(filePath))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue; // skip empty lines

                    var columns = line.Split(';');

                    // Use safe access to prevent IndexOutOfRangeException
                    var record = new ElectreObject
                    {
                        SheetName = columns.ElementAtOrDefault(0),
                        SheetNumber = columns.ElementAtOrDefault(1),
                        DrawingNumber = columns.ElementAtOrDefault(2),
                        DefaultGauge = columns.ElementAtOrDefault(3),
                        BundleName = columns.ElementAtOrDefault(4),
                        EquipmentName = columns.ElementAtOrDefault(5),
                        ConnectorName = columns.ElementAtOrDefault(6),
                        PinNumber = columns.ElementAtOrDefault(7),
                        Ends = columns.ElementAtOrDefault(8),
                        FunctionalDesignation = columns.ElementAtOrDefault(9),
                        SymbolName = columns.ElementAtOrDefault(10),
                        ComponentType = columns.ElementAtOrDefault(11),
                        WireNumber = columns.ElementAtOrDefault(12),
                        Group = columns.ElementAtOrDefault(13),
                        Gauge = columns.ElementAtOrDefault(14),
                        CableType = columns.ElementAtOrDefault(15),
                        Length = columns.ElementAtOrDefault(16),
                        Signal = columns.ElementAtOrDefault(17),
                        Layer = columns.ElementAtOrDefault(18),
                        OverShield = columns.ElementAtOrDefault(19),
                        Tag1 = columns.ElementAtOrDefault(20),
                        Net = columns.ElementAtOrDefault(21),
                        SubNet = columns.ElementAtOrDefault(22),
                        Shunt = columns.ElementAtOrDefault(23),
                        CoreNumber = columns.ElementAtOrDefault(24),
                        Tag2 = columns.ElementAtOrDefault(25),
                        Tag3 = columns.ElementAtOrDefault(26),
                        Voltage = columns.ElementAtOrDefault(27),
                        Panel = columns.ElementAtOrDefault(28),
                        NoMegger = columns.ElementAtOrDefault(29),
                        Tag4 = columns.ElementAtOrDefault(30),
                        Tag5 = columns.ElementAtOrDefault(31),
                        Tag6 = columns.ElementAtOrDefault(32),
                        Tag7 = columns.ElementAtOrDefault(33),
                    };

                    PanelConstants.dataExtractionList.Add(record);
                }
            }
            catch (IOException)
            {
                string fileName = Path.GetFileName(filePath);

                MessageBox.Show($"The file '{fileName}' is open in Excel. Please close it and try again.",
                                "File In Use",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                Application.Exit();
            }
            // ⚙️ Split into Above and Below lists
            // "Above" → component info (no wiring)
            PanelConstants.dataExtractionListAbove = PanelConstants.dataExtractionList
                .Where(x => string.IsNullOrEmpty(x.WireNumber))
                .ToList();

            // "Below" → wiring info (contains wire number or layer)
            PanelConstants.dataExtractionListBelow = PanelConstants.dataExtractionList
                .Where(x => !string.IsNullOrEmpty(x.WireNumber))
                .ToList();

            PanelConstants.listPanels = PanelConstants.dataExtractionList
               .Where(x => !string.IsNullOrEmpty(x.Panel))
               .Select(x => x.Panel)
               .Distinct()
               .ToList();

            PanelConstants.listComponents = PanelConstants.dataExtractionList
                .Where(x => !string.IsNullOrEmpty(x.ConnectorName))
                .Select(x => x.ConnectorName)
                .Distinct()
                .ToList();

            //Constants.listComponentsWithPartNumber = Constants.dataExtractionList
            //     .Where(x => !string.IsNullOrEmpty(x.ConnectorName)
            //              && !string.IsNullOrEmpty(x.CoreNumber)
            //              && (!int.TryParse(x.CoreNumber, out int coreNum) || coreNum < 1 || coreNum > 18))
            //     .Select(x => x.ConnectorName)
            //     .Distinct()
            //     .ToList();

            PanelConstants.listComponentsWithPartNumber = PanelConstants.dataExtractionListBelow
                .Where(x => !string.IsNullOrEmpty(x.ConnectorName) && x.Panel.Equals(PanelConstants.textPanelPartName, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.ConnectorName)
                .Distinct()
                .ToList();

            PanelConstants.listComponentsWitOuthPartNumber = PanelConstants.listComponents
                .Except(PanelConstants.listComponentsWithPartNumber)
                .ToList();
        }

        // Reads the panel info file and returns the panel number and panel name.
        public static (string PanelNumber, string PanelName) LoadPanelDetails(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Panel Info file not found at: {filePath}");

            var lines = File.ReadAllLines(filePath).Where(l => !string.IsNullOrWhiteSpace(l)).ToList();

            if (lines.Count == 0)
                throw new Exception("Panel details file is empty.");

            var parts = lines[0].Split(',').Select(x => x.Trim()).ToArray();

            if (parts.Length < 2)
            {
                throw new InvalidOperationException("Panel info file does not contain both panel number and panel name.");
            }
            return (parts[0], parts[1]);
        }

        public static List<BorderInfo> BorderInfoReader(string filePath)
        {
            // 🧹 Always clear old data before loading new
            PanelConstants.borderInfoList.Clear();

            // Validate file existence
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Border Info file not found at: {filePath}");

            var lines = ExcelService.ReadExcelFile(filePath);

            // Skip header row (i = 0)
            for (int i = 1; i < lines.Count; i++)
            {
                var c = lines[i].Split(';');

                // Use safe access to prevent IndexOutOfRangeException
                var border = new BorderInfo
                {
                    Template = c.ElementAtOrDefault(0),
                    Size = c.ElementAtOrDefault(1),
                    Orientation = c.ElementAtOrDefault(2),
                    Width = c.ElementAtOrDefault(3),
                    Height = c.ElementAtOrDefault(4),
                    Description = c.ElementAtOrDefault(5),
                    Unit = c.ElementAtOrDefault(6),
                    Border = c.ElementAtOrDefault(7),
                    Offset_X = c.ElementAtOrDefault(8),
                    Height1 = c.ElementAtOrDefault(9),
                    Range_Number_Horz = c.ElementAtOrDefault(10),
                    Width_Range_Horz = c.ElementAtOrDefault(11),
                    Squaring_Horz = c.ElementAtOrDefault(12),
                    Squaring_Vert = c.ElementAtOrDefault(13),
                    Undefined1 = c.ElementAtOrDefault(14),
                    Undefined2 = c.ElementAtOrDefault(15),
                    Range_Number_Vert = c.ElementAtOrDefault(16),
                    Width_Range_Vert = c.ElementAtOrDefault(17),
                    First_Character_Horz = c.ElementAtOrDefault(18),
                    First_Character_Vert = c.ElementAtOrDefault(19),
                    X_Ref = c.ElementAtOrDefault(20)
                };

                PanelConstants.borderInfoList.Add(border);
            }

            return PanelConstants.borderInfoList;
        }

        public static List<LibraryCatalog> LibraryCatalogReader(string filePath)
        {
            var list = new List<LibraryCatalog>();

            string[] lines = null;
            try
            {
                lines = File.ReadAllLines(filePath);

                if (lines.Length == 0)
                    throw new Exception("CSV file is empty.");
            }
            catch (IOException)
            {
                MessageBox.Show(
                    $"The file:\n{filePath}\nis currently in use.\nPlease close the file (Excel or other program) and run the application again.",
                    "File In Use",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                Application.Exit();   // closes WinForms app safely
                Environment.Exit(1);  // ensures full termination
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Unexpected error while reading library catalog.\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                Application.Exit();
                Environment.Exit(1);
            }
            // Detect delimiter from header row
            char delimiter = DetectDelimiter(lines[0]);

            // Skip header row (line 0)
            for (int i = 1; i < lines.Length; i++)
            {
                //var columns = lines[i].Split(delimiter);
                string line = lines[i].Trim();

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var columns = line.Split(delimiter)
                                  .Select(x => x.Trim())
                                  .ToList();

                var obj = new LibraryCatalog
                {
                    Symbol2D = columns.ElementAtOrDefault(0),
                    Symbol3D_PartCatia = columns.ElementAtOrDefault(1),
                    RefInternal = columns.ElementAtOrDefault(2),
                    RefSupplier = columns.ElementAtOrDefault(3),
                    Supplier = columns.ElementAtOrDefault(4),
                    Description = columns.ElementAtOrDefault(5),
                    Section = columns.ElementAtOrDefault(6),
                    MaxPins = columns.ElementAtOrDefault(7),
                    Diam = columns.ElementAtOrDefault(8),
                    MandatoryAccessory1 = columns.ElementAtOrDefault(9),
                    MandatoryAccessory2 = columns.ElementAtOrDefault(10),
                    Optionnal1 = columns.ElementAtOrDefault(11),
                    Optionnel2 = columns.ElementAtOrDefault(12),
                    Optionnel3 = columns.ElementAtOrDefault(13),
                    Optionnel4 = columns.ElementAtOrDefault(14),
                    Optionnel5 = columns.ElementAtOrDefault(15),
                    Optionnel6 = columns.ElementAtOrDefault(16),
                    Optionnel7 = columns.ElementAtOrDefault(17),
                    Optionnel8 = columns.ElementAtOrDefault(18),
                    Protection1 = columns.ElementAtOrDefault(19),
                    Protection2 = columns.ElementAtOrDefault(20),
                    Family = columns.ElementAtOrDefault(21),
                    Overlength = columns.ElementAtOrDefault(22),
                    Stripping = columns.ElementAtOrDefault(23),
                    Units = columns.ElementAtOrDefault(24),
                    Mnemo_Famille = columns.ElementAtOrDefault(25),
                    Mass = columns.ElementAtOrDefault(26),
                    RefInternal2 = columns.ElementAtOrDefault(27),
                    Color = columns.ElementAtOrDefault(28),
                    Standard = columns.ElementAtOrDefault(29),
                    Type_CatElectre = columns.ElementAtOrDefault(30),
                    Trad_Famille = columns.ElementAtOrDefault(31),
                    Price = columns.ElementAtOrDefault(32),
                    X_mm = columns.ElementAtOrDefault(33),
                    Y_mm = columns.ElementAtOrDefault(34),
                    Z_mm = columns.ElementAtOrDefault(35),
                    Spare_mm = columns.ElementAtOrDefault(36),
                    MountingTime = columns.ElementAtOrDefault(37),
                    TopViewSymbol = columns.ElementAtOrDefault(38),
                    SymbolBox_BundleDrawing = columns.ElementAtOrDefault(39),
                    Sealing = columns.ElementAtOrDefault(40),
                    MF = columns.ElementAtOrDefault(41),
                    TypeConnection_or_FamilleContact = columns.ElementAtOrDefault(42),
                    SealOnWire = columns.ElementAtOrDefault(43),

                    RefSupplier2 = columns.ElementAtOrDefault(44),
                    RefSupplier3 = columns.ElementAtOrDefault(45),
                    RefSupplier4 = columns.ElementAtOrDefault(46),
                    RefSupplier5 = columns.ElementAtOrDefault(47),
                    RefSupplier6 = columns.ElementAtOrDefault(48),
                    RefSupplier7 = columns.ElementAtOrDefault(49),
                    RefSupplier8 = columns.ElementAtOrDefault(50),
                    RefSupplier9 = columns.ElementAtOrDefault(51),
                    RefSupplier10 = columns.ElementAtOrDefault(52),
                    RefSupplier11 = columns.ElementAtOrDefault(53),
                    RefSupplier12 = columns.ElementAtOrDefault(54),
                    RefSupplier13 = columns.ElementAtOrDefault(55),
                    RefSupplier14 = columns.ElementAtOrDefault(56),
                    RefSupplier15 = columns.ElementAtOrDefault(57),
                    RefSupplier16 = columns.ElementAtOrDefault(58),
                    RefSupplier17 = columns.ElementAtOrDefault(59),
                    RefSupplier18 = columns.ElementAtOrDefault(60),
                    RefSupplier19 = columns.ElementAtOrDefault(61),
                    RefSupplier20 = columns.ElementAtOrDefault(62),
                    RefSupplier21 = columns.ElementAtOrDefault(63),
                    RefSupplier22 = columns.ElementAtOrDefault(64),

                    Status = columns.ElementAtOrDefault(65),
                    IND_Plan = columns.ElementAtOrDefault(66),
                    Counterpart = columns.ElementAtOrDefault(67),
                    Base = columns.ElementAtOrDefault(68),
                    Empreinte_ou_Specif_Doc = columns.ElementAtOrDefault(69),
                    ClassT = columns.ElementAtOrDefault(70),
                    ClasseSealing = columns.ElementAtOrDefault(71),
                    ClassVibration = columns.ElementAtOrDefault(72),
                    SupportFixation = columns.ElementAtOrDefault(73),
                    OuterDiameter = columns.ElementAtOrDefault(74),
                    BendRadius = columns.ElementAtOrDefault(75),
                    PN_Disconnect = columns.ElementAtOrDefault(76),
                    DiamAWG = columns.ElementAtOrDefault(77),
                    CompWidth = double.TryParse(columns.ElementAtOrDefault(78), out double width) ? width : 0,
                    CompHeight = double.TryParse(columns.ElementAtOrDefault(79), out double height) ? height : 0
                };

                list.Add(obj);
            }

            return list;
        }

        private static char DetectDelimiter(string headerLine)
        {
            int commaCount = headerLine.Split(',').Length;
            int semicolonCount = headerLine.Split(';').Length;

            // Most columns wins:
            return semicolonCount > commaCount ? ';' : ',';
        }

        public static void ReadPanelDetails()
        {
            PanelConstants.panelDetailsList.Clear();

            var lines = File.ReadLines(PanelConstants.PanelDetails_FilePath)
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .ToList();

            foreach (var line in lines)
            {
                var parts = line.Split(',').Select(x => x.Trim()).ToArray();

                var row = new PanelDetailsRow
                {
                    FromConnector = parts.ElementAtOrDefault(0),
                    FromPin = parts.ElementAtOrDefault(1),
                    ToConnector = parts.ElementAtOrDefault(2),
                    ToPin = parts.ElementAtOrDefault(3),
                    WireCode = parts.ElementAtOrDefault(4),
                    PinX = parts.ElementAtOrDefault(5),
                    PinY = parts.ElementAtOrDefault(6),
                    TPinX = parts.ElementAtOrDefault(7),
                    TPinY = parts.ElementAtOrDefault(8),
                    Usage = parts.ElementAtOrDefault(9),
                    FromType = parts.ElementAtOrDefault(10),
                    ToType = parts.ElementAtOrDefault(11),
                    FromOrientation = parts.ElementAtOrDefault(12),
                    ToOrientation = parts.ElementAtOrDefault(13),
                    GroupId = parts.ElementAtOrDefault(14),
                    WireLength = parts.ElementAtOrDefault(15),
                    WireType = parts.ElementAtOrDefault(16),
                    WireTypeNumber = parts.ElementAtOrDefault(17),
                };

                PanelConstants.panelDetailsList.Add(row);
            }

            PanelConstants.panelDetailsList = PanelSortingService.SortPanelDetails(PanelConstants.panelDetailsList);
        }

        public static void LoadMyDataFile(string csvPath)
        {
            PanelConstants.MyDataList.Clear();

            // var list = new List<MyData>();

            //if (!File.Exists(csvPath))
            //    return list;

            try
            {
                var lines = File.ReadAllLines(csvPath)
                                .Where(l => !string.IsNullOrWhiteSpace(l))
                                .ToList();

                foreach (var line in lines)
                {
                    // Auto detect delimiter
                    char delimiter = line.Contains(';') ? ';' : ',';

                    var parts = line.Split(delimiter);

                    // Ensure array has 22 columns
                    if (parts.Length < 22)
                    {
                        Array.Resize(ref parts, 22);
                    }

                    var d = new MyDataRow
                    {
                        Column1 = parts.ElementAtOrDefault(0),
                        ConnectorName = parts.ElementAtOrDefault(1),
                        PinNumber = parts.ElementAtOrDefault(2),
                        Orientation = parts.ElementAtOrDefault(3),
                        ComponentType = parts.ElementAtOrDefault(4),
                        Column6 = parts.ElementAtOrDefault(5),
                        Column7 = parts.ElementAtOrDefault(6),
                        Column8 = parts.ElementAtOrDefault(7),
                        Column9 = parts.ElementAtOrDefault(8),
                        Column10 = parts.ElementAtOrDefault(9),
                        Column11 = parts.ElementAtOrDefault(10),
                        Column12 = parts.ElementAtOrDefault(11),
                        Column13 = parts.ElementAtOrDefault(12),
                        Usage = parts.ElementAtOrDefault(13),
                        Column15 = parts.ElementAtOrDefault(14),
                        Column16 = parts.ElementAtOrDefault(15),
                        Column17 = parts.ElementAtOrDefault(16),
                        Column18 = parts.ElementAtOrDefault(17),
                        PinX = parts.ElementAtOrDefault(18),
                        PinY = parts.ElementAtOrDefault(19),
                        Column21 = parts.ElementAtOrDefault(20),
                        Column22 = parts.ElementAtOrDefault(21),
                    };

                    PanelConstants.MyDataList.Add(d);
                }
            }
            catch (IOException)
            {
                MessageBox.Show(
                    $"The file:\n{csvPath}\nis currently in use.\nPlease close the file (Excel or other program) and run the application again.",
                    "File In Use",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                Application.Exit();   // closes WinForms app safely
                Environment.Exit(1);  // ensures full termination
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            // return list;
        }
    }
        
}

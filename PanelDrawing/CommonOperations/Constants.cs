using PanelDrawing.Objects;

namespace PanelDrawing.CommonOperations
{
    public static class Constants
    {
        // Data extraction related collections
        public static List<ElectreObject> dataExtractionList = new List<ElectreObject>();
        public static List<ElectreObject> dataExtractionListAbove = new List<ElectreObject>();
        public static List<ElectreObject> dataExtractionListBelow = new List<ElectreObject>();      
        public static List<string> listPanels = new List<string>();
        public static List<string> listComponents = new List<string>();
        public static List<string> listComponentsWithPartNumber = new List<string>();
        public static List<string> listComponentsWitOuthPartNumber = new List<string>();

        // Border Info related collection
        public static List<BorderInfo> borderInfoList = new List<BorderInfo>();

        // Lib Catalog related collection
        public static List<LibraryCatalog> libCatalogList = new List<LibraryCatalog>();

        public static List<PanelComponentProperties> panelComponentProperties = new List<PanelComponentProperties>();

        public static List<PanelDetailsRow> panelDetailsList = new List<PanelDetailsRow>();

        public static List<MyDataRow> MyDataList = new List<MyDataRow>();

        // Coordinates section
        public static double CursorX = 0;  // Horizontal (X) Coordinate
        public static double CursorY = 0;  // Vertical (Y) Coordinate
        public static double CursorY_EQU_DIS = 0;
        public static double CursorX_EQU_DIS = 0;

        public static double CursorX_EQU_Left = 0;
        public static double CursorX_EQU_Right = 0;
        public static double CursorY_EQU = 0;
        public static double CursorY_EQU_Left = 0;
        public static double CursorY_EQU_Right = 0;

        public static double LastEquDisX = 0;

        public static double RowHeight = 0;  // keeps track of tallest component in this row

        public static double ColumnWidth = 0;  // keeps track of widest component in this column

        public static double ComponentSpacing_X_EQU = 40;

        // public static double SheetWidthTemp = 0;
        // public static double SheetHeightTemp = 0;

        public static double MarginX = 60; // Horizontal margin
        public static double MarginXEQU = 60;
        public static double MarginYEQU = 40;
        public static double MarginY = 80; // Vertical margin
        public static double ComponentSpacingX = 30;  // Horizontal space between components
        public static double ComponentSpacingY = 30;  // vertical space between components
        // Coordinates section end

        public static double WiringOffset = 0;
        public static List<string> fromConnectorProcessed_P2 = new List<string>();

        public static HashSet<double> UsedX3Values = new HashSet<double>();

        public static string borderInfoFilePath = string.Empty;//@"C:\ELECTRE\electre_customize\system\Border_info.csv";
        public static string panelInfoFilePath = string.Empty;//@"C:\electre_projects\PANEL_DWG2\PANEL_DWG2_PANELDRAWINGS\templ\TempFiles\PanelInfo.txt";
        public static string dataExtractionFilePath = string.Empty; //@"C:\electre_projects\PANEL_DWG2\PANEL_DWG2_PANELDRAWINGS\schema\data_extraction.csv";
        public static string el_ExecFilePath = string.Empty;//@"C:\Users\Admin\el_exec";
        public static string el_Exec_Temp_FilePath = string.Empty;//@"C:\Users\Admin\el_exec";
        public static string PanelDetails_FilePath = string.Empty;
        public static string Elec_Proj_Schem_Folder_Path = string.Empty;//@"C:\electre_projects\PANEL_DWG2\PANEL_DWG2_PANELDRAWINGS\PANEL_DWG2_PANELDRAWINGS_PanelDrawings\Schem";
        public static string sComponent_Catalog_File = string.Empty; //@"C:\ELECTRE\electre_customize\lib_Demo_MKII.csv";
        public static string Electre_Temp_Folder_Path = string.Empty;//@"C:\electre_projects\PANEL_DWG2\PANEL_DWG2_PANELDRAWINGS\templ\TempFiles\";
        public static string My_Data_File_Path = string.Empty; //@"C:\electre_projects\PANEL_DWG2\PANEL_DWG2_PANELDRAWINGS\schema\My_Data.csv";
        public static string PD_Symb_Inputs = string.Empty;
        //**************************
        //use below path to run el_exec file from vbs file
        //public const string Custom_Programs_File_Path = @"C:\ELECTRE\electre_customize\system\vbs\custom_programs.vbs";//****
        //C:\ELECTRE\electre_customize\system\PanelDrawing.exe  Run Dotnet8 PanelDrawing.exe file from this path ***
        //**************************
        //public const string My_Data_File_Path = @"C:\electre_projects\PANEL_DWG2\PANEL_DWG2_PANELDRAWINGS\templ\TempFiles\AAAAA-PanelDetails.txt"
        public const string PD_Title = "Panel Drawing";
        public const string Panel_Main_Fields_Not_Empty = "Fields should not be Empty,Please Provide proper information to Proceed... ";
        //"Fields should not be Empty,Please Provide proper information to Proceed... "
        public const string info = "info";
        public const string cb_t = "cb_t";
        public const string cb_s = "cb_s";
        public const string spl = "sth_";//splices
        public const string rel = "rel";//relay
        public const string ind = "ind";//relay
        public const string ant = "ant";//relay
        public const string bus = "bus";//relay
        public const string dd = "dd";//relay
        public const string fus = "fus";//relay
        public const string cnt = "cnt";//relay
        public const string snr = "snr";//relay
        public const string tbk= "sth_";//relay
        public const string ter = "sth_";//terminal block
        public const string cont = "cont";
        public const string strSpace = " ";
        public const string strComma = ",";
        public const string panelDetailsTextFileName = "PanelDetails.txt";
        public const string msg_file_Missing = "File Is Missing...";
        public const string ComponentsCreatedTextFileName = "ComponentsCreated.txt";
        public const string NewSheetDetails = "NewSheetDetails.txt";
        public const string ComponentsWiringCreatedTextFileName = "ComponentsWiringCreated.txt";
        public const string msgComponents = "Components Already Created/Placed, Do you want reload the Components or Want Continue with P2 Button Click for Wiring ? /n OK - For ReLoad Components No - For P2 Action ";
        public const string msgComponentsWiring = "Please Place the Components On P1 Button Click to proceed further for P2 Button Action...";
        public const string quotationMark = "'";
        public const string El_Exec_Temp_P1 ="el_exec_temp_P1";
        public const string El_Exec_Temp_P2 = "el_exec_temp_P2";
        public const char dashMark = '-';
        //public const char frontslash = '\';
        public const int colBI_UserSheetName = 6;
        public const int colBI_Width = 4;
        public const int colBI_Height = 5;
        public const int colBI_Size = 2;
        public const int colBI_ProgSheetName = 1;
        //Data Extraction Xl File Columns 
        public const int colDE_Connector = 7;
        public const int colDE_Panels = 29;
        public const int colDE_EquipmentBox = 6;
        public const int colDE_Looms = 5;
        public const int colDE_Info = 11;
        public const int colDE_PartNumber = 25;
        public const int colDE_TBKTER_Shunt_Value = 24;
        public const int colDE_Type = 12;
        public const int colDE_CBType_Name = 6;
        public const int colDE_CBType_Voltage = 28;
        public const int colDE_PinNumber = 8;
        public const int colDE_WireCode = 13;//wire code in DE
        public const int colDE_GroupId = 14;//14 col data will store here from data extraction excel file 
        public const int colDE_Wire_Type = 16;//16 col data will store here from data extraction excel file
        public const int colDE_Wire_Length = 17;//17 col data will store here from data extraction excel file
        //Lib Xl File Columns 
        public const int colLibCatalog_Accessory = 10;
        public const int colLibCatalog_InternalPartNumber = 3;
        public const int colLibCatalog_Macro = 1;
        public const int ColLibCatalog_MaxPin = 8;
        public const int temp_rownum = 9999;
        public const int arrPanelComponentsPropertiesMaxCols = 16;//changed 7 to 10
        public const int WireSpacing_X = 5;
        public const int SlantSPL_Name = 1;
        public const int SlantSPL_Yvalue = 2;
        public const int SlantSPL_Count = 3;
        

        //static variables
        public static string Env_Variable_Electre_Proj_Path = string.Empty;
        public static string Env_Variable_Electer_Customize_Path = string.Empty;
        //ELECTRE_CUSTOMIZE
        public static int SheetHeight;
        public static int SheetWidth;
        public static string SheetSize = string.Empty;
        public static string textPanelPartNumber = string.Empty;
        public static string textPanelPartName = string.Empty;
        public static string sPanelEQUori_File = string.Empty;
        public static string Custom_Programs_File = string.Empty;
        public static string txtEquName = string.Empty;
        public static string SheetTemplateName=string.Empty;
        public static string PanelDetailsFullFileName = string.Empty;
        public static string strPDlines = string.Empty;
        public static string strAssosiatePNinfo = string.Empty;
        public static string[] arrUpdatedPanelDetails;
        public static bool bIsComponentPlaced = false;
        public static string[] arrPinsOfEqu;
        public static string[] arrComponentsCreated;
        public static string[,] arrBorderInfo;
        public static string[] arrInfo;
        // public static string[] arrPanelInfo;
        public static string[,] arrSlantSPL;
        public static string[] arrCompNameForWireOffset;
        public static string[] arrCountForWireOffset;
        public static string[] arrSlantJUN;
        public static int SlantSPL_RowCounter;
        public static string[,] arrDEabove;
        public static string strDEPanelaboveAssosiatePNList;
        public static string strDEBelowTERTBK_Shunt_List;
        public static string[,] arrDEbelow;
        public static string[,] arrTableOfLibCatalog;
        public static string[,] arrPanelComponentsProperties;
        public static string[,] arrPanelDetails;
        public static List<string> lstInfo;
        public static List<string> remainingItems;
        public static List<string> processedItems;
        public static string[,] arr2Dinfo;
        public static string[] arrOriPininfo;
        public static string[] arrPanels;//= new string[10];
        public static string[] arrComponents;
        public static string[] arrComponentsWithPartNumber;
        public static string[] arrComponentsWitOuthPartNumber;
        public static string[] arrDEWireTypes;
        public static string[] arrTemp;
        public static List<string> lst_Size_Width_Height_Info;
        public static List<string> lst_Border_Info;
        public static List<string> lst_Orientation_Info;
        public static List<string> lst_WireCodes_Info_Processed;
        public static string[,] arr_My_Data;

        // Wires Variables
        //public const int colLibCatalog_Macro = 1;
        //public const int colLibCatalog_InternalPartNumber = 3;
        //public const int colLibCatalog_Accessory = 10;
        //public const int ColLibCatalog_MaxPin = 8;
        public const int colMD_PinX = 19;//changed 19 to 17 
        public const int colMD_PinY = 20;// changed 20 to 18
        public const int colMD_TextX = 17;
        public const int colMD_TextY = 18;
        public const int colMD_Connector = 2;
        public const int colMD_PinNumber = 3;
        public const int colMD_Type = 5;
        public const int colMD_SheetName = 22;

        public const int colPD_F_Connector = 1;
        public const int colPD_F_Pin = 2;
        public const int colPD_T_Connector = 3;
        public const int colPD_T_Pin = 4;
        public const int colPD_WireCode = 5;
        public const int colPD_F_PinX = 6;
        public const int colPD_F_PinY = 7;
        public const int colPD_T_PinX = 8;
        public const int colPD_T_PinY = 9;
        public const int colPD_Usage = 10;
        public const int colPD_F_Type = 11;
        public const int colPD_T_Type = 12;
        public const int colPD_F_Ori = 13;
        public const int colPD_T_Ori = 14;
        public const int colPD_F_GroupId = 15;//14 col data will store here from data extraction excel file 
        public const int colPD_F_Wire_Length = 16;//17 col data will store here from data extraction excel file
        public const int colPD_F_Wire_Type = 17;//16 col data will store here from data extraction excel file
        public const int colPD_F_Wire_Type_Number = 18;//16 col data will store here from data extraction excel file
        public const int colPD_Max_Columns = 18;//Panel Details max columns defined here
        public static int Sample_Equ_Left_Cur_Height_Count = 1;
        public static int Sample_Equ_Right_Cur_Height_Count = 1;
        public static int CB_Cur_Height_Count = 1;
        public static int SWT_Cur_Height_Count = 1;
        public static int DIS_Cur_Height_Count = 1;
        public static int Cur_Height_Incre_Count = 0;
        public static int Simple_EQU_Right_Cur_Height_Decre_Count = 0;
        public static int Simple_EQU_Left_Cur_Height_Incre_Count = 0;
        public static int CB_Cur_Height_Incre_Count = 0;
        public static int CB_Cur_Width_Incre_Count = 0;
        public static int SWT_Cur_Height_Incre_Count = 0;
        public static int SWT_Cur_Width_Incre_Count = 0;

        public static int DIS_Cur_Height_Incre_Count = 0;
        public static int CB_Cur_Height_Decre_Count = 0;
        public static int SWT_Cur_Height_Decre_Count = 0;
        public static int DIS_Cur_Height_Decre_Count = 0;
        public static int Simple_EQU_Cur_Height_Incre_Count = 0;
        public static int TBK_Cur_Height_Decre_Count = 0;
        public static int TBK_Height_Temp_Decre_Count = 0;
        public static int TBK_Cur_Width_Incre_Count = 0;
        public static int Simple_EQU_Right_CON_Height_Decre_Count = 0;
        public static int SPL_Cur_Height_Incre_Count = 0;
        public static int SPL_Height_Decre_Count = 0;
        public static int SPL_Cur_Width_Incre_Count = 0;
        public static int DTC_Cur_Width_Incre_Count = 0;
        public static int DTC_Cur_Height_Decre_Count = 0;
        public static int DTC_Cur_Height_Count = 0;
        public static int RELMSW_Cur_Height_Count = 0;
        public static int RELMSW_Cur_Height_Decre_Count = 0;
        public static int RELMSW_Cur_Width_Incre_Count = 0;
        public static int IND_Cur_Width_Incre_Count = 0;
        public static int IND_Cur_Height_Decre_Count = 0;

        public static int BUS_Cur_Width_Incre_Count = 0;
        public static int BUS_Cur_Height_Decre_Count = 0;

        public static int ANT_Cur_Width_Incre_Count = 0;
        public static int ANT_Cur_Height_Decre_Count = 0;

        public static int DD_Cur_Width_Incre_Count = 0;
        public static int DD_Cur_Height_Decre_Count = 0;

        public static int FUS_Cur_Width_Incre_Count = 0;
        public static int FUS_Cur_Height_Decre_Count = 0;

        public static int ERM_Cur_Width_Incre_Count = 0;
        public static int ERM_Cur_Height_Decre_Count = 0;
        //public static int Remove_Wire_OverLap_Add_Count = 2;//To remove the over lap issue adding count 2 for average of x coordinate for each wire required

        public static string GetWireSymbol(string wire_Type)
        {
            if (string.IsNullOrWhiteSpace(wire_Type))
                return "sth_s";

            return WireSymbolMap.TryGetValue(wire_Type.Trim(), out var symbol)
                ? symbol
                : "sth_s";
        }

        public static readonly Dictionary<string, string> WireSymbolMap = new(StringComparer.OrdinalIgnoreCase)
        {
            // ---- MONO ----
            ["86A9S"] = "sth_s",
            ["86A9US"] = "sth_s",
            ["86A9SS"] = "sth_s",
            ["S"] = "sth_s",
            ["S0"] = "sth_s",
            ["S00"] = "sth_s",

            // ---- Pair ----
            ["SS"] = "sth_ss",

            // ---- SP / TP / STP ----
            ["SP"] = "sth_sp4",
            ["TP"] = "sth_tp4",
            ["TPO"] = "sth_tp4",
            ["STP"] = "sth_stp4",

            // ---- T / TT ----
            ["ST"] = "sth_st4",
            ["TT"] = "sth_tt4",
            ["STT"] = "sth_stt4",

            // ---- Q ----
            ["SQ"] = "sth_sq4",
            ["TQ"] = "sth_tq4",
            ["STQ"] = "sth_stq4",

            // ---- COAX ----
            ["COAX"] = "sth_coax",
            ["CX"] = "sth_coax",
            ["F141"] = "sth_coax",
            ["F187"] = "sth_coax",
            ["F188"] = "sth_coax",
            ["F195"] = "sth_coax",
            ["RG141"] = "sth_coax",
            ["RG142 B/L"] = "sth_coax",
            ["RG179"] = "sth_coax",
            ["RG180"] = "sth_coax",
            ["RG188/U"] = "sth_coax",
            ["RG225"] = "sth_coax",
            ["RG303"] = "sth_coax",
            ["RG316"] = "sth_coax",
            ["SATE 6420YR004 WITH NPC SHEILD"] = "sth_coax",

            // ---- TRIAX ----
            ["TRIAX"] = "sth_triax",
            ["TX"] = "sth_triax",
            ["0024A03110"] = "sth_triax",
            ["07530A5314"] = "sth_triax",
            ["23-8600-000-383"] = "sth_triax",
            ["2524A2506"] = "sth_triax",
            ["50MF1FBSP58"] = "sth_triax",
            ["6024A024"] = "sth_triax",
            ["953211149"] = "sth_triax",
            ["EE2619STK2"] = "sth_triax",
            ["L36115"] = "sth_triax",
            ["P506424A"] = "sth_triax",
            ["TKB24"] = "sth_triax",

            // ---- QUADRAX ----
            ["QUADRAX"] = "sth_quadrax4",
            ["QX"] = "sth_quadrax4",
            ["QX1"] = "sth_quadrax4",
            ["QX2"] = "sth_quadrax4",
            ["QX3"] = "sth_quadrax4",
            ["QX4"] = "sth_quadrax4",

            // --- 5 to 18 cores---

            // Twisted core cables
            ["T5"] = "sth_t54",
            ["T6"] = "sth_t64",
            ["T7"] = "sth_t74",
            ["T8"] = "sth_t84",
            ["T9"] = "sth_t94",
            ["T10"] = "sth_t104",
            ["T11"] = "sth_t114",
            ["T12"] = "sth_t124",
            ["T13"] = "sth_t134",
            ["T14"] = "sth_t144",
            ["T15"] = "sth_t154",
            ["T16"] = "sth_t164",
            ["T17"] = "sth_t174",
            ["T18"] = "sth_t184",

            // Sheilded Twisted core cables
            ["ST5"] = "sth_st54",
            ["ST6"] = "sth_st64",
            ["ST7"] = "sth_st74",
            ["ST8"] = "sth_st84",
            ["ST9"] = "sth_st94",
            ["ST10"] = "sth_st104",
            ["ST11"] = "sth_st114",
            ["ST12"] = "sth_st124",
            ["ST13"] = "sth_st134",
            ["ST14"] = "sth_st144",
            ["ST15"] = "sth_st154",
            ["ST16"] = "sth_st164",
            ["ST17"] = "sth_st174",
            ["ST18"] = "sth_st184",

        };
    }
}

//using Microsoft.Vbe.Interop;
using PanelDrawing.CommonOperations;
using PanelDrawing.Forms;
using PanelDrawing.Objects;
using PanelDrawing.Services.P1;
using PanelDrawing.Services.P2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
namespace Panel_Drawing.Forms
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }
        
        public void frmMain_Load(object sender, EventArgs e)
        {
            #region// Commented Excesize2 Functional Straight and Z line wiring 
            //72,148 348,148
            //Constants.el_ExecFilePath = string.Concat(@"C:\Users\", Environment.GetEnvironmentVariable("USERNAME", EnvironmentVariableTarget.Machine), @"\el_exec");//@"C:\Users\Admin\el_exec";
            //modMain.InitiateOutPutFile(Constants.el_ExecFilePath);

            //modOPCommand.Simple2PointConnection(72, 148, 348, 148, "SS_10", "12");
            //modOPCommand.Simple4PointConnection_Mono_ZLine(74, 238, 386, 172, "SS__02", "14", "c1", "c2", "EQU", "EQU");
            //modOPCommand.Simple4PointConnection_TP_ZLine(74, 238, 386, 172, "TP__02", "14", "c1", "c2", "EQU", "EQU");
            //modOPCommand.Simple4PointConnection_STP_ZLine(74, 238, 386, 172, "STP__02", "14", "c1", "c2", "EQU", "EQU");
            //modOPCommand.Simple2PointConnection_TP_ConnecterType(74, 238, 392, 238, "TP__02", "14", "c1", "c2", "EQU", "EQU");
            //Environment.Exit(0);
            #endregion
            #region// Commented Excesize 1 - Karthik Sir Symbol List Display
            //Constants.Env_Variable_Electer_Customize_Path = Environment.GetEnvironmentVariable("ELECTRE_CUSTOMIZE", EnvironmentVariableTarget.Machine);// ? null : Environment.GetEnvironmentVariable("ELECTRE_CUSTOMIZE", EnvironmentVariableTarget.Machine);
            //string PD_Symb_Demo_FilePath = string.Concat(Constants.Env_Variable_Electer_Customize_Path, @"\MySymbols\",Environment.GetEnvironmentVariable("PD_Symb_List", EnvironmentVariableTarget.Machine)); //@"C:\ELECTRE\electre_customize\MySymbols\FN.txt";
            //var PD_Symb_Demo = TextOperations.ConvertTextFileDataInto2DArray(PD_Symb_Demo_FilePath);
            //string PD_Symb_el_ExecFilePath = string.Concat(@"C:\Users\", Environment.GetEnvironmentVariable("USERNAME", EnvironmentVariableTarget.Machine), @"\PD_Symb_el_exec");//@"C:\Users\Admin\el_exec";
            //InputForm inputform = new InputForm();
            //inputform.ShowDialog();
            //string[] strInputs = Constants.PD_Symb_Inputs.Split(',');
            //if (strInputs.Length.Equals(1) && string.IsNullOrEmpty(strInputs[0])) Environment.Exit(0);
            //modMain.InitiateOutPutFile(PD_Symb_el_ExecFilePath);
            ////modOPCommand.NewSheetWithTB(PD_Symb_el_ExecFilePath, lblPanelNumber.Text, "001", Constants.SheetTemplateName);
            //using (writer = File.AppendText(PD_Symb_el_ExecFilePath))
            //{

            //    #region
            //    //writer.WriteLine("EDI mytemp_new_a4; SAV(CHR(34) + 'C:\\ELECTRE\\electre_projects\\PANEL_DR03\\Schem\\CCCCC_12345' + CHR(34))");//("EDI " + SheetTemplateName + "; SAV (CHR(34)+" + Constants.quotationMark + "" + Constants.Elec_Proj_Schem_Folder_Path + "\\" + pnlnum + "" + Constants.quotationMark + "+CHR(34));");
            //    ////Open the new drawing
            //    //writer.WriteLine("NEW_OPEN_DRAWING " + Constants.quotationMark + "CCCCC_12345" + Constants.quotationMark + ";"); //'Open the new drawing
            //    //writer.WriteLine("MOD_TAG 2012 '" + "CCCCC_12345" + "'; ");// 'Modify the titleblock attributes
            //    //writer.WriteLine("MOD_TAG 2011 '" + "001" + "'; ");
            //    //writer.WriteLine("MOD_TAG 2013 '" + "CCCCC_12345" + "'; ");
            //    //writer.WriteLine("PRT_NAM; ");
            //    //writer.WriteLine("GRID 2.0,2;");
            //    //writer.WriteLine("REMOVE :A; ");// 'Shut the template to avoid sharing issue
            //    #endregion
            //    int X0 = 100, Y0 = 150;
            //    for (int symbcount = 0; symbcount <= strInputs.Length - 1; symbcount++)
            //    {
            //        //ADD I0 macro_00175510302 120,150;NOP;
            //        int Symb_Num = Convert.ToInt32(strInputs[symbcount]);
            //        if (Symb_Num > Convert.ToInt32(PD_Symb_Demo[PD_Symb_Demo.GetLength(0) - 1, 0]))//Convert.ToInt32(strInputs[strInputs.Length - 1]))
            //        {
            //            MessageBox.Show("Input Symbol Number should not exceed = "+ Convert.ToInt32(PD_Symb_Demo[PD_Symb_Demo.GetLength(0)-1, 0]),"Panel Drawing",MessageBoxButtons.OK,MessageBoxIcon.Error);
            //            break;

            //        }
            //        //int X0 = 100, Y0 = 150;
            //        //string iConnectorNameForComment = "breaker2", iPartNumber="Part_Number", CBK_Name = "CBK";
            //        //writer.WriteLine(string.Concat("ADD I0 ", PD_Symb_Demo[Symb_Num-1, 1], " 120,150;NOP;"));
            //        writer.WriteLine("ADD I2 " + PD_Symb_Demo[Symb_Num - 1, 1] + " " + X0 + "," + Y0 + ";NOP;");
            //        X0 = X0 + 20;Y0 = Y0 + 20;
            //        //writer.WriteLine("SMA; " + PD_Symb_Demo[Symb_Num-1, 1] + "; " + X0 + ", " + Y0 + ";");
            //        //writer.WriteLine("IDEN R252 " + X0 + "," + Y0 + ";" + ";");
            //        //writer.WriteLine("LET rect_x (((system_ll_x)+(system_ur_x))/2);");
            //        //writer.WriteLine("LET rect_y (system_ur_y);");
            //        //writer.WriteLine("MOD N51 (rect_x),(rect_y) 0,0 :E'" + iConnectorNameForComment + "';");
            //        ////writer.WriteLine("MOD N2 (rect_x),(system_ll_y) 0,0 :E'" + iConnectorNameForComment + "';");
            //        //writer.WriteLine("MOD N52 (rect_x),(system_ll_y) 0,0 :E'" + iPartNumber + "';");
            //        //writer.WriteLine("MOD N53 (rect_x),(system_ll_y) 0,0 :E'" + CBK_Name + "';");
            //        //MOD N53(rect_x),(system_ll_y)0,0 :E'Ref_Part_Number';
            //    }
            //}
            ////modMain.ExitOutputFile(PD_Symb_el_ExecFilePath);
            //Environment.Exit(0);
            #endregion

            bool IsComponentsCreated = false;
            bool IsComponentsWiringCreated = false;
            #region//Environment Variable Settings
            //Electre_Proj_Path
            Constants.Env_Variable_Electre_Proj_Path = "C:\\ELECTRE\\electre_projects\\PANELDRAWING_JAN07\\";
            //Constants.Env_Variable_Electre_Proj_Path = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ELECTRE_PROJ", EnvironmentVariableTarget.Machine)) ? null : Environment.GetEnvironmentVariable("ELECTRE_PROJ", EnvironmentVariableTarget.Machine); //Environment.GetEnvironmentVariable("ELECTRE_PROJ", EnvironmentVariableTarget.Machine);//string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ELECTRE_PROJ")) ? null : Environment.GetEnvironmentVariable("ELECTRE_PROJ");
            if (string.IsNullOrEmpty(Constants.Env_Variable_Electre_Proj_Path))
            {
                MessageBox.Show($"Project Path not passed as an arguement, Please provide in Custom_Program.vbs file in Electre_Customize/system/vbs path");
                Application.Exit();
            }
            Constants.panelInfoFilePath = string.Concat(Constants.Env_Variable_Electre_Proj_Path, @"\templ\TempFiles\PanelInfo.txt");//@"C:\electre_projects\PANEL_DWG2\PANEL_DWG2_PANELDRAWINGS\templ\TempFiles\PanelInfo.txt";
            Constants.Electre_Temp_Folder_Path = string.Concat(Constants.Env_Variable_Electre_Proj_Path, @"\templ\TempFiles\");
            Constants.dataExtractionFilePath = string.Concat(Constants.Env_Variable_Electre_Proj_Path, @"\schema\data_extraction.csv"); //@"C:\electre_projects\PANEL_DWG2\PANEL_DWG2_PANELDRAWINGS\schema\data_extraction.csv";
            Constants.My_Data_File_Path = string.Concat(Constants.Env_Variable_Electre_Proj_Path, @"\schema\My_Data.csv");//@"C:\electre_projects\PANEL_DWG2\PANEL_DWG2_PANELDRAWINGS\schema\My_Data.csv";
            Constants.Elec_Proj_Schem_Folder_Path = string.Concat(Constants.Env_Variable_Electre_Proj_Path, @"\schem");//@"C:\electre_projects\PANEL_DWG2\PANEL_DWG2_PANELDRAWINGS\PANEL_DWG2_PANELDRAWINGS_PanelDrawings\Schem";
            //Constants.PanelDetails_FilePath = string.Concat(Constants.Electre_Temp_Folder_Path, Constants.textPanelPartNumber, Constants.dashMark, Constants.panelDetailsTextFileName);

            //Electer_Customize_Path
            Constants.Env_Variable_Electer_Customize_Path = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ELECTRE_CUSTOMIZE", EnvironmentVariableTarget.Machine)) ? null : Environment.GetEnvironmentVariable("ELECTRE_CUSTOMIZE", EnvironmentVariableTarget.Machine);
            Constants.borderInfoFilePath = string.Concat(Constants.Env_Variable_Electer_Customize_Path, @"\system\", Environment.GetEnvironmentVariable("Border_File", EnvironmentVariableTarget.Machine)); ;//@"C:\ELECTRE\electre_customize\system\Border_info.csv";
            Constants.sComponent_Catalog_File = string.Concat(Constants.Env_Variable_Electer_Customize_Path, @"\",Environment.GetEnvironmentVariable("Lib_File", EnvironmentVariableTarget.Machine)); //@"C:\ELECTRE\electre_customize\lib_Demo_MKII.csv";
            Constants.Custom_Programs_File = string.Concat(Constants.Env_Variable_Electer_Customize_Path, @"\system\vbs\custom_programs.vbs");//C:\ELECTRE\electre_customize\system\vbs\custom_programs.vbs
            Constants.el_ExecFilePath = string.Concat(@"C:\Users\", Environment.GetEnvironmentVariable("USERNAME", EnvironmentVariableTarget.Machine), @"\el_exec");//@"C:\Users\Admin\el_exec";
            #endregion

            #region//Form level & Input Files Validation
            if (!CommonOperation.IsFileExist(Constants.panelInfoFilePath))
            {
                MessageBox.Show(string.Concat("Panel Info ",Constants.msg_file_Missing, Constants.panelInfoFilePath),Constants.PD_Title,MessageBoxButtons.OK,MessageBoxIcon.Error);
                Environment.Exit(0);
            }
            if (!CommonOperation.IsFileExist(Constants.borderInfoFilePath))
            {
                MessageBox.Show(string.Concat("Border Info ",Constants.msg_file_Missing, Constants.borderInfoFilePath), Constants.PD_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
            else if (!CommonOperation.IsFileExist(Constants.dataExtractionFilePath))
            {
                MessageBox.Show(string.Concat("Data Extraction ",Constants.msg_file_Missing, Constants.dataExtractionFilePath),Constants.PD_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
            else if (!CommonOperation.IsFileExist(Constants.Custom_Programs_File))
            {
                MessageBox.Show(string.Concat("Custom Programs VBS ", Constants.msg_file_Missing, Constants.Custom_Programs_File),Constants.PD_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
            else if (!CommonOperation.IsFileExist(Constants.sComponent_Catalog_File))
            {
                MessageBox.Show(string.Concat("Component Library ", Constants.msg_file_Missing, Constants.sComponent_Catalog_File), Constants.PD_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
            #endregion

            #region//PanelDetails Setting        
            var panelInfo = DataReader.LoadPanelDetails(Constants.panelInfoFilePath);

            lblPanelNumber.Text = panelInfo.PanelNumber;
            Constants.textPanelPartNumber = lblPanelNumber.Text;
            txtPanelName.Text = panelInfo.PanelName;
            Constants.textPanelPartName = txtPanelName.Text;
            #endregion

            #region //Border Info File

            var borderInfoList = DataReader.BorderInfoReader(Constants.borderInfoFilePath);
            // Populate sheet list in UI
            lstSheetSizes.Items.Clear();

            foreach (var border in borderInfoList)
            {
                lstSheetSizes.Items.Add(border.Description);
            }
            #endregion

            #region // BorderInfo old code commented on Nov 14, 2025 
            /*  Constants.lst_Size_Width_Height_Info = new List<string>();
            var lstBorderInfo = ExcelOperations.ReadExcelFile(Constants.borderInfoFilePath);
            for (int i = 0; i <= lstBorderInfo.Count - 1; i++)
            {
                Constants.arrInfo = lstBorderInfo[i].Split(';');
                Constants.arrBorderInfo = new string[lstBorderInfo.Count, Constants.arrInfo.Length];

                for (int j = 0; j <= Constants.arrInfo.Length - 1; j++)
                {
                    Constants.arrBorderInfo[i, j] = Constants.arrInfo[j];
                }
                if (i > 0)
                //if (i > 0 && lstSheetSizes.Items.Count < 13)
                {
                    lstSheetSizes.Items.Add(Constants.arrBorderInfo[i, Constants.colBI_UserSheetName - 1].ToString());
                    string str_Size_Width_Height = string.Concat(i - 1, ",", Constants.arrBorderInfo[i, Constants.colBI_ProgSheetName - 1], ",", Constants.arrBorderInfo[i, Constants.colBI_UserSheetName - 1], ",", Constants.arrBorderInfo[i, Constants.colBI_Size - 1], ",", Constants.arrBorderInfo[i, Constants.colBI_Width - 1], ",", Constants.arrBorderInfo[i, Constants.colBI_Height - 1]);
                    Constants.lst_Size_Width_Height_Info.Add(str_Size_Width_Height);
                }

            }*/
            #endregion

            #region // Commented P1 & P2 EXE Validations for Production Release Purpose --- Button Validation
            //if (File.Exists(string.Concat(Constants.Electre_Temp_Folder_Path, Constants.NewSheetDetails)))// && Environment.GetEnvironmentVariable("PD1_Flag", EnvironmentVariableTarget.Machine).Equals("PD1") && Environment.GetEnvironmentVariable("PD2_Flag", EnvironmentVariableTarget.Machine).Equals("PD2"))
            //{
            //    //Load Form for Create Wiring
            //    lstSheetSizes.Enabled = false;
            //    btnPanelComponents.Enabled = false;
            //    string[] arrtemp = TextOperations.ConvertListInto1DArray(TextOperations.ConvertTextFileIntoList(string.Concat(Constants.Electre_Temp_Folder_Path, Constants.ComponentsCreatedTextFileName)));
            //    lstSheetSizes.SelectedItem = arrtemp[2];
            //    txtSize.Text = arrtemp[3];
            //    txtWidth.Text = arrtemp[4];
            //    txtHeight.Text = arrtemp[5];
            //}
            //else if (!File.Exists(string.Concat(Constants.Electre_Temp_Folder_Path, Constants.NewSheetDetails)))// && Environment.GetEnvironmentVariable("PD1_Flag", EnvironmentVariableTarget.Machine).Equals("PD") && Environment.GetEnvironmentVariable("PD2_Flag", EnvironmentVariableTarget.Machine).Equals("PD2"))
            //{
            //    MessageBox.Show(Constants.msgComponentsWiring);
            //    this.Close();
            //}
            #endregion

            //Read Data extraction File and populate Lists
            DataReader.DataExtractionReader(Constants.dataExtractionFilePath);

            #region // Commented - old Data extraction reading logic, commented on Nov 14,2025

            //var lstDataExtractionInfo = ExcelOperations.ReadExcelFile(Constants.dataExtractionFilePath);
            //int iDEabove = 0;
            //int iDEbelow = 0;
            //string[] arrTotalDE = new string[lstDataExtractionInfo.Count()];
            //for (int i3 = 0; i3 <= lstDataExtractionInfo.Count - 1; i3++)
            //{
            //    arrTotalDE[i3] = lstDataExtractionInfo[i3];
            //    Constants.arrInfo = lstDataExtractionInfo[i3].Split(';');
            //    if (Constants.arrInfo[12] == "" && Constants.arrInfo[18] == "")
            //    {
            //        iDEabove = i3 + 1;
            //        Debug.WriteLine(iDEabove);
            //    }
            //}
            ////iDEabove - With out wiring items,iDEbelow - With Wiring items on Master Drawing
            //iDEbelow = lstDataExtractionInfo.Count - iDEabove;
            //Constants.arrDEabove = new string[iDEabove, Constants.arrInfo.Length];
            ////for DE Above details
            //#region//for DE Above details and storing Panel and Components 
            //int panelCounter = 0;
            //int componentCounter = 0;
            //int componentPartNumbercomponentCounter = 0;
            //Constants.arrPanels = new string[iDEabove - 1];
            //Constants.arrComponents = new string[iDEabove - 1];
            //Constants.arrComponentsWithPartNumber = new string[iDEabove - 1];

            //for (int i2 = 0; i2 <= iDEabove - 1; i2++)
            //{
            //    Constants.arrInfo = arrTotalDE[i2].Trim().Split(";");
            //    for (int j2 = 0; j2 <= Constants.arrInfo.Length - 1; j2++)
            //    {
            //        Constants.arrDEabove[i2, j2] = Constants.arrInfo[j2];
            //        //if(Constants.arrInfo[j2].Equals(Constants.textPanelPartName)) Constants.arrDEPanelabove[i2, j2] = Constants.arrInfo[j2];

            //    }
            //    #region//Panel Count from DE file - Store Panel info into arrPanel Array
            //    if (!string.IsNullOrEmpty(Constants.arrDEabove[i2, Constants.colDE_Panels - 1]))
            //    {
            //        string strPanel = Constants.arrDEabove[i2, Constants.colDE_Panels - 1].ToString();
            //        if (!string.IsNullOrEmpty(strPanel))
            //        {
            //            if (!Constants.arrPanels.Contains(strPanel))
            //            {
            //                Constants.arrPanels[panelCounter] = strPanel;
            //                panelCounter++;
            //            }
            //        }
            //    }
            //    #endregion
            //    #region//All Components in DE - Store Component info into arrComponents Array
            //    if (!string.IsNullOrEmpty(Constants.arrDEabove[i2, Constants.colDE_Connector - 1]))
            //    {
            //        string strComponent = Constants.arrDEabove[i2, Constants.colDE_Connector - 1].ToString();
            //        if (!string.IsNullOrEmpty(strComponent))
            //        {
            //            //if (Constants.arrPanels.Count(x => x.Equals(strComponent)) == 0)
            //            if (!Constants.arrComponents.Contains(strComponent))
            //            {
            //                Constants.arrComponents[componentCounter] = strComponent;
            //                componentCounter++;
            //            }
            //        }
            //    }
            //    #endregion
            //    // Components with Part Number - Store ComponentPartNumber info into arrComponentsWithPartNumber Array
            //    if ((!string.IsNullOrEmpty(Constants.arrDEabove[i2, Constants.colDE_Info - 1]) && !string.IsNullOrEmpty(Constants.arrDEabove[i2, Constants.colDE_PartNumber - 1])))
            //    {
            //        //if ((Constants.arrDEabove[i2, Constants.colDE_Info - 1].ToString().Equals(Constants.info) && Constants.arrDEabove[i2, Constants.colDE_PartNumber - 1].ToString() !=""))
            //        if ((Constants.arrDEabove[i2, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.info) || Constants.arrDEabove[i2, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.ter) || Constants.arrDEabove[i2, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.tbk) || Constants.arrDEabove[i2, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.fus) || Constants.arrDEabove[i2, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.snr) || Constants.arrDEabove[i2, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.dd) || Constants.arrDEabove[i2, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.ant) || Constants.arrDEabove[i2, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.snr) || Constants.arrDEabove[i2, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.spl) || Constants.arrDEabove[i2, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.rel) || Constants.arrDEabove[i2, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.cb_s) || (Constants.arrDEabove[i2, Constants.colDE_Info - 1].Substring(0, 4).Equals(Constants.cb_t)) && Constants.arrDEabove[i2, Constants.colDE_PartNumber - 1].ToString() != ""))
            //        //if ((Constants.arrDEabove[i2, Constants.colDE_Info - 1].ToString().Equals(Constants.info) || Constants.arrDEabove[i2, Constants.colDE_Info - 1].ToString().Equals(Constants.ter) || Constants.arrDEabove[i2, Constants.colDE_Info - 1].ToString().Equals(Constants.tbk) || Constants.arrDEabove[i2, Constants.colDE_Info - 1].ToString().Equals(Constants.fus) || Constants.arrDEabove[i2, Constants.colDE_Info - 1].ToString().Equals(Constants.snr) || Constants.arrDEabove[i2, Constants.colDE_Info - 1].ToString().Equals(Constants.dd) || Constants.arrDEabove[i2, Constants.colDE_Info - 1].ToString().Equals(Constants.ant) || Constants.arrDEabove[i2, Constants.colDE_Info - 1].ToString().Equals(Constants.snr) || Constants.arrDEabove[i2, Constants.colDE_Info - 1].ToString().Equals(Constants.spl) || Constants.arrDEabove[i2, Constants.colDE_Info - 1].ToString().Equals(Constants.rel) || Constants.arrDEabove[i2, Constants.colDE_Info - 1].ToString().Equals(Constants.cb_s) || (Constants.arrDEabove[i2, Constants.colDE_Info - 1].ToString().Equals(Constants.cb_t)) && Constants.arrDEabove[i2, Constants.colDE_PartNumber - 1].ToString() != ""))
            //        {
            //            string strComponentPartNumber = Constants.arrDEabove[i2, Constants.colDE_Connector - 1].ToString();
            //            if (!string.IsNullOrEmpty(strComponentPartNumber))
            //            {
            //                if (!Constants.arrComponentsWithPartNumber.Contains(strComponentPartNumber))
            //                {
            //                    Constants.arrComponentsWithPartNumber[componentPartNumbercomponentCounter] = strComponentPartNumber;
            //                    Debug.Print(strComponentPartNumber);
            //                    componentPartNumbercomponentCounter++;
            //                }
            //            }
            //        }
            //    }
            //}
            //#endregion

            ////Components W/O Part Number - Store ComponentWithOutParNumber info into arrComponentsWitOuthPartNumber Array
            //int iNumberOfComponentsWithoutPartNumber = 0;
            //int iNumberOfComponents = Constants.arrComponents.Where(x => !string.IsNullOrEmpty(x)).Count();
            //int iNumberOfComponentsWithPartNumber = Constants.arrComponentsWithPartNumber.Where(x => !string.IsNullOrEmpty(x)).Count();
            //iNumberOfComponentsWithoutPartNumber = iNumberOfComponents - iNumberOfComponentsWithPartNumber;//Constants.arrComponents.Length - Constants.arrComponentsWithPartNumber.Length;
            //Constants.arrComponentsWitOuthPartNumber = new string[iNumberOfComponentsWithoutPartNumber];
            //int componentsWithOutPartNumberCounter = 0;
            //for (int CwoPN = 0; CwoPN <= iNumberOfComponents; CwoPN++)
            //{
            //    //if (Is_Exist_ComponentWithPartNUmber(Constants.arrComponents[CwoPN], Constants.arrComponentsWithPartNumber) == false)
            //    if (!Constants.arrComponentsWithPartNumber.Contains(Constants.arrComponents[CwoPN]))
            //    {
            //        //if (Constants.arrComponentsWithPartNumber.Contains(Constants.arrComponents[CwoPN])){ }
            //        //componentsWitOuthPartNumberCounter = componentsWitOuthPartNumberCounter + 1;
            //        Constants.arrComponentsWitOuthPartNumber[componentsWithOutPartNumberCounter] = Constants.arrComponents[CwoPN];
            //        componentsWithOutPartNumberCounter = componentsWithOutPartNumberCounter + 1;
            //    }
            //}

            ////for DE below details
            //Constants.arrDEbelow = new string[iDEbelow, Constants.arrInfo.Length];
            //for (int i1 = 0; i1 <= iDEbelow - 1; i1++)
            //{
            //    Constants.arrInfo = arrTotalDE[iDEabove + i1].Trim().Split(";");
            //    for (int j1 = 0; j1 <= Constants.arrInfo.Length - 1; j1++)
            //    {
            //        Constants.arrDEbelow[i1, j1] = Constants.arrInfo[j1];
            //    }
            //}

            #endregion
        }

        #region //Commented old Is_Exist_ComponentWithPartNUmber code on 18 november, 2025
        //private bool Is_Exist_ComponentWithPartNUmber(string arrComponentItem, string[] arrComponentWithPart)
        //{
        //    bool isExist = false;
        //    for (int partItem = 0; partItem <= arrComponentWithPart.Length - 1; partItem++)
        //    {
        //        if (arrComponentItem == arrComponentWithPart[partItem])
        //        {
        //            isExist = true;
        //            break;
        //        }
        //        else
        //        {
        //            isExist = false;
        //        }
        //    }
        //        return isExist;
        //}
        #endregion

            private void btnPanelComponents_Click(object sender, EventArgs e)
            {
                #region//Validation Part
                Constants.PanelDetails_FilePath = string.Concat(Constants.Electre_Temp_Folder_Path, Constants.textPanelPartNumber, Constants.dashMark, Constants.panelDetailsTextFileName);
                if (!CommonOperation.IsFileExist(Constants.PanelDetails_FilePath))
                {
                    MessageBox.Show(string.Concat("Panel Details ", Constants.msg_file_Missing, Constants.PanelDetails_FilePath), Constants.PD_Title);
                    Environment.Exit(0);
                }
                if (string.IsNullOrEmpty(lblPanelNumber.Text) || string.IsNullOrEmpty(txtPanelName.Text) || string.IsNullOrEmpty(txtSize.Text) || string.IsNullOrEmpty(txtWidth.Text) || string.IsNullOrEmpty(txtHeight.Text))
                { 
                    MessageBox.Show(Constants.Panel_Main_Fields_Not_Empty, Constants.PD_Title);
                    return;
                }
            #endregion

                lstSheetSizes.Enabled = false;
                modMain.InitiateOutPutFile(Constants.el_ExecFilePath);
                modOPCommand.NewSheetWithTB(Constants.el_ExecFilePath, lblPanelNumber.Text, "001", Constants.SheetTemplateName);
                modMain.ReadLibCatalog();
                modMain.GatherPanelComponentProperties(Constants.textPanelPartName);
                DataReader.ReadPanelDetails();
                modMain.InitiateStep1();
                modMain.ExitOutputFile(Constants.el_ExecFilePath);
                CommonOperation.CreateTemp_El_Exec_File(Constants.El_Exec_Temp_P1);
                CommonOperation.CloseVbsFiles(Constants.Custom_Programs_File);//C:\ELECTRE\electre_customize\system\Custom_Programs.vbs
                // modMain.UpdatePanelDetailsTextFile(Constants.arrPanelDetails);
                modMain.UpdatePanelDetailsTextFile(Constants.panelDetailsList); 
                Constants.arrComponentsCreated = new string[1];
                Constants.arrComponentsCreated[0] = string.Concat(lblPanelNumber.Text, Constants.strComma, txtPanelName.Text, Constants.strComma, lstSheetSizes.SelectedItem, Constants.strComma, txtSize.Text, Constants.strComma, txtWidth.Text, Constants.strComma, txtHeight.Text);
                modMain.ComponentsCreatedTextFileCreation();
                MessageBox.Show("Panel Component Process Completed", Constants.PD_Title,MessageBoxButtons.OK,MessageBoxIcon.Information);
                this.Close();
            }

        public void btnWires_Click(object sender, EventArgs e)
        {
            Constants.PanelDetails_FilePath = string.Concat(Constants.Electre_Temp_Folder_Path, Constants.textPanelPartNumber, Constants.dashMark, Constants.panelDetailsTextFileName);
            if (string.IsNullOrEmpty(Constants.textPanelPartName))
            {
                MessageBox.Show("Please select the Sheet Name from list", Constants.PD_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(lblPanelNumber.Text) || string.IsNullOrEmpty(txtPanelName.Text) || string.IsNullOrEmpty(txtSize.Text) || string.IsNullOrEmpty(txtWidth.Text) || string.IsNullOrEmpty(txtHeight.Text))
            {
                MessageBox.Show(Constants.Panel_Main_Fields_Not_Empty, Constants.PD_Title);
                return;
            }
            if (!File.Exists(string.Concat(Constants.Electre_Temp_Folder_Path, Constants.ComponentsCreatedTextFileName)))
            {
               var result = MessageBox.Show("Click 'OK' if panel components are placed; otherwise click 'Cancel'.", Constants.PD_Title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.Cancel)
                {
                    Application.Exit();
                    return;
                }

                // MessageBox.Show("Please Create the Panel Components first to proceed further...", Constants.PD_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                // return;
                //Application.Exit();
            }
            lstSheetSizes.Enabled = false;
            modMain.InitiateOutPutFile(Constants.el_ExecFilePath);//Create el_exec file
            DataReader.LoadMyDataFile(Constants.My_Data_File_Path);
            DataReader.ReadPanelDetails();
            MyDataService.AssignOrientation();
            MyDataService.CoordinatesToPDPin();
            WireRouter.DrawWireLine();
            modMain.UpdatePanelDetailsTextFile(Constants.panelDetailsList); // Update PanelDetails file
            modMain.ExitOutputFile(Constants.el_ExecFilePath);//Generate OutputFile and Exit
            CommonOperation.CreateTemp_El_Exec_File(Constants.El_Exec_Temp_P2);
            CommonOperation.CloseVbsFiles(Constants.Custom_Programs_File);
            //File.AppendText(string.Concat(Constants.Electre_Temp_Folder_Path, Constants.ComponentsWiringCreatedTextFileName));
            MessageBox.Show("Wire Routing Of Selected Panel Components Completed..");
            File.Delete(string.Concat(Constants.Electre_Temp_Folder_Path, Constants.ComponentsCreatedTextFileName));
            //File.Delete(string.Concat(Constants.Electre_Temp_Folder_Path, Constants.ComponentsWiringCreatedTextFileName));
            this.Close();//Exit from this app
        }

        #region //Commented old btnWires_Click code on 18 november, 2025
        //public void btnWires_Click(object sender, EventArgs e)
        //{
        //    #region//Validation Part
        //    Constants.PanelDetails_FilePath = string.Concat(Constants.Electre_Temp_Folder_Path, Constants.textPanelPartNumber, Constants.dashMark, Constants.panelDetailsTextFileName);
        //    if (string.IsNullOrEmpty(Constants.textPanelPartName))
        //    {
        //        MessageBox.Show("Please select the Sheet Name from list",Constants.PD_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }
        //    if (string.IsNullOrEmpty(lblPanelNumber.Text) || string.IsNullOrEmpty(txtPanelName.Text) || string.IsNullOrEmpty(txtSize.Text) || string.IsNullOrEmpty(txtWidth.Text) || string.IsNullOrEmpty(txtHeight.Text))
        //    {
        //        MessageBox.Show(Constants.Panel_Main_Fields_Not_Empty, Constants.PD_Title);
        //        return;
        //    }
        //    if (!File.Exists(string.Concat(Constants.Electre_Temp_Folder_Path, Constants.ComponentsCreatedTextFileName)))
        //    {
        //        MessageBox.Show("Please Create the Panel Components first to proceed further...", Constants.PD_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }
        //    #endregion
        //    lstSheetSizes.Enabled = false;
        //    modMain.InitiateOutPutFile(Constants.el_ExecFilePath);//Create el_exec file
        //    modMain.ReadMyData(Constants.My_Data_File_Path);//Parsing of My_Data file & PD file,Assign CoordinatesToPDPin from My_Data file to PD array,Assign AssignOrientation from My_Data file to PD array,DrawWireLine
        //    modMain.UpdatePanelDetailsTextFile(Constants.panelDetailsList); // Update PanelDetails file
        //    modMain.ExitOutputFile(Constants.el_ExecFilePath);//Generate OutputFile and Exit
        //    CommonOperation.CreateTemp_El_Exec_File(Constants.El_Exec_Temp_P2);
        //    CommonOperation.CloseVbsFiles(Constants.Custom_Programs_File);
        //    //File.AppendText(string.Concat(Constants.Electre_Temp_Folder_Path, Constants.ComponentsWiringCreatedTextFileName));
        //    MessageBox.Show("Wire Routing Of Selected Panel Components Completed..");
        //    File.Delete(string.Concat(Constants.Electre_Temp_Folder_Path, Constants.ComponentsCreatedTextFileName));
        //    //File.Delete(string.Concat(Constants.Electre_Temp_Folder_Path, Constants.ComponentsWiringCreatedTextFileName));
        //    this.Close();//Exit from this app
        //}
        #endregion

        private void lstSheetSizes_SelectedIndexChanged(object sender, EventArgs e)
        {
            Fill_Size_Width_Height();
        }

        public void Fill_Size_Width_Height()
        {
            //Dim I As Integer
            // int I = 0;
            //I = lstSheetSizes.SelectedIndex; //'+1 for listindex '0' and +1 to avoid heading in the BorderInfo.csv
            if (lstSheetSizes.SelectedItem == null)
            {
                MessageBox.Show("Please select a size first.");
                return;
            }
            string selectedSize = lstSheetSizes.SelectedItem.ToString();

            List<BorderInfo> selectedItem = Constants.borderInfoList.Where(b => b.Description == selectedSize).ToList();

            if (selectedItem.Count > 0)
            {
                BorderInfo border = selectedItem[0];
                Constants.SheetTemplateName = border.Template;
                txtSize.Text = border.Size;
                txtWidth.Text = border.Width;
                txtHeight.Text = border.Height;
                Constants.SheetHeight = Convert.ToInt16(txtHeight.Text);
                Constants.SheetWidth = Convert.ToInt16(txtWidth.Text);
                Constants.SheetSize = txtSize.Text;
            }

            /*  var swh_info = Constants.lst_Size_Width_Height_Info[I].Split(',');
              Constants.SheetTemplateName = swh_info[1];
              txtSize.Text = swh_info[3]; //Constants.lst_Size_Width_Height_Info[I];
              txtWidth.Text = swh_info[4];//Constants.arrBorderInfo[I, Constants.colBI_Width-1];
              txtHeight.Text = swh_info[5]; //Constants.arrBorderInfo[I, Constants.colBI_Height-1];
              Constants.SheetHeight = Convert.ToInt16(txtHeight.Text);
              Constants.SheetWidth = Convert.ToInt16(txtWidth.Text);
              Constants.SheetSize = txtSize.Text;*/
        }
    }
}

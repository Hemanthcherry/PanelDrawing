//using Microsoft.Vbe.Interop;
using PanelDrawing.CommonOperations;
using PanelDrawing.Forms;
using PanelDrawing.Logs;
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
            #region//Environment Variable Settings
            if (string.IsNullOrEmpty(Constants.Env_Variable_Electre_Proj_Path))
            {
                AppLog.Error("Project Path not passed as argument. Application will exit.");
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

            #region // Commented P2 EXE Auto fill sheet details for Production Release Purpose --- Button Validation
            //if (File.Exists(string.Concat(Constants.Electre_Temp_Folder_Path, Constants.NewSheetDetails)))// && Environment.GetEnvironmentVariable("PD1_Flag", EnvironmentVariableTarget.Machine).Equals("PD1") && Environment.GetEnvironmentVariable("PD2_Flag", EnvironmentVariableTarget.Machine).Equals("PD2"))
            //{
            //    Load Form for Create Wiring
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
        }

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

            AppLog.Info($"Project started. Path = {Constants.Env_Variable_Electre_Proj_Path}");
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
            }
            lstSheetSizes.Enabled = false;
            modMain.InitiateOutPutFile(Constants.el_ExecFilePath);//Create el_exec file
            DataReader.LoadMyDataFile(Constants.My_Data_File_Path);
            DataReader.ReadPanelDetails();
            MyDataService.AssignOrientation();
            MyDataService.CoordinatesToPDPin();
            WireRouter.DrawWireLine();
            modMain.UpdatePanelDetailsTextFile(Constants.panelDetailsList); // Update PanelDetails file
            // modMain.ExitOutputFile(Constants.el_ExecFilePath);//Generate OutputFile and Exit
            CommonOperation.CreateTemp_El_Exec_File(Constants.El_Exec_Temp_P2);
            CommonOperation.CloseVbsFiles(Constants.Custom_Programs_File);
            MessageBox.Show("Wire Routing Of Selected Panel Components Completed..");
            File.Delete(string.Concat(Constants.Electre_Temp_Folder_Path, Constants.ComponentsCreatedTextFileName));
            this.Close();//Exit from this app
        }

        private void lstSheetSizes_SelectedIndexChanged(object sender, EventArgs e)
        {
            Fill_Size_Width_Height();
        }

        public void Fill_Size_Width_Height()
        {
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
        }
    }
}

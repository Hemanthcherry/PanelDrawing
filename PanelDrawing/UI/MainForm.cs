using PanelDrawing.Core.Constants;
using PanelDrawing.Core.Utilities;
using PanelDrawing.Logging;
using PanelDrawing.Models;
using PanelDrawing.Services.ComponentPlacement;
using PanelDrawing.Services.Infrastructure;
using PanelDrawing.Services.Wiring;
using System.Data;

namespace Panel_Drawing.Forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        
        public void frmMain_Load(object sender, EventArgs e)
        {
            #region//Environment Variable Settings
            if (string.IsNullOrEmpty(PanelConstants.Electre_Proj_Path))
            {
                ApplicationLogger.Error("Project Path not passed as argument. Application will exit.");
                MessageBox.Show($"Project Path not passed as an arguement, Please provide in Custom_Program.vbs file in Electre_Customize/system/vbs path");
                Application.Exit();
            }

            PanelConstants.panelInfoFilePath = Path.Combine(PanelConstants.Electre_Proj_Path, "templ", "TempFiles", "PanelInfo.txt");

            PanelConstants.Electre_Temp_Folder_Path = Path.Combine(PanelConstants.Electre_Proj_Path, "templ", "TempFiles");

            PanelConstants.dataExtractionFilePath = Path.Combine(PanelConstants.Electre_Proj_Path, "schema", "data_extraction.csv");

            PanelConstants.My_Data_File_Path = Path.Combine(PanelConstants.Electre_Proj_Path, "schema", "My_Data.csv");

            PanelConstants.Elec_Proj_Schem_Folder_Path = Path.Combine(PanelConstants.Electre_Proj_Path, "schem");

            // Read environment variables
            string electreCustomize = Environment.GetEnvironmentVariable("ELECTRE_CUSTOMIZE", EnvironmentVariableTarget.Machine);
            string borderFile = Environment.GetEnvironmentVariable("Border_File", EnvironmentVariableTarget.Machine);
            string libFile = Environment.GetEnvironmentVariable("Lib_File", EnvironmentVariableTarget.Machine);
            string userName = Environment.GetEnvironmentVariable("USERNAME", EnvironmentVariableTarget.Process);

            // Validate main path
            if (string.IsNullOrWhiteSpace(electreCustomize))
            {
                throw new Exception("ELECTRE_CUSTOMIZE environment variable is not set.");
            }

            PanelConstants.Electre_Customize_Path = electreCustomize;

            // Build paths safely
            PanelConstants.borderInfoFilePath = Path.Combine(electreCustomize, "system", borderFile ?? "");
            PanelConstants.Library_File_Path = Path.Combine(electreCustomize, libFile ?? "");
            PanelConstants.Custom_Programs_File = Path.Combine(electreCustomize, "system", "vbs", "custom_programs.vbs");
            PanelConstants.el_ExecFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "el_exec");
            #endregion

            #region//Form level & Input Files Validation
            if (!PanelUtilities.IsFileExist(PanelConstants.panelInfoFilePath))
            {
                MessageBox.Show(string.Concat("Panel Info ",PanelConstants.msg_file_Missing, PanelConstants.panelInfoFilePath),PanelConstants.PD_Title,MessageBoxButtons.OK,MessageBoxIcon.Error);
                Environment.Exit(0);
            }
            if (!PanelUtilities.IsFileExist(PanelConstants.borderInfoFilePath))
            {
                MessageBox.Show(string.Concat("Border Info ",PanelConstants.msg_file_Missing, PanelConstants.borderInfoFilePath), PanelConstants.PD_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
            if (!PanelUtilities.IsFileExist(PanelConstants.dataExtractionFilePath))
            {
                MessageBox.Show(string.Concat("Data Extraction ",PanelConstants.msg_file_Missing, PanelConstants.dataExtractionFilePath),PanelConstants.PD_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
            if (!PanelUtilities.IsFileExist(PanelConstants.Custom_Programs_File))
            {
                MessageBox.Show(string.Concat("Custom Programs VBS ", PanelConstants.msg_file_Missing, PanelConstants.Custom_Programs_File),PanelConstants.PD_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
            if (!PanelUtilities.IsFileExist(PanelConstants.Library_File_Path))
            {
                MessageBox.Show(string.Concat("Component Library ", PanelConstants.msg_file_Missing, PanelConstants.Library_File_Path), PanelConstants.PD_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
            #endregion

            #region//PanelDetails Setting        
            var panelInfo = DataReader.LoadPanelDetails(PanelConstants.panelInfoFilePath);

            lblPanelNumber.Text = panelInfo.PanelNumber;
            PanelConstants.textPanelPartNumber = lblPanelNumber.Text;
            txtPanelName.Text = panelInfo.PanelName;
            PanelConstants.textPanelPartName = txtPanelName.Text;
            #endregion

            #region //Border Info File

            var borderInfoList = DataReader.BorderInfoReader(PanelConstants.borderInfoFilePath);
            // Populate sheet list in UI
            lstSheetSizes.Items.Clear();

            foreach (var border in borderInfoList)
            {
                lstSheetSizes.Items.Add(border.Description);
            }
            #endregion

            #region // Commented P2 EXE Auto fill sheet details for Production Release Purpose --- Button Validation
            //if (File.Exists(Path.Combine(PanelConstants.Electre_Temp_Folder_Path, PanelConstants.ComponentsCreatedTextFileName)))
            //{
            //    // Load Form for Create Wiring
            //    lstSheetSizes.Enabled = false;
            //    btnPanelComponents.Enabled = false;
            //    string[] arrtemp = PanelUtilities.ConvertListInto1DArray(PanelUtilities.ConvertTextFileIntoList(Path.Combine(PanelConstants.Electre_Temp_Folder_Path, PanelConstants.ComponentsCreatedTextFileName)));
            //    lstSheetSizes.SelectedItem = arrtemp[2];
            //    txtSize.Text = arrtemp[3];
            //    txtWidth.Text = arrtemp[4];
            //    txtHeight.Text = arrtemp[5];
            //}
            //else if (!File.Exists(Path.Combine(PanelConstants.Electre_Temp_Folder_Path, PanelConstants.ComponentsCreatedTextFileName)))
            //{
            //    MessageBox.Show(PanelConstants.msgComponentsWiring);
            //    ApplicationLogger.Warn($"{Path.Combine(PanelConstants.Electre_Temp_Folder_Path, PanelConstants.ComponentsCreatedTextFileName)} not found");
            //    this.Close();
            //}
            #endregion

            //Read Data extraction File and populate Lists
            DataReader.DataExtractionReader(PanelConstants.dataExtractionFilePath);           
        }

        private void btnPanelComponents_Click(object sender, EventArgs e)
        {
            #region//Validation Part
            string panelDetailsFileName = $"{PanelConstants.textPanelPartNumber}-{PanelConstants.panelDetailsTextFileName}";
            PanelConstants.PanelDetails_FilePath = Path.Combine(PanelConstants.Electre_Temp_Folder_Path, panelDetailsFileName);

            if (!PanelUtilities.IsFileExist(PanelConstants.PanelDetails_FilePath))
            {
                MessageBox.Show(string.Concat("Panel Details ", PanelConstants.msg_file_Missing, PanelConstants.PanelDetails_FilePath), PanelConstants.PD_Title);
                Environment.Exit(0);
            }
            if (string.IsNullOrEmpty(lblPanelNumber.Text) || string.IsNullOrEmpty(txtPanelName.Text) || string.IsNullOrEmpty(txtSize.Text) || string.IsNullOrEmpty(txtWidth.Text) || string.IsNullOrEmpty(txtHeight.Text))
            { 
                MessageBox.Show(PanelConstants.Panel_Main_Fields_Not_Empty, PanelConstants.PD_Title);
                return;
            }
            #endregion

            ApplicationLogger.Info($"Project started. Path = {PanelConstants.Electre_Proj_Path}");
            lstSheetSizes.Enabled = false;
            PanelUtilities.InitiateOutPutFile(PanelConstants.el_ExecFilePath);
            CommandProcessor.NewSheetWithTB(PanelConstants.el_ExecFilePath, lblPanelNumber.Text, "001", PanelConstants.SheetTemplateName);
            DataReader.ReadLibCatalog();
            PanelUtilities.GatherPanelComponentProperties(PanelConstants.textPanelPartName);
            DataReader.ReadPanelDetails();
            PanelProcessor.InitiateStep1();
            PanelUtilities.ExitOutputFile(PanelConstants.el_ExecFilePath);
            PanelUtilities.CreateTemp_El_Exec_File(PanelConstants.El_Exec_Temp_P1);
            PanelUtilities.CloseVbsFiles(PanelConstants.Custom_Programs_File);//C:\ELECTRE\electre_customize\system\Custom_Programs.vbs
            // modMain.UpdatePanelDetailsTextFile(Constants.arrPanelDetails);
            PanelUtilities.UpdatePanelDetailsTextFile(PanelConstants.panelDetailsList); 
            PanelConstants.arrComponentsCreated = new string[1];
            PanelConstants.arrComponentsCreated[0] = string.Concat(lblPanelNumber.Text, PanelConstants.strComma, txtPanelName.Text, PanelConstants.strComma, lstSheetSizes.SelectedItem, PanelConstants.strComma, txtSize.Text, PanelConstants.strComma, txtWidth.Text, PanelConstants.strComma, txtHeight.Text);
            PanelUtilities.ComponentsCreatedTextFileCreation();
            MessageBox.Show("Panel Component Process Completed", PanelConstants.PD_Title,MessageBoxButtons.OK,MessageBoxIcon.Information);
            this.Close();
        }

        public void btnWires_Click(object sender, EventArgs e)
        {
            string fileName = $"{PanelConstants.textPanelPartNumber}-{PanelConstants.panelDetailsTextFileName}";
            PanelConstants.PanelDetails_FilePath = Path.Combine(PanelConstants.Electre_Temp_Folder_Path, fileName);

            if (string.IsNullOrEmpty(PanelConstants.textPanelPartName))
            {
                MessageBox.Show("Please select the Sheet Name from list", PanelConstants.PD_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(lblPanelNumber.Text) || string.IsNullOrEmpty(txtPanelName.Text) || string.IsNullOrEmpty(txtSize.Text) || string.IsNullOrEmpty(txtWidth.Text) || string.IsNullOrEmpty(txtHeight.Text))
            {
                MessageBox.Show(PanelConstants.Panel_Main_Fields_Not_Empty, PanelConstants.PD_Title);
                return;
            }
            lstSheetSizes.Enabled = false;
            PanelUtilities.InitiateOutPutFile(PanelConstants.el_ExecFilePath);//Create el_exec file
            DataReader.LoadMyDataFile(PanelConstants.My_Data_File_Path);
            DataReader.ReadPanelDetails();
            WiringDataMappingService.AssignOrientation();
            WiringDataMappingService.CoordinatesToPDPin();
            WireRouter.DrawWireLine();
            PanelUtilities.UpdatePanelDetailsTextFile(PanelConstants.panelDetailsList); // Update PanelDetails file
            // modMain.ExitOutputFile(Constants.el_ExecFilePath);//Generate OutputFile and Exit
            PanelUtilities.CreateTemp_El_Exec_File(PanelConstants.El_Exec_Temp_P2);
            PanelUtilities.CloseVbsFiles(PanelConstants.Custom_Programs_File);
            MessageBox.Show("Wire Routing Of Selected Panel Components Completed..");
            //File.Delete(string.Concat(Constants.Electre_Temp_Folder_Path, Constants.ComponentsCreatedTextFileName));
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

            List<BorderInfo> selectedItem = PanelConstants.borderInfoList.Where(b => b.Description == selectedSize).ToList();

            if (selectedItem.Count > 0)
            {
                BorderInfo border = selectedItem[0];
                PanelConstants.SheetTemplateName = border.Template;
                txtSize.Text = border.Size;
                txtWidth.Text = border.Width;
                txtHeight.Text = border.Height;
                PanelConstants.SheetHeight = Convert.ToInt16(txtHeight.Text);
                PanelConstants.SheetWidth = Convert.ToInt16(txtWidth.Text);
                PanelConstants.SheetSize = txtSize.Text;
            }
        }
    }
}

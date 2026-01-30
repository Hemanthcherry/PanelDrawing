using PanelDrawing.CommonOperations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Panel_Drawing.Forms
{
    public partial class frmPanelOri : Form
    {
        public string SelectedSide { get; private set; } = "LEFT";

        public frmPanelOri()
        {
            InitializeComponent();
        }

        public void frmPanelOri_Load(object sender, EventArgs e)
        {
            AddPinsInfo();
            listOri.Items.Clear();
            listOri.Items.Add("Left");
            listOri.Items.Add("Right");
            //listOri.Items.Add("Top");
            //listOri.Items.Add("Bottom");
            grdOriInfo.Columns.Add("col1", "Orientation");
            grdOriInfo.Rows.Add("Left");
            grdOriInfo.Rows.Add("Right");
            //grdOriInfo.Rows.Add("Top");
            //grdOriInfo.Rows.Add("Bottom");
            grdOriInfo.Columns.Add("clo2", "Pins");
            //grdOriInfo.Rows.Add("Value for column#1"); // [,"column 2",...]

        }

        private void AddPinsInfo()
        {
            if (Constants.listPinsOfEqu == null)
            {
                MessageBox.Show("Collection not available for List Display");
                return;
            }
            listPins.Items.Clear();
            var pinsCollection = from i in Constants.listPinsOfEqu
                                 where (!string.IsNullOrEmpty(i))
                                 select i;
            if (Constants.listPinsOfEqu.Count(x => !string.IsNullOrEmpty(x)) > 0)
            {
                foreach (string item in pinsCollection)
                {
                    listPins.Items.Add(item);
                    //listPins.SetSelected(Convert.ToInt16(item), true);
                }
            }
            if (listPins.Items.Count > 0)
            {
                for (int i = 0; i < listPins.Items.Count; i++)
                {
                    listPins.SetSelected(i, true);
                }
            }

        }

        private void Export_PanelEquOri(StreamWriter writer, string istr, string iOri)
        {
            if (!string.IsNullOrEmpty(istr))
            {
                string[] arr7 = istr.Split(',');
                for (int ii = 1; ii <= arr7.Length - 1; ii++) // Start from 1 like in VB6
                {
                    //if(!string.IsNullOrEmpty(arr7[ii])) writer.WriteLine($"{Constants.txtEquName};{!string.IsNullOrEmpty(arr7[ii])};{iOri}");
                    writer.WriteLine($"{Constants.txtEquName};{arr7[ii]};{iOri}");
                }
            }
        }

        //private DataGridView GetGrdOriInfo()
        //{
        //    return grdOriInfo;
        //}

        private void cmdOK_Click(object sender, EventArgs e)
        {
            this.Hide();
            // Assuming 'grdOriInfo' is a DataGridView-like control (e.g., DataGridView or similar)
            string cell10 = grdOriInfo[1, 0].Value?.ToString() ?? "";
            string cell11 = grdOriInfo[1, 1].Value?.ToString() ?? "";
            //string cell12 = grdOriInfo[1, 2].Value?.ToString() ?? "";
            //string cell13 = grdOriInfo[1, 3].Value?.ToString() ?? "";

            // If any cell has data and listPins is empty
            if ((cell10 != "" || cell11 != "") && listPins.Items.Count == 0)
                //if ((cell10 != "" || cell11 != "" || cell12 != "" || cell13 != "") && listPins.Items.Count == 0)
            {
                //this.Hide();  // Hides the form (equivalent to frmPanelOri.Hide)
            }
            else
            {
                DialogResult sta = MessageBox.Show("All/Other pins will be added to Left connector. Ok?",
                                                    "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (sta == DialogResult.Yes)
                {
                    //Select all items in the list
                    for (int m = 0; m < listPins.Items.Count; m++)
                    {
                        
                        listPins.SetSelected(m, true);
                    }

                    grdOriInfo[1, 1].Value = grdOriInfo[1, 1]?.ToString() + AddListSelectionToStringAndRemoveFromList(listPins).ToString();
                   // this.Hide();
                }
                else
                {
                    return;
                }
            }

            if (!string.IsNullOrEmpty(grdOriInfo[1, 0].Value?.ToString()))
                SelectedSide = "LEFT";
            else if (!string.IsNullOrEmpty(grdOriInfo[1, 1].Value?.ToString()))
                SelectedSide = "RIGHT";
            else
                SelectedSide = "LEFT";

            Constants.sPanelEQUori_File = string.Concat(Constants.Electre_Temp_Folder_Path, Constants.textPanelPartNumber, " " + "- " + Constants.txtEquName, " - PanelEquOri.txt");

            using (StreamWriter writer = new StreamWriter(Constants.sPanelEQUori_File))
            {
                Export_PanelEquOri(writer, (string)grdOriInfo[1, 0].Value?.ToString(), "L");
                Export_PanelEquOri(writer, (string)grdOriInfo[1, 1].Value?.ToString(), "R");
                //Export_PanelEquOri(writer, (string)grdOriInfo[1, 2].Value?.ToString(), "T");
                //Export_PanelEquOri(writer, (string)grdOriInfo[1, 3].Value?.ToString(), "B");
            }
            this.Hide();
        }

        private string AddListSelectionToStringAndRemoveFromList(ListBox listBox)
        {
            Constants.remainingItems = new List<string>();
            string result = "";

            for (int i = 0; i < listBox.Items.Count; i++)
            {
                if (listBox.SelectedIndices.Contains(i))
                {
                    result += "," + listBox.Items[i].ToString();
                }
                else
                {
                    Constants.remainingItems.Add(listBox.Items[i].ToString());
                }
            }

            listBox.Items.Clear();
            AddArrayToListBox(Constants.remainingItems.ToArray(), listBox);

            return result;
        }

        private void AddArrayToListBox(string[] iarr, ListBox listBox)
        {
            var df = (from i1 in Constants.listPinsOfEqu
                      where (!string.IsNullOrEmpty(i1))
                      select i1).ToArray();
            listBox.Items.Clear();
            if (df == null || df.Length <= 1)
                return;

            for (int i = 0; i <= df.Length - 1; i++)
            {
                listBox.Items.Add(df[i]);
            }
        }

        private void cmdUpdate_Click(object sender, EventArgs e)
        {
            // If there is no item in list, no point in updating
            if (listPins.Items.Count == 0)
                return;

            if (listPins.SelectedItems.Count == 0 || listOri.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select EQU and Ori");
                return;
            }

            string orientation = listOri.Text; // Gets the selected item text

            switch (orientation)
            {
                case "Left":
                    grdOriInfo[1, 0].Value = grdOriInfo[1, 0].Value?.ToString() + AddListSelectionToStringAndRemoveFromList(listPins);
                    break;
                case "Right":
                    grdOriInfo[1, 1].Value = grdOriInfo[1, 1].Value?.ToString() + AddListSelectionToStringAndRemoveFromList(listPins);//grdOriInfo[2, 1].Value?.ToString() + AddListSelectionToStringAndRemoveFromList(listPins);
                    break;
                //case "Top":
                //    grdOriInfo[1, 2].Value = grdOriInfo[1, 2].Value?.ToString() + AddListSelectionToStringAndRemoveFromList(listPins);
                //    break;
                //case "Bottom":
                //    grdOriInfo[1, 3].Value = grdOriInfo[1, 3].Value?.ToString() + AddListSelectionToStringAndRemoveFromList(listPins);//grdOriInfo[4, 1].Value?.ToString() + AddListSelectionToStringAndRemoveFromList(listPins);
                //    break;

            }
            listPins.Items.Clear();
        }

        private void cmdReset_Click(object sender, EventArgs e)
        {
            AddPinsInfo();
            //var df = from i1 in Constants.arrPinsOfEqu
            //         where(!string.IsNullOrEmpty(i1))
            //         select i1;
            //AddArrayToListBox(Constants.arrPinsOfEqu, listPins);
            grdOriInfo[1, 0].Value = "";
            grdOriInfo[1, 1].Value = "";
            //grdOriInfo[1, 2].Value = "";
            //grdOriInfo[1, 3].Value = "";
            //grdOriInfo.Rows.Clear();
            //grdOriInfo.Columns[0].Name = "Ori";

            //grdOriInfo.Columns.Add("col1", "Orienetation");
            //grdOriInfo.Rows.Add("Left");
            //grdOriInfo.Rows.Add("Right");
            //grdOriInfo.Rows.Add("Top");
            //grdOriInfo.Rows.Add("Bottom");
            //grdOriInfo.Columns[1].Name = "Pins";
        }

    }
}

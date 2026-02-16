using Microsoft.VisualBasic;
using PanelDrawing.Core.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using PanelDrawing.CommonOperations;
namespace PanelDrawing.Forms
{
    public partial class PanelInputForm : Form
    {
        public PanelInputForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtInputs.Text))
            {
                MessageBox.Show(" Field Should not be empty ","Panel Drawing",MessageBoxButtons.OK,MessageBoxIcon.Information);
                return;
            }
            PanelConstants.PD_Symb_Inputs = txtInputs.Text;
            this.Close();
        }
    }
}

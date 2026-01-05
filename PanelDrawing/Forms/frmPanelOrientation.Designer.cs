namespace Panel_Drawing.Forms
{
    partial class frmPanelOri
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        public void InitializeComponent()
        {
            groupBox1 = new GroupBox();
            label2 = new Label();
            label1 = new Label();
            lblEquName = new Label();
            cmdReset = new Button();
            cmdUpdate = new Button();
            listOri = new ListBox();
            grdOriInfo = new DataGridView();
            listPins = new ListBox();
            cmdOK = new Button();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grdOriInfo).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(lblEquName);
            groupBox1.Controls.Add(cmdReset);
            groupBox1.Controls.Add(cmdUpdate);
            groupBox1.Controls.Add(listOri);
            groupBox1.Controls.Add(grdOriInfo);
            groupBox1.Controls.Add(listPins);
            groupBox1.Location = new Point(30, 23);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(1029, 364);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(223, 192);
            label2.Name = "label2";
            label2.Size = new Size(157, 25);
            label2.TabIndex = 7;
            label2.Text = "List Of Orientation";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(19, 21);
            label1.Name = "label1";
            label1.Size = new Size(100, 25);
            label1.TabIndex = 6;
            label1.Text = "List Of Pins";
            // 
            // lblEquName
            // 
            lblEquName.AutoSize = true;
            lblEquName.Location = new Point(641, 228);
            lblEquName.Name = "lblEquName";
            lblEquName.Size = new Size(59, 25);
            lblEquName.TabIndex = 5;
            lblEquName.Text = "label1";
            // 
            // cmdReset
            // 
            cmdReset.Location = new Point(467, 315);
            cmdReset.Name = "cmdReset";
            cmdReset.Size = new Size(121, 34);
            cmdReset.TabIndex = 4;
            cmdReset.Text = "Reset";
            cmdReset.UseVisualStyleBackColor = true;
            cmdReset.Click += cmdReset_Click;
            // 
            // cmdUpdate
            // 
            cmdUpdate.Location = new Point(467, 224);
            cmdUpdate.Name = "cmdUpdate";
            cmdUpdate.Size = new Size(121, 40);
            cmdUpdate.TabIndex = 3;
            cmdUpdate.Text = "Update";
            cmdUpdate.UseVisualStyleBackColor = true;
            cmdUpdate.Click += cmdUpdate_Click;
            // 
            // listOri
            // 
            listOri.FormattingEnabled = true;
            listOri.ItemHeight = 25;
            listOri.Items.AddRange(new object[] { "Left", "Right" });
            listOri.Location = new Point(223, 224);
            listOri.Name = "listOri";
            listOri.Size = new Size(165, 129);
            listOri.TabIndex = 2;
            // 
            // grdOriInfo
            // 
            grdOriInfo.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grdOriInfo.Location = new Point(223, 24);
            grdOriInfo.Name = "grdOriInfo";
            grdOriInfo.ReadOnly = true;
            grdOriInfo.RowHeadersWidth = 62;
            grdOriInfo.Size = new Size(784, 159);
            grdOriInfo.TabIndex = 1;
            // 
            // listPins
            // 
            listPins.Enabled = false;
            listPins.FormattingEnabled = true;
            listPins.ItemHeight = 25;
            listPins.Location = new Point(21, 52);
            listPins.Name = "listPins";
            listPins.SelectionMode = SelectionMode.MultiSimple;
            listPins.Size = new Size(175, 304);
            listPins.TabIndex = 0;
            // 
            // cmdOK
            // 
            cmdOK.Location = new Point(461, 415);
            cmdOK.Name = "cmdOK";
            cmdOK.Size = new Size(186, 52);
            cmdOK.TabIndex = 1;
            cmdOK.Text = "Ok";
            cmdOK.UseVisualStyleBackColor = true;
            cmdOK.Click += cmdOK_Click;
            // 
            // frmPanelOri
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1104, 488);
            Controls.Add(cmdOK);
            Controls.Add(groupBox1);
            Name = "frmPanelOri";
            Text = "Orientation";
            Load += frmPanelOri_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)grdOriInfo).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        public ListBox listOri;
        public DataGridView grdOriInfo;
        public ListBox listPins;
        public Label lblEquName;
        private Button cmdReset;
        private Button cmdUpdate;
        private Button cmdOK;
        private Label label2;
        private Label label1;
    }
}
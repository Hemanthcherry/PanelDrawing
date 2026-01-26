namespace Panel_Drawing.Forms
{
    partial class frmMain
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
        private void InitializeComponent()
        {
            groupBox1 = new GroupBox();
            txtHeight = new TextBox();
            txtWidth = new TextBox();
            txtSize = new TextBox();
            lblHeight = new Label();
            lblWidth = new Label();
            lblSize = new Label();
            lstSheetSizes = new ListBox();
            txtPanelName = new TextBox();
            lblPanelNumber = new Label();
            groupBox2 = new GroupBox();
            btnWires = new Button();
            btnPanelComponents = new Button();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(txtHeight);
            groupBox1.Controls.Add(txtWidth);
            groupBox1.Controls.Add(txtSize);
            groupBox1.Controls.Add(lblHeight);
            groupBox1.Controls.Add(lblWidth);
            groupBox1.Controls.Add(lblSize);
            groupBox1.Controls.Add(lstSheetSizes);
            groupBox1.Controls.Add(txtPanelName);
            groupBox1.Controls.Add(lblPanelNumber);
            groupBox1.Location = new Point(26, 24);
            groupBox1.Margin = new Padding(2, 4, 2, 4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(2, 4, 2, 4);
            groupBox1.Size = new Size(519, 671);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Panel Block";
            // 
            // txtHeight
            // 
            txtHeight.BackColor = Color.WhiteSmoke;
            txtHeight.BorderStyle = BorderStyle.FixedSingle;
            txtHeight.Location = new Point(111, 619);
            txtHeight.Margin = new Padding(2, 4, 2, 4);
            txtHeight.Name = "txtHeight";
            txtHeight.ReadOnly = true;
            txtHeight.Size = new Size(380, 31);
            txtHeight.TabIndex = 8;
            // 
            // txtWidth
            // 
            txtWidth.BackColor = Color.WhiteSmoke;
            txtWidth.BorderStyle = BorderStyle.FixedSingle;
            txtWidth.Location = new Point(112, 575);
            txtWidth.Margin = new Padding(2, 4, 2, 4);
            txtWidth.Name = "txtWidth";
            txtWidth.ReadOnly = true;
            txtWidth.Size = new Size(380, 31);
            txtWidth.TabIndex = 7;
            // 
            // txtSize
            // 
            txtSize.BackColor = Color.WhiteSmoke;
            txtSize.BorderStyle = BorderStyle.FixedSingle;
            txtSize.Location = new Point(112, 531);
            txtSize.Margin = new Padding(2, 4, 2, 4);
            txtSize.Name = "txtSize";
            txtSize.ReadOnly = true;
            txtSize.Size = new Size(380, 31);
            txtSize.TabIndex = 6;
            // 
            // lblHeight
            // 
            lblHeight.AutoSize = true;
            lblHeight.Location = new Point(22, 622);
            lblHeight.Margin = new Padding(2, 0, 2, 0);
            lblHeight.Name = "lblHeight";
            lblHeight.Size = new Size(65, 25);
            lblHeight.TabIndex = 5;
            lblHeight.Text = "Height";
            // 
            // lblWidth
            // 
            lblWidth.AutoSize = true;
            lblWidth.Location = new Point(22, 579);
            lblWidth.Margin = new Padding(2, 0, 2, 0);
            lblWidth.Name = "lblWidth";
            lblWidth.Size = new Size(60, 25);
            lblWidth.TabIndex = 4;
            lblWidth.Text = "Width";
            // 
            // lblSize
            // 
            lblSize.AutoSize = true;
            lblSize.Location = new Point(22, 531);
            lblSize.Margin = new Padding(2, 0, 2, 0);
            lblSize.Name = "lblSize";
            lblSize.Size = new Size(43, 25);
            lblSize.TabIndex = 3;
            lblSize.Text = "Size";
            // 
            // lstSheetSizes
            // 
            lstSheetSizes.FormattingEnabled = true;
            lstSheetSizes.ItemHeight = 25;
            lstSheetSizes.Location = new Point(28, 109);
            lstSheetSizes.Margin = new Padding(2, 4, 2, 4);
            lstSheetSizes.Name = "lstSheetSizes";
            lstSheetSizes.ScrollAlwaysVisible = true;
            lstSheetSizes.Size = new Size(464, 404);
            lstSheetSizes.TabIndex = 2;
            lstSheetSizes.SelectedIndexChanged += lstSheetSizes_SelectedIndexChanged;
            // 
            // txtPanelName
            // 
            txtPanelName.BackColor = Color.WhiteSmoke;
            txtPanelName.BorderStyle = BorderStyle.FixedSingle;
            txtPanelName.Enabled = false;
            txtPanelName.Location = new Point(28, 75);
            txtPanelName.Margin = new Padding(2, 4, 2, 4);
            txtPanelName.MaximumSize = new Size(464, 26);
            txtPanelName.MinimumSize = new Size(464, 26);
            txtPanelName.Name = "txtPanelName";
            txtPanelName.ReadOnly = true;
            txtPanelName.Size = new Size(464, 26);
            txtPanelName.TabIndex = 1;
            // 
            // lblPanelNumber
            // 
            lblPanelNumber.AutoSize = true;
            lblPanelNumber.BorderStyle = BorderStyle.FixedSingle;
            lblPanelNumber.Location = new Point(28, 41);
            lblPanelNumber.Margin = new Padding(2, 0, 2, 0);
            lblPanelNumber.MinimumSize = new Size(464, 27);
            lblPanelNumber.Name = "lblPanelNumber";
            lblPanelNumber.Size = new Size(464, 27);
            lblPanelNumber.TabIndex = 0;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(btnWires);
            groupBox2.Controls.Add(btnPanelComponents);
            groupBox2.Location = new Point(26, 702);
            groupBox2.Margin = new Padding(2, 4, 2, 4);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(2, 4, 2, 4);
            groupBox2.Size = new Size(519, 142);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            // 
            // btnWires
            // 
            btnWires.Location = new Point(48, 79);
            btnWires.Margin = new Padding(2, 4, 2, 4);
            btnWires.Name = "btnWires";
            btnWires.Size = new Size(412, 45);
            btnWires.TabIndex = 1;
            btnWires.Text = "Create Wires";
            btnWires.UseVisualStyleBackColor = true;
            btnWires.Click += btnWires_Click;
            // 
            // btnPanelComponents
            // 
            btnPanelComponents.Location = new Point(46, 19);
            btnPanelComponents.Margin = new Padding(2, 4, 2, 4);
            btnPanelComponents.Name = "btnPanelComponents";
            btnPanelComponents.Size = new Size(412, 45);
            btnPanelComponents.TabIndex = 0;
            btnPanelComponents.Text = "Create Panel";
            btnPanelComponents.UseVisualStyleBackColor = true;
            btnPanelComponents.Click += btnPanelComponents_Click;
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(586, 871);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Margin = new Padding(2, 4, 2, 4);
            MaximumSize = new Size(608, 927);
            MinimumSize = new Size(608, 927);
            Name = "frmMain";
            Text = "Panel Main";
            Load += frmMain_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblHeight;
        private System.Windows.Forms.Label lblWidth;
        private System.Windows.Forms.Label lblSize;
        private System.Windows.Forms.ListBox lstSheetSizes;
        private System.Windows.Forms.TextBox txtPanelName;
        public System.Windows.Forms.Label lblPanelNumber;
        public System.Windows.Forms.TextBox txtHeight;
        public System.Windows.Forms.TextBox txtWidth;
        private System.Windows.Forms.TextBox txtSize;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnWires;
        private System.Windows.Forms.Button btnPanelComponents;
    }
}
namespace PanelDrawing.Forms
{
    partial class InputForm
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
            label1 = new Label();
            txtInputs = new TextBox();
            button1 = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(27, 27);
            label1.Name = "label1";
            label1.Size = new Size(159, 25);
            label1.TabIndex = 0;
            label1.Text = "Symbol Numbers :";
            // 
            // txtInputs
            // 
            txtInputs.BorderStyle = BorderStyle.None;
            txtInputs.Location = new Point(32, 77);
            txtInputs.Name = "txtInputs";
            txtInputs.Size = new Size(531, 24);
            txtInputs.TabIndex = 1;
            // 
            // button1
            // 
            button1.Location = new Point(436, 24);
            button1.Name = "button1";
            button1.Size = new Size(131, 32);
            button1.TabIndex = 2;
            button1.Text = "Ok";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // InputForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(586, 127);
            Controls.Add(button1);
            Controls.Add(txtInputs);
            Controls.Add(label1);
            Name = "InputForm";
            Text = "InputBox";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox txtInputs;
        private Button button1;
    }
}
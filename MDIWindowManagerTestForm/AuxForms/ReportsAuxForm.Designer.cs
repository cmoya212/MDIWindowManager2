namespace MDIWindowManagerTestForm.AuxForms
{
    partial class ReportsAuxForm
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
            label2 = new Label();
            label1 = new Label();
            button1 = new Button();
            SuspendLayout();
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            label2.Location = new Point(12, 9);
            label2.Name = "label2";
            label2.Size = new Size(173, 55);
            label2.TabIndex = 3;
            label2.Text = "Reports";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(25, 82);
            label1.Name = "label1";
            label1.Size = new Size(575, 31);
            label1.TabIndex = 4;
            label1.Text = "Click the button below to open a WPF window as a tab.";
            // 
            // button1
            // 
            button1.Location = new Point(25, 133);
            button1.Name = "button1";
            button1.Size = new Size(279, 44);
            button1.TabIndex = 5;
            button1.Text = "Open WPF Window";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // ReportsAuxForm
            // 
            AutoScaleDimensions = new SizeF(13F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(button1);
            Controls.Add(label1);
            Controls.Add(label2);
            Name = "ReportsAuxForm";
            Text = "ReportsAuxForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label2;
        private Label label1;
        private Button button1;
    }
}
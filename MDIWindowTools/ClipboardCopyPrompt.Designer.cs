namespace MDIWindowTools
{
    partial class ClipboardCopyPrompt
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
            ClipboardGroupBox = new GroupBox();
            ClipboardOption2 = new RadioButton();
            ClipboardOption1 = new RadioButton();
            FormOkButton = new Button();
            FormCancelButton = new Button();
            ClipboardGroupBox.SuspendLayout();
            SuspendLayout();
            // 
            // ClipboardGroupBox
            // 
            ClipboardGroupBox.Controls.Add(ClipboardOption2);
            ClipboardGroupBox.Controls.Add(ClipboardOption1);
            ClipboardGroupBox.Location = new Point(12, 12);
            ClipboardGroupBox.Name = "ClipboardGroupBox";
            ClipboardGroupBox.Size = new Size(490, 99);
            ClipboardGroupBox.TabIndex = 0;
            ClipboardGroupBox.TabStop = false;
            ClipboardGroupBox.Text = "Copy:";
            // 
            // ClipboardOption2
            // 
            ClipboardOption2.AutoSize = true;
            ClipboardOption2.Location = new Point(258, 37);
            ClipboardOption2.Name = "ClipboardOption2";
            ClipboardOption2.Size = new Size(134, 35);
            ClipboardOption2.TabIndex = 1;
            ClipboardOption2.TabStop = true;
            ClipboardOption2.Text = "Entire list";
            ClipboardOption2.UseVisualStyleBackColor = true;
            // 
            // ClipboardOption1
            // 
            ClipboardOption1.AutoSize = true;
            ClipboardOption1.Location = new Point(13, 37);
            ClipboardOption1.Name = "ClipboardOption1";
            ClipboardOption1.Size = new Size(239, 35);
            ClipboardOption1.TabIndex = 0;
            ClipboardOption1.TabStop = true;
            ClipboardOption1.Text = "Only selected items";
            ClipboardOption1.UseVisualStyleBackColor = true;
            // 
            // FormOkButton
            // 
            FormOkButton.DialogResult = DialogResult.OK;
            FormOkButton.Location = new Point(208, 129);
            FormOkButton.Name = "FormOkButton";
            FormOkButton.Size = new Size(144, 44);
            FormOkButton.TabIndex = 1;
            FormOkButton.Text = "OK";
            FormOkButton.UseVisualStyleBackColor = true;
            // 
            // FormCancelButton
            // 
            FormCancelButton.DialogResult = DialogResult.Cancel;
            FormCancelButton.Location = new Point(358, 129);
            FormCancelButton.Name = "FormCancelButton";
            FormCancelButton.Size = new Size(144, 44);
            FormCancelButton.TabIndex = 2;
            FormCancelButton.Text = "Cancel";
            FormCancelButton.UseVisualStyleBackColor = true;
            // 
            // ClipboardCopyPrompt
            // 
            AcceptButton = FormOkButton;
            AutoScaleDimensions = new SizeF(13F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = FormCancelButton;
            ClientSize = new Size(514, 185);
            Controls.Add(FormCancelButton);
            Controls.Add(FormOkButton);
            Controls.Add(ClipboardGroupBox);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ClipboardCopyPrompt";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Clipboard Copy";
            ClipboardGroupBox.ResumeLayout(false);
            ClipboardGroupBox.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox ClipboardGroupBox;
        private Button FormOkButton;
        private Button FormCancelButton;
        public RadioButton ClipboardOption2;
        public RadioButton ClipboardOption1;
    }
}
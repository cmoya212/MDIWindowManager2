namespace MDIWindowManagerTestForm
{
    partial class TextEditorForm
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
            TextBox1 = new TextBox();
            SuspendLayout();
            // 
            // TextBox1
            // 
            TextBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TextBox1.BorderStyle = BorderStyle.None;
            TextBox1.Location = new Point(12, 12);
            TextBox1.Multiline = true;
            TextBox1.Name = "TextBox1";
            TextBox1.ScrollBars = ScrollBars.Both;
            TextBox1.Size = new Size(855, 552);
            TextBox1.TabIndex = 0;
            // 
            // TextEditorForm
            // 
            AutoScaleDimensions = new SizeF(13F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Window;
            ClientSize = new Size(879, 576);
            Controls.Add(TextBox1);
            ForeColor = SystemColors.WindowText;
            Name = "TextEditorForm";
            Text = "TextEditorForm";
            Load += TextEditorForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox TextBox1;
    }
}
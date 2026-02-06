namespace MDIWindowTools
{
    partial class FindForm
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
            FindWhatLabel = new Label();
            SearchBackwardsCheckBox = new CheckBox();
            MatchWholeWordCheckBox = new CheckBox();
            MatchCaseCheckBox = new CheckBox();
            OkButton = new Button();
            CloseButton = new Button();
            FindTextComboBox = new ComboBox();
            SuspendLayout();
            // 
            // FindWhatLabel
            // 
            FindWhatLabel.AutoSize = true;
            FindWhatLabel.Location = new Point(12, 9);
            FindWhatLabel.Name = "FindWhatLabel";
            FindWhatLabel.Size = new Size(119, 31);
            FindWhatLabel.TabIndex = 0;
            FindWhatLabel.Text = "Find what:";
            // 
            // SearchBackwardsCheckBox
            // 
            SearchBackwardsCheckBox.AutoSize = true;
            SearchBackwardsCheckBox.Location = new Point(12, 97);
            SearchBackwardsCheckBox.Name = "SearchBackwardsCheckBox";
            SearchBackwardsCheckBox.Size = new Size(223, 35);
            SearchBackwardsCheckBox.TabIndex = 2;
            SearchBackwardsCheckBox.Text = "Search backwards";
            SearchBackwardsCheckBox.UseVisualStyleBackColor = true;
            // 
            // MatchWholeWordCheckBox
            // 
            MatchWholeWordCheckBox.AutoSize = true;
            MatchWholeWordCheckBox.Location = new Point(12, 138);
            MatchWholeWordCheckBox.Name = "MatchWholeWordCheckBox";
            MatchWholeWordCheckBox.Size = new Size(230, 35);
            MatchWholeWordCheckBox.TabIndex = 3;
            MatchWholeWordCheckBox.Text = "Match whole word";
            MatchWholeWordCheckBox.UseVisualStyleBackColor = true;
            // 
            // MatchCaseCheckBox
            // 
            MatchCaseCheckBox.AutoSize = true;
            MatchCaseCheckBox.Location = new Point(12, 179);
            MatchCaseCheckBox.Name = "MatchCaseCheckBox";
            MatchCaseCheckBox.Size = new Size(156, 35);
            MatchCaseCheckBox.TabIndex = 4;
            MatchCaseCheckBox.Text = "Match case";
            MatchCaseCheckBox.UseVisualStyleBackColor = true;
            // 
            // OkButton
            // 
            OkButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            OkButton.Location = new Point(266, 216);
            OkButton.Name = "OkButton";
            OkButton.Size = new Size(144, 44);
            OkButton.TabIndex = 5;
            OkButton.Text = "OK";
            OkButton.UseVisualStyleBackColor = true;
            OkButton.Click += OkButton_Click;
            // 
            // CloseButton
            // 
            CloseButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            CloseButton.Location = new Point(416, 216);
            CloseButton.Name = "CloseButton";
            CloseButton.Size = new Size(144, 44);
            CloseButton.TabIndex = 6;
            CloseButton.Text = "Cancel";
            CloseButton.UseVisualStyleBackColor = true;
            CloseButton.Click += CancelButton_Click;
            // 
            // FindTextComboBox
            // 
            FindTextComboBox.FormattingEnabled = true;
            FindTextComboBox.Location = new Point(12, 43);
            FindTextComboBox.Name = "FindTextComboBox";
            FindTextComboBox.Size = new Size(548, 39);
            FindTextComboBox.TabIndex = 1;
            // 
            // FindForm
            // 
            AcceptButton = OkButton;
            AutoScaleDimensions = new SizeF(13F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(572, 272);
            Controls.Add(FindTextComboBox);
            Controls.Add(CloseButton);
            Controls.Add(OkButton);
            Controls.Add(MatchCaseCheckBox);
            Controls.Add(MatchWholeWordCheckBox);
            Controls.Add(SearchBackwardsCheckBox);
            Controls.Add(FindWhatLabel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FindForm";
            StartPosition = FormStartPosition.Manual;
            Text = "Find";
            FormClosing += FindForm_FormClosing;
            VisibleChanged += FindForm_VisibleChanged;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label FindWhatLabel;
        private Button OkButton;
        private Button CloseButton;
        internal CheckBox SearchBackwardsCheckBox;
        internal CheckBox MatchWholeWordCheckBox;
        internal CheckBox MatchCaseCheckBox;
        internal ComboBox FindTextComboBox;
    }
}
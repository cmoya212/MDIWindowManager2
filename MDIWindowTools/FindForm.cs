using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDIWindowTools
{
    public partial class FindForm : Form
    {
        //this is necessary because hiding the form affects the built-in DialogResult property
        public DialogResult FindDialogResult = DialogResult.None;

        public FindForm()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            this.FindDialogResult = DialogResult.OK;
            this.Visible = false;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.FindDialogResult = DialogResult.Cancel;
            this.Visible = false;
        }

        private void FindForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
                this.FindDialogResult = DialogResult.Cancel;

            this.FindTextComboBox.Focus();
            e.Cancel = true;
            this.Visible = false;
        }

        private void FindForm_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                this.FindTextComboBox.SelectAll();
                this.FindTextComboBox.Focus();
            }
        }
    }
}

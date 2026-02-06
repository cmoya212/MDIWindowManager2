using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDIWindowManagerTestForm.AuxForms
{
    public partial class SettingsAuxForm : Form
    {
        public SettingsAuxForm()
        {
            InitializeComponent();
        }

        private void SettingsAuxForm_Load(object sender, EventArgs e)
        {
            this.checkBox1.Checked = !Program.UseMdiWindowManager;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Program.UseMdiWindowManager = !this.checkBox1.Checked;

            MessageBox.Show("You will have to close this window and re-open it.", "Tabs/MDI", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

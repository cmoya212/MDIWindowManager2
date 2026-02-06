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
    public partial class ReportsAuxForm : Form
    {
        public ReportsAuxForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var form = new MDIWindowManager.WpfHostForm(new WpfExampleWindow.ExampleForm(), "My WPF Window");

            var parent = this.MdiParent as AdvancedMdiForm;

            if (parent != null)
                parent.OpenForm(form);
            else
                form.Show();
        }
    }
}

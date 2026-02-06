using MDIWindowManagerTestForm.ChildForms;
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
    public partial class PatientsAuxForm : Form
    {
        public PatientsAuxForm()
        {
            InitializeComponent();
        }

        private void PatientsAuxForm_Load(object sender, EventArgs e)
        {
            string[] names = new[] { "Doe, John", "Doe, Jane", "Smith, John", "Piper, Peter", "Schmoe, Joe" };

            foreach (string name in names)
            {
                var item = new ListViewItem();

                item.ImageIndex = 0;
                item.Text = name;

                this.listView1.Items.Add(item);
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var selectedItem = this.listView1.SelectedItems[0];

            if (selectedItem != null)
            {
                //TODO: search open windows for matching item and activate it

                var form = new PatientChildForm();

                form.SetData(selectedItem.Text);

                var parent = this.MdiParent as AdvancedMdiForm;

                if (parent != null)
                    parent.OpenForm(form);
                else
                    form.Show();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDIWindowManagerTestForm.ChildForms
{
    public partial class PatientChildForm : Form
    {
        public PatientChildForm()
        {
            InitializeComponent();
        }

        public void SetData(string name)
        {
            this.Text = name;
            this.textBox1.Text = name;
        }
    }
}

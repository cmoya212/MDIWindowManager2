using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace MDIWindowManagerTestForm
{
    public partial class TextEditorForm : Form
    {
        public TextEditorForm()
        {
            InitializeComponent();
        }

        private void TextEditorForm_Load(object sender, EventArgs e)
        {

        }

        public void SetDocumentText(string text)
        {
            this.TextBox1.Text = text;
            this.TextBox1.SelectionStart = 0;
        }
    }
}

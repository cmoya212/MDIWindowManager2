using MDIWindowManagerTestForm.ChildForms;
using MDIWindowManagerTestForm.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;

namespace MDIWindowManagerTestForm
{
    public partial class SimpleMdiForm : Form
    {
        [DllImport("user32.dll", EntryPoint = "keybd_event")]
        private static extern void SendKbEvent(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);


        public SimpleMdiForm()
        {
            InitializeComponent();
        }

        private void SimpleMdiForm_Load(object sender, EventArgs e)
        {
            //Note: no special code used here.
            //AutoDetect property of MDIWindowManager allows for transparent usage.

            var form = new TextEditorForm();
            form.MdiParent = this;
            form.Text = "Intro";
            form.SetDocumentText(Resources.IntroTxt);
            form.Show();

            for (int count = 0; count < 10; count++)
            {
                form = new TextEditorForm();
                form.MdiParent = this;
                form.Text = $"Document {count + 1}";
                form.SetDocumentText(RandomLoremWords.Generate(20, 40, 3, 7));
                form.Show();
            }

            this.MdiChildren[0].Activate();
        }

        private void FileNewMenuItem_Click(object sender, EventArgs e)
        {
            //Again, no special code.
            //AutoDetect property of MDIWindowManager allows for transparent usage.

            var form = new TextEditorForm();
            form.MdiParent = this;
            form.Text = "New Document";
            form.SetDocumentText(RandomLoremWords.Generate(20, 40, 3, 7));
            form.Show();
        }

        private void ViewAdvancedMenuItem_Click(object sender, EventArgs e)
        {
            var form = new AdvancedMdiForm();

            form.Show();
            form.WindowState = FormWindowState.Maximized;

            SendKbEvent((byte)Keys.LWin, 0, 1, 0);
            SendKbEvent((byte)Keys.Z, 0, 1, 0);
            SendKbEvent((byte)Keys.LWin, 0, 1 | 2, 0);
            SendKbEvent((byte)Keys.Z, 0, 1 | 2, 0);
        }
        private void FileExitMenuItem_Click(object sender, EventArgs e)
            => Close();
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.TextFormatting;
using MDIWindowManagerTestForm.AuxForms;
using MDIWindowManagerTestForm.ChildForms;

namespace MDIWindowManagerTestForm
{
    public partial class AdvancedMdiForm : Form
    {
        private bool _useWinMan = true;
        private bool _openInTaskbar = false;
        private Form[] _auxWindows = new Form[4];
        private int _activeAuxWindowIndex = 0;
        private Button[] _sidebarButtons = new Button[4];
        private ToolStripMenuItem[] _viewMenuItems = new ToolStripMenuItem[4];

        public AdvancedMdiForm()
        {
            InitializeComponent();

            _sidebarButtons[0] = this.DashboardSideButton;
            _sidebarButtons[1] = this.PatientsSideButton;
            _sidebarButtons[2] = this.ReportsSideButton;
            _sidebarButtons[3] = this.SettingsSideButton;

            _viewMenuItems[0] = this.ViewDashboardMenuItem;
            _viewMenuItems[1] = this.ViewPatientsMenuItem;
            _viewMenuItems[2] = this.ViewReportsMenuItem;
            _viewMenuItems[3] = this.ViewSettingsMenuItem;

            _auxWindows[0] = new DashboardAuxForm();
            _auxWindows[1] = new PatientsAuxForm();
            _auxWindows[2] = new ReportsAuxForm();
            _auxWindows[3] = new SettingsAuxForm();
        }

        private void AdvancedMdiForm_Load(object sender, EventArgs e)
        {
            _useWinMan = Program.UseMdiWindowManager;

            if (_useWinMan)
            {
                this.WindowsMenu.Visible = false;
                SetTabsOrientation(MDIWindowManager.WindowManagerOrientation.Bottom);
                SetActiveAuxWindow(0);
            }
            else //classic MDI
            {
                this.WinManMenuItem.Visible = false;
                this.WindowManagerPanel.AutoDetectMdiChildWindows = false;

                foreach (var form in _auxWindows)
                {
                    form.MdiParent = this;
                    form.Show();
                }
            }
        }

        private void SetActiveAuxWindow(int index)
        {
            _activeAuxWindowIndex = index;

            foreach (var button in _sidebarButtons)
            {
                button.BackColor = SystemColors.Control;
                button.ForeColor = SystemColors.ControlText;
            }

            _sidebarButtons[index].BackColor = SystemColors.GradientInactiveCaption;
            _sidebarButtons[index].ForeColor = SystemColors.InactiveCaptionText;

            if (_useWinMan)
            {
                this.WindowManagerPanel.AuxiliaryWindow = _auxWindows[index];

                //optional, minimize the tabs
                //if (!this.WindowManagerPanel.MinMode)
                //    this.WindowManagerPanel.ToggleMinMode();
            }
            else //classic MDI
                _auxWindows[index].BringToFront();
        }

        public void OpenForm(Form form)
        {
            form.MdiParent = this;
            form.Show();

            if (_openInTaskbar)
                this.WindowManagerPanel.PopOutWrappedWindow(this.WindowManagerPanel.GetWrapperForWindow(form));
        }

        private void SetTabsOrientation(MDIWindowManager.WindowManagerOrientation orientation)
        {
            var bounds = this.WindowManagerPanel.GetMDIClientAreaBounds();

            switch (orientation)
            {
                case MDIWindowManager.WindowManagerOrientation.Bottom:
                    this.WindowManagerPanel.Orientation = MDIWindowManager.WindowManagerOrientation.Bottom;
                    this.WindowManagerPanel.AllowUserVerticalRepositioning = true;
                    this.WindowManagerPanel.Top = bounds.Height - (bounds.Height / 3);
                    this.WindowManagerPanel.AutoHide = true;
                    break;
                case MDIWindowManager.WindowManagerOrientation.Right:
                    this.WindowManagerPanel.Orientation = MDIWindowManager.WindowManagerOrientation.Right;
                    this.WindowManagerPanel.AllowUserVerticalRepositioning = false;
                    this.WindowManagerPanel.Top = this.WindowManagerPanel.GetMDIClientAreaBounds().Top;
                    this.WindowManagerPanel.Width = bounds.Width - ((bounds.Width / 3) * 2);
                    this.WindowManagerPanel.AutoHide = false;
                    break;
                case MDIWindowManager.WindowManagerOrientation.Left:
                    this.WindowManagerPanel.Orientation = MDIWindowManager.WindowManagerOrientation.Left;
                    this.WindowManagerPanel.AllowUserVerticalRepositioning = false;
                    this.WindowManagerPanel.Top = this.WindowManagerPanel.GetMDIClientAreaBounds().Top;
                    this.WindowManagerPanel.Width = bounds.Width - ((bounds.Width / 3) * 2);
                    this.WindowManagerPanel.AutoHide = false;
                    break;
                default:
                    throw new ArgumentException("Invalid orientation for this application.");
            }
        }

        private void AdvancedMdiForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Minimized)
                SetTabsOrientation(this.WindowManagerPanel.Orientation);
        }

        private void DashboardSideButton_Click(object sender, EventArgs e)
            => SetActiveAuxWindow(0);

        private void PatientsSideButton_Click(object sender, EventArgs e)
            => SetActiveAuxWindow(1);

        private void ReportsSideButton_Click(object sender, EventArgs e)
            => SetActiveAuxWindow(2);

        private void SettingsSideButton_Click(object sender, EventArgs e)
            => SetActiveAuxWindow(3);

        private void FileExitMenuItem_Click(object sender, EventArgs e)
            => Close();

        private void ViewDashboardMenuItem_Click(object sender, EventArgs e)
            => SetActiveAuxWindow(0);

        private void ViewPatientsMenuItem_Click(object sender, EventArgs e)
            => SetActiveAuxWindow(1);

        private void ViewReportsMenuItem_Click(object sender, EventArgs e)
            => SetActiveAuxWindow(2);

        private void ViewSettingsMenuItem_Click(object sender, EventArgs e)
            => SetActiveAuxWindow(3);

        private void ViewToolBarMenuItem_Click(object sender, EventArgs e)
            => this.MainToolbar.Visible = this.ViewToolbarMenuItem.Checked;

        private void ViewStatusBarMenuItem_Click(object sender, EventArgs e)
            => this.StatusStrip.Visible = this.ViewStatusbarMenuItem.Checked;

        private void ViewTabsTaskbarMenuItem_Click(object sender, EventArgs e)
        {
            _openInTaskbar = !_openInTaskbar;

            if (_openInTaskbar)
                MessageBox.Show(this, "New documents will be opened in the Windows taskbar. You can move them to the application's tabs via the menu accessible in the top-left icon of the window.", "Use Taskbar", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show(this, "New documents will be opened in tabs inside the application. You can pop them out to the Windows desktop via the \"Pop Out\" option in the Windows menu.", "Use Tabs", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void ViewMenu_DropDownOpening(object sender, EventArgs e)
        {
            foreach (var menuItem in _viewMenuItems)
                menuItem.Checked = false;

            _viewMenuItems[_activeAuxWindowIndex].Checked = true;
            this.ViewStatusbarMenuItem.Checked = this.StatusStrip.Visible;
            this.ViewToolbarMenuItem.Checked = this.MainToolbar.Visible;
        }

        private void ViewDocumentTabsMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            this.ViewTabsBottomMenuItem.Checked = false;
            this.ViewTabsRightMenuItem.Checked = false;
            this.ViewTabsLeftMenuItem.Checked = false;

            switch (this.WindowManagerPanel.Orientation)
            {
                case MDIWindowManager.WindowManagerOrientation.Bottom:
                    this.ViewTabsBottomMenuItem.Checked = true;
                    break;
                case MDIWindowManager.WindowManagerOrientation.Right:
                    this.ViewTabsRightMenuItem.Checked = true;
                    break;
                case MDIWindowManager.WindowManagerOrientation.Left:
                    this.ViewTabsLeftMenuItem.Checked = true;
                    break;
            }

            this.ViewTabsTaskbarMenuItem.Checked = _openInTaskbar;
        }

        private void ViewTabsBottomMenuItem_Click(object sender, EventArgs e)
            => SetTabsOrientation(MDIWindowManager.WindowManagerOrientation.Bottom);

        private void ViewTabsRightMenuItem_Click(object sender, EventArgs e)
            => SetTabsOrientation(MDIWindowManager.WindowManagerOrientation.Right);

        private void ViewTabsLeftMenuItem_Click(object sender, EventArgs e)
            => SetTabsOrientation(MDIWindowManager.WindowManagerOrientation.Left);

        #region ClassicMDI

        private void WindowCascadeMenuItem_Click(object sender, EventArgs e)
            => LayoutMdi(MdiLayout.Cascade);

        private void WindowTileVerticalMenuItem_Click(object sender, EventArgs e)
            => LayoutMdi(MdiLayout.TileVertical);

        private void WindowTileHorizontalMenuItem_Click(object sender, EventArgs e)
            => LayoutMdi(MdiLayout.TileHorizontal);

        private void WindowArrangeIconsMenuItem_Click(object sender, EventArgs e)
            => LayoutMdi(MdiLayout.ArrangeIcons);

        private void WindowCloseAllMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
                childForm.Close();
        }

        #endregion
    }
}

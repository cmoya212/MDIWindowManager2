namespace MDIWindowManagerTestForm
{
    partial class SimpleMdiForm
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SimpleMdiForm));
            MainMenuStrip = new MenuStrip();
            FileMenu = new ToolStripMenuItem();
            FIleNewMenuItem = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            FileExitMenuItem = new ToolStripMenuItem();
            ViewMenu = new ToolStripMenuItem();
            ViewAdvancedMenuItem = new ToolStripMenuItem();
            WindowManagerMenu = new ToolStripMenuItem();
            HelpMenu = new ToolStripMenuItem();
            HelpAboutMenuItem = new ToolStripMenuItem();
            StatusStrip = new StatusStrip();
            toolStripStatusLabel = new ToolStripStatusLabel();
            toolTip = new ToolTip(components);
            WindowManagerPanel1 = new MDIWindowManager.WindowManagerPanel();
            MainMenuStrip.SuspendLayout();
            StatusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // MainMenuStrip
            // 
            MainMenuStrip.ImageScalingSize = new Size(31, 31);
            MainMenuStrip.Items.AddRange(new ToolStripItem[] { FileMenu, ViewMenu, WindowManagerMenu, HelpMenu });
            MainMenuStrip.Location = new Point(0, 0);
            MainMenuStrip.Name = "MainMenuStrip";
            MainMenuStrip.Padding = new Padding(13, 5, 0, 5);
            MainMenuStrip.Size = new Size(1369, 45);
            MainMenuStrip.TabIndex = 0;
            MainMenuStrip.Text = "MenuStrip";
            // 
            // FileMenu
            // 
            FileMenu.DropDownItems.AddRange(new ToolStripItem[] { FIleNewMenuItem, toolStripSeparator3, FileExitMenuItem });
            FileMenu.ImageTransparentColor = SystemColors.ActiveBorder;
            FileMenu.Name = "FileMenu";
            FileMenu.Size = new Size(69, 35);
            FileMenu.Text = "&File";
            // 
            // FIleNewMenuItem
            // 
            FIleNewMenuItem.Image = (Image)resources.GetObject("FIleNewMenuItem.Image");
            FIleNewMenuItem.ImageTransparentColor = Color.Black;
            FIleNewMenuItem.Name = "FIleNewMenuItem";
            FIleNewMenuItem.ShortcutKeys = Keys.Control | Keys.N;
            FIleNewMenuItem.Size = new Size(270, 44);
            FIleNewMenuItem.Text = "&New";
            FIleNewMenuItem.Click += FileNewMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(267, 6);
            // 
            // FileExitMenuItem
            // 
            FileExitMenuItem.Name = "FileExitMenuItem";
            FileExitMenuItem.Size = new Size(270, 44);
            FileExitMenuItem.Text = "E&xit";
            FileExitMenuItem.Click += FileExitMenuItem_Click;
            // 
            // ViewMenu
            // 
            ViewMenu.DropDownItems.AddRange(new ToolStripItem[] { ViewAdvancedMenuItem });
            ViewMenu.Name = "ViewMenu";
            ViewMenu.Size = new Size(83, 35);
            ViewMenu.Text = "&View";
            // 
            // ViewAdvancedMenuItem
            // 
            ViewAdvancedMenuItem.Name = "ViewAdvancedMenuItem";
            ViewAdvancedMenuItem.Size = new Size(400, 44);
            ViewAdvancedMenuItem.Text = "Advanced MDI Example...";
            ViewAdvancedMenuItem.Click += ViewAdvancedMenuItem_Click;
            // 
            // WindowManagerMenu
            // 
            WindowManagerMenu.Name = "WindowManagerMenu";
            WindowManagerMenu.Size = new Size(118, 35);
            WindowManagerMenu.Text = "&Window";
            // 
            // HelpMenu
            // 
            HelpMenu.DropDownItems.AddRange(new ToolStripItem[] { HelpAboutMenuItem });
            HelpMenu.Name = "HelpMenu";
            HelpMenu.Size = new Size(82, 35);
            HelpMenu.Text = "&Help";
            // 
            // HelpAboutMenuItem
            // 
            HelpAboutMenuItem.Name = "HelpAboutMenuItem";
            HelpAboutMenuItem.Size = new Size(219, 44);
            HelpAboutMenuItem.Text = "&About...";
            // 
            // StatusStrip
            // 
            StatusStrip.BackColor = SystemColors.Control;
            StatusStrip.ImageScalingSize = new Size(31, 31);
            StatusStrip.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel });
            StatusStrip.Location = new Point(0, 1039);
            StatusStrip.Name = "StatusStrip";
            StatusStrip.Padding = new Padding(2, 0, 30, 0);
            StatusStrip.Size = new Size(1369, 41);
            StatusStrip.TabIndex = 2;
            StatusStrip.Text = "StatusStrip";
            // 
            // toolStripStatusLabel
            // 
            toolStripStatusLabel.Name = "toolStripStatusLabel";
            toolStripStatusLabel.Size = new Size(76, 31);
            toolStripStatusLabel.Text = "Status";
            // 
            // WindowManagerPanel1
            // 
            WindowManagerPanel1.AllowUserVerticalRepositioning = false;
            WindowManagerPanel1.AutoDetectMdiChildWindows = true;
            WindowManagerPanel1.AutoHide = false;
            WindowManagerPanel1.ButtonRenderMode = MDIWindowManager.ButtonRenderMode.Standard;
            WindowManagerPanel1.DisableCloseAction = false;
            WindowManagerPanel1.DisableHTileAction = false;
            WindowManagerPanel1.DisablePopoutAction = false;
            WindowManagerPanel1.DisableTileAction = false;
            WindowManagerPanel1.EmphasizeSelectedTab = true;
            WindowManagerPanel1.EnableAuroGlassEffect = false;
            WindowManagerPanel1.EnableTabPaintEvent = false;
            WindowManagerPanel1.Location = new Point(2, 47);
            WindowManagerPanel1.Margin = new Padding(0);
            WindowManagerPanel1.MdiWindowListItem = WindowManagerMenu;
            WindowManagerPanel1.MinMode = false;
            WindowManagerPanel1.Name = "WindowManagerPanel1";
            WindowManagerPanel1.Orientation = MDIWindowManager.WindowManagerOrientation.Top;
            WindowManagerPanel1.SelectedTabBackColor = SystemColors.Window;
            WindowManagerPanel1.SelectedTabForeColor = SystemColors.WindowText;
            WindowManagerPanel1.ShowIcons = true;
            WindowManagerPanel1.Size = new Size(1365, 52);
            WindowManagerPanel1.Style = MDIWindowManager.TabStyle.AngledTabs;
            WindowManagerPanel1.TabBackColor = SystemColors.Control;
            WindowManagerPanel1.TabForeColor = SystemColors.ControlText;
            WindowManagerPanel1.TabIndex = 4;
            WindowManagerPanel1.TabRenderMode = MDIWindowManager.TabsProvider.Standard;
            WindowManagerPanel1.WordWrap = false;
            // 
            // SimpleMdiForm
            // 
            AutoScaleDimensions = new SizeF(13F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1369, 1080);
            Controls.Add(WindowManagerPanel1);
            Controls.Add(StatusStrip);
            Controls.Add(MainMenuStrip);
            IsMdiContainer = true;
            Margin = new Padding(6, 7, 6, 7);
            Name = "SimpleMdiForm";
            Text = "Simple MDIWindowManagerTest";
            Load += SimpleMdiForm_Load;
            MainMenuStrip.ResumeLayout(false);
            MainMenuStrip.PerformLayout();
            StatusStrip.ResumeLayout(false);
            StatusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }
        #endregion


        private System.Windows.Forms.MenuStrip MainMenuStrip;
        private System.Windows.Forms.StatusStrip StatusStrip;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.ToolStripMenuItem HelpAboutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FileMenu;
        private System.Windows.Forms.ToolStripMenuItem FIleNewMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FileExitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ViewMenu;
        private System.Windows.Forms.ToolStripMenuItem HelpMenu;
        private System.Windows.Forms.ToolTip toolTip;
        private MDIWindowManager.WindowManagerPanel WindowManagerPanel1;
        private ToolStripMenuItem WindowManagerMenu;
        private ToolStripMenuItem ViewAdvancedMenuItem;
    }
}




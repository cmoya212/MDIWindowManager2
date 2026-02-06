namespace MDIWindowManagerTestForm
{
    partial class AdvancedMdiForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdvancedMdiForm));
            MainMenu = new MenuStrip();
            FileMenu = new ToolStripMenuItem();
            FileNewMenuItem = new ToolStripMenuItem();
            FileOpenMenuItem = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            FileSaveMenuItem = new ToolStripMenuItem();
            FileSaveAsMenuItem = new ToolStripMenuItem();
            toolStripSeparator4 = new ToolStripSeparator();
            FilePrintMenuItem = new ToolStripMenuItem();
            FilePrintPreviewMenuItem = new ToolStripMenuItem();
            FilePrintSetupMenuItem = new ToolStripMenuItem();
            toolStripSeparator5 = new ToolStripSeparator();
            FileExitMenuItem = new ToolStripMenuItem();
            EditMenu = new ToolStripMenuItem();
            EditUndoMenuItem = new ToolStripMenuItem();
            EditRedoMenuItem = new ToolStripMenuItem();
            toolStripSeparator6 = new ToolStripSeparator();
            EditCutMenuItem = new ToolStripMenuItem();
            EditCopyMenuItem = new ToolStripMenuItem();
            EditPasteMenuItem = new ToolStripMenuItem();
            toolStripSeparator7 = new ToolStripSeparator();
            EditSelectAllMenuItem = new ToolStripMenuItem();
            ViewMenu = new ToolStripMenuItem();
            ViewDashboardMenuItem = new ToolStripMenuItem();
            ViewPatientsMenuItem = new ToolStripMenuItem();
            ViewReportsMenuItem = new ToolStripMenuItem();
            ViewSettingsMenuItem = new ToolStripMenuItem();
            toolStripSeparator9 = new ToolStripSeparator();
            ViewToolbarMenuItem = new ToolStripMenuItem();
            ViewStatusbarMenuItem = new ToolStripMenuItem();
            ViewDocumentTabsMenuItem = new ToolStripMenuItem();
            ViewTabsBottomMenuItem = new ToolStripMenuItem();
            ViewTabsRightMenuItem = new ToolStripMenuItem();
            ViewTabsLeftMenuItem = new ToolStripMenuItem();
            toolStripSeparator10 = new ToolStripSeparator();
            ViewTabsTaskbarMenuItem = new ToolStripMenuItem();
            ToolsMenu = new ToolStripMenuItem();
            ToolsOptionsMenuItem = new ToolStripMenuItem();
            WindowsMenu = new ToolStripMenuItem();
            WindowCascadeMenuItem = new ToolStripMenuItem();
            WindowTileVerticalMenuItem = new ToolStripMenuItem();
            WindowTileHorizontalMenuItem = new ToolStripMenuItem();
            WindowCloseAllMenuItem = new ToolStripMenuItem();
            WindowArrangeIconsMenuItem = new ToolStripMenuItem();
            WinManMenuItem = new ToolStripMenuItem();
            HelpMenu = new ToolStripMenuItem();
            HelpIndexMenuItem = new ToolStripMenuItem();
            HelpSearchMenuItem = new ToolStripMenuItem();
            toolStripSeparator8 = new ToolStripSeparator();
            HelpAboutMenuItem = new ToolStripMenuItem();
            MainToolbar = new ToolStrip();
            newToolStripButton = new ToolStripButton();
            openToolStripButton = new ToolStripButton();
            saveToolStripButton = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            printToolStripButton = new ToolStripButton();
            printPreviewToolStripButton = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            helpToolStripButton = new ToolStripButton();
            StatusStrip = new StatusStrip();
            toolStripStatusLabel = new ToolStripStatusLabel();
            toolTip = new ToolTip(components);
            WindowManagerPanel = new MDIWindowManager.WindowManagerPanel();
            panel1 = new Panel();
            SettingsSideButton = new Button();
            ReportsSideButton = new Button();
            PatientsSideButton = new Button();
            DashboardSideButton = new Button();
            ToolStripPanel1 = new ToolStripPanel();
            MainMenu.SuspendLayout();
            MainToolbar.SuspendLayout();
            StatusStrip.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // MainMenu
            // 
            MainMenu.ImageScalingSize = new Size(31, 31);
            MainMenu.Items.AddRange(new ToolStripItem[] { FileMenu, EditMenu, ViewMenu, ToolsMenu, WindowsMenu, WinManMenuItem, HelpMenu });
            MainMenu.Location = new Point(0, 0);
            MainMenu.MdiWindowListItem = WindowsMenu;
            MainMenu.Name = "MainMenu";
            MainMenu.Padding = new Padding(7, 2, 0, 2);
            MainMenu.Size = new Size(737, 24);
            MainMenu.TabIndex = 0;
            MainMenu.Text = "MenuStrip";
            // 
            // FileMenu
            // 
            FileMenu.DropDownItems.AddRange(new ToolStripItem[] { FileNewMenuItem, FileOpenMenuItem, toolStripSeparator3, FileSaveMenuItem, FileSaveAsMenuItem, toolStripSeparator4, FilePrintMenuItem, FilePrintPreviewMenuItem, FilePrintSetupMenuItem, toolStripSeparator5, FileExitMenuItem });
            FileMenu.ImageTransparentColor = SystemColors.ActiveBorder;
            FileMenu.Name = "FileMenu";
            FileMenu.Size = new Size(37, 20);
            FileMenu.Text = "&File";
            // 
            // FileNewMenuItem
            // 
            FileNewMenuItem.Image = (Image)resources.GetObject("FileNewMenuItem.Image");
            FileNewMenuItem.ImageTransparentColor = Color.Black;
            FileNewMenuItem.Name = "FileNewMenuItem";
            FileNewMenuItem.ShortcutKeys = Keys.Control | Keys.N;
            FileNewMenuItem.Size = new Size(146, 22);
            FileNewMenuItem.Text = "&New";
            // 
            // FileOpenMenuItem
            // 
            FileOpenMenuItem.Image = (Image)resources.GetObject("FileOpenMenuItem.Image");
            FileOpenMenuItem.ImageTransparentColor = Color.Black;
            FileOpenMenuItem.Name = "FileOpenMenuItem";
            FileOpenMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            FileOpenMenuItem.Size = new Size(146, 22);
            FileOpenMenuItem.Text = "&Open";
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(143, 6);
            // 
            // FileSaveMenuItem
            // 
            FileSaveMenuItem.Image = (Image)resources.GetObject("FileSaveMenuItem.Image");
            FileSaveMenuItem.ImageTransparentColor = Color.Black;
            FileSaveMenuItem.Name = "FileSaveMenuItem";
            FileSaveMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            FileSaveMenuItem.Size = new Size(146, 22);
            FileSaveMenuItem.Text = "&Save";
            // 
            // FileSaveAsMenuItem
            // 
            FileSaveAsMenuItem.Name = "FileSaveAsMenuItem";
            FileSaveAsMenuItem.Size = new Size(146, 22);
            FileSaveAsMenuItem.Text = "Save &As";
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(143, 6);
            // 
            // FilePrintMenuItem
            // 
            FilePrintMenuItem.Image = (Image)resources.GetObject("FilePrintMenuItem.Image");
            FilePrintMenuItem.ImageTransparentColor = Color.Black;
            FilePrintMenuItem.Name = "FilePrintMenuItem";
            FilePrintMenuItem.ShortcutKeys = Keys.Control | Keys.P;
            FilePrintMenuItem.Size = new Size(146, 22);
            FilePrintMenuItem.Text = "&Print";
            // 
            // FilePrintPreviewMenuItem
            // 
            FilePrintPreviewMenuItem.Image = (Image)resources.GetObject("FilePrintPreviewMenuItem.Image");
            FilePrintPreviewMenuItem.ImageTransparentColor = Color.Black;
            FilePrintPreviewMenuItem.Name = "FilePrintPreviewMenuItem";
            FilePrintPreviewMenuItem.Size = new Size(146, 22);
            FilePrintPreviewMenuItem.Text = "Print Pre&view";
            // 
            // FilePrintSetupMenuItem
            // 
            FilePrintSetupMenuItem.Name = "FilePrintSetupMenuItem";
            FilePrintSetupMenuItem.Size = new Size(146, 22);
            FilePrintSetupMenuItem.Text = "Print Setup";
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(143, 6);
            // 
            // FileExitMenuItem
            // 
            FileExitMenuItem.Name = "FileExitMenuItem";
            FileExitMenuItem.Size = new Size(146, 22);
            FileExitMenuItem.Text = "E&xit";
            FileExitMenuItem.Click += FileExitMenuItem_Click;
            // 
            // EditMenu
            // 
            EditMenu.DropDownItems.AddRange(new ToolStripItem[] { EditUndoMenuItem, EditRedoMenuItem, toolStripSeparator6, EditCutMenuItem, EditCopyMenuItem, EditPasteMenuItem, toolStripSeparator7, EditSelectAllMenuItem });
            EditMenu.Name = "EditMenu";
            EditMenu.Size = new Size(39, 20);
            EditMenu.Text = "&Edit";
            // 
            // EditUndoMenuItem
            // 
            EditUndoMenuItem.Image = (Image)resources.GetObject("EditUndoMenuItem.Image");
            EditUndoMenuItem.ImageTransparentColor = Color.Black;
            EditUndoMenuItem.Name = "EditUndoMenuItem";
            EditUndoMenuItem.ShortcutKeys = Keys.Control | Keys.Z;
            EditUndoMenuItem.Size = new Size(164, 22);
            EditUndoMenuItem.Text = "&Undo";
            // 
            // EditRedoMenuItem
            // 
            EditRedoMenuItem.Image = (Image)resources.GetObject("EditRedoMenuItem.Image");
            EditRedoMenuItem.ImageTransparentColor = Color.Black;
            EditRedoMenuItem.Name = "EditRedoMenuItem";
            EditRedoMenuItem.ShortcutKeys = Keys.Control | Keys.Y;
            EditRedoMenuItem.Size = new Size(164, 22);
            EditRedoMenuItem.Text = "&Redo";
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new Size(161, 6);
            // 
            // EditCutMenuItem
            // 
            EditCutMenuItem.Image = (Image)resources.GetObject("EditCutMenuItem.Image");
            EditCutMenuItem.ImageTransparentColor = Color.Black;
            EditCutMenuItem.Name = "EditCutMenuItem";
            EditCutMenuItem.ShortcutKeys = Keys.Control | Keys.X;
            EditCutMenuItem.Size = new Size(164, 22);
            EditCutMenuItem.Text = "Cu&t";
            // 
            // EditCopyMenuItem
            // 
            EditCopyMenuItem.Image = (Image)resources.GetObject("EditCopyMenuItem.Image");
            EditCopyMenuItem.ImageTransparentColor = Color.Black;
            EditCopyMenuItem.Name = "EditCopyMenuItem";
            EditCopyMenuItem.ShortcutKeys = Keys.Control | Keys.C;
            EditCopyMenuItem.Size = new Size(164, 22);
            EditCopyMenuItem.Text = "&Copy";
            // 
            // EditPasteMenuItem
            // 
            EditPasteMenuItem.Image = (Image)resources.GetObject("EditPasteMenuItem.Image");
            EditPasteMenuItem.ImageTransparentColor = Color.Black;
            EditPasteMenuItem.Name = "EditPasteMenuItem";
            EditPasteMenuItem.ShortcutKeys = Keys.Control | Keys.V;
            EditPasteMenuItem.Size = new Size(164, 22);
            EditPasteMenuItem.Text = "&Paste";
            // 
            // toolStripSeparator7
            // 
            toolStripSeparator7.Name = "toolStripSeparator7";
            toolStripSeparator7.Size = new Size(161, 6);
            // 
            // EditSelectAllMenuItem
            // 
            EditSelectAllMenuItem.Name = "EditSelectAllMenuItem";
            EditSelectAllMenuItem.ShortcutKeys = Keys.Control | Keys.A;
            EditSelectAllMenuItem.Size = new Size(164, 22);
            EditSelectAllMenuItem.Text = "Select &All";
            // 
            // ViewMenu
            // 
            ViewMenu.DropDownItems.AddRange(new ToolStripItem[] { ViewDashboardMenuItem, ViewPatientsMenuItem, ViewReportsMenuItem, ViewSettingsMenuItem, toolStripSeparator9, ViewToolbarMenuItem, ViewStatusbarMenuItem, ViewDocumentTabsMenuItem });
            ViewMenu.Name = "ViewMenu";
            ViewMenu.Size = new Size(44, 20);
            ViewMenu.Text = "&View";
            ViewMenu.DropDownOpening += ViewMenu_DropDownOpening;
            // 
            // ViewDashboardMenuItem
            // 
            ViewDashboardMenuItem.Name = "ViewDashboardMenuItem";
            ViewDashboardMenuItem.Size = new Size(157, 22);
            ViewDashboardMenuItem.Text = "Dashboard";
            ViewDashboardMenuItem.Click += ViewDashboardMenuItem_Click;
            // 
            // ViewPatientsMenuItem
            // 
            ViewPatientsMenuItem.Name = "ViewPatientsMenuItem";
            ViewPatientsMenuItem.Size = new Size(157, 22);
            ViewPatientsMenuItem.Text = "Patients";
            ViewPatientsMenuItem.Click += ViewPatientsMenuItem_Click;
            // 
            // ViewReportsMenuItem
            // 
            ViewReportsMenuItem.Name = "ViewReportsMenuItem";
            ViewReportsMenuItem.Size = new Size(157, 22);
            ViewReportsMenuItem.Text = "Reports";
            ViewReportsMenuItem.Click += ViewReportsMenuItem_Click;
            // 
            // ViewSettingsMenuItem
            // 
            ViewSettingsMenuItem.Name = "ViewSettingsMenuItem";
            ViewSettingsMenuItem.Size = new Size(157, 22);
            ViewSettingsMenuItem.Text = "Settings";
            ViewSettingsMenuItem.Click += ViewSettingsMenuItem_Click;
            // 
            // toolStripSeparator9
            // 
            toolStripSeparator9.Name = "toolStripSeparator9";
            toolStripSeparator9.Size = new Size(154, 6);
            // 
            // ViewToolbarMenuItem
            // 
            ViewToolbarMenuItem.Checked = true;
            ViewToolbarMenuItem.CheckOnClick = true;
            ViewToolbarMenuItem.CheckState = CheckState.Checked;
            ViewToolbarMenuItem.Name = "ViewToolbarMenuItem";
            ViewToolbarMenuItem.Size = new Size(157, 22);
            ViewToolbarMenuItem.Text = "&Toolbar";
            ViewToolbarMenuItem.Click += ViewToolBarMenuItem_Click;
            // 
            // ViewStatusbarMenuItem
            // 
            ViewStatusbarMenuItem.Checked = true;
            ViewStatusbarMenuItem.CheckOnClick = true;
            ViewStatusbarMenuItem.CheckState = CheckState.Checked;
            ViewStatusbarMenuItem.Name = "ViewStatusbarMenuItem";
            ViewStatusbarMenuItem.Size = new Size(157, 22);
            ViewStatusbarMenuItem.Text = "&Status Bar";
            ViewStatusbarMenuItem.Click += ViewStatusBarMenuItem_Click;
            // 
            // ViewDocumentTabsMenuItem
            // 
            ViewDocumentTabsMenuItem.DropDownItems.AddRange(new ToolStripItem[] { ViewTabsBottomMenuItem, ViewTabsRightMenuItem, ViewTabsLeftMenuItem, toolStripSeparator10, ViewTabsTaskbarMenuItem });
            ViewDocumentTabsMenuItem.Name = "ViewDocumentTabsMenuItem";
            ViewDocumentTabsMenuItem.Size = new Size(157, 22);
            ViewDocumentTabsMenuItem.Text = "Document Tabs";
            ViewDocumentTabsMenuItem.DropDownOpening += ViewDocumentTabsMenuItem_DropDownOpening;
            // 
            // ViewTabsBottomMenuItem
            // 
            ViewTabsBottomMenuItem.Name = "ViewTabsBottomMenuItem";
            ViewTabsBottomMenuItem.Size = new Size(159, 22);
            ViewTabsBottomMenuItem.Text = "Bottom";
            ViewTabsBottomMenuItem.Click += ViewTabsBottomMenuItem_Click;
            // 
            // ViewTabsRightMenuItem
            // 
            ViewTabsRightMenuItem.Name = "ViewTabsRightMenuItem";
            ViewTabsRightMenuItem.Size = new Size(159, 22);
            ViewTabsRightMenuItem.Text = "Right";
            ViewTabsRightMenuItem.Click += ViewTabsRightMenuItem_Click;
            // 
            // ViewTabsLeftMenuItem
            // 
            ViewTabsLeftMenuItem.Name = "ViewTabsLeftMenuItem";
            ViewTabsLeftMenuItem.Size = new Size(159, 22);
            ViewTabsLeftMenuItem.Text = "Left";
            ViewTabsLeftMenuItem.Click += ViewTabsLeftMenuItem_Click;
            // 
            // toolStripSeparator10
            // 
            toolStripSeparator10.Name = "toolStripSeparator10";
            toolStripSeparator10.Size = new Size(156, 6);
            // 
            // ViewTabsTaskbarMenuItem
            // 
            ViewTabsTaskbarMenuItem.Name = "ViewTabsTaskbarMenuItem";
            ViewTabsTaskbarMenuItem.Size = new Size(159, 22);
            ViewTabsTaskbarMenuItem.Text = "Open in Taskbar";
            ViewTabsTaskbarMenuItem.Click += ViewTabsTaskbarMenuItem_Click;
            // 
            // ToolsMenu
            // 
            ToolsMenu.DropDownItems.AddRange(new ToolStripItem[] { ToolsOptionsMenuItem });
            ToolsMenu.Name = "ToolsMenu";
            ToolsMenu.Size = new Size(47, 20);
            ToolsMenu.Text = "&Tools";
            // 
            // ToolsOptionsMenuItem
            // 
            ToolsOptionsMenuItem.Name = "ToolsOptionsMenuItem";
            ToolsOptionsMenuItem.Size = new Size(125, 22);
            ToolsOptionsMenuItem.Text = "&Options...";
            // 
            // WindowsMenu
            // 
            WindowsMenu.DropDownItems.AddRange(new ToolStripItem[] { WindowCascadeMenuItem, WindowTileVerticalMenuItem, WindowTileHorizontalMenuItem, WindowCloseAllMenuItem, WindowArrangeIconsMenuItem });
            WindowsMenu.Name = "WindowsMenu";
            WindowsMenu.Size = new Size(63, 20);
            WindowsMenu.Text = "&Window";
            // 
            // WindowCascadeMenuItem
            // 
            WindowCascadeMenuItem.Name = "WindowCascadeMenuItem";
            WindowCascadeMenuItem.Size = new Size(151, 22);
            WindowCascadeMenuItem.Text = "&Cascade";
            WindowCascadeMenuItem.Click += WindowCascadeMenuItem_Click;
            // 
            // WindowTileVerticalMenuItem
            // 
            WindowTileVerticalMenuItem.Name = "WindowTileVerticalMenuItem";
            WindowTileVerticalMenuItem.Size = new Size(151, 22);
            WindowTileVerticalMenuItem.Text = "Tile &Vertical";
            WindowTileVerticalMenuItem.Click += WindowTileVerticalMenuItem_Click;
            // 
            // WindowTileHorizontalMenuItem
            // 
            WindowTileHorizontalMenuItem.Name = "WindowTileHorizontalMenuItem";
            WindowTileHorizontalMenuItem.Size = new Size(151, 22);
            WindowTileHorizontalMenuItem.Text = "Tile &Horizontal";
            WindowTileHorizontalMenuItem.Click += WindowTileHorizontalMenuItem_Click;
            // 
            // WindowCloseAllMenuItem
            // 
            WindowCloseAllMenuItem.Name = "WindowCloseAllMenuItem";
            WindowCloseAllMenuItem.Size = new Size(151, 22);
            WindowCloseAllMenuItem.Text = "C&lose All";
            WindowCloseAllMenuItem.Click += WindowCloseAllMenuItem_Click;
            // 
            // WindowArrangeIconsMenuItem
            // 
            WindowArrangeIconsMenuItem.Name = "WindowArrangeIconsMenuItem";
            WindowArrangeIconsMenuItem.Size = new Size(151, 22);
            WindowArrangeIconsMenuItem.Text = "&Arrange Icons";
            WindowArrangeIconsMenuItem.Click += WindowArrangeIconsMenuItem_Click;
            // 
            // WinManMenuItem
            // 
            WinManMenuItem.Name = "WinManMenuItem";
            WinManMenuItem.Size = new Size(63, 20);
            WinManMenuItem.Text = "&Window";
            // 
            // HelpMenu
            // 
            HelpMenu.DropDownItems.AddRange(new ToolStripItem[] { HelpIndexMenuItem, HelpSearchMenuItem, toolStripSeparator8, HelpAboutMenuItem });
            HelpMenu.Name = "HelpMenu";
            HelpMenu.Size = new Size(44, 20);
            HelpMenu.Text = "&Help";
            // 
            // HelpIndexMenuItem
            // 
            HelpIndexMenuItem.Image = (Image)resources.GetObject("HelpIndexMenuItem.Image");
            HelpIndexMenuItem.ImageTransparentColor = Color.Black;
            HelpIndexMenuItem.Name = "HelpIndexMenuItem";
            HelpIndexMenuItem.ShortcutKeys = Keys.Control | Keys.F1;
            HelpIndexMenuItem.Size = new Size(148, 22);
            HelpIndexMenuItem.Text = "&Index";
            // 
            // HelpSearchMenuItem
            // 
            HelpSearchMenuItem.Image = (Image)resources.GetObject("HelpSearchMenuItem.Image");
            HelpSearchMenuItem.ImageTransparentColor = Color.Black;
            HelpSearchMenuItem.Name = "HelpSearchMenuItem";
            HelpSearchMenuItem.Size = new Size(148, 22);
            HelpSearchMenuItem.Text = "&Search";
            // 
            // toolStripSeparator8
            // 
            toolStripSeparator8.Name = "toolStripSeparator8";
            toolStripSeparator8.Size = new Size(145, 6);
            // 
            // HelpAboutMenuItem
            // 
            HelpAboutMenuItem.Name = "HelpAboutMenuItem";
            HelpAboutMenuItem.Size = new Size(148, 22);
            HelpAboutMenuItem.Text = "&About...";
            // 
            // MainToolbar
            // 
            MainToolbar.ImageScalingSize = new Size(31, 31);
            MainToolbar.Items.AddRange(new ToolStripItem[] { newToolStripButton, openToolStripButton, saveToolStripButton, toolStripSeparator1, printToolStripButton, printPreviewToolStripButton, toolStripSeparator2, helpToolStripButton });
            MainToolbar.Location = new Point(0, 24);
            MainToolbar.Name = "MainToolbar";
            MainToolbar.Padding = new Padding(0, 0, 2, 0);
            MainToolbar.Size = new Size(737, 38);
            MainToolbar.TabIndex = 1;
            MainToolbar.Text = "ToolStrip";
            // 
            // newToolStripButton
            // 
            newToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            newToolStripButton.Image = (Image)resources.GetObject("newToolStripButton.Image");
            newToolStripButton.ImageTransparentColor = Color.Black;
            newToolStripButton.Name = "newToolStripButton";
            newToolStripButton.Size = new Size(35, 35);
            newToolStripButton.Text = "New";
            // 
            // openToolStripButton
            // 
            openToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            openToolStripButton.Image = (Image)resources.GetObject("openToolStripButton.Image");
            openToolStripButton.ImageTransparentColor = Color.Black;
            openToolStripButton.Name = "openToolStripButton";
            openToolStripButton.Size = new Size(35, 35);
            openToolStripButton.Text = "Open";
            // 
            // saveToolStripButton
            // 
            saveToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            saveToolStripButton.Image = (Image)resources.GetObject("saveToolStripButton.Image");
            saveToolStripButton.ImageTransparentColor = Color.Black;
            saveToolStripButton.Name = "saveToolStripButton";
            saveToolStripButton.Size = new Size(35, 35);
            saveToolStripButton.Text = "Save";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 38);
            // 
            // printToolStripButton
            // 
            printToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            printToolStripButton.Image = (Image)resources.GetObject("printToolStripButton.Image");
            printToolStripButton.ImageTransparentColor = Color.Black;
            printToolStripButton.Name = "printToolStripButton";
            printToolStripButton.Size = new Size(35, 35);
            printToolStripButton.Text = "Print";
            // 
            // printPreviewToolStripButton
            // 
            printPreviewToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            printPreviewToolStripButton.Image = (Image)resources.GetObject("printPreviewToolStripButton.Image");
            printPreviewToolStripButton.ImageTransparentColor = Color.Black;
            printPreviewToolStripButton.Name = "printPreviewToolStripButton";
            printPreviewToolStripButton.Size = new Size(35, 35);
            printPreviewToolStripButton.Text = "Print Preview";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 38);
            // 
            // helpToolStripButton
            // 
            helpToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            helpToolStripButton.Image = (Image)resources.GetObject("helpToolStripButton.Image");
            helpToolStripButton.ImageTransparentColor = Color.Black;
            helpToolStripButton.Name = "helpToolStripButton";
            helpToolStripButton.Size = new Size(35, 35);
            helpToolStripButton.Text = "Help";
            // 
            // StatusStrip
            // 
            StatusStrip.BackColor = SystemColors.Control;
            StatusStrip.ImageScalingSize = new Size(31, 31);
            StatusStrip.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel });
            StatusStrip.Location = new Point(0, 501);
            StatusStrip.Name = "StatusStrip";
            StatusStrip.Padding = new Padding(1, 0, 16, 0);
            StatusStrip.Size = new Size(737, 22);
            StatusStrip.TabIndex = 2;
            StatusStrip.Text = "StatusStrip";
            // 
            // toolStripStatusLabel
            // 
            toolStripStatusLabel.Name = "toolStripStatusLabel";
            toolStripStatusLabel.Size = new Size(39, 17);
            toolStripStatusLabel.Text = "Status";
            // 
            // WindowManagerPanel
            // 
            WindowManagerPanel.AllowUserVerticalRepositioning = true;
            WindowManagerPanel.AutoDetectMdiChildWindows = true;
            WindowManagerPanel.AutoHide = true;
            WindowManagerPanel.ButtonRenderMode = MDIWindowManager.ButtonRenderMode.Standard;
            WindowManagerPanel.DisableCloseAction = false;
            WindowManagerPanel.DisableHTileAction = false;
            WindowManagerPanel.DisablePopoutAction = false;
            WindowManagerPanel.DisableTileAction = false;
            WindowManagerPanel.EmphasizeSelectedTab = true;
            WindowManagerPanel.EnableAuroGlassEffect = false;
            WindowManagerPanel.EnableTabPaintEvent = false;
            WindowManagerPanel.Location = new Point(143, 71);
            WindowManagerPanel.Margin = new Padding(0);
            WindowManagerPanel.MdiWindowListItem = WinManMenuItem;
            WindowManagerPanel.MinMode = false;
            WindowManagerPanel.Name = "WindowManagerPanel";
            WindowManagerPanel.Orientation = MDIWindowManager.WindowManagerOrientation.Bottom;
            WindowManagerPanel.SelectedTabBackColor = SystemColors.Window;
            WindowManagerPanel.SelectedTabForeColor = SystemColors.WindowText;
            WindowManagerPanel.ShowIcons = true;
            WindowManagerPanel.Size = new Size(592, 25);
            WindowManagerPanel.Style = MDIWindowManager.TabStyle.AngledTabs;
            WindowManagerPanel.TabBackColor = SystemColors.Control;
            WindowManagerPanel.TabForeColor = SystemColors.ControlText;
            WindowManagerPanel.TabIndex = 4;
            WindowManagerPanel.TabRenderMode = MDIWindowManager.TabsProvider.Standard;
            WindowManagerPanel.WordWrap = false;
            // 
            // panel1
            // 
            panel1.Controls.Add(SettingsSideButton);
            panel1.Controls.Add(ReportsSideButton);
            panel1.Controls.Add(PatientsSideButton);
            panel1.Controls.Add(DashboardSideButton);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 62);
            panel1.Margin = new Padding(2, 1, 2, 1);
            panel1.Name = "panel1";
            panel1.Size = new Size(141, 439);
            panel1.TabIndex = 6;
            // 
            // SettingsSideButton
            // 
            SettingsSideButton.FlatStyle = FlatStyle.Flat;
            SettingsSideButton.Location = new Point(6, 91);
            SettingsSideButton.Margin = new Padding(2, 1, 2, 1);
            SettingsSideButton.Name = "SettingsSideButton";
            SettingsSideButton.Size = new Size(125, 24);
            SettingsSideButton.TabIndex = 3;
            SettingsSideButton.Text = "Settings";
            SettingsSideButton.UseVisualStyleBackColor = true;
            SettingsSideButton.Click += SettingsSideButton_Click;
            // 
            // ReportsSideButton
            // 
            ReportsSideButton.FlatStyle = FlatStyle.Flat;
            ReportsSideButton.Location = new Point(6, 64);
            ReportsSideButton.Margin = new Padding(2, 1, 2, 1);
            ReportsSideButton.Name = "ReportsSideButton";
            ReportsSideButton.Size = new Size(125, 24);
            ReportsSideButton.TabIndex = 2;
            ReportsSideButton.Text = "Reports";
            ReportsSideButton.UseVisualStyleBackColor = true;
            ReportsSideButton.Click += ReportsSideButton_Click;
            // 
            // PatientsSideButton
            // 
            PatientsSideButton.FlatStyle = FlatStyle.Flat;
            PatientsSideButton.Location = new Point(6, 37);
            PatientsSideButton.Margin = new Padding(2, 1, 2, 1);
            PatientsSideButton.Name = "PatientsSideButton";
            PatientsSideButton.Size = new Size(125, 24);
            PatientsSideButton.TabIndex = 1;
            PatientsSideButton.Text = "Patients";
            PatientsSideButton.UseVisualStyleBackColor = true;
            PatientsSideButton.Click += PatientsSideButton_Click;
            // 
            // DashboardSideButton
            // 
            DashboardSideButton.FlatStyle = FlatStyle.Flat;
            DashboardSideButton.Location = new Point(6, 10);
            DashboardSideButton.Margin = new Padding(2, 1, 2, 1);
            DashboardSideButton.Name = "DashboardSideButton";
            DashboardSideButton.Size = new Size(125, 24);
            DashboardSideButton.TabIndex = 0;
            DashboardSideButton.Text = "Dashboard";
            DashboardSideButton.UseVisualStyleBackColor = true;
            DashboardSideButton.Click += DashboardSideButton_Click;
            // 
            // ToolStripPanel1
            // 
            ToolStripPanel1.Dock = DockStyle.Top;
            ToolStripPanel1.Location = new Point(0, 24);
            ToolStripPanel1.Name = "ToolStripPanel1";
            ToolStripPanel1.Orientation = Orientation.Horizontal;
            ToolStripPanel1.RowMargin = new Padding(3, 0, 0, 0);
            ToolStripPanel1.Size = new Size(737, 25);
            // 
            // AdvancedMdiForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(737, 523);
            Controls.Add(panel1);
            Controls.Add(WindowManagerPanel);
            Controls.Add(StatusStrip);
            Controls.Add(MainToolbar);
            Controls.Add(MainMenu);
            IsMdiContainer = true;
            Name = "AdvancedMdiForm";
            Text = "Advanced MDIWindowManagerTest";
            Load += AdvancedMdiForm_Load;
            Resize += AdvancedMdiForm_Resize;
            MainMenu.ResumeLayout(false);
            MainMenu.PerformLayout();
            MainToolbar.ResumeLayout(false);
            MainToolbar.PerformLayout();
            StatusStrip.ResumeLayout(false);
            StatusStrip.PerformLayout();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }
        #endregion


        private System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.ToolStrip MainToolbar;
        private System.Windows.Forms.StatusStrip StatusStrip;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem FilePrintSetupMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.ToolStripMenuItem HelpAboutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem WindowTileHorizontalMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FileMenu;
        private System.Windows.Forms.ToolStripMenuItem FileNewMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FileOpenMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FileSaveMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FileSaveAsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FilePrintMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FilePrintPreviewMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FileExitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditMenu;
        private System.Windows.Forms.ToolStripMenuItem EditUndoMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditRedoMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditCutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditCopyMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditPasteMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditSelectAllMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ViewMenu;
        private System.Windows.Forms.ToolStripMenuItem ViewToolbarMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ViewStatusbarMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolsMenu;
        private System.Windows.Forms.ToolStripMenuItem ToolsOptionsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem WindowsMenu;
        private System.Windows.Forms.ToolStripMenuItem WindowCascadeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem WindowTileVerticalMenuItem;
        private System.Windows.Forms.ToolStripMenuItem WindowCloseAllMenuItem;
        private System.Windows.Forms.ToolStripMenuItem WindowArrangeIconsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem HelpMenu;
        private System.Windows.Forms.ToolStripMenuItem HelpIndexMenuItem;
        private System.Windows.Forms.ToolStripMenuItem HelpSearchMenuItem;
        private System.Windows.Forms.ToolStripButton newToolStripButton;
        private System.Windows.Forms.ToolStripButton openToolStripButton;
        private System.Windows.Forms.ToolStripButton saveToolStripButton;
        private System.Windows.Forms.ToolStripButton printToolStripButton;
        private System.Windows.Forms.ToolStripButton printPreviewToolStripButton;
        private System.Windows.Forms.ToolStripButton helpToolStripButton;
        private System.Windows.Forms.ToolTip toolTip;
        private MDIWindowManager.WindowManagerPanel WindowManagerPanel;
        private ToolStripMenuItem ViewDashboardMenuItem;
        private ToolStripMenuItem ViewPatientsMenuItem;
        private ToolStripMenuItem ViewReportsMenuItem;
        private ToolStripMenuItem ViewSettingsMenuItem;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripMenuItem ViewDocumentTabsMenuItem;
        private ToolStripMenuItem ViewTabsBottomMenuItem;
        private ToolStripMenuItem ViewTabsLeftMenuItem;
        private ToolStripMenuItem ViewTabsRightMenuItem;
        private Panel panel1;
        private Button DashboardSideButton;
        private Button PatientsSideButton;
        private Button SettingsSideButton;
        private Button ReportsSideButton;
        private ToolStripSeparator toolStripSeparator10;
        private ToolStripMenuItem ViewTabsTaskbarMenuItem;
        private ToolStripMenuItem WinManMenuItem;
        private ToolStripPanel ToolStripPanel1;
    }
}




using MDIWindowManager;
using MDIWindowTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Text;
using System.Windows.Forms;

namespace MdiParentHelper
{
    public enum UIToolMerging
    {
        /// <summary>
        /// Menu and toolbar merging is dependent on UIMode.
        /// </summary>
        /// <remarks>ChildrenOnly: All windows participate in merging. MultiView: Only the current master window participates in merging.</remarks>
        Auto = 0,
        /// <summary>
        /// Any window with the focus will have its menus and toolbars merged with $safeprojectname$.
        /// </summary>
        /// <remarks></remarks>
        All = 1
    }

    public class MergedUITool
    {
        public object Tool;
        public bool OriginalToolVisibility;
        public Point OriginalToolLocation;
        public int OriginalToolZorder;
        public object OriginalParent;
        public bool OriginalParentVisibility;

        public MergedUITool(object toolObject, bool originalToolVisibility, Point originalToolLocation, int originalToolZorder, object originalParentObject, bool originalParentVisibility)
        {
            this.Tool = toolObject;
            this.OriginalToolVisibility = originalToolVisibility;
            this.OriginalToolLocation = originalToolLocation;
            this.OriginalToolZorder = originalToolZorder;
            this.OriginalParent = originalParentObject;
            this.OriginalParentVisibility = originalParentVisibility;
        }
    }

    public class ToolsMerge : Component, ISupportInitialize
    {
        private Form? _mdiParentForm = null;
        private WindowManagerPanel? _windowManagerPanel = null;
        private ToolStripPanel? _toolStripPanel = null;
        private MenuStrip? _menuStrip = null;
        private Form? _mergedForm = null;
        private List<MergedUITool> _mergedToolStrips = new List<MergedUITool>();
        private List<MergedUITool> _mergedMenuItems = new List<MergedUITool>();

        private ToolStripMenuItem? _editMenuItem = null;
        private ToolStripMenuItem? _editUndoMenuItem = null;
        private ToolStripMenuItem? _editRedoMenuItem = null;
        private ToolStripMenuItem? _editCutMenuItem = null;
        private ToolStripMenuItem? _editCopyMenuItem = null;
        private ToolStripMenuItem? _editPasteMenuItem = null;
        private ToolStripMenuItem? _editSelectAllMenuItem = null;
        private ToolStripMenuItem? _editFindMenuItem = null;
        private ToolStripMenuItem? _editFindNextMenuItem = null;

        private BasicEditHelper _basicEditHelper = new BasicEditHelper();

        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [TypeConverter(typeof(ReferenceConverter))]
        public Form? MdiParentForm
        {
            get => _mdiParentForm;
            set
            {
                if (value != null && object.ReferenceEquals(value, _mdiParentForm))
                    return;

                if (_mdiParentForm != null)
                {
                    _mdiParentForm.MdiChildActivate -= MdiParentForm_MdiChildActivate;
                }

                _mdiParentForm = value;

                if (_mdiParentForm != null)
                {
                    _mdiParentForm.MdiChildActivate += MdiParentForm_MdiChildActivate;
                }
            }
        }

        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [TypeConverter(typeof(ReferenceConverter))]
        public WindowManagerPanel? WindowManagerPanel
        {
            get => _windowManagerPanel;
            set
            {
                if (value != null && object.ReferenceEquals(value, _windowManagerPanel))
                    return;

                if (_windowManagerPanel != null)
                {
                    _windowManagerPanel.WrappedWindowAdded -= WindowManagerPanel_WrappedWindowAdded;
                    _windowManagerPanel.WindowPoppedIn -= WindowManagerPanel_WindowPoppedIn;
                    _windowManagerPanel.WindowPoppedOut -= WindowManagerPanel_WindowPoppedOut;
                }

                _windowManagerPanel = value;

                if (_windowManagerPanel != null)
                {
                    _windowManagerPanel.WrappedWindowAdded += WindowManagerPanel_WrappedWindowAdded;
                    _windowManagerPanel.WindowPoppedIn += WindowManagerPanel_WindowPoppedIn;
                    _windowManagerPanel.WindowPoppedOut += WindowManagerPanel_WindowPoppedOut;
                }
            }
        }

        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [TypeConverter(typeof(ReferenceConverter))]
        public ToolStripPanel? ToolStripPanel
        {
            get => _toolStripPanel;
            set => _toolStripPanel = value;
        }

        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [TypeConverter(typeof(ReferenceConverter))]
        public MenuStrip? MenuStrip
        {
            get => _menuStrip;
            set => _menuStrip = value;
        }

        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [TypeConverter(typeof(ReferenceConverter))]
        public ToolStripMenuItem? EditMenuItem
        {
            get => _editMenuItem;
            set => _editMenuItem = value;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Form? MergedForm
        {
            get => _mergedForm;
            private set
            {
                if (value != null && object.ReferenceEquals(value, _mergedForm))
                    return;

                if (_mergedForm != null)
                {
                    RemoveMergedChildTools(true);
                    try { _mergedForm.FormClosed -= MergedForm_FormClosed; }
                    catch { }
                }

                _mergedForm = value;

                if (_mergedForm != null)
                {
                    MergeChildTools();
                    _mergedForm.FormClosed += MergedForm_FormClosed;
                }
            }
        }

        public ToolsMerge() { }

        void ISupportInitialize.BeginInit() { }

        void ISupportInitialize.EndInit()
        {
            if (!this.DesignMode && _editMenuItem != null)
            {
                if (_editMenuItem.DropDownItems.Count > 0)
                    _editMenuItem.DropDownItems.Insert(0, new  ToolStripSeparator());

                _editMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem("Find Next", Resources.EditFindNextImage, EditFindNextMenuItem_Click, Keys.F3));
                _editFindNextMenuItem = (ToolStripMenuItem)_editMenuItem.DropDownItems[0];
                _editMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem("&Find...", Resources.EditFindImage, EditFindMenuItem_Click, Keys.Control | Keys.F));
                _editFindMenuItem = (ToolStripMenuItem)_editMenuItem.DropDownItems[0];
                _editMenuItem.DropDownItems.Insert(0, new ToolStripSeparator());
                _editMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem("Select &All", null, EditSelectAllMenuItem_Click, Keys.Control | Keys.A));
                _editSelectAllMenuItem = (ToolStripMenuItem)_editMenuItem.DropDownItems[0];
                _editMenuItem.DropDownItems.Insert(0, new ToolStripSeparator());
                _editMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem("&Paste", Resources.EditPasteImage, EditPasteMenuItem_Click, Keys.Control | Keys.V));
                _editPasteMenuItem = (ToolStripMenuItem)_editMenuItem.DropDownItems[0];
                _editMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem("&Copy", Resources.EditCopyImage, EditCopyMenuItem_Click, Keys.Control | Keys.C));
                _editCopyMenuItem = (ToolStripMenuItem)_editMenuItem.DropDownItems[0];
                _editMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem("Cu&t", Resources.EditCutImage, EditCutMenuItem_Click, Keys.Control | Keys.X));
                _editCutMenuItem = (ToolStripMenuItem)_editMenuItem.DropDownItems[0];
                _editMenuItem.DropDownItems.Insert(0, new ToolStripSeparator());
                _editMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem("&Redo", Resources.EditRedoImage, EditRedoMenuItem_Click, Keys.Control | Keys.Y));
                _editRedoMenuItem = (ToolStripMenuItem)_editMenuItem.DropDownItems[0];
                _editMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem("&Undo", Resources.EditUndoImage, EditUndoMenuItem_Click, Keys.Control | Keys.Z));
                _editUndoMenuItem = (ToolStripMenuItem)_editMenuItem.DropDownItems[0];

                _editMenuItem.DropDownOpening += EditMenuItem_DropDownOpening;
                
            }
        }

        private void MergeActiveMdiChild()
        {
            this.MergedForm = _mdiParentForm?.ActiveMdiChild;
        }

        private void MergeChildTools()
        {
            var parentForm = _mdiParentForm;

            if (parentForm != null)
            {
                parentForm.SuspendLayout();

                try
                {
                    var childForm = parentForm.ActiveMdiChild;

                    if (childForm != null)
                    {
                        if (childForm is IToolsMergeCapable customUIToolsCapableForm)
                        {
                            if (_toolStripPanel != null)
                            {
                                var toolstrip = customUIToolsCapableForm.Toolbar;

                                if (toolstrip != null)
                                {
                                    //only allowing one toolstrip for now
                                    _mergedToolStrips.Add(new MergedUITool(
                                        toolstrip,
                                        toolstrip.Visible,
                                        toolstrip.Location,
                                        toolstrip.Parent?.Controls.GetChildIndex(toolstrip) ?? 0,
                                        toolstrip.Parent ?? childForm,
                                        toolstrip.Visible));

                                    foreach (MergedUITool item in _mergedToolStrips)
                                    {
                                        int row = customUIToolsCapableForm.PreferToolbarOnSecondRow ? 1 : 0;
                                        ToolStrip ts = (ToolStrip)item.Tool;

                                        if (_toolStripPanel.Rows.Length < row + 1)
                                        {
                                            _toolStripPanel.Join(ts, row);
                                        }
                                        else
                                        {
                                            _toolStripPanel.Join(ts, GetRightmostPoint(_toolStripPanel, row));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                finally
                {
                    parentForm.ResumeLayout();
                }
            }
        }

        private void RemoveMergedChildTools(bool merging = false)
        {
            var parentForm = _mdiParentForm;

            if (parentForm != null)
            {
                if (!merging)
                    parentForm.SuspendLayout();

                try
                {
                    //toobar

                    if (_toolStripPanel != null)
                    {
                        var currentMergedToolStripsClone = new List<MergedUITool>();

                        foreach (MergedUITool item in _mergedToolStrips)
                        {
                            currentMergedToolStripsClone.Add(item);
                        }

                        foreach (MergedUITool item in currentMergedToolStripsClone)
                        {
                            var toolstrip = (ToolStrip)item.Tool;
                            var originalparent = item.OriginalParent as Control;

                            bool removeHandled = false;

                            if (originalparent != null)
                            {
                                if (!originalparent.IsDisposed)
                                {
                                    if (originalparent is ToolStripPanel)
                                    {
                                        ((ToolStripPanel)originalparent).Join(toolstrip, GetRightmostPoint((ToolStripPanel)originalparent, 0));
                                    }
                                    else
                                    {
                                        originalparent.Controls.Add(toolstrip);
                                        originalparent.Controls.SetChildIndex(toolstrip, item.OriginalToolZorder);
                                    }

                                    originalparent.Visible = false;
                                    // originalparent.Visible = item.OriginalParentVisibility
                                    removeHandled = true;
                                }
                            }

                            if (!removeHandled)
                            {
                                if (_toolStripPanel.Controls.Contains(toolstrip))
                                {
                                    _toolStripPanel.Controls.Remove(toolstrip);
                                }
                            }

                            _mergedToolStrips.Remove(item);
                        }
                    }

                    //menu strip

                    if (_editMenuItem != null)
                    {

                    }
                }
                finally
                {
                    if (!merging)
                        parentForm.ResumeLayout();
                }
            }
        }

        private Point GetRightmostPoint(ToolStripPanel toolStripPanel, int row)
        {
            if (_toolStripPanel != null)
            {
                int edge = toolStripPanel.RowMargin.Left;

                foreach (Control ctl in toolStripPanel.Controls)
                {
                    if (ctl is ToolStrip toolstrip && toolstrip.Visible)
                    {
                        if (toolStripPanel.PointToRow(toolstrip.Location) == toolStripPanel.Rows[row])
                        {
                            if (toolstrip.Right > edge)
                            {
                                edge = toolstrip.Right;
                            }
                        }
                    }
                }

                return new Point(edge, _toolStripPanel.Rows[row].Bounds.Top);
            }
            else
            {
                return new Point(0, 0);
            }
        }

        private Control? GetActiveControlOrForm()
        {
            if (_mdiParentForm?.ActiveControl != null && _mdiParentForm.ActiveControl is Form activeForm)
            {
                if (activeForm.ActiveControl != null)
                {
                    return activeForm.ActiveControl;
                }
                else
                {
                    return _mdiParentForm.ActiveControl;
                }
            }

            return _mdiParentForm;
        }

        public void PerformEditUndo()
        {
            bool done = false;

            if (_mdiParentForm!.ActiveMdiChild is ICustomEditOperationsCapable childCapable)
            {
                if (childCapable.WillHandleClipboardOperations() && childCapable.CanUndo())
                {
                    childCapable.PerformUndo();
                    done = true;
                }
            }

            if (!done)
            {
                Control? ctl = GetActiveControlOrForm();

                if (ctl != null)
                {
                    if (_basicEditHelper.CanUndo(ctl))
                    {
                        _basicEditHelper.Undo(ctl);
                    }
                }
            }
        }

        public void PerformEditRedo()
        {
            bool done = false;

            if (_mdiParentForm!.ActiveMdiChild is ICustomEditOperationsCapable childCapable)
            {
                if (childCapable.WillHandleClipboardOperations() && childCapable.CanRedo())
                {
                    childCapable.PerformRedo();
                    done = true;
                }
            }

            if (!done)
            {
                Control? ctl = GetActiveControlOrForm();

                if (ctl != null)
                {
                    if (_basicEditHelper.CanRedo(ctl))
                    {
                        _basicEditHelper.Redo(ctl);
                    }
                }
            }
        }

        public void PerformEditCut()
        {
            bool done = false;

            if (_mdiParentForm!.ActiveMdiChild is ICustomEditOperationsCapable childCapable)
            {
                if (childCapable.WillHandleClipboardOperations() && childCapable.CanCut())
                {
                    childCapable.PerformCut();
                    done = true;
                }
            }

            if (!done)
            {
                Control? ctl = GetActiveControlOrForm();

                if (ctl != null)
                {
                    if (_basicEditHelper.CanCut(ctl))
                    {
                        _basicEditHelper.Cut(ctl);
                    }
                }
            }
        }

        public void PerformEditCopy()
        {
            bool done = false;

            if (_mdiParentForm!.ActiveMdiChild is ICustomEditOperationsCapable childCapable)
            {
                if (childCapable.WillHandleClipboardOperations() && childCapable.CanCopy())
                {
                    childCapable.PerformCopy();
                    done = true;
                }
            }

            if (!done)
            {
                Control? ctl = GetActiveControlOrForm();

                if (ctl != null)
                {
                    if (_basicEditHelper.CanCopy(ctl))
                    {
                        _basicEditHelper.Copy(ctl);
                    }
                }
            }
        }

        public void PerformEditPaste()
        {
            bool done = false;

            if (_mdiParentForm!.ActiveMdiChild is ICustomEditOperationsCapable childCapable)
            {
                if (childCapable.WillHandleClipboardOperations() && childCapable.CanPaste())
                {
                    childCapable.PerformPaste();
                    done = true;
                }
            }

            if (!done)
            {
                Control? ctl = GetActiveControlOrForm();

                if (ctl != null)
                {
                    if (_basicEditHelper.CanPaste(ctl))
                    {
                        _basicEditHelper.Paste(ctl);
                    }
                }
            }
        }

        public void PerformEditSelectAll()
        {
            bool done = false;

            if (_mdiParentForm!.ActiveMdiChild is ICustomEditOperationsCapable childCapable)
            {
                if (childCapable.WillHandleClipboardOperations() && childCapable.CanSelectAll())
                {
                    childCapable.PerformSelectAll();
                    done = true;
                }
            }

            if (!done)
            {
                Control? ctl = GetActiveControlOrForm();

                if (ctl != null)
                {
                    if (_basicEditHelper.CanSelectAll(ctl))
                    {
                        _basicEditHelper.SelectAll(ctl);
                    }
                }
            }
        }

        public void PerformEditFind(bool findNext = false)
        {            
            bool done = false;

            if (_mdiParentForm!.ActiveMdiChild is ICustomEditOperationsCapable childCapable)
            {
                if (childCapable.WillHandleClipboardOperations() && childCapable.CanFind())
                {
                    //childCapable.PerformFind();
                    done = true;
                }
            }

            if (!done)
            {
                Control? ctl = GetActiveControlOrForm();

                if (ctl != null)
                {
                    if (_basicEditHelper.CanFind(ctl))
                    {
                        if (findNext)
                            _basicEditHelper.FindFormNext(_mdiParentForm!, ctl);
                        else
                            _basicEditHelper.ShowFindForm(_mdiParentForm!, ctl);
                    }
                }
            }
        }

        private void MdiParentForm_MdiChildActivate(object? sender, EventArgs e)
        {
            MergeActiveMdiChild();
        }

        private void WindowManagerPanel_WrappedWindowAdded(object? sender, WrappedWindowEventArgs e)
        {
            if (e.WrappedWindow.Window is IToolsMergeCapable toolsMergeCapableForm)
            {
                if (toolsMergeCapableForm.ToolbarPanel != null)
                    toolsMergeCapableForm.ToolbarPanel.Visible = false;

                if (toolsMergeCapableForm.MenuPanel != null)
                    toolsMergeCapableForm.MenuPanel.Visible = false;

                toolsMergeCapableForm.AfterWindowAdded();
            }
        }

        private void WindowManagerPanel_WindowPoppedIn(object? sender, WrappedWindowEventArgs e)
        {
            if (e.WrappedWindow.Window is IToolsMergeCapable toolsMergeCapableForm)
            {
                if (toolsMergeCapableForm.ToolbarPanel != null)
                    toolsMergeCapableForm.ToolbarPanel.Visible = false;

                if (toolsMergeCapableForm.MenuPanel != null)
                    toolsMergeCapableForm.MenuPanel.Visible = false;

                toolsMergeCapableForm.AfterWindowPopIn();
            }
        }

        private void WindowManagerPanel_WindowPoppedOut(object? sender, WrappedWindowEventArgs e)
        {
            if (e.WrappedWindow.Window is IToolsMergeCapable toolsMergeCapableForm)
            {
                if (toolsMergeCapableForm.ToolbarPanel != null)
                    toolsMergeCapableForm.ToolbarPanel.Visible = true;

                if (toolsMergeCapableForm.MenuPanel != null)
                    toolsMergeCapableForm.MenuPanel.Visible = true;

                toolsMergeCapableForm.AfterWindowPopOut();
            }
        }

        private void MergedForm_FormClosed(object? sender, EventArgs e)
        {
            if (this.MergedForm != null && object.ReferenceEquals(this.MergedForm, sender))
                this.MergedForm = null;
        }

        private void EditMenuItem_DropDownOpening(object? sender, EventArgs e)
        {
            if (_mergedForm != null)
            {
                bool handled = false;
                //bool canUndo = true;
                //bool canRedo = true;
                //bool canCut = true;
                //bool canCopy = true;
                //bool canPaste = true;
                bool canUndo = false;
                bool canRedo = false;
                bool canCut = false;
                bool canCopy = false;
                bool canPaste = false;
                bool canSelectAll = false;
                bool canFind = false;

                if (_mergedForm is ICustomEditOperationsCapable childCapable)
                {
                    if (childCapable.WillHandleClipboardOperations())
                    {
                        canUndo = childCapable.CanUndo();
                        canRedo = childCapable.CanRedo();
                        canCut = childCapable.CanCut();
                        canCopy = childCapable.CanCopy();
                        canPaste = childCapable.CanPaste();
                        canSelectAll = childCapable.CanSelectAll();
                        canFind = childCapable.CanFind();
                        handled = true;
                    }
                }

                if (!handled)
                {
                    Control? ctl = GetActiveControlOrForm();

                    if (ctl != null)
                    {
                        canUndo = _basicEditHelper.CanUndo(ctl);
                        canRedo = _basicEditHelper.CanRedo(ctl);
                        canCut = _basicEditHelper.CanCut(ctl);
                        canCopy = _basicEditHelper.CanCopy(ctl);
                        canPaste = _basicEditHelper.CanPaste(ctl);
                        canSelectAll = _basicEditHelper.CanSelectAll(ctl);
                        canFind = _basicEditHelper.CanFind(ctl);
                    }
                }

                _editUndoMenuItem!.Enabled = canUndo;
                _editRedoMenuItem!.Enabled = canRedo;
                _editCutMenuItem!.Enabled = canCut;
                _editCopyMenuItem!.Enabled = canCopy;
                _editPasteMenuItem!.Enabled = canPaste;
                _editSelectAllMenuItem!.Enabled = canSelectAll;
                _editFindMenuItem!.Enabled = canFind;
                _editFindNextMenuItem!.Enabled = canFind;
            }
            else
            {
                _editUndoMenuItem!.Enabled = false;
                _editRedoMenuItem!.Enabled = false;
                _editCutMenuItem!.Enabled = false;
                _editCopyMenuItem!.Enabled = false;
                _editPasteMenuItem!.Enabled = false;
                _editSelectAllMenuItem!.Enabled = false;
                _editFindMenuItem!.Enabled = false;
                _editFindNextMenuItem!.Enabled = false;
            }
        }

        private void EditUndoMenuItem_Click(object? sender, EventArgs e)
        {
            PerformEditUndo();
        }

        private void EditRedoMenuItem_Click(object? sender, EventArgs e)
        {
            PerformEditRedo();
        }

        private void EditCutMenuItem_Click(object? sender, EventArgs e)
        {
            PerformEditCut();
        }
        
        private void EditCopyMenuItem_Click(object? sender, EventArgs e)
        {
            PerformEditCopy();
        }
        
        private void EditPasteMenuItem_Click(object? sender, EventArgs e)
        {
            PerformEditPaste();
        }
        
        private void EditSelectAllMenuItem_Click(object? sender, EventArgs e)
        {
            PerformEditSelectAll();
        }

        private void EditFindMenuItem_Click(object? sender, EventArgs e)
        {
            PerformEditFind();
        }

        private void EditFindNextMenuItem_Click(object? sender, EventArgs e)
        {
            PerformEditFind(true);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var basicEditHelper = _basicEditHelper;

                if (basicEditHelper != null)
                {
                    basicEditHelper.Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }
}

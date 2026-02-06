using System;
using System.Collections;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace MDIWindowTools
{
    public class BasicEditHelper : IDisposable
    {
        [DllImport("user32", EntryPoint = "SendMessageA")]
        private static extern IntPtr SendMessage(int hwnd, int wMsg, int wParam, int lParam);

        public enum EditFindDirection
        {
            Down,
            Up
        }

        [Flags]
        public enum EditFindOptions
        {
            None = 0,
            MatchWholeWord = 2,
            MatchCase = 4
        }

        public enum FindStartFrom
        {
            Auto,
            Beginning,
            End
        }

        public enum FindResult
        {
            BOF,
            EOF,
            OK
        }

        public class FindFormResult
        {
            public string FindText { get; set; } = string.Empty;
            public DialogResult FindResult { get; set; } = DialogResult.OK;
        }

        private bool _disposed;

        private FindForm? _findForm = new FindForm();

        private ImmutableArray<string> _viableProperties = ImmutableArray.Create(new[] { "SelectedItems", "SelectedItem", "SelectedValue", "SelectedText", "Value", "Text" });

        public ImmutableArray<string> ViableProperties
        {
            get => _viableProperties;
        }

        public void Copy(Control ctl) => _Copy(ctl);

        public bool CanCopy(Control ctl) => _Copy(ctl, true);

        public void Cut(Control ctl) => _Cut(ctl);

        public bool CanCut(Control ctl) => _Cut(ctl, true);

        public void Paste(Control ctl) => _Paste(ctl);

        public bool CanPaste(Control ctl) => _Paste(ctl, true);

        public void Undo(Control ctl) => _Undo(ctl);

        public bool CanUndo(Control ctl) => _Undo(ctl, true);

        public void Redo(Control ctl) => _Redo(ctl);

        public bool CanSelectAll(Control ctl) => _SelectAll(ctl, true);

        public void SelectAll(Control ctl) => _SelectAll(ctl);

        public bool CanRedo(Control ctl) => _Redo(ctl, true);

        public bool CanFind(Control ctl) => _Find(ctl, string.Empty, FindStartFrom.Auto, true) == FindResult.OK;

        public FindResult Find(Control ctl, string sFindText, FindStartFrom iFindStartFrom, bool bSearchBackwards = false, bool bMatchWholeWord = false, bool bCaseSensitive = false)
            => _Find(ctl, sFindText, iFindStartFrom, false, bSearchBackwards, bMatchWholeWord, bCaseSensitive);

        private string _findText = string.Empty;
        private List<string> _findTextHistory = new List<string>();
        private bool _findSearchBackwards = false;
        private bool _findMatchWholeWord = false;
        private bool _findMatchCase = false;

        private bool _Copy(Control ctl, bool bTestOnly = false)
        {
            if (ctl is TextBoxBase)
            {
                if (SendMessage(ctl.Handle.ToInt32(), 0xD2, 0, 0).ToInt32() == 0)
                {
                    var txt = (TextBoxBase)ctl;

                    if (txt.SelectionLength == 0)
                    {
                        return false;
                    }
                    else
                    {
                        if (!bTestOnly)
                        {
                            txt.Copy();
                        }

                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else if (ctl is ComboBox)
            {
                if (!bTestOnly)
                {
                    var cmb = (ComboBox)ctl;

                    if (cmb.SelectionLength == 0)
                        Clipboard.SetDataObject(cmb.Text);
                    else
                        Clipboard.SetDataObject(cmb.SelectedText);
                }

                return true;
            }
            else if (ctl is ListBox)
            {
                var lst = (ListBox)ctl;

                if (bTestOnly)
                {
                    return lst.Items.Count > 0;
                }
                else
                {
                    int iResult;

                    if (lst.SelectedItems.Count > 0)
                    {
                        var dlg = new ClipboardCopyPrompt();

                        iResult = dlg.ShowDialog() == DialogResult.OK
                            ? (dlg.ClipboardOption1.Checked ? 2 : (dlg.ClipboardOption2.Checked ? 1 : 1))
                            : 0;
                    }
                    else
                    {
                        iResult = 1;
                    }

                    switch (iResult)
                    {
                        case 1:
                        case 2:
                            IEnumerable colItems;
                            if (iResult == 1)
                                colItems = lst.Items;
                            else
                                colItems = lst.SelectedItems;

                            var sb = new StringBuilder();
                            foreach (object o in colItems)
                                sb.Append(o.ToString() + "\r\n");

                            Clipboard.SetDataObject(sb.ToString());
                            return true;
                        default:
                            return false;
                    }
                }
            }
            else if (ctl is ListControl)
            {
                var lst = (ListControl)ctl;

                if (lst.SelectedValue != null)
                {
                    if (!bTestOnly)
                        Clipboard.SetDataObject(lst.SelectedValue);

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (ctl is ListView)
            {
                var lvw = (ListView)ctl;

                if (bTestOnly)
                {
                    return lvw.Items.Count > 0;
                }
                else
                {
                    int iResult;

                    if (lvw.SelectedItems.Count > 0)
                    {
                        var dlg = new ClipboardCopyPrompt();

                        iResult = dlg.ShowDialog() == DialogResult.OK
                            ? (dlg.ClipboardOption1.Checked ? 2 : (dlg.ClipboardOption2.Checked ? 1 : 1))
                            : 0;
                    }
                    else
                    {
                        iResult = 1;
                    }

                    switch (iResult)
                    {
                        case 1:
                        case 2:
                            IEnumerable colItems;
                            if (iResult == 1)
                                colItems = lvw.Items;
                            else
                                colItems = lvw.SelectedItems;

                            var sb = new StringBuilder();

                            foreach (ListViewItem oItem in colItems)
                            {
                                foreach (ListViewItem.ListViewSubItem oSubItem in oItem.SubItems)
                                    sb.Append(oSubItem.Text + "\t");

                                sb.Append("\r\n");
                            }

                            Clipboard.SetDataObject(sb.ToString());
                            return true;
                        default:
                            return false;
                    }
                }
            }
            else if (ctl is TreeView)
            {
                // todo: add treeview capability here
                return false;
            }
            else if (ctl is DateTimePicker)
            {
                var dtp = (DateTimePicker)ctl;

                if (!bTestOnly)
                    Clipboard.SetDataObject(dtp.Value.ToString());

                return true;
            }
            else if (ctl is ButtonBase)
            {
                if (!bTestOnly)
                    Clipboard.SetDataObject(ctl.Text);

                return true;
            }
            //else if (ctl is System.Windows.Forms.DataGrid)
            //{
            //    // todo: implement datagrid copy
            //    return false;
            //}
            else
            {
                if (!bTestOnly)
                {
                    if (ObjectContainsViableProperties(ctl))
                        Clipboard.SetDataObject(GetSelectedItemsTextFromUnknownObject(ctl));
                    else
                        SendMessage(ctl.Handle.ToInt32(), 0x301, 0, 0);
                }

                return true;
            }
        }

        private bool _Cut(Control ctl, bool bTestOnly = false)
        {
            if (ctl is TextBoxBase)
            {
                if (SendMessage(ctl.Handle.ToInt32(), 0xD2, 0, 0).ToInt32() == 0)
                {
                    var txt = (TextBoxBase)ctl;

                    if (!txt.ReadOnly)
                    {
                        if (txt.SelectionLength == 0)
                            return false;
                        else
                        {
                            if (!bTestOnly)
                                txt.Cut();

                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else if (ctl is ComboBox)
            {
                var cmb = (ComboBox)ctl;

                if (cmb.DropDownStyle != ComboBoxStyle.DropDownList)
                {
                    if (cmb.SelectionLength == 0)
                        return false;
                    else
                    {
                        if (!bTestOnly)
                        {
                            Clipboard.SetDataObject(cmb.SelectedText);
                            cmb.Text = string.Empty;
                        }

                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else if (ctl is ListBox) return false;
            else if (ctl is ListControl) return false;
            else if (ctl is ListView) return false;
            else if (ctl is TreeView) return false;
            else if (ctl is DateTimePicker) return false;
            else if (ctl is ButtonBase) return false;
            //else if (ctl is System.Windows.Forms.DataGrid) return false;
            else
            {
                if (!bTestOnly)
                    SendMessage(ctl.Handle.ToInt32(), 0x300, 0, 0);

                return true;
            }
        }

        private bool _Paste(Control ctl, bool bTestOnly = false)
        {
            if (ctl is TextBoxBase)
            {
                var txt = (TextBoxBase)ctl;

                var clipData = Clipboard.GetDataObject();

                if (!txt.ReadOnly && clipData != null && clipData.GetDataPresent("System.String", true))
                {
                    if (!bTestOnly)
                        txt.Paste();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (ctl is ComboBox)
            {
                var cmb = (ComboBox)ctl;

                var clipData = Clipboard.GetDataObject();

                if (cmb.DropDownStyle != ComboBoxStyle.DropDownList && clipData != null && clipData.GetDataPresent("System.String", true))
                {
                    if (!bTestOnly)
                    {
                        var data = Clipboard.GetDataObject()?.GetData("System.String", true) as string ?? string.Empty;

                        if (cmb.SelectionLength == 0)
                            cmb.Text = cmb.Text.Insert(cmb.SelectionStart, data);
                        else
                            cmb.SelectedText = data;
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (ctl is ListBox) return false;
            else if (ctl is ListControl) return false;
            else if (ctl is ListView) return false;
            else if (ctl is TreeView) return false;
            else if (ctl is DateTimePicker)
            {
                var dtp = (DateTimePicker)ctl;

                var dataObj = Clipboard.GetDataObject();

                if (dataObj != null && dataObj.GetDataPresent("System.DateTime", true))
                {
                    if (!bTestOnly)
                        dtp.Value = Convert.ToDateTime(dataObj.GetData("System.DateTime", true));

                    return true;
                }
                else if (dataObj != null && dataObj.GetDataPresent("System.String", true))
                {
                    var s = dataObj.GetData("System.String", true) as string;
                    if (DateTime.TryParse(s, out var dt))
                    {
                        if (!bTestOnly)
                            dtp.Value = dt;

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else if (ctl is ButtonBase) return false;
            //else if (ctl is System.Windows.Forms.DataGrid) return false;
            else
            {
                if (!bTestOnly)
                    SendMessage(ctl.Handle.ToInt32(), 0x302, 0, 0);

                return true;
            }
        }

        public bool _Undo(Control ctl, bool bTestOnly = false)
        {
            if (ctl is TextBoxBase)
            {
                var txt = (TextBoxBase)ctl;

                if (txt.CanUndo)
                {
                    if (!bTestOnly)
                        txt.Undo();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (ObjectContainsProperty(ctl, "CanUndo"))
            {
                var oPropertyInfo = ctl.GetType().GetProperty("CanUndo");

                if (oPropertyInfo != null)
                {
                    var oResult = ctl.GetType().InvokeMember("CanUndo", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty, null, ctl, null);

                    if (oResult != null)
                    {
                        try
                        {
                            if (Convert.ToBoolean(oResult))
                            {
                                if (!bTestOnly)
                                {
                                    var oMethodInfo = ctl.GetType().GetMethod("Undo");
                                    if (oMethodInfo != null)
                                        ctl.GetType().InvokeMember("Undo", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, ctl, null);
                                }

                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        catch
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (SendMessage(ctl.Handle.ToInt32(), 0xC6, 0, 0).ToInt32() != 0)
                {
                    if (!bTestOnly)
                        SendMessage(ctl.Handle.ToInt32(), 0x304, 0, 0);

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool _Redo(Control ctl, bool bTestOnly = false)
        {
            if (ctl is RichTextBox)
            {
                var rtxt = (RichTextBox)ctl;

                if (rtxt.CanRedo)
                {
                    if (!bTestOnly)
                        rtxt.Redo();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (ObjectContainsProperty(ctl, "CanRedo"))
            {
                var oPropertyInfo = ctl.GetType().GetProperty("CanRedo");

                if (oPropertyInfo != null)
                {
                    var oResult = ctl.GetType().InvokeMember("CanRedo", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty, null, ctl, null);

                    if (oResult != null)
                    {
                        try
                        {
                            if (Convert.ToBoolean(oResult))
                            {
                                if (!bTestOnly)
                                {
                                    var oMethodInfo = ctl.GetType().GetMethod("Redo");
                                    if (oMethodInfo != null)
                                        ctl.GetType().InvokeMember("Redo", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, ctl, null);
                                }

                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        catch
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (SendMessage(ctl.Handle.ToInt32(), 0x047A, 0, 0).ToInt32() != 0)
                {
                    if (!bTestOnly)
                        SendMessage(ctl.Handle.ToInt32(), 0x047B, 0, 0);

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool _SelectAll(Control ctl, bool bTestOnly = false)
        {
            if (ctl is RichTextBox)
            {
                var rtxt = (RichTextBox)ctl;

                if (!bTestOnly)
                    rtxt.SelectAll();

                return true;
            }
            else if (ObjectContainsMethod(ctl, "SelectAll"))
            {
                if (!bTestOnly)
                {
                    var oMethodInfo = ctl.GetType().GetMethod("SelectAll");

                    if (oMethodInfo != null)
                        ctl.GetType().InvokeMember("SelectAll", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, ctl, null);
                }

                return true;
            }
            else
            {
                if (!bTestOnly)
                    SendMessage(ctl.Handle.ToInt32(), 0x00B1, 0, -1);

                return true;
            }
        }

        private FindResult _Find(Control ctl, string sFindText, FindStartFrom iFindStartFrom, bool bTestOnly = false, bool bSearchBackwards = false, bool bMatchWholeWord = false, bool bCaseSensitive = false)
        {
            if (ctl == null)
                return FindResult.EOF;

            if (ObjectContainsProperty(ctl, "SelectionStart", true) && ObjectContainsProperty(ctl, "SelectionLength", true) && ObjectContainsProperty(ctl, "Text"))
            {
                if (ctl is TextBoxBase)
                {
                    if (SendMessage(ctl.Handle.ToInt32(), 0xD2, 0, 0).ToInt32() != 0)
                        return FindResult.EOF;
                }

                if (!bTestOnly)
                {
                    var sTextObj = ctl.GetType().InvokeMember("Text", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty, null, ctl, null);
                    var sText = sTextObj?.ToString() ?? string.Empty;

                    if (!string.IsNullOrEmpty(sText))
                    {
                        var iSelectionStart = Convert.ToInt32(ctl.GetType().InvokeMember("SelectionStart", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty, null, ctl, null));
                        var iSelectionLength = Convert.ToInt32(ctl.GetType().InvokeMember("SelectionLength", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty, null, ctl, null));

                        int iFindStart;

                        switch (iFindStartFrom)
                        {
                            case FindStartFrom.Auto:
                                if (iSelectionLength > 0)
                                {
                                    iFindStart = !bSearchBackwards ? iSelectionStart + iSelectionLength : iSelectionStart;
                                }
                                else
                                {
                                    iFindStart = iSelectionStart;
                                }
                                break;
                            case FindStartFrom.Beginning:
                                iFindStart = 0;
                                break;
                            case FindStartFrom.End:
                                iFindStart = Math.Max(0, sText.Length - 1);
                                break;
                            default:
                                iFindStart = iSelectionStart;
                                break;
                        }

                        var comparison = bCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

                        int iFoundPos;
                        bool bSearchAgain = true;

                        while (bSearchAgain)
                        {
                            if (!bSearchBackwards)
                            {
                                // IndexOf uses zero-based start index
                                iFoundPos = sText.IndexOf(sFindText, Math.Max(0, iFindStart), comparison);
                            }
                            else
                            {
                                if (iFindStart >= sText.Length)
                                    iFindStart = sText.Length - 1;

                                // LastIndexOf startIndex is zero-based and inclusive
                                iFoundPos = sText.LastIndexOf(sFindText, Math.Max(0, iFindStart), comparison);
                            }

                            bSearchAgain = false;

                            if (iFoundPos >= 0)
                            {
                                if (bMatchWholeWord)
                                {
                                    if (!IsWholeWord(sText, iFoundPos, sFindText.Length))
                                    {
                                        bSearchAgain = true;
                                    }

                                    if (bSearchAgain)
                                    {
                                        if (!bSearchBackwards)
                                            iFindStart = iFoundPos + sFindText.Length;
                                        else
                                            iFindStart = iFoundPos - 1;
                                    }
                                }

                                if (!bSearchAgain)
                                {
                                    // Select found range
                                    var iSelectionStartNew = iFoundPos;
                                    var iSelectionLengthNew = sFindText.Length;

                                    ctl.GetType().InvokeMember("SelectionStart", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, ctl, new object[] { iSelectionStartNew });
                                    ctl.GetType().InvokeMember("SelectionLength", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, ctl, new object[] { iSelectionLengthNew });

                                    return FindResult.OK;
                                }
                            }
                            else
                            {
                                return bSearchBackwards ? FindResult.BOF : FindResult.EOF;
                            }
                        }
                    }
                    else
                    {
                        return bSearchBackwards ? FindResult.BOF : FindResult.EOF;
                    }
                }

                return FindResult.OK;
            }
            else if (ObjectContainsProperty(ctl, "Items") && (ObjectContainsProperty(ctl, "SelectedIndex", true) || ObjectContainsProperty(ctl, "SelectedItem", true) || ObjectContainsProperty(ctl, "SelectedItems")))
            {
                var oResult = ctl.GetType().InvokeMember("Items", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty, null, ctl, null);

                if (oResult is IList colItems)
                {
                    if (!bTestOnly)
                    {
                        int iSelectMode;

                        if (ObjectContainsProperty(ctl, "SelectedIndex"))
                            iSelectMode = 1;
                        else if (ObjectContainsProperty(ctl, "SelectedItem"))
                            iSelectMode = 2;
                        else
                            iSelectMode = 3;

                        object? oSelectedItem = null;
                        int iSelectedIndex = -1;
                        IList? colSelectedItems = null;

                        switch (iSelectMode)
                        {
                            case 1:
                                iSelectedIndex = Convert.ToInt32(ctl.GetType().InvokeMember("SelectedIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty, null, ctl, null));
                                break;
                            case 2:
                                oSelectedItem = ctl.GetType().InvokeMember("SelectedItem", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty, null, ctl, null);
                                break;
                            case 3:
                                colSelectedItems = ctl.GetType().InvokeMember("SelectedItems", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty, null, ctl, null) as IList;
                                break;
                        }

                        int iFindStart;
                        switch (iFindStartFrom)
                        {
                            case FindStartFrom.Auto:
                                switch (iSelectMode)
                                {
                                    case 1:
                                        iFindStart = iSelectedIndex >= 0 ? iSelectedIndex : 0;
                                        break;
                                    case 2:
                                        iFindStart = oSelectedItem != null ? colItems.IndexOf(oSelectedItem) : 0;
                                        break;
                                    case 3:
                                        iFindStart = (colSelectedItems != null && colSelectedItems.Count > 0) ? colItems.IndexOf(colSelectedItems[colSelectedItems.Count - 1]) : 0;
                                        break;
                                    default:
                                        iFindStart = 0;
                                        break;
                                }
                                break;
                            case FindStartFrom.Beginning:
                                iFindStart = -1;
                                break;
                            case FindStartFrom.End:
                                iFindStart = colItems.Count;
                                break;
                            default:
                                iFindStart = 0;
                                break;
                        }

                        int iFindEnd;
                        int iFindStep;

                        if (bSearchBackwards)
                        {
                            iFindEnd = 0;
                            iFindStart -= 1;
                            iFindStep = -1;

                            if (iFindStart < 0)
                                return FindResult.BOF;
                        }
                        else
                        {
                            iFindStart += 1;
                            iFindEnd = colItems.Count - 1;
                            iFindStep = 1;

                            if (iFindStart > colItems.Count - 1)
                                return FindResult.EOF;
                        }

                        var comparison = bCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

                        for (int iPosX = iFindStart; (iFindStep > 0) ? iPosX <= iFindEnd : iPosX >= iFindEnd; iPosX += iFindStep)
                        {
                            string sText = string.Empty;
                            var oItem = colItems[iPosX];

                            if (oItem is IEnumerable && !(oItem is string))
                            {
                                var sb = new StringBuilder();
                                foreach (var oSubItem in (IEnumerable)oItem)
                                    sb.Append(GetTextFromUnknownObject(oSubItem) + "\r\n");

                                sText = sb.ToString();
                            }
                            else if (oItem is string)
                            {
                                sText = (string)oItem;
                            }
                            else
                            {
                                sText = GetTextFromUnknownObject(oItem);
                            }

                            if (!string.IsNullOrEmpty(sText))
                            {
                                bool bSearchAgain = true;
                                int iFindSubStart = 0;

                                while (bSearchAgain)
                                {
                                    bSearchAgain = false;

                                    if (!(iFindSubStart < 0) && !(iFindSubStart > sText.Length))
                                    {
                                        int iFoundPos = sText.IndexOf(sFindText, iFindSubStart, comparison);

                                        if (iFoundPos >= 0)
                                        {
                                            if (bMatchWholeWord)
                                            {
                                                if (!IsWholeWord(sText, iFoundPos, sFindText.Length))
                                                {
                                                    iFindSubStart = iFoundPos + sFindText.Length;
                                                    bSearchAgain = true;
                                                }
                                            }

                                            if (!bSearchAgain)
                                            {
                                                switch (iSelectMode)
                                                {
                                                    case 1:
                                                        ctl.GetType().InvokeMember("SelectedIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, ctl, new object[] { iPosX });
                                                        return FindResult.OK;
                                                    case 2:
                                                        ctl.GetType().InvokeMember("SelectedItem", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, ctl, new object?[] { oItem });
                                                        return FindResult.OK;
                                                    case 3:
                                                        if (colSelectedItems != null && colSelectedItems.GetType().GetMethod("Add") != null)
                                                        {
                                                            colSelectedItems.Add(oItem);
                                                            return FindResult.OK;
                                                        }
                                                        else if (oItem?.GetType().GetProperty("Selected") != null)
                                                        {
                                                            oItem.GetType().InvokeMember("Selected", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, oItem, new object[] { true });
                                                            return FindResult.OK;
                                                        }
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        return bSearchBackwards ? FindResult.BOF : FindResult.EOF;
                    }

                    return FindResult.OK;
                }
                else
                {
                    return FindResult.EOF;
                }
            }
            else
            {
                return FindResult.EOF;
            }
        }

        public bool ObjectContainsViableProperties(object obj)
        {
            foreach (var sProperty in _viableProperties)
            {
                if (ObjectContainsProperty(obj, sProperty))
                    return true;
            }

            return false;
        }

        public string GetSelectedItemsTextFromUnknownObject(object? obj)
        {
            if (obj == null)
                return string.Empty;

            foreach (var sProperty in _viableProperties)
            {
                if (ObjectContainsProperty(obj, sProperty))
                    return GetPropertyValueTextFromUnknownObject(obj, sProperty);
            }

            return obj.ToString() ?? string.Empty;
        }

        public bool ObjectContainsProperty(object obj, string prop, bool bNotReadOnly = false)
        {
            try
            {
                var oPropertyInfo = obj.GetType().GetProperty(prop);
                if (oPropertyInfo != null)
                {
                    if (!bNotReadOnly)
                        return true;
                    else
                        return oPropertyInfo.CanWrite;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        public bool ObjectContainsMethod(object obj, string method)
        {
            try
            {
                var oMethodInfo = obj.GetType().GetMethod(method);
                if (oMethodInfo != null)
                    return true;
            }
            catch
            {
                return false;
            }
            return false;
        }

        public string GetPropertyValueTextFromUnknownObject(object obj, string prop)
        {
            var oPropertyInfo = obj.GetType().GetProperty(prop);

            if (oPropertyInfo != null)
            {
                var oResult = obj.GetType().InvokeMember(prop, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty, null, obj, null);

                if (oResult is IEnumerable && !(oResult is string))
                {
                    var col = (IEnumerable)oResult;
                    var sb = new StringBuilder();

                    foreach (var o in col)
                        sb.Append(GetTextFromUnknownObject(o) + "\r\n");

                    return sb.ToString();
                }
                else
                {
                    return GetTextFromUnknownObject(oResult);
                }
            }
            else
            {
                return GetTextFromUnknownObject(obj);
            }
        }

        public string GetTextFromUnknownObject(object? obj)
        {
            if (obj == null)
                return string.Empty;

            var oPropertyInfo = obj.GetType().GetProperty("Text");

            if (oPropertyInfo != null)
            {
                var oResult = obj.GetType().InvokeMember("Text", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty, null, obj, null);

                if (oResult != null)
                    return oResult.ToString() ?? string.Empty;
                else
                    return obj.ToString() ?? string.Empty;
            }
            else
            {
                return obj.ToString() ?? string.Empty;
            }
        }

        public bool IsWholeWord(string sText, int iWordStart, int iWordLength)
        {
            if (iWordStart > 0)
            {
                if (IsAlphaChar(sText[iWordStart - 1]))
                    return false;
            }

            if (iWordStart + iWordLength < sText.Length)
            {
                if (IsAlphaChar(sText[iWordStart + iWordLength]))
                    return false;
            }

            return true;
        }

        public bool IsAlphaChar(char c)
        {
            int iAsciiCode = (int)c;
            return ((iAsciiCode >= 65 && iAsciiCode <= 90) || (iAsciiCode >= 97 && iAsciiCode <= 122));
        }

        public void ShowFindForm(Form owner, Control control)
        {
            if (_findForm != null)
            {
                var ctl = control ?? owner.ActiveControl;

                if (ctl != null)
                {
                    DrawingHelper.IndicateControl(ctl);

                    _findForm.FindTextComboBox.Items.Clear();
                    _findForm.FindTextComboBox.Items.AddRange(_findTextHistory.AsEnumerable().Reverse().ToArray());
                    _findForm.FindTextComboBox.Text = _findText;
                    _findForm.SearchBackwardsCheckBox.Checked = _findSearchBackwards;
                    _findForm.MatchWholeWordCheckBox.Checked = _findMatchWholeWord;
                    _findForm.MatchCaseCheckBox.Checked = _findMatchCase;
                    _findForm.Left = owner.Left + 50;
                    _findForm.Top = owner.Top + 50;

                    _findForm.ShowDialog(owner);

                    if (_findForm.FindDialogResult == DialogResult.OK && !string.IsNullOrEmpty(_findForm.FindTextComboBox.Text))
                    {
                        _findText = _findForm.FindTextComboBox.Text;
                        _findTextHistory.Remove(_findText);
                        _findTextHistory.Add(_findText);
                        _findSearchBackwards = _findForm.SearchBackwardsCheckBox.Checked;
                        _findMatchWholeWord = _findForm.MatchWholeWordCheckBox.Checked;
                        _findMatchCase = _findForm.MatchCaseCheckBox.Checked;

                        _FindFormNext(owner, ctl, _findText, _findSearchBackwards, _findMatchWholeWord, _findMatchCase);
                    }
                }
            }
        }

        public void FindFormNext(Form owner, Control ctl)
        {
            _FindFormNext(owner, ctl, _findText, _findSearchBackwards, _findMatchWholeWord, _findMatchCase);
        }

        private void _FindFormNext(Form owner, Control ctl, string sFindText, bool bSearchBackwards = false, bool bMatchWholeWord = false, bool bCaseSensitive = false)
        {
            var findResult = _Find(ctl, _findText, FindStartFrom.Auto, false, bSearchBackwards, bMatchWholeWord, bCaseSensitive);

            if (findResult == FindResult.EOF || findResult == FindResult.BOF)
            {
                var promptText = findResult switch
                {
                    FindResult.EOF => "Continue searching from the top?",
                    FindResult.BOF or _ => "Continue searching from the end?"
                };

                if (MessageBox.Show(owner, $"Text not found. {promptText}", "Find", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    findResult = _Find(ctl, _findText, findResult == FindResult.EOF ? FindStartFrom.Beginning : FindStartFrom.End, false, bSearchBackwards, bMatchWholeWord, bCaseSensitive);

                    if (findResult != FindResult.OK)
                    {
                        MessageBox.Show(owner, "Text not found.", "Find", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }
        //=> _Find(ctl, _findForm.FindTextComboBox.Text, FindStartFrom.Auto, false, _findForm.SearchBackwardsCheckBox.Checked, _findForm.MatchWholeWordCheckBox.Checked, _findForm.MatchCaseCheckBox.Checked);

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    var findForm = _findForm;

                    if (findForm != null)
                    {
                        findForm.Dispose();
                        _findForm = null;
                    }
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDIWindowTools
{
    public interface ICustomEditOperationsCapable
    {
        bool WillHandleClipboardOperations();
        bool CanUndo();
        bool CanRedo();
        bool CanCut();
        bool CanCopy();
        bool CanPaste();
        bool CanSelectAll();
        bool CanFind();

        void PerformUndo();
        void PerformRedo();
        void PerformCut();
        void PerformCopy();
        void PerformPaste();
        void PerformSelectAll();
        void ShowFindDialog();
        void PerformFindNext();
    }
}

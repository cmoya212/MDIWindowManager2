using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDIWindowTools
{
    public interface IToolsMergeCapable
    {
        bool PreferToolbarOnSecondRow { get; }
        ToolStrip? Toolbar { get; }
        MenuStrip? Menu { get; }
        Panel? ToolbarPanel { get; }
        Panel? MenuPanel { get; }
        void AfterWindowAdded();
        void AfterWindowPopOut();
        void AfterWindowPopIn();
    }
}

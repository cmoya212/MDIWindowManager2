using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;

public static class DrawingHelper
{
    public static void IndicateControl(Control ctl)
    {
        if (ctl != null)
        {
            int iDelay = 50;

            if (ctl.Parent != null)
            {
                Rectangle rect = ctl.Parent.RectangleToScreen(ctl.Bounds);

                for (int x = 0; x < 6; x++)
                {
                    ControlPaint.DrawReversibleFrame(rect, Color.Black, FrameStyle.Thick);
                    Thread.Sleep(iDelay);
                }
            }
        }
    }

    [DllImport("shlwapi.dll", CharSet = CharSet.Auto, EntryPoint = "PathCompactPathEx")]
    private static extern bool PathCompactPathEx(StringBuilder pszOut, string pszSrc, int cchMax, int dwFlags);

    public static string PathCompactPath(string path, int maxChars = 34)
    {
        var sb = new StringBuilder(maxChars);
        // PathCompactPathEx writes a null-terminated string into the buffer.
        PathCompactPathEx(sb, path, sb.Capacity, 0);
        return sb.ToString().TrimEnd('\0');
    }
}
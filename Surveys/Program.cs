using System;
using System.Windows.Forms;

namespace Surveys
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.Run(new MainForm());
        }
    }
}
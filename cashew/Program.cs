using System;
using System.Windows.Forms;

namespace cashew
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Console.Title = "The Cashew Project - Native Shell";
            Application.Run(new MainForm());
        }
    }
}
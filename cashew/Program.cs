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
            Console.Title = "The Cashew Project - Native Shell (do not close)";
            Console.WriteLine("Welcome to Cashew. Your programs output will be redirected here");
            Application.Run(new MainForm());
        }
    }
}
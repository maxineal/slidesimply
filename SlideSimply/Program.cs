using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace SlideSimply
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var mainform = new MainForm();
            if (mainform.IsReady) Application.Run(mainform);
        }
    }
}

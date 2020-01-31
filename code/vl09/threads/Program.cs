using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Thread.CurrentThread.Name = "Main thread";

            Mutex a = new Mutex(false, "VL09_Application");

            if (!a.WaitOne(0))
            {
                MessageBox.Show("Sorry, your application is already running.");
                //Application.Exit();
            }
            else
            {
                Application.Run(new Form1());
            }
        }
    }
}

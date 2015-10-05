using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Windows.Forms;

namespace fxModel
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        private static ManagerForm form;

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                form = new ManagerForm();
            }
            catch (Exception ex)
            {
                if (ex.Message == "Trading closed")
                    Environment.Exit(1);
                else
                    Environment.Exit(-1);
            }

            Application.Run(form);
        }
    }
}

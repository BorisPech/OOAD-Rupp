using System;
using System.Windows.Forms;
using SIMS.WinForms.Data;
using SIMS.WinForms.UI;

namespace SIMS.WinForms
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                DbInitializer.Initialize();
            }
            catch (Exception ex)
            {
                MessageBox.Show("DB Init Error:\n" + ex.Message, "SIMS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}

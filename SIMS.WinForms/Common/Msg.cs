using System.Windows.Forms;

namespace SIMS.WinForms.Common
{
    public static class Msg
    {
        public static void Info(string text, string title = "SIMS") =>
            MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Information);

        public static void Error(string text, string title = "SIMS") =>
            MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Error);

        public static bool Confirm(string text, string title = "SIMS") =>
            MessageBox.Show(text, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
    }
}
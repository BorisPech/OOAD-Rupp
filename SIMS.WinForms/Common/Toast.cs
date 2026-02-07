using System.Drawing;
using System.Windows.Forms;

namespace SIMS.WinForms.Common
{
    public enum ToastType { Success, Info, Error }

    public static class Toast
    {
        public static void Success(string message) => Show(message, ToastType.Success);
        public static void Info(string message) => Show(message, ToastType.Info);
        public static void Error(string message) => Show(message, ToastType.Error);

        public static void Show(string message, ToastType type)
        {
            var toast = new ToastForm(message, type);
            toast.Show();
        }
    }

    internal sealed class ToastForm : Form
    {
        private readonly System.Windows.Forms.Timer _life = new System.Windows.Forms.Timer();
        private readonly System.Windows.Forms.Timer _fade = new System.Windows.Forms.Timer();

        public ToastForm(string message, ToastType type)
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;
            StartPosition = FormStartPosition.Manual;
            Width = 360;
            Height = 70;
            Opacity = 0.96;

            var t = ThemeManager.Instance.Current;
            BackColor = t.Surface;

            var strip = new Panel { Dock = DockStyle.Left, Width = 8 };
            strip.BackColor = type == ToastType.Success
                ? Color.FromArgb(45, 190, 110)
                : (type == ToastType.Error ? t.Danger : t.Primary);
            Controls.Add(strip);

            var lbl = new Label
            {
                Dock = DockStyle.Fill,
                Text = message,
                Font = UiTheme.FontBold,
                ForeColor = t.Text,
                Padding = new Padding(12, 12, 12, 12)
            };
            Controls.Add(lbl);

            var wa = Screen.PrimaryScreen.WorkingArea;
            Left = wa.Right - Width - 16;
            Top = wa.Bottom - Height - 16;

            _life.Interval = 2200;
            _life.Tick += (s, e) => { _life.Stop(); BeginFadeOut(); };
            _life.Start();
        }

        private void BeginFadeOut()
        {
            _fade.Interval = 30;
            _fade.Tick += (s, e) =>
            {
                Opacity -= 0.06;
                if (Opacity <= 0.05) { _fade.Stop(); Close(); }
            };
            _fade.Start();
        }
    }
}
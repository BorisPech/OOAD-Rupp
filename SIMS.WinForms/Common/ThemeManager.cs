using System;
using System.Drawing;
using System.Windows.Forms;

namespace SIMS.WinForms.Common
{
    public sealed class ThemeManager
    {
        private static readonly Lazy<ThemeManager> _lazy = new Lazy<ThemeManager>(() => new ThemeManager());
        public static ThemeManager Instance => _lazy.Value;

        private ThemeManager() { }

        public AppTheme Current { get; } = AppTheme.Light;

        public void ApplyTo(Control root)
        {
            ApplyOne(root);
            foreach (Control child in root.Controls)
                ApplyTo(child);
        }

        private void ApplyOne(Control c)
        {
            var t = Current;
            var tag = c.Tag as string;

            if (tag == "appbg") c.BackColor = t.AppBg;
            if (tag == "surface") c.BackColor = t.Surface;
            if (tag == "surface2") c.BackColor = t.Surface2;

            if (c is Label lbl)
                lbl.ForeColor = (tag == "muted") ? t.Muted : t.Text;

            if (c is TextBoxBase tb)
            {
                tb.BackColor = t.InputBg;
                tb.ForeColor = t.Text;
            }
            if (c is ComboBox cb)
            {
                cb.BackColor = t.InputBg;
                cb.ForeColor = t.Text;
            }
            if (c is NumericUpDown nud)
            {
                nud.BackColor = t.InputBg;
                nud.ForeColor = t.Text;
            }

            if (c is DataGridView gv)
                UiTheme.ApplyGridStyle(gv);

            if (c is Button btn)
                ApplyButtonStyle(btn, tag, t);
        }

        private static void ApplyButtonStyle(Button b, string tag, AppTheme t)
        {
            if (tag == "btn-primary")
            {
                b.BackColor = t.Primary;
                b.ForeColor = Color.White;
                b.FlatAppearance.BorderSize = 0;

                b.MouseEnter += (_, __) => b.BackColor = t.PrimaryHover;
                b.MouseLeave += (_, __) => b.BackColor = t.Primary;
            }
            else if (tag == "btn-danger")
            {
                b.BackColor = t.Danger;
                b.ForeColor = Color.White;

                b.MouseEnter += (_, __) => b.BackColor = t.DangerHover;
                b.MouseLeave += (_, __) => b.BackColor = t.Danger;
            }
            else if (tag == "btn-ghost")
            {
                b.BackColor = t.Surface2;
                b.ForeColor = t.Text;
                b.FlatAppearance.BorderColor = t.Border;
                b.FlatAppearance.BorderSize = 1;

                b.MouseEnter += (_, __) => b.BackColor = t.Surface;
                b.MouseLeave += (_, __) => b.BackColor = t.Surface2;
            }
            else if (tag == "btn-nav")
            {
                b.BackColor = t.Surface;
                b.ForeColor = t.Text;
                b.FlatAppearance.BorderSize = 0;

                b.MouseEnter += (_, __) => b.BackColor = t.Surface2;
                b.MouseLeave += (_, __) => b.BackColor = t.Surface;
            }
        }
    }
}
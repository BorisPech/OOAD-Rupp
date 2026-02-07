using System.Drawing;
using System.Windows.Forms;

namespace SIMS.WinForms.Common
{
    internal static class UiTheme
    {
        public static System.Drawing.Font FontNormal => new System.Drawing.Font("Segoe UI", 10F, FontStyle.Regular);
        public static System.Drawing.Font FontBold => new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold);
        public static System.Drawing.Font FontTitle => new System.Drawing.Font("Segoe UI", 16F, FontStyle.Bold);

        public static Panel Card(int padding = 14) =>
            new Panel { Padding = new Padding(padding), BorderStyle = BorderStyle.None, Tag = "surface" };

        public static Label Title(string text) =>
            new Label { Text = text, AutoSize = true, Font = FontTitle };

        public static Label Hint(string text) =>
            new Label { Text = text, AutoSize = true, Font = FontNormal, Tag = "muted" };

        public static TextBox Input(int width = 240) =>
            new TextBox { Font = FontNormal, BorderStyle = BorderStyle.FixedSingle, Width = width };

        public static Button PrimaryButton(string text, int height = 36)
        {
            var b = BaseButton(text, height);
            b.Tag = "btn-primary";
            return b;
        }

        public static Button DangerButton(string text, int height = 36)
        {
            var b = BaseButton(text, height);
            b.Tag = "btn-danger";
            return b;
        }

        public static Button GhostButton(string text, int height = 36)
        {
            var b = BaseButton(text, height);
            b.Tag = "btn-ghost";
            b.FlatAppearance.BorderSize = 1;
            return b;
        }

        public static Button NavButton(string text, Image icon)
        {
            var b = new Button
            {
                Height = 46,
                Dock = DockStyle.Top,
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                Padding = new Padding(10, 0, 10, 0),
                Font = FontBold,
                Cursor = Cursors.Hand,
                Image = icon,
                Text = "  " + text,
                Tag = "btn-nav"
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        public static void ApplyGridStyle(DataGridView g)
        {
            var t = ThemeManager.Instance.Current;

            g.BorderStyle = BorderStyle.None;
            g.BackgroundColor = t.Surface;
            g.GridColor = t.Border;

            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersDefaultCellStyle.BackColor = t.Surface2;
            g.ColumnHeadersDefaultCellStyle.ForeColor = t.Text;
            g.ColumnHeadersDefaultCellStyle.Font = FontBold;

            g.DefaultCellStyle.BackColor = t.Surface;
            g.DefaultCellStyle.ForeColor = t.Text;
            g.DefaultCellStyle.SelectionBackColor = t.Primary;
            g.DefaultCellStyle.SelectionForeColor = Color.White;

            g.RowHeadersVisible = false;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.MultiSelect = false;
            g.AllowUserToAddRows = false;
            g.AllowUserToResizeRows = false;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private static Button BaseButton(string text, int height)
        {
            var b = new Button
            {
                Text = text,
                Height = height,
                FlatStyle = FlatStyle.Flat,
                Font = FontBold,
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }
    }
}
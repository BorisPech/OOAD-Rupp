using System.Drawing;

namespace SIMS.WinForms.Common
{
    public sealed class AppTheme
    {
        public Color AppBg { get; init; }
        public Color Surface { get; init; }
        public Color Surface2 { get; init; }
        public Color Border { get; init; }

        public Color Primary { get; init; }
        public Color PrimaryHover { get; init; }
        public Color Danger { get; init; }
        public Color DangerHover { get; init; }

        public Color Text { get; init; }
        public Color Muted { get; init; }
        public Color InputBg { get; init; }

        public static AppTheme Light => new AppTheme
        {
            AppBg = Color.FromArgb(245, 246, 250),
            Surface = Color.White,
            Surface2 = Color.FromArgb(250, 250, 252),
            Border = Color.FromArgb(225, 228, 235),

            Primary = Color.FromArgb(64, 115, 255),
            PrimaryHover = Color.FromArgb(52, 98, 235),
            Danger = Color.FromArgb(220, 53, 69),
            DangerHover = Color.FromArgb(200, 40, 55),

            Text = Color.FromArgb(33, 37, 41),
            Muted = Color.FromArgb(108, 117, 125),
            InputBg = Color.White
        };
    }
}
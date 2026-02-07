using System.Drawing;
using System.Windows.Forms;

namespace SIMS.WinForms.Common
{
    // No Assets icons. We use built-in system icons so UI still looks clean.
    internal static class IconStore
    {
        public static Image Dashboard => SystemIcons.Application.ToBitmap();
        public static Image Products => SystemIcons.Shield.ToBitmap();
        public static Image Customers => SystemIcons.Information.ToBitmap();
        public static Image Sales => SystemIcons.Warning.ToBitmap();
        public static Image LowStock => SystemIcons.Error.ToBitmap();
        public static Image Export => SystemIcons.WinLogo.ToBitmap();
    }
}
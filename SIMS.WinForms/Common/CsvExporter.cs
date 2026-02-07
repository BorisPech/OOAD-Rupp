using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace SIMS.WinForms.Common
{
    public static class CsvExporter
    {
        public static void ExportWithDialog<T>(
            IWin32Window owner,
            string defaultFileName,
            IEnumerable<T> rows,
            params (string Header, System.Func<T, object> Value)[] cols)
        {
            using (var sfd = new SaveFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv",
                FileName = defaultFileName,
                AddExtension = true
            })
            {
                if (sfd.ShowDialog(owner) != DialogResult.OK) return;

                using (var sw = new StreamWriter(sfd.FileName, false, new UTF8Encoding(true)))
                {
                    WriteCsv(sw, rows, cols);
                }
            }

            Toast.Success("Exported CSV ✅");
        }

        private static void WriteCsv<T>(StreamWriter sw, IEnumerable<T> rows,
            params (string Header, System.Func<T, object> Value)[] cols)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                if (i > 0) sw.Write(",");
                sw.Write(Escape(cols[i].Header));
            }
            sw.WriteLine();

            foreach (var r in rows)
            {
                for (int i = 0; i < cols.Length; i++)
                {
                    if (i > 0) sw.Write(",");
                    var v = cols[i].Value(r);
                    sw.Write(Escape(v == null ? "" : v.ToString()));
                }
                sw.WriteLine();
            }
        }

        private static string Escape(string s)
        {
            if (s == null) s = "";
            if (s.Contains("\"")) s = s.Replace("\"", "\"\"");
            if (s.Contains(",") || s.Contains("\n") || s.Contains("\r") || s.Contains("\""))
                return "\"" + s + "\"";
            return s;
        }
    }
}
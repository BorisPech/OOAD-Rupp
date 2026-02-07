using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SIMS.WinForms.Common;
using SIMS.WinForms.Domain;
using SIMS.WinForms.Services;

namespace SIMS.WinForms.UI.Views
{
    public sealed class LowStockView : UserControl
    {
        private readonly StockService _stockService;

        private DataGridView _grid;
        private List<Product> _current = new List<Product>();

        public LowStockView(StockService stockService)
        {
            _stockService = stockService;

            Tag = "appbg";
            BuildUi();
            LoadData();
            ThemeManager.Instance.ApplyTo(this);
        }

        private void BuildUi()
        {
            var card = UiTheme.Card(16);
            card.Dock = DockStyle.Fill;
            Controls.Add(card);

            var bar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 48, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
            card.Controls.Add(bar);

            var btnRefresh = UiTheme.GhostButton("Refresh", 36);
            btnRefresh.Click += (_, __) => LoadData();
            bar.Controls.Add(btnRefresh);

            var btnExport = UiTheme.GhostButton("Export CSV", 36);
            btnExport.Click += (_, __) => ExportCsv();
            bar.Controls.Add(btnExport);

            _grid = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = true };
            card.Controls.Add(_grid);
        }

        private void LoadData()
        {
            // quiet refresh (no toast spam)
            _current = _stockService.GetLowStock(false);
            _grid.DataSource = _current;
            UiTheme.ApplyGridStyle(_grid);
        }

        private void ExportCsv()
        {
            CsvExporter.ExportWithDialog(this, "low_stock.csv", _current,
                ("Code", x => x.Code),
                ("Name", x => x.Name),
                ("Category", x => x.Category),
                ("Stock", x => x.Stock),
                ("ReorderLevel", x => x.ReorderLevel)
            );
        }
    }
}

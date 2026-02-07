using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SIMS.WinForms.Common;
using SIMS.WinForms.Domain;
using SIMS.WinForms.Services;

namespace SIMS.WinForms.UI.Views
{
    public sealed class ProductsView : UserControl
    {
        private readonly ProductService _service;

        private DataGridView _grid;
        private TextBox _txtSearch;

        private TextBox _txtCode, _txtName, _txtCategory;
        private NumericUpDown _nudCost, _nudPrice, _nudStock, _nudReorder;
        private CheckBox _chkActive;

        private long _editingId = 0;
        private List<Product> _current = new List<Product>();

        public ProductsView(ProductService service)
        {
            _service = service;

            Tag = "appbg";
            BuildUi();
            LoadData("");
            ThemeManager.Instance.ApplyTo(this);
        }

        private void BuildUi()
        {
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                SplitterDistance = 760,
                FixedPanel = FixedPanel.Panel2
            };
            Controls.Add(split);

            // LEFT: grid + search + export
            var left = UiTheme.Card(14);
            left.Dock = DockStyle.Fill;
            split.Panel1.Controls.Add(left);

            var bar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 48, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
            left.Controls.Add(bar);

            _txtSearch = UiTheme.Input(280);
            _txtSearch.TextChanged += (_, __) => LoadData(_txtSearch.Text);
            bar.Controls.Add(_txtSearch);

            var btnRefresh = UiTheme.GhostButton("Refresh", 36);
            btnRefresh.Click += (_, __) => LoadData(_txtSearch.Text);
            bar.Controls.Add(btnRefresh);

            var btnExport = UiTheme.GhostButton("Export CSV", 36);
            btnExport.Click += (_, __) => ExportCsv();
            bar.Controls.Add(btnExport);

            _grid = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = true };
            _grid.SelectionChanged += (_, __) => OnPickRow();
            left.Controls.Add(_grid);

            // RIGHT: form
            var right = UiTheme.Card(18);
            right.Dock = DockStyle.Fill;
            split.Panel2.Controls.Add(right);

            var title = UiTheme.Title("Product Form");
            title.Location = new Point(10, 8);
            right.Controls.Add(title);

            int y = 52;
            AddLabel(right, "Code", 12, y); _txtCode = AddText(right, 12, y += 22);
            AddLabel(right, "Name", 12, y += 46); _txtName = AddText(right, 12, y += 22);
            AddLabel(right, "Category", 12, y += 46); _txtCategory = AddText(right, 12, y += 22);

            AddLabel(right, "Cost", 12, y += 46); _nudCost = AddMoney(right, 12, y += 22);
            AddLabel(right, "Price", 12, y += 46); _nudPrice = AddMoney(right, 12, y += 22);

            AddLabel(right, "Stock", 12, y += 46); _nudStock = AddInt(right, 12, y += 22);
            AddLabel(right, "Reorder Level", 12, y += 46); _nudReorder = AddInt(right, 12, y += 22);

            _chkActive = new CheckBox { Text = "Active", AutoSize = true, Left = 12, Top = y += 46 };
            right.Controls.Add(_chkActive);

            var actions = new FlowLayoutPanel { Left = 12, Top = y += 40, Width = 420, Height = 44, WrapContents = false };
            right.Controls.Add(actions);

            var btnSave = UiTheme.PrimaryButton("Save", 36);
            btnSave.Click += (_, __) => Save();
            actions.Controls.Add(btnSave);

            var btnDelete = UiTheme.DangerButton("Delete", 36);
            btnDelete.Click += (_, __) => Delete();
            actions.Controls.Add(btnDelete);

            var btnClear = UiTheme.GhostButton("Clear", 36);
            btnClear.Click += (_, __) => ClearForm();
            actions.Controls.Add(btnClear);
        }

        private void LoadData(string keyword)
        {
            _current = string.IsNullOrWhiteSpace(keyword) ? _service.GetAll() : _service.Search(keyword);
            _grid.DataSource = _current;
            UiTheme.ApplyGridStyle(_grid);
        }

        private void OnPickRow()
        {
            if (_grid.CurrentRow == null) return;
            var p = _grid.CurrentRow.DataBoundItem as Product;
            if (p == null) return;

            _editingId = p.Id;
            _txtCode.Text = p.Code;
            _txtName.Text = p.Name;
            _txtCategory.Text = p.Category;
            _nudCost.Value = ClampMoney(p.Cost);
            _nudPrice.Value = ClampMoney(p.Price);
            _nudStock.Value = ClampInt(p.Stock);
            _nudReorder.Value = ClampInt(p.ReorderLevel);
            _chkActive.Checked = p.IsActive;
        }

        private void Save()
        {
            try
            {
                var p = new Product();
                p.Id = _editingId;
                p.Code = _txtCode.Text;
                p.Name = _txtName.Text;
                p.Category = _txtCategory.Text;
                p.Cost = _nudCost.Value;
                p.Price = _nudPrice.Value;
                p.Stock = (int)_nudStock.Value;
                p.ReorderLevel = (int)_nudReorder.Value;
                p.IsActive = _chkActive.Checked;

                if (_editingId == 0)
                {
                    _service.Add(p);
                    Toast.Success("Product added ✅");
                }
                else
                {
                    _service.Update(p);
                    Toast.Success("Product updated ✅");
                }

                LoadData(_txtSearch.Text);
                ClearForm();
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }

        private void Delete()
        {
            if (_editingId <= 0)
            {
                Msg.Info("Please select a product to delete.");
                return;
            }

            if (!Msg.Confirm("Delete this product?")) return;

            try
            {
                _service.Delete(_editingId);
                Toast.Success("Product deleted ✅");
                LoadData(_txtSearch.Text);
                ClearForm();
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }

        private void ClearForm()
        {
            _editingId = 0;
            _txtCode.Text = "";
            _txtName.Text = "";
            _txtCategory.Text = "";
            _nudCost.Value = 0;
            _nudPrice.Value = 0;
            _nudStock.Value = 0;
            _nudReorder.Value = 0;
            _chkActive.Checked = true;
        }

        private void ExportCsv()
        {
            CsvExporter.ExportWithDialog(this, "products.csv", _current,
                ("Id", x => x.Id),
                ("Code", x => x.Code),
                ("Name", x => x.Name),
                ("Category", x => x.Category),
                ("Cost", x => x.Cost),
                ("Price", x => x.Price),
                ("Stock", x => x.Stock),
                ("ReorderLevel", x => x.ReorderLevel),
                ("IsActive", x => x.IsActive)
            );
        }

        private static void AddLabel(Control parent, string text, int x, int y)
        {
            var lbl = new Label { Text = text, AutoSize = true, Left = x, Top = y, Tag = "muted", Font = UiTheme.FontBold };
            parent.Controls.Add(lbl);
        }

        private static TextBox AddText(Control parent, int x, int y)
        {
            var tb = UiTheme.Input(360);
            tb.Left = x; tb.Top = y;
            parent.Controls.Add(tb);
            return tb;
        }

        private static NumericUpDown AddMoney(Control parent, int x, int y)
        {
            var n = new NumericUpDown
            {
                Left = x, Top = y, Width = 360,
                DecimalPlaces = 2, Maximum = 1000000, Minimum = 0,
                Font = UiTheme.FontNormal
            };
            parent.Controls.Add(n);
            return n;
        }

        private static NumericUpDown AddInt(Control parent, int x, int y)
        {
            var n = new NumericUpDown
            {
                Left = x, Top = y, Width = 360,
                DecimalPlaces = 0, Maximum = 1000000, Minimum = 0,
                Font = UiTheme.FontNormal
            };
            parent.Controls.Add(n);
            return n;
        }

        private static decimal ClampMoney(decimal v)
        {
            if (v < 0) return 0;
            if (v > 1000000) return 1000000;
            return v;
        }

        private static decimal ClampInt(int v)
        {
            if (v < 0) return 0;
            if (v > 1000000) return 1000000;
            return v;
        }
    }
}

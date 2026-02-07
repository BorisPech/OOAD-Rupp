using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SIMS.WinForms.Common;
using SIMS.WinForms.Domain;
using SIMS.WinForms.Services;

namespace SIMS.WinForms.UI.Views
{
    public sealed class CustomersView : UserControl
    {
        private readonly CustomerService _service;

        private DataGridView _grid;
        private TextBox _txtSearch;

        private TextBox _txtCode, _txtName, _txtPhone, _txtAddress;
        private CheckBox _chkActive;

        private long _editingId = 0;
        private List<Customer> _current = new List<Customer>();

        public CustomersView(CustomerService service)
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

            // LEFT
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

            // RIGHT
            var right = UiTheme.Card(18);
            right.Dock = DockStyle.Fill;
            split.Panel2.Controls.Add(right);

            var title = UiTheme.Title("Customer Form");
            title.Location = new Point(10, 8);
            right.Controls.Add(title);

            int y = 52;
            AddLabel(right, "Code", 12, y); _txtCode = AddText(right, 12, y += 22);
            AddLabel(right, "Full Name", 12, y += 46); _txtName = AddText(right, 12, y += 22);
            AddLabel(right, "Phone", 12, y += 46); _txtPhone = AddText(right, 12, y += 22);
            AddLabel(right, "Address", 12, y += 46); _txtAddress = AddText(right, 12, y += 22);

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
            var c = _grid.CurrentRow.DataBoundItem as Customer;
            if (c == null) return;

            _editingId = c.Id;
            _txtCode.Text = c.Code;
            _txtName.Text = c.FullName;
            _txtPhone.Text = c.Phone;
            _txtAddress.Text = c.Address;
            _chkActive.Checked = c.IsActive;
        }

        private void Save()
        {
            try
            {
                var c = new Customer();
                c.Id = _editingId;
                c.Code = _txtCode.Text;
                c.FullName = _txtName.Text;
                c.Phone = _txtPhone.Text;
                c.Address = _txtAddress.Text;
                c.IsActive = _chkActive.Checked;

                if (_editingId == 0)
                {
                    _service.Add(c);
                    Toast.Success("Customer added ✅");
                }
                else
                {
                    _service.Update(c);
                    Toast.Success("Customer updated ✅");
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
                Msg.Info("Please select a customer to delete.");
                return;
            }

            if (!Msg.Confirm("Delete this customer?")) return;

            try
            {
                _service.Delete(_editingId);
                Toast.Success("Customer deleted ✅");
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
            _txtPhone.Text = "";
            _txtAddress.Text = "";
            _chkActive.Checked = true;
        }

        private void ExportCsv()
        {
            CsvExporter.ExportWithDialog(this, "customers.csv", _current,
                ("Id", x => x.Id),
                ("Code", x => x.Code),
                ("FullName", x => x.FullName),
                ("Phone", x => x.Phone),
                ("Address", x => x.Address),
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
    }
}

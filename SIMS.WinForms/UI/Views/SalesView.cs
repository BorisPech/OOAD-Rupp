using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SIMS.WinForms.Common;
using SIMS.WinForms.Domain;
using SIMS.WinForms.Domain.Enums;
using SIMS.WinForms.Services;

namespace SIMS.WinForms.UI.Views
{
    public sealed class SalesView : UserControl
    {
        private readonly ProductService _productService;
        private readonly CustomerService _customerService;
        private readonly SaleService _saleService;
        private readonly StockService _stockService;

        private ComboBox _cbCustomer;
        private ComboBox _cbProduct;
        private NumericUpDown _nudQty;

        private ComboBox _cbDiscountType;
        private NumericUpDown _nudDiscountValue;

        private DataGridView _gridItems;

        private Label _lblSubTotal;
        private Label _lblDiscount;
        private Label _lblGrand;

        private Button _btnSave;
        private Button _btnExport;

        private List<Product> _products = new List<Product>();
        private List<Customer> _customers = new List<Customer>();
        private List<SaleItem> _items = new List<SaleItem>();

        public SalesView(ProductService productService, CustomerService customerService, SaleService saleService, StockService stockService)
        {
            _productService = productService;
            _customerService = customerService;
            _saleService = saleService;
            _stockService = stockService;

            Tag = "appbg";
            BuildUi();
            LoadLookups();
            RefreshItemsGrid();
            ThemeManager.Instance.ApplyTo(this);
        }

        private void BuildUi()
        {
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
            Controls.Add(root);

            // LEFT: Build sale
            var left = UiTheme.Card(16);
            left.Dock = DockStyle.Fill;
            root.Controls.Add(left, 0, 0);

            var title = UiTheme.Title("Create Sale");
            title.Location = new Point(10, 8);
            left.Controls.Add(title);

            int y = 52;
            AddLabel(left, "Customer", 12, y);
            _cbCustomer = new ComboBox { Left = 12, Top = y += 22, Width = 520, DropDownStyle = ComboBoxStyle.DropDownList };
            left.Controls.Add(_cbCustomer);

            AddLabel(left, "Discount", 12, y += 46);
            _cbDiscountType = new ComboBox { Left = 12, Top = y += 22, Width = 260, DropDownStyle = ComboBoxStyle.DropDownList };
            left.Controls.Add(_cbDiscountType);

            _nudDiscountValue = new NumericUpDown
            {
                Left = 284, Top = y, Width = 248, DecimalPlaces = 2,
                Minimum = 0, Maximum = 1000000, Font = UiTheme.FontNormal
            };
            left.Controls.Add(_nudDiscountValue);

            _cbDiscountType.SelectedIndexChanged += (_, __) => RecalcTotals();
            _nudDiscountValue.ValueChanged += (_, __) => RecalcTotals();

            AddLabel(left, "Add Item", 12, y += 46);
            _cbProduct = new ComboBox { Left = 12, Top = y += 22, Width = 360, DropDownStyle = ComboBoxStyle.DropDownList };
            left.Controls.Add(_cbProduct);

            _nudQty = new NumericUpDown
            {
                Left = 384, Top = y, Width = 148, DecimalPlaces = 0,
                Minimum = 1, Maximum = 1000000, Font = UiTheme.FontNormal
            };
            left.Controls.Add(_nudQty);

            var btnAdd = UiTheme.PrimaryButton("Add", 36);
            btnAdd.Left = 544; btnAdd.Top = y - 2; btnAdd.Width = 110;
            btnAdd.Click += (_, __) => AddItem();
            left.Controls.Add(btnAdd);

            _gridItems = new DataGridView { Left = 12, Top = y += 56, Width = 760, Height = 360, Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom, AutoGenerateColumns = true };
            left.Controls.Add(_gridItems);

            var btnRemove = UiTheme.DangerButton("Remove Selected", 36);
            btnRemove.Left = 12; btnRemove.Top = y + 372; btnRemove.Width = 180;
            btnRemove.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            btnRemove.Click += (_, __) => RemoveSelected();
            left.Controls.Add(btnRemove);

            // RIGHT: Summary + Save
            var right = UiTheme.Card(16);
            right.Dock = DockStyle.Fill;
            root.Controls.Add(right, 1, 0);

            var t2 = UiTheme.Title("Summary");
            t2.Location = new Point(10, 8);
            right.Controls.Add(t2);

            _lblSubTotal = BuildSummaryRow(right, "SubTotal", 52);
            _lblDiscount = BuildSummaryRow(right, "Discount", 92);
            _lblGrand = BuildSummaryRow(right, "Grand Total", 132);

            _btnSave = UiTheme.PrimaryButton("Save Sale", 40);
            _btnSave.Left = 12; _btnSave.Top = 190; _btnSave.Width = 320;
            _btnSave.Click += (_, __) => SaveSale();
            right.Controls.Add(_btnSave);

            _btnExport = UiTheme.GhostButton("Export Items CSV", 40);
            _btnExport.Left = 12; _btnExport.Top = 240; _btnExport.Width = 320;
            _btnExport.Click += (_, __) => ExportItems();
            right.Controls.Add(_btnExport);

            var hint = UiTheme.Hint("Saving will reduce stock automatically.");
            hint.Left = 12; hint.Top = 292;
            right.Controls.Add(hint);
        }

        private void LoadLookups()
        {
            _products = _productService.GetAll();
            _customers = _customerService.GetAll();

            _cbProduct.DataSource = _products;
            _cbProduct.DisplayMember = "Name";

            _cbCustomer.DataSource = _customers;
            _cbCustomer.DisplayMember = "FullName";

            _cbDiscountType.Items.Clear();
            _cbDiscountType.Items.Add("None");
            _cbDiscountType.Items.Add("Percent");
            _cbDiscountType.Items.Add("Fixed Amount");
            _cbDiscountType.SelectedIndex = 0;
        }

        private void AddItem()
        {
            if (_cbProduct.SelectedItem == null)
            {
                Msg.Info("Please select a product.");
                return;
            }

            var p = _cbProduct.SelectedItem as Product;
            int qty = (int)_nudQty.Value;

            // refresh stock from DB (safer)
            var latest = _productService.GetById(p.Id);
            if (latest == null) { Msg.Error("Product not found."); return; }
            if (qty > latest.Stock) { Msg.Error("Not enough stock."); return; }

            // merge same product
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].ProductId == p.Id)
                {
                    _items[i].Qty += qty;
                    RefreshItemsGrid();
                    RecalcTotals();
                    return;
                }
            }

            var it = new SaleItem
            {
                ProductId = p.Id,
                ProductCode = p.Code,
                ProductName = p.Name,
                UnitPrice = p.Price,
                Qty = qty
            };
            _items.Add(it);

            RefreshItemsGrid();
            RecalcTotals();
        }

        private void RemoveSelected()
        {
            if (_gridItems.CurrentRow == null) return;
            var it = _gridItems.CurrentRow.DataBoundItem as SaleItem;
            if (it == null) return;

            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].ProductId == it.ProductId)
                {
                    _items.RemoveAt(i);
                    break;
                }
            }

            RefreshItemsGrid();
            RecalcTotals();
        }

        private void RefreshItemsGrid()
        {
            _gridItems.DataSource = null;
            _gridItems.DataSource = _items;
            UiTheme.ApplyGridStyle(_gridItems);
        }

        private void RecalcTotals()
        {
            var sale = BuildSalePreview();
            _saleService.RecalculateTotals(sale);

            _lblSubTotal.Text = sale.SubTotal.ToString("0.00");
            _lblDiscount.Text = sale.DiscountAmount.ToString("0.00");
            _lblGrand.Text = sale.GrandTotal.ToString("0.00");
        }

        private Sale BuildSalePreview()
        {
            var sale = new Sale();
            sale.Items = _items;

            sale.DiscountType = GetDiscountType();
            sale.DiscountValue = _nudDiscountValue.Value;

            return sale;
        }

        private DiscountType GetDiscountType()
        {
            if (_cbDiscountType.SelectedIndex == 1) return DiscountType.Percent;
            if (_cbDiscountType.SelectedIndex == 2) return DiscountType.FixedAmount;
            return DiscountType.None;
        }

        private void SaveSale()
        {
            try
            {
                if (_cbCustomer.SelectedItem == null) { Msg.Info("Please select customer."); return; }

                var c = _cbCustomer.SelectedItem as Customer;
                if (c == null) return;

                var sale = new Sale();
                sale.CustomerId = c.Id;
                sale.CustomerName = c.FullName;
                sale.Items = _items;

                sale.DiscountType = GetDiscountType();
                sale.DiscountValue = _nudDiscountValue.Value;

                var id = _saleService.SaveSale(sale);
                Toast.Success("Sale saved ✅ Invoice: " + sale.InvoiceNo);

                // refresh stock + notify low stock if any (publish true)
                _stockService.GetLowStock(true);

                // reset
                _items = new List<SaleItem>();
                RefreshItemsGrid();
                _nudDiscountValue.Value = 0;
                _cbDiscountType.SelectedIndex = 0;

                // reload product list to reflect stock changes
                LoadLookups();
                RecalcTotals();
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }

        private void ExportItems()
        {
            CsvExporter.ExportWithDialog(this, "sale_items.csv", _items,
                ("ProductCode", x => x.ProductCode),
                ("ProductName", x => x.ProductName),
                ("UnitPrice", x => x.UnitPrice),
                ("Qty", x => x.Qty),
                ("LineTotal", x => x.LineTotal)
            );
        }

        private static void AddLabel(Control parent, string text, int x, int y)
        {
            var lbl = new Label { Text = text, AutoSize = true, Left = x, Top = y, Tag = "muted", Font = UiTheme.FontBold };
            parent.Controls.Add(lbl);
        }

        private static Label BuildSummaryRow(Control parent, string title, int y)
        {
            var lblTitle = new Label { Text = title, AutoSize = true, Left = 12, Top = y, Tag = "muted", Font = UiTheme.FontBold };
            parent.Controls.Add(lblTitle);

            var lblValue = new Label { Text = "0.00", AutoSize = true, Left = 200, Top = y - 2, Font = new System.Drawing.Font("Segoe UI", 12F, FontStyle.Bold) };
            parent.Controls.Add(lblValue);

            return lblValue;
        }
    }
}

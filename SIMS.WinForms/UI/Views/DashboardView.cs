using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SIMS.WinForms.Common;
using SIMS.WinForms.Domain;
using SIMS.WinForms.Services;

namespace SIMS.WinForms.UI.Views
{
    public sealed class DashboardView : UserControl
    {
        private readonly ProductService _productService;
        private readonly CustomerService _customerService;
        private readonly SaleService _saleService;
        private readonly StockService _stockService;

        private Label _lblProducts;
        private Label _lblCustomers;
        private Label _lblLowStock;
        private DataGridView _gridSales;

        public DashboardView(ProductService productService, CustomerService customerService, SaleService saleService, StockService stockService)
        {
            _productService = productService;
            _customerService = customerService;
            _saleService = saleService;
            _stockService = stockService;

            Tag = "appbg";
            BuildUi();
            RefreshData();
            ThemeManager.Instance.ApplyTo(this);
        }

        private void BuildUi()
        {
            var top = new TableLayoutPanel { Dock = DockStyle.Top, Height = 140, ColumnCount = 4 };
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            Controls.Add(top);

            top.Controls.Add(BuildKpiCard("Products", out _lblProducts), 0, 0);
            top.Controls.Add(BuildKpiCard("Customers", out _lblCustomers), 1, 0);
            top.Controls.Add(BuildKpiCard("Low Stock", out _lblLowStock), 2, 0);

            var cardActions = UiTheme.Card(18);
            cardActions.Dock = DockStyle.Fill;
            var btnRefresh = UiTheme.GhostButton("Refresh", 36);
            btnRefresh.Width = 160;
            btnRefresh.Location = new Point(12, 12);
            btnRefresh.Click += (_, __) => RefreshData();
            cardActions.Controls.Add(btnRefresh);
            top.Controls.Add(cardActions, 3, 0);

            var card = UiTheme.Card(18);
            card.Dock = DockStyle.Fill;
            Controls.Add(card);

            var title = UiTheme.Title("Recent Sales");
            title.Location = new Point(10, 8);
            card.Controls.Add(title);

            _gridSales = new DataGridView { Dock = DockStyle.Fill, Top = 52, Height = 400, AutoGenerateColumns = true };
            card.Controls.Add(_gridSales);
        }

        private Panel BuildKpiCard(string title, out Label valueLabel)
        {
            var card = UiTheme.Card(18);
            card.Dock = DockStyle.Fill;

            var lblTitle = new Label { Text = title, AutoSize = true, Font = UiTheme.FontBold, Tag = "muted" };
            lblTitle.Location = new Point(10, 10);
            card.Controls.Add(lblTitle);

            valueLabel = new Label { Text = "0", AutoSize = true, Font = new System.Drawing.Font("Segoe UI", 22F, FontStyle.Bold) };
            valueLabel.Location = new Point(10, 44);
            card.Controls.Add(valueLabel);

            return card;
        }

        private void RefreshData()
        {
            var products = _productService.GetAll();
            var customers = _customerService.GetAll();
            var low = _stockService.GetLowStock(false);
            var sales = _saleService.GetRecentSales(30);

            _lblProducts.Text = products.Count.ToString();
            _lblCustomers.Text = customers.Count.ToString();
            _lblLowStock.Text = low.Count.ToString();

            _gridSales.DataSource = sales;
            UiTheme.ApplyGridStyle(_gridSales);
        }
    }
}

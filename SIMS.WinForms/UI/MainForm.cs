using System.Drawing;
using System.Windows.Forms;
using SIMS.WinForms.Common;
using SIMS.WinForms.Patterns.Observer;
using SIMS.WinForms.Services;
using SIMS.WinForms.UI.Views;

namespace SIMS.WinForms.UI
{
    public sealed class MainForm : Form
    {
        private readonly ProductService _productService = new ProductService();
        private readonly CustomerService _customerService = new CustomerService();
        private readonly SaleService _saleService = new SaleService();
        private readonly StockService _stockService = new StockService();
        private readonly NotificationService _notificationService = new NotificationService();

        private Panel _contentHost;
        private Label _pageTitle;

        public MainForm()
        {
            Text = "SIMS - Sales & Inventory Management System";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(1280, 720);
            MinimumSize = new Size(1100, 650);

            Tag = "appbg";
            BuildLayout();

            LowStockNotifier.Instance.Attach(_notificationService);

            ThemeManager.Instance.ApplyTo(this);
            Navigate(new DashboardView(_productService, _customerService, _saleService, _stockService), "Dashboard");
        }

        private void BuildLayout()
        {
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 270));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            Controls.Add(root);

            // Sidebar
            var sidebar = new Panel { Dock = DockStyle.Fill, Padding = new Padding(14), Tag = "surface" };
            root.Controls.Add(sidebar, 0, 0);

            var brand = UiTheme.Title("SIMS");
            brand.Location = new Point(12, 12);
            sidebar.Controls.Add(brand);

            var sub = UiTheme.Hint("Sales & Inventory (Light UI)");
            sub.Location = new Point(14, 44);
            sidebar.Controls.Add(sub);

            var nav = new Panel { Dock = DockStyle.Bottom, Height = 560 };
            sidebar.Controls.Add(nav);

            var btnLow = UiTheme.NavButton("Low Stock", IconStore.LowStock);
            var btnSales = UiTheme.NavButton("Sales", IconStore.Sales);
            var btnCustomers = UiTheme.NavButton("Customers", IconStore.Customers);
            var btnProducts = UiTheme.NavButton("Products", IconStore.Products);
            var btnDash = UiTheme.NavButton("Dashboard", IconStore.Dashboard);

            nav.Controls.Add(btnLow);
            nav.Controls.Add(btnSales);
            nav.Controls.Add(btnCustomers);
            nav.Controls.Add(btnProducts);
            nav.Controls.Add(btnDash);

            // Content
            var content = new Panel { Dock = DockStyle.Fill, Padding = new Padding(18), Tag = "appbg" };
            root.Controls.Add(content, 1, 0);

            var header = UiTheme.Card(14);
            header.Dock = DockStyle.Top;
            header.Height = 76;
            content.Controls.Add(header);

            _pageTitle = new Label { AutoSize = true, Font = UiTheme.FontTitle, Left = 10, Top = 18 };
            header.Controls.Add(_pageTitle);

            _contentHost = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 14, 0, 0), Tag = "appbg" };
            content.Controls.Add(_contentHost);

            btnDash.Click += (_, __) => Navigate(
                new DashboardView(_productService, _customerService, _saleService, _stockService), "Dashboard");

            btnProducts.Click += (_, __) => Navigate(
                new ProductsView(_productService), "Products");

            btnCustomers.Click += (_, __) => Navigate(
                new CustomersView(_customerService), "Customers");

            btnSales.Click += (_, __) => Navigate(
                new SalesView(_productService, _customerService, _saleService, _stockService), "Sales");

            btnLow.Click += (_, __) => Navigate(
                new LowStockView(_stockService), "Low Stock");
        }

        private void Navigate(UserControl view, string title)
        {
            _contentHost.SuspendLayout();
            _contentHost.Controls.Clear();

            view.Dock = DockStyle.Fill;
            _contentHost.Controls.Add(view);

            _contentHost.ResumeLayout();
            _pageTitle.Text = title;

            ThemeManager.Instance.ApplyTo(_contentHost);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            LowStockNotifier.Instance.Detach(_notificationService);
            base.OnFormClosed(e);
        }
    }
}

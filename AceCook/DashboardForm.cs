using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AceCook.Models;
using AceCook.Repositories;

namespace AceCook
{
    public partial class DashboardForm : Form
    {
        private Taikhoan _currentAccount;
        private Nhanvien _currentEmployee;
        private Quyentruycap _currentPermission;
        private AppDbContext _context;

        // UI Controls
        private Panel sidebarPanel;
        private Panel mainContentPanel;
        private Panel headerPanel;
        private TreeView menuTreeView;
        private Label lblUserInfo;
        private Label lblSystemTitle;
        private Panel contentPanel;

        public DashboardForm(Taikhoan account, Nhanvien employee, Quyentruycap permission)
        {
            _currentAccount = account;
            _currentEmployee = employee;
            _currentPermission = permission;
            InitializeDatabase();
            SetupUI();
            LoadDashboard();
        }

        private void InitializeDatabase()
        {
            try
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                var connectionString = configuration.GetConnectionString("Default");
                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                optionsBuilder.UseSqlServer(connectionString);
                _context = new AppDbContext(optionsBuilder.Options);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối database: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupUI()
        {
            // Form settings
            this.Text = "ACECOOK Sales Management System";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(240, 240, 240);

            // Header Panel
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(52, 73, 94)
            };

            lblSystemTitle = new Label
            {
                Text = "ACECOOK SALES MANAGEMENT",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft,
                Size = new Size(400, 40),
                Location = new Point(20, 20)
            };

            headerPanel.Controls.Add(lblSystemTitle);

            // Sidebar Panel
            sidebarPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 280,
                BackColor = Color.FromArgb(44, 62, 80)
            };

            // User Info Panel
            var userInfoPanel = new Panel
            {
                Size = new Size(280, 100),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(52, 73, 94)
            };

            lblUserInfo = new Label
            {
                Text = $"{_currentEmployee?.HoTenNv}\n{_currentPermission?.QuyenTruyCap}\nĐang hoạt động",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(280, 80),
                Location = new Point(0, 10)
            };

            userInfoPanel.Controls.Add(lblUserInfo);

            // Menu TreeView
            menuTreeView = new TreeView
            {
                Size = new Size(280, 600),
                Location = new Point(0, 100),
                BackColor = Color.FromArgb(44, 62, 80),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.None,
                ShowLines = false,
                ShowPlusMinus = false,
                FullRowSelect = true,
                HideSelection = false
            };

            // Setup menu items
            SetupMenuItems();

            sidebarPanel.Controls.Add(userInfoPanel);
            sidebarPanel.Controls.Add(menuTreeView);

            // Main Content Panel
            mainContentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250)
            };

            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250)
            };

            mainContentPanel.Controls.Add(contentPanel);

            // Add panels to form
            this.Controls.Add(mainContentPanel);
            this.Controls.Add(sidebarPanel);
            this.Controls.Add(headerPanel);

            // Event handlers
            menuTreeView.AfterSelect += MenuTreeView_AfterSelect;
        }

        private void SetupMenuItems()
        {
            // Dashboard
            var dashboardNode = new TreeNode("Dashboard")
            {
                Tag = "dashboard",
                ImageIndex = 0,
                SelectedImageIndex = 0
            };

            // Kinh doanh
            var businessNode = new TreeNode("Kinh doanh")
            {
                Tag = "business",
                ImageIndex = 1,
                SelectedImageIndex = 1
            };
            businessNode.Nodes.Add(new TreeNode("Quản lý khách hàng") { Tag = "customers" });
            businessNode.Nodes.Add(new TreeNode("Quản lý đơn hàng") { Tag = "orders" });

            // Kho hàng
            var warehouseNode = new TreeNode("Kho hàng")
            {
                Tag = "warehouse",
                ImageIndex = 2,
                SelectedImageIndex = 2
            };
            warehouseNode.Nodes.Add(new TreeNode("Quản lý sản phẩm") { Tag = "products" });
            warehouseNode.Nodes.Add(new TreeNode("Quản lý tồn kho") { Tag = "inventory" });

            // Nhà cung cấp
            var supplierNode = new TreeNode("Nhà cung cấp")
            {
                Tag = "suppliers",
                ImageIndex = 3,
                SelectedImageIndex = 3
            };

            // Báo cáo
            var reportNode = new TreeNode("Báo cáo")
            {
                Tag = "reports",
                ImageIndex = 4,
                SelectedImageIndex = 4
            };
            reportNode.Nodes.Add(new TreeNode("Báo cáo doanh thu") { Tag = "revenue_report" });
            reportNode.Nodes.Add(new TreeNode("Báo cáo tồn kho") { Tag = "inventory_report" });
            reportNode.Nodes.Add(new TreeNode("Báo cáo đơn hàng") { Tag = "order_report" });

            // Cài đặt
            var settingsNode = new TreeNode("Cài đặt")
            {
                Tag = "settings",
                ImageIndex = 5,
                SelectedImageIndex = 5
            };

            // Add nodes to treeview
            menuTreeView.Nodes.Add(dashboardNode);
            menuTreeView.Nodes.Add(businessNode);
            menuTreeView.Nodes.Add(warehouseNode);
            menuTreeView.Nodes.Add(supplierNode);
            menuTreeView.Nodes.Add(reportNode);
            menuTreeView.Nodes.Add(settingsNode);

            // Expand all nodes
            menuTreeView.ExpandAll();
        }

        private void MenuTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag != null)
            {
                string tag = e.Node.Tag.ToString();
                LoadContent(tag);
            }
        }

        private void LoadContent(string contentType)
        {
            contentPanel.Controls.Clear();

            switch (contentType)
            {
                case "dashboard":
                    LoadDashboard();
                    break;
                case "customers":
                    LoadCustomerManagement();
                    break;
                case "orders":
                    LoadOrderManagement();
                    break;
                case "products":
                    LoadProductManagement();
                    break;
                case "inventory":
                    LoadInventoryManagement();
                    break;
                case "suppliers":
                    LoadSupplierManagement();
                    break;
                case "revenue_report":
                    LoadRevenueReport();
                    break;
                case "inventory_report":
                    LoadInventoryReport();
                    break;
                case "order_report":
                    LoadOrderReport();
                    break;
                case "settings":
                    LoadSettings();
                    break;
            }
        }

        private void LoadDashboard()
        {
            var dashboardContent = new DashboardContent(_context);
            dashboardContent.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(dashboardContent);
        }

        private void LoadCustomerManagement()
        {
            var customerForm = new CustomerManagementForm(_context);
            customerForm.TopLevel = false;
            customerForm.FormBorderStyle = FormBorderStyle.None;
            customerForm.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(customerForm);
            customerForm.Show();
        }

        private void LoadOrderManagement()
        {
            var orderForm = new OrderManagementForm(_context);
            orderForm.TopLevel = false;
            orderForm.FormBorderStyle = FormBorderStyle.None;
            orderForm.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(orderForm);
            orderForm.Show();
        }

        private void LoadProductManagement()
        {
            var productForm = new ProductManagementForm(_context);
            productForm.TopLevel = false;
            productForm.FormBorderStyle = FormBorderStyle.None;
            productForm.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(productForm);
            productForm.Show();
        }

        private void LoadInventoryManagement()
        {
            var inventoryForm = new InventoryManagementForm(_context);
            inventoryForm.TopLevel = false;
            inventoryForm.FormBorderStyle = FormBorderStyle.None;
            inventoryForm.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(inventoryForm);
            inventoryForm.Show();
        }

        private void LoadSupplierManagement()
        {
            var supplierForm = new SupplierManagementForm(_context);
            supplierForm.TopLevel = false;
            supplierForm.FormBorderStyle = FormBorderStyle.None;
            supplierForm.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(supplierForm);
            supplierForm.Show();
        }

        private void LoadRevenueReport()
        {
            var revenueForm = new RevenueReportForm(_context);
            revenueForm.TopLevel = false;
            revenueForm.FormBorderStyle = FormBorderStyle.None;
            revenueForm.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(revenueForm);
            revenueForm.Show();
        }

        private void LoadInventoryReport()
        {
            var inventoryReportForm = new InventoryReportForm(_context);
            inventoryReportForm.TopLevel = false;
            inventoryReportForm.FormBorderStyle = FormBorderStyle.None;
            inventoryReportForm.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(inventoryReportForm);
            inventoryReportForm.Show();
        }

        private void LoadOrderReport()
        {
            var orderReportForm = new OrderReportForm(_context);
            orderReportForm.TopLevel = false;
            orderReportForm.FormBorderStyle = FormBorderStyle.None;
            orderReportForm.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(orderReportForm);
            orderReportForm.Show();
        }

        private void LoadSettings()
        {
            var settingsForm = new SettingsForm(_currentAccount, _context);
            settingsForm.TopLevel = false;
            settingsForm.FormBorderStyle = FormBorderStyle.None;
            settingsForm.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(settingsForm);
            settingsForm.Show();
        }
    }
} 
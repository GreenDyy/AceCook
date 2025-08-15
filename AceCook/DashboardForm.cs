using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AceCook.Models;
using AceCook.Repositories;
using System.Linq;

namespace AceCook
{
    public partial class DashboardForm : Form
    {
        private Taikhoan _currentAccount;
        private Nhanvien _currentEmployee;
        private Quyentruycap _currentPermission;
        private AppDbContext _context;

        public DashboardForm(Taikhoan account, Nhanvien employee, Quyentruycap permission)
        {
            _currentAccount = account;
            _currentEmployee = employee;
            _currentPermission = permission;
            InitializeComponent();
            InitializeDatabase();
            SetupForm();
            SetupMenuItems();
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

        private void SetupForm()
        {
            // Cập nhật thông tin người dùng
            lblUserName.Text = _currentEmployee?.HoTenNv ?? _currentAccount?.TenDangNhap ?? "Người dùng";
            lblUserRole.Text = _currentPermission?.QuyenTruyCap ?? "Nhân viên";
            lblUserStatus.Text = "Đang hoạt động";

            // Cấu hình form
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Cấu hình TreeView
            treeViewMenu.BackColor = Color.FromArgb(44, 62, 80);
            treeViewMenu.ForeColor = Color.White;
            treeViewMenu.Font = new Font("Segoe UI", 10);
            treeViewMenu.BorderStyle = BorderStyle.None;
            treeViewMenu.ShowLines = false;
            treeViewMenu.ShowPlusMinus = false;
            treeViewMenu.FullRowSelect = true;
            treeViewMenu.HideSelection = false;

            // Cấu hình header buttons
            SetupHeaderButtons();
        }

        private void SetupHeaderButtons()
        {
            // Nút Minimize
            btnMinimize.FlatAppearance.BorderSize = 0;
            btnMinimize.FlatStyle = FlatStyle.Flat;
            btnMinimize.BackColor = Color.FromArgb(52, 73, 94);
            btnMinimize.ForeColor = Color.White;
            btnMinimize.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnMinimize.Cursor = Cursors.Hand;

            // Nút Maximize
            btnMaximize.FlatAppearance.BorderSize = 0;
            btnMaximize.FlatStyle = FlatStyle.Flat;
            btnMaximize.BackColor = Color.FromArgb(52, 73, 94);
            btnMaximize.ForeColor = Color.White;
            btnMaximize.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnMaximize.Cursor = Cursors.Hand;

            // Nút Close
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.BackColor = Color.FromArgb(52, 73, 94);
            btnClose.ForeColor = Color.White;
            btnClose.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnClose.Cursor = Cursors.Hand;

            // Hover effects
            btnMinimize.MouseEnter += (s, e) => btnMinimize.BackColor = Color.FromArgb(70, 90, 110);
            btnMinimize.MouseLeave += (s, e) => btnMinimize.BackColor = Color.FromArgb(52, 73, 94);
            btnMaximize.MouseEnter += (s, e) => btnMaximize.BackColor = Color.FromArgb(70, 90, 110);
            btnMaximize.MouseLeave += (s, e) => btnMaximize.BackColor = Color.FromArgb(52, 73, 94);
            btnClose.MouseEnter += (s, e) => btnClose.BackColor = Color.FromArgb(220, 53, 69);
            btnClose.MouseLeave += (s, e) => btnClose.BackColor = Color.FromArgb(52, 73, 94);
        }

        private void SetupMenuItems()
        {
            treeViewMenu.Nodes.Clear();

            // Dashboard
            //var dashboardNode = CreateMenuNode("📊 Dashboard", "dashboard", Color.FromArgb(52, 152, 219));

            // Kinh doanh
            var businessNode = CreateMenuNode("💼 Kinh doanh", "business", Color.FromArgb(46, 204, 113));
            businessNode.Nodes.Add(CreateMenuNode("👥 Quản lý khách hàng", "customers", Color.FromArgb(46, 204, 113)));
            businessNode.Nodes.Add(CreateMenuNode("📋 Quản lý đơn hàng", "orders", Color.FromArgb(46, 204, 113)));

            // Kho hàng
            var warehouseNode = CreateMenuNode("🏪 Kho hàng", "warehouse", Color.FromArgb(155, 89, 182));
            warehouseNode.Nodes.Add(CreateMenuNode("📦 Quản lý sản phẩm", "products", Color.FromArgb(155, 89, 182)));
            warehouseNode.Nodes.Add(CreateMenuNode("📊 Quản lý tồn kho", "inventory", Color.FromArgb(155, 89, 182)));

            // Nhà cung cấp
            var supplierNode = CreateMenuNode("🚚 Nhà cung cấp", "suppliers", Color.FromArgb(230, 126, 34));

            // Báo cáo
            var reportNode = CreateMenuNode("📈 Báo cáo", "reports", Color.FromArgb(231, 76, 60));
            reportNode.Nodes.Add(CreateMenuNode("💰 Báo cáo doanh thu", "revenue_report", Color.FromArgb(231, 76, 60)));
            reportNode.Nodes.Add(CreateMenuNode("📊 Báo cáo tồn kho", "inventory_report", Color.FromArgb(231, 76, 60)));
            reportNode.Nodes.Add(CreateMenuNode("📋 Báo cáo đơn hàng", "order_report", Color.FromArgb(231, 76, 60)));

            // Đăng xuất
            var logoutNode = CreateMenuNode("🚪 Đăng xuất", "logout", Color.FromArgb(220, 53, 69));

            // Thêm nodes vào TreeView
            //treeViewMenu.Nodes.Add(dashboardNode);
            treeViewMenu.Nodes.Add(businessNode);
            treeViewMenu.Nodes.Add(warehouseNode);
            treeViewMenu.Nodes.Add(supplierNode);
            treeViewMenu.Nodes.Add(reportNode);
            treeViewMenu.Nodes.Add(logoutNode);

            // Mở rộng tất cả nodes
            treeViewMenu.ExpandAll();

            // Chọn Dashboard mặc định
            treeViewMenu.SelectedNode = businessNode.LastNode;
        }

        private TreeNode CreateMenuNode(string text, string tag, Color color)
        {
            var node = new TreeNode(text)
            {
                Tag = tag,
                ForeColor = color,
                NodeFont = new Font("Segoe UI", 10, FontStyle.Regular)
            };
            
            // Thêm tooltip cho nút đăng xuất
            if (tag == "logout")
            {
                node.ToolTipText = "Đăng xuất khỏi hệ thống và quay về màn hình đăng nhập";
            }
            
            return node;
        }

        private void treeViewMenu_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag != null)
            {
                string tag = e.Node.Tag.ToString();
                LoadContent(tag);
            }
        }

        private void LoadContent(string contentType)
        {
            panelContent.Controls.Clear();

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
                case "logout":
                    PerformLogout();
                    break;
            }
        }

        private void LoadDashboard()
        {
            try
            {
                var dashboardContent = new DashboardContent(_context);
                dashboardContent.Dock = DockStyle.Fill;
                panelContent.Controls.Add(dashboardContent);
            }
            catch (Exception ex)
            {
                ShowErrorContent("Dashboard", ex.Message);
            }
        }

        private void LoadCustomerManagement()
        {
            try
            {
                var customerForm = new CustomerManagementForm(_context);
                customerForm.TopLevel = false;
                customerForm.FormBorderStyle = FormBorderStyle.None;
                customerForm.Dock = DockStyle.Fill;
                panelContent.Controls.Add(customerForm);
                customerForm.Show();
            }
            catch (Exception ex)
            {
                ShowErrorContent("Quản lý khách hàng", ex.Message);
            }
        }

        private void LoadOrderManagement()
        {
            try
            {
                var orderForm = new OrderManagementForm(_context);
                orderForm.TopLevel = false;
                orderForm.FormBorderStyle = FormBorderStyle.None;
                orderForm.Dock = DockStyle.Fill;
                panelContent.Controls.Add(orderForm);
                orderForm.Show();
            }
            catch (Exception ex)
            {
                ShowErrorContent("Quản lý đơn hàng", ex.Message);
            }
        }

        private void LoadProductManagement()
        {
            try
            {
                var productForm = new ProductManagementForm(_context);
                productForm.TopLevel = false;
                productForm.FormBorderStyle = FormBorderStyle.None;
                productForm.Dock = DockStyle.Fill;
                panelContent.Controls.Add(productForm);
                productForm.Show();
            }
            catch (Exception ex)
            {
                ShowErrorContent("Quản lý sản phẩm", ex.Message);
            }
        }

        private void LoadInventoryManagement()
        {
            try
            {
                var inventoryForm = new InventoryManagementForm(_context);
                inventoryForm.TopLevel = false;
                inventoryForm.FormBorderStyle = FormBorderStyle.None;
                inventoryForm.Dock = DockStyle.Fill;
                panelContent.Controls.Add(inventoryForm);
                inventoryForm.Show();
            }
            catch (Exception ex)
            {
                ShowErrorContent("Quản lý tồn kho", ex.Message);
            }
        }

        private void LoadSupplierManagement()
        {
            try
            {
                var supplierForm = new SupplierForm(_context);
                supplierForm.TopLevel = false;
                supplierForm.FormBorderStyle = FormBorderStyle.None;
                supplierForm.Dock = DockStyle.Fill;
                panelContent.Controls.Add(supplierForm);
                supplierForm.Show();
            }
            catch (Exception ex)
            {
                ShowErrorContent("Quản lý nhà cung cấp", ex.Message);
            }
        }

        private void LoadRevenueReport()
        {
            try
            {
                var revenueReportForm = new RevenueReportForm();
                revenueReportForm.TopLevel = false;
                revenueReportForm.FormBorderStyle = FormBorderStyle.None;
                revenueReportForm.Dock = DockStyle.Fill;
                panelContent.Controls.Add(revenueReportForm);
                revenueReportForm.Show();
            }
            catch (Exception ex)
            {
                ShowErrorContent("Báo cáo doanh thu", ex.Message);
            }
        }

        private void LoadInventoryReport()
        {
            try
            {
                var inventoryReportForm = new InventoryReportForm(_context);
                inventoryReportForm.TopLevel = false;
                inventoryReportForm.FormBorderStyle = FormBorderStyle.None;
                inventoryReportForm.Dock = DockStyle.Fill;
                panelContent.Controls.Add(inventoryReportForm);
                inventoryReportForm.Show();
            }
            catch (Exception ex)
            {
                ShowErrorContent("Báo cáo tồn kho", ex.Message);
            }
        }

        private void LoadOrderReport()
        {
            try
            {
                var orderRepository = new OrderRepository(_context);
                var orderReportForm = new OrderReportForm(orderRepository);
                orderReportForm.TopLevel = false;
                orderReportForm.FormBorderStyle = FormBorderStyle.None;
                orderReportForm.Dock = DockStyle.Fill;
                panelContent.Controls.Add(orderReportForm);
                orderReportForm.Show();
            }
            catch (Exception ex)
            {
                ShowErrorContent("Báo cáo đơn hàng", ex.Message);
            }
        }

        private void PerformLogout()
        {
            try
            {
                var result = MessageBox.Show(
                    "Bạn có chắc chắn muốn đăng xuất?",
                    "Xác nhận đăng xuất",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Ẩn form hiện tại thay vì đóng
                    this.Hide();

                    // Mở lại form đăng nhập
                    var loginForm = new LoginForm();
                    loginForm.ShowDialog(); // Sử dụng ShowDialog để form đăng nhập chạy ở chế độ modal

                    // Sau khi form đăng nhập đóng, kiểm tra kết quả
                    // Nếu người dùng đăng nhập thành công
                    if (loginForm.DialogResult == DialogResult.OK)
                    {
                        // Hiển thị lại form hiện tại
                        this.Show();
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đăng xuất: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Bên trong LoginForm, sau khi đăng nhập thành công
        private void btnLogin_Click(object sender, EventArgs e)
        {
            // ... logic đăng nhập thành công
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ShowComingSoonContent(string title)
        {
            panelContent.Controls.Clear();
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250)
            };

            var label = new Label
            {
                Text = $"🚧 {title}\n\nTính năng này đang được phát triển...",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(108, 117, 125),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            panel.Controls.Add(label);
            panelContent.Controls.Add(panel);
        }

        private void ShowErrorContent(string title, string errorMessage)
        {
            panelContent.Controls.Clear();
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250)
            };

            var label = new Label
            {
                Text = $"❌ Lỗi: {title}\n\n{errorMessage}",
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                ForeColor = Color.FromArgb(220, 53, 69),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            panel.Controls.Add(label);
            panelContent.Controls.Add(panel);
        }

        // Header button events
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnMaximize_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
                btnMaximize.Text = "□";
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
                btnMaximize.Text = "❐";
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn thoát?", "Xác nhận thoát", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
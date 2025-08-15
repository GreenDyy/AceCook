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
            SetupHeaderButton(btnMinimize, Color.FromArgb(44, 62, 80), Color.FromArgb(70, 90, 110));
            SetupHeaderButton(btnMaximize, Color.FromArgb(44, 62, 80), Color.FromArgb(70, 90, 110));
            SetupHeaderButton(btnClose, Color.FromArgb(44, 62, 80), Color.FromArgb(220, 53, 69));
        }

        private void SetupHeaderButton(Button btn, Color normalColor, Color hoverColor)
        {
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatStyle = FlatStyle.Flat;
            btn.BackColor = normalColor;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;

            btn.MouseEnter += (s, e) => btn.BackColor = hoverColor;
            btn.MouseLeave += (s, e) => btn.BackColor = normalColor;
        }

        private void SetupMenuItems()
        {
            treeViewMenu.Nodes.Clear();
            treeViewMenu.DrawMode = TreeViewDrawMode.OwnerDrawText;
            treeViewMenu.ItemHeight = 35; // Cao hơn cho thoáng
            treeViewMenu.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            treeViewMenu.BackColor = Color.FromArgb(33, 37, 41); // Dark hơn
            treeViewMenu.ForeColor = Color.White;
            treeViewMenu.BorderStyle = BorderStyle.None;
            treeViewMenu.FullRowSelect = true;
            treeViewMenu.HideSelection = false;

            treeViewMenu.DrawNode += (s, e) =>
{
    e.DrawDefault = false;

    // Chiều rộng full của TreeView
    Rectangle nodeBounds = new Rectangle(0, e.Bounds.Top, treeViewMenu.Width, e.Bounds.Height);

    // Màu nền
    Color backColor;
    if (e.Node == treeViewMenu.SelectedNode)
        backColor = Color.FromArgb(52, 152, 219); // Active
    else if (nodeBounds.Contains(treeViewMenu.PointToClient(Cursor.Position)))
        backColor = Color.FromArgb(60, 72, 88); // Hover
    else
        backColor = treeViewMenu.BackColor;

    // Vẽ nền full chiều rộng
    using (SolidBrush brush = new SolidBrush(backColor))
        e.Graphics.FillRectangle(brush, nodeBounds);

    // Vẽ chữ (chừa khoảng trống icon + indent)
    int textOffset = e.Node.Level * treeViewMenu.Indent + 5;
    Rectangle textRect = new Rectangle(textOffset, e.Bounds.Top, treeViewMenu.Width - textOffset, e.Bounds.Height);

    TextRenderer.DrawText(
        e.Graphics,
        e.Node.Text,
        e.Node.NodeFont ?? treeViewMenu.Font,
        textRect,
        e.Node.ForeColor,
        TextFormatFlags.VerticalCenter | TextFormatFlags.Left
    );
};

            // === Menu items ===
            var businessNode = CreateMenuNode("💼 Kinh doanh", "business", Color.FromArgb(52, 152, 219));
            businessNode.Nodes.Add(CreateMenuNode("👥 Quản lý khách hàng", "customers", Color.LightGray));
            businessNode.Nodes.Add(CreateMenuNode("📋 Quản lý đơn hàng", "orders", Color.LightGray));

            var warehouseNode = CreateMenuNode("🏪 Kho hàng", "warehouse", Color.FromArgb(155, 89, 182));
            warehouseNode.Nodes.Add(CreateMenuNode("📦 Quản lý sản phẩm", "products", Color.LightGray));
            warehouseNode.Nodes.Add(CreateMenuNode("📊 Quản lý tồn kho", "inventory", Color.LightGray));

            var supplierNode = CreateMenuNode("🚚 Nhà cung cấp", "suppliers", Color.FromArgb(26, 188, 156));

            var reportNode = CreateMenuNode("📈 Báo cáo", "reports", Color.FromArgb(241, 196, 15));
            reportNode.Nodes.Add(CreateMenuNode("💰 Báo cáo doanh thu", "revenue_report", Color.LightGray));
            reportNode.Nodes.Add(CreateMenuNode("📊 Báo cáo tồn kho", "inventory_report", Color.LightGray));
            reportNode.Nodes.Add(CreateMenuNode("📋 Báo cáo đơn hàng", "order_report", Color.LightGray));

            var logoutNode = CreateMenuNode("🚪 Đăng xuất", "logout", Color.FromArgb(231, 76, 60));

            // Add nodes
            treeViewMenu.Nodes.Add(businessNode);
            treeViewMenu.Nodes.Add(warehouseNode);
            treeViewMenu.Nodes.Add(supplierNode);
            treeViewMenu.Nodes.Add(reportNode);
            treeViewMenu.Nodes.Add(logoutNode);

            treeViewMenu.ExpandAll();
            treeViewMenu.SelectedNode = businessNode.LastNode;
        }

        private TreeNode CreateMenuNode(string text, string tag, Color color)
        {
            var node = new TreeNode(text)
            {
                Tag = tag,
                ForeColor = color,
                NodeFont = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            if (tag == "logout")
                node.ToolTipText = "Đăng xuất khỏi hệ thống và quay về màn hình đăng nhập";

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
                    LoadOrderManagement(_currentEmployee);
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

        private void LoadOrderManagement(Nhanvien currentEmployee)
        {
            try
            {
                var orderForm = new OrderManagementForm(_context, currentEmployee);
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
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AceCook.Models;

namespace AceCook
{
    public partial class OrderManagementForm : Form
    {
        private AppDbContext _context;
        private DataGridView dataGridViewOrders;
        private TextBox txtSearch;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnRefresh;
        private Label lblTitle;

        public OrderManagementForm(AppDbContext context)
        {
            _context = context;
            SetupUI();
            LoadOrders();
        }

        private void SetupUI()
        {
            this.Text = "Quản lý Đơn hàng";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);

            // Title
            lblTitle = new Label
            {
                Text = "QUẢN LÝ ĐƠN HÀNG",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(300, 40),
                Location = new Point(20, 20)
            };

            // Search Panel
            var searchPanel = new Panel
            {
                Size = new Size(960, 60),
                Location = new Point(20, 70),
                BackColor = Color.White
            };

            var lblSearch = new Label
            {
                Text = "Tìm kiếm:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(80, 25),
                Location = new Point(20, 20)
            };

            txtSearch = new TextBox
            {
                Size = new Size(200, 25),
                Location = new Point(100, 18),
                Font = new Font("Segoe UI", 10)
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;

            btnRefresh = new Button
            {
                Text = "Làm mới",
                Size = new Size(80, 30),
                Location = new Point(320, 15),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += BtnRefresh_Click;

            searchPanel.Controls.Add(lblSearch);
            searchPanel.Controls.Add(txtSearch);
            searchPanel.Controls.Add(btnRefresh);

            // Button Panel
            var buttonPanel = new Panel
            {
                Size = new Size(960, 50),
                Location = new Point(20, 140),
                BackColor = Color.Transparent
            };

            btnAdd = new Button
            {
                Text = "Thêm mới",
                Size = new Size(100, 35),
                Location = new Point(0, 0),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button
            {
                Text = "Chỉnh sửa",
                Size = new Size(100, 35),
                Location = new Point(120, 0),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(241, 196, 15),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnEdit.FlatAppearance.BorderSize = 0;
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button
            {
                Text = "Xóa",
                Size = new Size(100, 35),
                Location = new Point(240, 0),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += BtnDelete_Click;

            buttonPanel.Controls.Add(btnAdd);
            buttonPanel.Controls.Add(btnEdit);
            buttonPanel.Controls.Add(btnDelete);

            // DataGridView
            dataGridViewOrders = new DataGridView
            {
                Size = new Size(960, 350),
                Location = new Point(20, 200),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                GridColor = Color.LightGray,
                RowHeadersVisible = false
            };

            // Add controls to form
            this.Controls.Add(lblTitle);
            this.Controls.Add(searchPanel);
            this.Controls.Add(buttonPanel);
            this.Controls.Add(dataGridViewOrders);

            // Add shadow effect to search panel
            searchPanel.Paint += (sender, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, searchPanel.ClientRectangle,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid);
            };
        }

        private async void LoadOrders()
        {
            try
            {
                var orders = await _context.Dondathangs
                    .Include(d => d.MaKhNavigation)
                    .Include(d => d.MaNvNavigation)
                    .ToListAsync();
                RefreshDataGridView(orders);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu đơn hàng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshDataGridView(System.Collections.Generic.List<Dondathang> orders)
        {
            dataGridViewOrders.DataSource = null;
            dataGridViewOrders.DataSource = orders;

            // Configure columns
            if (dataGridViewOrders.Columns.Count > 0)
            {
                dataGridViewOrders.Columns["MaDdh"].HeaderText = "Mã ĐH";
                dataGridViewOrders.Columns["NgayDat"].HeaderText = "Ngày Đặt";
                dataGridViewOrders.Columns["TrangThaiDh"].HeaderText = "Trạng Thái";
                dataGridViewOrders.Columns["MaKhNavigation.TenKh"].HeaderText = "Khách Hàng";
                dataGridViewOrders.Columns["MaNvNavigation.HoTenNv"].HeaderText = "Nhân Viên";

                // Format date and money columns
                if (dataGridViewOrders.Columns["NgayDat"] != null)
                {
                    dataGridViewOrders.Columns["NgayDat"].DefaultCellStyle.Format = "dd/MM/yyyy";
                }

                // Hide navigation properties
                dataGridViewOrders.Columns["MaKhNavigation"].Visible = false;
                dataGridViewOrders.Columns["MaNvNavigation"].Visible = false;
                dataGridViewOrders.Columns["CtDhs"].Visible = false;
            }
        }

        private async void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.ToLower();
            var filteredOrders = await _context.Dondathangs
                .Include(d => d.MaKhNavigation)
                .Include(d => d.MaNvNavigation)
                .Where(o => o.MaDdh.ToLower().Contains(searchText) ||
                           o.MaKhNavigation.TenKh.ToLower().Contains(searchText) ||
                           o.MaNvNavigation.HoTenNv.ToLower().Contains(searchText))
                .ToListAsync();

            RefreshDataGridView(filteredOrders);
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            LoadOrders();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng thêm đơn hàng sẽ được phát triển sau!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridViewOrders.SelectedRows.Count > 0)
            {
                var selectedOrder = dataGridViewOrders.SelectedRows[0].DataBoundItem as Dondathang;
                if (selectedOrder != null)
                {
                    MessageBox.Show($"Chỉnh sửa đơn hàng: {selectedOrder.MaDdh}", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một đơn hàng để chỉnh sửa", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void InitializeComponent()
        {

        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewOrders.SelectedRows.Count > 0)
            {
                var selectedOrder = dataGridViewOrders.SelectedRows[0].DataBoundItem as Dondathang;
                if (selectedOrder != null)
                {
                    var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa đơn hàng '{selectedOrder.MaDdh}'?",
                        "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            _context.Dondathangs.Remove(selectedOrder);
                            _context.SaveChanges();
                            MessageBox.Show("Xóa đơn hàng thành công!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadOrders();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Lỗi khi xóa đơn hàng: {ex.Message}", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một đơn hàng để xóa", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
} 
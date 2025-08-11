using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AceCook.Models;

namespace AceCook
{
    public partial class InventoryManagementForm : Form
    {
        private AppDbContext _context;
        private DataGridView dataGridViewInventory;
        private TextBox txtSearch;
        private Button btnRefresh;
        private Label lblTitle;

        public InventoryManagementForm(AppDbContext context)
        {
            _context = context;
            SetupUI();
            LoadInventory();
        }

        private void SetupUI()
        {
            this.Text = "Quản lý Tồn kho";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);

            // Title
            lblTitle = new Label
            {
                Text = "QUẢN LÝ TỒN KHO",
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

            // DataGridView
            dataGridViewInventory = new DataGridView
            {
                Size = new Size(960, 450),
                Location = new Point(20, 150),
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
            this.Controls.Add(dataGridViewInventory);

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

        private async void LoadInventory()
        {
            try
            {
                var inventory = await _context.CtTons
                    .Include(c => c.MaSpNavigation)
                    .Include(c => c.MaKhoNavigation)
                    .ToListAsync();
                RefreshDataGridView(inventory);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu tồn kho: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshDataGridView(System.Collections.Generic.List<CtTon> inventory)
        {
            dataGridViewInventory.DataSource = null;
            dataGridViewInventory.DataSource = inventory;

            // Configure columns
            if (dataGridViewInventory.Columns.Count > 0)
            {
                dataGridViewInventory.Columns["MaSpNavigation.TenSP"].HeaderText = "Tên Sản Phẩm";
                dataGridViewInventory.Columns["MaKhoNavigation.TenKho"].HeaderText = "Tên Kho";
                dataGridViewInventory.Columns["SoLuongTon"].HeaderText = "Số Lượng Tồn";
                dataGridViewInventory.Columns["NgayCapNhat"].HeaderText = "Ngày Cập Nhật";

                // Format date column
                if (dataGridViewInventory.Columns["NgayCapNhat"] != null)
                {
                    dataGridViewInventory.Columns["NgayCapNhat"].DefaultCellStyle.Format = "dd/MM/yyyy";
                }

                // Hide navigation properties
                dataGridViewInventory.Columns["MaSpNavigation"].Visible = false;
                dataGridViewInventory.Columns["MaKhoNavigation"].Visible = false;
            }
        }

        private async void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.ToLower();
            var filteredInventory = await _context.CtTons
                .Include(c => c.MaSpNavigation)
                .Include(c => c.MaKhoNavigation)
                .Where(i => i.MaSpNavigation.TenSp.ToLower().Contains(searchText) ||
                           i.MaKhoNavigation.TenKho.ToLower().Contains(searchText))
                .ToListAsync();

            RefreshDataGridView(filteredInventory);
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            LoadInventory();
        }
    }
} 
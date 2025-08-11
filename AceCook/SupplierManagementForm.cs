using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AceCook.Models;

namespace AceCook
{
    public partial class SupplierManagementForm : Form
    {
        private AppDbContext _context;
        private DataGridView dataGridViewSuppliers;
        private TextBox txtSearch;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnRefresh;
        private Label lblTitle;

        public SupplierManagementForm(AppDbContext context)
        {
            _context = context;
            SetupUI();
            LoadSuppliers();
        }

        private void SetupUI()
        {
            this.Text = "Quản lý Nhà cung cấp";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);

            // Title
            lblTitle = new Label
            {
                Text = "QUẢN LÝ NHÀ CUNG CẤP",
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
            dataGridViewSuppliers = new DataGridView
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
            this.Controls.Add(dataGridViewSuppliers);

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

        private async void LoadSuppliers()
        {
            try
            {
                var suppliers = await _context.Nhacungcaps.ToListAsync();
                RefreshDataGridView(suppliers);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu nhà cung cấp: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshDataGridView(System.Collections.Generic.List<Nhacungcap> suppliers)
        {
            dataGridViewSuppliers.DataSource = null;
            dataGridViewSuppliers.DataSource = suppliers;

            // Configure columns
            if (dataGridViewSuppliers.Columns.Count > 0)
            {
                dataGridViewSuppliers.Columns["MaNcc"].HeaderText = "Mã NCC";
                dataGridViewSuppliers.Columns["TenNcc"].HeaderText = "Tên Nhà Cung Cấp";
                dataGridViewSuppliers.Columns["DiaChiNcc"].HeaderText = "Địa Chỉ";
                dataGridViewSuppliers.Columns["Sdtncc"].HeaderText = "Số Điện Thoại";
                dataGridViewSuppliers.Columns["EmailNcc"].HeaderText = "Email";

                // Hide navigation properties
                dataGridViewSuppliers.Columns["Nguyenlieus"].Visible = false;
            }
        }

        private async void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.ToLower();
            var filteredSuppliers = await _context.Nhacungcaps
                .Where(s => s.TenNcc.ToLower().Contains(searchText) ||
                           s.MaNcc.ToLower().Contains(searchText) ||
                           s.Sdtncc.Contains(searchText))
                .ToListAsync();

            RefreshDataGridView(filteredSuppliers);
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            LoadSuppliers();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng thêm nhà cung cấp sẽ được phát triển sau!", "Thông báo", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridViewSuppliers.SelectedRows.Count > 0)
            {
                var selectedSupplier = dataGridViewSuppliers.SelectedRows[0].DataBoundItem as Nhacungcap;
                if (selectedSupplier != null)
                {
                    MessageBox.Show($"Chỉnh sửa nhà cung cấp: {selectedSupplier.TenNcc}", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một nhà cung cấp để chỉnh sửa", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewSuppliers.SelectedRows.Count > 0)
            {
                var selectedSupplier = dataGridViewSuppliers.SelectedRows[0].DataBoundItem as Nhacungcap;
                if (selectedSupplier != null)
                {
                    var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa nhà cung cấp '{selectedSupplier.TenNcc}'?", 
                        "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    
                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            _context.Nhacungcaps.Remove(selectedSupplier);
                            _context.SaveChanges();
                            MessageBox.Show("Xóa nhà cung cấp thành công!", "Thông báo", 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadSuppliers();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Lỗi khi xóa nhà cung cấp: {ex.Message}", "Lỗi", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một nhà cung cấp để xóa", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
} 
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using AceCook.Models;
using AceCook.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AceCook
{
    public partial class SupplierForm : Form
    {
        private readonly AppDbContext _context;
        private readonly SupplierRepository _supplierRepository;
        private DataGridView dataGridViewSuppliers;
        private TextBox txtSearch;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnRefresh;
        private Button btnClearFilter;
        private Label lblTotalSuppliers;
        private Panel pnlSummary;
        private List<Nhacungcap> _suppliers;

        public SupplierForm(AppDbContext context)
        {
            _context = context;
            _supplierRepository = new SupplierRepository(context);
            _suppliers = new List<Nhacungcap>();
            InitializeComponent();
            SetupUI();
            LoadDataAsync();
        }

        private void SetupUI()
        {
            // Form settings
            this.Text = "Quản lý Nhà cung cấp";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);

            // Title
            var lblTitle = new Label
            {
                Text = "QUẢN LÝ NHÀ CUNG CẤP",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(400, 50),
                Location = new Point(30, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Summary Panel
            pnlSummary = new Panel
            {
                Size = new Size(1140, 60),  // Giảm chiều cao
                Location = new Point(30, 80), // Di chuyển lên trên
                BackColor = Color.White,
                BorderStyle = BorderStyle.None // Bỏ border
            };

            // Tạo container cho thông tin tổng quan
            var summaryContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                Padding = new Padding(10),
                BackColor = Color.White
            };

            // Thông tin tổng số nhà cung cấp
            var totalPanel = new Panel
            {
                BackColor = Color.FromArgb(52, 152, 219),
                Dock = DockStyle.Fill,
                Margin = new Padding(5)
            };

            var lblTotalTitle = new Label
            {
                Text = "Tổng số nhà cung cấp",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Height = 25
            };

            lblTotalSuppliers = new Label
            {
                Text = "0",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            totalPanel.Controls.AddRange(new Control[] { lblTotalTitle, lblTotalSuppliers });
            summaryContainer.Controls.Add(totalPanel, 0, 0);

            pnlSummary.Controls.Add(summaryContainer);

            // Search Panel
            var searchPanel = new Panel
            {
                Size = new Size(1140, 50),  // Giảm chiều cao
                Location = new Point(30, 150), // Điều chỉnh vị trí
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            txtSearch = new TextBox
            {
                Size = new Size(300, 30),
                Location = new Point(10, 10),
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "Tìm kiếm theo mã, tên, số điện thoại..."
            };

            btnClearFilter = new Button
            {
                Text = "Xóa bộ lọc",
                Size = new Size(100, 30),
                Location = new Point(320, 10),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClearFilter.FlatAppearance.BorderSize = 0;

            btnRefresh = new Button
            {
                Text = "Làm mới",
                Size = new Size(100, 30),
                Location = new Point(430, 10),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;

            searchPanel.Controls.AddRange(new Control[] { txtSearch, btnClearFilter, btnRefresh });

            // Action Buttons Panel
            var actionPanel = new Panel
            {
                Size = new Size(1140, 50),
                Location = new Point(30, 210),
                BackColor = Color.Transparent
            };

            btnAdd = new Button
            {
                Text = "Thêm mới",
                Size = new Size(120, 35),
                Location = new Point(0, 7),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAdd.FlatAppearance.BorderSize = 0;

            btnEdit = new Button
            {
                Text = "Chỉnh sửa",
                Size = new Size(100, 35),
                Location = new Point(130, 7),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(241, 196, 15),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnEdit.FlatAppearance.BorderSize = 0;

            btnDelete = new Button
            {
                Text = "Xóa",
                Size = new Size(100, 35),
                Location = new Point(240, 7),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnDelete.FlatAppearance.BorderSize = 0;

            actionPanel.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete });

            // DataGridView
            dataGridViewSuppliers = new DataGridView
            {
                Size = new Size(1140, 450), // Tăng chiều cao
                Location = new Point(30, 270),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ColumnHeadersHeight = 40,
                RowTemplate = { Height = 40 }
            };

<<<<<<< HEAD
            // Style cho DataGridView
            dataGridViewSuppliers.DefaultCellStyle.Font = new Font("Segoe UI", 9);
=======
            // Style DataGridView
            dataGridViewSuppliers.DefaultCellStyle.Font = new Font("Segoe UI", 10);
>>>>>>> ffd30000a4e2f58a81e734a80d238e353f39a246
            dataGridViewSuppliers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridViewSuppliers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dataGridViewSuppliers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridViewSuppliers.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            dataGridViewSuppliers.DefaultCellStyle.SelectionForeColor = Color.White;
            dataGridViewSuppliers.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewSuppliers.GridColor = Color.FromArgb(224, 224, 224);

            // Add controls to form
            this.Controls.AddRange(new Control[] { 
                lblTitle,
                pnlSummary,
                searchPanel,
                actionPanel,
                dataGridViewSuppliers
            });
        }
        //bản ổn nhất nhà cung cấp

        private async Task LoadDataAsync()
        {
            try
            {
                _suppliers = await _supplierRepository.GetAllSuppliersAsync();
                RefreshDataGridView(_suppliers);
                UpdateSummary(_suppliers);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu nhà cung cấp: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshDataGridView(List<Nhacungcap> suppliers)
        {
            dataGridViewSuppliers.DataSource = null;
            dataGridViewSuppliers.Columns.Clear();

            // Create custom columns
            dataGridViewSuppliers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MaNcc",
                HeaderText = "Mã NCC",
                Width = 100
            });

            dataGridViewSuppliers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenNcc",
                HeaderText = "Tên nhà cung cấp",
                Width = 200
            });

            dataGridViewSuppliers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Sdtncc",
                HeaderText = "Số điện thoại",
                Width = 120
            });

            dataGridViewSuppliers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "EmailNcc",
                HeaderText = "Email",
                Width = 200
            });

            dataGridViewSuppliers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DiaChiNcc",
                HeaderText = "Địa chỉ",
                Width = 300
            });

            // Populate data
            foreach (var supplier in suppliers)
            {
                var rowIndex = dataGridViewSuppliers.Rows.Add();
                var row = dataGridViewSuppliers.Rows[rowIndex];

                row.Cells["MaNcc"].Value = supplier.MaNcc;
                row.Cells["TenNcc"].Value = supplier.TenNcc ?? "N/A";
                row.Cells["Sdtncc"].Value = supplier.Sdtncc ?? "N/A";
                row.Cells["EmailNcc"].Value = supplier.EmailNcc ?? "N/A";
                row.Cells["DiaChiNcc"].Value = supplier.DiaChiNcc ?? "N/A";
            }
        }

        private void UpdateSummary(List<Nhacungcap> suppliers)
        {
            lblTotalSuppliers.Text = suppliers.Count.ToString("N0");
            
            // Count active suppliers (those with materials)
            var activeSuppliers = suppliers.Count(s => s.Nguyenlieus?.Count > 0);
            var activeLabel = pnlSummary.Controls.OfType<Label>().FirstOrDefault(l => l.Text == "0");
            if (activeLabel != null)
            {
                activeLabel.Text = activeSuppliers.ToString("N0");
            }
        }

        private async void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            await ApplySearch();
        }

        private async Task ApplySearch()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    RefreshDataGridView(_suppliers);
                    UpdateSummary(_suppliers);
                    return;
                }

                var searchTerm = txtSearch.Text.Trim();
                var filteredSuppliers = await _supplierRepository.SearchSuppliersAsync(searchTerm);
                RefreshDataGridView(filteredSuppliers);
                UpdateSummary(filteredSuppliers);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClearFilter_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            RefreshDataGridView(_suppliers);
            UpdateSummary(_suppliers);
        }

        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            await LoadDataAsync();
        }

        private async void BtnAdd_Click(object sender, EventArgs e)
        {
            try 
            {
                using (var addForm = new SupplierAddEditForm(_context, null))
                {
                    var dialogResult = addForm.ShowDialog();
                    if (dialogResult == DialogResult.OK)
                    {
                        var newSupplier = addForm.Supplier;
                        
                        // Kiểm tra dữ liệu đầu vào
                        if (string.IsNullOrWhiteSpace(newSupplier.MaNcc))
                        {
                            MessageBox.Show("Mã nhà cung cấp không được để trống!", "Lỗi", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Kiểm tra trùng mã
                        var existingSupplier = _suppliers.FirstOrDefault(s => s.MaNcc == newSupplier.MaNcc);
                        if (existingSupplier != null)
                        {
                            MessageBox.Show("Mã nhà cung cấp đã tồn tại!", "Lỗi", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Chuẩn bị dữ liệu
                        newSupplier.TenNcc = newSupplier.TenNcc ?? "";
                        newSupplier.Sdtncc = newSupplier.Sdtncc ?? "";
                        newSupplier.EmailNcc = newSupplier.EmailNcc ?? "";
                        newSupplier.DiaChiNcc = newSupplier.DiaChiNcc ?? "";
                        
                        // Thêm nhà cung cấp
                        bool success = await _supplierRepository.AddSupplierAsync(newSupplier);
                        
                        if (success)
                        {
                            await LoadDataAsync(); // Cập nhật dữ liệu trước khi hiển thị thông báo
                            MessageBox.Show("Thêm nhà cung cấp thành công!", "Thông báo", 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm nhà cung cấp: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridViewSuppliers.SelectedRows.Count > 0)
            {
                try
                {
                    string maNcc = dataGridViewSuppliers.SelectedRows[0].Cells["MaNcc"].Value.ToString();
                    
                    // Lấy supplier trực tiếp từ database
                    var existingSupplier = await _context.Nhacungcaps
                        .AsNoTracking() // Quan trọng: Không track entity này
                        .FirstOrDefaultAsync(s => s.MaNcc == maNcc);

                    if (existingSupplier != null)
                    {
                        // Tạo bản sao để edit
                        var supplierCopy = new Nhacungcap
                        {
                            MaNcc = existingSupplier.MaNcc,
                            TenNcc = existingSupplier.TenNcc,
                            Sdtncc = existingSupplier.Sdtncc,
                            EmailNcc = existingSupplier.EmailNcc,
                            DiaChiNcc = existingSupplier.DiaChiNcc
                        };

                        var editForm = new SupplierAddEditForm(_context, supplierCopy);
                        if (editForm.ShowDialog() == DialogResult.OK)
                        {
                            try
                            {
                                var updatedSupplier = editForm.Supplier;
                                bool success = await _supplierRepository.UpdateSupplierAsync(updatedSupplier);
                                
                                if (success)
                                {
                                    await LoadDataAsync();
                                    MessageBox.Show("Cập nhật nhà cung cấp thành công!", "Thông báo", 
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                            catch (DbUpdateException dbEx)
                            {
                                MessageBox.Show($"Lỗi cập nhật database: {dbEx.InnerException?.Message ?? dbEx.Message}", 
                                    "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Lỗi khi cập nhật: {ex.Message}", "Lỗi", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi không mong muốn: {ex.Message}", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một nhà cung cấp để chỉnh sửa", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewSuppliers.SelectedRows.Count > 0)
            {
                // Lấy dữ liệu từ row được chọn
                string maNcc = dataGridViewSuppliers.SelectedRows[0].Cells["MaNcc"].Value.ToString();
                var selectedSupplier = _suppliers.FirstOrDefault(s => s.MaNcc == maNcc);

                if (selectedSupplier != null)
                {
                    // Check if supplier has materials
                    if (selectedSupplier.Nguyenlieus?.Count > 0)
                    {
                        MessageBox.Show($"Không thể xóa nhà cung cấp '{selectedSupplier.TenNcc}' vì đang cung cấp {selectedSupplier.Nguyenlieus.Count} nguyên liệu!", 
                            "Không thể xóa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa nhà cung cấp '{selectedSupplier.TenNcc}'?", 
                        "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    
                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            bool success = await _supplierRepository.DeleteSupplierAsync(selectedSupplier.MaNcc);
                            if (success)
                            {
                                MessageBox.Show("Xóa nhà cung cấp thành công!", "Thông báo", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                await LoadDataAsync();
                            }
                            else
                            {
                                MessageBox.Show("Lỗi khi xóa nhà cung cấp!", "Lỗi", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
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

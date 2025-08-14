using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using AceCook.Models;
using AceCook.Repositories;

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
                Size = new Size(1140, 80),
                Location = new Point(30, 90),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblTotalSuppliersTitle = new Label
            {
                Text = "Tổng số nhà cung cấp:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(180, 25),
                Location = new Point(20, 15),
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblTotalSuppliers = new Label
            {
                Text = "0",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 152, 219),
                Size = new Size(100, 25),
                Location = new Point(210, 15),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lblActiveSuppliersTitle = new Label
            {
                Text = "Nhà cung cấp đang hoạt động:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(200, 25),
                Location = new Point(20, 45),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lblActiveSuppliers = new Label
            {
                Text = "0",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113),
                Size = new Size(100, 25),
                Location = new Point(230, 45),
                TextAlign = ContentAlignment.MiddleLeft
            };

            pnlSummary.Controls.AddRange(new Control[] { 
                lblTotalSuppliersTitle, lblTotalSuppliers,
                lblActiveSuppliersTitle, lblActiveSuppliers
            });

            // Search Panel
            var searchPanel = new Panel
            {
                Size = new Size(1140, 80),
                Location = new Point(30, 190),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblSearch = new Label
            {
                Text = "Tìm kiếm:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(80, 25),
                Location = new Point(20, 15),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtSearch = new TextBox
            {
                Size = new Size(250, 30),
                Location = new Point(110, 12),
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "Mã NCC, tên NCC, SĐT..."
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;

            btnClearFilter = new Button
            {
                Text = "🔄 Xóa bộ lọc",
                Size = new Size(100, 35),
                Location = new Point(380, 12),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClearFilter.FlatAppearance.BorderSize = 0;
            btnClearFilter.Click += BtnClearFilter_Click;

            btnRefresh = new Button
            {
                Text = "🔄 Làm mới",
                Size = new Size(100, 35),
                Location = new Point(500, 12),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += BtnRefresh_Click;

            searchPanel.Controls.AddRange(new Control[] { 
                lblSearch, txtSearch, btnClearFilter, btnRefresh 
            });

            // Actions Panel
            var pnlActions = new Panel
            {
                Size = new Size(1140, 60),
                Location = new Point(30, 290),
                BackColor = Color.Transparent
            };

            btnAdd = new Button
            {
                Text = "➕ Thêm nhà cung cấp mới",
                Size = new Size(250, 60),
                Location = new Point(0, 0),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button
            {
                Text = "Chỉnh sửa",
                Size = new Size(100, 35),
                Location = new Point(270, 10),
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
                Location = new Point(410, 10),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += BtnDelete_Click;

            pnlActions.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete });

            // DataGridView
            dataGridViewSuppliers = new DataGridView
            {
                Size = new Size(1140, 350),
                Location = new Point(30, 370),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                GridColor = Color.LightGray,
                RowHeadersVisible = false,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersHeight = 50,
                RowTemplate = { Height = 45 }
            };

            // Style the DataGridView
            dataGridViewSuppliers.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dataGridViewSuppliers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridViewSuppliers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dataGridViewSuppliers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridViewSuppliers.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewSuppliers.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            dataGridViewSuppliers.DefaultCellStyle.SelectionForeColor = Color.White;

            // Add controls to form
            this.Controls.AddRange(new Control[] { 
                lblTitle, pnlSummary, searchPanel, pnlActions, dataGridViewSuppliers 
            });
        }

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

            dataGridViewSuppliers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SoNguyenLieu",
                HeaderText = "Số nguyên liệu",
                Width = 120
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
                row.Cells["SoNguyenLieu"].Value = supplier.Nguyenlieus?.Count ?? 0;
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
                // Debug Point 1: Bắt đầu thêm mới
                Console.WriteLine("Debug: Bắt đầu thêm nhà cung cấp mới");
                
                using (var addForm = new SupplierAddEditForm(_context, null))
                {
                    if (addForm.ShowDialog() == DialogResult.OK)
                    {
                        // Debug Point 2: Lấy dữ liệu từ form
                        var newSupplier = addForm.Supplier;
                        Console.WriteLine($"Debug: Thông tin nhà cung cấp mới:");
                        Console.WriteLine($"- Mã NCC: {newSupplier.MaNcc}");
                        Console.WriteLine($"- Tên NCC: {newSupplier.TenNcc}");
                        Console.WriteLine($"- SĐT: {newSupplier.Sdtncc}");
                        Console.WriteLine($"- Email: {newSupplier.EmailNcc}");
                        Console.WriteLine($"- Địa chỉ: {newSupplier.DiaChiNcc}");

                        // Kiểm tra dữ liệu
                        if (string.IsNullOrWhiteSpace(newSupplier.MaNcc))
                        {
                            MessageBox.Show("Mã nhà cung cấp không được để trống!", "Lỗi", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Debug Point 3: Kiểm tra trùng mã
                        var existingSupplier = _suppliers.FirstOrDefault(s => s.MaNcc == newSupplier.MaNcc);
                        if (existingSupplier != null)
                        {
                            Console.WriteLine($"Debug: Phát hiện mã NCC đã tồn tại: {newSupplier.MaNcc}");
                            MessageBox.Show("Mã nhà cung cấp đã tồn tại!", "Lỗi", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        try
                        {
                            // Debug Point 4: Chuẩn bị dữ liệu trước khi thêm
                            newSupplier.TenNcc = newSupplier.TenNcc ?? "";
                            newSupplier.Sdtncc = newSupplier.Sdtncc ?? "";
                            newSupplier.EmailNcc = newSupplier.EmailNcc ?? "";
                            newSupplier.DiaChiNcc = newSupplier.DiaChiNcc ?? "";
                            
                            Console.WriteLine("Debug: Bắt đầu gọi AddSupplierAsync");
                            bool success = await _supplierRepository.AddSupplierAsync(newSupplier);
                            
                            // Debug Point 5: Kết quả thêm mới
                            Console.WriteLine($"Debug: Kết quả thêm mới: {success}");

                            if (success)
                            {
                                MessageBox.Show("Thêm nhà cung cấp thành công!", "Thông báo", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                await LoadDataAsync();
                            }
                            else
                            {
                                MessageBox.Show("Lỗi khi thêm nhà cung cấp! Vui lòng kiểm tra lại thông tin.", 
                                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Debug Point 6: Bắt lỗi khi thêm
                            Console.WriteLine($"Debug: Lỗi trong quá trình thêm: {ex.Message}");
                            Console.WriteLine($"Debug: Stack trace: {ex.StackTrace}");
                            MessageBox.Show($"Lỗi khi thêm nhà cung cấp: {ex.Message}", "Lỗi", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Debug Point 7: Bắt lỗi tổng thể
                Console.WriteLine($"Debug: Lỗi không mong muốn: {ex.Message}");
                Console.WriteLine($"Debug: Stack trace: {ex.StackTrace}");
                MessageBox.Show($"Lỗi không mong muốn: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnEdit_Click(object sender, EventArgs e)
        {
            // Thay đổi cách lấy dữ liệu được chọn
            if (dataGridViewSuppliers.SelectedRows.Count > 0)
            {
                // Lấy dữ liệu từ row được chọn
                string maNcc = dataGridViewSuppliers.SelectedRows[0].Cells["MaNcc"].Value.ToString();
                var selectedSupplier = _suppliers.FirstOrDefault(s => s.MaNcc == maNcc);
                
                if (selectedSupplier != null)
                {
                    var editForm = new SupplierAddEditForm(_context, selectedSupplier);
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            bool success = await _supplierRepository.UpdateSupplierAsync(editForm.Supplier);
                            if (success)
                            {
                                MessageBox.Show("Cập nhật nhà cung cấp thành công!", "Thông báo", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                await LoadDataAsync();
                            }
                            else
                            {
                                MessageBox.Show("Lỗi khi cập nhật nhà cung cấp!", "Lỗi", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Lỗi khi cập nhật nhà cung cấp: {ex.Message}", "Lỗi", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
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

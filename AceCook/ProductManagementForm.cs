using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AceCook.Models;
using AceCook.Repositories;

namespace AceCook
{
    public partial class ProductManagementForm : Form
    {
        private ProductRepository _productRepository;
        private DataGridView dataGridViewProducts;
        private TextBox txtSearch;
        private ComboBox cboCategory;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnRefresh;
        private Button btnClearFilter;
        private Label lblTitle;
        private System.Collections.Generic.List<Sanpham> _products;

        public ProductManagementForm(AppDbContext context)
        {
            _productRepository = new ProductRepository(context);
            InitializeComponent();
            SetupUI();
            _ = LoadDataAsync(); // Sử dụng async method để load dữ liệu
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.Text = "Quản lý Sản phẩm";
            this.Size = new Size(1400, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            
            this.ResumeLayout(false);
        }

        private void SetupUI()
        {
            // Title
            lblTitle = new Label
            {
                Text = "QUẢN LÝ SẢN PHẨM",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Dock = DockStyle.Top,
                Height = 70,
                TextAlign = ContentAlignment.MiddleLeft,
            };

            // Search Panel
            var pnlSearch = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15),
                FlowDirection = FlowDirection.LeftToRight,
                AutoScroll = true,
                WrapContents = false,
                Margin = new Padding(0, 200, 0, 150)
            };

            var lblSearch = new Label
            {
                Text = "Tìm kiếm:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 8, 10, 0)
            };

            txtSearch = new TextBox
            {
                Width = 250,
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "Mã SP, tên SP, mô tả...",
                Margin = new Padding(0, 5, 20, 0)
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;

            var lblCategory = new Label
            {
                Text = "Loại:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 8, 10, 0)
            };

            cboCategory = new ComboBox
            {
                Width = 150,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 5, 20, 0)
            };
            cboCategory.SelectedIndexChanged += CboCategory_SelectedIndexChanged;

            btnClearFilter = new Button
            {
                Text = "🔄 Xóa bộ lọc",
                Width = 120,
                Height = 35,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 5, 10, 0)
            };
            btnClearFilter.FlatAppearance.BorderSize = 0;
            btnClearFilter.Click += BtnClearFilter_Click;

            btnRefresh = new Button
            {
                Text = "🔄 Làm mới",
                Width = 100,
                Height = 35,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 5, 10, 0)
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += BtnRefresh_Click;

            pnlSearch.Controls.AddRange(new Control[] {
                lblSearch, txtSearch, lblCategory, cboCategory, btnClearFilter, btnRefresh
            });

            // Actions Panel
            var pnlActions = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 70,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(15),
                BackColor = Color.Transparent
            };

            btnAdd = CreateActionButton("➕ Thêm sản phẩm mới", Color.FromArgb(46, 204, 113));
            btnAdd.Click += BtnAdd_Click;

            btnRefresh = CreateActionButton("🔄 Làm mới dữ liệu", Color.FromArgb(52, 152, 219));
            btnRefresh.Click += BtnRefresh_Click;

            btnEdit = CreateActionButton("✏️ Chỉnh sửa sản phẩm", Color.FromArgb(255, 193, 7));
            btnEdit.Click += BtnEdit_Click;

            btnDelete = CreateActionButton("🗑️ Xóa sản phẩm", Color.FromArgb(231, 76, 60));
            btnDelete.Click += BtnDelete_Click;

            pnlActions.Controls.AddRange(new Control[] { btnAdd, btnRefresh, btnEdit, btnDelete });

            // DataGridView
            dataGridViewProducts = new DataGridView
            {
                Dock = DockStyle.Fill,
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
                RowTemplate = { Height = 50 }
            };

            // Style the DataGridView
            dataGridViewProducts.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dataGridViewProducts.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridViewProducts.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dataGridViewProducts.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridViewProducts.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewProducts.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            dataGridViewProducts.DefaultCellStyle.SelectionForeColor = Color.White;

            // Add all to form
            this.Controls.AddRange(new Control[] { dataGridViewProducts, pnlActions, pnlSearch, lblTitle });
        }

        // Helper to create buttons
        private Button CreateActionButton(string text, Color backColor)
        {
            var btn = new Button
            {
                Text = text,
                Width = 250,
                Height = 60,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 0, 15, 0),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private async Task LoadDataAsync()
        {
            try
            {
                // Load products first
                _products = await _productRepository.GetAllProductsAsync();
                RefreshDataGridView(_products);
                
                // Then load categories
                await LoadCategories();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadCategories()
        {
            try
            {
                var categories = await _productRepository.GetProductCategoriesAsync();
                
                // Update UI trên main thread
                if (this.InvokeRequired)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        UpdateCategoryComboBox(categories);
                    });
                }
                else
                {
                    UpdateCategoryComboBox(categories);
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách loại sản phẩm: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateCategoryComboBox(List<string> categories)
        {
            cboCategory.Items.Clear();
            cboCategory.Items.Add("Tất cả");
            if (categories != null)
            {
                foreach (var category in categories)
                {
                    if (!string.IsNullOrEmpty(category))
                    {
                        cboCategory.Items.Add(category);
                    }
                }
            }
            cboCategory.SelectedIndex = 0;
        }

        private async Task LoadProducts()
        {
            try
            {
                _products = await _productRepository.GetAllProductsAsync();
                RefreshDataGridView(_products);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách sản phẩm: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshDataGridView(List<Sanpham> products)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    RefreshDataGridView(products);
                });
                return;
            }

            try
            {
                dataGridViewProducts.DataSource = null;
                dataGridViewProducts.Columns.Clear();

                // Create custom columns
                dataGridViewProducts.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "MaSp",
                    HeaderText = "Mã SP",
                    Width = 100
                });

                dataGridViewProducts.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TenSp",
                    HeaderText = "Tên Sản Phẩm",
                    Width = 200
                });

                dataGridViewProducts.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "MoTa",
                    HeaderText = "Mô Tả",
                    Width = 250
                });

                dataGridViewProducts.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Gia",
                    HeaderText = "Giá",
                    Width = 120
                });

                dataGridViewProducts.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "DVTSP",
                    HeaderText = "Đơn Vị",
                    Width = 100
                });

                dataGridViewProducts.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Loai",
                    HeaderText = "Loại",
                    Width = 120
                });

                dataGridViewProducts.Columns.Add(new DataGridViewButtonColumn
                {
                    Name = "ViewDetails",
                    HeaderText = "Xem chi tiết",
                    Width = 100,
                    Text = "👁️ Xem",
                    UseColumnTextForButtonValue = true
                });

                // Populate data
                if (products != null && products.Count > 0)
                {
                    foreach (var product in products)
                    {
                        try
                        {
                            if (product != null)
                            {
                                var rowIndex = dataGridViewProducts.Rows.Add();
                                var row = dataGridViewProducts.Rows[rowIndex];

                                row.Cells["MaSp"].Value = product.MaSp ?? "N/A";
                                row.Cells["TenSp"].Value = product.TenSp ?? "N/A";
                                row.Cells["MoTa"].Value = product.MoTa ?? "N/A";
                                row.Cells["Gia"].Value = (product.Gia ?? 0).ToString("N0") + " VNĐ";
                                row.Cells["DVTSP"].Value = product.Dvtsp ?? "N/A";
                                row.Cells["Loai"].Value = product.Loai ?? "N/A";
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error processing product {product?.MaSp}: {ex.Message}");
                            // Bỏ qua dòng lỗi và tiếp tục
                        }
                    }
                }

                // Handle button clicks - chỉ đăng ký một lần
                dataGridViewProducts.CellClick -= DataGridViewProducts_CellClick;
                dataGridViewProducts.CellClick += DataGridViewProducts_CellClick;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in RefreshDataGridView: {ex.Message}");
                MessageBox.Show($"Lỗi khi refresh DataGridView: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void DataGridViewProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dataGridViewProducts.Columns["ViewDetails"].Index)
            {
                try
                {
                    var productId = dataGridViewProducts.Rows[e.RowIndex].Cells["MaSp"].Value?.ToString();
                    if (!string.IsNullOrEmpty(productId))
                    {
                        var product = await _productRepository.GetProductByIdAsync(productId);
                        if (product != null)
                        {
                            ViewProductDetails(product);
                        }
                        else
                        {
                            MessageBox.Show("Không thể tải thông tin sản phẩm!", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không thể lấy mã sản phẩm!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xem chi tiết: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ViewProductDetails(Sanpham product)
        {
            try
            {
                if (product == null)
                {
                    MessageBox.Show("Không có thông tin sản phẩm để hiển thị!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var viewForm = new ProductAddEditForm(_productRepository, FormMode.View, product);
                viewForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form xem chi tiết: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private System.Windows.Forms.Timer _searchTimer;
        private bool _isSearching = false;

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                // Tạo timer để tránh tìm kiếm quá nhiều lần
                if (_searchTimer == null)
                {
                    _searchTimer = new System.Windows.Forms.Timer();
                    _searchTimer.Interval = 500; // Delay 500ms
                    _searchTimer.Tick += async (s, args) =>
                    {
                        _searchTimer.Stop();
                        if (!_isSearching)
                        {
                            await ApplyFilters();
                        }
                    };
                }
                
                // Reset timer
                _searchTimer.Stop();
                _searchTimer.Start();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in TxtSearch_TextChanged: {ex.Message}");
            }
        }

        private void CboCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                _ = ApplyFilters();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in CboCategory_SelectedIndexChanged: {ex.Message}");
            }
        }

        private async Task ApplyFilters()
        {
            if (_isSearching) return;
            
            try
            {
                _isSearching = true;
                string searchText = txtSearch.Text.Trim().ToLower();
                string selectedCategory = cboCategory.SelectedItem?.ToString();

                List<Sanpham> filteredProducts;

                if (!string.IsNullOrEmpty(searchText))
                {
                    filteredProducts = await _productRepository.SearchProductsAsync(searchText);
                }
                else
                {
                    filteredProducts = _products ?? new List<Sanpham>();
                }

                if (selectedCategory != "Tất cả" && !string.IsNullOrEmpty(selectedCategory))
                {
                    filteredProducts = filteredProducts.Where(p => p.Loai == selectedCategory).ToList();
                }

                RefreshDataGridView(filteredProducts);
                
                // Update title với số lượng kết quả
                var resultCount = filteredProducts.Count;
                this.Text = $"Quản lý Sản phẩm - Hiển thị {resultCount} sản phẩm";
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ApplyFilters: {ex.Message}");
                MessageBox.Show($"Lỗi khi áp dụng bộ lọc: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isSearching = false;
            }
        }

        private void BtnClearFilter_Click(object sender, EventArgs e)
        {
            try
            {
                txtSearch.Clear();
                cboCategory.SelectedIndex = 0;
                _ = ApplyFilters(); // Apply filters after clearing
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in BtnClearFilter_Click: {ex.Message}");
            }
        }

        private async void BtnViewDetails_Click(object sender, EventArgs e)
        {
            if (dataGridViewProducts.SelectedRows.Count > 0)
            {
                try
                {
                    // Get the selected product ID from the first column
                    var selectedRow = dataGridViewProducts.SelectedRows[0];
                    var productId = selectedRow.Cells["MaSp"].Value?.ToString();
                    
                    if (!string.IsNullOrEmpty(productId))
                    {
                        var product = await _productRepository.GetProductByIdAsync(productId);
                        if (product != null)
                        {
                            ViewProductDetails(product);
                        }
                        else
                        {
                            MessageBox.Show("Không thể tải thông tin sản phẩm!", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không thể lấy thông tin sản phẩm đã chọn!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi tải thông tin sản phẩm: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm để xem chi tiết!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi làm mới dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var addForm = new ProductAddEditForm(_productRepository, FormMode.Add);
                if (addForm.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("Thêm sản phẩm thành công!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadDataAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form thêm sản phẩm: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridViewProducts.SelectedRows.Count > 0)
            {
                try
                {
                    var selectedRow = dataGridViewProducts.SelectedRows[0];
                    var productId = selectedRow.Cells["MaSp"].Value?.ToString();
                    
                    if (!string.IsNullOrEmpty(productId))
                    {
                        var product = await _productRepository.GetProductByIdAsync(productId);
                        if (product != null)
                        {
                            var editForm = new ProductAddEditForm(_productRepository, FormMode.Edit, product);
                            if (editForm.ShowDialog() == DialogResult.OK)
                            {
                                MessageBox.Show("Cập nhật sản phẩm thành công!", "Thông báo", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                await LoadDataAsync();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Không thể tải thông tin sản phẩm!", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không thể lấy thông tin sản phẩm đã chọn!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi tải thông tin sản phẩm: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm để chỉnh sửa", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewProducts.SelectedRows.Count > 0)
            {
                try
                {
                    var selectedRow = dataGridViewProducts.SelectedRows[0];
                    var productId = selectedRow.Cells["MaSp"].Value?.ToString();
                    
                    if (!string.IsNullOrEmpty(productId))
                    {
                        var product = await _productRepository.GetProductByIdAsync(productId);
                        if (product != null)
                        {
                            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa sản phẩm '{product.TenSp}'?", 
                                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            
                            if (result == DialogResult.Yes)
                            {
                                try
                                {
                                    bool success = await _productRepository.DeleteProductAsync(productId);
                                    if (success)
                                    {
                                        MessageBox.Show("Xóa sản phẩm thành công!", "Thông báo", 
                                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        await LoadDataAsync();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Lỗi khi xóa sản phẩm!", "Lỗi", 
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                                catch (InvalidOperationException ex)
                                {
                                    MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show($"Lỗi khi xóa sản phẩm: {ex.Message}", "Lỗi", 
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Không thể tải thông tin sản phẩm để xóa!", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không thể lấy thông tin sản phẩm đã chọn!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi tải thông tin sản phẩm: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm để xóa", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    _searchTimer?.Stop();
                    _searchTimer?.Dispose();
                    _searchTimer = null;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error disposing search timer: {ex.Message}");
                }
            }
            base.Dispose(disposing);
        }
    }
} 
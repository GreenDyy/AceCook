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
            SetupUI();
            LoadProducts();
            LoadCategories();
        }

        private void SetupUI()
        {
            this.Text = "Quản lý Sản phẩm";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);

            // Title
            lblTitle = new Label
            {
                Text = "QUẢN LÝ SẢN PHẨM",
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

            var lblCategory = new Label
            {
                Text = "Loại:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(40, 25),
                Location = new Point(320, 20)
            };

            cboCategory = new ComboBox
            {
                Size = new Size(150, 25),
                Location = new Point(360, 18),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboCategory.SelectedIndexChanged += CboCategory_SelectedIndexChanged;

            btnClearFilter = new Button
            {
                Text = "Xóa bộ lọc",
                Size = new Size(80, 30),
                Location = new Point(530, 15),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnClearFilter.FlatAppearance.BorderSize = 0;
            btnClearFilter.Click += BtnClearFilter_Click;

            btnRefresh = new Button
            {
                Text = "Làm mới",
                Size = new Size(80, 30),
                Location = new Point(630, 15),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += BtnRefresh_Click;

            searchPanel.Controls.Add(lblSearch);
            searchPanel.Controls.Add(txtSearch);
            searchPanel.Controls.Add(lblCategory);
            searchPanel.Controls.Add(cboCategory);
            searchPanel.Controls.Add(btnClearFilter);
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
            dataGridViewProducts = new DataGridView
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
            this.Controls.Add(dataGridViewProducts);

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

        private async void LoadProducts()
        {
            try
            {
                _products = await _productRepository.GetAllProductsAsync();
                RefreshDataGridView(_products);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu sản phẩm: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void LoadCategories()
        {
            try
            {
                var categories = await _productRepository.GetProductCategoriesAsync();

                cboCategory.Items.Clear();
                cboCategory.Items.Add("Tất cả");
                cboCategory.Items.AddRange(categories.ToArray());
                cboCategory.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh mục: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshDataGridView(System.Collections.Generic.List<Sanpham> products)
        {
            dataGridViewProducts.DataSource = null;
            dataGridViewProducts.DataSource = products;

            // Configure columns
            if (dataGridViewProducts.Columns.Count > 0)
            {
                dataGridViewProducts.Columns["MaSp"].HeaderText = "Mã SP";
                dataGridViewProducts.Columns["TenSP"].HeaderText = "Tên Sản Phẩm";
                dataGridViewProducts.Columns["MoTa"].HeaderText = "Mô Tả";
                dataGridViewProducts.Columns["Gia"].HeaderText = "Giá";
                dataGridViewProducts.Columns["DVTSP"].HeaderText = "Đơn Vị";
                dataGridViewProducts.Columns["Loai"].HeaderText = "Loại";

                // Format price column
                if (dataGridViewProducts.Columns["Gia"] != null)
                {
                    dataGridViewProducts.Columns["Gia"].DefaultCellStyle.Format = "N0";
                }
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void CboCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private async void ApplyFilters()
        {
            string searchText = txtSearch.Text.ToLower();
            string selectedCategory = cboCategory.SelectedItem?.ToString();

            List<Sanpham> filteredProducts;

            if (!string.IsNullOrEmpty(searchText))
            {
                filteredProducts = await _productRepository.SearchProductsAsync(searchText);
            }
            else
            {
                filteredProducts = _products;
            }

            if (selectedCategory != "Tất cả" && !string.IsNullOrEmpty(selectedCategory))
            {
                filteredProducts = filteredProducts.Where(p => p.Loai == selectedCategory).ToList();
            }

            RefreshDataGridView(filteredProducts);
        }

        private void BtnClearFilter_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            cboCategory.SelectedIndex = 0;
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadProducts();
            LoadCategories();
        }

        private async void BtnAdd_Click(object sender, EventArgs e)
        {
            var addForm = new ProductAddEditForm();
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    bool success = await _productRepository.AddProductAsync(addForm.Product);
                    if (success)
                    {
                        MessageBox.Show("Thêm sản phẩm thành công!", "Thông báo", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadProducts();
                        LoadCategories();
                    }
                    else
                    {
                        MessageBox.Show("Lỗi khi thêm sản phẩm!", "Lỗi", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi thêm sản phẩm: {ex.Message}", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridViewProducts.SelectedRows.Count > 0)
            {
                var selectedProduct = dataGridViewProducts.SelectedRows[0].DataBoundItem as Sanpham;
                if (selectedProduct != null)
                {
                    var editForm = new ProductAddEditForm(selectedProduct);
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            bool success = await _productRepository.UpdateProductAsync(editForm.Product);
                            if (success)
                            {
                                MessageBox.Show("Cập nhật sản phẩm thành công!", "Thông báo", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadProducts();
                                LoadCategories();
                            }
                            else
                            {
                                MessageBox.Show("Lỗi khi cập nhật sản phẩm!", "Lỗi", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Lỗi khi cập nhật sản phẩm: {ex.Message}", "Lỗi", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
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
                var selectedProduct = dataGridViewProducts.SelectedRows[0].DataBoundItem as Sanpham;
                if (selectedProduct != null)
                {
                    var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa sản phẩm '{selectedProduct.TenSp}'?", 
                        "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    
                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            bool success = await _productRepository.DeleteProductAsync(selectedProduct.MaSp);
                            if (success)
                            {
                                MessageBox.Show("Xóa sản phẩm thành công!", "Thông báo", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadProducts();
                                LoadCategories();
                            }
                            else
                            {
                                MessageBox.Show("Lỗi khi xóa sản phẩm!", "Lỗi", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Lỗi khi xóa sản phẩm: {ex.Message}", "Lỗi", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm để xóa", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
} 
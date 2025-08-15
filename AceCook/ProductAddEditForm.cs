using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Globalization;
using AceCook.Models;
using AceCook.Repositories;

namespace AceCook
{
    public partial class ProductAddEditForm : Form
    {
        private readonly ProductRepository _productRepository;
        private FormMode _mode;
        private Sanpham _currentProduct;
        public Sanpham Product { get; private set; }

        public ProductAddEditForm(ProductRepository productRepository, FormMode mode = FormMode.Add, Sanpham product = null)
        {
            if (productRepository == null)
                throw new ArgumentNullException(nameof(productRepository));

            InitializeComponent();
            _productRepository = productRepository;
            _mode = mode;
            _currentProduct = product;
            
            InitializeForm();
        }

        private void InitializeForm()
        {
            try
            {
                switch (_mode)
                {
                    case FormMode.Add:
                        this.Text = "Thêm sản phẩm mới";
                        lblTitle.Text = "Thêm sản phẩm mới";
                        btnSave.Text = "Thêm";
                        break;
                    case FormMode.Edit:
                        this.Text = "Chỉnh sửa sản phẩm";
                        lblTitle.Text = "Chỉnh sửa sản phẩm";
                        btnSave.Text = "Cập nhật";
                        LoadProductData();
                        break;
                    case FormMode.View:
                        this.Text = "Xem thông tin sản phẩm";
                        lblTitle.Text = "Thông tin sản phẩm";
                        btnSave.Visible = false;
                        btnCancel.Text = "Đóng";
                        SetControlsReadOnly(true);
                        LoadProductData();
                        break;
                }

                // Set default product type
                if (cmbLoai.Items.Count > 0 && _mode == FormMode.Add)
                {
                    cmbLoai.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi khởi tạo form: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProductData()
        {
            try
            {
                if (_currentProduct != null)
                {
                    txtMaSP.Text = _currentProduct.MaSp ?? "";
                    txtTenSP.Text = _currentProduct.TenSp ?? "";
                    txtMoTa.Text = _currentProduct.MoTa ?? "";
                    txtGia.Text = _currentProduct.Gia?.ToString("N0", CultureInfo.GetCultureInfo("vi-VN")) ?? "";
                    txtDonVi.Text = _currentProduct.Dvtsp ?? "";
                    
                    if (!string.IsNullOrEmpty(_currentProduct.Loai))
                    {
                        int index = cmbLoai.FindStringExact(_currentProduct.Loai);
                        if (index != -1)
                            cmbLoai.SelectedIndex = index;
                        else
                        {
                            // If category doesn't exist in predefined list, add it
                            cmbLoai.Items.Add(_currentProduct.Loai);
                            cmbLoai.SelectedItem = _currentProduct.Loai;
                        }
                    }

                    // Disable MaSP for edit mode
                    if (_mode == FormMode.Edit)
                    {
                        txtMaSP.ReadOnly = true;
                        txtMaSP.BackColor = SystemColors.Control;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu sản phẩm: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetControlsReadOnly(bool readOnly)
        {
            try
            {
                txtMaSP.ReadOnly = readOnly;
                txtTenSP.ReadOnly = readOnly;
                txtMoTa.ReadOnly = readOnly;
                txtGia.ReadOnly = readOnly;
                txtDonVi.ReadOnly = readOnly;
                cmbLoai.Enabled = !readOnly;

                if (readOnly)
                {
                    var backColor = SystemColors.Control;
                    txtMaSP.BackColor = backColor;
                    txtTenSP.BackColor = backColor;
                    txtMoTa.BackColor = backColor;
                    txtGia.BackColor = backColor;
                    txtDonVi.BackColor = backColor;
                    cmbLoai.BackColor = backColor;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in SetControlsReadOnly: {ex.Message}");
            }
        }

        private bool ValidateInput()
        {
            try
            {
                var errors = new List<string>();

                // Validate MaSP
                if (string.IsNullOrWhiteSpace(txtMaSP.Text))
                {
                    errors.Add("Mã sản phẩm không được để trống.");
                }
                else if (txtMaSP.Text.Length > 10)
                {
                    errors.Add("Mã sản phẩm không được vượt quá 10 ký tự.");
                }

                // Validate TenSP
                if (string.IsNullOrWhiteSpace(txtTenSP.Text))
                {
                    errors.Add("Tên sản phẩm không được để trống.");
                }
                else if (txtTenSP.Text.Length > 50)
                {
                    errors.Add("Tên sản phẩm không được vượt quá 50 ký tự.");
                }

                // Validate MoTa
                if (!string.IsNullOrWhiteSpace(txtMoTa.Text) && txtMoTa.Text.Length > 100)
                {
                    errors.Add("Mô tả không được vượt quá 100 ký tự.");
                }

                // Validate Gia
                if (!string.IsNullOrWhiteSpace(txtGia.Text))
                {
                    var giaText = txtGia.Text.Replace(",", "").Replace(".", "");
                    if (!decimal.TryParse(giaText, out decimal gia) || gia < 0)
                    {
                        errors.Add("Giá phải là số dương.");
                    }
                    else if (gia > 999999999999999999)
                    {
                        errors.Add("Giá quá lớn.");
                    }
                }

                // Validate DonVi
                if (!string.IsNullOrWhiteSpace(txtDonVi.Text) && txtDonVi.Text.Length > 20)
                {
                    errors.Add("Đơn vị tính không được vượt quá 20 ký tự.");
                }

                // Validate Loai
                if (!string.IsNullOrWhiteSpace(cmbLoai.Text) && cmbLoai.Text.Length > 20)
                {
                    errors.Add("Loại sản phẩm không được vượt quá 20 ký tự.");
                }

                if (errors.Count > 0)
                {
                    MessageBox.Show(string.Join("\n", errors), "Lỗi xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xác thực dữ liệu: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                btnSave.Enabled = false;
                Cursor = Cursors.WaitCursor;

                decimal? gia = null;
                if (!string.IsNullOrWhiteSpace(txtGia.Text))
                {
                    var giaText = txtGia.Text.Replace(",", "").Replace(".", "");
                    if (decimal.TryParse(giaText, out decimal giaValue))
                    {
                        gia = giaValue;
                    }
                }

                var product = new Sanpham
                {
                    MaSp = txtMaSP.Text.Trim(),
                    TenSp = txtTenSP.Text.Trim(),
                    MoTa = string.IsNullOrWhiteSpace(txtMoTa.Text) ? null : txtMoTa.Text.Trim(),
                    Gia = gia,
                    Dvtsp = string.IsNullOrWhiteSpace(txtDonVi.Text) ? null : txtDonVi.Text.Trim(),
                    Loai = string.IsNullOrWhiteSpace(cmbLoai.Text) ? null : cmbLoai.Text.Trim()
                };

                bool success = false;
                string operation = "";

                if (_mode == FormMode.Add)
                {
                    success = await _productRepository.AddProductAsync(product);
                    operation = "thêm";
                }
                else if (_mode == FormMode.Edit)
                {
                    success = await _productRepository.UpdateProductAsync(product);
                    operation = "cập nhật";
                }

                if (success)
                {
                    Product = product;
                    MessageBox.Show($"Đã {operation} sản phẩm thành công!", "Thành công", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show($"Không thể {operation} sản phẩm. Vui lòng thử lại.", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSave.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in btnCancel_Click: {ex.Message}");
                this.Close();
            }
        }
    }
}

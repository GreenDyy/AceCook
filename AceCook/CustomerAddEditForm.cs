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
using AceCook.Models;
using AceCook.Repositories;

namespace AceCook
{
    public enum FormMode
    {
        Add,
        Edit,
        View
    }

    public partial class CustomerAddEditForm : Form
    {
        private readonly CustomerRepository _customerRepository;
        private FormMode _mode;
        private Khachhang _currentCustomer;
        public Khachhang Customer { get; private set; }

        public CustomerAddEditForm(CustomerRepository customerRepository, FormMode mode = FormMode.Add, Khachhang customer = null)
        {
            InitializeComponent();
            _customerRepository = customerRepository;
            _mode = mode;
            _currentCustomer = customer;
            
            InitializeForm();
        }

        private void InitializeForm()
        {
            switch (_mode)
            {
                case FormMode.Add:
                    this.Text = "Thêm khách hàng mới";
                    lblTitle.Text = "Thêm khách hàng mới";
                    btnSave.Text = "Thêm";
                    break;
                case FormMode.Edit:
                    this.Text = "Chỉnh sửa khách hàng";
                    lblTitle.Text = "Chỉnh sửa khách hàng";
                    btnSave.Text = "Cập nhật";
                    LoadCustomerData();
                    break;
                case FormMode.View:
                    this.Text = "Xem thông tin khách hàng";
                    lblTitle.Text = "Thông tin khách hàng";
                    btnSave.Visible = false;
                    btnCancel.Text = "Đóng";
                    SetControlsReadOnly(true);
                    LoadCustomerData();
                    break;
            }

            // Set default customer type
            if (cmbLoaiKH.Items.Count > 0 && _mode == FormMode.Add)
            {
                cmbLoaiKH.SelectedIndex = 0;
            }
        }

        private void LoadCustomerData()
        {
            if (_currentCustomer != null)
            {
                txtMaKH.Text = _currentCustomer.MaKh;
                txtTenKH.Text = _currentCustomer.TenKh;
                txtSDTKH.Text = _currentCustomer.Sdtkh;
                txtDiaChiKH.Text = _currentCustomer.DiaChiKh;
                txtEmailKH.Text = _currentCustomer.EmailKh;
                
                if (!string.IsNullOrEmpty(_currentCustomer.LoaiKh))
                {
                    int index = cmbLoaiKH.FindStringExact(_currentCustomer.LoaiKh);
                    if (index != -1)
                        cmbLoaiKH.SelectedIndex = index;
                }

                // Disable MaKH for edit mode
                if (_mode == FormMode.Edit)
                {
                    txtMaKH.ReadOnly = true;
                    txtMaKH.BackColor = SystemColors.Control;
                }
            }
        }

        private void SetControlsReadOnly(bool readOnly)
        {
            txtMaKH.ReadOnly = readOnly;
            txtTenKH.ReadOnly = readOnly;
            txtSDTKH.ReadOnly = readOnly;
            txtDiaChiKH.ReadOnly = readOnly;
            txtEmailKH.ReadOnly = readOnly;
            cmbLoaiKH.Enabled = !readOnly;

            if (readOnly)
            {
                var backColor = SystemColors.Control;
                txtMaKH.BackColor = backColor;
                txtTenKH.BackColor = backColor;
                txtSDTKH.BackColor = backColor;
                txtDiaChiKH.BackColor = backColor;
                txtEmailKH.BackColor = backColor;
                cmbLoaiKH.BackColor = backColor;
            }
        }

        private bool ValidateInput()
        {
            var errors = new List<string>();

            // Validate MaKH
            if (string.IsNullOrWhiteSpace(txtMaKH.Text))
            {
                errors.Add("Mã khách hàng không được để trống.");
            }
            else if (txtMaKH.Text.Length > 10)
            {
                errors.Add("Mã khách hàng không được vượt quá 10 ký tự.");
            }

            // Validate TenKH
            if (string.IsNullOrWhiteSpace(txtTenKH.Text))
            {
                errors.Add("Tên khách hàng không được để trống.");
            }
            else if (txtTenKH.Text.Length > 50)
            {
                errors.Add("Tên khách hàng không được vượt quá 50 ký tự.");
            }

            // Validate LoaiKH
            if (cmbLoaiKH.SelectedIndex == -1)
            {
                errors.Add("Vui lòng chọn loại khách hàng.");
            }

            // Validate SDTKH
            if (!string.IsNullOrWhiteSpace(txtSDTKH.Text))
            {
                if (txtSDTKH.Text.Length > 10)
                {
                    errors.Add("Số điện thoại không được vượt quá 10 ký tự.");
                }
                if (!Regex.IsMatch(txtSDTKH.Text, @"^[0-9]+$"))
                {
                    errors.Add("Số điện thoại chỉ được chứa các chữ số.");
                }
            }

            // Validate DiaChiKH
            if (!string.IsNullOrWhiteSpace(txtDiaChiKH.Text) && txtDiaChiKH.Text.Length > 100)
            {
                errors.Add("Địa chỉ không được vượt quá 100 ký tự.");
            }

            // Validate EmailKH
            if (!string.IsNullOrWhiteSpace(txtEmailKH.Text))
            {
                if (txtEmailKH.Text.Length > 50)
                {
                    errors.Add("Email không được vượt quá 50 ký tự.");
                }
                if (!Regex.IsMatch(txtEmailKH.Text, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
                {
                    errors.Add("Email không đúng định dạng.");
                }
            }

            if (errors.Count > 0)
            {
                MessageBox.Show(string.Join("\n", errors), "Lỗi xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                btnSave.Enabled = false;
                Cursor = Cursors.WaitCursor;

                var customer = new Khachhang
                {
                    MaKh = txtMaKH.Text.Trim(),
                    TenKh = txtTenKH.Text.Trim(),
                    LoaiKh = cmbLoaiKH.SelectedItem?.ToString(),
                    Sdtkh = string.IsNullOrWhiteSpace(txtSDTKH.Text) ? null : txtSDTKH.Text.Trim(),
                    DiaChiKh = string.IsNullOrWhiteSpace(txtDiaChiKH.Text) ? null : txtDiaChiKH.Text.Trim(),
                    EmailKh = string.IsNullOrWhiteSpace(txtEmailKH.Text) ? null : txtEmailKH.Text.Trim()
                };

                bool success = false;
                string operation = "";

                if (_mode == FormMode.Add)
                {
                    // Check if customer already exists
                    var existingCustomer = await _customerRepository.GetCustomerByIdAsync(customer.MaKh);
                    if (existingCustomer != null)
                    {
                        MessageBox.Show("Mã khách hàng đã tồn tại. Vui lòng chọn mã khác.", 
                            "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    success = await _customerRepository.AddCustomerAsync(customer);
                    operation = "thêm";
                }
                else if (_mode == FormMode.Edit)
                {
                    success = await _customerRepository.UpdateCustomerAsync(customer);
                    operation = "cập nhật";
                }

                if (success)
                {
                    Customer = customer;
                    MessageBox.Show($"Đã {operation} khách hàng thành công!", "Thành công", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show($"Không thể {operation} khách hàng. Vui lòng thử lại.", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

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
            if (customerRepository == null)
                throw new ArgumentNullException(nameof(customerRepository));

            InitializeComponent();
            _customerRepository = customerRepository;
            _mode = mode;
            _currentCustomer = customer;
            
            InitializeForm();
        }

        private void InitializeForm()
        {
            try
            {
                switch (_mode)
                {
                    case FormMode.Add:
                        this.Text = "Thêm khách hàng mới";
                        if (lblTitle != null) lblTitle.Text = "Thêm khách hàng mới";
                        if (btnSave != null) btnSave.Text = "Thêm";
                        // Auto-generate customer ID for Add mode
                        GenerateCustomerId();
                        break;
                    case FormMode.Edit:
                        this.Text = "Chỉnh sửa khách hàng";
                        if (lblTitle != null) lblTitle.Text = "Chỉnh sửa khách hàng";
                        if (btnSave != null) btnSave.Text = "Cập nhật";
                        LoadCustomerData();
                        break;
                    case FormMode.View:
                        this.Text = "Xem thông tin khách hàng";
                        if (lblTitle != null) lblTitle.Text = "Thông tin khách hàng";
                        if (btnSave != null) btnSave.Visible = false;
                        if (btnCancel != null) btnCancel.Text = "Đóng";
                        SetControlsReadOnly(true);
                        LoadCustomerData();
                        break;
                }

                // Set default customer type
                if (cmbLoaiKH != null && cmbLoaiKH.Items.Count > 0 && _mode == FormMode.Add)
                {
                    cmbLoaiKH.SelectedIndex = 0;
                }

                // Add event handler for phone number validation
                if (txtSDTKH != null)
                {
                    txtSDTKH.KeyPress += TxtSDTKH_KeyPress;
                    txtSDTKH.TextChanged += TxtSDTKH_TextChanged;
                    txtSDTKH.PlaceholderText = "Nhập 10 chữ số";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi khởi tạo form: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void GenerateCustomerId()
        {
            try
            {
                if (txtMaKH == null) return;

                // Get all existing customer IDs
                var customers = await _customerRepository.GetAllCustomersAsync();
                var existingIds = customers.Select(c => c.MaKh).ToList();

                // Find the highest existing number
                int highestNumber = 0;
                foreach (var id in existingIds)
                {
                    if (id != null && id.StartsWith("KH"))
                    {
                        // Extract the number part after "KH"
                        string numberPart = id.Substring(2);
                        if (int.TryParse(numberPart, out int number))
                        {
                            if (number > highestNumber)
                            {
                                highestNumber = number;
                            }
                        }
                    }
                }

                // Generate next ID
                int nextNumber = highestNumber + 1;
                string newId = $"KH{nextNumber:D2}";

                txtMaKH.Text = newId;
                txtMaKH.ReadOnly = true;
                txtMaKH.BackColor = SystemColors.Control;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error generating customer ID: {ex.Message}");
                // Fallback to manual entry if auto-generation fails
                if (txtMaKH != null)
                {
                    txtMaKH.ReadOnly = false;
                    txtMaKH.BackColor = SystemColors.Window;
                }
            }
        }

        private void TxtSDTKH_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Only allow digits and backspace
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }

        private void TxtSDTKH_TextChanged(object sender, EventArgs e)
        {
            if (txtSDTKH == null) return;

            // Remove any non-digit characters
            string digitsOnly = new string(txtSDTKH.Text.Where(char.IsDigit).ToArray());
            
            // Limit to 10 digits
            if (digitsOnly.Length > 10)
            {
                digitsOnly = digitsOnly.Substring(0, 10);
            }

            // Update text if it changed
            if (txtSDTKH.Text != digitsOnly)
            {
                txtSDTKH.Text = digitsOnly;
                txtSDTKH.SelectionStart = txtSDTKH.Text.Length;
            }

            // Show validation feedback
            if (!string.IsNullOrEmpty(digitsOnly) && digitsOnly.Length != 10)
            {
                txtSDTKH.BackColor = Color.LightYellow;
            }
            else if (!string.IsNullOrEmpty(digitsOnly) && digitsOnly.Length == 10)
            {
                txtSDTKH.BackColor = Color.LightGreen;
            }
            else
            {
                txtSDTKH.BackColor = SystemColors.Window;
            }
        }

        private void LoadCustomerData()
        {
            try
            {
                if (_currentCustomer != null)
                {
                    if (txtMaKH != null) txtMaKH.Text = _currentCustomer.MaKh ?? "";
                    if (txtTenKH != null) txtTenKH.Text = _currentCustomer.TenKh ?? "";
                    if (txtSDTKH != null) txtSDTKH.Text = _currentCustomer.Sdtkh ?? "";
                    if (txtDiaChiKH != null) txtDiaChiKH.Text = _currentCustomer.DiaChiKh ?? "";
                    if (txtEmailKH != null) txtEmailKH.Text = _currentCustomer.EmailKh ?? "";
                    
                    if (cmbLoaiKH != null && !string.IsNullOrEmpty(_currentCustomer.LoaiKh))
                    {
                        int index = cmbLoaiKH.FindStringExact(_currentCustomer.LoaiKh);
                        if (index != -1)
                            cmbLoaiKH.SelectedIndex = index;
                        else
                        {
                            // If customer type doesn't exist in predefined list, add it
                            cmbLoaiKH.Items.Add(_currentCustomer.LoaiKh);
                            cmbLoaiKH.SelectedItem = _currentCustomer.LoaiKh;
                        }
                    }

                    // Disable MaKH for edit mode
                    if (_mode == FormMode.Edit && txtMaKH != null)
                    {
                        txtMaKH.ReadOnly = true;
                        txtMaKH.BackColor = SystemColors.Control;
                    }

                    // Validate phone number display
                    if (txtSDTKH != null && !string.IsNullOrEmpty(txtSDTKH.Text))
                    {
                        TxtSDTKH_TextChanged(txtSDTKH, EventArgs.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu khách hàng: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetControlsReadOnly(bool readOnly)
        {
            try
            {
                // Check if controls are initialized before using them
                if (txtMaKH != null) txtMaKH.ReadOnly = readOnly;
                if (txtTenKH != null) txtTenKH.ReadOnly = readOnly;
                if (txtSDTKH != null) txtSDTKH.ReadOnly = readOnly;
                if (txtDiaChiKH != null) txtDiaChiKH.ReadOnly = readOnly;
                if (txtEmailKH != null) txtEmailKH.ReadOnly = readOnly;
                if (cmbLoaiKH != null) cmbLoaiKH.Enabled = !readOnly;

                if (readOnly)
                {
                    var backColor = SystemColors.Control;
                    if (txtMaKH != null) txtMaKH.BackColor = backColor;
                    if (txtTenKH != null) txtTenKH.BackColor = backColor;
                    if (txtSDTKH != null) txtSDTKH.BackColor = backColor;
                    if (txtDiaChiKH != null) txtDiaChiKH.BackColor = backColor;
                    if (txtEmailKH != null) txtEmailKH.BackColor = backColor;
                    if (cmbLoaiKH != null) cmbLoaiKH.BackColor = backColor;
                }
                else
                {
                    // Reset colors for edit mode
                    if (txtMaKH != null) txtMaKH.BackColor = SystemColors.Window;
                    if (txtTenKH != null) txtTenKH.BackColor = SystemColors.Window;
                    if (txtDiaChiKH != null) txtDiaChiKH.BackColor = SystemColors.Window;
                    if (txtEmailKH != null) txtEmailKH.BackColor = SystemColors.Window;
                    if (cmbLoaiKH != null) cmbLoaiKH.BackColor = SystemColors.Window;
                    
                    // Phone number will get its color from TextChanged event
                    if (txtSDTKH != null && !string.IsNullOrEmpty(txtSDTKH.Text))
                    {
                        TxtSDTKH_TextChanged(txtSDTKH, EventArgs.Empty);
                    }
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

                // Validate MaKH
                if (txtMaKH == null || string.IsNullOrWhiteSpace(txtMaKH.Text))
                {
                    errors.Add("Mã khách hàng không được để trống.");
                }
                else if (txtMaKH.Text.Length > 10)
                {
                    errors.Add("Mã khách hàng không được vượt quá 10 ký tự.");
                }

                // Validate TenKH
                if (txtTenKH == null || string.IsNullOrWhiteSpace(txtTenKH.Text))
                {
                    errors.Add("Tên khách hàng không được để trống.");
                }
                else if (txtTenKH.Text.Length > 50)
                {
                    errors.Add("Tên khách hàng không được vượt quá 50 ký tự.");
                }

                // Validate LoaiKH
                if (cmbLoaiKH == null || cmbLoaiKH.SelectedIndex == -1)
                {
                    errors.Add("Vui lòng chọn loại khách hàng.");
                }

                // Validate SDTKH
                if (txtSDTKH != null && !string.IsNullOrWhiteSpace(txtSDTKH.Text))
                {
                    if (txtSDTKH.Text.Length != 10)
                    {
                        errors.Add("Số điện thoại phải có đúng 10 chữ số.");
                    }
                    if (!Regex.IsMatch(txtSDTKH.Text, @"^[0-9]{10}$"))
                    {
                        errors.Add("Số điện thoại phải có đúng 10 chữ số.");
                    }
                }

                // Validate DiaChiKH
                if (txtDiaChiKH != null && !string.IsNullOrWhiteSpace(txtDiaChiKH.Text) && txtDiaChiKH.Text.Length > 100)
                {
                    errors.Add("Địa chỉ không được vượt quá 100 ký tự.");
                }

                // Validate EmailKH
                if (txtEmailKH != null && !string.IsNullOrWhiteSpace(txtEmailKH.Text))
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
                if (btnSave != null) btnSave.Enabled = false;
                Cursor = Cursors.WaitCursor;

                var customer = new Khachhang
                {
                    MaKh = txtMaKH?.Text?.Trim() ?? "",
                    TenKh = txtTenKH?.Text?.Trim() ?? "",
                    LoaiKh = cmbLoaiKH?.SelectedItem?.ToString(),
                    Sdtkh = txtSDTKH != null && !string.IsNullOrWhiteSpace(txtSDTKH.Text) ? txtSDTKH.Text.Trim() : null,
                    DiaChiKh = txtDiaChiKH != null && !string.IsNullOrWhiteSpace(txtDiaChiKH.Text) ? txtDiaChiKH.Text.Trim() : null,
                    EmailKh = txtEmailKH != null && !string.IsNullOrWhiteSpace(txtEmailKH.Text) ? txtEmailKH.Text.Trim() : null
                };

                bool success = false;
                string operation = "";

                if (_mode == FormMode.Add)
                {
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
                if (btnSave != null) btnSave.Enabled = true;
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

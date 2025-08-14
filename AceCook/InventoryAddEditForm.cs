using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using AceCook.Models;
using AceCook.Repositories;

namespace AceCook
{
    public enum InventoryOperationType
    {
        View,
        NhapKho,
        XuatKho
    }

    public partial class InventoryAddEditForm : Form
    {
        private readonly InventoryRepository _inventoryRepository;
        private InventoryOperationType _operationType;
        private CtTon _currentInventory;
        public CtTon InventoryItem { get; private set; }

        public InventoryAddEditForm(InventoryRepository inventoryRepository, InventoryOperationType operationType, CtTon inventory)
        {
            _inventoryRepository = inventoryRepository;
            _operationType = operationType;
            _currentInventory = inventory;
            
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            switch (_operationType)
            {
                case InventoryOperationType.View:
                    this.Text = "Xem thông tin tồn kho";
                    lblTitle.Text = "THÔNG TIN TỒN KHO";
                    lblSoLuongThayDoi.Visible = false;
                    numSoLuongThayDoi.Visible = false;
                    lblSoLuongSauKhi.Visible = false;
                    txtSoLuongSauKhi.Visible = false;
                    lblGhiChu.Visible = false;
                    txtGhiChu.Visible = false;
                    btnSave.Visible = false;
                    btnCancel.Text = "Đóng";
                    break;
                case InventoryOperationType.NhapKho:
                    this.Text = "Nhập kho";
                    lblTitle.Text = "NHẬP KHO";
                    lblTitle.ForeColor = Color.FromArgb(46, 204, 113);
                    lblSoLuongThayDoi.Text = "SL nhập:";
                    lblSoLuongSauKhi.Text = "SL sau nhập:";
                    btnSave.Text = "Nhập kho";
                    break;
                case InventoryOperationType.XuatKho:
                    this.Text = "Xuất kho";
                    lblTitle.Text = "XUẤT KHO";
                    lblTitle.ForeColor = Color.FromArgb(231, 76, 60);
                    lblSoLuongThayDoi.Text = "SL xuất:";
                    lblSoLuongSauKhi.Text = "SL sau xuất:";
                    txtSoLuongSauKhi.BackColor = Color.LightCoral;
                    btnSave.Text = "Xuất kho";
                    btnSave.BackColor = Color.FromArgb(231, 76, 60);
                    break;
            }

            LoadInventoryData();
        }

        private void LoadInventoryData()
        {
            if (_currentInventory != null)
            {
                txtMaSP.Text = _currentInventory.MaSp;
                txtTenSP.Text = _currentInventory.MaSpNavigation?.TenSp ?? "N/A";
                txtMaKho.Text = _currentInventory.MaKho;
                txtTenKho.Text = _currentInventory.MaKhoNavigation?.TenKho ?? "N/A";
                
                var currentStock = _currentInventory.SoLuongTonKho ?? 0;
                txtSoLuongHienTai.Text = currentStock.ToString("N0", CultureInfo.GetCultureInfo("vi-VN"));
                
                UpdateSoLuongSauKhi();
            }
        }

        private void NumSoLuongThayDoi_ValueChanged(object sender, EventArgs e)
        {
            UpdateSoLuongSauKhi();
        }

        private void UpdateSoLuongSauKhi()
        {
            var currentStock = _currentInventory?.SoLuongTonKho ?? 0;
            var changeAmount = (int)numSoLuongThayDoi.Value;
            
            int newStock = 0;
            switch (_operationType)
            {
                case InventoryOperationType.NhapKho:
                    newStock = currentStock + changeAmount;
                    break;
                case InventoryOperationType.XuatKho:
                    newStock = currentStock - changeAmount;
                    break;
            }

            txtSoLuongSauKhi.Text = newStock.ToString("N0", CultureInfo.GetCultureInfo("vi-VN"));
            
            // Cảnh báo nếu số lượng sau khi xuất < 0
            if (_operationType == InventoryOperationType.XuatKho && newStock < 0)
            {
                txtSoLuongSauKhi.BackColor = Color.Red;
                txtSoLuongSauKhi.ForeColor = Color.White;
            }
            else
            {
                txtSoLuongSauKhi.BackColor = _operationType == InventoryOperationType.NhapKho ? 
                    Color.LightGreen : Color.LightCoral;
                txtSoLuongSauKhi.ForeColor = Color.Black;
            }
        }

        private bool ValidateInput()
        {
            var changeAmount = (int)numSoLuongThayDoi.Value;
            
            if (changeAmount <= 0)
            {
                MessageBox.Show("Số lượng phải lớn hơn 0!", "Lỗi xác thực", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (_operationType == InventoryOperationType.XuatKho)
            {
                var currentStock = _currentInventory?.SoLuongTonKho ?? 0;
                if (changeAmount > currentStock)
                {
                    MessageBox.Show($"Không thể xuất {changeAmount} sản phẩm! Chỉ còn {currentStock} trong kho.", 
                        "Lỗi xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                btnSave.Enabled = false;
                Cursor = Cursors.WaitCursor;

                var changeAmount = (int)numSoLuongThayDoi.Value;
                var currentStock = _currentInventory?.SoLuongTonKho ?? 0;
                
                int newStock = 0;
                switch (_operationType)
                {
                    case InventoryOperationType.NhapKho:
                        newStock = currentStock + changeAmount;
                        break;
                    case InventoryOperationType.XuatKho:
                        newStock = currentStock - changeAmount;
                        break;
                }

                // Update inventory
                _currentInventory.SoLuongTonKho = newStock;
                bool success = await _inventoryRepository.UpdateInventoryAsync(_currentInventory);

                if (success)
                {
                    InventoryItem = _currentInventory;
                    
                    var operation = _operationType == InventoryOperationType.NhapKho ? "nhập" : "xuất";
                    MessageBox.Show($"Đã {operation} {changeAmount} sản phẩm thành công!\nSố lượng tồn kho mới: {newStock}", 
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Không thể cập nhật tồn kho. Vui lòng thử lại.", "Lỗi", 
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

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

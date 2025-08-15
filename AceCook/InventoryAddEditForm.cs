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
using Microsoft.EntityFrameworkCore;

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
                
                // Kiểm tra xem có đủ hàng để xuất không
                if (currentStock <= 0)
                {
                    MessageBox.Show("Kho đã hết hàng, không thể xuất kho!", 
                        "Lỗi xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                
                if (changeAmount > currentStock)
                {
                    MessageBox.Show($"Không thể xuất {changeAmount} sản phẩm! Chỉ còn {currentStock} trong kho.", 
                        "Lỗi xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Kiểm tra số lượng sau khi xuất có âm không
                var newStock = currentStock - changeAmount;
                if (newStock < 0)
                {
                    MessageBox.Show($"Số lượng sau khi xuất không được âm! ({newStock})", 
                        "Lỗi xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }

        private async Task<bool> RefreshInventoryDataAsync()
        {
            try
            {
                // Refresh dữ liệu tồn kho từ database
                var refreshedInventory = await _inventoryRepository.GetInventoryByIdAsync(_currentInventory.MaSp, _currentInventory.MaKho);
                if (refreshedInventory != null)
                {
                    _currentInventory = refreshedInventory;
                    LoadInventoryData();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to refresh inventory data: {ex.Message}");
                return false;
            }
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                btnSave.Enabled = false;
                Cursor = Cursors.WaitCursor;

                // Refresh dữ liệu trước khi thực hiện thao tác
                if (!await RefreshInventoryDataAsync())
                {
                    MessageBox.Show("Không thể cập nhật dữ liệu tồn kho. Vui lòng thử lại.", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

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

                // Log thông tin trước khi update
                System.Diagnostics.Debug.WriteLine($"Updating inventory: MaSP={_currentInventory.MaSp}, MaKho={_currentInventory.MaKho}");
                System.Diagnostics.Debug.WriteLine($"Current stock: {currentStock}, Change: {changeAmount}, New stock: {newStock}");

                // Update inventory
                _currentInventory.SoLuongTonKho = newStock;
                
                bool success = false;
                try
                {
                    success = await _inventoryRepository.UpdateInventoryAsync(_currentInventory);
                }
                catch (Exception updateEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Update failed: {updateEx.Message}");
                    System.Diagnostics.Debug.WriteLine($"Stack trace: {updateEx.StackTrace}");
                    throw;
                }

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
                System.Diagnostics.Debug.WriteLine($"Save operation failed: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                string errorMessage = ex switch
                {
                    InvalidOperationException => $"Lỗi thao tác: {ex.Message}",
                    DbUpdateException => "Lỗi cập nhật cơ sở dữ liệu. Vui lòng kiểm tra quyền truy cập.",
                    _ => $"Đã xảy ra lỗi: {ex.Message}"
                };
                
                MessageBox.Show(errorMessage, "Lỗi", 
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

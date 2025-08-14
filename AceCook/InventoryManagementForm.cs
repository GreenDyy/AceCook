using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AceCook.Models;
using AceCook.Repositories;
using System.Collections.Generic;

namespace AceCook
{
    public partial class InventoryManagementForm : Form
    {
        private readonly AppDbContext _context;
        private readonly InventoryRepository _inventoryRepository;
        private List<CtTon> _currentInventory; // Lưu trữ dữ liệu hiện tại

        public InventoryManagementForm(AppDbContext context)
        {
            _context = context;
            _inventoryRepository = new InventoryRepository(context);
            _currentInventory = new List<CtTon>(); // Khởi tạo danh sách rỗng
            InitializeComponent();
            _ = LoadInventory(); // Sử dụng async method
        }

        private async Task LoadInventory()
        {
            try
            {
                // Load data sequentially to avoid DbContext conflicts
                var inventory = await _inventoryRepository.GetAllInventoryAsync();
                await LoadWarehouseData();
                await LoadProductTypeData();

                RefreshDataGridView(inventory);
                await UpdateSummary(inventory);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu tồn kho: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadWarehouseData()
        {
            try
            {
                var warehouses = await _inventoryRepository.GetAllWarehousesAsync();
                cboWarehouseFilter.Items.Clear();
                cboWarehouseFilter.Items.Add("Tất cả kho");
                foreach (var warehouse in warehouses)
                {
                    cboWarehouseFilter.Items.Add(warehouse?.TenKho);
                }
                cboWarehouseFilter.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách kho: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadProductTypeData()
        {
            try
            {
                var productTypes = await _inventoryRepository.GetAllProductTypesAsync();

                cboProductTypeFilter.Items.Clear();
                cboProductTypeFilter.Items.Add("Tất cả loại");
                foreach (var type in productTypes)
                {
                    cboProductTypeFilter.Items.Add(type);
                }
                cboProductTypeFilter.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách loại sản phẩm: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task UpdateSummary(List<CtTon> inventory)
        {
            try
            {
                // Sử dụng Repository method để lấy thống kê
                var (totalItems, totalValue, lowStockCount) = await _inventoryRepository.GetInventorySummaryAsync();

                lblTotalItems.Text = totalItems.ToString("N0");
                lblTotalValue.Text = totalValue.ToString("N0") + " VNĐ";
                
                // Update low stock count in summary panel
                var lowStockLabel = pnlSummary.Controls.OfType<Label>().FirstOrDefault(l => l.Text == "0");
                if (lowStockLabel != null)
                {
                    lowStockLabel.Text = lowStockCount.ToString();
                }
            }
            catch (Exception ex)
            {
                // Fallback to local calculation if repository fails
                var totalItems = inventory.Sum(i => i.SoLuongTonKho ?? 0);
                var totalValue = inventory.Sum(i => (i.SoLuongTonKho ?? 0) * (i.MaSpNavigation?.Gia ?? 0));
                var lowStockCount = inventory.Count(i => (i.SoLuongTonKho ?? 0) <= 10);

                lblTotalItems.Text = totalItems.ToString("N0");
                lblTotalValue.Text = totalValue.ToString("N0") + " VNĐ";
                
                var lowStockLabel = pnlSummary.Controls.OfType<Label>().FirstOrDefault(l => l.Text == "0");
                if (lowStockLabel != null)
                {
                    lowStockLabel.Text = lowStockCount.ToString();
                }
            }
        }

        private async void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            await ApplyFilters();
        }

        private async void CboWarehouseFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            await ApplyFilters();
        }

        private async void CboProductTypeFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            await ApplyFilters();
        }

        private async Task ApplyFilters()
        {
            try
            {
                var searchTerm = txtSearch.Text.Trim();
                var selectedWarehouse = cboWarehouseFilter.SelectedItem?.ToString();
                var selectedProductType = cboProductTypeFilter.SelectedItem?.ToString();

                // Sử dụng Repository method để lấy dữ liệu đã được lọc
                var inventory = await _inventoryRepository.GetFilteredInventoryAsync(
                    searchTerm, selectedWarehouse, selectedProductType);

                RefreshDataGridView(inventory);
                await UpdateSummary(inventory);
                
                // Update title with result count
                var resultCount = inventory.Count;
                var totalCount = await _inventoryRepository.GetTotalItemsAsync();
                this.Text = $"Quản lý Tồn kho - Hiển thị {resultCount}/{totalCount} sản phẩm";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi áp dụng bộ lọc: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                // Clear all filters
                txtSearch.Text = "";
                cboWarehouseFilter.SelectedIndex = 0;
                cboProductTypeFilter.SelectedIndex = 0;
                
                // Reload all data
                await LoadInventory();
                
                MessageBox.Show("Đã làm mới dữ liệu thành công!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi làm mới dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnViewDetails_Click(object sender, EventArgs e)
        {
            if (dataGridViewInventory.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridViewInventory.SelectedRows[0];
                var maSp = selectedRow.Cells["MaSp"].Value?.ToString();
                var maKho = selectedRow.Cells["MaKho"].Value?.ToString();

                if (!string.IsNullOrEmpty(maSp) && !string.IsNullOrEmpty(maKho))
                {
                    try
                    {
                        var inventoryItem = await _inventoryRepository.GetInventoryByIdAsync(maSp, maKho);
                        if (inventoryItem != null)
                        {
                            ViewInventoryDetails(inventoryItem);
                        }
                        else
                        {
                            MessageBox.Show("Không thể tải thông tin tồn kho!", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi tải thông tin tồn kho: {ex.Message}", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một dòng tồn kho để xem chi tiết!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ViewInventoryDetails(CtTon inventoryItem)
        {
            try
            {
                var viewForm = new InventoryAddEditForm(_inventoryRepository, InventoryOperationType.View, inventoryItem);
                viewForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form xem chi tiết: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Inventory Operations
        private async void BtnNhapKho_Click(object sender, EventArgs e)
        {
            if (dataGridViewInventory.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridViewInventory.SelectedRows[0];
                var maSp = selectedRow.Cells["MaSp"].Value?.ToString();
                var maKho = selectedRow.Cells["MaKho"].Value?.ToString();

                if (!string.IsNullOrEmpty(maSp) && !string.IsNullOrEmpty(maKho))
                {
                    try
                    {
                        var inventoryItem = await _inventoryRepository.GetInventoryByIdAsync(maSp, maKho);
                        if (inventoryItem != null)
                        {
                            var nhapKhoForm = new InventoryAddEditForm(_inventoryRepository, InventoryOperationType.NhapKho, inventoryItem);
                            if (nhapKhoForm.ShowDialog() == DialogResult.OK)
                            {
                                MessageBox.Show("Nhập kho thành công!", "Thông báo", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                await LoadInventory();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Không thể tải thông tin tồn kho!", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi mở form nhập kho: {ex.Message}", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm để nhập kho!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void BtnXuatKho_Click(object sender, EventArgs e)
        {
            if (dataGridViewInventory.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridViewInventory.SelectedRows[0];
                var maSp = selectedRow.Cells["MaSp"].Value?.ToString();
                var maKho = selectedRow.Cells["MaKho"].Value?.ToString();

                if (!string.IsNullOrEmpty(maSp) && !string.IsNullOrEmpty(maKho))
                {
                    try
                    {
                        var inventoryItem = await _inventoryRepository.GetInventoryByIdAsync(maSp, maKho);
                        if (inventoryItem != null)
                        {
                            var xuatKhoForm = new InventoryAddEditForm(_inventoryRepository, InventoryOperationType.XuatKho, inventoryItem);
                            if (xuatKhoForm.ShowDialog() == DialogResult.OK)
                            {
                                MessageBox.Show("Xuất kho thành công!", "Thông báo", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                await LoadInventory();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Không thể tải thông tin tồn kho!", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi mở form xuất kho: {ex.Message}", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm để xuất kho!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AceCook.Models;
using AceCook.Repositories;
using AceCook.Helpers;
using System.Collections.Generic;

namespace AceCook
{
    public partial class InventoryManagementForm : Form
    {
        private readonly InventoryRepository _inventoryRepository;
        private List<CtTon> _currentInventory;
        private bool _isLoading = false;

        public InventoryManagementForm(AppDbContext context)
        {
            try
            {
                _inventoryRepository = new InventoryRepository(context);
                _currentInventory = new List<CtTon>();
                InitializeComponent();
                
                // Sử dụng Task.Run để tránh blocking UI thread
                Task.Run(async () => await LoadInventoryAsync());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khởi tạo form: {ex.Message}", "Lỗi Khởi tạo",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadInventoryAsync()
        {
            if (_isLoading) return;
            
            try
            {
                _isLoading = true;
                
                // Disable controls during loading
                this.Invoke((MethodInvoker)delegate
                {
                    this.Cursor = Cursors.WaitCursor;
                    this.Enabled = false;
                });

                // Test database connection first
                if (!await DatabaseHelper.TestConnectionAsync(_inventoryRepository.GetType().GetField("_context", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(_inventoryRepository) as AppDbContext))
                {
                    // MessageBox.Show("Không thể kết nối đến cơ sở dữ liệu. Vui lòng kiểm tra kết nối mạng và thử lại.", 
                    //     "Lỗi Kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Load data with timeout
                var inventoryTask = _inventoryRepository.GetAllInventoryAsync();
                var inventory = await Task.WhenAny(inventoryTask, Task.Delay(30000)) == inventoryTask 
                    ? await inventoryTask 
                    : throw new TimeoutException("Timeout khi load dữ liệu tồn kho");

                if (inventory == null || inventory.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu tồn kho nào được tìm thấy.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Load supporting data
                await LoadWarehouseDataAsync();
                await LoadProductTypeDataAsync();

                // Update UI on main thread
                this.Invoke((MethodInvoker)delegate
                {
                    RefreshDataGridView(inventory);
                    UpdateSummaryUI(inventory);
                    this.Text = $"Quản lý Tồn kho - {inventory.Count} sản phẩm";
                });
            }
            catch (Exception ex)
            {
                string errorMessage = ex switch
                {
                    TimeoutException => "Hệ thống quá tải, vui lòng thử lại sau.",
                    DbUpdateException => "Lỗi cập nhật cơ sở dữ liệu. Vui lòng kiểm tra quyền truy cập.",
                    InvalidOperationException => "Lỗi thao tác dữ liệu. Vui lòng kiểm tra cấu trúc database.",
                    _ => $"Lỗi không xác định: {ex.Message}"
                };

                this.Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show($"Lỗi khi tải dữ liệu tồn kho:\n{errorMessage}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
            }
            finally
            {
                _isLoading = false;
                this.Invoke((MethodInvoker)delegate
                {
                    this.Cursor = Cursors.Default;
                    this.Enabled = true;
                });
            }
        }

        private async Task<bool> TestDatabaseConnectionAsync()
        {
            try
            {
                // Test connection by trying to get a simple count
                var count = await _inventoryRepository.GetTotalItemsAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task LoadWarehouseDataAsync()
        {
            try
            {
                var warehouses = await _inventoryRepository.GetAllWarehousesAsync();
                this.Invoke((MethodInvoker)delegate
                {
                    cboWarehouseFilter.Items.Clear();
                    cboWarehouseFilter.Items.Add("Tất cả kho");
                    foreach (var warehouse in warehouses)
                    {
                        cboWarehouseFilter.Items.Add(warehouse.TenKho);
                    }
                    cboWarehouseFilter.SelectedIndex = 0;
                });
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show($"Lỗi khi tải danh sách kho: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
            }
        }

        private async Task LoadProductTypeDataAsync()
        {
            try
            {
                var productTypes = await _inventoryRepository.GetAllProductTypesAsync();
                this.Invoke((MethodInvoker)delegate
                {
                    cboProductTypeFilter.Items.Clear();
                    cboProductTypeFilter.Items.Add("Tất cả loại");
                    foreach (var type in productTypes)
                    {
                        cboProductTypeFilter.Items.Add(type);
                    }
                    cboProductTypeFilter.SelectedIndex = 0;
                });
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show($"Lỗi khi tải danh sách loại sản phẩm: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
            }
        }

        private void UpdateSummaryUI(List<CtTon> inventory)
        {
            try
            {
                var totalItems = inventory.Sum(i => i.SoLuongTonKho ?? 0);
                var totalValue = inventory.Sum(i => (i.SoLuongTonKho ?? 0) * (i.MaSpNavigation?.Gia ?? 0));
                var lowStockCount = inventory.Count(i => (i.SoLuongTonKho ?? 0) <= 10);

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
                MessageBox.Show($"Lỗi khi cập nhật thống kê: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            if (_isLoading) return;
            await ApplyFiltersAsync();
        }

        private async void CboWarehouseFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isLoading) return;
            await ApplyFiltersAsync();
        }

        private async void CboProductTypeFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isLoading) return;
            await ApplyFiltersAsync();
        }

        private async Task ApplyFiltersAsync()
        {
            if (_isLoading) return;
            
            try
            {
                _isLoading = true;
                this.Cursor = Cursors.WaitCursor;

                var searchTerm = txtSearch.Text.Trim();
                var selectedWarehouse = cboWarehouseFilter.SelectedItem?.ToString();
                var selectedProductType = cboProductTypeFilter.SelectedItem?.ToString();

                var inventory = await _inventoryRepository.GetFilteredInventoryAsync(
                    searchTerm, selectedWarehouse, selectedProductType);

                RefreshDataGridView(inventory);
                UpdateSummaryUI(inventory);

                var resultCount = inventory.Count;
                var totalCount = await _inventoryRepository.GetTotalItemsAsync();
                this.Text = $"Quản lý Tồn kho - Hiển thị {resultCount}/{totalCount} sản phẩm";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi áp dụng bộ lọc: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isLoading = false;
                this.Cursor = Cursors.Default;
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
                await LoadInventoryAsync();

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
                        this.Cursor = Cursors.WaitCursor;
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
                    finally
                    {
                        this.Cursor = Cursors.Default;
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
                        this.Cursor = Cursors.WaitCursor;
                        var inventoryItem = await _inventoryRepository.GetInventoryByIdAsync(maSp, maKho);
                        if (inventoryItem != null)
                        {
                            var nhapKhoForm = new InventoryAddEditForm(_inventoryRepository, InventoryOperationType.NhapKho, inventoryItem);
                            if (nhapKhoForm.ShowDialog() == DialogResult.OK)
                            {
                                MessageBox.Show("Nhập kho thành công!", "Thông báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                await LoadInventoryAsync();
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
                    finally
                    {
                        this.Cursor = Cursors.Default;
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
                        this.Cursor = Cursors.WaitCursor;
                        var inventoryItem = await _inventoryRepository.GetInventoryByIdAsync(maSp, maKho);
                        if (inventoryItem != null)
                        {
                            var xuatKhoForm = new InventoryAddEditForm(_inventoryRepository, InventoryOperationType.XuatKho, inventoryItem);
                            if (xuatKhoForm.ShowDialog() == DialogResult.OK)
                            {
                                MessageBox.Show("Xuất kho thành công!", "Thông báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                await LoadInventoryAsync();
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
                    finally
                    {
                        this.Cursor = Cursors.Default;
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

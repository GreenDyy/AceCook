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
        private DataGridView dataGridViewInventory;
        private TextBox txtSearch;
        private ComboBox cboWarehouseFilter;
        private ComboBox cboProductTypeFilter;
        private Button btnRefresh;
        private Button btnExport;
        private Label lblTitle;
        private Label lblTotalItems;
        private Label lblTotalValue; 
        private Panel pnlSummary;
        private List<CtTon> _currentInventory; // Lưu trữ dữ liệu hiện tại

        public InventoryManagementForm(AppDbContext context)
        {
            _context = context;
            _inventoryRepository = new InventoryRepository(context);
            _currentInventory = new List<CtTon>(); // Khởi tạo danh sách rỗng
            InitializeComponent();
            SetupUI();
            _ = LoadInventory(); // Sử dụng async method
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.Text = "Quản lý Tồn kho";
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
                Text = "QUẢN LÝ TỒN KHO",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Dock = DockStyle.Top,
                Height = 70,
                TextAlign = ContentAlignment.MiddleLeft,
            };

            // Summary Panel
            pnlSummary = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 200, 0, 150)
            };

            var lblTotalItemsTitle = new Label
            {
                Text = "Tổng số sản phẩm:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(150, 25),
                Location = new Point(20, 15),
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblTotalItems = new Label
            {
                Text = "0",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 152, 219),
                Size = new Size(100, 25),
                Location = new Point(180, 15),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lblTotalValueTitle = new Label
            {
                Text = "Tổng giá trị:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(120, 25),
                Location = new Point(320, 15),
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblTotalValue = new Label
            {
                Text = "0 VNĐ",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113),
                Size = new Size(200, 25),
                Location = new Point(450, 15),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lblLowStockTitle = new Label
            {
                Text = "Sản phẩm sắp hết:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(150, 25),
                Location = new Point(20, 45),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lblLowStockCount = new Label
            {
                Text = "0",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(231, 76, 60),
                Size = new Size(100, 25),
                Location = new Point(180, 45),
                TextAlign = ContentAlignment.MiddleLeft
            };

            pnlSummary.Controls.AddRange(new Control[] { 
                lblTotalItemsTitle, lblTotalItems, 
                lblTotalValueTitle, lblTotalValue,
                lblLowStockTitle, lblLowStockCount
            });

            // Filters Panel
            var pnlFilters = new FlowLayoutPanel
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
                Width = 200,
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "Mã SP, tên SP...",
                Margin = new Padding(0, 5, 20, 0)
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;

            var lblWarehouse = new Label
            {
                Text = "Kho hàng:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 8, 10, 0)
            };

            cboWarehouseFilter = new ComboBox
            {
                Width = 150,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 5, 20, 0)
            };
            cboWarehouseFilter.Items.Add("Tất cả kho");
            cboWarehouseFilter.SelectedIndex = 0;
            cboWarehouseFilter.SelectedIndexChanged += CboWarehouseFilter_SelectedIndexChanged;

            var lblProductType = new Label
            {
                Text = "Loại SP:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 8, 10, 0)
            };

            cboProductTypeFilter = new ComboBox
            {
                Width = 150,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 5, 20, 0)
            };
            cboProductTypeFilter.Items.Add("Tất cả loại");
            cboProductTypeFilter.SelectedIndex = 0;
            cboProductTypeFilter.SelectedIndexChanged += CboProductTypeFilter_SelectedIndexChanged;

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

            btnExport = new Button
            {
                Text = "📊 Xuất báo cáo",
                Width = 120,
                Height = 35,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 5, 10, 0)
            };
            btnExport.FlatAppearance.BorderSize = 0;
            btnExport.Click += BtnExport_Click;

            pnlFilters.Controls.AddRange(new Control[] { 
                lblSearch, txtSearch, lblWarehouse, cboWarehouseFilter,
                lblProductType, cboProductTypeFilter, btnRefresh, btnExport
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

             var btnViewDetails = CreateActionButton("👁️ Xem chi tiết", Color.FromArgb(108, 92, 231));
             btnViewDetails.Click += BtnViewDetails_Click;

             var btnNhapKho = CreateActionButton("📦 Nhập kho", Color.FromArgb(46, 204, 113));
             btnNhapKho.Click += BtnNhapKho_Click;
 
             var btnXuatKho = CreateActionButton("📤 Xuất kho", Color.FromArgb(255, 193, 7));
             btnXuatKho.Click += BtnXuatKho_Click;
 
             pnlActions.Controls.AddRange(new Control[] { btnViewDetails, btnNhapKho, btnXuatKho });

            // DataGridView
            dataGridViewInventory = new DataGridView
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
            dataGridViewInventory.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dataGridViewInventory.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridViewInventory.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dataGridViewInventory.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridViewInventory.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewInventory.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            dataGridViewInventory.DefaultCellStyle.SelectionForeColor = Color.White;

            // Add all to form
            this.Controls.AddRange(new Control[] { 
                dataGridViewInventory, pnlActions, pnlFilters, pnlSummary, lblTitle 
            });
        }

        // Helper to create buttons
        private Button CreateActionButton(string text, Color backColor)
        {
            var btn = new Button
            {
                Text = text,
                Width = 200,
                Height = 40,
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

        private async Task LoadInventory()
        {
            try
            {
                // Load data in parallel for better performance
                var inventoryTask = _inventoryRepository.GetAllInventoryAsync();
                var warehouseTask = LoadWarehouseData();
                var productTypeTask = LoadProductTypeData();

                // Wait for all tasks to complete
                var inventory = await inventoryTask;
                await warehouseTask;
                await productTypeTask;

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
                    cboWarehouseFilter.Items.Add(warehouse.TenKho);
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

        private void RefreshDataGridView(List<CtTon> inventory)
        {
            _currentInventory = inventory; // Lưu trữ dữ liệu hiện tại
            dataGridViewInventory.DataSource = null;
            dataGridViewInventory.Columns.Clear();

            // Create custom columns
            dataGridViewInventory.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MaSp",
                HeaderText = "Mã SP",
                Width = 100
            });

            dataGridViewInventory.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenSp",
                HeaderText = "Tên sản phẩm",
                Width = 200
            });

            dataGridViewInventory.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Loai",
                HeaderText = "Loại",
                Width = 120
            });

            dataGridViewInventory.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MaKho",
                HeaderText = "Mã kho",
                Width = 80
            });

            dataGridViewInventory.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenKho",
                HeaderText = "Tên kho",
                Width = 150
            });

            dataGridViewInventory.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ViTri",
                HeaderText = "Vị trí",
                Width = 150
            });

            dataGridViewInventory.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SoLuongTon",
                HeaderText = "Số lượng tồn",
                Width = 120
            });

            dataGridViewInventory.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DonGia",
                HeaderText = "Đơn giá",
                Width = 120
            });

            dataGridViewInventory.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ThanhTien",
                HeaderText = "Thành tiền",
                Width = 150
            });

            dataGridViewInventory.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TrangThai",
                HeaderText = "Trạng thái",
                Width = 100
            });

            // Populate data
            foreach (var item in inventory)
            {
                var rowIndex = dataGridViewInventory.Rows.Add();
                var row = dataGridViewInventory.Rows[rowIndex];

                row.Cells["MaSp"].Value = item.MaSp;
                row.Cells["TenSp"].Value = item.MaSpNavigation?.TenSp ?? "N/A";
                row.Cells["Loai"].Value = item.MaSpNavigation?.Loai ?? "N/A";
                row.Cells["MaKho"].Value = item.MaKho;
                row.Cells["TenKho"].Value = item.MaKhoNavigation?.TenKho ?? "N/A";
                row.Cells["ViTri"].Value = item.MaKhoNavigation?.ViTri ?? "N/A";
                row.Cells["SoLuongTon"].Value = item.SoLuongTonKho ?? 0;
                row.Cells["DonGia"].Value = (item.MaSpNavigation?.Gia ?? 0).ToString("N0") + " VNĐ";
                
                var thanhTien = (item.SoLuongTonKho ?? 0) * (item.MaSpNavigation?.Gia ?? 0);
                row.Cells["ThanhTien"].Value = thanhTien.ToString("N0") + " VNĐ";

                // Style status based on stock level
                var status = GetStockStatus(item.SoLuongTonKho ?? 0);
                row.Cells["TrangThai"].Value = status;
                StyleStatusCell(row.Cells["TrangThai"], status);
            }
        }

        private string GetStockStatus(int stockLevel)
        {
            if (stockLevel == 0) return "Hết hàng";
            if (stockLevel <= 10) return "Sắp hết";
            if (stockLevel <= 50) return "Trung bình";
            return "Đủ hàng";
        }

        private void StyleStatusCell(DataGridViewCell cell, string status)
        {
            switch (status)
            {
                case "Hết hàng":
                    cell.Style.ForeColor = Color.Red;
                    break;
                case "Sắp hết":
                    cell.Style.ForeColor = Color.Orange;
                    break;
                case "Trung bình":
                    cell.Style.ForeColor = Color.Blue;
                    break;
                case "Đủ hàng":
                    cell.Style.ForeColor = Color.Green;
                    break;
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

        private async void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                // Sử dụng dữ liệu đã được lọc thay vì tải lại tất cả
                if (_currentInventory == null || _currentInventory.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                    FileName = $"BaoCaoTonKho_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    await ExportToCSV(_currentInventory, saveFileDialog.FileName);
                    MessageBox.Show($"Xuất báo cáo thành công! Đã xuất {_currentInventory.Count} sản phẩm.", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất báo cáo: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task ExportToCSV(List<CtTon> inventory, string filePath)
        {
            try
            {
                // Sử dụng Repository method để lấy dữ liệu báo cáo
                var reportData = await _inventoryRepository.GetInventoryReportDataAsync(inventory);
                
                var lines = new List<string>
                {
                    "Mã SP,Tên sản phẩm,Loại,Mã kho,Tên kho,Vị trí,Số lượng tồn,Đơn giá,Thành tiền,Trạng thái"
                };

                foreach (dynamic item in reportData)
                {
                    var line = $"{item.MaSp}," +
                              $"\"{item.TenSp}\"," +
                              $"\"{item.Loai}\"," +
                              $"{item.MaKho}," +
                              $"\"{item.TenKho}\"," +
                              $"\"{item.ViTri}\"," +
                              $"{item.SoLuongTon}," +
                              $"{item.DonGia}," +
                              $"{item.ThanhTien}," +
                              $"\"{item.TrangThai}\"";
                    
                    lines.Add(line);
                }

                await System.IO.File.WriteAllLinesAsync(filePath, lines);
            }
            catch (Exception ex)
            {
                // Fallback to original method if repository fails
                var lines = new List<string>
                {
                    "Mã SP,Tên sản phẩm,Loại,Mã kho,Tên kho,Vị trí,Số lượng tồn,Đơn giá,Thành tiền,Trạng thái"
                };

                foreach (var item in inventory)
                {
                    var status = GetStockStatus(item.SoLuongTonKho ?? 0);
                    var thanhTien = (item.SoLuongTonKho ?? 0) * (item.MaSpNavigation?.Gia ?? 0);
                    
                    var line = $"{item.MaSp}," +
                              $"\"{item.MaSpNavigation?.TenSp ?? ""}\"," +
                              $"\"{item.MaSpNavigation?.Loai ?? ""}\"," +
                              $"{item.MaKho}," +
                              $"\"{item.MaKhoNavigation?.TenKho ?? ""}\"," +
                              $"\"{item.MaKhoNavigation?.ViTri ?? ""}\"," +
                              $"{item.SoLuongTonKho ?? 0}," +
                              $"{item.MaSpNavigation?.Gia ?? 0}," +
                              $"{thanhTien}," +
                              $"\"{status}\"";
                    
                    lines.Add(line);
                }

                await System.IO.File.WriteAllLinesAsync(filePath, lines);
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
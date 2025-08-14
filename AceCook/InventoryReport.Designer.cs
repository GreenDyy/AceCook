using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using AceCook.Models;
using AceCook.Repositories;

namespace AceCook
{
    public partial class InventoryReportForm : Form
    {
        private readonly ReportRepository _reportRepository;
        private DataGridView dataGridViewInventory;
        private DateTimePicker dtpFromDate;
        private DateTimePicker dtpToDate;
        private ComboBox cboWarehouse;
        private TextBox txtSearch;
        private Button btnGenerate;
        private Button btnExport;
        private Button btnPrint;
        private Button btnRefresh;
        private Button btnClearFilter;
        private Label lblTitle;
        private Label lblTotalValue;
        private Label lblTotalItems;
        private Label lblOutOfStock;
        private Label lblInStock;
        private List<ReportRepository.InventoryReportItem> _inventoryData;

        public InventoryReportForm(AppDbContext context)
        {
            _reportRepository = new ReportRepository(context);
            InitializeComponent();
            SetupUI();
            _ = LoadDataAsync();
        }

        private void InitializeComponent()
        {
            // This method is implemented in the main form class
            // All UI setup is done programmatically in SetupUI()
        }

        private void SetupUI()
        {
            // Title
            lblTitle = new Label
            {
                Text = "BÁO CÁO TỒN KHO",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Dock = DockStyle.Top,
                Height = 70,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0)
            };

            // Filter Panel
            var pnlFilter = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(20)
            };

            // Date Range Group
            var grpDateRange = new GroupBox
            {
                Text = "Khoảng thời gian",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(10, 10),
                Size = new Size(350, 90)
            };

            var lblFromDate = new Label
            {
                Text = "Từ ngày:",
                Font = new Font("Segoe UI", 9),
                Location = new Point(15, 25),
                Size = new Size(60, 20)
            };

            dtpFromDate = new DateTimePicker
            {
                Font = new Font("Segoe UI", 9),
                Location = new Point(80, 23),
                Size = new Size(120, 25),
                Value = DateTime.Now.AddMonths(-1)
            };

            var lblToDate = new Label
            {
                Text = "Đến ngày:",
                Font = new Font("Segoe UI", 9),
                Location = new Point(15, 55),
                Size = new Size(60, 20)
            };

            dtpToDate = new DateTimePicker
            {
                Font = new Font("Segoe UI", 9),
                Location = new Point(80, 53),
                Size = new Size(120, 25),
                Value = DateTime.Now
            };

            grpDateRange.Controls.AddRange(new Control[] { lblFromDate, dtpFromDate, lblToDate, dtpToDate });

            // Search Group
            var grpSearch = new GroupBox
            {
                Text = "Tìm kiếm & Lọc",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(380, 10),
                Size = new Size(400, 90)
            };

            var lblSearch = new Label
            {
                Text = "Tìm kiếm:",
                Font = new Font("Segoe UI", 9),
                Location = new Point(15, 25),
                Size = new Size(60, 20)
            };

            txtSearch = new TextBox
            {
                Font = new Font("Segoe UI", 9),
                Location = new Point(80, 23),
                Size = new Size(150, 25),
                PlaceholderText = "Mã SP, tên SP..."
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;

            var lblWarehouse = new Label
            {
                Text = "Kho:",
                Font = new Font("Segoe UI", 9),
                Location = new Point(15, 55),
                Size = new Size(60, 20)
            };

            cboWarehouse = new ComboBox
            {
                Font = new Font("Segoe UI", 9),
                Location = new Point(80, 53),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboWarehouse.SelectedIndexChanged += CboWarehouse_SelectedIndexChanged;

            btnClearFilter = new Button
            {
                Text = "🔄 Xóa bộ lọc",
                Font = new Font("Segoe UI", 9),
                Location = new Point(250, 23),
                Size = new Size(100, 25),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnClearFilter.FlatAppearance.BorderSize = 0;
            btnClearFilter.Click += BtnClearFilter_Click;

            grpSearch.Controls.AddRange(new Control[] { lblSearch, txtSearch, lblWarehouse, cboWarehouse, btnClearFilter });

            // Action Buttons
            btnGenerate = CreateActionButton("📊 Tạo báo cáo", Color.FromArgb(46, 204, 113));
            btnGenerate.Location = new Point(800, 25);
            btnGenerate.Click += BtnGenerate_Click;

            btnExport = CreateActionButton("📤 Xuất Excel", Color.FromArgb(52, 152, 219));
            btnExport.Location = new Point(800, 60);
            btnExport.Click += BtnExport_Click;

            btnPrint = CreateActionButton("🖨️ In báo cáo", Color.FromArgb(155, 89, 182));
            btnPrint.Location = new Point(920, 25);
            btnPrint.Click += BtnPrint_Click;

            btnRefresh = CreateActionButton("🔄 Làm mới", Color.FromArgb(241, 196, 15));
            btnRefresh.Location = new Point(920, 60);
            btnRefresh.Click += BtnRefresh_Click;

            pnlFilter.Controls.AddRange(new Control[] { grpDateRange, grpSearch, btnGenerate, btnExport, btnPrint, btnRefresh });

            // Statistics Panel
            var pnlStats = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 120,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(20),
                BackColor = Color.Transparent
            };

            var statsCard1 = CreateStatsCard("Tổng giá trị tồn kho", "0 VNĐ", Color.FromArgb(46, 204, 113));
            lblTotalValue = statsCard1.Controls.OfType<Label>().FirstOrDefault(l => l.Font.Bold);

            var statsCard2 = CreateStatsCard("Tổng sản phẩm", "0", Color.FromArgb(52, 152, 219));
            lblTotalItems = statsCard2.Controls.OfType<Label>().FirstOrDefault(l => l.Font.Bold);

            var statsCard3 = CreateStatsCard("Còn hàng", "0", Color.FromArgb(241, 196, 15));
            lblInStock = statsCard3.Controls.OfType<Label>().FirstOrDefault(l => l.Font.Bold);

            var statsCard4 = CreateStatsCard("Hết hàng", "0", Color.FromArgb(231, 76, 60));
            lblOutOfStock = statsCard4.Controls.OfType<Label>().FirstOrDefault(l => l.Font.Bold);

            pnlStats.Controls.AddRange(new Control[] { statsCard1, statsCard2, statsCard3, statsCard4 });

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
                RowTemplate = { Height = 45 }
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
            this.Controls.AddRange(new Control[] { dataGridViewInventory, pnlStats, pnlFilter, lblTitle });
        }

        private Button CreateActionButton(string text, Color backColor)
        {
            var btn = new Button
            {
                Text = text,
                Width = 110,
                Height = 30,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private Panel CreateStatsCard(string title, string value, Color accentColor)
        {
            var card = new Panel
            {
                Width = 280,
                Height = 80,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 0, 20, 0)
            };

            var accentLine = new Panel
            {
                Width = 5,
                Height = 80,
                BackColor = accentColor,
                Dock = DockStyle.Left
            };

            var lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(20, 15),
                AutoSize = true
            };

            var lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(20, 45),
                AutoSize = true
            };

            card.Controls.AddRange(new Control[] { accentLine, lblValue, lblTitle });
            return card;
        }

        private async Task LoadDataAsync()
        {
            try
            {
                await LoadWarehousesAsync();
                await GenerateReportAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadWarehousesAsync()
        {
            await Task.Run(() =>
            {
                // Load warehouses from database
                // For now, add default options
                this.Invoke((MethodInvoker)delegate
                {
                    cboWarehouse.Items.Clear();
                    cboWarehouse.Items.Add("Tất cả kho");
                    cboWarehouse.Items.Add("Kho A");
                    cboWarehouse.Items.Add("Kho B");
                    cboWarehouse.SelectedIndex = 0;
                });
            });
        }

        private async Task GenerateReportAsync()
        {
            try
            {
                var fromDate = dtpFromDate.Value;
                var toDate = dtpToDate.Value;

                await Task.Run(() =>
                {
                    _inventoryData = _reportRepository.GetInventoryReport(fromDate, toDate);
                    
                    this.Invoke((MethodInvoker)delegate
                    {
                        RefreshDataGridView(_inventoryData);
                        UpdateStatistics(_inventoryData);
                    });
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo báo cáo: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshDataGridView(List<ReportRepository.InventoryReportItem> data)
        {
            if (data == null) return;

            dataGridViewInventory.DataSource = null;
            dataGridViewInventory.Columns.Clear();

            // Create custom columns
            dataGridViewInventory.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MaSP",
                HeaderText = "Mã SP",
                Width = 100
            });

            dataGridViewInventory.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenSanPham",
                HeaderText = "Tên Sản Phẩm",
                Width = 250
            });

            dataGridViewInventory.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Loai",
                HeaderText = "Loại",
                Width = 120
            });

            dataGridViewInventory.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Gia",
                HeaderText = "Giá",
                Width = 120
            });

            dataGridViewInventory.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TonKho",
                HeaderText = "Tồn Kho",
                Width = 100
            });

            dataGridViewInventory.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "GiaTri",
                HeaderText = "Giá Trị",
                Width = 150
            });

            dataGridViewInventory.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ChiTietTheoKho",
                HeaderText = "Chi Tiết Theo Kho",
                Width = 300
            });

            // Populate data
            foreach (var item in data)
            {
                var rowIndex = dataGridViewInventory.Rows.Add();
                var row = dataGridViewInventory.Rows[rowIndex];

                row.Cells["MaSP"].Value = item.MaSP;
                row.Cells["TenSanPham"].Value = item.TenSanPham;
                row.Cells["Loai"].Value = item.Loai;
                row.Cells["Gia"].Value = item.Gia.ToString("N0") + " VNĐ";
                row.Cells["TonKho"].Value = item.TonKho;
                row.Cells["GiaTri"].Value = item.GiaTri.ToString("N0") + " VNĐ";
                row.Cells["ChiTietTheoKho"].Value = item.ChiTietTheoKho;

                // Color coding for stock levels
                if (item.TonKho == 0)
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 238);
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(231, 76, 60);
                }
                else if (item.TonKho < 10)
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 243, 205);
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(241, 196, 15);
                }
            }

            // Right-align numeric columns
            dataGridViewInventory.Columns["Gia"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewInventory.Columns["TonKho"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewInventory.Columns["GiaTri"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void UpdateStatistics(List<ReportRepository.InventoryReportItem> data)
        {
            if (data == null || !data.Any()) return;

            var totalValue = data.Sum(d => d.GiaTri);
            var totalItems = data.Count;
            var inStock = data.Count(d => d.TonKho > 0);
            var outOfStock = data.Count(d => d.TonKho == 0);

            lblTotalValue.Text = totalValue.ToString("N0") + " VNĐ";
            lblTotalItems.Text = totalItems.ToString();
            lblInStock.Text = inStock.ToString();
            lblOutOfStock.Text = outOfStock.ToString();
        }

        private void ApplyFilters()
        {
            if (_inventoryData == null) return;

            var filteredData = _inventoryData.AsEnumerable();

            // Apply search filter
            var searchText = txtSearch.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(searchText))
            {
                filteredData = filteredData.Where(d => 
                    d.MaSP.ToLower().Contains(searchText) ||
                    d.TenSanPham.ToLower().Contains(searchText) ||
                    d.Loai.ToLower().Contains(searchText));
            }

            // Apply warehouse filter
            var selectedWarehouse = cboWarehouse.SelectedItem?.ToString();
            if (selectedWarehouse != "Tất cả kho" && !string.IsNullOrEmpty(selectedWarehouse))
            {
                filteredData = filteredData.Where(d => 
                    d.ChiTietTheoKho.Contains(selectedWarehouse));
            }

            var result = filteredData.ToList();
            RefreshDataGridView(result);
            UpdateStatistics(result);
        }

        // Event Handlers
        private async void BtnGenerate_Click(object sender, EventArgs e)
        {
            await GenerateReportAsync();
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng xuất Excel sẽ được phát triển trong phiên bản tiếp theo.", 
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng in báo cáo sẽ được phát triển trong phiên bản tiếp theo.", 
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            await LoadDataAsync();
        }

        private void BtnClearFilter_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            cboWarehouse.SelectedIndex = 0;
            if (_inventoryData != null)
            {
                RefreshDataGridView(_inventoryData);
                UpdateStatistics(_inventoryData);
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void CboWarehouse_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }
    }
}
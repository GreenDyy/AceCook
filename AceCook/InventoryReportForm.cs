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
        // Xóa các controls không cần thiết
        // private DateTimePicker dtpFromDate;
        // private DateTimePicker dtpToDate;
        // private ComboBox cboWarehouse;
        // private TextBox txtSearch;
        // private Button btnGenerateReport;
        // private Button btnExportExcel;
        // private Button btnPrintReport;
        // private Button btnRefreshData;
        // private Button btnClearFilters;
        private Label lblFormTitle;
        private Label lblTotalValueAmount;
        private Label lblTotalItemsCount;
        private Label lblOutOfStockCount;
        private Label lblInStockCount;
        private List<ReportRepository.InventoryReportItem> _inventoryReportData;

        public InventoryReportForm(AppDbContext context)
        {
            _reportRepository = new ReportRepository(context);
            InitializeFormComponents();
            SetupUserInterface();
            
            // Defer data loading until form is fully loaded
            this.Load += InventoryReportForm_Load;
        }
        
        private async void InventoryReportForm_Load(object sender, EventArgs e)
        {
            await LoadInitialDataAsync();
        }

        private void InitializeFormComponents()
        {
            this.SuspendLayout();
            
            this.Text = "Báo cáo Tồn kho";
            this.Size = new Size(1600, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            
            this.ResumeLayout(false);
        }

        private void SetupUserInterface()
        {
            // Title
            lblFormTitle = new Label
            {
                Text = "BÁO CÁO TỒN KHO",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Dock = DockStyle.Top,
                Height = 70,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0)
            };

            // Xóa toàn bộ Filter Panel
            // var pnlFilter = new Panel
            // {
            //     Dock = DockStyle.Top,
            //     Height = 120,
            //     BackColor = Color.White,
            //     BorderStyle = BorderStyle.FixedSingle,
            //     Padding = new Padding(20)
            // };

            // Date Range Group
            // var grpDateRange = new GroupBox
            // {
            //     Text = "Khoảng thời gian",
            //     Font = new Font("Segoe UI", 10, FontStyle.Bold),
            //     Location = new Point(10, 10),
            //     Size = new Size(350, 90)
            // };

            // var lblFromDate = new Label
            // {
            //     Text = "Từ ngày:",
            //     Font = new Font("Segoe UI", 9),
            //     Location = new Point(15, 25),
            //     Size = new Size(60, 20)
            // };

            // dtpFromDate = new DateTimePicker
            // {
            //     Font = new Font("Segoe UI", 9),
            //     Location = new Point(80, 23),
            //     Size = new Size(120, 25),
            //     Value = DateTime.Now.AddMonths(-1)
            // };

            // var lblToDate = new Label
            // {
            //     Text = "Đến ngày:",
            //     Font = new Font("Segoe UI", 9),
            //     Location = new Point(15, 55),
            //     Size = new Size(60, 20)
            // };

            // dtpToDate = new DateTimePicker
            // {
            //     Font = new Font("Segoe UI", 9),
            //     Location = new Point(80, 53),
            //     Size = new Size(120, 25),
            //     Value = DateTime.Now
            // };

            // grpDateRange.Controls.AddRange(new Control[] { lblFromDate, dtpFromDate, lblToDate, dtpToDate });

            // Search Group
            // var grpSearch = new GroupBox
            // {
            //     Text = "Tìm kiếm & Lọc",
            //     Font = new Font("Segoe UI", 10, FontStyle.Bold),
            //     Location = new Point(380, 10),
            //     Size = new Size(400, 90)
            // };

            // var lblSearch = new Label
            // {
            //     Text = "Tìm kiếm:",
            //     Font = new Font("Segoe UI", 9),
            //     Location = new Point(15, 25),
            //     Size = new Size(60, 20)
            // };

            // txtSearch = new TextBox
            // {
            //     Font = new Font("Segoe UI", 9),
            //     Location = new Point(80, 23),
            //     Size = new Size(150, 25),
            //     PlaceholderText = "Mã SP, tên SP..."
            // };
            // txtSearch.TextChanged += OnSearchTextChanged;

            // var lblWarehouse = new Label
            // {
            //     Text = "Kho:",
            //     Font = new Font("Segoe UI", 9),
            //     Location = new Point(15, 55),
            //     Size = new Size(60, 20)
            // };

            // cboWarehouse = new ComboBox
            // {
            //     Font = new Font("Segoe UI", 9),
            //     Location = new Point(80, 53),
            //     Size = new Size(150, 25),
            //     DropDownStyle = ComboBoxStyle.DropDownList
            // };
            // cboWarehouse.SelectedIndexChanged += OnWarehouseSelectionChanged;

            // btnClearFilters = new Button
            // {
            //     Text = "🔄 Xóa bộ lọc",
            //     Font = new Font("Segoe UI", 9),
            //     Location = new Point(250, 23),
            //     Size = new Size(100, 25),
            //     BackColor = Color.FromArgb(149, 165, 166),
            //     ForeColor = Color.White,
            //     FlatStyle = FlatStyle.Flat
            // };
            // btnClearFilters.FlatAppearance.BorderSize = 0;
            // btnClearFilters.Click += OnClearFiltersClick;

            // grpSearch.Controls.AddRange(new Control[] { lblSearch, txtSearch, lblWarehouse, cboWarehouse, btnClearFilters });

            // Action Buttons
            // btnGenerateReport = CreateStyledActionButton("📊 Tạo báo cáo", Color.FromArgb(46, 204, 113));
            // btnGenerateReport.Location = new Point(800, 25);
            // btnGenerateReport.Click += OnGenerateReportClick;

            // btnExportExcel = CreateStyledActionButton("📤 Xuất Excel", Color.FromArgb(52, 152, 219));
            // btnExportExcel.Location = new Point(800, 60);
            // btnExportExcel.Click += OnExportExcelClick;

            // btnPrintReport = CreateStyledActionButton("🖨️ In báo cáo", Color.FromArgb(155, 89, 182));
            // btnPrintReport.Location = new Point(920, 25);
            // btnPrintReport.Click += OnPrintReportClick;

            // btnRefreshData = CreateStyledActionButton("🔄 Làm mới", Color.FromArgb(241, 196, 15));
            // btnRefreshData.Location = new Point(920, 60);
            // btnRefreshData.Click += OnRefreshDataClick;

            // pnlFilter.Controls.AddRange(new Control[] { grpDateRange, grpSearch, btnGenerateReport, btnExportExcel, btnPrintReport, btnRefreshData });

            // Statistics Panel
            var pnlStats = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 120,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(20),
                BackColor = Color.Transparent
            };

            var statsCard1 = CreateStatisticsCard("Tổng giá trị tồn kho", "0 VNĐ", Color.FromArgb(46, 204, 113));
            lblTotalValueAmount = statsCard1.Controls.OfType<Label>().FirstOrDefault(l => l.Font.Bold);

            var statsCard2 = CreateStatisticsCard("Tổng sản phẩm", "0", Color.FromArgb(52, 152, 219));
            lblTotalItemsCount = statsCard2.Controls.OfType<Label>().FirstOrDefault(l => l.Font.Bold);

            var statsCard3 = CreateStatisticsCard("Còn hàng", "0", Color.FromArgb(241, 196, 15));
            lblInStockCount = statsCard3.Controls.OfType<Label>().FirstOrDefault(l => l.Font.Bold);

            var statsCard4 = CreateStatisticsCard("Hết hàng", "0", Color.FromArgb(231, 76, 60));
            lblOutOfStockCount = statsCard4.Controls.OfType<Label>().FirstOrDefault(l => l.Font.Bold);

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

            // Add only remaining controls to form (không có pnlFilter)
            this.Controls.AddRange(new Control[] { dataGridViewInventory, pnlStats, lblFormTitle });
        }

        private Button CreateStyledActionButton(string text, Color backColor)
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

        private Panel CreateStatisticsCard(string title, string value, Color accentColor)
        {
            var card = new Panel
            {
                Width = 350, // Increased from 280 to 350 to accommodate longer Vietnamese text
                Height = 80,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                // Margin = new Padding(20, 0, 20, 0)
                Margin = new Padding(10, 0, 10, 0)
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
                Location = new Point(15, 10),
                Size = new Size(320, 30), // Set fixed size instead of AutoSize to prevent overflow
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false
            };

            var lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(15, 45),
                Size = new Size(325, 25), // Set fixed size instead of AutoSize to prevent overflow
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false
            };

            card.Controls.AddRange(new Control[] { accentLine, lblValue, lblTitle });
            return card;
        }

        private async Task LoadInitialDataAsync()
        {
            try
            {
                // Xóa LoadWarehouseOptionsAsync vì không còn cần thiết
                await GenerateInventoryReportAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task GenerateInventoryReportAsync()
        {
            try
            {
                // Sử dụng ngày mặc định thay vì từ DateTimePicker
                var fromDate = DateTime.Now.AddMonths(-1);
                var toDate = DateTime.Now;

                await Task.Run(() =>
                {
                    _inventoryReportData = _reportRepository.GetInventoryReport(fromDate, toDate);
                    
                    this.Invoke((MethodInvoker)delegate
                    {
                        RefreshInventoryDataGrid(_inventoryReportData);
                        UpdateReportStatistics(_inventoryReportData);
                    });
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo báo cáo: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshInventoryDataGrid(List<ReportRepository.InventoryReportItem> data)
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

        private void UpdateReportStatistics(List<ReportRepository.InventoryReportItem> data)
        {
            if (data == null || !data.Any()) return;

            var totalValue = data.Sum(d => d.GiaTri);
            var totalItems = data.Count;
            var inStock = data.Count(d => d.TonKho > 0);
            var outOfStock = data.Count(d => d.TonKho == 0);

            lblTotalValueAmount.Text = totalValue.ToString("N0") + " VNĐ";
            lblTotalItemsCount.Text = totalItems.ToString();
            lblInStockCount.Text = inStock.ToString();
            lblOutOfStockCount.Text = outOfStock.ToString();
        }

        // Xóa ApplyCurrentFilters method vì không còn filter

        // Xóa tất cả Event Handlers cho các button và filter controls
        // private async void OnGenerateReportClick(object sender, EventArgs e)
        // private void OnExportExcelClick(object sender, EventArgs e)
        // private void OnPrintReportClick(object sender, EventArgs e)
        // private async void OnRefreshDataClick(object sender, EventArgs e)
        // private void OnClearFiltersClick(object sender, EventArgs e)
        // private void OnSearchTextChanged(object sender, EventArgs e)
        // private void OnWarehouseSelectionChanged(object sender, EventArgs e)
    }
}

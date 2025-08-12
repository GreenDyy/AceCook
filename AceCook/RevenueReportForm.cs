using System;
using System.Windows.Forms;
using AceCook.Models;
using AceCook.Repositories;

namespace AceCook
{
    public partial class RevenueReportForm : Form
    {
        private readonly ReportRepository _reportRepository;
        private DateTimePicker dtpFromDate;
        private DateTimePicker dtpToDate;
        private Button btnGenerate;
        private DataGridView dgvReport;

        public RevenueReportForm()
        {
            InitializeComponent();
            _reportRepository = new ReportRepository(new AppDbContext());
            InitializeUI();
        }

        private void InitializeComponent()
        {
            this.Text = "Báo cáo doanh thu";
            this.Size = new System.Drawing.Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Date range controls
            Label lblFromDate = new Label
            {
                Text = "Từ ngày:",
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(80, 20)
            };

            dtpFromDate = new DateTimePicker
            {
                Location = new System.Drawing.Point(100, 20),
                Size = new System.Drawing.Size(150, 20),
                Format = DateTimePickerFormat.Short
            };

            Label lblToDate = new Label
            {
                Text = "Đến ngày:",
                Location = new System.Drawing.Point(270, 20),
                Size = new System.Drawing.Size(80, 20)
            };

            dtpToDate = new DateTimePicker
            {
                Location = new System.Drawing.Point(350, 20),
                Size = new System.Drawing.Size(150, 20),
                Format = DateTimePickerFormat.Short
            };

            btnGenerate = new Button
            {
                Text = "Tạo báo cáo",
                Location = new System.Drawing.Point(520, 20),
                Size = new System.Drawing.Size(100, 25)
            };
            btnGenerate.Click += BtnGenerate_Click;

            // Grid view
            dgvReport = new DataGridView
            {
                Location = new System.Drawing.Point(20, 60),
                Size = new System.Drawing.Size(940, 480),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true
            };

            // Add controls
            this.Controls.AddRange(new Control[] 
            { 
                lblFromDate, dtpFromDate, 
                lblToDate, dtpToDate,
                btnGenerate, dgvReport 
            });
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                var report = _reportRepository.GetRevenueReport(dtpFromDate.Value, dtpToDate.Value);
                dgvReport.DataSource = report.Details;

                MessageBox.Show($"Tổng doanh thu: {report.TotalRevenue:N0} VNĐ", 
                    "Thông tin", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo báo cáo: {ex.Message}", 
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

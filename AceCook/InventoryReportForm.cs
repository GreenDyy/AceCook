using System;
using System.Drawing;
using System.Windows.Forms;
using AceCook.Models;
using AceCook.Repositories;

namespace AceCook
{
    public partial class InventoryReportForm : Form
    {
        private readonly ReportRepository _reportRepository;
        private Label lblFromDate;
        private Label lblToDate;
        private Panel panel2;
        private Button button3;
        private Button button2;
        private GroupBox groupBox1;
        private Button button1;
        private DateTimePicker dateTimePicker2;
        private DateTimePicker dateTimePicker1;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private Button btnGenerate;
        private DataGridView dgvInventory;

        public InventoryReportForm()
        {
            InitializeComponent();
            _reportRepository = new ReportRepository(new AppDbContext());
        }

        private void InitializeComponent()
        {
            lblFromDate = new Label();
            lblToDate = new Label();
            btnGenerate = new Button();
            panel2 = new Panel();
            button3 = new Button();
            button2 = new Button();
            groupBox1 = new GroupBox();
            button1 = new Button();
            dateTimePicker2 = new DateTimePicker();
            dateTimePicker1 = new DateTimePicker();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            panel2.SuspendLayout();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // lblFromDate
            // 
            lblFromDate.Location = new Point(0, 0);
            lblFromDate.Name = "lblFromDate";
            lblFromDate.Size = new Size(100, 23);
            lblFromDate.TabIndex = 0;
            // 
            // lblToDate
            // 
            lblToDate.Location = new Point(0, 0);
            lblToDate.Name = "lblToDate";
            lblToDate.Size = new Size(100, 23);
            lblToDate.TabIndex = 2;
            // 
            // btnGenerate
            // 
            btnGenerate.Location = new Point(0, 0);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new Size(75, 23);
            btnGenerate.TabIndex = 4;
            btnGenerate.Click += BtnGenerate_Click;
            // 
            // panel2
            // 
            panel2.BackColor = Color.FromArgb(107, 111, 213);
            panel2.Controls.Add(button3);
            panel2.Controls.Add(button2);
            panel2.Controls.Add(groupBox1);
            panel2.Controls.Add(label2);
            panel2.Controls.Add(label1);
            panel2.Location = new Point(12, 12);
            panel2.Name = "panel2";
            panel2.Size = new Size(1563, 175);
            panel2.TabIndex = 5;
            // 
            // button3
            // 
            button3.Location = new Point(1168, 98);
            button3.Name = "button3";
            button3.Size = new Size(122, 29);
            button3.TabIndex = 4;
            button3.Text = "In báo cáo";
            button3.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Location = new Point(1168, 35);
            button2.Name = "button2";
            button2.Size = new Size(122, 29);
            button2.TabIndex = 3;
            button2.Text = "Xuất file Excel";
            button2.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(button1);
            groupBox1.Controls.Add(dateTimePicker2);
            groupBox1.Controls.Add(dateTimePicker1);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(label3);
            groupBox1.Location = new Point(392, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(685, 135);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            // 
            // button1
            // 
            button1.BackColor = Color.Blue;
            button1.ForeColor = Color.White;
            button1.Location = new Point(558, 37);
            button1.Name = "button1";
            button1.Size = new Size(94, 53);
            button1.TabIndex = 4;
            button1.Text = "Lọc";
            button1.UseVisualStyleBackColor = false;
            // 
            // dateTimePicker2
            // 
            dateTimePicker2.Location = new Point(166, 81);
            dateTimePicker2.Name = "dateTimePicker2";
            dateTimePicker2.Size = new Size(317, 27);
            dateTimePicker2.TabIndex = 3;
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Location = new Point(166, 26);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(317, 27);
            dateTimePicker1.TabIndex = 2;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.ForeColor = Color.White;
            label4.Location = new Point(13, 86);
            label4.Name = "label4";
            label4.Size = new Size(75, 20);
            label4.TabIndex = 1;
            label4.Text = "Đến ngày:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.ForeColor = Color.White;
            label3.Location = new Point(13, 30);
            label3.Name = "label3";
            label3.Size = new Size(65, 20);
            label3.TabIndex = 0;
            label3.Text = "Từ ngày:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 10.2F, FontStyle.Italic, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.White;
            label2.Location = new Point(38, 102);
            label2.Name = "label2";
            label2.Size = new Size(264, 23);
            label2.TabIndex = 1;
            label2.Text = "Thống kê doanh thu theo thời gian";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.White;
            label1.Location = new Point(34, 49);
            label1.Name = "label1";
            label1.Size = new Size(236, 38);
            label1.TabIndex = 0;
            label1.Text = "Báo cáo Tồn kho";
            // 
            // dgvInventory
            // 
            dgvInventory = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)(dgvInventory)).BeginInit();
            dgvInventory.Location = new Point(12, 200); // Below the panel2
            dgvInventory.Size = new Size(1563, 750);
            dgvInventory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvInventory.AllowUserToAddRows = false;
            dgvInventory.AllowUserToDeleteRows = false;
            dgvInventory.ReadOnly = true;
            dgvInventory.BackgroundColor = Color.White;

            // Add columns
            dgvInventory.Columns.AddRange(new DataGridViewColumn[] {
                new DataGridViewTextBoxColumn 
                { 
                    Name = "MaSP",
                    HeaderText = "MÃ SP",
                    DataPropertyName = "MaSP"
                },
                new DataGridViewTextBoxColumn 
                { 
                    Name = "TenSanPham",
                    HeaderText = "TÊN SẢN PHẨM",
                    DataPropertyName = "TenSanPham"
                },
                new DataGridViewTextBoxColumn 
                { 
                    Name = "Loai",
                    HeaderText = "LOẠI",
                    DataPropertyName = "Loai"
                },
                new DataGridViewTextBoxColumn 
                { 
                    Name = "Gia",
                    HeaderText = "GIÁ",
                    DataPropertyName = "Gia"
                },
                new DataGridViewTextBoxColumn 
                { 
                    Name = "TonKho",
                    HeaderText = "TỒN KHO",
                    DataPropertyName = "TonKho"
                },
                new DataGridViewTextBoxColumn 
                { 
                    Name = "GiaTri",
                    HeaderText = "GIÁ TRỊ",
                    DataPropertyName = "GiaTri"
                },
                new DataGridViewTextBoxColumn 
                { 
                    Name = "ChiTietTheoKho",
                    HeaderText = "CHI TIẾT THEO KHO",
                    DataPropertyName = "ChiTietTheoKho"
                }
            });

            // Add DataGridView to form controls
            Controls.Add(dgvInventory);

            // InventoryReportForm
            // 
            ClientSize = new Size(1587, 1003);
            Controls.Add(panel2);
            Controls.Add(lblFromDate);
            Controls.Add(lblToDate);
            Controls.Add(btnGenerate);
            Name = "InventoryReportForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Báo cáo tồn kho";
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(dgvInventory)).EndInit();
            ResumeLayout(false);
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                var fromDate = dateTimePicker1.Value;
                var toDate = dateTimePicker2.Value;
                
                // Get inventory report data from repository
                var report = _reportRepository.GetInventoryReport(fromDate, toDate);
                dgvInventory.DataSource = report;

                // Format currency columns
                dgvInventory.Columns["Gia"].DefaultCellStyle.Format = "N0";
                dgvInventory.Columns["GiaTri"].DefaultCellStyle.Format = "N0";
                
                // Right-align numeric columns
                dgvInventory.Columns["Gia"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvInventory.Columns["TonKho"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvInventory.Columns["GiaTri"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo báo cáo: {ex.Message}", 
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            BtnGenerate_Click(sender, e); // Reuse the same logic as Generate button
        }
    }
}

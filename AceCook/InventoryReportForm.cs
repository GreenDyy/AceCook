using System;
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
        private GroupBox groupBox5;
        private PictureBox pictureBox3;
        private Label label12;
        private Label label11;
        private GroupBox groupBox4;
        private PictureBox pictureBox2;
        private Label label10;
        private Label label9;
        private GroupBox groupBox3;
        private PictureBox pictureBox5;
        private Label label8;
        private Label label7;
        private GroupBox groupBox2;
        private Label label6;
        private Label label5;
        private PictureBox pictureBox1;
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
            groupBox5 = new GroupBox();
            pictureBox3 = new PictureBox();
            label12 = new Label();
            label11 = new Label();
            groupBox4 = new GroupBox();
            pictureBox2 = new PictureBox();
            label10 = new Label();
            label9 = new Label();
            groupBox3 = new GroupBox();
            pictureBox5 = new PictureBox();
            label8 = new Label();
            label7 = new Label();
            groupBox2 = new GroupBox();
            label6 = new Label();
            label5 = new Label();
            pictureBox1 = new PictureBox();
            dgvInventory = new DataGridView();
            panel2.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvInventory).BeginInit();
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
            // groupBox5
            // 
            groupBox5.Controls.Add(pictureBox3);
            groupBox5.Controls.Add(label12);
            groupBox5.Controls.Add(label11);
            groupBox5.Location = new Point(1244, 226);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(233, 125);
            groupBox5.TabIndex = 6;
            groupBox5.TabStop = false;
            groupBox5.Enter += groupBox5_Enter;
            // 
            // pictureBox3
            // 
            pictureBox3.Location = new Point(17, 37);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(50, 49);
            pictureBox3.TabIndex = 7;
            pictureBox3.TabStop = false;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(84, 67);
            label12.Name = "label12";
            label12.Size = new Size(130, 20);
            label12.TabIndex = 8;
            label12.Text = "Ngày có giao dịch";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label11.Location = new Point(95, 37);
            label11.Name = "label11";
            label11.Size = new Size(24, 28);
            label11.TabIndex = 7;
            label11.Text = "1";
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(pictureBox2);
            groupBox4.Controls.Add(label10);
            groupBox4.Controls.Add(label9);
            groupBox4.Location = new Point(863, 226);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(234, 125);
            groupBox4.TabIndex = 8;
            groupBox4.TabStop = false;
            // 
            // pictureBox2
            // 
            pictureBox2.Location = new Point(20, 38);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(50, 49);
            pictureBox2.TabIndex = 5;
            pictureBox2.TabStop = false;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(91, 66);
            label10.Name = "label10";
            label10.Size = new Size(140, 20);
            label10.TabIndex = 6;
            label10.Text = "Trung bình/hoá đơn";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label9.Location = new Point(91, 38);
            label9.Name = "label9";
            label9.Size = new Size(108, 28);
            label9.TabIndex = 5;
            label9.Text = "960,000 đ";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(pictureBox5);
            groupBox3.Controls.Add(label8);
            groupBox3.Controls.Add(label7);
            groupBox3.Location = new Point(468, 226);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(240, 125);
            groupBox3.TabIndex = 7;
            groupBox3.TabStop = false;
            // 
            // pictureBox5
            // 
            pictureBox5.Location = new Point(19, 38);
            pictureBox5.Name = "pictureBox5";
            pictureBox5.Size = new Size(50, 49);
            pictureBox5.TabIndex = 3;
            pictureBox5.TabStop = false;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(96, 67);
            label8.Name = "label8";
            label8.Size = new Size(102, 20);
            label8.TabIndex = 4;
            label8.Text = "Tổng hoá đơn";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label7.Location = new Point(96, 38);
            label7.Name = "label7";
            label7.Size = new Size(24, 28);
            label7.TabIndex = 3;
            label7.Text = "1";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(label6);
            groupBox2.Controls.Add(label5);
            groupBox2.Controls.Add(pictureBox1);
            groupBox2.Location = new Point(76, 226);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(238, 125);
            groupBox2.TabIndex = 5;
            groupBox2.TabStop = false;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(87, 67);
            label6.Name = "label6";
            label6.Size = new Size(114, 20);
            label6.TabIndex = 2;
            label6.Text = "Tổng doanh thu";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.Location = new Point(87, 38);
            label5.Name = "label5";
            label5.Size = new Size(108, 28);
            label5.TabIndex = 1;
            label5.Text = "960,000 đ";
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(19, 38);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(50, 49);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // dgvInventory
            // 
            dgvInventory.Location = new Point(12, 200);
            dgvInventory.Size = new Size(1563, 750);
            dgvInventory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvInventory.AllowUserToAddRows = false;
            dgvInventory.AllowUserToDeleteRows = false;
            dgvInventory.ReadOnly = true;
            dgvInventory.BackgroundColor = Color.White;
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
            // 
            // InventoryReportForm
            // 
            ClientSize = new Size(1587, 1003);
            Controls.Add(dgvInventory);
            Controls.Add(groupBox5);
            Controls.Add(panel2);
            Controls.Add(groupBox4);
            Controls.Add(lblFromDate);
            Controls.Add(groupBox3);
            Controls.Add(lblToDate);
            Controls.Add(groupBox2);
            Controls.Add(btnGenerate);
            Name = "InventoryReportForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Báo cáo tồn kho";
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvInventory).EndInit();
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

        //groupbox5
        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }
    }
}

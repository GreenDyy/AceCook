namespace AceCook
{
    partial class RevenueReportForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        //cmt test

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
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
            groupBox2 = new GroupBox();
            label6 = new Label();
            label5 = new Label();
            pictureBox1 = new PictureBox();
            groupBox3 = new GroupBox();
            pictureBox5 = new PictureBox();
            label8 = new Label();
            label7 = new Label();
            groupBox4 = new GroupBox();
            pictureBox2 = new PictureBox();
            label10 = new Label();
            label9 = new Label();
            groupBox5 = new GroupBox();
            pictureBox3 = new PictureBox();
            label12 = new Label();
            label11 = new Label();
            panel3 = new Panel();
            panel4 = new Panel();
            groupBox6 = new GroupBox();
            pictureBox4 = new PictureBox();
            label14 = new Label();
            label13 = new Label();
            groupBox7 = new GroupBox();
            pictureBox6 = new PictureBox();
            label15 = new Label();
            label16 = new Label();
            panel1 = new Panel();
            panel2.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
            groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).BeginInit();
            SuspendLayout();
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
            panel2.Size = new Size(1428, 175);
            panel2.TabIndex = 1;
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
            button1.Click += button1_Click;
            // 
            // dateTimePicker2
            // 
            dateTimePicker2.Location = new Point(166, 81);
            dateTimePicker2.Name = "dateTimePicker2";
            dateTimePicker2.Size = new Size(317, 27);
            dateTimePicker2.TabIndex = 3;
            dateTimePicker2.ValueChanged += dateTimePicker2_ValueChanged;
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Location = new Point(166, 26);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(317, 27);
            dateTimePicker1.TabIndex = 2;
            dateTimePicker1.ValueChanged += dateTimePicker1_ValueChanged;
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
            label3.Click += label3_Click;
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
            label2.Click += label2_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.White;
            label1.Location = new Point(34, 49);
            label1.Name = "label1";
            label1.Size = new Size(268, 38);
            label1.TabIndex = 0;
            label1.Text = "Báo cáo Doanh thu";
            label1.Click += label1_Click;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(label6);
            groupBox2.Controls.Add(label5);
            groupBox2.Controls.Add(pictureBox1);
            groupBox2.Location = new Point(12, 206);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(238, 125);
            groupBox2.TabIndex = 2;
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
            label6.Click += label6_Click;
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
            // groupBox3
            // 
            groupBox3.Controls.Add(pictureBox5);
            groupBox3.Controls.Add(label8);
            groupBox3.Controls.Add(label7);
            groupBox3.Location = new Point(404, 206);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(240, 125);
            groupBox3.TabIndex = 3;
            groupBox3.TabStop = false;
            groupBox3.Enter += groupBox3_Enter;
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
            // groupBox4
            // 
            groupBox4.Controls.Add(pictureBox2);
            groupBox4.Controls.Add(label10);
            groupBox4.Controls.Add(label9);
            groupBox4.Location = new Point(799, 206);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(234, 125);
            groupBox4.TabIndex = 4;
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
            label10.Click += label10_Click;
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
            label9.Click += label9_Click;
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(pictureBox3);
            groupBox5.Controls.Add(label12);
            groupBox5.Controls.Add(label11);
            groupBox5.Location = new Point(1180, 206);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(233, 125);
            groupBox5.TabIndex = 3;
            groupBox5.TabStop = false;
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
            label11.Click += label11_Click;
            // 
            // panel3
            // 
            panel3.Location = new Point(12, 450);
            panel3.Name = "panel3";
            panel3.Size = new Size(632, 453);
            panel3.TabIndex = 5;
            // 
            // panel4
            // 
            panel4.Location = new Point(0, 93);
            panel4.Name = "panel4";
            panel4.Size = new Size(544, 300);
            panel4.TabIndex = 6;
            // 
            // groupBox6
            // 
            groupBox6.Controls.Add(pictureBox4);
            groupBox6.Controls.Add(label14);
            groupBox6.Controls.Add(label13);
            groupBox6.Location = new Point(12, 347);
            groupBox6.Name = "groupBox6";
            groupBox6.Size = new Size(632, 95);
            groupBox6.TabIndex = 0;
            groupBox6.TabStop = false;
            // 
            // pictureBox4
            // 
            pictureBox4.Location = new Point(19, 26);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(54, 49);
            pictureBox4.TabIndex = 2;
            pictureBox4.TabStop = false;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label14.Location = new Point(88, 46);
            label14.Name = "label14";
            label14.Size = new Size(324, 20);
            label14.TabIndex = 1;
            label14.Text = "Thống kê doanh thu từ 1/8/2025 đến 31/8/2025";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label13.Location = new Point(92, 23);
            label13.Name = "label13";
            label13.Size = new Size(243, 23);
            label13.TabIndex = 0;
            label13.Text = "Biểu đồ doanh thu theo ngày";
            label13.Click += label13_Click;
            // 
            // groupBox7
            // 
            groupBox7.Controls.Add(pictureBox6);
            groupBox7.Controls.Add(label15);
            groupBox7.Controls.Add(panel4);
            groupBox7.Controls.Add(label16);
            groupBox7.Location = new Point(780, 347);
            groupBox7.Name = "groupBox7";
            groupBox7.Size = new Size(633, 93);
            groupBox7.TabIndex = 3;
            groupBox7.TabStop = false;
            // 
            // pictureBox6
            // 
            pictureBox6.Location = new Point(19, 26);
            pictureBox6.Name = "pictureBox6";
            pictureBox6.Size = new Size(54, 49);
            pictureBox6.TabIndex = 2;
            pictureBox6.TabStop = false;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label15.Location = new Point(92, 46);
            label15.Name = "label15";
            label15.Size = new Size(139, 20);
            label15.TabIndex = 1;
            label15.Text = "Thống kê năm 2025";
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label16.Location = new Point(92, 23);
            label16.Name = "label16";
            label16.Size = new Size(187, 23);
            label16.TabIndex = 0;
            label16.Text = "Doanh thu theo tháng";
            // 
            // panel1
            // 
            panel1.Location = new Point(780, 450);
            panel1.Name = "panel1";
            panel1.Size = new Size(633, 453);
            panel1.TabIndex = 6;
            // 
            // RevenueReportForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1452, 926);
            Controls.Add(groupBox7);
            Controls.Add(panel1);
            Controls.Add(groupBox6);
            Controls.Add(panel3);
            Controls.Add(groupBox5);
            Controls.Add(groupBox4);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(panel2);
            ForeColor = Color.Black;
            Name = "RevenueReportForm";
            Text = "Report";
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            groupBox6.ResumeLayout(false);
            groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            groupBox7.ResumeLayout(false);
            groupBox7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Panel panel2;
        private Label label1;
        private Label label2;
        private GroupBox groupBox1;
        private Label label4;
        private Label label3;
        private DateTimePicker dateTimePicker2;
        private DateTimePicker dateTimePicker1;
        private Button button1;
        private Button button3;
        private Button button2;
        private GroupBox groupBox2;
        private PictureBox pictureBox1;
        private GroupBox groupBox3;
        private GroupBox groupBox4;
        private GroupBox groupBox5;
        private Label label6;
        private Label label5;
        private Label label8;
        private Label label7;
        private Label label10;
        private Label label9;
        private Label label12;
        private Label label11;
        private PictureBox pictureBox5;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private Panel panel3;
        private Panel panel4;
        private GroupBox groupBox6;
        private Label label13;
        private Label label14;
        private PictureBox pictureBox4;
        private GroupBox groupBox7;
        private PictureBox pictureBox6;
        private Label label15;
        private Label label16;
        private Panel panel1;
    }
}
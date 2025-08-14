using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AceCook.Models;
using AceCook.Repositories;

namespace AceCook
{
    public partial class OrderReportForm : Form
    {
        private readonly OrderRepository _orderRepository;
        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpEndDate;
        private Button btnApplyFilter;
        private Button btnReset;
        private Label lblTitle;
        private Label lblDateRange;
        private Panel pnlFilters;
        private Panel pnlSummary;
        private DataGridView dgvOrderStatus;
        private DataGridView dgvTopCustomers;
        private Label lblTotalOrders;
        private Label lblCompletedOrders;
        private Label lblNewOrders;
        private Label lblOrdersByStatus;
        private Label lblTopCustomers;

        public OrderReportForm(OrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
            InitializeComponent(); // Phải gọi trước
            SetupUI();            // Sau đó mới setup UI custom
            InitializeGrids();    // Và khởi tạo grids
            _ = LoadDataAsync(); 
        }

        private void InitializeComponent()
        {
            this.dgvOrderStatus = new System.Windows.Forms.DataGridView();
            this.dgvTopCustomers = new System.Windows.Forms.DataGridView();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.button1 = new System.Windows.Forms.Button();
            
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrderStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTopCustomers)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvOrderStatus
            // 
            this.dgvOrderStatus.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOrderStatus.Location = new System.Drawing.Point(12, 12);
            this.dgvOrderStatus.Name = "dgvOrderStatus";
            this.dgvOrderStatus.Size = new System.Drawing.Size(776, 426);
            this.dgvOrderStatus.TabIndex = 0;
            // 
            // dgvTopCustomers
            // 
            this.dgvTopCustomers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTopCustomers.Location = new System.Drawing.Point(12, 444);
            this.dgvTopCustomers.Name = "dgvTopCustomers";
            this.dgvTopCustomers.Size = new System.Drawing.Size(776, 426);
            this.dgvTopCustomers.TabIndex = 1;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(794, 12);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker1.TabIndex = 2;
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.Location = new System.Drawing.Point(794, 38);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker2.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(794, 64);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Refresh";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // OrderReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1006, 882);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dateTimePicker2);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.dgvTopCustomers);
            this.Controls.Add(this.dgvOrderStatus);
            this.Name = "OrderReportForm";
            this.Text = "Order Report";
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrderStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTopCustomers)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DataGridView dgvOrderStatus;
        private System.Windows.Forms.DataGridView dgvTopCustomers;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.Button button1;
    }
}

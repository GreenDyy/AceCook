using System;
using System.Windows.Forms;

namespace AceCook
{
    public partial class ReportForm : Form
    {
        public ReportForm()
        {
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "Báo cáo";
            this.Size = new System.Drawing.Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Create menu buttons
            Button btnRevenue = new Button
            {
                Text = "Báo cáo doanh thu",
                Size = new System.Drawing.Size(200, 40),
                Location = new System.Drawing.Point(300, 150)
            };
            btnRevenue.Click += (s, e) => OpenRevenueReport();

            Button btnInventory = new Button
            {
                Text = "Báo cáo tồn kho",
                Size = new System.Drawing.Size(200, 40),
                Location = new System.Drawing.Point(300, 250)
            };
            btnInventory.Click += (s, e) => OpenInventoryReport();

            Button btnOrder = new Button
            {
                Text = "Báo cáo đơn hàng",
                Size = new System.Drawing.Size(200, 40),
                Location = new System.Drawing.Point(300, 350)
            };
            btnOrder.Click += (s, e) => OpenOrderReport();

            // Add controls to form
            this.Controls.AddRange(new Control[] { btnRevenue, btnInventory, btnOrder });
        }

        private void OpenRevenueReport()
        {
            var form = new RevenueReportForm();
            form.ShowDialog();
        }

        private void OpenInventoryReport()
        {
            var form = new InventoryReportForm();
            form.ShowDialog();
        }

        private void OpenOrderReport()
        {
            var form = new OrderReportForm();
            form.ShowDialog();
        }
    }
}

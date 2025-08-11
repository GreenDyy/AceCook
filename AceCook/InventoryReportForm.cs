using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AceCook.Models;

namespace AceCook
{
    public partial class InventoryReportForm : Form
    {
        private AppDbContext _context;
        private DataGridView dataGridViewInventory;
        private Button btnGenerate;
        private Button btnExport;
        private Label lblTitle;
        private Label lblTotalItems;

        public InventoryReportForm(AppDbContext context)
        {
            _context = context;
            SetupUI();
            LoadInventoryData();
        }

        private void SetupUI()
        {
            this.Text = "Báo cáo Tồn kho";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);

            // Title
            lblTitle = new Label
            {
                Text = "BÁO CÁO TỒN KHO",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(300, 40),
                Location = new Point(20, 20)
            };

            // Button Panel
            var buttonPanel = new Panel
            {
                Size = new Size(960, 60),
                Location = new Point(20, 70),
                BackColor = Color.White
            };

            btnGenerate = new Button
            {
                Text = "Tạo báo cáo",
                Size = new Size(100, 30),
                Location = new Point(20, 15),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnGenerate.FlatAppearance.BorderSize = 0;
            btnGenerate.Click += BtnGenerate_Click;

            btnExport = new Button
            {
                Text = "Xuất Excel",
                Size = new Size(100, 30),
                Location = new Point(140, 15),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnExport.FlatAppearance.BorderSize = 0;
            btnExport.Click += BtnExport_Click;

            buttonPanel.Controls.Add(btnGenerate);
            buttonPanel.Controls.Add(btnExport);

            // Total Items Label
            lblTotalItems = new Label
            {
                Text = "Tổng số sản phẩm: 0",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113),
                Size = new Size(300, 30),
                Location = new Point(20, 150)
            };

            // DataGridView
            dataGridViewInventory = new DataGridView
            {
                Size = new Size(960, 350),
                Location = new Point(20, 200),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                GridColor = Color.LightGray,
                RowHeadersVisible = false
            };

            // Add controls to form
            this.Controls.Add(lblTitle);
            this.Controls.Add(buttonPanel);
            this.Controls.Add(lblTotalItems);
            this.Controls.Add(dataGridViewInventory);

            // Add shadow effect to button panel
            buttonPanel.Paint += (sender, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, buttonPanel.ClientRectangle,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid);
            };
        }

        private async void LoadInventoryData()
        {
            await GenerateReport();
        }

        private async void BtnGenerate_Click(object sender, EventArgs e)
        {
            await GenerateReport();
        }

        private async Task GenerateReport()
        {
            try
            {
                var inventoryData = await _context.CtTons
                    .Include(c => c.MaSpNavigation)
                    .Include(c => c.MaKhoNavigation)
                    .Select(c => new
                    {
                        TenSanPham = c.MaSpNavigation.TenSp,
                        TenKho = c.MaKhoNavigation.TenKho,
                        SoLuongTon = c.SoLuongTonKho
                    })
                    .OrderBy(x => x.TenSanPham)
                    .ThenBy(x => x.TenKho)
                    .ToListAsync();

                RefreshDataGridView(inventoryData);

                var totalItems = inventoryData.Sum(x => x.SoLuongTon);
                lblTotalItems.Text = $"Tổng số sản phẩm: {totalItems:N0}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo báo cáo: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshDataGridView(dynamic inventoryData)
        {
            dataGridViewInventory.DataSource = null;
            dataGridViewInventory.DataSource = inventoryData;

            // Configure columns
            if (dataGridViewInventory.Columns.Count > 0)
            {
                dataGridViewInventory.Columns["TenSanPham"].HeaderText = "Tên Sản Phẩm";
                dataGridViewInventory.Columns["TenKho"].HeaderText = "Tên Kho";
                dataGridViewInventory.Columns["SoLuongTon"].HeaderText = "Số Lượng Tồn";
    
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng xuất Excel sẽ được phát triển sau!", "Thông báo", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
} 
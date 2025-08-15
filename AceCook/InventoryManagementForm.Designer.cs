namespace AceCook
{
    partial class InventoryManagementForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dataGridViewInventory = new System.Windows.Forms.DataGridView();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.cboWarehouseFilter = new System.Windows.Forms.ComboBox();
            this.cboProductTypeFilter = new System.Windows.Forms.ComboBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblTotalItems = new System.Windows.Forms.Label();
            this.lblTotalValue = new System.Windows.Forms.Label();
            this.pnlSummary = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewInventory)).BeginInit();
            this.pnlSummary.SuspendLayout();
            this.SuspendLayout();
            
            // Call SetupUI to configure all controls
            SetupUI();
            
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewInventory)).EndInit();
            this.pnlSummary.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void SetupUI()
        {
            // Title
            this.lblTitle.AutoSize = false;
            this.lblTitle.Text = "QUẢN LÝ TỒN KHO";
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitle.Height = 300;
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;


            // Filters Panel
            var pnlFilters = new System.Windows.Forms.FlowLayoutPanel
            {
                Dock = System.Windows.Forms.DockStyle.Top,
                Height = 90,
                BackColor = System.Drawing.Color.White,
                BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle,
                Padding = new System.Windows.Forms.Padding(15),
                FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight,
                AutoScroll = true,
                WrapContents = false,
                Margin = new System.Windows.Forms.Padding(0, 200, 0, 150)
            };

            var lblSearch = new System.Windows.Forms.Label
            {
                Text = "Tìm kiếm:",
                Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point),
                AutoSize = true,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Margin = new System.Windows.Forms.Padding(0, 8, 10, 0)
            };

            this.txtSearch.Width = 220; // Increased from 200 to 220 for better text display
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.GraphicsUnit.Point);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(0, 5, 25, 0); // Increased right margin from 20 to 25
            this.txtSearch.TextChanged += new System.EventHandler(this.TxtSearch_TextChanged);

            var lblWarehouse = new System.Windows.Forms.Label
            {
                Text = "Kho hàng:",
                Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point),
                AutoSize = true,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Margin = new System.Windows.Forms.Padding(0, 8, 10, 0)
            };

            this.cboWarehouseFilter.Width = 180; // Increased from 150 to 180 to prevent text truncation
            this.cboWarehouseFilter.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.GraphicsUnit.Point);
            this.cboWarehouseFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboWarehouseFilter.Margin = new System.Windows.Forms.Padding(0, 5, 25, 0); // Increased right margin from 20 to 25
            this.cboWarehouseFilter.Items.Add("Tất cả kho");
            this.cboWarehouseFilter.SelectedIndex = 0;
            this.cboWarehouseFilter.SelectedIndexChanged += new System.EventHandler(this.CboWarehouseFilter_SelectedIndexChanged);

            var lblProductType = new System.Windows.Forms.Label
            {
                Text = "Loại SP:",
                Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point),
                AutoSize = true,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Margin = new System.Windows.Forms.Padding(0, 8, 10, 0)
            };

            this.cboProductTypeFilter.Width = 180; // Increased from 150 to 180 to prevent text truncation
            this.cboProductTypeFilter.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.GraphicsUnit.Point);
            this.cboProductTypeFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProductTypeFilter.Margin = new System.Windows.Forms.Padding(0, 5, 25, 0); // Increased right margin from 20 to 25
            this.cboProductTypeFilter.Items.Add("Tất cả loại");
            this.cboProductTypeFilter.SelectedIndex = 0;
            this.cboProductTypeFilter.SelectedIndexChanged += new System.EventHandler(this.CboProductTypeFilter_SelectedIndexChanged);

            this.btnRefresh.Text = "🔄 Làm mới";
            this.btnRefresh.Width = 200;
            this.btnRefresh.Height = 40;
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(0, 5, 10, 0);
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);

            pnlFilters.Controls.AddRange(new System.Windows.Forms.Control[] { 
                lblSearch, this.txtSearch, lblWarehouse, this.cboWarehouseFilter,
                lblProductType, this.cboProductTypeFilter, this.btnRefresh
            });

            // Actions Panel
            var pnlActions = new System.Windows.Forms.FlowLayoutPanel
            {
                Dock = System.Windows.Forms.DockStyle.Top,
                Height = 70,
                FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight,
                Padding = new System.Windows.Forms.Padding(15),
                BackColor = System.Drawing.Color.Transparent
            };

            var btnViewDetails = CreateActionButton("👁️ Xem chi tiết", System.Drawing.Color.FromArgb(108, 92, 231));
            btnViewDetails.Click += new System.EventHandler(this.BtnViewDetails_Click);

            var btnNhapKho = CreateActionButton("📦 Nhập kho", System.Drawing.Color.FromArgb(46, 204, 113));
            btnNhapKho.Click += new System.EventHandler(this.BtnNhapKho_Click);

            var btnXuatKho = CreateActionButton("📤 Xuất kho", System.Drawing.Color.FromArgb(255, 193, 7));
            btnXuatKho.Click += new System.EventHandler(this.BtnXuatKho_Click);

            pnlActions.Controls.AddRange(new System.Windows.Forms.Control[] { btnViewDetails, btnNhapKho, btnXuatKho });

            // DataGridView
            this.dataGridViewInventory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewInventory.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None; // Changed from Fill to None
            this.dataGridViewInventory.AllowUserToAddRows = false;
            this.dataGridViewInventory.AllowUserToDeleteRows = false;
            this.dataGridViewInventory.ReadOnly = true;
            this.dataGridViewInventory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewInventory.MultiSelect = false;
            this.dataGridViewInventory.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewInventory.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dataGridViewInventory.GridColor = System.Drawing.Color.LightGray;
            this.dataGridViewInventory.RowHeadersVisible = false;
            this.dataGridViewInventory.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dataGridViewInventory.ColumnHeadersHeight = 50;
            this.dataGridViewInventory.RowTemplate.Height = 35; // Reduced from 50 to 35 for better density
            this.dataGridViewInventory.AllowUserToResizeRows = false;
            this.dataGridViewInventory.AllowUserToResizeColumns = true; // Allow users to resize columns

            // Style the DataGridView
            this.dataGridViewInventory.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.GraphicsUnit.Point);
            this.dataGridViewInventory.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.dataGridViewInventory.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.dataGridViewInventory.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.dataGridViewInventory.ColumnHeadersDefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridViewInventory.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            this.dataGridViewInventory.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            
            // Enable word wrap and better text display
            this.dataGridViewInventory.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewInventory.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            
            // Set alternating row colors for better readability
            this.dataGridViewInventory.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(248, 249, 250);

            // Add all to form
            this.Controls.AddRange(new System.Windows.Forms.Control[] { 
                this.dataGridViewInventory, pnlActions, pnlFilters, this.pnlSummary, this.lblTitle 
            });

            // Form properties
            this.BackColor = System.Drawing.Color.FromArgb(248, 249, 250);
            this.ClientSize = new System.Drawing.Size(1600, 900); // Increased from 1400x800 to 1600x900 for better spacing
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quản lý Tồn kho";
        }

        // Helper to create buttons
        private System.Windows.Forms.Button CreateActionButton(string text, System.Drawing.Color backColor)
        {
            var btn = new System.Windows.Forms.Button
            {
                Text = text,
                Width = 250,
                Height = 60,
                Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point),
                BackColor = backColor,
                ForeColor = System.Drawing.Color.White,
                FlatStyle = System.Windows.Forms.FlatStyle.Flat,
                Margin = new System.Windows.Forms.Padding(0, 0, 15, 0),
                Cursor = System.Windows.Forms.Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        // UI Methods moved from InventoryManagementForm.cs
        public void RefreshDataGridView(System.Collections.Generic.List<AceCook.Models.CtTon> inventory)
        {
            // Update current inventory reference
            var form = this as InventoryManagementForm;
            if (form != null)
            {
                // Use reflection to access private field
                var field = typeof(InventoryManagementForm).GetField("_currentInventory", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(form, inventory);
                }
            }

            this.dataGridViewInventory.DataSource = null;
            this.dataGridViewInventory.Columns.Clear();

            // Create custom columns with optimized widths for better readability
            this.dataGridViewInventory.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn
            {
                Name = "MaSp",
                HeaderText = "Mã SP",
                Width = 100,
                AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
            });

            this.dataGridViewInventory.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn
            {
                Name = "TenSp",
                HeaderText = "Tên sản phẩm",
                Width = 280,
                AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
            });

            this.dataGridViewInventory.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn
            {
                Name = "Loai",
                HeaderText = "Loại",
                Width = 140,
                AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
            });

            this.dataGridViewInventory.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn
            {
                Name = "MaKho",
                HeaderText = "Mã kho",
                Width = 100,
                AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
            });

            this.dataGridViewInventory.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn
            {
                Name = "TenKho",
                HeaderText = "Tên kho",
                Width = 200,
                AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
            });

            this.dataGridViewInventory.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn
            {
                Name = "ViTri",
                HeaderText = "Vị trí",
                Width = 280,
                AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
            });

            this.dataGridViewInventory.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn
            {
                Name = "SoLuongTon",
                HeaderText = "Số lượng tồn",
                Width = 140,
                AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
            });

            this.dataGridViewInventory.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn
            {
                Name = "DonGia",
                HeaderText = "Đơn giá",
                Width = 140,
                AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
            });

            this.dataGridViewInventory.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn
            {
                Name = "TrangThai",
                HeaderText = "Trạng thái",
                Width = 120,
                AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
            });

            // Populate data
            foreach (var item in inventory)
            {
                var rowIndex = this.dataGridViewInventory.Rows.Add();
                var row = this.dataGridViewInventory.Rows[rowIndex];

                row.Cells["MaSp"].Value = item.MaSp;
                row.Cells["TenSp"].Value = item.MaSpNavigation?.TenSp ?? "N/A";
                row.Cells["Loai"].Value = item.MaSpNavigation?.Loai ?? "N/A";
                row.Cells["MaKho"].Value = item.MaKho;
                row.Cells["TenKho"].Value = item.MaKhoNavigation?.TenKho ?? "N/A";
                row.Cells["ViTri"].Value = item.MaKhoNavigation?.ViTri ?? "N/A";
                row.Cells["SoLuongTon"].Value = item.SoLuongTonKho ?? 0;
                row.Cells["DonGia"].Value = (item.MaSpNavigation?.Gia ?? 0).ToString("N0") + " VNĐ";
                
                // Style status based on stock level
                var status = GetStockStatus(item.SoLuongTonKho ?? 0);
                row.Cells["TrangThai"].Value = status;
                StyleStatusCell(row.Cells["TrangThai"], status);
            }
        }

        public string GetStockStatus(int stockLevel)
        {
            if (stockLevel == 0) return "Hết hàng";
            if (stockLevel <= 10) return "Sắp hết";
            if (stockLevel <= 50) return "Trung bình";
            return "Còn hàng";
        }

        public void StyleStatusCell(System.Windows.Forms.DataGridViewCell cell, string status)
        {
            switch (status)
            {
                case "Hết hàng":
                    cell.Style.ForeColor = System.Drawing.Color.Red;
                    break;
                case "Sắp hết":
                    cell.Style.ForeColor = System.Drawing.Color.Orange;
                    break;
                case "Trung bình":
                    cell.Style.ForeColor = System.Drawing.Color.Blue;
                    break;
                case "Còn hàng":
                    cell.Style.ForeColor = System.Drawing.Color.Green;
                    break;
            }
        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewInventory;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.ComboBox cboWarehouseFilter;
        private System.Windows.Forms.ComboBox cboProductTypeFilter;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblTotalItems;
        private System.Windows.Forms.Label lblTotalValue;
        private System.Windows.Forms.Panel pnlSummary;
    }
}
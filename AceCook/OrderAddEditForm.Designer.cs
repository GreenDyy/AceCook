namespace AceCook
{
    partial class OrderAddEditForm
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
            lblOrderId = new Label();
            txtOrderId = new TextBox();
            lblCustomer = new Label();
            cboCustomer = new ComboBox();
            lblOrderDate = new Label();
            dtpOrderDate = new DateTimePicker();
            lblDeliveryDate = new Label();
            dtpDeliveryDate = new DateTimePicker();
            lblStatus = new Label();
            cboStatus = new ComboBox();
            grpProductSelection = new GroupBox();
            lblStockInfo = new Label();
            btnAddProduct = new Button();
            btnRemoveProduct = new Button();
            numQuantity = new NumericUpDown();
            lblQuantity = new Label();
            cboProduct = new ComboBox();
            lblProduct = new Label();
            dgvOrderItems = new DataGridView();
            lblTotalAmount = new Label();
            btnSave = new Button();
            btnCancel = new Button();
            grpOrderInfo = new GroupBox();
            grpOrderItems = new GroupBox();
            label1 = new Label();
            txtInventory = new TextBox();
            grpProductSelection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numQuantity).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvOrderItems).BeginInit();
            grpOrderInfo.SuspendLayout();
            grpOrderItems.SuspendLayout();
            SuspendLayout();
            // 
            // lblOrderId
            // 
            lblOrderId.AutoSize = true;
            lblOrderId.Location = new Point(37, 64);
            lblOrderId.Margin = new Padding(6, 0, 6, 0);
            lblOrderId.Name = "lblOrderId";
            lblOrderId.Size = new Size(163, 32);
            lblOrderId.TabIndex = 0;
            lblOrderId.Text = "Mã đơn hàng:";
            // 
            // txtOrderId
            // 
            txtOrderId.Location = new Point(223, 58);
            txtOrderId.Margin = new Padding(6);
            txtOrderId.Name = "txtOrderId";
            txtOrderId.ReadOnly = true;
            txtOrderId.Size = new Size(275, 39);
            txtOrderId.TabIndex = 1;
            // 
            // lblCustomer
            // 
            lblCustomer.AutoSize = true;
            lblCustomer.Location = new Point(37, 149);
            lblCustomer.Margin = new Padding(6, 0, 6, 0);
            lblCustomer.Name = "lblCustomer";
            lblCustomer.Size = new Size(145, 32);
            lblCustomer.TabIndex = 2;
            lblCustomer.Text = "Khách hàng:";
            // 
            // cboCustomer
            // 
            cboCustomer.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCustomer.FormattingEnabled = true;
            cboCustomer.Location = new Point(223, 143);
            cboCustomer.Margin = new Padding(6);
            cboCustomer.Name = "cboCustomer";
            cboCustomer.Size = new Size(461, 40);
            cboCustomer.TabIndex = 3;
            // 
            // lblOrderDate
            // 
            lblOrderDate.AutoSize = true;
            lblOrderDate.Location = new Point(37, 235);
            lblOrderDate.Margin = new Padding(6, 0, 6, 0);
            lblOrderDate.Name = "lblOrderDate";
            lblOrderDate.Size = new Size(116, 32);
            lblOrderDate.TabIndex = 4;
            lblOrderDate.Text = "Ngày đặt:";
            // 
            // dtpOrderDate
            // 
            dtpOrderDate.Format = DateTimePickerFormat.Short;
            dtpOrderDate.Location = new Point(223, 228);
            dtpOrderDate.Margin = new Padding(6);
            dtpOrderDate.Name = "dtpOrderDate";
            dtpOrderDate.Size = new Size(275, 39);
            dtpOrderDate.TabIndex = 5;
            // 
            // lblDeliveryDate
            // 
            lblDeliveryDate.AutoSize = true;
            lblDeliveryDate.Location = new Point(557, 235);
            lblDeliveryDate.Margin = new Padding(6, 0, 6, 0);
            lblDeliveryDate.Name = "lblDeliveryDate";
            lblDeliveryDate.Size = new Size(128, 32);
            lblDeliveryDate.TabIndex = 6;
            lblDeliveryDate.Text = "Ngày giao:";
            // 
            // dtpDeliveryDate
            // 
            dtpDeliveryDate.Format = DateTimePickerFormat.Short;
            dtpDeliveryDate.Location = new Point(743, 228);
            dtpDeliveryDate.Margin = new Padding(6);
            dtpDeliveryDate.Name = "dtpDeliveryDate";
            dtpDeliveryDate.Size = new Size(275, 39);
            dtpDeliveryDate.TabIndex = 7;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(37, 320);
            lblStatus.Margin = new Padding(6, 0, 6, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(125, 32);
            lblStatus.TabIndex = 8;
            lblStatus.Text = "Trạng thái:";
            // 
            // cboStatus
            // 
            cboStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cboStatus.FormattingEnabled = true;
            cboStatus.Items.AddRange(new object[] { "Chờ xử lý", "Đang xử lý", "Đã giao", "Đã hủy" });
            cboStatus.Location = new Point(120, 147);
            cboStatus.Name = "cboStatus";
            cboStatus.Size = new Size(150, 40);
            cboStatus.TabIndex = 9;
            // 
            // grpProductSelection
            // 
            grpProductSelection.Controls.Add(txtInventory);
            grpProductSelection.Controls.Add(label1);
            grpProductSelection.Controls.Add(lblStockInfo);
            grpProductSelection.Controls.Add(btnAddProduct);
            grpProductSelection.Controls.Add(btnRemoveProduct);
            grpProductSelection.Controls.Add(numQuantity);
            grpProductSelection.Controls.Add(lblQuantity);
            grpProductSelection.Controls.Add(cboProduct);
            grpProductSelection.Controls.Add(lblProduct);
            grpProductSelection.Location = new Point(37, 519);
            grpProductSelection.Margin = new Padding(6);
            grpProductSelection.Name = "grpProductSelection";
            grpProductSelection.Padding = new Padding(6);
            grpProductSelection.Size = new Size(1300, 304);
            grpProductSelection.TabIndex = 10;
            grpProductSelection.TabStop = false;
            grpProductSelection.Text = "Chọn sản phẩm";
            // 
            // lblStockInfo
            // 
            lblStockInfo.AutoSize = true;
            lblStockInfo.ForeColor = Color.Blue;
            lblStockInfo.Location = new Point(223, 139);
            lblStockInfo.Margin = new Padding(6, 0, 6, 0);
            lblStockInfo.Name = "lblStockInfo";
            lblStockInfo.Size = new Size(0, 32);
            lblStockInfo.TabIndex = 5;
            // 
            // btnAddProduct
            // 
            btnAddProduct.Location = new Point(743, 128);
            btnAddProduct.Margin = new Padding(6);
            btnAddProduct.Name = "btnAddProduct";
            btnAddProduct.Size = new Size(186, 64);
            btnAddProduct.TabIndex = 4;
            btnAddProduct.Text = "Thêm SP";
            btnAddProduct.UseVisualStyleBackColor = true;
            btnAddProduct.Click += btnAddProduct_Click;
            // 
            // btnRemoveProduct
            // 
            btnRemoveProduct.Location = new Point(955, 123);
            btnRemoveProduct.Margin = new Padding(6);
            btnRemoveProduct.Name = "btnRemoveProduct";
            btnRemoveProduct.Size = new Size(186, 64);
            btnRemoveProduct.TabIndex = 11;
            btnRemoveProduct.Text = "Xóa SP";
            btnRemoveProduct.UseVisualStyleBackColor = true;
            btnRemoveProduct.Click += btnRemoveProduct_Click;
            // 
            // numQuantity
            // 
            numQuantity.Location = new Point(223, 132);
            numQuantity.Margin = new Padding(6);
            numQuantity.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            numQuantity.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numQuantity.Name = "numQuantity";
            numQuantity.Size = new Size(186, 39);
            numQuantity.TabIndex = 3;
            numQuantity.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // lblQuantity
            // 
            lblQuantity.AutoSize = true;
            lblQuantity.Location = new Point(38, 144);
            lblQuantity.Margin = new Padding(6, 0, 6, 0);
            lblQuantity.Name = "lblQuantity";
            lblQuantity.Size = new Size(115, 32);
            lblQuantity.TabIndex = 2;
            lblQuantity.Text = "Số lượng:";
            // 
            // cboProduct
            // 
            cboProduct.DropDownStyle = ComboBoxStyle.DropDownList;
            cboProduct.FormattingEnabled = true;
            cboProduct.Location = new Point(223, 58);
            cboProduct.Margin = new Padding(6);
            cboProduct.Name = "cboProduct";
            cboProduct.Size = new Size(461, 40);
            cboProduct.TabIndex = 1;
            cboProduct.SelectedIndexChanged += CboProduct_SelectedIndexChanged;
            // 
            // lblProduct
            // 
            lblProduct.AutoSize = true;
            lblProduct.Location = new Point(37, 64);
            lblProduct.Margin = new Padding(6, 0, 6, 0);
            lblProduct.Name = "lblProduct";
            lblProduct.Size = new Size(126, 32);
            lblProduct.TabIndex = 0;
            lblProduct.Text = "Sản phẩm:";
            // 
            // dgvOrderItems
            // 
            dgvOrderItems.AllowUserToAddRows = false;
            dgvOrderItems.AllowUserToDeleteRows = false;
            dgvOrderItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvOrderItems.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvOrderItems.Location = new Point(37, 64);
            dgvOrderItems.Margin = new Padding(6);
            dgvOrderItems.MultiSelect = false;
            dgvOrderItems.Name = "dgvOrderItems";
            dgvOrderItems.ReadOnly = true;
            dgvOrderItems.RowHeadersWidth = 82;
            dgvOrderItems.RowTemplate.Height = 25;
            dgvOrderItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvOrderItems.Size = new Size(1263, 427);
            dgvOrderItems.TabIndex = 0;
            // 
            // lblTotalAmount
            // 
            lblTotalAmount.AutoSize = true;
            lblTotalAmount.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTotalAmount.Location = new Point(37, 533);
            lblTotalAmount.Margin = new Padding(6, 0, 6, 0);
            lblTotalAmount.Name = "lblTotalAmount";
            lblTotalAmount.Size = new Size(279, 45);
            lblTotalAmount.TabIndex = 1;
            lblTotalAmount.Text = "Tổng tiền: 0 VNĐ";
            // 
            // btnSave
            // 
            btnSave.Location = new Point(929, 640);
            btnSave.Margin = new Padding(6);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(186, 75);
            btnSave.TabIndex = 2;
            btnSave.Text = "Lưu";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(1151, 640);
            btnCancel.Margin = new Padding(6);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(186, 75);
            btnCancel.TabIndex = 3;
            btnCancel.Text = "Hủy";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // grpOrderInfo
            // 
            grpOrderInfo.Controls.Add(lblStatus);
            grpOrderInfo.Controls.Add(dtpDeliveryDate);
            grpOrderInfo.Controls.Add(lblDeliveryDate);
            grpOrderInfo.Controls.Add(dtpOrderDate);
            grpOrderInfo.Controls.Add(lblOrderDate);
            grpOrderInfo.Controls.Add(cboCustomer);
            grpOrderInfo.Controls.Add(lblCustomer);
            grpOrderInfo.Controls.Add(txtOrderId);
            grpOrderInfo.Controls.Add(lblOrderId);
            grpOrderInfo.Location = new Point(37, 43);
            grpOrderInfo.Margin = new Padding(6);
            grpOrderInfo.Name = "grpOrderInfo";
            grpOrderInfo.Padding = new Padding(6);
            grpOrderInfo.Size = new Size(1411, 405);
            grpOrderInfo.TabIndex = 12;
            grpOrderInfo.TabStop = false;
            grpOrderInfo.Text = "Thông tin đơn hàng";
            // 
            // grpOrderItems
            // 
            grpOrderItems.Controls.Add(btnCancel);
            grpOrderItems.Controls.Add(btnSave);
            grpOrderItems.Controls.Add(lblTotalAmount);
            grpOrderItems.Controls.Add(dgvOrderItems);
            grpOrderItems.Location = new Point(37, 824);
            grpOrderItems.Margin = new Padding(6);
            grpOrderItems.Name = "grpOrderItems";
            grpOrderItems.Padding = new Padding(6);
            grpOrderItems.Size = new Size(1411, 747);
            grpOrderItems.TabIndex = 13;
            grpOrderItems.TabStop = false;
            grpOrderItems.Text = "Danh sách sản phẩm";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(38, 214);
            label1.Name = "label1";
            label1.Size = new Size(107, 32);
            label1.TabIndex = 12;
            label1.Text = "Tồn kho:";
            // 
            // txtInventory
            // 
            txtInventory.Location = new Point(223, 211);
            txtInventory.Name = "txtInventory";
            txtInventory.Size = new Size(200, 39);
            txtInventory.TabIndex = 13;
            // 
            // OrderAddEditForm
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1486, 1782);
            Controls.Add(grpOrderItems);
            Controls.Add(grpOrderInfo);
            Controls.Add(grpProductSelection);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(6);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "OrderAddEditForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Thêm/Sửa đơn hàng";
            FormClosing += OrderAddEditForm_FormClosing;
            grpProductSelection.ResumeLayout(false);
            grpProductSelection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numQuantity).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvOrderItems).EndInit();
            grpOrderInfo.ResumeLayout(false);
            grpOrderInfo.PerformLayout();
            grpOrderItems.ResumeLayout(false);
            grpOrderItems.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblOrderId;
        private System.Windows.Forms.TextBox txtOrderId;
        private System.Windows.Forms.Label lblCustomer;
        private System.Windows.Forms.ComboBox cboCustomer;
        private System.Windows.Forms.Label lblOrderDate;
        private System.Windows.Forms.DateTimePicker dtpOrderDate;
        private System.Windows.Forms.Label lblDeliveryDate;
        private System.Windows.Forms.DateTimePicker dtpDeliveryDate;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ComboBox cboStatus;
        private System.Windows.Forms.GroupBox grpProductSelection;
        private System.Windows.Forms.Label lblProduct;
        private System.Windows.Forms.ComboBox cboProduct;
        private System.Windows.Forms.Label lblQuantity;
        private System.Windows.Forms.NumericUpDown numQuantity;
        private System.Windows.Forms.Button btnAddProduct;
        private System.Windows.Forms.Button btnRemoveProduct;
        private System.Windows.Forms.DataGridView dgvOrderItems;
        private System.Windows.Forms.Label lblTotalAmount;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox grpOrderInfo;
        private System.Windows.Forms.GroupBox grpOrderItems;
        private System.Windows.Forms.Label lblStockInfo;
        private TextBox txtInventory;
        private Label label1;
    }
}
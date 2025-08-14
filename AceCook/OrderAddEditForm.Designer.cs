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
            this.lblOrderId = new System.Windows.Forms.Label();
            this.txtOrderId = new System.Windows.Forms.TextBox();
            this.lblCustomer = new System.Windows.Forms.Label();
            this.cboCustomer = new System.Windows.Forms.ComboBox();
            this.lblOrderDate = new System.Windows.Forms.Label();
            this.dtpOrderDate = new System.Windows.Forms.DateTimePicker();
            this.lblDeliveryDate = new System.Windows.Forms.Label();
            this.dtpDeliveryDate = new System.Windows.Forms.DateTimePicker();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cboStatus = new System.Windows.Forms.ComboBox();
            this.grpProductSelection = new System.Windows.Forms.GroupBox();
            this.lblProduct = new System.Windows.Forms.Label();
            this.cboProduct = new System.Windows.Forms.ComboBox();
            this.lblQuantity = new System.Windows.Forms.Label();
            this.numQuantity = new System.Windows.Forms.NumericUpDown();
            this.btnAddProduct = new System.Windows.Forms.Button();
            this.lblStockInfo = new System.Windows.Forms.Label();
            this.btnRemoveProduct = new System.Windows.Forms.Button();
            this.dgvOrderItems = new System.Windows.Forms.DataGridView();
            this.lblTotalAmount = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpOrderInfo = new System.Windows.Forms.GroupBox();
            this.grpOrderItems = new System.Windows.Forms.GroupBox();
            this.grpProductSelection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numQuantity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrderItems)).BeginInit();
            this.grpOrderInfo.SuspendLayout();
            this.grpOrderItems.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblOrderId
            // 
            this.lblOrderId.AutoSize = true;
            this.lblOrderId.Location = new System.Drawing.Point(20, 30);
            this.lblOrderId.Name = "lblOrderId";
            this.lblOrderId.Size = new System.Drawing.Size(70, 15);
            this.lblOrderId.TabIndex = 0;
            this.lblOrderId.Text = "Mã đơn hàng:";
            // 
            // txtOrderId
            // 
            this.txtOrderId.Location = new System.Drawing.Point(120, 27);
            this.txtOrderId.Name = "txtOrderId";
            this.txtOrderId.ReadOnly = true;
            this.txtOrderId.Size = new System.Drawing.Size(150, 23);
            this.txtOrderId.TabIndex = 1;
            // 
            // lblCustomer
            // 
            this.lblCustomer.AutoSize = true;
            this.lblCustomer.Location = new System.Drawing.Point(20, 70);
            this.lblCustomer.Name = "lblCustomer";
            this.lblCustomer.Size = new System.Drawing.Size(80, 15);
            this.lblCustomer.TabIndex = 2;
            this.lblCustomer.Text = "Khách hàng:";
            // 
            // cboCustomer
            // 
            this.cboCustomer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCustomer.FormattingEnabled = true;
            this.cboCustomer.Location = new System.Drawing.Point(120, 67);
            this.cboCustomer.Name = "cboCustomer";
            this.cboCustomer.Size = new System.Drawing.Size(250, 23);
            this.cboCustomer.TabIndex = 3;
            // 
            // lblOrderDate
            // 
            this.lblOrderDate.AutoSize = true;
            this.lblOrderDate.Location = new System.Drawing.Point(20, 110);
            this.lblOrderDate.Name = "lblOrderDate";
            this.lblOrderDate.Size = new System.Drawing.Size(70, 15);
            this.lblOrderDate.TabIndex = 4;
            this.lblOrderDate.Text = "Ngày đặt:";
            // 
            // dtpOrderDate
            // 
            this.dtpOrderDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpOrderDate.Location = new System.Drawing.Point(120, 107);
            this.dtpOrderDate.Name = "dtpOrderDate";
            this.dtpOrderDate.Size = new System.Drawing.Size(150, 23);
            this.dtpOrderDate.TabIndex = 5;
            // 
            // lblDeliveryDate
            // 
            this.lblDeliveryDate.AutoSize = true;
            this.lblDeliveryDate.Location = new System.Drawing.Point(300, 110);
            this.lblDeliveryDate.Name = "lblDeliveryDate";
            this.lblDeliveryDate.Size = new System.Drawing.Size(80, 15);
            this.lblDeliveryDate.TabIndex = 6;
            this.lblDeliveryDate.Text = "Ngày giao:";
            // 
            // dtpDeliveryDate
            // 
            this.dtpDeliveryDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDeliveryDate.Location = new System.Drawing.Point(400, 107);
            this.dtpDeliveryDate.Name = "dtpDeliveryDate";
            this.dtpDeliveryDate.Size = new System.Drawing.Size(150, 23);
            this.dtpDeliveryDate.TabIndex = 7;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(20, 150);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(60, 15);
            this.lblStatus.TabIndex = 8;
            this.lblStatus.Text = "Trạng thái:";
            // 
            // cboStatus
            // 
            this.cboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStatus.FormattingEnabled = true;
            this.cboStatus.Items.AddRange(new object[] {
            "Chờ xử lý",
            "Đang xử lý",
            "Đã giao",
            "Đã hủy"});
            this.cboStatus.Location = new System.Drawing.Point(120, 147);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(150, 23);
            this.cboStatus.TabIndex = 9;
            // 
            // grpProductSelection
            // 
            this.grpProductSelection.Controls.Add(this.lblStockInfo);
            this.grpProductSelection.Controls.Add(this.btnAddProduct);
            this.grpProductSelection.Controls.Add(this.numQuantity);
            this.grpProductSelection.Controls.Add(this.lblQuantity);
            this.grpProductSelection.Controls.Add(this.cboProduct);
            this.grpProductSelection.Controls.Add(this.lblProduct);
            this.grpProductSelection.Location = new System.Drawing.Point(20, 200);
            this.grpProductSelection.Name = "grpProductSelection";
            this.grpProductSelection.Size = new System.Drawing.Size(560, 100);
            this.grpProductSelection.TabIndex = 10;
            this.grpProductSelection.TabStop = false;
            this.grpProductSelection.Text = "Chọn sản phẩm";
            // 
            // lblProduct
            // 
            this.lblProduct.AutoSize = true;
            this.lblProduct.Location = new System.Drawing.Point(20, 30);
            this.lblProduct.Name = "lblProduct";
            this.lblProduct.Size = new System.Drawing.Size(70, 15);
            this.lblProduct.TabIndex = 0;
            this.lblProduct.Text = "Sản phẩm:";
            // 
            // cboProduct
            // 
            this.cboProduct.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProduct.FormattingEnabled = true;
            this.cboProduct.Location = new System.Drawing.Point(120, 27);
            this.cboProduct.Name = "cboProduct";
            this.cboProduct.Size = new System.Drawing.Size(250, 23);
            this.cboProduct.TabIndex = 1;
            // 
            // lblQuantity
            // 
            this.lblQuantity.AutoSize = true;
            this.lblQuantity.Location = new System.Drawing.Point(20, 65);
            this.lblQuantity.Name = "lblQuantity";
            this.lblQuantity.Size = new System.Drawing.Size(60, 15);
            this.lblQuantity.TabIndex = 2;
            this.lblQuantity.Text = "Số lượng:";
            // 
            // numQuantity
            // 
            this.numQuantity.Location = new System.Drawing.Point(120, 62);
            this.numQuantity.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numQuantity.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numQuantity.Name = "numQuantity";
            this.numQuantity.Size = new System.Drawing.Size(100, 23);
            this.numQuantity.TabIndex = 3;
            this.numQuantity.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // btnAddProduct
            // 
            this.btnAddProduct.Location = new System.Drawing.Point(400, 60);
            this.btnAddProduct.Name = "btnAddProduct";
            this.btnAddProduct.Size = new System.Drawing.Size(100, 30);
            this.btnAddProduct.TabIndex = 4;
            this.btnAddProduct.Text = "Thêm SP";
            this.btnAddProduct.UseVisualStyleBackColor = true;
            this.btnAddProduct.Click += new System.EventHandler(this.btnAddProduct_Click);
            // 
            // lblStockInfo
            // 
            this.lblStockInfo.AutoSize = true;
            this.lblStockInfo.Location = new System.Drawing.Point(120, 65);
            this.lblStockInfo.Name = "lblStockInfo";
            this.lblStockInfo.Size = new System.Drawing.Size(200, 15);
            this.lblStockInfo.TabIndex = 5;
            this.lblStockInfo.Text = "";
            this.lblStockInfo.ForeColor = System.Drawing.Color.Blue;
            // 
            // btnRemoveProduct
            // 
            this.btnRemoveProduct.Location = new System.Drawing.Point(600, 200);
            this.btnRemoveProduct.Name = "btnRemoveProduct";
            this.btnRemoveProduct.Size = new System.Drawing.Size(100, 30);
            this.btnRemoveProduct.TabIndex = 11;
            this.btnRemoveProduct.Text = "Xóa SP";
            this.btnRemoveProduct.UseVisualStyleBackColor = true;
            this.btnRemoveProduct.Click += new System.EventHandler(this.btnRemoveProduct_Click);
            // 
            // dgvOrderItems
            // 
            this.dgvOrderItems.AllowUserToAddRows = false;
            this.dgvOrderItems.AllowUserToDeleteRows = false;
            this.dgvOrderItems.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvOrderItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOrderItems.Location = new System.Drawing.Point(20, 30);
            this.dgvOrderItems.MultiSelect = false;
            this.dgvOrderItems.Name = "dgvOrderItems";
            this.dgvOrderItems.ReadOnly = true;
            this.dgvOrderItems.RowTemplate.Height = 25;
            this.dgvOrderItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvOrderItems.Size = new System.Drawing.Size(680, 200);
            this.dgvOrderItems.TabIndex = 0;
            // 
            // lblTotalAmount
            // 
            this.lblTotalAmount.AutoSize = true;
            this.lblTotalAmount.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTotalAmount.Location = new System.Drawing.Point(20, 250);
            this.lblTotalAmount.Name = "lblTotalAmount";
            this.lblTotalAmount.Size = new System.Drawing.Size(120, 21);
            this.lblTotalAmount.TabIndex = 1;
            this.lblTotalAmount.Text = "Tổng tiền: 0 VNĐ";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(500, 300);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 35);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Lưu";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(620, 300);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 35);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Hủy";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // grpOrderInfo
            // 
            this.grpOrderInfo.Controls.Add(this.lblStatus);
            this.grpOrderInfo.Controls.Add(this.dtpDeliveryDate);
            this.grpOrderInfo.Controls.Add(this.lblDeliveryDate);
            this.grpOrderInfo.Controls.Add(this.dtpOrderDate);
            this.grpOrderInfo.Controls.Add(this.lblOrderDate);
            this.grpOrderInfo.Controls.Add(this.cboCustomer);
            this.grpOrderInfo.Controls.Add(this.lblCustomer);
            this.grpOrderInfo.Controls.Add(this.txtOrderId);
            this.grpOrderInfo.Controls.Add(this.lblOrderId);
            this.grpOrderInfo.Location = new System.Drawing.Point(20, 20);
            this.grpOrderInfo.Name = "grpOrderInfo";
            this.grpOrderInfo.Size = new System.Drawing.Size(760, 190);
            this.grpOrderInfo.TabIndex = 12;
            this.grpOrderInfo.TabStop = false;
            this.grpOrderInfo.Text = "Thông tin đơn hàng";
            // 
            // grpOrderItems
            // 
            this.grpOrderItems.Controls.Add(this.btnCancel);
            this.grpOrderItems.Controls.Add(this.btnSave);
            this.grpOrderItems.Controls.Add(this.lblTotalAmount);
            this.grpOrderItems.Controls.Add(this.dgvOrderItems);
            this.grpOrderItems.Location = new System.Drawing.Point(20, 320);
            this.grpOrderItems.Name = "grpOrderItems";
            this.grpOrderItems.Size = new System.Drawing.Size(760, 350);
            this.grpOrderItems.TabIndex = 13;
            this.grpOrderItems.TabStop = false;
            this.grpOrderItems.Text = "Danh sách sản phẩm";
            // 
            // OrderAddEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 700);
            this.Controls.Add(this.grpOrderItems);
            this.Controls.Add(this.grpOrderInfo);
            this.Controls.Add(this.btnRemoveProduct);
            this.Controls.Add(this.grpProductSelection);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OrderAddEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Thêm/Sửa đơn hàng";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OrderAddEditForm_FormClosing);
            this.grpProductSelection.ResumeLayout(false);
            this.grpProductSelection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numQuantity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrderItems)).EndInit();
            this.grpOrderInfo.ResumeLayout(false);
            this.grpOrderInfo.PerformLayout();
            this.grpOrderItems.ResumeLayout(false);
            this.grpOrderItems.PerformLayout();
            this.ResumeLayout(false);

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
    }
}
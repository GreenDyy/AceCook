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
            txtInventory = new TextBox();
            label1 = new Label();
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
            txtTenNv = new TextBox();
            label2 = new Label();
            lbStatus = new Label();
            grpOrderItems = new GroupBox();
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
            lblOrderId.Location = new Point(20, 30);
            lblOrderId.Name = "lblOrderId";
            lblOrderId.Size = new Size(81, 15);
            lblOrderId.TabIndex = 0;
            lblOrderId.Text = "Mã đơn hàng:";
            // 
            // txtOrderId
            // 
            txtOrderId.Location = new Point(120, 27);
            txtOrderId.Name = "txtOrderId";
            txtOrderId.ReadOnly = true;
            txtOrderId.Size = new Size(150, 23);
            txtOrderId.TabIndex = 1;
            // 
            // lblCustomer
            // 
            lblCustomer.AutoSize = true;
            lblCustomer.Location = new Point(20, 70);
            lblCustomer.Name = "lblCustomer";
            lblCustomer.Size = new Size(73, 15);
            lblCustomer.TabIndex = 2;
            lblCustomer.Text = "Khách hàng:";
            // 
            // cboCustomer
            // 
            cboCustomer.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCustomer.FormattingEnabled = true;
            cboCustomer.Location = new Point(120, 67);
            cboCustomer.Name = "cboCustomer";
            cboCustomer.Size = new Size(250, 23);
            cboCustomer.TabIndex = 3;
            // 
            // lblOrderDate
            // 
            lblOrderDate.AutoSize = true;
            lblOrderDate.Location = new Point(20, 110);
            lblOrderDate.Name = "lblOrderDate";
            lblOrderDate.Size = new Size(58, 15);
            lblOrderDate.TabIndex = 4;
            lblOrderDate.Text = "Ngày đặt:";
            // 
            // dtpOrderDate
            // 
            dtpOrderDate.Format = DateTimePickerFormat.Short;
            dtpOrderDate.Location = new Point(120, 107);
            dtpOrderDate.Name = "dtpOrderDate";
            dtpOrderDate.Size = new Size(150, 23);
            dtpOrderDate.TabIndex = 5;
            // 
            // lblDeliveryDate
            // 
            lblDeliveryDate.AutoSize = true;
            lblDeliveryDate.Location = new Point(300, 110);
            lblDeliveryDate.Name = "lblDeliveryDate";
            lblDeliveryDate.Size = new Size(64, 15);
            lblDeliveryDate.TabIndex = 6;
            lblDeliveryDate.Text = "Ngày giao:";
            // 
            // dtpDeliveryDate
            // 
            dtpDeliveryDate.Format = DateTimePickerFormat.Short;
            dtpDeliveryDate.Location = new Point(400, 107);
            dtpDeliveryDate.Name = "dtpDeliveryDate";
            dtpDeliveryDate.Size = new Size(150, 23);
            dtpDeliveryDate.TabIndex = 7;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(20, 150);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(63, 15);
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
            cboStatus.Size = new Size(150, 23);
            cboStatus.TabIndex = 9;
            // 
            // grpProductSelection
            // 
            grpProductSelection.Controls.Add(txtInventory);
            grpProductSelection.Controls.Add(label1);
            grpProductSelection.Controls.Add(btnAddProduct);
            grpProductSelection.Controls.Add(btnRemoveProduct);
            grpProductSelection.Controls.Add(numQuantity);
            grpProductSelection.Controls.Add(lblQuantity);
            grpProductSelection.Controls.Add(cboProduct);
            grpProductSelection.Controls.Add(lblProduct);
            grpProductSelection.Location = new Point(20, 205);
            grpProductSelection.Name = "grpProductSelection";
            grpProductSelection.Size = new Size(700, 142);
            grpProductSelection.TabIndex = 10;
            grpProductSelection.TabStop = false;
            grpProductSelection.Text = "Chọn sản phẩm";
            // 
            // txtInventory
            // 
            txtInventory.Location = new Point(461, 29);
            txtInventory.Margin = new Padding(2, 1, 2, 1);
            txtInventory.Name = "txtInventory";
            txtInventory.ReadOnly = true;
            txtInventory.Size = new Size(225, 23);
            txtInventory.TabIndex = 13;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(390, 29);
            label1.Margin = new Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new Size(54, 15);
            label1.TabIndex = 12;
            label1.Text = "Tồn kho:";
            // 
            // btnAddProduct
            // 
            btnAddProduct.Location = new Point(25, 99);
            btnAddProduct.Name = "btnAddProduct";
            btnAddProduct.Size = new Size(100, 30);
            btnAddProduct.TabIndex = 4;
            btnAddProduct.Text = "Thêm SP";
            btnAddProduct.UseVisualStyleBackColor = true;
            btnAddProduct.Click += btnAddProduct_Click;
            // 
            // btnRemoveProduct
            // 
            btnRemoveProduct.Location = new Point(154, 99);
            btnRemoveProduct.Name = "btnRemoveProduct";
            btnRemoveProduct.Size = new Size(100, 30);
            btnRemoveProduct.TabIndex = 11;
            btnRemoveProduct.Text = "Xóa SP";
            btnRemoveProduct.UseVisualStyleBackColor = true;
            btnRemoveProduct.Click += btnRemoveProduct_Click;
            // 
            // numQuantity
            // 
            numQuantity.Location = new Point(120, 58);
            numQuantity.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            numQuantity.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numQuantity.Name = "numQuantity";
            numQuantity.Size = new Size(100, 23);
            numQuantity.TabIndex = 3;
            numQuantity.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // lblQuantity
            // 
            lblQuantity.AutoSize = true;
            lblQuantity.Location = new Point(20, 60);
            lblQuantity.Name = "lblQuantity";
            lblQuantity.Size = new Size(57, 15);
            lblQuantity.TabIndex = 2;
            lblQuantity.Text = "Số lượng:";
            // 
            // cboProduct
            // 
            cboProduct.DropDownStyle = ComboBoxStyle.DropDownList;
            cboProduct.FormattingEnabled = true;
            cboProduct.Location = new Point(120, 27);
            cboProduct.Name = "cboProduct";
            cboProduct.Size = new Size(250, 23);
            cboProduct.TabIndex = 1;
            // 
            // lblProduct
            // 
            lblProduct.AutoSize = true;
            lblProduct.Location = new Point(20, 30);
            lblProduct.Name = "lblProduct";
            lblProduct.Size = new Size(63, 15);
            lblProduct.TabIndex = 0;
            lblProduct.Text = "Sản phẩm:";
            // 
            // dgvOrderItems
            // 
            dgvOrderItems.AllowUserToAddRows = false;
            dgvOrderItems.AllowUserToDeleteRows = false;
            dgvOrderItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvOrderItems.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvOrderItems.Location = new Point(20, 30);
            dgvOrderItems.MultiSelect = false;
            dgvOrderItems.Name = "dgvOrderItems";
            dgvOrderItems.ReadOnly = true;
            dgvOrderItems.RowHeadersWidth = 82;
            dgvOrderItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvOrderItems.Size = new Size(680, 200);
            dgvOrderItems.TabIndex = 0;
            // 
            // lblTotalAmount
            // 
            lblTotalAmount.AutoSize = true;
            lblTotalAmount.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTotalAmount.Location = new Point(20, 250);
            lblTotalAmount.Name = "lblTotalAmount";
            lblTotalAmount.Size = new Size(140, 21);
            lblTotalAmount.TabIndex = 1;
            lblTotalAmount.Text = "Tổng tiền: 0 VNĐ";
            // 
            // btnSave
            // 
            btnSave.Location = new Point(500, 300);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(100, 35);
            btnSave.TabIndex = 2;
            btnSave.Text = "Lưu";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(620, 300);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(100, 35);
            btnCancel.TabIndex = 3;
            btnCancel.Text = "Hủy";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // grpOrderInfo
            // 
            grpOrderInfo.Controls.Add(txtTenNv);
            grpOrderInfo.Controls.Add(label2);
            grpOrderInfo.Controls.Add(lbStatus);
            grpOrderInfo.Controls.Add(lblStatus);
            grpOrderInfo.Controls.Add(dtpDeliveryDate);
            grpOrderInfo.Controls.Add(lblDeliveryDate);
            grpOrderInfo.Controls.Add(dtpOrderDate);
            grpOrderInfo.Controls.Add(lblOrderDate);
            grpOrderInfo.Controls.Add(cboCustomer);
            grpOrderInfo.Controls.Add(lblCustomer);
            grpOrderInfo.Controls.Add(txtOrderId);
            grpOrderInfo.Controls.Add(lblOrderId);
            grpOrderInfo.Location = new Point(20, 20);
            grpOrderInfo.Name = "grpOrderInfo";
            grpOrderInfo.Size = new Size(760, 179);
            grpOrderInfo.TabIndex = 12;
            grpOrderInfo.TabStop = false;
            grpOrderInfo.Text = "Thông tin đơn hàng";
            // 
            // txtTenNv
            // 
            txtTenNv.Enabled = false;
            txtTenNv.Location = new Point(466, 67);
            txtTenNv.Margin = new Padding(2, 1, 2, 1);
            txtTenNv.Name = "txtTenNv";
            txtTenNv.Size = new Size(220, 23);
            txtTenNv.TabIndex = 11;
            txtTenNv.TextChanged += txtTenNv_TextChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(391, 69);
            label2.Name = "label2";
            label2.Size = new Size(64, 15);
            label2.TabIndex = 10;
            label2.Text = "Nhân viên:";
            // 
            // lbStatus
            // 
            lbStatus.AutoSize = true;
            lbStatus.Location = new Point(120, 150);
            lbStatus.Name = "lbStatus";
            lbStatus.Size = new Size(70, 15);
            lbStatus.TabIndex = 9;
            lbStatus.Text = "Hoàn thành";
            // 
            // grpOrderItems
            // 
            grpOrderItems.Controls.Add(btnCancel);
            grpOrderItems.Controls.Add(btnSave);
            grpOrderItems.Controls.Add(lblTotalAmount);
            grpOrderItems.Controls.Add(dgvOrderItems);
            grpOrderItems.Location = new Point(20, 353);
            grpOrderItems.Name = "grpOrderItems";
            grpOrderItems.Size = new Size(760, 350);
            grpOrderItems.TabIndex = 13;
            grpOrderItems.TabStop = false;
            grpOrderItems.Text = "Danh sách sản phẩm";
            // 
            // OrderAddEditForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 759);
            Controls.Add(grpOrderItems);
            Controls.Add(grpOrderInfo);
            Controls.Add(grpProductSelection);
            FormBorderStyle = FormBorderStyle.FixedDialog;
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
        private TextBox txtInventory;
        private Label label1;
        private Label lbStatus;
        private TextBox txtTenNv;
        private Label label2;
    }
}
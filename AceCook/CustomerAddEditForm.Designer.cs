namespace AceCook
{
    partial class CustomerAddEditForm
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

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblMaKH;
        private System.Windows.Forms.TextBox txtMaKH;
        private System.Windows.Forms.Label lblTenKH;
        private System.Windows.Forms.TextBox txtTenKH;
        private System.Windows.Forms.Label lblLoaiKH;
        private System.Windows.Forms.ComboBox cmbLoaiKH;
        private System.Windows.Forms.Label lblSDTKH;
        private System.Windows.Forms.TextBox txtSDTKH;
        private System.Windows.Forms.Label lblDiaChiKH;
        private System.Windows.Forms.TextBox txtDiaChiKH;
        private System.Windows.Forms.Label lblEmailKH;
        private System.Windows.Forms.TextBox txtEmailKH;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblTitle = new Label();
            lblMaKH = new Label();
            txtMaKH = new TextBox();
            lblTenKH = new Label();
            txtTenKH = new TextBox();
            lblLoaiKH = new Label();
            cmbLoaiKH = new ComboBox();
            lblSDTKH = new Label();
            txtSDTKH = new TextBox();
            lblDiaChiKH = new Label();
            txtDiaChiKH = new TextBox();
            lblEmailKH = new Label();
            txtEmailKH = new TextBox();
            btnSave = new Button();
            btnCancel = new Button();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Bold);
            lblTitle.Location = new Point(325, 49);
            lblTitle.Margin = new Padding(6, 0, 6, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(455, 51);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Thông tin khách hàng";
            // 
            // lblMaKH
            // 
            lblMaKH.AutoSize = true;
            lblMaKH.Location = new Point(65, 172);
            lblMaKH.Margin = new Padding(6, 0, 6, 0);
            lblMaKH.Name = "lblMaKH";
            lblMaKH.Size = new Size(184, 32);
            lblMaKH.TabIndex = 1;
            lblMaKH.Text = "Mã khách hàng:";
            // 
            // txtMaKH
            // 
            txtMaKH.Location = new Point(325, 165);
            txtMaKH.Margin = new Padding(6, 7, 6, 7);
            txtMaKH.Name = "txtMaKH";
            txtMaKH.Size = new Size(645, 39);
            txtMaKH.TabIndex = 2;
            // 
            // lblTenKH
            // 
            lblTenKH.AutoSize = true;
            lblTenKH.Location = new Point(65, 246);
            lblTenKH.Margin = new Padding(6, 0, 6, 0);
            lblTenKH.Name = "lblTenKH";
            lblTenKH.Size = new Size(188, 32);
            lblTenKH.TabIndex = 3;
            lblTenKH.Text = "Tên khách hàng:";
            // 
            // txtTenKH
            // 
            txtTenKH.Location = new Point(325, 239);
            txtTenKH.Margin = new Padding(6, 7, 6, 7);
            txtTenKH.Name = "txtTenKH";
            txtTenKH.Size = new Size(645, 39);
            txtTenKH.TabIndex = 4;
            // 
            // lblLoaiKH
            // 
            lblLoaiKH.AutoSize = true;
            lblLoaiKH.Location = new Point(65, 320);
            lblLoaiKH.Margin = new Padding(6, 0, 6, 0);
            lblLoaiKH.Name = "lblLoaiKH";
            lblLoaiKH.Size = new Size(188, 32);
            lblLoaiKH.TabIndex = 5;
            lblLoaiKH.Text = "Loại khách hàng:";
            // 
            // cmbLoaiKH
            // 
            cmbLoaiKH.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLoaiKH.FormattingEnabled = true;
            cmbLoaiKH.Items.AddRange(new object[] {
            "Lẻ",
            "Siêu thị",
            "Đại lý"});
            cmbLoaiKH.Location = new Point(325, 313);
            cmbLoaiKH.Margin = new Padding(6, 7, 6, 7);
            cmbLoaiKH.Name = "cmbLoaiKH";
            cmbLoaiKH.Size = new Size(645, 39);
            cmbLoaiKH.TabIndex = 6;
            // 
            // lblSDTKH
            // 
            lblSDTKH.AutoSize = true;
            lblSDTKH.Location = new Point(65, 394);
            lblSDTKH.Margin = new Padding(6, 0, 6, 0);
            lblSDTKH.Name = "lblSDTKH";
            lblSDTKH.Size = new Size(161, 32);
            lblSDTKH.TabIndex = 7;
            lblSDTKH.Text = "Số điện thoại:";
            // 
            // txtSDTKH
            // 
            txtSDTKH.Location = new Point(325, 386);
            txtSDTKH.Margin = new Padding(6, 7, 6, 7);
            txtSDTKH.Name = "txtSDTKH";
            txtSDTKH.Size = new Size(645, 39);
            txtSDTKH.TabIndex = 8;
            // 
            // lblDiaChiKH
            // 
            lblDiaChiKH.AutoSize = true;
            lblDiaChiKH.Location = new Point(65, 468);
            lblDiaChiKH.Margin = new Padding(6, 0, 6, 0);
            lblDiaChiKH.Name = "lblDiaChiKH";
            lblDiaChiKH.Size = new Size(92, 32);
            lblDiaChiKH.TabIndex = 9;
            lblDiaChiKH.Text = "Địa chỉ:";
            // 
            // txtDiaChiKH
            // 
            txtDiaChiKH.Location = new Point(325, 460);
            txtDiaChiKH.Margin = new Padding(6, 7, 6, 7);
            txtDiaChiKH.Multiline = true;
            txtDiaChiKH.Name = "txtDiaChiKH";
            txtDiaChiKH.Size = new Size(645, 142);
            txtDiaChiKH.TabIndex = 10;
            // 
            // lblEmailKH
            // 
            lblEmailKH.AutoSize = true;
            lblEmailKH.Location = new Point(65, 640);
            lblEmailKH.Margin = new Padding(6, 0, 6, 0);
            lblEmailKH.Name = "lblEmailKH";
            lblEmailKH.Size = new Size(76, 32);
            lblEmailKH.TabIndex = 11;
            lblEmailKH.Text = "Email:";
            // 
            // txtEmailKH
            // 
            txtEmailKH.Location = new Point(325, 633);
            txtEmailKH.Margin = new Padding(6, 7, 6, 7);
            txtEmailKH.Name = "txtEmailKH";
            txtEmailKH.Size = new Size(645, 39);
            txtEmailKH.TabIndex = 12;
            // 
            // btnSave
            // 
            btnSave.BackColor = Color.FromArgb(0, 123, 255);
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.ForeColor = Color.White;
            btnSave.Location = new Point(585, 738);
            btnSave.Margin = new Padding(6, 7, 6, 7);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(162, 74);
            btnSave.TabIndex = 13;
            btnSave.Text = "Lưu";
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += btnSave_Click;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.FromArgb(108, 117, 125);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.ForeColor = Color.White;
            btnCancel.Location = new Point(802, 738);
            btnCancel.Margin = new Padding(6, 7, 6, 7);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(162, 74);
            btnCancel.TabIndex = 14;
            btnCancel.Text = "Hủy";
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += btnCancel_Click;
            // 
            // CustomerAddEditForm
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1083, 886);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Controls.Add(txtEmailKH);
            Controls.Add(lblEmailKH);
            Controls.Add(txtDiaChiKH);
            Controls.Add(lblDiaChiKH);
            Controls.Add(txtSDTKH);
            Controls.Add(lblSDTKH);
            Controls.Add(cmbLoaiKH);
            Controls.Add(lblLoaiKH);
            Controls.Add(txtTenKH);
            Controls.Add(lblTenKH);
            Controls.Add(txtMaKH);
            Controls.Add(lblMaKH);
            Controls.Add(lblTitle);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(6, 7, 6, 7);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CustomerAddEditForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Khách hàng";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AceCook
{
    public partial class SupplierForm : Form
    {
        private Label lblTitle;
        private Button btnAddSupplier;

        private void InitializeComponent()
        {
            lblTitle = new Label();
            btnAddSupplier = new Button();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitle.Location = new Point(20, 19);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(363, 46);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Quản lý nhà cung cấp";
            // 
            // btnAddSupplier
            // 
            // btnAddSupplier.BackColor = Color.MediumSeaGreen;
            // btnAddSupplier.FlatStyle = FlatStyle.Flat;
            // btnAddSupplier.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            // btnAddSupplier.ForeColor = Color.White;
            // btnAddSupplier.Location = new Point(1150, 25);
            // btnAddSupplier.Name = "btnAddSupplier";
            // btnAddSupplier.Size = new Size(200, 40);
            // btnAddSupplier.TabIndex = 1;
            // btnAddSupplier.Text = "+ Thêm nhà cung cấp";
            // btnAddSupplier.UseVisualStyleBackColor = false;
            // 
            // SupplierForm
            // 
            ClientSize = new Size(1400, 800);
            Controls.Add(lblTitle);
            Controls.Add(btnAddSupplier);
            Name = "SupplierForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Quản lý nhà cung cấp";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}

using System;
using System.Drawing;
using System.Windows.Forms;

namespace AceCook
{
    public partial class SupplierForm : Form
    {
        private Button btnAddSupplier;

        private void InitializeComponent()
        {
            btnAddSupplier = new Button();
            SuspendLayout();
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
            Controls.Add(btnAddSupplier);
            Name = "SupplierForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Quản lý nhà cung cấp";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}

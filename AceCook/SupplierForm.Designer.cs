using System;
using System.Drawing;
using System.Windows.Forms;

namespace AceCook
{
    public partial class SupplierForm : Form
    {
        private Label lblTitle;
        private Button btnAddSupplier;
        private DataGridView dgvSuppliers;

        private void InitializeComponent()
        {
            lblTitle = new Label();
            btnAddSupplier = new Button();
            dgvSuppliers = new DataGridView();
            colView = new DataGridViewButtonColumn();
            colEdit = new DataGridViewButtonColumn();
            colDelete = new DataGridViewButtonColumn();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn5 = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)dgvSuppliers).BeginInit();
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
            btnAddSupplier.BackColor = Color.MediumSeaGreen;
            btnAddSupplier.FlatStyle = FlatStyle.Flat;
            btnAddSupplier.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnAddSupplier.ForeColor = Color.White;
            btnAddSupplier.Location = new Point(1150, 25);
            btnAddSupplier.Name = "btnAddSupplier";
            btnAddSupplier.Size = new Size(200, 40);
            btnAddSupplier.TabIndex = 1;
            btnAddSupplier.Text = "+ Thêm nhà cung cấp";
            btnAddSupplier.UseVisualStyleBackColor = false;
            // 
            // dgvSuppliers
            // 
            dgvSuppliers.AllowUserToAddRows = false;
            dgvSuppliers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSuppliers.ColumnHeadersHeight = 29;
            dgvSuppliers.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2, dataGridViewTextBoxColumn3, dataGridViewTextBoxColumn4, dataGridViewTextBoxColumn5, colView, colEdit, colDelete });
            dgvSuppliers.Font = new Font("Segoe UI", 10F);
            dgvSuppliers.Location = new Point(20, 80);
            dgvSuppliers.Name = "dgvSuppliers";
            dgvSuppliers.RowHeadersWidth = 51;
            dgvSuppliers.RowTemplate.Height = 40;
            dgvSuppliers.Size = new Size(1330, 600);
            dgvSuppliers.TabIndex = 2;
            // 
            // colView
            // 
            colView.HeaderText = "Xem";
            colView.MinimumWidth = 6;
            colView.Name = "colView";
            colView.Text = "👁";
            colView.UseColumnTextForButtonValue = true;
            // 
            // colEdit
            // 
            colEdit.HeaderText = "Sửa";
            colEdit.MinimumWidth = 6;
            colEdit.Name = "colEdit";
            colEdit.Text = "✏";
            colEdit.UseColumnTextForButtonValue = true;
            // 
            // colDelete
            // 
            colDelete.HeaderText = "Xóa";
            colDelete.MinimumWidth = 6;
            colDelete.Name = "colDelete";
            colDelete.Text = "🗑";
            colDelete.UseColumnTextForButtonValue = true;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.HeaderText = "Mã NCC";
            dataGridViewTextBoxColumn1.MinimumWidth = 6;
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewTextBoxColumn2.HeaderText = "Tên nhà cung cấp";
            dataGridViewTextBoxColumn2.MinimumWidth = 6;
            dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewTextBoxColumn3.HeaderText = "Số điện thoại";
            dataGridViewTextBoxColumn3.MinimumWidth = 6;
            dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            // 
            // dataGridViewTextBoxColumn4
            // 
            dataGridViewTextBoxColumn4.HeaderText = "Email";
            dataGridViewTextBoxColumn4.MinimumWidth = 6;
            dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            // 
            // dataGridViewTextBoxColumn5
            // 
            dataGridViewTextBoxColumn5.HeaderText = "Địa chỉ";
            dataGridViewTextBoxColumn5.MinimumWidth = 6;
            dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            // 
            // SupplierForm
            // 
            ClientSize = new Size(1400, 800);
            Controls.Add(lblTitle);
            Controls.Add(btnAddSupplier);
            Controls.Add(dgvSuppliers);
            Name = "SupplierForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Quản lý nhà cung cấp";
            ((System.ComponentModel.ISupportInitialize)dgvSuppliers).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private DataGridViewButtonColumn colView;
        private DataGridViewButtonColumn colEdit;
        private DataGridViewButtonColumn colDelete;
    }
}

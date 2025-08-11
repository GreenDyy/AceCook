using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AceCook.Models;
using AceCook.Repositories;

namespace AceCook
{
    public partial class CustomerManagementForm : Form
    {
        private CustomerRepository _customerRepository;
        private DataGridView dataGridViewCustomers;
        private TextBox txtSearch;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnRefresh;
        private Label lblTitle;
        private List<Khachhang> _customers;

        public CustomerManagementForm(AppDbContext context)
        {
            _customerRepository = new CustomerRepository(context);
            SetupUI();
            LoadCustomers();
        }

        private void SetupUI()
        {
            this.Text = "Quản lý Khách hàng";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);

            // Title
            lblTitle = new Label
            {
                Text = "QUẢN LÝ KHÁCH HÀNG",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(300, 40),
                Location = new Point(20, 20)
            };

            // Search Panel
            var searchPanel = new Panel
            {
                Size = new Size(960, 60),
                Location = new Point(20, 70),
                BackColor = Color.White
            };

            var lblSearch = new Label
            {
                Text = "Tìm kiếm:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(80, 25),
                Location = new Point(20, 20)
            };

            txtSearch = new TextBox
            {
                Size = new Size(200, 25),
                Location = new Point(100, 18),
                Font = new Font("Segoe UI", 10)
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;

            btnRefresh = new Button
            {
                Text = "Làm mới",
                Size = new Size(80, 30),
                Location = new Point(320, 15),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += BtnRefresh_Click;

            searchPanel.Controls.Add(lblSearch);
            searchPanel.Controls.Add(txtSearch);
            searchPanel.Controls.Add(btnRefresh);

            // Button Panel
            var buttonPanel = new Panel
            {
                Size = new Size(960, 50),
                Location = new Point(20, 140),
                BackColor = Color.Transparent
            };

            btnAdd = new Button
            {
                Text = "Thêm mới",
                Size = new Size(100, 35),
                Location = new Point(0, 0),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button
            {
                Text = "Chỉnh sửa",
                Size = new Size(100, 35),
                Location = new Point(120, 0),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(241, 196, 15),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnEdit.FlatAppearance.BorderSize = 0;
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button
            {
                Text = "Xóa",
                Size = new Size(100, 35),
                Location = new Point(240, 0),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += BtnDelete_Click;

            buttonPanel.Controls.Add(btnAdd);
            buttonPanel.Controls.Add(btnEdit);
            buttonPanel.Controls.Add(btnDelete);

            // DataGridView
            dataGridViewCustomers = new DataGridView
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
            this.Controls.Add(searchPanel);
            this.Controls.Add(buttonPanel);
            this.Controls.Add(dataGridViewCustomers);

            // Add shadow effect to search panel
            searchPanel.Paint += (sender, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, searchPanel.ClientRectangle,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid);
            };
        }

        private async void LoadCustomers()
        {
            try
            {
                _customers = await _customerRepository.GetAllCustomersAsync();
                RefreshDataGridView(_customers);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu khách hàng: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshDataGridView(System.Collections.Generic.List<Khachhang> customers)
        {
            dataGridViewCustomers.DataSource = null;
            dataGridViewCustomers.DataSource = customers;

            // Configure columns
            if (dataGridViewCustomers.Columns.Count > 0)
            {
                dataGridViewCustomers.Columns["MaKh"].HeaderText = "Mã KH";
                dataGridViewCustomers.Columns["TenKh"].HeaderText = "Tên Khách Hàng";
                dataGridViewCustomers.Columns["DiaChiKh"].HeaderText = "Địa Chỉ";
                dataGridViewCustomers.Columns["Sdtkh"].HeaderText = "Số Điện Thoại";
                dataGridViewCustomers.Columns["EmailKh"].HeaderText = "Email";

                // Hide navigation properties
                dataGridViewCustomers.Columns["Dondathangs"].Visible = false;
            }
        }

        private async void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text;
            if (!string.IsNullOrEmpty(searchText))
            {
                var filteredCustomers = await _customerRepository.SearchCustomersAsync(searchText);
                RefreshDataGridView(filteredCustomers);
            }
            else
            {
                if (_customers != null)
                {
                    RefreshDataGridView(_customers);
                }
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            LoadCustomers();
        }

        private async void BtnAdd_Click(object sender, EventArgs e)
        {
            var addForm = new CustomerAddEditForm();
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    bool success = await _customerRepository.AddCustomerAsync(addForm.Customer);
                    if (success)
                    {
                        MessageBox.Show("Thêm khách hàng thành công!", "Thông báo", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadCustomers();
                    }
                    else
                    {
                        MessageBox.Show("Lỗi khi thêm khách hàng!", "Lỗi", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi thêm khách hàng: {ex.Message}", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridViewCustomers.SelectedRows.Count > 0)
            {
                var selectedCustomer = dataGridViewCustomers.SelectedRows[0].DataBoundItem as Khachhang;
                if (selectedCustomer != null)
                {
                    var editForm = new CustomerAddEditForm(selectedCustomer);
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            bool success = await _customerRepository.UpdateCustomerAsync(editForm.Customer);
                            if (success)
                            {
                                MessageBox.Show("Cập nhật khách hàng thành công!", "Thông báo", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadCustomers();
                            }
                            else
                            {
                                MessageBox.Show("Lỗi khi cập nhật khách hàng!", "Lỗi", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Lỗi khi cập nhật khách hàng: {ex.Message}", "Lỗi", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một khách hàng để chỉnh sửa", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewCustomers.SelectedRows.Count > 0)
            {
                var selectedCustomer = dataGridViewCustomers.SelectedRows[0].DataBoundItem as Khachhang;
                if (selectedCustomer != null)
                {
                    var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa khách hàng '{selectedCustomer.TenKh}'?", 
                        "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    
                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            bool success = await _customerRepository.DeleteCustomerAsync(selectedCustomer.MaKh);
                            if (success)
                            {
                                MessageBox.Show("Xóa khách hàng thành công!", "Thông báo", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadCustomers();
                            }
                            else
                            {
                                MessageBox.Show("Lỗi khi xóa khách hàng!", "Lỗi", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Lỗi khi xóa khách hàng: {ex.Message}", "Lỗi", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một khách hàng để xóa", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
} 
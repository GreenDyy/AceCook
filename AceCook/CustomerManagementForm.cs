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
            InitializeComponent();
            SetupUI();
            LoadCustomers();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.Text = "Quản lý Khách hàng";
            this.Size = new Size(1400, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            
            this.ResumeLayout(false);
        }

        private void SetupUI()
        {
            // Title
            lblTitle = new Label
            {
                Text = "QUẢN LÝ KHÁCH HÀNG",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Dock = DockStyle.Top,
                Height = 70,
                TextAlign = ContentAlignment.MiddleLeft,
            };

            // Search Panel
            var pnlSearch = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15),
                FlowDirection = FlowDirection.LeftToRight,
                AutoScroll = true,
                WrapContents = false,
                Margin = new Padding(0, 200, 0, 150)
            };

            var lblSearch = new Label
            {
                Text = "Tìm kiếm:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 8, 10, 0)
            };

            txtSearch = new TextBox
            {
                Width = 300,
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "Mã KH, tên KH, số điện thoại, email...",
                Margin = new Padding(0, 5, 20, 0)
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;

            //var btnSearch = new Button
            //{
            //    Text = "🔍 Tìm kiếm",
            //    Width = 120,
            //    Height = 35,
            //    Font = new Font("Segoe UI", 9, FontStyle.Bold),
            //    BackColor = Color.FromArgb(52, 152, 219),
            //    ForeColor = Color.White,
            //    FlatStyle = FlatStyle.Flat,
            //    Margin = new Padding(0, 5, 10, 0)
            //};
            //btnSearch.FlatAppearance.BorderSize = 0;
            //btnSearch.Click += BtnSearch_Click;

            var btnReset = new Button
            {
                Text = "🔄 Làm mới",
                Width = 200,
                Height = 35,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(95, 95, 95),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 5, 10, 0)
            };
            btnReset.FlatAppearance.BorderSize = 0;
            btnReset.Click += BtnReset_Click;

            pnlSearch.Controls.AddRange(new Control[] {
                lblSearch, txtSearch, btnReset
            });

            // Actions Panel
            var pnlActions = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 70,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(15),
                BackColor = Color.Transparent
            };

            btnAdd = CreateActionButton("➕ Thêm khách hàng mới", Color.FromArgb(46, 204, 113));
            btnAdd.Click += BtnAdd_Click;

            btnRefresh = CreateActionButton("🔄 Làm mới dữ liệu", Color.FromArgb(52, 152, 219));
            btnRefresh.Click += BtnRefresh_Click;

            var btnViewDetails = CreateActionButton("👁️ Xem chi tiết", Color.FromArgb(108, 92, 231));
            btnViewDetails.Click += BtnViewDetails_Click;

            btnEdit = CreateActionButton("✏️ Chỉnh sửa khách hàng", Color.FromArgb(255, 193, 7));
            btnEdit.Click += BtnEdit_Click;

            btnDelete = CreateActionButton("🗑️ Xóa khách hàng", Color.FromArgb(231, 76, 60));
            btnDelete.Click += BtnDelete_Click;

            pnlActions.Controls.AddRange(new Control[] { btnAdd, btnRefresh, btnViewDetails, btnEdit, btnDelete });

            // DataGridView
            dataGridViewCustomers = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                GridColor = Color.LightGray,
                RowHeadersVisible = false,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersHeight = 50,
                RowTemplate = { Height = 50 }
            };

            dataGridViewCustomers.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dataGridViewCustomers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridViewCustomers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dataGridViewCustomers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridViewCustomers.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCustomers.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            dataGridViewCustomers.DefaultCellStyle.SelectionForeColor = Color.White;

            // Add all to form
            this.Controls.AddRange(new Control[] { dataGridViewCustomers, pnlActions, pnlSearch, lblTitle });
        }

        // Helper to create buttons
        private Button CreateActionButton(string text, Color backColor)
        {
            var btn = new Button
            {
                Text = text,
                Width = 250,
                Height = 60,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 0, 15, 0),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
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
            dataGridViewCustomers.Columns.Clear();

            // Create custom columns
            dataGridViewCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MaKh",
                HeaderText = "Mã KH",
                Width = 100
            });

            dataGridViewCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenKh",
                HeaderText = "Tên Khách Hàng",
                Width = 200
            });

            dataGridViewCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DiaChiKh",
                HeaderText = "Địa Chỉ",
                Width = 250
            });

            dataGridViewCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Sdtkh",
                HeaderText = "Số Điện Thoại",
                Width = 150
            });

            dataGridViewCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "EmailKh",
                HeaderText = "Email",
                Width = 200
            });

            dataGridViewCustomers.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "ViewDetails",
                HeaderText = "Xem chi tiết",
                Width = 100,
                Text = "👁️ Xem",
                UseColumnTextForButtonValue = true
            });

            // Populate data
            foreach (var customer in customers)
            {
                var rowIndex = dataGridViewCustomers.Rows.Add();
                var row = dataGridViewCustomers.Rows[rowIndex];

                row.Cells["MaKh"].Value = customer.MaKh;
                row.Cells["TenKh"].Value = customer.TenKh;
                row.Cells["DiaChiKh"].Value = customer.DiaChiKh;
                row.Cells["Sdtkh"].Value = customer.Sdtkh;
                row.Cells["EmailKh"].Value = customer.EmailKh;
            }

            // Handle button clicks
            dataGridViewCustomers.CellClick += DataGridViewCustomers_CellClick;
        }

        private async void DataGridViewCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dataGridViewCustomers.Columns["ViewDetails"].Index)
            {
                try
                {
                    var customerId = dataGridViewCustomers.Rows[e.RowIndex].Cells["MaKh"].Value.ToString();
                    var customer = await _customerRepository.GetCustomerByIdAsync(customerId);
                    if (customer != null)
                    {
                        ViewCustomerDetails(customer);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xem chi tiết: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ViewCustomerDetails(Khachhang customer)
        {
            try
            {
                var viewForm = new CustomerAddEditForm(_customerRepository, FormMode.View, customer);
                viewForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form xem chi tiết: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnSearch_Click(object sender, EventArgs e)
        {
            await ApplySearch();
        }

        private async void BtnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            await ApplySearch();
        }

        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            LoadCustomers();
        }

        private async Task ApplySearch()
        {
            try
            {
                string searchText = txtSearch.Text.Trim();
                if (!string.IsNullOrEmpty(searchText))
                {
                    var filteredCustomers = await _customerRepository.SearchCustomersAsync(searchText);
                    RefreshDataGridView(filteredCustomers);
                }
                else
                {
                    LoadCustomers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnViewDetails_Click(object sender, EventArgs e)
        {
            if (dataGridViewCustomers.SelectedRows.Count > 0)
            {
                try
                {
                    // Get the selected customer ID from the first column
                    var selectedRow = dataGridViewCustomers.SelectedRows[0];
                    var customerId = selectedRow.Cells["MaKh"].Value?.ToString();
                    
                    if (!string.IsNullOrEmpty(customerId))
                    {
                        var customer = await _customerRepository.GetCustomerByIdAsync(customerId);
                        if (customer != null)
                        {
                            ViewCustomerDetails(customer);
                        }
                        else
                        {
                            MessageBox.Show("Không thể tải thông tin khách hàng!", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không thể lấy thông tin khách hàng đã chọn!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi tải thông tin khách hàng: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một khách hàng để xem chi tiết!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void BtnAdd_Click(object sender, EventArgs e)
        {
            var addForm = new CustomerAddEditForm(_customerRepository, FormMode.Add);
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("Thêm khách hàng thành công!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadCustomers();
            }
        }

        private async void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridViewCustomers.SelectedRows.Count > 0)
            {
                var selectedCustomer = dataGridViewCustomers.SelectedRows[0].DataBoundItem as Khachhang;
                if (selectedCustomer != null)
                {
                    var editForm = new CustomerAddEditForm(_customerRepository, FormMode.Edit, selectedCustomer);
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        MessageBox.Show("Cập nhật khách hàng thành công!", "Thông báo", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadCustomers();
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
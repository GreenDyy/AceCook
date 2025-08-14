using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using AceCook.Models;
using AceCook.Repositories;

namespace AceCook
{
    public partial class OrderManagementForm : Form
    {
        private readonly AppDbContext _context;
        private readonly OrderRepository _orderRepository;
        private DataGridView dataGridViewOrders;
        private TextBox txtSearch;
        private ComboBox cboStatusFilter;
        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpEndDate;
        private Button btnSearch;
        private Button btnReset;
        private Button btnCreateOrder;
        private Button btnRefresh;
        private Label lblTitle;
        private Label lblSearch;
        private Label lblStatusFilter;
        private Label lblDateRange;
        private Panel pnlFilters;
        private Panel pnlActions;

        public OrderManagementForm(AppDbContext context)
        {
            _context = context;
            _orderRepository = new OrderRepository(context);
            InitializeComponent();
            SetupUI();
            LoadOrders();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.Text = "Quản lý Đơn hàng";
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
                Text = "QUẢN LÝ ĐƠN HÀNG",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(800, 50),
                Location = new Point(30, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Filters Panel
            pnlFilters = new Panel
            {
                Size = new Size(1340, 80),
                Location = new Point(30, 90),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Search controls
            lblSearch = new Label
            {
                Text = "Tìm kiếm:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(80, 25),
                Location = new Point(20, 15),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtSearch = new TextBox
            {
                Size = new Size(200, 30),
                Location = new Point(110, 12),
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "Mã đơn hàng, tên KH, trạng thái..."
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;

            // Status filter
            lblStatusFilter = new Label
            {
                Text = "Trạng thái:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(80, 25),
                Location = new Point(330, 15),
                TextAlign = ContentAlignment.MiddleLeft
            };

            cboStatusFilter = new ComboBox
            {
                Size = new Size(150, 30),
                Location = new Point(420, 12),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboStatusFilter.Items.AddRange(new object[] { "Tất cả", "Chờ xử lý", "Đang xử lý", "Đã giao", "Đã hủy" });
            cboStatusFilter.SelectedIndex = 0;
            cboStatusFilter.SelectedIndexChanged += CboStatusFilter_SelectedIndexChanged;

            // Date range
            lblDateRange = new Label
            {
                Text = "Từ ngày:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(70, 25),
                Location = new Point(590, 15),
                TextAlign = ContentAlignment.MiddleLeft
            };

            dtpStartDate = new DateTimePicker
            {
                Size = new Size(130, 30),
                Location = new Point(670, 12),
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now.AddDays(-30)
            };

            Label lblToDate = new Label
            {
                Text = "đến:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(40, 25),
                Location = new Point(820, 15),
                TextAlign = ContentAlignment.MiddleLeft
            };

            dtpEndDate = new DateTimePicker
            {
                Size = new Size(130, 30),
                Location = new Point(870, 12),
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now
            };

            // Filter buttons
            btnSearch = new Button
            {
                Text = "🔍 Tìm kiếm",
                Size = new Size(100, 35),
                Location = new Point(1020, 12),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Click += BtnSearch_Click;

            btnReset = new Button
            {
                Text = "🔄 Làm mới",
                Size = new Size(100, 35),
                Location = new Point(1130, 12),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(95, 95, 95),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnReset.FlatAppearance.BorderSize = 0;
            btnReset.Click += BtnReset_Click;

            // Add controls to filters panel
            pnlFilters.Controls.AddRange(new Control[] { 
                lblSearch, txtSearch, lblStatusFilter, cboStatusFilter,
                lblDateRange, dtpStartDate, lblToDate, dtpEndDate,
                btnSearch, btnReset
            });

            // Actions Panel
            pnlActions = new Panel
            {
                Size = new Size(1340, 60),
                Location = new Point(30, 190),
                BackColor = Color.Transparent
            };

            btnCreateOrder = new Button
            {
                Text = "➕ Tạo đơn hàng mới",
                Size = new Size(250, 60),
                Location = new Point(0, 0),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,

            };
            btnCreateOrder.FlatAppearance.BorderSize = 0;
            btnCreateOrder.Click += BtnCreateOrder_Click;

            btnRefresh = new Button
            {
                Text = "🔄 Làm mới dữ liệu",
                Size = new Size(200, 60),
                Location = new Point(270, 0),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += BtnRefresh_Click;

            var btnEditOrder = new Button
            {
                Text = "✏️ Chỉnh sửa đơn hàng",
                Size = new Size(200, 60),
                Location = new Point(490, 0),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(255, 193, 7),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnEditOrder.FlatAppearance.BorderSize = 0;
            btnEditOrder.Click += BtnEditOrder_Click;

            var btnChangeStatus = new Button
            {
                Text = "🔄 Thay đổi trạng thái",
                Size = new Size(200, 60),
                Location = new Point(710, 0),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(155, 89, 182),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnChangeStatus.FlatAppearance.BorderSize = 0;
            btnChangeStatus.Click += BtnChangeStatus_Click;

            pnlActions.Controls.AddRange(new Control[] { btnCreateOrder, btnRefresh, btnEditOrder, btnChangeStatus });

            // DataGridView
            dataGridViewOrders = new DataGridView
            {
                Size = new Size(1340, 480),
                Location = new Point(30, 270),
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
                RowTemplate = { Height = 60 }
            };

            // Style the DataGridView
            dataGridViewOrders.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dataGridViewOrders.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridViewOrders.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dataGridViewOrders.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridViewOrders.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewOrders.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            dataGridViewOrders.DefaultCellStyle.SelectionForeColor = Color.White;

            // Add controls to form
            this.Controls.AddRange(new Control[] { 
                lblTitle, pnlFilters, pnlActions, dataGridViewOrders 
            });
        }

        private async void LoadOrders()
        {
            try
            {
                var orders = await _orderRepository.GetAllOrdersAsync();
                RefreshDataGridView(orders);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu đơn hàng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshDataGridView(List<Dondathang> orders)
        {
            dataGridViewOrders.DataSource = null;
            dataGridViewOrders.Columns.Clear();

            // Create custom columns
            dataGridViewOrders.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MaDdh",
                HeaderText = "Mã đơn hàng",
                Width = 120
            });

            dataGridViewOrders.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CustomerInfo",
                HeaderText = "Thông tin khách hàng",
                Width = 200
            });

            dataGridViewOrders.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NgayDat",
                HeaderText = "Ngày đặt",
                Width = 120
            });

            dataGridViewOrders.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NgayGiao",
                HeaderText = "Ngày giao",
                Width = 120
            });

            dataGridViewOrders.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "EmployeeInfo",
                HeaderText = "Nhân viên xử lý",
                Width = 150
            });

            dataGridViewOrders.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TrangThai",
                HeaderText = "Trạng thái",
                Width = 120
            });

            dataGridViewOrders.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalAmount",
                HeaderText = "Tổng tiền",
                Width = 120
            });

            dataGridViewOrders.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "ViewDetails",
                HeaderText = "Xem chi tiết",
                Width = 100,
                Text = "👁️ Xem",
                UseColumnTextForButtonValue = true
            });

            dataGridViewOrders.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "DeleteOrder",
                HeaderText = "Xóa đơn",
                Width = 100,
                Text = "🗑️ Xóa",
                UseColumnTextForButtonValue = true
            });

            // Populate data
            foreach (var order in orders)
            {
                var rowIndex = dataGridViewOrders.Rows.Add();
                var row = dataGridViewOrders.Rows[rowIndex];

                row.Cells["MaDdh"].Value = order.MaDdh;
                row.Cells["CustomerInfo"].Value = $"{order.MaKhNavigation?.TenKh ?? "N/A"}\n{order.MaKhNavigation?.Sdtkh ?? "N/A"}";
                row.Cells["NgayDat"].Value = order.NgayDat?.ToString("dd/MM/yyyy");
                row.Cells["NgayGiao"].Value = order.NgayGiao?.ToString("dd/MM/yyyy") ?? "Chưa giao";
                row.Cells["EmployeeInfo"].Value = order.MaNvNavigation?.HoTenNv ?? "N/A";
                row.Cells["TrangThai"].Value = order.TrangThai ?? "Chờ xử lý";
                
                // Calculate total amount
                decimal totalAmount = 0;
                if (order.CtDhs != null)
                {
                    totalAmount = order.CtDhs.Sum(ct => (decimal)((ct.SoLuong ?? 0) * (ct.DonGia ?? 0)));
                }
                row.Cells["TotalAmount"].Value = totalAmount.ToString("N0") + " VNĐ";

                // Style status column
                StyleStatusCell(row.Cells["TrangThai"], order.TrangThai);
            }

            // Handle button clicks
            dataGridViewOrders.CellClick += DataGridViewOrders_CellClick;
            
            // Add double-click to view details
            dataGridViewOrders.CellDoubleClick += DataGridViewOrders_CellDoubleClick;
        }

        private void StyleStatusCell(DataGridViewCell cell, string status)
        {
            if (status == "Hoàn thành" || status == "Đã giao")
            {
                cell.Style.ForeColor = Color.Green;
            }
            else if (status == "Đang xử lý")
            {
                cell.Style.ForeColor = Color.Orange;
            }
            else if (status == "Đã hủy")
            {
                cell.Style.ForeColor = Color.Red;
            }
            else
            {
                cell.Style.ForeColor = Color.Blue;
            }
        }

        private bool _isProcessing = false;

        private async void DataGridViewOrders_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && !_isProcessing)
            {
                try
                {
                    _isProcessing = true;
                    var orderId = dataGridViewOrders.Rows[e.RowIndex].Cells["MaDdh"].Value.ToString();
                    
                    if (e.ColumnIndex == dataGridViewOrders.Columns["ViewDetails"].Index)
                    {
                        var order = await _orderRepository.GetOrderByIdAsync(orderId);
                        if (order != null)
                        {
                            ViewOrderDetails(order);
                        }
                    }
                    else if (e.ColumnIndex == dataGridViewOrders.Columns["DeleteOrder"].Index)
                    {
                        var order = await _orderRepository.GetOrderByIdAsync(orderId);
                        if (order != null)
                        {
                            DeleteOrder(order);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xử lý thao tác: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    _isProcessing = false;
                }
            }
        }



        private void ViewOrderDetails(Dondathang order)
        {
            try
            {
                var viewForm = new OrderAddEditForm(order, true); // true = view mode
                viewForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form xem chi tiết: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditOrder(Dondathang order)
        {
            try
            {
                var editForm = new OrderAddEditForm(order, false); // false = edit mode
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadOrders(); // Reload data after edit
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form chỉnh sửa: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task ChangeOrderStatus(string orderId, string newStatus)
        {
            try
            {
                var success = await _orderRepository.UpdateOrderStatusAsync(orderId, newStatus);
                if (success)
                {
                    MessageBox.Show($"Đã cập nhật trạng thái đơn hàng {orderId} thành '{newStatus}'", 
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadOrders(); // Reload data
                }
                else
                {
                    MessageBox.Show("Không thể cập nhật trạng thái đơn hàng!", 
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật trạng thái: {ex.Message}", 
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void DeleteOrder(Dondathang order)
        {
            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa đơn hàng {order.MaDdh}?\n\n" +
                                       "Hành động này không thể hoàn tác!", 
                                       "Xác nhận xóa", 
                                       MessageBoxButtons.YesNo, 
                                       MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    var success = await _orderRepository.DeleteOrderAsync(order.MaDdh);
                    if (success)
                    {
                        MessageBox.Show($"Đã xóa đơn hàng {order.MaDdh} thành công!", 
                            "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadOrders(); // Reload data
                    }
                    else
                    {
                        MessageBox.Show("Không thể xóa đơn hàng!", 
                            "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa đơn hàng: {ex.Message}", 
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }



        private async void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                LoadOrders();
            }
        }

        private async void CboStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            await ApplyFilters();
        }

        private async void BtnSearch_Click(object sender, EventArgs e)
        {
            await ApplyFilters();
        }

        private async void BtnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            cboStatusFilter.SelectedIndex = 0;
            dtpStartDate.Value = DateTime.Now.AddDays(-30);
            dtpEndDate.Value = DateTime.Now;
            await ApplyFilters();
        }

        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadOrders();
        }

        private async void DataGridViewOrders_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && 
                e.ColumnIndex != dataGridViewOrders.Columns["ViewDetails"].Index && 
                e.ColumnIndex != dataGridViewOrders.Columns["DeleteOrder"].Index)
            {
                try
                {
                    var orderId = dataGridViewOrders.Rows[e.RowIndex].Cells["MaDdh"].Value.ToString();
                    var order = await _orderRepository.GetOrderByIdAsync(orderId);
                    if (order != null)
                    {
                        ViewOrderDetails(order);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xem chi tiết: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async Task ApplyFilters()
        {
            try
            {
                List<Dondathang> orders;

                // Apply status filter
                if (cboStatusFilter.SelectedIndex > 0)
                {
                    var status = cboStatusFilter.SelectedItem.ToString();
                    orders = await _orderRepository.GetOrdersByStatusAsync(status);
                }
                else
                {
                    orders = await _orderRepository.GetAllOrdersAsync();
                }

                // Apply date range filter
                if (dtpStartDate.Value <= dtpEndDate.Value)
                {
                    orders = orders.Where(o => o.NgayDat.HasValue &&
                        o.NgayDat.Value.ToDateTime(TimeOnly.MinValue) >= dtpStartDate.Value &&
                        o.NgayDat.Value.ToDateTime(TimeOnly.MinValue) <= dtpEndDate.Value.AddDays(1).AddSeconds(-1))
                        .ToList();
                }

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    var searchTerm = txtSearch.Text.ToLower();
                    orders = orders.Where(o => 
                        o.MaDdh.ToLower().Contains(searchTerm) ||
                        (o.MaKhNavigation?.TenKh?.ToLower().Contains(searchTerm) ?? false) ||
                        (o.TrangThai?.ToLower().Contains(searchTerm) ?? false)
                    ).ToList();
                }

                RefreshDataGridView(orders);
                
                // Show result count
                var resultCount = orders.Count;
                var totalCount = await _orderRepository.GetTotalOrdersAsync(DateTime.MinValue, DateTime.MaxValue);
                this.Text = $"Quản lý Đơn hàng - Hiển thị {resultCount}/{totalCount} đơn hàng";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi áp dụng bộ lọc: {ex.Message}", 
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnCreateOrder_Click(object sender, EventArgs e)
        {
            if (_isProcessing) return;
            
            try
            {
                _isProcessing = true;
                var addForm = new OrderAddEditForm(); // new order
                if (addForm.ShowDialog() == DialogResult.OK)
                {
                    LoadOrders(); // Reload data after adding
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form thêm đơn hàng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isProcessing = false;
            }
        }

        private async void BtnEditOrder_Click(object sender, EventArgs e)
        {
            if (_isProcessing) return;
            
            if (dataGridViewOrders.SelectedRows.Count > 0)
            {
                try
                {
                    _isProcessing = true;
                    var selectedRow = dataGridViewOrders.SelectedRows[0];
                    var orderId = selectedRow.Cells["MaDdh"].Value.ToString();
                    var order = await _orderRepository.GetOrderByIdAsync(orderId);
                    
                    if (order != null)
                    {
                        EditOrder(order);
                    }
                    else
                    {
                        MessageBox.Show("Không thể tải thông tin đơn hàng!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi tải thông tin đơn hàng: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    _isProcessing = false;
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn đơn hàng cần chỉnh sửa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void BtnChangeStatus_Click(object sender, EventArgs e)
        {
            if (_isProcessing) return;
            
            if (dataGridViewOrders.SelectedRows.Count > 0)
            {
                try
                {
                    _isProcessing = true;
                    var selectedRow = dataGridViewOrders.SelectedRows[0];
                    var orderId = selectedRow.Cells["MaDdh"].Value.ToString();
                    var order = await _orderRepository.GetOrderByIdAsync(orderId);
                    
                    if (order != null)
                    {
                        ShowStatusChangeDialog(order);
                    }
                    else
                    {
                        MessageBox.Show("Không thể tải thông tin đơn hàng!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi tải thông tin đơn hàng: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    _isProcessing = false;
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn đơn hàng cần thay đổi trạng thái!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ShowStatusChangeDialog(Dondathang order)
        {
            var statuses = new[] { "Chờ xử lý", "Đang xử lý", "Đã giao", "Đã hủy" };
            var currentStatus = order.TrangThai ?? "Chờ xử lý";
            
            var availableStatuses = statuses.Where(s => s != currentStatus).ToArray();
            
            if (availableStatuses.Length == 0)
            {
                MessageBox.Show("Đơn hàng này đã ở trạng thái cuối cùng!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var statusForm = new Form
            {
                Text = "Thay đổi trạng thái đơn hàng",
                Size = new Size(400, 200),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedSingle,
                MaximizeBox = false,
                BackColor = Color.FromArgb(248, 249, 250)
            };

            var lblCurrent = new Label
            {
                Text = $"Trạng thái hiện tại: {currentStatus}",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(350, 25),
                Location = new Point(20, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lblNew = new Label
            {
                Text = "Chọn trạng thái mới:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(200, 25),
                Location = new Point(20, 60),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var cboNewStatus = new ComboBox
            {
                Size = new Size(200, 30),
                Location = new Point(20, 90),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboNewStatus.Items.AddRange(availableStatuses);
            cboNewStatus.SelectedIndex = 0;

            var btnOK = new Button
            {
                Text = "OK",
                Size = new Size(80, 35),
                Location = new Point(200, 120),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            btnOK.FlatAppearance.BorderSize = 0;

            var btnCancel = new Button
            {
                Text = "Hủy",
                Size = new Size(80, 35),
                Location = new Point(300, 120),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            statusForm.Controls.AddRange(new Control[] { lblCurrent, lblNew, cboNewStatus, btnOK, btnCancel });

            if (statusForm.ShowDialog() == DialogResult.OK)
            {
                var newStatus = cboNewStatus.SelectedItem.ToString();
                _ = ChangeOrderStatus(order.MaDdh, newStatus);
            }
        }
    }
}
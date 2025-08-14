                                                                            namespace AceCook
{
    partial class DashboardForm
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
            panelSidebar = new Panel();
            panelUserInfo = new Panel();
            lblUserRole = new Label();
            lblUserName = new Label();
            lblUserStatus = new Label();
            treeViewMenu = new TreeView();
            panelHeader = new Panel();
            lblSystemTitle = new Label();
            btnMinimize = new Button();
            btnMaximize = new Button();
            btnClose = new Button();
            panelMainContent = new Panel();
            panelContent = new Panel();
            panelSidebar.SuspendLayout();
            panelUserInfo.SuspendLayout();
            panelHeader.SuspendLayout();
            panelMainContent.SuspendLayout();
            SuspendLayout();
            // 
            // panelSidebar
            // 
            panelSidebar.BackColor = Color.FromArgb(44, 62, 80);
            panelSidebar.Controls.Add(treeViewMenu);
            panelSidebar.Controls.Add(panelUserInfo);
            panelSidebar.Dock = DockStyle.Left;
            panelSidebar.Location = new Point(0, 0);
            panelSidebar.Name = "panelSidebar";
            panelSidebar.Size = new Size(280, 1000);
            panelSidebar.TabIndex = 0;
            // 
            // panelUserInfo
            // 
            panelUserInfo.BackColor = Color.FromArgb(52, 73, 94);
            panelUserInfo.Controls.Add(lblUserStatus);
            panelUserInfo.Controls.Add(lblUserName);
            panelUserInfo.Controls.Add(lblUserRole);
            panelUserInfo.Dock = DockStyle.Top;
            panelUserInfo.Location = new Point(0, 0);
            panelUserInfo.Name = "panelUserInfo";
            panelUserInfo.Size = new Size(280, 120);
            panelUserInfo.TabIndex = 0;
            // 
            // lblUserRole
            // 
            lblUserRole.AutoSize = true;
            lblUserRole.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            lblUserRole.ForeColor = Color.FromArgb(200, 200, 200);
            lblUserRole.Location = new Point(20, 50);
            lblUserRole.Name = "lblUserRole";
            lblUserRole.Size = new Size(80, 20);
            lblUserRole.TabIndex = 0;
            lblUserRole.Text = "Vai trò";
            // 
            // lblUserName
            // 
            lblUserName.AutoSize = true;
            lblUserName.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblUserName.ForeColor = Color.White;
            lblUserName.Location = new Point(20, 20);
            lblUserName.Name = "lblUserName";
            lblUserName.Size = new Size(120, 28);
            lblUserName.TabIndex = 1;
            lblUserName.Text = "Tên người dùng";
            // 
            // lblUserStatus
            // 
            lblUserStatus.AutoSize = true;
            lblUserStatus.Font = new Font("Segoe UI", 8F, FontStyle.Italic);
            lblUserStatus.ForeColor = Color.FromArgb(100, 200, 100);
            lblUserStatus.Location = new Point(20, 80);
            lblUserStatus.Name = "lblUserStatus";
            lblUserStatus.Size = new Size(100, 19);
            lblUserStatus.TabIndex = 2;
            lblUserStatus.Text = "Đang hoạt động";
            // 
            // treeViewMenu
            // 
            treeViewMenu.BackColor = Color.FromArgb(44, 62, 80);
            treeViewMenu.BorderStyle = BorderStyle.None;
            treeViewMenu.Dock = DockStyle.Fill;
            treeViewMenu.Font = new Font("Segoe UI", 10F);
            treeViewMenu.ForeColor = Color.White;
            treeViewMenu.FullRowSelect = true;
            treeViewMenu.HideSelection = false;
            treeViewMenu.Location = new Point(0, 120);
            treeViewMenu.Name = "treeViewMenu";
            treeViewMenu.ShowLines = false;
            treeViewMenu.ShowPlusMinus = false;
            treeViewMenu.Size = new Size(280, 880);
            treeViewMenu.TabIndex = 1;
            treeViewMenu.AfterSelect += treeViewMenu_AfterSelect;
            // 
            // panelHeader
            // 
            panelHeader.BackColor = Color.FromArgb(52, 73, 94);
            panelHeader.Controls.Add(btnClose);
            panelHeader.Controls.Add(btnMaximize);
            panelHeader.Controls.Add(btnMinimize);
            panelHeader.Controls.Add(lblSystemTitle);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Location = new Point(280, 0);
            panelHeader.Name = "panelHeader";
            panelHeader.Size = new Size(1008, 60);
            panelHeader.TabIndex = 1;
            // 
            // lblSystemTitle
            // 
            lblSystemTitle.AutoSize = true;
            lblSystemTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblSystemTitle.ForeColor = Color.White;
            lblSystemTitle.Location = new Point(20, 15);
            lblSystemTitle.Name = "lblSystemTitle";
            lblSystemTitle.Size = new Size(400, 37);
            lblSystemTitle.TabIndex = 0;
            lblSystemTitle.Text = "ACECOOK SALES MANAGEMENT";
            // 
            // btnMinimize
            // 
            btnMinimize.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMinimize.FlatAppearance.BorderSize = 0;
            btnMinimize.FlatStyle = FlatStyle.Flat;
            btnMinimize.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnMinimize.ForeColor = Color.White;
            btnMinimize.Location = new Point(880, 10);
            btnMinimize.Name = "btnMinimize";
            btnMinimize.Size = new Size(35, 35);
            btnMinimize.TabIndex = 1;
            btnMinimize.Text = "─";
            btnMinimize.UseVisualStyleBackColor = false;
            btnMinimize.Click += btnMinimize_Click;
            // 
            // btnMaximize
            // 
            btnMaximize.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMaximize.FlatAppearance.BorderSize = 0;
            btnMaximize.FlatStyle = FlatStyle.Flat;
            btnMaximize.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnMaximize.ForeColor = Color.White;
            btnMaximize.Location = new Point(925, 10);
            btnMaximize.Name = "btnMaximize";
            btnMaximize.Size = new Size(35, 35);
            btnMaximize.TabIndex = 2;
            btnMaximize.Text = "□";
            btnMaximize.UseVisualStyleBackColor = false;
            btnMaximize.Click += btnMaximize_Click;
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnClose.ForeColor = Color.White;
            btnClose.Location = new Point(970, 10);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(35, 35);
            btnClose.TabIndex = 3;
            btnClose.Text = "×";
            btnClose.UseVisualStyleBackColor = false;
            btnClose.Click += btnClose_Click;
            // 
            // panelMainContent
            // 
            panelMainContent.BackColor = Color.FromArgb(248, 249, 250);
            panelMainContent.Controls.Add(panelContent);
            panelMainContent.Dock = DockStyle.Fill;
            panelMainContent.Location = new Point(280, 60);
            panelMainContent.Name = "panelMainContent";
            panelMainContent.Size = new Size(1008, 940);
            panelMainContent.TabIndex = 2;
            // 
            // panelContent
            // 
            panelContent.BackColor = Color.FromArgb(248, 249, 250);
            panelContent.Dock = DockStyle.Fill;
            panelContent.Location = new Point(0, 0);
            panelContent.Name = "panelContent";
            panelContent.Size = new Size(1008, 940);
            panelContent.TabIndex = 0;
            // 
            // DashboardForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1288, 1000);
            Controls.Add(panelMainContent);
            Controls.Add(panelHeader);
            Controls.Add(panelSidebar);
            MinimumSize = new Size(1000, 600);
            Name = "DashboardForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ACECOOK Sales Management System";
            WindowState = FormWindowState.Maximized;
            panelSidebar.ResumeLayout(false);
            panelUserInfo.ResumeLayout(false);
            panelUserInfo.PerformLayout();
            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            panelMainContent.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panelSidebar;
        private Panel panelUserInfo;
        private Label lblUserRole;
        private Label lblUserName;
        private Label lblUserStatus;
        private TreeView treeViewMenu;
        private Panel panelHeader;
        private Label lblSystemTitle;
        private Button btnMinimize;
        private Button btnMaximize;
        private Button btnClose;
        private Panel panelMainContent;
        private Panel panelContent;
    }
}

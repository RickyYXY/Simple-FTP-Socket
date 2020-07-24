namespace APPForm
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("计算机");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.toolStripInfo = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBoxIpAddr = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBoxPort = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBoxName = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBoxPassword = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnLogin = new System.Windows.Forms.ToolStripButton();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.treeLocal = new System.Windows.Forms.TreeView();
            this.menuLocal = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.FTPflowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.toolStripForFTP = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonReturn = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRefresh = new System.Windows.Forms.ToolStripButton();
            this.statusStripMain = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.lblMsg = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuRightKey = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripInfo.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.menuLocal.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.toolStripForFTP.SuspendLayout();
            this.statusStripMain.SuspendLayout();
            this.menuRightKey.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripInfo
            // 
            this.toolStripInfo.BackColor = System.Drawing.SystemColors.ControlLight;
            this.toolStripInfo.ImageScalingSize = new System.Drawing.Size(25, 25);
            this.toolStripInfo.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripTextBoxIpAddr,
            this.toolStripLabel4,
            this.toolStripTextBoxPort,
            this.toolStripSeparator1,
            this.toolStripLabel2,
            this.toolStripTextBoxName,
            this.toolStripSeparator2,
            this.toolStripLabel3,
            this.toolStripTextBoxPassword,
            this.toolStripSeparator3,
            this.btnLogin});
            this.toolStripInfo.Location = new System.Drawing.Point(0, 0);
            this.toolStripInfo.Name = "toolStripInfo";
            this.toolStripInfo.Size = new System.Drawing.Size(1268, 32);
            this.toolStripInfo.TabIndex = 0;
            this.toolStripInfo.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(54, 29);
            this.toolStripLabel1.Text = "主机：";
            // 
            // toolStripTextBoxIpAddr
            // 
            this.toolStripTextBoxIpAddr.BackColor = System.Drawing.SystemColors.Window;
            this.toolStripTextBoxIpAddr.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.toolStripTextBoxIpAddr.Name = "toolStripTextBoxIpAddr";
            this.toolStripTextBoxIpAddr.Size = new System.Drawing.Size(132, 32);
            this.toolStripTextBoxIpAddr.Text = "192.168.0.102";
            this.toolStripTextBoxIpAddr.ToolTipText = "输入主机地址";
            // 
            // toolStripLabel4
            // 
            this.toolStripLabel4.Name = "toolStripLabel4";
            this.toolStripLabel4.Size = new System.Drawing.Size(54, 29);
            this.toolStripLabel4.Text = "端口：";
            // 
            // toolStripTextBoxPort
            // 
            this.toolStripTextBoxPort.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.toolStripTextBoxPort.Name = "toolStripTextBoxPort";
            this.toolStripTextBoxPort.Size = new System.Drawing.Size(132, 32);
            this.toolStripTextBoxPort.Text = "21";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 32);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(69, 29);
            this.toolStripLabel2.Text = "用户名：";
            // 
            // toolStripTextBoxName
            // 
            this.toolStripTextBoxName.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.toolStripTextBoxName.Name = "toolStripTextBoxName";
            this.toolStripTextBoxName.Size = new System.Drawing.Size(132, 32);
            this.toolStripTextBoxName.Text = "ding0";
            this.toolStripTextBoxName.ToolTipText = "输入用户名";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 32);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(54, 29);
            this.toolStripLabel3.Text = "密码：";
            // 
            // toolStripTextBoxPassword
            // 
            this.toolStripTextBoxPassword.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.toolStripTextBoxPassword.Name = "toolStripTextBoxPassword";
            this.toolStripTextBoxPassword.Size = new System.Drawing.Size(132, 32);
            this.toolStripTextBoxPassword.Text = "13986251398Dj";
            this.toolStripTextBoxPassword.ToolTipText = "请输入密码";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 32);
            // 
            // btnLogin
            // 
            this.btnLogin.Image = global::APPForm.Properties.Resources.Internet;
            this.btnLogin.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(98, 29);
            this.btnLogin.Text = "点击连接";
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(0, 36);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(439, 689);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.treeLocal);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage1.Size = new System.Drawing.Size(431, 660);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "本地驱动器";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // treeLocal
            // 
            this.treeLocal.ContextMenuStrip = this.menuLocal;
            this.treeLocal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeLocal.ImageIndex = 0;
            this.treeLocal.ImageList = this.imgList;
            this.treeLocal.Location = new System.Drawing.Point(4, 4);
            this.treeLocal.Margin = new System.Windows.Forms.Padding(4);
            this.treeLocal.Name = "treeLocal";
            treeNode1.Name = "节点0";
            treeNode1.Text = "计算机";
            this.treeLocal.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.treeLocal.SelectedImageIndex = 0;
            this.treeLocal.Size = new System.Drawing.Size(423, 652);
            this.treeLocal.TabIndex = 0;
            this.treeLocal.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeLocal_BeforeExpand);
            this.treeLocal.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeLocal_AfterSelect);
            // 
            // menuLocal
            // 
            this.menuLocal.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuLocal.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuLoad});
            this.menuLocal.Name = "menuLocal";
            this.menuLocal.Size = new System.Drawing.Size(109, 28);
            // 
            // menuLoad
            // 
            this.menuLoad.Name = "menuLoad";
            this.menuLoad.Size = new System.Drawing.Size(108, 24);
            this.menuLoad.Text = "上传";
            this.menuLoad.Click += new System.EventHandler(this.menuLoad_Click);
            // 
            // imgList
            // 
            this.imgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgList.ImageStream")));
            this.imgList.TransparentColor = System.Drawing.Color.Transparent;
            this.imgList.Images.SetKeyName(0, "My Computer.ico");
            this.imgList.Images.SetKeyName(1, "Local Disk.ico");
            this.imgList.Images.SetKeyName(2, "Folder Opened.ico");
            this.imgList.Images.SetKeyName(3, "My Documents.ico");
            this.imgList.Images.SetKeyName(4, "computer.gif");
            this.imgList.Images.SetKeyName(5, "drive.gif");
            this.imgList.Images.SetKeyName(6, "folder.ico");
            this.imgList.Images.SetKeyName(7, "file.ico");
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage2);
            this.tabControl2.Location = new System.Drawing.Point(441, 36);
            this.tabControl2.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(827, 689);
            this.tabControl2.TabIndex = 2;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.FTPflowLayoutPanel);
            this.tabPage2.Controls.Add(this.toolStripForFTP);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage2.Size = new System.Drawing.Size(819, 660);
            this.tabPage2.TabIndex = 0;
            this.tabPage2.Text = "FTP服务器";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // FTPflowLayoutPanel
            // 
            this.FTPflowLayoutPanel.AutoScroll = true;
            this.FTPflowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FTPflowLayoutPanel.Location = new System.Drawing.Point(4, 36);
            this.FTPflowLayoutPanel.Name = "FTPflowLayoutPanel";
            this.FTPflowLayoutPanel.Size = new System.Drawing.Size(811, 620);
            this.FTPflowLayoutPanel.TabIndex = 0;
            // 
            // toolStripForFTP
            // 
            this.toolStripForFTP.ImageScalingSize = new System.Drawing.Size(25, 25);
            this.toolStripForFTP.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonReturn,
            this.toolStripButtonRefresh});
            this.toolStripForFTP.Location = new System.Drawing.Point(4, 4);
            this.toolStripForFTP.Name = "toolStripForFTP";
            this.toolStripForFTP.Size = new System.Drawing.Size(811, 32);
            this.toolStripForFTP.TabIndex = 2;
            this.toolStripForFTP.Text = "toolStrip2";
            // 
            // toolStripButtonReturn
            // 
            this.toolStripButtonReturn.Image = global::APPForm.Properties.Resources.Up;
            this.toolStripButtonReturn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonReturn.Name = "toolStripButtonReturn";
            this.toolStripButtonReturn.Size = new System.Drawing.Size(128, 29);
            this.toolStripButtonReturn.Text = "返回上级目录";
            this.toolStripButtonReturn.Click += new System.EventHandler(this.toolStripButtonReturn_Click);
            // 
            // toolStripButtonRefresh
            // 
            this.toolStripButtonRefresh.Image = global::APPForm.Properties.Resources.Refresh;
            this.toolStripButtonRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRefresh.Name = "toolStripButtonRefresh";
            this.toolStripButtonRefresh.Size = new System.Drawing.Size(68, 29);
            this.toolStripButtonRefresh.Text = "刷新";
            this.toolStripButtonRefresh.Click += new System.EventHandler(this.toolStripButtonRefresh_Click);
            // 
            // statusStripMain
            // 
            this.statusStripMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripProgressBar1,
            this.lblMsg});
            this.statusStripMain.Location = new System.Drawing.Point(0, 730);
            this.statusStripMain.Name = "statusStripMain";
            this.statusStripMain.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStripMain.Size = new System.Drawing.Size(1268, 26);
            this.statusStripMain.TabIndex = 4;
            this.statusStripMain.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(93, 20);
            this.toolStripStatusLabel1.Text = "欢迎使用……";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(133, 18);
            // 
            // lblMsg
            // 
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(0, 20);
            // 
            // menuRightKey
            // 
            this.menuRightKey.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuRightKey.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuSaveAs,
            this.menuDelete});
            this.menuRightKey.Name = "menuRightKey";
            this.menuRightKey.Size = new System.Drawing.Size(124, 52);
            // 
            // menuSaveAs
            // 
            this.menuSaveAs.Name = "menuSaveAs";
            this.menuSaveAs.Size = new System.Drawing.Size(123, 24);
            this.menuSaveAs.Text = "另存为";
            this.menuSaveAs.Click += new System.EventHandler(this.menuSaveAs_Click);
            // 
            // menuDelete
            // 
            this.menuDelete.Name = "menuDelete";
            this.menuDelete.Size = new System.Drawing.Size(123, 24);
            this.menuDelete.Text = "删除";
            this.menuDelete.Click += new System.EventHandler(this.menuDelete_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(1268, 756);
            this.Controls.Add(this.statusStripMain);
            this.Controls.Add(this.tabControl2);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.toolStripInfo);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Text = "Fast FTP";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.toolStripInfo.ResumeLayout(false);
            this.toolStripInfo.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.menuLocal.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.toolStripForFTP.ResumeLayout(false);
            this.toolStripForFTP.PerformLayout();
            this.statusStripMain.ResumeLayout(false);
            this.statusStripMain.PerformLayout();
            this.menuRightKey.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripInfo;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxIpAddr;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxName;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxPassword;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton btnLogin;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TreeView treeLocal;
        private System.Windows.Forms.ContextMenuStrip menuLocal;
        private System.Windows.Forms.ImageList imgList;
        private System.Windows.Forms.ToolStripLabel toolStripLabel4;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxPort;
        private System.Windows.Forms.ToolStripMenuItem menuLoad;
        private System.Windows.Forms.StatusStrip statusStripMain;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ContextMenuStrip menuRightKey;
        private System.Windows.Forms.FlowLayoutPanel FTPflowLayoutPanel;
        private System.Windows.Forms.ToolStripMenuItem menuSaveAs;
        private System.Windows.Forms.ToolStripMenuItem menuDelete;
        private System.Windows.Forms.ToolStripStatusLabel lblMsg;
        private System.Windows.Forms.ToolStrip toolStripForFTP;
        private System.Windows.Forms.ToolStripButton toolStripButtonReturn;
        private System.Windows.Forms.ToolStripButton toolStripButtonRefresh;
    }
}


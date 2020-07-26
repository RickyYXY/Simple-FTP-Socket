using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FTPUtils;

namespace APPForm
{
    public partial class MainForm : Form
    {
        //private ftpClient ftpClient;

        private  DateTime clickTime;

        private bool isClicked = false;

        private FTPClient ftpClient;

        public MainForm()
        {
            InitializeComponent();
            //密码显示为"*"
            TextBox tb = toolStripTextBoxPassword.Control as TextBox;
            tb.PasswordChar = '*';
        }

        //初始化
        private void MainForm_Load(object sender, EventArgs e)
        {
            InitTreeView();
        }

        private void InitTreeView()
        {
            string[] drives = Environment.GetLogicalDrives();
            int i = 0;
            foreach (string drive in drives)
            {
                DriveInfo d = new DriveInfo(drive);
                if ((d.DriveType & DriveType.Fixed) == DriveType.Fixed)
                {
                    string drive1 = drive.Substring(0, drive.Length - 1);
                    this.treeLocal.Nodes[0].Nodes.Add(drive1);
                    this.treeLocal.Nodes[0].Nodes[i].ImageIndex = 1;
                    this.treeLocal.Nodes[0].Nodes[i].SelectedImageIndex = 1;
                    this.treeLocal.Nodes[0].Nodes[i].Tag = drive1;
                    this.treeLocal.Nodes[0].Nodes[i].Nodes.Add("");//增加一个空白节点，看起来是加号
                    i++;
                }
            }
        }

        //树菜单相关代码
        private void treeLocal_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Level > 0)
            {
                //点击之前，先填充节点：
                string path = e.Node.FullPath.Substring(e.Node.FullPath.IndexOf("\\") + 1) + "\\";
                e.Node.Nodes.Clear();
                //string path = string.Format("{0}{1}",e.Node.Tag,this.treeLocal.PathSeparator);
                string[] files = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);
                int i = 0;
                foreach (string file in files)
                {
                    FileInfo f = new FileInfo(file);
                    if ((f.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden && (f.Attributes & FileAttributes.System) != FileAttributes.System)
                    {
                        e.Node.Nodes.Add(Path.GetFileName(file));
                        e.Node.Nodes[i].ImageIndex = 3;
                        e.Node.Nodes[i].SelectedImageIndex = 3;
                        e.Node.Nodes[i].Tag = file;
                        i++;
                    }
                }
                string[] dirs = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
                foreach (string dir in dirs)
                {
                    DirectoryInfo d = new DirectoryInfo(dir);
                    if ((d.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden && (d.Attributes & FileAttributes.System) != FileAttributes.System)
                    {
                        e.Node.Nodes.Add(Path.GetFileName(dir));
                        e.Node.Nodes[i].ImageIndex = 2;
                        e.Node.Nodes[i].SelectedImageIndex = 2;
                        e.Node.Nodes[i].Tag = dir;
                        e.Node.Nodes[i].Nodes.Add("");//增加一个空白节点，看起来是加号
                        i++;
                    }
                }
            }
        }
        private void treeLocal_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Level > 0 && e.Node.ImageIndex == 3)
            {
                this.menuLocal.Enabled = true;
                string path = e.Node.FullPath.Substring(e.Node.FullPath.IndexOf("\\") + 1);
                this.menuLoad.Tag = path;
            }
            else
            {
                this.menuLocal.Enabled = false;
            }
        }

        /* 下面是ftp相关代码*/
        /*-------------------------------------------*/

        /// <summary>
        /// 右键上传
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuLoad_Click(object sender, EventArgs e)
        {
            if (ftpClient == null)
                {
                    MessageBox.Show("请先登录！");
                    return;
                }
            try
            {
                lblMsg.Text = "上传中";
                ToolStripMenuItem mi = (ToolStripMenuItem)sender;
                string path = "";
                if (mi.Tag != null)
                    path = mi.Tag.ToString();
                if (File.Exists(path))
                {
                    ftpClient.RelatePath = string.Format("{0}/{1}", ftpClient.RelatePath, Path.GetFileName(path));
                    ftpClient.Upload(path, updateProgress);
                    lblMsg.Text = "上传成功";
                    MessageBox.Show("上传完成");
                    toolStripProgressBar1.Value = 0;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                lblMsg.Text = "上传失败";
            }
            finally
            {
                ftpClient.SetPrePath();
                ShowFilesDirectory();
                lblMsg.Text = "";
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            //如果就目录,先设置上级目录，再列出目录
            //ftpClient.SetPrePath();
            //this.ListDirectory();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if(btnLogin.Text=="点击连接")
                {
                    if (CheckUserInfo())
                    {
                        string ipAddr = this.toolStripTextBoxIpAddr.Text.Trim();
                        string port = this.toolStripTextBoxPort.Text.Trim();
                        string userName = this.toolStripTextBoxName.Text.Trim();
                        string password = this.toolStripTextBoxPassword.Text.Trim();
                        ftpClient = new FTPClient(ipAddr, port, userName, password);
                        if (!ftpClient.Connect())
                            lblMsg.Text = "连接失败，请检查服务器状况";
                        else
                        {
                            if (ftpClient.Login())
                            {
                                lblMsg.Text = "登录成功";
                                btnLogin.Text = "断开连接";
                                toolStripTextBoxIpAddr.ReadOnly = true;
                                toolStripTextBoxName.ReadOnly = true;
                                toolStripTextBoxPassword.ReadOnly = true;
                                toolStripTextBoxPort.ReadOnly = true;
                                ShowFilesDirectory();
                            }
                            else
                            {
                                lblMsg.Text = "用户名/密码出错";
                                ftpClient.Close();
                            }
                        }
                    }
                }
                else
                {
                    ftpClient.Close();
                    FTPflowLayoutPanel.Controls.Clear();
                    btnLogin.Text = "点击连接";
                    toolStripTextBoxIpAddr.ReadOnly = false;
                    toolStripTextBoxName.ReadOnly = false;
                    toolStripTextBoxPassword.ReadOnly = false;
                    toolStripTextBoxPort.ReadOnly = false;
                    lblMsg.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 检查用户登录信息
        /// </summary>
        /// <returns></returns>
        private bool CheckUserInfo()
        {
            string ipaddr = this.toolStripTextBoxIpAddr.Text.Trim();
            string port = this.toolStripTextBoxPort.Text.Trim();
            string username = this.toolStripTextBoxName.Text.Trim();
            string password = this.toolStripTextBoxPassword.Text.Trim();
            if (string.IsNullOrEmpty(ipaddr) || string.IsNullOrEmpty(port)
                || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("请输入登录信息");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 服务器端文件图标鼠标事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnTmp_MouseDown(object sender, MouseEventArgs e)
        {
            //如果是右键按下
            if (e.Button == MouseButtons.Right) //右键按下显示菜单
            {
                Button btnTmp = (Button)sender;
                string name = btnTmp.Text;
                menuSaveAs.Tag = name;
                menuDelete.Tag = name;
            }
        }

        /// <summary>
        /// 双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTmp_DoubleClick(object sender, EventArgs e)
        {
            Button btnTmp = (Button)sender;
            FtpContentType contentType;
            if (Enum.TryParse(btnTmp.Tag.ToString(), out contentType))
            {
                switch (contentType)
                {
                    case FtpContentType.folder:
                        //如果是目录，则重新设置当前的相对路径
                        ftpClient.SetRelatePath(btnTmp.Text);
                        ShowFilesDirectory();//再次列出目录
                        break;
                    default:
                        MessageBox.Show("非目录，请点击右键", "提示", MessageBoxButtons.OK);
                        break;
                }
            }
        }

        /// <summary>
        /// 用于模拟双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTmp_Click(object sender, EventArgs e)
        {
            if (isClicked)
            {
                TimeSpan span = DateTime.Now - clickTime;
                if (span.Milliseconds < SystemInformation.DoubleClickTime)
                {
                    btnTmp_DoubleClick(sender, e);
                    isClicked = false;
                }
            }
            else
            {
                isClicked = true;
                clickTime = DateTime.Now;
            }
        }

        /// <summary>
        /// 在对应板块展示服务器端文件的内容
        /// </summary>
        private void ShowFilesDirectory()
        {
            try
            {
                string[] arrAccept = ftpClient.GetFilesList();//调用Ftp目录显示功能
                FTPflowLayoutPanel.Controls.Clear();
                foreach (string accept in arrAccept)
                {
                    if (accept == "")
                        continue;
                    string name = accept.Substring(39);
                    //创建一个临时控件用于显示ftp服务器端的文件
                    Button btnTmp = new Button()
                    {
                        BackColor = Color.Azure,
                        TextImageRelation = TextImageRelation.ImageAboveText,
                        Text = name,
                        Font = new Font("微软雅黑",9),
                        Width = 80,
                        Height = 80
                    };
                    btnTmp.AutoSize = true;
                    btnTmp.FlatAppearance.BorderSize = 0;
                    btnTmp.FlatStyle = FlatStyle.Flat;

                    if (accept.IndexOf("<DIR>") != -1)       //如果是文件夹
                    {
                        btnTmp.Image = Properties.Resources.folder.ToBitmap();
                        btnTmp.Tag = FtpContentType.folder;
                    }
                    else          //如果是文件
                    {
                        btnTmp.Image = Properties.Resources.file.ToBitmap();
                        btnTmp.Tag = FtpContentType.file;
                        btnTmp.ContextMenuStrip = menuRightKey; //只有文件右键才有按钮出现
                        btnTmp.MouseDown += new MouseEventHandler(BtnTmp_MouseDown);
                    }
                    btnTmp.Click += new EventHandler(btnTmp_Click);//Button中不支持双击事件，所以用单击事件模拟双击
                    FTPflowLayoutPanel.Controls.Add(btnTmp);
                }
                lblMsg.Text = "服务器端文件载入成功";
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //返回上级文件夹
        private void toolStripButtonReturn_Click(object sender, EventArgs e)
        {
            if (ftpClient == null)
            {
                MessageBox.Show("请先登录！");
                return;
            }
            ftpClient.SetPrePath();
            ShowFilesDirectory();
        }

        private void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            if (ftpClient == null)
            {
                MessageBox.Show("请先登录！");
                return;
            }
            ShowFilesDirectory();
        }

        private void menuDelete_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            string name = menuItem.Tag.ToString();
            ftpClient.RelatePath = string.Format("{0}/{1}",ftpClient.RelatePath, name);
            ftpClient.DeleteFile(out bool isOK);
            ftpClient.SetPrePath();
            if (isOK)
            {
                ShowFilesDirectory();
                lblMsg.Text = "删除成功";
            }
            else
                lblMsg.Text = "删除失败";
        }

        /// <summary>
        /// 右键另存为
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuSaveAs_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            string name = menuItem.Tag.ToString();
            lblMsg.Text = "开始下载";
            SaveFileDialog sfd = new SaveFileDialog()
            {
                FileName = name,
                Filter = "All Files|(*.*)",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Title = "另存为"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                //下载文件
                string filePath = sfd.FileName + ".temp";
                ftpClient.RelatePath = string.Format("{0}/{1}", ftpClient.RelatePath, name);
                try
                {
                    long size = 0;
                    if (File.Exists(filePath))
                    {
                        using (FileStream outputStream = new FileStream(filePath, FileMode.Open))
                        {
                            size = outputStream.Length;
                        }
                    }
                    ftpClient.Download(filePath, size, updateProgress);

                    File.Delete(filePath.Substring(0, filePath.Length - 5));
                    FileInfo fileInfo = new FileInfo(filePath);
                    fileInfo.MoveTo(filePath.Substring(0, filePath.Length - 5));

                    lblMsg.Text = "下载成功";
                    MessageBox.Show("下载完成");
                    toolStripProgressBar1.Value = 0;
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    lblMsg.Text = "下载失败";
                }
                finally
                {
                    ftpClient.SetPrePath();
                }
            }
            lblMsg.Text = "";
        }

        private void updateProgress(int total, int progress)
        {
            if(total == 0)
            {
                toolStripProgressBar1.Value = 100;
                return;
            }
            toolStripProgressBar1.Value = (int)((double)progress / total * 100);
        }
    }
}

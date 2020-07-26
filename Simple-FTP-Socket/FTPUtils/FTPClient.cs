using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

namespace FTPUtils
{
    public enum FtpContentType  //文件类型，0代表未定义类型，1代表文件，2代表文件夹
    {
        undefined = 0,
        file = 1,
        folder = 2
    }

    public class FTPClient
    {
        public string IpAddr { get; set; }
        public string RelatePath { get; set; }
        public string Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        private Socket cmdSocket;
        private Socket dataSocket;
        private const int BUFFER_SIZE = 4096;

        public FTPClient() { }
        public FTPClient(string ipAddr, string port, string userName, string password)
        {
            IpAddr = ipAddr;
            Port = port;
            UserName = userName;
            Password = password;
            RelatePath = "";
        }

        /// <summary>
        /// 接收控制socket收到的应答信息
        /// </summary>
        /// <returns></returns>
        private string CmdSocketReceive()
        {
            byte[] bytes = new byte[BUFFER_SIZE];
            int size = cmdSocket.Receive(bytes, bytes.Length, 0);
            return Encoding.UTF8.GetString(bytes, 0, size);
        }

        /// <summary>
        /// 开启连接
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool Connect()
        {
            cmdSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            dataSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            dataSocket.ReceiveTimeout = 0;
            dataSocket.SendTimeout = 0;
            try
            {
                cmdSocket.Connect(new IPEndPoint(IPAddress.Parse(IpAddr), int.Parse(Port)));
                var localEndPoint = ((IPEndPoint)cmdSocket.LocalEndPoint);
                dataSocket.Bind(new IPEndPoint(IPAddress.Parse(localEndPoint.Address.ToString()),
                    localEndPoint.Port + 1)); //与客户端绑定？？
                string response = CmdSocketReceive();
                if (response.StartsWith("220"))
                    return true;
                return false;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        /// <summary>
        /// 使用账户登录FTP服务器
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public bool Login()
        {
            cmdSocket.Send(Encoding.UTF8.GetBytes("USER " + UserName + "\r\n"));
            string response = CmdSocketReceive();

            if (response.StartsWith("331")) //331  用户名正常，需要密码
            {
                cmdSocket.Send(Encoding.UTF8.GetBytes("PASS " + Password + "\r\n"));
                response = CmdSocketReceive();
                if (response.StartsWith("230")) //230  用户已登录，请继续
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            try
            {
                cmdSocket.Send(Encoding.UTF8.GetBytes("QUIT" + "\r\n"));
                CmdSocketReceive();
            }
            catch (Exception)
            {
                throw;
            }
            cmdSocket.Close();
            dataSocket.Close();
        }

        /// <summary>
        /// 进入被动模式
        /// </summary>
        /// <returns></returns>
        private bool EnterPassiveMode()
        {
            // 启用 Binary Mode
            cmdSocket.Send(Encoding.UTF8.GetBytes("TYPE I" + "\r\n"));
            string response = CmdSocketReceive();
            if (!response.Contains("200")) //200  命令成功
                return false;
            // 进入被动模式
            cmdSocket.Send(Encoding.UTF8.GetBytes("PASV" + "\r\n"));
            response = CmdSocketReceive();
            if (response.Contains("227")) //227 进入被动模式
            {
                int server_data_port;   // Unspecified
                                        // 解析被动模式下服务器数据端口，比如：(127,0,0,1,74,93)
                                        // 端口即为74*256+93
                int le = response.LastIndexOf("(");
                int re = response.LastIndexOf(")");
                response = response.Substring(le + 1, re - le - 1);
                string[] da = response.Split(',');
                server_data_port = int.Parse(da[da.Length - 2]) * 256 + int.Parse(da[da.Length - 1]);
                dataSocket.ConnectAsync(new IPEndPoint(IPAddress.Parse(IpAddr), server_data_port)).Wait();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取当前路径下服务器端的文件信息
        /// </summary>
        /// <returns></returns>
        public string[] GetFilesList()
        {
            if (!EnterPassiveMode())
                throw new Exception("无法接收数据！");
            dataSocket.ReceiveBufferSize = 1 * 1024 * 1024;
            cmdSocket.Send(Encoding.GetEncoding("gb2312").GetBytes("LIST " + RelatePath + "\r\n"));
            string response = CmdSocketReceive();
            while (!response.Contains("226"))
                response = CmdSocketReceive();

            MemoryStream ms = new MemoryStream();
            byte[] buf = new byte[1 * 1024 * 1024];
            int length = 0;
            while (dataSocket.Available > 0 || (length >= 1 && buf[length - 1] != '\n'))
            {
                length = dataSocket.Receive(buf);
                // Connected 只反映上一个 Receive/Send 操作时的 Socket 状态
                if (dataSocket.Connected == false) throw new Exception("远程主机断开连接");
                ms.Write(buf, 0, length);
            }
            // 发送FIN
            dataSocket.Shutdown(SocketShutdown.Send);
            // 接收所有剩余的数据，直到Receive返回0，表明对方已发送FIN
            while (true)
            {
                length = dataSocket.Receive(buf);
                if (length == 0) break;
                ms.Write(buf, 0, length);
            }
            dataSocket.Disconnect(true);

            //将得到的文件信息分成一行一行，以\r\n分割
            string rawInfo = Encoding.GetEncoding("gb2312").GetString(ms.ToArray());
            if (rawInfo.LastIndexOf("\r\n") == rawInfo.Length - 2)
                rawInfo = rawInfo.Remove(rawInfo.Length - 2, 2);
            string[] filesInfo = Regex.Split(rawInfo, @"\r\n");
            return filesInfo;
        }

        /// <summary>
        /// 设置上级目录
        /// </summary>
        public void SetPrePath()
        {
            string relatePath = RelatePath;
            if (string.IsNullOrEmpty(relatePath) || relatePath.LastIndexOf("/") == 0)
                relatePath = "";
            else
                relatePath = relatePath.Substring(0, relatePath.LastIndexOf("/"));
            RelatePath = relatePath;
        }

        /// <summary>
        /// 获取上级目录
        /// </summary>
        public string GetPrePath()
        {
            string relatePath = RelatePath;
            if (string.IsNullOrEmpty(relatePath) || relatePath.LastIndexOf("/") == 0)
                relatePath = "";
            else
                relatePath = relatePath.Substring(0, relatePath.LastIndexOf("/"));
            return relatePath;
        }

        /// <summary>
        /// 设置相对路径
        /// </summary>
        /// <param name="folderName"></param>
        public void SetRelatePath(string folderName)
        {
            RelatePath = string.Format("{0}/{1}", RelatePath, folderName);
        }

        /// <summary>
        /// 删除服务器上的文件
        /// </summary>
        /// <param name="isOK"></param>
        public void DeleteFile(out bool isOK)
        {
            cmdSocket.Send(Encoding.GetEncoding("gb2312").GetBytes("DELE " + RelatePath + "\r\n"));
            string response = CmdSocketReceive();
            if (response.StartsWith("250"))
                isOK = true;
            else
                isOK = false;
        }

        /**************************************************************************************************/



        public void Download(string filepath, long size, Action<int, int> updateProgress)
        {
            long totalDownloadedByte = size;
            var totalBytes = GetFileSize();
            if (totalBytes == -1) throw new Exception("无法获取远程文件的文件大小，或该远程文件已经不存在");

            cmdSocket.Send(Encoding.GetEncoding("gb2312").GetBytes("CWD " + this.GetPrePath() + "\r\n"));
            string response = CmdSocketReceive();
            if (!response.StartsWith("250"))
                throw new Exception("文件目录访问出错");
            /*response = CmdSocketReceive();
            response = CmdSocketReceive();
            response = CmdSocketReceive();
            response = CmdSocketReceive();
            response = CmdSocketReceive();*/
            EnterPassiveMode();
            cmdSocket.Send(Encoding.UTF8.GetBytes("REST " + size + "\r\n"));
            response = CmdSocketReceive();
            if (!response.StartsWith("300") && !response.StartsWith("350"))
            {
                throw new Exception("响应错误");
            }
            cmdSocket.Send(Encoding.GetEncoding("gb2312").GetBytes("RETR " + RelatePath + "\r\n"));
            response = CmdSocketReceive();
            if (!response.StartsWith("125") && !response.StartsWith("150"))
            {
                throw new Exception("响应错误");
            }
            using (FileStream outputStream = new FileStream(filepath, FileMode.Append))
            {

                dataSocket.ReceiveBufferSize = 1 * 1024 * 1024;

                byte[] buffer = new byte[1 * 1024 * 1024];
                int readCount = dataSocket.Receive(buffer);
                updateProgress((int)totalBytes, (int)totalDownloadedByte);
                while (readCount > 0)
                {
                    totalDownloadedByte += readCount;
                    outputStream.Write(buffer, 0, readCount);
                    updateProgress((int)totalBytes, (int)totalDownloadedByte);
                    readCount = dataSocket.Receive(buffer);
                }
            }
            response = CmdSocketReceive();
            if (response.StartsWith("226"))
            {
                dataSocket.Shutdown(SocketShutdown.Send);
                dataSocket.Disconnect(true);
            }
            else
            {
                throw new Exception("响应错误");
            }

        }

        public void Upload(string localPath, Action<int, int> updateProgress)
        {
            FileInfo fileInf = new FileInfo(localPath);
            long allbye = fileInf.Length;
            var size = GetFileSize();
            if (size != -1)
            {
                throw new Exception("远程文件已存在");
            }
            RelatePath += ".temp";
            long startfilesize = GetFileSize();
            long startbye = startfilesize <= 0 ? 0 : startfilesize;
            updateProgress((int)allbye, (int)startfilesize);//更新进度条   


            string response;
            cmdSocket.Send(Encoding.GetEncoding("gb2312").GetBytes("CWD " + GetPrePath() + "\r\n"));
            response = CmdSocketReceive();
            EnterPassiveMode();
            if (startbye > 0)
            {
                cmdSocket.Send(Encoding.UTF8.GetBytes("REST " + startbye + "\r\n"));
                response = CmdSocketReceive();
                if (!response.StartsWith("300") && !response.StartsWith("350"))
                {
                    throw new Exception("响应错误");
                }
            }
            else
            {
                cmdSocket.Send(Encoding.UTF8.GetBytes("ALLO " + allbye + "\r\n"));
                response = CmdSocketReceive();
            }


            cmdSocket.Send(Encoding.GetEncoding("gb2312").GetBytes("STOR " + RelatePath + "\r\n"));
            response = CmdSocketReceive();
            if (!response.StartsWith("125") && !response.StartsWith("150"))
            {
                throw new Exception("响应错误");
            }

            using (FileStream fs = fileInf.OpenRead())
            {
                int buffLength = 1 * 1024 * 1024;
                byte[] buffer = new byte[buffLength];
                dataSocket.SendBufferSize = buffLength;
                fs.Seek(startbye, 0);
                int contentLen = fs.Read(buffer, 0, buffLength);
                while (contentLen != 0)
                {
                    dataSocket.Send(buffer, 0, contentLen, SocketFlags.None);
                    contentLen = fs.Read(buffer, 0, buffLength);
                    startbye += contentLen;
                    updateProgress((int)allbye, (int)startbye);
                }
                updateProgress(1, 1);
            }
            dataSocket.Shutdown(SocketShutdown.Send);
            dataSocket.Disconnect(true);

            cmdSocket.Send(Encoding.GetEncoding("gb2312").GetBytes("RNFR " + RelatePath + "\r\n"));
            CmdSocketReceive();
            cmdSocket.Send(Encoding.GetEncoding("gb2312").GetBytes("RNTO " + RelatePath.Substring(0,RelatePath.Length - 5) + "\r\n"));
            CmdSocketReceive();
        }

        private long GetFileSize()
        {
            try
            {
                cmdSocket.Send(Encoding.GetEncoding("gb2312").GetBytes("SIZE " + RelatePath + "\r\n"));
                var response = CmdSocketReceive();
                if (response.StartsWith("213"))
                {
                    return long.Parse(response.Substring(4, response.Length - 4));
                }
                return -1;
            }
            catch
            {
                return -1;
            }
        }


        public string GetFileLastModifiedTime()
        {
            cmdSocket.Send(Encoding.UTF8.GetBytes("MDTM " + RelatePath + "\r\n"));
            var response = CmdSocketReceive();
            if (response.StartsWith("213"))
            {
                return response.Substring(4, response.Length - 4); // 去掉响应码
            }
            return null;
        }

    }
}

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
            try
            {
                cmdSocket.Connect(new IPEndPoint(IPAddress.Parse(IpAddr), int.Parse(Port)));
                /*var localEndPoint = ((IPEndPoint)cmdSocket.LocalEndPoint);
                dataSocket.Bind(new IPEndPoint(IPAddress.Parse(localEndPoint.Address.ToString()), 
                    localEndPoint.Port + 1)); //与客户端绑定？？*/
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
            if (!response.StartsWith("200")) //200  命令成功
                return false;
            // 进入被动模式
            cmdSocket.Send(Encoding.UTF8.GetBytes("PASV" + "\r\n"));
            response = CmdSocketReceive();
            if (response.StartsWith("227")) //227 进入被动模式
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
            cmdSocket.Send(Encoding.UTF8.GetBytes("LIST " + RelatePath + "\r\n"));
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
            cmdSocket.Send(Encoding.UTF8.GetBytes("DELE " + RelatePath + "\r\n"));
            string response = CmdSocketReceive();
            if (response.StartsWith("250"))
                isOK = true;
            else
                isOK = false;
        }

/**************************************************************************************************/

        public void Download(string filepath, long size, Action<int,int> updateProgress)
        {
            FtpWebRequest reqFTP, ftpsize;
            Stream ftpStream = null;
            FtpWebResponse response = null;
            FileStream outputStream = null;
            try
            {
                outputStream = new FileStream(filepath, FileMode.Append);
                Uri uri = new Uri(string.Format("ftp://{0}:{1}{2}", IpAddr, Port, RelatePath));
                ftpsize = (FtpWebRequest)FtpWebRequest.Create(uri);
                ftpsize.UseBinary = true;
                ftpsize.ContentOffset = size;

                reqFTP = (FtpWebRequest)FtpWebRequest.Create(uri);
                reqFTP.UseBinary = true;
                reqFTP.KeepAlive = false;
                reqFTP.ContentOffset = size;
                ftpsize.Credentials = new NetworkCredential(UserName, Password);
                reqFTP.Credentials = new NetworkCredential(UserName, Password);
                ftpsize.Method = WebRequestMethods.Ftp.GetFileSize;
                FtpWebResponse re = (FtpWebResponse)ftpsize.GetResponse();
                long totalBytes = re.ContentLength;
                re.Close();

                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                response = (FtpWebResponse)reqFTP.GetResponse();
                ftpStream = response.GetResponseStream();
                long totalDownloadedByte = size;
                updateProgress((int)totalBytes, (int)totalDownloadedByte);
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];
                readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    totalDownloadedByte += readCount;
                    outputStream.Write(buffer, 0, readCount);
                    updateProgress((int)totalBytes, (int)totalDownloadedByte);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }
                ftpStream.Close();
                outputStream.Close();
                response.Close();

                //File.Delete(filepath.Substring(0, filepath.Length - 5));
                //FileInfo fileInfo = new FileInfo(filepath);
                //fileInfo.MoveTo(filepath.Substring(0, filepath.Length - 5));

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (ftpStream != null)
                {
                    ftpStream.Close();
                }
                if (outputStream != null)
                {
                    outputStream.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
                SetPrePath();
            }
        }

        public void Upload(string localPath, Action<int, int> updateProgress)
        {
            FileInfo fileInf = new FileInfo(localPath);
            long allbye = fileInf.Length;
            long startfilesize = GetFileSize();

            long startbye = startfilesize;
            updateProgress((int)allbye, (int)startfilesize);//更新进度条   

            string uri = string.Format("ftp://{0}:{1}{2}", this.IpAddr, this.Port, this.RelatePath + ".temp");

            FtpWebRequest request;
            request = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
            request.Credentials = new NetworkCredential(UserName, Password);
            request.KeepAlive = false;
            request.Method = WebRequestMethods.Ftp.AppendFile;
            request.UseBinary = true;
            request.ContentLength = fileInf.Length;
            request.Timeout = 10 * 1000;
            int buffLength = 2048; 
            byte[] buff = new byte[buffLength];
            FileStream fs = fileInf.OpenRead();
            Stream strm = null;
            try
            {
                strm = request.GetRequestStream();
                fs.Seek(startfilesize, 0);
                int contentLen = fs.Read(buff, 0, buffLength);
                while (contentLen != 0)
                {
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                    startbye += contentLen;
                    updateProgress((int)allbye, (int)startbye);
                    
                }
                strm.Close();
                fs.Close();
                Rename(fileInf.Name);
                SetPrePath();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
                if (strm != null)
                {
                    strm.Close();
                }
            }
        }

        private long GetFileSize()
        {
            try
            {
                FtpWebRequest reqFTP;
                //FileInfo fi = new FileInfo(filename);
                //string uri;
                //if (remoteFilepath.Length == 0)
                //{
                //    uri = "ftp://" + FtpServerIP + "/" + fi.Name;
                //}
                //else
                //{
                //    uri = "ftp://" + FtpServerIP + "/" + remoteFilepath + "/" + fi.Name;
                //}
                Uri uri = new Uri(string.Format("ftp://{0}:{1}{2}", IpAddr, Port, RelatePath) + ".temp");
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(uri);
                reqFTP.KeepAlive = false;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(UserName, Password);
                reqFTP.Method = WebRequestMethods.Ftp.GetFileSize;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                long filesize = response.ContentLength;
                return filesize;
            }
            catch
            {
                return 0;
            }
        }

        private void Rename(string filename)
        {
            try
            {
                string uri = string.Format("ftp://{0}:{1}{2}", this.IpAddr, this.Port, this.RelatePath + ".temp");
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                request.Credentials = new NetworkCredential(UserName, Password);
                request.UseBinary = true;
                request.Method = WebRequestMethods.Ftp.Rename;
                request.RenameTo = filename;
                request.KeepAlive = false;

                FtpWebResponse ftpWebResponse = (FtpWebResponse)request.GetResponse();
                Stream ftpResponseStream = ftpWebResponse.GetResponseStream();

            }
            catch (Exception)
            {
                throw;
            }
            //finally
            //{
            //    if (ftpResponseStream != null)
            //    {
            //        ftpResponseStream.Close();
            //    }
            //    if (ftpWebResponse != null)
            //    {
            //        ftpWebResponse.Close();
            //    }
            //}
        }
    }
}

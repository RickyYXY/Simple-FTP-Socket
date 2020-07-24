using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Threading;

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
        #region 属性
        /// <summary>
        /// IP地址
        /// </summary>
        public string IpAddr { get; set; }

        /// <summary>
        /// 相对路径
        /// </summary>
        public string RelatePath { get; set; }

        /// <summary>
        /// 端口号
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        #endregion

        #region 构造函数
        public FTPClient() { }
        public FTPClient(string ipAddr, string port, string userName, string password)
        {
            IpAddr = ipAddr;
            Port = port;
            UserName = userName;
            Password = password;
        }
        #endregion

        /// <summary>
        /// 获取服务器端的文件信息
        /// </summary>
        /// <param name="isOk"></param>
        /// <returns></returns>
        public string[] GetFilesDirectory(out bool isOk)
        {
            string method = WebRequestMethods.Ftp.ListDirectoryDetails;
            var statusCode = FtpStatusCode.DataAlreadyOpen;
            FtpWebResponse response = CallFTP(method);
            return ReadByLine(response, statusCode, out isOk);
        }

        /// <summary>
        /// 设置上级目录
        /// </summary>
        public void SetPrePath()
        {
            string relatePath = RelatePath;
            if (string.IsNullOrEmpty(relatePath) || relatePath.LastIndexOf("/") == 0)
            {
                relatePath = "";
            }
            else
            {
                relatePath = relatePath.Substring(0, relatePath.LastIndexOf("/"));
            }
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
            string method = WebRequestMethods.Ftp.DeleteFile;
            var statusCode = FtpStatusCode.FileActionOK;
            FtpWebResponse response = CallFTP(method);
            if (statusCode == response.StatusCode)
                isOK = true;
            else
                isOK = false;
            response.Close();
        }

        /// <summary>
        /// 请求FTP服务
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private FtpWebResponse CallFTP(string method)
        {
            //设置uri
            string uri = string.Format("ftp://{0}:{1}{2}", IpAddr, Port, RelatePath);
            //创建请求
            FtpWebRequest request;
            request = (FtpWebRequest)FtpWebRequest.Create(uri);
            //设置请求参数
            request.UseBinary = true;
            request.UsePassive = true;
            request.Credentials = new NetworkCredential(UserName, Password);
            request.KeepAlive = false;
            request.Method = method;
            //等待回复
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            return response;
        }

        /// <summary>
        /// 按行读取
        /// </summary>
        /// <param name="response"></param>
        /// <param name="statusCode"></param>
        /// <param name="isOk"></param>
        /// <returns></returns>
        private string[] ReadByLine(FtpWebResponse response, FtpStatusCode statusCode, out bool isOk)
        {
            List<string> lstAccpet = new List<string>();
            int clock = 0;
            isOk = false;
            while (clock <= 5)
            {
                if (response.StatusCode == statusCode)
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        string line = sr.ReadLine();
                        while (!string.IsNullOrEmpty(line))
                        {
                            lstAccpet.Add(line);
                            line = sr.ReadLine();
                        }
                    }
                    isOk = true;
                    break;
                }
                clock++;
                Thread.Sleep(200);
            }
            response.Close();
            return lstAccpet.ToArray();
        }

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

using System;
using System.Linq;
using System.IO;

namespace Xamarin_IPE_IOS.SRC.Model
{
    public class FileRead
    {
        FileInfo fileInfo;
        FileStream fileStream;
        int bufferSize = 4096;
        //static void Main(string[] args)
        //{
        //    var files = Directory.GetFiles(@"c:\", "*").Select(fn => new FileInfo(fn)).OrderBy(f => f.Length);
        //    FileRead t = new FileRead("C:\\Users\\RDPC1021\\Documents\\visual studio 2015\\Projects\\TcpFileTransfer\\TcpFileTransfer\\test.htm", 4096);
        //    t.Position = 100;
        //    Console.Out.WriteLine("{0}, {1}, {2}", t.Length, t.Position, t.ToString());
        //    Console.Out.WriteLine("{0}==>{1}", t.ParseResponse(t.ToString()), long.Parse(t.ParseResponse(t.ToString()) ?? "0"));
        //    while (true)
        //    {
        //        var buf = t.GetBlockData();
        //        t.Position += 100;
        //        if (buf == null || buf.Length == 0) break;
        //        Console.Out.WriteLine("{0}, {1}, {2} ", t.Length, t.Position, t.Percent);
        //    }
        //    Console.Out.WriteLine("{0}", t.ToString()); t.Delete();
        //}
        /// <summary>
        /// 初始化物件
        /// </summary>
        /// <param name="fileName">開啟檔案名稱</param>
        public FileRead(string fileName)
        {
            fileInfo = new FileInfo(fileName);
            if (fileInfo.Exists)
                fileStream = new FileStream(fileName, FileMode.Open);
        }
        /// <summary>
        /// 初始化物件
        /// </summary>
        /// <param name="fileName">開啟檔案名稱</param>
        /// <param name="bufSize">設定BLOCK大小</param>
        public FileRead(string fileName, int bufSize)
        {
            bufferSize = bufSize;
            fileInfo = new FileInfo(fileName);
            if (Exists)
                fileStream = new FileStream(fileName, FileMode.Open);
        }
        /// <summary>
        /// 檔案同步字串產生
        /// 檔案存在:字串回傳
        /// 檔案不存在:回傳null
        /// </summary>
        /// <returns>"Length=000000;FileName=Name"</returns>
        public override string ToString()
        {
            if (Exists)
                return "Length=" + Length + ";FileName=" + fileInfo.Name;
            else
                return null;
        }
        /// <summary>
        /// 分析回傳檔案位置，名稱
        /// 檔案指標移到指定位址
        /// 字串存在:字串回傳(長度解析)
        /// 字串不存在或長度為0:回傳null
        /// </summary>
        /// <param name="response">回傳字串</param>
        /// <returns>遠端檔案大小</returns>
        public string ParseResponse(string response)
        {
            if (response == null || response.Length == 0)
                return null;
            string[] items = response.Split(';');
            // 檔案大小
            string filelength = items[0].Substring(items[0].IndexOf("=") + 1);
            // 檔案名稱
            string filename = items[1].Substring(items[1].IndexOf("=") + 1);
            //檔案指標移到指定位址
            Position = long.Parse(filelength);
            // 回傳檔案大小(字串)
            return filelength;
        }
        /// <summary>
        /// 檔案是否存在
        /// </summary>
        public bool Exists
        {
            get
            {
                return (fileInfo == null) ? false : fileInfo.Exists;
            }
        }
        /// <summary>
        /// 檔案存在回傳檔案長度
        /// 檔案不存在回傳0
        /// </summary>
        public long Length
        {
            get
            {
                return (Exists) ? fileInfo.Length : 0;
            }
        }
        /// <summary>
        /// 檔案現在位址
        /// </summary>
        public long Position
        {
            get
            {
                return (Exists) ? fileStream.Position : 0;
            }
            set
            {
                if (Exists)
                    if (value > Length)
                        fileStream.Seek(Length, SeekOrigin.Begin);
                    else
                        fileStream.Seek(value, SeekOrigin.Begin);
            }
        }
        /// <summary>
        /// 資料區塊大小
        /// </summary>
        public int Size
        {
            get
            {
                return bufferSize;
            }
            set
            {
                bufferSize = value;
            }
        }
        /// <summary>
        /// 讀取檔案進度(百分比)
        /// </summary>
        public float Percent
        {
            get
            {
                if (!Exists || Length <= 0)
                    return 0.0f;
                else
                    return (float)Position / Length * 100.0f;
            }
        }
        /// <summary>
        /// 取得資料區塊
        /// </summary>
        /// <returns>資料區塊</returns>
        public byte[] GetBlockData()
        {
            long pktSize = ((Length - Position) >= bufferSize) ? bufferSize : (Length - Position);
            byte[] buf = new byte[pktSize];
            if (pktSize > 0)
                fileStream.Read(buf, 0, (int)pktSize);
            return buf;
        }
        /// <summary>
        /// 關閉檔案
        /// </summary>
        public void Close()
        {
            if (Exists)
            {
                if (fileStream != null)
                    fileStream.Close();
            }
            fileStream = null;
        }
        /// <summary>
        /// 刪除檔案
        /// </summary>
        public void Delete()
        {
            if (Exists)
            {
                Close();
                fileInfo.Delete();
                fileInfo = null;
            }
        }
    }

}

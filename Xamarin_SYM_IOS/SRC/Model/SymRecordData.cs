using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Data;
using System.IO;
using System.Collections.Concurrent;
using Foundation;
using System.Diagnostics;

namespace Xamarin_SYM_IOS.SRC.Model
{
    public class SymRecordData
    {
        string dbName = "SYM_SCHEMA";
        string dbPath;
        string documentPath;
        BinaryWriter writer;
        bool isFileOpen = false;

        public SymRecordData()
        {
            //android版
            //var filePath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/SYM_DB";
            //var file = new Java.IO.File(filePath);
            //if (!file.Exists())
            //{
            //    file.Mkdirs();
            //}

            //dbPath = Path.Combine(filePath, dbName + ".dat");
            //file = new Java.IO.File(dbPath);
            //if (!file.Exists())
            //{
            //    file.CreateNewFile();
            //}
            //// 讓 mtp 可直接存取檔
            //MediaScannerConnection.ScanFile(Application.Context, new string[] { file.AbsolutePath }, null, null);
            //writer = new BinaryWriter(File.Open(dbPath, FileMode.Append));


            //ios版
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var filePath = Path.Combine(documents, "SYM_DB");
            Directory.CreateDirectory(filePath);


            dbPath = Path.Combine(filePath, dbName + ".dat");
            documentPath = Path.Combine(documents, "SYM_DB");

            //File.WriteAllText(dbPath, "");
            if(writer == null)
                writer = new BinaryWriter(File.Open(dbPath, FileMode.Append));

        }

        public SymRecordData(String FileName)
        {
            dbName = FileName;
            CreateSymRecordData();
        }

        private void CreateSymRecordData()
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var filePath = Path.Combine(documents, "SYM_DB");
            Directory.CreateDirectory(filePath);


            dbPath = Path.Combine(filePath, dbName + ".dat");
            documentPath = Path.Combine(documents, "SYM_DB");


            bool isExists = File.Exists(dbPath);
            FileStream fs;
            try
            {
                if (isExists)
                {
                    if (isFileOpen)
                        return;
                    else
                    {
                        isFileOpen = true;
                        fs = File.Open(dbPath, FileMode.Append);
                    }
                }
                else
                {
                    fs = File.Create(dbPath);
                }

                if (writer == null)
                {
                    if (fs != null)
                        writer = new BinaryWriter(fs);
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
                fs = File.Create(dbPath);
            }


        }
        //android版
        //public long FreeBytes()
        //{
        //    long freeBytes = 0;
        //    StatFs stat = new StatFs(dbPath);
        //    freeBytes = stat.FreeBytes;
        //    return freeBytes;
        //}

        //ios版
        public long FreeBytes()
        {
            ulong freeBytes = 0;
            //freeBytes = NSFileManager.DefaultManager.GetFileSystemAttributes(Environment
                                                                        //.GetFolderPath(Environment.SpecialFolder.Personal))
                                                                        //.FreeSize;
            freeBytes = NSFileManager.DefaultManager.GetFileSystemAttributes(documentPath).FreeSize;
            return (long)freeBytes;
        }

        public bool Write(byte[] data)
        {
            if (writer == null)
                CreateSymRecordData();
            writer.Write(data);
            writer.Flush();

            return true;
        }

        public void Close()
        {
            if (writer != null)
                writer.Close();
            writer = null;
            isFileOpen = false;
        }

        //儲存資料檔案名稱
        public string FileName
        {
            get
            {
                return dbPath;
            }
        }
    }
}

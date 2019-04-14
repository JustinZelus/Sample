using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Xamarin_SYM_IOS.SRC.Utils;
using Xamarin_SYM_IOS.ViewControllers;

namespace Xamarin_SYM_IOS.SRC.Model
{
    public class NetWorkPacket
    {
        // 本機端的流水號
        static byte localSN = 0;
        // 本機端訊息種類
        static byte localKind = 0;
        // 遠端的流水號
        static byte remoteSN = 0;
        // 遠端訊息種類
        static byte remoteKind = 0;
        // 定義封包類型ID號碼
        public enum Type
        {
            Sync,
            Header,
            DTC,
            LVDataComp,
            LVDataRaw,
            GPS = 16,
            Photo,
            UploadFile,
            Sync_Sync = Sync + 0x80, // 使用本地時間訊息
            Header_Sync,
            DTC_Sync,
            LVDataComp_Sync,
            LVDataRaw_Sync,
            GPS_Sync = GPS + 0x80,
            Photo_Sync,
            UploadFile_Sync
        }
        public static byte LocalSN
        {
            get
            {
                return localSN;
            }
            set
            {
                localSN = value;
            }
        }
        public static byte RemoteSN
        {
            get
            {
                return remoteSN;
            }
            set
            {
                remoteSN = value;
            }
        }
        public static byte RemoteKind
        {
            get
            {
                return remoteKind;
            }
            set
            {
                remoteKind = value;
            }
        }
        static public byte[] Pack(byte[] bytes, byte type)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(CSharpTcpClient.ToBytes((short)bytes.Count()));
            bw.Write(type);
            bw.Write(bytes);
            bw.Write(localSN++);
            return ms.ToArray();
        }
        /// <summary>
        /// 資料加入封包訊息(長度,類型,時間(可選))
        /// </summary>
        /// <param name="bytes">資料</param>
        /// <param name="type">類型</param>
        /// <param name="Timestamp">時間(for JAVA)</param>
        /// <returns>打包資料(以上傳格式打包)</returns>
        static public byte[] Pack(byte[] bytes, byte type, long Timestamp)
        {
            #region 沒有資料就返回
            // 2018/01/24 新增
            // 防止空資枓打包變成空的資料包
            if (bytes == null || bytes.Length == 0)
                return null;
            #endregion
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            // 計算封包長度
            short packetSize = (short)bytes.Count();
            // 如果type的msb為1時,會加入訊息(ID, ECUID, 時間)
            if ((type & 0x80) > 0)
            {
                // packetSize += sizeof(UserID)+sizeof(EcuID)+sizeof(time)
                packetSize += sizeof(long) + sizeof(Int32) + sizeof(Int32);
            }
            // 寫入封包長度
            bw.Write(CSharpTcpClient.ToBytes(packetSize));
            // 寫入封包類型
            bw.Write(type);
            // 加入訊息(ID, ECUID, 時間)
            if ((type & 0x80) > 0)
            {
                // 2018/01/23 加入訊息ID, ECUID
                //Console.WriteLine();
                bw.Write(CSharpTcpClient.ToBytes((Int32)UploadViewController.Instance.UserID));
                bw.Write(CSharpTcpClient.ToBytes((Int32)UploadViewController.Instance.EcuID));
                // 2018/01/23 加入時間訊息
                bw.Write(CSharpTcpClient.ToBytes(Timestamp));
            }
            bw.Write(bytes);
            bw.Write(localSN++);
            return ms.ToArray();
        }
        static public bool CheckLocalSN(byte SN)
        {
            if (SN == localSN)
            {
                localSN++;
                return true;
            }
            return false;
        }
        static public bool CheckRemoteSN(byte SN)
        {
            if (SN == remoteSN)
            {
                remoteSN++;
                return true;
            }
            return false;
        }
    }
}

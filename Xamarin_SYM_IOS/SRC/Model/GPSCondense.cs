//#define 舊程式
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin_SYM_IOS.SRC.Utils;

namespace Xamarin_SYM_IOS.SRC.Model
{
    // GPS DATA 壓縮
    public class GPSCondense
    {
        static public byte[] ToBytes(double longitude, double latitude)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bwOut = new BinaryWriter(ms);
#if 舊程式
   byte[] byteArray;
   /// 經度
   byteArray = BitConverter.GetBytes((short) 6001);
   if (BitConverter.IsLittleEndian)
    Array.Reverse(byteArray);
   bwOut.Write(byteArray);
   byteArray = BitConverter.GetBytes(longitude);
   if (BitConverter.IsLittleEndian)
    Array.Reverse(byteArray);
   bwOut.Write(byteArray);
   /// 緯度
   byteArray = BitConverter.GetBytes((short) 6002);
   if (BitConverter.IsLittleEndian)
    Array.Reverse(by`   teArray);
   bwOut.Write(byteArray);
   byteArray = BitConverter.GetBytes(latitude);
   if (BitConverter.IsLittleEndian)
    Array.Reverse(byteArray);
   bwOut.Write(byteArray);
#else
            /// 經度
            bwOut.Write(CSharpTcpClient.ToBytes((short)6001));
            bwOut.Write(CSharpTcpClient.ToBytes(longitude));
            /// 緯度
            bwOut.Write(CSharpTcpClient.ToBytes((short)6002));
            bwOut.Write(CSharpTcpClient.ToBytes(latitude));
#endif
            // 回傳
            return ms.ToArray();
        }
    }
}

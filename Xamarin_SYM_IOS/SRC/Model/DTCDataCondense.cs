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
    // DTC DATA 壓縮
    public class DTCDataCondense
    {
        // DTC資料
        static public byte[] ToBytes(List<Int32> DTCValues)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bwOut = new BinaryWriter(ms);
#if 舊程式
   byte[] byteArray;
   foreach(var item in DTCValues) {
    // 寫入DTC到Buffer
    byteArray = BitConverter.GetBytes((Int32) item);
    if (BitConverter.IsLittleEndian)
     Array.Reverse(byteArray);
    bwOut.Write(byteArray);
   }
#else
            foreach (var item in DTCValues)
            {
                bwOut.Write(CSharpTcpClient.ToBytes((Int32)item));
            }
#endif
            // 回傳
            return ms.ToArray();
        }
    }
}

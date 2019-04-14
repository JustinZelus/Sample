//#define 舊程式
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin_SYM_IOS.SRC.Utils;
// 2018/01/23 修改程式架構不變(增加可讀性)
/// <summary>
/// JAVA Read/Write 都是使用BigEndian
/// 所以資料必須Reverse
/// </summary>

namespace Xamarin_SYM_IOS.SRC.Model
{
    // LV DATA 壓縮
    public class InfoDataCondense
    {
        static public byte[] ToBytes(
         String InterfaceSN,
         String FW_Version,
         String SW_Version,
         String Brand,
         string VehicleId,
         String VehicleName,
         String ModCode,
         int EcuID)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bwOut = new BinaryWriter(ms);
            int StringBufferSize = 0;
            //bwOut.Write((byte)InterfaceSN.ToArray().Length);
            StringBufferSize = (InterfaceSN == null) ? 0 : Encoding.UTF8.GetBytes(InterfaceSN).Length;
            bwOut.Write((byte)StringBufferSize);
            if (StringBufferSize > 0)
                bwOut.Write(InterfaceSN.ToArray());
            StringBufferSize = (FW_Version == null) ? 0 : Encoding.UTF8.GetBytes(FW_Version).Length;
            bwOut.Write((byte)StringBufferSize);
            if (StringBufferSize > 0)
                bwOut.Write(FW_Version.ToArray());
            StringBufferSize = (SW_Version == null) ? 0 : Encoding.UTF8.GetBytes(SW_Version).Length;
            bwOut.Write((byte)StringBufferSize);
            if (StringBufferSize > 0)
                bwOut.Write(SW_Version.ToArray());
            StringBufferSize = (Brand == null) ? 0 : Encoding.UTF8.GetBytes(Brand).Length;
            bwOut.Write((byte)StringBufferSize);
            if (StringBufferSize > 0)
                bwOut.Write(Brand.ToArray());
            // 2017/11/23 新增
            StringBufferSize = (VehicleId == null) ? 0 : Encoding.UTF8.GetBytes(VehicleId).Length;
            bwOut.Write((byte)StringBufferSize);
            if (StringBufferSize > 0)
                bwOut.Write(VehicleId.ToArray());
            StringBufferSize = (VehicleName == null) ? 0 : Encoding.UTF8.GetBytes(VehicleName).Length;
            bwOut.Write((byte)StringBufferSize);
            if (StringBufferSize > 0)
                bwOut.Write(VehicleName.ToArray());
            StringBufferSize = (ModCode == null) ? 0 : Encoding.UTF8.GetBytes(ModCode).Length;
            bwOut.Write((byte)StringBufferSize);
            if (StringBufferSize > 0)
                bwOut.Write(ModCode.ToArray());
#if 舊程式
   byte[] byteArray;
   byteArray = BitConverter.GetBytes((Int32) EcuID);
   if (BitConverter.IsLittleEndian)
    Array.Reverse(byteArray);
   bwOut.Write(byteArray);
#else
            bwOut.Write(CSharpTcpClient.ToBytes((Int32)EcuID));
#endif
            // 回傳
            return ms.ToArray();
        }
    }
}

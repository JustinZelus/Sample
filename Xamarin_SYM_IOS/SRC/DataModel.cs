

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using IcmLib.Database.IcmDBHelper;
using System.Runtime.CompilerServices;
using IcmLib.Data.Unpacker;
using IcmLib.Data;
using System.IO;
using IcmLib;
using IcmLib.Database;
using Foundation;
using CoreFoundation;
using System.Collections.Concurrent;
using CommonLib.IcmLib.Data;
using IcmComLib.Utils.iOS;
using Xamarin_SYM_IOS.SRC.Utils;
using IcmComLib_iOS.IcmComLib.Utils;
using CommonLib.IcmLib.Database;
using static CommonLib.IcmLib.Database.DemoBinUnpacker;

namespace iPhoneBLE
{
    /// <summary>
    /// 資料模組
    /// </summary>
    public class DataModel
    {
        

        /// <summary>
        /// 物件鎖
        /// </summary>
        public static readonly object DataModelOpLocker = new object();

        /// <summary>
        /// SharedPreferencesExtractor
        /// </summary>
        SYMSharedPreferencesExtractor spf;
        public SYMSharedPreferencesExtractor SYMSharedPreferencesExtractor { get => spf; }
        /// <summary>
        /// 目前裝置國家名稱 - 多語用
        /// </summary>
        private String mCountryName = null;



        #region INIT


        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="activity"></param>
        public DataModel(String countryName = null)
        {
            mCountryName = countryName;
            spf = new SYMSharedPreferencesExtractor();
            InitIcmDB();

         
        }
#endregion




#region LV DATA MODEL
        /// <summary>
        /// Private LV Data Unpacker
        /// </summary>
        //private BleVdiDataUnpacker vdiUnpacker = new BleVdiDataUnpacker();
        private BleVdiDataUnpacker vdiUnpacker;

        /// <summary>
        /// Public LV Data Unpacker
        /// </summary>
        public BleVdiDataUnpacker VdiUnpacker
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return vdiUnpacker;
            }
        }

        /// <summary>
        /// 蒐集所有ECU支援的LV IDs
        /// </summary>
        /// <param name="lvIds"></param>
        public void CollectSupportedEcuLvIds(byte[] lvIds)
        {
            if (vdiUnpacker == null)
                return;
            VdiUnpacker.Add((byte[])lvIds.Clone());
        }

        /// <summary>
        /// 清除所有的LV IDs
        /// </summary>
        public void ClearSupportedEcuLvIds()
        {
            VdiUnpacker.ClearLVIDs();
        }

        /// <summary>
        /// 目前選擇的LV ID
        /// </summary>
        public List<int> SelectedLvIds
        {
            get
            {
                return vdiUnpacker.CurrentDmIDValues.Values.ToList();
            }
        }

        /// <summary>
        /// 設定選擇的LV ID
        /// </summary>
        /// <param name="currentLvIds">目前選擇?LV ID</param>
        public void SettingSelectedLvIds(byte[] currentLvIds)
        {
            vdiUnpacker.Setting(currentLvIds);
        }

        /// <summary>
        /// 清除LV DATA資料
        /// </summary>
        public void ClearLvDatas()
        {
            VdiUnpacker.ClearLVData();
        }

        /// <summary>
        /// LV DATA 數據資料拆解函數
        /// </summary>
        /// <param name="lvDatas"></param>
        public void UnpackLvDatas(byte[] lvDatas)
        {
            VdiUnpacker.Unpack(lvDatas);
        }

        /// <summary>
        /// 獲得LV DATA數據資料
        /// </summary>
        public ConcurrentDictionary<Int32, Single> LvValues
        {
            get
            {
                return vdiUnpacker.Values;
            }
        }



        //public List<float> LvValuesToList{
        //    get
        //    {
        //        return (float)(LvValues.Values).ToList();
        //    }
        //}

        /// <summary>
        /// 獲得LV DATA字串
        /// </summary>
        public List<string> LvValuesString
        {
            get
            {
                var lvValues = vdiUnpacker.Values;
                if (lvValues == null)
                    return null;
                try
                {
                    return ConvertLvValuesToStrings(lvValues);
                }
                catch (Exception ex)
                {
                    return null;
                }

            }
        }

        /// <summary>
        /// 轉換LV DATA為字串函數
        /// </summary>
        /// <param name="dmVals">LV DATAs</param>
        /// <returns>LV DATA??</returns>
        private List<string> ConvertLvValuesToStrings(ConcurrentDictionary<int, float> dmVals)
        {
            var dmDb = DmIcmDBHelper.DmValues;
            if (dmVals == null || dmDb == null)
                return null;

            List<string> convertResults = new List<string>();
            string value = "";
            string result = "";
            DmData dbInfo = null;
            foreach (var key in dmVals.Keys.ToList())
            {
                if (dmDb.ContainsKey(key))
                {
                    result = "";
                    dbInfo = dmDb[key];
                    if (dbInfo.PatternDisplay.Count > 0)
                    {
                        if (dbInfo.PatternDisplay.ContainsKey((int)dmVals[key]))
                        {
                            value = dbInfo.PatternDisplay[(int)dmVals[key]];
                        }
                        else
                        {
                            value = "0";
                        }
                    }
                    else
                    {
                        value = dmVals[key].ToString("F" + dbInfo.NumberOfDecimals);
                    }

                    //result += dbInfo.ID + ". " + dbInfo.Name + " , " + value + " " + dbInfo.Unit + "\r\n";
                    result = dbInfo.ID + "\t" + dbInfo.Name + "\t" + value + "\t" + dbInfo.Unit + "\t" + dbInfo.ShortName;
                    convertResults.Add(result);
                }

            }
            return convertResults;

        }

        /// <summary>
        /// Private DmIcmDBHelper 
        /// </summary>
        private DmIcmDBHelper dmIcmDBHelper = null;
        public DmIcmDBHelper DmIcmDBHelper
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return dmIcmDBHelper;
            }
        }

        /// <summary>
        /// Private Ecu LV 所有ID
        /// </summary>
        private List<int> allEcuLvIds = new List<int>();

        /// <summary>
        /// 設定初始LiveData的四個動態值順序
        /// 170    Time Since Engine Start ;Run Time
        /// 11    Abs. throttle position SSR  ;TPS
        /// 20    Air Flow Rate from Mass Air Flow Sensor ;MAF
        /// 102    Number of diagnosis codes   ;Number of DTCs
        /// </summary>
        //private List<int> lvDataDefaultIdList = new List<int> { 170, 11, 20, 102 };
        private List<int> lvDataDefaultIdList = new List<int>();

        /// <summary>
        /// Public Ecu LV 所有ID
        /// </summary>
        public List<int> AllEcuLvIds
        {
            get
            {
                if (lvDataDefaultIdList != null && lvDataDefaultIdList.Count > 0)
                {
                    var allEcuSupportedLvIds = VdiUnpacker.AllDmIDValues.Values.ToList();
                    allEcuLvIds.Clear();
                    foreach (var lvDataDefaultId in lvDataDefaultIdList)
                    {
                        if (allEcuSupportedLvIds.Contains(lvDataDefaultId))
                        {
                            allEcuLvIds.Add(lvDataDefaultId);
                            allEcuSupportedLvIds.Remove(lvDataDefaultId);
                        }
                    }

                    foreach (var item in allEcuSupportedLvIds)
                    {
                        allEcuLvIds.Add(item);
                    }
                }
                else
                    allEcuLvIds = VdiUnpacker.AllDmIDValues.Values.ToList();
                return allEcuLvIds;
            }
        }

        /// <summary>
        /// 獲取所有通訊後的Lv ID，並經由EcuId過濾表過濾
        /// </summary>
        public List<int> AllEcuLvIdsFilterByEcuIdList(List<uint> filterEcuIdList)
        {
            List<int> result = new List<int>();
            var allEcuIds = AllEcuLvIds;
            var allDmValues = StateMachine.DataModel.DmIcmDBHelper.DmValues.Values;
            foreach (var filterEcuId in filterEcuIdList)
            {
                var filterDmValues = allDmValues.Where(x => x.EcuID == filterEcuId);
                foreach (var filterDmValue in filterDmValues)
                {
                    if (allEcuIds.Contains((int)filterDmValue.ID))
                        result.Add((int)filterDmValue.ID);
                }
            }
            return result;
        }

        /// <summary>
        /// 獲取所有通訊後的Lv ID，並經由EcuId過濾表過濾
        /// </summary>
        public Dictionary<int, DmData> AllEcuDmDatasFilterByEcuIdList(List<uint> filterEcuIdList)
        {
            Dictionary<int, DmData> result = new Dictionary<int, DmData>();
            var allEcuIds = AllEcuLvIds;
            var allDmValues = StateMachine.DataModel.DmIcmDBHelper.DmValues.Values;
            foreach (var filterEcuId in filterEcuIdList)
            {
                var filterDmValues = allDmValues.Where(x => x.EcuID == filterEcuId);
                foreach (var filterDmValue in filterDmValues)
                {
                    if (allEcuIds.Contains((int)filterDmValue.ID))
                        result.Add((int)filterDmValue.ID, filterDmValue.Clone() as DmData);
                }
            }
            return result;
        }

        /// <summary>
        /// Dm Data 資料庫所有資料
        /// </summary>
        public List<DmData> AllLvDatasForGauge
        {
            get
            {
                if (VdiUnpacker == null)
                    return null;
                else
                    return VdiUnpacker.AllLvDatasForGauge;
            }
        }

        /// <summary>
        /// Dm Data 資料庫通訊後支援的資料
        /// </summary>
        public List<DmData> CurrentLvDatasForGauge
        {
            get
            {
                if (VdiUnpacker == null)
                    return null;
                else
                    return VdiUnpacker.CurrentLvDatasForGauge;
            }
        }

        /// <summary>
        /// 儲存所有ECU LV NAME的LIST
        /// </summary>
        private List<String> allEcuLvNameList = new List<string>();

        /// <summary>
        /// 儲存所有ECU LV NAME的一維陣列
        /// </summary>
        public List<String> AllEcuLvNames
        {
            get
            {
                //var EcuAllLvIDs = vdiUnpacker.AllDmIDValues;
                var EcuAllLvIDs = AllEcuLvIds;
                var DmDbDatas = dmIcmDBHelper.DmValues;
                if (EcuAllLvIDs == null || DmDbDatas == null)
                    return null;

                allEcuLvNameList.Clear();
                foreach (var key in EcuAllLvIDs)
                {
                    if (DmDbDatas.ContainsKey(key))
                        allEcuLvNameList.Add(DmDbDatas[key].Name);
                }
                return allEcuLvNameList;
            }
            //get
            //{
            //    var EcuAllLvIDs = vdiUnpacker.AllDmIDValues;
            //    var DmDbDatas = dmIcmDBHelper.DmValues;
            //    if (EcuAllLvIDs == null || DmDbDatas == null)
            //        return null;

            //    allEcuLvNameList.Clear();
            //    foreach (var key in EcuAllLvIDs.Values)
            //    {
            //        if (DmDbDatas.ContainsKey(key))
            //            allEcuLvNameList.Add(DmDbDatas[key].Name);
            //    }
            //    return allEcuLvNameList;
            //}
        }

        /// <summary>
        /// 儲存所有通訊完後，ECU LV NAME的一維陣列，並經由EcuId過濾表過濾
        /// </summary>
        public List<String> AllEcuLvNamesFilterByEcuIdList(List<uint> filterEcuIdList)
        {
            //var EcuAllLvIDs = vdiUnpacker.AllDmIDValues;
            var EcuAllLvIDs = AllEcuLvIdsFilterByEcuIdList(filterEcuIdList);
            var DmDbDatas = dmIcmDBHelper.DmValues;
            if (EcuAllLvIDs == null || DmDbDatas == null)
                return null;

            allEcuLvNameList.Clear();
            foreach (var key in EcuAllLvIDs)
            {
                if (DmDbDatas.ContainsKey(key))
                    allEcuLvNameList.Add(DmDbDatas[key].Name);
            }
            return allEcuLvNameList;
        }

        /// <summary>
        /// Public MotorVinIcmDBHelper
        /// <summary>
        private MotorVinUnpacker motorVinUnpacker = null;


        /// <summary>
        /// MotorVinUnpacker
        /// </summary>

        public MotorVinUnpacker VinIcmUnpacker
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return motorVinUnpacker;
            }
        }

        /// <summary>
        /// Public DmIcmDBHelper
        /// </summary>
        private VinIcmDBHelper vinIcmDBHelper = null;

        /// <summary>
        /// 獲得VinIcmDBHelper實例
        /// </summary>
        public VinIcmDBHelper VinIcmDBHelper
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return vinIcmDBHelper;
            }
        }
#endregion


#region VIN DATA MODEL
        private VINDecoder vinDecoder = null;

       


        public IcmDB GetAssetsIcmDB(String filePath)
        {
            IcmDB db = null;
            String str = "IcmDB Path : \r\n" + filePath + "\r\n";

            //db = IcmDB.OpenIcmDB("/Users/icm_mobile/Documents/DB/GET_VIN.dat");
            db = IcmDB.OpenIcmDB(filePath);
            foreach (IcmDB.Table table in db.Tables)
            {
                Console.WriteLine(table.Name);
                str += table.Name;
                str += "\r\n";
            }
            Console.WriteLine("Load DB {0} Completed.", db.Name);
            return db;
        }

        /// <summary>
        /// Read IcmDB from Bundle For iOS
        /// </summary>
        /// <param name="filePath">iOS Bundle檔案路徑</param>
        /// <param name="fileExt">iOS Bundle檔案副檔名</param>
        /// <returns>IcmDB實例</returns>
        public IcmDB GetIcmDBFromBundle(String filePath, String fileExt)
        {
            IcmDB db = null;
            var resPath = NSBundle.MainBundle.PathForResource(filePath, fileExt);
            String str = "IcmDB Path : \r\n" + resPath + "\r\n";

            //db = IcmDB.OpenIcmDB("/Users/icm_mobile/Documents/DB/GET_VIN.dat");
            db = IcmDB.OpenIcmDB(resPath);
            if (db == null)
                return null;

#if DEBUG
            foreach (IcmDB.Table table in db.Tables)
            {
                Console.WriteLine(table.Name);
                str += table.Name;
                str += "\r\n";
            }
            Console.WriteLine("Load DB {0} Completed.", db.Name);
#endif
            return db;
        }


        private readonly String DM_DB_FileName = @"IcmDB/DM";
        private readonly String DTC_DB_FileName = @"IcmDB/DTC";
        private readonly String VIN_DB_FileName = @"IcmDB/MOTOR_VIN";
        private readonly String DM_ICON_DB_FileName = @"IcmDB/DM_ICON";
        private readonly String DEMO_BIN_FileName = @"Bin/M3_F6A_GR125_demo.bin";
        private readonly String ECU_INFO_DB_FileName = @"IcmDB/ECU_INFO";

        /// <summary>
        /// 初始化ICMDB
        /// </summary>
        private void InitIcmDB()
        {
            //INIT DATA MONITOR DB
            IcmDB dmDB = GetIcmDBFromBundle(DM_DB_FileName, @"dat");

            dmIcmDBHelper = new DmIcmDBHelper(dmDB, mCountryName,Encoding.Unicode);

            //INIT VIN DB
            IcmDB vinDB = GetIcmDBFromBundle(VIN_DB_FileName, @"dat");
            //IcmDB vinDB = GetIcmDBFromBundle(@"IcmDB/MOTOR_VIN", @"dat");
            //vinIcmDBHelper = new VinIcmDBHelper(vinDB, mCountryName);
            motorVinUnpacker = new MotorVinUnpacker(vinDB,mCountryName,Encoding.Unicode);


            //INIT DATA MONITOR UNPACKER
            vdiUnpacker = new BleVdiDataUnpacker(dmIcmDBHelper);

            //INIT DTC DEB
            IcmDB dtcDB = GetIcmDBFromBundle(DTC_DB_FileName, @"dat");
            dtcUnpacker = new MotorDtcDataUnpacker(dtcDB, mCountryName,Encoding.Unicode);

            //INIT DM ICON DB
            IcmDB dmIcon = GetIcmDBFromBundle(DM_ICON_DB_FileName, @"dat");
            dmIconIcmDBHelper = new DmIconIcmDBHelper(dmIcon, mCountryName, Encoding.Unicode);

            //IcmDB demoBin = GetIcmDBFromBundle(DEMO_BIN_FileName,@"bin");
            string fileName = DEMO_BIN_FileName;
            var demoBin = new FileStream(fileName, FileMode.Open,FileAccess.Read);
            //var asset = mActivity.Assets.Open(fileName);
            //demoBinUnpacker = new DemoBinUnpacker(asset);
            demoBinUnpacker = new DemoBinUnpacker(demoBin);

            IcmDB ecuDB = GetIcmDBFromBundle(ECU_INFO_DB_FileName, @"dat");
            ecuInfoIcmDBHelper = new EcuInfoIcmDBHelper(ecuDB, mCountryName, Encoding.Unicode);
        }


        private String vin;
        /// <summary>
        /// 獲取VIN字串
        /// </summary>
        public String VIN
        {
            get
            {
                return vin;
            }
        }

        /// <summary>
        /// VIN解碼
        /// </summary>
        /// <param name="VIN">VIN碼</param>
        public void DecodeVIN(String VIN)
        {

            if (vinDecoder == null)
            {
                this.vin = VIN;
                vinDecoder = new VINDecoder(VIN);
            }
            else
            {   
                this.vin = VIN;
                vinDecoder.Setting(VIN);
            }
        }

        /// <summary>
        /// 獲得機車名稱
        /// </summary>
        public String VehicleNameValue
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                if (motorVinUnpacker == null)
                    return null;

                return motorVinUnpacker?.GetVehicleName();
            }
        }

        /// <summary>
        /// 獲得製造商代碼
        /// </summary>
        public String ManufactureValue
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                if (motorVinUnpacker == null)
                    return null;

                return motorVinUnpacker?.GetManufacture();
            }
        }
#endregion


        /// <summary>
        /// 機車模組代碼
        /// </summary>
        public String ModuleCode
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                if (motorVinUnpacker == null)
                    return null;

                //return motorVinIcmDBHelper.GetManufacture(vinDecoder.VIN.WMI);
                return motorVinUnpacker?.ModuleCode;
            }
        }


#region INFO VALUE
        private byte[] infoValue;
        /// <summary>
        /// 獲得Info數據資料
        /// </summary>
        public byte[] InfoValue
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                infoValue = value;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return infoValue;
            }
        }

        /// <summary>
        /// 獲得Info字串型態
        /// </summary>
        private String InfoValueString
        {
            get
            {
                if (infoValue == null)
                    return "";

                try
                {
                    return System.Text.Encoding.Default.GetString(infoValue);
                }
                catch (Exception ex)
                {
                    return "";
                }
            }
        }

        private float fuelConsumption = 0.0f;

        public void ConvertInfoValue2FuelConsumption()
        {
            if (InfoValue == null)
                fuelConsumption = 0.0f;
            else
            {
                try
                {
                    Array.Reverse(InfoValue);
                    fuelConsumption = BitConverter.ToSingle(InfoValue, 0);
                }
                catch
                {
                    fuelConsumption = 0.0f;
                }
            }
        }

        public float FuelConsumption
        {
            set
            {
                fuelConsumption = value;
            }

            get
            {
                return fuelConsumption / 1000;
            }
        }

#endregion


#region VALVE VALUE
        public ValveMode ValveMode = ValveMode.NONE_MODE;
        public int ValveValue
        {
            get
            {
                return (int)ValveMode;
            }
        }
#endregion

#region DTC DATA MODEL
        //private DtcDataUnpacker dtcUnpacker = null;
        private MotorDtcDataUnpacker dtcUnpacker = null;
        /// <summary>
        /// 獲取DtcUnpacker實例
        /// </summary>
        public MotorDtcDataUnpacker DtcUnpacker
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return dtcUnpacker;
            }
        }

        /// <summary>
        /// 獲取DTC 所有資料
        /// </summary>
        public DtcData[] DtcValues
        {
            get
            {
                if (dtcUnpacker == null)
                    return null;

                return dtcUnpacker.Values;
            }
        }

        /// <summary>
        /// 獲取DTC Mapping資料
        /// </summary>
        //public Dictionary<int, DtcData> DtcValuesMap
        //{
        //    get
        //    {
        //        if (dtcUnpacker == null)
        //            return null;
        //        return dtcUnpacker.ValuesMap;
        //    }
        //}

        public Dictionary<UInt32,ConcurrentDictionary<int,DtcData>> DtcValuesMap
        {
            get
            {
                if (dtcUnpacker == null)
                    return null;
                return dtcUnpacker.ValuesMap;
            }
        }

        private uint ecuId = 0;

        /// <summary>
        /// 獲取EcuID
        /// </summary>
        public uint EcuID
        {
            set
            {
                ecuId = value;
            }
            get
            {
                return ecuId;
            }
        }

        /// <summary>
        /// 獲取所有Lv資料
        /// </summary>
        public Dictionary<int, DmData> LvDatas
        {
            get
            {
                if (vdiUnpacker == null)
                    return null;
                else
                    return vdiUnpacker.LvDatas;
            }
        }



        public List<DmDataGroup> DemoLvDatas
        {
            get
            {
                if (demoBinUnpacker == null)
                    return null;
                else
                    return demoBinUnpacker.Values;
            }
        }

        /// <summary>
        /// 清除所有DTC資料
        /// </summary>
        public void ClearDtcValues()
        {
            if (dtcUnpacker == null)
                return;

            dtcUnpacker.Clear();
        }
#endregion


#region GETSTURE DATA
        public GesturePos GestureValue
        {
            get
            {
                if (vdiUnpacker == null)
                    return GesturePos.NONE;
                else
                    return vdiUnpacker.GesturePosValue;
            }
        }
#endregion

#region HARDWARE STATUS
        public bool HardwareStatus
        {
            get
            {
                if (vdiUnpacker == null)
                    return false;
                else


                    return vdiUnpacker.HardwareStatus;
            }
        }
#endregion

#region READINESS DATA
        public Dictionary<int, int> AllReadinessIDValues
        {
            get
            {
                if (vdiUnpacker == null)
                    return null;
                return vdiUnpacker.AllReadinessIDValues;
            }
        }
#endregion

#region READ WRITE MEMORY
        private IPEReadWriteMemoryValue ipeReadWriteMemoryValue = new IPEReadWriteMemoryValue();
        public IPEReadWriteMemoryValue IPEReadWriteMemoryValue
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return ipeReadWriteMemoryValue;
            }
        }
        ////public ConcurrentDictionary<int, float> ReadWriteMemoryValues
        ////{
        ////    get {
        ////        return StateMachine.BLEComModel.RWMemoryResultValues;
        ////    }
        ////}
        //private float readMemory_SunModeBrightnessValue = 0f;
        //private float readMemory_NightModeBrightnessValue = 0f;
        //private float readMemory_ValveModeValue = 0f;
        //public float ReadMemory_SunModeBrightnessValue = 0f;
        //public float ReadMemory_NightModeBrightnessValue = 0f;
        //public float ReadMemory_ValveModeValue = 0f;

        //public Boolean IsReadWriteMemoryOpOk = false;
        public void InitReadWriteMemoryValues()
        {
            if (IPEReadWriteMemoryValue != null)
                IPEReadWriteMemoryValue.Clear();



        }


        public int ReadMemory_SunModeBrightnessValue
        {
            get
            {
                if (IPEReadWriteMemoryValue == null)
                    return 0;
                else
                    return IPEReadWriteMemoryValue.SunModeBrightnessValue;
            }
        }

        public int ReadMemory_NightModeBrightnessValue
        {
            get
            {
                if (IPEReadWriteMemoryValue == null)
                    return 0;
                else
                    return IPEReadWriteMemoryValue.NightModeBrightnessValue;
            }
        }

        public int ReadMemory_ValveNormalOpenCloseMode
        {
            get
            {
                if (IPEReadWriteMemoryValue == null)
                    return 0;
                else
                    return IPEReadWriteMemoryValue.ValveNormalOpenCloseMode;
            }
        }
#endregion


#region Device Info Service
        public BleDeviceInfo BleDeviceInfo = new BleDeviceInfo();
        public String DeviceInfo_FirmwareRevistion
        {
            get
            {
                if (StateMachine.Instance.IsUseCommunicationMode)
                    return BleDeviceInfo.FirmwareRevision;
                else
                    //return mActivity.Resources.GetString(Resource.String.Simulation);
                    return "Demo B25";
            }
        }

        public String DeviceInfo_SoftwareRevision
        {
            get
            {
                if (StateMachine.Instance.IsUseCommunicationMode)
                    return BleDeviceInfo.SoftwareRevision;
                else
                    //return mActivity.Resources.GetString(Resource.String.Simulation);
                    return "Demo 1.17,1.02";

            }
        }

        public String DeviceInfo_Name
        {
            get
            {
                if (StateMachine.Instance.IsUseCommunicationMode)
                    return BleDeviceInfo.DeviceName;
                else
                    //return mActivity.Resources.GetString(Resource.String.Simulation);
                    return "V.Dialogue_BLE_Demo";
            }
        }

        public String DeviceInfo_SerialNumber
        {
            get
            {
                if(StateMachine.Instance.IsUseCommunicationMode)
                   return BleDeviceInfo.SerialNumber;
               else
                   return "XXXXXXXXXXX";
            }
        }
#endregion

#region AUTO MODE1 SETTING VALUE
        public int ReadMemory_ValveAutoMode1RpmValue
        {
            get
            {
                if (IPEReadWriteMemoryValue == null)
                    return 0;
                else
                    return IPEReadWriteMemoryValue.AutoMode1RPM;
            }
        }

        public float ReadMemory_ValveAutoMode1TpsValue
        {
            get
            {
                if (IPEReadWriteMemoryValue == null)
                    return 0;
                else
                    return IPEReadWriteMemoryValue.AutoMode1TPS;
            }
        }

        public int ReadMemory_ValveAutoMode1DelayTime
        {
            get
            {
                if (IPEReadWriteMemoryValue == null)
                    return 0;
                else
                    return IPEReadWriteMemoryValue.AutoMode1DelayTime;
            }
        }
#endregion

#region AUTO MODE2 SETTING VALUE
        public int ReadMemory_ValveAutoMode2RpmValue
        {
            get
            {
                if (IPEReadWriteMemoryValue == null)
                    return 0;
                else
                    return IPEReadWriteMemoryValue.AutoMode2RPM;
            }
        }

        public float ReadMemory_ValveAutoMode2TpsValue
        {
            get
            {
                if (IPEReadWriteMemoryValue == null)
                    return 0;
                else
                    return IPEReadWriteMemoryValue.AutoMode2TPS;
            }
        }

        public int ReadMemory_ValveAutoMode2DelayTime
        {
            get
            {
                if (IPEReadWriteMemoryValue == null)
                    return 0;
                else
                    return IPEReadWriteMemoryValue.AutoMode2DelayTime;
            }
        }
#endregion

#region AUTO MODE3 SETTING VALUE
        public int ReadMemory_ValveAutoMode3RpmValue
        {
            get
            {
                if (IPEReadWriteMemoryValue == null)
                    return 0;
                else
                    return IPEReadWriteMemoryValue.AutoMode3RPM;
            }
        }

        public float ReadMemory_ValveAutoMode3TpsValue
        {
            get
            {
                if (IPEReadWriteMemoryValue == null)
                    return 0;
                else
                    return IPEReadWriteMemoryValue.AutoMode3TPS;
            }
        }

        public int ReadMemory_ValveAutoMode3DelayTime
        {
            get
            {
                if (IPEReadWriteMemoryValue == null)
                    return 0;
                else
                    return IPEReadWriteMemoryValue.AutoMode3DelayTime;
            }
        }
#endregion

#region Created for FACTORY PAGE

        /// <summary>
        /// ODO補償百分比 成員變數
        /// </summary>
        private int mODOCompensationPercent = -1;
        /// <summary>
        /// ODO補償百分比，單位:百分比
        /// ODO補償百分比加上保護，避免頻繁存取
        /// </summary>
        int ODOCompensationPercent
        {
            set
            {
                if (value != mODOCompensationPercent)
                    spf?.SetODOCompensationPercent(value);
            }
            get
            {
                if (mODOCompensationPercent == -1)
                    mODOCompensationPercent = (int)spf?.GetODOCompensationPercent();

                return mODOCompensationPercent;
            }
        }


        private int mFuelConsumptionCompensationPercent = -1;
        /// <summary>
        /// 油耗補償百分比，單位:百分比
        /// 油耗補償百分比加上保護，避免頻繁存取
        /// </summary>
        int FuelConsumptionCompensationPercent
        {
            set
            {
                if (value != mFuelConsumptionCompensationPercent)
                    spf?.SetFuelConsumptionCompensationPercent(value);
            }
            get
            {
                if (mFuelConsumptionCompensationPercent == -1)
                    mFuelConsumptionCompensationPercent = (int)spf?.GetFuelConsumptionCompensationPercent();
                return mFuelConsumptionCompensationPercent;
            }
        }


        /// <summary>
        /// Remind ODO設定值，單位:Km
        /// </summary>
        int RemindODOValue
        {
            set
            {
                spf?.SetRemindODOValue(value);
            }
            get
            {
                return (int)spf?.GetRemindODOValue();
            }
        }

        /// <summary>
        /// Remind Trip設定值，單位:Km
        /// </summary>
        int RemindTripValue
        {
            set
            {
                spf?.SetRemindTripValue(value);
            }
            get
            {
                return (int)spf?.GetRemindTripValue();
            }
        }

        /// <summary>
        /// 獲得機車ID
        /// </summary>
        public String VehicleIdValue
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                if (motorVinUnpacker == null)
                    return null;

                return motorVinUnpacker?.GetVehicleId();
            }
        }

        /// <summary>
        /// 保養週期設定值，單位:Km
        /// </summary>
        int MaintenancePeriodValue
        {
            set
            {
                spf?.SetMaintenancePeriodValue(value);
            }
            get
            {
                return (int)spf?.GetMaintenancePeriodValue();
            }
        }

        /// <summary>
        /// 保養歸零設定值，單位:Km
        /// </summary>
        int MaintenanceResetValue
        {
            set
            {
                spf?.SetMaintenanceResetValue(value);
            }
            get
            {
                return (int)spf?.GetMaintenanceResetValue();
            }
        }

        /// <summary>
        /// 是否使用Km單位旗標，預設值為true
        /// </summary>
        bool IsUseKmUnit
        {
            set
            {
                spf?.SetUnitKmFlag(value);
            }
            get
            {
                return (spf == null) ? true : spf.GetUnitKmFlag();
            }
        }

#endregion

#region DM ICON DATA
        DmIconIcmDBHelper dmIconIcmDBHelper = null;
        public DmIconIcmDBHelper DmIconDBHelper
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return dmIconIcmDBHelper;
            }
        }

        /// <summary>
        /// 獲取LV ICON OIL_TEMPERATURE ID列表
        /// </summary>
        public List<UInt32> LvIcon_OilTemperature_CommunicationIDs
        {
            get
            {
                return DmIconDBHelper?.GetLvIconIdList(LiveDataIconItemNames.OIL_TEMPERATURE);
            }
        }

        /// <summary>
        /// 獲取LV ICON SPEED ID列表
        /// </summary>
        public List<UInt32> LvIcon_Speed_CommunicationIDs
        {
            get
            {
                return DmIconDBHelper?.GetLvIconIdList(LiveDataIconItemNames.SPEED);
            }
        }

        /// <summary>
        /// 獲取LV ICON RPM ID列表
        /// </summary>
        public List<UInt32> LvIcon_Rpm_CommunicationIDs
        {
            get
            {
                return DmIconDBHelper?.GetLvIconIdList(LiveDataIconItemNames.RPM);
            }
        }

        /// <summary>
        /// 獲取LV ICON WATER_TEMPERATURE ID列表
        /// </summary>
        public List<UInt32> LvIcon_WaterTemperature_CommunicationIDs
        {
            get
            {
                return DmIconDBHelper?.GetLvIconIdList(LiveDataIconItemNames.WATER_TEMPERATURE);
            }
        }

        /// <summary>
        /// 獲取LV ICON BOOST ID列表
        /// </summary>
        public List<UInt32> LvIcon_Boost_CommunicationIDs
        {
            get
            {
                return DmIconDBHelper?.GetLvIconIdList(LiveDataIconItemNames.BOOST);
            }
        }

        /// <summary>
        /// 獲取LV ICON FUEL_PRESSURE ID列表
        /// </summary>
        public List<UInt32> LvIcon_FuelPressure_CommunicationIDs
        {
            get
            {
                return DmIconDBHelper?.GetLvIconIdList(LiveDataIconItemNames.FUEL_PRESSURE);
            }
        }


        /// <summary>
        /// 獲取LV ICON BATTERY ID列表
        /// </summary>
        public List<UInt32> LvIcon_Battery_CommunicationIDs
        {
            get
            {
                return DmIconDBHelper?.GetLvIconIdList(LiveDataIconItemNames.BATTERY);
            }
        }

        /// <summary>
        /// 獲取LV ICON EXHAUST_TEMPERATURE ID列表
        /// </summary>
        public List<UInt32> LvIcon_ExhaustTemperature_CommunicationIDs
        {
            get
            {
                return DmIconDBHelper?.GetLvIconIdList(LiveDataIconItemNames.EXHAUST_TEMPERATURE);
            }
        }

        /// <summary>
        /// 獲取LV ICON AFR ID列表
        /// </summary>
        public List<UInt32> LvIcon_AFR_CommunicationIDs
        {
            get
            {
                return DmIconDBHelper?.GetLvIconIdList(LiveDataIconItemNames.AFR);
            }
        }

        /// <summary>
        /// 獲取LV ICON OIL_PRESSURE ID列表
        /// </summary>
        public List<UInt32> LvIcon_OilPressure_CommunicationIDs
        {
            get
            {
                return DmIconDBHelper?.GetLvIconIdList(LiveDataIconItemNames.OIL_PRESSURE);
            }
        }

        /// <summary>
        /// 獲取LV ICON TROTTLE ID列表
        /// </summary>
        public List<UInt32> LvIcon_ThrottlePosition_CommunicationIDs
        {
            get
            {
                return DmIconDBHelper?.GetLvIconIdList(LiveDataIconItemNames.TROTTLE);
            }
        }

        /// <summary>
        /// 獲取LV ICON INTAKE_TEMP ID列表
        /// </summary>
        public List<UInt32> LvIcon_IntakeTemperature_CommunicationIDs
        {
            get
            {
                return DmIconDBHelper?.GetLvIconIdList(LiveDataIconItemNames.INTAKE_TEMP);
            }
        }
        #endregion

#region DEMO BIN FILE DATA
        public DemoBinUnpacker demoBinUnpacker = null;
        #endregion

#region LV CLOUD FUNCTIONS
        Dictionary<int, DmData> AllDmDatasInIcmDB
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get { return dmIcmDBHelper.DmValues; }
        }

        private Dictionary<int, String> allEcuNameDict = new Dictionary<int, string>();

        /// <summary>
        /// 儲存所有ECU LV NAME的一維陣列
        /// </summary>
        public Dictionary<int, String> AllEcuLvNamesDict
        {
            get
            {
                var EcuAllLvIDs = vdiUnpacker.AllDmIDValues;
                var DmDbDatas = dmIcmDBHelper.DmValues;
                if (EcuAllLvIDs == null || DmDbDatas == null)
                    return null;

                allEcuNameDict.Clear();
                foreach (var key in EcuAllLvIDs.Values)
                {
                    if (DmDbDatas.ContainsKey(key))
                        allEcuNameDict.Add(key, DmDbDatas[key].Name);
                }
                return allEcuNameDict;
            }
        }


        /// <summary>
        /// 儲存所有ECU LV NAME的字典檔，並經由EcuId過濾表過濾
        /// </summary>
        public Dictionary<int, String> AllEcuLvNamesDictByEcuIdList(List<uint> filterEcuIdList)
        {
            var EcuAllLvIDs = vdiUnpacker.AllDmIDValues;
            var DmDbDatas = AllEcuDmDatasFilterByEcuIdList(filterEcuIdList);

            if (EcuAllLvIDs == null || DmDbDatas == null)
                return null;

            allEcuNameDict.Clear();
            foreach (var key in EcuAllLvIDs.Values)
            {
                if (DmDbDatas.ContainsKey(key))
                    allEcuNameDict.Add(key, DmDbDatas[key].Name);
            }
            return allEcuNameDict;
        }

        /// <summary>
        /// 用以儲存Lv Item Names反找Lv Id的列表
        /// </summary>
        private List<int> lvIds = new List<int>();

        /// <summary>
        /// 使用Lv Item名稱來反找Lv Id列表
        /// </summary>
        /// <param name="items">動態值名稱列表</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public List<int> AllEcuLvIdsByNames(List<string> items)
        {
            lvIds.Clear();

            if (items == null || items.Count <= 0)
                return lvIds;

            var lvIdNameDict = AllEcuLvNamesDict;
            if (allEcuNameDict.Count <= 0)
                return lvIds;

            foreach (var item in items)
            {
                var myKey = lvIdNameDict.FirstOrDefault(x => x.Value == item).Key;
                if (myKey != 0)
                    lvIds.Add(myKey);
            }


            return lvIds;
        }

        /// <summary>
        /// 使用Lv Item名稱來反找Lv Id列表，可經由EcuId過濾表過濾
        /// </summary>
        /// <param name="items">動態值名稱列表</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public List<int> AllEcuLvIdsByNames(List<string> items, List<uint> currentEcuIdList)
        {
            lvIds.Clear();

            if (items == null ||
                items.Count <= 0 ||
                currentEcuIdList == null)
                return lvIds;

            var lvIdNameDict = AllEcuLvNamesDictByEcuIdList(currentEcuIdList);
            if (allEcuNameDict.Count <= 0)
                return lvIds;

            foreach (var item in items)
            {
                var myKey = lvIdNameDict.FirstOrDefault(x => x.Value == item).Key;
                if (myKey != 0)
                    lvIds.Add(myKey);
            }


            return lvIds;
        }


        #endregion

        #region 遠距診斷用變數
#if IsEnableAMQRemoteDiagnosticFunction
        public readonly bool IsEnableAMQRemoteDiagnostic = true;
#else
        public readonly bool IsEnableAMQRemoteDiagnostic = false;
#endif

        private byte[] remoteDiagRecvValues = null;
        public byte[] RemoteDiagRecvValues
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return remoteDiagRecvValues;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                remoteDiagRecvValues = value;
            }
        }

        /// <summary>
        /// 目前是否執行遠距診斷中
        /// </summary>
        public bool IsRemoteDiagnosticStart
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                //改成ios的controller
                //if (StateMachine.UIModel.remoteDiagFragment == null)
                    return false;
                //else
                    //return StateMachine.UIModel.remoteDiagFragment.IsRemoteDiagnosticStart;
            }
        }
        #endregion

        private List<DtcData> currentDtcCodesList = null;
        /// <summary>
        /// 用以儲存DTC Codes的結果
        /// </summary>
        public List<DtcData> CurrentDtcCodesList
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                currentDtcCodesList = value;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return currentDtcCodesList;
            }
        }

        #region ECU_INFO_DATA
        private EcuInfoIcmDBHelper ecuInfoIcmDBHelper = null;
        //private Dictionary<uint, EcuInfoData> mEcuInfoList = new Dictionary<uint, EcuInfoData>();
        private Dictionary<uint, EcuInfoData> mEcuInfoList = new Dictionary<uint, EcuInfoData>()
        {
            { 0x0F44, new EcuInfoData() },
            { 0x0F45, new EcuInfoData() },
            { 0x9C40, new EcuInfoData() },
            { 0x9C41, new EcuInfoData() },
            { 0x9C42, new EcuInfoData() },
            { 0x9C43, new EcuInfoData() },
            { 0x0F3F, new EcuInfoData() }
        };

        /// <summary>
        /// 用以儲存EcuInfo結構之字典檔
        /// Key為該EcuInfo之ID
        /// </summary>
        public Dictionary<uint, EcuInfoData> EcuInfoDict
        {
            get
            {
                try
                {
                    var v = ecuInfoIcmDBHelper.AllEcuInfos;
                    return v;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 儲存通訊時獲得之各ECU ID之列表
        /// </summary>
        public List<uint> EcuIdList = new List<uint>();

        /// <summary>
        /// Ecu群組列表
        /// </summary>
        public List<EcuGroupData> EcuGroupList
        {
            get
            {
                return ecuInfoIcmDBHelper.EcuGroupsList;
            }
        }

        /// <summary>
        /// Ecu群組字典檔
        /// 利用ECU ID (UInt32)來當作Primary Key映射分群各系統Group
        /// </summary>
        public Dictionary<UInt32, EcuGroupData> EcuGroupsDict
        {
            get
            {
                return ecuInfoIcmDBHelper.EcuGroupsDict;
            }
        }

        public EcuInfoIcmDBHelper EcuInfoIcmDBHelper
        {
            get
            {
                return ecuInfoIcmDBHelper;
            }
        }
        #endregion

        #region DTC CMD INFO
        public Dictionary<UInt32, List<DtcCmdInfo>> DtcCmdsDict
        {
            get
            {
                return dtcUnpacker?.DtcCmdsDict;
            }
        }

        /// <summary>
        /// 解碼DTC J1939通訊接口
        /// </summary>
        /// <param name="dtcRawValues">DTC J1939通訊回傳之Raw Data，包含前兩個BYTE要去除的數據(0x0F1 + STATUS)</param>
        /// <param name="ecuId">當前選擇之系統EcuID</param>
        /// <param name="dtcDecodeType">DTC解碼類型數字碼</param>
        /// <returns></returns>
        public bool UnpackJ1939DtcValues(byte[] dtcRawValues, uint ecuId, int dtcDecodeType)
        {
            if (dtcUnpacker == null)
                return false;

            return dtcUnpacker.UnpackJ1939(dtcRawValues, ecuId, dtcDecodeType);
        }

        /// <summary>
        /// 獲取當前Dtc通訊指令ID欄位
        /// </summary>
        private uint mCurrentDtcId = 0;
        /// <summary>
        /// 獲取當前Dtc通訊指令欄位
        /// </summary>
        private byte[] mCurrentDtcCmd = { 0xF1, 0x00, 0x00 };

        /// <summary>
        /// 當前Dtc通訊指令ID屬性
        /// </summary>
        public uint CurrentDtcCmdId
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                mCurrentDtcId = value;
                mCurrentDtcCmd[1] = (byte)(mCurrentDtcId >> 8);
                mCurrentDtcCmd[2] = (byte)(mCurrentDtcId);
            }
            get
            {
                return mCurrentDtcId;
            }
        }

        /// <summary>
        /// 獲取當前Dtc通訊指令
        /// </summary>
        public byte[] CurrentDtcCmd
        {
            get
            {
                return mCurrentDtcCmd;
            }
        }
        #endregion

     
    }
}
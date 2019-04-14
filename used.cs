using Android.App;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using System;
using System.Threading;
using IcmLib.Database.IcmDBHelper;
using IcmLib.Data.Unpacker;
using IcmLib.Data;
using IcmComLib.Communication.BLE.AndroidBLE;
using System.Runtime.CompilerServices;
using MonoAndroid_VDialogueBLE_Truck.SRC;
using icm.com.tw.Util;
using Android.Content.PM;
using System.Text.RegularExpressions;
using Android.Views;
using Android.Content;
using Android.Support.V4.App;
using Android;
using IcmCommLib.IcmComLib.Utils;
using Java.Util;
using MonoAndroid_VDialogueBLE_Truck.SRC.UI.utils;
using IcmComLib.Utils.AndroidOS;
using Android.Support.V4.Content;
using Android.Gms.Location;
using Android.Gms.Common;
using Android.Util;
using MonoAndroid_VDialogueBLE_Truck.SRC.UI.callbacks;
using System.Threading.Tasks;
using Android.Locations;
using tw.com.kc.amq;
using Icm.AndroidOS.UI.Fragments;

namespace MonoAndroid_VDialogueBLE_Truck
{
    [Activity(Label = "@string/app_name", Icon = "@drawable/app_icon", Theme = "@style/AppTheme", 
        MainLauncher = true,
        ScreenOrientation = ScreenOrientation.ReverseLandscape, 
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    //[Activity(Label = "@string/app_name", Icon = "@drawable/ipe_logo", Theme = "@style/Theme.AppCompat.Light.NoActionBar", MainLauncher = true)]
    public class MainActivity : Activity
    {
        public static MainActivity Instance = null;
        public static bool DEBUG = false;
        public DataModel DataModel = null;
        private TruckPages CurrentPage = TruckPages.None;

        private static bool killAllUIThread = false;
        private List<Thread> UIThreadList = new List<Thread>();

        private static bool killAllCommThread = false;
        private List<Thread> CommThreadList = new List<Thread>();

        public BLEWrapper BLECentral = null;

        public LogCat2File logcat2File;


        //COMMUNICATION MODE
        private CommMode mode = CommMode.NONE;
        public CommMode Mode
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                mode = value;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return mode;
            }
        }

        //BLE MAC ADDR
        //private String TargetBLEMacAddr = "BB:A0:50:06:19:DE";
        //印度出差藍芽MAC
        //private String TargetBLEMacAddr = "BB:A0:50:C4:A9:0B";
        //private String TargetBLEMacAddr = "BB:A0:50:C4:A8:61";

        //Boot Loader DEBUG用
        //private String TargetBLEMacAddr = "BB:A0:50:E9:22:EB";
        private String TargetBLEMacAddr = "BB:A0:50:10:59:64";
        private String TargetBLEDeviceName = "V.Dialogue_BLE_";
        //private String TargetBLEDeviceName = "V.Dialogue_BLE_000001";

        #region 添加外部修改BLE MAC 地址函數
        /// <summary>
        /// Android SD Card Path
        /// </summary>
        Java.IO.File SDCard = Android.OS.Environment.ExternalStorageDirectory;
        String BleMacAddrSettingFileName = "BleMac.txt";
        /// <summary>
        /// RegEx for Mac Address Format
        /// RegEx : ^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$
        /// Supported two format
        /// XX-XX-XX-XX-XX-XX
        /// XX:XX:XX:XX:XX:XX
        /// </summary>
        Regex macRegex = new Regex(@"^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$");
        private bool CheckMacAddrFormat(String macAddrStr)
        {
            bool isMacAddrFormat = false;
            isMacAddrFormat = macRegex.IsMatch(macAddrStr);
            return isMacAddrFormat;
        }

        private String GetSDCardMacAddrFromSettingFile()
        {
            String macAddressStr = null;
            List<String> macAddrList = new List<string>();
            var file = new Java.IO.File(SDCard, BleMacAddrSettingFileName);
            if (!file.Exists())
                return null;
            try
            {
                Java.IO.BufferedReader br = new Java.IO.BufferedReader(new Java.IO.FileReader(file));
                String line;

                while ((line = br.ReadLine()) != null)
                {
                    macAddrList.Add(line);
                }
                br.Close();

                foreach (var macAddr in macAddrList)
                {
                    if (CheckMacAddrFormat(macAddr))
                    {
                        macAddressStr = macAddr;
                        if (macAddressStr.Contains("-"))
                            macAddressStr = macAddressStr.Replace("-", ":");
                        break;
                    }
                }
            }
            catch (Java.IO.IOException e)
            {
                //You'll need to add proper error handling here
                return null;
            }

            return macAddressStr;
        }
        #endregion


        #region 添加外部修改BLE NAME函數
        String BleDeviceNameSettingFileName = "BleName.txt";
        private String GetSDCardBleDeviceNameFromSettingFile()
        {
            String bleDeviceNameStr = null;
            List<String> bleDeviceNameList = new List<string>();
            var file = new Java.IO.File(SDCard, BleDeviceNameSettingFileName);
            if (!file.Exists())
                return null;
            try
            {
                Java.IO.BufferedReader br = new Java.IO.BufferedReader(new Java.IO.FileReader(file));
                String line;

                while ((line = br.ReadLine()) != null)
                {
                    bleDeviceNameList.Add(line);
                }
                br.Close();

                foreach (var deviceName in bleDeviceNameList)
                {
                    if (deviceName != null)
                    {
                        bleDeviceNameStr = deviceName;
                        break;
                    }
                }
            }
            catch (Java.IO.IOException e)
            {
                //You'll need to add proper error handling here
                return null;
            }

            return bleDeviceNameStr;
        }
        #endregion


        /// <summary>
        /// USER SHARED PREFERENCES
        /// </summary>
        IPESharedPreferencesExtractor userPrefs;



        //DATA MODLE
        //public BleVdiDataUnpacker VdiUnpacker = new BleVdiDataUnpacker();
        public DmIcmDBHelper DmIcmDBHelper = null;
        public VinIcmDBHelper VinIcmDBHelper = null;
        private byte[] cmdBuffer;
        //通訊共用CMD BUFFER
        public byte[] CmdBuffer
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                cmdBuffer = value;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return cmdBuffer;
            }
        }


        private CrashHandler crashHandler;

        public StateMachine StateMachine;

        #region AMQ Varialbes
        static private String userName = @"admin";
        static private String password = @"admin";
        static String target = @"tcp://210.65.88.10:61616";
        #endregion

        /// <summary>
        /// Lock Back Button
        /// </summary>
        public override void OnBackPressed()
        {
            ////base.OnBackPressed();
            //if (TruckUIModel.Instance.CurrentPage == TruckPages.LvCloud_Show)
            //{
            //    //當動態值數據顯示頁面，AMQ未作動，直接返回上一頁；AMQ作動時，彈出警告視窗
            //    if (!DataMonitorFragment.IsStartAqmCommunication)
            //    {
            //        StateMachine.SendMessage(StateMachineStatus.Communication_LV_STOP);
            //        StateMachine.TruckUIModel.SwitchFragment(StateMachine.TruckUIModel.lvCloudFragment, "lvCloudFragment");
            //    }
            //    else
            //        StateMachine.TruckUIModel.DMFragment.JudgeAmqEnabledAndReturnParentPage();
            //}
            //else
            if (TruckUIModel.Instance.CurrentPage == TruckPages.MenuFragment)
            {
                TruckUIModel.Instance.CurrentPage = TruckPages.MainviewFragment;
                TruckUIModel.Instance.SwitchFragment(TruckUIModel.Instance.mainviewFragment, "mainviewFragment");
            }
            else
                StateMachine.TruckUIModel.ShowExitAPPDialogDialog();
        }

        private BLEGPSChecker bleGpsChecker;
        private Locale appLang;
        private String appLangStr;
        LinearLayout lv_data_pad, rl_bg_base;
        public GridView gridView;
        public Button btnOK, btnNO;

        public LVDataDisplay display;
        public static Handler handler = new Handler();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);
            SetContentView(Resource.Layout.truck_main);
            rl_bg_base = (LinearLayout)FindViewById(Resource.Id.rl_bg_base);
            lv_data_pad = (LinearLayout)FindViewById(Resource.Id.lv_data_pad);
            Instance = this;

            try
            {
                appLang = Resources.Configuration.Locale;
                appLangStr = appLang.ToLanguageTag();
                System.Diagnostics.Debug.WriteLine("App Language : " + appLangStr);

                //ISO-639-2 無法區分簡繁體中文
                var multiLang = Locale.Default.ISO3Language;
                System.Diagnostics.Debug.WriteLine("Multi Language : " + multiLang);

                //ISO-3166-1
                var countryZipCode = appLang.Country;
                System.Diagnostics.Debug.WriteLine("CountryZipCode : " + countryZipCode);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }

            MainInit();
            //gridView = (GridView)FindViewById(Resource.Id.lv_data_display);

            //btnOK = (Button)FindViewById(Resource.Id.btnOK);

            //display = new LVDataDisplay(Instance);
            //// 記錄logcat
            //// logcat2File = new LogCat2File(BLEWrapper.CLASS_NAME);
            //// logcat2File.Go();


            ////AmqFunctionTest();
            DisplayMetricConverter.PrintDisplayInfo();
        }

        public void AmqFunctionTest()
        {
            new Thread(() => { 
                AMQManager amqManager = new AMQManager(userName, password, target);
                amqManager.EnumerateQueues();
            }).Start();
        }
        

        public void GoMainView(bool go)
        {
            if (go)
            {
                if(lv_data_pad != null)
                    lv_data_pad.Visibility = ViewStates.Invisible;
                if (rl_bg_base != null)
                    rl_bg_base.Visibility = ViewStates.Visible;        
            }
            else
            {
                if (lv_data_pad != null)
                    lv_data_pad.Visibility = ViewStates.Visible;
                if (rl_bg_base != null)
                    rl_bg_base.Visibility = ViewStates.Invisible;
            }
        }
        public void MainInit()
        {
            GoMainView(true);
            bleGpsChecker = new BLEGPSChecker(Instance, Resource.Drawable.Icon, appLangStr);
            if (bleGpsChecker.CheckVersion())
            {
                #region GPS CHECK
                if (!bleGpsChecker.IsGpsEnable())
                {
                    bleGpsChecker.ShowGpsAlertDialog();
                }
                #endregion
            }
            else
            {
                Main();
                IsMainFunctionStart = true;
            }
        }

        private bool IsMainFunctionStart = false;
        private void Main()
        {
            userPrefs = new IPESharedPreferencesExtractor(this);
            //help user to set Defalut Value
            userPrefs.SetDestOilValue(5000);
            userPrefs.SetRemindOilValue(500);

            crashHandler = CrashHandler.Instance;
            crashHandler.Init(ApplicationContext);

            var macAddrFromFile = GetSDCardMacAddrFromSettingFile();
            if (macAddrFromFile != null)
                TargetBLEMacAddr = macAddrFromFile;

            var bleDeviceNameFromFile = GetSDCardBleDeviceNameFromSettingFile();
            if (bleDeviceNameFromFile != null)
                TargetBLEDeviceName = bleDeviceNameFromFile;

            #region FUNCTION TEST
            ////DTC TEST
            //DTCTest();

            ////VIN TEST
            //VINTest();

            ////DM TEST
            //DMTest();
            #endregion


            CurrentPage = TruckPages.MainviewFragment;

            StateMachine = new StateMachine(this, TargetBLEMacAddr, TargetBLEDeviceName, CurrentPage);



            PermissionRequestProcess();



            ////if (StateMachine.BLEComModel.IsBtEnabled == false)
            ////    StateMachine.BLEComModel.EnableBT();
            //StateMachine.Start();
        }

        private const int PERMISSION_LOCATION_REQUEST_CODE = 1;
        private const int PERMISSION_READ_WRITE_EXTERNAL_STORAGE_REQUEST_CODE = 2;

        /// <summary>
        /// 請求權限程序
        /// </summary>
        private void PermissionRequestProcess()
        {
            if (AppAttributes.EnableReadWriteExternalStorage)
            {
                if (CheckReadWriteFilePermission(this.ApplicationContext))
                {
                    StateMachine.InitModels();
                    Console.WriteLine("Has the ReadWriteExternalStorage Permission");

                    if (CheckGPSPermission(this.ApplicationContext))
                    {
                        StartGooglePlayServicesOfLocation();
                        StateMachine.Start();
                        Console.WriteLine("Has the GPS Permission");
                    }
                    else
                    {
                        ShowGPSPermissionDialog();
                        Console.WriteLine("Not has the GPS Permission.");
                    }
                }
                else
                {
                    ShowReadWriteFilePermissionDialog();
                    Console.WriteLine("Not has the ReadWriteExternalStorage Permission.");
                }
            }



            //if (CheckGPSPermission(this.ApplicationContext))
            //{
            //    StateMachine.Start();
            //    Console.WriteLine("Has the GPS Permission");
            //}
            //else
            //{
            //    ShowGPSPermissionDialog();
            //    Console.WriteLine("Not has the GPS Permission.");
            //}
        }


        /// <summary>
        /// 檢查檔案讀寫權限
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool CheckReadWriteFilePermission(Context context)
        {
            return ActivityCompat.CheckSelfPermission(context, Manifest.Permission.WriteExternalStorage) == Permission.Granted
                && ActivityCompat.CheckSelfPermission(context, Manifest.Permission.ReadExternalStorage) == Permission.Granted;
        }


        /// <summary>
        /// 顯示獲取檔案讀寫權限對話框
        /// </summary>
        private void ShowReadWriteFilePermissionDialog()
        {
            if (!CheckReadWriteFilePermission(this))
            {
                ActivityCompat.RequestPermissions(
                    this,
                    new String[] { Manifest.Permission.WriteExternalStorage, Manifest.Permission.ReadExternalStorage },
                    PERMISSION_READ_WRITE_EXTERNAL_STORAGE_REQUEST_CODE);
                //2);
            }
        }

        /// <summary>
        /// 檢查GPS權限
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool CheckGPSPermission(Context context)
        {
            return ActivityCompat.CheckSelfPermission(context, Manifest.Permission.AccessFineLocation) == Permission.Granted
                    && ActivityCompat.CheckSelfPermission(context, Manifest.Permission.AccessCoarseLocation) == Permission.Granted;
        }

        /// <summary>
        /// 顯示獲取GPS權限對話框
        /// </summary>
        private void ShowGPSPermissionDialog()
        {
            if (!CheckGPSPermission(this))
            {
                ActivityCompat.RequestPermissions(
                    this,
                    new String[] { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation },
                    //PERMISSION_LOCATION_REQUEST_CODE);
                    1);
            }
        }

        /// <summary>
        /// 按完獲取權限後的CALLBACK
        /// </summary>
        /// <param name="requestCode"></param>
        /// <param name="permissions"></param>
        /// <param name="grantResults"></param>
        public override async void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            //base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            switch (requestCode)
            {
                case PERMISSION_LOCATION_REQUEST_CODE:
                    {
                        // If request is cancelled, the result arrays are empty.
                        if (grantResults.Length > 0
                        && grantResults[0] == Permission.Granted)
                        {
                            Console.WriteLine("GPS Permission Granted.");
                            StartGooglePlayServicesOfLocation();
                            StateMachine.Start();
                        }
                        else
                        {
                            // permission denied, boo! Disable the
                            // functionality that depends on this permission.
                            Console.WriteLine("GPS Permission Denined.");
                        }
                        return;
                    }

                case PERMISSION_READ_WRITE_EXTERNAL_STORAGE_REQUEST_CODE:
                    {
                        // If request is cancelled, the result arrays are empty.
                        if (grantResults.Length > 0
                        && grantResults[0] == Permission.Granted)
                        {
                            Console.WriteLine("ReadWriteExternalStorage Permission Granted.");
                            StateMachine.InitModels();

                            if (!CheckGPSPermission(this.ApplicationContext))
                            {
                                ShowGPSPermissionDialog();
                                Console.WriteLine("Not has the GPS Permission.");
                            }
                        }
                        else
                        {
                            // permission denied, boo! Disable the
                            // functionality that depends on this permission.
                            Console.WriteLine("ReadWriteExternalStorage Permission Denined.");
                            StateMachine.CloseApp();
                        }
                        return;
                    }
                    break;
                    // other 'case' lines to check for other
                    // permissions this app might request
            }
        }




        protected async override void OnResume()
        {
            base.OnResume();
            if (bleGpsChecker == null) return;
            if (bleGpsChecker.CheckVersion())
            {
                if (bleGpsChecker.IsGpsEnable())
                {
                    if (!IsMainFunctionStart)
                    {
                        Main();
                        IsMainFunctionStart = true;
                    }
                }
                else
                {
                    bleGpsChecker.GpsStatusCheck();
                }
            }
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            switch (requestCode)
            {
                //case 9973:
                //    //StateMachine.UIModel.homeFragment.OnActivityResult(requestCode, resultCode, data);
                //    StateMachine.TruckUIModel.testFuncFragment.OnActivityResult(requestCode, resultCode, data);
                //    break;
                //case 5487:
                //    StateMachine.TruckUIModel.testFuncFragment.OnActivityResult(requestCode, resultCode, data);
                //    break;
            }

        }
        private void DTCTest()
        {
            //String fileName = "IcmDB/916.dat";
            String fileName = "IcmDB/DTC.dat";
            var asset = Assets.Open(fileName);
            DtcDataUnpacker dtcUnpacker = new DtcDataUnpacker(asset, @"file:///android.assets/" + fileName);
            dtcUnpacker.Unpack(new byte[] { 0x00, 0x01, 0x03, 0x00, 0x00, 0x05 });
            var values = dtcUnpacker.Values;
            foreach (var value in values)
            {
                Console.WriteLine(value.DtcName);
                Console.WriteLine(value.DtcHexNumber);
                Console.WriteLine(value.DtcCodeForDisplay);
            }
        }

        private void VINTest()
        {
            var fileName = "IcmDB/VIN.dat";
            var asset = Assets.Open(fileName);
            VINDecoder vinDecoder = new VINDecoder("KMHGC41E79U028838");
            VinIcmDBHelper vinIcmDBHelper = new VinIcmDBHelper(asset, @"file:///android.assets/" + fileName);
            Console.WriteLine(vinIcmDBHelper.GetArea(vinDecoder.VIN.WMI));
            Console.WriteLine(vinIcmDBHelper.GetManufacture(vinDecoder.VIN.WMI));
            vinDecoder.Setting("W0LPD8EDXG8034955");
            Console.WriteLine(vinIcmDBHelper.GetArea(vinDecoder.VIN.WMI));
            Console.WriteLine(vinIcmDBHelper.GetManufacture(vinDecoder.VIN.WMI));

        }

        private void DMTest()
        {
            var fileName = "IcmDB/DM.dat";
            var asset = Assets.Open(fileName);
            DmIcmDBHelper dmIcmDBHelper = new DmIcmDBHelper(asset, @"file:///android.assets/" + fileName);

            BleVdiDataUnpacker vdiUnpacker = new BleVdiDataUnpacker(dmIcmDBHelper);
            vdiUnpacker.Add(new byte[] { 0x00, 0x01, 0x00, 0x02, 0x00, 0x73, 0x00, 0x87 });
            vdiUnpacker.Setting(new byte[] { 0x00, 0x01, 0x00, 0x02, 0x00, 0x73, 0x00, 0x87 });
            vdiUnpacker.Unpack(new byte[] { 0x00, 0x0F, 0x42, 0xA0, 0x00, 0x00, 0x45, 0x3B, 0x80, 0x00, 0x40, 0x80, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00 });
            var vdiValues = vdiUnpacker.Values;
            if (vdiValues != null)
            {
                float value = 0f;
                foreach (var vdiID in vdiValues.Keys)
                {
                    if (dmIcmDBHelper.Exist(vdiID))
                    {
                        var vdiInfo = dmIcmDBHelper.DmValues[vdiID];
                        value = vdiValues[vdiID];

                        if (vdiInfo.PatternDisplay.Count == 0)
                        {
                            Console.WriteLine("{0} : {1} {2}", vdiInfo.Name, value, vdiInfo.Unit);
                        }
                        else
                        {
                            if (vdiInfo.PatternDisplay.ContainsKey((int)value))
                                Console.WriteLine("{0} : {1} {2}", vdiInfo.Name, vdiInfo.PatternDisplay[(int)value], vdiInfo.Unit);
                            else
                                Console.WriteLine("{0} : {1} {2}", vdiInfo.Name, "Uknown", vdiInfo.Unit);
                        }

                    }
                }
            }
        }



        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (StateMachine.BLEComModel != null)
                StateMachine.BLEComModel.Disconnect();
        }

        private void KawasakiKsdFileParseTest()
        {
            String iniStr = @";
                                ;Template Setting Data File
                                ;
                                [INFORMATION]
                                Version=1
                                UstCode=0893
                                ECUS=1

                                [ECU1]
                                Enable=1
                                Maker=2
                                Model=3
                                Year=1
                                SendCanID=0699
                                RecvCanID=0698
                                MonCanID=0688
                                Cylinder=1
                                Injector=1
                                Ignition=1
                                RPM=6
                                TH=6
                                AACC=0
                                Serial=8045100031
                                UnitType=1
                                InstallUnitInfo=1
                                SupportStatus=1
                                SupportFunction=0B0000
                                MonCategory=2
                                SupportMonIds=7F44C40100
                                ";
            //var fileParser = new IniParser.FileIniDataParser();
            //var textParser = new IniParser.Parser.IniDataParser();
            //var parsedData = textParser.Parse(iniStr);
            //var data = parsedData["ECU1"]["SupportMonIds"];
            //fileParser.WriteFile(SDCard + @"/KSD_INI.txt", parsedData);
        }

        #region GOOGLE GPS FUNCTIONS
        private const long ONE_MINUTE = 60 * 1000;
        private const long ONE_SECOND = 1000;
        private int count = 1;
        private bool isRequestingLocationUpdates = false;
        private bool isGooglePlayServicesInstalled = false;
        private FusedLocationProviderClient fusedLocationProviderClient;
        private LocationRequest locationRequest;
        private LocationCallback locationCallback;
        private Location fusedLocation;
        public Location FusedLocation { get => fusedLocation; }

        private async void StartGooglePlayServicesOfLocation()
        {
            //runtime permission check
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == Permission.Granted)
            {
                //StartRequestingLocationUpdates();
                isRequestingLocationUpdates = true;
            }
            else
            {

            }
            isGooglePlayServicesInstalled = IsGooglePlayServicesInstalled();

            if (isGooglePlayServicesInstalled)
            {
                locationRequest = new LocationRequest()
                    .SetPriority(LocationRequest.PriorityHighAccuracy)
                    .SetInterval(ONE_SECOND)
                    .SetFastestInterval(ONE_SECOND);

                locationCallback = new FusedLocationProviderCallback(this);

                fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(this);
                if (fusedLocationProviderClient != null)
                {
                    await StartRequestingLocationUpdates();
                    await GetLastLocationFromDevice();
                }

            }
            else
            {

            }
        }

        bool IsGooglePlayServicesInstalled()
        {
            var queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (queryResult == ConnectionResult.Success)
            {
                Log.Info("MainActivity", "Google Play Services is installed on this device.");
                return true;
            }

            if (GoogleApiAvailability.Instance.IsUserResolvableError(queryResult))
            {
                // Check if there is a way the user can resolve the issue
                var errorString = GoogleApiAvailability.Instance.GetErrorString(queryResult);
                Log.Error("MainActivity", "There is a problem with Google Play Services on this device: {0} - {1}",
                          queryResult, errorString);

                // Alternately, display the error to the user.
            }
            return false;
        }

        async Task StartRequestingLocationUpdates()
        {
            await fusedLocationProviderClient.RequestLocationUpdatesAsync(locationRequest, locationCallback);
        }

        async Task GetLastLocationFromDevice()
        {
            fusedLocation = await fusedLocationProviderClient.GetLastLocationAsync();

            if (fusedLocation == null)
            {
                
            }
            else
            {
               
            }
        }
        #endregion
    }
}



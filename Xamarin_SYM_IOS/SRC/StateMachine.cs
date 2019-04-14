//#define ENABLE_AUTO_ADD_FUEL_OXYGEN_ID_FUNCTION
//#define ENABLE_READ_HARDWARE_FUNCTION  //讀取硬體狀態 條件式編譯旗標，取消define註解，啟動該功能
//#define ENABLE_TEST_PRESENT_FUNCTION  //TEST_PRESENT 條件式編譯旗標，取消define註解，啟動該功能
//#define DEBUG
#define 上傳資料篩選
#define DTC頁面每秒送一次開啟
//#define 開啟DTC_ReadinessLayout
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using IcmLib.Data;
using System;
using System.Diagnostics;
using IcmLib;
using System.Collections;
using iPhoneBLE.SRC;
using iPhoneBLE;
using UIKit;
using CoreFoundation;
using IcmComLib.Communication.BLE;

using Xamarin_SYM_IOS;
using CommonLib.IcmLib.Enum.Communication;
using IcmComLib.Communication.BLE.iOSBLE;
using CoreBluetooth;
using Foundation;
using Xamarin_SYM_IOS.ViewControllers;
using IcmComLib.Utils.iOS;
using IcmComLib_iOS.IcmComLib.Utils;
using static IcmComLib.Communication.BLE.iOSBLE.CBBLEWrapper;
using CoreLocation;
using CommonLib.IcmLib.Data;
using IcmLib.Database;
using System.Globalization;

/// <summary>
/// IPE用狀態機:
/// 程式主流程控制
/// </summary>
public class StateMachine : MonitorModel
{
    private static bool isActivted = false;
    private Dictionary<uint, List<int>> dictLiveDataItems = new Dictionary<uint, List<int>>()
    {
        //2 ,7 9 是ecu id
        [2] = new List<int>() { 205, 206, 207, 208, 209, 210, 211, 212, 214, 216, 217, 224, 225, 245, 246, 248 },
        [7] = new List<int>() { 706, 710, 729, 732, 736, 741, 742, 743, 744, 745, 746, 747, 749, 750, 751 },
        [9] = new List<int>() { 903, 905, 906, 907, 908, 909, 910, 911, 912, 913, 916, 922 },
    };

    private static DataModel mDataModel = null;
    private Dictionary<string, UIViewController> mUIViewControllerTable = new Dictionary<string, UIViewController>();
    private LinkedHashMap<int, float> writeMemoryKeyValues = new LinkedHashMap<int, float>();
    /// <summary>
    /// 是否開啟通訊模式
    /// </summary>
    private bool UseCommunicationMode = true;
    public bool IsUseCommunicationMode { get => UseCommunicationMode; }
    /// <summary>
    /// 是否開啟手勢換頁模式
    /// </summary>
    private bool UseGestureMode = false;
    /// <summary>
    /// 是否開啟手勢模擬模式
    /// </summary>
    private bool UseGestureSimulationMode = false;
    private long GestureSimlationChangePageTimeInMilleSecs = 6000;
    public delegate void CustomFunc(int CurrentBlockIndex, int AllLogBlocks, byte[] CurrentReadLogRawValue, byte[] TotalLogRawValues, bool IsLogOperationOk);
    public CustomFunc CircularBarCallBack = null;
    public SharedPreferencesExtractor SharedPreferencesExtractor;
    private string storedBleDeviceName = "";
    private readonly Object MutexLocker = new Object();
    public SYMSharedPreferencesExtractor symSharedPreferencesExtractor;
    private bool IsAlreadyShowBleDeviceListDialog = false;
    public bool isNeedRemindToOpenBLE = true;
    private bool IsUsingChoiceMultipleDeviceMode = true;

    public string StoredBleDeviceName
    {
        get => storedBleDeviceName;

    }
    public static bool IsActivted
    {
        get
        {
            return isActivted;
        }
        set
        {
            isActivted = value;
        }
    }

    /// <summary>
    /// 資料模型
    /// </summary>
    public static DataModel DataModel
    {
        get
        {
            return mDataModel;
        }
    }

    /// <summary>
    /// 目標藍芽裝置MAC地址
    /// </summary>
    private string mTargetDeviceName = "";

    /// <summary>
    /// 通訊模型
    /// </summary>
    public static CommunicationModel BLEComModel = null;

    /// <summary>
    /// 當前頁面
    /// </summary>
    private Page mPage = Page.None;

    /// <summary>
    /// 狀態機當前狀態
    /// </summary>
    private StateMachineStatus Status = StateMachineStatus.None;

    /// <summary>
    /// 狀態機實例
    /// </summary>
    public static StateMachine Instance = null;

    /// <summary>
    /// 狀態機訊息佇列
    /// </summary>
    private List<StateMachineStatus> msgQueue = new List<StateMachineStatus>();

    private List<DtcData> dtcList = new List<DtcData>();

    /// <summary>
    /// UI模型
    /// </summary>
    public static UIModel UIModel = null;

    //private ContainerViewController containVC;
    private UIView mUIView = null;

    private UIViewController mUIViewController;


    /// <summary>
    ///目前裝置之本地化配置 - 多語用
    /// </summary>
    private NSLocale mAppLocale;

    /// <summary>
    /// 目前裝置國家代碼 - 多語用
    /// </summary>
    private String mCountryCode;

    /// <summary>
    /// 目前裝置國家名稱 - 多語用
    /// </summary>
    private String mCountryName;

    public DiscoverBleDeviceCallback BLEDiscoveredDeviceAction = null;

    private CLLocationManager locationManager;
    public CLLocationManager LocationManager { get => locationManager; }
    private bool isAllowLocationRead = false;

    /// <summary>
    /// 狀態機建構子
    /// </summary>
    /// <param name="activity">Activity 實例</param>
    /// <param name="targetDeviceName">目標藍芽裝置名稱</param>
    /// <param name="page">當前頁面</param>
	private static bool isAutoConnect = false;
    public static bool IsAutoConnect
    {
        get
        {
            return isAutoConnect;
        }
        set
        {
            isAutoConnect = value;
        }
    }

    private static int stateCode = 1;

    void HandleAction()
    {
        
    }




    public StateMachine(UIViewController containerViewController, string targetDeviceName, Page page, bool isAutoConnect)
    {
        Instance = this;
        symSharedPreferencesExtractor = new SYMSharedPreferencesExtractor();
        storedBleDeviceName = symSharedPreferencesExtractor.GetBleDeviceName();

       

        //設定獲取當前語言
        mAppLocale = NSLocale.AutoUpdatingCurrentLocale;
        mCountryCode = mAppLocale.CountryCode;
        mCountryName = mAppLocale.GetCountryCodeDisplayName(mCountryCode).ToUpper();


        IsAutoConnect = isAutoConnect;
        this.mTargetDeviceName = targetDeviceName;
        mUIViewController = containerViewController;
        mPage = page;
        InitModels();


    }

    /// <summary>
    /// 先確認Info.plist有無gps的key，
    /// </summary>
    private void RequestLocationPermission()
    {
        locationManager = new CLLocationManager();

        EventHandler<CLAuthorizationChangedEventArgs> authCallback = null;

        authCallback = (sender, e) =>
        {
            Debug.WriteLine("CLAuthorizationStatus : " + e.Status.ToString());
            if (e.Status == CLAuthorizationStatus.NotDetermined)
                return;
            if (e.Status == CLAuthorizationStatus.Denied)
                isAllowLocationRead = false;
            if (e.Status == CLAuthorizationStatus.AuthorizedWhenInUse)
                isAllowLocationRead = true;
            
            locationManager.AuthorizationChanged -= authCallback;
            //do stuff here 
            symSharedPreferencesExtractor.SetGPSAllow(isAllowLocationRead);
        };

        //locationManager.AuthorizationChanged += authCallback;
        locationManager.Delegate = ContainerViewController.Instance;

        var info = NSBundle.MainBundle.InfoDictionary;
        if (info.ContainsKey(new NSString("NSLocationWhenInUseUsageDescription")))
            locationManager.RequestWhenInUseAuthorization();
        //else if (info.ContainsKey(new NSString("NSLocationAlwaysUsageDescription")))
            //locationManager.RequestAlwaysAuthorization();
        else
            throw new UnauthorizedAccessException("On iOS 8.0 and higher you must set either NSLocationWhenInUseUsageDescription or NSLocationAlwaysUsageDescription in your Info.plist file to enable Authorization Requests for Location updates!");
    }

    public void InitModels()
    {
        RequestLocationPermission();

        mDataModel = new DataModel(mCountryCode);


        //RUN COM THREAD
        BLEComModel = new CommunicationModel(mTargetDeviceName);
        BLEComModel.BindDataModel(mDataModel);
        BLEComModel.BLEGetLogRawValueAction = BLEGetLogRawValueMethod;
        //BLEComModel.BLEPowerStateAction = BLEPowerState;
        BLEComModel.BLEGetLvDataAction = BLEGetLvData;


        //RUN UI THREAD
        UIModel = new UIModel(mUIViewController, mDataModel, mPage);
        UIModel.IsUIMode = true;
        UIModel.Start();
     
        //if (UseCommunicationMode)
        //	UIModel.ShowProgressDialog();


        //SIMULATION GESTURE
        if (UseGestureMode && UseGestureSimulationMode)
        {
            GestureSimulator gestureSimulator = new GestureSimulator(GestureSimlationChangePageTimeInMilleSecs);
            gestureSimulator.Start();
        }

        if (AppAttribute.BLE_SCAN_VIEW == AppAttribute.RunningMode.NewScanView)
        {
            BLEComModel.BLEConnetedAction = BLEConnectedMethod;
            BLEComModel.BLEDisconnetedAction = BLEDisconnectedMethod;
            BLEComModel.BLEScanFailedAction = BLEScanFailedMethod;
            BLEComModel.BLEDiscoveredDeviceAction = TriggerBLEDiscoveredDeviceAction;
            BLEComModel.BLEGetLvDataAction = BLEGetLvData;
        }

    }

    private void TriggerBLEDiscoveredDeviceAction(string deviceName)
    {
        if (BLEDiscoveredDeviceAction != null)
            BLEDiscoveredDeviceAction(deviceName);
    }

    /// <summary>
    /// BLE連線成功回調方法
    /// </summary>
    private void BLEConnectedMethod()
    {
        var deviceName = StateMachine.BLEComModel.mCBBLECentral.BLEDeviceName;

        if (deviceName != null && !deviceName.Equals(""))
            symSharedPreferencesExtractor.SetBleDeviceName(deviceName);


        //這邊不用寫，下Device_init完會依步驟進行到Communication_Init
        //SendMessage(StateMachineStatus.Communication_Init);

        //UIModel.SetProgerssDialogMessage("Connected.\ r\nWaiting Communication ...");

        //ios 按下重連按鈕Alert系統會自動close
        //UIModel.CloseReconnectAlertDialog();

        Debug.WriteLine("Run BLEConnectedMethod()");
    }

    /// <summary>
    /// BLE連線失敗回調方法
    /// </summary>
    private void BLEDisconnectedMethod()
    {
        if (Instance != null)
        {
            //IsAppPause = true;
            StateMachine.Instance.RemoveAllMessage();
            Debug.WriteLine("Run BLEDisconnectedMethod()");
            IsAlreadyShowBleDeviceListDialog = false;
            //UIModel.SetProgerssDialogMessage("Disconnected, Reconnecting ...");
            //UIModel.ShowReconnectAlertDialog();
            HomeViewController.Instance.ClearBLENamesInList();
            DataModel.ClearLvDatas();

            lock (BLEComModel.ConnectLocker)
            {
                BLEComModel.IsInConnect = false;
            }
            UIModel.ShowRescanAlertDialog();
        }
    }

    /// <summary>
    /// BLE掃描失敗後的回調方法
    /// </summary>
    private void BLEScanFailedMethod()
    {
        if (Instance != null)
        {
            Debug.WriteLine("Run BLEScanFailedMethod()");
            if (BLEComModel != null)
                BLEComModel.StopScan();
            Thread.Sleep(1000);

            UIModel.CloseProgressDialog();
            //UIModel.CloseReconnectAlertDialog();
            if (!BLEComModel.IsConnected)
                UIModel.ShowRescanAlertDialog();
        }
    }

    private bool isDownLoadNow = false;
    public bool IsDownLoadNow
    {
        get
        {
            return isDownLoadNow;
        }
        set
        {
            isDownLoadNow = value;
        }
    }

    //private void BLEPowerState(CBCentralManagerState state) { 

    //}

    /// <summary>
    /// Get Log Raw Value Method
    /// </summary>
    /// <param name="CurrentBlockIndex">Current block index</param>
    /// <param name="AllLogBlocks">All of log blocks count</param>
    /// <param name="CurrentReadLogRawValue">Current received log raw value</param>
    /// <param name="TotalLogRawValues">Total received log raw value</param>
    /// <param name="IsLogOperationOk">Log opertation is completed or not</param>
    private void BLEGetLogRawValueMethod(int CurrentBlockIndex, int AllLogBlocks, byte[] CurrentReadLogRawValue, byte[] TotalLogRawValues, bool IsLogOperationOk)
    {
        //if (!isDownLoadNow && !IsLogOperationOk)
        //isDownLoadNow = true;
        //else
        //	isDownLoadNow = false;
        if (CircularBarCallBack != null)
            CircularBarCallBack(CurrentBlockIndex, AllLogBlocks, CurrentReadLogRawValue, TotalLogRawValues, IsLogOperationOk);

    }

    private void BLEGetLvData(ConcurrentDictionary<Int32, Single> values)
    {
        switch (StateMachine.UIModel.CurrentPage)
        {
            case Page.Upload:
                //沒加上此行，使用上會變慢，須注意
                if (ContainerViewController.Instance.uploadViewController == null) return;
                //if (!TestFuncFragment.IsStartAqmCommunication)
                    //return;

                new Thread(() =>
                {
                    if (values == null || values.Count == 0)
                        return;
#if 上傳資料篩選
                    UploadValues = null;
                    UploadValues = CheckLvDatasChange(values);
#else
            UploadValues = null;
            UploadValues = values;
#endif
                    UploadViewController.Instance.SendLVData(UploadValues);
                }).Start();
                break;

            case Page.LvCloud_Show:
                //沒加上此行，使用上會變慢，須注意
                if (!DataMonitorViewController.IsStartAqmCommunication)
                    return;

                new Thread(() => {
                    ContainerViewController.Instance.dataMonitorViewController.SendLVDataByAMQ();
                }).Start();
                break;
        }

//        if (UploadViewController.Instance != null)
//        {

           
        //}

    }

    ConcurrentDictionary<Int32, Single> UploadValues = null;
    ConcurrentDictionary<Int32, Single> TempValues = null;
    /// <summary>
    /// 過濾LV Data資料
    /// LV DATA有變化的才傳
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    private ConcurrentDictionary<Int32, Single> CheckLvDatasChange(ConcurrentDictionary<Int32, Single> values)
    {
        lock (this)
        {
            if (values == null || values.Count == 0)
                return null;

            if (TempValues == null)
                TempValues = new ConcurrentDictionary<int, float>(values);

            int currentValueKeyCounts = values.Keys.Count;
            int tempValueKeyCounts = TempValues.Keys.Count;
            if (currentValueKeyCounts == tempValueKeyCounts)
            {
                var result = new ConcurrentDictionary<int, float>(values.Where(entry => TempValues[entry.Key] != entry.Value)
                     .ToDictionary(entry => entry.Key, entry => entry.Value));
                if (result.Count > 1)
                {
                    Console.WriteLine("result.Count > 1");
                    TempValues = null;
                    TempValues = new ConcurrentDictionary<int, float>(values);
                }
                return result;
            }
            else
            {
                TempValues = null;
                TempValues = new ConcurrentDictionary<int, float>(values);
                return values;
            }
        }
    }


    public CBBLEWrapper.CustomFunc BLEConnectionTimeoutAction
    {
        set
        {
            if (BLEComModel == null || value == null)
                return;
            else
            {
                BLEComModel.BLEConnectionTimeoutAction = value;
            }

        }
    }

    public IcmComLib.Communication.BLE.iOSBLE.CBBLEWrapper.CustomFunc BLEConnetedAction
    {
        set
        {
            if (BLEComModel == null || value == null)
                return;
            else
            {
                BLEComModel.BLEConnetedAction = value;
            }
        }
    }
    public IcmComLib.Communication.BLE.iOSBLE.CBBLEWrapper.CustomFunc BLEDisconnetedAction
    {
        set
        {
            if (BLEComModel == null || value == null)
                return;
            else
            {
                BLEComModel.BLEDisconnetedAction = value;
            }
        }
    }

    public IcmComLib.Communication.BLE.iOSBLE.CBBLEWrapper.CustomFunc BLEScanFailedAction
    {
        set
        {
            if (BLEComModel == null || value == null)
                return;
            else
            {
                BLEComModel.BLEScanFailedAction = value;
            }
        }
    }



    /// <summary>
    /// 傳送訊息 , 將訊息佇列在訊息堆裡輪詢處理
    /// </summary>
    /// <param name="status">狀態機狀態</param>
    public void SendMessage(StateMachineStatus status)
    {
        msgQueue.Add(status);
    }

    /// <summary>
    /// 移除特定訊息
    /// </summary>
    /// <param name="status">要移除的狀態機狀態</param>
    public void RemoveMessage(StateMachineStatus status)
    {
        msgQueue.Remove(status);
    }

    /// <summary>
    /// 移除所有特定訊息
    /// </summary>
    /// <param name="status">要移除的狀態機狀態</param>
    public void RemoveAllSpecificMessage(StateMachineStatus status)
    {
        //msgQueue.RemoveAll(it => msgQueue.Contains(status));
        msgQueue.RemoveAll(it => it == status);
    }

    /// <summary>
    /// 移除所有訊息
    /// </summary>
    public void RemoveAllMessage()
    {
        msgQueue.Clear();
    }

    /// <summary>
    /// 訊息壓棧
    /// </summary>
    /// <param name="status">要加入佇列的狀態機狀態</param>
    private void Enqueue(StateMachineStatus status)
    {
        msgQueue.Add(status);
    }

    /// <summary>
    /// 狀態機出棧
    /// </summary>
    /// <returns>返回壓棧中最先壓入的狀態機訊息，如果訊息棧中為空，返回NONE</returns>
    private StateMachineStatus Dequeue()
    {
        if (msgQueue.Count <= 0)
            return StateMachineStatus.None;
        else
        {
            ////LIFO, Last In First Out
            //var status = msgQueue[msgQueue.Count - 1];
            //msgQueue.RemoveAt(msgQueue.Count - 1);

            //FIFO, First-In-First-Out
            var status = msgQueue[0];
            msgQueue.RemoveAt(0);
            return status;
        }
    }

    /// <summary>
    /// 連接計時器
    /// </summary>
    private Stopwatch connectTimer = new Stopwatch();


    private Stopwatch gestureTimer = new Stopwatch();
    private readonly long gestureTimeout = 300L;
    private Stopwatch testPresentTimer = new Stopwatch();
    private readonly long testPresentTimeout = 3000L;
    private bool IsHardwareOK = true;
    private bool IsReadOk = false;

    /// <summary>
    /// 連接逾時值
    /// </summary>
    private readonly long connectTimeout = 5000L;

    /// <summary>
    /// 通訊是否初始化，當運行完LV DATA成功後，設為TRUE
    /// </summary>
    public bool IsCommunicationInited = false;

    /// <summary>
    /// 狀態機主程序
    ///     Device_Init >> Device_Connect >> Communication_Init >> 
    ///     Communication_VIN >> Communication_LV_ID >> Communication_DTC
    /// </summary>
    /// 

    private bool IsDeviceInit = false;
    private void StateMachineProcess()
    {
        switch (Dequeue())
        {
            //case StateMachineStatus.BleScan:ƒ
            //    //Console.WriteLine("StateMachineStatus.BleScan");
            //    if (!BLEComModel.IsScanning && !UIModel.IsShowScanViewController)
            //    {
            //        UIModel.ShowScanViewController();
            //    }



            //    Status = StateMachineStatus.BleScan;
            //    Enqueue(Status);
            //    break;

            //case StateMachineStatus.BlePowerOff:
            //    //Console.WriteLine("StateMachineStatus.BlePowerOff");

            //    if (BLEComModel.IsPowerOff &&
            //        !UIModel.Instance.IsShowBlePowerOffAlert)
            //    {
            //        UIModel.Instance.IsShowBlePowerOffAlert = true;
            //        BLEComModel.IsPowerOff = false;
            //        UIModel.Instance.ShowBlePowerOffAlert(ContainerViewController.Instance.CurrentController);
            //    }

            //    Status = StateMachineStatus.BlePowerOff;

            //    Enqueue(Status);
            //    //Thread.Sleep(10);
            //    break;
            //case StateMachineStatus.BlePowerOn:
                ////Console.WriteLine("StateMachineStatus.BlePowerOnnnnnnnnnnnnnnnnn");

                //Status = StateMachineStatus.Device_Init;
                //Enqueue(Status);
                //break;
            case StateMachineStatus.None:
                //Console.WriteLine("StateMachineStatus.None");
                if (!BLEComModel.IsConnected)
                {
                    if (UIModel.Instance.isActIndAnimating)
                    {
                        UIModel.Instance.CloseProgressDialog();
                    }
                }
                else
                {
                    UIModel.Instance.CloseProgressDialog();
                    UIModel.Instance.CloseAlertDialog();
                    //UIModel.CloseRescanAlertDialog();
                }

                if (StateMachine.UIModel.Instance.IsrescanAlertDialogShowing 
                    && HomeViewController.Instance.BleDeviceListDialog.IsShowing)
                {
                    UIModel.CloseRescanAlertDialog();
                }
                //Console.WriteLine("RSSI : {0}",BLEComModel.ReadRssi());
                Thread.Sleep(10);
                break;

            case StateMachineStatus.Device_Init:
                if (BLEComModel.IsScanning || BLEComModel.IsInited)
                {
                    Status = StateMachineStatus.Communication_Init;
                    Enqueue(Status);
                    break;
                }

                int recvLen = 0;
                IsCommunicationInited = false;
                Status = StateMachineStatus.Device_Init;
                recvLen = BLEComModel.ComProcess(CommMode.BLE_INIT);
                if (recvLen > 0)
                {
                    IsDeviceInit = true;
                    Status = StateMachineStatus.Device_Connect;
                }
                //Status = StateMachineStatus.Device_Connect;

                Enqueue(Status);
                break;

            case StateMachineStatus.Device_Connect:
                if (!connectTimer.IsRunning)
                    connectTimer.Start();
                else
                    connectTimer.Reset();

                if (connectTimer.ElapsedMilliseconds > connectTimeout)
                {
                    RemoveAllMessage();
                    connectTimer.Reset();
                    connectTimer.Stop();
                    UIModel.Instance.CloseProgressDialog();
                    //UIModel.Instance.ShowAlertDialog();
                    Thread.Sleep(300);
                    break;
                }

                Status = StateMachineStatus.Device_Connect;
                recvLen = BLEComModel.ComProcess(CommMode.CONNECT);
                if (recvLen > 0)
                {
                    if (BLEComModel.IsConnected)
                    {
                        Status = StateMachineStatus.Communication_Init;
                        connectTimer.Reset();
                        connectTimer.Stop();
                    }
                }
                else
                {
                    Thread.Sleep(500);
                    Status = StateMachineStatus.Device_Connect;
                    Debug.WriteLine("  yo oy yo recvLen -1 in Device_connect");
                }
                Enqueue(Status);
                break;

            case StateMachineStatus.Communication_Init:
                //邏輯與Android同
                Status = StateMachineStatus.Communication_Init;

                if (!IsReadOk)
                {
                    Enqueue(Status);
                    break;
                }

                recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_INIT);

                if (BLEComModel.SettingResultValue == true)
                {
                    IsCommunicationInited = true;

                    Debug.WriteLine("Init Cmd OK.");
                  
                    if (BLEComModel.InfoValue != null)
                    {

                        Status = StateMachineStatus.Communication_LV_ID;
                    }
                    else
                    {

                                                Status = StateMachineStatus.Communication_VehicleInfos_With_J1939DataLink_Protocol;
                    }
                }
                else
                {

                    Status = StateMachineStatus.Communication_Init;
                }
                //舊的程式碼
                //                if (connectTimer.IsRunning)
                //                {
                //                    connectTimer.Reset();
                //                    connectTimer.Stop();
                //                }

                //                Status = StateMachineStatus.Communication_Init;
                //                recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_INIT);
                //                if (recvLen > 0)
                //                {
                //                    if (BLEComModel.SettingResultValue == true)
                //                    {
                //                        IsCommunicationInited = true;

                //                        DispatchQueue.MainQueue.DispatchAsync(() =>
                //                        {
                //#if DEBUG
                //                            //Toast.MakeText("BLE Init OK.").Show();
                //#endif
                //                        });


                //                        Debug.WriteLine("Init Cmd OK.");
                //                        RemoveAllMessage();
                //                        if (BLEComModel.InfoValue != null)
                //                        {
                //                            Status = StateMachineStatus.Communication_LV_ID;
                //                        }
                //                        else
                //                        {
                //                            Status = StateMachineStatus.Communication_ECU_ID;
                //                        }

                //                    }
                //                    else
                //                    {
                //                        Status = StateMachineStatus.Communication_Init;
                //                    }
                //                }
                //                else
                //                    Status = StateMachineStatus.Device_Connect;

                Enqueue(Status);
                break;

            case StateMachineStatus.Communication_ResetMemory:
                Status = StateMachineStatus.Communication_ResetMemory;
                Console.WriteLine("MessageQueue : {0}", msgQueue.Count);
                recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_RESET_MEMORY);
                if (recvLen > 0)
                {
                    Console.WriteLine("Reset Memory Cmd OK.");
#if DEBUG
                    DispatchQueue.MainQueue.DispatchAsync(() =>
                    {
                        //Toast.MakeText("Reset Memory Cmd OK.").Show();
                    });
#endif
                    if (BLEComModel.InfoValue != null)
                    {
                        string vin = System.Text.Encoding.Default.GetString(BLEComModel.InfoValue);
                        //VIN 獲得後不再從新送指令
                        //如果已經拿到VIN，直接進入LV通訊if

                        if (vin.Length == 17 && IsNatural_Number(vin))
                        {
                            if (UIModel.CurrentPage == Page.LiveData || UIModel.CurrentPage == Page.Shift
                                                  || UIModel.CurrentPage == Page.LiveData_2_Frame
                                                  || UIModel.CurrentPage == Page.LiveData_4_Frame
                                                  || UIModel.CurrentPage == Page.LiveData_6_Frame)

                                Status = StateMachineStatus.Communication_LV;
                            else
                                //Status = StateMachineStatus.Communication_VIN;
                                Status = StateMachineStatus.None;
                        }
                        else
                            Status = StateMachineStatus.Communication_VIN;
                    }
                    else
                    {
                        Status = StateMachineStatus.Communication_VIN;
                    }

                    //if (UIModel.CurrentPage == Page.LiveData || UIModel.CurrentPage == Page.Shift
                    //                     || UIModel.CurrentPage == Page.LiveData_2_Frame
                    //                     || UIModel.CurrentPage == Page.LiveData_4_Frame
                    //                     || UIModel.CurrentPage == Page.LiveData_6_Frame)

                    //    Status = StateMachineStatus.Communication_LV;
                    //else
                    //Status = StateMachineStatus.Communication_VIN;
                }
                else
                {
                    Status = StateMachineStatus.Communication_Init;
                }
                Enqueue(Status);
                break;

            case StateMachineStatus.Communication_ECU_ID:
                Status = StateMachineStatus.Communication_ECU_ID;
                BLEComModel.SettingCmd(VdiCommand.GetEcuIdCmd);
                recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_INFO);

                if (recvLen > 0)
                {
                    var ecuId = BLEComModel.InfoValue;
                    Status = StateMachineStatus.Communication_VIN;
                }
                else if (recvLen <= 0)
                    Status = StateMachineStatus.Communication_Init;
                Enqueue(Status);
                break;

            case StateMachineStatus.Communication_VIN:
                Status = StateMachineStatus.Communication_VIN;
                BLEComModel.SettingCmd(VdiCommand.GetVINCmd);
                recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_INFO);



                if (recvLen >= 3)
                {
                    if (mDataModel.VinIcmUnpacker.Unpack(BLEComModel.InfoValue, DataModel.EcuID))
                        Debug.WriteLine("Unpack VIN OK");
                    else
                        Debug.WriteLine("Unpack VIN Failed");
                    var VehicleImage = DataModel.VinIcmUnpacker.MotorVinData.ImgName;
                    //UIModel?.homeFragment?.SetBitmapByDrawableID(VehicleImage);
                    ContainerViewController.Instance.homeViewController.SetUIImageByResouceName(VehicleImage);
                    //UIModel?.homeFragment?.SetBitmapByDrawableID(Resource.Drawable.M3_Ecu);

                    //LV IDs獲得後不再重新通訊
                    //第一次去拿Live Data IDs，後來直接進入Live Data通訊


                    if (StateMachine.DataModel.AllEcuLvIds.ToList().Count <= 0)
                        Status = StateMachineStatus.Communication_LV_ID;
                    else
                        Status = StateMachineStatus.Communication_LV;
                }
                else if (recvLen <= 0)
                    Status = StateMachineStatus.Communication_Init;
                Enqueue(Status);

                break;

            case StateMachineStatus.Communication_FuelConsumption:
                Status = StateMachineStatus.Communication_FuelConsumption;
                BLEComModel.SettingCmd(VdiCommand.GetFuelConsumptionCmd);
                recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_INFO);
                if (recvLen > 0)
                {
                    mDataModel.ConvertInfoValue2FuelConsumption();
                    Status = StateMachineStatus.None;
                }
                else if (recvLen <= 0)
                    Status = StateMachineStatus.Communication_Init;
                Enqueue(Status);
                break;

            case StateMachineStatus.Communication_LV_ID:
                Status = StateMachineStatus.Communication_LV_ID;
                recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_LV_ID);
                if (recvLen > 0)
                {
#if DEBUG
                    DispatchQueue.MainQueue.DispatchAsync(() =>
                    {
                    //Toast.MakeText("All Ecu Lv ID Count : " + mDataModel.AllEcuLvIds.Count).Show();
                    });
#endif
                    Status = StateMachineStatus.Communication_DTC;
                    //Status = StateMachineStatus.Communication_ACT;

                    //ACCORDING PAGE TO CONTINUE OPERATION
                    AccordingPageContinueOperation();

                    //CLOSE PROGRESS DIALOG
                    UIModel.Instance.CloseProgressDialog();
                    IsCommunicationInited = true;
                }
                else if (recvLen == -1)
                    Status = StateMachineStatus.Communication_Init;
                Enqueue(Status);
                break;

            case StateMachineStatus.Communication_Gesture:
                if (BLEComModel.ReadGestureValue())
                {
#if DEBUG
                    DispatchQueue.MainQueue.DispatchAsync(() =>
                    {
                        //Toast.MakeText("ReadGesture OK.").Show();
                    });
#endif
                }
                break;

            case StateMachineStatus.Device_Disconnect:
                BLEComModel.ComProcess(CommMode.DISCONNECT);
                break;


            #region COMMUNIVATION LV
            //livedata id在這裡做
            case StateMachineStatus.Communication_LV:
                if (!BLEComModel.IsConnected)
                {
                    RemoveAllMessage();
                    Status = StateMachineStatus.Communication_LV;
                    Enqueue(Status);
                    break;
                }
                Status = StateMachineStatus.Communication_LV;
                if (StateMachine.DataModel == null)
                {
                    Enqueue(Status);
                    return;
                }

                //var IDTable = MainActivity.Instance.VdiUnpacker.AllIDValues;
                var IDTable = StateMachine.DataModel.AllEcuLvIds;
                if (IDTable == null)
                {
                    Enqueue(Status);
                    return;
                }

                List<int> liveDataIds = new List<int>();



                //判斷頁面
                //LIVE DATA PAGE:
                //如果LV DATA ID沒有VSS，加入 VSS
                //SHIFT PATA:
                //如果LV DATA ID沒有RPM，加入 RPM
                switch (StateMachine.UIModel.CurrentPage)
                {
                    case Page.LiveData:

                        //@@@@@@
                        int[] checkedValues = null;
                        try
                        {
                            checkedValues = UIModel.Instance.vcManger.liveDataViewController.selectedPositions;
                        }
                        catch (Exception ex)
                        {

                        }
                        //獲得選擇的項目
                        //var liveDataPositions = LiveDataFragment.LiveDataPositions;
                        //var checkedValues = liveDataPositions;
                        //轉換項目Index為LV DATA ID

                        int lvId = 0;
                        if (checkedValues != null)
                            foreach (var checkedPos in checkedValues)
                            {
                                //if (IDTable.Count > checkedPos)

                                lvId = IDTable[checkedPos];

                                if (!liveDataIds.Contains(lvId))
                                    liveDataIds.Add(lvId);
                                //如果沒有240就改拿3906
                                if (lvId == 240)
                                {
                                    if (!liveDataIds.Contains(lvId)) liveDataIds.Add(3906);
                                }
                            }

                        if (LiveDataGaugesViewController.Instance != null)
                        {
                            if (!liveDataIds.Contains((int)LiveDataGaugesViewController.Instance.DM_ID))
                                liveDataIds.Add((int)LiveDataGaugesViewController.Instance.DM_ID);
                        }
                        else
                        {
                            //RPM
                            if (!liveDataIds.Contains((int)LiveDataItemsSN.RPM))
                                liveDataIds.Add((int)LiveDataItemsSN.RPM);
                        }

                        try
                        {
                            foreach (uint item in dictLiveDataItems[DataModel.EcuID])
                            {
                                //如果IDTable沒有支援就不Add
                                if (!liveDataIds.Contains((int)item) && IDTable.Contains((int)item))
                                    liveDataIds.Add((int)item);
                            }
                        }
                        catch (Exception e)
                        {

                        }





#if ENABLE_AUTO_ADD_FUEL_OXYGEN_ID_FUNCTION
                        //Fuel system 1 status
                        if (IDTable.Contains((int)LiveDataItemsSN.Fuel_System_1_Status))
						{
							if (!liveDataIds.Contains((int)LiveDataItemsSN.Fuel_System_1_Status))
								liveDataIds.Add((int)LiveDataItemsSN.Fuel_System_1_Status);
						}

						//Fuel system 2 status
						if (IDTable.Contains((int)LiveDataItemsSN.Fuel_System_2_Status))
						{
							if (!liveDataIds.Contains((int)LiveDataItemsSN.Fuel_System_2_Status))
								liveDataIds.Add((int)LiveDataItemsSN.Fuel_System_2_Status);
						}

						//CHECK IF B1-S1 LAMBDA EXIST , SEND LAMBDA, ELSE SEND VOLTAGE
						if (IDTable.Contains((int)LiveDataItemsSN.Equivalence_Ratio_lambda_B1_S1))
						{
							//Equivalence Ratio (lambda) (B1-S1)
							if (!liveDataIds.Contains((int)LiveDataItemsSN.Equivalence_Ratio_lambda_B1_S1))
								liveDataIds.Add((int)LiveDataItemsSN.Equivalence_Ratio_lambda_B1_S1);
						}
						else
						{
							//Oxygen Sensor Output Voltage(B2-S1)
							if (!liveDataIds.Contains((int)LiveDataItemsSN.Oxygen_Sensor_Output_Voltage_B1_S1))
								liveDataIds.Add((int)LiveDataItemsSN.Oxygen_Sensor_Output_Voltage_B1_S1);
						}

						//CHECK IF B2-S1 LAMBDA EXIST , SEND LAMBDA, ELSE SEND VOLTAGE
						if (IDTable.Contains((int)LiveDataItemsSN.Equivalence_Ratio_lambda_B2_S1))
						{
							//Equivalence Ratio (lambda) (B1-S1)
							if (!liveDataIds.Contains((int)LiveDataItemsSN.Equivalence_Ratio_lambda_B2_S1))
								liveDataIds.Add((int)LiveDataItemsSN.Equivalence_Ratio_lambda_B2_S1);
						}
						else
						{
							//Oxygen Sensor Output Voltage(B2-S1)
							if (!liveDataIds.Contains((int)LiveDataItemsSN.Oxygen_Sensor_Output_Voltage_B2_S1))
								liveDataIds.Add((int)LiveDataItemsSN.Oxygen_Sensor_Output_Voltage_B2_S1);
						}
#endif
                        //                  //Fuel system 1 status
                        //                  if (!liveDataIds.Contains((int)LiveDataItemsSN.Fuel_System_1_Status))
                        //                      liveDataIds.Add((int)LiveDataItemsSN.Fuel_System_1_Status);
                        //                  //Fuel system 2 status
                        //                  if (!liveDataIds.Contains((int)LiveDataItemsSN.Fuel_System_2_Status))
                        //                      liveDataIds.Add((int)LiveDataItemsSN.Fuel_System_2_Status);
                        //                  ////Short Term Fuel Trim - Bank 1
                        //                  //if (!liveDataIds.Contains((int)LiveDataItemsSN.Short_Term_Fuel_Trim_Bank_1))
                        //                  //    liveDataIds.Add((int)LiveDataItemsSN.Short_Term_Fuel_Trim_Bank_1);
                        //                  ////Short Term Fuel Trim - Bank 2 
                        //                  //if (!liveDataIds.Contains((int)LiveDataItemsSN.Short_Term_Fuel_Trim_Bank_2))
                        //                  //    liveDataIds.Add((int)LiveDataItemsSN.Short_Term_Fuel_Trim_Bank_2);
                        ////Oxygen Sensor Output Voltage(B2-S1)
                        //                  if (!liveDataIds.Contains((int)LiveDataItemsSN.Oxygen_Sensor_Output_Voltage_B1_S1))
                        //	liveDataIds.Add((int)LiveDataItemsSN.Oxygen_Sensor_Output_Voltage_B1_S1);
                        ////Oxygen Sensor Output Voltage(B2-S1)
                        //if (!liveDataIds.Contains((int)LiveDataItemsSN.Oxygen_Sensor_Output_Voltage_B1_S2))
                        //	liveDataIds.Add((int)LiveDataItemsSN.Oxygen_Sensor_Output_Voltage_B1_S2);
                        break;

                    case Page.Shift:
                        //RPM
                        if (!liveDataIds.Contains((int)LiveDataItemsSN.RPM))
                            liveDataIds.Add((int)LiveDataItemsSN.RPM);
                        break;

                    case Page.DTC:
#if 開啟DTC_ReadinessLayout
                        var readinessIds = DataModel.AllReadinessIDValues;
                        foreach (var readinessId in readinessIds.Values)
                        {
                            if (!liveDataIds.Contains(readinessId))
                                liveDataIds.Add(readinessId);
                        }
                        break;
#endif
                    case Page.Valve:
                        if (!liveDataIds.Contains((int)LiveDataItemsSN.ValveControlStatus))
                            liveDataIds.Add((int)LiveDataItemsSN.ValveControlStatus);
                        break;
                    //             case Page.LiveDataIconMenu:
                    //                 foreach(var id in ContainerViewController.Instance.LvIDsForIconPage){
                    //if (!liveDataIds.Contains(id))
                    //liveDataIds.Add(id);
                    //}
                    //break;
                    case Page.LiveData_2_Frame:
                        foreach (var id in ContainerViewController.Instance.LiveDataID_For_IconPage)
                        {
                            if (!liveDataIds.Contains(id))
                                liveDataIds.Add(id);
                        }
                        break;
                    case Page.LiveData_4_Frame:
                        foreach (var id in ContainerViewController.Instance.LiveDataID_For_IconPage)
                        {
                            if (!liveDataIds.Contains(id))
                                liveDataIds.Add(id);
                        }
                        break;
                    case Page.LiveData_6_Frame:
                        foreach (var id in ContainerViewController.Instance.LiveDataID_For_IconPage)
                        {
                            if (!liveDataIds.Contains(id))
                                liveDataIds.Add(id);
                        }
                        break;

                    case Page.Maintenance:
                        liveDataIds.Clear();
                        if (!liveDataIds.Contains((int)LiveDataItemsSN.ODO))
                            liveDataIds.Add((int)LiveDataItemsSN.ODO);
                        if (!liveDataIds.Contains((int)LiveDataItemsSN.FuelConsumption))
                            liveDataIds.Add((int)LiveDataItemsSN.FuelConsumption);
                        if (!liveDataIds.Contains((int)LiveDataItemsSN.InstantaneousFuelConsumption))
                            liveDataIds.Add((int)LiveDataItemsSN.InstantaneousFuelConsumption);
                        if (!liveDataIds.Contains((int)LiveDataItemsSN.Trip))
                            liveDataIds.Add((int)LiveDataItemsSN.Trip);
                        break;
                    case Page.LvCloud:
                    case Page.LvCloud_Show:
                        liveDataIds.Clear();
                        if (ContainerViewController.Instance.dataMonitorViewController != null)
                        {
                            liveDataIds.AddRange(ContainerViewController.Instance.lvDataCloudViewController.CurrentSendLvItemsForLvCloud);
                        }
                        break;
                }


                var splitCmdsList = ListExtensions.ChunkBy(liveDataIds, 10);

                foreach (var splitCmds in splitCmdsList)
                {
                    using (MemoryStream ms = new MemoryStream(splitCmds.Count))
                    {
                        ByteBufferWriter writer = new ByteBufferWriter(ms, ByteOrder.BIG_ENDIAN);
                        //foreach (var checkBoxItemSN in checkedValues)
                        //{
                        //    writer.Write((short)IDTable[checkBoxItemSN]);
                        //}
                        foreach (var liveDataId in splitCmds)
                        {
                            writer.Write((short)liveDataId);
                        }
                        BLEComModel.SettingCmd((byte[])ms.ToArray().Clone());
                    }

                    recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_LV);
                    if (recvLen > 0)
                    {
                        //DataModel.VdiUnpacker.Unpack((byte[])BLEComModel.LvDataValue.Clone());
                        Status = StateMachineStatus.None;
                    }
                    else if (recvLen == -1)
                        Status = StateMachineStatus.Communication_Init;

                    Enqueue(Status);
                }
                break;

#endregion

            case StateMachineStatus.Communication_LV_STOP:
                Status = StateMachineStatus.Communication_LV_STOP;
                if (BLEComModel.ComProcess(CommMode.COMMUNICATION_LV_STOP) > 0)
                {
                    Status = StateMachineStatus.None;
#if DEBUG
                    DispatchQueue.MainQueue.DispatchAsync(() =>
                    {
                        //Toast.MakeText(@"LV Stop Cmd OK.").Show();
                    });
#endif
                }
                Enqueue(Status);
                break;


            case StateMachineStatus.Communication_Log:

                Status = StateMachineStatus.Communication_Log;
                if (BLEComModel.ComProcess(CommMode.COMMUNICATION_LOG) > 0)
                {
                    Status = StateMachineStatus.None;
                }
                //Enqueue(Status);
                break;

            case StateMachineStatus.Communication_DTC:
                lock (MutexLocker)
                {
                    //如果不在DTC & HOME頁面，離開DTC程序
                    if (!(UIModel.Instance.CurrentPage == Page.DTC ||
                        UIModel.Instance.CurrentPage == Page.Home))
                        break;

                    if (BLEComModel.ComProcess(CommMode.COMMUNICATION_DTC) > 0)
                    {
                        if (UIModel.CurrentPage == Page.DTC)
                        {
#if DTC頁面每秒送一次開啟
                            var popTime = new DispatchTime(DispatchTime.Now, (long)1.0 * 1000000000);
                            DispatchQueue.MainQueue.DispatchAfter(popTime, () =>
                            {
                                Status = StateMachineStatus.Communication_DTC;
                                Enqueue(Status);
                            });
#endif
                        }
                        else
                        {
                            Enqueue(Status = StateMachineStatus.None);
                        }
                    }
                    else
                    {
                        Enqueue(StateMachineStatus.Communication_DTC);
                    }
                }
                break;

            case StateMachineStatus.Communication_ACT:
                Status = StateMachineStatus.Communication_ACT;
                //StateMachine.DataModel.ValveMode = ValveMode.ON_MODE;
                if (BLEComModel.ComProcess(CommMode.COMMUNICATION_ACT_VALVE) > 0)
                {
                    Status = StateMachineStatus.None;
#if DEBUG
                    DispatchQueue.MainQueue.DispatchAsync(() => {
                        //Toast.MakeText("ACT OK. Mode : " + StateMachine.DataModel.ValveMode.ToString()).Show();
                    });
#endif
                }
				//無論成功與否都要enqueue
				Enqueue(Status);
                break;

            case StateMachineStatus.Setting_0_100_ON:
                Status = StateMachineStatus.Setting_0_100_ON;
                BLEComModel.SettingCmd(VdiCommand.Setting0_100_ON);
                if (BLEComModel.ComProcess(CommMode.COMMUNICATION_SETTING) > 0 && BLEComModel.SettingResultValue == true)
                {
                    Status = StateMachineStatus.Communication_SpeedTimeCalc;
                    Console.WriteLine("Setting_Start0_100 Cmd OK.");
                }
                Enqueue(Status);
                break;

            case StateMachineStatus.Setting_0_100_OFF:
                Status = StateMachineStatus.Setting_0_100_OFF;
                BLEComModel.SettingCmd(VdiCommand.Setting0_100_OFF);
                if (BLEComModel.ComProcess(CommMode.COMMUNICATION_SETTING) > 0 && BLEComModel.SettingResultValue == true)
                {
                    Status = StateMachineStatus.None;
                    Console.WriteLine("Setting_Stop0_100 Cmd OK.");
                }
                Enqueue(Status);
                break;

            case StateMachineStatus.Setting_0_400_ON:
                Status = StateMachineStatus.Setting_0_400_ON;
                BLEComModel.SettingCmd(VdiCommand.Setting0_400_ON);
                if (BLEComModel.ComProcess(CommMode.COMMUNICATION_SETTING) > 0 && BLEComModel.SettingResultValue == true)
                {
                    Status = StateMachineStatus.Communication_SpeedTimeCalc;
                    Console.WriteLine("Setting_Start0_400 Cmd OK.");
                }
                Enqueue(Status);
                break;

            case StateMachineStatus.Setting_0_400_OFF:
                Status = StateMachineStatus.Setting_0_400_OFF;
                BLEComModel.SettingCmd(VdiCommand.Setting0_400_OFF);
                if (BLEComModel.ComProcess(CommMode.COMMUNICATION_SETTING) > 0 && BLEComModel.SettingResultValue == true)
                {
                    Status = StateMachineStatus.None;
                    Console.WriteLine("Setting_Stop0_400 Cmd OK.");
                }
                Enqueue(Status);
                break;

            case StateMachineStatus.Communication_SpeedTimeCalc:
                Status = StateMachineStatus.Communication_SpeedTimeCalc;
                BLEComModel.SettingCmd(VdiCommand.StartSpeedTimeCalcCmd);
                if (BLEComModel.ComProcess(CommMode.COMMUNICATION_LV) > 0)
                {
                    Status = StateMachineStatus.None;
                    Console.WriteLine("Communication_SpeedTimeCalc Cmd OK.");
                }
                Enqueue(Status);
                break;

			case StateMachineStatus.Communication_ClearDTC:
                lock (MutexLocker)
                {   
                    BLEComModel.SettingCmd(VdiCommand.ClearDtcCmd);
                    if (BLEComModel.ComProcess(CommMode.COMMUNICATION_SETTING) > 0 && BLEComModel.SettingResultValue == true)
                    {
                        Enqueue(StateMachineStatus.None);
                        Debug.WriteLine("Communication_ClearDTC Cmd OK.");
                    }
                    else
                    {
                        Enqueue(StateMachineStatus.Communication_ClearDTC);
                        Debug.WriteLine("Communication_ClearDTC Cmd Failed.");
                    }
                    Thread.Sleep(1000);
                }
				break;
 			case StateMachineStatus.Communication_TestPresent:
				Status = StateMachineStatus.Communication_TestPresent;
				recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_TEST_PRESENT);
				if (recvLen > 0)
				{
					Status = StateMachineStatus.None;
					Console.WriteLine("Communication_TestPresent Cmd OK.");
					break;
				}

				//if (recvLen == (int)CommErrorCode.WriteCommandError)
				//{
				//	if (BLEComModel.IsConnected)
				//	{
				//		BLEComModel.ComProcess(CommMode.DISCONNECT);
				//		break;
				//	}
				//}
				Thread.Sleep(20);
				//Enqueue(Status);
				break;

            case StateMachineStatus.Communication_WriteMemory_For_SunModeSetting:
                Status = StateMachineStatus.Communication_WriteMemory_For_SunModeSetting;
                writeMemoryKeyValues.Clear();
                //Get SettingViewController Sun Brightness Value
                float sunBrightnessValue = UIModel.vcManger.settinggViewController.SunBrightness;
                writeMemoryKeyValues.Add((int)ReadWriteMemoryID.SunModeBrightness, sunBrightnessValue);
                BLEComModel.SettingCmd(VdiCommand.SettingWriteMemoryCommand(writeMemoryKeyValues));
                recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_READ_WRITE_MEMORY);
                if (recvLen > 0)
                {
                    Console.WriteLine("WriteMemory_SunModeBrightness Cmd OK. Write Value : {0}", sunBrightnessValue);
                }
                else
                {
                    Console.WriteLine("WriteMemory_SunModeBrightness Cmd Failed. ");
                    break;
                }
                break;

            case StateMachineStatus.Communication_WriteMemory_For_NightModeSetting:
                Status = StateMachineStatus.Communication_WriteMemory_For_NightModeSetting;
                writeMemoryKeyValues.Clear();
                //Get SettingViewController Night Brightness Value
                float nightBrightnessValue = UIModel.vcManger.settinggViewController.NightBrightness;
                writeMemoryKeyValues.Add((int)ReadWriteMemoryID.NightModeBrightness, nightBrightnessValue);
                BLEComModel.SettingCmd(VdiCommand.SettingWriteMemoryCommand(writeMemoryKeyValues));
                recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_READ_WRITE_MEMORY);
                if (recvLen > 0)
                {
                    Console.WriteLine("WriteMemory_NightModeBrightness Cmd OK. Write Value : {0}", nightBrightnessValue);
                }
                else
                {
                    Console.WriteLine("WriteMemory_NightModeBrightness Cmd Failed. ");
                    break;
                }
                break;

            case StateMachineStatus.Communication_WriteMemory_For_ValveMode_Adjust:
                Status = StateMachineStatus.Communication_WriteMemory_For_ValveMode_Adjust;
                writeMemoryKeyValues.Clear();
                //Get SettingViewController Valve Mode Value
                float valveNormalModeValue = UIModel.vcManger.settinggViewController.ValveNormalOpenCloseMode;
                writeMemoryKeyValues.Add((int)ReadWriteMemoryID.ValveOpenCloseMode, valveNormalModeValue);
                BLEComModel.SettingCmd(VdiCommand.SettingWriteMemoryCommand(writeMemoryKeyValues));
                recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_READ_WRITE_MEMORY);
                if (recvLen > 0)
                {
                    Console.WriteLine("WriteMemory_ValveNormalMode Cmd OK. Write Value : {0}", valveNormalModeValue);
                }
                else
                {
                    Console.WriteLine("WriteMemory_NightModeBrightness Cmd Failed. ");
                    break;
                }
                break;
			case StateMachineStatus.Communication_ReadMemory_For_Setting:
				Status = StateMachineStatus.Communication_ReadMemory_For_Setting;
				if (Communication_ReadMemory_For_Setting_Method())
					Status = StateMachineStatus.None;
				break;
          

#region WRITE_MEMORY_FOR_VALVE_AUTO_MODES_SETTING
			case StateMachineStatus.Communication_WriteMemory_For_ValveAutoMode1RpmSetting:
				Status = StateMachineStatus.Communication_WriteMemory_For_ValveAutoMode1RpmSetting;
				writeMemoryKeyValues.Clear();
				//取得SETTING FRAGMENT的數值
                float autoMode1Rpm = SettinggViewController.Instance.Model_1._RPM;
                //float autoMode1Rpm = SettinggViewController.Instance.GetAutoMode1RpmText();
                Console.WriteLine("autoMode1111111Rpm " + autoMode1Rpm);
				//float autoMode1Rpm = 3000;
				writeMemoryKeyValues.Add((int)ReadWriteMemoryID.ValveAuto1Mode_RPM, autoMode1Rpm);
				BLEComModel.SettingCmd(VdiCommand.SettingWriteMemoryCommand(writeMemoryKeyValues));
				recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_READ_WRITE_MEMORY);
				if (recvLen > 0)
				{
					Debug.WriteLine("WriteMemory_ValveAutoMode1RpmSetting Cmd OK. Write Value : {0}", autoMode1Rpm);
				}
				else
				{
					Debug.WriteLine("WriteMemory_ValveAutoMode1RpmSetting Cmd Failed. ");
					break;
				}
				break;

			case StateMachineStatus.Communication_WriteMemory_For_ValveAutoMode2RpmSetting:
				Status = StateMachineStatus.Communication_WriteMemory_For_ValveAutoMode2RpmSetting;
				writeMemoryKeyValues.Clear();
                //取得SETTING FRAGMENT的數值
                float autoMode2Rpm = SettinggViewController.Instance.Model_2._RPM;
				//float autoMode2Rpm = SettinggViewController.Instance.GetAutoMode2RpmText();
                //Console.WriteLine("autoMode2222222Rpm " + autoMode2Rpm);
				//float autoMode2Rpm = 3500;
				writeMemoryKeyValues.Add((int)ReadWriteMemoryID.ValveAuto2Mode_RPM, autoMode2Rpm);
				BLEComModel.SettingCmd(VdiCommand.SettingWriteMemoryCommand(writeMemoryKeyValues));
				recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_READ_WRITE_MEMORY);
				if (recvLen > 0)
				{
					Debug.WriteLine("WriteMemory_ValveAutoMode2RpmSetting Cmd OK. Write Value : {0}", autoMode2Rpm);
				}
				else
				{
					Debug.WriteLine("WriteMemory_ValveAutoMode2RpmSetting Cmd Failed. ");
					break;
				}
				break;

			case StateMachineStatus.Communication_WriteMemory_For_ValveAutoMode3RpmSetting:
				Status = StateMachineStatus.Communication_WriteMemory_For_ValveAutoMode3RpmSetting;
				writeMemoryKeyValues.Clear();
				//取得SETTING FRAGMENT的數值
                float autoMode3Rpm = SettinggViewController.Instance.Model_3._RPM;
				//float autoMode3Rpm = SettinggViewController.Instance.GetAutoMode3RpmText();
                //Console.WriteLine("autoMode333333Rpm " + autoMode3Rpm);
				//float autoMode3Rpm = 4000;
				writeMemoryKeyValues.Add((int)ReadWriteMemoryID.ValveAuto3Mode_RPM, autoMode3Rpm);
				BLEComModel.SettingCmd(VdiCommand.SettingWriteMemoryCommand(writeMemoryKeyValues));
				recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_READ_WRITE_MEMORY);
				if (recvLen > 0)
				{
					Debug.WriteLine("WriteMemory_ValveAutoMode3RpmSetting Cmd OK. Write Value : {0}", autoMode3Rpm);
				}
				else
				{
					Debug.WriteLine("WriteMemory_ValveAutoMode3RpmSetting Cmd Failed. ");
					break;
				}
				break;

			case StateMachineStatus.Communication_WriteMemory_For_ValveAutoMode1TpsSetting:
				Status = StateMachineStatus.Communication_WriteMemory_For_ValveAutoMode1TpsSetting;
				writeMemoryKeyValues.Clear();
				//取得SETTING FRAGMENT的數值
				//float autoMode1Tps = SettinggViewController.Instance.GetAutoMode1TpsText();
                float autoMode1Tps = SettinggViewController.Instance.Model_1._TPS;
                Console.WriteLine("autoMode1111111Tps " + autoMode1Tps);
				//float autoMode1Tps = 30.0f;
				writeMemoryKeyValues.Add((int)ReadWriteMemoryID.ValveAuto1Mode_TPS, autoMode1Tps);
				BLEComModel.SettingCmd(VdiCommand.SettingWriteMemoryCommand(writeMemoryKeyValues));
				recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_READ_WRITE_MEMORY);
				if (recvLen > 0)
				{
					Debug.WriteLine("WriteMemory_ValveAutoMode1TpsSetting Cmd OK. Write Value : {0}", autoMode1Tps);
				}
				else
				{
					Debug.WriteLine("WriteMemory_ValveAutoMode1TpsSetting Cmd Failed. ");
					break;
				}
				break;

			case StateMachineStatus.Communication_WriteMemory_For_ValveAutoMode2TpsSetting:
				Status = StateMachineStatus.Communication_WriteMemory_For_ValveAutoMode2TpsSetting;
				writeMemoryKeyValues.Clear();
                //取得SETTING FRAGMENT的數值
                float autoMode2Tps = SettinggViewController.Instance.Model_2._TPS;
				//float autoMode2Tps = SettinggViewController.Instance.GetAutoMode2TpsText();

				//float autoMode2Tps = 30.0f;
				writeMemoryKeyValues.Add((int)ReadWriteMemoryID.ValveAuto2Mode_TPS, autoMode2Tps);
				BLEComModel.SettingCmd(VdiCommand.SettingWriteMemoryCommand(writeMemoryKeyValues));
				recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_READ_WRITE_MEMORY);
				if (recvLen > 0)
				{
					Debug.WriteLine("WriteMemory_ValveAutoMode2TpsSetting Cmd OK. Write Value : {0}", autoMode2Tps);
				}
				else
				{
					Debug.WriteLine("WriteMemory_ValveAutoMode2TpsSetting Cmd Failed. ");
					break;
				}
				break;

			case StateMachineStatus.Communication_WriteMemory_For_ValveAutoMode3TpsSetting:
				Status = StateMachineStatus.Communication_WriteMemory_For_ValveAutoMode3RpmSetting;
				writeMemoryKeyValues.Clear();
				//取得SETTING FRAGMENT的數值
                float autoMode3Tps = SettinggViewController.Instance.Model_3._TPS;
				//float autoMode3Tps = SettinggViewController.Instance.GetAutoMode3TpsText();
				//float autoMode3Tps = 40.0f;
				writeMemoryKeyValues.Add((int)ReadWriteMemoryID.ValveAuto3Mode_TPS, autoMode3Tps);
				BLEComModel.SettingCmd(VdiCommand.SettingWriteMemoryCommand(writeMemoryKeyValues));
				recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_READ_WRITE_MEMORY);
				if (recvLen > 0)
				{
					Debug.WriteLine("WriteMemory_ValveAutoMode3TpsSetting Cmd OK. Write Value : {0}", autoMode3Tps);
				}
				else
				{
					Debug.WriteLine("WriteMemory_ValveAutoMode3TpsSetting Cmd Failed. ");
					break;
				}
				break;

			case StateMachineStatus.Communication_WriteMemory_For_ValveAutoMode1DelayTimeSetting:
				Status = StateMachineStatus.Communication_WriteMemory_For_ValveAutoMode1DelayTimeSetting;
				writeMemoryKeyValues.Clear();
				//取得SETTING FRAGMENT的數值
				//float autoMode1DelayTimeSec = SettinggViewController.Instance.GetAutoMode1DelaytimeText();
                float autoMode1DelayTimeSec = SettinggViewController.Instance.Model_1._DELAYTIME;
                Console.WriteLine("autoMode1111DelayTimeSec " + autoMode1DelayTimeSec);
				//float autoMode1DelayTimeSecMillisec = 1;
				//float autoMode1DelayTimeSec = autoMode1DelayTimeSecMillisec * 1000;
				writeMemoryKeyValues.Add((int)ReadWriteMemoryID.ValveAuto1Mode_DelayTime, autoMode1DelayTimeSec);
                BLEComModel.SettingCmd(VdiCommand.SettingWriteMemoryCommand(writeMemoryKeyValues));
				recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_READ_WRITE_MEMORY);
				if (recvLen > 0)
				{
					Debug.WriteLine("WriteMemory_ValveAutoMode1DelayTimeSetting Cmd OK. Write Value : {0}", autoMode1DelayTimeSec);
				}
				else
				{
					Debug.WriteLine("WriteMemory_ValveAutoMode1DelayTimeSetting Cmd Failed. ");
					break;
				}
				break;

			case StateMachineStatus.Communication_WriteMemory_For_ValveAutoMode2DelayTimeSetting:
				Status = StateMachineStatus.Communication_WriteMemory_For_ValveAutoMode2DelayTimeSetting;
				writeMemoryKeyValues.Clear();
				//取得SETTING FRAGMENT的數值
                float autoMode2DelayTimeSec = SettinggViewController.Instance.Model_2._DELAYTIME;
				//float autoMode2DelayTimeSec = SettinggViewController.Instance.GetAutoMode2DelaytimeText();
				//float autoMode2DelayTimeMillisec = 1;
				//float autoMode2DelayTimeSec = autoMode2DelayTimeMillisec * 1000;
				writeMemoryKeyValues.Add((int)ReadWriteMemoryID.ValveAuto2Mode_DelayTime, autoMode2DelayTimeSec);
				BLEComModel.SettingCmd(VdiCommand.SettingWriteMemoryCommand(writeMemoryKeyValues));
				recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_READ_WRITE_MEMORY);
				if (recvLen > 0)
				{
					Debug.WriteLine("WriteMemory_ValveAutoMode2DelayTimeSetting Cmd OK. Write Value : {0}", autoMode2DelayTimeSec);
				}
				else
				{
					Debug.WriteLine("WriteMemory_ValveAutoMode2DelayTimeSetting Cmd Failed. ");
					break;
				}
				break;

			case StateMachineStatus.Communication_WriteMemory_For_ValveAutoMode3DelayTimeSetting:
				Status = StateMachineStatus.Communication_WriteMemory_For_ValveAutoMode3DelayTimeSetting;
				writeMemoryKeyValues.Clear();
				//取得SETTING FRAGMENT的數值
                float autoMode3DelayTimeSec = SettinggViewController.Instance.Model_3._DELAYTIME;
				//float autoMode3DelayTimeSec = SettinggViewController.Instance.GetAutoMode3DelaytimeText();
				//float autoMode3DelayTimeMillisec = 1;
				//float autoMode3DelayTimeSec = autoMode3DelayTimeMillisec * 1000;
				writeMemoryKeyValues.Add((int)ReadWriteMemoryID.ValveAuto3Mode_DelayTime, autoMode3DelayTimeSec);
				BLEComModel.SettingCmd(VdiCommand.SettingWriteMemoryCommand(writeMemoryKeyValues));
				recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_READ_WRITE_MEMORY);
				if (recvLen > 0)
				{
					Debug.WriteLine("WriteMemory_ValveAutoMode3DelayTimeSetting Cmd OK. Write Value : {0}", autoMode3DelayTimeSec);
				}
				else
				{
					Debug.WriteLine("WriteMemory_ValveAutoMode3DelayTimeSetting Cmd Failed. ");
					break;
				}
				break;

			//執行清除ODO
			case StateMachineStatus.Communication_WriteMemory_For_ClearODO:
				Status = StateMachineStatus.Communication_WriteMemory_For_ClearODO;
				writeMemoryKeyValues.Clear();
				int odoClearValue = 0;
				writeMemoryKeyValues.Add((int)ReadWriteMemoryID.ODO_Memory, odoClearValue);
				BLEComModel.SettingCmd(VdiCommand.SettingWriteMemoryCommand(writeMemoryKeyValues));
				recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_READ_WRITE_MEMORY);
				if (recvLen > 0)
				{
					Debug.WriteLine("Communication_WriteMemory_For_ClearODO Cmd OK. Write Value : {0}", odoClearValue);
				}
				else
				{
					Debug.WriteLine("Communication_WriteMemory_For_ClearODO Cmd Failed. ");
					break;
				}
				break;

			//執行設定ODO
			case StateMachineStatus.Communication_WriteMemory_For_SettingODO:
				Status = StateMachineStatus.Communication_WriteMemory_For_SettingODO;
				writeMemoryKeyValues.Clear();
                //設定ODO值
                float odoValue = 0;
                //var odoNav = ContainerViewController.Instance.odoNavViewController;

                if(ContainerViewController.Instance.odoNavViewController != null)
                {
                    //var childs = odoNav.ChildViewControllers;
                    odoValue = OdoViewController.Instance.OdoRemind;
                }
				
				writeMemoryKeyValues.Add((int)ReadWriteMemoryID.ODO_Memory, odoValue);
				BLEComModel.SettingCmd(VdiCommand.SettingWriteMemoryCommand(writeMemoryKeyValues));
				recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_READ_WRITE_MEMORY);
				if (recvLen > 0)
				{
					Debug.WriteLine("Communication_WriteMemory_For_SettingODO Cmd OK. Write Value : {0}", odoValue);
				}
				else
				{
					Debug.WriteLine("Communication_WriteMemory_For_SettingODO Cmd Failed. ");
					break;
				}
				break;

			//執行清除TRIP
			case StateMachineStatus.Communication_WriteMemory_For_ClearTrip:
				Status = StateMachineStatus.Communication_WriteMemory_For_ClearTrip;
				writeMemoryKeyValues.Clear();
				int tripClearValue = 0;
				writeMemoryKeyValues.Add((int)ReadWriteMemoryID.Trip_Memory, tripClearValue);
				BLEComModel.SettingCmd(VdiCommand.SettingWriteMemoryCommand(writeMemoryKeyValues));
				recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_READ_WRITE_MEMORY);
				if (recvLen > 0)
				{
					Debug.WriteLine("Communication_WriteMemory_For_ClearTrip Cmd OK. Write Value : {0}", tripClearValue);
				}
				else
				{
					Debug.WriteLine("Communication_WriteMemory_For_ClearTrip Cmd Failed. ");
					break;
				}
				break;

			//執行設定Trip
			case StateMachineStatus.Communication_WriteMemory_For_SettingTrip:
				Status = StateMachineStatus.Communication_WriteMemory_For_SettingTrip;
				writeMemoryKeyValues.Clear();
				//設定TRIP值
				int tripValue = 0;
				writeMemoryKeyValues.Add((int)ReadWriteMemoryID.Trip_Memory, tripValue);
				BLEComModel.SettingCmd(VdiCommand.SettingWriteMemoryCommand(writeMemoryKeyValues));
				recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_READ_WRITE_MEMORY);
				if (recvLen > 0)
				{
					Debug.WriteLine("Communication_WriteMemory_For_SettingTrip Cmd OK. Write Value : {0}", tripValue);
				}
				else
				{
					Debug.WriteLine("Communication_WriteMemory_For_SettingTrip Cmd Failed. ");
					break;
				}
				break;




            #endregion

#region Remote Diagnostic Process
            case StateMachineStatus.Communication_AMQ_Send:
                {
                    stateCode++;
                    //break;

                    var amqManager = RemoteDiagViewController.amqManager;
                    var producer = ContainerViewController.Instance.remoteDiagViewController.messageProducer;
                    var consumer = ContainerViewController.Instance.remoteDiagViewController.messageConsumer;
                    byte[] amqRecvData = StateMachine.DataModel.RemoteDiagRecvValues;
                    while (true)
                    {
                        try
                        {
                            if (amqManager != null &&
                                amqManager.Enabled &&
                                amqRecvData != null)
                            {
                                if (amqManager.SendMessage(producer, amqRecvData))
                                {
                                    string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                                    var result = String.Format("{0}\n{1}", timestamp, BitConverter.ToString(amqRecvData));
                                    ContainerViewController.Instance.remoteDiagViewController.SetRemoteRecvTextViewCotent(result);
                                    break;
                                }
                            }
                            Thread.Sleep(100);
                            var isRemoteDiagStart = StateMachine.DataModel.IsRemoteDiagnosticStart;
                            if (!isRemoteDiagStart)
                            {
                                Enqueue(StateMachineStatus.Communication_Remote_Diagnostic_Exit);
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            break;
                        }
                    }
                    Status = StateMachineStatus.Communication_AMQ_Recv;
                    Enqueue(Status);
                }
                break;

            case StateMachineStatus.Communication_AMQ_Recv:
                {
                    var amqManager = RemoteDiagViewController.amqManager;
                    var producer = ContainerViewController.Instance.remoteDiagViewController.messageProducer;
                    var consumer = ContainerViewController.Instance.remoteDiagViewController.messageConsumer;
                    byte[] amqRecvData = null;

                    while (true)
                    {
                        try
                        {
                            if (amqManager != null &&
                                amqManager.Enabled)
                            {
                                amqRecvData = amqManager.RecvBytesMessage(consumer, 500);
                                if (amqRecvData != null)
                                {
                                    string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                                    var result = String.Format("{0}\n{1}", timestamp, BitConverter.ToString(amqRecvData));
                                    ContainerViewController.Instance.remoteDiagViewController.SetRemoteSendTextViewCotent(result);
                                    BLEComModel.SettingCmd(amqRecvData);
                                    break;
                                }
                            }
                            var isRemoteDiagStart = StateMachine.DataModel.IsRemoteDiagnosticStart;
                            if (!isRemoteDiagStart)
                            {
                                Enqueue(StateMachineStatus.Communication_Remote_Diagnostic_Exit);
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            break;
                        }
                    }
                    Status = StateMachineStatus.Communication_Remote_Diagnostic_Write;
                    Enqueue(Status);
                }
                break;

            case StateMachineStatus.Communication_Remote_Diagnostic_Write:
                {
                    Status = StateMachineStatus.Communication_Remote_Diagnostic_Write;
                    StateMachine.BLEComModel.IsStartJ1939DataLinkProtocol = true;
                    //如果不為AMQ遠距診斷模式，使用預設指令
                    if (!StateMachine.DataModel.IsEnableAMQRemoteDiagnostic)
                    {
                        if (stateCode == 1)
                        {
                            //KEHIN ECU用INIT指令
                            VdiCommand.RemoteDiagnosticInit[2] = 0x09;
                            BLEComModel.SettingCmd(VdiCommand.RemoteDiagnosticInit);
                        }
                        else
                        {
                            //KEHIN ECU用LV指令
                            VdiCommand.RemoteDiagnosticLvData[2] = 0x10;
                            BLEComModel.SettingCmd(VdiCommand.RemoteDiagnosticLvData);
                        }
                    }



                    recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_REMOTE_DIAGNOSTIC_WRITE);
                    if (recvLen > 0)
                    {
                        Status = StateMachineStatus.Communication_Remote_Diagnostic_Read;
                        StateMachine.BLEComModel.IsStartJ1939DataLinkProtocol = false;
                        Debug.WriteLine("Communication_Remote_Diagnostic_Write Cmd OK.");
                    }

                    var isRemoteDiagStart = StateMachine.DataModel.IsRemoteDiagnosticStart;
                    if (!isRemoteDiagStart)
                    {
                        Enqueue(StateMachineStatus.Communication_Remote_Diagnostic_Exit);
                        break;
                    }

                    if (recvLen == (int)CommErrorCode.WriteCommandError)
                    {
                        if (BLEComModel.IsConnected == false)
                            break;
                    }
                    Enqueue(Status);
                }
                break;

            case StateMachineStatus.Communication_Remote_Diagnostic_Read:
                {
                    Status = StateMachineStatus.Communication_Remote_Diagnostic_Read;
                    StateMachine.BLEComModel.IsStartJ1939DataLinkProtocol = true;

                    recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_REMOTE_DIAGNOSTIC_READ);
                    if (recvLen > 0)
                    {
                        if (StateMachine.DataModel.IsEnableAMQRemoteDiagnostic)
                            Status = StateMachineStatus.Communication_AMQ_Send;
                        else
                            Status = StateMachineStatus.Communication_Remote_Diagnostic_Write;

                        StateMachine.BLEComModel.IsStartJ1939DataLinkProtocol = false;
                        Debug.WriteLine("Communication_Remote_Diagnostic_Read Cmd OK.");
                        stateCode++;
                        //break;

                        //var amqManager = RemoteDiagFragment.amqManager;
                        //var producer = StateMachine.UIModel.remoteDiagFragment.messageProducer;
                        //var consumer = StateMachine.UIModel.remoteDiagFragment.messageConsumer;
                        //byte[] amqRecvData = StateMachine.DataModel.RemoteDiagRecvValues;
                        //while (true)
                        //{
                        //    try
                        //    {
                        //        if (amqManager != null &&
                        //            amqManager.Enabled &&
                        //            amqRecvData != null)
                        //        {
                        //            if (amqManager.SendMessage(producer, amqRecvData))
                        //                break;
                        //        }
                        //        Thread.Sleep(100);
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        break;
                        //    }
                        //}
                    }

                    var isRemoteDiagStart = StateMachine.DataModel.IsRemoteDiagnosticStart;
                    if (!isRemoteDiagStart)
                    {
                        Enqueue(StateMachineStatus.Communication_Remote_Diagnostic_Exit);
                        break;
                    }

                    if (recvLen == (int)CommErrorCode.WriteCommandError)
                    {
                    }
                    Enqueue(Status);
                }
                break;

            case StateMachineStatus.Communication_Remote_Diagnostic_Exit:
                {
                    Status = StateMachineStatus.Communication_Remote_Diagnostic_Exit;

                    if (!IsReadOk)
                    {
                        Enqueue(Status);
                        break;
                    }

                    recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_REMOTE_DIAGNOSTIC_EXIT);

                    //if (recvLen > 0)
                    if (BLEComModel.SettingResultValue == true)
                    {
                        Debug.WriteLine("Remote Diagnostic Exit Cmd OK.");
                        stateCode = 1;
                        break;
                    }
                    Enqueue(Status);
                    break;
                }

            case StateMachineStatus.Communication_AllEcuId_With_J1939DataLink_Protocol:
                {
                    Status = StateMachineStatus.Communication_AllEcuId_With_J1939DataLink_Protocol;
                    StateMachine.BLEComModel.IsStartJ1939DataLinkProtocol = true;
                    byte[] cmd = { 0xF0, 0x0F, 0x44 };
                    BLEComModel.SettingCmd(cmd);
                    recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_REMOTE_DIAGNOSTIC_WRITE);
                    if (recvLen > 0)
                    {
                        Debug.WriteLine("Communication_AllEcuId_With_J1939_Protocol Write Cmd OK.");
                        recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_REMOTE_DIAGNOSTIC_READ);
                        if (recvLen > 0)
                        {
                            Debug.WriteLine("Communication_AllEcuId_With_J1939_Protocol Read Cmd OK.");
                            System.Diagnostics.Debug.WriteLine("Recv Data : {0}", BitConverter.ToString(StateMachine.DataModel.RemoteDiagRecvValues));
                            Status = StateMachineStatus.Communication_VIN_With_J1939DataLink_Protocol;
                            StateMachine.BLEComModel.IsStartJ1939DataLinkProtocol = false;
                        }
                    }
                    else
                    {
                        var isRemoteDiagStart = StateMachine.DataModel.IsRemoteDiagnosticStart;
                        if (!isRemoteDiagStart)
                        {
                            Enqueue(Status);
                            break;
                        }
                    }

                    if (recvLen == (int)CommErrorCode.WriteCommandError)
                    {
                        if (BLEComModel.IsConnected == false)
                            break;
                    }
                    Enqueue(Status);
                }
                break;

            case StateMachineStatus.Communication_VIN_With_J1939DataLink_Protocol:
                {
                    Status = StateMachineStatus.Communication_VIN_With_J1939DataLink_Protocol;
                    StateMachine.BLEComModel.IsStartJ1939DataLinkProtocol = true;
                    byte[] cmd = { 0xF0, 0x0F, 0x3F };
                    BLEComModel.SettingCmd(cmd);
                    recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_REMOTE_DIAGNOSTIC_WRITE);
                    if (recvLen > 0)
                    {
                        Debug.WriteLine("Communication_VIN_With_J1939_Protocol Write Cmd OK.");
                        recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_REMOTE_DIAGNOSTIC_READ);
                        if (recvLen > 0)
                        {
                            Debug.WriteLine("Communication_VIN_With_J1939_Protocol Read Cmd OK.");
                            System.Diagnostics.Debug.WriteLine("Recv Data : {0}", BitConverter.ToString(StateMachine.DataModel.RemoteDiagRecvValues));
                            Status = StateMachineStatus.Communication_ECU_ID;
                            StateMachine.BLEComModel.IsStartJ1939DataLinkProtocol = false;
                        }
                    }
                    else
                    {
                        var isRemoteDiagStart = StateMachine.DataModel.IsRemoteDiagnosticStart;
                        if (!isRemoteDiagStart)
                        {
                            Enqueue(Status);
                            break;
                        }
                    }

                    if (recvLen == (int)CommErrorCode.WriteCommandError)
                    {
                        if (BLEComModel.IsConnected == false)
                            break;
                    }
                    Enqueue(Status);
                }
                break;

            case StateMachineStatus.Communication_VehicleInfos_With_J1939DataLink_Protocol:
                {
                    Status = StateMachineStatus.Communication_VIN_With_J1939DataLink_Protocol;
                    StateMachine.BLEComModel.IsStartJ1939DataLinkProtocol = true;
                    //List<byte[]> cmds = new List<byte[]> { new byte[]{ 0xF0, 0x0F, 0x3F } ,
                    //                                       new byte[]{ 0xF0, 0x0F, 0x44 } ,
                    //                                       new byte[]{ 0xF0, 0x0F, 0x45 } ,
                    //                                       new byte[]{ 0xF0, 0x9C, 0x40 } ,
                    //                                       new byte[]{ 0xF0, 0x9C, 0x41 } ,
                    //                                       new byte[]{ 0xF0, 0x9C, 0x42 } ,
                    //                                       new byte[]{ 0xF0, 0x9C, 0x43 } ,
                    //};

                    //var ecuInfoDict = DataModel.EcuInfoDict;
                    //var ecuInfoDict = DataModel.EcuInfoIcmDBHelper.AllEcuInfosByEcuIds(new List<uint>() { 9 });
                    #region 獲取ALL_ECU_ID與VEHICLE_ID
                    Dictionary<UInt32, EcuInfoData> ecuInfoDict = null;

                    //如果EcuInfo為null，原本為離開此段程序
                    //改為加入EcuInfo一定要有的指令
                    //硬體規範 : 
                    //ALL ECU ID = 0x0F44
                    //GET Vehicle ID (Model Code) = 0x0F45
                    if (ecuInfoDict == null ||
                        !(ecuInfoDict.Keys.Contains(VdiCommand.GetEcuIdCmdID) && (ecuInfoDict.Keys.Contains(VdiCommand.GetModelCodeCmdID))))
                    {
                        //Status = StateMachineStatus.Communication_ECU_ID;
                        //StateMachine.BLEComModel.IsStartJ1939DataLinkProtocol = false;
                        //Enqueue(Status);
                        //break;
                        if (ecuInfoDict == null)
                            ecuInfoDict = new Dictionary<uint, CommonLib.IcmLib.Data.EcuInfoData>();

                        //ALL ECU ID = 0x0F44
                        UInt16 allEcuIdCmdID = VdiCommand.GetEcuIdCmdID;
                        var allEcuIdEcuInfoData = new CommonLib.IcmLib.Data.EcuInfoData();
                        allEcuIdEcuInfoData.Name = @"All Ecu ID";

                        //GET Vehicle ID (Vehicle Code) = 0x0F45
                        UInt16 getModelCodeCmdID = VdiCommand.GetModelCodeCmdID;
                        var getModelCodeEcuInfoData = new CommonLib.IcmLib.Data.EcuInfoData();
                        getModelCodeEcuInfoData.IcmDataType = IcmDataType.ASCII;
                        getModelCodeEcuInfoData.Name = @"Model Code";

                        ecuInfoDict.Add(allEcuIdCmdID, allEcuIdEcuInfoData);
                        ecuInfoDict.Add(getModelCodeCmdID, getModelCodeEcuInfoData);
                    }

                    List<byte[]> cmds = new List<byte[]>();
                    byte[] cmd = { 0xF0, 0x00, 0x00 };

                    foreach (var key in ecuInfoDict.Keys)
                    {
                        cmd[1] = (byte)(key >> 8);
                        cmd[2] = (byte)(key);
                        if (!cmds.Contains(cmd))
                            cmds.Add(cmd.Clone() as byte[]);
                    }

                    recvLen = -1;

                    for (int i = 0; i < cmds.Count; i++)
                    {
                        BLEComModel.SettingCmd(cmds[i]);
                        recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_REMOTE_DIAGNOSTIC_WRITE);
                        if (recvLen > 0)
                        {
                            Debug.WriteLine("Communication_VehicleInfos_With_J1939_Protocol Write Cmd OK.");
                            recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_REMOTE_DIAGNOSTIC_READ);
                            if (recvLen > 0)
                            {
                                Debug.WriteLine("Communication_VehicleInfos_With_J1939_Protocol Read Cmd OK.");
                                System.Diagnostics.Debug.WriteLine(String.Format("Send Cmd : {0}, Recv Data : {1}", BitConverter.ToString(cmds[i]), BitConverter.ToString(StateMachine.DataModel.RemoteDiagRecvValues)));
                                var ecuInfoId = ((cmds[i][1] << 8) + cmds[i][2]);
                                if (ecuInfoDict.ContainsKey((uint)ecuInfoId))
                                {
                                    var rawValues = StateMachine.DataModel.RemoteDiagRecvValues.Clone() as byte[];
                                    //去除回覆回來的第一個指令bytes 0xF0
                                    ArraySegment<byte> segment = new ArraySegment<byte>(rawValues, 1, rawValues.Length - 1);
                                    ecuInfoDict[(uint)ecuInfoId].RawValues = segment.ToArray();
                                }
                            }
                        }
                        else
                        {
                            var isRemoteDiagStart = StateMachine.DataModel.IsRemoteDiagnosticStart;
                            if (!isRemoteDiagStart)
                            {
                                Enqueue(Status);
                                break;
                            }
                            i--;
                            if (i < 0)
                                i = 0;
                        }
                    }
                    #endregion

                    //儲存通訊或得之ECU ID列表
                    var ecuIdList = StateMachine.DataModel.EcuIdList;
                    var datas = ecuInfoDict[VdiCommand.GetEcuIdCmdID].RawValues;
                    int count = datas?.Count() / 2 ?? 0;
                    ecuIdList.Clear();
                    using (IcmBinaryReader reader = new IcmBinaryReader(datas))
                    {
                        try
                        {
                            for (int i = 0; i < count; i++)
                            {
                                var ecuId = reader.ReadUInt16();
                                ecuIdList.Add(ecuId);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.StackTrace);
                        }
                    }

                    //獲取全部EcuId通訊完畢後，將獲得之EcuId列表的第一項，預設賦值給資料模組的EcuID屬性
                    if (ecuIdList.Count > 0)
                        StateMachine.DataModel.EcuID = ecuIdList[0];

                    //var ecuGroupListForEngine = DataModel.EcuGroupList.Where(x=>x.EcuName.Equals("EMS"));
                    var ecuGroupDict = DataModel.EcuGroupsDict;
                    //將ECU ID列表存進原始資料結構
                    foreach (var ecuId in ecuIdList)
                    {
                        if (ecuGroupDict.ContainsKey(ecuId) &&
                            ecuGroupDict[ecuId].GroupName.Equals("EMS"))
                        {
                            DataModel.EcuID = ecuId;
                            DataModel.DtcUnpacker.EcuID = ecuId;
                        }
                    }


                    //將解碼完畢之VehicleCode做主頁UI更新
                    var vehicleIdBytes = ecuInfoDict[VdiCommand.GetModelCodeCmdID].RawValues;
                    Array.Reverse(vehicleIdBytes);
                    var vehicleId = BitConverter.ToUInt32(vehicleIdBytes, 0);
                    var vehicleIdStr = vehicleId.ToString();
                    var IsUnpackVin = mDataModel.VinIcmUnpacker.Unpack(vehicleIdStr);
                    if (IsUnpackVin)
                        Debug.WriteLine("Unpack VIN OK");
                    else
                        Debug.WriteLine("Unpack VIN Failed");

                    var motorVinData = DataModel.VinIcmUnpacker.MotorVinData;
                    if (motorVinData != null)
                    {
                        var VehicleImage = motorVinData.ImgName;
                        if (VehicleImage != null)
                            ContainerViewController.Instance.homeViewController.SetUIImageByResouceName(VehicleImage);
                            //UIModel?.homew?.SetBitmapByDrawableID(VehicleImage);
                    }



                    #region 利用獲取之ECU_ID列表，來取得各ECU_ID相關之ECU_INFO
                    //ecuInfoDict = DataModel.EcuInfoIcmDBHelper.AllEcuInfosByEcuIds(new List<uint>() { 9 });
                    ecuInfoDict = DataModel.EcuInfoIcmDBHelper.AllEcuInfosByEcuIds(ecuIdList);
                    cmds.Clear();
                    foreach (var key in ecuInfoDict.Keys)
                    {
                        cmd[1] = (byte)(key >> 8);
                        cmd[2] = (byte)(key);
                        if (!cmds.Contains(cmd))
                            cmds.Add(cmd.Clone() as byte[]);
                    }

                    recvLen = -1;
                    for (int i = 0; i < cmds.Count; i++)
                    {
                        BLEComModel.SettingCmd(cmds[i]);
                        recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_REMOTE_DIAGNOSTIC_WRITE);
                        if (recvLen > 0)
                        {
                            Debug.WriteLine("Communication_VehicleInfos_With_J1939_Protocol Write Cmd OK.");
                            recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_REMOTE_DIAGNOSTIC_READ);
                            if (recvLen > 0)
                            {
                                Debug.WriteLine("Communication_VehicleInfos_With_J1939_Protocol Read Cmd OK.");
                                System.Diagnostics.Debug.WriteLine(String.Format("Send Cmd : {0}, Recv Data : {1}", BitConverter.ToString(cmds[i]), BitConverter.ToString(StateMachine.DataModel.RemoteDiagRecvValues)));
                                var ecuInfoId = ((cmds[i][1] << 8) + cmds[i][2]);
                                if (ecuInfoDict.ContainsKey((uint)ecuInfoId))
                                {
                                    var rawValues = StateMachine.DataModel.RemoteDiagRecvValues.Clone() as byte[];
                                    //去除回覆回來的第一個指令bytes 0xF0
                                    ArraySegment<byte> segment = new ArraySegment<byte>(rawValues, 1, rawValues.Length - 1);
                                    ecuInfoDict[(uint)ecuInfoId].RawValues = segment.ToArray();
                                }
                            }
                        }
                        else
                        {
                            var isRemoteDiagStart = StateMachine.DataModel.IsRemoteDiagnosticStart;
                            if (!isRemoteDiagStart)
                            {
                                Enqueue(Status);
                                break;
                            }
                            i--;
                            if (i < 0)
                                i = 0;
                        }
                    }
                    #endregion
                    //Status = StateMachineStatus.Communication_ECU_ID;
                    Status = StateMachineStatus.Communication_LV_ID;
                    StateMachine.BLEComModel.IsStartJ1939DataLinkProtocol = false;

                    if (recvLen == (int)CommErrorCode.WriteCommandError)
                    {
                        if (BLEComModel.IsConnected == false)
                            break;
                    }
                    Enqueue(Status);
                }
                break;

            case StateMachineStatus.Communication_DTC_With_J1939DataLink_Protocol:
                {
                    Status = StateMachineStatus.Communication_DTC_With_J1939DataLink_Protocol;
                    StateMachine.BLEComModel.IsStartJ1939DataLinkProtocol = true;
                    //byte[] cmd = { 0xF1, 0x03, 0xEB };

                    var dtcCmdsDict = StateMachine.DataModel.DtcCmdsDict;
                    var currentSelectedEcuId = DataModel.EcuID;
                    //IcmDB資料庫內，如果不包含現在的EcuID，DTC通訊返回
                    //反則將通訊指令改為當前EcuID之DTC通訊指令
                    if (!dtcCmdsDict.ContainsKey(currentSelectedEcuId))
                    {
                        StateMachine.BLEComModel.IsStartJ1939DataLinkProtocol = false;
                        if (UIModel.CurrentPage == Page.DTC)
                            Enqueue(StateMachineStatus.Communication_DTC_With_J1939DataLink_Protocol);
                        else
                            RemoveAllSpecificMessage(StateMachineStatus.Communication_DTC_With_J1939DataLink_Protocol);
                        break;
                    }
                    else
                    {
                        //var dtcCmdId = dtcCmdsDict[currentSelectedEcuId][0].DtcCmd;
                        //StateMachine.DataModel.CurrentDtcCmdId = dtcCmdId;
                    }
                    var cmd = StateMachine.DataModel.CurrentDtcCmd;
                    BLEComModel.SettingCmd(cmd);
                    recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_REMOTE_DIAGNOSTIC_WRITE);
                    if (recvLen > 0)
                    {
                        Debug.WriteLine("Communication_DTC_With_J1939DataLink_Protocol Write Cmd OK.");
                        recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_REMOTE_DIAGNOSTIC_READ);
                        if (recvLen > 0)
                        {
                            Debug.WriteLine("Communication_DTC_With_J1939DataLink_Protocol Read Cmd OK.");
                            System.Diagnostics.Debug.WriteLine("Recv Data : {0}", BitConverter.ToString(StateMachine.DataModel.RemoteDiagRecvValues));

                            byte[] dtcRawValues = null;
                            int unpackPerBytes = 2;
                            var dtcDecodeType = dtcCmdsDict[currentSelectedEcuId][0].DtcDecodeType;
#if 模擬DTC_DECODE_TYPE_0x18
                            dtcDecodeType = DtcDecodeType.BytesBitsPosDecode;
#endif
                            switch (dtcDecodeType)
                            {
                                case DtcDecodeType.BytesBitsPosDecode:
#if 模擬DTC_DECODE_TYPE_0x18
                                    //模擬ECU ID = 35572 發送過來的DTC原始數據
                                    //0xF1 DTC指令前綴
                                    //0x01 狀態
                                    //後面8bytes，DTC原始數據
                                    dtcRawValues = new byte[] { 0xF1, 0x00, 0x00, 0x08, 0xFF, 0xFF, 0x01, 0x07, 0x0F, 0x5A };
                                    currentSelectedEcuId = 35572;
                                    dtcRawValues = DTCFragment.ConvertDtcRawDataToByteBitPosFormat(dtcRawValues).Clone() as byte[];
                                    //轉換資料為null的話，跳離此狀態機
                                    if (dtcRawValues == null)
                                    {
                                        StateMachine.BLEComModel.IsStartJ1939DataLinkProtocol = false;
                                        break;
                                    }
#else
                                    dtcRawValues = StateMachine.DataModel.RemoteDiagRecvValues.Clone() as byte[];
                                    dtcRawValues = NewDTCViewController.ConvertDtcRawDataToByteBitPosFormat(dtcRawValues).Clone() as byte[];
#endif

                                    unpackPerBytes = 2;
                                    break;

                                case DtcDecodeType.None:
                                case DtcDecodeType.DtcHexCompare:
                                default:
                                    dtcRawValues = StateMachine.DataModel.RemoteDiagRecvValues.Clone() as byte[];
                                    unpackPerBytes = dtcCmdsDict[currentSelectedEcuId][0].UnpackPerBytes;
                                    break;
                            }

                            //dtcRawValues為null或是dtc狀態非0的話，跳離此狀態機
                            if (dtcRawValues == null ||
                                (dtcRawValues.Count() >= 2 && dtcRawValues[1] != 0) ||
                                (dtcRawValues.Count() == 1 && dtcRawValues[0] == 0xEE))
                            {
                                StateMachine.BLEComModel.IsStartJ1939DataLinkProtocol = false;
                                if (UIModel.CurrentPage == Page.DTC)
                                    Enqueue(StateMachineStatus.Communication_DTC_With_J1939DataLink_Protocol);
                                else
                                    RemoveAllSpecificMessage(StateMachineStatus.Communication_DTC_With_J1939DataLink_Protocol);
                                break;
                            }
                            else
                                StateMachine.DataModel.UnpackJ1939DtcValues(dtcRawValues, currentSelectedEcuId, unpackPerBytes);

                            #region 獲取DTC數據與更新畫面
                            var dtcValuesMap = StateMachine.DataModel.DtcValuesMap[currentSelectedEcuId];
                            dtcList.Clear();
                            if (dtcValuesMap != null)
                            {
                                foreach (var dtcVal in dtcValuesMap.Values)
                                {
                                    dtcList.Add(dtcVal);
                                }
                            }
                            else
                            {
                                //IF NO DTC OR DTC VALUE IS NULL
                                DtcData dtcVal = new DtcData
                                {
                                    DtcHexNumber = 0,
                                    DtcName = "No Trouble Code.",
                                    DtcCodeForDisplay = "No DTC"
                                };
                                dtcList.Add(dtcVal);
                            }
                            #region DTC排序
                            //原資料排序
                            //dtcList.Sort((x,y)=> string.Compare(x.DtcCodeForDisplay, y.DtcCodeForDisplay));

                            //產生新list物件，先排序DtcCodeForDisplay，再排序DtcName
                            dtcList = dtcList.OrderBy(x => x.DtcCodeForDisplay).ThenBy(x => x.DtcName).ToList();
                            #endregion
                            
                            if (ContainerViewController.Instance.dtcViewController != null)
                            {
                                ContainerViewController.Instance.dtcViewController.Refresh(dtcList);
                            }
                            #endregion

                            if (UIModel.CurrentPage == Page.DTC)
                                Status = StateMachineStatus.Communication_DTC_With_J1939DataLink_Protocol;
                            else
                            {
                                RemoveAllSpecificMessage(StateMachineStatus.Communication_DTC_With_J1939DataLink_Protocol);
                                Status = StateMachineStatus.None;
                            }
                            StateMachine.BLEComModel.IsStartJ1939DataLinkProtocol = false;
                        }
                    }
                    else
                    {
                        var isRemoteDiagStart = StateMachine.DataModel.IsRemoteDiagnosticStart;
                        if (!isRemoteDiagStart)
                        {
                            Enqueue(Status);
                            break;
                        }
                    }

                    if (recvLen == (int)CommErrorCode.WriteCommandError)
                    {
                        if (BLEComModel.IsConnected == false)
                            break;
                    }
                    Enqueue(Status);
                }
                break;

            case StateMachineStatus.Communication_Set_Record_Mode_Time_With_J1939DataLink_Protocol:
                {
                    Status = StateMachineStatus.Communication_Set_Record_Mode_Time_With_J1939DataLink_Protocol;
                    StateMachine.BLEComModel.IsStartJ1939DataLinkProtocol = true;
                    byte[] cmd = { 0xF5, 0x57, 0x00, 0x00, 0x11, 0x22, 0x33, 0x44, 0x55 };
                    //var currentTime = DateTime.UtcNow;
                    var currentTime = DateTime.Now;
                    //Year
                    cmd[2] = (byte)(currentTime.Year >> 8);
                    cmd[3] = (byte)(currentTime.Year);
                    //Month
                    cmd[4] = (byte)(currentTime.Month);
                    //Day
                    cmd[5] = (byte)(currentTime.Day);
                    //Hour
                    cmd[6] = (byte)(currentTime.Hour);
                    //Minute
                    cmd[7] = (byte)(currentTime.Minute);
                    //Seconds
                    cmd[8] = (byte)(currentTime.Second);

                    BLEComModel.SettingCmd(cmd);
                    recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_REMOTE_DIAGNOSTIC_WRITE);
                    if (recvLen > 0)
                    {
                        Debug.WriteLine("Communication_Set_Record_Mode_Time_With_J1939DataLink_Protocol Write Cmd OK.");
                        recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_REMOTE_DIAGNOSTIC_READ);
                        if (recvLen > 0)
                        {
                            Debug.WriteLine("Communication_Set_Record_Mode_Time_With_J1939DataLink_Protocol Read Cmd OK.");
                            System.Diagnostics.Debug.WriteLine("Recv Data : {0}", BitConverter.ToString(StateMachine.DataModel.RemoteDiagRecvValues));
                            Status = StateMachineStatus.Communication_Change_to_Record_Mode;
                            StateMachine.BLEComModel.IsStartJ1939DataLinkProtocol = false;
                        }
                    }
                    else
                    {
                        var isRemoteDiagStart = StateMachine.DataModel.IsRemoteDiagnosticStart;
                        if (!isRemoteDiagStart)
                        {
                            Enqueue(Status);
                            break;
                        }
                    }

                    if (recvLen == (int)CommErrorCode.WriteCommandError)
                    {
                        if (BLEComModel.IsConnected == false)
                            break;
                    }
                    Enqueue(Status);
                }
                break;
 #endregion
            default:
                break;
        }
    }

	public void AccordingPageContinueOperation()
	{
		switch (UIModel.CurrentPage)
		{
			case Page.None:
				break;

			case Page.Init:
				break;

			case Page.Home:
				break;

			case Page.Log:
				//StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV_STOP);
				StateMachine.Instance.SendMessage(StateMachineStatus.Communication_FuelConsumption);
				break;

			case Page.LiveData:
				StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV);
				break;
            case Page.LiveData_2_Frame:
				StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV);
                //ContainerViewController.Instance.liveData_2_Frame_ViewController.Instance.Fan_1.SetNeedsDisplay();
				break;
			case Page.LiveData_4_Frame:
				StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV);
				//ContainerViewController.Instance.liveData_2_Frame_ViewController.Instance.Fan_1.SetNeedsDisplay();
				break;
			case Page.LiveData_6_Frame:
				StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV);
				//ContainerViewController.Instance.liveData_2_Frame_ViewController.Instance.Fan_1.SetNeedsDisplay();
				break;
			case Page.Gear:
				break;

			case Page.DTC:
				StateMachine.DataModel.ClearDtcValues();
				StateMachine.Instance.RemoveAllMessage();
				StateMachine.Instance.SendMessage(StateMachineStatus.Communication_DTC);
				StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV_STOP);
				StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV);
				break;

			case Page.Valve:
				//StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV_STOP);
				StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV);
				break;

			case Page.Speed0_100:
				//StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV_STOP);
				break;

			case Page.Speed0_400:
				//StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV_STOP);
				break;

			case Page.Shift:
				//StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV_STOP);
				StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV);
				break;

			case Page.Setting:
				//StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV_STOP);
				StateMachine.Instance.SendMessage(StateMachineStatus.Communication_ReadMemory_For_Setting);
				break;

			case Page.Maintenance:
				StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV);
				break;

			default:
				break;
		}
	}

	/// <summary>
	/// STATEMACHINE COMMUNICATION METHOD FOR READ_MEMORY_SETTING
	/// </summary>
	/// <returns></returns>
	private bool Communication_ReadMemory_For_Setting_Method()
	{
		int recvLen = -1;
		Status = StateMachineStatus.Communication_ReadMemory_For_Setting;

		//SUN MODE BRIGHTNESS
		BLEComModel.SettingCmd(VdiCommand.ReadMemory_SunModeBrightness);
		recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_READ_WRITE_MEMORY);
        if (recvLen > 0)
		{
			//Status = StateMachineStatus.None;
			var st_rwmVals = mDataModel.ReadMemory_SunModeBrightnessValue;
            SettinggViewController.Instance.SetSunModeSeekbarValue(st_rwmVals);
			
			Debug.WriteLine("ReadMemory_SunModeBrightness Cmd OK. , ReadValue: {0}", st_rwmVals);
		}
		else
			return false;

		//NIGHT MODE BRIGHTNESS
		BLEComModel.SettingCmd(VdiCommand.ReadMemory_NightModeBrightness);
		recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_READ_WRITE_MEMORY);
		if (recvLen > 0)
		{
			//Status = StateMachineStatus.None;
			var st_rwmVals = mDataModel.ReadMemory_NightModeBrightnessValue;
            SettinggViewController.Instance.SetNightModeSeekbarValue(st_rwmVals);
			
			Debug.WriteLine("ReadMemory_NightModeBrightnessValue Cmd OK. , ReadValue: {0}", st_rwmVals);
		}
		else
			return false;

		//VALVE MODE NORMAL CLOSE/OPEN
		BLEComModel.SettingCmd(VdiCommand.ReadMemory_ValveMode);
		recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_READ_WRITE_MEMORY);
		if (recvLen > 0)
		{
			//Status = StateMachineStatus.None;
			var st_rwmVals = mDataModel.ReadMemory_ValveNormalOpenCloseMode;
			bool value = st_rwmVals > 0 ? true : false;
            SettinggViewController.Instance.SetValveSwitchChecked(value);

			
			Debug.WriteLine("ReadMemory_ValveModeValue Cmd OK. , ReadValue: {0}", st_rwmVals);
		}
		else
			return false;

		//AUTO MODE1 RPM
		BLEComModel.SettingCmd(VdiCommand.ReadMemory_AutoMode1RPM);
		recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_READ_WRITE_MEMORY);
		if (recvLen > 0)
		{
			//Status = StateMachineStatus.None;
			var st_rwmVals = mDataModel.ReadMemory_ValveAutoMode1RpmValue;

            //SettinggViewController.Instance.SetAutoMode1RpmText(st_rwmVals);

            SettinggViewController.Instance.Model_1._RPM = st_rwmVals;
            SettinggViewController.Instance.SetAutoMode1RpmText();
			
			Debug.WriteLine("ReadMemory_ValveAutoMode1RpmValue Cmd OK. , ReadValue: {0} rpm", st_rwmVals);
		}
		else
			return false;

		//AUTO MODE1 TPS
		BLEComModel.SettingCmd(VdiCommand.ReadMemory_AutoMode1TPS);
		recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_READ_WRITE_MEMORY);
		if (recvLen > 0)
		{
			//Status = StateMachineStatus.None;
			var st_rwmVals = mDataModel.ReadMemory_ValveAutoMode1TpsValue;
            SettinggViewController.Instance.Model_1._TPS = st_rwmVals;
            SettinggViewController.Instance.SetAutoMode1TpsText();
			//SettinggViewController.Instance.SetAutoMode1TpsText(st_rwmVals);

			Debug.WriteLine("ReadMemory_ValveAutoMode1TpsValue Cmd OK. , ReadValue: {0} %", st_rwmVals);
		}
		else
			return false;

		//AUTO MODE1 DELAY TIME
		BLEComModel.SettingCmd(VdiCommand.ReadMemory_AutoMode1DelayTime);
		recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_READ_WRITE_MEMORY);
		if (recvLen > 0)
		{
			//Status = StateMachineStatus.None;
			var st_rwmVals = mDataModel.ReadMemory_ValveAutoMode1DelayTime;
            //SettinggViewController.Instance.SetAutoMode1DelayTimeText(st_rwmVals);
            SettinggViewController.Instance.Model_1._DELAYTIME = st_rwmVals;
            SettinggViewController.Instance.SetAutoMode1DelayTimeText();
			
			Debug.WriteLine("ReadMemory_ValveAutoMode1DelayTime Cmd OK. , ReadValue: {0} ms", st_rwmVals);
		}
		else
			return false;
        
		//AUTO MODE2 RPM
		BLEComModel.SettingCmd(VdiCommand.ReadMemory_AutoMode2RPM);
		recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_READ_WRITE_MEMORY);
		if (recvLen > 0)
		{
			//Status = StateMachineStatus.None;

			var st_rwmVals = mDataModel.ReadMemory_ValveAutoMode2RpmValue;
            SettinggViewController.Instance.Model_2._RPM = st_rwmVals;
            SettinggViewController.Instance.SetAutoMode2RpmText();
            //SettinggViewController.Instance.SetAutoMode2RpmText(st_rwmVals);
			
			Debug.WriteLine("ReadMemory_ValveAutoMode2RpmValue Cmd OK. , ReadValue: {0} rpm", st_rwmVals);
		}
		else
			return false;

		//AUTO MODE2 TPS
		BLEComModel.SettingCmd(VdiCommand.ReadMemory_AutoMode2TPS);
		recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_READ_WRITE_MEMORY);
		if (recvLen > 0)
		{
			//Status = StateMachineStatus.None;
			var st_rwmVals = mDataModel.ReadMemory_ValveAutoMode2TpsValue;
            //SettinggViewController.Instance.SetAutoMode2TpsText(st_rwmVals);
            SettinggViewController.Instance.Model_2._TPS = st_rwmVals;
            SettinggViewController.Instance.SetAutoMode2TpsText();
			Debug.WriteLine("ReadMemory_ValveAutoMode2TpsValue Cmd OK. , ReadValue: {0} %", st_rwmVals);
		}
		else
			return false;

		//AUTO MODE2 DELAY TIME
		BLEComModel.SettingCmd(VdiCommand.ReadMemory_AutoMode2DelayTime);
		recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_READ_WRITE_MEMORY);
		if (recvLen > 0)
		{
			//Status = StateMachineStatus.None;
			var st_rwmVals = mDataModel.ReadMemory_ValveAutoMode2DelayTime;
            SettinggViewController.Instance.Model_2._DELAYTIME = st_rwmVals;
            SettinggViewController.Instance.SetAutoMode2DelayTimeText();
            //SettinggViewController.Instance.SetAutoMode2DelayTimeText(st_rwmVals);
			
			Debug.WriteLine("ReadMemory_ValveAutoMode2DelayTime Cmd OK. , ReadValue: {0} ms", st_rwmVals);
		}
		else
			return false;

		//AUTO MODE3 RPM
		BLEComModel.SettingCmd(VdiCommand.ReadMemory_AutoMode3RPM);
		recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_READ_WRITE_MEMORY);
		if (recvLen > 0)
		{
			//Status = StateMachineStatus.None;
			var st_rwmVals = mDataModel.ReadMemory_ValveAutoMode3RpmValue;
            SettinggViewController.Instance.Model_3._RPM = st_rwmVals;
            SettinggViewController.Instance.SetAutoMode3RpmText();
            //SettinggViewController.Instance.SetAutoMode3RpmText(st_rwmVals);
			
			Debug.WriteLine("ReadMemory_ValveAutoMode3RpmValue Cmd OK. , ReadValue: {0} rpm", st_rwmVals);
		}
		else
			return false;

		//AUTO MODE3 TPS
		BLEComModel.SettingCmd(VdiCommand.ReadMemory_AutoMode3TPS);
		recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_READ_WRITE_MEMORY);
		if (recvLen > 0)
		{
			//Status = StateMachineStatus.None;
			var st_rwmVals = mDataModel.ReadMemory_ValveAutoMode3TpsValue;
            SettinggViewController.Instance.Model_3._TPS = st_rwmVals;
            SettinggViewController.Instance.SetAutoMode3TpsText();
            //SettinggViewController.Instance.SetAutoMode3TpsText(st_rwmVals);
		
			Debug.WriteLine("ReadMemory_ValveAutoMode3TpsValue Cmd OK. , ReadValue: {0} %", st_rwmVals);
		}
		else
			return false;

		//AUTO MODE3 DELAY TIME
		BLEComModel.SettingCmd(VdiCommand.ReadMemory_AutoMode3DelayTime);
		recvLen = BLEComModel.ComProcess(CommMode.COMMUNICATION_READ_WRITE_MEMORY);
		if (recvLen > 0)
		{
			//Status = StateMachineStatus.None;
			var st_rwmVals = mDataModel.ReadMemory_ValveAutoMode3DelayTime;
            SettinggViewController.Instance.Model_3._DELAYTIME = st_rwmVals;
            SettinggViewController.Instance.SetAutoMode3DelayTimeText();
            //SettinggViewController.Instance.SetAutoMode3DelayTimeText(st_rwmVals);
			
			Debug.WriteLine("ReadMemory_ValveAutoMode3DelayTime Cmd OK. , ReadValue: {0} ms", st_rwmVals);
		}
		else
			return false;

		return true;

	}

    /// <summary>
    /// 進入狀態機前的設定
    /// </summary>
    public override void SettingPreCondition()
    {
#if ENABLE_READ_HARDWARE_FUNCTION
        gestureTimer.Start();
#endif

#if ENABLE_TEST_PRESENT_FUNCTION
        testPresentTimer.Start();
#endif

        if (UseCommunicationMode)
        {
            //是否需要提醒藍芽有無開啟
            if (isNeedRemindToOpenBLE)
            {
                //如果藍芽沒有開啟(提醒直到開啟為止)
                //int re = 0;
                while (true)
                {
                    
                    Thread.Sleep(100);

                    if (BLEComModel.BleState == CBCentralManagerState.PoweredOn)
                    {
                        Debug.WriteLine("SettingPreCondition() - " + "BLEComModel.BleState == PoweredOn");
                        break;
                    }
                    else
                    {
                        Debug.WriteLine("SettingPreCondition() - " + "BLEComModel.BleState == PowerOff or else");
                        //++re;
                        //if (UIModel != null && re == 3)
                        if(UIModel != null )
                        {
                            if(!UIModel.isUserEnterBackground)
                                UIModel.ShowCheckBTSwitchAlertDialog();
                            Debug.WriteLine("SettingPreCondition() - " + "ShowCheckBTSwitchAlertDialog()");
                        }
                    }

                }

                //關閉提醒視窗
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {

                    if (UIModel != null)
                    {
                        StateMachine.UIModel.CloseBlePowerOffAlert();
                        Debug.WriteLine("SettingPreCondition() - " + "CloseBlePowerOffAlert()");
                    }

                });
            }

            //是否啟用多裝置模式
            if (IsUsingChoiceMultipleDeviceMode)
            {
                //如果檢查到未儲存裝置名稱，彈出列舉多個裝置的對話框
                if (storedBleDeviceName.Equals(""))
                {
                    //Thread.Sleep(1000);

                    DispatchQueue.MainQueue.DispatchAsync(() =>
                    {
                        if (HomeViewController.Instance != null)
                            HomeViewController.Instance.BleDeviceListDialog.Show();
                    });

                    while (true)
                    {
                        if (BLEComModel.BleState == CBCentralManagerState.PoweredOn)
                        {
                            BLEComModel.StartScan();
                            break;
                        }
                        Thread.Sleep(100);
                    }

                    DateTime now = DateTime.Now;
                    Debug.Print("START SCAN" + string.Format(" : {0} {1}.{2}\r",
                                         now.ToLongDateString(),
                                         now.ToLongTimeString(),
                                                             now.Millisecond.ToString()));
                }
                else
                {
                    if (UseCommunicationMode )
                    {
                        if (UIModel != null && BLEComModel.mCBBLECentral != null)
                            UIModel.ShowProgressDialog();
                    }
                }
            }
            else
            {
                //if (UseCommunicationMode)
                //UIModel.ShowProgressDialog();
            }

            //是否啟用多裝置模式
            if (IsUsingChoiceMultipleDeviceMode)
            {
                //存儲裝至名稱不為空，直接進入連線
                if (!storedBleDeviceName.Equals(""))
                {
                    BLEComModel.BindBLEDevice(storedBleDeviceName);
                    //var msg = mContext.Resources.GetString(Resource.String.BLE_Scanning) + StoredBleDeviceName + "...";
                    //UIModel.SetProgerssDialogMessage(msg);
                    Status = StateMachineStatus.Device_Init;
                    Enqueue(Status);
                }
            }
            else
            {
                //var msg = mContext.Resources.GetString(Resource.String.BLE_Scanning) + StoredBleDeviceName + "...";
                //UIModel.SetProgerssDialogMessage(msg);
                Status = StateMachineStatus.Device_Init;
                Enqueue(Status);
            }
        }

    }


	private bool IsHardwareRunOK = true;
    /// <summary>
    /// 狀態機輪詢入口點
    /// </summary>
    public override void DoSomething()
    {

        //Console.WriteLine("Threads Count : " + ThreadCounts);
        try
        {
			
			if (UseCommunicationMode)
			{
				StateMachineProcess();
				Thread.Sleep(20);
			}

            //if (UseGestureMode)
            //{
            //	//if(IsCommunicationInited)
            //	UseGesture2ChangePage();
            //}

#if ENABLE_READ_HARDWARE_FUNCTION
            //讀取硬體狀態 & 手勢狀態
            if (BLEComModel != null)
            {
				if (IsCommunicationInited && gestureTimer.ElapsedMilliseconds > gestureTimeout)//當硬體狀態改變的時候
                {
                    BLEComModel.ReadGestureValue();
					//Console.WriteLine(StateMachine.DataModel.GestureValue);
                    UseGesture2ChangePage();
					IsHardwareRunOK = mDataModel.HardwareStatus;

					if (IsHardwareRunOK == false)
						UIModel.Instance.ShowECUCommunicationfailed();
					else
						UIModel.Instance.CloseECUCommunicationfailed();
					gestureTimer.Reset();
					gestureTimer.Restart();
					//Console.WriteLine("HardwareStatus : {0}",mDataModel.HardwareStatus);
					Thread.Sleep(20);
                }
            }
#endif

#if ENABLE_TEST_PRESENT_FUNCTION
            //掛住
            if (BLEComModel.IsConnected && BLEComModel.IsSendInitCommand)
            {
                if (testPresentTimer.ElapsedMilliseconds > testPresentTimeout)
                {
                    StateMachine.Instance.SendMessage(StateMachineStatus.Communication_TestPresent);
                    testPresentTimer.Reset();
                    testPresentTimer.Restart();
                    Thread.Sleep(20);
                }
            }
#endif

            //獲取藍芽硬體資訊
            if (mDataModel != null && UseCommunicationMode && !IsReadOk)
            {
                if (mDataModel.DeviceInfo_SerialNumber == null)
                {
                    if (BLEComModel != null)
                        BLEComModel.ReadSerialNumber();
                }

                if (mDataModel.DeviceInfo_FirmwareRevistion == null)
                {
                    if (BLEComModel != null)
                        BLEComModel.ReadFirmwareRevision();
                }

                if (mDataModel.DeviceInfo_SoftwareRevision == null)
                {
                    if (BLEComModel != null)
                        BLEComModel.ReadSoftwareRevision();
                }

                if (mDataModel.DeviceInfo_SerialNumber != null &&
                  mDataModel.DeviceInfo_FirmwareRevistion != null &&
                  DataModel.DeviceInfo_SoftwareRevision != null)
                    IsReadOk = true;
            }

            Thread.Sleep(50);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
        }
    }

    private void ShowThreadInfo(Thread t)
    {
        Console.WriteLine("Thread id = {0} , name = {1}", t.ManagedThreadId, t.Name);
    }

    private int ThreadCounts
    {
        get {

            ////int workThreads = 0;
            ////int completionThreads = 0;
            ////ThreadPool.GetAvailableThreads(out workThreads, out completionThreads);
            ////return workThreads;

            //var Threads = Java.Lang.Thread.AllStackTraces.Keys;
            //int activeCount = 0;
            //foreach (var t in Threads)
            //{
            //    if (t.GetState() == Java.Lang.Thread.State.Runnable)
            //    {
            //        activeCount++;
            //        //ShowThreadInfo(t);
            //    }
            //}
            //return activeCount;


            ////return ((IEnumerable)System.Diagnostics.Process.GetCurrentProcess().Threads)
            ////        .OfType<System.Diagnostics.ProcessThread>()
            ////        .Where(t => t.ThreadState == System.Diagnostics.ThreadState.Running)
            ////        .Count();

            return Process.GetCurrentProcess().Threads.Count;
        }
    }

    public void BLESwitch(bool sw)
    {
        IsUsingChoiceMultipleDeviceMode = sw;
        UseCommunicationMode = sw;
        if (!sw)
        {
            UIModel.CloseProgressDialog();
            UIModel.simUI();
            BLEComModel.StopScan();
            BLEComModel.Stop();
        }
    }



    /// <summary>
    /// 獲取當前頁面位置
    /// </summary>
    /// <returns></returns>
    private int GetCurrentPagePos()
    {
        if (UIModel == null)
            return 0;

        int posIndex = 0;
        var currentPage = UIModel.CurrentPage;
        foreach (var page in pages)
        {
            if (page.Equals(currentPage))
                return posIndex;
            posIndex ++;
        }

        return 0;
    }


    /// <summary>
    /// 上一個手勢狀態
    /// </summary>
    GesturePos prevGesturePos = GesturePos.NONE;
    /// <summary>
    /// 目前手勢狀態
    /// </summary>
    GesturePos currentGesturePos = GesturePos.NONE;
    /// <summary>
    /// 要切換的頁面陣列
    /// </summary>
    static readonly Page[] pages = new Page[] { Page.Home, Page.Log, Page.Speed0_100, Page.Speed0_400, Page.LiveData, Page.DTC, Page.Shift, Page.Valve};
    /// <summary>
    /// 目前頁面位置
    /// </summary>
    int pagePos = 0;
    /// <summary>
    /// 最大頁面數
    /// </summary>
    static readonly int pageMaxPos = pages.Length;
    /// <summary>
    /// 利用手勢狀態切換頁面
    /// 手勢狀態 0=無, 1=左, 2=右
    /// 相同狀態不予以切頁，例如要由左再切到右
    /// 訊號需為 0 -> 1 -> 0 -> 2
    /// 訊號若為 0 -> 0 -> 0 -> 0 -> 1 -> 1 -> 1 -> 0 -> 0 -> 2 -> 2 -> 2 -> 2 -> 2
    /// 兩者訊號相同
    /// </summary>
    private void UseGesture2ChangePage()
    {
        currentGesturePos = mDataModel.GestureValue;
        if (prevGesturePos != currentGesturePos)
        {
            prevGesturePos = currentGesturePos;

            pagePos = GetCurrentPagePos();
            switch (currentGesturePos)
            {
                case GesturePos.NONE:
                    break;

                case GesturePos.LEFT:
                    pagePos --;
                    break;

                case GesturePos.RIGHT:
                    pagePos ++;
                    break;

                default:
                    break;
            }

            //限制最大最小值
            if (pagePos <= 0)
                pagePos = 0;
            else if (pagePos >= pageMaxPos)
                pagePos = pageMaxPos;

            //循環重頭來
            if (pagePos >= pageMaxPos)
                pagePos = 0;

            ////@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            ////當pagePos為0時，scroll到最前面，pagePos為dtc頁面時，scroll到最後面
            //if (pagePos <= 0)
            //    UIModel.SlideScrollView2Start();
            //else if (pagePos >= pageMaxPos-3)
            //    UIModel.SlideScrollView2End();

            //mPage = pages[pagePos];

            //DispatchQueue.MainQueue.DispatchAsync(() => { 
            //    switch (mPage)
            //    {
            //        case Page.Home:
            //            UIModel.CurrentPage = Page.Home;
            //            UIModel.SwitchFragment(UIModel.FragmentTable["homeFragment"], "homeFragment");
            //            break;

            //        case Page.Log:
            //            UIModel.CurrentPage = Page.Log;
            //            UIModel.SwitchFragment(UIModel.FragmentTable["logFragment"], "logFragment");
            //            break;

            //        case Page.LiveData:
            //            UIModel.CurrentPage = Page.LiveData;
            //            UIModel.SwitchFragment(UIModel.FragmentTable["liveDataFragment"], "liveDataFragment");
            //            StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV_STOP);
            //            StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV);
            //            break;

            //        case Page.DTC:
            //            UIModel.CurrentPage = Page.DTC;
            //            UIModel.SwitchFragment(UIModel.FragmentTable["dtcFragment"], "dtcFragment");
            //            StateMachine.DataModel.ClearDtcValues();
            //            StateMachine.Instance.RemoveAllMessage();
            //            StateMachine.Instance.SendMessage(StateMachineStatus.Communication_DTC);
            //            break;

            //        case Page.Shift:
            //            UIModel.CurrentPage = Page.Shift;
            //            UIModel.SwitchFragment(UIModel.FragmentTable["shiftFragment"], "shiftFragment");
            //            StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV_STOP);
            //            StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV);
            //            break;

            //        case Page.Valve:
            //            UIModel.CurrentPage = Page.Valve;
            //            UIModel.SwitchFragment(UIModel.FragmentTable["valveFragment"], "valveFragment");
            //            break;

            //        case Page.Speed0_100:
            //            UIModel.CurrentPage = Page.Speed0_100;
            //            UIModel.SwitchFragment(UIModel.FragmentTable["zero2HundredFragment"], "zero2HundredFragment");
            //            StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV_STOP);
            //            break;

            //        case Page.Speed0_400:
            //            UIModel.CurrentPage = Page.Speed0_400;
            //            UIModel.SwitchFragment(UIModel.FragmentTable["zero24HundredFragment"], "zero24HundredFragment");
            //            StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV_STOP);
            //            break;

            //        default:
            //            break;
            //    }
            //});
        }
    }

    /// <summary>
    /// 測試 HorizontalScrollView 左右滑動
    /// </summary>
    private void Test()
    {
        if (!UIModel.IsScrollInEndPos)
        {
            if (UIModel.SlideScrollView2End())
                Console.WriteLine("SLIDE TO END.");
        }
    }

	/// <summary>
	/// 正則表達式變數
	/// 正則表達式變數
	/// </summary>
	System.Text.RegularExpressions.Regex reg1;
    /// <summary>
    /// 檢查字串是否為數字與英文組合
    /// </summary>
    /// <param name="str">要檢測的字串</param>
    /// <returns></returns>
    public bool IsNatural_Number(string str)
    {
        if(reg1 == null)
            reg1 = new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9]+$");
        return reg1.IsMatch(str);
    }


#region TEST
    public void BindView(UITextView uiTextView)
    {
        if (BLEComModel != null && uiTextView != null)
            BLEComModel.BindView(uiTextView);
    }

    public String GetBLEInfo()
    {
        if (BLEComModel == null)
            return null;
        else
            return BLEComModel.GetBLEInfo();
    }
#endregion

	/// <summary>
	/// 回傳DM Value的相關資料
	/// 目前使用的有:
	/// MaxValue, MinValue, MultipleRate
	/// </summary>
	/// <returns>回傳DM Value的相關資料</returns>
	public Dictionary<int, DmData> GetDmValues()
	{
		return DataModel.DmIcmDBHelper.DmValues;
	}
}
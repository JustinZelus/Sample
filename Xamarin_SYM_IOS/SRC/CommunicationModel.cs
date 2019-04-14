using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using System.Threading.Tasks;
using IcmComLib.Communication.BLE.iOSBLE;
using System.Threading;
using IcmLib.Data;
using IcmComLib.Communication.BLE;
using System.Diagnostics;
using CommonLib.IcmLib.Enum.Communication;
using Xamarin_SYM_IOS;
using CoreBluetooth;
using IcmLib.Data.Unpacker;
using static IcmComLib.Communication.BLE.iOSBLE.CBBLEWrapper;
using Xamarin_SYM_IOS.ViewControllers;
using System.Runtime.CompilerServices;
using CommonLib.IcmLib.Communication.BLE;

namespace iPhoneBLE.SRC
{
    public class CommunicationModel
    {
        private Task mComTask = null;
        private CancellationTokenSource mCts = new CancellationTokenSource();
        public CBBLEWrapper mCBBLECentral = null;
        private String mTargetBLEDeviceName = "";
        private byte[] mCmdBuffer = null;
        private DataModel mDataModel = null;
        private static bool IsAlreadGetLvIDs = false;
        public delegate void CustomFuc();
        public CustomFuc CallBackForScanBtn = null;
        public DiscoverBleDeviceCallback BLEDiscoveredDeviceAction = null;

        private List<DtcData> dtcList = new List<DtcData>();
        private readonly Object DTC_OP_LOCKER = new Object();
        private CBCentralManagerState bleState;
        private bool isPowerOff = true;
        private bool isPowerOn = false;
        /// <summary>
        /// 通訊模組建構子
        /// </summary>
        /// <param name="activity">Activity實例</param>
        /// <param name="TargetBLEMacAddr">目標藍牙MAC地址</param>
        public CommunicationModel(String TargetBLEDeviceName)
        {
            mCBBLECentral = new CBBLEWrapper(TargetBLEDeviceName);
            if (AppAttribute.BLE_SCAN_VIEW == AppAttribute.RunningMode.OldScanView)
            {
                mCBBLECentral.IsUseOldScanView(true);
            }
            mTargetBLEDeviceName = TargetBLEDeviceName;
            mCBBLECentral.BLEPowerStateAction = BLEPowerState;
            mCBBLECentral.discoverBleDeviceCallback = TriggerBLEDiscoveredDeviceAction;
        }

        private void TriggerBLEDiscoveredDeviceAction(string deviceName)
        {
            if (BLEDiscoveredDeviceAction != null)
                BLEDiscoveredDeviceAction(deviceName);
        }
      

        public CBCentralManagerState DetectBleState()
        {
            return mCBBLECentral.UpdatedState();
        }

        private void BLEPowerState(CBCentralManagerState state)
        {
            Debug.WriteLine("CBCentralManagerState : " + state);

            this.bleState = state;
            //if (state == CBCentralManagerState.PoweredOff)
            //{
            //    isPowerOff = true;
            //    isPowerOn = false;

            //    if (AppAttribute.BLE_SCAN_VIEW == AppAttribute.RunningMode.OldScanView)
            //    {
            //        StateMachine.Instance.RemoveAllMessage();
            //        StateMachine.Instance.SendMessage(StateMachineStatus.BlePowerOff);
            //    }
            //}
            //else if (state == CBCentralManagerState.PoweredOn)
            //{
            //    isPowerOff = false;
            //    isPowerOn = true;

            //    if (AppAttribute.BLE_SCAN_VIEW == AppAttribute.RunningMode.OldScanView)
            //    {
            //        StateMachine.Instance.RemoveAllMessage();
            //        //如果有序號即自動連線，反之則進行掃描
            //        if (StateMachine.IsAutoConnect)
            //        {

            //            //StateMachine.UIModel.ShowProgressDialog();
            //            StateMachine.Instance.SendMessage(StateMachineStatus.BlePowerOn);
            //        }
            //        else
            //            StateMachine.Instance.SendMessage(StateMachineStatus.BleScan);
            //    }
               

            //}
        }

        public async Task<bool> Init()
        {
            if (mCBBLECentral == null)
                return false;
            else
                return await mCBBLECentral.Init();
        }

        public bool IsPowerOff
        {
            get
            {
                return isPowerOff;
            }
            set
            {
                isPowerOff = value;
            }
        }

        public bool IsPowerOn
        {
            get
            {
                return isPowerOn;
            }
            set
            {
                isPowerOn = value;
            }
        }

        public CBCentralManagerState BleState
        {
            get
            {
                return bleState;
            }
        }

        /// <summary>
        /// 讀取手勢數值
        /// </summary>
        /// <returns></returns>
        public bool ReadGestureValue()
        {
            if (mCBBLECentral != null)
            {
                var result = mCBBLECentral.ReadGestureCharacteristicAsync().Result;
                //Console.WriteLine("CUSTOM VALUE : {0}",BitConverter.ToString(mCBBLECentral.GetGetsuteValue()));
                return result;
            }
            else
                return false;
        }

        /// <summary>
        /// 綁定資料模組
        /// </summary>
        /// <param name="dataModel"></param>
        public void BindDataModel(DataModel dataModel)
        {
            mDataModel = dataModel;
            mCBBLECentral.BindLVUnpacker(dataModel.VdiUnpacker);
            mCBBLECentral.BindDtcUnpacker(dataModel.DtcUnpacker);
            mCBBLECentral.BindIPEReadWriteMemoryValue(dataModel.IPEReadWriteMemoryValue);
            mCBBLECentral.BindDeviceInfo(dataModel.BleDeviceInfo);
        }

    
        /// <summary>
        /// 綁定裝置
        /// </summary>
        /// <param name="BleMacAddr">藍牙裝置名稱</param>
        public void BindBLEDevice(String TargetBLEDeviceName)
        {
            mTargetBLEDeviceName = TargetBLEDeviceName;
            if (mCBBLECentral != null)
                mCBBLECentral.BindDeviceName(mTargetBLEDeviceName);
        }

        public void BindBLEDPeripheral(string peripheralName)
        {
            if (mCBBLECentral != null)
                mCBBLECentral.BindPeripheral(peripheralName);
        }

        private int ThreadCounts
        {
            get
            {
                return Process.GetCurrentProcess().Threads.Count;
            }
        }


        /// <summary>
        /// 開始函數
        /// </summary>
        public void Start()
        {
            if (mComTask == null)
            {
                messageQueue.Enqueue(CommMode.CONNECT);
                mComTask = Task.Run(() =>
                {
                    while (true)
                    {
                        if (mCts.IsCancellationRequested)
                        {
                            ClearMessageQueue();
                            mCts.Token.ThrowIfCancellationRequested();
                            mComTask = null;
                            break;
                        }
                        ComProcess();
                        Thread.Sleep(20);
                    }
                }, mCts.Token);
            }
        }

        /// <summary>
        /// 停止涵數
        /// </summary>
        public void Stop()
        {
            mCts.Cancel();
        }


        int recvLen = 0;
        byte[] resultVal = null;
        int lvIdCount = 0;
        byte[] sendCmd;
        byte[] recvCmd;
        byte[] lvIdCmd = new byte[1];
        int logBlockCount = 0;
        /// <summary>
        /// 訊息佇列
        /// </summary>
        private Queue<CommMode> messageQueue = new Queue<CommMode>();

        /// <summary>
        /// SETTING MESSAGE QUEUE
        /// </summary>
        /// <param name="msg">COMMUNICATION MODE MSG</param>
        public bool SetMessageQueue(CommMode msg)
        {
            if (messageQueue == null)
                return false;

            messageQueue.Enqueue(msg);
            return true;
        }

        /// <summary>
        /// CHECK MESSAGE IS EXIST OR NOT
        /// </summary>
        /// <param name="msg">COMMUNICATION MODE MSG</param>
        /// <returns></returns>
        public bool MessageIsExist(CommMode msg)
        {
            if (messageQueue == null)
                return false;
            return messageQueue.Contains(msg);
        }

        /// <summary>
        /// CLEAR MESSSAGE QUEUE
        /// </summary>
        public void ClearMessageQueue()
        {
            if (messageQueue == null)
                return;
            messageQueue.Clear();
        }

        /// <summary>
        /// 是否連接
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if (mCBBLECentral == null)
                    return false;
                if (mCBBLECentral.Peripheral == null)
                    return false;
                return mCBBLECentral.IsConnected;
            }
        }

        public void StopScanThenStartScan()
        {
            if (mCBBLECentral == null)
                return;
            mCBBLECentral.StartScanning();
        }

        /// <summary>
        /// 設定要傳送的指令
        /// </summary>
        /// <param name="cmd">指令</param>
        public void SettingCmd(byte[] cmd)
        {
            mCmdBuffer = (byte[])cmd.Clone();
        }

        /// <summary>
        /// Get INFO Infomatiomn
        /// </summary>
        public byte[] InfoValue
        {
            get
            {
                if (mCBBLECentral == null)
                    return null;

                return mCBBLECentral.GetInfoValue();
            }
        }

        public void ClearInfoValue()
        {
            if (mCBBLECentral == null)
                return;
            mCBBLECentral.InitInfoValue();
        }

        /// <summary>
        /// Get LV DATA Infomation
        /// </summary>
        public byte[] LvDataValue
        {
            get
            {
                if (mCBBLECentral == null)
                    return null;

                return mCBBLECentral.GetLvDataValue();
            }
        }

        /// <summary>
        /// Get LV DATA ITEM Infomation
        /// </summary>
        public byte[] LvDataItemValue
        {
            get
            {
                if (mCBBLECentral == null)
                    return null;

                return mCBBLECentral.GetLvIdValue();
            }
        }

        /// <summary>
        /// Get LV DATA ITEM Count
        /// </summary>
        public int LvDataItemCount
        {
            get
            {
                if (mCBBLECentral == null)
                    return 0;

                return mCBBLECentral.GetLvIdCount();
            }
        }

        /// <summary>
        /// Get Log Value
        /// </summary>
        public byte[] LogValue
        {
            get
            {
                if (mCBBLECentral == null)
                    return null;

                return mCBBLECentral.GetLogValue();
            }
        }

        /// <summary>
        /// Get ReadWriteMemory Value
        /// </summary>
        public byte[] ReadWriteMemoryValue
        {
            get
            {
                if (mCBBLECentral == null)
                    return null;

                return mCBBLECentral.GetRWMemoryValue();
            }
        }

        /// <summary>
        /// Get DTC VALUE
        /// </summary>
        public byte[] DtvValue
        {
            get
            {
                return mCBBLECentral.GetDTCValue();
            }
        }

        /// <summary>
        /// Get DTC Count
        /// </summary>
        public int DtcCount
        {
            get
            {
                return mCBBLECentral.GetDTCBlocksCount();
            }
        }

        public bool IsSendInitCommand = false;

        #region 新的通訊函數
        /// <summary>
        /// 通訊函數
        /// </summary>
        /// <returns></returns>
        public int ComProcess()
        {
            try
            {

                recvLen = 0;
                if (messageQueue.Count > 0)
                {
                    recvLen = ComProcess(messageQueue.Dequeue());
                }
                return recvLen;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }


        /// <summary>
        /// await 10s after start scan , then stop & do UI callback
        /// </summary>
        async void RestoreScanBtn()
        {
            await Task.Delay(10000);
            if (mCBBLECentral.IsScanning && ScanViewController.Instance.IsShowing)
            {
                mCBBLECentral.StopScanning();
                if (CallBackForScanBtn != null)
                {
                    CallBackForScanBtn();
                }

            }
        }
        /// <summary>
        /// Scan 10s then stop
        /// </summary>
        public void ScanManually()
        {
            if (!mCBBLECentral.IsScanning)
            {
                //if (ScanViewController.Instance != null)
                //{
                //	ScanViewController.Instance.CleanBleNames();
                //}
                //async,a callback for ui
                RestoreScanBtn();
                mCBBLECentral.StartScanning();
                return;
            }
        }

        public void StartScan()
        {
            if (mCBBLECentral == null)
            {
                //WriteLog(filePath);
                Debug.WriteLine("mCBBLECentral =  NULL");

                return;
            }
            else
            {
                if (mCBBLECentral.IsScanning)
                {
                    Debug.WriteLine("Detect cm scanning. Close it !");
                    mCBBLECentral.StopScanning();
                }
            }

            Debug.WriteLine("mCBBLECentral = " + mCBBLECentral);
            mCBBLECentral.StartScanning();
        }

        public void StopScan()
        {
            if (mCBBLECentral != null)
                mCBBLECentral.StopScanning();
        }


        private readonly Object connectLocker = new Object();
        public Object ConnectLocker { get => connectLocker; }

        private bool _IsInConnect = false;

        public bool IsInConnect
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => _IsInConnect;

            [MethodImpl(MethodImplOptions.Synchronized)]
            set => _IsInConnect = value;

        }

        /// <summary>
        /// 通訊進程
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public int ComProcess(CommMode msg)
        {
            try
            {
                recvLen = 0;
                switch (msg)
                {
                    case CommMode.NONE:
                        break;

                    case CommMode.BLE_INIT:
                        {
                            if (mCBBLECentral.Init().Result)
                            {
                                Debug.WriteLine("INIT OK.");

                                //Mode = CommMode.NONE;
                                return 1;
                            }
                        }
                        return -1;

                    case CommMode.SCAN:
                        {
                            mCBBLECentral.StartScanning();
                            //RestoreScanBtn();
                            //Console.WriteLine("SCANNNNNNNNNNNNNNNNNnnnnn");
                            //Mode = CommMode.NONE;
                        }
                        break;

                    case CommMode.STOP_SCAN:
                        {
                            mCBBLECentral.StopScanning();
                            //Mode = CommMode.NONE;
                        }
                        break;

                    case CommMode.CONNECT:
                        {
                            lock (connectLocker)
                            {
                                if (mCBBLECentral.IsConnected)
                                    return 1;

                                //mBLECentral.StopScanning();
                                if (mCBBLECentral.Peripheral != null && !mCBBLECentral.IsConnected &&
                                    !IsInConnect)
                                {
                                    IsInConnect = true;
                                    //if (mCBBLECentral.Peripheral == null && !mCBBLECentral.IsConnected)
                                    //{
                                    //  mCBBLECentral.BeginScanningForDevices();
                                    //  break;
                                    //}

                                    if (mCBBLECentral.Connect())
                                    {
                                        Debug.WriteLine("======= > Connect Ok.");
                                        return 1;
                                    }
                                    else
                                    {
                                        Debug.WriteLine("======= > Connect Failed.");
                                        return -1;
                                    }

                                }

                                //return 1;
                                return -1;
                            }
                        }

                    case CommMode.DISCONNECT:
                        {
                            Disconnect();
                            //Mode = CommMode.NONE;
                        }
                        break;

                    case CommMode.COMMUNICATION_INIT:
                        {
                            if (mCBBLECentral.IsConnected)
                            {
                                IsAlreadGetLvIDs = false;
                                //VdiUnpacker.ClearLVIDs();
                                //mDataModel.ClearSupportedEcuLvIds();

                                VdiCommand.GetDMItemMultiBlockIdCmd[0] = 0;
                                VdiCommand.GetDtcItemMultiBlockIdCmd[0] = 0;
                                mCBBLECentral.Setting(ProtocolType.SETTING);
                                resultVal = mCBBLECentral.SendAndReadData(VdiCommand.InitCmd);
                                recvLen = resultVal == null ? -1 : resultVal.Length;
                                if (recvLen > 0)
                                {
                                    IsSendInitCommand = true;
                                    return recvLen;
                                }
                            }
                            return -1;
                        }

                    case CommMode.COMMUNICATION_RESET_MEMORY:
                        {
                            if (mCBBLECentral.IsConnected)
                            {
                                mCBBLECentral.Setting(ProtocolType.SETTING);
                                resultVal = mCBBLECentral.SendAndReadData(VdiCommand.eraseMemoryCmd);
                                recvLen = resultVal == null ? -1 : resultVal.Length;
                                if (recvLen > 0)
                                {
                                    return recvLen;
                                }
                            }
                            return -1;
                        }


                    case CommMode.COMMUNICATION_ACT_VALVE:
                        {
                            //@@@@@@@@@@@@@@@@@@@@@@@@@@
                            //測試用
                            //int valveVal = 3;
                            int valveVal = mDataModel.ValveValue;
                            if (valveVal != 0)
                            {
                                mCBBLECentral.Setting(ProtocolType.SETTING);
                                VdiCommand.ActValveControlCmd[2] = (byte)valveVal;
                                resultVal  = mCBBLECentral.SendAndReadData(VdiCommand.ActValveControlCmd);
                                recvLen = resultVal == null ? -1 : resultVal.Length;
                                if (recvLen > 0)
                                {
                                    //RESTORE ACT VALVE VALUE
                                    if (mCBBLECentral.SettingResult == true)
                                    {
                                        VdiCommand.ActValveControlCmd[2] = 0;
                                        return recvLen;
                                    }
                                    else
                                    {
                                        return -1;
                                    }
                                }

                            }
                            return -1;
                        }


                    case CommMode.COMMUNICATION_LV_ID:
                        {
                            //if (!IsAlreadGetLvIDs)
                            //@@@@@@@@@@@@@@@@@@@@@@@@@@
                            //測試用
                            if (true)
                            {
                                mCBBLECentral.Setting(ProtocolType.DATA_MONITOR_ITEM);
                                resultVal = mCBBLECentral.SendAndReadData(VdiCommand.GetDMItemMultiBlockIdCmd);
                                recvLen = resultVal == null ? -1 : resultVal.Length;
                                if (recvLen > 0)
                                {
                                    lvIdCount = mCBBLECentral.GetLvIdCount();
                                }
                                for (int i = 0; i < lvIdCount; i++)
                                {

                                    VdiCommand.GetDMItemMultiBlockIdCmd[0]++;
                                    resultVal = mCBBLECentral.SendAndReadData(VdiCommand.GetDMItemMultiBlockIdCmd);
                                    recvLen = resultVal == null ? -1 : resultVal.Length;

                                    if (recvLen > 0)
                                    {
                                        //VdiUnpacker.Add((byte[])BLECentral.GetLvIdValue().Clone());
                                        mDataModel.CollectSupportedEcuLvIds(mCBBLECentral.GetLvIdValue());
                                    }
                                    else
                                    {
                                        if (i <= 1)
                                            i = 1;
                                        else
                                            i--;
                                    }
                                }
                                IsAlreadGetLvIDs = true;
                                VdiCommand.GetDMItemMultiBlockIdCmd[0] = 0;
                                //Mode = CommMode.NONE;
                                //liveDataFragment.SetValues(VdiUnpacker.AllIDValues, DmIcmDBHelper.Values);
                                //liveDataFragment.SetValues(mDataModel.AllEcuLvNames);
                                return recvLen;
                            }

                            return -1;
                        }

                    case CommMode.COMMUNICATION_GESTURE:
                        {
                            ////@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                            //if (mCBBLECentral.SetNotifyCh(IcmUuid.GESTURE_READ_UUID, true))
                            //    return 1;
                            //else
                            return -1;
                        }

                    case CommMode.COMMUNICATION_LV:
                        {


                            //mDataModel.ClearLvDatas();
                            mDataModel.SettingSelectedLvIds(mCmdBuffer);
                            mCBBLECentral.Setting(ProtocolType.DATA_MONITOR);
                            resultVal = mCBBLECentral.SendAndReadData((byte[])mCmdBuffer.Clone());
                            recvLen = resultVal == null ? -1 : resultVal.Length;
                            if (recvLen > 0)
                                return recvLen;
                            else
                                return -1;

                        }


                    case CommMode.COMMUNICATION_READ_LV_VALUE:
                        {
                            var lvDataValue = mCBBLECentral.GetLvDataValue();
                            //VdiUnpacker.Unpack(lvDataValue);
                            mDataModel.UnpackLvDatas(lvDataValue);
                            SetMessageQueue(CommMode.COMMUNICATION_READ_LV_VALUE);
                        }
                        break;

                    case CommMode.COMMUNICATION_LV_STOP:
                        {
                            mDataModel.ClearLvDatas();
                            mCBBLECentral.Setting(ProtocolType.DATA_MONITOR);
                            resultVal = mCBBLECentral.SendAndReadData(VdiCommand.StopLVCmd);
                            recvLen = resultVal == null ? -1 : resultVal.Length;
                            if (recvLen > 0)
                            {
                                //Mode = CommMode.NONE;
                                return recvLen;
                            }
                            return 1;
                        }
                    case CommMode.COMMUNICATION_DTC:
                        {

                            lock (DTC_OP_LOCKER)
                            {
                                mCBBLECentral.Setting(ProtocolType.SETTING);
                                resultVal = mCBBLECentral.SendAndReadData(VdiCommand.DtcSettingCmd);
                                recvLen = resultVal == null ? -1 : resultVal.Length;

                                if (recvLen <= 0)
                                    return -1;

                                if (mCBBLECentral.GetSettingValue()[0] != 1)
                                    return -1;

                                mCBBLECentral.Setting(ProtocolType.DTC);
                                //LABEL REGET DTC COUNT
                                Debug.WriteLine("***** DTC COMMUNICATION START *****");
                            ReGetDtcCount:
                                //如果不在DTC頁面，離開DTC程序
                                if (StateMachine.UIModel.Instance.CurrentPage != Page.DTC)
                                {
                                    StateMachine.Instance.RemoveAllSpecificMessage(StateMachineStatus.Communication_DTC);
                                    return 1;
                                }

                                resultVal = mCBBLECentral.SendAndReadData(VdiCommand.GetDtcItemMultiBlockIdCmd);
                                recvLen = resultVal == null ? -1 : resultVal.Length;

                                if (recvLen > 0)
                                {
                                    int dtcBlocks = mCBBLECentral.GetDTCBlocksCount();
                                    if (dtcBlocks == 0x00)
                                    {
                                        //StateMachine.Instance.SendMessage(StateMachineStatus.Communication_DTC);
                                        return -1;
                                    }

                                    if (dtcBlocks == 0xFF)
                                        goto ReGetDtcCount;
                                    
                                    for (int i = 1; i <= dtcBlocks; i++)
                                    {   
                                        Debug.WriteLine("***** 如果不在DTC頁面，離開DTC程序 *****");
                                        //如果不在DTC頁面，離開DTC程序
                                        if (StateMachine.UIModel.Instance.CurrentPage != Page.DTC)
                                        {
                                            StateMachine.Instance.RemoveAllSpecificMessage(StateMachineStatus.Communication_DTC);
                                            return 1;
                                        }

                                        if (dtcBlocks == 0xFF)
                                        {
                                            i--;
                                            if (i < 1)
                                                i = 0;
                                            continue;
                                        }

                                        VdiCommand.GetDtcItemMultiBlockIdCmd[0] = (byte)i;
                                        resultVal = mCBBLECentral.SendAndReadData(VdiCommand.GetDtcItemMultiBlockIdCmd);
                                        recvLen = resultVal == null ? -1 : resultVal.Length;

                                        Debug.WriteLine("i = {0}, recvLen = {1}, value = {2}", i, recvLen, resultVal == null ? "NULL" : BitConverter.ToString(resultVal));

                                        if (recvLen <= 0)
                                        {
                                            i--;
                                            if (i < 1)
                                                i = 0;
                                            continue;
                                        }
                                    }
                                    //RESET MULTIBLOCK CMD
                                    VdiCommand.GetDtcItemMultiBlockIdCmd[0] = 0;
                                    var ecuIds = StateMachine.DataModel.DtcValuesMap.Keys.ToArray();
                                    if (ecuIds?.Length <= 0)
                                    {
                                        Debug.WriteLine("***** DTC COMMUNICATION END - NO ECU ID *****");
                                        return -1;
                                    }
                                    var dtcValuesMap = StateMachine.DataModel.DtcValuesMap[ecuIds[0]];
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

                                    var dTCViewController = ContainerViewController.Instance.dtcViewController;
                                    if (dTCViewController != null)
                                    {
                                        dTCViewController.Value = dtcList;
                                    }
                                    else
                                    {
                                        ContainerViewController.Instance.DTCValue = dtcList;
                                    }

                                    switch (StateMachine.UIModel.CurrentPage)
                                    {
                                        case Page.Upload:
                                            //TODO SYM好像沒有這頁面
                                            //等於Android的TestFuncFragment
                                            var uploadVC = ContainerViewController.Instance.uploadViewController;
                                            
                                            if (uploadVC != null)
                                            {
                                                StateMachine.DataModel.CurrentDtcCodesList = new List<DtcData>(dtcList);
                                                //if (TestFuncFragment.IsStartAqmCommunication)
                                                //{
                                                //    new Thread(() =>
                                                //    {
                                                //        testFuncFragment.SendDtcByAMQ();
                                                //    }).Start();

                                                //}
                                            }
                                            break;

                                        case Page.LvCloud_Show:
                                            //var dmFragment = StateMachine.UIModel.DMFragment;
                                            //if (dmFragment != null)
                                            //{
                                            //    StateMachine.DataModel.CurrentDtcCodesList = new List<DtcData>(dtcList);
                                            //    if (DataMonitorFragment.IsStartAqmCommunication)
                                            //    {
                                            //        new Thread(() => {
                                            //            dmFragment.SendDtcByAMQ();
                                            //        }).Start();
                                            //    }
                                            //}
                                            break;
                                    }

                                    //UploadViewController uploadViewController = UploadViewController.Instance;
                                    //if(uploadViewController != null)
                                    //{
                                    //    uploadViewController.EcuID = ecuIds[0];
                                    //    if(uploadViewController.bSendPkt)
                                    //    {
                                    //        uploadViewController.Value = dtcList;
                                    //        uploadViewController.SendDTC();
                                    //    }
                                    //}
                                
                                    Debug.WriteLine("***** DTC COMMUNICATION END *****");
                                    return 1;
                                }
                                Debug.WriteLine("***** DTC COMMUNICATION END - FAILED *****");
                                return -1;
                            }
                        }
                    //case CommMode.COMMUNICATION_DTC:
                        //{
                            
                        //    mCBBLECentral.Setting(ProtocolType.SETTING);
                        //    resultVal= mCBBLECentral.SendAndReadData(VdiCommand.DtcSettingCmd);
                        //    recvLen = resultVal == null ? -1 : resultVal.Length;
                        //    if (recvLen <= 0)
                        //        return -1;

                        //    if (mCBBLECentral.GetSettingValue()[0] != 1)
                        //        return -1;

                        //    mCBBLECentral.Setting(ProtocolType.DTC);
                        //    //LABEL REGET DTC COUNT
                        //    //c# goto方法
                        //ReGetDtcCount:
                        //    resultVal = mCBBLECentral.SendAndReadData(VdiCommand.GetDtcItemMultiBlockIdCmd);
                        //    recvLen = resultVal == null ? -1 : resultVal.Length;
                        //    if (recvLen > 0)
                        //    {
                        //        int dtcBlocks = mCBBLECentral.GetDTCBlocksCount();
                        //        if (dtcBlocks == 0xFF)
                        //            goto ReGetDtcCount;
                        //        for (int i = 1; i <= dtcBlocks; i++)
                        //        {
                        //            if (dtcBlocks == 0xFF)
                        //            {
                        //                i--;
                        //                if (i <= 1)
                        //                    i = 1;
                        //            }

                        //            VdiCommand.GetDtcItemMultiBlockIdCmd[0] = (byte)i;
                        //            resultVal = mCBBLECentral.SendAndReadData(VdiCommand.GetDtcItemMultiBlockIdCmd);
                        //            recvLen = resultVal == null ? -1 : resultVal.Length;
                        //            if (recvLen <= 0)
                        //            {
                        //                if (!mCBBLECentral.IsConnected)
                        //                {
                        //                    VdiCommand.GetDtcItemMultiBlockIdCmd[0] = 0;
                        //                    return -1;
                        //                }

                        //                i--;
                        //                if (i <= 1)
                        //                    i = 1;

                        //            }
                        //        }
                        //        //RESET MULTIBLOCK CMD
                        //        VdiCommand.GetDtcItemMultiBlockIdCmd[0] = 0;
                        //        var ecuIds = StateMachine.DataModel.DtcValuesMap.Keys.ToArray();
                        //        if (ecuIds?.Length <= 0)
                        //            return -1;
                        //        var dtcValuesMap = StateMachine.DataModel.DtcValuesMap[ecuIds[0]];
                        //        dtcList.Clear();
                        //        if (dtcValuesMap != null)
                        //        {
                        //            foreach (var dtcVal in dtcValuesMap.Values)
                        //            {
                        //                dtcList.Add(dtcVal);
                        //            }
                        //        }
                        //        else
                        //        {
                        //            //IF NO DTC OR DTC VALUE IS NULL
                        //            DtcData dtcVal = new DtcData
                        //            {
                        //                DtcHexNumber = 0,
                        //                DtcName = "No Trouble Code",
                        //                DtcCodeForDisplay = "No DTC "
                        //            };
                        //            dtcList.Add(dtcVal);
                        //        }

                        //        if (ContainerViewController.Instance.dtcViewController != null)
                        //        {
                        //            ContainerViewController.Instance.dtcViewController.Value = dtcList;
                        //        }
                        //        else
                        //        {
                        //            ContainerViewController.Instance.DTCValue = dtcList;
                        //        }

                        //        if (ContainerViewController.Instance.uploadViewController != null)
                        //        {
                        //            ContainerViewController.Instance.uploadViewController.EcuID = ecuIds[0];
                        //            if (ContainerViewController.Instance.uploadViewController.bSendDtc)
                        //            {
                        //                ContainerViewController.Instance.uploadViewController.Value = dtcList;
                        //                ContainerViewController.Instance.uploadViewController.SendDTC();
                        //            }
                        //        }
                        //        else
                        //        {
                        //            ContainerViewController.Instance.EcuID = ecuIds[0];
                        //        }

                        //        //var dtcValues = mDataModel.DtcValues;

                        //        //if (dtcValues != null)
                        //        //{
                        //        //    dtcList.Clear();
                        //        //    foreach (var dtcVal in dtcValues)
                        //        //    {
                        //        //        dtcList.Add(dtcVal);
                        //        //    }
                        //        //    //dtcFragment.Value = dtcList;
                        //        //    DTCViewController.Instance?.SetDtcData(dtcList);

                        //        //}
                        //        return 1;
                        //    }
                        //    return -1;
                        //}


                    case CommMode.COMMUNICATION_SETTING:
                        mCBBLECentral.Setting(ProtocolType.SETTING);
                        resultVal = mCBBLECentral.SendAndReadData(mCmdBuffer);
                        recvLen = resultVal == null ? -1 : resultVal.Length;
                        if (recvLen > 0)
                        {
                            ////Mode = CommMode.NONE;
                            //if (recvLen == 17)
                            //{
                            //    string vin = System.Text.Encoding.Default.GetString(InfoValue);
                            //    mDataModel.DecodeVIN(vin);
                            //}
                            return recvLen;
                        }
                        return -1;

                    case CommMode.COMMUNICATION_INFO:
                        mCBBLECentral.Setting(ProtocolType.INFO);
                        resultVal = mCBBLECentral.SendAndReadData(mCmdBuffer);
                        recvLen = resultVal == null ? -1 : resultVal.Length;
                        if (recvLen > 0)
                        {
                            if ((mCmdBuffer[0] == VdiCommand.GetEcuIdCmd[0]) &&
                            (mCmdBuffer[1] == VdiCommand.GetEcuIdCmd[1]))
                            {
                                var ecuId = (uint)((resultVal[0] << 8) + resultVal[1]);
                                if (mDataModel.DtcUnpacker is MotorDtcDataUnpacker)
                                {
                                    mDataModel.EcuID = ecuId;
                                    mDataModel.DtcUnpacker.EcuID = ecuId;
                                }
                            }

                            mDataModel.InfoValue = mCBBLECentral.GetInfoValue();
                            ////Mode = CommMode.NONE;
                            //if (recvLen == 17)
                            //{
                            //    string vin = System.Text.Encoding.Default.GetString(InfoValue);
                            //    mDataModel.DecodeVIN(vin);
                            //}
                            return recvLen;
                        }
                        return -1;

                    case CommMode.COMMUNICATION_LOG:
                        {
                            if (mCBBLECentral.IsConnected)
                            {
                                mCBBLECentral.Setting(ProtocolType.LOG);
                                resultVal = mCBBLECentral.SendAndReadData(VdiCommand.GetLogItemMultiBlockIdCmd);
                                recvLen = resultVal == null ? -1 : resultVal.Length;
                                if (recvLen > 0)
                                {
                                    logBlockCount = mCBBLECentral.GetLogBlocksCount();
                                }
                                for (int i = 0; i < logBlockCount; i++)
                                {
                                    if (i < 255)
                                        VdiCommand.GetLogItemMultiBlockIdCmd[1] = (byte)i;
                                    else
                                    {
                                        VdiCommand.GetLogItemMultiBlockIdCmd[0] = (byte)((logBlockCount & 0xFF00) >> 8);
                                        VdiCommand.GetLogItemMultiBlockIdCmd[1] = (byte)logBlockCount;
                                    }
                                    resultVal = mCBBLECentral.SendAndReadData(VdiCommand.GetLogItemMultiBlockIdCmd);
                                    recvLen = resultVal == null ? -1 : resultVal.Length;
                                    if (recvLen > 0)
                                    {
                                        //mDataModel.CollectSupportedEcuLvIds(mBLECentral.GetLvIdValue());
                                    }
                                    else
                                    {
                                        if (mCBBLECentral.IsConnected == false)
                                            return -1;

                                        if (i <= 1)
                                            i = 1;
                                        else
                                            i--;
                                    }
                                }
                                VdiCommand.GetLogItemMultiBlockIdCmd[0] = 0;
                                VdiCommand.GetLogItemMultiBlockIdCmd[1] = 0;
                                return recvLen;
                            }

                            return -1;
                        }
                        break;

                    case CommMode.COMMUNICATION_READ_WRITE_MEMORY:
                        {
                            //mDataModel.InitReadWriteMemoryValues();
                            mCBBLECentral.Setting(ProtocolType.READ_WRITE_MEMORY);
                            //mDataModel.IsReadWriteMemoryOpOk = mBLECentral.IsReadWriteMemoryOK;
                            resultVal = mCBBLECentral.SendAndReadData(mCmdBuffer);
                            recvLen = resultVal == null ? -1 : resultVal.Length;
                            if (recvLen > 0)
                            {
                                //var rwmVals = mBLECentral.ReadWriteMemoryValue;
                                //if (mCmdBuffer.Count() == 0x02 && mCmdBuffer[0] == 0x22)
                                //{
                                //    switch (mCmdBuffer[1])
                                //    {
                                //        case (int)ReadWriteMemoryID.SunModeBrightness:
                                //            mDataModel.ReadMemory_SunModeBrightnessValue = rwmVals;
                                //            break;
                                //        case (int)ReadWriteMemoryID.NightModeBrightness:
                                //            mDataModel.ReadMemory_NightModeBrightnessValue = rwmVals;
                                //            break;
                                //        case (int)ReadWriteMemoryID.ValveOpenCloseMode:
                                //            mDataModel.ReadMemory_ValveModeValue = rwmVals;
                                //            break;
                                //        default:
                                //            break;
                                //    }
                                //}
                                //else if (recvLen == 1 && mCmdBuffer[0] == 0x3B)
                                //{
                                //    mDataModel.IsReadWriteMemoryOpOk = mBLECentral.IsReadWriteMemoryOK;
                                //}
                                return recvLen;
                            }
                        }
                        return -1;
                        break;
                    case CommMode.COMMUNICATION_TEST_PRESENT:
                        {
                            recvLen = mCBBLECentral.WriteTestPresentCharacteristic();

                            if (recvLen == (int)CommErrorCode.WriteCommandError)
                            {
                                Console.WriteLine("HOLD ON ERROR.");
                                IsSendInitCommand = false;
                            }

                            if (recvLen > 0)
                                return recvLen;

                            return -1;
                        }

                    #region REMOTE DIAGNOSTIC COMMUNICATION PROCESS
                    case CommMode.COMMUNICATION_REMOTE_DIAGNOSTIC_WRITE:
                        {
                            mCBBLECentral.Setting(ProtocolType.DATA_LINK_J1939);
                            RemoteDiagnositcVdiCommand.Build(mCmdBuffer, RemoteDiagnositcVdiCommand.Mode.Send);
                            var sendDataVals = RemoteDiagnositcVdiCommand.SendDataValues;
                            resultVal = mCBBLECentral.SendAndReadDataAsync(RemoteDiagnositcVdiCommand.SendCommand).Result;
                            recvLen = resultVal == null ? -1 : resultVal.Length;
                            if (recvLen > 0)
                            {
                                int sendCount = -1;
                                int index = -1;
                                byte[] currentCmd = null;
                                while (resultVal[0] != 0x93)
                                {
                                    //如果在寫入狀態，遇到接收的答案為對方要求數據的指令時(0x90-0xYY-0xYY-0xWW)
                                    //跳離迴圈，讓通訊程序跳至到遠距診斷讀取狀態
                                    if (resultVal.Length == 4 &&
                                        resultVal[0] == 0x90)
                                    {
                                        System.Diagnostics.Debug.WriteLine("Recv 0x90-0xYY-0xYY-0xWW Pattern In RemoteDiag Write Process.");
                                        return resultVal.Length;
                                    }

                                    try
                                    {
                                        sendCount = resultVal[1];
                                        index = resultVal[1];
                                        currentCmd = null;
                                        for (int i = 0; i < sendCount; i++)
                                        {
                                            currentCmd = sendDataVals[index - 1].ToArray();
                                            recvLen = mCBBLECentral.SendDataAsync(currentCmd).Result;
                                            if (recvLen < 0)
                                            {
                                                //var isRemoteDiagStart = StateMachine.DataModel.IsRemoteDiagnosticStart;
                                                //if (!isRemoteDiagStart)
                                                //    break;
                                                if (!isStartJ1939DataLinkProtocol)
                                                    break;

                                                i--;
                                                if (i < 0)
                                                    i = 0;
                                                if (mCBBLECentral.IsConnected == false)
                                                    return -1;
                                            }
                                        }
                                        //if (!StateMachine.DataModel.IsRemoteDiagnosticStart)
                                        //    return -1;
                                        if (!isStartJ1939DataLinkProtocol)
                                            return -1;

                                        var recvResult = mCBBLECentral.RecvDataAsync(currentCmd).Result;
                                        if (recvResult != null)
                                            resultVal = recvResult;

                                        if (mCBBLECentral.DataLinkProtocol.IsWriteOk)
                                            return recvLen;
                                    }
                                    catch (Exception ex)
                                    {
                                        System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                                        return -1;
                                        //break;
                                    }
                                }
                                return recvLen;
                            }
                        }
                        break;


                    case CommMode.COMMUNICATION_REMOTE_DIAGNOSTIC_READ:
                        {
                            var dataLinkProtocol = mCBBLECentral.DataLinkProtocol;
                            var perRecvBlockCount = 5;
                            while (!dataLinkProtocol.IsReadCompleted)
                            {
                                //var isRemoteDiagStart = StateMachine.DataModel.IsRemoteDiagnosticStart;
                                if (dataLinkProtocol.ReadValue != null &&
                                    dataLinkProtocol.ReadValue.Length == 4 &&
                                    dataLinkProtocol.ReadValue[0] == 0x90)
                                {
                                    int totoalSendTimes = -1;
                                    int sendPerBlockCheck = -1;
                                    int blockIndex = -1;
                                    if (dataLinkProtocol.RecvTotalBlockCount <= perRecvBlockCount)
                                    {
                                        sendPerBlockCheck = dataLinkProtocol.RecvTotalBlockCount;
                                        totoalSendTimes = 1;
                                    }
                                    else
                                    {
                                        sendPerBlockCheck = perRecvBlockCount;
                                        totoalSendTimes = (int)Math.Ceiling(dataLinkProtocol.RecvTotalBlockCount / (perRecvBlockCount * 1.0));
                                    }

                                    var currentCmd = new byte[] { 0x91, 0x00, 0x00 };
                                    bool needResend = false;
                                    //blockIndex = (perRecvBlockCount * i) + 1;
                                    blockIndex = 1;
                                    currentCmd[1] = (byte)sendPerBlockCheck;
                                    currentCmd[2] = (byte)blockIndex;
                                    for (int i = 0; i < totoalSendTimes; i++)
                                    {

                                        recvLen = mCBBLECentral.SendDataAsync(currentCmd).Result;
                                        if (recvLen > 0)
                                        {
                                            //等待資料接收完畢
                                            while (true)
                                            {
                                                Thread.Sleep(100);
                                                if (dataLinkProtocol.IsReadyForCheck)
                                                    break;

                                                if (dataLinkProtocol.IsRecvIncompleted)
                                                    break;

                                                //isRemoteDiagStart = StateMachine.DataModel.IsRemoteDiagnosticStart;
                                                //if (!isRemoteDiagStart)
                                                //    return -1;
                                                if (!isStartJ1939DataLinkProtocol)
                                                    return -1;
                                            }
                                            System.Diagnostics.Debug.WriteLine("Block Start Index : {0}, Recv Blocks : {1}, Recv Datas : {2}", blockIndex, dataLinkProtocol.CurrentRecvBlockIndex, BitConverter.ToString(dataLinkProtocol.AllRecvDataBytes));

                                            if (dataLinkProtocol.IsRecvIncompleted)
                                            {
                                                i--;
                                                if (i < 0)
                                                    i = 0;
                                                blockIndex = (byte)dataLinkProtocol.CurrentRecvBlockIndex;
                                                currentCmd[1] = (byte)(sendPerBlockCheck - blockIndex);
                                            }
                                            else
                                            {
                                                blockIndex = (perRecvBlockCount * i) + 1;
                                                currentCmd[1] = (byte)sendPerBlockCheck;
                                            }
                                            currentCmd[2] = (byte)blockIndex;
                                        }

                                        //isRemoteDiagStart = StateMachine.DataModel.IsRemoteDiagnosticStart;
                                        //if (!isRemoteDiagStart)
                                        //    return -1;
                                        if (!isStartJ1939DataLinkProtocol)
                                            return -1;
                                    }


                                }//end if
                            }//end while

                            if (dataLinkProtocol.IsReadCompleted)
                            {
                                var result = dataLinkProtocol.AllRecvDataBytes;
                                if (result != null)
                                {
                                    StateMachine.DataModel.RemoteDiagRecvValues = result.Clone() as byte[];
                                    System.Diagnostics.Debug.WriteLine("Recv Completed , Data : {0}", BitConverter.ToString(result));
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine("Recv Completed , Data : NULL");
                                }
                                var readOkCmd = new byte[] { 0x93, 0x00, 0x00, 0x00 };
                                var recvLen = result.Length;
                                readOkCmd[1] = (byte)(recvLen >> 8);
                                readOkCmd[2] = (byte)(recvLen);
                                readOkCmd[3] = (byte)dataLinkProtocol.RecvTotalBlockCount;
                                recvLen = mCBBLECentral.SendDataAsync(readOkCmd).Result;
                                if (recvLen > 0)
                                {
                                    return result.Length;
                                }
                            }
                            else
                                return -1;
                        }
                        break;

                    case CommMode.COMMUNICATION_REMOTE_DIAGNOSTIC_EXIT:
                        {
                            if (mCBBLECentral.IsConnected)
                            {
                                mCBBLECentral.Setting(ProtocolType.SETTING);
                                resultVal = mCBBLECentral.SendAndReadDataAsync(VdiCommand.RemoteDiagnosticExit).Result;
                                recvLen = resultVal == null ? -1 : resultVal.Length;

                                if (recvLen > 0)
                                    return recvLen;
                            }
                            return -1;
                        }
                    #endregion
                    default:
                        break;
                }
                return recvLen;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        #endregion


        /// <summary>
        /// Read RSSI
        /// </summary>
        /// <returns>RSSI Value</returns>
        public int ReadRssi()
        {
            mCBBLECentral.ReadRssiValue();
            return mCBBLECentral.RSSI;
        }


        #region SETTING DATA
        public bool SettingResultValue
        {
            get
            {
                return mCBBLECentral.SettingResult;
            }
        }
        #endregion


        //public bool Connect()
        //{
        //    if (mCBBLECentral == null)
        //        return false;
        //    else
        //        return mCBBLECentral.Connect();
        //}

        public bool Disconnect()
        {
            if (mCBBLECentral == null)
                return false;
            else
                return mCBBLECentral.Disconnect();
        }

        public bool IsInited
        {
            get
            {
                if (mCBBLECentral == null)
                    return false;
                else
                    return mCBBLECentral.IsInited;
            }
        }

        public bool IsScanning
        {
            get
            {
                if (mCBBLECentral == null)
                    return false;
                else
                    return mCBBLECentral.IsScanning;
            }
        }

        #region 測試用函數與欄位
        private UITextView mTvText = null;
        public void BindView(UITextView tvText)
        {
            mTvText = tvText;
            if (mCBBLECentral != null)
                mCBBLECentral.BindView(tvText);
        }

        public String GetBLEInfo()
        {
            if (mCBBLECentral != null)
                return mCBBLECentral.GetBLEInfo();
            return null;
        }

        #endregion

        public bool IsBtEnable
        {
            get
            {
                if (mCBBLECentral == null)
                    return false;
                else
                {
                    return mCBBLECentral.IsBtEnabled();
                }
            }
        }

        public CBBLEWrapper.GetLvDataFunc BLEGetLvDataAction
        {
            set
            {
                if (mCBBLECentral == null)
                    return;
                else
                {
                    mCBBLECentral.GetLvDataAction = value;
                }
            }
        }


        public CBBLEWrapper.CustomFunc BLEConnectionTimeoutAction
        {
            set
            {
                if (mCBBLECentral == null || value == null)
                    return;
                else
                {
                    mCBBLECentral.BLEConnectionTimeoutAction = value;
                }
            }
        }

        #region 2016-11-17 後期添加函數
        public IcmComLib.Communication.BLE.iOSBLE.CBBLEWrapper.CustomFunc BLEConnetedAction
        {
            set
            {
                if (mCBBLECentral == null || value == null)
                    return;
                else
                {
                    mCBBLECentral.BLEConnetedAction = value;
                }
            }
        }
        public IcmComLib.Communication.BLE.iOSBLE.CBBLEWrapper.CustomFunc BLEDisconnetedAction
        {
            set
            {
                if (mCBBLECentral == null || value == null)
                    return;
                else
                {
                    mCBBLECentral.BLEDisconnetedAction = value;
                }
            }
        }

        public CBBLEWrapper.DiscoverBleDeviceCallback DiscoverBleDeviceCallback
        {
            set
            {
                if (mCBBLECentral == null)
                    return;
                else
                {
                    mCBBLECentral.discoverBleDeviceCallback = value;
                }
            }
        }


        public IcmComLib.Communication.BLE.iOSBLE.CBBLEWrapper.CustomFunc BLEScanFailedAction
        {
            set

            {
                if (mCBBLECentral == null || value == null)
                    return;
                else
                {
                    mCBBLECentral.BLEScanFailedAction = value;
                }
            }
        }


        public IcmComLib.Communication.BLE.iOSBLE.CBBLEWrapper.GetLogRawValueFunc BLEGetLogRawValueAction
        {
            set
            {
                if (mCBBLECentral == null)
                    return;
                else
                {
                    mCBBLECentral.BLEGetLogRawValueAction = value;
                }
            }
        }
        #endregion

        //public CBBLEWrapper.BLEPowerState BLEPowerStateAction
        //{ 
        //	set
        //	{
        //		mCBBLECentral.BLEPowerStateAction = value;	
        //	}
        //}

        public void ReadSerialNumber()
        {
            if (mCBBLECentral == null)
                return;

            mCBBLECentral.ReadSerialNumber();
        }

        public void ReadFirmwareRevision()
        {
            if (mCBBLECentral == null)
                return;

            mCBBLECentral.ReadFirmwareRevision();
        }

        public void ReadSoftwareRevision()
        {
            if (mCBBLECentral == null)
                return;

            mCBBLECentral.ReadSoftwareRevision();
        }

        /// <summary>
        /// BLE是否開始使用J1939 DataLink通訊協定
        /// 此旗標主要用於隨時離開J1939 DataLink通訊協定用
        /// 當設為false，如果此時在J1939 DataLink通訊時，
        /// 恢復原本的BLE協定通訊
        /// </summary>
        private bool isStartJ1939DataLinkProtocol = false;
        public bool IsStartJ1939DataLinkProtocol
        {
            set { isStartJ1939DataLinkProtocol = value; }
            get { return isStartJ1939DataLinkProtocol; }
        }
    }
}
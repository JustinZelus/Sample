#define AMQ_Queue通道使用KAWASAKI資料
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;
using Apache.NMS;
using CoreFoundation;
using Foundation;
using IcmLib.Data;
using Model;
using ToastIOS;
using tw.com.kc.amq;
using UIKit;
using Xamarin_SYM_IOS.SRC.Model;
using Xamarin_SYM_IOS.SRC.UI;

namespace Xamarin_SYM_IOS.ViewControllers
{
    public partial class DataMonitorViewController : CustomViewController, IUITableViewDataSource, IUITableViewDelegate,IUIScrollViewDelegate
    {
        List<KawasakiDataMonitor> mData = new List<KawasakiDataMonitor>();
        public Dictionary<int, KawasakiDataMonitor> Values { get { return KawasakiLvDataDic; } }
        private bool isButtonClicked = false;
        //DIALOG
        private UIAlertController amqEnabledAlertDialog = null;

        //控制滑動不更新資料旗標
        private bool isScrolling = false;

        protected DataMonitorViewController(IntPtr handle) : base(handle)
        {
         
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            tblList.DataSource = this;
            tblList.Delegate = this;

            btnSendLvByAmq.TouchUpInside += BtnSendLvByAmq_Click;
            //btnSendLvByAmq.SetTitle(NSBundle.MainBundle.GetLocalizedString("start_amq_communication"), UIControlState.Normal);
            UpdateTimerValue += DataMonitorViewController_UpdateTimerValue;
            btnBack.TouchUpInside += BtnBack_Click;
            InitModel();
            InitDialog();

            //文字加邊框
            btnItem.Layer.BorderWidth = 0.0f;
            btnItem.Layer.BorderColor = UIColor.Blue.CGColor;
            btnItem.SetTitleColor(UIColor.White,UIControlState.Normal);
            btnValue.Layer.BorderWidth = 0.0f;
            btnValue.Layer.BorderColor = UIColor.Blue.CGColor;
            btnValue.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnUnit.Layer.BorderWidth = 0.0f;
            btnUnit.Layer.BorderColor = UIColor.Blue.CGColor;
            btnUnit.SetTitleColor(UIColor.White, UIControlState.Normal);

        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            IsInited = true;
            InitModel();
            StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            IsInited = false;
            #region 如果AMQ開啟，關閉AMQ
            if (IsStartAqmCommunication)
            {
                StopAmqCommunication();
                isButtonClicked = false;
            }
            #endregion
            //StateMachine.UIModel.CurrentPage = Page;
            StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV_STOP);

        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();

        }

        public nint RowsInSection(UITableView tableView, nint section)
        {   
            return mData.Count;
        }

        [Export("scrollViewWillBeginDragging:")]
        public void DraggingStarted(UIScrollView scrollView)
        {
            Console.WriteLine("開始滑動");
            isScrolling = true;

        }

        [Export("scrollViewDidEndDragging:willDecelerate:")]
        public void DraggingEnded(UIScrollView scrollView, bool willDecelerate)
        {
            Console.WriteLine("停止滑動");
            isScrolling = false;
        }

        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = tableView.DequeueReusableCell("DMCell", indexPath); 


            return SettingEachCellUIState(tableView, cell, indexPath.Row);
        }

        private UITableViewCell SettingEachCellUIState(UITableView tableView, UITableViewCell cell, int row)
        {

            var lblItem = (UILabel)cell.ContentView.ViewWithTag(1);
            string name = "";
            if (!mData[row].ShortName.Equals(""))
                name = mData[row].ShortName;
            else
                name = mData[row].LongName;

            lblItem.Text = "" + name;


            var lblValue = (UILabel)cell.ContentView.ViewWithTag(2);
            lblValue.Text = mData[row].Value.ToString("n" + mData[row].NumberOfDecimals);
            var patternDisplay = mData[row].PatternDisplayList;
            var value = mData[row].Value;
            if (patternDisplay == null ||
                patternDisplay.Count == 0 ||
                !patternDisplay.ContainsKey((int)value))
                lblValue.Text = value.ToString("n" + mData[row].NumberOfDecimals);
            else
                lblValue.Text = patternDisplay[(int)value];



            var lblUnit = (UILabel)cell.ContentView.ViewWithTag(3);
            lblUnit.Text = mData[row].Unit;

            return cell;
        }

        public override void setCurrentPageName()
        {

        }

        void BtnBack_Click(object sender, EventArgs e)
        {   


            if (StateMachine.UIModel.CurrentPage == Page.LvCloud_Show)
            {
                //當動態值數據顯示頁面，AMQ未作動，直接返回上一頁；AMQ作動時，彈出警告視窗
                if (!IsStartAqmCommunication)
                {
                    StateMachine.UIModel.CurrentPage = Page.LvCloud;
                    StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV_STOP);
                    ContainerViewController.Instance.PerformSegue(MyCustomPages.buttonRespondSegueID["btnLvDataCloud"], null);
                }
                else
                    JudgeAmqEnabledAndReturnParentPage();
            }
       
        }

        Dictionary<int, KawasakiDataMonitor> KawasakiLvDataDic = new Dictionary<int, KawasakiDataMonitor>();

        public void InitModel()
        {
            KawasakiLvDataDic.Clear();
            mData.Clear();


            var lvIds = ContainerViewController.Instance.lvDataCloudViewController.CurrentSendLvItemsForLvCloud;

            var dbVals = StateMachine.DataModel.DmIcmDBHelper.DmValues;
            foreach (var lvId in lvIds)
            {
                if (!dbVals.ContainsKey(lvId))
                    continue;

                string itemName = dbVals[lvId].Name;
                float value = 0.0f;
                string unit = dbVals[lvId].Unit;
                uint numberOfDecimals = dbVals[lvId].NumberOfDecimals;
                Dictionary<int, string> patternDisplayList = dbVals[lvId].PatternDisplay;
                string shortName = dbVals[lvId].ShortName;
                float? minValue = dbVals[lvId].MinValue;
                float? maxValue = dbVals[lvId].MaxValue;

                KawasakiDataMonitor dmDataItem = new KawasakiDataMonitor(itemName, value, unit, numberOfDecimals, 
                                                                         patternDisplayList, shortName,
                                                                            minValue,
                                                                            maxValue);


                if (!KawasakiLvDataDic.ContainsKey(lvId))
                    KawasakiLvDataDic.Add(lvId, dmDataItem);
                else
                    KawasakiLvDataDic[lvId] = dmDataItem;

                mData.Add(dmDataItem);
                //mAdapter.Refresh(mData);
            }

            StateMachine.UIModel.CurrentPage = Page.LvCloud_Show;
        }

        private void InitDialog()
        {
            InitAmqEnabledAlertDialog();
        }

        private void InitAmqEnabledAlertDialog()
        {
            if (amqEnabledAlertDialog == null && (ContainerViewController.Instance != null))
            {

                amqEnabledAlertDialog = UIAlertController.Create(NSBundle.MainBundle.GetLocalizedString("warning_title")
                                                                 , NSBundle.MainBundle.GetLocalizedString("exit_when_amq_communication_enabled")
                                                                 , UIAlertControllerStyle.Alert);
                //第一顆按鈕

                amqEnabledAlertDialog.AddAction(UIAlertAction.Create(NSBundle.MainBundle.GetLocalizedString("rpmSettingBtnOK"), UIAlertActionStyle.Default, (action) => {
                    isButtonClicked = false;
                    StopAmqCommunication();
                    RestoreUI();
                    StateMachine.UIModel.CurrentPage = Page.LvCloud;
                    StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV_STOP);
                    ContainerViewController.Instance.PerformSegue(MyCustomPages.buttonRespondSegueID["btnLvDataCloud"], null);
                    //amqEnabledAlertDialog.DismissViewController(false, null);
                    //this.DismissViewController(false, null);
                    //amqEnabledAlertDialog.PresentViewController(this, false, null);
                    ContainerViewController.Instance.DismissViewController(false, null);
                }));
                //第二顆按鈕
                amqEnabledAlertDialog.AddAction(UIAlertAction.Create(NSBundle.MainBundle.GetLocalizedString("rpmSettingBtnCancel"), UIAlertActionStyle.Cancel, (action) => {
                    //amqEnabledAlertDialog.Dismiss();
                    //this.DismissViewController(false, null);
                    //amqEnabledAlertDialog.DismissViewController(false, null);
                    ContainerViewController.Instance.DismissViewController(false, null);
                }));

            }
        }

        /// <summary>
        /// AMQ傳送LvData按鈕的點擊事件響應
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSendLvByAmq_Click(object sender, EventArgs e)
        {
            isButtonClicked = !isButtonClicked;
            string toastMessageId = "-1";
            //當按鈕按下
            if (isButtonClicked)
            {

                toastMessageId = NSBundle.MainBundle.GetLocalizedString("stop_amq_communication");
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    btnSendLvByAmq.SetTitle(toastMessageId, UIControlState.Normal);
                });

                StartAmqCommunication();
            }
            else
            {
                toastMessageId = NSBundle.MainBundle.GetLocalizedString("start_amq_communication");
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    btnSendLvByAmq.SetTitle(toastMessageId, UIControlState.Normal);
                });
                StopAmqCommunication();
            }
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                Toast.MakeText(toastMessageId).Show();
            });
        }

        public void Refresh(List<KawasakiDataMonitor> datas)
        {
            if (mData != null)
            {
                mData = null;
            }
            mData = datas;
            Thread.Sleep(10);
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                this.tblList.ReloadData();
            });
        }

        //判斷AMQ是否作動中，返回上一頁
        public void JudgeAmqEnabledAndReturnParentPage()
        {   
        
            if (IsInited)
            {
                //如果AMQ通訊中，彈出是否要停止通訊回到上一頁的對話視窗
                if (IsStartAqmCommunication && amqEnabledAlertDialog != null)
                {
                    if (!amqEnabledAlertDialog.IsBeingPresented)
                    {
                       
                        //需加入判斷alert是否已存在，否則alert將不會出現，畫面會卡住。
                        if(PresentedViewController == null) 
                        {
                            this.PresentViewController(amqEnabledAlertDialog, true, null);
                        }
                        else
                        {
                            this.DismissViewController(false,() => {
                                this.PresentViewController(amqEnabledAlertDialog, true, null);

                            });
                        }


                    }
                        
                }
            }
        }

        //恢復UI頁面設置
        public void RestoreUI()
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                btnSendLvByAmq.SetTitle(NSBundle.MainBundle.GetLocalizedString("start_amq_communication"), UIControlState.Normal);
            });
        }

        void DataMonitorViewController_UpdateTimerValue()
        {
            //Console.WriteLine("DataMonitorViewController update data");
            if (!IsInited)
                return;

            var lvVales = StateMachine.DataModel.LvValues;


            var UiValues = Values;

            foreach (var lvId in lvVales.Keys)
            {
                if (UiValues.ContainsKey(lvId))
                {
                    var unit = UiValues[lvId].Unit;
                    UiValues[lvId].Unit = unit;
                    UiValues[lvId].Value = lvVales[lvId];
                }


            }

            if(!isScrolling)
                Refresh(UiValues.Values.ToList());
        }

        //------------------------------------------------------------------------------------------
        //--------------------- A M Q -------------------------------------------------------------
        //--------------------------------------------------------------------------------------------

        #region AM通訊函數與變數
        static private String userName = @"admin";
        static private String password = @"admin";
        static String target = @"tcp://210.65.88.10:61616";
        static AMQManager amqManager = null;
        IMessageProducer messageProducer = null;
        int amqCommunicationErrorCount = 0;
        int amqCommunicationErrorAlarmTimes = 5;

#if AMQ_Queue通道使用KAWASAKI資料
        //string desternationStr = @"topic://Kawasaki";
        string desternationStr = @"topic://VTMS_Data";
        string brand = "Kawasaki";
        string vehicleName = "Kawasaki";
#else
        string desternationStr = @"topic://Keihin";
        string brand = "Keihin";
        string vehicleName = "Keihin";
#endif
        public static bool IsStartAqmCommunication
        {
            get
            {
                if (amqManager == null)
                    return false;
                else
                    return amqManager.Enabled;
            }
        }
#if 使用豐碩定義之XSD格式
        root root = null;
        rootCarInfo rootCarInfo = null;
        ConcurrentDictionary<int, rootCarInfoPropertys> propertyDict = new ConcurrentDictionary<int, rootCarInfoPropertys>();
#else
        InterfaceInfo interfaceInfo = null;
        ECU ecu = null;
        ConcurrentDictionary<uint, ECU> ecuDict = new ConcurrentDictionary<uint, ECU>();
        ConcurrentDictionary<int, LvData> lvDataDict = new ConcurrentDictionary<int, LvData>();
        ConcurrentDictionary<int, DTCode> dtcCodeDict = new ConcurrentDictionary<int, DTCode>();
#endif
        public void StartAmqCommunication()
        {
            new Thread(() => {
                try
                {
                    amqManager = new AMQManager(userName, password, target);
                    messageProducer = amqManager.CreateProducer(desternationStr);
                    if (messageProducer == null)
                        return;
                    long currentTimestamp = TimestampToLong();
#if 使用豐碩定義之XSD格式
                    if (root != null)
                        root = null;

                    if (rootCarInfo != null)
                        rootCarInfo = null;

                    propertyDict.Clear();
                    //init root & carInfo Instance
                    root = new root();
                    root.CarInfo = new rootCarInfo[1];
                    rootCarInfo = new rootCarInfo();
                    rootCarInfo.uuid = "26104410-af59-11e8-98d0-529269fb1459";
                    root.CarInfo[0] = rootCarInfo;
#else
                    dtcCodeDict.Clear();
                    lvDataDict.Clear();
                    ecuDict.Clear();

                    if (interfaceInfo != null)
                        interfaceInfo = null;
                    interfaceInfo = new InterfaceInfo();

                    interfaceInfo.serialNumber = StateMachine.DataModel.DeviceInfo_SerialNumber;
                    interfaceInfo.softwareVersion = StateMachine.DataModel.DeviceInfo_SoftwareRevision;
                    interfaceInfo.firmwareVersion = StateMachine.DataModel.DeviceInfo_FirmwareRevistion;
                    if (interfaceInfo.VehicleInfo != null)
                        interfaceInfo.VehicleInfo = null;
                    interfaceInfo.VehicleInfo = new VehicleInfo();
                    interfaceInfo.VehicleInfo.brand = StateMachine.DataModel.ManufactureValue;
                    //品牌目前先寫固定
                    //interfaceInfo.VehicleInfo.brand = brand;
                    interfaceInfo.VehicleInfo.diagTimestamp = currentTimestamp;

                    int vehicleId = 0;
                    var vehicleIdStr = StateMachine.DataModel.VehicleIdValue;
                    if (vehicleIdStr != null)
                        int.TryParse(vehicleIdStr, out vehicleId);
                    interfaceInfo.VehicleInfo.vehicleId = vehicleId;
                    interfaceInfo.VehicleInfo.vehicleName = StateMachine.DataModel.VehicleNameValue;
                    //車輛名稱目前先寫固定
                    //interfaceInfo.VehicleInfo.vehicleName = vehicleName;
                    interfaceInfo.VehicleInfo.modCode = StateMachine.DataModel.ModuleCode;


                    var ecuId = StateMachine.DataModel.EcuID;
                    if (ecu != null)
                        ecu = null;
                    ecu = new ECU();
                    ecu.id = ecuId.ToString();


                    if (interfaceInfo.VehicleInfo.ECUs != null)
                        interfaceInfo.VehicleInfo.ECUs = null;
                    ecuDict.TryAdd(ecuId, ecu);
                    interfaceInfo.VehicleInfo.ECUs = ecuDict.Values.ToArray();


                    StateMachine.UIModel.DisableIconTabs();
                    //interfaceInfo.serialNumber = StateMachine.DataModel.DeviceInfo_SerialNumber;
                    //interfaceInfo.softwareVersion = StateMachine.DataModel.DeviceInfo_SoftwareRevision;
                    //interfaceInfo.firmwareVersion = StateMachine.DataModel.DeviceInfo_FirmwareRevistion;

                    //if (interfaceInfo.VehicleInfo != null)
                    //    interfaceInfo.VehicleInfo = null;
                    //interfaceInfo.VehicleInfo = new VehicleInfo();
                    //interfaceInfo.VehicleInfo.brand = "SYM";
                    //interfaceInfo.VehicleInfo.vehicleId = 90393;
                    //interfaceInfo.VehicleInfo.vehicleName = @"高手 125 X'PRO";
                    //interfaceInfo.VehicleInfo.modCode = @"FEA";
                    //interfaceInfo.VehicleInfo.ecuId = 7;
                    //interfaceInfo.VehicleInfo.diagTimestamp = currentTimestamp;

                    #region 無連接裝置，模擬填充數據發送到AMQ
                    //ConcurrentDictionary<int, float> simulationDatas = new ConcurrentDictionary<int, float>();
                    //simulationDatas.TryAdd(2, 100);
                    //simulationDatas.TryAdd(912, 13.7f);
                    //simulationDatas.TryAdd(913, 1.22f);
                    //simulationDatas.TryAdd(905, 42.5f);
                    //simulationDatas.TryAdd(907, 61);
                    //simulationDatas.TryAdd(908, 100);
                    //simulationDatas.TryAdd(909, 37);
                    //simulationDatas.TryAdd(910, 2);
                    //simulationDatas.TryAdd(911, 10.5f);
                    //int sendTimes = 50;
                    //for (int i = 0; i < sendTimes; i++)
                    //{
                    //    foreach (var key in simulationDatas.Keys)
                    //    {
                    //        simulationDatas[key] += i;
                    //    }
                    //    SendLVDataByAMQ(simulationDatas);
                    //}
                    #endregion
#endif

                    //StateMachine.Instance.SendMessage(StateMachineStatus.Communication_DTC);
                    //Thread.Sleep(50);
                    //StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV);
                }
                catch (Exception)
                {
                    StopAmqCommunicationAndShowError();
                }
            }).Start();
        }

        /// <summary>
        /// 停止AMQ通訊
        /// </summary>
        public void StopAmqCommunication()
        {
            StateMachine.UIModel.EnableIconTabs();
            amqCommunicationErrorAlarmTimes = 0;
            if (amqManager != null)
            {
                if (messageProducer != null)
                    messageProducer.Close();
                messageProducer = null;
                amqManager.ShutDown();
                amqManager.Dispose();
            }
        }

        /// <summary>
        /// 此函數用於停止AMQ通訊，並顯示連線錯誤
        /// </summary>
        public void StopAmqCommunicationAndShowError()
        {
            StopAmqCommunication();
            //isButtonClicked = !isButtonClicked;
            isButtonClicked = false;

           

            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                btnSendLvByAmq.SetTitle("Start Cloud Upload", UIControlState.Normal);
                Toast.MakeText("Create the internet connection error, please check your internet connection exists or not.").Show();
            });
        }

        public static string SerializeObject<T>(T toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }


        private IList<DtcData> mList = new List<DtcData>();
        // 上傳DTC By AMQ
        public void SendDtcByAMQ()
        {
            if (!IsStartAqmCommunication) return;
            if (interfaceInfo.VehicleInfo.ECUs[0] == null)
                return;

            long timestamp = TimestampToLong();
            lock (this)
            {
                var dtclist = new List<Int32>();
                dtcCodeDict.Clear();
                foreach (var item in mList)
                {
                    var dtcHexNum = item.DtcHexNumber;
                    dtclist.Add(dtcHexNum);
                    if (!dtcCodeDict.ContainsKey(dtcHexNum))
                        dtcCodeDict.TryAdd(dtcHexNum, new DTCode());
                    dtcCodeDict[dtcHexNum].diagTimestamp = timestamp;
                    dtcCodeDict[dtcHexNum].hexCode = dtcHexNum.ToString("X4");
                }


#if 上傳DTC_DATA篩選
                if (dtcDatasTemp == null)
                    dtcDatasTemp = new List<int>(dtclist);
                else
                {
                    int currentDtcCount = dtclist.Count;
                    int preDtcCount = dtcDatasTemp.Count;

                    //2018-05-30比對DTC與上次資料是否相符合，
                    //如果兩個DTC列表長度一樣，比對資料是否一樣，
                    //資料一樣不上傳，不一樣的話上傳；
                    //如果DTC列表長度不一樣，直接上傳
                    if (currentDtcCount == preDtcCount)
                    {
                        var compareResult = dtclist.Except(dtcDatasTemp).ToList();
                        if (compareResult.Count == 0)
                        {
                            compareResult = null;
                            return;
                        }
                        else
                        {
                            dtcDatasTemp = null;
                            dtcDatasTemp = new List<int>(dtclist);
                        }
                    }
                }
                //當篩選後，沒有新的DTC Code，直接返回
                if (dtcDatasTemp.Count <= 0)
                    return;
#endif
                interfaceInfo.VehicleInfo.ECUs[0].DTCodes = dtcCodeDict.Values.ToArray();




                var msg = SerializeObject<InterfaceInfo>(interfaceInfo);

                System.Diagnostics.Debug.WriteLine(msg);
                if (messageProducer == null || msg == null)
                    return;
                if (amqManager.SendMessage(messageProducer, msg))
                {
                    System.Diagnostics.Debug.WriteLine("Send ActiveMQ DTC Message Succeeded.");
                    amqCommunicationErrorCount = 0;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Send ActiveMQ DTC Message Failed.");
                    amqCommunicationErrorCount++;
                    if (amqCommunicationErrorCount > amqCommunicationErrorAlarmTimes)
                        StopAmqCommunicationAndShowError();
                }

                mList.Clear();
            }
        }


        // 上傳DTC By AMQ
        public void SendDtcByAMQ(List<DtcData> dtcDataList)
        {
            if (!IsStartAqmCommunication) return;
            if (interfaceInfo.VehicleInfo.ECUs[0] == null)
                return;

            long timestamp = TimestampToLong();
            lock (this)
            {
                var dtclist = new List<Int32>();
                dtcCodeDict.Clear();
                foreach (var item in dtcDataList)
                {
                    var dtcHexNum = item.DtcHexNumber;
                    dtclist.Add(dtcHexNum);
                    if (!dtcCodeDict.ContainsKey(dtcHexNum))
                        dtcCodeDict.TryAdd(dtcHexNum, new DTCode());
                    dtcCodeDict[dtcHexNum].diagTimestamp = timestamp;
                    dtcCodeDict[dtcHexNum].hexCode = dtcHexNum.ToString("X4");
                }


#if 上傳DTC_DATA篩選
                if (dtcDatasTemp == null)
                    dtcDatasTemp = new List<int>(dtclist);
                else
                {
                    int currentDtcCount = dtclist.Count;
                    int preDtcCount = dtcDatasTemp.Count;

                    //2018-05-30比對DTC與上次資料是否相符合，
                    //如果兩個DTC列表長度一樣，比對資料是否一樣，
                    //資料一樣不上傳，不一樣的話上傳；
                    //如果DTC列表長度不一樣，直接上傳
                    if (currentDtcCount == preDtcCount)
                    {
                        var compareResult = dtclist.Except(dtcDatasTemp).ToList();
                        if (compareResult.Count == 0)
                        {
                            compareResult = null;
                            return;
                        }
                        else
                        {
                            dtcDatasTemp = null;
                            dtcDatasTemp = new List<int>(dtclist);
                        }
                    }
                }

                //當篩選後，沒有新的DTC Code，直接返回
                if (dtcDatasTemp.Count <= 0)
                    return;
#endif
                interfaceInfo.VehicleInfo.ECUs[0].DTCodes = dtcCodeDict.Values.ToArray();




                var msg = SerializeObject<InterfaceInfo>(interfaceInfo);

                System.Diagnostics.Debug.WriteLine(msg);
                if (messageProducer == null || msg == null)
                    return;
                if (amqManager.SendMessage(messageProducer, msg))
                {
                    System.Diagnostics.Debug.WriteLine("Send ActiveMQ DTC Message Succeeded.");
                    amqCommunicationErrorCount = 0;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Send ActiveMQ DTC Message Failed.");
                    amqCommunicationErrorCount++;
                    if (amqCommunicationErrorCount > amqCommunicationErrorAlarmTimes)
                        StopAmqCommunicationAndShowError();
                }

                dtcDataList.Clear();
            }
        }

        /// <summary>
        /// 使用當前Data Model的LvValues數值送至AMQ
        /// 
        /// 直接使用當前Data Model的LvData數據，
        /// 避免一直產生執行緒後，發生執行緒執行先後不一的問題，
        /// 造成有些新產生的執行緒後執行的問題，造成即時資料先前的數據一直在插進目前的資料(之前的資料，不斷加入當前的資料)
        /// </summary>
        public void SendLVDataByAMQ()
        {
            if (!IsStartAqmCommunication) return;

            var lvValues = StateMachine.DataModel.LvValues;

            if (lvValues == null || lvValues.Count == 0)
                return;

            var timestamp = DateTime.Now;
            var timestampStr = timestamp.ToString("yyyy/MM/dd hh:mm:ss.fff tt");
            Console.WriteLine("Upload SendLVData : {0}", timestampStr);
            long currentTimestamp = TimestampToLong();
#if 使用豐碩定義之XSD格式
            rootCarInfo.timestamp = timestampStr;
            rootCarInfo.Lat = (float)GPSData.LatLng.Latitude;
            rootCarInfo.Lng = (float)GPSData.LatLng.Longitude;
            rootCarInfo.LatSpecified = true;
            rootCarInfo.LngSpecified = true;
            rootCarInfo.Lng = (float)GPSData.LatLng.Longitude;
#endif
            lock (this)
            {
#if AMQ動態值每次刷新到最新_不留之前的暫存ID與數據
                //每次都將要送的資料刷新重來
#if 使用豐碩定義之XSD格式
                propertyDict.Clear();
#else
                lvDataDict.Clear();
#endif
#endif

#if 使用豐碩定義之XSD格式
                foreach (var lvValueId in lvValues.Keys)
                {
                    if (!propertyDict.ContainsKey(lvValueId))
                        propertyDict.TryAdd(lvValueId, new rootCarInfoPropertys());

                    propertyDict[lvValueId].LV_ID = (short)lvValueId;
                    propertyDict[lvValueId].LV_Value = (short)lvValues[lvValueId];
                }
                if (root == null)
                    return;
                
                rootCarInfo.Propertys = propertyDict.Values.ToArray();
                root.CarInfo[0] = rootCarInfo;
                var msg = SerializeObject<root>(root);

#else
                //LvID = 6001 ; Longitude
                int lngLvId = 6001;
                lvDataDict.TryAdd(lngLvId, new LvData());
                lvDataDict[lngLvId].id = (short)lngLvId;
                lvDataDict[lngLvId].value = (float)GPSData.LatLng.Longitude;
                lvDataDict[lngLvId].diagTimestamp = currentTimestamp;

                //LvID = 6002 ; Latitude
                int latLvId = 6002;
                lvDataDict.TryAdd(latLvId, new LvData());
                lvDataDict[latLvId].id = (short)latLvId;
                lvDataDict[latLvId].value = (float)GPSData.LatLng.Latitude;
                lvDataDict[latLvId].diagTimestamp = currentTimestamp;

                foreach (var lvValueId in lvValues.Keys)
                {
                    if (!lvDataDict.ContainsKey(lvValueId))
                        lvDataDict.TryAdd(lvValueId, new LvData());

                    lvDataDict[lvValueId].id = (short)lvValueId;
                    lvDataDict[lvValueId].value = lvValues[lvValueId];
                    lvDataDict[lvValueId].diagTimestamp = currentTimestamp;
                }
                if (interfaceInfo == null ||
                    interfaceInfo.VehicleInfo == null ||
                    interfaceInfo.VehicleInfo.ECUs == null)
                    return;

                interfaceInfo.VehicleInfo.ECUs[0].LvDatas = lvDataDict.Values.ToArray();

                if (lvValues.ContainsKey(913))
                    Console.WriteLine("ID 913 VALUE = {0}", lvValues[913]);

                var msg = SerializeObject<InterfaceInfo>(interfaceInfo);
#endif
                System.Diagnostics.Debug.WriteLine(msg);
                if (messageProducer == null || msg == null)
                    return;
                if (amqManager.SendMessage(messageProducer, msg))
                {
                    System.Diagnostics.Debug.WriteLine("Send ActiveMQ LV Message Succeeded.");
                    amqCommunicationErrorCount = 0;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Send ActiveMQ LV Message Failed.");
                    amqCommunicationErrorCount++;
                    if (amqCommunicationErrorCount > amqCommunicationErrorAlarmTimes)
                        StopAmqCommunicationAndShowError();
                }
            }
        }


        //上傳LvData By AMQ
        public void SendLVDataByAMQ(ConcurrentDictionary<int, float> lvValues)
        {
            if (!IsStartAqmCommunication) return;

            if (lvValues == null || lvValues.Count == 0)
                return;

            var timestamp = DateTime.Now;
            var timestampStr = timestamp.ToString("yyyy/MM/dd hh:mm:ss.fff tt");
            Console.WriteLine("Upload SendLVData : {0}", timestampStr);
            long currentTimestamp = TimestampToLong();
#if 使用豐碩定義之XSD格式
            rootCarInfo.timestamp = timestampStr;
            rootCarInfo.Lat = (float)GPSData.LatLng.Latitude;
            rootCarInfo.Lng = (float)GPSData.LatLng.Longitude;
            rootCarInfo.LatSpecified = true;
            rootCarInfo.LngSpecified = true;
            rootCarInfo.Lng = (float)GPSData.LatLng.Longitude;
#endif
            lock (this)
            {
#if AMQ動態值每次刷新到最新_不留之前的暫存ID與數據
                //每次都將要送的資料刷新重來
#if 使用豐碩定義之XSD格式
                propertyDict.Clear();
#else
                lvDataDict.Clear();
#endif
#endif

#if 使用豐碩定義之XSD格式
                foreach (var lvValueId in lvValues.Keys)
                {
                    if (!propertyDict.ContainsKey(lvValueId))
                        propertyDict.TryAdd(lvValueId, new rootCarInfoPropertys());

                    propertyDict[lvValueId].LV_ID = (short)lvValueId;
                    propertyDict[lvValueId].LV_Value = (short)lvValues[lvValueId];
                }
                if (root == null)
                    return;
                
                rootCarInfo.Propertys = propertyDict.Values.ToArray();
                root.CarInfo[0] = rootCarInfo;
                var msg = SerializeObject<root>(root);

#else
                //LvID = 6001 ; Longitude
                int lngLvId = 6001;
                lvDataDict.TryAdd(lngLvId, new LvData());
                lvDataDict[lngLvId].id = (short)lngLvId;
                lvDataDict[lngLvId].value = (float)GPSData.LatLng.Longitude;
                lvDataDict[lngLvId].diagTimestamp = currentTimestamp;

                //LvID = 6002 ; Latitude
                int latLvId = 6002;
                lvDataDict.TryAdd(latLvId, new LvData());
                lvDataDict[latLvId].id = (short)latLvId;
                lvDataDict[latLvId].value = (float)GPSData.LatLng.Latitude;
                lvDataDict[latLvId].diagTimestamp = currentTimestamp;

                foreach (var lvValueId in lvValues.Keys)
                {
                    if (!lvDataDict.ContainsKey(lvValueId))
                        lvDataDict.TryAdd(lvValueId, new LvData());

                    lvDataDict[lvValueId].id = (short)lvValueId;
                    lvDataDict[lvValueId].value = lvValues[lvValueId];
                    lvDataDict[lvValueId].diagTimestamp = currentTimestamp;
                }
                if (interfaceInfo == null ||
                    interfaceInfo.VehicleInfo == null ||
                    interfaceInfo.VehicleInfo.ECUs == null)
                    return;

                interfaceInfo.VehicleInfo.ECUs[0].LvDatas = lvDataDict.Values.ToArray();

                if (lvValues.ContainsKey(913))
                    Console.WriteLine("ID 913 VALUE = {0}", lvValues[913]);

                var msg = SerializeObject<InterfaceInfo>(interfaceInfo);
#endif
                System.Diagnostics.Debug.WriteLine(msg);
                if (messageProducer == null || msg == null)
                    return;
                if (amqManager.SendMessage(messageProducer, msg))
                {
                    System.Diagnostics.Debug.WriteLine("Send ActiveMQ LV Message Succeeded.");
                    amqCommunicationErrorCount = 0;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Send ActiveMQ LV Message Failed.");
                    amqCommunicationErrorCount++;
                    if (amqCommunicationErrorCount > amqCommunicationErrorAlarmTimes)
                        StopAmqCommunicationAndShowError();
                }
            }
        }

        static public long TimestampToLong()
        {
            #region timestamp 取得，和JAVA一樣
            TimeSpan Now = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1));
            long TimeStamp = Convert.ToInt64(Math.Floor(Now.TotalMilliseconds));
            #endregion
            return TimeStamp;
        }

        #endregion
    }
}


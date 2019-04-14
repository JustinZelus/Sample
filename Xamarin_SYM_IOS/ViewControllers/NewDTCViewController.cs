using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using CoreFoundation;
using Foundation;
using IcmLib.Data;
using UIKit;

namespace Xamarin_SYM_IOS.ViewControllers
{
    public partial class NewDTCViewController : CustomViewController, IUITableViewDataSource, IUITableViewDelegate
    {
        private IList<DtcData> mList = new List<DtcData>();
        private const String EcuSystemRadioGroupTagName = @"EcuSystem";
        private const String DtcTypeRadioGroupTagName = @"DtcType";

        Dictionary<string, UIStackView> _EcuSystemRadioGroup = new Dictionary<string, UIStackView>();


        public NewDTCViewController(IntPtr handle) : base(handle)
        {

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            table.Delegate = this;
            table.DataSource = this;

            //var btns = mStackView.ArrangedSubviews;
            //Console.WriteLine("Get subviews from stackview");
            // btnObd2.Hidden = true;

            //改與Android同
            InitView();
        }

        private void InitView()
        {
            InitDtcTypeRadioButtons();
            InitEcuSystemRadioButtons();
            IsInited = true;   
        }

        private void InitDtcTypeRadioButtons()
        {
            //TODO 改
            segDtcType.TouchUpInside += DtcTypeOnCheckedChanged;

            //rgDtcType.Tag = DtcTypeRadioGroupTagName;
            //rgDtcType.SetOnCheckedChangeListener(this);
        }


        /// <summary>
        /// 初始化Ecu系統Radio Button
        /// </summary>
        /// <param name="inflaterView"></param>
        private void InitEcuSystemRadioButtons()
        {
            btnEngine.Tag = (int)EcuSystemGroup.Engine;
            ecuSystemRadioButtonDict.Add(EcuSystemGroup.Engine.ToString(), btnEngine);

            btnIsg.Tag = ((int)(EcuSystemGroup.ISG));
            ecuSystemRadioButtonDict.Add(EcuSystemGroup.ISG.ToString(), btnIsg);

            btnObd2.Tag = ((int)(EcuSystemGroup.OBDII));
            ecuSystemRadioButtonDict.Add(EcuSystemGroup.OBDII.ToString(), btnObd2);

            btnAbs.Tag = ((int)(EcuSystemGroup.ABS_ASC_ESP));
            ecuSystemRadioButtonDict.Add(EcuSystemGroup.ABS_ASC_ESP.ToString(), btnAbs);

            btnBms.Tag = ((int)(EcuSystemGroup.BMS));
            ecuSystemRadioButtonDict.Add(EcuSystemGroup.BMS.ToString(), btnBms);

            btnMcu.Tag = ((int)(EcuSystemGroup.MCU));
            ecuSystemRadioButtonDict.Add(EcuSystemGroup.MCU.ToString(), btnMcu);

            btnIcm.Tag = ((int)(EcuSystemGroup.ICM));
            ecuSystemRadioButtonDict.Add(EcuSystemGroup.ICM.ToString(), btnIcm);

            btnEps.Tag = ((int)(EcuSystemGroup.EPS));
            ecuSystemRadioButtonDict.Add(EcuSystemGroup.EPS.ToString(), btnEps);

            btnEv.Tag = ((int)(EcuSystemGroup.EV));
            ecuSystemRadioButtonDict.Add(EcuSystemGroup.EV.ToString(), btnEv);


            //可能用不到
            _EcuSystemRadioGroup.Add(EcuSystemRadioGroupTagName, svEcuSystem);


            

            //通訊獲得之EcuId列表
            var ecuIdList = StateMachine.DataModel.EcuIdList;
            var ecuGroupDict = StateMachine.DataModel.EcuGroupsDict;
            foreach (var ecuId in ecuIdList)
            {
                //如果該Ecu群組字典檔包含此EcuID，填充Ecu系統所涵蓋的Ecu列表字典檔 EcuSystemAndIdListDict
                if (ecuGroupDict.ContainsKey(ecuId))
                {
                    var ecuSystemGroup = ecuGroupDict[ecuId].EcuSystemGroup;
                    if (!EcuSystemAndIdListDict.ContainsKey(ecuSystemGroup))
                    {
                        //添加各Ecu系統群組之涵蓋EcuID
                        EcuSystemAndIdListDict.Add(ecuSystemGroup, new List<uint>());
                        EcuSystemAndIdListDict[ecuSystemGroup].Add(ecuId);

                        //添加各Ecu系統群組之通訊格式
                        EcuSystemAndCommunicationTypeDict.Add(ecuSystemGroup, ecuGroupDict[ecuId].EcuCommunicationType);
                    }
                    else
                    {
                        //添加各Ecu系統群組之涵蓋EcuID
                        EcuSystemAndIdListDict[ecuSystemGroup].Add(ecuId);

                        //添加各Ecu系統群組之通訊格式
                        EcuSystemAndCommunicationTypeDict[ecuSystemGroup] = ecuGroupDict[ecuId].EcuCommunicationType;
                    }
                }
            }

            //隱藏系統沒用到的系統圖示，並設置event 
            foreach (var RadioButton in ecuSystemRadioButtonDict.Values)
            {
                var ecuSystemGroup = ConvertRadioButtonTagToEcuSystemGroup((int)RadioButton.Tag);
                if (!EcuSystemAndIdListDict.ContainsKey(ecuSystemGroup))
                    //RadioButton.Visibility = ViewStates.Gone; //Android
                    RadioButton.Hidden = true;
                else
                {
                    //event
                    RadioButton.TouchUpInside += EcuSystemOnCheckedChanged;

                    //將第一個偵測到的系統Toggle Button高亮
                    if (!IsFirstInitRadioButtonChecked)
                    {
                        IsFirstInitRadioButtonChecked = true;
                        //RadioButton.Checked = true; //Android
                        RadioButton.Highlighted = true;
                    }
                }
            }


        }


        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();

        }

        public override void setCurrentPageName()
        {

        }

        [Export("numberOfSectionsInTableView:")]
        public nint NumberOfSections(UITableView tableView)
        {
            //Console.WriteLine("NumberOfSections()");
            return 1;
        }

        public nint RowsInSection(UITableView tableView, nint section)
        {
            return mList.Count;
        }

        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (NewDTCTableViewCell)tableView.DequeueReusableCell("dtcCell");
            if (cell == null)
            {
                var defaultCell = new UITableViewCell(UITableViewCellStyle.Default, "dtcCell");
                defaultCell.TextLabel.Text = "P1108";
                defaultCell.DetailTextLabel.Text = "OG Anunoby addresses the media following Toronto's 129-120 win over Washington on Wednesday.";

                return defaultCell;
            }
            else
            {
                cell.Title.Text = mList[indexPath.Row].DtcCodeForDisplay;
                cell.SubTitle.Text = mList[indexPath.Row].DtcName;
                cell.Title.Font = UIFont.SystemFontOfSize(Main.PHONE_SIZE_TABLE_TITLE);
                cell.SubTitle.Font = UIFont.SystemFontOfSize(Main.PHONE_SIZE_TABLE_CONTENT);
            }
            return cell;
        }

        [Export("tableView:heightForRowAtIndexPath:")]
        public nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return Main.PHONE_SIZE_DTC_TABLE_CELLHEIGHT;
        }

        /// <summary>
        /// 選擇DTC類型時之響應函數
        /// 當選擇完DTC類型後，會將選擇之DTC類型通訊指令，設定給DataModel資料模組的當前DTC通訊指令變數
        /// </summary>
        /// <param name="group"></param>
        /// <param name="checkedId"></param>
        public void DtcTypeOnCheckedChanged(object sender, EventArgs e)
        {
            UISegmentedControl seg = (UISegmentedControl)sender;
            #region 設定DTC通訊指令
            switch (seg.SelectedSegment)
            {
                case 0:
                    if (currentDtcCmdList != null &&
                        currentDtcCmdList.Count > 0)
                        StateMachine.DataModel.CurrentDtcCmdId = currentDtcCmdList[0].DtcCmd;
                    break;

                case 1:
                    if (pendingDtcCmdList != null &&
                        pendingDtcCmdList.Count > 0)
                        StateMachine.DataModel.CurrentDtcCmdId = pendingDtcCmdList[0].DtcCmd;
                    break;

                case 2:
                    if (historyDtcCmdList != null &&
                        historyDtcCmdList.Count > 0)
                        StateMachine.DataModel.CurrentDtcCmdId = historyDtcCmdList[0].DtcCmd;
                    break;

                default:
                    break;
            }
            #endregion
        }


        /// <summary>
        /// 選擇Ecu系統時之響應函數
        /// </summary>
        public void EcuSystemOnCheckedChanged(object sender ,EventArgs e)
        {
            UIButton btnView = (UIButton)sender;

            #region 系統通訊類別，設定DataModel之當前使用EcuID與改變Dtc類型之UI
            List<uint> systemEcuIdList = null;

            var ecuGroup = ConvertRadioButtonTagToEcuSystemGroup((int)btnView.Tag);
            var communicationType = EcuSystemAndCommunicationTypeDict[ecuGroup];

            systemEcuIdList = EcuSystemAndIdListDict[ecuGroup];
            if (systemEcuIdList == null ||
                systemEcuIdList.Count <= 0)
                return;

            #region 設定DTC通訊指令
            //var dtcCmdsDict = StateMachine.DataModel.DtcCmdsDict;
            //var dtcCmdId = dtcCmdsDict[currentSelectedEcuId][0].DtcCmd;
            //StateMachine.DataModel.CurrentDtcCmdId = dtcCmdId;
            #endregion

            var dtcCmdsDict = StateMachine.DataModel.DtcCmdsDict;
            //改變DTC Type UI
            foreach (var currentSelectedEcuId in systemEcuIdList)
            {
                if (dtcCmdsDict.ContainsKey(currentSelectedEcuId))
                {
                    StateMachine.DataModel.EcuID = currentSelectedEcuId;
                    ChangeDtcTypeUI(currentSelectedEcuId);
                    break;
                }
            }



            #endregion
        }


        #region 多ECU系統UI相關程式碼
        /// <summary>
        /// Ecu系統名稱與Toggle Button之字典檔
        /// Key : EcuSystemGroup.ToString() ; Value : Toggle Button
        /// </summary>
        Dictionary<String, UIButton> ecuSystemRadioButtonDict = new Dictionary<string, UIButton>();

        /// <summary>
        /// 通訊完畢後，各Ecu系統群組所涵蓋的EcuID列表
        /// </summary>
        Dictionary<EcuSystemGroup, List<uint>> EcuSystemAndIdListDict = new Dictionary<EcuSystemGroup, List<uint>>();

        /// <summary>
        /// 通訊完畢後，各Ecu系統群組所屬之通訊格式
        /// </summary>
        Dictionary<EcuSystemGroup, EcuCommunicationType> EcuSystemAndCommunicationTypeDict = new Dictionary<EcuSystemGroup, EcuCommunicationType>();

        /// <summary>
        /// 當前所選Ecu系統群組所涵蓋之EcuID列表
        /// </summary>
        List<uint> currentChoiceEcuIdList = new List<uint>();


        /// <summary>
        /// 是否已初始化使某Toggle Button高亮
        /// </summary>
        bool IsFirstInitRadioButtonChecked = false;

        /// <summary>
        /// 目前存在的Ecu系統群組
        /// </summary>
        List<EcuSystemGroup> ExistEcuSystemGroup
        {
            get
            {
                return EcuSystemAndIdListDict.Keys.ToList();
            }
        }

        /// <summary>
        /// 當前使用的通訊類別
        /// </summary>
        EcuCommunicationType CurrentUsedCommunicationType = EcuCommunicationType.None;


       
      



        /// <summary>
        /// 將設定的Toggle Button Tag轉換為Ecu系統群組ID
        /// </summary>
        /// <param name="RadioButtonViewTag">Toggle Button的Tag</param>
        /// <returns>轉換完畢的Ecu系統群組ID</returns>
        private uint ConvertRadioButtonTagToEcuSystemGroupID(int RadioButtonViewTag)
        {
            uint ecuSystemGroupId = 0;

            ecuSystemGroupId = (uint)RadioButtonViewTag;
                
            return ecuSystemGroupId;
        }

        /// <summary>
        /// 將設定的Toggle Button Tag轉換為Ecu系統群組列舉
        /// </summary>
        /// <param name="RadioButtonViewTag">Toggle Button的Tag</param>
        /// <returns>轉換完畢的Ecu系統群組列舉</returns>
        private EcuSystemGroup ConvertRadioButtonTagToEcuSystemGroup(int RadioButtonViewTag)
        {
            var ecuSystemGroupId = ConvertRadioButtonTagToEcuSystemGroupID(RadioButtonViewTag);
            var ecuSystemGroup = EcuSystemGroup.None;
            try
            {
                ecuSystemGroup = (EcuSystemGroup)ecuSystemGroupId;
            }
            catch (Exception ex)
            {
                ecuSystemGroup = EcuSystemGroup.None;
            }
            return ecuSystemGroup;
        }


        /// <summary>
        /// 目前啟動中的ECU系統數量屬性
        /// </summary>
        private int CurrentEnabledSystemCount
        {
            get
            {
                return currentUsedEcuSystemAndCommunicationTypeDict.Count;
            }
        }

        public IList<DtcData> Value
        {
            set
            {
                mList.Clear();
                int i = 0;
                foreach (var item in value)
                {
                    mList.Add(item);
                }

                if (ContainerViewController.Instance.dtcViewController != null)
                {
                    DispatchQueue.MainQueue.DispatchAsync(() =>
                    {
                        if (table != null)
                            table.ReloadData();
                    });
                }
            }
            get
            {
                return mList;
            }
        }

        public void Refresh(List<DtcData> dtcDatas)
        {
            Value = dtcDatas;
        }

        /// <summary>
        /// 目前UI所選擇的Ecu系統與通訊格式字典檔
        /// </summary>
        Dictionary<EcuSystemGroup, EcuCommunicationType> currentUsedEcuSystemAndCommunicationTypeDict = new Dictionary<EcuSystemGroup, EcuCommunicationType>();
        /// <summary>
        /// 當前所選擇之CAN系統是否含有KLine系統
        /// </summary>
        private bool isCANSystemHasKLine = false;
        private bool IsCANSystemHasKLine
        {
            get
            {
                var canSystemCount = currentUsedEcuSystemAndCommunicationTypeDict.Values.Where(x => x == EcuCommunicationType.CAN).Count();
                var klineSystemCount = currentUsedEcuSystemAndCommunicationTypeDict.Values.Where(x => x == EcuCommunicationType.KLine).Count();

                if (canSystemCount > 0)
                    CurrentUsedCommunicationType = EcuCommunicationType.CAN;
                else if (canSystemCount == 0 &&
                    klineSystemCount == 1)
                    CurrentUsedCommunicationType = EcuCommunicationType.KLine;
                else if (canSystemCount == 0 &&
                    klineSystemCount == 0)
                    CurrentUsedCommunicationType = EcuCommunicationType.None;


                if (klineSystemCount <= 0)
                    isCANSystemHasKLine = false;
                else
                    isCANSystemHasKLine = true;

                return isCANSystemHasKLine;
            }
        }

        /// <summary>
        /// Ecu系統通訊類型判斷邏輯函數
        /// 判別當前所選擇的Ecu系統之通訊格式，與當前Ecu系統之通訊格式是否有不同
        /// </summary>
        /// <returns>返回false表示不須切換通訊格式，反為true表示需要切換通訊格式</returns>
        private bool IsNeedToChangeEcuCommunicationType(EcuCommunicationType choiceSystemCommunicationType)
        {
            bool ret = false;
            //如果當前沒有Ecu系統選擇，通訊類型改為None
            if (CurrentEnabledSystemCount <= 0)
            {
                CurrentUsedCommunicationType = EcuCommunicationType.None;
                currentUsedEcuSystemAndCommunicationTypeDict.Clear();
                isCANSystemHasKLine = false;
            }

            //當前選擇的系統如果為CAN，直接略過判斷；為KLine的話，需經過屬性判斷
            if (choiceSystemCommunicationType == EcuCommunicationType.CAN)
                ret = false;
            else
                ret = IsCANSystemHasKLine;
            return ret;
        }

        /// <summary>
        /// 當選擇RadioButton後之主響應函數
        /// </summary>
        /// <param name="group"></param>
        /// <param name="checkedId"></param>
        public void OnCheckedChanged(int checkedId)
        {
      
        }

        /// <summary>
        /// 選擇Ecu系統時之響應函數
        /// </summary>
        /// <param name="group"></param>
        /// <param name="checkedId"></param>
        public void EcuSystemOnCheckedChanged(int checkedId)
        {
            //TODO 改 - 待確認
            Console.WriteLine("checkedId : " + checkedId);
            //RadioButton buttonView = group.FindViewById<RadioButton>(checkedId);
            //if (buttonView == null)
            //    return;

            ////在此響應中改變RadioButton Checked的屬性值，不會再重複觸發此響應
            //ecuSystemRadioButtonDict[EcuSystemGroup.Engine.ToString()].Checked = false;
            //bool isChecked = buttonView.Checked;
            //var currentRadioButtonId = buttonView.Id;


            #region 系統通訊類別，設定DataModel之當前使用EcuID與改變Dtc類型之UI
            List<uint> systemEcuIdList = null;
            //var ecuGroup = ConvertRadioButtonTagToEcuSystemGroup(buttonView.Tag.ToString());
            //var communicationType = EcuSystemAndCommunicationTypeDict[ecuGroup];

            //systemEcuIdList = EcuSystemAndIdListDict[ecuGroup];
            if (systemEcuIdList == null ||
                systemEcuIdList.Count <= 0)
                return;

            #region 設定DTC通訊指令

            #endregion

            var dtcCmdsDict = StateMachine.DataModel.DtcCmdsDict;
            //改變DTC Type UI
            foreach (var currentSelectedEcuId in systemEcuIdList)
            {
                if (dtcCmdsDict.ContainsKey(currentSelectedEcuId))
                {
                    StateMachine.DataModel.EcuID = currentSelectedEcuId;
                    ChangeDtcTypeUI(currentSelectedEcuId);
                    break;
                }
            }

            #endregion

        }

        /// <summary>
        /// 選擇DTC類型時之響應函數
        /// 當選擇完DTC類型後，會將選擇之DTC類型通訊指令，設定給DataModel資料模組的當前DTC通訊指令變數
        /// </summary>
        /// <param name="group"></param>
        /// <param name="checkedId"></param>
        public void DtcTypeOnCheckedChangedint (int checkedId)
        {
            //TODO 改 - 待確認
            #region 設定DTC通訊指令
            switch (checkedId)
            {
                //case Resource.Id.rbCurrentDtc:
                //    if (currentDtcCmdList != null &&
                //        currentDtcCmdList.Count > 0)
                //        StateMachine.DataModel.CurrentDtcCmdId = currentDtcCmdList[0].DtcCmd;
                //    break;

                //case Resource.Id.rbPendingDtc:
                //    if (pendingDtcCmdList != null &&
                //        pendingDtcCmdList.Count > 0)
                //        StateMachine.DataModel.CurrentDtcCmdId = pendingDtcCmdList[0].DtcCmd;
                //    break;

                //case Resource.Id.rbHistoryDtc:
                //    if (historyDtcCmdList != null &&
                //        historyDtcCmdList.Count > 0)
                //        StateMachine.DataModel.CurrentDtcCmdId = historyDtcCmdList[0].DtcCmd;
                //    break;

                //default:
                //    break;
            }
            #endregion
        }

        /// <summary>
        /// 用以儲存當前選擇之Current DTC通訊指令相關資訊
        /// </summary>
        List<DtcCmdInfo> currentDtcCmdList = null;
        /// <summary>
        /// 用以儲存當前選擇之Pending DTC通訊指令相關資訊
        /// </summary>
        List<DtcCmdInfo> pendingDtcCmdList = null;
        /// <summary>
        /// 用以儲存當前選擇之History DTC通訊指令相關資訊
        /// </summary>
        List<DtcCmdInfo> historyDtcCmdList = null;


        /// <summary>
        /// 藉由選擇系統後，透過系統之EcuID來改變DTC各類型UI呈現或是不呈現
        /// </summary>
        /// <param name="currentEcuId"></param>
        private void ChangeDtcTypeUI(uint currentEcuId)
        {
            //TODO 改 - 待確認
            //if (rbCurrentDtc == null ||
            //    rbPendingDtc == null ||
            //    rbHistoryDtc == null)
            //    return;

            var dtcCmdsDict = StateMachine.DataModel.DtcCmdsDict;
            if (!dtcCmdsDict.ContainsKey(currentEcuId))
                return;

            var dtcCmdIdList = dtcCmdsDict[currentEcuId];

            #region 建立DTC類型初始值與UI預設選項
            bool isInitedDefaultDtcType = false;

            currentDtcCmdList = dtcCmdIdList.Where(x => x.DtcType == IcmLib.DtcType.Current).ToList();
            if (!isInitedDefaultDtcType && currentDtcCmdList.Count > 0)
            {
                isInitedDefaultDtcType = true;
                //TODO 改 - 待確認
                //rbCurrentDtc.Checked = true;

                //UI預設為Current DTC設置完畢，強制切換為Current DTC指令
                if (currentDtcCmdList != null &&
                        currentDtcCmdList.Count > 0)
                    StateMachine.DataModel.CurrentDtcCmdId = currentDtcCmdList[0].DtcCmd;
            }

            pendingDtcCmdList = dtcCmdIdList.Where(x => x.DtcType == IcmLib.DtcType.Pending).ToList();
            if (!isInitedDefaultDtcType && pendingDtcCmdList.Count > 0)
            {
                isInitedDefaultDtcType = true;
                //TODO 改 - 待確認
                //rbPendingDtc.Checked = true;
                //UI預設為Pending DTC設置完畢，強制切換為Pending DTC指令
                if (pendingDtcCmdList != null &&
                        pendingDtcCmdList.Count > 0)
                    StateMachine.DataModel.CurrentDtcCmdId = pendingDtcCmdList[0].DtcCmd;
            }

            historyDtcCmdList = dtcCmdIdList.Where(x => x.DtcType == IcmLib.DtcType.History).ToList();
            if (!isInitedDefaultDtcType && historyDtcCmdList.Count > 0)
            {
                isInitedDefaultDtcType = true;
                //TODO 改 - 待確認
                //rbHistoryDtc.Checked = true;
                //UI預設為History DTC設置完畢，強制切換為History DTC指令
                if (historyDtcCmdList != null &&
                        historyDtcCmdList.Count > 0)
                    StateMachine.DataModel.CurrentDtcCmdId = historyDtcCmdList[0].DtcCmd;
            }
            #endregion

            #region 各功能UI是否可用
            //TODO 改 - 待確認
            //if (currentDtcCmdList.Count > 0)
            //    rbCurrentDtc.Enabled = true;
            //else
            //    rbCurrentDtc.Enabled = false;

            //if (pendingDtcCmdList.Count > 0)
            //    rbPendingDtc.Enabled = true;
            //else
            //    rbPendingDtc.Enabled = false;

            //if (historyDtcCmdList.Count > 0)
            //    rbHistoryDtc.Enabled = true;
            //else
            //    rbHistoryDtc.Enabled = false;
            #endregion
        }
        #endregion

        /// <summary>
        /// 從V.Dialogue收到的DTC原始資料為轉換0x18 DecodeType解碼格式
        /// 例如 : F1 00 0D 02 01 00 00 00 00 11 (LSB -> MSB)
        /// F1 => DTC指令回覆前綴
        /// 00 => DTC狀態00表示成功；其餘為失敗
        /// 0D => 0000 1101 => 第0個Byte的第0,2,4個Bit有資料(bit為1) => 0001,0004,0008
        /// 02 => 0000 0010 => 第1個Byte的第1個Bit有資料(bit為1) => 0102
        /// 01 => 0000 0001 => 第2個Byte的第0個Bit有資料(bit為1) => 0201
        /// 00 => 0000 0000 => 第3個Byte，無任何資料
        /// 00 => 0000 0000 => 第4個Byte，無任何資料
        /// 00 => 0000 0000 => 第5個Byte，無任何資料
        /// 00 => 0000 0000 => 第6個Byte，無任何資料
        /// 01 => 0001 0001 => 第7個Byte的第0個Bit有資料(bit為1) => 0701 0710
        /// 
        /// 解碼成對照表的資料 位元組陣列 (LSB -> MSB 兩兩一組)
        /// 0001 0004 0008 0102 0201 0701 0710
        /// </summary>
        /// <param name="dtcRawData"></param>
        /// <returns>返回值為 DTC指令回覆前綴(0xF1) + DTC狀態(0x00) + 轉換為高位元ByteIndex 低位元2^BitIndex數值(兩兩一組)</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        static public byte[] ConvertDtcRawDataToByteBitPosFormat(byte[] dtcRawData)
        {
            if (dtcRawData == null ||
                dtcRawData.Length < 2)
                return null;

            byte[] result = null;

            //模擬ECU ID = 35572 發送過來的DTC原始數據
            //uint ecuId = 35572;
            //dtcRawData = new byte[]{ 0xF1, 0x00, 0x00, 0x08, 0xFF, 0xFF, 0x01, 0x07, 0x0F, 0x5A };
            var dtcCmdPrefix = dtcRawData[0];
            var dtcStatus = dtcRawData[1];
            if (dtcStatus != 0)
                return null;

            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms, Encoding.BigEndianUnicode))
                {
                    writer.Write((byte)dtcCmdPrefix);
                    writer.Write((byte)dtcStatus);
                    for (int i = 2; i < dtcRawData.Length; i++)
                    {
                        int currentData = dtcRawData[i];
                        for (int j = 0; j < 8; j++)
                        {
                            var powData = (int)Math.Pow(2.0, j);
                            var hasData = currentData & powData;

                            if (hasData != 0)
                            {
                                writer.Write((byte)(i - 2));
                                writer.Write((byte)powData);
                            }
                        }
                    }
                    result = ms.ToArray();
                }
            }

            return result;
        }
    }
}


﻿#define Debug時每秒顯示GPS位置
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using IcmComLib.Utils.iOS;
using iPhoneBLE.SRC;
using UIKit;
using Xamarin_SYM_IOS.ViewControllers;
using AVFoundation;
using IcmLib.Data;
using Xamarin_SYM_IOS.SRC.Utils;
using System.Diagnostics;
using IcmComLib_iOS.IcmComLib.Utils;
using MapKit;
using CoreLocation;
using static CoreFoundation.DispatchSource;
using System.Threading.Tasks;
using Xamarin_SYM_IOS.SRC.Model;

namespace Xamarin_SYM_IOS
{

	public struct SeguesID
	{
		public const string LiveDataGauges = "embedLiveDataGauges";
	}

	//放一些不會更動的變數
	public struct MyCustomPages
	{
		//新增頁面
		public const string First = "embedHome";
		public const string Second = "embedLog";
		public const string Third = "embedZero2Hundred";
		public const string Forth = "embedZero2HundredChart";
		public const string Fifth = "embedZero24Hundred";
		public const string Sixth = "embedZero24HundredChart";
		public const string Seventh = "embedLiveData";
		public const string Eighth = "embedLiveDataItems";
		public const string Ninth = "embedDTC";
		public const string Tenth = "embedDTCPage2";
        public const string NewDTC = "embedNewDTC";
        public const string Eleventh = "embedShift";
		public const string Twelfth = "embedValve";
		public const string LiveDataGauges = "embedLiveDataGauges";
		public const string Setting = "embedSetting";
		public const string Settingg = "embedSettingg";
		public const string ScanView = "embedScanView";
		public const string Log = "embedLogg";
		public const string BtnMenus = "embedMenu";
        public const string LiveData_2_Frame = "embedLiveData_2_Frame";
        public const string LiveData_4_Frame = "embedLiveData_4_Frame";
        public const string LiveData_6_Frame = "embedLiveData_6_Frame";
        public const string LiveDataIconMenu = "embedLiveDataIconMenu";
        public const string Maintenance = "embedMaintenance";
        public const string Upload = "embedUpload";
        public const string Map = "embedMap";
        public const string LvDataCloud = "embedLvDataCloud";
        public const string DataMonitor = "embedDataMonitor";
        //public const string Test = "embedTest";


        public static string previousButton;



		//所有呼叫頁面的按鈕
		public static Dictionary<string, string> buttonRespondSegueID = new Dictionary<string, string> {
				{"btnHome", First},
				{"btnLog", Log},
				{"btnZero2Hundred", Third},
				{"btnDisplay0_100Chart", Forth},
				{"btnZero24Hundred", Fifth},
				{"btnDisplay0_400Chart", Sixth},
				{"btnLiveData", Seventh},
				{"labelLeftTop", Eighth},
				{"labelRightTop", Eighth},
				{"labelLeftBottom", Eighth},
				{"labelRightBottom", Eighth},
				//{"labelLeftTop", BtnMenus},
				//{"labelRightTop", BtnMenus},
				//{"labelLeftBottom", BtnMenus},
				//{"labelRightBottom", BtnMenus},
				{"btnDTC", NewDTC},
				{"btnDTCPage2",Tenth},
				{"btnShift", Eleventh},
				{"btnValve", Twelfth},
				{"btnBack0_100",Third},
				{"btnBack0_400",Fifth},
				{"DtcCell",Tenth},
				{"btnBack_dtc",Ninth},
				{"gaugeView",LiveDataGauges},
				{"btnSetting",Settingg},
				{"btnScanView",ScanView},
                {"btnLiveDataIcon",LiveDataIconMenu},//LiveDataIcon模式選單
                {"LiveData_2_Frame",LiveData_2_Frame},
                {"LiveData_4_Frame",LiveData_4_Frame},
                {"LiveData_6_Frame",LiveData_6_Frame},
                {"btnMaintenance",Maintenance},
                {"btnUpload",Upload},
                {"btnMap",Map},
                {"btnLvDataCloud",LvDataCloud},
                {"DataMonitor",DataMonitor}

        };

		//更換按鈕的背景圖
		public static Dictionary<string, string> buttonBackground = new Dictionary<string, string> {
				{"btnHome", "btn_home" },
				{"btnLog","btn_log"},
				{"btnZero2Hundred","btn_zero2hundred"},
				{"btnZero24Hundred","btn_zero24hundred"},
				{"btnLiveData","btn_livedata"},
				{"btnDTC","btn_dtc"},
				{"btnShift","btn_shift"},
				{"btnValve","btn_valve"},
                {"btnLiveDataIcon","btn_livedataicon"},
                {"btnMaintenance","btn_maintenance"},
                {"btnUpload","btn_upload"},
                {"btnMap","btn_map"},
                {"btnLvDataCloud","btn_upload"}
        };

		public static Dictionary<string, string> buttonPressedBackground = new Dictionary<string, string> {
				{"btnHome", "btn_home_pressed" },
				{"btnLog","btn_log_pressed"},
				{"btnZero2Hundred","btn_zero2hundred_pressed"},
				{"btnZero24Hundred","btn_zero24hundred_pressed"},
				{"btnLiveData","btn_livedata_pressed"},
				{"btnDTC","btn_dtc_pressed"},
				{"btnShift","btn_shift_pressed"},
				{"btnValve","btn_valve_pressed"},
				{"btnLiveDataIcon","btn_livedataicon_pressed"},
                {"btnMaintenance","btn_maintenance_pressed"},
                {"btnUpload","btn_upload_pressed"},
                {"btnMap","btn_map_pressed"},
                {"btnLvDataCloud","btn_upload_pressed"}
        };



        public static Dictionary<string, Page> buttonRespondPage = new Dictionary<string, Page>
        {
            {"btnHome", Page.Home },
            {"btnLog",Page.Log},
            {"btnZero2Hundred",Page.Speed0_100},
            {"btnZero24Hundred",Page.Speed0_400},
            {"btnLiveData",Page.LiveData},
            {"btnDTC",Page.DTC},
            {"btnShift",Page.Shift},
            {"btnValve",Page.Valve},
            {"btnLiveDataIcon",Page.LiveDataIconMenu},
            {"btnMaintenance",Page.Maintenance},
            {"btnUpload",Page.Upload},
            {"btnMap",Page.Map},
            {"btnLvDataCloud",Page.LvCloud},
            {"btnDataMonitor",Page.LvCloud_Show}
            //{"btnLiveData_2_Frame",Page.LiveData_2_Frame},
            //{"btnLiveData_4_Frame",Page.LiveData_4_Frame},
            //{"btnLiveData_6_Frame",Page.LiveData_6_Frame}
		};


	}

    //<<<<<<<<<<<<<<<< >> >> >> >> >>>>>>>>>>>>>>>>>>>>
    //<<<<<<<<<<<<<<< 頁 面 管 理 者 >>>>>>>>>>>>>>>>>>>>
    //<<<<<<<<<<<<<<<< >> >> >> >> >>>>>>>>>>>>>>>>>>>>
    public partial class ContainerViewController : CustomViewController, LiveDataIconViewController.OnBtnSendClickListener,ICLLocationManagerDelegate
    {
       

        private static bool isAlive = false;
        private UIViewController currentController;
        public static ContainerViewController Instance;

        private static Dictionary<string, UIButton> buttonBoxes = new Dictionary<string, UIButton>();
        public Dictionary<string, UIButton> ButtonBoxes  { get => buttonBoxes; }
        private static Dictionary<string, UIViewController> destinationDictionary = new Dictionary<string, UIViewController>();

        private UIButton currentButton;
        private bool isDefultPressed = true;
        private bool transitionInProgress = false;

        public HomeViewController homeViewController;
        public LoggViewController loggViewController;
        public Zero2HundredViewController zero2HundredViewController;
        public Zero2HundredPage2ViewController zero2HundredPage2ViewController;
        public Zero24HundredViewController zero24HundredViewController;
        public Zero24HundredPage2ViewController zero24HundredPage2ViewController;
        public LiveDataViewController liveDataViewController;
        public LiveDataItemsViewController liveDataItemsViewController;
        public LiveDataGaugesViewController liveDataGaugesViewController;
        //public DTCViewController dtcViewController;
        public NewDTCViewController dtcViewController;
        public ShiftViewController shiftViewController;
        public ValveViewController valveViewController;
        public SettinggViewController settinggViewController;
        public ScanViewController scanViewController;
        public LiveData_2_Frame_ViewController liveData_2_Frame_ViewController;
        public LiveData_4_Frame_ViewController liveData_4_Frame_ViewController;
        public LiveData_6_Frame_ViewController liveData_6_Frame_ViewController;
        public LiveDataIconViewController liveDataIconViewController;

        public OdoNavViewController odoNavViewController;
        public UploadViewController uploadViewController;
        public MapViewController mapViewController;
        public LvDataCloudViewController lvDataCloudViewController;
        public DataMonitorViewController dataMonitorViewController;
        public RemoteDiagViewController remoteDiagViewController;

        //public AppAttribute.RunningMode APP_RUNNING_MODE = AppAttribute.RunningMode.Normal;
        public StateMachine stateMachine = null;
        public SYMSharedPreferencesExtractor spf = new SYMSharedPreferencesExtractor();
        public bool isStateMachineActived = true; //關閉通訊用的旗標
        private static string targetDeviceName = "V.Dialogue_BLE";
        public string TargetDeviceName { get => targetDeviceName; }
      
		private CGRect SCREEN_SIZE = UIScreen.MainScreen.Bounds;

        /**liveDataicon 傳id用 */
        public List<int> liveDataID_For_IconPage = new List<int>();
		

        public List<int> LiveDataID_For_IconPage {
            get => liveDataID_For_IconPage;
            set => liveDataID_For_IconPage = value;
        }

        /**liveDataicon 傳圖片name用 */
        private List<string> LvGaugesBG;

		

		private AVPlayer player;
		private AVPlayerLayer playerLayer;
		private UIView videoBG;

		private float fuelConsumptionCompensationPercent;
		public float FuelConsumptionCompensationPercent
		{
			get => fuelConsumptionCompensationPercent;
			set => fuelConsumptionCompensationPercent = value;
		}

        private IList<DtcData> mDTCList = new List<DtcData>();
        public IList<DtcData> DTCValue
        {
            set
            {
                mDTCList.Clear();

                foreach (var item in value)
                {
                    mDTCList.Add(item);
                }

                //DispatchQueue.MainQueue.DispatchAsync(() =>
                //{
                //    table.ReloadData();
                //});
            }

            get
            {
                return mDTCList;
            }
        }

        private uint mEcID;
        public uint EcuID
        {
            set
            {
                mEcID = value;
            }
            get
            {
                return mEcID;
            }
        }

        private CLLocationManager locationManager;
        public CLLocationManager LocationManager { get => locationManager; }

		void BtnSetting_TouchUpInside(object sender, EventArgs e)
		{
			this.PerformSegue(MyCustomPages.Settingg, null);
		}

		void ClickEvent(string buttonName)
		{
			
			
			switch (buttonName)
			{
				
                case "btnDTC:":
					
                    break;

				case "btnLog":
					
					break;
                case "btnOdo":
					
					
                    break;
			}
		}

        private void EnableButtons()
        {
          
            DispatchQueue.MainQueue.DispatchAfter(new DispatchTime(DispatchTime.Now, 2 * 1000000000), () => {
                foreach (UIButton btn in buttonBoxes.Values)
                {
                    btn.Enabled = true;
                }
            });
        }

        private void DisableOtherButton(UIButton curButton)
        {
            foreach(UIButton btn in buttonBoxes.Values) {
                if (!btn.Equals(curButton))
                {
                    DispatchQueue.MainQueue.DispatchAsync(() => {
                        btn.Enabled = false;
                    });
                }
                //Console.WriteLine("button : " + b);
            }
            EnableButtons();
            //Task.Run(() => {
                //NSTimer.CreateScheduledTimer(TimeSpan.FromSeconds(10),EnableButtons);
            //});
           
        }

		private void ButtonPressed(object sender, EventArgs e)
		{
            
			//第一次預設跑這裡
			if (isDefultPressed)
			{
				currentButton = (UIButton)sender;
				currentButton.SetBackgroundImage(UIImage.FromFile("btn_home_pressed.png"), UIControlState.Normal);
				currentButton.Selected = true;
				this.PerformSegue(MyCustomPages.buttonRespondSegueID[currentButton.RestorationIdentifier], null);
			}
			else
			{
				//第一件事: 先做換背景圖 ,順便判斷如果已經按過就returnd不做事
				if (!((UIButton)sender).Selected)
				{
					string pressedButtonID = ((UIButton)sender).RestorationIdentifier;
					string currentButtonID = currentButton.RestorationIdentifier;
					if (pressedButtonID != null && currentButtonID != null)
					{
						if (currentButtonID != pressedButtonID)
						{
							if (StateMachine.IsActivted)
							{
								//叫statemachine用Queue送訊息
								//ClickEvent(pressedButtonID);
								
                                //重新設定currentpage，跑updateTimerValue用
								StateMachine.UIModel.CurrentPage = MyCustomPages.buttonRespondPage[pressedButtonID];
							}
							//把上一個按鈕的背景圖還原
							buttonBoxes[currentButtonID].SetBackgroundImage(UIImage.FromFile(MyCustomPages.buttonBackground[currentButtonID]),
																			UIControlState.Normal);
							//參考到新按的按鈕,並重設isSelected
							currentButton.Selected = false;
							currentButton = (UIButton)sender;
							currentButton.Selected = true;

							//更換目前點擊按鈕的背景圖
							buttonBoxes[pressedButtonID].SetBackgroundImage(UIImage.FromFile(MyCustomPages.buttonPressedBackground[pressedButtonID]),
																			UIControlState.Normal);

							//第二件事: 執行換頁
							//根據button的id拿到對應的頁面id,然後使用換頁的fuction
							this.PerformSegue(MyCustomPages.buttonRespondSegueID[pressedButtonID], null);
						}
					}
                    //DisableOtherButton((UIButton)sender);
				}
				else
				{
					return;
				}
			}
		}

		private void FirstTimeViewWillDo(UIStoryboardSegue segue)
		{
			//初始化賦予值
			homeViewController = (HomeViewController)segue.DestinationViewController;

			destinationDictionary.Add(MyCustomPages.First, homeViewController);

			//加入子child
			this.AddChildViewController(segue.DestinationViewController);

			//第一次是自己要load home畫面,沒有要to的VC,所以用這方法產生畫面
			UIView destinationView = segue.DestinationViewController.View;
			if (destinationView != null)
			{
				destinationView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;

				destinationView.Frame = new CGRect(0, SCREEN_SIZE.Height / 5.2, destinationView.Frame.Size.Width, destinationView.Frame.Size.Height);
				this.View.AddSubview(destinationView);
				segue.DestinationViewController.DidMoveToParentViewController(this);
			}


		}



		private void SecondTimeViewWillDo(UIStoryboardSegue segue)
		{
			//主要做初始化
			switch (segue.Identifier)
			{
				case MyCustomPages.First:
					break;
				case MyCustomPages.Log:
					loggViewController = (LoggViewController)segue.DestinationViewController;
					destinationDictionary.Add(MyCustomPages.Log, loggViewController);
					break;
				case MyCustomPages.Third:
					zero2HundredViewController = (Zero2HundredViewController)segue.DestinationViewController;
					destinationDictionary.Add(MyCustomPages.Third, zero2HundredViewController);
					break;
				case MyCustomPages.Forth:
					zero2HundredPage2ViewController = (Zero2HundredPage2ViewController)segue.DestinationViewController;
					destinationDictionary.Add(MyCustomPages.Forth, zero2HundredPage2ViewController);
					break;
				case MyCustomPages.Fifth:
					zero24HundredViewController = (Zero24HundredViewController)segue.DestinationViewController;
					destinationDictionary.Add(MyCustomPages.Fifth, zero24HundredViewController);
					break;
				case MyCustomPages.Sixth:
					zero24HundredPage2ViewController = (Zero24HundredPage2ViewController)segue.DestinationViewController;
					destinationDictionary.Add(MyCustomPages.Sixth, zero24HundredPage2ViewController);
					break;
				case MyCustomPages.Seventh:
					liveDataViewController = (LiveDataViewController)segue.DestinationViewController;
					destinationDictionary.Add(MyCustomPages.Seventh, liveDataViewController);
					break;
				case MyCustomPages.Eighth:
					liveDataItemsViewController = (LiveDataItemsViewController)segue.DestinationViewController;
					destinationDictionary.Add(MyCustomPages.Eighth, liveDataItemsViewController);
					break;
				case MyCustomPages.NewDTC:
					dtcViewController = (NewDTCViewController)segue.DestinationViewController;
					destinationDictionary.Add(MyCustomPages.NewDTC, dtcViewController);
					break;
				case MyCustomPages.Eleventh:
					shiftViewController = (ShiftViewController)segue.DestinationViewController;
					destinationDictionary.Add(MyCustomPages.Eleventh, shiftViewController);
					break;
				case MyCustomPages.Twelfth:
					valveViewController = (ValveViewController)segue.DestinationViewController;
					destinationDictionary.Add(MyCustomPages.Twelfth, valveViewController);
					break;
				case SeguesID.LiveDataGauges:
					liveDataGaugesViewController = (LiveDataGaugesViewController)segue.DestinationViewController;
					destinationDictionary.Add(SeguesID.LiveDataGauges, liveDataGaugesViewController);
					break;
				case MyCustomPages.ScanView:
					scanViewController = (ScanViewController)segue.DestinationViewController;
					destinationDictionary.Add(MyCustomPages.ScanView, scanViewController);
					break;
				case MyCustomPages.Settingg:
					settinggViewController = (SettinggViewController)segue.DestinationViewController;
					destinationDictionary.Add(MyCustomPages.Settingg, settinggViewController);
					break;
                case MyCustomPages.LiveData_2_Frame:
					liveData_2_Frame_ViewController = (LiveData_2_Frame_ViewController)segue.DestinationViewController;
                    liveData_2_Frame_ViewController.SetLiveDataID(LiveDataID_For_IconPage);
                    liveData_2_Frame_ViewController.SetLiveDataIconImage(LvGaugesBG);
                    destinationDictionary.Add(MyCustomPages.LiveData_2_Frame, liveData_2_Frame_ViewController);
					break;
				case MyCustomPages.LiveData_4_Frame:
					liveData_4_Frame_ViewController = (LiveData_4_Frame_ViewController)segue.DestinationViewController;
					liveData_4_Frame_ViewController.SetLiveDataID(LiveDataID_For_IconPage);
                    liveData_4_Frame_ViewController.SetLiveDataIconImage(LvGaugesBG);
					destinationDictionary.Add(MyCustomPages.LiveData_4_Frame, liveData_4_Frame_ViewController);
					break;
				case MyCustomPages.LiveData_6_Frame:
					liveData_6_Frame_ViewController = (LiveData_6_Frame_ViewController)segue.DestinationViewController;
					liveData_6_Frame_ViewController.SetLiveDataID(LiveDataID_For_IconPage);
                    liveData_6_Frame_ViewController.SetLiveDataIconImage(LvGaugesBG);
					destinationDictionary.Add(MyCustomPages.LiveData_6_Frame, liveData_6_Frame_ViewController);
					break;
                case MyCustomPages.LiveDataIconMenu:
                    liveDataIconViewController = (LiveDataIconViewController)segue.DestinationViewController;
                    liveDataIconViewController.SetOnBtnSendClickListener(this);
					destinationDictionary.Add(MyCustomPages.LiveDataIconMenu, liveDataIconViewController);
					break;
   
                case MyCustomPages.Maintenance:
                    odoNavViewController = (OdoNavViewController)segue.DestinationViewController;
                    destinationDictionary.Add(MyCustomPages.Maintenance, odoNavViewController);
                    break;
                case MyCustomPages.Upload:
                    uploadViewController = (UploadViewController)segue.DestinationViewController;
                    destinationDictionary.Add(MyCustomPages.Upload, uploadViewController);
                    break;
                case MyCustomPages.Map:
                    mapViewController = (MapViewController)segue.DestinationViewController;
                    destinationDictionary.Add(MyCustomPages.Map, mapViewController);
                    break;
                case MyCustomPages.LvDataCloud:
                    lvDataCloudViewController = (LvDataCloudViewController)segue.DestinationViewController;
                    destinationDictionary.Add(MyCustomPages.LvDataCloud, lvDataCloudViewController);
                    break;
                case MyCustomPages.DataMonitor:
                    dataMonitorViewController = (DataMonitorViewController)segue.DestinationViewController;
                    destinationDictionary.Add(MyCustomPages.DataMonitor, dataMonitorViewController);
                    break;
            }


		}

		public void SwapFromViewController(UIViewController from, UIViewController to, string segueID)
		{
			if (from != null)
			{
				if (segueID != null &&
				   segueID != MyCustomPages.ScanView &&
				   segueID != MyCustomPages.LiveDataGauges &&
				   segueID != MyCustomPages.Eighth &&
				   segueID != MyCustomPages.Settingg)
					MyCustomPages.previousButton = segueID;

                //如果是liveDataDialog的話
                if (to.RestorationIdentifier == "DialogLiveDataItems" ||
                     to.RestorationIdentifier == "DialogLiveDataGauges" ||
                     to.RestorationIdentifier == "ScanView")
                {
                    //new一塊dialog大小的frame,但這一塊會在tablview底下
                    to.View.Frame = new CGRect(Main.SCREEN_SIZE.Width / 20.7, Main.SCREEN_SIZE.Height / 18.4
                                               , Main.SCREEN_SIZE.Width / 1.11, Main.SCREEN_SIZE.Height / 1.1);//670
                    to.View.Layer.CornerRadius = 12.0f;
                    //to.View.RestorationIdentifier = "dialog";
                    this.AddChildViewController(to);

                    //加入一塊透明的view在背後,防止後面的的物件被響應到
                    var transParentView = new UIView();
                    transParentView.Frame = new CGRect(0, 0, 520, 736);
                    transParentView.BackgroundColor = new UIColor(0.0f, 0.0f);
                    //transParentView.RestorationIdentifier = "transParentBackground";

                    //透明的先加,再加dialog
                    this.View.AddSubview(transParentView);
                    this.View.AddSubview(to.View);

                    //故意先設0,在用動畫跑到1,就會有淡入效果
                    to.View.Alpha = 0;

                    //加個動畫,讓它看起來很假掰
                    UIView.Animate(0.1, () =>
                    {
                        from.View.Alpha = 1.0f; to.View.Alpha = 1;
                    });
                    to.DidMoveToParentViewController(this);
                    this.transitionInProgress = false;
                }

                else if (to.RestorationIdentifier == "DialogSettinggs")
                {
                    to.View.Frame = new CGRect(0, 0, to.View.Frame.Size.Width, to.View.Frame.Size.Height);
                    //先call willMove
                    from.WillMoveToParentViewController(null);
                    //加入目的地子View
                    this.AddChildViewController(to);
                    //正式轉場 ,最後的閉包的參數finished什麼時機用不清楚,抑或是只是一種含義,須研究
                    this.Transition(from,
                                    to,
                                    0.1,
                                    UIViewAnimationOptions.TransitionCrossDissolve,
                                    () => { },//這裡寫null會crash,swift就不會
                                    (finished) =>
                                    {
                                        from.RemoveFromParentViewController();
                                        to.DidMoveToParentViewController(this);
                                        this.transitionInProgress = false;
                                    });



                    //from.WillMoveToParentViewController(null);
                    ////from.View.RemoveFromSuperview();
                    //from.RemoveFromParentViewController();

                    //               this.AddChildViewController(to);
                    //               this.View.AddSubview(to.View);
                    //to.DidMoveToParentViewController(this);

                    //this.transitionInProgress = false;
                }
                else if (to.RestorationIdentifier == "LiveData_2_Frame_View" ||
                         to.RestorationIdentifier == "LiveData_4_Frame_View" ||
                         to.RestorationIdentifier == "LiveData_6_Frame_View")
                {
                    

                    to.View.Frame = new CGRect(0,0,to.View.Frame.Size.Width, to.View.Frame.Size.Height);
					from.WillMoveToParentViewController(null);
					this.AddChildViewController(to);
					this.Transition(from,
									to,
									0.1,
									UIViewAnimationOptions.TransitionCrossDissolve,
									() => { },//這裡寫null會crash,swift就不會
									(finished) =>
									{
										from.RemoveFromParentViewController();
										to.DidMoveToParentViewController(this);
										this.transitionInProgress = false;
									}
								   );
                }
				//其它頁面
				else
				{
					//之後要加入autoResizingMask
					//to.View.Frame = new CGRect(0, 134, 520, 603);
					to.View.Frame = new CGRect(0, SCREEN_SIZE.Height / 5.2, to.View.Frame.Size.Width, to.View.Frame.Size.Height);
					//先call willMove
					from.WillMoveToParentViewController(null);
					//加入目的地子View
					this.AddChildViewController(to);
					//正式轉場 ,最後的閉包的參數finished什麼時機用不清楚,抑或是只是一種含義,須研究
					this.Transition(from,
									to,
									0.1,
									UIViewAnimationOptions.TransitionCrossDissolve,
									() => { },//這裡寫null會crash,swift就不會
									(finished) =>
									{
										from.RemoveFromParentViewController();
										to.DidMoveToParentViewController(this);
										this.transitionInProgress = false;
									}
								   );
				}

			}

		}



		public override void PrepareForSegue(UIStoryboardSegue segue, Foundation.NSObject sender)
		{
			base.PrepareForSegue(segue, sender);

			//用來保護的旗標,如過畫面尚未轉換完
			if (transitionInProgress)
			{
				return;
			}

			string destSegueID = segue.Identifier;
			if (destSegueID != null)
			{
				//第一個畫面例外,跑別的函式,跑完之後下面switch不跑
				if (isDefultPressed)
				{
					FirstTimeViewWillDo(segue);
					isDefultPressed = false;
					//if (AppAttribute.APP_RUNNING_MODE == AppAttribute.RunningMode.Normal)
					//{
					//	if (!isAutoConnect && !isStateMachineActived)
					//	{
					//		PerformSegue(MyCustomPages.ScanView, null);
					//	}
					//}
					MyCustomPages.previousButton = MyCustomPages.First;
					return;
				}

				//預設是null,進函式做初始化
				switch (destSegueID)
				{
					case MyCustomPages.First:
						if (homeViewController == null)
						{
							SecondTimeViewWillDo(segue);
						}
						break;
					case MyCustomPages.Log:
						if (loggViewController == null)
						{
							SecondTimeViewWillDo(segue);
						}
						break;
					case MyCustomPages.Third:
						if (zero2HundredViewController == null)
						{
							SecondTimeViewWillDo(segue);
						}
						break;
					case MyCustomPages.Forth:
						if (zero2HundredPage2ViewController == null)
						{
							SecondTimeViewWillDo(segue);
						}
						break;
					case MyCustomPages.Fifth:
						if (zero24HundredViewController == null)
						{
							SecondTimeViewWillDo(segue);
						}
						break;
					case MyCustomPages.Sixth:
						if (zero24HundredPage2ViewController == null)
						{
							SecondTimeViewWillDo(segue);
						}
						break;
					case MyCustomPages.Seventh:
						if (liveDataViewController == null)
						{
							SecondTimeViewWillDo(segue);
						}
						break;
					case MyCustomPages.Eighth:
						if (liveDataItemsViewController == null)
						{
							SecondTimeViewWillDo(segue);
						}
						break;
					case MyCustomPages.NewDTC:
						if (dtcViewController == null)
						{
							SecondTimeViewWillDo(segue);
						}
						break;
					
						//傳資料
						//在進行transition之前做傳值的動作(Passing data when a segue is performed)
						//dtcPage2ViewController.pcodeImage = DTCViewController.currentImage;
						//dtcPage2ViewController.pcodeDescription = DTCViewController.currentDescription;
					
					case MyCustomPages.Eleventh:
						if (shiftViewController == null)
						{
							SecondTimeViewWillDo(segue);
						}
						break;
					case MyCustomPages.Twelfth:
						if (valveViewController == null)
						{
							SecondTimeViewWillDo(segue);
						}
						break;
					//這裡以後改成segue
					case SeguesID.LiveDataGauges:
						if (liveDataGaugesViewController == null)
							SecondTimeViewWillDo(segue);
						break;
					
					case MyCustomPages.ScanView:
						if (scanViewController == null)
							SecondTimeViewWillDo(segue);
						break;
					case MyCustomPages.Settingg:
						if (settinggViewController == null)
							SecondTimeViewWillDo(segue);
						break;
                    case MyCustomPages.LiveData_2_Frame:
                        if (liveData_2_Frame_ViewController == null)
                            SecondTimeViewWillDo(segue);
                        else if (liveData_2_Frame_ViewController != null)
                        {
                            liveData_2_Frame_ViewController.SetLiveDataID(LiveDataID_For_IconPage);
                            liveData_2_Frame_ViewController.SetLiveDataIconImage(LvGaugesBG);
                            liveData_2_Frame_ViewController.UpdateGaugeBG();
                        }
                        break;
					case MyCustomPages.LiveData_4_Frame:
                        if (liveData_4_Frame_ViewController == null)
                            SecondTimeViewWillDo(segue);
                        else if (liveData_4_Frame_ViewController != null)
                        {
                            liveData_4_Frame_ViewController.SetLiveDataID(LiveDataID_For_IconPage);
                            liveData_4_Frame_ViewController.SetLiveDataIconImage(LvGaugesBG);
                            liveData_4_Frame_ViewController.UpdateGaugeBG();
                        }
						break;
					case MyCustomPages.LiveData_6_Frame:
                        if (liveData_6_Frame_ViewController == null)
                            SecondTimeViewWillDo(segue);
                        else if (liveData_6_Frame_ViewController != null)
                        {
                            liveData_6_Frame_ViewController.SetLiveDataID(LiveDataID_For_IconPage);
                            liveData_6_Frame_ViewController.SetLiveDataIconImage(LvGaugesBG);
                            liveData_6_Frame_ViewController.UpdateGaugeBG();
                        }
						break;
                    case MyCustomPages.LiveDataIconMenu:
                        if(liveDataIconViewController == null)
                            SecondTimeViewWillDo(segue);
                        break;
                    case MyCustomPages.Maintenance:
                        if(odoNavViewController == null)
                            SecondTimeViewWillDo(segue);
                        break;
                    case MyCustomPages.Upload:
                        if (uploadViewController == null)
                            SecondTimeViewWillDo(segue);
                        break;
                    case MyCustomPages.Map:
                        if (mapViewController == null)
                            SecondTimeViewWillDo(segue);
                        break;
                    case MyCustomPages.LvDataCloud:
                        if (lvDataCloudViewController == null)
                            SecondTimeViewWillDo(segue);
                        break;
                    case MyCustomPages.DataMonitor:
                        if (dataMonitorViewController == null)
                            SecondTimeViewWillDo(segue);
                        break;
                    default:
						break;
				}

				//轉換頁面
				if (this.ChildViewControllers != null && this.ChildViewControllers.Length > 0)
				{
					transitionInProgress = true;
					this.SwapFromViewController(this.ChildViewControllers[0], destinationDictionary[destSegueID], destSegueID);
				}

			}

		}

		private void SetComponentEvent()
		{
			buttonBoxes.Add("btnHome", btnHome);
			buttonBoxes.Add("btnLog", btnLog);
			buttonBoxes.Add("btnZero2Hundred", btnZero2Hundred);
			buttonBoxes.Add("btnZero24Hundred", btnZero24Hundred);
			buttonBoxes.Add("btnLiveData", btnLiveData);
			buttonBoxes.Add("btnDTC", btnDTC);
			buttonBoxes.Add("btnShift", btnShift);
			buttonBoxes.Add("btnValve", btnValve);
			buttonBoxes.Add("btnSetting", btnSetting);
            buttonBoxes.Add("btnLiveDataIcon", btnLiveDataIcon);
            buttonBoxes.Add("btnMaintenance", btnODO);
            buttonBoxes.Add("btnUpload",btnUpload);
            buttonBoxes.Add("btnMap", btnMap);
            buttonBoxes.Add("btnLvDataCloud", btnLvDataCloud);


            //暫時隱藏odo頁面
            btnODO.Hidden = true;

			//c#需定義event
			btnHome.TouchUpInside          += ButtonPressed;
			btnLog.TouchUpInside           += ButtonPressed;
			btnZero2Hundred.TouchUpInside  += ButtonPressed;
			btnZero24Hundred.TouchUpInside += ButtonPressed;
			btnLiveData.TouchUpInside      += ButtonPressed;
			btnDTC.TouchUpInside           += ButtonPressed;
			btnShift.TouchUpInside         += ButtonPressed;
			btnValve.TouchUpInside         += ButtonPressed;
			btnSetting.TouchUpInside       += BtnSetting_TouchUpInside;
            btnLiveDataIcon.TouchUpInside  += ButtonPressed;
            btnODO.TouchUpInside += ButtonPressed;
            btnUpload.TouchUpInside += ButtonPressed;
            btnMap.TouchUpInside += ButtonPressed;
            btnLvDataCloud.TouchUpInside += ButtonPressed;
		}


		protected ContainerViewController(IntPtr handle) : base(handle)
		{
		}

		//------------------------------------生命週期分隔線---------------------------------------

		private bool isAutoConnect = false;
		public bool IsAutoConnect
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

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			Console.WriteLine("ContainerViewController: " + "ViewDidLoad()");

            //         AVAudioSession.SharedInstance().SetCategory(AVAudioSessionCategory.Playback);
            //AVAsset asset = AVAsset.FromUrl(NSUrl.FromFilename("Eminem2.mp4"));
            //AVPlayerItem playerItem = new AVPlayerItem(asset);
            //player = new AVPlayer(playerItem);
            //playerLayer = AVPlayerLayer.FromPlayer(player);
            //playerLayer.Frame = View.Frame;
            //playerLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
            ////playerLayer.AC = AVPlayerActionAtItemEnd.None;
            //NSObject videoEndNotificationToken = NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.DidPlayToEndTimeNotification, VideoDidFinishPlaying, playerItem);
            //View.Layer.AddSublayer(playerLayer);

            //螢幕不鎖屏
            UIApplication.SharedApplication.IdleTimerDisabled = true;

            fuelConsumptionCompensationPercent = spf.GetFuelConsumptionCompensationPercent();
			//螢幕亮度
			//UIScreen.MainScreen.Brightness = 1.0f;

			//btnSetting.Hidden = true;
			Instance = this;

			//初始化按鈕的Dictionary及按鈕加入event
			SetComponentEvent();

			//判斷有無儲存裝置名稱
			if (spf.GetBleDeviceName() == null || spf.GetBleDeviceName() == "")
				isAutoConnect = false;
			else
			{
				isAutoConnect = true;
				targetDeviceName = spf.GetBleDeviceName();
				//Console.WriteLine(" name is ...... " + spf.GetBleDeviceName().ToString());
			}

			if (AppAttribute.APP_RUNNING_MODE == AppAttribute.RunningMode.Normal)
			{
				isStateMachineActived = true;
				stateMachine = new StateMachine(this, targetDeviceName, Page.Home, isAutoConnect);
                //斷線,掃不到裝置時要做的事情

                if (AppAttribute.BLE_SCAN_VIEW == AppAttribute.RunningMode.OldScanView)
                {
                    stateMachine.BLEConnetedAction = BLEConnectedMethod;
                    stateMachine.BLEDisconnetedAction = BLEDisconnectedMethod;
                    stateMachine.BLEScanFailedAction = BLEScanFailedMethod;
                }
                else
                {
                    
                }
			}


            //預設home畫面
            if(AppAttribute.START_MOVIE == AppAttribute.RunningMode.RunStartMovie)
                DoPlayVideoTransitions();
            else
                ButtonPressed(btnHome, null);
            
			setCurrentPageName();
			
			//scroll bar 長寬
			//scrollView.Frame.Width = scrollView.Frame.Width - btnDTC.Frame.Width;
			//nfloat d = 100;
			//scrollView.Frame = new CGRect(scrollView.Frame.Location.X,
			//							  scrollView.Frame.Location.Y,
			//							  414-66,
			//                              scrollView.Frame.Height);
			//stackview_buttons.Frame = new CGRect(stackview_buttons.Frame.Location.X,
			//stackview_buttons.Frame.Location.Y,
			//528-66,
			//stackview_buttons.Frame.Height);

            //var lang = NSBundle.MainBundle.PreferredLocalizations[0];

            //Console.WriteLine("language : " + lang);

            //foreach(var la in lang)
            //{
            //    Console.WriteLine("language : " + la);
            //}

            InitLocalization();
            //LocationManager locationManager = new LocationManager();

            //隱藏不需要的頁面
            //btnZero2Hundred.Hidden = true;
            //btnZero24Hundred.Hidden = true;
            //btnLog.Hidden = true;
            //btnShift.Hidden = true;
            //btnValve.Hidden = true;
            btnZero2Hundred.Alpha = 0.1f;
            btnZero24Hundred.Alpha = 0.1f;
            btnLog.Alpha = 0.1f;
            btnShift.Alpha = 0.1f;
            btnValve.Alpha = 0.1f;

            btnZero2Hundred.Enabled = false;
            btnZero24Hundred.Enabled = false;
            btnLog.Enabled = false;
            btnShift.Enabled = false;
            btnValve.Enabled = false;

            //ask for location authorization
            //if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            //locationManager.RequestWhenInUseAuthorization();
            //RequestLocationPermission();
        }

        //private bool isAllowLocationRead = false;
        //public bool IsAllowLocationRead { get => isAllowLocationRead; }
        ///// <summary>
        ///// 先確認Info.plist有無gps的key，
        ///// </summary>
        //private void RequestLocationPermission()
        //{
        //    locationManager = new CLLocationManager();

        //    EventHandler<CLAuthorizationChangedEventArgs> authCallback = null;

        //    authCallback = (sender, e) =>
        //    {
        //        Debug.WriteLine("CLAuthorizationStatus : " + e.Status.ToString());
        //        if (e.Status == CLAuthorizationStatus.NotDetermined)
        //            return;
        //        if (e.Status == CLAuthorizationStatus.Denied)
        //            isAllowLocationRead = false;
        //        if (e.Status == CLAuthorizationStatus.AuthorizedWhenInUse)
        //            isAllowLocationRead = true;

        //        locationManager.AuthorizationChanged -= authCallback;
        //        //do stuff here 
        //        symSharedPreferencesExtractor.SetGPSAllow(isAllowLocationRead);
        //    };

        //    locationManager.AuthorizationChanged += authCallback;

        //    var info = NSBundle.MainBundle.InfoDictionary;
        //    if (info.ContainsKey(new NSString("NSLocationWhenInUseUsageDescription")))
        //        locationManager.RequestWhenInUseAuthorization();
        //    //else if (info.ContainsKey(new NSString("NSLocationAlwaysUsageDescription")))
        //    //locationManager.RequestAlwaysAuthorization();
        //    else
        //        throw new UnauthorizedAccessException("On iOS 8.0 and higher you must set either NSLocationWhenInUseUsageDescription or NSLocationAlwaysUsageDescription in your Info.plist file to enable Authorization Requests for Location updates!");
        //}


		private void InitLocalization()
		{
            btnSetting.SetBackgroundImage(UIImage.FromBundle("btn_setting.png"),UIControlState.Normal);
		}


		
        //播放過場影片
        private void DoPlayVideoTransitions()
        {
            videoBG = new UIView(new CGRect(0,0,Main.SCREEN_SIZE.Width,Main.SCREEN_SIZE.Height));
            videoBG.BackgroundColor = UIColor.Black;
            AVAudioSession.SharedInstance().SetCategory(AVAudioSessionCategory.Playback);
            //AVPlayer player;
            //AVPlayerLayer playerLayer;
            AVAsset asset = AVAsset.FromUrl(NSUrl.FromFilename("ipe_app_open.mov"));
            AVPlayerItem playerItem = new AVPlayerItem(asset);
            player = new AVPlayer(playerItem);
            playerLayer = AVPlayerLayer.FromPlayer(player);
            playerLayer.Frame = View.Frame;
            playerLayer.VideoGravity = AVLayerVideoGravity.ResizeAspect;
            //playerLayer.AC = AVPlayerActionAtItemEnd.None;
            NSObject videoEndNotificationToken = NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.DidPlayToEndTimeNotification
                                                                                                ,VideoDidFinishPlaying,playerItem);
            View.AddSubview(videoBG);
            View.Layer.AddSublayer(playerLayer);
            //ButtonPressed(btnHome, null);

            player.Play();
        }

        //影片結束後呼叫的函式
        private void VideoDidFinishPlaying(NSNotification obj)
        {
            Console.WriteLine("Video Finished !");
            videoBG.RemoveFromSuperview();
            playerLayer.RemoveFromSuperLayer();
			ButtonPressed(btnHome, null);
		}

		private void DoImageTransitions()
		{
			//要加入的圖
			UIImageView ipeImageView = new UIImageView(new CGRect(0,
																  0,
																  this.View.Frame.Size.Width,
																  this.View.Frame.Size.Height));
			UIImageView blackView = new UIImageView(new CGRect(0,
															   0,
															   this.View.Frame.Size.Width,
															   this.View.Frame.Size.Height));

            //UIImageView ipeImageView = new UIImageView(new CGRect(50,50,100,60));
			//背景
            ipeImageView.Image = UIImage.FromFile("kk.png");
            //ipeImageView.Image = UIImage.FromBundle("aa.png");
			//ipeImageView.BackgroundColor = UIColor.Purple;
			ipeImageView.Alpha = 1.0f;

			blackView.BackgroundColor = UIColor.Black;
			blackView.Alpha = 1.0f;

            //this.View.AddSubview(blackView);
            this.View.AddSubview(ipeImageView);
			//ContainerViewController.Instance.View.AddSubview(blackView);
			ImageFadeIn(ipeImageView, blackView);
		}

		private void ImageFadeIn(UIImageView ipeImageView, UIImageView blackView)
		{
			//ContainerViewController.Instance.View.InsertSubviewAbove(ipeImageView,
			//														 blackView);
			this.View.InsertSubviewAbove(ipeImageView , blackView);
			UIView.Animate(2.0,//動畫整體的時間
						   2.0,//未進入動畫之前置時間
			               UIViewAnimationOptions.CurveEaseIn,
						   () => { ipeImageView.Alpha = 0.0f; },
						   () =>
						   {
							   //blackView.RemoveFromSuperview();
							   ipeImageView.RemoveFromSuperview();
							   //過場動畫結束後，進入Home頁面
                			   ButtonPressed(btnHome, null);
							   Console.WriteLine("ImageFadeIn---------------------======-----=-");
						   });
		}

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            Console.WriteLine("ContainerViewController: " + "ViewWillAppear()");
            //player.Play();
            //執行過場動畫
            //DoPlayVideoTransitions();
            //DoImageTransitions();
            //ButtonPressed(btnHome, null);

            //if (StateMachine.Instance.symSharedPreferencesExtractor.GetGPSAllow())
                StartUpdatingLocation();
            //else
            //{
                //btnMap.Enabled = false;
            //}

            //接口從底層上來
            if (HomeViewController.Instance != null && StateMachine.Instance != null)
            {
                StateMachine.Instance.BLEDiscoveredDeviceAction = HomeViewController.Instance.TriggerBLEDiscoveredDeviceAction;
            }


        }

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			Console.WriteLine("ContainerViewController: " + "ViewDidAppear()");

			isAlive = true;

            //DownloadLogDatas();
            //if (!isAutoConnect)
          
		}


		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			Console.WriteLine("ContainerViewController: " + "ViewWillDisappear()");
		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);
			Console.WriteLine("ContainerViewController: " + "ViewDidDisappear()");
			isAlive = false;

            //if (StateMachine.BLEComModel != null)
                //StateMachine.BLEComModel.Disconnect();

		}


		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}


		private void BLEDisconnectedMethod()
		{
			Console.WriteLine("Run BLEDisconnectedMethod()");
            StateMachine.BLEComModel.ClearInfoValue();
            //stateMachine.RemoveAllMessage();
            if(homeViewController != null)
                homeViewController.InitUI();
            if(loggViewController != null)
                loggViewController.InitUI();

			DispatchQueue.MainQueue.DispatchAsync(() =>
				{
					var connectAlert = UIAlertController.Create("Disconnected", "Press 'OK' to re-connect", UIAlertControllerStyle.Alert);
					StateMachine.UIModel.Instance.CloseProgressDialog();

					connectAlert.AddAction(UIAlertAction.Create("Forget", UIAlertActionStyle.Default, (action) =>
					{
						StateMachine.IsAutoConnect = false;
						stateMachine.RemoveAllMessage();
						spf.SetBleDeviceName("");
						if (ScanViewController.Instance != null && ScanViewController.Instance.DevNames != null
						   && ScanViewController.Instance.DevNames.Count > 0)
							ScanViewController.Instance.CleanBleNames();
						StateMachine.BLEComModel.mCBBLECentral.EraseBLEDevices();
						StateMachine.BLEComModel.ScanManually();
						this.PerformSegue(MyCustomPages.ScanView, null);
					}));
					connectAlert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (action) =>
					{
						//先檢查藍芽有沒有在掃，有的話則關閉
						if (StateMachine.BLEComModel.mCBBLECentral.IsScanning)
						{
							StateMachine.BLEComModel.mCBBLECentral.StopScanning();
						}

						//if (spf.GetBleDeviceName() != "")
						//{ 

						//}
						StateMachine.BLEComModel.mCBBLECentral.EraseBLEDevices();
						StateMachine.UIModel.Instance.ShowProgressDialog();
						stateMachine.RemoveAllMessage();
						

						StateMachine.Instance.SendMessage(StateMachineStatus.Device_Init);

					}));
					this.PresentViewController(connectAlert, false, null);
				});

		}

		private void BLEConnectedMethod()
		{
			Console.WriteLine("Run BLEConnectedMethod()");
			ToastIOS.Toast.MakeText("Run BLEConnectedMethod()").Show();
		}


		private void BLEScanFailedMethod()
		{
			//StateMachine.UIModel.CloseProgressDialog();

			//DispatchQueue.MainQueue.DispatchAsync(() =>
			//{

			//	var failedAlert = UIAlertController.Create("Device Not Found", "Vdi-BT not be detected,\r\ncheck power is On \r\n and Press 'OK' to rescan", UIAlertControllerStyle.Alert);
			//	failedAlert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (action) =>
			//	{
			//		StateMachine.IsAutoConnect = true;
			//		StateMachine.Instance.RemoveAllMessage();
			//		StateMachine.Instance.SendMessage(StateMachineStatus.Device_Init);
			//		StateMachine.UIModel.ShowProgressDialog();

			//	}));
			//	this.PresentViewController(failedAlert, false, null);
			//});


            if (Instance != null)
            {
                Debug.WriteLine("Run BLEScanFailedMethod()");
                if (StateMachine.BLEComModel != null)
                    StateMachine.BLEComModel.StopScan();
                Thread.Sleep(1000);

                StateMachine.UIModel.CloseProgressDialog();
                //UIModel.CloseReconnectAlertDialog();
                if (!StateMachine.BLEComModel.IsConnected)
                    StateMachine.UIModel.ShowRescanAlertDialog();

            
            }
		}

		public override void setCurrentPageName()
		{
			currentPageView = "HomeView";
		}

        //private List<int> LvIDsForIconPage { get; set; }


        public void RefreshLiveDataID(List<int> liveData,string vc)
        {
            //LvIDsForIconPage = liveData.ToList();
            //if(LiveDataID_For_IconPage != null)
            //if (LiveDataID_For_IconPage.Count > 0) LiveDataID_For_IconPage.Clear();
            switch (vc){
                case "LiveData_2_Frame":
					if (liveData_2_Frame_ViewController != null)
						liveData_2_Frame_ViewController.SetLiveDataID(liveData);
					//else
						//LiveDataID_For_IconPage = liveData;
                    break;
				case "LiveData_4_Frame":
					if (liveData_4_Frame_ViewController != null)
						liveData_4_Frame_ViewController.SetLiveDataID(liveData);
					//else
						//LiveDataID_For_IconPage = liveData;
					break;
				case "LiveData_6_Frame":
					if (liveData_6_Frame_ViewController != null)
						liveData_6_Frame_ViewController.SetLiveDataID(liveData);
					//else
						//LiveDataID_For_IconPage = liveData;
					break;
            }

            LiveDataID_For_IconPage = liveData;

        }

        public void RefreshLiveDataIconImage(List<string> images)
        {
            LvGaugesBG = images;
            //if(liveData_2_Frame_ViewController == null || liveData_4_Frame_ViewController == null
            //   || liveData_6_Frame_ViewController == null){
            //    LvGaugesBG = images;
            //}
            //else{
                
            //}
        }


        public static bool IsAlive
		{
			get
			{
				return isAlive;
			}
			set
			{
				isAlive = value;
			}
		}

		public UIScrollView ScrollView
		{
			get
			{
				return scrollView;
			}
		}

		public UIViewController CurrentController
		{
			get
			{
				return currentController;
			}
			set
			{
				currentController = value;
			}
		}


        private double myLatitude = 0.0;
        private double myLongitude = 0.0;
        public double MyLatitude { get => myLatitude; }
        public double MyLongitude { get => myLongitude; }
        private bool isSetRegionOnce = false;
        private bool isStartingLocation = false;
        private MKMapView map;
        public void SetMKMapView(MKMapView map) { this.map = map; }
        private bool isAllowLocationRead = false;

        /// <summary>
        /// Authorization change callback
        /// </summary>
        [Export("locationManager:didChangeAuthorizationStatus:")]
        public void AuthorizationChanged(CLLocationManager manager, CLAuthorizationStatus status)
        {
            Debug.WriteLine("CLAuthorizationStatus : " + status.ToString());
            if (status == CLAuthorizationStatus.NotDetermined)
                return;
            if (status == CLAuthorizationStatus.Denied)
                isAllowLocationRead = false;
            if (status == CLAuthorizationStatus.AuthorizedWhenInUse)
                isAllowLocationRead = true;

            //do stuff here 
            StateMachine.Instance.symSharedPreferencesExtractor.SetGPSAllow(isAllowLocationRead);
        }

        /// <summary>
        /// location讀取成功callback,每秒一次
        /// </summary>
        [Export("locationManager:didUpdateLocations:")]
        public void LocationsUpdated(CLLocationManager manager, CLLocation[] locations)
        {
            //foreach(var loc in locations) {
            //Console.WriteLine(loc);
            //}

            CLLocation location = locations[locations.Length - 1];

            //如果
            //NSDate eventDate = location.Timestamp;
            //eventDate .

            DateTime timeStamp = DateTime.Now;
            String curTime = timeStamp.ToString("yyyy-MM-dd HH:mm:ss:ffff");
            myLatitude = location.Coordinate.Latitude;
            myLongitude = location.Coordinate.Longitude;

            GPSData.SetData(location);
#if Debug時每秒顯示GPS位置
            Debug.WriteLine(curTime + "  latitude " + location.Coordinate.Latitude + " , "
                              + " longitude " + location.Coordinate.Longitude);
#endif
            //以人物為中心重繪範圍
            if (map != null && !isSetRegionOnce)
            {
                isSetRegionOnce = true;
                CLLocationCoordinate2D coordinate2D = new CLLocationCoordinate2D(myLatitude, myLongitude);
                MKCoordinateRegion region = MKCoordinateRegion.FromDistance(coordinate2D, 500, 500);
                map.SetRegion(region, true);
            }
            else
                return;
        }

        /// <summary>
        /// location讀取失敗callback
        /// </summary>
        [Export("locationManager:didFailWithError:")]
        public void Failed(CLLocationManager manager, NSError error)
        {
            //WriteToLog();
        }

        /// <summary>
        /// start location
        /// </summary>
        private void StartUpdatingLocation()
        {

            if (StateMachine.Instance.LocationManager == null)
            {
                locationManager = new CLLocationManager();

            }
            else
                locationManager = StateMachine.Instance.LocationManager;

            if(locationManager.Delegate == null)
                locationManager.Delegate = this;

            if (map != null)
                map.ShowsUserLocation = true;

            if (locationManager != null && isStartingLocation) return;

            //if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            //locationManager.RequestWhenInUseAuthorization();

            isStartingLocation = true;
            locationManager.StartUpdatingLocation();
            Debug.WriteLine(" -- location services start -- ");
        }

        /// <summary>
        /// stop location
        /// </summary>
        private void StopUpdatingLocation()
        {
            if (locationManager != null)
            {
                isStartingLocation = false;
                locationManager.StopUpdatingLocation();
                if (map != null)
                    map.ShowsUserLocation = false;
                locationManager = null;
                Debug.WriteLine("-- location services stop -- ");
            }
        }
	}
}


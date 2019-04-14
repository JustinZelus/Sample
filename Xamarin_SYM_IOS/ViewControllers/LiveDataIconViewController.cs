﻿using System;
using System.Collections.Generic;
using System.Linq;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using UIKit;

/**
 * This class serves as an LiveDataIconMenu oft handling and invoking tap events of UIButtons within custom UITableViewCell implementations.
 * In this class, a custom UITableViewCell implementation contains a UIButton that, when tapped, executes a function within the View Controller.
 * The process is done by setting up a "stack" of events between the UIButton and the View Controller, and connecting them with event handlers.
 * Generally the connections work like so: View Controller <--> UITableViewSource <--> UITableViewCell <--> UIButton
 * 
 * This pattern is not specific to UIButtons, but rather it can be implemented for any event-based element added within custom UITableViewCell implementations.
 * 
 * Created by JZ on 6/13/2017
 * 
 **/

namespace Xamarin_SYM_IOS.ViewControllers
{
    // ********* Protocal Table ***********/
    //public enum LiveDataTable
    //{
    //	Fuck = 9526,
    //	Oil_Temp = 9527,
    //	Speed = 1,
    //	RPM = 2,
    //	Water_Temp = 118,
    //	Boost = 9528,
    //	Fuel_Pressure = 127,
    //	Battery = 9529,
    //	Exhaust_Temp = 9530,
    //	AFR = 20,
    //	Oil_Pressure = 9531,
    //	Trottle = 11,
    //	Intake_Temp = 132,
    //}


    public partial class LiveDataIconViewController : UIViewController
    {

        public interface OnBtnSendClickListener
        {
            void RefreshLiveDataID(List<int> liveData, string vc);
            void RefreshLiveDataIconImage(List<string> images);

        }

        //奈米秒
        private const int NSEC_PER_SEC = 1000000000;
        private OnBtnSendClickListener onBtnSendClickListener;
        private UITableView IconTableView;
        private UIAlertController warninigAlert;
        private readonly int MAXIMUM = 6;
        private readonly int MINIMUM = 0;
        private int count = 0;
        public static LiveDataIconViewController Instance;
        private UIButton btnSend;
        private string pageForGo;


        /** gauge圖片 */
        private Dictionary<int, string> gaugesBG = new Dictionary<int, string>()
        {
            {(int)LiveDataItemsSN.EngineOilTemperature,"sp_oil_temp.png"}, //oil_temp
            {(int)LiveDataItemsSN.VSS,"sp_speed.png"}, //speed
            {(int)LiveDataItemsSN.RPM,"sp_rpm.png"}, //rpm
            {(int)LiveDataItemsSN.Water_Temp,"sp_water_temp.png"}, //water_temp
            {(int)LiveDataItemsSN.BoostPressureSensor,"sp_boost.png"}, //boost
            {(int)LiveDataItemsSN.Fuel_Pressure,"sp_fuel_pressure.png"}, //fuel pressure
            {(int)LiveDataItemsSN.Battery,"sp_battery.png"}, //battery
            {(int)LiveDataItemsSN.ExhaustGasTemperature,"sp_exhaust_temp.png"}, //exhaust temp
            {(int)LiveDataItemsSN.AFR,"sp_afr.png"}, //afr
            {9531,"sp_oil_pressure.png"}, //oil pressure
            {(int)LiveDataItemsSN.Trottle,"sp_throttle.png"}, //throttle
            {(int)LiveDataItemsSN.Intake_Temp,"sp_intake_temp.png"} //intake
        };

        /** 儲存user選過的圖片*/
        private List<string> selectedImages = new List<string>();
        private List<string> SelectedImages { get => selectedImages; set => selectedImages = value; }

        private List<int> chiocedLV = new List<int>();
        private List<int> sendLV = new List<int>();

        public List<int> ChiocedLV { get => chiocedLV; set => chiocedLV = value; }
        public List<int> SendLV { get => sendLV; set => sendLV = value; }
        /** 存值用(deprecated) */
        private List<int> LvIds = new List<int>();

        /** cell層設定Tag用*/
        private Dictionary<int, int> lvIDsTag = new Dictionary<int, int>(){
            {1,(int)LiveDataItemsSN.EngineOilTemperature},
            {2,(int)LiveDataItemsSN.VSS},
            {3,(int)LiveDataItemsSN.RPM},
            {4,(int)LiveDataItemsSN.Water_Temp},
            {5,(int)LiveDataItemsSN.BoostPressureSensor},
            {6,(int)LiveDataItemsSN.Fuel_Pressure},
            {7,(int)LiveDataItemsSN.Battery},
            {8,(int)LiveDataItemsSN.ExhaustGasTemperature},
            {9,(int)LiveDataItemsSN.AFR},
            {10,9531},
            {11,(int)LiveDataItemsSN.Trottle},
            {12,(int)LiveDataItemsSN.Intake_Temp}

        };
        public Dictionary<int, int> LvIDsTag { get => lvIDsTag; }



        /** 換圖用 */
        private Dictionary<int, bool> selectedLiveDatas = new Dictionary<int, bool>(){
            {(int)LiveDataItemsSN.EngineOilTemperature,false},
            {(int)LiveDataItemsSN.VSS,false},
            {(int)LiveDataItemsSN.RPM,false},
            {(int)LiveDataItemsSN.Water_Temp,false},
            {(int)LiveDataItemsSN.BoostPressureSensor,false},
            {(int)LiveDataItemsSN.Fuel_Pressure,false},
            {(int)LiveDataItemsSN.Battery,false},
            {(int)LiveDataItemsSN.ExhaustGasTemperature,false},
            {(int)LiveDataItemsSN.AFR,false},
            {9531,false},
            {(int)LiveDataItemsSN.Trottle,false},
            {(int)LiveDataItemsSN.Intake_Temp,false}
        };


        /** Icon 圖片 - 英文 */
        private Dictionary<int, UIImage> btnBG = new Dictionary<int, UIImage>(){
            {(int)LiveDataItemsSN.EngineOilTemperature,UIImage.FromBundle("ipe_oil_temp_off.png")},
            {(int)LiveDataItemsSN.VSS,UIImage.FromBundle("ipe_speed_off.png")},
            {(int)LiveDataItemsSN.RPM,UIImage.FromBundle("ipe_rpm_off.png")},
            {(int)LiveDataItemsSN.Water_Temp,UIImage.FromBundle("ipe_water_temp_off.png")},
            {(int)LiveDataItemsSN.BoostPressureSensor,UIImage.FromBundle("ipe_boost_off.png")},
            {(int)LiveDataItemsSN.Fuel_Pressure,UIImage.FromBundle("ipe_fuel_pressure_off.png")},
            {(int)LiveDataItemsSN.Battery,UIImage.FromBundle("ipe_battery_off.png")},
            {(int)LiveDataItemsSN.ExhaustGasTemperature,UIImage.FromBundle("ipe_exhaust_temp_off.png")},
            {(int)LiveDataItemsSN.AFR,UIImage.FromBundle("ipe_afr_off.png")},
            {9531,UIImage.FromBundle("ipe_oil_pressure_off")},
            {(int)LiveDataItemsSN.Trottle,UIImage.FromBundle("ipe_throttle_off.png")},
            {(int)LiveDataItemsSN.Intake_Temp,UIImage.FromBundle("ipe_intake_temp_off.png")}
        };

		/** Icon 圖片 - 英文 */
		private Dictionary<int, UIImage> btnBG_Pressed = new Dictionary<int, UIImage>(){
            {(int)LiveDataItemsSN.EngineOilTemperature,UIImage.FromBundle("ipe_oil_temp_on.png")},
            {(int)LiveDataItemsSN.VSS,UIImage.FromBundle("ipe_speed_on.png")},
            {(int)LiveDataItemsSN.RPM,UIImage.FromBundle("ipe_rpm_on.png")},
            {(int)LiveDataItemsSN.Water_Temp,UIImage.FromBundle("ipe_water_temp_on.png")},
            {(int)LiveDataItemsSN.BoostPressureSensor,UIImage.FromBundle("ipe_boost_on.png")},
            {(int)LiveDataItemsSN.Fuel_Pressure,UIImage.FromBundle("ipe_fuel_pressure_on.png")},
            {(int)LiveDataItemsSN.Battery,UIImage.FromBundle("ipe_battery_on.png")},
            {(int)LiveDataItemsSN.ExhaustGasTemperature,UIImage.FromBundle("ipe_exhaust_temp_on.png")},
            {(int)LiveDataItemsSN.AFR,UIImage.FromBundle("ipe_afr_on.png")},
            {9531,UIImage.FromBundle("ipe_oil_pressure_on.png")},
            {(int)LiveDataItemsSN.Trottle,UIImage.FromBundle("ipe_throttle_on.png")},
            {(int)LiveDataItemsSN.Intake_Temp,UIImage.FromBundle("ipe_intake_temp_on.png")}
        };

		
        /** 有支援的id*/
        private Dictionary<int, bool> lvIdSuppout = new Dictionary<int, bool>{
		    {(int)LiveDataItemsSN.EngineOilTemperature,false},
			{(int)LiveDataItemsSN.VSS,false},
			{(int)LiveDataItemsSN.RPM,false},
			{(int)LiveDataItemsSN.Water_Temp,false},
			{(int)LiveDataItemsSN.BoostPressureSensor,false},
			{(int)LiveDataItemsSN.Fuel_Pressure,false},
			{(int)LiveDataItemsSN.Battery,false},
			{(int)LiveDataItemsSN.ExhaustGasTemperature,false},
			{(int)LiveDataItemsSN.AFR,false},
            {9531,false},
            {(int)LiveDataItemsSN.Trottle,false},
			{(int)LiveDataItemsSN.Intake_Temp,false}
        };

        private Dictionary<int, List<UInt32>> lvIdSuppoutList = new Dictionary<int, List<UInt32>>{
            {(int)LiveDataItemsSN.EngineOilTemperature,StateMachine.DataModel.LvIcon_OilTemperature_CommunicationIDs},
            {(int)LiveDataItemsSN.VSS,StateMachine.DataModel.LvIcon_Speed_CommunicationIDs},
            {(int)LiveDataItemsSN.RPM,StateMachine.DataModel.LvIcon_Rpm_CommunicationIDs},
            {(int)LiveDataItemsSN.Water_Temp,StateMachine.DataModel.LvIcon_WaterTemperature_CommunicationIDs},
            {(int)LiveDataItemsSN.BoostPressureSensor,StateMachine.DataModel.LvIcon_Boost_CommunicationIDs},
            {(int)LiveDataItemsSN.Fuel_Pressure,StateMachine.DataModel.LvIcon_FuelPressure_CommunicationIDs},
            {(int)LiveDataItemsSN.Battery,StateMachine.DataModel.LvIcon_Battery_CommunicationIDs},
            {(int)LiveDataItemsSN.ExhaustGasTemperature,StateMachine.DataModel.LvIcon_ExhaustTemperature_CommunicationIDs},
            {(int)LiveDataItemsSN.AFR,StateMachine.DataModel.LvIcon_AFR_CommunicationIDs},
            {9531,StateMachine.DataModel.LvIcon_OilPressure_CommunicationIDs},
            {(int)LiveDataItemsSN.Trottle,StateMachine.DataModel.LvIcon_ThrottlePosition_CommunicationIDs},
            {(int)LiveDataItemsSN.Intake_Temp,StateMachine.DataModel.LvIcon_IntakeTemperature_CommunicationIDs}
        };

        private Dictionary<int, int> dynamicLvId = new Dictionary<int, int>();

        private Dictionary<int, bool> SettingDictLvIdSuppout()
        {
            var baseLvIds = StateMachine.DataModel.AllEcuLvIds;
            foreach(var key in lvIdSuppoutList.Keys)
            {
                var lvIds = lvIdSuppoutList[key];

                foreach(var id in lvIds)
                {
                    if(baseLvIds.Contains((int)id))
                    {
                        lvIdSuppout[key] = true;
                        dynamicLvId.Add(key,(int)id);
                        break;
                    }
                    Console.WriteLine("The ID need to send : " + id);
                }
            }

            return lvIdSuppout;
        }


        public Dictionary<int, bool> LvIdSuppout
        {
            get => lvIdSuppout;
            set => lvIdSuppout = value;
        }


		
        private bool isFontSizeStandard = true;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            //Console.WriteLine("LiveDataIconViewController : ViewDidLoad()");

            Instance = this;
			//初始化警告視窗
			InitAlert();




            //this.Title = "Icon Menu";
            //this.View.BackgroundColor = UIColor.ViewFlipsideBackgroundColor;
            if(UIScreen.MainScreen.NativeScale > 2.7)
                isFontSizeStandard = false;
			

            ////比對支援的id
            //if(StateMachine.IsActivted)
            //{
            //    var allId = StateMachine.DataModel.AllEcuLvIds;

            //    foreach(int i in lvIDsTag.Values)
            //    {
            //        if (allId.Contains(i))
            //            lvIdSuppout[i] = true;
            //    }
            //}


            // Creating the table view and adding it to the ViewController's view.
            IconTableView = new UITableView(new CGRect(), UITableViewStyle.Plain){
                SeparatorStyle = UITableViewCellSeparatorStyle.None,
                AllowsSelection = false,
                BackgroundColor = UIColor.Black
                    
            };
            this.View.AddSubview(IconTableView);


            // Creating a list of liveDatas.
            List<LiveData> LiveDatas = new List<LiveData>()
            {   
                
                new LiveData("Oil_Temp" ,(int)LiveDataItemsSN.EngineOilTemperature),
                new LiveData("Speed"    ,(int)LiveDataItemsSN.VSS),
                new LiveData("RPM"      ,(int)LiveDataItemsSN.RPM),
                new LiveData("Water_Temp"    ,(int)LiveDataItemsSN.Water_Temp),
                new LiveData("Boost"         ,(int)LiveDataItemsSN.BoostPressureSensor),
                new LiveData("Fuel_Pressure" ,(int)LiveDataItemsSN.Fuel_Pressure),
                new LiveData("Battery"       ,(int)LiveDataItemsSN.Battery),
                new LiveData("Exhaust_Temp"  ,(int)LiveDataItemsSN.ExhaustGasTemperature),
                new LiveData("AFR"           ,(int)LiveDataItemsSN.AFR),
                new LiveData("Oil_Pressure"  ,9531),
                new LiveData("Trottle"       ,(int)LiveDataItemsSN.Trottle),
                new LiveData("Intake_Temp"   ,(int)LiveDataItemsSN.Intake_Temp)

            };

	        
            // Applying a new LiveDataIconTableViewSource to the table view and setting the livaData data.
            LiveDataIconTableViewSource TableViewSource = new LiveDataIconTableViewSource();
            IconTableView.Source = TableViewSource;
            TableViewSource.SetButtonsBG(btnBG);
            TableViewSource.SetData(LiveDatas);
            TableViewSource.SetIconTag(lvIDsTag);
            TableViewSource.SetLvIdSuppout(SettingDictLvIdSuppout());

            //Creating a HeadView and add it to the top of tableview
            UIView view = new UIView(new CGRect(0, 0, Main.SCREEN_SIZE.Width, Main.SCREEN_SIZE.Width/8.28));
            view.BackgroundColor = UIColor.Clear;

            btnSend = new UIButton(new CGRect(Main.SCREEN_SIZE.Width/20.7,3,Main.SCREEN_SIZE.Width - Main.SCREEN_SIZE.Width /10.35,Main.SCREEN_SIZE.Width/8.28));
            btnSend.SetTitle("SEND", UIControlState.Normal);
            btnSend.BackgroundColor = UIColor.Clear;
            btnSend.SetBackgroundImage(UIImage.FromFile("btn_send_bg.png"),UIControlState.Normal);
            btnSend.SetBackgroundImage(UIImage.FromFile("btn_send_pressed_bg.png"), UIControlState.Selected);
			btnSend.TouchUpInside += BtnSend_TouchUpInside; 

			//new MonoTouch.ObjCRuntime.Selector ("demo:")
            //btnSend.PerformSelector(new ObjCRuntime.Selector("BtnSend_TouchUpInside:"), btnSend,2.0);

            view.AddSubview(btnSend);
            IconTableView.TableHeaderView = view;
            IconTableView.ReloadData();

			
			// Applying a new LiveDataIconTableViewDelegate (目前沒用到delegate)


			// Creating the top-level event handler for the LiveDataIconTableViewSource's IconButtonTapped event.
			TableViewSource.IconButtonTapped -= OnIconButtonTapped;
            TableViewSource.IconButtonTapped += OnIconButtonTapped;


   
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            //Console.WriteLine("LiveDataIconViewController : ViewDidAppear()");

			if (StateMachine.IsActivted)
			{
			  StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV_STOP);
			}

			//偵測到字體非標準時
			if (!isFontSizeStandard)
			{
				DispatchQueue.MainQueue.DispatchAsync(() => {
					var alert = UIAlertController.Create("Font size should be standard", "Please press the \"Setting\" and go to the setting page to change font size to standard", UIAlertControllerStyle.Alert);
					alert.AddAction(UIAlertAction.Create("Setting", UIAlertActionStyle.Default, (action) =>
					{
						NSUrl url = new NSUrl("App-prefs:root=DISPLAY");
						if (UIApplication.SharedApplication.CanOpenUrl(url))
							UIApplication.SharedApplication.OpenUrl(url);

					}));

                    isFontSizeStandard = true;
					this.PresentViewController(alert, false, null);
				});
			}

		}



        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            //Console.WriteLine("LiveDataIconViewController : ViewDidDisappear()");
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();

        }


        private void InitAlert(){
			warninigAlert = UIAlertController.Create("Wrong Selected !", "please choice 2,4,6 ", UIAlertControllerStyle.Alert);
			warninigAlert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (action) =>
			{

				DispatchQueue.MainQueue.DispatchAsync(() =>
				{
					warninigAlert.DismissViewController(true, null);
				});

			}));
        }


		void HandleAction()
		{
			//恢復按鈕的背景
			if (btnSend.Selected)
				btnSend.Selected = false;
            
            ContainerViewController.Instance.PerformSegue(pageForGo, null);
			
		}


        //[Export("BtnSend_TouchUpInside:")]
        //void BtnSend_TouchUpInside(object sender)
        void BtnSend_TouchUpInside(object sender, EventArgs e)
        {   
            //如果按了就返回
            if (((UIButton)sender).Selected)
                return;
            
            //檢查是否為偶數且小於8
            if((count % 2) == 0 && count <= MAXIMUM && count > 0){
				//0.3秒後跳轉頁面
				var popTime = new DispatchTime(DispatchTime.Now, (long)0.3 * NSEC_PER_SEC);
                var vcName = "";

				DispatchQueue.MainQueue.DispatchAfter(popTime, HandleAction);


				if (!((UIButton)sender).Selected)
				{
					((UIButton)sender).Selected = true;

				}

                switch(count){
                    case 2:
                        
                        //設定page讓uimodel跑
                        if(StateMachine.IsActivted) StateMachine.UIModel.CurrentPage = Page.LiveData_2_Frame;

                        vcName = "LiveData_2_Frame";
                        //排序由左至右，由上至下
                        //SortingSelectedPosition();
                        break;
					case 4:
                        
                        if (StateMachine.IsActivted) StateMachine.UIModel.CurrentPage = Page.LiveData_4_Frame;

                        vcName = "LiveData_4_Frame";

						break;
					case 6:
                        
                        if (StateMachine.IsActivted) StateMachine.UIModel.CurrentPage = Page.LiveData_6_Frame;

                        vcName = "LiveData_6_Frame";

						break;
                    default:
                        break;
                }

				
				//更新所選擇的liveData
                onBtnSendClickListener.RefreshLiveDataID(GetSelectedButtonsLvIDs(), vcName);

				//更新所選擇的liveDataIconImages
				onBtnSendClickListener.RefreshLiveDataIconImage(SelectedImages);

                pageForGo = MyCustomPages.buttonRespondSegueID[vcName];

                //送指令給stateMachine
                if (StateMachine.IsActivted)
                {
                    StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV_STOP);
                    StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV);
                }

                //恢復按鈕的背景
                //((UIButton)sender).Selected = false;
            }
            else{
                //Console.WriteLine("Please choice 2,4,6  -  " + count); 

				DispatchQueue.MainQueue.DispatchAsync(() =>
			    {
                    ContainerViewController.Instance.PresentViewController(warninigAlert, true, null);
			    });

            }

        }

        private List<int> SortingSelectedPosition(List<int> list){
            //List<int> sort = new List<int>();
            Dictionary<int, int> dicForSorting = new Dictionary<int, int>();

            //把拿到的id組回成dictionary
            foreach(var id in list){
                dicForSorting.Add(lvIDsTag.FirstOrDefault(x => x.Value == id).Key,id);

            }
            //組好的dic在排序
            var sortedList = dicForSorting.OrderBy(pair => pair.Key).ToList();

            return list;
        }

        //拿到user所選擇的live data id
        private List<int> GetSelectedButtonsLvIDs(){
            
            if (chiocedLV.Count > 0) chiocedLV.Clear();

            if (sendLV.Count > 0) sendLV.Clear();
                
            //用linq語法拿到為true的key
            var keysWithMatchingValues = selectedLiveDatas.Where(p => p.Value == true).Select(p => p.Key);

            //蒐集
            foreach(var key in keysWithMatchingValues){
                    
                chiocedLV.Add(key);
                try
                {
                    sendLV.Add(dynamicLvId[key]);
                }
                catch(Exception e)
                {
                    
                }
            }

            //更新user所選擇的圖片名稱
            if (SelectedImages.Count > 0) SelectedImages.Clear();

            foreach(var lvID in chiocedLV){
                //Console.WriteLine("user sected true images is : " + lvID);
                SelectedImages.Add(gaugesBG[lvID]);

                //Console.WriteLine("selceted key is " + lvIDsTag.FirstOrDefault(x => x.Value == lvID).Key);
            }

            return sendLV;
            //return chiocedLV;
            //return SortingSelectedPosition(chiocedLV);
        }

        private void SaveLvIdsByUserChoice(List<int> lvIds){
            this.LvIds = lvIds.ToList();
        }

        //void OnIconButtonTapped(object sender, LiveData e)
        void OnIconButtonTapped(object sender, EventArgs e)
        {
            //Console.WriteLine("tag : " + ((UIButton)sender).Tag + ",  name : " + e.Name + "  , id: " + e.ID + " , selected : " + !((UIButton)sender).Selected);

            ChangeBG(sender);

        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            if (IconTableView != null)
                IconTableView.Frame = View.Bounds;
        }

        private void ChangeBG(object sender){
			int tag = (int)((UIButton)sender).Tag;
            //tag 轉 pos 
            //int pos =  

			if (!((UIButton)sender).Selected)
			{
				((UIButton)sender).Selected = true;
				DispatchQueue.MainQueue.DispatchAsync(() =>
				{
					((UIButton)sender).SetBackgroundImage(btnBG_Pressed[tag], UIControlState.Normal);
				});

				if (count >= 0){
                    count++;
                    //if (count > MAXIMUM) count = MAXIMUM;
				}

                selectedLiveDatas[tag] = true;

			}
			else
			{
				((UIButton)sender).Selected = false;
				DispatchQueue.MainQueue.DispatchAsync(() =>
				{
					((UIButton)sender).SetBackgroundImage(btnBG[tag], UIControlState.Normal);
				});

				if (count >=0){
					count--;
                    if (count < MINIMUM) count = MINIMUM;
				}

                selectedLiveDatas[tag] = false;
			}
        }

        public void SetOnBtnSendClickListener(OnBtnSendClickListener listener){
            this.onBtnSendClickListener = listener;
        }

		protected LiveDataIconViewController(IntPtr handle) : base(handle)
		{
		}
    }

    //-----------------------TABLEVIEW DELEGATE------------------------
    public class LiveDataIconTableViewDelegate : UITableViewDelegate
    {
        public LiveDataIconTableViewDelegate() : base()
        {
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            return base.GetViewForHeader(tableView, section);
        }
    }

    //-----------------------TABLEVIEW SOURCE------------------------
    public class LiveDataIconTableViewSource : UITableViewSource
    {
        /// <summary>
        /// The reuse identifier used for recycling cells.
        /// </summary>
        public const string REUSE_IDENTIFIER = @"LiveDataIconTable";

        /// <summary>
        /// Occurs when a Icon button is tapped within one of the cells.
        /// </summary>
        //public event EventHandler<LiveData> IconButtonTapped;
        public event EventHandler<EventArgs> IconButtonTapped;

        private Dictionary<int, UIImage> Images;
        private List<LiveData> LiveDatas;
        private Dictionary<int, int> lvIDsTag;

        private Dictionary<int, bool> lvIdSuppout;
        private Dictionary<int, UIImage> lvIdSuppoutImage;
        private Dictionary<int, UIImage> lvIdUnSuppoutImage;

        private int modifyValue = 0;

        public LiveDataIconTableViewSource() : base()
        {
        }

        public void SetButtonsBG(Dictionary<int, UIImage> Images)
        {
            this.Images = Images;
        }

        public void SetIconTag(Dictionary<int, int>  lvIDsTag){
            this.lvIDsTag = lvIDsTag;
        }

        public void SetData(List<LiveData> LiveDatas)
        {
            this.LiveDatas = LiveDatas;
            //LiveDataIconTableViewCell.LiveData = LiveDatas;
        }

        public void SetLvIdSuppout(Dictionary<int, bool> lvIdSuppout)
        {
            this.lvIdSuppout = lvIdSuppout;
        }

        //public void SetLvIdSuppoutImage(Dictionary<int, UIImage> lvIdSuppoutImage)
        //{
        //    this.lvIdSuppoutImage = lvIdSuppoutImage;
        //}

        //public void SetLvIdUnSuppoutImage(Dictionary<int, UIImage> lvIdUnSuppoutImage)
        //{
        //    this.lvIdUnSuppoutImage = lvIdUnSuppoutImage;   
        //}

        private bool IsLvIdSupport(int lvID)
        {
            return lvIdSuppout[lvID];
        }

        //private UIImage GetBgImages(bool isSupport,int id)
        //{
        //    if(isSupport){
        //        return lvIdSuppoutImage[id];
        //    }
        //    else
        //        return lvIdUnSuppoutImage[id];
        //}

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (LiveDatas == null || LiveDatas.Count < 1)
                return null;
            

            LiveDataIconTableViewCell Cell = tableView.DequeueReusableCell(REUSE_IDENTIFIER) as LiveDataIconTableViewCell;

			//這一段只會call四次，因為 RowsInSection return 4
			if (Cell == null)
            {
                
                Cell = new LiveDataIconTableViewCell(REUSE_IDENTIFIER);

            }


            return SettingEachCellUIState(Cell);
            //return Cell;
        }

        private LiveDataIconTableViewCell SettingEachCellUIState(LiveDataIconTableViewCell Cell)
        {
            //第一行設tag是因為只需要顯示11個，第12個要隱藏
            //第二行是設置是否支援
            //第三行設置要顯示的alpha值
            //第四行設置圖片
            Cell.IconButtonLeft.Tag = lvIDsTag[++modifyValue];
            Cell.IconButtonLeft.Enabled = IsLvIdSupport(lvIDsTag[modifyValue]);
            Cell.IconButtonLeft.Alpha = Cell.IconButtonLeft.Enabled ? 1.0f : 0.5f;
            Cell.IconButtonLeft.SetBackgroundImage(Images[lvIDsTag[modifyValue]], UIControlState.Normal);


            Cell.IconButtonCenter.Tag = lvIDsTag[++modifyValue];
            Cell.IconButtonCenter.Enabled = IsLvIdSupport(lvIDsTag[modifyValue]);
            Cell.IconButtonCenter.Alpha = Cell.IconButtonCenter.Enabled ? 1.0f : 0.5f;
            Cell.IconButtonCenter.SetBackgroundImage(Images[lvIDsTag[modifyValue]], UIControlState.Normal);


            Cell.IconButtonRight.Tag = lvIDsTag[++modifyValue];
            Cell.IconButtonRight.Enabled = IsLvIdSupport(lvIDsTag[modifyValue]);
            Cell.IconButtonRight.Alpha = Cell.IconButtonRight.Enabled ? 1.0f : 0.5f;
            Cell.IconButtonRight.SetBackgroundImage(Images[lvIDsTag[modifyValue]], UIControlState.Normal);
            //這裡因為一個row固定要3個，暫時先這樣寫
            //if (Cell.IconButtonRight.Tag == 9531) Cell.IconButtonRight.Hidden = true;
            //else
                //Cell.IconButtonRight.SetBackgroundImage(Images[lvIDsTag[modifyValue]], UIControlState.Normal);



            // Here is where the OnClick event handler is set for the cell.
            // Notice that the event handler only needs to be applied on the creation of a brand new cell.
            // Recycled cells can use their existing event handlers.
            Cell.IconButtonTapped -= OnCellIconButtonTapped;
            Cell.IconButtonTapped += OnCellIconButtonTapped;

            return Cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            //return LiveDatas == null ? 0 : LiveDatas.Count;
            return 4;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return LiveDataIconTableViewCell.HEIGHT;
        }

        //測試無效
        //public override UIView GetViewForHeader(UITableView tableView, nint section)
        //{
        //    UIView view = new UIView(new CGRect(0,0,Main.SCREEN_SIZE.Width,30));
        //    view.BackgroundColor = UIColor.Blue;
        //    return view;
        //}

        //void OnCellIconButtonTapped(object sender, LiveData e)
        void OnCellIconButtonTapped(object sender, EventArgs e)
        {
            // This is the event handler for the LiveDataIconTableViewCell's IconButtonTapped event.
            // In this handler, we will invoke the LiveDataIconTableViewSource's IconButtonTapped event causing it to "bubble up" into the view controller.
            if (IconButtonTapped != null)
                IconButtonTapped(sender, e);
        }
    }

    public class LiveDataIconTableViewCell : UITableViewCell
    {
        /// <summary>
        /// The magin of each button.
        /// </summary>
        public int MARGIN;

        /// <summary>
        /// The constant height of each cell.
        /// </summary>
        public static int HEIGHT;


        //public static List<LiveData> LiveData { get; set; }
        /// <summary>
        /// The current LiveData data object of this cell.
        /// </summary>
        public LiveData[] CurrentLiveData { get; private set; }

        /// <summary>
        /// The Icon button
        /// </summary>
        public UIButton IconButtonLeft { get; }
        public UIButton IconButtonCenter { get; }
        public UIButton IconButtonRight { get; }


		///<summary>
		/// Occurs when the Icon button is tapped.
		/// </summary>
         public event EventHandler<EventArgs> IconButtonTapped;
		//public event EventHandler<LiveData> IconButtonTapped;

		//private static readonly UIColor cellBGColor = UIColor.Clear;

        public LiveDataIconTableViewCell(string ReuseIdentifier) : base(UITableViewCellStyle.Default, ReuseIdentifier)
        {
            
            /** 調整cell,對應螢幕 */
            int cellContentWidth = 0;
            switch (Main.PHONE)
            {
                case Devices.Iphone6s:
                    cellContentWidth = 240;
                    break;
                case Devices.Iphone6plus:
                    cellContentWidth = 320;
                    break;
            }

            //Caculating margin of each button on screen.
            this.MARGIN = ((int)Main.SCREEN_SIZE.Width - cellContentWidth) / 4;
            HEIGHT = (int)cellContentWidth / 3 + this.MARGIN;

            //this.MARGIN = ((int)Main.SCREEN_SIZE.Width - (int)ContentView.Bounds.Width) / 4;


            // Creating and adding the Icon button to the cell's content view.
            this.IconButtonLeft = new UIButton(new CGRect(MARGIN, MARGIN, cellContentWidth / 3, cellContentWidth / 3));
            this.ContentView.BackgroundColor = UIColor.Black;
            this.ContentView.AddSubview(this.IconButtonLeft);

            this.IconButtonCenter = new UIButton(new CGRect(MARGIN + cellContentWidth / 3 + MARGIN, MARGIN, cellContentWidth / 3, cellContentWidth / 3));
            this.ContentView.BackgroundColor = UIColor.Black;
            this.ContentView.AddSubview(this.IconButtonCenter);

            this.IconButtonRight = new UIButton(new CGRect(MARGIN + (cellContentWidth / 3) * 2 + MARGIN * 2, MARGIN, cellContentWidth / 3, cellContentWidth / 3));
            this.ContentView.BackgroundColor = UIColor.Black;
            this.ContentView.AddSubview(this.IconButtonRight);


            // Here is where the lowest-level event handler is applied to the icon button.
            IconButtonLeft.TouchUpInside -= OnIconButtonTapped;
            IconButtonLeft.TouchUpInside += OnIconButtonTapped;

            IconButtonCenter.TouchUpInside -= OnIconButtonTapped;
            IconButtonCenter.TouchUpInside += OnIconButtonTapped;

            IconButtonRight.TouchUpInside -= OnIconButtonTapped;
            IconButtonRight.TouchUpInside += OnIconButtonTapped;
        }


        void OnIconButtonTapped(object sender, EventArgs e)
        {
            if (IconButtonTapped != null)
            {

                int pos = (int)((UIButton)sender).Tag;

                IconButtonTapped(sender, e);
                //IconButtonTapped(sender, LiveData[pos]);

                //switch ((int)((UIButton)sender).Tag)
                //{
                //    case 1:
                //        IconButtonTapped(sender, LiveData[0]);
                //        break;
                //    case 2:
                //        IconButtonTapped(sender, LiveData[1]);
                //        break;
                //    default:
                //        break;
                //}


                //IconButtonTapped(sender, new LiveData("FFFF",LiveDataID.AF));
                //對應9個LiveData的按鈕
                //           switch((int)((UIButton)sender).Tag)
                //           {   
                //               case (int)LiveDataID.Oil_Temperature:
                //                   IconButtonTapped(sender, CurrentLiveData[(int)LiveDataID.Oil_Temperature]);
                //                   break;
                //case (int)LiveDataID.Water_Temperature:
                //	IconButtonTapped(sender, CurrentLiveData[(int)LiveDataID.Water_Temperature]);
                //	break;
                //case (int)LiveDataID.Oil_Pressure:
                //	IconButtonTapped(sender, CurrentLiveData[(int)LiveDataID.Oil_Pressure]);
                //	break;
                //case (int)LiveDataID.LSD:
                //	IconButtonTapped(sender, CurrentLiveData[(int)LiveDataID.LSD]);
                //	break;
                //case (int)LiveDataID.Turbo_Boost:
                //	IconButtonTapped(sender, CurrentLiveData[(int)LiveDataID.Turbo_Boost]);
                //	break;
                //case (int)LiveDataID.AF:
                //	IconButtonTapped(sender, CurrentLiveData[(int)LiveDataID.AF]);
                //	break;
                //case (int)LiveDataID.Intake:
                //	IconButtonTapped(sender, CurrentLiveData[(int)LiveDataID.Intake]);
                //	break;
                //case (int)LiveDataID.Electric_Pressure:
                //	IconButtonTapped(sender, CurrentLiveData[(int)LiveDataID.Electric_Pressure]);
                //	break;
                //case (int)LiveDataID.Exhaust_Temperature:
                //IconButtonTapped(sender, CurrentLiveData[(int)LiveDataID.Exhaust_Temperature]);
                //break;
                //    default:
                //        IconButtonTapped(sender, new LiveData("Fuck",9527));
                //        break;

                //}

            }
        }

        public void SetLiveData(LiveData Livedata, int index)
        {
            this.CurrentLiveData[index - 1] = Livedata;
        }

        //public override void LayoutSubviews()
        //{
        //          //base.LayoutSubviews();


        //          //if (NameLabel != null) 
        //          //NameLabel.Frame = new CGRect (10, 10, ContentView.Bounds.Width - 20, ContentView.Bounds.Height - 20);

        //          //if (SpeakButton != null) {
        //          //CGSize ButtonSize = new CGSize (60, 30);
        //          //SpeakButton.Frame = new CGRect (new CGPoint (ContentView.Bounds.Width - ButtonSize.Width - 10, ContentView.Bounds.Height / 2.0f - ButtonSize.Height / 2.0f), ButtonSize);
        //          //}

        //}
    }



    /// <summary>
    /// An example data model class. Nothing fancy.
    /// </summary>
    public class LiveData
    {
        public string Name { get; private set; }
        public int ID { get; private set; }

        public LiveData() { }

        public LiveData(string Name, int ID)
        {
            this.Name = Name;
            this.ID = ID;
        }
    }



}


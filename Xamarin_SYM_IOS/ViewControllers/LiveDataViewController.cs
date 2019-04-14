using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using CoreFoundation;
using CoreGraphics;
using IcmLib.Data;
using Steema.TeeChart;
using Steema.TeeChart.Styles;
using UIKit;
using static CommonLib.IcmLib.Database.DemoBinUnpacker;

namespace Xamarin_SYM_IOS
{
	public struct LabelReslID
	{
		public const string LeftTop = "labelLeftTop";
		public const string RightTop = "labelRightTop";
		public const string LeftBottom = "labelLeftBottom";
		public const string RightBottom = "labelRightBottom";
		public static string position;
	}


	public partial class LiveDataViewController : CustomViewController, LiveDataGaugesViewController.BtnOkCallBack
	{
		//指針圖
		private UIImageView imgNeedle;
		//0的起始角(需先跟美工說好)
		private readonly float START_ANGLE = -125.0f;
		//10的起始角
		private readonly float END_ANGLE = 125.0f;
		//RPM的最大值
		private readonly float MAX_VALUE = 10.0f;

		public static LiveDataViewController Instance;
		//目前點選的Label
		public static string currentSelectedLabel = "";
		//四個位置標頭
		public string[] titles = new string[4];

		public int[] selectedPositions = { 0, 1, 2, 3 };//選擇的position

		public List<string> liveDataItems = new List<string>();

		private TChart tChart = new TChart();
		private DmData data = new DmData();

		private static string currentGauge = "test";

		private int leftTop = 1;
		private int rightTop = 2;
		private int leftBottom = 3;
		private int rightBottom = 4;

        //指針圖片
        private UIImageView needleImageView;
        //指針數值
        private float speedometerCurrentValue;
        //前一個數值的角度
        private float preAngleFactor;
        //指針需要旋轉的角度
        private float angle;
        //錶頭的最大值
        private string maxVal;

        private float fuelConsumptionCompensationPercent;

		//------------------------------------生命週期分隔線---------------------------------------

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			UpdateTimerValue += LiveDataViewController_UpdateTimerValue;


			Instance = this;
			//實作手勢監聽,四個位置都要...
			FourLabelTouchListener();

			SetTitlesByEcuNames();

			//AddTeeChartView();
            AddNewGauge();
			

			ChangeFontStyleAndSize(30, Main.PHONE_SIZE_LIVEDATA_TITLE);
            //ChangeFontStyleAndSize(Main.PHONE_SIZE_LIVEDATA_VALUE, Main.PHONE_SIZE_LIVEDATA_TITLE);

            fuelConsumptionCompensationPercent = ContainerViewController.Instance.spf.GetFuelConsumptionCompensationPercent();
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			Console.WriteLine("LiveDataViewController: " + "ViewWillAppear()");

		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			Console.WriteLine("LiveDataViewController: " + "ViewDidAppear()");


			IsInited = true;
            if (StateMachine.IsActivted)
            {   
                StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV_STOP);
                StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV);
            }

			if (AppAttribute.APP_RUNNING_MODE == AppAttribute.RunningMode.DoUITest)
			{
				UIView.BeginAnimations(null);
				UIView.SetAnimationDuration(2.0f);

				imgNeedle.Transform = CGAffineTransform.MakeRotation(CaculateAngleByFormula(0));

				UIView.CommitAnimations();
			}
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			Console.WriteLine("LiveDataViewController: " + "ViewWillDisappear()");
			
            if (StateMachine.IsActivted)
				StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV_STOP);
		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);
			Console.WriteLine("LiveDataViewController: " + "ViewDidDisappear()");

			setCurrentPageName();
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();

		}

		//計算指針需要旋轉的角度
		private float CaculateAngleByFormula(float val)
		{
			return (float)((Math.PI / 180f) * (START_ANGLE + ((END_ANGLE - START_ANGLE) / MAX_VALUE) * val));
		}

		private void AddNewGauge()
		{
			//meter
			UIImageView meterImageView = new UIImageView(new CGRect(0, 0, 286, 286));
			meterImageView.Image = UIImage.FromFile("meter_1.png");
			meterImageView.Center = new CGPoint(Main.SCREEN_SIZE.Width / 2, Main.SCREEN_SIZE.Height / 2.11);
			this.View.AddSubview(meterImageView);

			//needle
			imgNeedle = new UIImageView(new CGRect(0, 0, 22, 84));
			imgNeedle.Image = UIImage.FromFile("arrow_1");
            imgNeedle.Center = new CGPoint(Main.SCREEN_SIZE.Width / 2, Main.SCREEN_SIZE.Height / 2.11);
			imgNeedle.Layer.AnchorPoint = new CGPoint(0.5, 0.5 * 2);
			this.View.AddSubview(imgNeedle);


			//center_wheel
			UIImageView meterDotImageView = new UIImageView(new CGRect(0, 0, 43, 38));
			meterDotImageView.Image = UIImage.FromFile("center_wheel_1.png");
			meterDotImageView.Center = new CGPoint(Main.SCREEN_SIZE.Width / 2, Main.SCREEN_SIZE.Height / 2.11);
			this.View.AddSubview(meterDotImageView);
		}

       

		//給予轉速表值
        private void SetSpeedometerCurrentValue(float theValue){
            speedometerCurrentValue = theValue/100;//先除100模擬
            CalculateDeviationAngle();
        }

		//計算指針需要的旋轉角度//
		private void CalculateDeviationAngle(){

            if (float.Parse(maxVal) > 0){
                angle = ((speedometerCurrentValue * 237.4f) / float.Parse(maxVal)) - 118.4f;
            }
            else{
                angle = 0;
            }

            if (angle <= -118.4f)
			{
                angle = -118.4f;
			}

            if (angle >= 119f)
			{//119度為"值100"的弧度
                angle = 119f;
			}

            if (Math.Abs(angle - preAngleFactor) > 180f){
                UIView.BeginAnimations(null);
				RotateIt(angle/3);
				UIView.CommitAnimations();

				UIView.BeginAnimations(null);
                RotateIt((angle*2)/3);
				UIView.CommitAnimations();
			}

            preAngleFactor = angle;

            RoateNeedle();
        }

		//第一次載入時需要旋轉多少
		private void RotateIt(float angl)
		{
			UIView.BeginAnimations(null);
			//UIView.SetAnimationDuration(0.025f);
			needleImageView.Transform = CGAffineTransform.MakeRotation((nfloat)((Math.PI / 180) * angl));
			UIView.CommitAnimations();
		}

		//旋轉指針
		private void RoateNeedle()
		{
            UIView.BeginAnimations(null);
            //UIView.SetAnimationDuration(0.025f);
			needleImageView.Transform = CGAffineTransform.MakeRotation((nfloat)((Math.PI / 180) * angle));
			UIView.CommitAnimations();

		}

		//Callback
		public void RestoreView()
		{
			RefreshUnitView();
		}

		private void RefreshUnitView()
		{
			//var length = ContainerViewController.Instance.View.Subviews[5].Subviews.Length;
			//ContainerViewController.Instance.View.Subviews[5].Subviews[length - 1].RemoveFromSuperview();
			var count = this.View.Subviews.Length;
			this.View.Subviews[count - 1].RemoveFromSuperview();
			this.View.AddSubview(UnitView());
		}

		public void DynamicChangeGauge(DmData data)
		{
			if (data != null)
			{
				this.data = data;
				currentGauge = data.Name;
				//tChart.Header.Text = data.Name;//有問題
				tChart.Series.RemoveAt(0);
				//tChart.Series.Remove(MyCustomTeeChart.Gauge);
				tChart.Series.Add(MyCustomTeeChart.ChangeGauge(data)); //加入錶頭
				//tChart.ClickSeries += TChart1_ClickSeries;
			}
			else
				return;

		}
		//.......................Button功能..........................

		public void TChart1_ClickSeries(object sender, Series s, int valueIndex, UIGestureRecognizer e)
		{
			//tChart.Header.Text = "";	
			Console.WriteLine("準備要呈現的畫面");
			Console.WriteLine("準備完畢");

			if (StateMachine.IsActivted)
				StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV_STOP);

			Console.WriteLine("叫出Gauge選單(TableView)之前");
			if (ContainerViewController.IsAlive)
				ContainerViewController.Instance.PerformSegue(MyCustomPages.buttonRespondSegueID["gaugeView"], null);

		}

		//@@@@@@
		private void ShowLVItemsDialog(string labelID)
		{
			if (StateMachine.IsActivted)
				StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV_STOP);

			LabelReslID.position = labelID;

			ContainerViewController.Instance.PerformSegue(MyCustomPages.buttonRespondSegueID[labelID], null);
		}


		private void FourLabelTouchListener()//如果要監聽label的event必須要寫的手勢監聽func
		{
            bool isEnable = StateMachine.Instance.IsUseCommunicationMode;    
			//左上
            lblLeftTopValue.UserInteractionEnabled = isEnable;
			lblLeftTopValue.AddGestureRecognizer(new UITapGestureRecognizer(sender =>
			{
				currentSelectedLabel = LabelReslID.LeftTop;
				ShowLVItemsDialog(LabelReslID.LeftTop);
			}));

			//右上
            lblRightTopValue.UserInteractionEnabled = isEnable;
			lblRightTopValue.AddGestureRecognizer(new UITapGestureRecognizer(sender =>
			{
				currentSelectedLabel = LabelReslID.RightTop;
				ShowLVItemsDialog(LabelReslID.RightTop);
			}));

			//左下
            lblLeftBottomValue.UserInteractionEnabled = isEnable;
			lblLeftBottomValue.AddGestureRecognizer(new UITapGestureRecognizer(sender =>
			{
				currentSelectedLabel = LabelReslID.LeftBottom;
				ShowLVItemsDialog(LabelReslID.LeftBottom);
			}));

			//右下
            lblRightBottomValue.UserInteractionEnabled = isEnable;
			lblRightBottomValue.AddGestureRecognizer(new UITapGestureRecognizer(sender =>
			{
				currentSelectedLabel = LabelReslID.RightBottom;
				ShowLVItemsDialog(LabelReslID.RightBottom);
			}));
		}



		//||||||||||||||||||||||||||||||||||||||自定義功能||||||||||||||||||||||||||||||||||||
		private UILabel UnitView()
		{
			var unitText = new UILabel();
			unitText.Text = data.ChartDisplayText;
			unitText.TextColor = UIColor.Gray;
			unitText.Font = UIFont.SystemFontOfSize(Main.PHONE_SIZE_LIVEDATA_UNITVIEW);
			unitText.Frame = new CGRect(Main.SCREEN_SIZE.Width / 2.3, Main.SCREEN_SIZE.Height / 1.86
										, 58, 55);
			unitText.TextAlignment = UITextAlignment.Center;
			unitText.BackgroundColor = UIColor.FromRGBA(255, 255, 255, 0);

			return unitText;
		}


		//判定數據是否變化
		public bool IsDataChanged(ref float preValue, ref float currentValue, float newValue)
		{
			//做判定數據是否有變化，有變化才更新
			currentValue = newValue;
			if (currentValue.Equals(preValue))
				return false;
			else
			{
				preValue = currentValue;
				return true;
			}

		}
		//如果值有變化才重設ui圖片,不然一直set畫面會lag
		private float fuel1StatusPrevValue = 0.0f;
		private float fuel1StatusCurrentValue = 0.0f;

		private void SetFuel1StatusImage(float value)
		{
			if (!IsDataChanged(ref fuel1StatusPrevValue, ref fuel1StatusCurrentValue, value))
				return;

			UIImage image = null;
			switch ((int)value)
			{
				case 0:
					image = ImageInitializer.img_transparent;
					break;
				case 1:
					image = ImageInitializer.img_ol_orange;
					break;
				case 2:
					image = ImageInitializer.img_cl_green;
					break;
				case 4:
					image = ImageInitializer.img_ol_drive_orange;
					break;
				case 8:
					image = ImageInitializer.img_ol_fault_red;
					break;
				case 10:
					image = ImageInitializer.img_cl_fault_red;
					break;
			}
			if (image != null)
				imgFuelStatus1.Image = image;
		}

		private float fuel2StatusPrevValue = 0.0f;
		private float fuel2StatusCurrentValue = 0.0f;
		private void SetFuel2StatusImage(float value)
		{
			if (!IsDataChanged(ref fuel2StatusPrevValue, ref fuel2StatusCurrentValue, value))
				return;

			UIImage image = null;
			switch ((int)value)
			{
				case 0:
					image = ImageInitializer.img_transparent;
					break;
				case 1:
					image = ImageInitializer.img_ol_orange;
					break;
				case 2:
					image = ImageInitializer.img_cl_green;
					break;
				case 4:
					image = ImageInitializer.img_ol_drive_orange;
					break;
				case 8:
					image = ImageInitializer.img_ol_fault_red;
					break;
				case 10:
					image = ImageInitializer.img_cl_fault_red;
					break;
			}
			if (image != null)
				imgFuelStatus2.Image = image;
		}


		private float fuelTrimBank1PrevValue = 0.0f;
		private float fuelTrimBank1CurrentValue = 0.0f;
		private void SetFuelTrimBank1Image(float value)
		{
			if (!IsDataChanged(ref fuelTrimBank1PrevValue, ref fuelTrimBank1CurrentValue, value))
				return;

			UIImage image = null;
			if (value == 0.0f)
				image = ImageInitializer.img_lean_gray;
			else if (value > 0.0f)
				image = ImageInitializer.img_rich_red;
			else
				image = ImageInitializer.img_lean_blue;

			if (image != null)
				imgShortTermFuelTrimBank1.Image = image;
		}

		private float fuelTrimBank2PrevValue = 0.0f;
		private float fuelTrimBank2CurrentValue = 0.0f;
		private void SetFuelTrimBank2Image(float value)
		{
			if (!IsDataChanged(ref fuelTrimBank2PrevValue, ref fuelTrimBank2CurrentValue, value))
				return;

			UIImage image = null;
			if (value == 0.0f)
				image = ImageInitializer.img_lean_gray;
			else if (value > 0.0f)
				image = ImageInitializer.img_rich_red;
			else
				image = ImageInitializer.img_lean_blue;

			if (image != null)
				imgShortTermFuelTrimBank2.Image = image;
		}







		private float oxygenBank1PrevValue = -9999.0f;
		private float oxygenBank1CurrentValue = 0.0f;
		/// <summary>
		/// 設定ShortTermFuelTrimBank1的濃稀度圖式
		/// Lv Id 為 Oxygen_Sensor_Output_Voltage_B1_S1時，
		/// 數據 > 0.45時，呈現紅色RICH；反之,呈現藍色LEAN。
		/// Lv Id 為 Equivalence_Ratio_lambda_B1_S1時，
		/// 數據 < 1.000時，呈現紅色RICH；反之,呈現藍色LEAN。
		/// </summary>
		/// <param name="value">通訊完解析後的數值</param>
		public void SetOxygenBank1Image(int lvId, float value)
		{
			//判定數據是否有變化，有變化才更新
			if (!IsDataChanged(ref oxygenBank1PrevValue, ref oxygenBank1CurrentValue, value))
				return;
			UIImage image = null;
			switch (lvId)
			{
				case (int)LiveDataItemsSN.Oxygen_Sensor_Output_Voltage_B1_S1:
					if (value > 0.45f)
						image = ImageInitializer.img_rich_red;
					else
						image = ImageInitializer.img_lean_blue;
					break;

				case (int)LiveDataItemsSN.Equivalence_Ratio_lambda_B1_S1:
					if (value < 1.00f)
						image = ImageInitializer.img_rich_red;
					else
						image = ImageInitializer.img_lean_blue;
					break;

				default:
					break;
			}


			if (image != null)
				imgShortTermFuelTrimBank1.Image = image;
		}

		private float oxygenBank2PrevValue = -9999.0f;
		private float oxygenBank2CurrentValue = 0.0f;
		/// <summary>
		/// 設定ShortTermFuelTrimBank2的濃稀度圖式
		/// Lv Id 為 Oxygen_Sensor_Output_Voltage_B2_S1時，
		/// 數據 > 0.45時，呈現紅色RICH；反之,呈現藍色LEAN。
		/// Lv Id 為 Equivalence_Ratio_lambda_B2_S1時，
		/// 數據 > 1.000時，呈現紅色RICH；反之,呈現藍色LEAN。
		/// </summary>
		/// <param name="value">通訊完解析後的數值</param>
		public void SetOxygenBank2Image(int lvId, float value)
		{
			//判定數據是否有變化，有變化才更新
			if (!IsDataChanged(ref oxygenBank2PrevValue, ref oxygenBank2CurrentValue, value))
				return;

			UIImage image = null;
			switch (lvId)
			{
				case (int)LiveDataItemsSN.Oxygen_Sensor_Output_Voltage_B2_S1:
					if (value > 0.45f)
						image = ImageInitializer.img_rich_red;
					else
						image = ImageInitializer.img_lean_blue;
					break;

				case (int)LiveDataItemsSN.Equivalence_Ratio_lambda_B2_S1:
					if (value > 1.00f)
						image = ImageInitializer.img_rich_red;
					else
						image = ImageInitializer.img_lean_blue;
					break;

				default:
					break;
			}

			if (image != null)
				imgShortTermFuelTrimBank2.Image = image;
		}

		private float GetFixedRpmValue(float value)
		{
           

			float rpmValue = value / 1000;

			if (rpmValue <= 0)
				rpmValue = 0;
			if (rpmValue >= 10.0)
				rpmValue = 10.0f;

			return rpmValue;

		}

		public List<string> RefreshLiveDataItems(List<string> datas)
		{
			datas.Clear();

			if (liveDataItems != null)
				datas = liveDataItems;
			return datas;
		}

		public void RefreshTitles(int[] index)
		{
			for (int i = 0; i < index.Length; i++)
			{	
				
				titles[i] = liveDataItems[index[i]];
			}
		}

		private void SetTitlesByEcuNames()
		{
			if (StateMachine.IsActivted)
			{
				//此語法相當於list.Clone()
				liveDataItems = new List<string>(StateMachine.DataModel.AllEcuLvNames);

				//一開始初始化,數值單位也要嗎?
				if (liveDataItems.Count >= 4)
				{
					for (int i = 0; i < liveDataItems.Count(); i++)
					{
						titles[i] = liveDataItems[i];
						titles[i] = liveDataItems[i];
						titles[i] = liveDataItems[i];
						titles[i] = liveDataItems[i];
						if (i == 3)
							break;

					}
					lblLeftTopTitle.Text = titles[0];
					lblRightTopTitle.Text = titles[1];
					lblLeftBottomTitle.Text = titles[2];
					lblRightBottomTitle.Text = titles[3];
				}
				else {
					lblLeftTopTitle.Text = "";
					lblRightTopTitle.Text = "";
					lblLeftBottomTitle.Text = "";
					lblRightBottomTitle.Text = "";
				}

				//if (liveDataItems.Count >= 4)
				//{
				//	titles[0] = liveDataItems[0];
				//	titles[1] = liveDataItems[1];
				//	titles[2] = liveDataItems[2];
				//	titles[3] = liveDataItems[3];

				//	lblLeftTopTitle.Text = titles[0];
				//	lblRightTopTitle.Text = titles[1];
				//	lblLeftBottomTitle.Text = titles[2];
				//	lblRightBottomTitle.Text = titles[3];
				//}

			}
		}

		private void AddTeeChartView()
		{
			//MyCustomTeeChart.CreateLvRpmGauge(this.View);
			this.data.ID = 9999;
			this.data.ChartDisplayText = "x1000";

			//tChart.Frame = new CGRect(36, 176, 340, 340);//如果是auto layout,這裡要改活的
			tChart.Frame = new CGRect(0, Main.SCREEN_SIZE.Height / 3.96, Main.SCREEN_SIZE.Width, Main.SCREEN_SIZE.Width / 1.28); //1.217)
			tChart.Panel.Transparent = true; //去背景
			tChart.Walls.Back.Transparent = true;
			tChart.BackgroundColor = UIColor.FromRGBA(255, 255, 255, 0); ;

			//tChart.Header.Text = "Engine revolution speed";//圖型上面的小字
			tChart.Header.Visible = false;
			tChart.Header.Font.Color = Color.White;
			tChart.Header.Font.Size = 12;
			tChart.Series.Add(MyCustomTeeChart.DefaultGauge(tChart.Chart));
			//tChart.ClickSeries += TChart1_ClickSeries;

			this.View.AddSubview(tChart);
			this.View.AddSubview(UnitView());
		}

		private void ChangeFontStyleAndSize(float size, float size2)
		{
			//lblLeftTopValue.Font = UIFont.FromName(@"Digital-7", size);
			//lblRightTopValue.Font = UIFont.FromName(@"Digital-7", size);
			//lblLeftBottomValue.Font = UIFont.FromName(@"Digital-7", size);
			//lblRightBottomValue.Font = UIFont.FromName(@"Digital-7", size);

			lblLeftTopValue.Font     = UIFont.SystemFontOfSize(size);
			lblRightTopValue.Font    = UIFont.SystemFontOfSize(size);
			lblLeftBottomValue.Font  = UIFont.SystemFontOfSize(size);
			lblRightBottomValue.Font = UIFont.SystemFontOfSize(size);

			lblLeftTopTitle.Font     = UIFont.SystemFontOfSize(size2);
			lblRightTopTitle.Font    = UIFont.SystemFontOfSize(size2);
			lblLeftBottomTitle.Font  = UIFont.SystemFontOfSize(size2);
			lblRightBottomTitle.Font = UIFont.SystemFontOfSize(size2);
		}

		

		private float FixGaugeValue(float value)
		{
			if (LiveDataGaugesViewController.Instance != null)
			{
				value = value / LiveDataGaugesViewController.Instance.divideVal;
				if (value > LiveDataGaugesViewController.Instance.maxVal)
					value = (float)LiveDataGaugesViewController.Instance.maxVal;

				return value;
			}

			return value;
		}

		/// <summary>
		/// Gets the support live data identifier By StateMachine.
		/// </summary>
		/// <returns>The support live data identifier.</returns>
		public List<int> GetSupportLiveDataID()
		{
			var id = new List<int>();
			id.Add(1);
			id.Add(2);
			id.Add(3);
			id.Add(4);
			id.Add(5);
			id.Add(6);
			return id;
		}

        private List<DmDataGroup> demoLvValues = null;
        private int currentLvDataIndex = 0;
        private int accumulation = 1;

        public void LiveDataViewController_UpdateTimerValue()
		{
            
            var lvPos = selectedPositions;
			//拿到此頁面需要的8個LiveData資料
			var lvValuesStrList = StateMachine.DataModel.LvValuesString;
            var lvValues = StateMachine.DataModel.LvValues;

            if (StateMachine.Instance.IsUseCommunicationMode)
            {
                if (lvValuesStrList == null)
                    return;
            }
			if (lvPos.Length > 0 && lvValuesStrList.Count > 0)
			{
                
				//Console.WriteLine("lvValuesStrList.Count : " + lvPos.Length);

				//拿回87個Ecu ID
                var allEcuIds = StateMachine.DataModel.AllEcuLvIds.ToList();//ToList()相當於clone()
				if (allEcuIds.Count <= 0)
					return;
				try
				{
                    //if (ContainerViewController.Instance.uploadViewController != null)
                        //ContainerViewController.Instance.uploadViewController.SendLVData(lvValues);


					//第一步,設定四個label的值
					//拿回4個label對應的ecu id
					var leftTopLvId = allEcuIds[selectedPositions[0]];
					var rightTopLvId = allEcuIds[selectedPositions[1]];
					var leftBottomLvId = allEcuIds[selectedPositions[2]];
					var rightBottomLvId = allEcuIds[selectedPositions[3]];


					//lvValuesStrList裡面前四個物件剛好對應我們的四個label,但是要自己去拆字
					//總共會有8個,用迴圈去跑
					//資料長這個樣子 ->  2   Engine revolution speed	   6729.8	rpm
					for (int i = 0; i < lvValuesStrList.Count; i++)
					{
						//拆完之後拿到一個陣列,在自己去對應索引
						var data = lvValuesStrList[i].Split('\t');

						if (data != null)
						{
							//左上,右上,左下,右下共有三個label需要賦值
							//data索引 0,1,2,3對應 'ecuid','標頭','數值','單位' 
							if (data[0].Equals(leftTopLvId.ToString()))
							{
								//if (data[4] != null)
                                if (!data[4].Equals(""))
									lblLeftTopTitle.Text = data[4];
								else
									lblLeftTopTitle.Text = data[1];

                                //如果是選到瞬時油耗或油耗
                                if (leftTopLvId == (int)3901 || leftTopLvId == (int)3902 )
                                {
                                    double tmp;
                                    Double.TryParse(data[2],out tmp);
									tmp = tmp * ContainerViewController.Instance.FuelConsumptionCompensationPercent / 100.0;
									lblLeftTopValue.Text = tmp.ToString("0.00");
                                        
                                }
                                else
								    lblLeftTopValue.Text = data[2];

								lblLeftTopUnit.Text = data[3];
							}

							else if (data[0].Equals(rightTopLvId.ToString()))
							{
								//if (data[4] != null)
                                if (!data[4].Equals(""))
									lblRightTopTitle.Text = data[4];
								else
									lblRightTopTitle.Text = data[1];

								//如果是選到瞬時油耗或油耗
								if (rightTopLvId == (int)3901 || rightTopLvId == (int)3902)
								{
									double tmp;
									Double.TryParse(data[2], out tmp);
                                    tmp = tmp * ContainerViewController.Instance.FuelConsumptionCompensationPercent / 100.0;
									lblRightTopValue.Text = tmp.ToString("0.00");

								}
								else
									lblRightTopValue.Text = data[2];

								
								lblRightTopUnit.Text = data[3];
							}

							else if (data[0].Equals(leftBottomLvId.ToString()))
							{
								if (!data[4].Equals(""))
									lblLeftBottomTitle.Text = data[4];
								else
									lblLeftBottomTitle.Text = data[1];

								//如果是選到瞬時油耗或油耗
								if (leftBottomLvId == (int)3901 || leftBottomLvId == (int)3902)
								{
									double tmp;
									Double.TryParse(data[2], out tmp);
									tmp = tmp * ContainerViewController.Instance.FuelConsumptionCompensationPercent / 100.0;
									lblLeftBottomValue.Text = tmp.ToString("0.00");

								}
								else
									lblLeftBottomValue.Text = data[2];

								
								lblLeftBottomUnit.Text = data[3];
							}

							else if (data[0].Equals(rightBottomLvId.ToString()))
							{
								//if (data[4] != null)
                                if (!data[4].Equals(""))
									lblRightBottomTitle.Text = data[4];
								else
									lblRightBottomTitle.Text = data[1];

								//如果是選到瞬時油耗或油耗
								if (rightBottomLvId == (int)3901 || rightBottomLvId == (int)3902)
								{
									double tmp;
									Double.TryParse(data[2], out tmp);
									tmp = tmp * ContainerViewController.Instance.FuelConsumptionCompensationPercent / 100.0;
                                    lblRightBottomValue.Text = tmp.ToString("0.00");

								}
								else
									lblRightBottomValue.Text = data[2];

								lblRightBottomUnit.Text = data[3];
							}

						}

						//第二步,讀取其它liveData數值,並設定狀態圖片
						//拿到dictionary,給iD還你值
						var lvDatas = StateMachine.DataModel.LvValues;

						//VSS,車速
						if (LiveDataGaugesViewController.Instance == null)
						{
                            //此函數呼叫與rpm不同，是為了當初多個錶設定
                            if (lvDatas.ContainsKey((int)LiveDataItemsSN.RPM))
                            {
                                //SetSpeedometerCurrentValue(lvDatas[(int)LiveDataItemsSN.RPM]);
								//MyCustomTeeChart.SetGaugeValue(GetFixedRpmValue(lvDatas[(int)LiveDataItemsSN.RPM]));

								UIView.BeginAnimations(null);
								UIView.SetAnimationDuration(0.0);
								imgNeedle.Transform = CGAffineTransform.MakeRotation(CaculateAngleByFormula(GetFixedRpmValue(lvDatas[(int)LiveDataItemsSN.RPM])));
								UIView.CommitAnimations();
							}
								
						}
						else
						{	
							
							if (lvDatas.ContainsKey((int)this.data.ID))
								MyCustomTeeChart.SetGaugeValue(FixGaugeValue(lvDatas[(int)this.data.ID]));
						}

						//if(lvDatas.ContainsKey((int)LiveDataItemsSN.RPM))
						//	MyCustomTeeChart.SetGaugeValue(GetFixedRpmValue(lvDatas[(int)LiveDataItemsSN.RPM]));
						
						//Fuel_System_1_Status
						if (lvDatas.ContainsKey((int)LiveDataItemsSN.Fuel_System_1_Status))
						{
							SetFuel1StatusImage(lvDatas[(int)LiveDataItemsSN.Fuel_System_1_Status]);
							int richLeanBank1Id;
							//Oxygen Sensor Output Voltage(B1-S1)
							if (lvDatas.ContainsKey((int)LiveDataItemsSN.Oxygen_Sensor_Output_Voltage_B1_S1))
							{
								richLeanBank1Id = (int)LiveDataItemsSN.Oxygen_Sensor_Output_Voltage_B1_S1;
								SetOxygenBank1Image(richLeanBank1Id, lvDatas[richLeanBank1Id]);
							}
							//Equivalence Ratio (lambda) (B1-S1)
							else if (lvDatas.ContainsKey((int)LiveDataItemsSN.Equivalence_Ratio_lambda_B1_S1))
							{
								richLeanBank1Id = (int)LiveDataItemsSN.Equivalence_Ratio_lambda_B1_S1;
								SetOxygenBank1Image(richLeanBank1Id, lvDatas[richLeanBank1Id]);
							}
						}

						//Fuel_System_2_Status
						if (lvDatas.ContainsKey((int)LiveDataItemsSN.Fuel_System_2_Status))
						{
							SetFuel2StatusImage(lvDatas[(int)LiveDataItemsSN.Fuel_System_2_Status]);
							int richLeanBank2Id;
							//Oxygen Sensor Output Voltage(B2-S1)
							if (lvDatas.ContainsKey((int)LiveDataItemsSN.Oxygen_Sensor_Output_Voltage_B2_S1))
							{
								richLeanBank2Id = (int)LiveDataItemsSN.Oxygen_Sensor_Output_Voltage_B2_S1;
								SetOxygenBank2Image(richLeanBank2Id, lvDatas[richLeanBank2Id]);
							}
							//Equivalence Ratio (lambda) (B2-S1)
							else if (lvDatas.ContainsKey((int)LiveDataItemsSN.Equivalence_Ratio_lambda_B2_S1))
							{
								richLeanBank2Id = (int)LiveDataItemsSN.Equivalence_Ratio_lambda_B2_S1;
								SetOxygenBank2Image(richLeanBank2Id, lvDatas[richLeanBank2Id]);
							}
						}

						//2017.01.04
						////Fuel_System_1_Status
						//if (lvDatas.ContainsKey((int)LiveDataItemsSN.Fuel_System_1_Status))
						//	SetFuel1StatusImage(lvDatas[(int)LiveDataItemsSN.Fuel_System_1_Status]);

						////Fuel_System_2_Status
						//if(lvDatas.ContainsKey((int)LiveDataItemsSN.Fuel_System_2_Status))
						//	SetFuel2StatusImage(lvDatas[(int)LiveDataItemsSN.Fuel_System_2_Status]);

						//////Short_Term_Fuel_Trim_Bank_1
						////if (lvDatas.ContainsKey((int)LiveDataItemsSN.Short_Term_Fuel_Trim_Bank_1))
						////	SetFuelTrimBank1Image(lvDatas[(int)LiveDataItemsSN.Short_Term_Fuel_Trim_Bank_1]);

						//////Short_Term_Fuel_Trim_Bank_2
						////if (lvDatas.ContainsKey((int)LiveDataItemsSN.Short_Term_Fuel_Trim_Bank_2))
						////	SetFuelTrimBank2Image(lvDatas[(int)LiveDataItemsSN.Short_Term_Fuel_Trim_Bank_2]);

						////Oxygen Sensor Output Voltage(B1-S1)
						//if (lvDatas.ContainsKey((int)LiveDataItemsSN.Oxygen_Sensor_Output_Voltage_B1_S1))
						//{
						//	SetOxygenBank1Image(lvDatas[(int)LiveDataItemsSN.Oxygen_Sensor_Output_Voltage_B1_S1]);
						//}

						////Oxygen Sensor Output Voltage(B2-S1)
						//if (lvDatas.ContainsKey((int)LiveDataItemsSN.Oxygen_Sensor_Output_Voltage_B1_S2))
						//{
						//	SetOxygenBank2Image(lvDatas[(int)LiveDataItemsSN.Oxygen_Sensor_Output_Voltage_B1_S2]);
						//}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.StackTrace);
				}


			}
            else if (!StateMachine.Instance.IsUseCommunicationMode)
            {
                try
                {
                    //if (addPRM)
                    //{
                    //    simRPM += 100;
                    //    if (simRPM > 10000)
                    //    {
                    //        simRPM = 10000;
                    //        addPRM = !addPRM;
                    //    }
                    //}
                    //else
                    //{
                    //    simRPM -= 100;
                    //    if (simRPM < 0)
                    //    {
                    //        simRPM = 0;
                    //        addPRM = !addPRM;
                    //    }
                    //}
                    if (StateMachine.DataModel.demoBinUnpacker == null) return;
                    if (demoLvValues == null)
                        demoLvValues = StateMachine.DataModel.DemoLvDatas;

                    var dmDataGroup = demoLvValues[currentLvDataIndex];
                    currentLvDataIndex += accumulation;
                    if (currentLvDataIndex >= demoLvValues.Count)
                    {
                        currentLvDataIndex = demoLvValues.Count - 1;
                        accumulation = -1;
                    }
                    else if (currentLvDataIndex <= 0)
                    {
                        currentLvDataIndex = 0;
                        accumulation = 1;
                    }
                    var rpmData = dmDataGroup.DmDataDict[0x0002];
                    var airPressureData = dmDataGroup.DmDataDict[0x02D9];
                    var o2BatteryData = dmDataGroup.DmDataDict[0x2EE];
                    var batteryData = dmDataGroup.DmDataDict[0x2ED];

                    SetGaugeValue((float)rpmData.MaxValue);

                    lblLeftTopTitle.Text = rpmData.Name;
                    //lblLeftTopValue.Text = "" + rpmData.MaxValue.ToString("n" + rpmData.NumberOfDecimals);
                    lblLeftTopValue.Text = "" + rpmData.MaxValue;
                    lblLeftTopUnit.Text = rpmData.Unit;

                    lblRightTopTitle.Text = airPressureData.Name;
                    //lblRightTopValue.Text = "" + airPressureData.MaxValue.ToString("n" + airPressureData.NumberOfDecimals);
                    lblRightTopValue.Text = "" + airPressureData.MaxValue;
                    lblRightTopUnit.Text = airPressureData.Unit;

                    lblLeftBottomTitle.Text = o2BatteryData.Name;
                    lblLeftBottomValue.Text = "" + o2BatteryData.MaxValue;
                    //lblLeftBottomValue.Text = "" + o2BatteryData.MaxValue.ToString("n" + o2BatteryData.NumberOfDecimals);;
                    lblLeftBottomUnit.Text = o2BatteryData.Unit;

                    //RIGHT BOTTOM
                    lblRightBottomTitle.Text = batteryData.Name;
                    //lblRightBottomValue.Text = "" + batteryData.MaxValue.ToString("n" + batteryData.NumberOfDecimals);
                    lblRightBottomValue.Text = "" + batteryData.MaxValue;
                    lblRightBottomUnit.Text = batteryData.Unit;

                }
                catch (Exception ex)
                {
                }
            }
		}

        private void SetGaugeValue(float simRPM)
        {
            UIView.BeginAnimations(null);
            UIView.SetAnimationDuration(0.0);
            imgNeedle.Transform = CGAffineTransform.MakeRotation(CaculateAngleByFormula(GetFixedRpmValue(simRPM)));
            UIView.CommitAnimations();
        }

        int simRPM = 0;
        bool addPRM = false;

		public override void setCurrentPageName()
		{
			currentPageView = "LiveDataView";
			//Console.WriteLine("current page is " + currentPageView);
		}

		protected LiveDataViewController(IntPtr handle) : base(handle)
		{
		}

		public int LeftTop
		{
			get
			{
				return leftTop;
			}
			set
			{
				leftTop = value;
			}
		}

		public int RightTop
		{
			get
			{
				return rightTop;
			}
			set
			{
				rightTop = value;
			}
		}


		public int LeftBottom
		{
			get
			{
				return leftBottom;
			}
			set
			{
				leftBottom = value;
			}
		}

		public int RightBottom
		{
			get
			{
				return rightBottom;
			}
			set
			{
				rightBottom = value;
			}
		}

		public static string CurrentGauge
		{
			get
			{
				return currentGauge;
			}
			set
			{
				currentGauge = value;
			}
		}
	}
}




using System;
using System.Collections.Generic;
using System.Linq;
using CoreAnimation;
using CoreFoundation;
using CoreGraphics;
using UIKit;
using Xamarin_SYM_IOS.SRC.UI;

namespace Xamarin_SYM_IOS.ViewControllers
{
    public partial class LiveData_6_Frame_ViewController : CustomViewController
    {


		//gauge上數值的字體大小 
		private int VALUE_TEXT_SIZE;
		//gauge上單位的字體大小
		private int UNIT_TEXT_SIZE;
		//gauge上標題的字體大小
		private int TITLE_TEXT_SIZE;


		//gauge上數值的顏色
		private UIColor vColorRGB = UIColor.FromRGB(54, 227, 255);
		//gauge上單位的顏色
		private UIColor uColorRGB = UIColor.FromRGB(54, 227, 255);
		//gauge上標題的顏色
		private UIColor tColorRGB = UIColor.FromRGB(54, 227, 255);


		//gauge的高
		private float GAUGE_HEIGHT;
		//gauge上數值的高
		private float GAUGE_VALUE_HEIGHT;

		//gauges上的title們的位置
		private List<CGPoint> tCenters = new List<CGPoint>();
		//gauges上的單位們的位置
		private List<CGPoint> uCenters = new List<CGPoint>();
		//gauges上的數值們的位置
		private List<CGPoint> gCenters = new List<CGPoint>();

		//gauges上的數值們
		private List<UILabel> gValues = new List<UILabel>();
		//gauges上的單位們
		private List<UILabel> gUnits = new List<UILabel>();
		//gauges上的標題們
		private List<UILabel> gTitles = new List<UILabel>();

		//多個Maximum，更新用
		private List<float> MAXVALUE = new List<float>();
		//多個標題，更新用
		private List<string> UNITS = new List<string>();
		//多個單位，更新用
		private List<string> TITLES = new List<string>();


		//DataModel的datas，會不斷更新
		private List<float> datas;

		//由icon頁帶過來的liveData id
        //public List<int> liveData { get; set}
		private List<int> liveData;
		//由icon頁帶過來的gauge bg
		private List<string> lvGaugesBG;

		//微調gauge背景圖用
		private float offset;


		//gauge & frame圖
		private CALayer graphicGaugeView_1;
		private CALayer graphicGaugeView_2;
		private CALayer graphicGaugeView_3;
		private CALayer graphicGaugeView_4;
        private CALayer graphicGaugeView_5;
        private CALayer graphicGaugeView_6;

		//扇形1
		private GraphicFan fan_1;
		//扇形2
		private GraphicFan fan_2;
		//扇形4
		private GraphicFan fan_3;
		//扇形4
		private GraphicFan fan_4;
		//扇形5
		private GraphicFan fan_5;
		//扇形6
		private GraphicFan fan_6;




        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Console.WriteLine("LiveData_6_Frame_ViewController : " + "ViewDidLoad()");

            //給unit & title 調整位置用
			GAUGE_VALUE_HEIGHT = (float)(Main.SCREEN_SIZE.Width / 1.59f) / 8.67f;
			GAUGE_HEIGHT = SP_LV6_BG_Gauge.LT_GAUGE_SIZE(1.0f);
            //全屏調整位置的差值
            offset = (float)btnBack.Frame.Bottom;
			VALUE_TEXT_SIZE = (int)(offset / 1.54f);
			UNIT_TEXT_SIZE = (int)(offset / 3.75f);
			TITLE_TEXT_SIZE = (int)(offset / 2.19f);

            btnBack.TouchUpInside += BtnBack_TouchUpInside;
            UpdateTimerValue += LiveData_6_Frame_ViewController_UpdateTimerValue;

            IsInited = true;

			AddFan();
			AddBG(lvGaugesBG);
			AddTextForGauge();


            //InitGraphicValue();
			//fan.CenterX();
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
            Console.WriteLine("LiveData_6_Frame_ViewController : " + "ViewDidAppear()");


            SetFanDrawingKey(LiveDataIconViewController.Instance.SendLV);
            UpdateGaugesByUserSelected();
            //RefreshTextOnGauge();

			//foreach (int id in liveData)
			//{
			//	Console.WriteLine(" id : " + id);
			//}

			//for (int i = 0; i < lvGaugesBG.Count; i++)
			//{
			//	Console.WriteLine(" name : " + lvGaugesBG[i]);
			//}

			//fan_1.SetValue(7000.0f / 8000.0f);
			//fan_2.SetValue(7000.0f / 8000.0f);
			//fan_3.SetValue(7000.0f / 8000.0f);
			//fan_4.SetValue(7000.0f / 8000.0f);
            //fan_5.SetValue(7000.0f / 8000.0f);
            //fan_6.SetValue(7000.0f / 8000.0f);
		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);
            Console.WriteLine("LiveData_6_Frame_ViewController : " + "ViewDidDisappear()");

			//if (StateMachine.IsActivted)
				//StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV_STOP);
		}
		private void RefreshTextOnGauge()
		{
			if (UNITS.Count == 6)
			{
				gUnits[0].Text = UNITS[0];
				gUnits[1].Text = UNITS[1];
				gUnits[2].Text = UNITS[2];
				gUnits[3].Text = UNITS[3];
				gUnits[4].Text = UNITS[4];
				gUnits[5].Text = UNITS[5];
			}

			if (TITLES.Count == 6)
			{
				gTitles[0].Text = TITLES[0];
				gTitles[1].Text = TITLES[1];
				gTitles[2].Text = TITLES[2];
				gTitles[3].Text = TITLES[3];
				gTitles[4].Text = TITLES[4];
				gTitles[5].Text = TITLES[5];
			}
		}

		private void SetPosition()
		{
			//text 1的位置
			gCenters.Add(new CGPoint(SP_LV6_BG_Gauge.LT_GAUGE_CENTER_X(1.0f)
									 , SP_LV6_BG_Gauge.LT_GAUGE_CENTER_Y(1.0f, offset / 2.0f)));
			//text 2的位置
			gCenters.Add(new CGPoint(SP_LV6_BG_Gauge.RT_GAUGE_CENTER_X(1.0f)
									 , SP_LV6_BG_Gauge.RT_GAUGE_CENTER_Y(1.0f, offset / 2.0f)));
			//text 3的位置
			gCenters.Add(new CGPoint(SP_LV6_BG_Gauge.LC_GAUGE_CENTER_X(1.0f)
									 , SP_LV6_BG_Gauge.LC_GAUGE_CENTER_Y(1.0f, offset / 2.4f)));
			//text 4的位置
			gCenters.Add(new CGPoint(SP_LV6_BG_Gauge.RC_GAUGE_CENTER_X(1.0f)
									 , SP_LV6_BG_Gauge.RC_GAUGE_CENTER_Y(1.0f, offset / 2.4f)));
			//text 5的位置
			gCenters.Add(new CGPoint(SP_LV6_BG_Gauge.LB_GAUGE_CENTER_X(1.0f)
									 , SP_LV6_BG_Gauge.LB_GAUGE_CENTER_Y(1.0f, offset * 1.2f)));
			//text 6的位置
			gCenters.Add(new CGPoint(SP_LV6_BG_Gauge.RB_GAUGE_CENTER_X(1.0f)
									 , SP_LV6_BG_Gauge.RB_GAUGE_CENTER_Y(1.0f, offset * 1.2f)));



			//unit 1的位置
			uCenters.Add(new CGPoint(SP_LV6_BG_Gauge.LT_GAUGE_CENTER_X(1.0f)
                                     , SP_LV6_BG_Gauge.LT_GAUGE_CENTER_Y(1.0f, (offset / 2.0f) - GAUGE_VALUE_HEIGHT)));
			//unit 2的位置
			uCenters.Add(new CGPoint(SP_LV6_BG_Gauge.RT_GAUGE_CENTER_X(1.0f)
									 , SP_LV6_BG_Gauge.RT_GAUGE_CENTER_Y(1.0f, (offset / 2.0f) - GAUGE_VALUE_HEIGHT)));
			//unit 3的位置
			uCenters.Add(new CGPoint(SP_LV6_BG_Gauge.LC_GAUGE_CENTER_X(1.0f)
									 , SP_LV6_BG_Gauge.LC_GAUGE_CENTER_Y(1.0f, (offset / 2.4f) + GAUGE_VALUE_HEIGHT)));
			//unit 4的位置
			uCenters.Add(new CGPoint(SP_LV6_BG_Gauge.RC_GAUGE_CENTER_X(1.0f)
									 , SP_LV6_BG_Gauge.RC_GAUGE_CENTER_Y(1.0f, (offset / 2.4f) + GAUGE_VALUE_HEIGHT)));

			//unit 5的位置
			uCenters.Add(new CGPoint(SP_LV6_BG_Gauge.LB_GAUGE_CENTER_X(1.0f)
									 , SP_LV6_BG_Gauge.LB_GAUGE_CENTER_Y(1.0f, (offset * 1.2f) + GAUGE_VALUE_HEIGHT)));
			//unit 6的位置
			uCenters.Add(new CGPoint(SP_LV6_BG_Gauge.RB_GAUGE_CENTER_X(1.0f)
									 , SP_LV6_BG_Gauge.RB_GAUGE_CENTER_Y(1.0f, (offset * 1.2f) + GAUGE_VALUE_HEIGHT)));


			//title 1的位置
			tCenters.Add(new CGPoint(SP_LV6_BG_Gauge.LT_GAUGE_CENTER_X(1.0f)
                                     , SP_LV6_BG_Gauge.LT_GAUGE_CENTER_Y(1.0f, (offset / 2.0f )- GAUGE_HEIGHT / 1.85f)));
			//title 2的位置
			tCenters.Add(new CGPoint(SP_LV6_BG_Gauge.RT_GAUGE_CENTER_X(1.0f)
									 , SP_LV6_BG_Gauge.RT_GAUGE_CENTER_Y(1.0f, (offset / 2.0f) - GAUGE_HEIGHT / 1.85f)));
			//title 3的位置
			tCenters.Add(new CGPoint(SP_LV6_BG_Gauge.LC_GAUGE_CENTER_X(1.0f)
									 , SP_LV6_BG_Gauge.LC_GAUGE_CENTER_Y(1.0f, (offset / 2.4f) + GAUGE_HEIGHT / 1.85f)));
			//title 4的位置
			tCenters.Add(new CGPoint(SP_LV6_BG_Gauge.RC_GAUGE_CENTER_X(1.0f)
									 , SP_LV6_BG_Gauge.RC_GAUGE_CENTER_Y(1.0f, (offset / 2.4f) + GAUGE_HEIGHT / 1.85f)));
			//title 5的位置
			tCenters.Add(new CGPoint(SP_LV6_BG_Gauge.LB_GAUGE_CENTER_X(1.0f)
									 , SP_LV6_BG_Gauge.LB_GAUGE_CENTER_Y(1.0f, (offset * 1.2f) + GAUGE_HEIGHT / 1.85f)));
			//title 6的位置
			tCenters.Add(new CGPoint(SP_LV6_BG_Gauge.RB_GAUGE_CENTER_X(1.0f)
									 , SP_LV6_BG_Gauge.RB_GAUGE_CENTER_Y(1.0f, (offset * 1.2f) + GAUGE_HEIGHT / 1.85f)));
		}

		private void AddTextForGauge()
		{
			SetPosition();

			//加入gauge text , unit , title   
			for (int i = 0; i < liveData.Count - 0; i++)
			{
				var gValue = new UILabel(new CGRect(0, 0
													, SP_LV6_BG_Gauge.LT_GAUGE_SIZE(1.0f)
													, SP_LV6_BG_Gauge.LT_GAUGE_SIZE(1.0f) / 6.0f))
				{
					Text = AppAttribute.APP_RUNNING_MODE == AppAttribute.RunningMode.DoUITest ? "9999" : "",
					TextColor = vColorRGB,
                    Font = UIFont.BoldSystemFontOfSize(VALUE_TEXT_SIZE),
					TextAlignment = UITextAlignment.Center,
					BackgroundColor = UIColor.Clear,
					Center = gCenters[i]
				};

				var gUnit = new UILabel(new CGRect(0, 0
													, SP_LV6_BG_Gauge.LT_GAUGE_SIZE(0.9f)
													, SP_LV6_BG_Gauge.LT_GAUGE_SIZE(0.9f) / 8.67f))
				{
					//Text = "km/h",
					Text = AppAttribute.APP_RUNNING_MODE == AppAttribute.RunningMode.DoUITest ? "km/h" : "",
					TextColor = uColorRGB,
                    Font = UIFont.ItalicSystemFontOfSize(UNIT_TEXT_SIZE),
					TextAlignment = UITextAlignment.Center,
					BackgroundColor = UIColor.Clear,
					Center = uCenters[i]
				};

				var gTitle = new UILabel(new CGRect(0, 0
													, SP_LV4_BG_Gauge.LT_FRAME_SIZE(1.0f)
													 , SP_LV4_BG_Gauge.LT_GAUGE_SIZE(1.0f) / 6.0f))
				{
					//Text = "Fuel Pressure",
					Text = AppAttribute.APP_RUNNING_MODE == AppAttribute.RunningMode.DoUITest ? "Fuel Pressure" : "",
					TextColor = tColorRGB,
                    Font = UIFont.ItalicSystemFontOfSize(TITLE_TEXT_SIZE),
					TextAlignment = UITextAlignment.Center,
					BackgroundColor = UIColor.Clear,
					Center = tCenters[i]
				};

				
				this.View.AddSubview(gTitle);
				this.View.AddSubview(gUnit);
				this.View.AddSubview(gValue);
				gTitles.Add(gTitle);
				gUnits.Add(gUnit);
				gValues.Add(gValue);

			}



		}

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();


        }

		private void SetFanDrawingKey(List<int> keys)
		{
			fan_1.SetKeyForDrawingWithValue(keys[0]);
			fan_2.SetKeyForDrawingWithValue(keys[1]);
			fan_3.SetKeyForDrawingWithValue(keys[2]);
			fan_4.SetKeyForDrawingWithValue(keys[3]);
            fan_5.SetKeyForDrawingWithValue(keys[4]);
            fan_6.SetKeyForDrawingWithValue(keys[5]);
		}

		private void UpdateGaugesByUserSelected()
		{
			if (StateMachine.IsActivted)
			{
				//洗掉舊的MAXIMUM
				if (MAXVALUE.Count > 0) MAXVALUE.Clear();
				//洗掉舊的UNIT
				if (UNITS.Count > 0) UNITS.Clear();
				//洗掉舊的TITLES
				if (TITLES.Count > 0) TITLES.Clear();


				//拿dmData
				var dmDatas = StateMachine.Instance.GetDmValues();

				//更新MAXIMUM、UNIT、TITLES
				for (int i = 0; i < liveData.Count; i++)
				{
					//預防錯誤
					try
					{
						if (dmDatas.ContainsKey(liveData[i]))
						{
                            MAXVALUE.Add((float)((dmDatas[liveData[i]].MaxValue - dmDatas[liveData[i]].MinValue) * dmDatas[liveData[i]].MultipleRate));
							//MAXVALUE.Add((float)(dmDatas[liveData[i]].MaxValue * dmDatas[liveData[i]].MultipleRate));
                            UNITS.Add(dmDatas[liveData[i]].Unit);
                            //UNITS.Add(dmDatas[liveData[i]].ChartDisplayText);
                            if(!dmDatas[liveData[i]].ShortName.Equals(""))
							    TITLES.Add(dmDatas[liveData[i]].ShortName);
                            else
                                TITLES.Add(dmDatas[liveData[i]].Name);
						}
						else
						{
							MAXVALUE.Add(0.0f);
							UNITS.Add("");
							TITLES.Add("");

						}
					}
					catch (KeyNotFoundException ex)
					{
						Console.WriteLine("ex ----- " + ex);
					}
				}

				//更新上面的字
				RefreshTextOnGauge();
				//RefresgGaugeBG();
				//
				//AssignFanToDraw();
			}

		}

        private void InitGraphicValue(){
            
        }

		private void AddFan(){


			//扇形(左上)
			fan_1 = new GraphicFan(
                 SP_LV6.LT_FAN_CENTER_X(1.0f)
                ,SP_LV6.LT_FAN_CENTER_Y(1.0f, offset / 2)
                ,SP_LV6.LT_FAN_RADIUS(1.1f)
                ,SP_LV6.LT_FAN_STARTANGLE(Math.PI / 2)
                ,SP_LV6.LT_FAN_ENDANGLE(Math.PI * 2)
                ,SP_LV6.LT_FAN_CLOCKWISE(true));
            
			this.View.Layer.AddSublayer(fan_1);

			//扇形(右上)
			fan_2 = new GraphicFan(
                 SP_LV6.RT_FAN_CENTER_X(1.0f)
                ,SP_LV6.RT_FAN_CENTER_Y(1.0f, offset / 2)
                ,SP_LV6.RT_FAN_RADIUS(1.1f)											  
                ,SP_LV6.RT_FAN_STARTANGLE(Math.PI / 2)
                ,SP_LV6.RT_FAN_ENDANGLE(Math.PI * 2)											  
                ,SP_LV6.RT_FAN_CLOCKWISE(true));
            
			this.View.Layer.AddSublayer(fan_2);

			//扇形(左中)
			fan_3 = new GraphicFan(
                 SP_LV6.LC_FAN_CENTER_X(1.0f)
                ,SP_LV6.LC_FAN_CENTER_Y(1.0f, offset / 2.4f)
				,SP_LV6.LC_FAN_RADIUS(1.1f)
				,SP_LV6.LC_FAN_STARTANGLE(Math.PI / 2)
				,SP_LV6.LC_FAN_ENDANGLE(Math.PI * 2)
				,SP_LV6.LC_FAN_CLOCKWISE(true));
            
			this.View.Layer.AddSublayer(fan_3);

			//扇形(右中)
			fan_4 = new GraphicFan(
			      SP_LV6.RC_FAN_CENTER_X(1.0f)
				, SP_LV6.RC_FAN_CENTER_Y(1.0f, offset / 2.4f)
				, SP_LV6.RC_FAN_RADIUS(1.1f)
				, SP_LV6.RC_FAN_STARTANGLE(Math.PI / 2)
				, SP_LV6.RC_FAN_ENDANGLE(Math.PI * 2)
				, SP_LV6.RC_FAN_CLOCKWISE(true));
			this.View.Layer.AddSublayer(fan_4);

			//扇形(左下)
			fan_5 = new GraphicFan(
				  SP_LV6.LB_FAN_CENTER_X(1.0f)
                , SP_LV6.LB_FAN_CENTER_Y(1.0f, offset * 1.2f)
                , SP_LV6.LB_FAN_RADIUS(1.1f)
                , SP_LV6.LB_FAN_STARTANGLE(Math.PI / 2)
                , SP_LV6.LB_FAN_ENDANGLE(Math.PI * 2)
                , SP_LV6.LB_FAN_CLOCKWISE(true));
			this.View.Layer.AddSublayer(fan_5);

			//扇形(右下)
			fan_6 = new GraphicFan(
				  SP_LV6.RB_FAN_CENTER_X(1.0f)
				, SP_LV6.RB_FAN_CENTER_Y(1.0f, offset * 1.2f)
				, SP_LV6.RB_FAN_RADIUS(1.1f)
				, SP_LV6.RB_FAN_STARTANGLE(Math.PI / 2)
				, SP_LV6.RB_FAN_ENDANGLE(Math.PI * 2)
				, SP_LV6.RB_FAN_CLOCKWISE(true));
			this.View.Layer.AddSublayer(fan_6);
		}



        private void AddBG(List<string> gaugesBG){
            //Gauge圖(左上)
            graphicGaugeView_1 = new GraphicGaugeView().CreateBG_Gauge(
                  SP_LV6_BG_Gauge.LT_GAUGE_IMAGE(gaugesBG[0])
                , SP_LV6_BG_Gauge.LT_GAUGE_SIZE(1.0f)
                , SP_LV6_BG_Gauge.LT_GAUGE_CENTER_X(1.0f)
                , SP_LV6_BG_Gauge.LT_GAUGE_CENTER_Y(1.0f, offset / 2.0f));
            this.View.Layer.AddSublayer(graphicGaugeView_1);

			//框框(左上)
            this.View.Layer.AddSublayer(new GraphicGaugeView().CreateBG_Frame(
				  SP_LV6_BG_Gauge.LT_FRAME_IMAGE("sp_frame.png")
				, SP_LV6_BG_Gauge.LT_FRAME_SIZE(1.0f)
				, SP_LV6_BG_Gauge.LT_FRAME_CENTER_X(1.0f)
                , SP_LV6_BG_Gauge.LT_FRAME_CENTER_Y(1.0f, (offset * 1.5f) / 7)));


            //Gauge圖(右上)
            graphicGaugeView_2 = new GraphicGaugeView().CreateBG_Gauge(
                  SP_LV6_BG_Gauge.RT_GAUGE_IMAGE(gaugesBG[1])
                , SP_LV6_BG_Gauge.RT_GAUGE_SIZE(1.0f)
                , SP_LV6_BG_Gauge.RT_GAUGE_CENTER_X(1.0f)
                , SP_LV6_BG_Gauge.RT_GAUGE_CENTER_Y(1.0f, offset / 2.0f));
			this.View.Layer.AddSublayer(graphicGaugeView_2);
			//框框(右上)
            this.View.Layer.AddSublayer(new GraphicGaugeView().CreateBG_Frame(
				  SP_LV6_BG_Gauge.RT_FRAME_IMAGE("sp_frame.png")
				, SP_LV6_BG_Gauge.RT_FRAME_SIZE(1.0f)
				, SP_LV6_BG_Gauge.RT_FRAME_CENTER_X(1.0f)
				, SP_LV6_BG_Gauge.RT_FRAME_CENTER_Y(1.0f, (offset * 1.5f) / 7)));


            //Gauge圖(左中)
            graphicGaugeView_3 = new GraphicGaugeView().CreateBG_Gauge(
                  SP_LV6_BG_Gauge.LC_GAUGE_IMAGE(gaugesBG[2])
                , SP_LV6_BG_Gauge.LC_GAUGE_SIZE(1.0f)
                , SP_LV6_BG_Gauge.LC_GAUGE_CENTER_X(1.0f)
                , SP_LV6_BG_Gauge.LC_GAUGE_CENTER_Y(1.0f, offset / 2.4f));
			this.View.Layer.AddSublayer(graphicGaugeView_3);
			//框框(左中)
            this.View.Layer.AddSublayer(new GraphicGaugeView().CreateBG_Frame(
				  SP_LV6_BG_Gauge.LC_FRAME_IMAGE("sp_frame.png")
				, SP_LV6_BG_Gauge.LC_FRAME_SIZE(1.0f)
				, SP_LV6_BG_Gauge.LC_FRAME_CENTER_X(1.0f)
                , SP_LV6_BG_Gauge.LC_FRAME_CENTER_Y(1.0f, offset / 1.5f)));


            //Gauge圖(右中)
            graphicGaugeView_4 = new GraphicGaugeView().CreateBG_Gauge(
                  SP_LV6_BG_Gauge.RC_GAUGE_IMAGE(gaugesBG[3])
                , SP_LV6_BG_Gauge.RC_GAUGE_SIZE(1.0f)
                , SP_LV6_BG_Gauge.RC_GAUGE_CENTER_X(1.0f)
                , SP_LV6_BG_Gauge.RC_GAUGE_CENTER_Y(1.0f, offset / 2.4f));
			this.View.Layer.AddSublayer(graphicGaugeView_4);
			//框框(右中)
            this.View.Layer.AddSublayer(new GraphicGaugeView().CreateBG_Frame(
                  SP_LV6_BG_Gauge.RC_FRAME_IMAGE("sp_frame.png")
				, SP_LV6_BG_Gauge.RC_FRAME_SIZE(1.0f)
				, SP_LV6_BG_Gauge.RC_FRAME_CENTER_X(1.0f)
				, SP_LV6_BG_Gauge.RC_FRAME_CENTER_Y(1.0f, offset / 1.5f)));


            //Gauge圖(左下)
            graphicGaugeView_5 = new GraphicGaugeView().CreateBG_Gauge(
                  SP_LV6_BG_Gauge.LB_GAUGE_IMAGE(gaugesBG[4])
                , SP_LV6_BG_Gauge.LB_GAUGE_SIZE(1.0f)
                , SP_LV6_BG_Gauge.LB_GAUGE_CENTER_X(1.0f)
                , SP_LV6_BG_Gauge.LB_GAUGE_CENTER_Y(1.0f, offset * 1.2f));
			this.View.Layer.AddSublayer(graphicGaugeView_5);
			//框框(左下)
            this.View.Layer.AddSublayer(new GraphicGaugeView().CreateBG_Frame(
                  SP_LV6_BG_Gauge.LB_FRAME_IMAGE("sp_frame.png")
                , SP_LV6_BG_Gauge.LB_FRAME_SIZE(1.0f)
                , SP_LV6_BG_Gauge.LB_FRAME_CENTER_X(1.0f)
                , SP_LV6_BG_Gauge.LB_FRAME_CENTER_Y(1.0f, offset * 1.5f)));

            //Gauge圖(右下)
            graphicGaugeView_6 = new GraphicGaugeView().CreateBG_Gauge(
                  SP_LV6_BG_Gauge.RB_GAUGE_IMAGE(gaugesBG[5])
                , SP_LV6_BG_Gauge.RB_GAUGE_SIZE(1.0f)
                , SP_LV6_BG_Gauge.RB_GAUGE_CENTER_X(1.0f)
                , SP_LV6_BG_Gauge.RB_GAUGE_CENTER_Y(1.0f, offset * 1.2f));
			this.View.Layer.AddSublayer(graphicGaugeView_6);
			//框框(右下)
            this.View.Layer.AddSublayer(new GraphicGaugeView().CreateBG_Frame(
                  SP_LV6_BG_Gauge.RB_FRAME_IMAGE("sp_frame.png")
				, SP_LV6_BG_Gauge.RB_FRAME_SIZE(1.0f)
				, SP_LV6_BG_Gauge.RB_FRAME_CENTER_X(1.0f)
				, SP_LV6_BG_Gauge.RB_FRAME_CENTER_Y(1.0f, offset * 1.5f)));
        }

		public void SetLiveDataID(List<int> liveData)
		{
			this.liveData = liveData;
		}

		public void SetLiveDataIconImage(List<string> images)
		{
			this.lvGaugesBG = images;
		}

		public void UpdateGaugeBG()
		{
			DispatchQueue.MainQueue.DispatchAsync(() =>
			{
				graphicGaugeView_1.Contents = UIImage.FromFile(lvGaugesBG[0]).CGImage;
				graphicGaugeView_2.Contents = UIImage.FromFile(lvGaugesBG[1]).CGImage;
				graphicGaugeView_3.Contents = UIImage.FromFile(lvGaugesBG[2]).CGImage;
				graphicGaugeView_4.Contents = UIImage.FromFile(lvGaugesBG[3]).CGImage;
				graphicGaugeView_5.Contents = UIImage.FromFile(lvGaugesBG[4]).CGImage;
				graphicGaugeView_6.Contents = UIImage.FromFile(lvGaugesBG[5]).CGImage;

			});
			
		}

		void BtnBack_TouchUpInside(object sender, EventArgs e)
		{
			//if (StateMachine.IsActivted)
				//StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV_STOP);

			ContainerViewController.Instance.PerformSegue(MyCustomPages.buttonRespondSegueID["btnLiveDataIcon"], null);
		}

		/** UI 更新數值 */
		void LiveData_6_Frame_ViewController_UpdateTimerValue()
        {
			var lvDatas = StateMachine.DataModel.LvValues;

			if (lvDatas.Count > 0)
			{
				try
				{

					fan_1.SetValue(lvDatas[fan_1.GetKey()] / MAXVALUE[0]); //弧線數值，需除以最大值
					fan_1.SetColorTransformValue(lvDatas[fan_1.GetKey()] / MAXVALUE[0]); //弧線顏色
                    gValues[0].Text = lvDatas[fan_1.GetKey()].ToString(fan_1.GetFormat(fan_1.GetKey()));
					//if (fan_1.GetKey() == 1)
						//gValues[0].Text = lvDatas[fan_1.GetKey()].ToString("0."); 
					//else
						//gValues[0].Text = lvDatas[fan_1.GetKey()].ToString("0.##");


					fan_2.SetValue(lvDatas[fan_2.GetKey()] / MAXVALUE[1]);
					fan_2.SetColorTransformValue(lvDatas[fan_2.GetKey()] / MAXVALUE[1]);
                    gValues[1].Text = lvDatas[fan_2.GetKey()].ToString(fan_2.GetFormat(fan_2.GetKey()));
					//if (fan_2.GetKey() == 1)
						//gValues[1].Text = lvDatas[fan_2.GetKey()].ToString("0.");
					//else
						//gValues[1].Text = lvDatas[fan_2.GetKey()].ToString("0.##");



					fan_3.SetValue(lvDatas[fan_3.GetKey()] / MAXVALUE[2]);
					fan_3.SetColorTransformValue(lvDatas[fan_3.GetKey()] / MAXVALUE[2]);
                    gValues[2].Text = lvDatas[fan_3.GetKey()].ToString(fan_3.GetFormat(fan_3.GetKey())); 
					//if (fan_3.GetKey() == 1)
						//gValues[2].Text = lvDatas[fan_3.GetKey()].ToString("0."); 
					//else
						//gValues[2].Text = lvDatas[fan_3.GetKey()].ToString("0.##");

					fan_4.SetValue(lvDatas[fan_4.GetKey()] / MAXVALUE[3]);
					fan_4.SetColorTransformValue(lvDatas[fan_4.GetKey()] / MAXVALUE[3]);
                    gValues[3].Text = lvDatas[fan_4.GetKey()].ToString(fan_4.GetFormat(fan_4.GetKey()));

					//if (fan_4.GetKey() == 1)
					//gValues[3].Text = lvDatas[fan_4.GetKey()].ToString("0."); 
					//else
					//gValues[3].Text = lvDatas[fan_4.GetKey()].ToString("0.##");

					fan_5.SetValue(lvDatas[fan_5.GetKey()] / MAXVALUE[4]);
					fan_5.SetColorTransformValue(lvDatas[fan_5.GetKey()] / MAXVALUE[4]);
                    gValues[4].Text = lvDatas[fan_5.GetKey()].ToString(fan_5.GetFormat(fan_5.GetKey()));
					//if (fan_5.GetKey() == 1)
						//gValues[4].Text = lvDatas[fan_5.GetKey()].ToString("0.");
					//else
						//gValues[4].Text = lvDatas[fan_5.GetKey()].ToString("0.##");

					fan_6.SetValue(lvDatas[fan_6.GetKey()] / MAXVALUE[5]);
					fan_6.SetColorTransformValue(lvDatas[fan_6.GetKey()] / MAXVALUE[5]);
                    gValues[5].Text = lvDatas[fan_6.GetKey()].ToString(fan_6.GetFormat(fan_6.GetKey()));
					//if (fan_6.GetKey() == 1)
						//gValues[5].Text = lvDatas[fan_6.GetKey()].ToString("0.");
					//else
						//gValues[5].Text = lvDatas[fan_6.GetKey()].ToString("0.##");

				}
				catch (Exception ex)
				{

				}

				
			}
        }

        public override void setCurrentPageName()
        {
			
        }

        protected LiveData_6_Frame_ViewController(IntPtr handle) : base(handle)
		{
		}
    }
}


//private void AddFan()
//{
//	//扇形(左上)
//	fan_1 = new GraphicFan(
//										(float)Main.SCREEN_SIZE.Width / 4
//									  , (float)(((Main.SCREEN_SIZE.Height * 1) / 4) - btnBack.Frame.Bottom / 2)
//									  , (float)(Main.SCREEN_SIZE.Width / 13.57)
//									  , (float)Math.PI / 2
//									  , (float)Math.PI * 2
//									  , true);
//	this.View.Layer.AddSublayer(fan_1);

			//	//扇形(右上)
			//	fan_2 = new GraphicFan(
			//										(float)((Main.SCREEN_SIZE.Width * 3) / 4)
			//									  , (float)(((Main.SCREEN_SIZE.Height * 1) / 4) - btnBack.Frame.Bottom / 2)
			//									  , (float)(Main.SCREEN_SIZE.Width / 13.57)
			//									  , (float)Math.PI / 2
			//									  , (float)Math.PI * 2
			//									  , true);
			//	this.View.Layer.AddSublayer(fan_2);

			//	//扇形(左中)
			//	fan_3 = new GraphicFan(
			//									  (float)Main.SCREEN_SIZE.Width / 4
			//									  , (float)(((Main.SCREEN_SIZE.Height * 1) / 2) + btnBack.Frame.Bottom / 2.3)
			//									  , (float)(Main.SCREEN_SIZE.Width / 13.57)
			//									  , (float)Math.PI / 2
			//									  , (float)Math.PI * 2
			//									  , true);
			//	this.View.Layer.AddSublayer(fan_3);

			//	//扇形(右中)
			//	fan_4 = new GraphicFan(
			//										(float)((Main.SCREEN_SIZE.Width * 3) / 4)
			//									  , (float)(((Main.SCREEN_SIZE.Height * 1) / 2) + btnBack.Frame.Bottom / 2.4)
			//									  , (float)(Main.SCREEN_SIZE.Width / 13.57)
			//									  , (float)Math.PI / 2
			//									  , (float)Math.PI * 2
			//									  , true);
			//	this.View.Layer.AddSublayer(fan_4);

			//	//扇形(左下)
			//	fan_5 = new GraphicFan(
			//										(float)Main.SCREEN_SIZE.Width / 4
			//									  , (float)(((Main.SCREEN_SIZE.Height * 3) / 4) + btnBack.Frame.Bottom * 1.2)
			//									  , (float)(Main.SCREEN_SIZE.Width / 13.57)
			//									  , (float)Math.PI / 2
			//									  , (float)Math.PI * 2
			//									  , true);
			//	this.View.Layer.AddSublayer(fan_5);

			//	//扇形(右下)
			//	fan_6 = new GraphicFan(
			//										(float)(Main.SCREEN_SIZE.Width * 3) / 4
			//									  , (float)(((Main.SCREEN_SIZE.Height * 3) / 4) + btnBack.Frame.Bottom * 1.2)
			//									  , (float)(Main.SCREEN_SIZE.Width / 13.57)
			//									  , (float)Math.PI / 2
			//									  , (float)Math.PI * 2
			//									  , true);
			//	this.View.Layer.AddSublayer(fan_6);
			//}



			////Gauge圖(右上)
			//this.View.Layer.AddSublayer(graphicGaugeView.CreateBG_Gauge(
            //                                "sp_gauge_1.png"
            //                                , (float)(Main.SCREEN_SIZE.Width / 2.3)
            //                                , (float)((Main.SCREEN_SIZE.Width * 3) / 4)
            //                                , (float)(((Main.SCREEN_SIZE.Height * 1) / 4) - btnBack.Frame.Bottom / 2)));
            ////框框(右上)
            //this.View.Layer.AddSublayer(graphicGaugeView.CreateBG_Frame(
            //                                "sp_frame.png"
            //                                , (float)(Main.SCREEN_SIZE.Width / 2.18)
            //                                , (float)((Main.SCREEN_SIZE.Width * 3) / 4)
            //                                //, ((float)Main.SCREEN_SIZE.Width / 4) + ((float)Main.SCREEN_SIZE.Width / 2)
            //                                , (float)(((Main.SCREEN_SIZE.Height * 1) / 4) - (btnBack.Frame.Bottom * 1.5) / 7)));

            ////Gauge圖(左中)
            //this.View.Layer.AddSublayer(graphicGaugeView.CreateBG_Gauge(
            //                                "sp_gauge_1.png"
            //                                , (float)(Main.SCREEN_SIZE.Width / 2.3)
            //                                , (float)Main.SCREEN_SIZE.Width / 4
            //                                , (float)(((Main.SCREEN_SIZE.Height * 1) / 2) + btnBack.Frame.Bottom / 2.4)));
            ////框框(左中)
            //this.View.Layer.AddSublayer(graphicGaugeView.CreateBG_Frame(
            //                                "sp_frame.png"
            //                                , (float)(Main.SCREEN_SIZE.Width / 2.18)
            //                                , (float)Main.SCREEN_SIZE.Width / 4
            //                                , (float)(((Main.SCREEN_SIZE.Height * 1) / 2) + btnBack.Frame.Bottom / 1.5)));
            ////Gauge圖(右中)
            //this.View.Layer.AddSublayer(graphicGaugeView.CreateBG_Gauge(
            //                                "sp_gauge_1.png"
            //                                , (float)(Main.SCREEN_SIZE.Width / 2.3)
            //                                , (float)((Main.SCREEN_SIZE.Width * 3) / 4)
            //                                , (float)(((Main.SCREEN_SIZE.Height * 1) / 2) + btnBack.Frame.Bottom / 2.4)));
            ////框框(右中)
            //this.View.Layer.AddSublayer(graphicGaugeView.CreateBG_Frame(
            //                                "sp_frame.png"
            //                                , (float)(Main.SCREEN_SIZE.Width / 2.18)
            //                                , (float)((Main.SCREEN_SIZE.Width * 3) / 4)
            //                                , (float)(((Main.SCREEN_SIZE.Height * 1) / 2) + btnBack.Frame.Bottom / 1.5)));

            ////Gauge圖(左下)
            //this.View.Layer.AddSublayer(graphicGaugeView.CreateBG_Gauge(
            //                                "sp_gauge_1.png"
            //                                , (float)(Main.SCREEN_SIZE.Width / 2.3)
            //                                , (float)Main.SCREEN_SIZE.Width / 4
            //                                , (float)(((Main.SCREEN_SIZE.Height * 3) / 4) + btnBack.Frame.Bottom * 1.2)));

            ////框框(左下)
            //this.View.Layer.AddSublayer(graphicGaugeView.CreateBG_Frame(
            //                                "sp_frame.png"
            //                                , (float)(Main.SCREEN_SIZE.Width / 2.18)
            //                                , (float)Main.SCREEN_SIZE.Width / 4
            //                                , (float)(((Main.SCREEN_SIZE.Height * 3) / 4) + btnBack.Frame.Bottom * 1.5)));
            ////Gauge圖(右下)
            //this.View.Layer.AddSublayer(graphicGaugeView.CreateBG_Gauge(
            //                                "sp_gauge_1.png"
            //                                , (float)(Main.SCREEN_SIZE.Width / 2.3)
            //                                , (float)(Main.SCREEN_SIZE.Width * 3) / 4
            //                                , (float)(((Main.SCREEN_SIZE.Height * 3) / 4) + btnBack.Frame.Bottom * 1.2)));
            ////框框(右下)
            //this.View.Layer.AddSublayer(graphicGaugeView.CreateBG_Frame(
                                            //"sp_frame.png"
                                            //, (float)(Main.SCREEN_SIZE.Width / 2.18)
                                            //, (float)(Main.SCREEN_SIZE.Width * 3) / 4
                                            //, (float)(((Main.SCREEN_SIZE.Height * 3) / 4) + btnBack.Frame.Bottom * 1.5)));

﻿﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CoreAnimation;
using CoreFoundation;
using CoreGraphics;
using UIKit;
using Xamarin_SYM_IOS.SRC.UI;


//Created by Justin on 2017/6/30


namespace Xamarin_SYM_IOS.ViewControllers
{   
    
    public partial class LiveData_2_Frame_ViewController : CustomViewController
    {
        //gauge上數值的字體大小 
        private int VALUE_TEXT_SIZE;
		//gauge上單位的字體大小
		private int UNIT_TEXT_SIZE;
		//gauge上標題的字體大小
		private int TITLE_TEXT_SIZE;

        //gauge上數值的顏色
        private UIColor vColorRGB = UIColor.FromRGB(54,227,255);
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


        //多個Mininum，更新用
		private List<float> MINIVALUE = new List<float>();
        //多個Maximum，更新用
        private List<float> MAXVALUE = new List<float>();
		//多個標題，更新用
		private List<string> UNITS = new List<string>();
		//多個單位，更新用
		private List<string> TITLES = new List<string>();


        //DataModel的datas，會不斷更新
        //private List<float> datas;

        //由icon頁帶過來的liveData id
        public List<int> liveData;
		//由icon頁帶過來的gauge bg
		private List<string> lvGaugesBG;

        //微調gauge背景圖用
        private float offset;

      
        //gauge & frame圖
        private CALayer graphicGaugeView_1;
        private CALayer graphicGaugeView_2;

        //扇形1
        private GraphicFan fan_1;
		//扇形2
		private GraphicFan fan_2;



       
        //private CAShapeLayer fanlayer = new CAShapeLayer();
        private UISlider slider;

		

		public LiveData_2_Frame_ViewController Instance;

		public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Console.WriteLine("LiveData_2_Frame_ViewController : " + "ViewDidLoad()");
			//給unit & title 調整位置用
			GAUGE_VALUE_HEIGHT = (float)(Main.SCREEN_SIZE.Width / 1.59f) / 6.0f;
            GAUGE_HEIGHT = SP_LV2_BG_Gauge.TOP_GAUGE_SIZE(1.0f);
			//全屏調整位置的差值
			offset = (float)btnBack.Frame.Bottom; //52.6

            VALUE_TEXT_SIZE = (int)(offset / 1.09f);
            UNIT_TEXT_SIZE  = (int)(offset / 2.5f);
            TITLE_TEXT_SIZE =  (int)(offset / 1.54f);
            //InitTextForGauge();



            this.View.BackgroundColor = UIColor.Black;

            Instance = this;
            IsInited = true;
            btn_Up.Hidden = true;
            btnBack.TouchUpInside += BtnBack_TouchUpInside;
            UpdateTimerValue += LiveData_2_Frame_ViewController_UpdateTimerValue;

			//btn_Up.SetTitle("0",UIControlState.Normal);
			//Console.WriteLine("btnBack Bottom : " + btnBack.Frame.Bottom);


            //加入扇形、背景圖、文字
			AddFan();
            AddBG(lvGaugesBG);
            AddTextForGauge();


            //foreach (var font in UIFont.FamilyNames){
            //    Console.WriteLine("fonts : " + font.ToString());
            //}
            //nfloat red = 255;
            //nfloat green = 255;
            //nfloat blue= 255;
            //nfloat alpha= 255;
            //vColorRGB.GetRGBA(out red,out green,out blue,out alpha);
            //var CGColor = vColorRGB.CGColor.Components;

        }   

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            //update UI
            SetFanDrawingKey(LiveDataIconViewController.Instance.SendLV);
            UpdateGaugesByUserSelected();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            Console.WriteLine("LiveData_2_Frame_ViewController : " + "ViewDidAppear()");
            isViewDidAppear = true;


            //update UI
            RefreshTextOnGauge();

            //測試資料
            //         for (int i = 0; i < lvGaugesBG.Count; i++)
            //         {
            //             Console.WriteLine(" name : " + lvGaugesBG[i]);
            //}
            //SetFanDrawingKey(LiveDataIconViewController.Instance.ChiocedLV);
            //UpdateGaugesByUserSelected();


            //foreach (float val in MAXVALUE){
            //    Console.WriteLine(" float : " + val);
            //}

            //AddSlider();

            //test UI
            //if (isUIMode)
            //{
            //    DispatchQueue.DefaultGlobalQueue.DispatchAsync(() =>
            //    {
            //        while (true)
            //        {
            //            DispatchQueue.MainQueue.DispatchAsync(() =>
            //              {
            //              //btn_Up.TitleLabel.Text = "" + num;
            //              btn_Up.SetTitle("" + num, UIControlState.Normal);

            //              });

            //            num++;
            //            Thread.Sleep(100);
            //        }
            //    });
            //}
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            Console.WriteLine("LiveData_2_Frame_ViewController : " + "ViewDidDisappear()");

            //if (StateMachine.IsActivted)
                //StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV_STOP);

            isViewDidAppear = false;

        }

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}


		//=============================================================
		//================       FUNCTIONS         ====================
		//=============================================================


        private void SetFanDrawingKey(List<int> keys){
			

            fan_1.SetKeyForDrawingWithValue(keys[0]);
            fan_2.SetKeyForDrawingWithValue(keys[1]);
        }

		private void UpdateGaugesByUserSelected()
		{
            if (StateMachine.IsActivted)
			{
			    //洗掉舊的MAXIMUM
			    if (MAXVALUE.Count > 0) MAXVALUE.Clear();
			    //洗掉舊的UNIT
                if (UNITS.Count > 0)    UNITS.Clear();
			    //洗掉舊的TITLES
                if (TITLES.Count > 0)   TITLES.Clear();


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
							//公式 : (最大 - 最小) / MultipleRate
							MAXVALUE.Add((float)((dmDatas[liveData[i]].MaxValue - dmDatas[liveData[i]].MinValue) * dmDatas[liveData[i]].MultipleRate));

                            UNITS.Add(dmDatas[liveData[i]].Unit);
							
							if (!dmDatas[liveData[i]].ShortName.Equals(""))
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
                //RefreshTextOnGauge();
                //RefresgGaugeBG();
                //
                //AssignFanToDraw();
            }
			
		}

        //private void RefresgGaugeBG(){
        //    graphicGaugeView_1.Contents = UIImage.FromFile(lvGaugesBG[0]).CGImage;
        //    graphicGaugeView_2.Contents = UIImage.FromFile(lvGaugesBG[1]).CGImage;
        //}

		private void RefreshTextOnGauge(){
            for (int i = 0; i < liveData.Count; i++){
                try
                {
					//DispatchQueue.MainQueue.DispatchAsync(() =>
				    //{
						gUnits[i].Text = UNITS[i];
						
						gTitles[i].Text = TITLES[i];
				    //});
                    //gUnits[i].Text = UNITS[i];
                    ///gUnits[i].Text = UNITS[i];
                    //gTitles[i].Text = TITLES[i];
                    //gTitles[i].Text = TITLES[i];
				}
                catch (ArgumentOutOfRangeException ex)
				{
					Console.WriteLine("ex ----- " + ex);
				}


            }
        }

        private void SetPosition(){
            //text 1的位置
            gCenters.Add(new CGPoint(SP_LV2_BG_Gauge.TOP_GAUGE_CENTER_X(1.0f)
                                     ,SP_LV2_BG_Gauge.TOP_GAUGE_CENTER_Y(1.0f, 0.0f)));
			//text 2的位置
			gCenters.Add(new CGPoint(SP_LV2_BG_Gauge.BOTTOM_GAUGE_CENTER_X(1.0f)
                                     ,SP_LV2_BG_Gauge.BOTTOM_GAUGE_CENTER_Y(1.0f, offset / 2.0f)));

            //unit 1的位置
            uCenters.Add(new CGPoint(SP_LV2_BG_Gauge.TOP_GAUGE_CENTER_X(1.0f)
                                     ,SP_LV2_BG_Gauge.TOP_GAUGE_CENTER_Y(1.0f, -GAUGE_VALUE_HEIGHT)));
			//unit 2的位置
			uCenters.Add(new CGPoint(SP_LV2_BG_Gauge.BOTTOM_GAUGE_CENTER_X(1.0f)
                                     , SP_LV2_BG_Gauge.BOTTOM_GAUGE_CENTER_Y(1.0f, (offset / 2.0f)- GAUGE_VALUE_HEIGHT)));

			//title 1的位置
			tCenters.Add(new CGPoint(SP_LV2_BG_Gauge.TOP_GAUGE_CENTER_X(1.0f)
                                     , SP_LV2_BG_Gauge.TOP_GAUGE_CENTER_Y(1.0f, -GAUGE_HEIGHT / 1.8f)));
			//title 2的位置
			tCenters.Add(new CGPoint(SP_LV2_BG_Gauge.BOTTOM_GAUGE_CENTER_X(1.0f)
                                     , SP_LV2_BG_Gauge.BOTTOM_GAUGE_CENTER_Y(1.0f, (offset / 2.0f) - GAUGE_HEIGHT / 1.8f)));
		}

        private void AddTextForGauge(){
            //設置text、unit、title的位置
            SetPosition();


			//加入gauge text , unit , title   
			for (int i = 0; i < liveData.Count; i++){
                var gValue = new UILabel(new CGRect(0, 0
                                                     , SP_LV2_BG_Gauge.TOP_GAUGE_SIZE(1.0f)
                                                     , SP_LV2_BG_Gauge.TOP_GAUGE_SIZE(1.0f) / 6.0f))
                {
                    //if(AppAttribute.APP_RUNNING_MODE == AppAttribute.RunningMode.DoUITest)
                    Text = AppAttribute.APP_RUNNING_MODE == AppAttribute.RunningMode.DoUITest ? "9999" : "",
                    TextColor = vColorRGB,
                    Font = UIFont.BoldSystemFontOfSize(VALUE_TEXT_SIZE), //48
                    TextAlignment = UITextAlignment.Center,
                    BackgroundColor = UIColor.Clear,
                    Center = gCenters[i]
                };

                var gUnit = new UILabel(new CGRect(0, 0
                                                    , SP_LV2_BG_Gauge.TOP_GAUGE_SIZE(0.9f)
                                                    , SP_LV2_BG_Gauge.TOP_GAUGE_SIZE(0.9f) / 6.0f))
                {
                    //Text = "km/h",
                    Text = AppAttribute.APP_RUNNING_MODE == AppAttribute.RunningMode.DoUITest ? "km/h" : "",
                    TextColor = uColorRGB,
                    Font = UIFont.ItalicSystemFontOfSize(UNIT_TEXT_SIZE),//21
                    TextAlignment = UITextAlignment.Center,
                    BackgroundColor = UIColor.Clear,
                    Center = uCenters[i]
                };

                var gTitle = new UILabel(new CGRect(0, 0
                                                    , SP_LV2_BG_Gauge.BOTTOM_FRAME_SIZE(1.0f)
                                                     , SP_LV2_BG_Gauge.TOP_GAUGE_SIZE(1.0f) / 6.0f))
                {
					//Text = "Fuel Pressure",
                    Text = AppAttribute.APP_RUNNING_MODE == AppAttribute.RunningMode.DoUITest ? "Fuel Pressure" : "",
                    TextColor = tColorRGB,
                    Font = UIFont.ItalicSystemFontOfSize(TITLE_TEXT_SIZE),//34
					TextAlignment = UITextAlignment.Center,
					BackgroundColor = UIColor.Clear,
                    Center = tCenters[i]
                };

                //Console.WriteLine("gValue - height : " + gValue.Frame.Size.Height);
                this.View.AddSubview(gTitle);
                this.View.AddSubview(gUnit);
                this.View.AddSubview(gValue);
                gTitles.Add(gTitle);
                gUnits.Add(gUnit);
                gValues.Add(gValue);

			}

			
   //         gValue.Frame = new CGRect(0, 0, (float)(Main.SCREEN_SIZE.Width / 1.59), 
   //                                          (float)(Main.SCREEN_SIZE.Width / 1.59)/6);
   //         gValue.Text = "9999";
   //         gValue.TextColor = UIColor.FromRGB(39, 177, 255);
			//gValue.Font = UIFont.BoldSystemFontOfSize(48);
			//gValue.TextAlignment = UITextAlignment.Center;
			//gValue.BackgroundColor = UIColor.White;
            //gValue.Center = new CGPoint( SP_LV2_BG_Gauge.TOP_GAUGE_CENTER_X(1.0f)
                                         //,SP_LV2_BG_Gauge.TOP_GAUGE_CENTER_Y(1.0f, 0.0f));

            //this.View.AddSubview(gValue);
		}

        private void AddFan(){
            //offset = (float)btnBack.Frame.Bottom;
            //扇形(上)
            fan_1 = new GraphicFan(
                  SP_LV2.TOP_FAN_CENTER_X(1.0f)
                , SP_LV2.TOP_FAN_CENTER_Y(1.0f, 0.0f)
                , SP_LV2.TOP_FAN_RADIUS(1.1f)
                , SP_LV2.TOP_FAN_STARTANGLE(Math.PI / 2)
                , SP_LV2.TOP_FAN_ENDANGLE(Math.PI * 2.3)
                , SP_LV2.TOP_FAN_CLOCKWISE(true));

            //AddGlow(fan_1);
			this.View.Layer.AddSublayer(fan_1);


			//扇形(下)
			fan_2 = new GraphicFan(
				  SP_LV2.BOTTOM_FAN_CENTER_X(1.0f)
                //, SP_LV2.BOTTOM_FAN_CENTER_Y(1.0f, 0.0f)
                , SP_LV2.BOTTOM_FAN_CENTER_Y(1.0f, offset / 2)
				, SP_LV2.BOTTOM_FAN_RADIUS(1.1f)
				, SP_LV2.BOTTOM_FAN_STARTANGLE(Math.PI / 2)
				, SP_LV2.BOTTOM_FAN_ENDANGLE(Math.PI * 2.3)
				, SP_LV2.BOTTOM_FAN_CLOCKWISE(true));

            //AddGlow(fan_2);
			this.View.Layer.AddSublayer(fan_2);
        }

        private void AddBG(List<string> gaugesBG){
            //Gauge圖(上)
            graphicGaugeView_1 = new GraphicGaugeView().CreateBG_Gauge(
                  SP_LV2_BG_Gauge.TOP_GAUGE_IMAGE(gaugesBG[0])
                , SP_LV2_BG_Gauge.TOP_GAUGE_SIZE(1.0f)
                , SP_LV2_BG_Gauge.TOP_GAUGE_CENTER_X(1.0f)
                , SP_LV2_BG_Gauge.TOP_GAUGE_CENTER_Y(1.0f, 0.0f));
            
            this.View.Layer.AddSublayer(graphicGaugeView_1);
											
										
			//框框(上)
            this.View.Layer.AddSublayer(new GraphicGaugeView().CreateBG_Frame(
                  SP_LV2_BG_Gauge.TOP_FRAME_IMAGE("sp_frame.png")
				, SP_LV2_BG_Gauge.TOP_FRAME_SIZE(1.0f)
				, SP_LV2_BG_Gauge.TOP_FRAME_CENTER_X(1.0f)
                , SP_LV2_BG_Gauge.TOP_FRAME_CENTER_Y(1.0f, offset / 2.0f)));


            //Gauge圖(下)
            graphicGaugeView_2 = new GraphicGaugeView().CreateBG_Gauge(
                  SP_LV2_BG_Gauge.BOTTOM_GAUGE_IMAGE(gaugesBG[1])
                , SP_LV2_BG_Gauge.BOTTOM_GAUGE_SIZE(1.0f)
                , SP_LV2_BG_Gauge.BOTTOM_GAUGE_CENTER_X(1.0f)
                , SP_LV2_BG_Gauge.BOTTOM_GAUGE_CENTER_Y(1.0f, offset / 2));
			
            this.View.Layer.AddSublayer(graphicGaugeView_2);
			


			////框框(下)
            this.View.Layer.AddSublayer(new GraphicGaugeView().CreateBG_Frame(
				  SP_LV2_BG_Gauge.BOTTOM_FRAME_IMAGE("sp_frame.png")
				, SP_LV2_BG_Gauge.BOTTOM_FRAME_SIZE(1.0f)
				, SP_LV2_BG_Gauge.BOTTOM_FRAME_CENTER_X(1.0f)
				, SP_LV2_BG_Gauge.BOTTOM_FRAME_CENTER_Y(1.0f, 0.0f)));
			
		}

        private void AddSlider(){
            slider = new UISlider(new CGRect(100,700,150,30));
            slider.MinValue = 0f;
            //slider.MaxValue = MAX_VALUE;
            slider.Value = 0;
            //slider.ValueChanged += Slider_ValueChanged;

            this.View.AddSubview(slider);
        }


        private void AddGlow(CAShapeLayer layer){
            layer.ShadowRadius = 30.0f;
			layer.ShadowOffset = new CGSize(0.0, 0.0);
            layer.ShadowColor = UIColor.White.CGColor;
			layer.ShadowOpacity = 1.0f;
        }

  

        void BtnBack_TouchUpInside(object sender, EventArgs e)
        {
			//if (StateMachine.IsActivted)
				//StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV_STOP);

            ContainerViewController.Instance.PerformSegue(MyCustomPages.buttonRespondSegueID["btnLiveDataIcon"],null);
        }

        public void SetLiveDataID(List<int> liveData){
            this.liveData = liveData;    
        }

        public void SetLiveDataIconImage(List<string> images)
		{
			this.lvGaugesBG = images;
		}


        public override void setCurrentPageName()
        {
            
        }

        public void UpdateGaugeBG(){
			DispatchQueue.MainQueue.DispatchAsync(() =>
			{
				graphicGaugeView_1.Contents = UIImage.FromFile(lvGaugesBG[0]).CGImage;
				graphicGaugeView_2.Contents = UIImage.FromFile(lvGaugesBG[1]).CGImage;

			});
        }

        private void SetDynamicTextColor(){
            
        }

        /** UI 更新數值 */
        void LiveData_2_Frame_ViewController_UpdateTimerValue()
        {
            //這裡拿到應該要有兩個
            var lvDatas = StateMachine.DataModel.LvValues;
            //var lvDatas = StateMachine.DataModel.DmIcmDBHelper.DmValues;
            //var d = lvDatas[1];
            //d.NumberOfDecimals
            //datas = lvDatas.Values.ToList();
            //datas = StateMachine.DataModel.LvValues.Values.ToList();
            if (lvDatas.Count > 0)
            {
                try
                {
                    if (isViewDidAppear)
                    {
                        fan_1.SetValue(lvDatas[fan_1.GetKey()] / MAXVALUE[0]); //弧線數值，需除以最大值
                        fan_1.SetColorTransformValue(lvDatas[fan_1.GetKey()] / MAXVALUE[0]); //弧線顏色
                        gValues[0].Text = lvDatas[fan_1.GetKey()].ToString(fan_1.GetFormat(fan_1.GetKey())); //數值

						//if(fan_1.GetKey() == 1)
                        //    gValues[0].Text = lvDatas[fan_1.GetKey()].ToString("0."); //數值
                        //else
                            //gValues[0].Text = lvDatas[fan_1.GetKey()].ToString("0.##");                                                                                       



						fan_2.SetValue(lvDatas[fan_2.GetKey()] / MAXVALUE[1]);
                        fan_2.SetColorTransformValue(lvDatas[fan_2.GetKey()] / MAXVALUE[1]);
                        gValues[1].Text = lvDatas[fan_2.GetKey()].ToString(fan_2.GetFormat(fan_2.GetKey()));
						//if (fan_2.GetKey() == 1)
						//	gValues[1].Text = lvDatas[fan_2.GetKey()].ToString("0.");
						//else
							//gValues[1].Text = lvDatas[fan_2.GetKey()].ToString("0.##");
                    }

                }
                catch (Exception ex)
                {

                }
            }

        }

        protected LiveData_2_Frame_ViewController(IntPtr handle) : base(handle)
		{
            
		}

       
    }
}




//      void Slider_ValueChanged(object sender, EventArgs e)
//      {
//          //float val = slider.Value / 100;

//          if (slider.Value < OFFSET_VALUE)
//          {
//              fanlayer.StrokeColor = UIColor.FromRGB(39, 177, 255).CGColor;
//              //fanlayer.ShadowColor = UIColor.FromRGB(39, 177, 255).CGColor;
//              vValue.TextColor = UIColor.FromRGB(39, 177, 255);
//          }
//          else if(slider.Value >=OFFSET_VALUE && slider.Value < MAX_VALUE - OFFSET_VALUE){
//      fanlayer.StrokeColor = UIColor.FromRGB(255, 255, 0).CGColor;
//              //fanlayer.ShadowColor = UIColor.FromRGB(255, 255, 0).CGColor;
//      vValue.TextColor = UIColor.FromRGB(255, 255, 0);
//          }
//          else
//          {
//              fanlayer.StrokeColor = UIColor.Red.CGColor;
//              //fanlayer.ShadowColor = UIColor.Red.CGColor;
//              vValue.TextColor = UIColor.Red;
//          }

//          fanlayer.StrokeEnd = slider.Value / MAX_VALUE;
//  DispatchQueue.MainQueue.DispatchAsync(() =>{
//              vValue.Text = ((int)slider.Value).ToString();
//  });
//}
//private void DrawFan()
//{
//	float offset = 30;
//	float offset_description = 14;
//	float ratio = 260;
//	float ratio2 = 310;
//	CGPoint point = new CGPoint(0, Main.SCREEN_SIZE.Height / 2 / 2 + offset);

//	UIBezierPath path = UIBezierPath.FromArc(new CGPoint(Main.SCREEN_SIZE.Width / 2, (Main.SCREEN_SIZE.Height / 2 / 2) + offset),
//											 50,
//											 (float)Math.PI / 2,
//											 //0,
//											 (float)Math.PI * 2,
//											 true);

//	fanlayer.LineWidth = 100;
//	fanlayer.LineCap = CAShapeLayer.CapButt;
//	fanlayer.StrokeColor = UIColor.Clear.CGColor;
//	fanlayer.FillColor = null;
//	fanlayer.StrokeStart = 0;
//	fanlayer.StrokeEnd = 0;
//	fanlayer.Path = path.CGPath;

//	//加入光暈
//	AddGlow(fanlayer);


//	//gauge圖
//	CALayer imgLayer_Gauge = new CALayer();
//	imgLayer_Gauge.Frame = new CGRect(100, 100, ratio, ratio);
//	imgLayer_Gauge.Position = new CGPoint(Main.SCREEN_SIZE.Width / 2, Main.SCREEN_SIZE.Height / 2 / 2 + offset);
//	imgLayer_Gauge.Contents = UIImage.FromFile("sp_gauge_1.png").CGImage;
//	imgLayer_Gauge.MasksToBounds = true;



//	//框框
//	CALayer imgLayer_Frame = new CALayer();
//	imgLayer_Frame.Bounds = new CGRect(0, 0, ratio2, ratio2 + 20);
//	imgLayer_Frame.Position = new CGPoint(Main.SCREEN_SIZE.Width / 2, Main.SCREEN_SIZE.Height / 2 / 2 + offset + 10);
//	imgLayer_Frame.Contents = UIImage.FromFile("sp_frame.png").CGImage;
//	//imgLayer_Gauge.Frame = new CGRect(100, 100, 330, 330);
//	imgLayer_Frame.MasksToBounds = true;

//	//描述
//	UILabel description = new UILabel(new CGRect(0, 0, ratio, 40));
//	//description.Center = new CGPoint(Main.SCREEN_SIZE.Width / 2, Main.SCREEN_SIZE.Height / 2 / 2 + offset);
//	Console.WriteLine("imgLayer_Gauge  Bottom  : " + imgLayer_Gauge.Frame.Bottom);
//	//description.Center = new CGPoint(Main.SCREEN_SIZE.Width / 2, Main.SCREEN_SIZE.Height / 2 / 2 + offset);
//	description.Center = new CGPoint(Main.SCREEN_SIZE.Width / 2, imgLayer_Gauge.Frame.Bottom + offset_description);
//	description.TextColor = UIColor.FromRGB(39, 177, 255);
//	description.Text = "Justin Say";
//	description.Font = UIFont.ItalicSystemFontOfSize(22);
//	description.TextAlignment = UITextAlignment.Center;
//	//description.BackgroundColor = UIColor.White;

//	//數值
//	vValue = new UILabel(new CGRect(0, 0, ratio, 45));
//	vValue.TextColor = UIColor.FromRGB(39, 177, 255);
//	vValue.Center = new CGPoint(Main.SCREEN_SIZE.Width / 2, Main.SCREEN_SIZE.Height / 2 / 2 + offset);
//	vValue.Text = "6888";
//	vValue.Font = UIFont.BoldSystemFontOfSize(45);
//	vValue.TextAlignment = UITextAlignment.Center;
//	//vValue.BackgroundColor = UIColor.White;

//	//單位
//	UILabel unit = new UILabel(new CGRect(0, 0, ratio / 2, 30));
//	unit.TextColor = UIColor.FromRGB(39, 177, 255);
//	unit.Center = new CGPoint(Main.SCREEN_SIZE.Width / 2, Main.SCREEN_SIZE.Height / 2 / 2 + offset * 2.3);
//	unit.Text = "km/h";
//	unit.Font = UIFont.ItalicSystemFontOfSize(18);
//	unit.TextAlignment = UITextAlignment.Center;




//	this.View.Layer.AddSublayer(imgLayer_Frame);
//	this.View.Layer.AddSublayer(fanlayer);
//	this.View.Layer.AddSublayer(imgLayer_Gauge);
//	this.View.AddSubview(description);
//	this.View.AddSubview(vValue);
//	this.View.AddSubview(unit);
//}


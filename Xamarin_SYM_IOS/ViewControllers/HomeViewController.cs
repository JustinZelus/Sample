using System;
using CoreGraphics;
using UIKit;
using Foundation;
using WebKit;
using CoreFoundation;
using Xamarin_SYM_IOS.SRC.Utils;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using Xamarin_SYM_IOS.SRC.UI;
using CoreLocation;

namespace Xamarin_SYM_IOS
{

    
    public partial class HomeViewController : CustomViewController,IJZDialogViewDelegate
                                            , IJZDialogViewDataSource
    //public partial class HomeViewController : CustomViewController, IUIScrollViewDelegate
	{   
        //播放YOUTUBE用類別
		private WKWebView webView;
        //播放YOUTUBE用回調函數
        private WkWebViewDelegate wkDelegate;
        //播放YOUTUBE用類別
        private YouTubePlayer player;
        public static HomeViewController Instance;



        /* 自定義彈出dialog */
        //private JZDialogView jZDialogView = new JZDialogView("取消", "確定", "請選擇藍芽裝置");
        private JZDialogView jZDialogView = new JZDialogView(NSBundle.MainBundle.GetLocalizedString("textDialogDemo")
                                                             , NSBundle.MainBundle.GetLocalizedString("textDialogOK")
                                                             , NSBundle.MainBundle.GetLocalizedString("textDialogDevice"));
        public JZDialogView BleDeviceListDialog { get => jZDialogView; }
        /* tableView的data */
        private List<string> bles = new List<string>();
        public List<string> Bles
        {
            get => bles;
            set => bles = value;
        }

        public void ClearBLENamesInList()
        {
            if (bles == null) return;
            if (bles.Count > 0) bles.Clear();
        }

        /* Dialog callback functions */
        public void DidConfirmWithItemAtRow(JZDialogView dialogView, nint row)
        {
            Debug.WriteLine("----    DidConfirmWithItemAtRow:  " + row);
            Debug.WriteLine("-----   user selected : " + bles[(int)row]);

            StateMachine.BLEComModel.mCBBLECentral.StopScanning();
            StateMachine.BLEComModel.BindBLEDevice(bles[(int)row]);
            StateMachine.BLEComModel.BindBLEDPeripheral(bles[(int)row]);
            StateMachine.Instance.SendMessage(StateMachineStatus.Device_Connect);
            StateMachine.UIModel.ShowProgressDialog();
            //clear ble names
            ClearBLENamesInList();
        }

        public void JZDialogViewDidClickCancelButton(JZDialogView dialogView)
        {
            Debug.WriteLine("JZDialogViewDidClickCancelButton: " + " go to demo mode");
            if(StateMachine.UIModel.Instance.isActIndAnimating)
                StateMachine.UIModel.CloseProgressDialog();
            StateMachine.Instance.BLESwitch(false);
        }

        public void JZDialogViewWillDisplay(JZDialogView dialogView)
        {
            //Console.WriteLine("JZDialogViewWillDisplay:");
        }

        public void JZDialogViewDidDisplay(JZDialogView dialogView)
        {
            Console.WriteLine("JZDialogViewDidDisplay:");
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                jZDialogView.ReloadData();

            });

        }

        public void JZDialogViewWillDismiss(JZDialogView dialogView)
        {
            //Console.WriteLine("JZDialogViewWillDismiss:");
        }

        public void JZDialogViewDidDismiss(JZDialogView dialogView)
        {
            //Console.WriteLine("JZDialogViewDidDismiss:");


        }

        public nint NumberOfRowsInDialogView(int rowsCount)
        {
            return rowsCount;
        }

        public string TitleForRow(JZDialogView dialogView, int row)
        {
            //if (row > bles.Count - 1)
            //return "";
            return bles[row];
        }

        //-------------------          -------------------     
        //------------------- CALLBACK -------------------
        //-------------------          -------------------
        public void TriggerBLEDiscoveredDeviceAction(string deviceName)
        {
            Debug.WriteLine("-----All----- BLE : ... " + deviceName);

            if (deviceName.StartsWith(ContainerViewController.Instance.TargetDeviceName))
            {
                Debug.WriteLine("sym BLE Catch: ... " + deviceName);

                //if (!mFilterBles.ContainsKey(deviceName))
                if (!bles.Contains(deviceName))
                {
                    //mFilterBles.Add(deviceName, deviceName);
                    bles.Add(deviceName);

                    if (jZDialogView.IsShowing)
                        jZDialogView.ReloadData();
                }

            }
        }

		//
		//------------------------------------生命週期分隔線---------------------------------------
		//

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			Console.WriteLine("HomeViewController: " + "ViewDidLoad()");
            Instance = this;
			UpdateTimerValue += HomeViewController_UpdateTimerValue;


			brandLabel.Text = "EMPTY";//lamborghini
			areaLabel.Text  = "EMPTY";
			vinLabel.Text   = "EMPTY";

			ChangeFontSize(Main.PHONE_SIZE_HOME);

            //DynamicConstraints(); //動態增減UI用


            //ScrollViewAndPageControl(); //可左右滑動頁面的功能
            //AddWebView();

            player = new YouTubePlayer(myView.Bounds);
            myView.AddSubview(player.PlayerView);
            Dictionary<string, object> playerVars = new Dictionary<string, object>{
                {"controls" , 1},
                {"playsinline" , 1},
                {"autohide" , 0},
                {"showinfo" , 1},
                {"modestbranding" , 0}
            };
            //player.LoadWithPlaylistId("LLOdHwyB1YmTx_j4uLFE9pNA",playerVars);
            player.LoadWithVideoId("70O576_rxF4",playerVars);

            InitLocalization();


            //var directories = Directory.EnumerateDirectories("./");
            //foreach(var directory in directories)
            //{
            //    Console.WriteLine(" ------------- " + directory);
            //}

            //var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //foreach(var document in documents)
            //{
            //    Console.WriteLine(" #### " + document);
            //}
            //var directoryname = Path.Combine(documents, "SYM_DB_TEST");
            //var d = Directory.CreateDirectory(directoryname);
            //Console.WriteLine(" ddd " + d);

            //var filename = Path.Combine(directoryname, directoryname + ".dat");
            //File.WriteAllText(filename,"");

            //NSFileManager fm = NSFileManager.DefaultManager;
            //var isSuccess = fm.CreateFile();

            //ulong freeBytes = 0;
            //ulong freeBytes_2 = 0;
            //var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            //var path_2 = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SYM_DB"), "SYM_SCHEMA.dat");
            //Console.WriteLine("--------- dest path -------  " + path);
            //Console.WriteLine("--------- dest path -------  " + path_2);

            //freeBytes = NSFileManager
            //    .DefaultManager
            //    .GetFileSystemAttributes(Environment.GetFolderPath(Environment.SpecialFolder.Personal))
            //    .FreeSize;
            //Console.WriteLine(" -------- freesize ------  " + freeBytes);

            //freeBytes_2 = NSFileManager
            //    .DefaultManager
            //    .GetFileSystemAttributes(path_2)
            //    .FreeSize;

            //Console.WriteLine(" -------- freesize ------  " + freeBytes_2);
            //ble dialog
            jZDialogView.Delegate = this;
            jZDialogView.DataSource = this;
            jZDialogView.needFootView = true;
            //if (AppAttribute.BLE_SCAN_VIEW == AppAttribute.RunningMode.NewScanView)
                

		}


		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			Console.WriteLine("HomeViewController: " + "ViewWillAppear()");
			//IsInited = true;


		}



		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			Console.WriteLine("HomeViewController: " + "ViewDidAppear()");

			IsInited = true;
			//ContainerViewController.Instance.CurrentController = this;

			//statemachine啟動時，會初始化cbblewrapper。cbblewrapper			
            if (!StateMachine.IsActivted && AppAttribute.APP_RUNNING_MODE == AppAttribute.RunningMode.Normal)
            {
                ContainerViewController.Instance.stateMachine.Start();
                StateMachine.IsActivted = true;
            }

            //測試多語
            //UIImageView ipeImageView = new UIImageView(new CGRect(50, 50, 100, 60));
            //ipeImageView.Image = UIImage.FromBundle("aa.png");
            //this.View.AddSubview(ipeImageView);

            //RequestLocationPermission();
		}

       



		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			Console.WriteLine("HomeViewController: " + "ViewWillDisappear()");
		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);
			Console.WriteLine("HomeViewController: " + "ViewDidDisappear()");
			setCurrentPageName();
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}

        public void SetUIImageByResouceName(string imgName)
        {
            UIImage img = UIImage.FromBundle(imgName);
            if (img != null)
            {
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    imgMotor.Image = img;
                });
            }
            else
                Debug.WriteLine("Home image not found !");

        }

		//========================================================================
		//FUNCTIONS
		//========================================================================
		private void InitLocalization()
		{
			imgBrand.Image = UIImage.FromBundle("home_brand.png");
			imgArea.Image = UIImage.FromBundle("home_model.png");
		}

		public void ChangeFontSize(float size)
		{
			brandLabel.Font = UIFont.BoldSystemFontOfSize(size);
			areaLabel.Font = UIFont.BoldSystemFontOfSize(size);
			vinLabel.Font = UIFont.BoldSystemFontOfSize(size);

		}

		protected HomeViewController(IntPtr handle) : base(handle)
		{

		}

		//[Export("scrollViewDidEndDecelerating:")]
		//public void DecelerationEnded(UIScrollView scrollView)
		//{
		//	//float offset = (float) myScrollView.ContentOffset.X;
		//	//float width = (float) myScrollView.Bounds.Size.Width;
		//	//var index = offset / width;

		//	//myPageControl.CurrentPage = (nint)index;

		//}

		private void DynamicConstraints()
		{
			//UIImageView bg_0_100;
			//bg_0_100 = new UIImageView(UIImage.FromFile("log_0_100_bg"));
			//bg_0_100.Frame = new CGRect(10, 207.67, 394, 94);

			//bg_0_100.TranslatesAutoresizingMaskIntoConstraints = false;
			//this.View.AddSubview(bg_0_100);


			//bg_0_100.AddConstraint(NSLayoutConstraint.Create(bg_0_100, NSLayoutAttribute.Width
			//                                           , NSLayoutRelation.Equal
			//                                           , bg_0_100, NSLayoutAttribute.Height
			//                                          , 197 / 47
			//                                          , 0));
			//this.View.AddConstraint(NSLayoutConstraint.Create(bg_0_100, NSLayoutAttribute.Width
			//                                           , NSLayoutRelation.Equal
			//                                           , this.View, NSLayoutAttribute.Width
			//                                          , 0.95f
			//                                          , 0));
			//this.View.AddConstraint(NSLayoutConstraint.Create(bg_0_100, NSLayoutAttribute.Width
			//                                           , NSLayoutRelation.Equal
			//                                           , bg_0_100, NSLayoutAttribute.Height
			//                                          , 197 / 47
			//                                          , 0));

			//this.View.AddConstraint(NSLayoutConstraint.Create(bg_0_100,NSLayoutAttribute.CenterX
			//                                                  ,NSLayoutRelation.Equal
			//                                                  ,this.View,NSLayoutAttribute.CenterX
			//                                                 ,1
			//                                                 ,0));
			//this.View.AddConstraint(NSLayoutConstraint.Create(this.BottomLayoutGuide, NSLayoutAttribute.Top
			//                                                , NSLayoutRelation.Equal
			//                                                  , bg_0_100,NSLayoutAttribute.Bottom
			//                                               , 1/0.5f
			//                                               , 0));
		}

		private void ScrollViewAndPageControl()
		{
			//myScrollView.ContentSize = new CGSize(myStackView.Frame.Width, myStackView.Frame.Height);
			//myPageControl.Pages = 0;
			//myPageControl.PageIndicatorTintColor = UIColor.Green;
			//myPageControl.CurrentPageIndicatorTintColor = UIColor.Blue;
			//myScrollView.Delegate = this;
		}

		public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
		{
			Console.WriteLine("keyPath : " + keyPath);
		}

		private void AddWebView()
		{

			webView = new WKWebView(new CGRect(0, 0, myView.Bounds.Width, myView.Bounds.Height), new WKWebViewConfiguration());

			webView.BackgroundColor = UIColor.Clear;
			webView.Opaque = false;
			//myStackView.AddSubview(webView);
			wkDelegate = new WkWebViewDelegate();
			//wkDelegate.DecidePolicy(webView,;
			webView.NavigationDelegate = wkDelegate;

			webView.AddObserver(this, "loading", NSKeyValueObservingOptions.New, IntPtr.Zero);

			myView.AddSubview(webView);

			//var width = myView.Bounds.Width * 1.93; // 2.5 = 982 (6 plus)
			//var height = myView.Bounds.Height * 2.0;   // 2.44 = 751 (6 plus)


			NSMutableString html = new NSMutableString();
			html.Append(new NSString("<html>"));
			html.Append(new NSString("<head>"));
			html.Append(new NSString("<style type=\"text/css\">"));
			html.Append(new NSString("body {"));
			html.Append(new NSString("background-color: transparent;"));
			html.Append(new NSString("color: black;"));
			html.Append(new NSString("margin: 0;"));
			html.Append(new NSString("}"));
			html.Append(new NSString("</style>"));
			html.Append(new NSString("</head>"));
			html.Append(new NSString("<body>"));
			html.Append(new NSString("<iframe id=\"ytplayer\" type=\"text/html\" width=\""
									 + Main.YOUTUBE_WIDTH_SIZE * myView.Bounds.Width
									 + "\" height=\" " + Main.YOUTUBE_HEIGHT_SIZE * myView.Bounds.Height
			//+ "\" src=\"https://www.youtube.com/embed/kKao5a2SOKM\"" + " frameborder=\"0\" >/>"));
			+ "\" src=\"https://www.youtube.com/embed/kKao5a2SOKM\"" + " frameborder=\"0\" allowfullscreen>/>"));
			html.Append(new NSString("</body>"));
			html.Append(new NSString("</html>"));

			webView.LoadHtmlString(html, null);



		}

		private void ShowWarningDialog()
		{
			DispatchQueue.MainQueue.DispatchAsync(() =>
				{
					var alert = UIAlertController.Create("Open BLE", "Go Setting Your BLE", UIAlertControllerStyle.Alert);
					
					alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (action) =>
					{
						

					}));
					this.PresentViewController(alert, false, null);
				}
			);
		}

        public void InitUI()
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                brandLabel.Text = "";
                areaLabel.Text = "";
                vinLabel.Text = "";
            });
        }

		//ui更新委派
		void HomeViewController_UpdateTimerValue()
		{
			try
			{
				if (StateMachine.Instance != null)
				{
                    //if (StateMachine.Instance.IsUseCommunicationMode)
                    //{
                    //    if (StateMachine.BLEComModel.mCBBLECentral.GetInfoValue() == null)
                    //        return;
                    //}

                    var motorVinData = StateMachine.DataModel.VinIcmUnpacker.MotorVinData;
                    if (motorVinData != null)
					{


						if (brandLabel.Text.Equals("EMPTY") ||
							   brandLabel.Text.Equals(""))
						{
							if (StateMachine.DataModel.ManufactureValue != null)
							{
								brandLabel.Text = motorVinData.Brand;
                            }
						}

						if (areaLabel.Text.Equals("EMPTY") ||
							   areaLabel.Text.Equals(""))
						{
                            if (StateMachine.DataModel.VehicleNameValue != null)
							{
                                areaLabel.Text = motorVinData.VehicleName;
                            }
						}

						if (vinLabel.Text.Equals("EMPTY") ||
						vinLabel.Text.Equals(""))
						{
							vinLabel.Text = motorVinData.ModuleCode;
                        }


						//if(lbl_vin_1.Text.Equals("-"))
						//{

						//	//vinLabel.Text = vin;

						//	var vins = vin.ToCharArray();
						//	lbl_vin_1.Text = vins[0].ToString();
						//	lbl_vin_2.Text = vins[1].ToString();
						//	lbl_vin_3.Text = vins[2].ToString();
						//	lbl_vin_4.Text = vins[3].ToString();
						//	lbl_vin_5.Text = vins[4].ToString();
						//	lbl_vin_6.Text = vins[5].ToString();
						//	lbl_vin_7.Text = vins[6].ToString();
						//	lbl_vin_8.Text = vins[7].ToString();
						//	lbl_vin_9.Text = vins[8].ToString();
						//	lbl_vin_10.Text = vins[9].ToString();
						//	lbl_vin_11.Text = vins[10].ToString();
						//	lbl_vin_12.Text = vins[11].ToString();
						//	lbl_vin_13.Text = vins[12].ToString();
						//	lbl_vin_14.Text = vins[13].ToString();
						//	lbl_vin_15.Text = vins[14].ToString();
						//	lbl_vin_16.Text = vins[15].ToString();
						//	lbl_vin_17.Text = vins[16].ToString();
						//}



					}
                    else if(!StateMachine.Instance.IsUseCommunicationMode)
                    {
                        vinLabel.Text = "F81";
                        brandLabel.Text = "SYM";
                        areaLabel.Text = "Z1 attila";
                    }

					//else // -
					//{
					//	lbl_vin_1.Text = "-";
					//	lbl_vin_2.Text = "-";
					//	lbl_vin_3.Text = "-";
					//	lbl_vin_4.Text = "-";
					//	lbl_vin_5.Text = "-";
					//	lbl_vin_6.Text = "-";
					//	lbl_vin_7.Text = "-";
					//	lbl_vin_8.Text = "-";
					//	lbl_vin_9.Text = "-";
					//	lbl_vin_10.Text = "-";
					//	lbl_vin_11.Text = "-";
					//	lbl_vin_12.Text = "-";
					//	lbl_vin_13.Text = "-";
					//	lbl_vin_14.Text = "-";
					//	lbl_vin_15.Text = "-";
					//	lbl_vin_16.Text = "-";
					//	lbl_vin_17.Text = "-";
					//	vin = "";
					//}


				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.StackTrace);
			}

		}

		public override void setCurrentPageName()
		{
			currentPageView = "HomeView";
			//Console.WriteLine("current page is " + currentPageView);
		}

        #region 從Assets資料夾指定檔名設定圖檔 相關函數
        //private ImageManager resourcesManager = null;
        //public void SetBitmapByDrawableID(String resName)
        //{
        //    resourcesManager?.SetBitmapByDrawableID(ivLogo, resName);
        //}
        #endregion
	}
}


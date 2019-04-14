using System;
using CoreGraphics;
using iPhoneBLE.SRC;
using UIKit;

namespace Xamarin_SYM_IOS
{	
	enum Devices
	{
		Iphone6s,
		IphoneSE,
		Iphone6plus
	}

	struct FontSize
	{
        public const float Iphone6plus_youtube_width = 2.5f;
        public const float Iphone6plus_youtube_height = 2.44f;
		public const int Iphone6plus_liveData_gauge_font = 24;
		public const float Iphone6plus_liveData_unitview = 18;
		public const float Iphone6plus_liveData_value = 44;
		public const float Iphone6plus_liveData_title = 16;
		public const float Iphone6plus_home = 21;
		public const float Iphone6plus_log_history = 60;
		public const float Iphone6plus_0_100_time = 160;
		public const float Iphone6plus_table_height = 68;
		public const float Iphone6plus_table_font = 18;
		public const float Iphone6plus_dtc_title = 21;
		public const float Iphone6plus_dtc_table_height = 110;
		public const float Iphone6plus_table_title = 30;


		public const float Iphone6s_youtube_width = 1.93f;
		public const float Iphone6s_youtube_height = 2.0f;
		public const int Iphone6s_liveData_gauge_font = 19;
		public const float Iphone6s_liveData_unitview = 14;
		public const float Iphone6s_liveData_value = 30;
		public const float Iphone6s_liveData_title = 12;
		public const float Iphone6s_home = 17;
		public const float Iphone6s_log_history = 52;
		public const float Iphone6s_0_100_time = 126;
		public const float Iphone6s_table_height = 50;	
		public const float Iphone6s_table_font = 16;
		public const float Iphone6s_dtc_title = 17;
		public const float Iphone6s_dtc_table_height = 80;
		public const float Iphone6s_table_title = 22;
	}

	class Main
	{ 
		public static CGRect SCREEN_SIZE;
		public static Devices PHONE;

        public static float YOUTUBE_WIDTH_SIZE;
        public static float YOUTUBE_HEIGHT_SIZE;
		public static int PHONE_SIZE_LIVEDATA_GAUGE_FONT;
		public static float PHONE_SIZE_LIVEDATA_UNITVIEW;
		public static float PHONE_SIZE_LIVEDATA_TITLE;
		public static float PHONE_SIZE_LIVEDATA_VALUE;
		public static float PHONE_SIZE_HOME;
		public static float PHONE_SIZE_LOG_HISTORY;
		public static float PHONE_SIZE_HOME_0_100_TIMER;
		public static float PHONE_SIZE_TABLE_CELLHEIGHT;
		public static float PHONE_SIZE_TABLE_CONTENT;
		public static float PHONE_SIZE_TABLE_TITLE;
		public static float PHONE_SIZE_DTC_TITLE;
		public static float PHONE_SIZE_DTC_TABLE_CELLHEIGHT;

        //public static CGRect GAUGES_SCREEN;
	}



	public partial class BaseViewController : UIViewController
	{	



		private ContainerViewController containerViewController;



		private void SetDetailForDevice()
		{ 
			Main.SCREEN_SIZE = UIScreen.MainScreen.Bounds;
            //Main.GAUGES_SCREEN = UIScreen.MainScreen.Bounds;
			if (Main.SCREEN_SIZE.Width <= 320) //4.7吋
			{
				Main.PHONE = Devices.Iphone6s;

			}
			else if (Main.SCREEN_SIZE.Width >= 414)  //5.5吋
			{
				Main.PHONE = Devices.Iphone6plus;

			}


			switch (Main.PHONE)
			{
				case Devices.Iphone6s:
                    Main.YOUTUBE_WIDTH_SIZE = FontSize.Iphone6s_youtube_width;
                    Main.YOUTUBE_HEIGHT_SIZE = FontSize.Iphone6s_youtube_height;
					Main.PHONE_SIZE_LIVEDATA_VALUE = FontSize.Iphone6s_liveData_value;
					Main.PHONE_SIZE_LIVEDATA_TITLE = FontSize.Iphone6s_liveData_title;
					Main.PHONE_SIZE_HOME = FontSize.Iphone6s_home;
					Main.PHONE_SIZE_LOG_HISTORY = FontSize.Iphone6s_log_history;
					Main.PHONE_SIZE_HOME_0_100_TIMER = FontSize.Iphone6s_0_100_time;
					Main.PHONE_SIZE_TABLE_CELLHEIGHT = FontSize.Iphone6s_table_height;
					Main.PHONE_SIZE_TABLE_CONTENT = FontSize.Iphone6s_table_font;
					Main.PHONE_SIZE_LIVEDATA_UNITVIEW = FontSize.Iphone6s_liveData_unitview;
					Main.PHONE_SIZE_LIVEDATA_GAUGE_FONT = FontSize.Iphone6s_liveData_gauge_font;
					Main.PHONE_SIZE_DTC_TITLE = FontSize.Iphone6s_dtc_title;
					Main.PHONE_SIZE_TABLE_TITLE = FontSize.Iphone6s_table_title;
					Main.PHONE_SIZE_DTC_TABLE_CELLHEIGHT = FontSize.Iphone6s_dtc_table_height;
					break;
				case Devices.Iphone6plus:
					Main.YOUTUBE_WIDTH_SIZE = FontSize.Iphone6plus_youtube_width;
                    Main.YOUTUBE_WIDTH_SIZE = FontSize.Iphone6plus_youtube_height;
					Main.YOUTUBE_HEIGHT_SIZE = FontSize.Iphone6s_youtube_height;
					Main.PHONE_SIZE_LIVEDATA_VALUE = FontSize.Iphone6plus_liveData_value;
					Main.PHONE_SIZE_LIVEDATA_TITLE = FontSize.Iphone6plus_liveData_title;
					Main.PHONE_SIZE_HOME = FontSize.Iphone6plus_home;
					Main.PHONE_SIZE_LOG_HISTORY = FontSize.Iphone6plus_log_history;
					Main.PHONE_SIZE_HOME_0_100_TIMER = FontSize.Iphone6plus_0_100_time;
					Main.PHONE_SIZE_TABLE_CELLHEIGHT = FontSize.Iphone6plus_table_height;
					Main.PHONE_SIZE_TABLE_CONTENT = FontSize.Iphone6plus_table_font;
					Main.PHONE_SIZE_LIVEDATA_UNITVIEW = FontSize.Iphone6plus_liveData_unitview;
					Main.PHONE_SIZE_LIVEDATA_GAUGE_FONT = FontSize.Iphone6plus_liveData_gauge_font;
					Main.PHONE_SIZE_DTC_TITLE = FontSize.Iphone6plus_dtc_title;
					Main.PHONE_SIZE_TABLE_TITLE = FontSize.Iphone6plus_table_title;
					Main.PHONE_SIZE_DTC_TABLE_CELLHEIGHT = FontSize.Iphone6plus_dtc_table_height;
					break;
			}
		}

		//內建建構子,不用理
		protected BaseViewController(IntPtr handle) : base(handle)
		{
			Console.WriteLine("建構子" + "BaseViewController");
		}

		public override bool ShouldPerformSegue(string segueIdentifier, Foundation.NSObject sender)
		{
			SetDetailForDevice();
			Console.WriteLine("BaseViewController: " + "ShouldPerformSegue()");

			return base.ShouldPerformSegue(segueIdentifier, sender);
		}

		public override void PrepareForSegue(UIStoryboardSegue segue, Foundation.NSObject sender)
		{	
			//c#要呼叫base,swift不用
			base.PrepareForSegue(segue, sender);
			Console.WriteLine("BaseViewController: " + "PrepareForSegue()");

			if (segue.Identifier == "embedContainer")
			{
				containerViewController = (ContainerViewController)segue.DestinationViewController;
			}
		}

		//------------------------------------生命週期分隔線---------------------------------------

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			Console.WriteLine("BaseViewController: " + "ViewDidLoad()");



			//stateMachine = new StateMachine(containerViewController,targetDeviceName,mCurrentPage);
			//stateMachine.BLEConnetedAction = BLEConnectedMethod;
			//stateMachine.BLEDisconnetedAction = BLEDisconnectedMethod;
			//stateMachine.Start();
			//StateMachine.UIModel.ShowProgressDialog();

		}



		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			Console.WriteLine("BaseViewController: " + "ViewWillAppear()");
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			Console.WriteLine("BaseViewController: " + "ViewDidAppear()");
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			Console.WriteLine("BaseViewController: " + "ViewWillDisappear()");
		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);
			Console.WriteLine("BaseViewController: " + "ViewDidDisappear()");
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();

		}
	}
}


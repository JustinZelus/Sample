using System;
using System.Threading;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using IcmComLib.Utils.iOS;
using UIKit;

namespace Xamarin_IPE_IOS
{	
	

	public partial class LogViewController : CustomViewController, IUIScrollViewDelegate,IUIImagePickerControllerDelegate
	{	
		private SharedPreferencesExtractor spf = new SharedPreferencesExtractor();
		private NSNumberFormatter formatter = new NSNumberFormatter();

		private DispatchQueue gloQueue;//Demo用

		public UILabel BrandLabel
		{
			get
			{
				return lblBrand;
			}
		}

		public UILabel AreaLabel
		{
			get
			{
				return lblArea;
			}
		}

		public UILabel VinLabel
		{
			get
			{
				return lblVin;
			}
		}

		void ReadMyHistory()
		{
			if (spf.Get0_100BestRecord() > 0)
			{
				lbl_0_100_bestRecord.Text = String.Format("{0:0.00}", spf.Get0_100BestRecord());
			}
			else
				lbl_0_100_bestRecord.Text = "00.00";
			//StateMachine.Instance.SendMessage(StateMachineStatus.Communication_FuelConsumption);

		}

		private void ScrollViewAndPageControl()
		{ 
			myScrollView.ContentSize = new CGSize(myStackViewBG.Frame.Width, myStackViewBG.Frame.Height);
			myPageControl.Pages = 3;
			//myPageControl.PageIndicatorTintColor = UIColor.Green;
			//myPageControl.CurrentPageIndicatorTintColor = UIColor.Blue;
			myScrollView.Delegate = this;
		}

		private void DemoUI()
		{
			gloQueue = DispatchQueue.DefaultGlobalQueue;
			gloQueue.DispatchAsync(() =>
			{
				while (true)
				{

					DispatchQueue.MainQueue.DispatchAsync(() =>
					{
						lbl_avg_fuel_permin.Text = RandomDouble(0, 999).ToString("000.0");
					});

					Thread.Sleep(100);
				}
			});
		}

		public double RandomDouble(double minimum, double maximum)
		{
			Random random = new Random();
			return random.NextDouble() * (maximum - minimum) + minimum;
		}

		[Export("scrollViewDidEndDecelerating:")]
		public void DecelerationEnded(UIScrollView scrollView)
		{
			float offset = (float)myScrollView.ContentOffset.X;
			float width = (float)myScrollView.Bounds.Size.Width;
			var index = offset / width;

			myPageControl.CurrentPage = (nint)index;
		}

		void BtnPhoto_TouchUpInside(object sender, EventArgs e)
		{
			SelectPhoto();
		}

		protected LogViewController(IntPtr handle) : base(handle)
		{
		}

		public void ChangeFontSize(float size,float size2)
		{
			BrandLabel.Font = UIFont.BoldSystemFontOfSize(size);
			AreaLabel.Font = UIFont.BoldSystemFontOfSize(size);
			VinLabel.Font = UIFont.BoldSystemFontOfSize(size);

			lbl_0_100_bestRecord.Font = UIFont.FromName(@"Digital-7", size2);
			lbl_0_400_bestRecord.Font = UIFont.FromName(@"Digital-7", size2);
			lbl_avg_fuel.Font = UIFont.FromName(@"Digital-7", size2);
		}

		//------------------------------------生命週期分隔線---------------------------------------

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			//IsInited = true;

			UpdateTimerValue += LogController_UpdateTimerValue;

			//因為有時候會讀不到storyboard賦予的值,所以在這邊做掉
			BrandLabel.Text = "EMPTY";
			AreaLabel.Text = "EMPTY";
			VinLabel.Text = "EMPTY";
			//lbl_avg_fuel.Text = "999.9";

			formatter.MaximumIntegerDigits = 3;
			formatter.MinimumIntegerDigits = 3;
			formatter.MaximumFractionDigits = 1;
			formatter.MinimumFractionDigits = 1;

		

			ReadMyHistory();
			ChangeFontSize(Main.PHONE_SIZE_HOME,Main.PHONE_SIZE_LOG_HISTORY);

			ScrollViewAndPageControl();
			btnPhoto.TouchUpInside += BtnPhoto_TouchUpInside;


			DemoUI();

		}
		UIImagePickerController picker;
		private void SelectPhoto()
		{
			picker = new UIImagePickerController();
			picker.AllowsEditing = false;
			picker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
			picker.Delegate = this;
			this.PresentViewController(picker, true, null);
		}

		[Export("imagePickerController:didFinishPickingImage:editingInfo:")]
		public void FinishedPickingImage(UIImagePickerController picker, UIImage image, NSDictionary editingInfo)
		{
			imgPhoto.Image = image;
			imgPhoto.ClipsToBounds = true;
			imgPhoto.ContentMode = UIViewContentMode.ScaleAspectFill;
			//ParentViewController.DismissModalViewController(true);
			DismissModalViewController(true);

		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			Console.WriteLine("LogViewController: " + "ViewWillAppear()");

			//var value = spf.Get0_100BestRecord();
			//Console.WriteLine("format : " + String.Format("{0:0.00}", spf.Get0_100BestRecord()));
			ReadMyHistory();

		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			Console.WriteLine("LogViewController: " + "ViewDidAppear()");
			IsInited = true;

		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			Console.WriteLine("LogViewController: " + "ViewWillDisappear()");
		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);
			Console.WriteLine("LogViewController: " + "ViewDidDisappear()");
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}

		void LogController_UpdateTimerValue()
		{
			try
			{
				if (StateMachine.Instance != null)
				{
					//home
					var vin = StateMachine.DataModel.VIN;
					if (vin != null)
					{
						if (BrandLabel.Text.Equals("EMPTY") ||
							   BrandLabel.Text.Equals(""))
						{
							if (StateMachine.DataModel.ManufactureValue != null)
								BrandLabel.Text = StateMachine.DataModel.ManufactureValue;
							
						}

						if (AreaLabel.Text.Equals("EMPTY") ||
							   AreaLabel.Text.Equals(""))
						{
							if (StateMachine.DataModel.AreaValue != null)
								AreaLabel.Text = StateMachine.DataModel.AreaValue;
							
						}

						if (VinLabel.Text.Equals("EMPTY") ||
							VinLabel.Text.Equals(""))
						{ 
							VinLabel.Text = vin;
						}
						//if (lbl_vin_1.Text.Equals("-"))
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
						//}
						if (lbl_avg_fuel.Text.Equals("000.0"))
						{
							lbl_avg_fuel.Text = formatter.StringFromNumber(StateMachine.DataModel.FuelConsumption);
						}

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
			throw new NotImplementedException();
		}
	}
}


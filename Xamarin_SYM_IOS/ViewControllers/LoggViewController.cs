using System;
using System.Threading;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using IcmComLib.Utils.iOS;
using UIKit;

namespace Xamarin_SYM_IOS
{


	public partial class LoggViewController : CustomViewController
	{	

		private IPESharedPreferencesExtractor spf = new IPESharedPreferencesExtractor();
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
			if (spf.Get0_100BestRecord() > 0.0f)
			{
				lbl_0_100_bestRecord.Text = String.Format("{0:0.00}", spf.Get0_100BestRecord());
			}
			else
				lbl_0_100_bestRecord.Text = "0.00";
			
			if (spf.Get0_400BestRecord() > 0.0f)
			{
				lbl_0_400_bestRecord.Text = String.Format("{0:0.00}", spf.Get0_400BestRecord());
			}
			else
				lbl_0_400_bestRecord.Text = "0.00";

			if(StateMachine.IsActivted)
            	StateMachine.Instance.SendMessage(StateMachineStatus.Communication_FuelConsumption);
            if (StateMachine.DataModel != null)
                lbl_avg_fuel_permin.Text = StateMachine.DataModel.FuelConsumption.ToString("#####0.00");
		}


		void BtnPhoto_TouchUpInside(object sender, EventArgs e)
		{
			SelectPhoto();
		}

		protected LoggViewController(IntPtr handle) : base(handle)
		{
		}

		public void ChangeFontSize(float size, float size2)
		{
			BrandLabel.Font = UIFont.BoldSystemFontOfSize(size);
			AreaLabel.Font = UIFont.BoldSystemFontOfSize(size);
			VinLabel.Font = UIFont.BoldSystemFontOfSize(size);

			lbl_0_100_bestRecord.Font = UIFont.BoldSystemFontOfSize(size2);
			lbl_0_400_bestRecord.Font = UIFont.BoldSystemFontOfSize(size2);
			lbl_avg_fuel_permin.Font = UIFont.BoldSystemFontOfSize(size2);
			//lbl_0_100_bestRecord.Font = UIFont.FromName(@"Digital-7", size2);
			//lbl_0_400_bestRecord.Font = UIFont.FromName(@"Digital-7", size2);
			//lbl_avg_fuel_permin.Font = UIFont.FromName(@"Digital-7", size2);
		}

		//------------------------------------生命週期分隔線---------------------------------------

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			UpdateTimerValue += LogController_UpdateTimerValue;

			BrandLabel.Text = "EMPTY";
			AreaLabel.Text = "EMPTY";
			VinLabel.Text = "EMPTY";

			formatter.MaximumIntegerDigits = 3;
			formatter.MinimumIntegerDigits = 3;
			formatter.MaximumFractionDigits = 1;
			formatter.MinimumFractionDigits = 1;

			btnClear_0_100.TouchUpInside += BtnClear_0_100_TouchUpInside;
			btnClear_0_400.TouchUpInside += BtnClear_0_400_TouchUpInside;
			btnClear_fc.TouchUpInside += BtnClear_Fc_TouchUpInside;


			ReadMyHistory();
			ChangeFontSize(Main.PHONE_SIZE_HOME, Main.PHONE_SIZE_LOG_HISTORY);

            InitLocalization();
		}

		private void InitLocalization()
		{
			imgBrand.Image = UIImage.FromBundle("home_brand.png");
			imgArea.Image = UIImage.FromBundle("home_model.png");
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

	

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			Console.WriteLine("LogViewController: " + "ViewWillAppear()");

			ReadMyHistory();

		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			Console.WriteLine("LogViewController: " + "ViewDidAppear()");
			IsInited = true;

            if (StateMachine.IsActivted)
            {
                //StateMachine.Instance.RemoveAllMessage();
                StateMachine.Instance.SendMessage(StateMachineStatus.Communication_FuelConsumption);
            }
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			Console.WriteLine("LogViewController: " + "ViewWillDisappear()");

            if (StateMachine.IsActivted)
            {
                //StateMachine.Instance.RemoveAllMessage();
                StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV_STOP);
            }
		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);
			Console.WriteLine("LogViewController: " + "ViewDidDisappear()");
			setCurrentPageName();
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();

		}

		public void InitUI()
		{
			DispatchQueue.MainQueue.DispatchAsync(() =>
			{
				BrandLabel.Text = "";
				AreaLabel.Text = "";
				VinLabel.Text = "";
			});
		}

		void LogController_UpdateTimerValue()
		{
			try
			{
				if (StateMachine.Instance != null)
				{
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
							//if (StateMachine.DataModel.AreaValue != null)
								//AreaLabel.Text = StateMachine.DataModel.AreaValue;

						}

						if (VinLabel.Text.Equals("EMPTY") ||
							VinLabel.Text.Equals(""))
						{
							VinLabel.Text = vin;
						}
						if (lbl_avg_fuel_permin.Text.Equals("000.0"))
						{
							lbl_avg_fuel_permin.Text = formatter.StringFromNumber(StateMachine.DataModel.FuelConsumption);
						}

					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.StackTrace);
			}
		}

		private void WarninigDialog(UILabel lbl, string str)
		{
			var connectAlert = UIAlertController.Create("Erase Record", "Are you sure to Erase ?", UIAlertControllerStyle.Alert);
			connectAlert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (action) =>
			{

				switch (str)
				{
					case "100":
						spf.Set0_100BestRecord((float)0.0);
						lbl.Text = "0.00";
						break;
					case "400":
						spf.Set0_400BestRecord((float)0.0);
						lbl.Text = "0.00";
						break;
					case "fc":
						if (StateMachine.Instance != null)
							StateMachine.Instance.SendMessage(StateMachineStatus.Communication_WriteMemory_For_ClearFuel);
						lbl.Text = "0.0";
						break;

				}


			}));
			connectAlert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (action) =>
			{

			}));

			this.PresentViewController(connectAlert, false, null);
		}

		void BtnClear_Fc_TouchUpInside(object sender, EventArgs e)
		{
			WarninigDialog(lbl_avg_fuel_permin, "fc");
		}

		void BtnClear_0_400_TouchUpInside(object sender, EventArgs e)
		{
			WarninigDialog(lbl_0_400_bestRecord, "400");
		}

		void BtnClear_0_100_TouchUpInside(object sender, EventArgs e)
		{
			WarninigDialog(lbl_0_100_bestRecord, "100");
		}

		public override void setCurrentPageName()
		{
			currentPageView = "LogView";
			//Console.WriteLine("current page is " + currentPageView);
		}
	}
}


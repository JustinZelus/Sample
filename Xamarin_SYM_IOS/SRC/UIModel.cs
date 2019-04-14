using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IcmComLib.Utils.iOS;
using Foundation;
using UIKit;
using CoreGraphics;
using ToastIOS;
using CoreFoundation;
using Xamarin_SYM_IOS;
using Xamarin_SYM_IOS.ViewControllers;
using System.Threading;
using Xamarin_SYM_IOS.SRC.UI;

namespace iPhoneBLE.SRC
{


	public class UIModel : MonitorModel
	{
		public UIModel Instance = null;
		private DataModel mDataModel;
		private Page mCurrentPage;
		private Dictionary<string, UIViewController> mUIViewControllerTable = new Dictionary<string, UIViewController>();
		public ContainerViewController vcManger = null;
		private UIView mContainerView = null;

		UIActivityIndicatorView failedActInd;
		public delegate void CustomFuc();
		public CustomFuc CallBackForRemoveView = null;
		private bool isShowScanViewController = false;
        public bool isrescanAlertDialogShowing = false;
        public bool IsrescanAlertDialogShowing { get => isrescanAlertDialogShowing; set => isrescanAlertDialogShowing = value; }

        private AlertDialog alertDialog = new AlertDialog();
        public AlertDialog AlertDialog { get => alertDialog; } 


		public Page CurrentPage
		{
			get
			{
				return mCurrentPage;
			}
			set
			{
				this.mCurrentPage = value;
			}
		}

        private bool isForgotDeviceNameDialogShowing = false;         public bool IsForgotDeviceNameDialogShowing         {             set => isForgotDeviceNameDialogShowing = value;         } 

		public Dictionary<string, UIViewController> UIViewControllerTable
		{
			get
			{
				return mUIViewControllerTable;
			}
		}



		//這裡就是跑各頁面的delegate
		public override void DoSomething()
		{
            //if (isActIndAnimating)
                //CloseProgressDialog();

			switch (CurrentPage)
			{
				case Page.Home:
					if (vcManger.homeViewController!= null && vcManger.homeViewController.IsInited) //旗標,確定已經初始化
					{
						vcManger.homeViewController.UpdateTimerValue();
					}
					break;
				case Page.Log:
					if (vcManger.loggViewController!= null && vcManger.loggViewController.IsInited)
					{
						vcManger.loggViewController.UpdateTimerValue();
					}
					break;
				case Page.Shift:
					if (vcManger.shiftViewController!= null && vcManger.shiftViewController.IsInited)
					{
						vcManger.shiftViewController.UpdateTimerValue();
					}
					break;
				case Page.LiveData:
					if (vcManger.liveDataViewController!= null && vcManger.liveDataViewController.IsInited)
						vcManger.liveDataViewController.UpdateTimerValue();
					break; 
                case Page.LiveData_2_Frame:
					if (vcManger.liveData_2_Frame_ViewController != null && vcManger.liveData_2_Frame_ViewController.IsInited)
						vcManger.liveData_2_Frame_ViewController.UpdateTimerValue();
					break;
                case Page.LiveData_4_Frame:
					if (vcManger.liveData_4_Frame_ViewController != null && vcManger.liveData_4_Frame_ViewController.IsInited)
						vcManger.liveData_4_Frame_ViewController.UpdateTimerValue();
                    break;
				case Page.LiveData_6_Frame:
					if (vcManger.liveData_6_Frame_ViewController != null && vcManger.liveData_6_Frame_ViewController.IsInited)
						vcManger.liveData_6_Frame_ViewController.UpdateTimerValue();
					break;
				case Page.Speed0_100:
					if (vcManger.zero2HundredViewController!= null && vcManger.zero2HundredViewController.IsInited)
						vcManger.zero2HundredViewController.UpdateTimerValue();
					break;
				case Page.Speed0_400:
					if (vcManger.zero24HundredViewController!= null && vcManger.zero24HundredViewController.IsInited)
						vcManger.zero24HundredViewController.UpdateTimerValue();
					break;
				case Page.DTC:
					if (vcManger.dtcViewController!= null && vcManger.dtcViewController.IsInited)
						vcManger.dtcViewController.UpdateTimerValue();
					break;

				case Page.Valve:
					if (vcManger.valveViewController!= null &&  vcManger.valveViewController.IsInited)
						vcManger.valveViewController.UpdateTimerValue();
					break;
                case Page.Maintenance:
                    if (vcManger.odoNavViewController.ChildViewControllers[0] != null && ((OdoViewController)vcManger.odoNavViewController.ChildViewControllers[0]).IsInited)
                        ((OdoViewController)vcManger.odoNavViewController.ChildViewControllers[0]).UpdateTimerValue();
                        break;
                case Page.Upload:
                    if (vcManger.uploadViewController != null && vcManger.uploadViewController.IsInited)
                        vcManger.uploadViewController.UpdateTimerValue();
                        break;

                case Page.LvCloud:
                    //lvCloudFragment.UpdateTimerValue();
                    break;

                case Page.LvCloud_Show:
                    if (vcManger.dataMonitorViewController.UpdateTimerValue != null)
                        vcManger.dataMonitorViewController.UpdateTimerValue();
                    break;
            }


		}

		//---------------------------------建構子-----------------------------------

		public UIModel(UIViewController containerViewController, DataModel dataModel, Page currentPage)
		{
            //CreateDisConnectAlert();
			vcManger = (ContainerViewController)containerViewController;
			mContainerView = containerViewController.View;
			this.IsUIMode = true;
			Instance = this;
			mDataModel = dataModel;
			mCurrentPage = currentPage;
			//StateMachine.BLEComModel.mCBBLECentral.BLEConnectionTimeoutAction += DelegateGetBLEConnected;
			InitDialogs();
			try
			{
				SettingUIUpdateEvent();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.StackTrace);
			}
		}

        /// <summary>         /// 顯示遺忘裝置對話框         /// </summary>         public void ShowForgotDeviceNameDialog()         {             if (!isForgotDeviceNameDialogShowing)             {                 isForgotDeviceNameDialogShowing = true;                 DispatchQueue.MainQueue.DispatchAsync(() =>                 {                     ContainerViewController.Instance.PresentViewController(alertDialog.ForgotDeviceAlert                                                                            , true                                                                            , null);                 });             }         }

		/// <summary>
		/// Callback for connection of time out.
		/// </summary>
		public void DelegateGetBLEConnected()
		{
			DispatchQueue.MainQueue.DispatchAsync(() =>
			{
				if (isActIndAnimating)
					CloseProgressDialog();
				if (!StateMachine.BLEComModel.mCBBLECentral.IsConnected)
				{
					var failedAlert = UIAlertController.Create("Device Not Found", "Vdi-BT not be detected,\r\ncheck power is On \r\n and Press 'OK' to rescan", UIAlertControllerStyle.Alert);
					failedAlert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (action) =>
					{
						StateMachine.UIModel.ShowProgressDialog();
						StateMachine.Instance.RemoveAllMessage();
                        StateMachine.Instance.SendMessage(StateMachineStatus.Device_Init);
					}));

					if (ContainerViewController.Instance != null)
						ContainerViewController.Instance.PresentViewController(failedAlert, false, null);

				}

			});

		}
		private void InitDialogs()
		{
			InitCustomProgressDialog();
			InitAlertDialog();
			InitECUConnectionFailedDialog();
			//InitFailedWaringAlert();
			InitBlePowerOffAlert();
		}

		public void SettingUIUpdateEvent()
		{



		}



		//寫成靜態的話,第二次呼叫alert時會沒有出現,ui卡住
		public void InitFailedWaringAlert()
		{
			//創建一個轉圈圈圖案並加入alert裡面
			//如果要cancel功能的話: dismissViewControllerAnimated()
			//failedActInd = new UIActivityIndicatorView(vcManger.View.Bounds);
			//failedActInd.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
			//failedActInd.Frame = new CGRect(0.0, 0.0, 60.0, 60.0);
			//failedActInd.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.WhiteLarge;
			//failedActInd.Center = new CGPoint(166,
			//	76);

			var failedAlert = UIAlertController.Create("No BLE Devices Found", "Press 'OK' rescan\r\nPress 'Cancel' Exit app", UIAlertControllerStyle.Alert);
			failedAlert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (action) =>
			{
				StateMachine.Instance.SendMessage(StateMachineStatus.Device_Init);
				ShowProgressDialog();

			}));
			failedAlert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (action) =>
			{
				NSThread.Exit();
			})); ;
			//failedAlert.View.AddSubview(failedActInd);
			//failedActInd.StartAnimating();


			vcManger.PresentViewController(failedAlert, false, null);

		}

		UIView view = null;
		UIActivityIndicatorView actInd = null;
		private void InitCustomProgressDialog()
		{
			view = new UIView();
			view.Frame = mContainerView.Frame;
			view.Center = mContainerView.Center;
			view.BackgroundColor = UIColor.FromRGBA(255, 255, 255, 100);

			var loadingView = new UIView();
			loadingView.Frame = new CGRect(0, 0, 160, 160);
			loadingView.Center = mContainerView.Center;
			loadingView.BackgroundColor = UIColor.FromRGBA(68, 68, 68, 200);
			loadingView.ClipsToBounds = true;
			loadingView.Layer.CornerRadius = 10;

			actInd = new UIActivityIndicatorView();
			actInd.Frame = new CGRect(0.0, 0.0, 40.0, 40.0);
			actInd.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.WhiteLarge;
			actInd.Center = new CGPoint(loadingView.Frame.Size.Width / 2,
				loadingView.Frame.Size.Height / 2);

			nfloat labelX = actInd.Bounds.Size.Width + 2;
			UILabel label = new UILabel(new CGRect(labelX, 0.0f, loadingView.Bounds.Size.Width - (labelX + 2), loadingView.Frame.Size.Height));
			label.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			label.Font = UIFont.BoldSystemFontOfSize(12.0f);
			label.Lines = 1;
			label.BackgroundColor = UIColor.Clear;
			label.TextColor = UIColor.White;
			label.Text = @"Connecting ...";

			//彈出一個視窗,背後有加一塊透明
			loadingView.AddSubview(actInd);
			loadingView.AddSubview(label);
			view.AddSubview(loadingView);
			//mUIView.AddSubview(mContainer);
			//mContainer.Hidden = true         
		}

		UIView view2 = null;
		UIActivityIndicatorView actInd2 = null;
		private void InitECUConnectionFailedDialog()
		{
			view2 = new UIView();
			view2.Frame = mContainerView.Frame;
			view2.Center = mContainerView.Center;
			view2.BackgroundColor = UIColor.FromRGBA(255, 255, 255, 100);

			var loadingView = new UIView();
			loadingView.Frame = new CGRect(0, 0, 160, 160);
			loadingView.Center = mContainerView.Center;
			loadingView.BackgroundColor = UIColor.FromRGBA(68, 68, 68, 200);
			loadingView.ClipsToBounds = true;
			loadingView.Layer.CornerRadius = 10;

			actInd2 = new UIActivityIndicatorView();
			actInd2.Frame = new CGRect(0.0, 0.0, 40.0, 40.0);
			actInd2.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.WhiteLarge;
			actInd2.Center = new CGPoint(loadingView.Frame.Size.Width / 2,
				loadingView.Frame.Size.Height / 2);

			nfloat labelX = actInd2.Bounds.Size.Width + 2;
			UILabel label = new UILabel(new CGRect(labelX, 0.0f, loadingView.Bounds.Size.Width - (labelX + 2), loadingView.Frame.Size.Height));
			label.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			label.Font = UIFont.BoldSystemFontOfSize(12.0f);
			label.Lines = 1;
			label.BackgroundColor = UIColor.Clear;
			label.TextColor = UIColor.White;
			label.Text = @"ECU disconnect...";

			//彈出一個視窗,背後有加一塊透明
			loadingView.AddSubview(actInd2);
			loadingView.AddSubview(label);
			view2.AddSubview(loadingView);
			//mUIView.AddSubview(mContainer);
			//mContainer.Hidden = true         
		}

        public bool isCheckBTSwitchAlertDialogShow = false;         public void ShowCheckBTSwitchAlertDialog()         {             if (isCheckBTSwitchAlertDialogShow)             {                 return;             }              isCheckBTSwitchAlertDialogShow = true;

            DispatchQueue.MainQueue.DispatchAsync(() =>             {                 ContainerViewController.Instance.PresentViewController(blePowerOffAlert, true, () => {

                });              });         }

		public bool isToastShow2 = false;
		public void ShowECUCommunicationfailed()
		{
			if (isToastShow2 == false)
			{
				DispatchQueue.MainQueue.DispatchAsync(() =>
				{
					mContainerView.AddSubview(view2);
					isToastShow2 = true;
					if (!actInd2.IsAnimating)
						actInd2.StartAnimating();
				});
			}
			else
				return;
		}

		public void CloseECUCommunicationfailed()
		{
			if (isToastShow2 == true)
			{
				DispatchQueue.MainQueue.DispatchAsync(() =>
				{
					if (actInd2.IsAnimating)
						actInd2.StopAnimating();
					view2.RemoveFromSuperview();
					//mContainer.Hidden = true;
					isToastShow2 = false;
				});
			}
			else
				return;
		}

		public bool isToastShow = false;
		public bool isActIndAnimating = false;
		public void ShowProgressDialog()
		{
			if (isToastShow == false)
			{
				DispatchQueue.MainQueue.DispatchAsync(() =>
				{
					mContainerView.AddSubview(view);
					isToastShow = true;
					if (!actInd.IsAnimating)
					{
						actInd.StartAnimating();
						isActIndAnimating = true;
					}
				});
			}
		}

		public void CloseProgressDialog()
		{
			if (isToastShow == true)
			{
				DispatchQueue.MainQueue.DispatchAsync(() =>
				{
					if (actInd.IsAnimating)
					{
						actInd.StopAnimating();
						isActIndAnimating = false;
					}
					view.RemoveFromSuperview();
					//mContainer.Hidden = true;
					isToastShow = false;
					if (ScanViewController.Instance != null)
					{
						if (ScanViewController.Instance.IsShowing && CallBackForRemoveView != null)
						{
							CallBackForRemoveView();
						}
					}
				});
			}
		}

        public void ShowRescanAlertDialog()
        {

            if (!isrescanAlertDialogShowing)
            {
                isrescanAlertDialogShowing = true;
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    vcManger.PresentViewController(AlertDialog.DisConnectedAlert,false,null);
                });
            }
            else
                return;
        }

        public void CloseRescanAlertDialog()
        {
            if (isrescanAlertDialogShowing)
            {
                isrescanAlertDialogShowing = false;
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    AlertDialog.DisConnectedAlert.DismissViewController(false, null);

                });
            }
        }

        //public void ShowScanFailedDialog()
        //{ 
        //	//DispatchQueue.MainQueue.DispatchAsync(() =>
        //	//{
        //	//	vcManger.PresentViewController(failedAlert, false, null);
        //	//});

        //}

        bool bMaintenance = false;
        bool bLVIcon = false;
        bool bLvCloud = false;
        bool bMap = false;

        public void simUI()
        {
            bMaintenance = false;
            bLVIcon = false;
            //bTestFunc = false;
            if (!bMaintenance)
            {

                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                      ContainerViewController.Instance.ButtonBoxes["btnMaintenance"].Hidden = true;
                });

                //ContainerbtnMaintenance.Visibility = ViewStates.Gone;

            }
            if (!bLVIcon)
            {
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    ContainerViewController.Instance.ButtonBoxes["btnLiveDataIcon"].Hidden = true;
                });

                //btnLVIcon.Visibility = ViewStates.Gone;
            }
            if (!bLvCloud)
            {
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    ContainerViewController.Instance.ButtonBoxes["btnLvDataCloud"].Hidden = true;
                });

                //btnLVIcon.Visibility = ViewStates.Gone;
            }
            if (!bMap)
            {
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    ContainerViewController.Instance.ButtonBoxes["btnMap"].Hidden = true;
                });

                //btnLVIcon.Visibility = ViewStates.Gone;
            }

            //if (!bTestFunc)
            //    btnTestFunc.Visibility = ViewStates.Gone;

        }

		private UIAlertController alert;
		private void InitAlertDialog()
		{
			String title = "Warning";
			String message = "Would you want to re-connect BLE ?";
			UIAlertControllerStyle preferredStyle = UIAlertControllerStyle.Alert;
			alert = UIAlertController.Create(title, message, preferredStyle);



			UIAlertAction continueAction = UIAlertAction.Create("Continue", UIAlertActionStyle.Default, (continueAct) =>
			{
				Toast.MakeText("Continue").Show();
			});
			UIAlertAction cancelAction = UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (cancelAct) =>
			{
				Toast.MakeText("Cancel").Show();
			});
			alert.AddAction(continueAction);
			alert.AddAction(cancelAction);

		}

        public UIAlertController BlePowerOffAlert         {             get             {                 return blePowerOffAlert;             }         }
        private UIAlertController blePowerOffAlert;
        public bool isUserEnterBackground = false;
        private void InitBlePowerOffAlert()
        {
            blePowerOffAlert = UIAlertController.Create("Bluetooth switch enable", "Please press the \"Setting\" and go to the setting page to enable Bluetooth switch", UIAlertControllerStyle.Alert);
            UIAlertAction goAction = UIAlertAction.Create("Setting", UIAlertActionStyle.Default, (goAct) =>
            {
                this.isUserEnterBackground = true;                 this.isCheckBTSwitchAlertDialogShow = false;
                NSUrl url = new NSUrl("App-prefs:root=Bluetooth");
                if (UIApplication.SharedApplication.CanOpenUrl(url))
                    UIApplication.SharedApplication.OpenUrl(url);

            });

            blePowerOffAlert.AddAction(goAction);
        }

        public void CloseBlePowerOffAlert()
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                this.isCheckBTSwitchAlertDialogShow = false;
                blePowerOffAlert.DismissViewController(false, null);
            });

        }

		//暫時棄用
		public void CloseAlertDialog()
		{
			if (IsAlertDialogShow == true)
			{
				DispatchQueue.MainQueue.DispatchAsync(() =>
				{
					if (alert != null && IsAlertDialogShow == true)
					{
						alert.DismissViewControllerAsync(true);
						IsAlertDialogShow = false;
					}
				});
			}
		}

		private bool IsAlertDialogShow = false;
		public void ShowAlertDialog()
		{
			if (IsAlertDialogShow == false)
			{
				DispatchQueue.MainQueue.DispatchAsync(() =>
				{
					if (vcManger != null && alert != null && IsAlertDialogShow == false)
					{
						vcManger.PresentViewControllerAsync(alert, true);
						IsAlertDialogShow = true;
					}
				});
			}
		}

		public bool IsScrollInEndPos = false;
		public bool SlideScrollView2End()
		{
			return true;
		}
		private bool isShowBlePowerOffAlert = false;
		public bool IsShowBlePowerOffAlert
        {
            get
            {
                return isShowBlePowerOffAlert;
            }
            set
            {
                isShowBlePowerOffAlert = value;
            }
        }

        /// <summary>         /// 顯示斷線時的對話框         /// </summary>         public void ShowDisconnectedAlertDialog()         {             DispatchQueue.MainQueue.DispatchAsync(() => {                 ContainerViewController.Instance.PresentViewController(alertDialog.DisConnectedAlert                                                                        , true                                                                        , null);             });         }

		public void ShowScanViewController()
		{
			if (!isShowScanViewController)
			{
				isShowScanViewController = true;
				DispatchQueue.MainQueue.DispatchAsync(() =>
				{
					ContainerViewController.Instance.PerformSegue(MyCustomPages.ScanView, null);
				});

			}
		}

       

		public bool IsShowScanViewController
		{ 
			get
			{
				return isShowScanViewController;
			}
			set
			{
				isShowScanViewController = value;
			}
		}

        /// <summary>
        /// 開啟各Image Button Icon為不可按狀態
        /// </summary>
        public void DisableIconTabs()
        {   
            var buttonList = ContainerViewController.Instance.ButtonBoxes.Values;
            foreach (var imgBtn in buttonList)
            {
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    imgBtn.Enabled = false;
                });

            }
        }

        /// <summary>
        /// 開啟各Image Button Icon為可按狀態
        /// </summary>
        public void EnableIconTabs()
        {
            var buttonList = ContainerViewController.Instance.ButtonBoxes.Values;
            foreach (var imgBtn in buttonList)
            {
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    imgBtn.Enabled = true;
                });

            }
        }
    }
}
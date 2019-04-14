using System;
using System.Threading;
using CoreFoundation;
using Foundation;
using UIKit;

namespace Xamarin_SYM_IOS.SRC.UI
{
    public class AlertDialog
    {
        //private UIAlertController disConnectedAlert;
        private UIAlertController forgotDeviceAlert;
        private UIAlertController gpsOpenAlert;
        //private UIAlertController amqEnabledAlertDialog;
        public UIAlertController DisConnectedAlert 
        { 
            get
            {
                return BuildDisConnectedAlert(); 
            } 
        }
        public UIAlertController ForgotDeviceAlert { get => forgotDeviceAlert; }
        public UIAlertController GpsOpenAlert { get => gpsOpenAlert; }
        //public static UIAlertController AmqEnabledAlertDialog { get => amqEnabledAlertDialog; }


        public AlertDialog()
        {
            //斷線後的提醒視窗
            BuildDisConnectedAlert();
            //遺忘裝置的視窗
            BuildForgotDeviceAlert();
            //GPS需開啟提醒視窗
            BuildGpsOpenAlert();
            //AMQ上傳提醒視窗
            //BuildAMQEnableAlert();
        }

        //private void BuildAMQEnableAlert()
        //{
        //    if(amqEnabledAlertDialog == null)
        //    {
        //        amqEnabledAlertDialog = UIAlertController.Create("Warning", "Cloud upload process is enabled, do you want to stop the cloud upload process and return Data Monitor selection page ?",UIAlertControllerStyle.Alert);
        //        //第一顆按鈕
        //        amqEnabledAlertDialog.AddAction(UIAlertAction.Create("OK",UIAlertActionStyle.Default,(action) => {
        //            //amqEnabledAlertDialog.Dismiss();
        //            isButtonClicked = false;
        //            StopAmqCommunication();
        //            RestoreUI();
        //            StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV_STOP);
        //            StateMachine.UIModel.SwitchFragment(StateMachine.UIModel.lvCloudFragment, "lvCloudFragment");
        //        }));
        //        //第二顆按鈕
        //        amqEnabledAlertDialog.AddAction(UIAlertAction.Create("NO", UIAlertActionStyle.Cancel, (action) => {
        //            //amqEnabledAlertDialog.Dismiss();
        //        }));
        //    }
        //}

        private void BuildGpsOpenAlert()
        {
            if(gpsOpenAlert == null)
            {
                gpsOpenAlert = UIAlertController.Create("開啟取用定位功能", "請允許開啟取用定位功能，開啟後將上傳您的GPS至伺服器作資料分析。", UIAlertControllerStyle.Alert);
                gpsOpenAlert.AddAction(UIAlertAction.Create("設定",UIAlertActionStyle.Default
                                       , (action) => {
                    NSUrl url = new NSUrl(UIApplication.OpenSettingsUrlString);
                    if (UIApplication.SharedApplication.CanOpenUrl(url))
                        UIApplication.SharedApplication.OpenUrl(url);            
                }));
            }
        }

        private void BuildForgotDeviceAlert()
        {
            if (forgotDeviceAlert == null)
            {
                forgotDeviceAlert = UIAlertController.Create(NSBundle.MainBundle.LocalizedString("forgot", "forgot")
                                                             , NSBundle.MainBundle.LocalizedString("forgotAction", "forgotAction")
                                                             , UIAlertControllerStyle.Alert);
                forgotDeviceAlert.AddAction(UIAlertAction.Create("Forget"
                                                                 , UIAlertActionStyle.Destructive
                                                                 , (action) =>
                                                                 {
                                                                     //清除手機儲存的藍芽名稱
                                                                    StateMachine.Instance.symSharedPreferencesExtractor.SetBleDeviceName("");
                                                                     //關閉藍芽連線動畫
                                                                     StateMachine.UIModel.CloseProgressDialog();
                                                                     //秀出藍芽連線視窗(必須先自動掃描一次)
                                                                     if (StateMachine.DataModel.SYMSharedPreferencesExtractor.GetBleDeviceName().Equals(""))
                                                                     {
                                                                         DispatchQueue.MainQueue.DispatchAsync(() =>
                                                                         {
                                                                             if (HomeViewController.Instance != null)
                                                                                 HomeViewController.Instance.BleDeviceListDialog.Show();
                                                                         });
                                                                         StateMachine.BLEComModel.Disconnect();
                                                                         StateMachine.BLEComModel.StopScanThenStartScan();
                                                                     }

                                                                     StateMachine.UIModel.IsForgotDeviceNameDialogShowing = false;
                                                                 }));
                forgotDeviceAlert.AddAction(UIAlertAction.Create("Cancel"
                                                                 , UIAlertActionStyle.Cancel
                                                                 , (action) =>
                                                                 {
                                                                     StateMachine.UIModel.IsForgotDeviceNameDialogShowing = false;
                                                                 }));
            }
        }

        private UIAlertController BuildDisConnectedAlert()
        {
            UIAlertController disConnectedAlert = null;
            if (disConnectedAlert == null)
            {
                disConnectedAlert = UIAlertController.Create(NSBundle.MainBundle.LocalizedString("disconnected", "disconnected")
                                                             , NSBundle.MainBundle.LocalizedString("disconnectedAction", "disconnectedAction")
                                                             , UIAlertControllerStyle.Alert);
                disConnectedAlert.AddAction(UIAlertAction.Create("Forget"
                                                                 , UIAlertActionStyle.Destructive
                                                                 , (action) =>
                                                                 {
                                                                     HomeViewController.Instance.ClearBLENamesInList();
                                                                     //清除手機儲存的藍芽名稱
                                                                     StateMachine.DataModel.SYMSharedPreferencesExtractor.SetBleDeviceName("");
                                                                     //關閉藍芽連線動畫
                                                                     StateMachine.UIModel.CloseProgressDialog();

                                                                     Thread.Sleep(100);
                                                                     //秀出藍芽連線視窗(必須先自動掃描一次)
                                                                     if (StateMachine.DataModel.SYMSharedPreferencesExtractor.GetBleDeviceName().Equals(""))
                                                                     {
                                                                         DispatchQueue.MainQueue.DispatchAsync(() =>
                                                                         {
                                                                             if (HomeViewController.Instance != null)
                                                                                 HomeViewController.Instance.BleDeviceListDialog.Show();
                                                                         });

                                                                         Thread.Sleep(100);
                                                                         StateMachine.BLEComModel.StopScanThenStartScan();
                                                                     }
                                                                 }));
                disConnectedAlert.AddAction(UIAlertAction.Create("Yes", UIAlertActionStyle.Default, (action) =>
                {
                    StateMachine.UIModel.IsrescanAlertDialogShowing = false;

                    HomeViewController.Instance.ClearBLENamesInList();
                    //清除StateMachine所有狀態
                    StateMachine.Instance.RemoveAllMessage();
                    //秀連線動畫
                    StateMachine.UIModel.ShowProgressDialog();
                    //下Device_Init指令
                    StateMachine.Instance.SendMessage(StateMachineStatus.Device_Init);
                }));
            }
            return disConnectedAlert;
        }

    }
}

using System;
namespace Xamarin_SYM_IOS
{   


	public class AppAttribute
	{
		public enum RunningMode
		{
			DoUITest,   
			Normal,
			IndividualModeTest,
			StateMachineModeTest,
			DeviceBlePowerOff,
			DeviceBlePowerOn,
            RunStartMovie,
            NoStartMovie,
            NewScanView,
            OldScanView
		}

        public static AppAttribute.RunningMode APP_RUNNING_MODE = AppAttribute.RunningMode.Normal;
        public static AppAttribute.RunningMode DEVICE_BLE_STATE = AppAttribute.RunningMode.DeviceBlePowerOff;
        public static AppAttribute.RunningMode START_MOVIE = AppAttribute.RunningMode.NoStartMovie;
        public static AppAttribute.RunningMode  BLE_SCAN_VIEW = AppAttribute.RunningMode.NewScanView;
	}
}

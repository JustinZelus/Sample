using System;
using Foundation;
using UIKit;

namespace Xamarin_SYM_IOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations

		public override UIWindow Window
		{
			get;
			set;
		}

		public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
		{
			// Override point for customization after application launch.
			// If not required for your application you can safely delete this method
			Console.WriteLine("AppDelegate : " +"FinishedLaunching");
			return true;
		}

		public override void OnResignActivation(UIApplication application)
		{
			// Invoked when the application is about to move from active to inactive state.
			// This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
			// or when the user quits the application and it begins the transition to the background state.
			// Games should use this method to pause the game.
			Console.WriteLine("AppDelegate : " +"OnResignActivation");
		}

		public override void DidEnterBackground(UIApplication application)
		{
			// Use this method to release shared resources, save user data, invalidate timers and store the application state.
			// If your application supports background exection this method is called instead of WillTerminate when the user quits.
			Console.WriteLine("AppDelegate : " +"DidEnterBackground");
		}

		public override void WillEnterForeground(UIApplication application)
		{
			// Called as part of the transiton from background to active state.
			// Here you can undo many of the changes made on entering the background.
			Console.WriteLine("AppDelegate : " +"WillEnterForeground");
            StateMachine.UIModel.isUserEnterBackground = false;
			if (StateMachine.IsActivted)
			{
				//StateMachine.BLEComModel.DetectBleState();
				//Console.WriteLine("WillEnterForeground - CBCentralManagerState : " + StateMachine.BLEComModel.DetectBleState());
				//if (!StateMachine.BLEComModel.IsPowerOff) {
					//StateMachine.BLEComModel.IsPowerOff = true;
					//StateMachine.UIModel.Instance.IsShowBlePowerOffAlert = false;
				//}
			}
				
		}

		public override void OnActivated(UIApplication application)
		{
			// Restart any tasks that were paused (or not yet started) while the application was inactive. 
			// If the application was previously in the background, optionally refresh the user interface.
			Console.WriteLine("AppDelegate : " +"OnActivated");
		}

		public override void WillTerminate(UIApplication application)
		{
			// Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
			Console.WriteLine("AppDelegate : " +"WillTerminate");
		}

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations(UIApplication application, UIWindow forWindow)
        {
			var presentedViewController = TopMostViewController();
			//var presentedViewController = TopViewController();
			string className = null;

			if (presentedViewController == null)
				return UIInterfaceOrientationMask.Portrait;

			if (presentedViewController != null)
			{
				//className = presentedViewController.GetType().Name;

				//className = presentedViewController.GetType().Name;
				className = presentedViewController.GetType().FullName;
			}

			if (forWindow != null && className.Equals("UIKit.UIViewController"))
			{
				return UIInterfaceOrientationMask.All;
			}
			else
				return UIInterfaceOrientationMask.Portrait;
        }

		private UIViewController TopMostViewController()
		{
			if (UIApplication.SharedApplication.KeyWindow != null)
			{
				UIViewController topController = UIApplication.SharedApplication.KeyWindow.RootViewController;
				while (topController.PresentedViewController != null)
				{
					topController = topController.PresentedViewController;
				}
				return topController;
			}
			else
				return null;

		}
	}
}


using System;
using UIKit;
namespace Xamarin_SYM_IOS
{	
	//各vc需實作此類
	public abstract class CustomViewController:UIViewController
	{	
		public bool IsInited = false;
        public bool isViewDidAppear = false;

		public delegate void UIupdateHandler();
		public UIupdateHandler UpdateTimerValue;

		protected static string currentPageView = "";

		protected CustomViewController(IntPtr handle) : base(handle)
		{ 
			
		}

		public abstract void setCurrentPageName();


		public float TransformSelectedIdToValue()
		{
			return 0;
		}
	}
}

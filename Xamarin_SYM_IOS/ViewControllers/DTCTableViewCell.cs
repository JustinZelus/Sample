using System;

using Foundation;
using UIKit;

namespace Xamarin_SYM_IOS
{
	public partial class DTCTableViewCell : UITableViewCell
	{
		//public static readonly NSString Key = new NSString("DTCTableViewCell");
		//public static readonly UINib Nib;

		static DTCTableViewCell()
		{
			//Nib = UINib.FromName("DTCTableViewCell", NSBundle.MainBundle);
		}

		protected DTCTableViewCell(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();
		}

		public override void SetSelected(bool selected, bool animated)
		{
			base.SetSelected(selected, animated);
		}



		public UILabel Title
		{ 
			get
			{
				return title;
			}
		}

		public UILabel SubTitle
		{
			get
			{
				return subTitle;
			}
		}

       
	}
}

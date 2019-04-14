using System;

using Foundation;
using UIKit;

namespace Xamarin_SYM_IOS.ViewControllers
{
    public partial class NewDTCTableViewCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("dtcCell");
        public static readonly UINib Nib;

        static NewDTCTableViewCell()
        {
           // Nib = UINib.FromName("NewDTCTableViewCell", NSBundle.MainBundle);
        }

        protected NewDTCTableViewCell(IntPtr handle) : base(handle)
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
                return lblTitle;
            }
        }

        public UILabel SubTitle
        {
            get
            {
                return lblSubTitle;
            }
        }

    }
}

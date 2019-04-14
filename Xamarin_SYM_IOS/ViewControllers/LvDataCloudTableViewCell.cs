using System;

using Foundation;
using UIKit;

namespace Xamarin_SYM_IOS.ViewControllers
{
    public partial class LvDataCloudTableViewCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("LvDataCloudTableViewCell");
        public static readonly UINib Nib;

        static LvDataCloudTableViewCell()
        {
            Nib = UINib.FromName("LvDataCloudTableViewCell", NSBundle.MainBundle);
        }

        protected LvDataCloudTableViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public UILabel Title 
        {
            get {
                return lblTitle;
            }
        }

        //public UIButton CheckBox
        //{
        //    get {
        //        return btnCheckBox;
        //    }
        //}
    }
}

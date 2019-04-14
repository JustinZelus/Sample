// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Xamarin_SYM_IOS.ViewControllers
{
    [Register ("LvDataCloudViewController")]
    partial class LvDataCloudViewController
    {
        [Outlet]
        UIKit.UIButton btnCancelAllItems { get; set; }


        [Outlet]
        UIKit.UIButton btnChoiceAllItems { get; set; }


        [Outlet]
        UIKit.UIButton btnSendLv { get; set; }


        [Outlet]
        UIKit.UITableView tblList { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnCancelAllItems != null) {
                btnCancelAllItems.Dispose ();
                btnCancelAllItems = null;
            }

            if (btnChoiceAllItems != null) {
                btnChoiceAllItems.Dispose ();
                btnChoiceAllItems = null;
            }

            if (btnSendLv != null) {
                btnSendLv.Dispose ();
                btnSendLv = null;
            }

            if (tblList != null) {
                tblList.Dispose ();
                tblList = null;
            }
        }
    }
}
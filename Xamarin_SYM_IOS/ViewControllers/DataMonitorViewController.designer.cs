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
    [Register ("DataMonitorViewController")]
    partial class DataMonitorViewController
    {
        [Outlet]
        UIKit.UIButton btnBack { get; set; }

        [Outlet]
        UIKit.UIButton btnItem { get; set; }

        [Outlet]
        UIKit.UIButton btnSendLvByAmq { get; set; }

        [Outlet]
        UIKit.UIButton btnUnit { get; set; }

        [Outlet]
        UIKit.UIButton btnValue { get; set; }

        [Outlet]
        UIKit.UITableView tblList { get; set; }

        [Outlet]
        UIKit.UITableView tblTop { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnBack != null) {
                btnBack.Dispose ();
                btnBack = null;
            }

            if (btnItem != null) {
                btnItem.Dispose ();
                btnItem = null;
            }

            if (btnSendLvByAmq != null) {
                btnSendLvByAmq.Dispose ();
                btnSendLvByAmq = null;
            }

            if (btnUnit != null) {
                btnUnit.Dispose ();
                btnUnit = null;
            }

            if (btnValue != null) {
                btnValue.Dispose ();
                btnValue = null;
            }

            if (tblList != null) {
                tblList.Dispose ();
                tblList = null;
            }
        }
    }
}
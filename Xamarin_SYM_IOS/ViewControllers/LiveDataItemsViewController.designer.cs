// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Xamarin_SYM_IOS
{
    [Register ("LiveDataItemsViewController")]
    partial class LiveDataItemsViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnOK { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView liveDataItemsTable { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnOK != null) {
                btnOK.Dispose ();
                btnOK = null;
            }

            if (liveDataItemsTable != null) {
                liveDataItemsTable.Dispose ();
                liveDataItemsTable = null;
            }
        }
    }
}
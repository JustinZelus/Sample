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
    [Register ("LiveData_2_Frame_ViewController")]
    partial class LiveData_2_Frame_ViewController
    {
        [Outlet]
        UIKit.UIButton btn_Up { get; set; }


        [Outlet]
        UIKit.UIButton btnBack { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btn_Up != null) {
                btn_Up.Dispose ();
                btn_Up = null;
            }

            if (btnBack != null) {
                btnBack.Dispose ();
                btnBack = null;
            }
        }
    }
}
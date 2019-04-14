// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Xamarin_SYM_IOS
{
    [Register ("HomeViewController")]
    partial class HomeViewController
    {
        [Outlet]
        UIKit.UILabel areaLabel { get; set; }


        [Outlet]
        UIKit.UILabel brandLabel { get; set; }


        [Outlet]
        UIKit.UIImageView imgArea { get; set; }


        [Outlet]
        UIKit.UIImageView imgBrand { get; set; }


        [Outlet]
        UIKit.UIImageView imgMotor { get; set; }


        [Outlet]
        UIKit.UIImageView ivLogo { get; set; }


        [Outlet]
        UIKit.UIPageControl myPageControl { get; set; }


        [Outlet]
        UIKit.UIView myView { get; set; }


        [Outlet]
        UIKit.UILabel vinLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (areaLabel != null) {
                areaLabel.Dispose ();
                areaLabel = null;
            }

            if (brandLabel != null) {
                brandLabel.Dispose ();
                brandLabel = null;
            }

            if (imgArea != null) {
                imgArea.Dispose ();
                imgArea = null;
            }

            if (imgBrand != null) {
                imgBrand.Dispose ();
                imgBrand = null;
            }

            if (imgMotor != null) {
                imgMotor.Dispose ();
                imgMotor = null;
            }

            if (ivLogo != null) {
                ivLogo.Dispose ();
                ivLogo = null;
            }

            if (myPageControl != null) {
                myPageControl.Dispose ();
                myPageControl = null;
            }

            if (myView != null) {
                myView.Dispose ();
                myView = null;
            }

            if (vinLabel != null) {
                vinLabel.Dispose ();
                vinLabel = null;
            }
        }
    }
}
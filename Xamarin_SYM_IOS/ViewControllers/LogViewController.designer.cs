// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Xamarin_IPE_IOS
{
    [Register ("LogViewController")]
    partial class LogViewController
    {
        [Outlet]
        UIKit.UIButton btnPhoto { get; set; }


        [Outlet]
        UIKit.UIImageView imgPhoto { get; set; }


        [Outlet]
        UIKit.UILabel lbl_0_100_bestRecord { get; set; }


        [Outlet]
        UIKit.UILabel lbl_0_400_bestRecord { get; set; }


        [Outlet]
        UIKit.UILabel lbl_avg_fuel { get; set; }


        [Outlet]
        UIKit.UILabel lbl_avg_fuel_permin { get; set; }


        [Outlet]
        UIKit.UILabel lblArea { get; set; }


        [Outlet]
        UIKit.UILabel lblBrand { get; set; }


        [Outlet]
        UIKit.UILabel lblVin { get; set; }


        [Outlet]
        UIKit.UIPageControl myPageControl { get; set; }


        [Outlet]
        UIKit.UIScrollView myScrollView { get; set; }


        [Outlet]
        UIKit.UIStackView myStackViewBG { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnPhoto != null) {
                btnPhoto.Dispose ();
                btnPhoto = null;
            }

            if (imgPhoto != null) {
                imgPhoto.Dispose ();
                imgPhoto = null;
            }

            if (lbl_0_100_bestRecord != null) {
                lbl_0_100_bestRecord.Dispose ();
                lbl_0_100_bestRecord = null;
            }

            if (lbl_0_400_bestRecord != null) {
                lbl_0_400_bestRecord.Dispose ();
                lbl_0_400_bestRecord = null;
            }

            if (lbl_avg_fuel != null) {
                lbl_avg_fuel.Dispose ();
                lbl_avg_fuel = null;
            }

            if (lbl_avg_fuel_permin != null) {
                lbl_avg_fuel_permin.Dispose ();
                lbl_avg_fuel_permin = null;
            }

            if (lblArea != null) {
                lblArea.Dispose ();
                lblArea = null;
            }

            if (lblBrand != null) {
                lblBrand.Dispose ();
                lblBrand = null;
            }

            if (lblVin != null) {
                lblVin.Dispose ();
                lblVin = null;
            }

            if (myPageControl != null) {
                myPageControl.Dispose ();
                myPageControl = null;
            }

            if (myScrollView != null) {
                myScrollView.Dispose ();
                myScrollView = null;
            }

            if (myStackViewBG != null) {
                myStackViewBG.Dispose ();
                myStackViewBG = null;
            }
        }
    }
}
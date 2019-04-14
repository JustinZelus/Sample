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
    [Register ("DTCTableViewCell")]
    partial class DTCTableViewCell
    {
        [Outlet]
        UIKit.UILabel subTitle { get; set; }


        [Outlet]
        UIKit.UILabel title { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel subTitleLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView thumbnailImageView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel titleLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (subTitle != null) {
                subTitle.Dispose ();
                subTitle = null;
            }

            if (subTitleLabel != null) {
                subTitleLabel.Dispose ();
                subTitleLabel = null;
            }

            if (thumbnailImageView != null) {
                thumbnailImageView.Dispose ();
                thumbnailImageView = null;
            }

            if (title != null) {
                title.Dispose ();
                title = null;
            }

            if (titleLabel != null) {
                titleLabel.Dispose ();
                titleLabel = null;
            }
        }
    }
}
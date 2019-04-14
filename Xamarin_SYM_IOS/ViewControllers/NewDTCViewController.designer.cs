// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Xamarin_SYM_IOS.ViewControllers
{
	[Register ("NewDTCViewController")]
	partial class NewDTCViewController
	{
		[Outlet]
		UIKit.UIButton btnAbs { get; set; }

		[Outlet]
		UIKit.UIButton btnBms { get; set; }

		[Outlet]
		UIKit.UIButton btnEngine { get; set; }

		[Outlet]
		UIKit.UIButton btnEps { get; set; }

		[Outlet]
		UIKit.UIButton btnEv { get; set; }

		[Outlet]
		UIKit.UIButton btnIcm { get; set; }

		[Outlet]
		UIKit.UIButton btnIsg { get; set; }

		[Outlet]
		UIKit.UIButton btnMcu { get; set; }

		[Outlet]
		UIKit.UIButton btnObd2 { get; set; }

		[Outlet]
		UIKit.UISegmentedControl segDtcType { get; set; }

		[Outlet]
		UIKit.UIStackView svEcuSystem { get; set; }

		[Outlet]
		UIKit.UITableView table { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (svEcuSystem != null) {
				svEcuSystem.Dispose ();
				svEcuSystem = null;
			}

			if (btnAbs != null) {
				btnAbs.Dispose ();
				btnAbs = null;
			}

			if (btnBms != null) {
				btnBms.Dispose ();
				btnBms = null;
			}

			if (btnEngine != null) {
				btnEngine.Dispose ();
				btnEngine = null;
			}

			if (btnEps != null) {
				btnEps.Dispose ();
				btnEps = null;
			}

			if (btnEv != null) {
				btnEv.Dispose ();
				btnEv = null;
			}

			if (btnIcm != null) {
				btnIcm.Dispose ();
				btnIcm = null;
			}

			if (btnIsg != null) {
				btnIsg.Dispose ();
				btnIsg = null;
			}

			if (btnMcu != null) {
				btnMcu.Dispose ();
				btnMcu = null;
			}

			if (btnObd2 != null) {
				btnObd2.Dispose ();
				btnObd2 = null;
			}

			if (segDtcType != null) {
				segDtcType.Dispose ();
				segDtcType = null;
			}

			if (table != null) {
				table.Dispose ();
				table = null;
			}
		}
	}
}

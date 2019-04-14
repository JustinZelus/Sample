using System;
using System.Collections.Generic;
using UIKit;

namespace Xamarin_IPE_IOS
{
	public struct LabelReslID
	{
		public const string LeftTop = "labelLeftTop";
		public const string RightTop = "labelRightTop";
		public const string LeftBottom = "labelLeftBottom";
		public const string RightBottom = "labelRightBottom";
		public static string position;
	}

	// This Class show menu of live data that make user to choice;
	// 4 position Live data can choice repeatably.
	// Send cmd by statemachine when click btnOK.
	// Set the support LiveData in initial period by CheckSupportForBtns()
	public partial class BtnsMenuController : UIViewController
	{


		protected BtnsMenuController(IntPtr handle) : base(handle)
		{

		}

		public Dictionary<string, string> btnDisableBG = new Dictionary<string, string> {
				{"btn_clv", "img_clv_disable" },
				{"btn_ect","img_ect_disable"},
				{"btn_frp","img_frp_disable"},
				{"btn_map","img_map_disable"},
				{"btn_rpm","img_rpm_disable"},
				{"btn_vss","img_vss_disable"},
				{"btn_ita","img_ita_disable"},
				{"btn_iat","img_iat_disable"},
				{"btn_afm","img_afm_disable"},
				{"btn_tps","img_tps_disable"},
				{"btn_oov","img_oov_disable"},
				{"btn_stft","img_stft_disable"},
				{"btn_er","img_er_disable"},
				{"btn_ctb","img_ctb_disable"}};
		public Dictionary<string, string> btnEnableBG = new Dictionary<string, string> {
				{"btn_clv", "img_clv_enable" },
				{"btn_ect","img_ect_enable"},
				{"btn_frp","img_frp_enable"},
				{"btn_map","img_map_enable"},
				{"btn_rpm","img_rpm_enable"},
				{"btn_vss","img_vss_enable"},
				{"btn_ita","img_ita_enable"},
				{"btn_iat","img_iat_enable"},
				{"btn_afm","img_afm_enable"},
				{"btn_tps","img_tps_enable"},
				{"btn_oov","img_oov_enable"},
				{"btn_stft","img_stft_enable"},
				{"btn_er","img_er_enable"},
				{"btn_ctb","img_ctb_enable"}};

		public Dictionary<string, string> btnPressedBG = new Dictionary<string, string> {
				{"btn_clv", "img_clv_pressed" },
				{"btn_ect","img_ect_pressed"},
				{"btn_frp","img_frp_pressed"},
				{"btn_map","img_map_pressed"},
				{"btn_rpm","img_rpm_pressed"},
				{"btn_vss","img_vss_pressed"},
				{"btn_ita","img_ita_pressed"},
				{"btn_iat","img_iat_pressed"},
				{"btn_afm","img_afm_pressed"},
				{"btn_tps","img_tps_pressed"},
				{"btn_oov","img_oov_pressed"},
				{"btn_stft","img_stft_pressed"},
				{"btn_er","img_er_pressed"},
				{"btn_ctb","img_ctb_pressed"}};

		public Dictionary<int, UIButton> btnLiveDataID = new Dictionary<int, UIButton>();
		private Dictionary<string, int> btnBox = new Dictionary<string, int>();


		private UIButton currentBtn;

		private void Btn_Group_TouchUpInside(object sender, EventArgs e)
		{

			var selectedBtn = (UIButton)sender;

			var res = selectedBtn.RestorationIdentifier;
			var selectID = btnBox[res];

			if (selectedBtn.Equals(currentBtn))
			{
				return;
			}
			else
			{
				if (currentBtn.Selected)
				{
					currentBtn.SetBackgroundImage(UIImage.FromFile(btnEnableBG[currentBtn.RestorationIdentifier])
												  , UIControlState.Normal);
					currentBtn.Selected = false;
				}

				if (!selectedBtn.Selected)
				{
					selectedBtn.SetBackgroundImage(UIImage.FromFile(btnPressedBG[selectedBtn.RestorationIdentifier])
												  , UIControlState.Normal);
					selectedBtn.Selected = true;
				}

				currentBtn = selectedBtn;
			}


			switch (LabelReslID.position)
			{
				case LabelReslID.LeftTop:
					LiveDataViewController.Instance.LeftTop = selectID;
					break;
				case LabelReslID.RightTop:
					LiveDataViewController.Instance.RightTop = selectID;
					break;
				case LabelReslID.LeftBottom:
					LiveDataViewController.Instance.LeftBottom = selectID;
					break;
				case LabelReslID.RightBottom:
					LiveDataViewController.Instance.RightBottom = selectID;
					break;
			}



		}



		private void BtnSend_TouchUpInside(object sender, EventArgs e)
		{
			//Console.WriteLine("send");


			if (StateMachine.IsActivted)
			{
				StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV_STOP);
				StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV);
			}

			ContainerViewController.Instance.PerformSegue("embedLiveData", null);

		}


		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			InitBtnEvent();
			InitBtnDefaultLiveDataID();
			InitBtnDefaultState();

			CheckSupportForBtns();

		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			switch (LabelReslID.position)
			{
				case LabelReslID.LeftTop:
					//one HighLight, others Disable

					SetDisable(LiveDataViewController.Instance.RightTop);
					SetDisable(LiveDataViewController.Instance.LeftBottom);
					SetDisable(LiveDataViewController.Instance.RightBottom);
					SetHightLight(LiveDataViewController.Instance.LeftTop);
					break;
				case LabelReslID.RightTop:

					SetDisable(LiveDataViewController.Instance.LeftTop);
					SetDisable(LiveDataViewController.Instance.LeftBottom);
					SetDisable(LiveDataViewController.Instance.RightBottom);
					SetHightLight(LiveDataViewController.Instance.RightTop);
					break;
				case LabelReslID.LeftBottom:

					SetDisable(LiveDataViewController.Instance.RightTop);
					SetDisable(LiveDataViewController.Instance.LeftTop);
					SetDisable(LiveDataViewController.Instance.RightBottom);
					SetHightLight(LiveDataViewController.Instance.LeftBottom);
					break;
				case LabelReslID.RightBottom:

					SetDisable(LiveDataViewController.Instance.RightTop);
					SetDisable(LiveDataViewController.Instance.LeftBottom);
					SetDisable(LiveDataViewController.Instance.LeftTop);
					SetHightLight(LiveDataViewController.Instance.RightBottom);
					break;
			}

		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			Console.WriteLine("BtnMenuController: " + "ViewDidAppear()");




		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);
			Console.WriteLine("BtnMenuController: " + "ViewDidDisappear()");



		}

		private void SetHightLight(int id)
		{

			string res = btnLiveDataID[id].RestorationIdentifier;
			currentBtn = btnLiveDataID[id];
			btnLiveDataID[id].SetBackgroundImage(UIImage.FromFile(btnPressedBG[res]), UIControlState.Normal);
			btnLiveDataID[id].Selected = true;

		}

		private void SetDisable(int id)
		{
			string res = btnLiveDataID[id].RestorationIdentifier;
			btnLiveDataID[id].SetBackgroundImage(UIImage.FromFile(btnEnableBG[res]), UIControlState.Normal);
			btnLiveDataID[id].Selected = false;
		}

		/// <summary>
		/// Checks the support Live Data for btns.
		/// </summary>
		private void CheckSupportForBtns()
		{
			//get support ID 
			var supprtID = LiveDataViewController.Instance.GetSupportLiveDataID();
			foreach (int id in supprtID)
			{
				if (btnLiveDataID.ContainsKey(id))
				{
					btnLiveDataID[id].SetBackgroundImage(UIImage.FromFile(btnEnableBG[btnLiveDataID[id].RestorationIdentifier]), UIControlState.Normal);
					btnLiveDataID[id].Enabled = true;
				}
			}

		}

		private void InitBtnDefaultState()
		{
			btn_clv.SetBackgroundImage(UIImage.FromFile(btnDisableBG["btn_clv"]), UIControlState.Normal);
			btn_ect.SetBackgroundImage(UIImage.FromFile(btnDisableBG["btn_ect"]), UIControlState.Normal);
			btn_frp.SetBackgroundImage(UIImage.FromFile(btnDisableBG["btn_frp"]), UIControlState.Normal);
			btn_map.SetBackgroundImage(UIImage.FromFile(btnDisableBG["btn_map"]), UIControlState.Normal);
			btn_rpm.SetBackgroundImage(UIImage.FromFile(btnDisableBG["btn_rpm"]), UIControlState.Normal);
			btn_vss.SetBackgroundImage(UIImage.FromFile(btnDisableBG["btn_vss"]), UIControlState.Normal);
			btn_ita.SetBackgroundImage(UIImage.FromFile(btnDisableBG["btn_ita"]), UIControlState.Normal);
			btn_iat.SetBackgroundImage(UIImage.FromFile(btnDisableBG["btn_iat"]), UIControlState.Normal);
			btn_afm.SetBackgroundImage(UIImage.FromFile(btnDisableBG["btn_afm"]), UIControlState.Normal);
			btn_tps.SetBackgroundImage(UIImage.FromFile(btnDisableBG["btn_tps"]), UIControlState.Normal);
			btn_oov.SetBackgroundImage(UIImage.FromFile(btnDisableBG["btn_oov"]), UIControlState.Normal);
			btn_stft.SetBackgroundImage(UIImage.FromFile(btnDisableBG["btn_stft"]), UIControlState.Normal);
			btn_er.SetBackgroundImage(UIImage.FromFile(btnDisableBG["btn_er"]), UIControlState.Normal);
			btn_ctb.SetBackgroundImage(UIImage.FromFile(btnDisableBG["btn_ctb"]), UIControlState.Normal);

			btn_clv.Enabled = false;
			btn_ect.Enabled = false;
			btn_frp.Enabled = false;
			btn_map.Enabled = false;
			btn_rpm.Enabled = false;
			btn_vss.Enabled = false;
			btn_ita.Enabled = false;
			btn_iat.Enabled = false;
			btn_afm.Enabled = false;
			btn_tps.Enabled = false;
			btn_oov.Enabled = false;
			btn_stft.Enabled = false;
			btn_er.Enabled = false;
			btn_ctb.Enabled = false;

		}

		private void InitBtnDefaultLiveDataID()
		{
			btnLiveDataID.Add(1, btn_clv);
			btnLiveDataID.Add(2, btn_ect);
			btnLiveDataID.Add(3, btn_frp);
			btnLiveDataID.Add(4, btn_map);
			btnLiveDataID.Add(5, btn_rpm);
			btnLiveDataID.Add(6, btn_vss);
			btnLiveDataID.Add(7, btn_ita);
			btnLiveDataID.Add(8, btn_iat);
			btnLiveDataID.Add(9, btn_afm);
			btnLiveDataID.Add(10, btn_tps);
			btnLiveDataID.Add(11, btn_oov);
			btnLiveDataID.Add(12, btn_stft);
			btnLiveDataID.Add(13, btn_er);
			btnLiveDataID.Add(14, btn_ctb);

			btnBox.Add("btn_clv", 1);
			btnBox.Add("btn_ect", 2);
			btnBox.Add("btn_frp", 3);
			btnBox.Add("btn_map", 4);
			btnBox.Add("btn_rpm", 5);
			btnBox.Add("btn_vss", 6);
			btnBox.Add("btn_ita", 7);
			btnBox.Add("btn_iat", 8);
			btnBox.Add("btn_afm", 9);
			btnBox.Add("btn_tps", 10);
			btnBox.Add("btn_oov", 11);
			btnBox.Add("btn_stft", 12);
			btnBox.Add("btn_er", 13);
			btnBox.Add("btn_ctb", 14);
		}
		private void InitBtnEvent()
		{
			//btnOK.TouchUpInside += BtnOK_TouchUpInside;
			btnSend.TouchUpInside += BtnSend_TouchUpInside;

			btn_clv.TouchUpInside += Btn_Group_TouchUpInside;
			btn_ect.TouchUpInside += Btn_Group_TouchUpInside;
			btn_frp.TouchUpInside += Btn_Group_TouchUpInside;
			btn_map.TouchUpInside += Btn_Group_TouchUpInside;
			btn_rpm.TouchUpInside += Btn_Group_TouchUpInside;
			btn_vss.TouchUpInside += Btn_Group_TouchUpInside;
			btn_ita.TouchUpInside += Btn_Group_TouchUpInside;
			btn_iat.TouchUpInside += Btn_Group_TouchUpInside;
			btn_afm.TouchUpInside += Btn_Group_TouchUpInside;
			btn_tps.TouchUpInside += Btn_Group_TouchUpInside;
			btn_oov.TouchUpInside += Btn_Group_TouchUpInside;
			btn_stft.TouchUpInside += Btn_Group_TouchUpInside;
			btn_er.TouchUpInside += Btn_Group_TouchUpInside;
			btn_ctb.TouchUpInside += Btn_Group_TouchUpInside;


		}
		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}


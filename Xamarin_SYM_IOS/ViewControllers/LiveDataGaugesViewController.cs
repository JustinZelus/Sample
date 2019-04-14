using System;
using System.Collections.Generic;
using System.Drawing;
using CoreGraphics;
using Foundation;
using IcmLib.Data;
using Steema.TeeChart.Styles;
using UIKit;

namespace Xamarin_SYM_IOS
{
	public partial class LiveDataGaugesViewController : CustomViewController, IUITableViewDataSource, IUITableViewDelegate
	{
		public static LiveDataGaugesViewController Instance;
		private List<DmData> datas = new List<DmData>();
		private int indexOfGauges; //預設0是RPM gauge
		public bool isChangeGauged = false;
		private string gaugeName = "Engine revolution speed";
		public  CircularGauge gauge;
		public int DM_ID;
		public float divideVal;
		public double maxVal;


		private Dictionary<string, string> nameDic = new Dictionary<string, string>
		{
			{"Engine revolution speed","Engine revolution speed"}
		};

		public interface BtnOkCallBack
		{
			 void DynamicChangeGauge(DmData data);
			 void RestoreView();
		}


		void BtnOK_TouchUpInside(object sender, EventArgs e)
		{	
			Console.WriteLine("準備結束Gauges選單之前");


			//設id
			if (datas.Count > 0)
			{
				DM_ID = (int)datas[indexOfGauges].ID;
				//設要除的值
				divideVal = (float)datas[indexOfGauges].MultipleRate;
				maxVal = (double)datas[indexOfGauges].MaxValue;
			}
			//送指令
			if (StateMachine.IsActivted)
			{
				StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV_STOP);
				StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV);
			}


			Console.WriteLine("開始移除選單,錶,單位");
		
			if (isChangeGauged)
			{
				
				Console.WriteLine("改錶");
				LiveDataViewController.Instance.RestoreView();
				LiveDataViewController.Instance.DynamicChangeGauge(datas[indexOfGauges]);
			}

			this.ParentViewController.View.Subviews[5].Alpha = 1;
			Console.WriteLine("移除透明背景");
			this.ParentViewController.View.Subviews[6].RemoveFromSuperview();


			this.WillMoveToParentViewController(null);
			this.View.RemoveFromSuperview();
			this.RemoveFromParentViewController();
		}

		private UILabel UnitView()
		{ 
			var unitText = new UILabel();
			unitText.Text = datas[indexOfGauges].ChartDisplayText;
			unitText.TextColor = UIColor.Gray;
			unitText.Font = UIFont.SystemFontOfSize(18);
			unitText.Frame = new CGRect(180, 396, 58, 55);
			unitText.TextAlignment = UITextAlignment.Center;
			unitText.BackgroundColor = UIColor.FromRGBA(255, 255, 255, 0);

			return unitText;
		}

		private void SampleData()
		{ 
			
				datas.Add(new DmData() { Name = "0xdddd" });
				datas.Add(new DmData() { Name = "0xffffff" });
				datas.Add(new DmData() { Name = "0xeeeeeeee" });
				datas.Add(new DmData() { Name = "0xiiiiiiiii" });
				datas.Add(new DmData() { Name = "@@@@@@@@" });
				datas.Add(new DmData() { Name = "abcdefghijk" });
				datas.Add(new DmData() { Name = "091234567" });
				datas.Add(new DmData() { Name = "55AAEERRTTHHJJ" });
				datas.Add(new DmData() { Name = "0x90909090" });
				datas.Add(new DmData() { Name = "0xWWWWWWWWWWW" });
				datas.Add(new DmData() { Name = "BMWBMWBMWBMW" });
				datas.Add(new DmData() { Name = "BENZBENZBENZ" });
				datas.Add(new DmData() { Name = "porscheporscheporsche" });

		}

		protected LiveDataGaugesViewController(IntPtr handle) : base(handle)
		{
		}

		//------------------------------------生命週期分隔線---------------------------------------

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			

			IsInited = true;
			Instance = this;
			btnOK.TouchUpInside += BtnOK_TouchUpInside;

			Console.WriteLine("拿取gauges要用的值");
			if(StateMachine.IsActivted)
				datas = StateMachine.DataModel.CurrentLvDatasForGauge;

			//如果沒有塞測試資料
			if (datas.Count == 0)
				SampleData();

			//if(datas.Count >1)
				//Console.WriteLine("成功拿取!");
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			Console.WriteLine("LiveDataGaugesViewController: " + "ViewWillAppear()");

		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			Console.WriteLine("LiveDataItemsViewController: " + "ViewDidAppear()");
			Console.WriteLine("叫出Gauge選單(TableView)之後");
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			Console.WriteLine("LiveDataGaugesViewController: " + "ViewWillDisappear()");
		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);
			Console.WriteLine("LiveDataGaugesViewController: " + "ViewDidDisappear()");
			Console.WriteLine("已跳出Guage選單");
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();

		}

		//====================實作DataSource,Delegate的方法=====================
		[Export("tableView:willSelectRowAtIndexPath:")]
		public NSIndexPath WillSelectRow(UITableView tableView, NSIndexPath indexPath)
		{	

			//selectedCell.Selected = false;
			return indexPath;
		}

		[Export("tableView:didSelectRowAtIndexPath:")]
		public void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			//selectedCell = tableView.CellAt(indexPath);
			//selectedCell.Selected = true;

			var cell = tableView.CellAt(indexPath);
			indexOfGauges = indexPath.Row;
			gaugeName = cell.TextLabel.Text;


			if (nameDic.ContainsKey(gaugeName))
			{
				if (gaugeName != LiveDataViewController.CurrentGauge)
					isChangeGauged = true;
			}
			


			Console.WriteLine("您選擇了 : " + gaugeName);

		}

		[Export("tableView:willDisplayCell:forRowAtIndexPath:")]
		public void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
		{	
	
			//if (indexPath.Row == indexOfGauges)
			//{
			//	cell.SetSelected(true, false);
			//	selectedCell = cell;
			//}
		}

		[Export("tableView:heightForRowAtIndexPath:")]
		public nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			nfloat heightForRow = Main.PHONE_SIZE_TABLE_CELLHEIGHT;
			return heightForRow;
		}



		public nint RowsInSection(UITableView tableView, nint section)
		{
			if (datas.Count != 0)
				return datas.Count;
			else
				return 0;
		}

		public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell("Cell");

			if(datas.Count > 0)
				cell.TextLabel.Text = datas[indexPath.Row].Name;

			cell.SelectionStyle = UITableViewCellSelectionStyle.Gray;
			cell.TextLabel.Font = UIFont.SystemFontOfSize(Main.PHONE_SIZE_TABLE_CONTENT);

			if (cell.TextLabel.Text.Equals(gaugeName))
			{
				indexOfGauges = indexPath.Row;
			}

			return cell;
		}

		public override void setCurrentPageName()
		{
			throw new NotImplementedException();
		}
	}
}


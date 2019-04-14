using System;
using Foundation;
using UIKit;
using System.Collections.Generic;
using System.Linq;

namespace Xamarin_SYM_IOS
{	


	public partial class LiveDataItemsViewController : CustomViewController,IUITableViewDataSource,IUITableViewDelegate
	{

		private List<string> liveDataItems = new List<string>();
		public LiveDataItemsViewController Instance;
		public int[] labelsSelectedIndex = { 0, 1, 2, 3 }; //索引0,1,2,3分別是 左上, 右上, 左下, 右下
		private UITableViewCell currentSelectedCell;
		private bool isNeedReloadData = false; //第一次預設不用reloaddata



		//.......................Button功能..........................
		void BtnOkPressed(object sender, EventArgs e)
		{
			//測試
			//MyCustomTeeChart.RunUIWithoutCommunication.isClickDialog = false;

			//按下去之後刷新整個table旗標
			//也就是第二次點選,在table show出來之前會reloaddata一次
			isNeedReloadData = true;
			LiveDataViewController.Instance.selectedPositions = labelsSelectedIndex;
			LiveDataViewController.Instance.RefreshTitles(labelsSelectedIndex);

			//送指令
			if (StateMachine.IsActivted)
			{
				StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV_STOP);
				StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV);
			}

			int count = ContainerViewController.Instance.View.Subviews.Length;
			for (int i = 0; i < count; i++)
			{
				//恢復liveData的view透明度
				if (this.ParentViewController.View.Subviews[i].RestorationIdentifier == "LiveDataView")
					this.ParentViewController.View.Subviews[i].Alpha = 1;
				//移除table
					else if (this.ParentViewController.View.Subviews[i].RestorationIdentifier == "LiveDataItemsView")
					{
						this.ParentViewController.View.Subviews[6].RemoveFromSuperview();
						// Notify Child View Controller
						this.WillMoveToParentViewController(null);
						// Remove self From Superview
						this.View.RemoveFromSuperview();

						// Notify Child View Controller
						this.RemoveFromParentViewController();
					}
			}



		}


		//||||||||||||||||||||||||||||||||||||||自定義功能||||||||||||||||||||||||||||||||||||



		//====================實作DataSource,Delegate的方法========
		public  nint RowsInSection(UITableView tableView, nint section)
		{	
			return LiveDataViewController.Instance.liveDataItems.Count;
		}

		[Export("tableView:willSelectRowAtIndexPath:")]
		public NSIndexPath WillSelectRow(UITableView tableView, NSIndexPath indexPath)
		{
			//要選之前把前一個選的cell高亮取消
			currentSelectedCell.Selected = false;
			return indexPath;
		}

		[Export("tableView:didSelectRowAtIndexPath:")]
		public void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			//把選的cell高亮,並改變目索引
			currentSelectedCell = tableView.CellAt(indexPath);
			currentSelectedCell.Selected = true;

			switch (LiveDataViewController.currentSelectedLabel)
			{ 
				case LabelReslID.LeftTop:
					labelsSelectedIndex[0] = indexPath.Row;
					break;
				case LabelReslID.RightTop:
					labelsSelectedIndex[1] = indexPath.Row;
					break;	
				case LabelReslID.LeftBottom:
					labelsSelectedIndex[2] = indexPath.Row;
					break;
				case LabelReslID.RightBottom:
					labelsSelectedIndex[3] = indexPath.Row;
					break;	
			}

		}

		[Export("tableView:willDisplayCell:forRowAtIndexPath:")]
		public void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
		{
			//為了滑動時,讓之前選過的cell背景顏色要存在,不能讓它跳掉,所以複寫willDisplay
			switch (LiveDataViewController.currentSelectedLabel)
			{ 
				case LabelReslID.LeftTop:
					if (indexPath.Row == labelsSelectedIndex[0])
					{ 
						cell.SetSelected(true, false);
						currentSelectedCell = cell;
					}
					break;
				case LabelReslID.RightTop:
					if (indexPath.Row == labelsSelectedIndex[1])
					{
						cell.SetSelected(true, false);
						currentSelectedCell = cell;
					}
					break;
				case LabelReslID.LeftBottom:
					if (indexPath.Row == labelsSelectedIndex[2])
					{
						cell.SetSelected(true, false);
						currentSelectedCell = cell;
					}
					break;
				case LabelReslID.RightBottom:
					if (indexPath.Row == labelsSelectedIndex[3])
					{
						cell.SetSelected(true, false);
						currentSelectedCell = cell;
					}
					break;	
			}
		}

		public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell("Cell");

			//cell.ContentView.Layer.CornerRadius = 7;
			//cell.ContentView.Layer.MasksToBounds = true;
			cell.TextLabel.Font = UIFont.SystemFontOfSize(Main.PHONE_SIZE_TABLE_CONTENT);

				switch (LiveDataViewController.currentSelectedLabel)
				{ 
					case LabelReslID.LeftTop:
						//cell.TextLabel.Text = indexPath.Row + ". " + liveDataItems[indexPath.Row];
						cell.TextLabel.Text = liveDataItems[indexPath.Row];

						//其它label選過的要把它隱藏
						if ((indexPath.Row == labelsSelectedIndex[1]) || (indexPath.Row == labelsSelectedIndex[2])
							    || (indexPath.Row == labelsSelectedIndex[3]))
						{
							cell.Hidden = true;
						}
						break;
					case LabelReslID.RightTop:
						cell.TextLabel.Text = liveDataItems[indexPath.Row];
						if ((indexPath.Row == labelsSelectedIndex[0]) || (indexPath.Row == labelsSelectedIndex[2])
							|| (indexPath.Row == labelsSelectedIndex[3]))
						{
							cell.Hidden = true;
						}
						break;
					case LabelReslID.LeftBottom:
						cell.TextLabel.Text =  liveDataItems[indexPath.Row];
						if ((indexPath.Row == labelsSelectedIndex[0]) || (indexPath.Row == labelsSelectedIndex[1])
							|| (indexPath.Row == labelsSelectedIndex[3]))
						{
							cell.Hidden = true;
						}
						break;
				case LabelReslID.RightBottom:
						cell.TextLabel.Text = liveDataItems[indexPath.Row];
						if ((indexPath.Row == labelsSelectedIndex[0]) || (indexPath.Row == labelsSelectedIndex[1])
							|| (indexPath.Row == labelsSelectedIndex[2]))
						{
							cell.Hidden = true;
						}
						break;
				}

			return cell;
		}

		[Export("tableView:heightForRowAtIndexPath:")]
		public nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			nfloat heightForRow = Main.PHONE_SIZE_TABLE_CELLHEIGHT;

			switch (LiveDataViewController.currentSelectedLabel)
			{ 
				case LabelReslID.LeftTop:
					if (indexPath.Row == labelsSelectedIndex[1] || indexPath.Row == labelsSelectedIndex[2]
						|| indexPath.Row == labelsSelectedIndex[3])
					{
						heightForRow = 0;
					}
					else
					{
						heightForRow = Main.PHONE_SIZE_TABLE_CELLHEIGHT;
					}
					break;
				case LabelReslID.RightTop:
					if (indexPath.Row == labelsSelectedIndex[0] || indexPath.Row == labelsSelectedIndex[2]
						|| indexPath.Row == labelsSelectedIndex[3])
					{
						heightForRow = 0;
					}
					else
					{
						heightForRow = Main.PHONE_SIZE_TABLE_CELLHEIGHT;
					}
					break;	
				case LabelReslID.LeftBottom:
					if (indexPath.Row == labelsSelectedIndex[0] || indexPath.Row == labelsSelectedIndex[1]
						|| indexPath.Row == labelsSelectedIndex[3])
					{
						heightForRow = 0;
					}
					else
					{
						heightForRow = Main.PHONE_SIZE_TABLE_CELLHEIGHT;
					}
					break;	
				case LabelReslID.RightBottom:
					if (indexPath.Row == labelsSelectedIndex[0] || indexPath.Row == labelsSelectedIndex[1]
						|| indexPath.Row == labelsSelectedIndex[2])
					{
						heightForRow = 0;
					}
					else
					{
						heightForRow = Main.PHONE_SIZE_TABLE_CELLHEIGHT;
					}
					break;	
			}
			return heightForRow;
		}

		private void SampleData()
		{
			for (int i = 0; i < 30; i++)
			{
				liveDataItems.Add("You're coming back down and you really don't mind");
			}

		}

		//------------------------------------生命週期分隔線---------------------------------------

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			Instance = this;

			btnOK.TouchUpInside += BtnOkPressed;


			//去livedata頁面拿資料
			liveDataItems = LiveDataViewController.Instance.RefreshLiveDataItems(liveDataItems);

			//如果沒有塞測試資料
			if (liveDataItems.Count == 0)
				SampleData();


		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			Console.WriteLine("LiveDataItemsViewController: " + "ViewWillAppear()");


			//tableView需要更新
			if (isNeedReloadData)
			{
				if (liveDataItemsTable != null) 
				{ 
					liveDataItemsTable.ReloadData();
				}

			}
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			Console.WriteLine("LiveDataItemsViewController: " + "ViewDidAppear()");

		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			Console.WriteLine("LiveDataItemsViewController: " + "ViewWillDisappear()");
		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);
			Console.WriteLine("LiveDataItemsViewController: " + "ViewDidDisappear()");
			//foreach (int index in labelsSelectedIndex) {
			//	Console.WriteLine("index:"+ index);
			//}
			setCurrentPageName();
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}

		public override void setCurrentPageName()
		{
			currentPageView = "LiveDataItemsView";
			//Console.WriteLine("current page is " + currentPageView);
		}

		protected LiveDataItemsViewController(IntPtr handle) : base(handle)
		{
		}

		//void LiveDataItemsViewController_UpdateTimerValue()
		//{

		//}
	}
}


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using ToastIOS;
using UIKit;

namespace Xamarin_SYM_IOS.ViewControllers
{
    public partial class LvDataCloudViewController : UIViewController,IUITableViewDataSource,IUITableViewDelegate
    {

        private Boolean isChoiceAllItems = false;
        private List<string> itemNames = new List<string>();
        private Dictionary<int,string> mChoiceItems = new Dictionary<int,string>();
        private Dictionary<int, bool> cellChoiced = new Dictionary<int, bool>();
        private static int _MAX = 16;


        protected LvDataCloudViewController(IntPtr handle) : base(handle)
        {

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            tblList.DataSource = this;
            tblList.Delegate = this;


            btnChoiceAllItems.TouchUpInside += BtnChoiceAllItems_Click;
            btnCancelAllItems.TouchUpInside += BtnCancelAllItems_Click;
            btnSendLv.TouchUpInside += BtnSendLv_Click;

            if(StateMachine.IsActivted)
                itemNames = StateMachine.DataModel.AllEcuLvNames;
            //_MAX = itemNames.Count;

            //if (mChoiceItems.Count == 0)
            //{
            //    for (int i = 0; i < liveDataItems.Count; i++)
            //    {
            //        mChoiceItems.Add(i, liveDataItems[i]);
            //    }
            //}
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();

        }

        [Export("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            Console.WriteLine("Row selected : " + indexPath.Row);
            if (itemNames.Count <= 0) return;

            int row = indexPath.Row;

            if (!cellChoiced.ContainsKey(row))
            {
                //達到上限時返回
                if (cellChoiced.Count >= _MAX)
                {
                    string toastMessageId = _MAX + " " +  NSBundle.MainBundle.GetLocalizedString("lv_cloud_max_choice_reminding");
                    DispatchQueue.MainQueue.DispatchAsync(() =>
                    {   

                        Toast.MakeText(toastMessageId).Show();
                    });
                    return;
                } 

                cellChoiced.Add(row, true);
                var cell = tableView.CellAt(indexPath);
                cell.Accessory = UITableViewCellAccessory.Checkmark;

                //將資料加入
                if (!mChoiceItems.ContainsKey(row))
                {
                    mChoiceItems.Add(row, itemNames[row]);
                    Debug.WriteLine("加入 : " + itemNames[row]);
                }
            }
            else
            {
                cellChoiced.Remove(row);
                var cell = tableView.CellAt(indexPath);
                cell.Accessory = UITableViewCellAccessory.None;

                //將資料移除
                if (mChoiceItems.ContainsKey(row))
                    mChoiceItems.Remove(row);
                Debug.WriteLine("移除 : " + itemNames[row]);
            }

        }

        [Export("tableView:didDeselectRowAtIndexPath:")]
        public void RowDeselected(UITableView tableView, NSIndexPath indexPath)
        {
            Console.WriteLine("Row deselected : " + indexPath.Row);
            if (itemNames.Count <= 0) return;

            int row = indexPath.Row;
            if (cellChoiced.ContainsKey(row))
            {
                cellChoiced.Remove(row);
                var cell = tableView.CellAt(indexPath);
                cell.Accessory = UITableViewCellAccessory.None;

                //將資料移除
                if (mChoiceItems.ContainsKey(row))
                    mChoiceItems.Remove(row);
                Debug.WriteLine("移除 : " + itemNames[row]);
            }
        }

        [Export("tableView:willDisplayCell:forRowAtIndexPath:")]
        public void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {

        }

        public nint RowsInSection(UITableView tableView, nint section)
        {
            int count = itemNames.Count;

            return count > 0 ? count : 100;
        }


        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("CheckBoxCell", indexPath) as LvDataCloudTableViewCell;
            //var pathOfTheCell = tableView.IndexPathForCell(cell);
            //var rowOfTheCell = pathOfTheCell.Row;
            //Console.WriteLine("rowOfTheCell : " + rowOfTheCell);

            return SettingEachCellUIState(tableView,cell, indexPath.Row);
        }
        
        private UITableViewCell SettingEachCellUIState(UITableView tableView, LvDataCloudTableViewCell cell,int row)
        {
        
            var lbl = cell.Title;
            string itemName = "";
            //var lbl = (UILabel)cell.ContentView.ViewWithTag(1);
            if (itemNames != null  && itemNames.Count > 0 )
            {
                itemName = itemNames[row];
                lbl.Text = itemName;
            }

            else
                lbl.Text = "data receive error";

            if(cellChoiced.ContainsKey(row))
            {
                cell.Accessory = UITableViewCellAccessory.Checkmark;
            }
            else
            {
                cell.Accessory = UITableViewCellAccessory.None;
            }

            //客製化checkmark
            //var btnChk = cell.CheckBox; 
            //btnChk.Frame = new CGRect(0, 0, 36, 36);
            //btnChk.TouchUpInside -= CheckboxClicked;
            //btnChk.TouchUpInside += CheckboxClicked;
            //btnChk.Tag = row;

            //if (isChoiceAllItems)
            //{
            //    if (row < _MAX)
            //    {
            //        btnChk.Selected = true;
            //    }
            //    else
            //    {
            //        btnChk.Selected = false;
            //    }
            //}
            //else
            //{
            //    btnChk.Selected = false;
            //}


            return cell;
        }

        /// <summary>
        /// 按下Send Lv 按鈕之點擊事件響應
        /// 如果未選取項目，無法進入動態值即時訊息畫面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BtnSendLv_Click(object sender, EventArgs e)
        {
            if (GetCurrentChoiceItems().Count <= 0)
            {
                Toast.MakeText("Haven\'t selected any data monitor item.").Show();
                return;
            }

            //if (mChoiceItems.Count > 0)
            //{
            //    //foreach (string v in mChoiceItems.Values)
            //    //{
            //    //    Console.WriteLine("items : " + v);
            //    //}
            //    //foreach (string v in GetCurrentChoiceItems())
            //    //{
            //    //    Console.WriteLine("items : " + v);
            //    //}
            //    GetLvIdListFromLvItemNames();
            //}
            //else
                //Console.WriteLine("沒選");

            lock (this)
            {
                //GetCurrentChoiceItems();
                //IncreaseItem();
                string page = MyCustomPages.buttonRespondSegueID["DataMonitor"];
                GetLvIdListFromLvItemNames();
                StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV_STOP);
                Thread.Sleep(50);
                StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV);
                Thread.Sleep(50);
                StateMachine.UIModel.CurrentPage = Page.LvCloud_Show;
                ContainerViewController.Instance.PerformSegue(page, null);
            }

        }

        /// <summary>
        /// checkbox的事件，達到上限就不能再選
        /// </summary>
        void CheckboxClicked(object sender, EventArgs e)
        {

            var btnChk = (UIButton)sender;
            int row = (int)btnChk.Tag;

            if (!btnChk.Selected && mChoiceItems.Count >= _MAX) return;

            bool isSelected = !btnChk.Selected;

            btnChk.Selected = isSelected;

            if(isSelected) 
            {
                if(!mChoiceItems.ContainsKey(row)){
                    mChoiceItems.Add(row, itemNames[row]);
                    Debug.WriteLine("加入 : " + itemNames[row]);
                }
            }
            else
            {
                if(mChoiceItems.ContainsKey(row))
                    mChoiceItems.Remove(row);
                Debug.WriteLine("移除 : " + itemNames[row]);
            }
        }

        //取消全選按鈕
        private void BtnCancelAllItems_Click(object sender, EventArgs e)
        {
            CancelAllCheckBoxItems();
        }

        //點擊全選按鈕
        private void BtnChoiceAllItems_Click(object sender, EventArgs e)
        {
            ChoiceAllCheckBoxItems();
        }

        /// <summary>
        /// 填充當前的Lv項目名稱與更新列表
        /// </summary>
        /// <param name="pItemNames">要顯示的項目名稱，如果為空，預設抓取所有當前Ecu支援的Lv項目</param>
        public void RefreshListView(List<String> pItemNames = null)
        {
            //print all lv ids
            var ecuIds = StateMachine.DataModel.AllEcuLvIds;
            Debug.WriteLine("ECU ID :");
            ecuIds.ForEach((id) => { Console.WriteLine("{0},", id); });

            if (this.itemNames == null)
                this.itemNames = StateMachine.DataModel.AllEcuLvNames;
            else
                this.itemNames = pItemNames;

            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                this.tblList.ReloadData();
            });
        }

        /// <summary>
        /// 儲存當前選擇的Lv Items轉換為LV IDs的列表
        /// </summary>
        private List<int> lvIdsByNames = new List<int>();
        public List<int> CurrentSendLvItemsForLvCloud
        {
            get { return lvIdsByNames; }
        }

        /// <summary>
        /// 藉由Lv Item Names獲取Lv Id列表
        /// </summary>
        private void GetLvIdListFromLvItemNames()
        {
            var currentChoinceLvNames = GetCurrentChoiceItems();
            //foreach (var item in currentChoinceLvNames)
            //{
            //    System.Diagnostics.Debug.WriteLine("LV Name {0} , ", item);
            //}

            lvIdsByNames.Clear();
            lvIdsByNames.AddRange(StateMachine.DataModel.AllEcuLvIdsByNames(currentChoinceLvNames));
            System.Diagnostics.Debug.WriteLine("Current selected Lv Id");
            foreach (var item in lvIdsByNames)
            {
                System.Diagnostics.Debug.WriteLine("LV ID {0} , ", item);
            }
        }

        /// <summary>
        /// 將全部選項勾選
        /// </summary>
        private void ChoiceAllCheckBoxItems()
        {
            if (tblList == null)
                return;
            //isChoiceAllItems = true;

            //先清除
            if(mChoiceItems != null)
               mChoiceItems.Clear();
            if(cellChoiced != null)
                cellChoiced.Clear();

            if (_MAX < itemNames.Count)
            {
                for (int i = 0; i < _MAX; i++)
                {
                    if (!mChoiceItems.ContainsKey(i))
                        mChoiceItems.Add(i, itemNames[i]);
                    if (!cellChoiced.ContainsKey(i))
                        cellChoiced.Add(i, true);
                }
            }
            else
            {
                for (int i = 0; i < itemNames.Count; i++)
                {
                    if (!mChoiceItems.ContainsKey(i))
                        mChoiceItems.Add(i, itemNames[i]);
                    if (!cellChoiced.ContainsKey(i))
                        cellChoiced.Add(i, true);
                }
            }

            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                this.tblList.ReloadData();
            });
        }

        /// <summary>
        /// 將全部選項取消勾選
        /// </summary>
        private void CancelAllCheckBoxItems()
        {
            if (tblList == null)
                return;
            //isChoiceAllItems = false;

            //for (int i = 0; i < _MAX; i++)
            //{
            //    if (mChoiceItems.ContainsKey(i))
            //        mChoiceItems.Remove(i);
            //}
            if (mChoiceItems != null)
                mChoiceItems.Clear();
            if (cellChoiced != null)
                cellChoiced.Clear();


            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                this.tblList.ReloadData();
            });
        }

        /// <summary>
        /// 獲取目前選取項目
        /// </summary>
        private List<string> GetCurrentChoiceItems()
        {
            List<string> result = new List<string>();

            if(mChoiceItems.Count > 0)
            {

                //listNumber = dicNumber.Select(kvp => kvp.Key).ToList();
                //listNumber = dicNumber.Keys.ToList();
                result = mChoiceItems.Select( v => v.Value).ToList();
            }

            return result;
        }


    }




}


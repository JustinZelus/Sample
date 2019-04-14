using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using CoreFoundation;
using Foundation;
using IcmLib.Data;
using UIKit;
using System.Threading;
using System.Linq;

namespace Xamarin_SYM_IOS
{
    public partial class DTCViewController : CustomViewController, IUITableViewDataSource, IUITableViewDelegate
    {
        public static DTCViewController Instance;
        private DtcData[] dtcObjects;
        //private List<string> titles = new List<string>();
        //private List<string> subTitles = new List<string>();
        //private IList<DtcData> tableItems = new List<DtcData>();

        private IList<DtcData> mList = new List<DtcData>();


        //Delay time for button click.
        private const int NSEC_PER_SEC = 1000000000;

        public Dictionary<int, int> dmReadinessState = new Dictionary<int, int>();
        public Dictionary<int, string> dmReadinessShortName = new Dictionary<int, string> {
            {103,"MIS"},{104,"FUE"},{105,"CCM"},
            {106,"CAT"},{107,"HCA"},{108,"EVA"},
            {109,"AIR"},{110,"ACR"},{111,"O2S"},
            {112,"HTR"},{113,"EGR"}
        };
        public Dictionary<string, Dictionary<int, UIImage>> dmBackGround = new Dictionary<string, Dictionary<int, UIImage>>();




        //==========================TableView====================================
        [Export("tableView:heightForRowAtIndexPath:")]
        public nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return Main.PHONE_SIZE_DTC_TABLE_CELLHEIGHT;
        }

        [Export("numberOfSectionsInTableView:")]
        public nint NumberOfSections(UITableView tableView)
        {
            //Console.WriteLine("NumberOfSections()");
            return 1;
        }

        public nint RowsInSection(UITableView tableView, nint section)
        {
            //Console.WriteLine("RowsInSection()");
            //return dtcCodes.Length;
            //return tableItems.Count;
            return mList.Count;
        }

        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            //Console.WriteLine("GetCell()");
            var cell = (DTCTableViewCell)tableView.DequeueReusableCell("Cell", indexPath);
            cell.Title.Text = mList[indexPath.Row].DtcCodeForDisplay;
            cell.SubTitle.Text = mList[indexPath.Row].DtcName;
            cell.Title.Font = UIFont.SystemFontOfSize(Main.PHONE_SIZE_TABLE_TITLE);
            cell.SubTitle.Font = UIFont.SystemFontOfSize(Main.PHONE_SIZE_TABLE_CONTENT);

            //cell.TitleLabel.Text = dtcCodes[indexPath.Row];
            //cell.SubTitleLabel.Text = dtcSubTitles[indexPath.Row];
            //cell.SelectedBackgroundView = bgColorView;
            //cell.ThumbnailImageView.Image = UIImage.FromFile(dtcImages[indexPath.Row]);
            return cell;
        }

        [Export("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            //Console.WriteLine("RowSelected()");

            return;

            //因要求暫時拿掉
            //currentImage = dtcImages[indexPath.Row];
            //currentDescription = dtcDescriptions[indexPath.Row];

            //if (containerViewController == null)
            //{
            //	containerViewController = (ContainerViewController)this.ParentViewController;
            //	containerViewController.PerformSegue(MyCustomPages.buttonRespondSegueID["DtcCell"], null);

            //}
            //else
            //{
            //	containerViewController.PerformSegue(MyCustomPages.buttonRespondSegueID["DtcCell"], null);
            //}

        }

        protected DTCViewController(IntPtr handle) : base(handle)
        {
        }


        //------------------------------------生命週期分隔線---------------------------------------



        private void InitDTC()
        {
            if (StateMachine.IsActivted)
            {
                dtcObjects = StateMachine.DataModel.DtcValues;
                if (dtcObjects != null)
                {
                    for (int i = 0; i < dtcObjects.Length; i++)
                    {
                        mList.Add(dtcObjects[i]);
                    }
                }
            }
        }

        private void InitDmBackGround()
        {

            dmBackGround.Add("AIR", new Dictionary<int, UIImage> {
                                    {0 ,ImageInitializer.imgAIR_1},
                                    {1, ImageInitializer.imgAIR_2}});
            dmBackGround.Add("BPS", new Dictionary<int, UIImage> {
                                    {0 ,ImageInitializer.imgBPS_1},
                                    {1, ImageInitializer.imgBPS_2}});
            dmBackGround.Add("CAT", new Dictionary<int, UIImage> {
									//{0 ,ImageInitializer.imgCAT_0},
									{0 ,ImageInitializer.imgCAT_1},
                                    {1, ImageInitializer.imgCAT_2}});
            dmBackGround.Add("CCM", new Dictionary<int, UIImage> {
									//{0 ,ImageInitializer.imgCCM_0},
									{0 ,ImageInitializer.imgCCM_1},
                                    {1, ImageInitializer.imgCCM_2}});
            dmBackGround.Add("DPF", new Dictionary<int, UIImage> {
									//{0 ,ImageInitializer.imgDPF_0},
									{0 ,ImageInitializer.imgDPF_1},
                                    {1, ImageInitializer.imgDPF_2}});
            dmBackGround.Add("EGR", new Dictionary<int, UIImage> {
									//{0 ,ImageInitializer.imgEGR_0},
									{0 ,ImageInitializer.imgEGR_1},
                                    {1, ImageInitializer.imgEGR_2}});
            dmBackGround.Add("EGS", new Dictionary<int, UIImage> {
									//{0 ,ImageInitializer.imgEGS_0},
									{0 ,ImageInitializer.imgEGS_1},
                                    {1, ImageInitializer.imgEGS_2}});
            dmBackGround.Add("EVA", new Dictionary<int, UIImage> {
									//{0 ,ImageInitializer.imgEVA_0},
									{0 ,ImageInitializer.imgEVA_1},
                                    {1, ImageInitializer.imgEVA_2}});
            dmBackGround.Add("FUE", new Dictionary<int, UIImage> {
									//{0 ,ImageInitializer.imgFUE_0},
									{0 ,ImageInitializer.imgFUE_1},
                                    {1, ImageInitializer.imgFUE_2}});
            dmBackGround.Add("HCA", new Dictionary<int, UIImage> {
									//{0 ,ImageInitializer.imgHCA_0},
									{0 ,ImageInitializer.imgHCA_1},
                                    {1, ImageInitializer.imgHCA_2}});
            dmBackGround.Add("HCC", new Dictionary<int, UIImage> {
									//{0 ,ImageInitializer.imgHCC_0},
									{0 ,ImageInitializer.imgHCC_1},
                                    {1, ImageInitializer.imgHCC_2}});
            dmBackGround.Add("HTR", new Dictionary<int, UIImage> {
									//{0 ,ImageInitializer.imgHTR_0},
									{0 ,ImageInitializer.imgHTR_1},
                                    {1, ImageInitializer.imgHTR_2}});
            dmBackGround.Add("MIS", new Dictionary<int, UIImage> {
									//{0 ,ImageInitializer.imgMIS_0},
									{0,ImageInitializer.imgMIS_1},
                                    {1, ImageInitializer.imgMIS_2}});
            dmBackGround.Add("NOX", new Dictionary<int, UIImage> {
									//{0 ,ImageInitializer.imgNOX_0},
									{0 ,ImageInitializer.imgNOX_1},
                                    {1, ImageInitializer.imgNOX_2}});
            dmBackGround.Add("O2S", new Dictionary<int, UIImage> {
									//{0 ,ImageInitializer.imgO2X_0},
									{0 ,ImageInitializer.imgO2X_1},
                                    {1, ImageInitializer.imgO2X_2}});
            dmBackGround.Add("ACR", new Dictionary<int, UIImage> {
									//{0 ,ImageInitializer.imgO2X_0},
									{0 ,ImageInitializer.imgACR_1},
                                    {1, ImageInitializer.imgACR_2}});
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Console.WriteLine("ViewDidLoad()");
            table.Delegate = this;
            table.DataSource = this;
            Instance = this;

            //InitDTC();
            InitDmBackGround();
            UpdateTimerValue += DTCViewController_UpdateTimerValue;

            //dtcTitle.Font = UIFont.SystemFontOfSize(Main.PHONE_SIZE_DTC_TITLE);
            btnClear.TouchUpInside += (sender, e) =>
            {
                //update UI

                mList.Clear();
                mList.Add(GetNoDtcData());
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    table.ReloadData();
                });


                btnClear.Enabled = false;
                btnClear.SetTitleColor(UIColor.DarkGray, UIControlState.Normal);

                if (StateMachine.Instance != null)
                {
                    //StateMachine.Instance.RemoveAllMessage();
                    StateMachine.Instance.RemoveAllSpecificMessage(StateMachineStatus.Communication_DTC);
                    StateMachine.Instance.SendMessage(StateMachineStatus.Communication_ClearDTC);
                    Thread.Sleep(50);
                    StateMachine.Instance.SendMessage(StateMachineStatus.Communication_DTC);
                    Thread.Sleep(50);

                }

                //tableItems.Clear();

                //2秒後才能再按
                DispatchQueue.MainQueue.DispatchAfter(new DispatchTime(DispatchTime.Now, 2 * NSEC_PER_SEC), () =>
                {
                    btnClear.SetTitleColor(UIColor.White, UIControlState.Normal);
                    btnClear.Enabled = true;
                });

            };


        }


        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            //Console.WriteLine("DTCViewController: " + "ViewWillAppear()");
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            Console.WriteLine("DTCViewController: " + "ViewDidAppear()");


            if (StateMachine.IsActivted)
            {
                StateMachine.DataModel.ClearDtcValues();
                StateMachine.Instance.RemoveAllMessage();
                StateMachine.Instance.SendMessage(StateMachineStatus.Communication_DTC);
                StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV_STOP);
                StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV);
                //Thread.Sleep(50);
            }

            IsInited = true;
            setCurrentPageName();

            if (!StateMachine.Instance.IsUseCommunicationMode)
            {
                mList.Clear();
                mList.Add(GetNoDtcData());
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    table.ReloadData();
                });
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            Console.WriteLine("DTCViewController: " + "ViewWillDisappear()");
            if (StateMachine.IsActivted)
                StateMachine.Instance.SendMessage(StateMachineStatus.Communication_LV_STOP);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            Console.WriteLine("DTCViewController: " + "ViewDidDisappear()");


        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public IList<DtcData> Value
        {
            set
            {
                mList.Clear();
                //int i = 0;
                foreach (var item in value)
                {
                    mList.Add(item);
                }

                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    table.ReloadData();
                });
            }

            get
            {
                return mList;
            }
        }


        public void SetDtcData(List<DtcData> data)
        {
            if (mList != null)
                mList.Clear();

            mList = data.ToList();

            if (mList.Count() > 0)
            {


                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    table.ReloadData();
                });
            }
        }

        public void SetNoDtcData()
        {
            if (mList != null)
                mList.Clear();

            mList.Add(GetNoDtcData());

            if (mList.Count() > 0)
            {
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    table.ReloadData();
                });
            }
        }

        /// <summary>
        /// Runtime Update Data Method
        /// </summary>
		void DTCViewController_UpdateTimerValue()
        {
            if (StateMachine.DataModel.LvValues.ContainsKey(103))
            {
                if (!dmReadinessState.ContainsKey(103))
                    dmReadinessState.Add(103, (int)StateMachine.DataModel.LvValues[103]);
                else
                {
                    dmReadinessState[103] = (int)StateMachine.DataModel.LvValues[103];
                    imgMIS.Image = dmBackGround["MIS"][dmReadinessState[103]];
                }

            }
            if (StateMachine.DataModel.LvValues.ContainsKey(104))
            {
                if (!dmReadinessState.ContainsKey(104))
                    dmReadinessState.Add(104, (int)StateMachine.DataModel.LvValues[104]);
                else
                {
                    dmReadinessState[104] = (int)StateMachine.DataModel.LvValues[104];
                    imgFUE.Image = dmBackGround["FUE"][dmReadinessState[104]];
                }
            }
            if (StateMachine.DataModel.LvValues.ContainsKey(105))
            {
                if (!dmReadinessState.ContainsKey(105))
                    dmReadinessState.Add(105, (int)StateMachine.DataModel.LvValues[105]);
                else
                {
                    dmReadinessState[105] = (int)StateMachine.DataModel.LvValues[105];
                    imgCCM.Image = dmBackGround["CCM"][dmReadinessState[105]];
                }
            }
            if (StateMachine.DataModel.LvValues.ContainsKey(106))
            {
                if (!dmReadinessState.ContainsKey(106))
                    dmReadinessState.Add(106, (int)StateMachine.DataModel.LvValues[106]);
                else
                {
                    dmReadinessState[106] = (int)StateMachine.DataModel.LvValues[106];
                    imgCAT.Image = dmBackGround["CAT"][dmReadinessState[106]];
                }
            }
            if (StateMachine.DataModel.LvValues.ContainsKey(107))
            {
                if (!dmReadinessState.ContainsKey(107))
                    dmReadinessState.Add(107, (int)StateMachine.DataModel.LvValues[107]);
                else
                {
                    dmReadinessState[107] = (int)StateMachine.DataModel.LvValues[107];
                    imgHCA.Image = dmBackGround["HCA"][dmReadinessState[107]];
                }
            }
            if (StateMachine.DataModel.LvValues.ContainsKey(108))
            {
                if (!dmReadinessState.ContainsKey(108))
                    dmReadinessState.Add(108, (int)StateMachine.DataModel.LvValues[108]);
                else
                {
                    dmReadinessState[108] = (int)StateMachine.DataModel.LvValues[108];
                    imgEVA.Image = dmBackGround["EVA"][dmReadinessState[108]];
                }
            }
            if (StateMachine.DataModel.LvValues.ContainsKey(109))
            {
                if (!dmReadinessState.ContainsKey(109))
                    dmReadinessState.Add(109, (int)StateMachine.DataModel.LvValues[109]);
                else
                {
                    dmReadinessState[109] = (int)StateMachine.DataModel.LvValues[109];
                    imgAIR.Image = dmBackGround["AIR"][dmReadinessState[109]];
                }
            }
            if (StateMachine.DataModel.LvValues.ContainsKey(110))
            {
                if (!dmReadinessState.ContainsKey(110))
                    dmReadinessState.Add(110, (int)StateMachine.DataModel.LvValues[110]);
                else
                {
                    dmReadinessState[110] = (int)StateMachine.DataModel.LvValues[110];
                    imgACR.Image = dmBackGround["ACR"][dmReadinessState[110]];
                }
            }
            if (StateMachine.DataModel.LvValues.ContainsKey(111))
            {
                if (!dmReadinessState.ContainsKey(111))
                    dmReadinessState.Add(111, (int)StateMachine.DataModel.LvValues[111]);
                else
                {
                    dmReadinessState[111] = (int)StateMachine.DataModel.LvValues[111];
                    imgO2X.Image = dmBackGround["O2S"][dmReadinessState[111]];
                }
            }
            if (StateMachine.DataModel.LvValues.ContainsKey(112))
            {
                if (!dmReadinessState.ContainsKey(112))
                    dmReadinessState.Add(112, (int)StateMachine.DataModel.LvValues[112]);
                else
                {
                    dmReadinessState[112] = (int)StateMachine.DataModel.LvValues[112];
                    imgHTR.Image = dmBackGround["HTR"][dmReadinessState[112]];
                }
            }
            if (StateMachine.DataModel.LvValues.ContainsKey(113))
            {
                if (!dmReadinessState.ContainsKey(113))
                    dmReadinessState.Add(113, (int)StateMachine.DataModel.LvValues[113]);
                else
                {
                    dmReadinessState[113] = (int)StateMachine.DataModel.LvValues[113];
                    imgEGR.Image = dmBackGround["EGR"][dmReadinessState[113]];
                }
            }
        }

        /// <summary>
        /// Gets dtc data when click "btnClearDTC".
        /// </summary>
        /// <returns>The "no dtc data".</returns>
        private DtcData GetNoDtcData()
        {
            var ret = new DtcData();
            ret.DtcHexNumber = 0;
            ret.DtcName = "No Trouble Code.";
            ret.DtcCodeForDisplay = "No DTC";
            return ret;

        }

        public override void setCurrentPageName()
        {
            currentPageView = "DTCView";
            //Console.WriteLine("current page is " + currentPageView);
        }

    }
}


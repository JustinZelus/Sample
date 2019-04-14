using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IcmLib;
using iPhoneBLE;
using iPhoneBLE.SRC;
using CoreFoundation;


class GestureManager
{
    /// <summary>
    /// 當前頁面
    /// </summary>
    private Page mPage = Page.None;


    /// <summary>
    /// 資料模組
    /// </summary>
    private DataModel mDataModel;

    /// <summary>
    /// UI模組
    /// </summary>
    private UIModel UIModel;



    public GestureManager(UIModel uiModel, DataModel dataModel)
    {
        this.UIModel = uiModel;
        this.mDataModel = dataModel;
    }

    /// <summary>
    /// 上一個手勢狀態
    /// </summary>
    GesturePos prevGesturePos = GesturePos.NONE;
    /// <summary>
    /// 目前手勢狀態
    /// </summary>
    GesturePos currentGesturePos = GesturePos.NONE;
    /// <summary>
    /// 要切換的頁面陣列
    /// </summary>
    static readonly Page[] pages = new Page[] { Page.Home, Page.Log, Page.Speed0_100, Page.Speed0_400, Page.LiveData, Page.DTC, Page.Valve };
    /// <summary>
    /// 目前頁面位置
    /// </summary>
    int pagePos = 0;
    /// <summary>
    /// 最大頁面數
    /// </summary>
    static readonly int pageMaxPos = pages.Length;
    /// <summary>
    /// 利用手勢狀態切換頁面
    /// 手勢狀態 0=無, 1=左, 2=右
    /// 相同狀態不予以切頁，例如要由左再切到右
    /// 訊號需為 0 -> 1 -> 0 -> 2
    /// 訊號若為 0 -> 0 -> 0 -> 0 -> 1 -> 1 -> 1 -> 0 -> 0 -> 2 -> 2 -> 2 -> 2 -> 2
    /// 兩者訊號相同
    /// </summary>
    public void UseGesture2ChangePage()
    {
        if (mDataModel == null)
            return;

        currentGesturePos = mDataModel.GestureValue;
        if (prevGesturePos != currentGesturePos)
        {
            prevGesturePos = currentGesturePos;
            switch (currentGesturePos)
            {
                case GesturePos.NONE:
                    break;

                case GesturePos.LEFT:
                    pagePos--;
                    break;

                case GesturePos.RIGHT:
                    pagePos++;
                    break;

                default:
                    break;
            }

            if (pagePos <= 0)
                pagePos = 0;
            if (pagePos >= pageMaxPos)
                pagePos = pageMaxPos;

            mPage = pages[pagePos];


            DispatchQueue.MainQueue.DispatchAsync(() => {
                switch (mPage)
                {
                    case Page.None:
                        break;
                    case Page.Init:
                        break;
                    case Page.Home:
                        break;
                    case Page.Log:
                        break;
                    case Page.LiveData:
                        break;
                    case Page.Gear:
                        break;
                    case Page.DTC:
                        break;
                    case Page.Valve:
                        break;
                    case Page.Speed0_100:
                        break;
                    case Page.Speed0_400:
                        break;
                    case Page.Shift:
                        break;
                    default:
                        break;
                }  
            });

            //mActivity.RunOnUiThread(() =>
            //{
            //    switch (mPage)
            //    {
            //        case Page.Home:
            //            UIModel.SwitchFragment(UIModel.FragmentTable["homeFragment"], "homeFragment");
            //            break;

            //        case Page.Log:
            //            UIModel.SwitchFragment(UIModel.FragmentTable["logFragment"], "logFragment");
            //            break;

            //        case Page.LiveData:
            //            UIModel.SwitchFragment(UIModel.FragmentTable["liveDataFragment"], "liveDataFragment");
            //            break;

            //        case Page.DTC:
            //            UIModel.SwitchFragment(UIModel.FragmentTable["dtcFragment"], "dtcFragment");
            //            break;

            //        case Page.Valve:
            //            UIModel.SwitchFragment(UIModel.FragmentTable["valveFragment"], "valveFragment");
            //            break;

            //        case Page.Speed0_100:
            //            UIModel.SwitchFragment(UIModel.FragmentTable["zero2HundredFragment"], "zero2HundredFragment");
            //            break;

            //        case Page.Speed0_400:
            //            UIModel.SwitchFragment(UIModel.FragmentTable["zero24HundredFragment"], "zero24HundredFragment");
            //            break;

            //        default:
            //            break;
            //    }
            //});
        }
    }
}
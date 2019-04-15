using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using EIP.Models;
using Newtonsoft.Json;
using PagedList;
using PushNotiLibrary;

namespace EIP.Controllers
{
    public class MyInfoController : BaseController
    {
		
        private www_MyInfoRepository oMyInfoRepository = RepositoryHelper.Getwww_MyInfoRepository();
     

        public ActionResult Index(MyInfoIndexViewModel oMyInfoIndexViewModel)
        {
			//我的viewmodel
			//Search()方法裡面做初始化
            oMyInfoIndexViewModel.Search();
            return View(oMyInfoIndexViewModel);
        }

        public ActionResult MyInfoNotificationIndex(MyInfoNotificationIndexViewModel oMyInfoNotificationIndexViewModel)
        {
           
            oMyInfoNotificationIndexViewModel.Search();
            return View(oMyInfoNotificationIndexViewModel);              
        }

        public ActionResult Edit(int SN)
        {
            www_MyInfo oWww_MyInfo = oMyInfoRepository.Find(SN);

            if (oWww_MyInfo == null)
            {
                oWww_MyInfo = new www_MyInfo();
            }
            MyInfoEditViewModel oMyInfoEditViewModel = new MyInfoEditViewModel();
            Tools.AutoGetProperties(oMyInfoEditViewModel, oWww_MyInfo);

            return View(oMyInfoEditViewModel);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult PushNotification(PushObject oPushObject, List<int> ListMyInfoSN)
        {
            bool bResult = true;
            www_MyInfoNotificationRepository oMyInfoNotificationRepository = RepositoryHelper.Getwww_MyInfoNotificationRepository();
            try
            {
                foreach (var MyInfo in oMyInfoRepository.All().Where(o=> ListMyInfoSN.Contains( o.SN)).ToList())
                {
                    #region 新增一筆推播
                    www_MyInfoNotification owww_MyInfoNotification = new www_MyInfoNotification();
                    owww_MyInfoNotification.MyInfoSN = MyInfo.SN;
                    owww_MyInfoNotification.Title = oPushObject.Title;
                    owww_MyInfoNotification.Content = oPushObject.Body;
                    owww_MyInfoNotification.CreateDate = DateTime.Now;
                    owww_MyInfoNotification.IsRead = false;
                    oMyInfoNotificationRepository.Insert(owww_MyInfoNotification);
                    #endregion

                    oPushObject.Badge = oMyInfoNotificationRepository.FindByMyInfoSN(MyInfo.SN).Where(o=>o.IsRead != true).Count();
                    PushNotificationService service = new PushNotificationService();
                    bool IsSuccess = service.PushToIOS(oPushObject, MyInfo.App_Token, out string Message);
                    Console.WriteLine(IsSuccess ? "Success" : "Faild");
                    bResult = bResult && IsSuccess;
                }
            }
            catch (Exception)
            {

                throw;
            }
           

            return Json(new { IsSuccess  = bResult }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int SN)
        {
            try
            {                
                www_MyInfo owww_MyInfo = oMyInfoRepository.Find(SN);
                oMyInfoRepository.Delete(owww_MyInfo);
                return DeleteSuccess();
            }
            catch (Exception e)
            {
                return DeleteFail(e.Message);
            }
        }
    }

    public class MyInfoIndexViewModel
    {
        private www_MyInfoRepository oMyInfoRepository = RepositoryHelper.Getwww_MyInfoRepository();

        public string Q_Name { get; set; } = "";
        public int Q_ParentSN { get; set; } = 0;
        public int pageNo { get; set; } = 1;
        public string Btn { get; set; }
        public int pageSize { get; set; } = Tools.Manager_PageSize;

        public IPagedList<MyInfoEditViewModel> ListData { get; set; }

        private IQueryable<MyInfoEditViewModel> _Search()
        {
            #region 查詢條件 資料處理
            IQueryable<www_MyInfo> Result1 = oMyInfoRepository.GetAll();

            if (!string.IsNullOrEmpty(Q_Name))
            {
                Result1 = Result1.Where(o => o.UserName.Contains(Q_Name));
            }
            IQueryable<MyInfoEditViewModel> Result = Result1.ProjectToQueryable<MyInfoEditViewModel>();
            #endregion

            return Result;
        }

        public void Search()
        {
            IQueryable<MyInfoEditViewModel> Result1 = _Search();
            ListData = Result1.ToPagedList(pageNo, pageSize);
        }
    }

    public class MyInfoEditViewModel : IMapFrom<www_MyInfo>
    {

        public int SN { get; set; }
        public Nullable<int> Level { get; set; }
        public string Notification { get; set; }
        public string FB_ID { get; set; }
        public string ICCardID { get; set; }
        //特例:資料庫欄位存的是json，所以需要做反序列化
        public string User_Info {
            get { return JsonConvert.SerializeObject(oMyInfo_UserInfo); }
            set {
                oMyInfo_UserInfo = JsonConvert.DeserializeObject<MyInfo_UserInfo>(value);
            }
        }
        public MyInfo_UserInfo oMyInfo_UserInfo { get; set; } = new MyInfo_UserInfo();
        public string App_Token { get; set; }
        public string DeviceUID { get; set; }
        [Display(Name = "~%214%~姓名%%")]
        public string UserName { get; set; }

        public string StatusString
        {
            get
            {
                string Result = "Error";
                if (FB_ID == "" || FB_ID == null)
                {
                    Result = "<span class=\"status is-warning  icon-danger \">否</span>";
                }
                else
                {
                    Result = "<span class=\"status is-success icon-success\">是</span>";
                }

                return Result;
            }
        }
        public class MyInfo_UserInfo
        {
            public string name { get; set; }
            public string phone { get; set; }
            public string mobile { get; set; }
            public string age { get; set; }
            public string gender { get; set; }
            public string pid { get; set; }
            public string living_city { get; set; }
            public string register_living_city { get; set; }
            public string cAddress { get; set; }
            public string pAddress { get; set; }
            public string email { get; set; }
            public string disability { get; set; }
            public string company { get; set; }
            public string title { get; set; }
        }
    }

    public class MyInfoNotificationIndexViewModel
    {
        private www_MyInfoNotificationRepository oMyInfoNotificationRepository = RepositoryHelper.Getwww_MyInfoNotificationRepository();
        public int MyInfoSN { get; set; } 

        public int pageNo { get; set; } = 1;
        //public string Btn { get; set; }
        public int pageSize { get; set; } = Tools.Manager_PageSize;

        public IPagedList<MyInfoNotificationEditViewModel> ListData { get; set; }

        public void Search()
        {
            ListData = _Search(MyInfoSN).OrderByDescending(o => o.SN).ToPagedList(pageNo, pageSize);
        }

        private IQueryable<MyInfoNotificationEditViewModel> _Search(int MyInfoSN)
        {
            #region 查詢條件 資料處裡

            IQueryable<MyInfoNotificationEditViewModel> Results = oMyInfoNotificationRepository.FindByMyInfoSN(MyInfoSN, null).ProjectToQueryable<MyInfoNotificationEditViewModel>();

            #endregion

            return Results;
          
        }

    
    }

    public class MyInfoNotificationEditViewModel : IMapFrom<www_MyInfoNotification>
    {
        public int SN { get; set; }
        public Nullable<int> MyInfoSN { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Nullable<bool> IsRead { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
    }
}
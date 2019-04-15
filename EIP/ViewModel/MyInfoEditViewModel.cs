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

namespace EIP.ViewModel
{

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
		//我的model
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
}
	
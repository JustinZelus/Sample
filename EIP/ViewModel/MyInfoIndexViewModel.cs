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

	public class MyInfoIndexViewModel
    {
		//我的model
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
}
	
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;

namespace EIP.Models
{   
	public  class www_MyInfoRepository : EFRepository<www_MyInfo>, Iwww_MyInfoRepository
	{
        public IQueryable<www_MyInfo> GetAll()
        {
            IQueryable<www_MyInfo> Result = base.All();
            return Result.OrderBy(o => o.SN);
        }

        public www_MyInfo Find(int? SN)
        {
            return All().Where(o => o.SN == SN).SingleOrDefault();
        }

        public www_MyInfo Find(string DeviceUID)
        {
            return All().Where(o => o.DeviceUID == DeviceUID).SingleOrDefault();
        }

        public override void Delete(www_MyInfo entity)
        {
            UnitOfWork.Context.Entry(entity).State = EntityState.Deleted;
            UnitOfWork.Commit();
        }
        public void Update(www_MyInfo entity)
        {
            UnitOfWork.Context.Entry(entity).State = EntityState.Modified;
            UnitOfWork.Commit();
        }

        public void Insert(www_MyInfo entity)
        {
            Add(entity);
            UnitOfWork.Commit();
        }
    }

	public  interface Iwww_MyInfoRepository : IRepository<www_MyInfo>
	{

	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin_SYM_IOS.ViewControllers;

namespace Xamarin_SYM_IOS.SRC.Utils
{
    public static class UnitConverter
    {   
        //public static float UnitKmMile(float val,Unit unit)
        //{
        //    if(unit == Unit.Km)
        //        return 
        //    return 0;
        //}

        /// <summary>
        /// 單位轉換 公里轉換為英里
        /// </summary>
        /// <param name="kmValue">公里數值</param>
        /// <returns>英里數值</returns>
        public static float UnitKm2Mile(float kmValue)
        {
            return (kmValue * 0.621371192f);
        }

        /// <summary>
        /// 單位轉換 英里轉換為公里
        /// </summary>
        /// <param name="kmValue">英里數值</param>
        /// <returns>公里數值</returns>
        public static float UnitMile2Km(float mileValue)
        {
            return (mileValue * 1.609344f);
        }
    }
}






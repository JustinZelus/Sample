using System;
using CoreLocation;

namespace Xamarin_SYM_IOS.SRC.Model
{
    /// <summary>
    /// 用以儲存GPS資料之類別
    /// </summary>
    public class GPSData
    {
        /// <summary>
        /// 設定經緯度資料
        /// </summary>
        /// <param name="location">Location實例</param>
        public static void SetData(CLLocation location)
        {

            //var d = location.Coordinate.Latitude;
            if (latLng == null)
                latLng = new LatLng(location.Coordinate.Latitude, location.Coordinate.Longitude);
            else
            {
                latLng.Latitude = location.Coordinate.Latitude;
                latLng.Longitude = location.Coordinate.Longitude;
            }
        }

        /// <summary>
        /// 設定經緯度資料
        /// </summary>
        /// <param name="latitude">緯度</param>
        /// <param name="longitude">經度</param>
        public static void SetData(double latitude, double longitude)
        {
            if (latLng == null)
                latLng = new LatLng(latitude, longitude);
            else
            {
                latLng.Latitude = latitude;
                latLng.Longitude = longitude;
            }
        }

        /// <summary>
        /// 經緯度資訊欄位
        /// </summary>
        private static LatLng latLng = null;


        /// <summary>
        /// 經緯度資訊屬性
        /// </summary>
        public static LatLng LatLng
        {
            get
            {
                if (latLng == null)
                    latLng = new LatLng(0, 0);
                return latLng;
            }
        }
    }
}

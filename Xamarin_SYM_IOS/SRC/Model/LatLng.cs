using System;
namespace Xamarin_SYM_IOS.SRC.Model
{
    public class LatLng
    {

        public double latitude;
        public double longitude;

        public LatLng(double latitude, double longitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
        }

        public double Latitude
        {
            get
            {
                return latitude;
            }
            set
            {
                latitude = value;
            }
        }

        public double Longitude
        {
            get
            {
                return longitude;
            }
            set
            {
                longitude = value;
            }
        }
    }
}

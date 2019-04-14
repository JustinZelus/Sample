using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CoreLocation;
using Foundation;
//using Plugin.Geolocator;
//using Plugin.Geolocator.Abstractions;
//using Plugin.Permissions;
//using Plugin.Permissions.Abstractions;

namespace Xamarin_SYM_IOS.SRC.Utils
{
    public class ComLocationManager
    {
        private DateTimeOffset timestamp;

        private double latitude;

        private double longitude;

        //IGeolocator locator;

        public ComLocationManager()
        {
            //RequestLocationPermission();
            //test();
            //InitLocation();
            //asyncInitLocation();
        }

        void RequestLocationPermission()
        {

            var locationManager = new CLLocationManager();

            EventHandler<CLAuthorizationChangedEventArgs> authCallback = null;

            authCallback = (sender, e) =>
            {
                if (e.Status == CLAuthorizationStatus.NotDetermined)
                    return;


                locationManager.AuthorizationChanged -= authCallback;
                //do stuff here 
            };

            locationManager.AuthorizationChanged += authCallback;


            var info = NSBundle.MainBundle.InfoDictionary;
            if (info.ContainsKey(new NSString("NSLocationWhenInUseUsageDescription")))
                locationManager.RequestWhenInUseAuthorization();
            else if (info.ContainsKey(new NSString("NSLocationAlwaysUsageDescription")))
                locationManager.RequestAlwaysAuthorization();
            else
                throw new UnauthorizedAccessException("On iOS 8.0 and higher you must set either NSLocationWhenInUseUsageDescription or NSLocationAlwaysUsageDescription in your Info.plist file to enable Authorization Requests for Location updates!");
        }

        private async Task<bool> DisplayAlert(string msg){
            
            return true;
        }


        public DateTimeOffset getTimestamp()
        {
            return this.timestamp;
        }

        public double getLatitude()
        {
            return this.latitude;
        }

        public double getLongitude()
        {
            return this.longitude;
        }

        public void refresh()
        {
            //asyncInitLocation();
        }
    }
}

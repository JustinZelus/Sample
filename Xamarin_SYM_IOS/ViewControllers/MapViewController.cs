using System;
using MapKit;
using UIKit;
using CoreLocation;
using CoreGraphics;
using Foundation;
using CoreFoundation;
using System.Diagnostics;

namespace Xamarin_SYM_IOS.ViewControllers
{
    public partial class MapViewController : UIViewController, IMKMapViewDelegate
    {
        //CLLocationManager locationManager;
        MKMapView map;

        UIButton btnStart;
        UIButton btnStop;

        UIButton btnStartMap;
        private double myLatitude;
        private double myLongitude;
        //private bool isSetRegionOnce = false;
        //private bool isStartingLocation = false;

        protected MapViewController(IntPtr handle) : base(handle)
        {

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            btnStart = new UIButton(new CGRect(0, 0, 300, 60));
            btnStart.BackgroundColor = UIColor.Gray;
            btnStart.SetTitle("location start", UIControlState.Normal);
            btnStart.TouchUpInside += (sender, e) =>
            {
                //StartUpdatingLocation();
            };

            btnStop = new UIButton(new CGRect(0, 80, 300, 60));
            btnStop.BackgroundColor = UIColor.Gray;
            btnStop.SetTitle("location Stop", UIControlState.Normal);
            btnStop.TouchUpInside += (sender, e) =>
            {
                //StopUpdatingLocation();
            };
            btnStartMap = new UIButton(new CGRect(0, 160, 300, 60));
            btnStartMap.BackgroundColor = UIColor.Gray;
            btnStartMap.SetTitle("My Map", UIControlState.Normal);
            btnStartMap.TouchUpInside += (sender, e) =>
            {
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    //CreateMapKit();    
                });

            };
          
            //this.View.AddSubview(btnStartMap);
            CreateMapKit();
            //View.AddSubview(btnStart);
            //View.AddSubview(btnStop);
            ContainerViewController.Instance.SetMKMapView(map);
        }

		public override void ViewDidAppear(bool animated)
		{
            base.ViewDidAppear(animated);

            if (StateMachine.IsActivted)
            {   
                //if(StateMachine.Instance.symSharedPreferencesExtractor.GetGPSAllow())
                    //StartUpdatingLocation();
            }
		}
		public override void ViewDidDisappear(bool animated)
		{
            base.ViewDidDisappear(animated);
            //StopUpdatingLocation();
		}

		/// <summary>
		/// create map
		/// </summary>
		private void CreateMapKit()
        {
            //if (locationManager == null)
            //{
            //    locationManager = new CLLocationManager();
            //    locationManager.Delegate = this;
            //    Debug.WriteLine("created - CLLocationManager");
            //}

            if (map == null)
            {
                //map = new MKMapView(new CGRect(0, 240, 400, 400));
                CGRect SCREEN_SIZE = UIScreen.MainScreen.Bounds;
                float width = (float)View.Frame.Width;
                float height = (float)View.Frame.Height;
                double offset = SCREEN_SIZE.Height / 5.2;

                map = new MKMapView(new CGRect(0, 0, width, height - offset));
                map.ShowsUserLocation = true;
                map.Delegate = this;
                Debug.WriteLine("created - MKMapView : size(w: " + width + " h: " + (height - offset) + ")");
                //map.SetUserTrackingMode(MKUserTrackingMode.Follow, false);
                //CLLocationCoordinate2D coordinate2D = new CLLocationCoordinate2D(myLatitude, myLongitude);
                //MKCoordinateSpan coordinateSpan = new MKCoordinateSpan();
                //MKCoordinateRegion region = MKCoordinateRegion.FromDistance(coordinate2D, 500, 500);
                //map.SetRegion(region, false);
                //map.AddSubview(btnStart);
                View.AddSubview(map);
            }
        }

        /// <summary>
        /// location位置更新callback
        /// </summary>
        [Export("mapView:didUpdateUserLocation:")]
        public void DidUpdateUserLocation(MKMapView mapView, MKUserLocation userLocation)
        {
            //map.SetCenterCoordinate(userLocation.Coordinate, true);
        }

        private void WriteToLog()
        {

        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}


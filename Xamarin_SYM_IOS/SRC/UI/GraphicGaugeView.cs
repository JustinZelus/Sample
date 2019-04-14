using System;
using System.Collections.Generic;
using System.Threading;
using CoreAnimation;
using CoreFoundation;
using CoreGraphics;
using UIKit;

//Created by Justin on 2017/6/30

namespace Xamarin_SYM_IOS.SRC.UI
{
    public class GraphicGaugeView : CALayer
    {
        public GraphicGaugeView()
        {
        }

        //public CALayer CreateBG_Gauge(string image,float coordinate_X,float coordinate_Y,
        public CALayer CreateBG_Gauge(UIImage image, float size, float centerX, float centerY){
			

			CALayer bg_gauge   = new CALayer();
			bg_gauge.Frame    = new CGRect(0, 0, size, size);
			bg_gauge.Position = new CGPoint(centerX,centerY);
            bg_gauge.Contents = image.CGImage;
			bg_gauge.MasksToBounds = true;

            return bg_gauge;
        }



        public CALayer CreateBG_Frame(UIImage image, float size, float centerX, float centerY){
			CALayer bg_frame = new CALayer();
			bg_frame.Bounds = new CGRect(0, 0, size, size + 20); //下方加長
			bg_frame.Position = new CGPoint(centerX, centerY);
			bg_frame.Contents = image.CGImage;
			//imgLayer_Gauge.Frame = new CGRect(100, 100, 330, 330);
			bg_frame.MasksToBounds = true;

            return bg_frame;
        }
   
    }
}

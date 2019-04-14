﻿using System;
using System.Collections.Generic;
using System.Threading;
using CoreAnimation;
using CoreFoundation;
using CoreGraphics;
using UIKit;

//Created by Justin on 2017/6/30

namespace Xamarin_SYM_IOS.SRC.UI
{
    public class GraphicFan : CAShapeLayer
    {
        //起始顏色
        private float Red_Start = 0;
        private float Green_Start = 255;
        private float BLue_Start  = 255;
		//終點顏色
		private float Red_End = 255;
		private float Green_End = 20;
		private float BLue_End = 157;

		//private UIColor vColorRGB_Start;
        //private UIColor vColorRGB_End;

        public static float[] dynamicColor = new float[3];

        private int lvID;
       

        public GraphicFan(float centerX, float centerY, float radius, float startAngle, float endAngle,bool isClockwise)
        {
            UIBezierPath path = UIBezierPath.FromArc(new CGPoint(centerX, centerY),
                                                     radius,
                                                     startAngle,
													 //0,
                                                     endAngle,
                                                     isClockwise);

			this.LineWidth = radius * 2;
			this.LineCap = CAShapeLayer.CapButt;
            this.StrokeColor = AppAttribute.APP_RUNNING_MODE == AppAttribute.RunningMode.DoUITest ? UIColor.Green.CGColor : UIColor.Clear.CGColor;
			this.FillColor = null;
			this.StrokeStart = 0;
			this.StrokeEnd = 0;
			this.Path = path.CGPath;


            //vColorRGB_Start = UIColor.FromRGB(Red_Start, Green_Start, BLue_Start);
            //vColorRGB_End   = UIColor.FromRGB(Red_End ,Green_End, BLue_End);
        }

        public void SetValue(float val){
            //SetNeedsDisplay();
            this.StrokeEnd = val;

        }

        public  UIColor GetDynamicColor(float val){
            return UIColor.FromRGB(GetRedColorValue(val), GetGreenColorValue(val), GetBLueColorValue(val));
        }
        //計算紅色
        private  int GetRedColorValue(float val){
            
            return (int)(Red_Start  + (val * 255));
        }
		//計算綠色
		private  int GetGreenColorValue(float val)
		{   
          
            return (int)(Green_Start  - (val * 235));
		}
		//計算藍色
		private  int GetBLueColorValue(float val)
		{

            return (int)(BLue_Start - (val * 98));
		}
        //提供給外部的接口
        public void SetColorTransformValue(float val){
            if (val > 1.0) val = 1.0f;
                
            this.StrokeColor = GetDynamicColor(val).CGColor;

        }

        public void SetKeyForDrawingWithValue(int lvID){
            this.lvID = lvID;
        }

        public int GetKey(){
            return lvID;
        }

        public string GetFormat(int key)
        {  
            //var i = StateMachine.DataModel.LvDatas[key].NumberOfDecimals;
            return "f" + StateMachine.DataModel.LvDatas[key].NumberOfDecimals;
            //if (key == 1)
            //    return "0.";
            //return "0.##";
        }

		//public string GetFormatByTable(int key)
		//{
  //          var i = StateMachine.DataModel.LvDatas[key].NumberOfDecimals;

		//	//return "0.##";
		//}
    }
}

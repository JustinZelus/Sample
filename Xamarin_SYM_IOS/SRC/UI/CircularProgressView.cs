using System;
using System.ComponentModel;
using System.Drawing;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Xamarin_SYM_IOS
{
	public class CircularProgressView : UIView
	{
		private float       _progress = 0;
		private float       _startAngle = 0;
		private float       _progressBarWidth = 0;
		private float       hintViewSpacing = 0;
		private bool        hintHidden = false;
		private const float DefaultProgressBarWidth = 33.0f;
		private const float DefaultHintSpacing = 20.0f;
		private const float AnimationChangeTimeDuration = 0.2f;

		private UIColor _progressBarProgressColor = null;
		private UIColor _progressBarTrackColor = null;
		private UIColor hintViewBackgroundColor = new UIColor(0.71f, 0.099f, 0.099f, 0.7f); //divide 255
		private UIColor DefaultProgressBarProgressColor = new UIColor(0.71f, 0.099f, 0.099f, 0.7f);
		private UIColor DefaultProgressBarTrackColor = new UIColor(1f, 1f, 1f, 0.7f);
		private UIColor DefaultHintBackgroundColor = new UIColor(0, 0.7f);

		//private string hintTextGenerationBlock = null;
		//private string defaultHintTextGenerationBlock = null;

	

		//初始化
		public CircularProgressView(CGRect rect)
		{
			//nfloat lx = (UIScreen.MainScreen.Bounds.Width - size.Width) / 2;
			//nfloat ly = (UIScreen.MainScreen.Bounds.Height - size.Height) / 2;
			this.Frame = rect;
			this.BackgroundColor = UIColor.Clear;
		}

		/// <summary>
		/// Use CGxxxx class for Draw the circular progress bar.
		/// </summary>
		public override void Draw(CGRect rect)
		{
			base.Draw(rect);
			using (CGContext context = UIGraphics.GetCurrentContext())
			{
				CGPoint innerCenter = new CGPoint(this.Bounds.Size.Width / 2, this.Bounds.Size.Height / 2);
				float radius = (float)Math.Min(innerCenter.X, innerCenter.Y);
				float currentProgressAngle = (_progress * 360) + _startAngle;
				context.ClearRect(rect);

				this.DrawBackground(context);
				this.DrawProgressBar(context, currentProgressAngle, innerCenter, radius);

				if (!hintHidden)
				{
					this.DrawHint(context, innerCenter, radius);
				}
			}
		}

	
		private void DrawProgressBar(CGContext c, float progressAngle, CGPoint center, float radius)
		{
			float barWidth = this.ProgressBarWidthForDrawing();
			if (barWidth > radius)
			{
				barWidth = radius;
			}
			c.SetFillColor(this.ProgressBarProgressColorForDrawing().CGColor);

			c.BeginPath();
			c.AddArc(center.X, center.Y, radius, DEGREES_TO_RADIANS(_startAngle), DEGREES_TO_RADIANS(progressAngle), false);
			c.AddArc(center.X, center.Y, radius - barWidth, DEGREES_TO_RADIANS(progressAngle), DEGREES_TO_RADIANS(_startAngle), true);
			c.ClosePath();
			c.FillPath();
			c.SetFillColor(this.ProgressBarTrackColorForDrawing().CGColor);

			c.BeginPath();
			c.AddArc(center.X, center.Y, radius, DEGREES_TO_RADIANS(progressAngle), DEGREES_TO_RADIANS(_startAngle + 360), false);
			c.AddArc(center.X, center.Y, radius - barWidth, DEGREES_TO_RADIANS(_startAngle + 360), DEGREES_TO_RADIANS(progressAngle), true);


			c.ClosePath();
			c.FillPath();

		}


		private void DrawBackground(CGContext c)
		{
			c.SetFillColor(this.BackgroundColor.CGColor);
			c.FillRect(this.Bounds);
		}

		private void DrawSimpleHintTextAtCenter(CGPoint center)
		{
			String progressString = this.StringRepresentationOfProgress(_progress);
			CGSize hintTextSize = new CGSize(39, 37);


			//use NSAttributedString instead of String for render font color
			var attributedString = new NSAttributedString(
				progressString,
				new UIStringAttributes
				{
					ForegroundColor = UIColor.White,
					Font = UIFont.FromName("HelveticaNeue-CondensedBlack", 28)
				});

			attributedString.DrawString(new CGPoint(center.X - hintTextSize.Width / 2,
													center.Y - hintTextSize.Height / 2));

		}

		private void DrawHint(CGContext c, CGPoint center, float radius)
		{
			float barWidth = this.ProgressBarWidthForDrawing();
			if (barWidth + this.HintViewSpacingForDrawing() > radius)
			{
				return;
			}
			c.SetFillColor(this.HintViewBackgroundColorForDrawing().CGColor);
			c.BeginPath();
			c.AddArc(center.X, center.Y, radius - barWidth - this.HintViewSpacingForDrawing(), DEGREES_TO_RADIANS(0), DEGREES_TO_RADIANS(360), true);
			c.ClosePath();
			c.FillPath();

			this.DrawSimpleHintTextAtCenter(center);
		}

		public void SetProgress(float p, bool animated)
		{
			SetProgress(p, animated, AnimationChangeTimeDuration);
		}

		private void SetProgress(float p, bool animated, float duration)
		{
			p = this.ProgressAccordingToBounds(p);
			if (_progress == p)
			{
				return;
			}

			if (animated)
			{
				//做動畫效果的
			}
			else
			{
				_progress = p;
				this.SetNeedsDisplay();
			}
		}

		private float ProgressAccordingToBounds(float p)
		{
			p = Math.Min(p, 1);
			p = Math.Max(p, 0);
			return p;
		}

		private string StringRepresentationOfProgress(float p)
		{
			return DefaultHintTextGenerationBlock(p);
		}

		private string DefaultHintTextGenerationBlock(float p)
		{
			return String.Format("{0:0%}", p);
		}

		private UIColor HintViewBackgroundColorForDrawing()
		{
			return hintViewBackgroundColor != null ? hintViewBackgroundColor : DefaultHintBackgroundColor;
		}

		private float HintViewSpacingForDrawing()
		{
			return (hintViewSpacing != 0 ? hintViewSpacing : DefaultHintSpacing);
		}

		private float ProgressBarWidthForDrawing()
		{
			return (_progressBarWidth > 0 ? _progressBarWidth : DefaultProgressBarWidth);
		}



		private UIColor ProgressBarProgressColorForDrawing()
		{
			return (_progressBarProgressColor != null ? _progressBarProgressColor : DefaultProgressBarProgressColor);
		}

		private UIColor ProgressBarTrackColorForDrawing()
		{
			return (_progressBarTrackColor != null ? _progressBarTrackColor : DefaultProgressBarTrackColor);
		}

		private float DEGREES_TO_RADIANS(float angle)
		{
			return angle / 180.0f * (float)Math.PI;
		}


		public float _Progress
		{
			get
			{
				return _progress;
			}
		}
	}
}

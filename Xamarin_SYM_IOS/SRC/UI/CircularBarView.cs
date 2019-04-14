using System;
using System.Diagnostics;
using System.IO;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Xamarin_SYM_IOS
{
	public class CircularBarView : I0_100_400_UI
	{
		//用來限制字串到小數點第二位
		private NSNumberFormatter formatter = new NSNumberFormatter();
		//use for ipe document
		private IpeFile ipeFile;
		//轉圈圈的view
		private CircularProgressView cpv;
		//一塊全屏的透明View,用來加上轉圈圈view
		private UIView transParentView_Add_CPBar;

		public delegate void UICallBack();
		public UICallBack ResetTimerValue;



		public CircularBarView(string fileName)
		{
			this.ipeFile = new IpeFile(fileName);
			formatter.MaximumIntegerDigits = 2;
			formatter.MinimumIntegerDigits = 2;
			formatter.MaximumFractionDigits = 2;
			formatter.MinimumFractionDigits = 2;
		}

		/// <summary>
		/// Get five parameters to Run circularprogress bar from CBBLEWrapper.
		/// 
		/// </summary>
		/// <param name="CurrentBlockIndex">Current block index.</param>
		/// <param name="AllLogBlocks">All log blocks.</param>
		/// <param name="CurrentReadLogRawValue">Current read log raw value.</param>
		/// <param name="TotalLogRawValues">Total log raw values.</param>
		/// <param name="IsLogOperationOk">If set to <c>true</c> is log operation ok.</param>
		public void LoadingDatafromBle(int CurrentBlockIndex, int AllLogBlocks, byte[] CurrentReadLogRawValue, byte[] TotalLogRawValues, bool IsLogOperationOk)
		{
			//Console.WriteLine("CurrentBlockIndex  , AllLogBlocks ,IsLogOperationOk : "
			//+ CurrentBlockIndex + " , " + AllLogBlocks + " , " + IsLogOperationOk);

			if (!IsLogOperationOk)
			{
				DispatchQueue.MainQueue.DispatchAsync(() =>
				{
					if (AllLogBlocks > 0)
					{
						//Debug.WriteLine("Get LogRawValue Process : {0:0.00} %", (((float)CurrentBlockIndex / AllLogBlocks) * 100));
						//Console.WriteLine("CurrentBlockIndex  , AllLogBlocks : " + CurrentBlockIndex+ " , " + AllLogBlocks);
						cpv.SetProgress((((float)CurrentBlockIndex / AllLogBlocks)), false);
					}
				});
			}
			//when 100%
			else if (IsLogOperationOk && TotalLogRawValues != null)
			{
				Debug.WriteLine(BitConverter.ToString(TotalLogRawValues));
				RemoveCirProgressView(transParentView_Add_CPBar);
				WriteFileToDocument(TotalLogRawValues);
			}
		}

		public void ChangeFontSize(UILabel lblTime, float size)
		{
			//lblTime.Font = UIFont.FromName(@"Digital-7", size);
			lblTime.Font = UIFont.BoldSystemFontOfSize(size);
		}

		public void WriteFileToDocument(byte[] data)
		{
			var isFolder = true;
			try
			{
				if (!NSFileManager.DefaultManager.FileExists(ipeFile.DirName, ref isFolder))
				{
					Directory.CreateDirectory(ipeFile.DirName);
				}
				File.WriteAllBytes(ipeFile.Filename, data);

			}
			catch (Exception ex)
			{
				Debug.WriteLine("0_100.bin fail , dude " + ex);
			}

			//ResetTimerValue();
		}

		public void ShowCircularProgressBar()
		{
			this.isShowCPBar = true;
			transParentView_Add_CPBar.AddSubview(cpv);
			ContainerViewController.Instance.View.AddSubview(transParentView_Add_CPBar);
		}

		public void RemoveCirProgressView(UIView v)
		{
			v.RemoveFromSuperview();
		}

		public void InitCirProgressView()
		{
			cpv = new CircularProgressView(new CGRect((Main.SCREEN_SIZE.Width - Main.SCREEN_SIZE.Width / 1.725) / 2,
													  (Main.SCREEN_SIZE.Height - Main.SCREEN_SIZE.Height / 3.066) / 2,
													  Main.SCREEN_SIZE.Width / 1.725,
													  Main.SCREEN_SIZE.Height / 3.066));

			transParentView_Add_CPBar = new UIView();
			transParentView_Add_CPBar.Frame = new CGRect(0, 0, Main.SCREEN_SIZE.Width, Main.SCREEN_SIZE.Height);
			transParentView_Add_CPBar.BackgroundColor = new UIColor(0.5f, 0.5f);
		}

		//This class use to set the file path,file name.
		public class IpeFile
		{
			private string documents;
			private string dirPath;
			private string dirName;
			private string filename;

			public IpeFile(string fileName)
			{
				documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				dirPath = Path.Combine(documents, "..", "Documents");
				dirName = Path.Combine(dirPath, "IPE_DB");
				filename = Path.Combine(dirName, fileName);
			}

			public string Filename
			{
				get
				{
					return filename;
				}
				set
				{
					filename = value;
				}
			}

			public string DirName
			{
				get
				{
					return dirName;
				}
				set
				{
					dirName = value;
				}
			}

			public string DirPath
			{
				get
				{
					return dirPath;
				}
				set
				{
					dirPath = value;
				}
			}

			public string Documents
			{
				get
				{
					return documents;
				}
				set
				{
					documents = value;
				}
			}

		}

		public NSNumberFormatter Formatter
		{
			get
			{
				return formatter;
			}
		}

		public UIView TransParentView_Add_CPBar
		{
			get
			{
				return transParentView_Add_CPBar;
			}
		}

		public CircularProgressView Cpv
		{
			get
			{
				return cpv;
			}
		}

		public IpeFile _IpeFile
		{ 
			get
			{
				return ipeFile;
			}
		}
	}
}

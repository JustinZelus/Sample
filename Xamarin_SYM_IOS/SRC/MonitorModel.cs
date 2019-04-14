using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using System.Threading.Tasks;
using System.Threading;
using CoreFoundation;

namespace iPhoneBLE.SRC
{
    public abstract class MonitorModel
    {
        protected Task mTask = null;
        protected CancellationTokenSource mCts = new CancellationTokenSource();
        public bool IsUIMode = false;


        public MonitorModel()
        {
        }

        public void Start()
        {	
			

            mTask = Task.Factory.StartNew(Run, mCts.Token);

        }

        public void Stop()
        {
            mCts.Cancel();
        }

        public int DelayTimeMilliSec = 20;

        public void Run()
        {   
            
            SettingPreCondition();
            try
            {
                while (true)
                {
                    if (mCts.IsCancellationRequested)
                    {
                        mCts.Token.ThrowIfCancellationRequested();
                        break;
                    }

                    if (IsUIMode)
                    {
                        DispatchQueue.MainQueue.DispatchAsync(() =>
                        {
                            DoSomething();
                        });
                    }
                    else
                        DoSomething();

                    Thread.Sleep(DelayTimeMilliSec);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("MonitorModel : " + ex);
            }
        }

        public virtual void SettingPreCondition()
        { }

        public abstract void DoSomething();
    }
}

using iPhoneBLE.SRC;
using System.Diagnostics;

public class GestureSimulator:MonitorModel
{
    Stopwatch simulationGestureTimer = new Stopwatch();
    long simulationGestureResetTimeInMillisecs = 1000;
    long simulationGestureTimeInMillisecs = 5000;

    public GestureSimulator(long changePageTimeMilliSecs = 5000)
    {
        simulationGestureTimeInMillisecs = changePageTimeMilliSecs;
        DelayTimeMilliSec = 100;
    }

    private void Run()
    {
        if (!simulationGestureTimer.IsRunning)
        {
            simulationGestureTimer.Start();
        }

        if (simulationGestureTimer.ElapsedMilliseconds > simulationGestureResetTimeInMillisecs)
        {
            if (StateMachine.DataModel.VdiUnpacker != null)
                StateMachine.DataModel.VdiUnpacker.GesturePosValue = IcmLib.GesturePos.NONE;
        }

        if (simulationGestureTimer.ElapsedMilliseconds > simulationGestureTimeInMillisecs)
        {
            if (StateMachine.DataModel.VdiUnpacker != null)
                StateMachine.DataModel.VdiUnpacker.GesturePosValue = IcmLib.GesturePos.RIGHT;
            simulationGestureTimer.Reset();
        }    
        
    }

    public override void DoSomething()
    {
        this.Run();
    }
}
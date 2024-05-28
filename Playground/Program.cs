using System.Collections.Generic;
using System.Threading.Tasks;
using Epsitec.Common.Widgets.Platform;

class Assert
{
    public static void AreEqual<T>(T a, T b)
    {
        if (!EqualityComparer<T>.Default.Equals(a, b))
        {
            throw new System.Exception($"assert fail: expected {a} but got {b}");
        }
    }
}

class Playground
{
    static void Main()
    {
        Assert.AreEqual(326, 326);
        Assert.AreEqual(TimerState.Running, TimerState.Running);
        var mainTask = AsyncMain();
        mainTask.Wait();
    }

    static async Task AsyncMain()
    {
        int eventCount = 0;
        var timer = new Timer(new System.TimeSpan(0, 0, 0, 2));
        timer.TimeElapsed += _ =>
        {
            eventCount++;
        };
        Assert.AreEqual(TimerState.Stopped, timer.State);
        timer.Start();
        await WaitForTimerEvents(1000);
        Assert.AreEqual(TimerState.Running, timer.State);
        timer.Suspend();
        Assert.AreEqual(TimerState.Suspended, timer.State);
        await WaitForTimerEvents(3000);
        Assert.AreEqual(0, eventCount);
        Assert.AreEqual(TimerState.Suspended, timer.State);
        timer.Start();
        Assert.AreEqual(TimerState.Running, timer.State);
        await WaitForTimerEvents(1500);
        Assert.AreEqual(1, eventCount);
        Assert.AreEqual(TimerState.Stopped, timer.State);
    }

    static async Task WaitForTimerEvents(int durationMS)
    {
        var startTime = System.DateTime.Now;
        while (true)
        {
            double elapsedTimeMS = System.DateTime.Now.Subtract(startTime).TotalMilliseconds;
            if (elapsedTimeMS >= durationMS)
            {
                return;
            }
            Timer.FirePendingEvents();
            await Task.Delay(0); // this is to allow other tasks to run
        }
    }
}

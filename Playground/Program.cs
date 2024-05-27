using System.Threading.Tasks;
using Epsitec.Common.Widgets.Platform;

void Main()
{
    var mainTask = AsyncMain();
    mainTask.Wait();
}

async Task AsyncMain()
{
    int eventCount = 0;
    var timer = new Timer(new System.TimeSpan(0, 0, 0, 1));
    timer.TimeElapsed += _ =>
    {
        eventCount++;
    };
    timer.AutoRepeat = true;
    System.Console.WriteLine("test - call timer start");
    timer.Start();
    await WaitForTimerEvents(1500);
    System.Console.WriteLine("test - call timer stop");
    timer.Stop();
    await WaitForTimerEvents(3000);
    System.Console.WriteLine(eventCount);
}

async Task WaitForTimerEvents(int durationMS)
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

Main();

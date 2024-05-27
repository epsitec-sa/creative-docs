using System.Threading.Tasks;
using Epsitec.Common.Widgets.Platform;
using NUnit.Framework;

namespace Epsitec.Common.Tests.Widgets.Platform
{
    [TestFixture]
    public class TimerTest
    {
        [Test]
        public async Task TestTimerFiresOnceInTime()
        {
            int eventCount = 0;
            var timer = new Timer(new System.TimeSpan(0, 0, 1));
            timer.TimeElapsed += _ =>
            {
                eventCount++;
            };
            Assert.AreEqual(TimerState.Stopped, timer.State);
            timer.Start();
            Assert.AreEqual(TimerState.Running, timer.State);
            await WaitForTimerEvents(1500);
            Assert.AreEqual(TimerState.Stopped, timer.State);
            Assert.AreEqual(1, eventCount);
        }

        [Test]
        public async Task TestTimerGetAndSetPeriod()
        {
            int eventCount = 0;
            var timer = new Timer(new System.TimeSpan(0, 0, 1));
            timer.TimeElapsed += _ =>
            {
                eventCount++;
            };
            Assert.AreEqual(TimerState.Stopped, timer.State);
            timer.Start();
            Assert.AreEqual(TimerState.Running, timer.State);
            Assert.AreEqual(1.0, timer.Period);
            await WaitForTimerEvents(1500);
            Assert.AreEqual(TimerState.Stopped, timer.State);
            Assert.AreEqual(1, eventCount);
            timer.Stop();
            timer.Period = 0.2;
            Assert.AreEqual(0.2, timer.Period);
            timer.Start();
            Assert.AreEqual(TimerState.Running, timer.State);
            await WaitForTimerEvents(250);
            Assert.AreEqual(TimerState.Stopped, timer.State);
            Assert.AreEqual(2, eventCount);
        }

        [Test]
        public async Task TestTimerFiresManyTimesInTime()
        {
            int eventCount = 0;
            var timer = new Timer(new System.TimeSpan(0, 0, 0, 0, 50));
            timer.TimeElapsed += _ =>
            {
                eventCount++;
            };
            timer.AutoRepeat = true;
            timer.Start();
            await WaitForTimerEvents(520);
            Assert.AreEqual(10, eventCount);
        }

        [Test]
        public async Task TestTimerDoesNotFireWhenStopped()
        {
            int eventCount = 0;
            var timer = new Timer(new System.TimeSpan(0, 0, 0, 0, 100));
            timer.TimeElapsed += _ =>
            {
                eventCount++;
            };
            timer.AutoRepeat = true;
            Assert.AreEqual(TimerState.Stopped, timer.State);
            System.Console.WriteLine("test - call timer start");
            timer.Start();
            Assert.AreEqual(TimerState.Running, timer.State);
            await WaitForTimerEvents(150);
            Assert.AreEqual(TimerState.Running, timer.State);
            System.Console.WriteLine("test - call timer stop");
            timer.Stop();
            Assert.AreEqual(TimerState.Stopped, timer.State);
            Assert.AreEqual(1, eventCount);
            await WaitForTimerEvents(350);
            Assert.AreEqual(TimerState.Stopped, timer.State);
            Assert.AreEqual(1, eventCount);
        }

        [Test]
        public async Task TestTimerCanRestart()
        {
            int eventCount = 0;
            var timer = new Timer(new System.TimeSpan(0, 0, 0, 0, 200));
            timer.TimeElapsed += _ =>
            {
                eventCount++;
            };
            Assert.AreEqual(TimerState.Stopped, timer.State);
            timer.Start();
            await WaitForTimerEvents(100);
            Assert.AreEqual(TimerState.Running, timer.State);
            timer.Suspend();
            Assert.AreEqual(TimerState.Suspended, timer.State);
            await WaitForTimerEvents(1000);
            Assert.AreEqual(0, eventCount);
            Assert.AreEqual(TimerState.Suspended, timer.State);
            timer.Start();
            Assert.AreEqual(TimerState.Running, timer.State);
            await WaitForTimerEvents(150);
            Assert.AreEqual(1, eventCount);
            Assert.AreEqual(TimerState.Stopped, timer.State);
        }

        public async Task WaitForTimerEvents(int durationMS)
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
}

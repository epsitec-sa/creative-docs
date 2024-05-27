using System.Threading.Tasks;
using Epsitec.Common.Widgets.Platform;
using NUnit.Framework;

namespace Epsitec.Common.Tests.Widgets.Platform
{
    [TestFixture]
    public class TimerTest
    {
        [Test]
        public void TestTimerFiresOnceInTime()
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
            Task.Delay(1500).Wait();
            Assert.AreEqual(TimerState.Stopped, timer.State);
            Assert.AreEqual(1, eventCount);
        }

        [Test]
        public void TestTimerGetAndSetPeriod()
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
            Task.Delay(1500).Wait();
            Assert.AreEqual(TimerState.Stopped, timer.State);
            Assert.AreEqual(1, eventCount);
            timer.Stop();
            timer.Period = 0.2;
            Assert.AreEqual(0.2, timer.Period);
            timer.Start();
            Assert.AreEqual(TimerState.Running, timer.State);
            Task.Delay(250).Wait();
            Assert.AreEqual(TimerState.Stopped, timer.State);
            Assert.AreEqual(2, eventCount);
        }

        [Test]
        public void TestTimerFiresManyTimesInTime()
        {
            int eventCount = 0;
            var timer = new Timer(new System.TimeSpan(0, 0, 0, 0, 50));
            timer.TimeElapsed += _ =>
            {
                eventCount++;
            };
            timer.AutoRepeat = true;
            timer.Start();
            Task.Delay(520).Wait();
            Assert.AreEqual(10, eventCount);
        }

        [Test]
        public void TestTimerDoesNotFireWhenStopped()
        {
            int eventCount = 0;
            var timer = new Timer(new System.TimeSpan(0, 0, 0, 0, 100));
            timer.TimeElapsed += _ =>
            {
                eventCount++;
            };
            timer.AutoRepeat = true;
            Assert.AreEqual(TimerState.Stopped, timer.State);
            timer.Start();
            Assert.AreEqual(TimerState.Running, timer.State);
            Task.Delay(150).Wait();
            Assert.AreEqual(TimerState.Running, timer.State);
            timer.Stop();
            Assert.AreEqual(TimerState.Stopped, timer.State);
            Assert.AreEqual(1, eventCount);
            Task.Delay(350).Wait();
            Assert.AreEqual(TimerState.Stopped, timer.State);
            Assert.AreEqual(1, eventCount);
        }

        [Test]
        public void TestTimerCanRestart()
        {
            int eventCount = 0;
            var timer = new Timer(new System.TimeSpan(0, 0, 0, 0, 200));
            timer.TimeElapsed += _ =>
            {
                eventCount++;
            };
            Assert.AreEqual(TimerState.Stopped, timer.State);
            timer.Start();
            Task.Delay(100).Wait();
            Assert.AreEqual(TimerState.Running, timer.State);
            timer.Suspend();
            Assert.AreEqual(TimerState.Suspended, timer.State);
            Task.Delay(1000).Wait();
            Assert.AreEqual(0, eventCount);
            Assert.AreEqual(TimerState.Suspended, timer.State);
            timer.Start();
            Assert.AreEqual(TimerState.Running, timer.State);
            Task.Delay(110).Wait();
            Assert.AreEqual(1, eventCount);
            Assert.AreEqual(TimerState.Stopped, timer.State);
        }
    }
}

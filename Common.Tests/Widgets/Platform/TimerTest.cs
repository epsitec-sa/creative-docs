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
                System.Console.WriteLine("increment count");
            };
            System.Console.WriteLine("start timer");
            timer.Start();
            Task.Delay(1500).Wait();
            System.Console.WriteLine("check count");
            Assert.AreEqual(1, eventCount);
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
            timer.Start();
            Task.Delay(150).Wait();
            timer.Stop();
            Assert.AreEqual(1, eventCount);
            Task.Delay(350).Wait();
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
            timer.Start();
            Task.Delay(100).Wait();
            timer.Suspend();
            Task.Delay(1000).Wait();
            Assert.AreEqual(0, eventCount);
            timer.Start();
            Task.Delay(110).Wait();
            Assert.AreEqual(1, eventCount);
        }
    }
}

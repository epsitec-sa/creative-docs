/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


using Epsitec.Common.Support;
using NUnit.Framework;

namespace Epsitec.Common.Tests.Support
{
    [TestFixture]
    public class WeakEventListenersTest
    {
        [Test]
        public void Check01AddRemove()
        {
            WeakEventListeners listeners = new WeakEventListeners();

            this.event1Counter = 0;
            this.event2Counter = 0;

            listeners.Add(new SimpleCallback(this.HandleEvent1));
            listeners.Add(new SimpleCallback(this.HandleEvent1));
            listeners.Add(new SimpleCallback(this.HandleEvent2));

            Assert.AreEqual(3, listeners.DebugGetListenerCount());

            listeners.Invoke();

            Assert.AreEqual(2, this.event1Counter);
            Assert.AreEqual(1, this.event2Counter);

            listeners.Remove(new SimpleCallback(this.HandleEvent2));
            listeners.Invoke();

            Assert.AreEqual(4, this.event1Counter);
            Assert.AreEqual(1, this.event2Counter);

            listeners.Remove(new SimpleCallback(this.HandleEvent2));
            listeners.Invoke();

            Assert.AreEqual(6, this.event1Counter);
            Assert.AreEqual(1, this.event2Counter);

            listeners.Remove(new SimpleCallback(this.HandleEvent1));
            listeners.Invoke();

            Assert.AreEqual(7, this.event1Counter);
            Assert.AreEqual(1, this.event2Counter);
            Assert.AreEqual(1, listeners.DebugGetListenerCount());

            listeners.Remove(new SimpleCallback(this.HandleEvent1));
            listeners.Invoke();

            Assert.AreEqual(7, this.event1Counter);
            Assert.AreEqual(1, this.event2Counter);
            Assert.AreEqual(0, listeners.DebugGetListenerCount());
        }

        [Test]
        public void Check02InvokeWithArgs()
        {
            WeakEventListeners listeners = new WeakEventListeners();

            this.event3Counter = 0;

            listeners.Add(new EventHandler(this.HandleEvent3));

            Assert.AreEqual(1, listeners.DebugGetListenerCount());
            listeners.Invoke(this);

            Assert.AreEqual(1, this.event3Counter);
        }

        [Test]
        public void Check03InvokeWithArgs()
        {
            WeakEventListeners listeners = new WeakEventListeners();

            this.event4Counter = 0;

            listeners.Add(new EventHandler<EventArgs>(this.HandleEvent4));

            Assert.AreEqual(1, listeners.DebugGetListenerCount());
            listeners.Invoke(this, new EventArgs());

            Assert.AreEqual(1, this.event4Counter);
        }

        [Test]
        public void Check04InvokeNoListener()
        {
            WeakEventListeners listeners = new WeakEventListeners();

            listeners.Invoke();
            listeners.Invoke(this);
            listeners.Invoke(this, new EventArgs());
        }

        class Dummy
        {
            public void HandleEvent()
            {
                this.Counter = this.Counter + 1;
            }

            public int Counter { get; set; }
        }

        private void HandleEvent1()
        {
            this.event1Counter++;
        }

        private void HandleEvent2()
        {
            this.event2Counter++;
        }

        private void HandleEvent3(object sender)
        {
            Assert.AreEqual(sender, this);

            this.event3Counter++;
        }

        private void HandleEvent4(object sender, EventArgs e)
        {
            Assert.AreEqual(sender, this);
            Assert.IsNotNull(e);

            this.event4Counter++;
        }

        private int event1Counter;
        private int event2Counter;
        private int event3Counter;
        private int event4Counter;
    }
}

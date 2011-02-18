using NUnit.Framework;

namespace Epsitec.Common.Support
{
	[TestFixture] public class EventsTest
	{
		[Test]
		public void CheckWeakDelegateReclaimSource()
		{
			Source s = new Source ();
			Target t = new Target ();

			s.Event += new EventHandler<EventArgs> (t.Handler);

			System.GC.Collect ();
			
			s.Send (EventArgs.Empty);

			Assert.AreEqual (1, t.Count);
			
			System.WeakReference weakT = new System.WeakReference (t);
			System.WeakReference weakS = new System.WeakReference (s);

			s = null;

			System.GC.Collect ();

			Assert.IsFalse (weakS.IsAlive);
			Assert.IsTrue (weakT.IsAlive);
		}

		[Test]
		public void CheckWeakDelegateReclaimTarget()
		{
			Source s = new Source ();
			Target t = new Target ();

			s.Event += new EventHandler<EventArgs> (t.Handler);

			System.GC.Collect ();

			s.Send (EventArgs.Empty);

			Assert.AreEqual (1, t.Count);

			System.WeakReference weakT = new System.WeakReference (t);
			System.WeakReference weakS = new System.WeakReference (s);

			t = null;

			System.GC.Collect ();

			Assert.IsTrue (weakS.IsAlive);
			Assert.IsFalse (weakT.IsAlive);
		}

		[Test]
		public void CheckWeakDelegateReclaimTargetAndSource()
		{
			Source s = new Source ();
			Target t = new Target ();

			s.Event += new EventHandler<EventArgs> (t.Handler);
			s.Event += new EventHandler<EventArgs> (t.Handler);

			s.Send (EventArgs.Empty);

			Assert.AreEqual (2, t.Count);
			
			System.WeakReference weakT = new System.WeakReference (t);
			System.WeakReference weakS = new System.WeakReference (s);

			s = null;
			t = null;

			System.GC.Collect ();

			Assert.IsFalse (weakS.IsAlive);
			Assert.IsFalse (weakT.IsAlive);
		}

		private class Source
		{
			public Source()
			{
			}

			public void Send(EventArgs e)
			{
				this.Event.Invoke (this, e);
			}

			public WeakDelegate<EventArgs> Event = new WeakDelegate<EventArgs> ();
		}
		
		private class Target : IWeakDelegateTarget
		{
			public Target()
			{
			}

			public int Count
			{
				get
				{
					return this.count;
				}
			}
			
			public void Handler(object sender, EventArgs args)
			{
				System.Console.Out.WriteLine ("Target.Handler executed");
				this.count++;
			}

			#region IWeakDelegateTarget Members
			void IWeakDelegateTarget.AddTrampoline(object t)
			{
				if (this.list == null)
				{
					this.list = new System.Collections.ArrayList ();
				}
				this.list.Add (t);
			}

			void IWeakDelegateTarget.RemoveTrampoline(object t)
			{
				if (this.list != null)
				{
					this.list.Remove (t);
				}
			}
			#endregion

			System.Collections.ArrayList list;
			int count;
		}
	}
}

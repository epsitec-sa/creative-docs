//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	//	Thanks to Eric Gunnerson for recommending this be a struct rather
	//	than a class - avoids a heap allocation.
	//	(In Debug mode, we make it a class so that we can add a finalizer
	//	in order to detect when the object is not freed.)
	//	http://www.interact-sw.co.uk/iangblog/2004/03/23/locking

#if DEBUG
	public class TimedLock : System.IDisposable
#else
	public struct TimedLock : System.IDisposable
#endif
	{
		public static System.IDisposable Lock(object o)
		{
			return TimedLock.Lock (o, System.TimeSpan.FromSeconds (10));
		}
		
		public static System.IDisposable Lock(object o, System.TimeSpan timeout)
		{
			TimedLock tl = new TimedLock (o);
			
			if (!System.Threading.Monitor.TryEnter (o, timeout))
			{
				throw new LockTimeoutException ();
			}
			
			return tl;
		}
		
		private TimedLock(object o)
		{
			this.target = o;
		}
		
		public void Dispose()
		{
			System.Threading.Monitor.Exit (this.target);
			
			//	It's a bad error if someone forgets to call Dispose,
			//	so in Debug builds, we put a finalizer in to detect
			//	the error. If Dispose is called, we suppress the
			//	finalizer.
#if DEBUG
			System.GC.SuppressFinalize (this);
#endif
		}

#if DEBUG
		~TimedLock()
		{
			//	If this finalizer runs, someone somewhere failed to
			//	call Dispose, which means we've failed to leave
			//	a monitor!
			
			System.Diagnostics.Debug.Fail ("Undisposed lock.");
		}
#endif
		
		private object target;
	}
}

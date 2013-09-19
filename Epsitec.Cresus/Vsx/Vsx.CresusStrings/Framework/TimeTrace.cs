using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Epsitec
{
	public class TimeTrace : IDisposable
	{
		public TimeTrace()
		{
			var methodInfo = new StackFrame(1).GetMethod ();
			this.methodName = string.Format("{0}.{1}", methodInfo.DeclaringType.Name, methodInfo.Name);
			Trace.WriteLine (string.Format("[{0}] .. {1} ...", Thread.CurrentThread.ManagedThreadId, this.methodName));
			this.stopwatch = Stopwatch.StartNew ();
		}

		#region IDisposable Members

		public void Dispose()
		{
			Trace.WriteLine (string.Format ("[{0}] >> {1}: {2} [ms]", Thread.CurrentThread.ManagedThreadId, this.methodName, this.stopwatch.Elapsed.TotalMilliseconds.ToString ()));
		}

		#endregion

		private Stopwatch stopwatch;
		private string methodName;
	}
}

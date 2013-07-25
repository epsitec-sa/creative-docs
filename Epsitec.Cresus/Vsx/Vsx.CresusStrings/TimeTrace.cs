using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsitec.Cresus.Strings
{
	public class TimeTrace : IDisposable
	{
		public TimeTrace(string prefix = null)
		{
			if (!string.IsNullOrWhiteSpace (prefix))
			{
				Trace.WriteLine (">> " + prefix);
			}
			this.prefix = prefix;
			this.stopwatch = Stopwatch.StartNew ();
		}

		#region IDisposable Members

		public void Dispose()
		{
			Trace.WriteLine (string.Join(" ", this.TracedAtoms));
		}

		#endregion

		private IEnumerable<string> TracedAtoms
		{
			get
			{
				if (!string.IsNullOrWhiteSpace (this.prefix))
				{
					yield return "<< " + this.prefix + ':';
				}
				yield return this.stopwatch.Elapsed.TotalMilliseconds.ToString () + " [ms]";
			}
		}

		private Stopwatch stopwatch;
		private string prefix;
	}
}

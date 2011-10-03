//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Epsitec.Common.Debug
{
	public class DebugOutputTraceListener : System.Diagnostics.TraceListener
	{
		public DebugOutputTraceListener(System.Action<string> writer)
		{
			this.writer = writer;
			this.buffer = new System.Text.StringBuilder ();
			this.timer  = new System.Threading.Timer (this.HandlerTimerTicked, null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

			this.needsDisposing = true;
		}

		~DebugOutputTraceListener()
		{
			this.Dispose (false);
		}

		public override void Write(string message)
		{
			DebugOutputTraceListener.OutputDebugString (message);
			this.EnqueueMessage (message);
		}

		public override void WriteLine(string message)
		{
			this.Write (message + "\r\n");
		}

		[DllImport ("kernel32.dll", CharSet=CharSet.Auto)]
		private static extern void OutputDebugString(string message);

		private void EnqueueMessage(string message)
		{
			try
			{
				lock (this.buffer)
				{
					this.buffer.Append (message);
				}
				
				this.timer.Change (2500, System.Threading.Timeout.Infinite);
			}
			catch
			{
			}
		}

		public void Flush()
		{
			string text;

			lock (this.buffer)
			{
				text = this.buffer.ToString ();
				this.buffer.Length = 0;
			}

			if (text.Length > 0)
			{
				this.writer (text);
			}
		}

		public override void Close()
		{
			this.Flush ();
			this.needsDisposing = false;
			base.Close ();
		}

		protected override void Dispose(bool disposing)
		{
			if (this.needsDisposing)
			{
				this.Flush ();
				this.needsDisposing = false;
			}

			base.Dispose (disposing);
		}

		private void HandlerTimerTicked(object state)
		{
			this.Flush ();
		}

		private bool							needsDisposing;
		private readonly System.Text.StringBuilder		buffer;
		private readonly System.Action<string>	writer;
		private readonly System.Threading.Timer	timer;
	}
}

//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Epsitec.Common.Debug
{
	/// <summary>
	/// The <c>DebugOutputTraceListener</c> class implements a specific trace listener which
	/// can call a delegate after buffering the output and using a timeout before emitting
	/// the messages.
	/// </summary>
	public class DebugOutputTraceListener : System.Diagnostics.TraceListener
	{
		public DebugOutputTraceListener(System.Action<string> writer)
		{
			this.writer = writer;
			this.buffer = new System.Text.StringBuilder ();
			this.timer  = new System.Threading.Timer (this.HandlerTimerTicked, null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
		}


		/// <summary>
		/// Gets or sets a value indicating whether output should also be sent to the Win32
		/// trace API (and to the attached debugger).
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the output should be sent to the Win32 trace API; otherwise, <c>false</c>.
		/// </value>
		public bool EnableOutputDebugString
		{
			get;
			set;
		}


		public override void Write(string message)
		{
			if (this.EnableOutputDebugString)
			{
				DebugOutputTraceListener.OutputDebugString (message);
			}

			this.EnqueueMessage (message);
		}

		public override void WriteLine(string message)
		{
			this.Write (message + "\r\n");
		}

		public override void Flush()
		{
			base.Flush ();
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
			base.Close ();
		}

		
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

		private void HandlerTimerTicked(object state)
		{
			this.Flush ();
		}


		#region Win32 API

		[DllImport ("kernel32.dll", CharSet=CharSet.Auto)]
		private static extern void OutputDebugString(string message);

		#endregion

		
		private readonly System.Text.StringBuilder buffer;
		private readonly System.Action<string>	writer;
		private readonly System.Threading.Timer	timer;
	}
}

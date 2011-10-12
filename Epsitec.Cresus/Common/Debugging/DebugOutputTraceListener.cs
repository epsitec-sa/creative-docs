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
			this.Write (message + System.Environment.NewLine);
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

		public override void Fail(string message, string detailMessage)
		{
			var stackTrace  = new System.Diagnostics.StackTrace (fNeedFileInfo: true);
			var stackDump   = DebugOutputTraceListener.StackTraceToString (stackTrace, 0, stackTrace.FrameCount-1);
			var description = DebugOutputTraceListener.GetEnvironmentDescription ();
			
			var outputMessage = string.Concat ("ASSERT Failed: ", message ?? "-", System.Environment.NewLine, detailMessage ?? "", stackDump, description);

			this.Write (outputMessage);
			this.Flush ();

			if (System.Diagnostics.Debugger.IsAttached)
			{
				base.Fail (message, detailMessage);
			}
			else
			{
				System.Environment.FailFast (outputMessage);
			}
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

		private static string StackTraceToString(System.Diagnostics.StackTrace trace, int startFrameIndex, int endFrameIndex)
		{
			try
			{
				var buffer = new System.Text.StringBuilder (1000);

				for (int i = startFrameIndex; i <= endFrameIndex; i++)
				{
					var frame = trace.GetFrame (i);
					var method = frame.GetMethod ();
					buffer.Append (System.Environment.NewLine);
					buffer.Append ("    at ");
					if (method.ReflectedType != null)
					{
						buffer.Append (method.ReflectedType.Name);
					}
					else
					{
						buffer.Append ("<Module>");
					}
					buffer.Append (".");
					buffer.Append (method.Name);
					buffer.Append ("(");
					var parameters = method.GetParameters ();
					for (int j = 0; j < parameters.Length; j++)
					{
						var info = parameters[j];
						if (j > 0)
						{
							buffer.Append (", ");
						}
						buffer.Append (info.ParameterType.Name);
						buffer.Append (" ");
						buffer.Append (info.Name);
					}
					buffer.Append (")  ");
					buffer.Append (frame.GetFileName ());
					int fileLineNumber = frame.GetFileLineNumber ();
					if (fileLineNumber > 0)
					{
						buffer.Append ("(");
						buffer.Append (fileLineNumber.ToString (System.Globalization.CultureInfo.InvariantCulture));
						buffer.Append (")");
					}
				}

				buffer.Append (System.Environment.NewLine);

				return buffer.ToString ();
			}
			catch
			{
				return "?";
			}
		}

		private static string GetEnvironmentDescription()
		{
			try
			{
				var buffer = new System.Text.StringBuilder (1000);

				buffer.Append ("Thread: ");
				buffer.Append (System.Threading.Thread.CurrentThread.Name ?? "<no name>");
				buffer.Append (System.Environment.NewLine);
				buffer.Append ("Culture: ");
				buffer.Append (System.Threading.Thread.CurrentThread.CurrentCulture.DisplayName);
				buffer.Append (" - UI ");
				buffer.Append (System.Threading.Thread.CurrentThread.CurrentUICulture.DisplayName);
				buffer.Append (System.Environment.NewLine);
#if DOTNET35
				buffer.Append (System.Environment.Is64BitOperatingSystem ? "64-bit OS" : "32-bit OS");
				buffer.Append (", ");
#endif
				buffer.Append (System.Environment.OSVersion.VersionString);
				buffer.Append (System.Environment.NewLine);
				buffer.Append ("CLR Version ");
				buffer.Append (System.Environment.Version.ToString ());
				buffer.Append (System.Environment.NewLine);
				buffer.AppendFormat ("CPU has {0} cores", System.Environment.ProcessorCount);
				buffer.Append (", working set is ");
				buffer.AppendFormat ("{0} MB", System.Environment.WorkingSet / (1024*1024));
				buffer.Append (System.Environment.NewLine);

				return buffer.ToString ();
			}
			catch
			{
				return "?";
			}
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

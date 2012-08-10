//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Debug
{
	/// <summary>
	/// The <c>GeneralExceptionCatcher</c> class catches all exceptions and forwards them
	/// to a handler chain, before aborting.
	/// </summary>
	public class GeneralExceptionCatcher
	{
		public static void Setup()
		{
			if (GeneralExceptionCatcher.isActive)
			{
				return;
			}

			System.Windows.Forms.Application.ThreadException += (sender, e) => GeneralExceptionCatcher.ProcessException (e.Exception);
			System.AppDomain.CurrentDomain.UnhandledException += (sender, e) => GeneralExceptionCatcher.ProcessException (e.ExceptionObject as System.Exception);

#if NO_TASK
			//	Compiling without support for System.Threading.Tasks.
			//	This is the case for Cresus Updater, which includes this source file.
#else
			//	See http://blog.cincura.net/232922-unobserved-tasks-in-net-4-5/ and http://msdn.microsoft.com/en-us/library/system.threading.tasks.taskscheduler.unobservedtaskexception.aspx
			System.Threading.Tasks.TaskScheduler.UnobservedTaskException += (sender, e) => GeneralExceptionCatcher.ProcessException (e.Exception);
#endif

			System.Windows.Forms.Application.SetUnhandledExceptionMode (System.Windows.Forms.UnhandledExceptionMode.CatchException);
			
			GeneralExceptionCatcher.isActive = true;
		}

		public static bool IsActive
		{
			get
			{
				return GeneralExceptionCatcher.isActive;
			}
		}

		public static bool AbortOnException
		{
			get
			{
				return GeneralExceptionCatcher.abortOnException;
			}
			set
			{
				GeneralExceptionCatcher.abortOnException = value;
			}
		}

		public static void AddExceptionHandler(System.Action<System.Exception> handler)
		{
			GeneralExceptionCatcher.handlers.Add (handler);
		}
		
		private static void ProcessException(System.Exception exception)
		{
			foreach (var handler in GeneralExceptionCatcher.handlers.ToArray ())
			{
				handler (exception);
			}

			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			while (exception != null)
			{
				buffer.AppendLine (exception.Message);
				buffer.AppendLine (exception.Source);
				buffer.AppendLine (exception.StackTrace);
				buffer.AppendLine ();

				exception = exception.InnerException;
			}

			System.Diagnostics.Debug.WriteLine (buffer.ToString ());

			if (GeneralExceptionCatcher.abortOnException)
			{
				System.Environment.Exit (1);
			}
		}

		private static readonly List<System.Action<System.Exception>> handlers = new List<System.Action<System.Exception>> ();
		private static bool isActive;
		private static bool abortOnException;
	}
}

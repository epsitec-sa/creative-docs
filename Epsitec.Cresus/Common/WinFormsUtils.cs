using System;
using System.Threading;
using System.Windows.Forms;

namespace Epsitec.Common.Support
{
	public static class WinFormsUtils
	{
		/// <summary>
		/// This method is used to execute a long running method in a windows forms application
		/// without blocking the main thread.
		/// </summary>
		/// <remarks>
		/// This method is usefull to launch a non win forms application that is embedded within a
		/// win forms application. I know, it doesn't make any sense, but let me explain.
		/// For Cresus.Core, we have the desktop application and the server application which are
		/// both in the same executable whose main method is decoradted with the [STAThread]
		/// attribute and use win forms. But the server app is not a win forms application, in the
		/// sense that it doesn't create any form and waits until it is closed. This behavior
		/// blocks the message pump and Visual Studio complains that there is a Context Switch
		/// Deadlock because it thinks that the application is not responding. This method can be
		/// used to launch the server on its own thread without blocking the main thread, so that
		/// the message pump is not blocked, and thus Visual Studio is happy.
		/// </remarks>
		/// <param name="action">The action to execute</param>
		public static void ExecuteWithoutForm(Action action)
		{
			var thread = new Thread (() =>
			{
				action ();

				Application.Exit ();
			});

			thread.Start ();

			Application.Run ();
		}
	}
}

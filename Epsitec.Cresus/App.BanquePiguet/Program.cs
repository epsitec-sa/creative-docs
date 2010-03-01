//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Cresus.Core;
using Epsitec.Common.Debug;

using System.Linq;

namespace Epsitec.App.BanquePiguet
{

	/// <summary>
	/// The <c>Program</c> class is the main entry point of the program.
	/// </summary>
	static class Program
	{

		/// <summary>
		/// Entry point of the program.
		/// </summary>
		/// <param name="args">The args given by the user on the commandl line.</param>
		[System.STAThread]
		static void Main(string[] args)
		{
			Program.SetupExceptionHandlers();
			
			try
			{
				UI.Initialize ();

				bool adminMode = args.Contains ("-admin");

				using (Application application = new Application (adminMode))
				{
					application.Window.Show ();
					application.Window.Run ();
				}
				UI.ShutDown ();
			}
			catch (System.Exception e)
			{
				ErrorLogger.LogAndThrowException (e);
			}
		}

		/// <summary>
		/// Sets up the exception handlers.
		/// </summary>
		static void SetupExceptionHandlers()
		{
			System.AppDomain.CurrentDomain.UnhandledException += (sender, e) => ErrorLogger.LogAndThrowException (e.ExceptionObject as System.Exception);
			System.Windows.Forms.Application.ThreadException += (sender, e) => ErrorLogger.LogAndThrowException (e.Exception);
			System.Windows.Forms.Application.SetUnhandledExceptionMode (System.Windows.Forms.UnhandledExceptionMode.CatchException);
		}

	}

}

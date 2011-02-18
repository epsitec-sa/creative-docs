//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Cresus.Core.Library;

using Epsitec.Cresus.Graph.Splash;
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
		/// <param name="args">The arguments given by the user on the command line.</param>
		[System.STAThread]
		static void Main(string[] args)
		{
			using (SplashScreen splashScreen = new SplashScreen("Splash\\splash.png"))
			{
				// Call the program in another method to allow faster just-in-time
				// compilation for the splash screen.
				Program.ExecuteProgram (args, splashScreen);
			}
		}

		/// <summary>
		/// The core of the program.
		/// </summary>
		/// <param name="args">The arguments given by the user on the command line.</param>
		/// <param name="splashScreen">The splash screen.</param>
		public static void ExecuteProgram(string[] args, SplashScreen splashScreen)
		{
			try
			{
				Program.SetupExceptionHandlers ();
				UI.Initialize ();

				bool adminMode = args.Contains ("-admin");

				using (Application application = new Application (adminMode))
				{
					application.Window.Show ();
					splashScreen.NotifyIsRunning ();
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

//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Cresus.Core;

using System.Linq;

namespace Epsitec.App.BanquePiguet
{

	static class Program
	{

		[System.STAThread]
		static void Main(string[] args)
		{
			Program.SetupExceptionHandlers();

			try
			{
				UI.Initialize ();

				using (Application application = new Application (args.Contains ("-admin")))
				{
					application.Window.Show ();
					application.Window.Run ();
				}
				UI.ShutDown ();
			}
			catch (System.Exception e)
			{
				Tools.Error (e);
			}
		}

		static void SetupExceptionHandlers()
		{
			System.AppDomain.CurrentDomain.UnhandledException += (sender, e) => Tools.Error (e.ExceptionObject as System.Exception);
			System.Windows.Forms.Application.ThreadException += (sender, e) => Tools.Error (e.Exception);
			System.Windows.Forms.Application.SetUnhandledExceptionMode (System.Windows.Forms.UnhandledExceptionMode.CatchException);
		}


	}

}

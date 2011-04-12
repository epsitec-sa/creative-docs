//	Copyright © 2008-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Debug;
using Epsitec.Common.Splash;

using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core
{
	public static class CoreProgram
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[System.STAThread]
		public static void Main(string[] args)
		{
			if (args.Length > 1)
			{
				CoreProgramOperations.ProcessCommandLine (args.Skip (1).ToArray ());
			}
			else
			{
				CoreProgram.ExecuteCoreProgram ();
			}
		}
		
        private static void ExecuteCoreProgram()
		{
//-			Data.Test.Example1 ();
			
			GeneralExceptionCatcher.Setup ();
			UI.Initialize ();

			using (var app = new CoreApplication ())
			{
				System.Diagnostics.Debug.Assert (app.ResourceManagerPool.PoolName == "Core");

				app.SetupApplication ();

				SplashScreen.DismissSplashScreen ();

				var user = app.UserManager.FindActiveUser ();

				if (app.UserManager.Authenticate (app, user, softwareStartup: true))
				{
					app.Window.Show ();
					app.Window.Run ();
				}

				UI.ShutDown ();
			}
		}
	}
}

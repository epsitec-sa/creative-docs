//	Copyright © 2008-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Debug;
using Epsitec.Common.Splash;
using Epsitec.Common.Types.Collections;

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
			GeneralExceptionCatcher.Setup ();
			
			CoreContext.ParseOptionalSettingsFile (CoreProgram.ReadOptionalSettingsFile ());

			if ((args.Length > 0) &&
				(args[0] != "-start"))
			{
				CoreProgramOperations.ProcessCommandLine (args);
			}
			else
			{
				CoreProgram.ExecuteCoreProgram ();
			}
		}

		private static IEnumerable<string> ReadOptionalSettingsFile()
		{
			var file = System.Reflection.Assembly.GetExecutingAssembly ().Location;
			var dir  = System.IO.Path.GetDirectoryName (file);
			var name = System.IO.Path.GetFileNameWithoutExtension (file);
			var path = System.IO.Path.Combine (dir, name + ".crconfig");

			if (System.IO.File.Exists (path))
			{
				return System.IO.File.ReadLines (path, System.Text.Encoding.Default);
			}
			else
			{
				return EmptyEnumerable<string>.Instance;
			}
		}
		
		private static void ExecuteCoreProgram()
		{
			Library.CoreContext.StartAsInteractive ();
			Library.UI.Services.Initialize ();

			using (var app = new CoreApplication ())
			{
				System.Diagnostics.Debug.Assert (app.ResourceManagerPool.PoolName == "Core");

				app.SetupApplication ();

				SplashScreen.DismissSplashScreen ();

				var user = app.UserManager.FindActiveSystemUser ();

				if (app.UserManager.Authenticate (app, user, softwareStartup: true))
				{
					app.Window.Show ();
					app.Window.Run ();
				}

				Library.UI.Services.ShutDown ();
			}
		}
	}
}

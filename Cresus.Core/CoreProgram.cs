//	Copyright © 2008-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Debug;
using Epsitec.Common.Splash;

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
            GeneralExceptionCatcher.Setup();
            GeneralExceptionCatcher.AbortOnException = true;

            Library.CoreContext.ParseOptionalSettingsFile(Library.CoreContext.ReadCoreContextSettingsFile());

            if ((args.Length > 0) && (args[0] != "-start"))
            {
                CoreProgramOperations.ProcessCommandLine(args);
            }
            else
            {
                CoreProgram.ExecuteCoreProgram();
            }
        }

        private static void ExecuteCoreProgram()
        {
            Library.CoreContext.StartAsInteractive();
            Library.UI.Services.Initialize();

            var snapshotService = Library.CoreContext.EnableSnapshotService
                ? new Library.Business.CoreSnapshotService()
                : null;

            using var app = Library.CoreContext.CreateApplication<Library.CoreInteractiveApp>() ?? new CoreApplication();
            System.Diagnostics.Debug.Assert(app.ResourceManagerPool.PoolName == "Core");

            app.SetupApplication();

            if (snapshotService != null)
            {
                snapshotService.NotifyApplicationStarted(app);
            }

            SplashScreen.DismissSplashScreen();

            if (app.StartupLogin())
            {
                app.Window.Show();
                app.Window.Run();
            }

            Library.UI.Services.ShutDown();
        }
    }
}

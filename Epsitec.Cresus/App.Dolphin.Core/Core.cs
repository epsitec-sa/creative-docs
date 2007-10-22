using System;
using System.Collections.Generic;
using System.Text;

namespace Epsitec.App.Dolphin
{
	public class Core : Opac.Shell.Interfaces.IApplication
	{
		public Core()
		{
		}

		#region IApplication Members

		public void DisposeApplication(Opac.Shell.Interfaces.IShellContext context, bool exceptionOccurred)
		{
			this.application.Dispose ();
		}

		public void LoadApplication(Opac.Shell.Interfaces.IShellContext context)
		{
			Epsitec.Common.Widgets.Widget.Initialize ();
			Epsitec.Common.Document.Engine.Initialize ();

			string[] args = System.Environment.GetCommandLineArgs ();
			Epsitec.Common.Support.ResourceManagerPool pool = new Epsitec.Common.Support.ResourceManagerPool ("App.Dolphin");
			pool.DefaultPrefix = "file";
			pool.SetupDefaultRootPaths ();

			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookSimply");

			this.application = new DolphinApplication (pool, args);
			
		}

		public void RunApplication(Opac.Shell.Interfaces.IShellContext context)
		{
			this.application.Show (null);
			this.application.Window.Show ();
			this.application.Window.MakeActive ();

			this.application.PumpMessageLoop ();

			context.NotifyApplicationReady ();

			this.application.RunMessageLoop ();
		}

		#endregion

		DolphinApplication application;
	}
}

//	Copyright © 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.App.DocumentEditor.Installer
{
	/// <summary>
	/// Summary description for CustomInstaller.
	/// </summary>
	[System.ComponentModel.RunInstaller(true)]
	public sealed class CustomInstaller : System.Configuration.Install.Installer
	{
		public CustomInstaller()
		{
			this.Installers.Add (new Serial ());
		}
		
		public class Serial : System.Configuration.Install.Installer
		{
			
			public override void Install(System.Collections.IDictionary stateSaver)
			{
				base.Install (stateSaver);
				
				SerialInput form = new SerialInput ();
				
				form.ShowDialog ();
				
				if (! form.SerialOk)
				{
					throw new System.Configuration.Install.InstallException ("L'installation a été interrompue...");
				}
			}
			
			public override void Uninstall(System.Collections.IDictionary savedState)
			{
				base.Uninstall (savedState);
			}

			public override void Commit(System.Collections.IDictionary savedState)
			{
				base.Commit (savedState);
			}

			public override void Rollback(System.Collections.IDictionary savedState)
			{
				base.Rollback (savedState);
			}
		}
	}
}

//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Server
{
	using ServiceProcessInstaller = System.ServiceProcess.ServiceProcessInstaller;
	using ServiceInstaller        = System.ServiceProcess.ServiceInstaller;
	using ServiceController       = System.ServiceProcess.ServiceController;
	using ServiceControllerStatus = System.ServiceProcess.ServiceControllerStatus;
	
	/// <summary>
	/// Summary description for CustomInstaller.
	/// </summary>
	[System.ComponentModel.RunInstaller(true)]
	public sealed class CustomInstaller : System.Configuration.Install.Installer
	{
		public CustomInstaller()
		{
			this.service_process_installer = new System.ServiceProcess.ServiceProcessInstaller ();
			this.service_installer         = new System.ServiceProcess.ServiceInstaller ();
			this.service_startup_installer = new ServiceStartupInstaller (GlobalNames.ServiceName);
			
			this.service_process_installer.Account  = System.ServiceProcess.ServiceAccount.LocalService;
			this.service_process_installer.Password = null;
			this.service_process_installer.Username = null;
			
			this.service_installer.DisplayName = "Crésus Serveur";
			this.service_installer.ServiceName = GlobalNames.ServiceName;
			this.service_installer.StartType   = System.ServiceProcess.ServiceStartMode.Automatic;
			
			this.Installers.Add (this.service_process_installer);
			this.Installers.Add (this.service_installer);
			this.Installers.Add (this.service_startup_installer);
		}
		
		
		#region ServiceStartupInstaller Class
		public class ServiceStartupInstaller : System.Configuration.Install.Installer
		{
			public ServiceStartupInstaller(string name)
			{
				this.service_controller = new System.ServiceProcess.ServiceController (name);
			}
			
			
			public override void Install(System.Collections.IDictionary state)
			{
				base.Install (state);
				
				try
				{
					this.service_controller.Start ();
					this.service_controller.WaitForStatus (ServiceControllerStatus.Running, this.timeout);
				}
				catch (System.ServiceProcess.TimeoutException)
				{
					System.Diagnostics.Debug.WriteLine ("Cannot start service within allotted time.");
				}
				catch (System.Exception ex)
				{
					System.Diagnostics.Debug.WriteLine (ex.Message);
				}
			}
			
			public override void Uninstall(System.Collections.IDictionary state)
			{
				base.Uninstall (state);
				
				try
				{
					this.service_controller.Stop ();
					this.service_controller.WaitForStatus (ServiceControllerStatus.Stopped, this.timeout);
				}
				catch (System.ServiceProcess.TimeoutException)
				{
					System.Diagnostics.Debug.WriteLine ("Cannot stop service within allotted time.");
				}
				catch (System.Exception ex)
				{
					System.Diagnostics.Debug.WriteLine (ex.Message);
				}
			}
			
			public override void Commit(System.Collections.IDictionary state)
			{
				base.Commit (state);
			}
			
			public override void Rollback(System.Collections.IDictionary state)
			{
				base.Rollback (state);
				
				try
				{
					this.service_controller.Stop ();
					this.service_controller.WaitForStatus (ServiceControllerStatus.Stopped, this.timeout);
				}
				catch (System.ServiceProcess.TimeoutException)
				{
					System.Diagnostics.Debug.WriteLine ("Cannot stop service within allotted time.");
				}
				catch (System.Exception ex)
				{
					System.Diagnostics.Debug.WriteLine (ex.Message);
				}
			}
			
			
			private ServiceController			service_controller;
			private System.TimeSpan				timeout = new System.TimeSpan (0, 0, 0, 0, 5000);
		}
		#endregion
		
		private ServiceProcessInstaller			service_process_installer;
		private ServiceInstaller				service_installer;
		private ServiceStartupInstaller			service_startup_installer;
	}
}

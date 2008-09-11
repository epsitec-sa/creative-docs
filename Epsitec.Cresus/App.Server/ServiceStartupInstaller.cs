//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.ServiceProcess;

namespace Epsitec.Cresus.Server
{
	/// <summary>
	/// The <c>ServiceStartupInstaller</c> class starts and stops the specified
	/// service when the installer installs or uninstalls the service.
	/// </summary>
	public class ServiceStartupInstaller : System.Configuration.Install.Installer
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceStartupInstaller"/> class.
		/// </summary>
		/// <param name="serviceName">Name of the service.</param>
		public ServiceStartupInstaller(string serviceName)
		{
			this.serviceController = new ServiceController (serviceName);
			this.timeout = new System.TimeSpan (0, 0, 0, 5);
		}
		
		
		public override void Install(System.Collections.IDictionary state)
		{
			base.Install (state);
			
			try
			{
				this.serviceController.Start ();
				this.serviceController.WaitForStatus (ServiceControllerStatus.Running, this.timeout);
			}
			catch (TimeoutException)
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
				this.serviceController.Stop ();
				this.serviceController.WaitForStatus (ServiceControllerStatus.Stopped, this.timeout);
			}
			catch (TimeoutException)
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
				this.serviceController.Stop ();
				this.serviceController.WaitForStatus (ServiceControllerStatus.Stopped, this.timeout);
			}
			catch (TimeoutException)
			{
				System.Diagnostics.Debug.WriteLine ("Cannot stop service within allotted time.");
			}
			catch (System.Exception ex)
			{
				System.Diagnostics.Debug.WriteLine (ex.Message);
			}
		}
		
		
		private readonly ServiceController	serviceController;
		private readonly System.TimeSpan	timeout;
	}
}

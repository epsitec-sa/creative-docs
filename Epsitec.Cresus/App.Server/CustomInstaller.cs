//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.ServiceProcess;

namespace Epsitec.Cresus.Server
{
	/// <summary>
	/// The <c>CustomInstaller</c> class registers the Firebird-based Crésus service,
	/// which runs as a Windows Service.
	/// </summary>
	[System.ComponentModel.RunInstaller(true)]
	public sealed class CustomInstaller : System.Configuration.Install.Installer
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CustomInstaller"/> class.
		/// </summary>
		public CustomInstaller()
		{
			this.serviceProcessInstaller = new ServiceProcessInstaller ()
			{
				Account = ServiceAccount.LocalService,
				Password = null,
				Username = null
			};

			this.serviceInstaller = new ServiceInstaller ()
			{
				DisplayName = Properties.ServiceResources.ServiceDisplayName,
				ServiceName = GlobalNames.ServiceName,
				StartType   = ServiceStartMode.Automatic
			};

			this.serviceStartupInstaller = new ServiceStartupInstaller (GlobalNames.ServiceName);
			
//			this.serviceInstaller.ServicesDependedOn = new string[] { GlobalNames.FirebirdInstance };

			//	Installation consists of the following steps :
			
			this.Installers.Add (this.serviceProcessInstaller);		//	register the executable containing the service(s)
			this.Installers.Add (this.serviceInstaller);			//	register the service itself
			this.Installers.Add (this.serviceStartupInstaller);		//	start the registered service
		}
		
		
		private ServiceProcessInstaller			serviceProcessInstaller;
		private ServiceInstaller				serviceInstaller;
		private ServiceStartupInstaller			serviceStartupInstaller;
	}
}

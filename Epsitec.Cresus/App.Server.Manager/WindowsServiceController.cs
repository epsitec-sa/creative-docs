//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.ServerManager
{
	using ServiceController       = System.ServiceProcess.ServiceController;
	using ServiceControllerStatus = System.ServiceProcess.ServiceControllerStatus;

	/// <summary>
	/// The <c>WindowsServiceController</c> class provides the basic interface to start
	/// and stop the Crésus Server instance.
	/// </summary>
	public class WindowsServiceController
	{
		public WindowsServiceController()
		{
			this.serviceController = new System.ServiceProcess.ServiceController (Epsitec.Cresus.Server.GlobalNames.ServiceName);
		}


		/// <summary>
		/// Gets a value indicating whether the server is running.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the server is running; otherwise, <c>false</c>.
		/// </value>
		public bool								IsRunning
		{
			get
			{
				try
				{
					this.serviceController.Refresh ();
					return this.serviceController.Status == ServiceControllerStatus.Running;
				}
				catch
				{
					return false;
				}
			}
		}


		/// <summary>
		/// Starts the server.
		/// </summary>
		public void Start()
		{
			this.serviceController.Start ();
		}

		/// <summary>
		/// Stops the server.
		/// </summary>
		public void Stop()
		{
			this.serviceController.Stop ();
		}
		
		
		private ServiceController				serviceController;
	}
}

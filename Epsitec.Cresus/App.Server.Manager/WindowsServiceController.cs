//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.ServerManager
{
	using ServiceController       = System.ServiceProcess.ServiceController;
	using ServiceControllerStatus = System.ServiceProcess.ServiceControllerStatus;
	
	/// <summary>
	/// Summary description for WindowsServiceController.
	/// </summary>
	public class WindowsServiceController
	{
		public WindowsServiceController()
		{
			string name = "CresusServer";
			
			this.service_controller = new System.ServiceProcess.ServiceController (name);
		}
		
		
		public bool								IsRunning
		{
			get
			{
				this.service_controller.Refresh ();
				return this.service_controller.Status == ServiceControllerStatus.Running;
			}
		}
		
		
		public void Start()
		{
			this.service_controller.Start ();
		}
		
		public void Stop()
		{
			this.service_controller.Stop ();
		}
		
		
		private ServiceController				service_controller;
	}
}

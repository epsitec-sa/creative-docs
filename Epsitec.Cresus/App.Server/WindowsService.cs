//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Server
{
	using PowerBroadcastStatus = System.ServiceProcess.PowerBroadcastStatus;
	
	/// <summary>
	/// Summary description for WindowsService.
	/// </summary>
	public class WindowsService : System.ServiceProcess.ServiceBase
	{
		public WindowsService()
		{
			this.AutoLog = false;
			
			this.CanShutdown         = true;
			this.CanStop             = true;
			this.CanPauseAndContinue = false;
			this.CanHandlePowerEvent = true;
			
			this.ServiceName = GlobalNames.ServiceName;
		}
		
		
		private static void Main()
		{
			System.ServiceProcess.ServiceBase[] services_to_run;
			
			services_to_run = new System.ServiceProcess.ServiceBase[] { new WindowsService () };
			
			System.ServiceProcess.ServiceBase.Run (services_to_run);
		}
		
		
		protected override void OnStart(string[] args)
		{
			base.OnStart (args);
			this.StartServices ();
			EventLog.WriteEntry ("Service started successfully.", System.Diagnostics.EventLogEntryType.Information);
		}
		
		protected override void OnStop()
		{
			base.OnStop ();
			this.StopServices ();
			EventLog.WriteEntry ("Service stopped.", System.Diagnostics.EventLogEntryType.Information);
		}
		
		protected override void OnShutdown()
		{
			base.OnShutdown ();
			this.StopServices ();
			EventLog.WriteEntry ("Shutting down.", System.Diagnostics.EventLogEntryType.Information);
		}
		
		protected override bool OnPowerEvent(System.ServiceProcess.PowerBroadcastStatus status)
		{
			EventLog.WriteEntry ("Got power status: " + status.ToString (), System.Diagnostics.EventLogEntryType.Information);
			
			switch (status)
			{
				case PowerBroadcastStatus.QuerySuspend:
				case PowerBroadcastStatus.QuerySuspendFailed:
				case PowerBroadcastStatus.ResumeAutomatic:
				case PowerBroadcastStatus.ResumeSuspend:
				case PowerBroadcastStatus.ResumeCritical:
				case PowerBroadcastStatus.Suspend:
					break;
			}
			
			return base.OnPowerEvent (status);
		}
		
		private void StartServices()
		{
			System.Diagnostics.Debug.WriteLine ("Cresus Server: starting.");
//-			System.Threading.Thread.Sleep (20*1000);
			
			Database.DbAccess access = this.CreateAccess ();
			
			this.infrastructure = new Epsitec.Cresus.Database.DbInfrastructure ();
			this.infrastructure.AttachDatabase (access);
			
			this.infrastructure.LocalSettings.IsServer = true;
			this.orchestrator = new Epsitec.Cresus.Requests.Orchestrator (this.infrastructure);
			this.engine = new Epsitec.Cresus.Services.Engine (this.orchestrator, 1234);
			this.infrastructure.LocalSettings.IsServer = false;
			
			System.Diagnostics.Debug.WriteLine ("Cresus Server: running.");
		}
		
		private void StopServices()
		{
			if (this.infrastructure != null)
			{
				System.Diagnostics.Debug.WriteLine ("Cresus Server: stopping.");
				
				Common.Support.Globals.SignalAbort ();
				
				this.orchestrator.Dispose ();
				this.infrastructure.Dispose ();
				
				System.Diagnostics.Debug.WriteLine ("Cresus Server: stopped.");
				
				this.infrastructure = null;
				this.orchestrator   = null;
				this.engine         = null;
			}
		}
		
		private Database.DbAccess CreateAccess()
		{
			Database.DbAccess db_access = new Database.DbAccess ();
			
			db_access.Provider		= "Firebird";
			db_access.LoginName		= "sysdba";
			db_access.LoginPassword = "masterkey";
			db_access.Database		= "fiche";
			db_access.Server		= "localhost";
			db_access.Create		= false;
			
			return db_access;
		}
		
		
		private Database.DbInfrastructure		infrastructure;
		private Requests.Orchestrator			orchestrator;
		private Services.Engine					engine;
	}
}

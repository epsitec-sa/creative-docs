//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			System.Diagnostics.Debug.WriteLine ("OnStart called: " + string.Join ("; ", args));
			base.OnStart (args);
			this.StartServices ();
			System.Diagnostics.Debug.WriteLine ("OnStart done");
			EventLog.WriteEntry ("Service started successfully.", System.Diagnostics.EventLogEntryType.Information);
		}
		
		protected override void OnStop()
		{
			System.Diagnostics.Debug.WriteLine ("OnStop called");
			base.OnStop ();
			this.StopServices ();
			System.Diagnostics.Debug.WriteLine ("OnStop done");
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
			
			this.infrastructure = this.GetDatabaseInfrastructure ();
			
			System.Diagnostics.Debug.Assert (this.infrastructure.LocalSettings.IsServer);
			System.Diagnostics.Debug.Assert (this.infrastructure.LocalSettings.ClientId == 1);
			
			this.engine = new Epsitec.Cresus.Services.Engine (this.infrastructure, 1234);
			
			System.Diagnostics.Debug.WriteLine ("Cresus Server: running.");
		}
		
		private void StopServices()
		{
			if (this.infrastructure != null)
			{
				System.Diagnostics.Debug.WriteLine ("Cresus Server: stopping.");
				
				Common.Support.Globals.SignalAbort ();
				
				this.engine.Dispose ();
				this.infrastructure.Dispose ();
				
				System.Diagnostics.Debug.WriteLine ("Cresus Server: stopped.");
				
				this.engine         = null;
				this.infrastructure = null;
			}
		}
		
		
		private Database.DbInfrastructure GetDatabaseInfrastructure()
		{
			Database.DbInfrastructure infrastructure = new Database.DbInfrastructure ();
			Database.DbAccess         access         = WindowsService.CreateAccess ();
			
			try
			{
				System.Diagnostics.Debug.WriteLine ("Trying to open database.");
				infrastructure.AttachDatabase (access);
			}
			catch
			{
				infrastructure.Dispose ();
				infrastructure = new Database.DbInfrastructure ();
				
				try
				{
					System.Diagnostics.Debug.WriteLine ("Creating database.");
					InitialDatabase.Create (this);
					System.Diagnostics.Debug.WriteLine ("Trying to open database (2).");
					infrastructure.AttachDatabase (access);
				}
				catch (System.Exception ex)
				{
					System.Diagnostics.Debug.WriteLine ("Database could not be opened.");
					System.Diagnostics.Debug.WriteLine (ex.Message);
					System.Diagnostics.Debug.WriteLine (ex.StackTrace);
					infrastructure.Dispose ();
					infrastructure = null;
				}
			}
			
			return infrastructure;
		}
		
		internal static Database.DbAccess CreateAccess()
		{
			string provider		 = System.Configuration.ConfigurationManager.AppSettings["DatabaseProvider"];
			string database		 = System.Configuration.ConfigurationManager.AppSettings["DatabaseSource"];
			string server		 = System.Configuration.ConfigurationManager.AppSettings["DatabaseServer"];
			string loginName	 = System.Configuration.ConfigurationManager.AppSettings["DatabaseUserName"];
			string loginPassword = System.Configuration.ConfigurationManager.AppSettings["DatabaseUserPass"];
			
			return new Database.DbAccess (provider, database, server, loginName, loginPassword, false);
		}
		
		
		private Database.DbInfrastructure		infrastructure;
		private Services.Engine					engine;
	}
}

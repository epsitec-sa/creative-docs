//	Copyright � 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
#if false
			Database.DbInfrastructure infrastructure = DatabaseTools.GetDatabase (null);

			System.Diagnostics.Debug.Assert (infrastructure.LocalSettings.IsServer);
			System.Diagnostics.Debug.Assert (infrastructure.LocalSettings.ClientId == 1);

			Epsitec.Cresus.Services.Engine engine = new Epsitec.Cresus.Services.Engine (infrastructure, 1234);
#else
			System.ServiceProcess.ServiceBase[] services_to_run;
			
			services_to_run = new System.ServiceProcess.ServiceBase[] { new WindowsService () };
			
			System.ServiceProcess.ServiceBase.Run (services_to_run);
#endif
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
//-			this.StopServices ();
			this.Stop ();
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
//			System.Diagnostics.Debugger.Break ();
			
			this.infrastructure = DatabaseTools.GetDatabase (this.EventLog);
			
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
		
		
		private Database.DbInfrastructure		infrastructure;
		private Services.Engine					engine;
	}
}

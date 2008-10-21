//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Server
{
	partial class WindowsService
	{
		/// <summary>
		/// Starts the database engine.
		/// </summary>
		private void StartDatabaseEngine()
		{
//-			System.Diagnostics.Debug.WriteLine ("Cresus Server: starting.");
//-			System.Diagnostics.Debugger.Break ();
			
			this.infrastructure = DatabaseTools.GetDatabase (this.EventLog);
			
			System.Diagnostics.Debug.Assert (this.infrastructure.LocalSettings.IsServer);
			System.Diagnostics.Debug.Assert (this.infrastructure.LocalSettings.ClientId == 1);
			
			this.engine = new Epsitec.Cresus.Services.Engine (this.infrastructure, 1234);
			
//-			System.Diagnostics.Debug.WriteLine ("Cresus Server: running.");
		}

		/// <summary>
		/// Stops the database engine.
		/// </summary>
		private void StopDatabaseEngine()
		{
			if (this.infrastructure != null)
			{
//-				System.Diagnostics.Debug.WriteLine ("Cresus Server: stopping.");
				
				Common.Support.Globals.SignalAbort ();
				
				this.engine.Dispose ();
				this.infrastructure.Dispose ();
				
//-				System.Diagnostics.Debug.WriteLine ("Cresus Server: stopped.");
				
				this.engine         = null;
				this.infrastructure = null;
			}
		}
		
		
		private Database.DbInfrastructure		infrastructure;
		private Services.Engine					engine;
	}
}

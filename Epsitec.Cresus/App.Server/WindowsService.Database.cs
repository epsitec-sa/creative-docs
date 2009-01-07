//	Copyright © 2008-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
			this.infrastructure = DatabaseTools.GetDatabase (this.EventLog);
			
			System.Diagnostics.Debug.Assert (this.infrastructure.LocalSettings.IsServer);
			System.Diagnostics.Debug.Assert (this.infrastructure.LocalSettings.ClientId == 1);

			this.host = new Epsitec.Cresus.Services.EngineHost (1234);
			this.engine = new Epsitec.Cresus.Services.Engine (this.infrastructure, System.Guid.Empty);
			this.host.AddEngine (this.engine);
		}

		/// <summary>
		/// Stops the database engine.
		/// </summary>
		private void StopDatabaseEngine()
		{
			if (this.infrastructure != null)
			{
				Common.Support.Globals.SignalAbort ();

				this.engine.Dispose ();
				this.host.Dispose ();
				this.infrastructure.Dispose ();

				this.host           = null;
				this.engine         = null;
				this.infrastructure = null;
			}
		}
		
		
		private Database.DbInfrastructure		infrastructure;
		private Services.Engine					engine;
		private Services.EngineHost				host;
	}
}

//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Configuration;

namespace Epsitec.Cresus.Server
{
	/// <summary>
	/// The <c>DatabaseTools</c> class knows how to create the initial, empty database,
	/// shared by all Crésus applications.
	/// </summary>
	internal static class DatabaseTools
	{
		/// <summary>
		/// Gets the database infrastructure object.
		/// </summary>
		/// <param name="eventLog">The event log.</param>
		/// <returns>The database infrastructure, or <c>null</c> in case of failure.</returns>
		public static Database.DbInfrastructure GetDatabase(System.Diagnostics.EventLog eventLog)
		{
			Database.DbInfrastructure infrastructure = new Database.DbInfrastructure ();
			Database.DbAccess         access         = DatabaseTools.GetAccess ();

			try
			{
				System.Diagnostics.Debug.WriteLine ("Trying to open database.");
				infrastructure.AttachToDatabase (access);
			}
			catch
			{
				infrastructure.Dispose ();
				infrastructure = new Database.DbInfrastructure ();

				try
				{
					System.Diagnostics.Debug.WriteLine ("Creating database.");
					DatabaseTools.CreateEmptyDatabase (eventLog);
					System.Diagnostics.Debug.WriteLine ("Trying to open database (2).");
					infrastructure.AttachToDatabase (access);
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

		/// <summary>
		/// Creates the empty database.
		/// </summary>
		/// <param name="eventLog">The event log.</param>
		public static void CreateEmptyDatabase(System.Diagnostics.EventLog eventLog)
		{
			Database.DbInfrastructure infrastructure = new Database.DbInfrastructure ();
			Database.DbAccess         access         = DatabaseTools.GetAccess ();
			
			infrastructure.CreateDatabase (access);
			
			eventLog.WriteEntry ("Created empty database.", System.Diagnostics.EventLogEntryType.Warning);
			
			infrastructure.LocalSettings.IsServer = true;
			infrastructure.LocalSettings.ClientId = 1;
			
			using (Database.DbTransaction transaction = infrastructure.BeginTransaction (Database.DbTransactionMode.ReadWrite))
			{
				infrastructure.LocalSettings.PersistToBase (transaction);
				transaction.Commit ();
			}
			
			infrastructure.Dispose ();
		}

		/// <summary>
		/// Gets the database access.
		/// </summary>
		/// <returns>The database access.</returns>
		public static Database.DbAccess GetAccess()
		{
			string provider		 = ConfigurationManager.AppSettings["DatabaseProvider"];
			string database		 = ConfigurationManager.AppSettings["DatabaseSource"];
			string server		 = ConfigurationManager.AppSettings["DatabaseServer"];
			string loginName	 = ConfigurationManager.AppSettings["DatabaseUserName"];
			string loginPassword = ConfigurationManager.AppSettings["DatabaseUserPass"];

			return new Database.DbAccess (provider, database, server, loginName, loginPassword, false);
		}
	}
}

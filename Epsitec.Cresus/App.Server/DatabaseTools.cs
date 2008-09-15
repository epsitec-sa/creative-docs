//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Configuration;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.Exceptions;

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
		public static DbInfrastructure GetDatabase(System.Diagnostics.EventLog eventLog)
		{
			DbInfrastructure infrastructure = new DbInfrastructure ();
			DbAccess         access         = DatabaseTools.GetAccess ();

			try
			{
				System.Diagnostics.Debug.WriteLine ("Trying to open database.");
				infrastructure.AttachToDatabase (access);
			}
			catch (EmptyDatabaseException ex)
			{
				infrastructure.Dispose ();
				DbTools.DeleteDatabase (access);
				infrastructure = DatabaseTools.GetDatabaseAttempt2 (eventLog, infrastructure, access);
			}
			catch
			{
				infrastructure.Dispose ();
				infrastructure = DatabaseTools.GetDatabaseAttempt2 (eventLog, infrastructure, access);
			}

			return infrastructure;
		}

		private static DbInfrastructure GetDatabaseAttempt2(System.Diagnostics.EventLog eventLog, DbInfrastructure infrastructure, DbAccess access)
		{
			infrastructure = new DbInfrastructure ();

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
			
			return infrastructure;
		}

		/// <summary>
		/// Creates the empty database.
		/// </summary>
		/// <param name="eventLog">The event log.</param>
		public static void CreateEmptyDatabase(System.Diagnostics.EventLog eventLog)
		{
			DbInfrastructure infrastructure = new DbInfrastructure ();
			DbAccess         access         = DatabaseTools.GetAccess ();

			infrastructure.CreateDatabase (access);

			eventLog.WriteEntry ("Created empty database.", System.Diagnostics.EventLogEntryType.Warning);

			infrastructure.LocalSettings.IsServer = true;
			infrastructure.LocalSettings.ClientId = 1;

			using (DbTransaction transaction = infrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
			{
				infrastructure.LocalSettings.PersistToBase (transaction);
				transaction.Commit ();
			}

			infrastructure.Dispose ();
		}

		/// <summary>
		/// Deletes the database.
		/// </summary>
		/// <param name="eventLog">The event log.</param>
		public static void DeleteDatabase(System.Diagnostics.EventLog eventLog)
		{
			DbInfrastructure infrastructure = new DbInfrastructure ();
			DbAccess         access         = DatabaseTools.GetAccess ();

			infrastructure.CreateDatabase (access);

			eventLog.WriteEntry ("Created empty database.", System.Diagnostics.EventLogEntryType.Warning);

			infrastructure.LocalSettings.IsServer = true;
			infrastructure.LocalSettings.ClientId = 1;

			using (DbTransaction transaction = infrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
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
		public static DbAccess GetAccess()
		{
			string provider		 = ConfigurationManager.AppSettings["DatabaseProvider"];
			string database		 = ConfigurationManager.AppSettings["DatabaseSource"];
			string server		 = ConfigurationManager.AppSettings["DatabaseServer"];
			string loginName	 = ConfigurationManager.AppSettings["DatabaseUserName"];
			string loginPassword = ConfigurationManager.AppSettings["DatabaseUserPass"];

			return new DbAccess (provider, database, server, loginName, loginPassword, false);
		}
	}
}

//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Schema;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Dialogs;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Factories;

namespace Epsitec.Cresus.Core.Data
{
	/// <summary>
	/// The <c>Infrastructure</c> class handles the connection to the database (i.e. setting
	/// up the lower level <see cref="DataInfrastructure"/> class and making sure that the
	/// database exists and has the proper schema).
	/// </summary>
	public sealed class Infrastructure : CoreDataComponent, IIsDisposed
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Infrastructure"/> class.
		/// This will initialize the connection to the database and setup the lower
		/// level <see cref="DataInfrastructure"/> class.
		/// </summary>
		/// <param name="data">The data.</param>
		public Infrastructure(CoreData data)
			: base (data)
		{
			this.dataInfrastructure = this.SetupDatabase ();
			this.Host.NotifyDatabaseReady ();
		}

		public DataInfrastructure				DataInfrastructure
		{
			get
			{
				return this.dataInfrastructure;
			}	
		}

		#region IIsDisposed Members

		public bool IsDisposed
		{
			get
			{
				return this.isDisposed;
			}
		}

		#endregion
	
		public override void ExecuteSetupPhase()
		{
			base.ExecuteSetupPhase ();
			
			this.connectionManager = this.Host.ConnectionManager;
		}

		internal static bool CheckDatabaseExistence(DbAccess access)
		{
			return DbInfrastructure.CheckDatabaseExistence (access);
		}

		public static void DropDatabase(DbAccess access)
		{
			DbInfrastructure.DropDatabase (access);
		}

		private DataInfrastructure SetupDatabase()
		{
			var access = CoreData.GetDatabaseAccess ();
			var managedEntityTypeIds = Infrastructure.GetManagedEntityIds ();

			bool forceDatabaseCreation = this.Host.ForceDatabaseCreation;
			bool allowDatabaseUpdate = this.Host.AllowDatabaseUpdate;

			try
			{
				bool databaseExists = Infrastructure.CheckDatabaseExistence (access);

				if (forceDatabaseCreation && databaseExists)
				{
					Infrastructure.DropDatabase (access);
				}

				bool createDatabase = forceDatabaseCreation || !databaseExists;

				if (createDatabase)
				{
					this.dbInfrastructure = Infrastructure.CreateDatabase (access);
					Infrastructure.CreateDatabaseSchemas (this.dbInfrastructure, managedEntityTypeIds);
				}
				else
				{
					this.dbInfrastructure = Infrastructure.ConnectToDatabase (access);

					bool valid = Infrastructure.VerifyDatabaseSchemas (this.dbInfrastructure, managedEntityTypeIds);

					if (!valid)
					{
						bool updateAllowed = Infrastructure.IsDatabasesSchemaUpdateAllowed (allowDatabaseUpdate);

						bool updateSuccess = false;

						if (updateAllowed)
						{
							updateSuccess = Infrastructure.UpdateDatabaseSchemas (this.dbInfrastructure, managedEntityTypeIds);
						}

						if (!updateAllowed)
						{
							throw new System.Exception ("Database schema is incompatible.");
						}
						else if (!updateSuccess)
						{
							throw new System.Exception ("Error while updating database schema.");
						}
					}
				}

				return Infrastructure.ConnectToDatabase (this.dbInfrastructure, managedEntityTypeIds);
			}
			catch (System.Exception ex)
			{
				Library.UI.Services.ShowErrorMessage
				(
					Epsitec.Cresus.Core.Library.Data.Res.Strings.Error.CannotConnectToLocalDatabase,
					Epsitec.Cresus.Core.Library.Data.Res.Strings.Hint.Error.CannotConnectToLocalDatabase,
					ex
				);

				System.Environment.Exit (0);
				throw;
			}
		}

		private static DbInfrastructure CreateDatabase(DbAccess access)
		{
			DbInfrastructure dbInfrastructure = new DbInfrastructure ();

			dbInfrastructure.CreateDatabase (access);

			return dbInfrastructure;
		}

		private static DbInfrastructure ConnectToDatabase(DbAccess access)
		{
			DbInfrastructure dbInfrastructure = new DbInfrastructure ();

			dbInfrastructure.AttachToDatabase (access);

			return dbInfrastructure;
		}

		private static void CreateDatabaseSchemas(DbInfrastructure dbInfrastructure, IEnumerable<Druid> managedEntityTypeIds)
		{
			EntityEngine.Create (dbInfrastructure, managedEntityTypeIds);
		}

		private static bool VerifyDatabaseSchemas(DbInfrastructure dbInfrastructure, IEnumerable<Druid> managedEntityTypeIds)
		{
			return EntityEngine.Check (dbInfrastructure, managedEntityTypeIds);
		}

		private static bool IsDatabasesSchemaUpdateAllowed(bool allowDatabaseUpdate)
		{
			bool updateAllowed = false;

			if (allowDatabaseUpdate)
			{
				IDialog d = MessageDialog.CreateYesNo ("Base de donnée incompatible", DialogIcon.Warning, "La base de donnée est incompatible. Voulez vous la modifier pour la mettre à jour?");

				d.OpenDialog ();

				if (d.Result == DialogResult.Yes)
				{
					updateAllowed = true;
				}
			}

			return updateAllowed;
		}

		private static bool UpdateDatabaseSchemas(DbInfrastructure dbInfrastructure, IEnumerable<Druid> managedEntityTypeIds)
		{
			try
			{
				EntityEngine.Update (dbInfrastructure, managedEntityTypeIds);

				return true;
			}
			catch (System.Exception e)
			{
				System.Diagnostics.Debug.WriteLine ("Error while updating database schema: " + e.Message);

				return false;
			}
		}

		private static DataInfrastructure ConnectToDatabase(DbInfrastructure dbInfrastructure, IEnumerable<Druid> managedEntityTypeIds)
		{
			EntityEngine entityEngine = EntityEngine.Connect (dbInfrastructure, managedEntityTypeIds);

			return new DataInfrastructure (dbInfrastructure, entityEngine);
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (!this.isDisposed)
			{
				if (this.dataInfrastructure != null)
				{
					this.dataInfrastructure.Dispose ();
				}
				if (this.dbInfrastructure != null)
				{
					this.dbInfrastructure.Dispose ();
				}

				this.isDisposed = true;		
			}
		}

		#endregion
		
		#region Factory Class

		private sealed class Factory : ICoreDataComponentFactory
		{
			#region ICoreDataComponentFactory Members

			public bool CanCreate(CoreData data)
			{
				return true;
			}

			public bool ShouldCreate(CoreData host)
			{
				return true;
			}

			public CoreDataComponent Create(CoreData data)
			{
				return new Infrastructure (data);
			}

			public System.Type GetComponentType()
			{
				return typeof (Infrastructure);
			}

			#endregion
		}

		#endregion

		public static IEnumerable<Druid> GetManagedEntityIds()
		{
			return Infrastructure.GetManagedEntityStructuredTypes ().Select (x => x.CaptionId);
		}

		public static IEnumerable<StructuredType> GetManagedEntityStructuredTypes()
		{
			var allEntityIds = EntityInfo.GetAllTypeIds ();

			foreach (var type in allEntityIds.Select (x => EntityInfo.GetStructuredType (x)))
			{
				if (type == null)
				{
					continue;
				}

				if (type.Flags.HasFlag (StructuredTypeFlags.GenerateSchema))
				{
					yield return type;
				}
			}
		}


		private readonly DataInfrastructure		dataInfrastructure;
		private DbInfrastructure				dbInfrastructure;
		private ConnectionManager				connectionManager;
		private bool							isDisposed;
	}
}

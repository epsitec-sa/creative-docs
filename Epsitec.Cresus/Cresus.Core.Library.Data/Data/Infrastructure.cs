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
	public sealed class Infrastructure : CoreDataComponent, IIsDisposed
	{

		// HACK This class has been temporarily hacked because of how things happens in Cresus.Core
		// in order to be retro compatible until things are changed there. The hack in the class is
		// the initialization of the field this.dataInfrastructure done in the constructor that must
		// be removed and the ConnectToDatabase method and its call site that must be reverted back
		// to their original values.
		// Also, in order to remove this hack, things have to be cleaned up, regarding the order
		// in which the components are set up, how they access the DataInfrastructure property of
		// this object, etc. This property should never be called before this object setup method
		// has been called. That's why there is this assertion. So probably that the sequence of
		// CanExecuteSetupPhase and ExecuteSetupPhase methods must be changed in some
		// CoreDataComponents. Moreover, maybe there should be some changes in how stuff is done,
		// like to have a second setup phase for objects that implement ICoreManualComponent, to
		// ensure that this object is properly initialized when they are initialized.
		// Marc

		public Infrastructure(CoreData data)
			: base (data)
		{
			this.dataInfrastructure = new DataInfrastructure (DbAccess.Empty, null);
			this.isDisposed = false;
		}

		public DataInfrastructure DataInfrastructure
		{
			get
			{
				// This assertion will fire as if this property is called but the ExecupteSetupPhase
				// method has not yet been called. See comment above.
				// Marc
				System.Diagnostics.Debug.Assert (this.Host.IsReady);

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

			this.SetupDatabase ();
			this.Host.IsReady = true;
		}

		public void SetupDatabase()
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
					Infrastructure.CreateDatabase (access);
					Infrastructure.CreateDatabaseSchemas (access, managedEntityTypeIds);
				}
				else
				{
					bool valid = Infrastructure.VerifyDatabaseSchemas (access, managedEntityTypeIds);

					if (!valid)
					{
						bool updateAllowed = Infrastructure.IsDatabasesSchemaUpdateAllowed (allowDatabaseUpdate);
						
						bool updateSuccess = false;

						if (updateAllowed)
						{
							updateSuccess = Infrastructure.UpdateDatabaseSchemas (access, managedEntityTypeIds);
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

				this.dataInfrastructure = Infrastructure.ConnectToDatabase (this.dataInfrastructure, access, managedEntityTypeIds);
			}
			catch (System.Exception ex)
			{
				UI.ShowErrorMessage
				(
					Res.Strings.Error.CannotConnectToLocalDatabase,
					Res.Strings.Hint.Error.CannotConnectToLocalDatabase,
					ex
				);

				System.Environment.Exit (0);
			}
		}

		internal static bool CheckDatabaseExistence(DbAccess access)
		{
			return DbInfrastructure.CheckDatabaseExistence (access);
		}

		public static void DropDatabase(DbAccess access)
		{
			DbInfrastructure.DropDatabase (access);
		}

		private static void CreateDatabase(DbAccess access)
		{
			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				dbInfrastructure.CreateDatabase (access);
			}
		}

		private static void CreateDatabaseSchemas(DbAccess access, IEnumerable<Druid> managedEntityTypeIds)
		{
			EntityEngine.Create (access, managedEntityTypeIds);
		}

		private static bool VerifyDatabaseSchemas(DbAccess access, IEnumerable<Druid> managedEntityTypeIds)
		{
			return EntityEngine.Check (access, managedEntityTypeIds);
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

		private static bool UpdateDatabaseSchemas(DbAccess access, IEnumerable<Druid> managedEntityTypeIds)
		{
			try
			{
				EntityEngine.Update (access, managedEntityTypeIds);

				return true;
			}
			catch (System.Exception e)
			{
				return false;
			}
		}

		private static DataInfrastructure ConnectToDatabase(DataInfrastructure dataInfrastructure /*TMP ARGUMENT*/, DbAccess access, IEnumerable<Druid> managedEntityTypeIds)
		{
			EntityEngine entityEngine = EntityEngine.Connect (access, managedEntityTypeIds);

			//return new DataInfrastructure (access, entityEngine, enableConnectionRecycling);
			/* TMP STUFF */
			dataInfrastructure.TMPSETUP (access, entityEngine);

			return dataInfrastructure;
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
			var allEntityIds = EntityClassFactory.GetAllEntityIds ();

			foreach (var type in allEntityIds.Select (x => EntityInfo.GetStructuredType (x)))
			{
				if (type.Flags.HasFlag (StructuredTypeFlags.GenerateSchema))
				{
					yield return type;
				}
			}
		}

		private DataInfrastructure dataInfrastructure;
		private ConnectionManager connectionManager;
		private bool isDisposed;

	}
}

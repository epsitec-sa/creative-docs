//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer.Infrastructure;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Dialogs;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Factories;

namespace Epsitec.Cresus.Core.Data
{
	public sealed class Infrastructure : CoreDataComponent, System.IDisposable
	{
		public Infrastructure(CoreData data)
			: base (data)
		{
			this.dbInfrastructure = new DbInfrastructure ();
			this.dataInfrastructure = new DataLayer.Infrastructure.DataInfrastructure (this.dbInfrastructure);
		}


		public DataInfrastructure DataInfrastructure
		{
			get
			{
				return this.dataInfrastructure;
			}
		}

		public override bool CanExecuteSetupPhase()
		{
			return this.Host.ConnectionManager != null
				&& this.Host.DataLocker != null;
		}

		public override void ExecuteSetupPhase()
		{
			base.ExecuteSetupPhase ();

			this.connectionManager = this.Host.ConnectionManager;

			this.SetupDatabase ();
			this.Host.IsReady = true;
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.dataInfrastructure.Dispose ();

			if (this.dbInfrastructure.IsConnectionOpen)
			{
				this.dbInfrastructure.Dispose ();
			}
		}

		#endregion

		public void SetupDatabase(bool createNewDatabase)
		{
			createNewDatabase |= this.Host.ForceDatabaseCreation;

			if (createNewDatabase)
			{
				this.CreateDatabaseSchemas ();
				this.PopulateDatabase ();
			}
			else
			{
				this.VerifyDatabaseSchemas ();
				this.ReloadDatabase ();
			}

			this.ValidateConnection ();
			this.VerifyUidGenerators ();
		}

		public void SetupDatabase()
		{
			System.Diagnostics.Debug.Assert (this.dbInfrastructure.IsConnectionOpen == false);

			var databaseAccess = CoreData.GetDatabaseAccess ();

			try
			{
				bool databaseIsNew  = this.ConnectToDatabase (databaseAccess);

				System.Diagnostics.Debug.Assert (this.dbInfrastructure.IsConnectionOpen);

//-				var context = this.Data.CreateDataContext ("setup-only");
//-				this.Data.SetupDataContext (context);
				this.SetupDatabase (createNewDatabase: databaseIsNew);
//-				this.Data.DisposeDataContext (context);
				
				System.Diagnostics.Debug.WriteLine ("Database ready");
			}
			catch (Epsitec.Cresus.Database.Exceptions.IncompatibleDatabaseException ex)
			{
				// TODO All this part where we try to update the schema of the database if it is
				// incompatible is experimental, and must be checked/modified in order to be of
				// production quality.
				// Marc

				System.Diagnostics.Trace.WriteLine ("Failed to connect to database: " + ex.Message + "\n\n" + ex.StackTrace);

				if (this.Host.AllowDatabaseUpdate)
				{
					IDialog d = MessageDialog.CreateYesNo ("Base de donnée incompatible", DialogIcon.Warning, "La base de donnée est incompatible. Voulez vous la modifier pour la mettre à jour?");

					d.OpenDialog ();

					if (d.Result == DialogResult.Yes)
					{
						try
						{
							this.dataInfrastructure.UpdateSchema (Infrastructure.GetManagedEntityIds ());
						}
						catch (System.Exception e)
						{
							UI.ShowErrorMessage ("Erreur", "Impossible de mettre à jour la base de donnée.", e);
						}
					}

					System.Environment.Exit (0);
				}
				else
				{
					// TODO This way of exiting the program is kind of violent. It might be a
					// good idea to soften it.
					// Marc

					UI.ShowErrorMessage (
						Res.Strings.Error.IncompatibleDatabase,
						Res.Strings.Hint.Error.IncompatibleDatabase, ex);

					System.Environment.Exit (0);
				}
			}
			catch (System.Exception ex)
			{
				UI.ShowErrorMessage (
					Res.Strings.Error.CannotConnectToLocalDatabase,
					Res.Strings.Hint.Error.CannotConnectToLocalDatabase, ex);

				System.Environment.Exit (0);
			}
		}
		
		internal bool ConnectToDatabase(DbAccess access)
		{
			bool forceCreation = this.Host.ForceDatabaseCreation;

			if (forceCreation && Infrastructure.CheckDatabaseEsistence (access))
			{
				Infrastructure.DropDatabase (access);
			}

			try
			{
				if (Infrastructure.CheckDatabaseEsistence (access))
				{
					this.dbInfrastructure.AttachToDatabase (access);
					System.Diagnostics.Trace.WriteLine ("Connected to database");
					return false;
				}
			}
			catch (Epsitec.Cresus.Database.Exceptions.IncompatibleDatabaseException)
			{
				throw;
			}
			catch (System.Exception ex)
			{
				System.Diagnostics.Trace.WriteLine ("Failed to connect to database: " + ex.Message + "\n\n" + ex.StackTrace);
			}

			System.Diagnostics.Trace.WriteLine ("Cannot connect to database");
			this.dbInfrastructure.CreateDatabase (access);
			System.Diagnostics.Trace.WriteLine ("Created new database");

			return true;
		}

		internal static bool CheckDatabaseEsistence(DbAccess access)
		{
			return DbInfrastructure.CheckDatabaseExistence (access);
		}

		public static void DropDatabase(DbAccess access)
		{
			DbInfrastructure.DropDatabase (access);
		}

		private void ValidateConnection()
		{
			this.connectionManager.Validate ();
		}

		private void VerifyUidGenerators()
		{
#if false
			this.refIdGeneratorPool.GetGenerator<RelationEntity> ();
			this.refIdGeneratorPool.GetGenerator<AffairEntity> ();
			this.refIdGeneratorPool.GetGenerator<ArticleDefinitionEntity> ();
#endif
		}

		private void VerifyDatabaseSchemas()
		{
			this.connectionManager.Validate ();

			if (!this.dataInfrastructure.CheckSchema (Infrastructure.GetManagedEntityIds ()))
			{
				throw new Epsitec.Cresus.Database.Exceptions.IncompatibleDatabaseException ("Incompatible database schema");
			}
		}

		public static IEnumerable<Druid> GetManagedEntityIds()
		{
			return Infrastructure.GetManagedEntityStructuredTypes ().Select (x => x.CaptionId);
		}

		public static IEnumerable<StructuredType> GetManagedEntityStructuredTypes()
		{
			var allEntityIds = EntityClassFactory.GetAllEntityIds ();

			foreach (var type in allEntityIds.Select (x => EntityInfo.GetStructuredType (x)))
			{
				if ((type.Flags & StructuredTypeFlags.GenerateSchema) != 0)
				{
					yield return type;
				}
			}
		}


		private void CreateDatabaseSchemas()
		{
			this.connectionManager.Validate ();
			this.dataInfrastructure.CreateSchema (Infrastructure.GetManagedEntityIds ());
		}

		private void PopulateDatabase()
		{
			this.connectionManager.Validate ();

//-			this.PopulateDatabaseHack ();
//-			this.PopulateUsers ();
		}
		
		private void ReloadDatabase()
		{
			// TODO
		}

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

		private readonly DbInfrastructure dbInfrastructure;
		private readonly DataLayer.Infrastructure.DataInfrastructure dataInfrastructure;
		private ConnectionManager connectionManager;
	}
}

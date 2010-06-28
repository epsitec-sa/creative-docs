//	Copyright © 2008-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.Core
{
	public enum EntityCreationScope
	{
		CurrentContext,
		Independent,
	}

	public sealed partial class CoreData : System.IDisposable
	{
		public CoreData(bool forceDatabaseCreation)
		{
			this.IsReady = false;
			this.ForceDatabaseCreation = forceDatabaseCreation;

			this.dbInfrastructure = new DbInfrastructure ();
			this.independentEntityContext = new EntityContext (Resources.DefaultManager, EntityLoopHandlingMode.Throw, "Independent Entities");
		}

		public DbInfrastructure DbInfrastructure
		{
			get
			{
				return this.dbInfrastructure;
			}
		}

		public DataContext DataContext
		{
			get
			{
				if (this.activeDataContext == null)
				{
					this.SetupDataContext ();
				}

				return this.activeDataContext;
			}
		}

		public bool IsReady
		{
			get;
			private set;
		}

		public bool ForceDatabaseCreation
		{
			get;
			private set;
		}


		public void SetupDatabase()
		{
			if (!this.IsReady)
			{
				System.Diagnostics.Debug.Assert (this.dbInfrastructure.IsConnectionOpen == false);
				System.Diagnostics.Debug.Assert (this.activeDataContext == null);

				var  databaseAccess = CoreData.GetDatabaseAccess ();
				bool databaseIsNew  = this.ConnectToDatabase (databaseAccess);

				System.Diagnostics.Debug.Assert (this.dbInfrastructure.IsConnectionOpen);

				this.SetupDataContext ();
				this.SetupDatabase (databaseIsNew || this.ForceDatabaseCreation);
				
				System.Diagnostics.Debug.WriteLine ("Database ready");
			}

			this.IsReady = true;
		}

		public void SaveDataContext(DataContext context)
		{
			if (context != null)
			{
				System.Diagnostics.Debug.WriteLine ("About to save context #" + context.UniqueId);
				this.OnAboutToSaveDataContext (context);
				context.SaveChanges ();
				this.UpdateEditionSaveRecordCommandState ();
				System.Diagnostics.Debug.WriteLine ("Done");
			}
		}

		public DataContext CreateDataContext()
		{
			var context = new DataContext (this.dbInfrastructure)
			{
				EnableEntityNullReferenceVirtualizer = true,
			};

			DataContextPool.Instance.Add (context);

			return context;
		}

		public void DisposeDataContext(DataContext context)
		{
			if (DataContextPool.Instance.Remove (context))
			{
				context.Dispose ();

				if (this.activeDataContext == context)
				{
					this.activeDataContext = null;
					this.OnDataContextChanged (context);
				}
			}
			else
			{
				throw new System.InvalidOperationException ("Context does not belong to the pool");
			}
		}


		public AbstractEntity CreateNewEntity(string dataSetName, EntityCreationScope entityCreationScope)
		{
			var context = this.GetEntityContext (entityCreationScope);

			switch (dataSetName)
			{
				case "Customers":
					return CoreData.CreateNewCustomer (context);

				default:
					return null;
			}
		}

		private EntityContext GetEntityContext(EntityCreationScope entityCreationScope)
		{
			switch (entityCreationScope)
			{
				case EntityCreationScope.Independent:
					return this.independentEntityContext;

				case EntityCreationScope.CurrentContext:
					return this.DataContext.EntityContext;

				default:
					throw new System.NotSupportedException (string.Format ("EntityCreationScope.{0} not supported", entityCreationScope));
			}
		}

		private static AbstractEntity CreateNewCustomer(EntityContext context)
		{
			return context.CreateEmptyEntity<RelationEntity> ();
		}
		


		#region IDisposable Members


		public void Dispose()
		{
			if (this.activeDataContext != null)
			{
				this.activeDataContext.Dispose ();
				this.activeDataContext = null;
			}

			if (this.dbInfrastructure.IsConnectionOpen)
			{
				this.dbInfrastructure.Dispose ();
			}
		}


		#endregion
		
		private bool ConnectToDatabase(DbAccess access)
		{
			if (this.ForceDatabaseCreation)
			{
				this.DeleteDatabase (access);
			}

			if (this.DbInfrastructure.AttachToDatabase (access))
			{
				System.Diagnostics.Trace.WriteLine ("Connected to database");

				return false;
			}
			else
			{
				System.Diagnostics.Trace.WriteLine ("Cannot connect to database");

				try
				{
					this.DbInfrastructure.CreateDatabase (access);
				}
				catch (System.Exception ex)
				{
					UI.ShowErrorMessage (
						Res.Strings.Error.CannotConnectToLocalDatabase,
						Res.Strings.Hint.Error.CannotConnectToLocalDatabase, ex);

					System.Environment.Exit (0);
				}

				System.Diagnostics.Trace.WriteLine ("Created new database");
				
				return true;
			}
		}

		private void DeleteDatabase(DbAccess access)
		{
			string path = DbFactory.GetDatabaseFilePaths (access).First ();

			try
			{
				if (System.IO.File.Exists (path))
				{
					System.IO.File.Delete (path);
				}
			}
			catch (System.IO.IOException ex)
			{
				System.Console.Out.WriteLine ("Cannot delete database file. Error message :\n{0}\nWaiting for 5 seconds...", ex.ToString ());
				System.Threading.Thread.Sleep (5000);

				try
				{
					System.IO.File.Delete (path);
					System.Console.Out.WriteLine ("Finally succeeded");
				}
				catch
				{
					System.Console.Out.WriteLine ("Failed again, giving up");
					throw;
				}
			}
		}

		private void SetupDatabase(bool createNewDatabase)
		{
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
		}

		private void VerifyDatabaseSchemas()
		{
			// TODO
		}

		private void CreateDatabaseSchemas()
		{
			this.DataContext.CreateSchema<RelationEntity> ();
			this.DataContext.CreateSchema<NaturalPersonEntity> ();
			this.DataContext.CreateSchema<AbstractPersonEntity> ();
			this.DataContext.CreateSchema<MailContactEntity> ();
			this.DataContext.CreateSchema<TelecomContactEntity> ();
			this.DataContext.CreateSchema<UriContactEntity> ();
		}

		private void PopulateDatabase()
		{
			this.PopulateDatabaseHack ();
		}

		private void ReloadDatabase()
		{
			// TODO
		}

		private void SetupDataContext()
		{
			var oldContext = this.activeDataContext;
			this.activeDataContext = this.CreateDataContext ();
			this.OnDataContextChanged (oldContext);
		}

		private static DbAccess GetDatabaseAccess()
		{
			DbAccess access = DbInfrastructure.CreateDatabaseAccess ("core");

			access.IgnoreInitialConnectionErrors = true;
			access.CheckConnection = true;

			return access;
		}

		private void OnDataContextChanged(DataContext oldDataContext)
		{
			var newDataContext = this.activeDataContext;

			if (oldDataContext != null)
			{
				this.DetachSaveStateHandler (oldDataContext);
			}
			if (newDataContext != null)
            {
				this.AttachSaveStateHandler (newDataContext);
            }

			try
			{
				if (System.Threading.Interlocked.Increment (ref this.dataContextChangedLevel) == 1)
				{
					var handler = this.DataContextChanged;

					if (handler != null)
					{
						handler (this, new DataContextEventArgs (oldDataContext));
					}
				}
			}
			finally
			{
				System.Threading.Interlocked.Decrement (ref this.dataContextChangedLevel);
			}
		}

		private void OnAboutToSaveDataContext(DataContext context)
		{
			var handler = this.AboutToSaveDataContext;

			if (handler != null)
			{
				handler (this, new DataContextEventArgs (context));
			}
		}

		private void AttachSaveStateHandler(DataContext context)
		{
			context.EntityContext.EntityChanged += this.HandleEntityContextEntityChanged;
			CoreProgram.Application.Commands.PushHandler (Res.Commands.Edition.SaveRecord, () => this.SaveDataContext (this.DataContext));
			
			this.UpdateEditionSaveRecordCommandState ();
		}

		private void DetachSaveStateHandler(DataContext context)
		{
			context.EntityContext.EntityChanged -= this.HandleEntityContextEntityChanged;
			CoreProgram.Application.Commands.PopHandler (Res.Commands.Edition.SaveRecord);
			this.UpdateEditionSaveRecordCommandState ();
		}

		private void HandleEntityContextEntityChanged(object sender, EntityChangedEventArgs e)
		{
			this.UpdateEditionSaveRecordCommandState ();
		}

		private void UpdateEditionSaveRecordCommandState()
		{
			if ((this.activeDataContext != null) &&
				(this.activeDataContext.ContainsChanges ()))
			{
				CoreProgram.Application.SetEnable (Res.Commands.Edition.SaveRecord, true);
			}
			else
			{
				CoreProgram.Application.SetEnable (Res.Commands.Edition.SaveRecord, false);
			}
		}
		
		public event EventHandler<DataContextEventArgs> DataContextChanged;

		public event EventHandler<DataContextEventArgs> AboutToSaveDataContext;

		private readonly DbInfrastructure dbInfrastructure;
		private readonly EntityContext independentEntityContext;
		private DataContext activeDataContext;
		private int dataContextChangedLevel;
	}
}

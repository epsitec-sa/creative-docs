//	Copyright © 2008-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.Core
{
	public enum EntityCreationScope
	{
		CurrentContext,
		SpecificContext,
		Independent,
	}

	public sealed partial class CoreData : System.IDisposable
	{
		public CoreData(bool forceDatabaseCreation)
		{
			this.IsReady = false;
			this.ForceDatabaseCreation = forceDatabaseCreation;

			this.dbInfrastructure = new DbInfrastructure ();
			this.dataInfrastructure = new DataLayer.Infrastructure.DataInfrastructure (this.dbInfrastructure);
			this.independentEntityContext = new EntityContext (Resources.DefaultManager, EntityLoopHandlingMode.Throw, "Independent Entities");
			this.refIdGeneratorPool = new BusinessLogic.RefIdGeneratorPool (this);
			this.connectionManager = new CoreDataConnectionManager (this.dataInfrastructure);
			this.locker = new CoreDataLocker (this.dataInfrastructure);
		}

		public DataLayer.Infrastructure.DataInfrastructure DataInfrastructure
		{
			get
			{
				return this.dataInfrastructure;
			}
		}

		public bool IsDataContextActive
		{
			get
			{
				return this.activeDataContext != null;
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

		public BusinessLogic.RefIdGeneratorPool RefIdGeneratorPool
		{
			get
			{
				return this.refIdGeneratorPool;
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
				System.Diagnostics.Debug.Assert (this.activeDataContext == null);

				this.SetupDataContext ();
				this.SetupDatabase (databaseIsNew || this.ForceDatabaseCreation);
				this.DisposeDataContext (this.activeDataContext);

				System.Diagnostics.Debug.Assert (this.activeDataContext == null);
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

				if ((this.suspendDataContextSave == 0) &&
					(context.ContainsChanges ()))
				{
					context.SaveChanges ();
					
					this.UpdateEditionSaveRecordCommandState ();
				}
				
				System.Diagnostics.Debug.WriteLine ("Done");
			}
		}

		/// <summary>
		/// Discards the data context.
		/// </summary>
		/// <param name="context">The context.</param>
		public void DiscardDataContext(DataContext context)
		{
			if (context != null)
            {
				System.Diagnostics.Debug.WriteLine ("About to discard context #" + context.UniqueId);

				using (new DataContextDiscarder (this))
				{
					this.OnAboutToDiscardDataContext (context);

					System.Diagnostics.Debug.Assert (context.IsDisposed);
				}
			}
		}

		class DataContextDiscarder : System.IDisposable
		{
			public DataContextDiscarder(CoreData data)
			{
				this.data = data;
				this.data.suspendDataContextSave++;
			}

			#region IDisposable Members

			public void Dispose()
			{
				this.data.suspendDataContextSave--;
			}

			#endregion

			private readonly CoreData data;
		}

		public DataContext CreateDataContext()
		{
			var context = new DataContext (this.dbInfrastructure, true);

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


		public AbstractEntity CreateNewEntity(string dataSetName, EntityCreationScope entityCreationScope, DataContext specificContext = null)
		{
			var context = this.GetEntityContext (entityCreationScope, specificContext);

			switch (dataSetName)
			{
				case "Customers":
					return CoreData.CreateNewCustomer (context);

				case "InvoiceDocuments":
					return CoreData.CreateNewInvoiceDocument (context);

				case "ArticleDefinitions":
					return CoreData.CreateNewArticleDefinition (context);

				default:
					return null;
			}
		}

		private EntityContext GetEntityContext(EntityCreationScope entityCreationScope, DataContext specificContext)
		{
			switch (entityCreationScope)
			{
				case EntityCreationScope.Independent:
					return this.independentEntityContext;

				case EntityCreationScope.CurrentContext:
					return this.DataContext.EntityContext;

				case EntityCreationScope.SpecificContext:
					return specificContext.EntityContext;

				default:
					throw new System.NotSupportedException (string.Format ("EntityCreationScope.{0} not supported", entityCreationScope));
			}
		}

		private static AbstractEntity CreateNewCustomer(EntityContext context)
		{
			return context.CreateEmptyEntity<RelationEntity> ();
		}

		private static AbstractEntity CreateNewArticleDefinition(EntityContext context)
		{
			return context.CreateEmptyEntity<ArticleDefinitionEntity> ();
		}

		private static AbstractEntity CreateNewInvoiceDocument(EntityContext context)
		{
			return context.CreateEmptyEntity<InvoiceDocumentEntity> ();
		}
		


		#region IDisposable Members


		public void Dispose()
		{
			this.locker.Dispose ();

			if (this.activeDataContext != null)
			{
				this.activeDataContext.Dispose ();
				this.activeDataContext = null;
			}
			
			this.connectionManager.Dispose ();

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

			if (this.dbInfrastructure.AttachToDatabase (access))
			{
				System.Diagnostics.Trace.WriteLine ("Connected to database");

				return false;
			}
			else
			{
				System.Diagnostics.Trace.WriteLine ("Cannot connect to database");

				try
				{
					this.dbInfrastructure.CreateDatabase (access);
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

			this.ValidateConnection ();
			this.VerifyUidGenerators ();
		}

		private void ValidateConnection()
		{
			this.connectionManager.Validate ();
			this.locker.Validate ();
		}

		private void VerifyUidGenerators()
		{
			this.refIdGeneratorPool.GetGenerator<RelationEntity> ();
			this.refIdGeneratorPool.GetGenerator<AffairEntity> ();
			this.refIdGeneratorPool.GetGenerator<ArticleDefinitionEntity> ();
		}

		private void VerifyDatabaseSchemas()
		{
			// TODO
		}

		private void CreateDatabaseSchemas()
		{
			var dataContext = this.DataContext;

			dataContext.CreateSchema<RelationEntity> ();
			dataContext.CreateSchema<NaturalPersonEntity> ();
			dataContext.CreateSchema<AbstractPersonEntity> ();
			dataContext.CreateSchema<MailContactEntity> ();
			dataContext.CreateSchema<TelecomContactEntity> ();
			dataContext.CreateSchema<UriContactEntity> ();
			dataContext.CreateSchema<ArticleDefinitionEntity> ();
			dataContext.CreateSchema<VatDefinitionEntity> ();
			dataContext.CreateSchema<InvoiceDocumentEntity> ();

			dataContext.CreateSchema<ArticleDocumentItemEntity> ();
			dataContext.CreateSchema<TextDocumentItemEntity> ();
			dataContext.CreateSchema<PriceDocumentItemEntity> ();
			dataContext.CreateSchema<TaxDocumentItemEntity> ();

			dataContext.CreateSchema<EnumValueArticleParameterDefinitionEntity> ();
			dataContext.CreateSchema<NumericValueArticleParameterDefinitionEntity> ();
			
			dataContext.CreateSchema<PaymentDetailEventEntity> ();
			dataContext.CreateSchema<TotalDocumentItemEntity> ();
		}

		private void PopulateDatabase()
		{
			this.PopulateDatabaseHack ();
		}

		private void ReloadDatabase()
		{
			// TODO
		}

		public void SetupDataContext()
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

		private void OnAboutToDiscardDataContext(DataContext context)
		{
			var handler = this.AboutToDiscardDataContext;

			if (handler != null)
			{
				handler (this, new DataContextEventArgs (context));
			}
		}

		private void OnSaveRecordCommandExecuted(DataContext context)
		{
			var handler = this.SaveRecordCommandExecuted;

			if (handler != null)
			{
				handler (this, new DataContextEventArgs (context));
			}
		}

		private void OnDiscardRecordCommandExecuted(DataContext context)
		{
			var handler = this.DiscardRecordCommandExecuted;

			if (handler != null)
			{
				handler (this, new DataContextEventArgs (context));
			}
		}


		private void AttachSaveStateHandler(DataContext context)
		{
			context.EntityChanged += this.HandleEntityContextEntityChanged;

			CoreProgram.Application.Commands.PushHandler (Res.Commands.Edition.SaveRecord,
				delegate
				{
					var activeDataContext = this.DataContext;

					this.SaveDataContext (activeDataContext);
					this.OnSaveRecordCommandExecuted (activeDataContext);
				});

			CoreProgram.Application.Commands.PushHandler (Res.Commands.Edition.DiscardRecord,
				delegate
				{
					var activeDataContext = this.DataContext;

					this.DiscardDataContext (activeDataContext);
					this.OnDiscardRecordCommandExecuted (activeDataContext);
				});

			this.UpdateEditionSaveRecordCommandState ();
		}

		private void DetachSaveStateHandler(DataContext context)
		{
			context.EntityChanged -= this.HandleEntityContextEntityChanged;
			CoreProgram.Application.Commands.PopHandler (Res.Commands.Edition.SaveRecord);
			CoreProgram.Application.Commands.PopHandler (Res.Commands.Edition.DiscardRecord);
			this.UpdateEditionSaveRecordCommandState ();
		}


		private void HandleEntityContextEntityChanged(object sender, Epsitec.Cresus.DataLayer.Context.EntityChangedEventArgs e)
		{
			if (e.EventType == EntityChangedEventType.Updated)
			{
				this.UpdateEditionSaveRecordCommandState ();
			}
		}


		private void UpdateEditionSaveRecordCommandState()
		{
			if (this.activeDataContext != null &&
				this.activeDataContext.ContainsChanges ())
			{
				CoreProgram.Application.SetEnable (Res.Commands.Edition.SaveRecord, true);
				CoreProgram.Application.SetEnable (Res.Commands.Edition.DiscardRecord, true);
			}
			else
			{
				CoreProgram.Application.SetEnable (Res.Commands.Edition.SaveRecord, false);
				CoreProgram.Application.SetEnable (Res.Commands.Edition.DiscardRecord, false);
			}
		}

		internal string GetNewAffairId()
		{
			var repo = new Epsitec.Cresus.Core.Data.AffairRepository (this.activeDataContext);

			return (repo.GetAllAffairs ().Select (x => CoreData.RobustParseNumber (x.IdA)).OrderByDescending (n => n).FirstOrDefault () + 1).ToString ();
		}


		private static int RobustParseNumber(string value)
		{
			int result;
			int.TryParse (value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out result);
			return result;
		}
		
		public event EventHandler<DataContextEventArgs> DataContextChanged;
		public event EventHandler<DataContextEventArgs> AboutToSaveDataContext;
		public event EventHandler<DataContextEventArgs> AboutToDiscardDataContext;
		public event EventHandler<DataContextEventArgs> SaveRecordCommandExecuted;
		public event EventHandler<DataContextEventArgs> DiscardRecordCommandExecuted;

		private readonly DbInfrastructure dbInfrastructure;
		private readonly DataLayer.Infrastructure.DataInfrastructure dataInfrastructure;
		private readonly EntityContext independentEntityContext;
		private readonly BusinessLogic.RefIdGeneratorPool refIdGeneratorPool;
		private readonly CoreDataConnectionManager connectionManager;
		private readonly CoreDataLocker locker;

		private DataContext activeDataContext;
		private int dataContextChangedLevel;
		private int suspendDataContextSave;
	}
}
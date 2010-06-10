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


	public sealed partial class CoreData : System.IDisposable
	{
		public CoreData(bool useHack, bool forceDatabasebCreation)
		{
			this.IsReady = false;
			this.UseHack = useHack;
			this.ForceDatabaseCreation = forceDatabasebCreation;

			this.dbInfrastructure = new DbInfrastructure ();
			this.dataContextPool = new DataContextPool ();
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

		public DataContextPool DataContextPool
		{
			get
			{
				return this.dataContextPool;
			}
		}

		public bool IsReady
		{
			get;
			private set;
		}

		public bool UseHack
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
			if (!this.UseHack && !this.IsReady)
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
				context.SaveChanges ();
				System.Diagnostics.Debug.WriteLine ("Done");
			}
		}

		public DataContext CreateDataContext()
		{
			var context = new DataContext (this.dbInfrastructure)
			{
				EnableEntityNullReferenceVirtualizer = true,
			};

			this.dataContextPool.Add (context);

			return context;
		}

		public void DisposeDataContext(DataContext context)
		{
			if (this.dataContextPool.Remove (context))
			{
				context.Dispose ();

				if (this.activeDataContext == context)
				{
					this.activeDataContext = null;
					this.OnDataContextChanged ();
				}
			}
			else
			{
				throw new System.InvalidOperationException ("Context does not belong to the pool");
			}
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
			this.activeDataContext = this.CreateDataContext ();
		}

		private static DbAccess GetDatabaseAccess()
		{
			DbAccess access = DbInfrastructure.CreateDatabaseAccess ("core");

			access.IgnoreInitialConnectionErrors = true;
			access.CheckConnection = true;

			return access;
		}

		private void OnDataContextChanged()
		{
			var handler = this.DataContextChanged;

			if (handler != null)
			{
				handler (this);
			}
		}

		public event EventHandler DataContextChanged;

		private readonly DbInfrastructure dbInfrastructure;
		private readonly DataContextPool dataContextPool;
		private DataContext activeDataContext;
	}
}

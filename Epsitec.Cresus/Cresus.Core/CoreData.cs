//	Copyright © 2008-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core
{
	public sealed partial class CoreData : System.IDisposable
	{
		public CoreData()
		{
			this.infrastructure = new DbInfrastructure ();
			this.resolver = new ResolverImplementation (this);
		}

		public DataContext DataContext
		{
			get
			{
				return this.dataContext;
			}
		}

		public bool IsReady
		{
			get
			{
				return this.ready;
			}
		}

		public void SetupDatabase()
		{
			System.Diagnostics.Debug.Assert (this.infrastructure.IsConnectionOpen == false);
			System.Diagnostics.Debug.Assert (this.dataContext == null);

			DbAccess access = CoreData.GetDatabaseAccess ();
			bool     empty  = this.ConnectToDatabase (access);

			System.Diagnostics.Debug.Assert (this.infrastructure.IsConnectionOpen);

			this.SetupDataContext ();

			if (empty)
			{
				this.CreateDatabaseSchemas ();
				this.PopulateDatabase ();
			}
			else
			{
				this.VerifyDatabaseSchemas ();
				this.ReloadDatabase ();
			}
			
			System.Diagnostics.Debug.WriteLine ("Database ready");

			this.SetupDataBrowser ();
			this.ready = true;
		}

		private void SetupDataContext()
		{
			this.dataContext = new DataContext (this.infrastructure);
		}

		private void SetupDataBrowser()
		{
			this.dataBrowser = new DataBrowser (this.infrastructure);
		}

		private bool ConnectToDatabase(DbAccess access)
		{
			if (this.infrastructure.AttachToDatabase (access))
			{
				System.Diagnostics.Trace.WriteLine ("Connected to database");

				return false;
			}
			else
			{
				System.Diagnostics.Trace.WriteLine ("Cannot connect to database");

				try
				{
					this.infrastructure.CreateDatabase (access);
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

		private void VerifyDatabaseSchemas()
		{
		}

		private void CreateDatabaseSchemas()
		{
			this.dataContext.CreateSchema<Epsitec.Cresus.Mai2008.Entities.ClientEntity> ();
			this.dataContext.CreateSchema<Epsitec.Cresus.Mai2008.Entities.FactureEntity> ();
		}

		private static DbAccess GetDatabaseAccess()
		{
			DbAccess access = DbInfrastructure.CreateDatabaseAccess ("core");

			access.IgnoreInitialConnectionErrors = true;
			access.CheckConnection = true;

			return access;
		}

		
		#region IDisposable Members

		public void Dispose()
		{
			if (this.resolver != null)
			{
				this.resolver.Dispose ();
				this.resolver = null;
			}
			
			if (this.dataContext != null)
			{
				this.dataContext.Dispose ();
				this.dataContext = null;
			}
			
			if (this.infrastructure.IsConnectionOpen)
			{
				this.infrastructure.Dispose ();
			}
		}

		#endregion
		
		private readonly DbInfrastructure infrastructure;
		private DataContext dataContext;
		private DataBrowser dataBrowser;
		private bool ready;
	}
}

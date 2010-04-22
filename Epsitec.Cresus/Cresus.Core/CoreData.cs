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

		public void SetupDatabase()
		{
			System.Diagnostics.Debug.Assert (this.infrastructure.IsConnectionOpen == false);
			System.Diagnostics.Debug.Assert (this.dataContext == null);

			DbAccess access = DbInfrastructure.CreateDatabaseAccess ("core");
			bool     empty  = false;

			access.IgnoreInitialConnectionErrors = true;
			access.CheckConnection = true;

			if (this.infrastructure.AttachToDatabase (access))
			{
				System.Diagnostics.Trace.WriteLine ("Connected to database");
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
				empty = true;
			}

			this.dataContext = new DataContext (this.infrastructure);

			System.Diagnostics.Debug.Assert (this.infrastructure.IsConnectionOpen);

			if (empty)
			{
				this.CreateSchemas ();
				this.PopulateDatabase ();
			}
			else
			{
				this.VerifySchemas ();
				this.ReloadDatabase ();
			}
			
			System.Diagnostics.Debug.WriteLine ("Database ready");

			this.dataBrowser = new DataBrowser (this.infrastructure);
		}

		private void VerifySchemas()
		{
		}

		private void CreateSchemas()
		{
			this.dataContext.CreateSchema<Epsitec.Cresus.Mai2008.Entities.ClientEntity> ();
			this.dataContext.CreateSchema<Epsitec.Cresus.Mai2008.Entities.FactureEntity> ();
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
	}
}

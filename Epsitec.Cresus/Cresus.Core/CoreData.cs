﻿//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;

namespace Epsitec.Cresus.Core
{
	public sealed class CoreData : System.IDisposable
	{
		public CoreData()
		{
			this.infrastructure = new DbInfrastructure ();
		}


		public void SetupDatabase()
		{
			System.Diagnostics.Debug.Assert (this.infrastructure.IsConnectionOpen == false);
			System.Diagnostics.Debug.Assert (this.dataContext == null);

			DbAccess access = DbInfrastructure.CreateDatabaseAccess ("core");

			access.IgnoreInitialConnectionErrors = true;
			access.CheckConnection = true;

			if (this.infrastructure.AttachToDatabase (access))
			{
				System.Diagnostics.Debug.WriteLine ("Connected to database");
			}
			else
			{
				System.Diagnostics.Debug.WriteLine ("Cannot connect to database");
				this.infrastructure.CreateDatabase (access);
				System.Diagnostics.Debug.WriteLine ("Created new database");
			}

			this.dataContext = new DataContext (this.infrastructure);

			System.Diagnostics.Debug.Assert (this.infrastructure.IsConnectionOpen);
		}

		public void CreateSchemas()
		{
			System.Diagnostics.Debug.Assert (this.dataContext != null);

//			this.dataContext.CreateSchema<Epsitec.Cresus.AddressBook.Entities.AdresseEntity> ();
//			this.dataContext.CreateSchema<Epsitec.Cresus.AddressBook.Entities.LocalitéEntity> ();
//			this.dataContext.CreateSchema<Epsitec.Cresus.AddressBook.Entities.PaysEntity> ();
			this.dataContext.CreateSchema<Epsitec.Cresus.AddressBook.Entities.PersonAddressEntity> ();
		}


		#region IDisposable Members

		public void Dispose()
		{
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
	}
}

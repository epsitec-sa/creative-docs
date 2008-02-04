//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Database;

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
			DbAccess access = DbInfrastructure.CreateDatabaseAccess ("core");

			access.IgnoreInitialConnectionErrors = true;

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
		}


		#region IDisposable Members

		public void Dispose()
		{
			if (this.infrastructure.IsConnectionOpen)
			{
				this.infrastructure.Dispose ();
			}
		}

		#endregion

		private readonly DbInfrastructure infrastructure;
	}
}

//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Database.Services;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.DataLayer.Infrastructure
{
	public sealed class LockOwner
	{
		public LockOwner(System.Tuple<DbConnection, DbLock> dbConnectionAndLock)
			: this (dbConnectionAndLock.Item1, dbConnectionAndLock.Item2)
		{
		}

		public LockOwner(DbConnection dbConnection, DbLock dbLock)
		{
			this.ConnectionIdentity = dbConnection.Identity;
			this.LockName           = dbLock.Name;
			this.LockDateTime       = dbLock.CreationTime;
		}

		
		public string							ConnectionIdentity
		{
			get;
			private set;
		}

		public string							LockName
		{
			get;
			private set;
		}

		public System.DateTime					LockDateTime
		{
			get;
			private set;
		}
	}
}

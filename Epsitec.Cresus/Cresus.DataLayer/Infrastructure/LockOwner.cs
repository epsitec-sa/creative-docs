//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Database.Services;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.DataLayer.Infrastructure
{
	/// <summary>
	/// The <c>LockOwner</c> class provides information about who owns a given
	/// lock in the database.
	/// </summary>
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


		/// <summary>
		/// Gets the low level connection identity.
		/// </summary>
		/// <value>The connection identity.</value>
		public string							ConnectionIdentity
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the low level name of the lock.
		/// </summary>
		/// <value>The name of the lock.</value>
		public string							LockName
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the date and time when the lock was acquired in the database.
		/// </summary>
		/// <value>The lock date and time.</value>
		public System.DateTime					LockDateTime
		{
			get;
			private set;
		}
	}
}

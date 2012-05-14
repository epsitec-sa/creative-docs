//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Database;
using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer.Context
{
	/// <summary>
	/// The <c>IsolatedTransaction</c> class encapsulates an isolated read-only transaction. It
	/// is mainly used by <see cref="RequestView"/>.
	/// </summary>
	public sealed class IsolatedTransaction : System.IDisposable
	{
		public IsolatedTransaction(DataContext dataContext)
		{
			var dataInfrastructure = dataContext.DataInfrastructure;
			var dbInfrastructure = dataInfrastructure.DbInfrastructure;

			this.iDbAbstraction = IsolatedTransaction.CreateIDbAbstraction (dbInfrastructure);
			this.dbTransaction  = IsolatedTransaction.CreateDbTransaction (dbInfrastructure, this.iDbAbstraction);
		}


		public DbTransaction					Transaction
		{
			get
			{
				return this.dbTransaction;
			}
		}

		
		private static IDbAbstraction CreateIDbAbstraction(DbInfrastructure dbInfrastructure)
		{
			var idbAbstraction = dbInfrastructure.CreateDatabaseAbstraction ();

			idbAbstraction.SqlBuilder.AutoClear = true;

			return idbAbstraction;
		}

		private static DbTransaction CreateDbTransaction(DbInfrastructure dbInfrastructure, IDbAbstraction iDbAbstraction)
		{
			return dbInfrastructure.BeginTransaction (DbTransactionMode.ReadOnly, iDbAbstraction);
		}


		#region System.IDisposable Members

		public void Dispose()
		{
			this.dbTransaction.Dispose ();
			this.iDbAbstraction.Dispose ();
		}

		#endregion


		private readonly IDbAbstraction			iDbAbstraction;
		private readonly DbTransaction			dbTransaction;
	}
}

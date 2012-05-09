using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer.Loader;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Context
{
	
	
	public sealed class RequestView : IDisposable
	{

		// TODO Finish this class as it is a simple stub that I updated in order to allow Pierre to
		// continue its work on the BigList.


		internal RequestView(DataContext dataContext, Request request)
		{
			this.dataContext = dataContext;
			this.request = request;

			//var dataInfrastructure = dataContext.DataInfrastructure;
			//var dbInfrastructure = dataInfrastructure.DbInfrastructure;

			//this.iDbAbstraction = this.CreateIDbAbstraction (dbInfrastructure);
			//this.dbTransaction = this.CreateDbTransaction (dbInfrastructure, this.iDbAbstraction);
		}


		private IDbAbstraction CreateIDbAbstraction(DbInfrastructure dbInfrastructure)
		{
			return dbInfrastructure.CreateDatabaseAbstraction ();
		}


		private DbTransaction CreateDbTransaction(DbInfrastructure dbInfrastructure, IDbAbstraction iDbAbstraction)
		{
			return dbInfrastructure.BeginTransaction (DbTransactionMode.ReadOnly, iDbAbstraction);
		}


		public IEnumerable<EntityKey> GetKeys(int index, int count)
		{
			return from entity in this.GetEntities (index, count)
				   select this.dataContext.GetNormalizedEntityKey (entity).Value;
		}


		public IEnumerable<AbstractEntity> GetEntities(int index, int count)
		{
			this.request.Skip = index + 1;
			this.request.Take = count;

			// TODO Use this.dbTransaction here.

			return this.dataContext.GetByRequest (this.request);
		}


		public int GetCount()
		{
			this.request.Skip = null;
			this.request.Take = null;

			// TODO Use this.dbTransaction here.

			return this.dataContext.GetCount (this.request);
		}


		#region IDisposable Members


		public void Dispose()
		{
			//this.dbTransaction.Commit ();

			//this.iDbAbstraction.Dispose ();
		}


		#endregion


		private readonly IDbAbstraction iDbAbstraction;


		private readonly DbTransaction dbTransaction;


		private readonly DataContext dataContext;


		private readonly Request request;


	}


}

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Loader;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Context
{
	
	
	/// <summary>
	/// The RequestView class gives a view on a request that will not change over time. For this, it
	/// uses a new connection do the database and opens a readonly transaction. That means that the
	/// results of the queries made to the database via this class are totally independent of
	/// whatever might happen in the database via the DataContext bound to this class.
	/// Note also that the RequestView uses its request more than once. In fact it uses it each time
	/// one of its method is called. The Request used internally is copy of the one given in the
	/// constructor so you are free to modify it after the call to the constructor. However, the
	/// entities embedded in the request are not copied, and therefore any modification made to
	/// these entities might end up in causing problems here.
	/// </summary>
	public sealed class RequestView : IDisposable
	{


		internal RequestView(DataContext dataContext, Request request)
		{
			this.dataContext = dataContext;

			this.entityKeyRequest = this.GetEntityKeyRequest (request);
			this.countRequest = this.GetCountRequest (request);

			var dataInfrastructure = dataContext.DataInfrastructure;
			var dbInfrastructure = dataInfrastructure.DbInfrastructure;

			this.iDbAbstraction = this.CreateIDbAbstraction (dbInfrastructure);
			this.dbTransaction = this.CreateDbTransaction (dbInfrastructure, this.iDbAbstraction);
		}


		private IDbAbstraction CreateIDbAbstraction(DbInfrastructure dbInfrastructure)
		{
			var idbAbstraction = dbInfrastructure.CreateDatabaseAbstraction ();

			idbAbstraction.SqlBuilder.AutoClear = true;

			return idbAbstraction;
		}


		private DbTransaction CreateDbTransaction(DbInfrastructure dbInfrastructure, IDbAbstraction iDbAbstraction)
		{
			return dbInfrastructure.BeginTransaction (DbTransactionMode.ReadOnly, iDbAbstraction);
		}


		private Request GetEntityKeyRequest(Request request)
		{
			var entityKeyRequest = request.Clone ();

			entityKeyRequest.SortClauses.Add
			(
				new SortClause
				(
					InternalField.CreateId (request.RequestedEntity),
					SortOrder.Ascending
				)
			);

			return entityKeyRequest;
		}


		private Request GetCountRequest(Request request)
		{
			var countRequest = request.Clone ();

			countRequest.SortClauses.RemoveRange (0, countRequest.SortClauses.Count);

			return countRequest;
		}


		public IList<EntityKey> GetKeys(int index, int count)
		{
			this.entityKeyRequest.Skip = index;
			this.entityKeyRequest.Take = count;

			return this.dataContext.DataLoader.GetEntityKeys (this.entityKeyRequest, this.dbTransaction);
		}


		public int GetCount()
		{
			return this.dataContext.DataLoader.GetCount (this.countRequest, this.dbTransaction);
		}


		#region IDisposable Members


		public void Dispose()
		{
			this.dbTransaction.Commit ();

			this.iDbAbstraction.Dispose ();
		}


		#endregion


		private readonly IDbAbstraction iDbAbstraction;


		private readonly DbTransaction dbTransaction;


		private readonly DataContext dataContext;


		private readonly Request entityKeyRequest;


		private readonly Request countRequest;


	}


}

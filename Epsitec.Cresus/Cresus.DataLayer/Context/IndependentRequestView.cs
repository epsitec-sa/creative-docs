using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Loader;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Context
{


	/// <summary>
	/// The <c>IndependentRequestView</c> class gives a view on a request that will not change over
	/// time. For this, it uses a new connection do the database and opens a readonly transaction.
	/// That means that the results of the queries made to the database via this class are totally
	/// independent of whatever might happen in the database via the DataContext bound to this
	/// class.
	/// </summary>
	public sealed class IndependentRequestView : AbstractRequestView
	{


		internal IndependentRequestView(DataContext dataContext, Request request, IsolatedTransaction isolatedTransaction)
			: base (dataContext, request)
		{
			this.ownIsolatedTransaction = isolatedTransaction == null;
			this.isolatedTransaction = isolatedTransaction ?? new IsolatedTransaction (dataContext);
		}


		private DbTransaction DbTransaction
		{
			get
			{
				return this.isolatedTransaction.Transaction;
			}
		}


		public override IList<AbstractEntity> GetEntities(Request request)
		{
			// This method is not implemented as it would be impossible to load the data of the
			// entities within the isolated transaction. Even if we load their data, proxy would be
			// resolved by the dataContext in another transaction, and the DataContext might use
			// data that it has already loaded for some entities. If this method is really required
			// for this class, then lots of changes in the DataContext will have to be made.

			throw new NotImplementedException ();
		}


		public override IList<EntityKey> GetKeys(Request request)
		{
			return this.DataLoader.GetEntityKeys (request, this.DbTransaction);
		}


		public override int? GetIndex(Request request, EntityKey entityKey)
		{
			return this.DataLoader.GetIndex (request, entityKey, this.DbTransaction);
		}


		public override int GetCount(Request request)
		{
			return this.DataLoader.GetCount (request, this.DbTransaction);
		}


		protected override void Dispose(bool disposing)
		{
			if (this.ownIsolatedTransaction)
			{
				this.isolatedTransaction.Dispose ();
			}
		}


		private readonly bool ownIsolatedTransaction;


		private readonly IsolatedTransaction isolatedTransaction;


	}


}

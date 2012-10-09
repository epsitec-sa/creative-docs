//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.DataLayer.Loader;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Context
{
	
	
	/// <remarks>
	/// The <c>RequestView</c> uses its request more than once. In fact it uses it each time
	/// one of its method is called. The <c>Request</c> used internally is copy of the one given in the
	/// constructor so you are free to modify it after the call to the constructor. However, the
	/// entities embedded in the request are not copied, and therefore any modification made to
	/// these entities might end up in causing problems here.
	/// </remarks>
	public abstract class AbstractRequestView : IDisposable
	{


		internal AbstractRequestView(DataContext dataContext, Request request)
		{
			this.dataContext = dataContext;

			this.entitiesRequest = AbstractRequestView.GetEntitiesRequest (request);
			this.countRequest = AbstractRequestView.GetCountRequest (request);
		}


		protected DataContext DataContext
		{
			get
			{
				return this.dataContext;
			}
		}


		internal DataLoader DataLoader
		{
			get
			{
				return this.dataContext.DataLoader;
			}
		}


		public IList<AbstractEntity> GetEntities(int index, int count)
		{
			this.SetupEntitiesRequest (index, count);

			return this.GetEntities (this.entitiesRequest);
		}


		public abstract IList<AbstractEntity> GetEntities(Request request);


		public IList<EntityKey> GetKeys(int index, int count)
		{
			this.SetupEntitiesRequest (index, count);

			return this.GetKeys (this.entitiesRequest);
		}


		public abstract IList<EntityKey> GetKeys(Request request);


		public int? GetIndex(EntityKey entityKey)
		{
			this.SetupEntitiesRequest (null, null);

			return this.GetIndex (this.entitiesRequest, entityKey);
		}


		public abstract int? GetIndex(Request request, EntityKey entityKey);


		public int GetCount()
		{
			return this.GetCount (this.countRequest);
		}


		public abstract int GetCount(Request request);


		private void SetupEntitiesRequest(int? nbTake, int? nbSkip)
		{
			this.entitiesRequest.Skip = nbTake;
			this.entitiesRequest.Take = nbSkip;
		}


		private static Request GetEntitiesRequest(Request request)
		{
			var entitiesRequest = request.Clone ();

			entitiesRequest.AddIdSortClause (request.RequestedEntity);

			return entitiesRequest;
		}

		private static Request GetCountRequest(Request request)
		{
			var countRequest = request.Clone ();

			countRequest.SortClauses.RemoveRange (0, countRequest.SortClauses.Count);

			return countRequest;
		}


		#region IDisposable Members


		public void Dispose()
		{
			this.Dispose (true);
		}


		protected virtual void Dispose(bool disposing)
		{
		}


		#endregion


		private readonly DataContext dataContext;


		private readonly Request entitiesRequest;


		private readonly Request countRequest;


	}


}
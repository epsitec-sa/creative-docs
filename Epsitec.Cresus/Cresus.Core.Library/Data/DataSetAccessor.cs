//	Copyright � 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Metadata;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Loader;

using System;
using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.Core.Data
{
	/// <summary>
	/// The <c>DataSetAccessor</c> class is used to retrieve entities from a data set. The
	/// retrieval is done in an isolated transaction and can be done in small chunks, based
	/// on the user's need.
	/// </summary>
	public abstract class DataSetAccessor : System.IDisposable
	{
		protected DataSetAccessor(CoreData data, DataSetMetadata dataSetMetadata, IsolatedTransaction isolatedTransaction = null)
		{
			this.data            = data;
			this.dataSetMetadata = dataSetMetadata;
			this.businessContext = this.data.CreateIsolatedBusinessContext ("DataSetAccessor", false);
			this.customizers     = new List<Action<DataContext, Request, AbstractEntity>> ();

			this.isolatedTransaction = isolatedTransaction;
		}


		public DataContext						IsolatedDataContext
		{
			get
			{
				return this.businessContext.DataContext;
			}
		}

		public DataSetMetadata					DataSetMetadata
		{
			get
			{
				return this.dataSetMetadata;
			}
		}


		/// <summary>
		/// The DataSetAccessor won't use the scope filters if this property is set to true.
		/// </summary>
		public bool								DisableScopeFilter
		{
			get;
			set;
		}


		/// <summary>
		/// This action will be called before the creation of the RequestView object, so you can
		/// set up the Request as you want here.
		/// </summary>
		public IList<Action<DataContext, Request, AbstractEntity>> Customizers
		{
			get
			{
				return this.customizers;
			}
		}


		public void MakeDependent()
		{
			this.isDependent = true;
		}

		public void SetRequest(Request request)
		{
			this.request = request;
		}

		public int GetItemCount()
		{
			if (this.itemCount == null)
			{
				this.itemCount = this.GetRequestView ().GetCount ();
			}

			return this.itemCount.Value;
		}

		public int IndexOf(EntityKey? entityKey)
		{
			if (entityKey == null)
			{
				return -1;
			}

			return this.GetRequestView ().GetIndex (entityKey.Value) ?? -1;
		}

		public AbstractEntity[] GetItems(int index, int count)
		{
			return this.ExecuteRequest (index, count, (r, i, c) => r.GetEntities (i, c));
		}
		
		public EntityKey[] GetItemKeys(int index, int count)
		{
			return this.ExecuteRequest (index, count, (r, i, c) => r.GetKeys (i, c));
		}

		private T[] ExecuteRequest<T>(int index, int count, System.Func<AbstractRequestView, int, int, IEnumerable<T>> getter)
		{
			int total = this.GetItemCount ();
			int end   = index + count;

			if ((index >= total) ||
				(count < 1))
			{
				return new T[0];
			}

			if (end >= total)
			{
				count = total - index;
			}

			var requestView = this.GetRequestView ();

			return getter (requestView, index, count).ToArray ();
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose (true);
		}

		#endregion

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.businessContext.IsDisposed == false)
				{
					this.businessContext.Discard ();
					this.businessContext.Dispose ();
				}

				this.DisposeRequestView ();
			}
		}

		protected abstract AbstractEntity GetExample();


		private AbstractRequestView GetRequestView()
		{
			if (this.requestView == null)
			{
				this.requestView = this.CreateRequestView ();
			}

			return this.requestView;
		}

		private void DisposeRequestView()
		{
			if (this.requestView != null)
			{
				this.requestView.Dispose ();
				this.requestView = null;
			}
		}

		private Request CreateRequest()
		{
			if (this.request != null)
			{
				return this.request;
			}
			else
			{
				var example = this.GetExample ();
				var request = new Request ()
				{
					RequestedEntity = example,
					RootEntity = example,
				};
				
				return request;
			}
		}

		private AbstractRequestView CreateRequestView()
		{
			var request = this.CreateRequest ();
			var example = request.RootEntity;
			var session = UserManager.Current.ActiveSession;

			request.AddCondition (this.IsolatedDataContext, example, this.dataSetMetadata.Filter);

			if (!this.DisableScopeFilter)
			{
				var scopeFilter = session.GetScopeFilter (this.dataSetMetadata, example);
				request.AddCondition (this.IsolatedDataContext, example, scopeFilter);
			}

			var dataSetSettings = session.GetDataSetSettings (this.dataSetMetadata);
			request.AddCondition (this.IsolatedDataContext, example, dataSetSettings.Filter);
			
			var additionalFilter = session.GetAdditionalFilter (this.dataSetMetadata, example);
			request.AddCondition (this.IsolatedDataContext, example, additionalFilter);
			
			IEnumerable<SortClause> sortClauses;

			if (dataSetSettings != null)
			{
				var settingsSort = dataSetSettings.Sort;

				sortClauses = settingsSort.Select (sc => this.CreateSortClause (sc, example));
			}
			else
			{
				var defaultSort = this.dataSetMetadata.EntityTableMetadata.GetSortColumns ();

				sortClauses = defaultSort.Select (c => this.CreateSortClause (c, example));
			}

			request.SortClauses.AddRange (sortClauses);

			foreach (var customizer in this.Customizers)
			{
				customizer (this.IsolatedDataContext, request, example);
			}

			return this.IsolatedDataContext.GetRequestView (request, !this.isDependent, this.isolatedTransaction);
		}

		private SortClause CreateSortClause(ColumnRef<EntityColumnSort> sortColumn, AbstractEntity example)
		{
			var column = this.DataSetMetadata.EntityTableMetadata.FindColumn (sortColumn.Id);

			return sortColumn.Value.ToSortClause (column, example);
		}

		private SortClause CreateSortClause(EntityColumnMetadata column, AbstractEntity example)
		{
			return column.DefaultSort.ToSortClause (column, example);
		}


		private readonly CoreData				data;
		private readonly BusinessContext		businessContext;
		private readonly IsolatedTransaction	isolatedTransaction;
		private readonly DataSetMetadata		dataSetMetadata;
		private readonly IList<Action<DataContext, Request, AbstractEntity>> customizers;
		
		private AbstractRequestView				requestView;
		private int?							itemCount;
		private bool							isDependent;
		private Request							request;
	}
}

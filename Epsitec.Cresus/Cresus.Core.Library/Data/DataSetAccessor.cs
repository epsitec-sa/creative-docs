//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Metadata;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Loader;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;
using Epsitec.Cresus.Core.Business.UserManagement;


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
			this.entityType      = dataSetMetadata.DataSetEntityType;
			this.dataSetMetadata = dataSetMetadata;
			this.dataContext     = this.data.CreateIsolatedDataContext ("DataSetAccessor");

			this.isolatedTransaction = isolatedTransaction;
		}


		public DataContext						IsolatedDataContext
		{
			get
			{
				return this.dataContext;
			}
		}

		public DataSetMetadata					DataSetMetadata
		{
			get
			{
				return this.dataSetMetadata;
			}
		}

		public int GetItemCount()
		{
			if (this.itemCount == null)
			{
				this.itemCount = this.RetrieveItemCount ();
			}

			return this.itemCount.Value;
		}

		public int IndexOf(EntityKey? entityKey)
		{
			if (entityKey == null)
			{
				return -1;
			}

			return this.requestView.GetIndex (entityKey.Value) ?? -1;
		}
		
		public EntityKey[] GetItemKeys(int index, int count)
		{
			int total = this.GetItemCount ();
			int end   = index + count;

			if ((index >= total) ||
				(count < 1))
			{
				return new EntityKey[0];
			}

			if (end >= total)
			{
				count = total - index;
			}

			return this.requestView
				.GetKeys (index, count)
				.ToArray ();
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
				if (this.dataContext.IsDisposed == false)
				{
					this.data.DisposeDataContext (this.dataContext);
				}

				if (this.requestView != null)
				{
					this.requestView.Dispose ();
					this.requestView = null;
				}
			}
		}

		protected abstract AbstractEntity GetExample();

		private void SetUpRequestView()
		{
			if (this.requestView != null)
			{
				return;
			}
			if (this.dataContext.IsDisposed)
			{
				throw new System.ObjectDisposedException ("dataContext", "The data set accessor was disposed");
			}

			//	TODO: ...

			var example = this.GetExample ();

			var request = new Request ()
			{
				RequestedEntity = example,
				RootEntity = example,
			};

			var session = UserManager.Current.ActiveSession;

			var scopeFilter = session.GetScopeFilter (this.entityType, example);
			var tableSettings = session.GetTableSettings (this.entityType);

			request.AddCondition (this.dataContext, example, this.dataSetMetadata.Filter);
			request.AddCondition (this.dataContext, example, scopeFilter);
			request.AddCondition (this.dataContext, example, tableSettings.Filter);

			IEnumerable<SortClause> sortClauses;

			if (tableSettings.Sort.Any ())
			{
				var settingsSort = tableSettings.Sort;

				sortClauses = settingsSort.Select (sc => this.CreateSortClause (sc, example));
			}
			else
			{
				var defaultSort = this.dataSetMetadata.EntityTableMetadata.GetSortColumns ();

				sortClauses = defaultSort.Select (c => this.CreateSortClause (c, example));
			}

			request.SortClauses.AddRange (sortClauses);

			this.requestView = this.dataContext.GetRequestView (request, true, this.isolatedTransaction);
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


		private int RetrieveItemCount()
		{
			this.SetUpRequestView ();

			return this.requestView.GetCount ();
		}



		private readonly CoreData				data;
		private readonly DataContext			dataContext;
		private readonly System.Type			entityType;
		private readonly IsolatedTransaction	isolatedTransaction;
		private readonly DataSetMetadata		dataSetMetadata;
		
		private AbstractRequestView				requestView;
		private int?							itemCount;
	}
}

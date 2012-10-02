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
			this.sortColumns     = new List<EntityColumnMetadata> ();

			this.isolatedTransaction = isolatedTransaction;
		}


		public DataContext						IsolatedDataContext
		{
			get
			{
				return this.dataContext;
			}
		}

		public DataSetMetadata					Metadata
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

		public void SetSortOrder(IEnumerable<EntityColumnMetadata> columns)
		{
			this.sortColumns.Clear ();

			if (columns != null)
			{
				this.sortColumns.AddRange (columns);
			}
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

			if (this.dataSetMetadata.DataSetName == "AiderGroup")
			{
#if true
				var ex = new ColumnFilterComparisonExpression ()
				{
					Comparison = ColumnFilterComparisonCode.Equal,
					Constant = ColumnFilterConstant.From (0),
				};

				var ef = new EntityFilter (Druid.Parse ("[LVA54]"));

				ef.Columns.Add (new ColumnRef<EntityColumnFilter> ("[LVAED]", new EntityColumnFilter (ex)));

				request.AddCondition (this.dataContext, example, ef);
#else
				var fieldPath  = EntityFieldPath.CreateAbsolutePath (Druid.Parse ("[LVA54]"), "[LVAED]");
				var fieldExpr  = fieldPath.CreateLambda ();
				var groupParam = System.Linq.Expressions.Expression.Parameter (this.dataSetMetadata.DataSetEntityType, "group");
				var parameter  = ExpressionAnalyzer.ReplaceParameter (fieldExpr, groupParam);
				
				var compExpr  = new ColumnFilterComparisonExpression ()
				{
					Comparison = ColumnFilterComparisonCode.Equal,
					Constant = ColumnFilterConstant.From (0),
				};

				var lambda = System.Linq.Expressions.Expression.Lambda (compExpr.GetExpression (parameter), groupParam);

				request.AddCondition (example, lambda);
#endif
			}

			request.SortClauses.AddRange (this.CreateSortClauses (example));
			request.AddIdSortClause (example);

			this.requestView = this.dataContext.GetRequestView (request, this.isolatedTransaction);
		}

		private IEnumerable<SortClause> CreateSortClauses(AbstractEntity example)
		{
			return this.sortColumns.Select (c => c.DefaultSort.ToSortClause (c, example));
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
		private readonly List<EntityColumnMetadata>	sortColumns;
		private readonly DataSetMetadata		dataSetMetadata;
		
		private RequestView						requestView;
		private int?							itemCount;
	}
}

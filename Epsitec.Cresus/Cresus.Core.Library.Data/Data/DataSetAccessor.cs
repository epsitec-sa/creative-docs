//	Copyright � 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Data.Extraction;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Loader;

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
		protected DataSetAccessor(CoreData data, System.Type entityType, IsolatedTransaction isolatedTransaction = null)
		{
			this.data = data;
			this.entityType = entityType;
			this.dataContext = this.data.CreateDataContext ("DataSetAccessor");
			this.isolatedTransaction = isolatedTransaction;
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
			//	TODO: implement...

			return -1;
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

		public void SetSortOrder(IEnumerable<EntityDataColumn> columns)
		{
			foreach (var column in columns)
			{
				var fieldId = EntityInfo.GetFieldCaption (column.Lambda).Id;
				var fieldSetter = Epsitec.Common.Types.ExpressionAnalyzer.CreateSetter (column.Lambda);
//				var fieldNode = new Epsitec.Cresus.DataLayer.Expressions.PublicField (entity, fieldId);
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

			this.requestView = this.dataContext.GetRequestView (request, this.isolatedTransaction);
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
		
		private RequestView						requestView;
		private int?							itemCount;
	}
}

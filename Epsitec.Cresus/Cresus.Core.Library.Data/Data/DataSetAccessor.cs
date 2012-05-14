//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Common.Support.EntityEngine;

namespace Epsitec.Cresus.Core.Data
{
	/// <summary>
	/// The <c>DataSetAccessor</c> class is used to retrieve entities from a data set. The
	/// retrieval is done in an isolated transaction and can be done in small chunks, based
	/// on the user's need.
	/// </summary>
	public abstract class DataSetAccessor : System.IDisposable
	{
		protected DataSetAccessor(CoreData data, System.Type entityType)
		{
			this.data = data;
			this.entityType = entityType;
			this.dataContext = this.data.CreateDataContext ("DataSetAccessor");
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

			this.requestView = this.dataContext.GetRequestView (request);
		}

		private int RetrieveItemCount()
		{
			this.SetUpRequestView ();

			return this.requestView.GetCount ();
		}



		private EntityKey GetEntityKey(Epsitec.Common.Support.EntityEngine.AbstractEntity x)
		{
			return this.dataContext.GetNormalizedEntityKey (x).Value;
		}

		private readonly CoreData				data;
		private readonly DataContext			dataContext;
		private readonly System.Type			entityType;
		
		private RequestView						requestView;
		private int?							itemCount;
	}
}

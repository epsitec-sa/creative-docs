//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Reporting
{
	public class DataView
	{
		internal DataView(DataViewContext context)
		{
			this.context = context;
		}


		/// <summary>
		/// Creates a root data view starting with the specified root entity.
		/// </summary>
		/// <param name="context">The data view context.</param>
		/// <param name="root">The root entity.</param>
		/// <returns>A root data view.</returns>
		public static DataView CreateRoot(DataViewContext context, GenericEntity root)
		{
			DataItem item = new DataItems.EntityDataItem (context, root);
			return item.DataView;
		}

		/// <summary>
		/// Gets the data item for the specified view and path.
		/// </summary>
		/// <param name="view">The view.</param>
		/// <param name="path">The path.</param>
		/// <returns>The data item.</returns>
		public static IDataItem GetDataItem(DataView view, string path)
		{
			return DataView.GetDataItem (ref view, path);
		}

		/// <summary>
		/// Gets the data item for the specified view and path.
		/// </summary>
		/// <param name="view">The view; returns the immediate parent view of the item.</param>
		/// <param name="path">The path.</param>
		/// <returns>The data item.</returns>
		public static IDataItem GetDataItem(ref DataView view, string path)
		{
			if (view == null)
			{
				throw new System.ArgumentNullException ("view");
			}
			
			if (string.IsNullOrEmpty (path))
			{
				return view.self;
			}

			string[] pathElements = path.Split ('.');

			DataViewContext context = view.context;
			DataItem        item    = view.self;

			System.Diagnostics.Debug.Assert (item.DataView == view);

			int i = 0;
			int n = pathElements.Length;
			
			while (i < n)
			{
				string id = pathElements[i++];

				System.Diagnostics.Debug.Assert (id.Length > 0);
				System.Diagnostics.Debug.Assert (item != null);
				System.Diagnostics.Debug.Assert (item.ValueStore != null);
				System.Diagnostics.Debug.Assert (item.DataView != null);

				DataItem cachedItem;
				
				view = item.DataView;

				if ((view.items != null) &&
					(view.items.TryGetValue (id, out cachedItem)))
				{
					//	We have already been asked at least once about this path; we
					//	can reuse the existing data item, since the source graphe is
					//	read only

					item = cachedItem;
					continue;
				}

				//	Get the value for the specified id. If this fails, abort here and
				//	return an empty data item.

				IValueStore store = item.ValueStore;
				object      value = store.GetValue (id);

				if ((UndefinedValue.IsUndefinedValue (value)) ||
					(UnknownValue.IsUnknownValue (value)) ||
					(value == null))
				{
					return new DataItems.EmptyDataItem ();
				}

				//	If the item is stored as an enumerable, handle it as a collection.

				System.Collections.IEnumerable collection = context.GetCollection (value);

				if (collection != null)
				{
					//	The value is a collection; we will have to map it to a
					//	(sorted/filtered/grouped) table, as required.

					//	TODO: ...
					throw new System.NotImplementedException ();
				}
				else
				{
					//	If the item is stored as an entity, handle it as a row, otherwise
					//	handle it as a value.

					item = DataView.CreateSimpleDataItem (context, value);

					if ((i < n) &&
						(item.ValueStore == null))
					{
						//	Trying to get a value on something which does not implement
						//	the IValueStore interface is a fatal error, unless we have
						//	reached the end of the path.

						throw new System.InvalidOperationException (string.Format ("Path {0} contains an invalid node {1}", path, id));
					}
				}

				if (view.items == null)
				{
					view.items = new Dictionary<string, DataItem> ();
				}

				view.items[id] = item;
			}
			
			return item;
		}


		/// <summary>
		/// Creates a simple data item for a row or a value.
		/// </summary>
		/// <param name="context">The data view context.</param>
		/// <param name="value">The value.</param>
		/// <returns>The data item.</returns>
		private static DataItem CreateSimpleDataItem(DataViewContext context, object value)
		{
			AbstractEntity entity = value as AbstractEntity;
			DataItem item;

			if (entity != null)
			{
				item = new DataItems.EntityDataItem (context, entity);
				item.ItemClass = DataItemClass.ValueRow;
			}
			else
			{
				item = new DataItems.ValueDataItem (value);
				item.ItemClass = DataItemClass.ValueItem;
			}

			return item;
		}

		#region DataItem Class

		/// <summary>
		/// The <c>DataItem</c> class implements the common base class for every
		/// data item implementation in the <c>DataItems</c> sub-namespace.
		/// </summary>
		public abstract class DataItem : IDataItem
		{
			public DataItem()
			{
			}

			/// <summary>
			/// Gets or sets the containg data view.
			/// </summary>
			/// <value>The data view.</value>
			public DataView DataView
			{
				get
				{
					return this.view;
				}
				set
				{
					if (this.view != value)
					{
						if (this.view != null)
						{
							this.view.self = null;
						}

						this.view = value;

						if (this.view != null)
						{
							this.view.self = this;
						}
					}
				}
			}

			/// <summary>
			/// Gets the raw object value.
			/// </summary>
			/// <value>The raw object value.</value>
			public abstract object ObjectValue
			{
				get;
			}

			/// <summary>
			/// Gets the object value cast to the <see cref="IValueStore"/>
			/// interface.
			/// </summary>
			/// <value>The value store.</value>
			public IValueStore ValueStore
			{
				get
				{
					return this.ObjectValue as IValueStore;
				}
			}

			#region IDataItem Members

			public virtual string Value
			{
				get
				{
					throw new System.NotImplementedException ();
				}
			}

			public virtual int Count
			{
				get
				{
					throw new System.NotImplementedException ();
				}
			}

			public virtual DataItemClass ItemClass
			{
				get;
				set;
			}

			public virtual DataItemType ItemType
			{
				get
				{
					throw new System.NotImplementedException ();
				}
			}

			public virtual INamedType DataType
			{
				get
				{
					throw new System.NotImplementedException ();
				}
			}

			#endregion

			private DataView view;
		}

		#endregion


		private readonly DataViewContext context;
		private Dictionary<string, DataItem> items;
		private DataItem self;
	}
}

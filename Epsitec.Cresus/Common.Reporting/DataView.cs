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
		public DataView(DataViewContext context)
		{
			this.context = context;
		}


		public static DataView CreateRoot(DataViewContext context, GenericEntity root)
		{
			DataItem item = new DataItems.EntityDataItem (context, root);
			return item.DataView;
		}

		public static IDataItem GetValue(DataView view, string path)
		{
			return DataView.GetValue (ref view, path);
		}
		
		public static IDataItem GetValue(ref DataView view, string path)
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
				
				view = item.DataView;

				if ((view.items != null) &&
					(view.items.TryGetValue (id, out item)))
				{
					//	We have already been asked at least once about this path; we
					//	can reuse the existing data item, since the source graphe is
					//	read only

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

				System.Collections.IEnumerable collection = context.GetEnumerable (value);

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

		private static DataItem CreateSimpleDataItem(DataViewContext context, object value)
		{
			AbstractEntity entity = value as AbstractEntity;

			if (entity != null)
			{
				return new DataItems.EntityDataItem (context, entity);
			}
			else
			{
				return new DataItems.ValueDataItem (value);
			}
		}


		public abstract class DataItem : IDataItem
		{
			public DataItem()
			{
			}

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

			public abstract object ObjectValue
			{
				get;
			}

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
				get
				{
					throw new System.NotImplementedException ();
				}
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


		private readonly DataViewContext context;
		private Dictionary<string, DataItem> items;
		private DataItem self;
	}
}

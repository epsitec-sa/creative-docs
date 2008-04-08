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
		public DataView(DataViewContext context, AbstractEntity root)
		{
			this.context = context;
			this.root = root;
			this.items = new Dictionary<string, DataItem> ();
		}


		public static IDataItem GetValue(ref DataView view, string path)
		{
			if (string.IsNullOrEmpty (path))
			{
				return new DataItems.EntityDataItem (view.root);
			}

			string[] pathElements = path.Split ('.');

			DataViewContext context = view.context;
			AbstractEntity  entity  = view.root;
			DataItem        item    = null;
			object          value   = null;

			int i = 0;
			int n = pathElements.Length;
			
			while (i < n)
			{
				string id = pathElements[i++];

				if (view.items.TryGetValue (id, out item))
				{
					//	We have already been asked at least once about this path; we
					//	can reuse the existing data item, since the source graphe is
					//	read only

					view = item.DataView;
					continue;
				}

				//	Get the value for the specified id. If this fails, abort here and
				//	return an empty data item.

				IValueStore store = entity;
				value = store.GetValue (id);

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

					item = DataView.CreateSimpleDataItem (value, out entity);

					if ((entity == null) &&
						(i < n))
					{
						//	Trying to get a value on something which does not implement
						//	the IValueStore interface is a fatal error, unless we have
						//	reached the end of the path.

						throw new System.InvalidOperationException (string.Format ("Path {0} contains an invalid node {1}", path, id));
					}
				}

				view.items[id] = item;
				view = item.DataView;
			}
			
			return item;
		}

		private static DataItem CreateSimpleDataItem(object value, out AbstractEntity entity)
		{
			entity = value as AbstractEntity;

			if (entity != null)
			{
				return new DataItems.EntityDataItem (entity);
			}
			else
			{
				return new DataItems.ValueDataItem (value);
			}
		}


		public class DataItem : IDataItem
		{
			public DataItem()
			{
			}

			public DataView DataView
			{
				get;
				set;
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
		}


		private readonly DataViewContext context;
		private readonly AbstractEntity root;
		private readonly Dictionary<string, DataItem> items;
	}
}

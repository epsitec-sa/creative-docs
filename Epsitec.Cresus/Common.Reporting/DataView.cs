//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Reporting
{
	public partial class DataView
	{
		internal DataView(DataViewContext context)
		{
			this.context = context;
		}


		public string Path
		{
			get
			{
				return DataView.GetPath (this.self);
			}
		}

		public DataItem Item
		{
			get
			{
				return this.self;
			}
		}

		public DataViewContext Context
		{
			get
			{
				return this.context;
			}
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
			return DataView.GetDataItem (view, path, null);
		}

#if false
		/// <summary>
		/// Gets the data item for the specified view and path.
		/// </summary>
		/// <param name="view">The view; returns the immediate parent view of the item.</param>
		/// <param name="path">The path.</param>
		/// <returns>The data item.</returns>
		public static IDataItem GetDataItem(ref DataView view, string path)
		{
		}
#endif

		/// <summary>
		/// Gets the data item for the specified view and path.
		/// </summary>
		/// <param name="view">The view.</param>
		/// <param name="path">The path.</param>
		/// <param name="itemSpy">The data item spy which gets called for every item found in the path, or <c>null</c>.</param>
		/// <returns>The leaf data item.</returns>
		public static DataItem GetDataItem(DataView view, string path, System.Action<DataItem> itemSpy)
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
				System.Diagnostics.Debug.Assert (item.ValueStore != null || item.IsCollection);
				System.Diagnostics.Debug.Assert (item.DataView != null);

				DataItem cachedItem;
				
				view = item.DataView;

				if ((view.items != null) &&
					(view.items.TryGetValue (id, out cachedItem)))
				{
					//	We have already been asked at least once about this path; we
					//	can reuse the existing data item, since the source graph is
					//	read only

					item = cachedItem;
					
					if (itemSpy != null)
					{
						itemSpy (item);
					}

					continue;
				}

				//	Get the value for the specified id. If this fails, abort here and
				//	return an empty data item.

				object value;

				if (item.IsCollection)
				{
					if (id[0] != '@')
					{
						throw new System.InvalidOperationException (string.Format ("Path {0} contains invalid index {1}", path, id));
					}

					int index;

					if (int.TryParse (id.Substring (1), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out index))
					{
						value = item.GetValue (index);
					}
					else
					{
						throw new System.InvalidOperationException (string.Format ("Path {0} contains invalid index {1}", path, id));
					}
				}
				else
				{
					System.Diagnostics.Debug.Assert (id[0] != '@');
					
					IValueStore store = item.ValueStore;
					value = store.GetValue (id);
				}

				//	Now we should have a value to work with...

				if ((UndefinedValue.IsUndefinedValue (value)) ||
					(UnknownValue.IsUnknownValue (value)) ||
					(value == null))
				{
					return new DataItems.EmptyDataItem ();
				}

				//	If the item is stored as an enumeration of entities, handle it as a
				//	collection, which we map to a table.

				IEnumerable<AbstractEntity> collection = context.ToEntityEnumerable (value);

				if (collection != null)
				{
					//	The value is a collection of entities; we will have to map it to
					//	a (sorted/filtered/grouped) table, as required.

					string                     fullPath    = DataView.GetPath (view.self, id);
					Settings.CollectionSetting collSetting = context.GetCollectionSetting (fullPath);
					Settings.VectorSetting     vectSetting = context.GetVectorSetting (fullPath);

					if (collSetting.GroupDepth > 0)
					{
						//	TODO: grouping
						throw new System.NotImplementedException ();
					}
					else
					{
						List<AbstractEntity> list = collSetting.CreateList (collection);
						item = new DataItems.TableCollectionDataItem (context, list, collSetting, vectSetting);
						item.ItemClass = DataItemClass.Table;
					}
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

				DataView.RegisterItem (view, item, id);
				
				if (itemSpy != null)
				{
					itemSpy (item);
				}
			}
			
			return item;
		}

		public static void RegisterItem(DataView view, DataItem item, string id)
		{
			item.Id = id;
			item.ParentDataView = view;
			view.items[id] = item;
		}

		public static string GetVirtualNodeId(VirtualNodeType nodeType)
		{
			switch (nodeType)
			{
				case VirtualNodeType.Data:
				case VirtualNodeType.BodyData:
					return null;

				case VirtualNodeType.Header1:
					return "%Head1";
				case VirtualNodeType.Header2:
					return "%Head2";
				case VirtualNodeType.Footer1:
					return "%Foot1";
				case VirtualNodeType.Footer2:
					return "%Foot2";

				default:
					throw new System.InvalidOperationException (string.Format ("Invalid node type {0}", nodeType));
			}
		}


		/// <summary>
		/// Gets the full path for the specified item.
		/// </summary>
		/// <param name="dataItem">The data item.</param>
		/// <param name="additionalPathElements">The additional path elements.</param>
		/// <returns>The full path.</returns>
		public static string GetPath(IDataItem dataItem, params string[] additionalPathElements)
		{
			DataItem item = dataItem as DataItem;

			if (item == null)
			{
				throw new System.ArgumentException ("Invalid data item");
			}

			List<string> items = new List<string> (DataView.GetReversePath (item));
			items.Reverse ();
			items.AddRange (additionalPathElements);

			return string.Join (".", items.ToArray ());
		}

		/// <summary>
		/// Gets the reverse path for the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>The reverse path.</returns>
		private static IEnumerable<string> GetReversePath(DataItem item)
		{
			while ((item != null) && (item.Id != null))
			{
				yield return item.Id;
				
				DataView view = item.ParentDataView;
				
				if (view == null)
				{
					break;
				}

				item = view.self;
			}
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


		private readonly DataViewContext context;
		private Dictionary<string, DataItem> items;
		private DataItem self;
	}
}

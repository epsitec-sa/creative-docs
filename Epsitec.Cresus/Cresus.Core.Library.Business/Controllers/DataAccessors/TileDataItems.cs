//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types.Collections;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors
{
	public class TileDataItems : IEnumerable<TileDataItem>
	{
		public TileDataItems(EntityViewController controller)
		{
			this.controller = controller;

			this.simpleItems         = new List<TileDataItem> ();
			this.emptyItems          = new List<TileDataItem> ();
			this.collectionItems     = new List<TileDataItem> ();
			this.collectionAccessors = new List<CollectionAccessor> ();
			this.countedNames        = new CountedStringDictionary ();
		}


		public EntityViewController Controller
		{
			get
			{
				return this.controller;
			}
		}

		public object SyncObject
		{
			get
			{
				return this.exclusion;
			}
		}


		public void Add(TileDataItem data)
		{
			this.EnsureTileDataItemUniqueName (data);

			int rank = this.emptyItems.Count + this.simpleItems.Count;

			if (data.Rank == 0)
			{
				data.Rank = TileDataItem.CreateRank (rank+1, 0);
			}
			
			System.Diagnostics.Debug.Assert (data.DataType == TileDataType.Undefined);

			if (data.CreateEditionUI != null)
			{
				data.DataType = TileDataType.EditableItem;
				this.simpleItems.Add (data);
			}
			else if (data.CreateCustomizedUI != null)
			{
				data.DataType = TileDataType.CustomizedItem;
				this.simpleItems.Add (data);
			}
			else if (data.EntityMarshaler == null)
			{
				data.DataType = TileDataType.EmptyItem;
				this.emptyItems.Add (data);
			}
			else
			{
				data.DataType = TileDataType.SimpleItem;
				this.simpleItems.Add (data);
			}
		}

		public void Add(CollectionAccessor collectionAccessor)
		{
			System.Diagnostics.Debug.Assert (collectionAccessor.Template != null);

			this.collectionAccessors.Add (collectionAccessor);
		}

		
		public void RefreshCollectionItems()
		{
			var items = new List<TileDataItem> ();

			foreach (var accessor in this.collectionAccessors)
			{
				items.AddRange (accessor.Resolve (this.GetTemplate));
			}

			this.collectionItems.Clear ();
			this.collectionItems.AddRange (items);

			this.RefreshAddNewItemFunctionForEmptyItems ();
		}


		/// <summary>
		/// Ensures that the name of the tile data item is unique. This will add a suffix to the
		/// name if the name is already present in the collection (such as <c>Foo@01</c>).
		/// </summary>
		/// <param name="data">The tile data item.</param>
		private void EnsureTileDataItemUniqueName(TileDataItem data)
		{
			data.Name = this.countedNames.AddUnique (data.Name);
		}

		private void RefreshAddNewItemFunctionForEmptyItems()
		{
			foreach (var item in this.emptyItems)
			{
				if (item.AddNewItem == null)
				{
					var accessor = this.GetCollectionAccessor (item.Name);

					if (accessor != null)
					{
						accessor.Template.BindCreateItem (item, accessor);
					}
				}
			}
		}

		private CollectionAccessor GetCollectionAccessor(string templateName)
		{
			return this.collectionAccessors.FirstOrDefault (x => x.Template.NamePrefix == templateName);
		}

		private TileDataItem GetTemplate(string name, int index)
		{
			//	Look for templates in the existing collection items first, then
			//	in the empty items. This will enforce reuse of existing items.

			var items = this.collectionItems.Concat (this.emptyItems);
			return TileDataItems.GetTemplate (items, name, index);
		}

		/// <summary>
		/// Gets the <see cref="TileDataItem"/> template for the specified name; look it up in the
		/// collection. If an exact match (name + index) cannot be found, this will create a new
		/// template.
		/// </summary>
		/// <param name="collection">The collection of templates.</param>
		/// <param name="name">The name of the template.</param>
		/// <param name="index">The index of the template.</param>
		/// <returns>The <see cref="TileDataItem"/> template.</returns>
		private static TileDataItem GetTemplate(IEnumerable<TileDataItem> collection, string name, int index)
		{
			TileDataItem template;

			if (index == -1)
			{
				//	Never mind if the name does contain or not a suffix with the item index
				//	in it.
			}
			else
			{
				System.Diagnostics.Debug.Assert (name.Contains ('.'));
			}

			if ((TileDataItems.FindTemplate (collection, name, out template)) &&
				(index >= 0))
			{
				return TileDataItems.CreateSummayData (template, name, index);
			}
			else
			{
				return template;
			}
		}

		/// <summary>
		/// Finds the template and returns <c>true</c> if the template must be used to create a
		/// new instance of <see cref="TileDataItem"/>.
		/// </summary>
		/// <param name="collection">The collection.</param>
		/// <param name="name">The item name.</param>
		/// <param name="result">The matching template (if any).</param>
		/// <returns><c>true</c> if the caller should create a new <see cref="TileDataItem"/>; otherwise, <c>false</c>.</returns>
		private static bool FindTemplate(IEnumerable<TileDataItem> collection, string name, out TileDataItem result)
		{
			string prefix = TileDataItem.GetNamePrefix (name);
			string search = prefix + ".";

			TileDataItem template = null;

			foreach (var item in collection)
			{
				if (item.Name == name)
				{
					//	Exact match: return the item and tell the caller there is no need to
					//	create a new TileDataItem -- the template can be reused as is.

					result = item;
					return false;
				}

				//	If there is a partial match, remember the item as a possible template for
				//	creating the expected summary data instance :

				if (item.Name == prefix)
				{
					template = item;
				}

				if ((template == null) &&
					(item.Name.StartsWith (search, System.StringComparison.Ordinal)))
				{
					template = item;
				}
			}

			result = template;
			return result != null;
		}

		/// <summary>
		/// Creates a summay data based on the specified template.
		/// </summary>
		/// <param name="template">The template.</param>
		/// <param name="name">The name.</param>
		/// <param name="index">The index.</param>
		/// <returns>The summary data based on the specified template.</returns>
		private static TileDataItem CreateSummayData(TileDataItem template, string name, int index)
		{
			string prefix = TileDataItem.GetNamePrefix (name);
			
			string summaryName = TileDataItem.BuildName (prefix, index);
			int    summaryRank = TileDataItem.CreateRank (template.GroupingRank, index);

			return new TileDataItem (template)
			{
				Name = summaryName,
				Rank = summaryRank,
			};
		}


		private IEnumerable<TileDataItem> GetItems()
		{
			lock (this.SyncObject)
			{
				var itemNames = new HashSet<string> ();

				return new List<TileDataItem>
				(
					this.simpleItems.Concat (this.collectionItems.Where (x => itemNames.Add (x.Name)))
					.Concat (this.emptyItems.Where (x => itemNames.Add (x.Name + ".0")))
				);
			}
		}


		#region IEnumerable<TileDataItem> Members

		public IEnumerator<TileDataItem> GetEnumerator()
		{
			return this.GetItems ().GetEnumerator ();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetItems ().GetEnumerator ();
		}

		#endregion


		private readonly object exclusion = new object ();

		private readonly EntityViewController			controller;
		private readonly List<TileDataItem>				simpleItems;
		private readonly List<TileDataItem>				emptyItems;
		private readonly List<TileDataItem>				collectionItems;
		private readonly List<CollectionAccessor>		collectionAccessors;
		private readonly CountedStringDictionary		countedNames;
	}
}

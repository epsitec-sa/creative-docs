//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors
{
	public class SummaryDataItems : IEnumerable<SummaryData>
	{
		public SummaryDataItems(EntityViewController controller)
		{
			this.controller = controller;
			this.simpleItems = new List<SummaryData> ();
			this.emptyItems  = new List<SummaryData> ();
			this.collectionItems = new List<SummaryData> ();
			this.collectionAccessors = new List<CollectionAccessor> ();
		}


		public EntityViewController Controller
		{
			get
			{
				return this.controller;
			}
		}

		public IList<SummaryData> SimpleItems
		{
			get
			{
				return this.simpleItems;
			}
		}

		public IList<SummaryData> EmptyItems
		{
			get
			{
				return this.emptyItems;
			}
		}

		public IList<CollectionAccessor> CollectionAccessors
		{
			get
			{
				return this.collectionAccessors;
			}
		}


		public object SyncObject
		{
			get
			{
				return this.exclusion;
			}
		}


		public void Add(SummaryData data)
		{
			int rank = this.EmptyItems.Count + this.SimpleItems.Count;

			if (data.Rank == 0)
			{
				data.Rank = SummaryData.CreateRank (rank+1, 0);
			}
			
			System.Diagnostics.Debug.Assert (data.DataType == SummaryDataType.Undefined);
			
			if (data.EntityAccessor == null)
			{
				data.DataType = SummaryDataType.EmptyItem;
				this.EmptyItems.Add (data);
			}
			else
			{
				data.DataType = SummaryDataType.SimpleItem;
				this.SimpleItems.Add (data);
			}
		}

		public void Add(CollectionAccessor collectionAccessor)
		{
			System.Diagnostics.Debug.Assert (collectionAccessor.Template != null);

			this.CollectionAccessors.Add (collectionAccessor);
		}

		
		public void RefreshCollectionItems()
		{
			var items = new List<SummaryData> ();

			foreach (var accessor in this.collectionAccessors)
			{
				items.AddRange (accessor.Resolve (this.GetTemplate));
			}

			this.collectionItems.Clear ();
			this.collectionItems.AddRange (items);

			this.RefreshAddNewItemFunctionForEmptyItems ();
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

		private SummaryData GetTemplate(string name, int index)
		{
			//	Look for templates in the existing collection items first, then
			//	in the empty items. This will enforce reuse of existing items.

			var items = this.collectionItems.Concat (this.emptyItems);
			return CollectionAccessor.GetTemplate (items, name, index);
		}

		private IEnumerable<SummaryData> GetItems()
		{
			lock (this.SyncObject)
			{
				var itemNames = new HashSet<string> ();
				return new List<SummaryData> (this.simpleItems.Concat (this.collectionItems.Where (x => itemNames.Add (x.Name))).Concat (this.emptyItems.Where (x => itemNames.Add (x.Name + ".0"))));
			}
		}


		#region IEnumerable<SummaryData> Members

		public IEnumerator<SummaryData> GetEnumerator()
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

		private readonly EntityViewController controller;
		private readonly List<SummaryData> simpleItems;
		private readonly List<SummaryData> emptyItems;
		private readonly List<SummaryData> collectionItems;
		private readonly List<CollectionAccessor> collectionAccessors;
	}
}

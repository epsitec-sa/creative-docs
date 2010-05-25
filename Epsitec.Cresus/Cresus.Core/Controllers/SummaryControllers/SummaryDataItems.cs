//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryDataItems : IEnumerable<SummaryData>
	{
		public SummaryDataItems()
		{
			this.staticItems = new List<SummaryData> ();
			this.emptyItems  = new List<SummaryData> ();
			this.collectionItems = new List<SummaryData> ();
			this.collectionAccessors = new List<CollectionAccessor> ();
		}


		public IList<SummaryData> StaticItems
		{
			get
			{
				return this.staticItems;
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

		public void RefreshCollectionItems()
		{
			var items = new List<SummaryData> ();

			foreach (var accessor in this.collectionAccessors)
			{
				items.AddRange (accessor.Resolve (this.GetTemplate));
			}

			this.collectionItems.Clear ();
			this.collectionItems.AddRange (items);
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
				return new List<SummaryData> (this.staticItems.Concat (this.collectionItems).Concat (this.emptyItems).Where (x => itemNames.Add (x.Name)));
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

		private readonly List<SummaryData> staticItems;
		private readonly List<SummaryData> emptyItems;
		private readonly List<SummaryData> collectionItems;
		private readonly List<CollectionAccessor> collectionAccessors;
	}
}

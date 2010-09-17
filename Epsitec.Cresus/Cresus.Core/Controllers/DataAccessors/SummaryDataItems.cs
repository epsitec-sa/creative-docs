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

		public object SyncObject
		{
			get
			{
				return this.exclusion;
			}
		}


		public void Add(SummaryData data)
		{
			int rank = this.emptyItems.Count + this.simpleItems.Count;

			if (data.Rank == 0)
			{
				data.Rank = SummaryData.CreateRank (rank+1, 0);
			}
			
			System.Diagnostics.Debug.Assert (data.DataType == SummaryDataType.Undefined);
			
			if (data.EntityMarshaler == null)
			{
				data.DataType = SummaryDataType.EmptyItem;
				this.emptyItems.Add (data);
			}
			else
			{
				data.DataType = SummaryDataType.SimpleItem;
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
			return SummaryDataItems.GetTemplate (items, name, index);
		}

		/// <summary>
		/// Gets the <see cref="SummaryData"/> template for the specified name; look it up in the
		/// collection. If an exact match (name + index) cannot be found, this will create a new
		/// template.
		/// </summary>
		/// <param name="collection">The collection of templates.</param>
		/// <param name="name">The name of the template.</param>
		/// <param name="index">The index of the template.</param>
		/// <returns>The <see cref="SummaryData"/> template.</returns>
		private static SummaryData GetTemplate(IEnumerable<SummaryData> collection, string name, int index)
		{
			SummaryData template;

			System.Diagnostics.Debug.Assert (name.Contains ('.'));

			if (SummaryDataItems.FindTemplate (collection, name, out template))
			{
				return SummaryDataItems.CreateSummayData (template, name, index);
			}
			else
			{
				return template;
			}
		}

		/// <summary>
		/// Finds the template and returns <c>true</c> if the template must be used to create a
		/// new instance of <see cref="SummaryData"/>.
		/// </summary>
		/// <param name="collection">The collection.</param>
		/// <param name="name">The item name.</param>
		/// <param name="result">The matching template (if any).</param>
		/// <returns><c>true</c> if the caller should create a new <see cref="SummaryData"/>; otherwise, <c>false</c>.</returns>
		private static bool FindTemplate(IEnumerable<SummaryData> collection, string name, out SummaryData result)
		{
			string prefix = SummaryData.GetNamePrefix (name);
			string search = prefix + ".";

			SummaryData template = null;

			foreach (var item in collection)
			{
				if (item.Name == name)
				{
					//	Exact match: return the item and tell the caller there is no need to
					//	create a new SummaryData -- the template can be reused as is.

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

		private static SummaryData CreateSummayData(SummaryData template, string name, int index)
		{
			string prefix = SummaryData.GetNamePrefix (name);

			return new SummaryData
			{
				Name         = SummaryData.BuildName (prefix, index),
				AutoGroup    = template.AutoGroup,
				IconUri      = template.IconUri,
				Title        = template.Title,
				CompactTitle = template.CompactTitle,
				Rank         = SummaryData.CreateRank (template.GroupingRank, index),
			};
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

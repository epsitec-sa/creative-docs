//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors
{
	public class CollectionTemplate<T> : ICollectionTemplate
			where T : AbstractEntity, new ()
	{
		public CollectionTemplate(string name)
		{
			this.name = name;

			this.DefineCreateItem (() => EntityContext.Current.CreateEmptyEntity<T> ());
			this.DefineDeleteItem (item =>
			{
			});
		}

		public CollectionTemplate(string name, System.Predicate<T> filter)
			: this (name)
		{
			this.Filter = filter;
		}

		public bool HasCreateItem
		{
			get
			{
				return this.createItem != null;
			}
		}

		public bool HasDeleteItem
		{
			get
			{
				return this.deleteItem != null;
			}
		}

		public CollectionTemplate<T> DefineCreateItem(System.Func<T> action)
		{
			this.createItem = action;
			return this;
		}

		public CollectionTemplate<T> DefineDeleteItem(System.Action<T> action)
		{
			this.deleteItem = action;
			return this;
		}

		public CollectionTemplate<T> DefineSetupItem(System.Action<T> action)
		{
			this.setupItem = action;
			return this;
		}

		public CollectionTemplate<T> DefineTitle(System.Func<T, FormattedText> action)
		{
			this.TitleAccessor = IndirectAccessor<T>.Create (action);
			return this;
		}

		public CollectionTemplate<T> DefineText(System.Func<T, FormattedText> action)
		{
			this.TextAccessor = IndirectAccessor<T>.Create (action);
			return this;
		}

		public CollectionTemplate<T> DefineCompactTitle(System.Func<T, FormattedText> action)
		{
			this.CompactTitleAccessor = IndirectAccessor<T>.Create (action);
			return this;
		}

		public CollectionTemplate<T> DefineCompactText(System.Func<T, FormattedText> action)
		{
			this.CompactTextAccessor = IndirectAccessor<T>.Create (action);
			return this;
		}

		public IndirectAccessor<T, FormattedText> TitleAccessor
		{
			get;
			set;
		}

		public IndirectAccessor<T, FormattedText> TextAccessor
		{
			get;
			set;
		}

		public IndirectAccessor<T, FormattedText> CompactTitleAccessor
		{
			get;
			set;
		}

		public IndirectAccessor<T, FormattedText> CompactTextAccessor
		{
			get;
			set;
		}

		public System.Predicate<T> Filter
		{
			get;
			set;
		}

		public string NamePrefix
		{
			get
			{
				return this.name;
			}
		}

		public bool IsCompatible(AbstractEntity entity)
		{
			T source = entity as T;

			if (source == null)
			{
				return false;
			}
			else if (this.Filter == null)
			{
				return true;
			}
			else
			{
				return this.Filter (source);
			}
		}

		public void BindSummaryData(SummaryData data, AbstractEntity entity, ICollectionAccessor collectionAccessor)
		{
			T source = entity as T;

			data.EntityAccessor		  = () => source;
			data.TitleAccessor        = IndirectAccessor<T, FormattedText>.GetAccessor (this.TitleAccessor, source);
			data.TextAccessor         = IndirectAccessor<T, FormattedText>.GetAccessor (this.TextAccessor, source);
			data.CompactTitleAccessor = IndirectAccessor<T, FormattedText>.GetAccessor (this.CompactTitleAccessor, source);
			data.CompactTextAccessor  = IndirectAccessor<T, FormattedText>.GetAccessor (this.CompactTextAccessor, source);
			data.DataType			  = SummaryDataType.CollectionItem;

			if (this.HasCreateItem && this.HasDeleteItem && collectionAccessor != null)
			{
				data.AddNewItem = () => collectionAccessor.AddItem (this.CreateItem ());
				data.DeleteItem = () => collectionAccessor.RemoveItem (source);
			}
		}

		public void BindCreateItem(SummaryData data, ICollectionAccessor collectionAccessor)
		{
			if (this.HasCreateItem && collectionAccessor != null)
			{
				data.AddNewItem = () => collectionAccessor.AddItem (this.CreateItem ());
			}
		}

		public T CreateItem()
		{
			var item = this.createItem ();

			if (item != null)
			{
				EntityNullReferenceVirtualizer.PatchNullReferences (item);

				if (this.setupItem != null)
				{
					this.setupItem (item);
				}
			}

			return item;
		}

		public void DeleteItem(T item)
		{
			this.deleteItem (item);
		}


		#region ICollectionTemplate Members

		AbstractEntity ICollectionTemplate.CreateItem()
		{
			return this.CreateItem ();
		}

		void ICollectionTemplate.DeleteItem(AbstractEntity item)
		{
			this.DeleteItem (item as T);
		}

		#endregion

		private readonly string name;
		private System.Func<T> createItem;
		private System.Action<T> deleteItem;
		private System.Action<T> setupItem;
	}
}

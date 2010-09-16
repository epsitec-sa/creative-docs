//	Copyright � 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.BusinessLogic;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors
{
	public class CollectionTemplate<T> : CollectionTemplate
			where T : AbstractEntity, new ()
	{
		public CollectionTemplate(string name, EntityViewController controller, DataContext dataContext)
			: base (name)
		{
			this.DefineCreateItem (() => CollectionTemplate<T>.CreateEmptyItem (dataContext));
		}

		public CollectionTemplate(string name, BusinessContext businessContext)
			: base (name)
		{
			this.businessContext = businessContext;

			this.DefineCreateItem (() => this.businessContext.CreateEntityAndRegisterAsEmpty<T> ());
			this.DefineDeleteItem (x => this.businessContext.ArchiveEntity<T> (x));
		}

		public CollectionTemplate(string name, EntityViewController controller, DataContext dataContext, System.Predicate<T> filter)
			: this (name, controller, dataContext)
		{
			this.Filter = filter;
		}


		public BusinessContext BusinessContext
		{
			get
			{
				return this.businessContext;
			}
		}

		
		public bool HasCreateItem
		{
			get
			{
				return this.createItem != null;
			}
		}

		public bool HasCreateGetIndex
		{
			get
			{
				return this.createGetIndex != null;
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
			//	D�finition de l'action qui cr�e une nouvelle entit�.
			this.createItem = action;
			return this;
		}

		public CollectionTemplate<T> DefineCreateGetIndex(System.Func<int> action)
		{
			//	D�finition de l'action qui retourne l'index o� ins�rer la nouvelle entit� cr��e.
			this.createGetIndex = action;
			return this;
		}

		public CollectionTemplate<T> DefineDeleteItem(System.Action<T> action)
		{
			//	D�finition de l'action qui supprime une entit�.
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

		public override bool IsCompatible(AbstractEntity entity)
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

		public override void BindSummaryData(SummaryData data, AbstractEntity entity, Marshaler marshaler, ICollectionAccessor collectionAccessor)
		{
			T source = entity as T;

			data.EntityMarshaler = marshaler;
			data.DataType		 = SummaryDataType.CollectionItem;
			
			var context = Epsitec.Cresus.DataLayer.Context.DataContextPool.Instance.FindDataContext (source);

			if ((context != null) &&
				(context.IsRegisteredAsEmptyEntity (source)))
			{
				this.BindEmptyEntitySummaryData (data, source);
			}
			else
			{
				this.BindRealEntitySummaryData (data, source, collectionAccessor);
			}

			if (this.HasCreateItem && this.HasDeleteItem && collectionAccessor != null)
			{
				data.AddNewItem = () => this.CreateItem (data, collectionAccessor);
				data.DeleteItem = () => this.DeleteItem (data, source, collectionAccessor);
			}

			data.GroupController = new GroupedItemController (collectionAccessor.GetItemCollection (), source, x => this.IsCompatible (x));
		}

		public override void BindCreateItem(SummaryData data, ICollectionAccessor collectionAccessor)
		{
			if (this.HasCreateItem && collectionAccessor != null)
			{
				data.AddNewItem = () => this.CreateItem (data, collectionAccessor);
			}
		}

		private void CreateItem(SummaryData data, ICollectionAccessor collectionAccessor)
		{
			//	Cr�e une nouvelle entit� et ins�re-la au bon endroit.
			//	Si aucune action CreateGetIndex n'est d�finie, elle est ins�r�e � la fin.
			//	Sinon, CreateGetIndex d�termine l'index � utiliser.
			int index = collectionAccessor.GetItemCollection ().Count;  // index pour ins�rer � la fin

			if (this.HasCreateGetIndex)  // action CreateGetIndex d�finie ?
			{
				index = this.createGetIndex ();  // index selon l'action
			}

			T item = this.GenericCreateItem ();
			collectionAccessor.InsertItem (index, item);

			// M�morise l'index de l'entit� ins�r�e, pour permettre de la s�lectionner dans la tuile.
			data.CreatedIndex = index;
		}

		
		private void DeleteItem(SummaryData data, T item, ICollectionAccessor collectionAccessor)
		{
			collectionAccessor.RemoveItem (item);
			this.GenericDeleteItem (item);
		}

		private static T CreateEmptyItem(DataContext dataContext)
		{
			return dataContext.CreateEntityAndRegisterAsEmpty<T> ();
		}

		private void BindEmptyEntitySummaryData(SummaryData data, T source)
		{
			data.TitleAccessor        = IndirectAccessor<T, FormattedText>.GetAccessor (this.TitleAccessor, source);
			data.TextAccessor         = IndirectAccessor<T, FormattedText>.GetAccessor (this.TextAccessor, source, CollectionTemplate.DefaultDefinitionInProgressText, x => x.IsNullOrEmpty);
			data.CompactTitleAccessor = IndirectAccessor<T, FormattedText>.GetAccessor (this.CompactTitleAccessor, source);
			data.CompactTextAccessor  = IndirectAccessor<T, FormattedText>.GetAccessor (this.CompactTextAccessor, source, CollectionTemplate.DefaultDefinitionInProgressText, x => x.IsNullOrEmpty);
		}

		private void BindRealEntitySummaryData(SummaryData data, T source, ICollectionAccessor collectionAccessor)
		{
			data.TitleAccessor        = IndirectAccessor<T, FormattedText>.GetAccessor (this.TitleAccessor, source);
			data.TextAccessor         = IndirectAccessor<T, FormattedText>.GetAccessor (this.TextAccessor, source, CollectionTemplate.DefaultEmptyText, x => x.IsNullOrEmpty);
			data.CompactTitleAccessor = IndirectAccessor<T, FormattedText>.GetAccessor (this.CompactTitleAccessor, source);
			data.CompactTextAccessor  = IndirectAccessor<T, FormattedText>.GetAccessor (this.CompactTextAccessor, source, CollectionTemplate.DefaultEmptyText, x => x.IsNullOrEmpty);
		}
		
		private T GenericCreateItem()
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

		private void GenericDeleteItem(T item)
		{
			this.deleteItem (item);
		}

		private readonly BusinessContext businessContext;

		private System.Func<T> createItem;
		private System.Func<int> createGetIndex;
		private System.Action<T> deleteItem;
		private System.Action<T> setupItem;
	}
}

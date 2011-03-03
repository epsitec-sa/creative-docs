//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Business;

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
			this.businessContext = null;
			this.dataContextPool = dataContext.DataContextPool;

			this.DefineCreateItem (() => dataContext.CreateEntityAndRegisterAsEmpty<T> ());
		}

		public CollectionTemplate(string name, BusinessContext businessContext)
			: base (name)
		{
			this.businessContext = businessContext;
			this.dataContextPool = businessContext.Data.DataContextPool;

			this.DefineCreateItem (() => this.businessContext.CreateEntityAndRegisterAsEmpty<T> ());
			this.DefineDeleteItem (x => this.businessContext.ArchiveEntity<T> (x));
		}

		public CollectionTemplate(string name, EntityViewController controller, DataContext dataContext, System.Predicate<T> filter)
			: this (name, controller, dataContext)
		{
			this.Filter = filter;
		}


		public BusinessContext					BusinessContext
		{
			get
			{
				return this.businessContext;
			}
		}
		
		public bool								HasCreateItem
		{
			get
			{
				return this.createItem != null;
			}
		}

		public bool								HasCreateGetIndex
		{
			get
			{
				return this.createGetIndex != null;
			}
		}

		public bool								HasDeleteItem
		{
			get
			{
				return this.deleteItem != null;
			}
		}

		public System.Predicate<T>				Filter
		{
			get;
			set;
		}


	
		private IndirectAccessor<T, FormattedText>	TitleAccessor
		{
			get;
			set;
		}

		private IndirectAccessor<T, FormattedText> TextAccessor
		{
			get;
			set;
		}

		private IndirectAccessor<T, FormattedText> CompactTitleAccessor
		{
			get;
			set;
		}

		private IndirectAccessor<T, FormattedText> CompactTextAccessor
		{
			get;
			set;
		}

		
		public CollectionTemplate<T> CreateItemsOfType<T1>()
			where T1 : T, new ()
		{
			System.Diagnostics.Debug.Assert (this.businessContext != null);
			System.Diagnostics.Debug.Assert (this.createItem != null);
			System.Diagnostics.Debug.Assert (this.deleteItem != null);

			this.createItem = () => this.businessContext.CreateEntityAndRegisterAsEmpty<T1> ();
			
			return this;
		}

		
		public CollectionTemplate<T> DefineCreateItem(System.Func<T> action)
		{
			//	Définition de l'action qui crée une nouvelle entité.
			this.createItem = action;
			return this;
		}

		public CollectionTemplate<T> DefineCreateGetIndex(System.Func<int> action)
		{
			//	Définition de l'action qui retourne l'index où insérer la nouvelle entité créée.
			this.createGetIndex = action;
			return this;
		}

		public CollectionTemplate<T> DefineDeleteItem(System.Action<T> action)
		{
			//	Définition de l'action qui supprime une entité.
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
			this.TitleAccessor = IndirectAccessor.Create (action);
			return this;
		}

		public CollectionTemplate<T> DefineText(System.Func<T, FormattedText> action)
		{
			this.TextAccessor = IndirectAccessor.Create (action);
			return this;
		}

		public CollectionTemplate<T> DefineCompactTitle(System.Func<T, FormattedText> action)
		{
			this.CompactTitleAccessor = IndirectAccessor.Create (action);
			return this;
		}

		public CollectionTemplate<T> DefineCompactText(System.Func<T, FormattedText> action)
		{
			this.CompactTextAccessor = IndirectAccessor.Create (action);
			return this;
		}

		
		public override void GenericDefine(CollectionTemplateProperty property, object value)
		{
			switch (property)
			{
				case CollectionTemplateProperty.Title:
					this.DefineTitle ((System.Func<T, FormattedText>) value);
					break;

				case CollectionTemplateProperty.Text:
					this.DefineText ((System.Func<T, FormattedText>) value);
					break;
				
				case CollectionTemplateProperty.CompactTitle:
					this.DefineCompactTitle ((System.Func<T, FormattedText>) value);
					break;
				
				case CollectionTemplateProperty.CompactText:
					this.DefineCompactText ((System.Func<T, FormattedText>) value);
					break;
			}
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

		public override void BindTileData(TileDataItem data, AbstractEntity entity, Marshaler marshaler, ICollectionAccessor collectionAccessor)
		{
			T source = entity as T;

			data.EntityMarshaler = marshaler;
			data.DataType		 = TileDataType.CollectionItem;

			var context = this.dataContextPool.FindDataContext (source);

			if ((context != null) &&
				(context.IsRegisteredAsEmptyEntity (source)))
			{
				this.BindEmptyEntityTileData (data, source);
			}
			else
			{
				this.BindRealEntityTileData (data, source, collectionAccessor);
			}

			if (collectionAccessor != null)
			{
				if (this.HasCreateItem)
				{
					data.AddNewItem = () => this.CreateItem (data, collectionAccessor);
				}
				if (this.HasDeleteItem)
				{
					data.DeleteItem = () => this.DeleteItem (data, source, collectionAccessor);
				}

				data.GroupController = new GroupedItemController (collectionAccessor, source, x => this.IsCompatible (x));
			}
		}

		public override void BindCreateItem(TileDataItem data, ICollectionAccessor collectionAccessor)
		{
			if (this.HasCreateItem && collectionAccessor != null)
			{
				data.AddNewItem = () => this.CreateItem (data, collectionAccessor);
			}
		}

		
		private void CreateItem(TileDataItem data, ICollectionAccessor collectionAccessor)
		{
			//	Crée une nouvelle entité et insère-la au bon endroit.
			//	Si aucune action CreateGetIndex n'est définie, elle est insérée à la fin.
			//	Sinon, CreateGetIndex détermine l'index à utiliser.
			int index = -1;

			if (this.HasCreateGetIndex)  // action CreateGetIndex définie ?
			{
				index = this.createGetIndex ();  // index selon l'action
			}

			T item = this.GenericCreateItem ();

			if (index < 0)
			{
				index = collectionAccessor.AddItem (item);
			}
			else
			{
				collectionAccessor.InsertItem (index, item);
			}

			// Mémorise l'index de l'entité insérée, pour permettre de la sélectionner dans la tuile.
			data.CreatedIndex = index;
		}
		
		private void DeleteItem(TileDataItem data, T item, ICollectionAccessor collectionAccessor)
		{
			collectionAccessor.RemoveItem (item);
			this.GenericDeleteItem (item);
		}


		private void BindEmptyEntityTileData(TileDataItem data, T source)
		{
			data.TitleAccessor        = IndirectAccessor<T, FormattedText>.GetAccessor (this.TitleAccessor, source);
			data.TextAccessor         = IndirectAccessor<T, FormattedText>.GetAccessor (this.TextAccessor, source, CollectionTemplate.DefaultDefinitionInProgressText, x => x.IsNullOrEmpty);
			data.CompactTitleAccessor = IndirectAccessor<T, FormattedText>.GetAccessor (this.CompactTitleAccessor, source);
			data.CompactTextAccessor  = IndirectAccessor<T, FormattedText>.GetAccessor (this.CompactTextAccessor, source, CollectionTemplate.DefaultDefinitionInProgressText, x => x.IsNullOrEmpty);
		}

		private void BindRealEntityTileData(TileDataItem data, T source, ICollectionAccessor collectionAccessor)
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

		private readonly BusinessContext		businessContext;
		private readonly DataContextPool		dataContextPool;

		private System.Func<T>					createItem;
		private System.Func<int>				createGetIndex;
		private System.Action<T>				deleteItem;
		private System.Action<T>				setupItem;
	}
}

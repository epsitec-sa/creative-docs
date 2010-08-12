//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.DataLayer.Context;


// TODO There was some kind of bug or something was not completely implemented.
// With the old version of the DataLayer, created entities where not unregistered as empty entities
// when they changed, so they where not saved. That resulted in the creation of a new entity not
// persisted to the database. When the entity changed, the "définition en cours" message was changed
// (I don't know how) to the real summary text.
// With the new DataLayer, the created entities where still not unregistered as empty entities, but
// the update of the "définition en cours" feature was broken. There was probably a subtle change
// that I can't find.
// To correct that, I added an event to AbstractEntity so that we know when an entity changes
// and when it is fired, we unregister the changed entity as an empty entity. This solves the problem
// of the entities not being persisted, but I didn't found a way to refresh the display so we can
// update the "définition en cours" message.
// That means that there is some work to do, because that feature is not implemented correctly, it is
// implemented to that it minimaly works in good cases.
// 1) Check that this way of doing this stuff is a good solution.
// 2) Refresh the display when an entity is changed so we can update the "définition en cours" message.
// 3) Unregister the CollectionTemplate from the event of an entity when the entity has changed once
//    (because we need to unregister from the empty entities only once) and if the CollectionTemplate
//    is "disposed" because then we don't need to listen to that event anymore.
// 4) Add a Dispose() method to CollectionTemplate so that we can call it to unregister the change
//    events that are still registered.
// I put a comment with text "// Stuff added for the entity change feature." everywhere in the code
// where I made a modification regarding that feature. For any question about all that stuff, ask Marc.
// Marc

namespace Epsitec.Cresus.Core.Controllers.DataAccessors
{
	/// <summary>
	/// The <c>CollectionTemplate</c> class provides the basic functionality
	/// needed to create and delete items, related to a <see cref="CollectionAccessor"/>.
	/// </summary>
	public abstract class CollectionTemplate
	{
		protected CollectionTemplate(string name)
		{
			this.name = name;
		}

		public string NamePrefix
		{
			get
			{
				return this.name;
			}
		}

		public abstract bool IsCompatible(AbstractEntity entity);

		public abstract void BindSummaryData(SummaryData data, AbstractEntity entity, Marshaler marshaler, ICollectionAccessor collectionAccessor);

		public abstract AbstractEntity CreateItem();
		
		public abstract void DeleteItem(AbstractEntity item);

		public abstract void BindCreateItem(SummaryData data, ICollectionAccessor collectionAccessor);

		public static readonly FormattedText DefaultEmptyText = TextFormatter.FormatText ("<i>vide</i>");
		public static readonly FormattedText DefaultDefinitionInProgressText = TextFormatter.FormatText ("<i>définition en cours</i>");

		private readonly string name;
	}

	public class CollectionTemplate<T> : CollectionTemplate
			where T : AbstractEntity, new ()
	{
		public CollectionTemplate(string name, EntityViewController controller, DataContext dataContext)
			: base (name)
		{
			this.DefineCreateItem (() => controller.NotifyChildItemCreated (CollectionTemplate<T>.CreateEmptyItem (dataContext)));
			this.DefineDeleteItem (item => controller.NotifyChildItemDeleted (item));
			
			// Stuff added for the entity change feature.
			this.DefineChangeItem ((s, e) =>
			{
				dataContext.UnregisterEmptyEntity (e.Entity);
			});
		}

		public CollectionTemplate(string name, EntityViewController controller, DataContext dataContext, System.Predicate<T> filter)
			: this (name, controller, dataContext)
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

		// Stuff added for the entity change feature.
		public CollectionTemplate<T> DefineChangeItem(System.Action<T, Epsitec.Common.Support.EntityEngine.EntityFieldChangedEventArgs> action)
		{
			this.changeItem = action;
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
				data.DeleteItem = () => collectionAccessor.RemoveItem (source);
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
			//	Crée une nouvelle entité et insère-la au bon endroit.
			//	Si aucune action CreateGetIndex n'est définie, elle est insérée à la fin.
			//	Sinon, CreateGetIndex détermine l'index à utiliser.
			int index = collectionAccessor.GetItemCollection ().Count;  // index pour insérer à la fin

			if (this.HasCreateGetIndex)  // action CreateGetIndex définie ?
			{
				index = this.createGetIndex ();  // index selon l'action
			}

			collectionAccessor.InsertItem (index, this.GenericCreateItem ());

			// Mémorise l'index de l'entité insérée, pour permettre de la sélectionner dans la tuile.
			data.CreatedIndex = index;
		}

		
		public override AbstractEntity CreateItem()
		{
			return this.GenericCreateItem ();
		}

		public override void DeleteItem(AbstractEntity item)
		{
			this.GenericDeleteItem (item as T);
		}

		private static T CreateEmptyItem(DataContext dataContext)
		{
			return dataContext.CreateEmptyEntity<T> ();
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

			// Stuff added for the entity change feature.
			item.EntityChanged += (s, e) => this.changeItem ((T) s, e);
			
			return item;
		}

		private void GenericDeleteItem(T item)
		{
			this.deleteItem (item);
		}


		private System.Func<T> createItem;
		private System.Func<int> createGetIndex;
		private System.Action<T> deleteItem;
		private System.Action<T> setupItem;

		// Stuff added for the entity change feature.
		private System.Action<T, Epsitec.Common.Support.EntityEngine.EntityFieldChangedEventArgs> changeItem;
	}
}

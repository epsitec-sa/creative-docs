//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;

using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Controllers
{
	/// <summary>
	/// The <c>BindingController</c> class is used to bind an entity with one of
	/// the item picker widgets.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class SelectionController<T> : IWidgetUpdater
		where T : AbstractEntity, new ()
	{
		public SelectionController(BusinessContext businessContext)
		{
			this.businessContext = businessContext;

			this.ToTextArrayConverter     = x => x.GetEntityKeywords ();
			this.ToFormattedTextConverter = x => x.GetCompactSummary ();
		}
		

		public System.Func<IEnumerable<T>>		PossibleItemsGetter
		{
			get;
			set;
		}

		public System.Predicate<T>				PossibleItemsFilter
		{
			get;
			set;
		}

		public System.Action<T>					ValueSetter
		{
			get;
			set;
		}

		public System.Func<T>					ValueGetter
		{
			set
			{
				this.valueGetter = value;
			}
		}

		public ReferenceController				ReferenceController
		{
			get;
			set;
		}

		public System.Func<IList<T>>			CollectionValueGetter
		{
			get;
			set;
		}

		public System.Func<T, string[]>			ToTextArrayConverter
		{
			get;
			set;
		}

		public System.Func<T, FormattedText>	ToFormattedTextConverter
		{
			get;
			set;
		}


		public void Attach(Widgets.AutoCompleteTextField widget)
		{
			this.attachedWidget = widget;
			this.widgetItems    = widget.Items;

			widget.ValueToDescriptionConverter = this.ConvertHintValueToDescription;

			widget.HintComparer = (value, text) => this.MatchUserText (value as T, text);
			widget.HintComparisonConverter = x => HintComparer.GetComparableText (x);

			widget.EditionAccepted += this.HandleEditionAccepted;

			this.Update ();
		}

		public void Attach(Widgets.ItemPicker widget)
		{
			this.attachedPicker = widget;
			this.widgetItems    = widget.Items;

			widget.ValueToDescriptionConverter = this.ConvertHintValueToDescription;

			if (this.CollectionValueGetter != null)
			{
				this.AttachMultipleValueSelector ();
			}
			else if ((this.valueGetter != null) &&
					 (this.ValueSetter != null))
			{
				this.AttachSingleValueSelector ();
			}

			this.Update ();
		}

		public void SetValue(T value)
		{
			if (this.ValueSetter == null)
			{
				throw new System.InvalidOperationException ("Cannot set value without setter");
			}
			else
			{
				this.ValueSetter (this.GetBusinessContextCompatibleEntity (value));
			}
		}

		public T GetValue()
		{
			if (this.valueGetter == null)
			{
				return null;
			}

			return this.valueGetter ();
		}

		public IEnumerable<T> GetPossibleItems()
		{
			IEnumerable<T> collection;

			if (this.PossibleItemsGetter == null)
			{
				 collection = this.businessContext.Data.GetAllEntities<T> (DataExtractionMode.Sorted, this.businessContext.DataContext);
			}
			else
			{
				collection = this.PossibleItemsGetter ();
			}

			if (this.PossibleItemsFilter == null)
			{
				return collection;
			}
			else
			{
				return collection.Where (x => this.PossibleItemsFilter (x));
			}
		}

		private T GetBusinessContextCompatibleEntity(T value)
		{
			System.Diagnostics.Debug.Assert (this.businessContext != null);

			if (this.businessContext != null)
			{
				return this.businessContext.GetLocalEntity (value);
			}
			else
			{
				return value;
			}
		}


		#region PickerMode Enumeration

		private enum PickerMode
		{
			None,
			SingleValue,
			MultipleValue,
		}

		#endregion

		#region IWidgetUpdater Members

		public void Update()
		{
			this.widgetItems.Clear ();
			this.widgetItems.AddRange (this.GetPossibleItems ());
			
			if (this.attachedWidget != null)
			{
				T currentValue = this.GetValue ();
				this.attachedWidget.SelectedItemIndex = widgetItems.FindIndexByValue<T> (x => x.DbKeyEquals (currentValue));
			}
			if (this.attachedPicker != null)
			{
				this.attachedPicker.RefreshContents ();

				switch (this.attachedPickerMode)
				{
					case PickerMode.MultipleValue:
						var originalItems = this.CollectionValueGetter ();
						this.attachedPicker.AddSelection (originalItems.Select (x => this.widgetItems.FindIndexByValue<T> (y => x.DbKeyEquals (y))).Where (x => x != -1));
						break;

					case PickerMode.SingleValue:
						var initialValue  = this.GetValue ();
						int selectedIndex = this.widgetItems.FindIndexByValue<T> (x => x.DbKeyEquals (initialValue));
						this.attachedPicker.AddSelection (selectedIndex);
						break;
				}
			}
		}

		#endregion

		
		private void AttachMultipleValueSelector()
		{
			this.attachedPickerMode = PickerMode.MultipleValue;
			this.attachedPicker.SelectedItemChanged += this.HandleMultiSelectionChanged;
		}

		private void AttachSingleValueSelector()
		{
			this.attachedPickerMode = PickerMode.SingleValue;
			this.attachedPicker.SelectedItemChanged += this.HandleSingleSelectionChanged;
		}

		private void HandleEditionAccepted(object sender)
		{
			if (this.attachedWidget.SelectedItemIndex > -1)
			{
				this.SetValue (this.widgetItems.GetValue<T> (this.attachedWidget.SelectedItemIndex));
			}
			else
			{
				this.SetValue (null);
			}
		}
		
		private void HandleMultiSelectionChanged(object sender)
		{
			var selectedItems = this.CollectionValueGetter ();
			var newSelection  = new List<T> ();

			var indexes = this.attachedPicker.GetSortedSelection ();

			foreach (int selectedIndex in indexes)
			{
				var item = this.widgetItems.GetValue<T> (selectedIndex);
				newSelection.Add (this.GetBusinessContextCompatibleEntity (item));
			}

			if (Comparer.EqualObjects (selectedItems, newSelection))
			{
				return;
			}
			
			using (this.SuspendNotifications (selectedItems))
			{
				selectedItems.Clear ();
				selectedItems.AddRange (newSelection);
			}
		}

		private void HandleSingleSelectionChanged(object sender)
		{
			this.SetValue (this.widgetItems.GetValue<T> (this.attachedPicker.SelectedItemIndex));
		}

		private System.IDisposable SuspendNotifications(IList<T> list)
		{
			var suspendCollection = list as ISuspendCollectionChanged;
			
			if (suspendCollection != null)
			{
				return suspendCollection.SuspendNotifications ();
			}
			else
			{
				return null;
			}
		}

		private FormattedText ConvertHintValueToDescription(object value)
		{
			var entity = value as T;
			
			if ((entity.IsNull ()) ||
				(this.ToFormattedTextConverter == null))
			{
				return FormattedText.Empty;
			}

			var data = this.businessContext.Data;
			var context = data.DataContextPool.FindDataContext (entity);

			if ((context != null) &&
				(context.IsRegisteredAsEmptyEntity (entity)) &&
				(entity.GetEntityStatus () != EntityStatus.Valid))
			{
				return Epsitec.Cresus.Core.Controllers.DataAccessors.CollectionTemplate.DefaultDefinitionInProgressText;
			}
			else
			{
				return this.ToFormattedTextConverter (entity);
			}
		}

		private Widgets.HintComparerResult MatchUserText(T entity, string userText)
		{
			if ((entity == null) ||
				(this.ToTextArrayConverter == null))
			{
				return Widgets.HintComparerResult.NoMatch;
			}

			var texts = this.ToTextArrayConverter (entity);
			var result = Widgets.HintComparerResult.NoMatch;

			if (texts == null)
			{
				throw new System.InvalidOperationException ("Source entity probably missing a GetEntityKeywords implementation");
			}

			foreach (var text in texts)
			{
				var itemText = HintComparer.GetComparableText (text);
				result = Widgets.HintComparer.GetBestResult (result, Widgets.HintComparer.Compare (itemText, userText));
			}

			return result;
		}


		private readonly BusinessContext		businessContext;

		private System.Func<T>					valueGetter;

		private Widgets.AutoCompleteTextField	attachedWidget;
		private Widgets.ItemPicker				attachedPicker;
		private PickerMode						attachedPickerMode;

		private Epsitec.Common.Widgets.Collections.StringCollection widgetItems;
	}
}
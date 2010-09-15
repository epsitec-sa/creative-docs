﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Validators;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epsitec.Cresus.DataLayer.Context;

namespace Epsitec.Cresus.Core.Controllers
{
	/// <summary>
	/// The <c>BindingController</c> class is used to bind an entity with one of
	/// the item picker widgets.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class SelectionController<T> : IWidgetUpdater
		where T : AbstractEntity
	{
		public SelectionController()
		{
		}
		

		public System.Func<IEnumerable<T>> PossibleItemsGetter
		{
			get;
			set;
		}

		public System.Action<T> ValueSetter
		{
			get;
			set;
		}

		public Expression<System.Func<T>> ValueGetter
		{
			set
			{
				this.valueGetterExpression = value;
				this.valueGetter = null;
			}
		}

		public ReferenceController ReferenceController
		{
			get;
			set;
		}

		public System.Func<System.Collections.Generic.IList<T>> CollectionValueGetter
		{
			get;
			set;
		}

		public System.Func<T, string[]> ToTextArrayConverter
		{
			get;
			set;
		}

		public System.Func<T, FormattedText> ToFormattedTextConverter
		{
			get;
			set;
		}


		public T GetValue()
		{
			if (this.valueGetterExpression == null)
			{
				return null;
			}
			if (this.valueGetter == null)
			{
				this.valueGetter = this.valueGetterExpression.Compile ();
			}


			return this.valueGetter ();
		}

		public Expression<System.Func<T>> GetValueExpression()
		{
			return this.valueGetterExpression;
		}

		public void SetValue(T value)
		{
			if (this.ValueSetter == null)
			{
				throw new System.InvalidOperationException ("Cannot set value without setter");
			}
			else
			{
				this.ValueSetter (value);
			}
		}

		public void Attach(Widgets.AutoCompleteTextField widget)
		{
			this.attachedWidget = widget;
			this.widgetItems    = widget.Items;

			widget.ValueToDescriptionConverter = this.ConvertHintValueToDescription;

			widget.HintComparer = (value, text) => this.MatchUserText (value as T, text);
			widget.HintComparisonConverter = x => TextConverter.ConvertToLowerAndStripAccents (x);

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
			else if ((this.valueGetterExpression != null) &&
					 (this.ValueSetter != null))
			{
				this.AttachSingleValueSelector ();
			}

			this.Update ();
		}

		private enum PickerMode
		{
			None,
			SingleValue,
			MultipleValue,
		}

		#region IWidgetUpdater Members

		public void Update()
		{
			this.widgetItems.Clear ();
			this.widgetItems.AddRange (this.GetSortedItemList ());
			
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

	
		public static int CompareItems(T a, T b)
		{
			var ra = a as Entities.IItemRank;
			var rb = b as Entities.IItemRank;

			int valueA = ra.Rank ?? -1;
			int valueB = rb.Rank ?? -1;

			if (valueA < valueB)
            {
				return -1;
            }
			if (valueA > valueB)
			{
				return 1;
			}
			return 0;
		}
		
		private static void Sort(List<T> list)
		{
			if ((list.Count > 0) &&
				(list[0] is IItemRank))
			{
				list.Sort ((a, b) => SelectionController<T>.CompareItems (a, b));
			}
		}

		private List<T> GetSortedItemList()
		{
			List<T> list = new List<T> (this.PossibleItemsGetter ());
			SelectionController<T>.Sort (list);
			return list;
		}

		private void AttachMultipleValueSelector()
		{
			this.attachedPickerMode = PickerMode.MultipleValue;
			this.attachedPicker.MultiSelectionChanged += this.HandleMultiSelectionChanged;
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
				this.ValueSetter (this.widgetItems.GetValue<T> (this.attachedWidget.SelectedItemIndex));
			}
			else
			{
				this.ValueSetter (null);
			}
		}
		
		private void HandleMultiSelectionChanged(object sender)
		{
			var selectedItems = this.CollectionValueGetter ();

			var indexes = this.attachedPicker.GetSortedSelection ();

			using (this.SuspendNotifications (selectedItems))
			{
				selectedItems.Clear ();

				foreach (int selectedIndex in indexes)
				{
					var item = this.widgetItems.GetValue<T> (selectedIndex);
					selectedItems.Add (item);
				}
			}
		}

		private void HandleSingleSelectionChanged(object sender)
		{
			this.ValueSetter (this.widgetItems.GetValue<T> (this.attachedPicker.SelectedItemIndex));
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

			var context = DataLayer.Context.DataContextPool.Instance.FindDataContext (entity);

			if ((context != null) &&
				(context.IsRegisteredAsEmptyEntity (entity)))
			{
				return Epsitec.Cresus.Core.Controllers.DataAccessors.CollectionTemplate.DefaultDefinitionInProgressText;
			}
			
			return this.ToFormattedTextConverter (entity);
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

			foreach (var text in texts)
			{
				var itemText = TextConverter.ConvertToLowerAndStripAccents (text);
				result = Widgets.AutoCompleteTextField.Bestof (result, Widgets.AutoCompleteTextField.Compare (itemText, userText));
			}

			return result;
		}


		private Expression<System.Func<T>> valueGetterExpression;
		private System.Func<T> valueGetter;

		private Widgets.AutoCompleteTextField	attachedWidget;
		private Widgets.ItemPicker				attachedPicker;
		private PickerMode						attachedPickerMode;

		private Epsitec.Common.Widgets.Collections.StringCollection widgetItems;
	}
}
//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Validators;

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
	public class SelectionController<T>
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
			foreach (var item in this.PossibleItemsGetter ())
			{
				widget.Items.Add (item);
			}

			widget.ValueToDescriptionConverter = this.ConvertHintValueToDescription;
			
			widget.HintComparer = (value, text) => this.MatchUserText (value as T, text);
			widget.HintComparisonConverter = x => TextConverter.ConvertToLowerAndStripAccents (x);

			widget.SelectedItemIndex = widget.Items.FindIndexByValue (this.GetValue ());

			widget.AcceptingEdition +=
				delegate
				{
					if (widget.SelectedItemIndex > -1)
					{
						this.ValueSetter (widget.Items.GetValue (widget.SelectedItemIndex) as T);
					}
					else
					{
						this.ValueSetter (null);
					}
				};
		}

		static int CompareItems(T a, T b)
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
		
		public void Attach(Widgets.ItemPicker widget)
		{
			List<T> list = new List<T> (this.PossibleItemsGetter ());

			if (typeof (T).GetInterfaces ().Contains (typeof (Entities.IItemRank)))
			{
				list.Sort (SelectionController<T>.CompareItems);
			}

			foreach (var item in list)
			{
				widget.Items.Add (item);
			}

			widget.ValueToDescriptionConverter = this.ConvertHintValueToDescription;
			widget.CreateUI ();

			if (this.CollectionValueGetter != null)
			{
				this.AttachMultipleValueSelector (widget);
			}
			else if ((this.valueGetterExpression != null) &&
					 (this.ValueSetter != null))
			{
				this.AttachSingleValueSelector (widget);
			}
		}

		private void AttachMultipleValueSelector(Widgets.ItemPicker widget)
		{
			foreach (var item in this.CollectionValueGetter ())
			{
				int index = widget.Items.FindIndexByValue (item);

				if (index != -1)
				{
					widget.AddSelection (new int[] { index });
				}
			}

			widget.MultiSelectionChanged +=
				delegate
				{
					var selectedItems = this.CollectionValueGetter ();

					selectedItems.Clear ();
					var list = widget.GetSortedSelection ();

					foreach (int sel in list)
					{
						var item = widget.Items.GetValue (sel) as T;
						selectedItems.Add (item);
					}
				};
		}

		private void AttachSingleValueSelector(Widgets.ItemPicker widget)
		{
			var initialValue = this.GetValue ();

			int index = widget.Items.FindIndexByValue (initialValue);
			if (index != -1)
			{
				widget.AddSelection (new int[] { index });
			}

			widget.SelectedItemChanged +=
				delegate
				{
					this.ValueSetter (widget.Items.GetValue (widget.SelectedItemIndex) as T);
				};
		}
		
		private FormattedText ConvertHintValueToDescription(object value)
		{
			var entity = value as T;
			
			entity = entity.UnwrapNullEntity ();

			if ((entity == null) ||
				(this.ToFormattedTextConverter == null))
			{
				return FormattedText.Empty;
			}

			var context = DataContextPool.Instance.FindDataContext (entity);

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
	}

	public class TextValueController
	{
		public TextValueController(Marshaler marshaler)
		{
			this.marshaler = marshaler;
		}


		public void Attach(AbstractTextField widget)
		{
			this.widget = widget;
			this.UpdateWidget ();
			
			new MarshalerValidator (this.widget, this.marshaler);

			widget.AcceptingEdition +=
				delegate
				{
					string text = TextConverter.ConvertToSimpleText (widget.Text);
					this.marshaler.SetStringValue (text);
				};

			widget.KeyboardFocusChanged += (sender, e) => this.UpdateWidget ();
		}

		private void UpdateWidget()
		{
			if (this.widget != null)
			{
				this.widget.Text = TextConverter.ConvertToTaggedText (this.marshaler.GetStringValue ());
			}
		}
		
		private readonly Marshaler marshaler;
		private Widget widget;
	}
}
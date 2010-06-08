//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	/// <summary>
	/// The <c>BindingController</c> class is used to bind 
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

		public System.Func<T> ValueGetter
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
			if (this.ValueGetter == null)
			{
				return null;
			}
			else
			{
				return this.ValueGetter ();
			}
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

		public void Attach(Widgets.HintEditor widget)
		{
			foreach (var item in this.PossibleItemsGetter ())
			{
				widget.Items.Add (item);
			}

			widget.ValueToDescriptionConverter = this.ConvertHintValueToDescription;
			
			widget.HintComparer            = (value, text) => this.MatchUserText (value as T, text);
			widget.HintComparisonConverter = x => TextConverter.ConvertToLowerAndStripAccents (x);

			widget.SelectedItemIndex       = widget.Items.FindIndexByValue (this.GetValue ());
		}
		
		public void Attach(Widgets.DetailedCombo widget)
		{
			foreach (var item in this.PossibleItemsGetter ())
			{
				widget.Items.Add (item);
			}

			widget.ValueToDescriptionConverter = this.ConvertHintValueToDescription;
			widget.CreateUI ();

			if (this.CollectionValueGetter != null)
			{
				this.AttachMultipleValueSelector (widget);
			}
			else if ((this.ValueGetter != null) &&
					 (this.ValueSetter != null))
			{
				this.AttachSingleValueSelector (widget);
			}
		}

		private void AttachMultipleValueSelector(Widgets.DetailedCombo widget)
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

		private void AttachSingleValueSelector(Widgets.DetailedCombo widget)
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

			if ((entity == null) ||
				(this.ToFormattedTextConverter == null))
			{
				return FormattedText.Empty;
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

			foreach (var text in texts)
			{
				result = Widgets.HintEditor.Bestof (result, Widgets.HintEditor.Compare (TextConverter.ConvertToLowerAndStripAccents (text), userText));
			}

			return result;
		}
	}
}

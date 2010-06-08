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

		public void Attach(Widgets.HintEditor editor)
		{
			foreach (var item in this.PossibleItemsGetter ())
			{
				editor.Items.Add (item);
			}

			editor.ValueToDescriptionConverter = this.ConvertHintValueToDescription;
			
			editor.HintComparer            = (value, text) => this.MatchUserText (value as T, text);
			editor.HintComparisonConverter = x => TextConverter.ConvertToLowerAndStripAccents (x);

			editor.SelectedItemIndex       = editor.Items.FindIndexByValue (this.GetValue ());
		}
		
		public void Attach(Widgets.DetailedCombo editor)
		{
			foreach (var item in this.PossibleItemsGetter ())
			{
				editor.Items.Add (item);
			}

			editor.ValueToDescriptionConverter = this.ConvertHintValueToDescription;
			editor.CreateUI ();

			if (this.CollectionValueGetter != null)
			{
				foreach (var item in this.CollectionValueGetter ())
				{
					int index = editor.Items.FindIndexByValue (item);

					if (index != -1)
					{
						editor.AddSelection (new int[] { index });
					}
				}

				editor.MultiSelectionChanged +=
					delegate
					{
						var selectedItems = this.CollectionValueGetter ();

						selectedItems.Clear ();
						var list = editor.GetSortedSelection ();

						foreach (int sel in list)
						{
							var item = editor.Items.GetValue (sel) as T;
							selectedItems.Add (item);
						}
					};
			}
			else if ((this.ValueGetter != null) &&
					 (this.ValueSetter != null))
			{
				var initialValue = this.GetValue ();

				int index = editor.Items.FindIndexByValue (initialValue);
				if (index != -1)
				{
					editor.AddSelection (new int[] { index });
				}

				editor.SelectedItemChanged +=
					delegate
					{
						this.ValueSetter (editor.Items.GetValue (editor.SelectedItemIndex) as T);
					};

			}
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

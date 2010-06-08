//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	public class BindingController<T>
		where T : AbstractEntity
	{
		public BindingController()
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

		private string ConvertHintValueToDescription(object value)
		{
			var entity = value as T;

			if ((entity == null) ||
				(this.ToFormattedTextConverter == null))
			{
				return null;
			}
			else
			{
				return this.ToFormattedTextConverter (entity).ToSimpleText ();
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

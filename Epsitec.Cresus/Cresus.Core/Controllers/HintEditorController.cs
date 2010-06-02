//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Controllers
{
	public class HintEditorController<T>
		where T : AbstractEntity
	{
		public HintEditorController()
		{
		}

		public IEnumerable<T> Items
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

		public void Attach(Widgets.HintEditor editor)
		{
			foreach (var item in this.Items)
			{
				editor.Items.Add (item);
			}

			editor.ValueToDescriptionConverter = this.ConvertHintValueToDescription;
			
			editor.HintComparer                = this.MatchUserText;
			editor.HintComparisonConverter = Misc.RemoveAccentsToLower;

//			editor.SelectedItemIndex = editor.Items.FindIndexByValue (entity);
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

		private Widgets.HintComparerResult MatchUserText(object value, string userText)
		{
			var entity = value as T;

			if ((entity == null) ||
				(this.ToTextArrayConverter == null))
			{
				return Widgets.HintComparerResult.NoMatch;
			}

			var texts = this.ToTextArrayConverter (entity);
			var result = Widgets.HintComparerResult.NoMatch;

			foreach (var text in texts)
			{
				result = Widgets.HintEditor.Bestof (result, Widgets.HintEditor.Compare (Misc.RemoveAccentsToLower (text), userText));
			}

			return result;
		}
	}
}

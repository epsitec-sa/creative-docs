//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Validators;

using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	public class EnumValueController<T> : IWidgetUpdater
	{
		public EnumValueController(Marshaler marshaler,
								   IEnumerable<EnumKeyValues<T>> possibleItems = null,
								   ValueToFormattedTextConverter<EnumKeyValues<T>> getUserText = null)
		{
			this.marshaler     = marshaler;
			this.possibleItems = possibleItems;
			this.getUserText   = getUserText;

			if (this.possibleItems != null)
			{
				System.Diagnostics.Debug.Assert (this.getUserText != null);
			}
		}


		public void Attach(AutoCompleteTextField widget)
		{
			this.AddItems (widget);

			widget.ValueToDescriptionConverter = value => this.getUserText (value as EnumKeyValues<T>);
			widget.HintComparer = (value, text) => EnumValueController<T>.MatchUserText (value as EnumKeyValues<T>, text);
			widget.HintComparisonConverter = x => Widgets.HintComparer.GetComparableText (x);

			this.widget = widget;
			this.Update ();

			widget.EditionAccepted += delegate
			{
				int    index = widget.SelectedItemIndex;
				string key   = index < 0 ? null : widget.Items.GetKey (index);
				this.marshaler.SetStringValue (key);
			};

			//	Je ne sais pas qui a eu l'idée farfelue d'appeler Update lorsque
			//	le widget perd le focus, mais cela cause des catastrophes. La valeur
			//	éditée reprend l'ancien contenu !
			//widget.KeyboardFocusChanged += (sender, e) => this.Update ();
		}

		private void AddItems(AutoCompleteTextField widget)
		{
			foreach (var item in this.possibleItems)
			{
				string key;

				if (typeof (T).IsEnum)
				{
					key = EnumConverter<T>.ConvertToNumericString (item.Key);
				}
				else
				{
					key = item.Key.ToString ();
				}

				widget.Items.Add (key, item);
			}
		}
		
		private static HintComparerResult MatchUserText(EnumKeyValues<T> value, string userText)
		{
			if (string.IsNullOrWhiteSpace (userText))
			{
				return Widgets.HintComparerResult.NoMatch;
			}

			var result = Widgets.HintComparerResult.NoMatch;

			foreach (var text in value.Values.Select (x => x.ToSimpleText ()))
			{
				var itemText = Widgets.HintComparer.GetComparableText (text);
				result = Widgets.HintComparer.GetBestResult (result, Widgets.HintComparer.Compare (itemText, userText));
			}

			return result;
		}


		#region IWidgetUpdater Members

		public void Update()
		{
			if (this.widget != null)
			{
				this.widget.SelectedItemIndex = this.widget.Items.FindIndexByKey (this.marshaler.GetStringValue ());
			}
		}

		#endregion

		private readonly Marshaler marshaler;
		private readonly IEnumerable<EnumKeyValues<T>> possibleItems;
		private readonly ValueToFormattedTextConverter<EnumKeyValues<T>> getUserText;
		private AutoCompleteTextField widget;
	}
}

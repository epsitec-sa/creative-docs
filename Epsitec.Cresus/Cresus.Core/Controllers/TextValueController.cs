//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Validators;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Controllers
{
	public class TextValueController : IWidgetUpdater
	{
		public TextValueController(Marshaler marshaler, IEnumerable<string[]> possibleItems = null, System.Func<string[], FormattedText> getUserText = null)
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
			foreach (var item in possibleItems)
			{
				widget.Items.Add (item[0], item);
			}

			widget.ValueToDescriptionConverter = value => this.getUserText (value as string[]);
			widget.HintComparer = (value, text) => TextValueController.MatchUserText (value as string[], text);
			widget.HintComparisonConverter = x => TextConverter.ConvertToLowerAndStripAccents (x);

			this.Attach (widget as AbstractTextField);
		}

		public void Attach(AbstractTextField widget)
		{
			this.widget = widget;
			this.Update ();

			new MarshalerValidator (this.widget, this.marshaler);

			widget.AcceptingEdition +=
				delegate
				{
					if (this.widget is AutoCompleteTextField)
					{
						var auto = this.widget as AutoCompleteTextField;

						string[] texts = auto.Items.GetValue (auto.SelectedItemIndex) as string[];
						this.marshaler.SetStringValue (texts[0]);  // utilise key
					}
					else
					{
						string text = TextConverter.ConvertToSimpleText (widget.Text);
						this.marshaler.SetStringValue (text);
					}
				};

			widget.KeyboardFocusChanged += (sender, e) => this.Update ();
		}


		private static HintComparerResult MatchUserText(string[] value, string userText)
		{
			if (string.IsNullOrWhiteSpace (userText))
			{
				return Widgets.HintComparerResult.NoMatch;
			}

			var result = Widgets.HintComparerResult.NoMatch;

			foreach (var text in value)
			{
				var itemText = TextConverter.ConvertToLowerAndStripAccents (text);
				result = Widgets.AutoCompleteTextField.Bestof (result, Widgets.AutoCompleteTextField.Compare (itemText, userText));
			}

			return result;
		}


		#region IWidgetUpdater Members

		public void Update()
		{
			if (this.widget != null)
			{
				if (this.widget is AutoCompleteTextField)
				{
					var auto = this.widget as AutoCompleteTextField;

					auto.SelectedItemIndex = auto.Items.FindIndexByKey (this.marshaler.GetStringValue ());
				}
				else
				{
					this.widget.Text = TextConverter.ConvertToTaggedText (this.marshaler.GetStringValue ());
				}
			}
		}

		#endregion

		private readonly Marshaler marshaler;
		private readonly IEnumerable<string[]> possibleItems;
		private readonly System.Func<string[], FormattedText> getUserText;
		private Widget widget;
	}
}

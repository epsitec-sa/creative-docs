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

			System.Diagnostics.Debug.Assert (this.marshaler != null);
			System.Diagnostics.Debug.Assert ((this.possibleItems == null) || (this.getUserText != null));

			this.useFormattedText = this.marshaler.MarshaledType == typeof (FormattedText);
		}


		/// <summary>
		/// Gets the active language id (either the language specific to this controller, or the
		/// one defined globally in the UI settings).
		/// </summary>
		/// <value>The active language id.</value>
		public string LanguageId
		{
			get
			{
				if (this.languageId != null)
				{
					return this.languageId;
				}

				if (UI.Settings.CultureForData.HasLanguageId)
				{
					return UI.Settings.CultureForData.LanguageId;
				}

				return null;
			}
			set
			{
				this.languageId = value;
			}
		}

		
		public void Attach(AutoCompleteTextField widget)
		{
			foreach (string[] item in possibleItems)
			{
				widget.Items.Add (item[TextValueController.KeyIndex], item);
			}

			widget.ValueToDescriptionConverter = value => this.getUserText (value as string[]);
			widget.HintComparer                = (value, text) => TextValueController.MatchUserText (value as string[], text);
			widget.HintComparisonConverter     = TextConverter.ConvertToLowerAndStripAccents;

			this.Attach (widget as AbstractTextField);
		}

		public void Attach(AbstractTextField widget)
		{
			this.widget = widget;
			this.Update ();

			MarshalerValidator.CreateValidator (this.widget, this.marshaler);

			widget.EditionAccepted += this.HandleEditionAccepted;
		}

		private void HandleEditionAccepted(object sender)
		{
			string text = this.GetWidgetText ();
			this.SetMarshalerText (text);
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

		private string GetWidgetText()
		{
			if (this.widget is AutoCompleteTextField)
			{
				var autoCompleteTextField = this.widget as AutoCompleteTextField;

				string[] item = autoCompleteTextField.Items.GetValue (autoCompleteTextField.SelectedItemIndex) as string[];

				return item[TextValueController.KeyIndex];
			}
			else
			{
				return this.widget.Text;
			}
		}

		private void SetWidgetText(string text)
		{
			if (this.widget is AutoCompleteTextField)
			{
				var auto = this.widget as AutoCompleteTextField;

				auto.SelectedItemIndex = auto.Items.FindIndexByKey (text);
			}
			else
			{
				// Il ne faut absolument pas utiliser TextConverter.ConvertToTaggedText, car le texte peut
				// contenir des tags <br/>, <b>, etc. qui doivent être édités par le widget !
				this.widget.Text = text;
			}
		}

		private bool CheckIfMarshalerIsUsingFormattedText()
		{
			string value = this.marshaler.GetStringValue ();
			return marshaler.MarshaledType == typeof (FormattedText);
		}


		private string GetMarshalerText()
		{
			string value = this.marshaler.GetStringValue ();

			if (marshaler.MarshaledType == typeof (FormattedText))
			{
				FormattedText formattedText = new FormattedText (value);

				//	Handle formatted text, which could be stored as a multilingual text : the UI
				//	can only display and handle one language at any given time.
				
				if (MultilingualText.IsMultilingual (formattedText))
				{
					MultilingualText multilingual = new MultilingualText (formattedText);

					if (multilingual.ContainsLocalizations)
					{
						value = multilingual.GetTextOrDefault (this.LanguageId).ToString ();
					}
				}
			}

			return value;
		}

		private void SetMarshalerText(string text)
		{
			if (marshaler.MarshaledType == typeof (FormattedText))
			{
				var originalValue = this.marshaler.GetStringValue ();
				var originalFormattedText = new FormattedText (originalValue);

				//	Handle formatted text, which could be stored as a multilingual text : the UI
				//	can only display and handle one language at any given time.

				if ((MultilingualText.IsMultilingual (originalFormattedText)) ||
					(MultilingualText.IsDefaultLanguageId (this.LanguageId) == false))
				{
					var multilingual = new MultilingualText (originalFormattedText);

					multilingual.SetText (this.LanguageId, new FormattedText (text));
					text = multilingual.ToString ();
				}
			}
			
			this.marshaler.SetStringValue (text);
		}

		#region IWidgetUpdater Members

		public void Update()
		{
			if (this.widget != null)
			{
				AbstractTextField textField = this.widget as AbstractTextField;

				if (textField != null)
				{
					if (this.CheckIfMarshalerIsUsingFormattedText ())
					{
						textField.IsFormattedText    = true;
						textField.IsMultilingualText = true;
					}
					else
					{
						textField.IsFormattedText    = false;
						textField.IsMultilingualText = false;
					}
				}

				string text = this.GetMarshalerText ();
				this.SetWidgetText (text);
			}
		}

		#endregion


		private const int KeyIndex = 0;
		
		private readonly Marshaler marshaler;
		private readonly IEnumerable<string[]> possibleItems;
		private readonly System.Func<string[], FormattedText> getUserText;
		private readonly bool useFormattedText;
		
		private Widget widget;
		private string languageId;
	}
}

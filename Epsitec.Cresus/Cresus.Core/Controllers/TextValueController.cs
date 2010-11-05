//	Copyright � 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Validators;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Binders;
using Epsitec.Cresus.Core.Factories;

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

			widget.MultilingualEditionCalled += new Common.Support.EventHandler (this.HandleTextFieldMultilingualEditionCalled);

			var validator = MarshalerValidator.CreateValidator (this.widget, this.marshaler);

			this.SetupFieldBinder ();
			this.AttachFieldBinder (validator);
			
			this.UpdateWidgetForMultilingualText ();

			widget.EditionAccepted += this.HandleEditionAccepted;
		}

		private void SetupFieldBinder()
		{
			INamedType fieldType = this.GetFieldType ();

			if (fieldType != null)
			{
				this.fieldBinder = FieldBinderFactory.Create (fieldType);
			}
		}

		private void AttachFieldBinder(MarshalerValidator validator)
		{
			if (this.fieldBinder != null)
			{
				validator.AdditionalPredicate = this.fieldBinder.GetPredicate ();
			}
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
				// contenir des tags <br/>, <b>, etc. qui doivent �tre �dit�s par le widget !
				this.widget.Text = text;
			}
		}

		private bool CheckIfMarshalerIsUsingFormattedText()
		{
			string value = this.marshaler.GetStringValue ();
			return marshaler.MarshaledType == typeof (FormattedText);
		}


		private INamedType GetFieldType()
		{
			return EntityInfo.GetFieldType (this.marshaler.ValueGetterExpression);
		}

		private string GetMarshalerText()
		{
			string value = this.marshaler.GetStringValue ();
			MultilingualText multilingual = null;

			if (marshaler.MarshaledType == typeof (FormattedText))
			{
				FormattedText formattedText = new FormattedText (value);

				//	Handle formatted text, which could be stored as a multilingual text : the UI
				//	can only display and handle one language at any given time.
				
				if (MultilingualText.IsMultilingual (formattedText))
				{
					multilingual = new MultilingualText (formattedText);

					if (multilingual.ContainsLocalizations)
					{
						value = multilingual.GetTextOrDefault (this.LanguageId).ToString ();
					}
				}
			}

			this.UpdateWidgetAfterTextChanged (multilingual);

			return this.ConvertToUI (value);
		}

		private void SetMarshalerText(string text)
		{
			text = this.ConvertFromUI (text);
			MultilingualText multilingual = null;

			if (this.marshaler.MarshaledType == typeof (FormattedText))
			{
				var originalValue = this.marshaler.GetStringValue ();
				var originalFormattedText = new FormattedText (originalValue);

				//	Handle formatted text, which could be stored as a multilingual text : the UI
				//	can only display and handle one language at any given time.

				if ((MultilingualText.IsMultilingual (originalFormattedText)) ||
					(MultilingualText.IsDefaultLanguageId (this.LanguageId) == false))
				{
					multilingual = new MultilingualText (originalFormattedText);

					multilingual.SetText (this.LanguageId, new FormattedText (text));
					text = multilingual.ToString ();

					this.marshaler.SetStringValue (text);
				}
			}
			
			this.marshaler.SetStringValue (text);
			this.UpdateWidgetAfterTextChanged (multilingual);
		}

		private string ConvertToUI(string value)
		{
			if (this.fieldBinder != null)
			{
				return this.fieldBinder.ConvertToUI (value);
			}
			else
			{
				return value;
			}
		}

		private string ConvertFromUI(string value)
		{
			if (this.fieldBinder != null)
			{
				return this.fieldBinder.ConvertFromUI (value);
			}
			else
			{
				return value;
			}
		}


		private void HandleTextFieldMultilingualEditionCalled(object sender)
		{
			//	Appel� lorsque le commande 'MultilingualEdition' est ex�cut�e, par exemple
			//	depuis le menu contextuel de AbstractTextField.
			var textField = sender as AbstractTextField;

			string value = this.marshaler.GetStringValue ();

			if (marshaler.MarshaledType == typeof (FormattedText))
			{
				FormattedText formattedText = new FormattedText (value);
				MultilingualText multilingual = new MultilingualText (formattedText);

				var dialog = new Dialogs.MultilingualEditionDialog (textField, multilingual);
				dialog.IsModal = true;
				dialog.OpenDialog ();

				if (dialog.Result == Common.Dialogs.DialogResult.Accept)
				{
					var text = multilingual.ToString ();
					this.marshaler.SetStringValue (text);

					this.UpdateWidgetAfterTextChanged (multilingual);
				}
			}
		}


		private void UpdateWidgetForMultilingualText()
		{
			if (this.widget != null)
			{
				var textField = this.widget as AbstractTextField;

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

		private void UpdateWidgetAfterTextChanged(MultilingualText multilingualText)
		{
			if (this.widget != null)
			{
				var textField = this.widget as AbstractTextField;

				if (textField != null && textField.IsMultilingualText)
				{
					if (multilingualText == null)
					{
						textField.SetUndefinedLanguage (UI.Settings.CultureForData.HasLanguageId);
					}
					else
					{
						textField.SetUndefinedLanguage (UI.Settings.CultureForData.HasLanguageId && !multilingualText.ContainsLanguage (UI.Settings.CultureForData.LanguageId));
					}
				}
			}
		}


		#region IWidgetUpdater Members

		void IWidgetUpdater.Update()
		{
			this.UpdateWidgetForMultilingualText ();
		}

		#endregion


		private const int KeyIndex = 0;
		
		private readonly Marshaler marshaler;
		private readonly IEnumerable<string[]> possibleItems;
		private readonly System.Func<string[], FormattedText> getUserText;
		private readonly bool useFormattedText;
		
		private Widget widget;
		private string languageId;
		private IFieldBinder fieldBinder;
	}
}

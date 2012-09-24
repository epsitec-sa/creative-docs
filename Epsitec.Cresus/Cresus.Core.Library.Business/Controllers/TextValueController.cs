//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Validators;

using Epsitec.Cresus.Core.Binders;
using Epsitec.Cresus.Core.Dialogs;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	/// <summary>
	/// The <c>TextValueController</c> binds a <see cref="Marshaler"/> with a <see cref="Widget"/>
	/// and handles validation.
	/// </summary>
	public class TextValueController : IWidgetUpdater
	{
		public TextValueController(Marshaler marshaler, IEnumerable<string[]> possibleItems = null, ValueToFormattedTextConverter<string[]> getUserText = null)
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
		public string TwoLetterISOLanguageName
		{
			get
			{
				if (this.twoLetterISOLanguageName != null)
				{
					return this.twoLetterISOLanguageName;
				}

				if (Library.UI.Services.Settings.CultureForData.HasTwoLetterISOLanguageName)
				{
					return Library.UI.Services.Settings.CultureForData.TwoLetterISOLanguageName;
				}

				return null;
			}
			set
			{
				this.twoLetterISOLanguageName = value;
			}
		}

		public INamedType FieldType
		{
			get
			{
				return this.namedType;
			}
			set
			{
				this.namedType = value;
			}
		}

		
		public void Attach(AutoCompleteTextFieldEx widget)
		{
			foreach (string[] item in possibleItems)
			{
				widget.Items.Add (item[TextValueController.KeyIndex], item);
			}

			widget.ValueToDescriptionConverter = value => this.getUserText (value as string[]);
			widget.HintComparer                = (value, text) => TextValueController.MatchUserText (value as string[], text);
			widget.HintComparisonConverter     = HintComparer.GetComparableText;

			this.Attach (widget as AbstractTextField);
		}

		public void Attach(AbstractTextField widget)
		{
			this.widget = widget;

			var validator = MarshalerValidator.CreateValidator (this.widget, this.marshaler);

			this.SetupFieldBinder ();
			this.AttachFieldBinder (validator);
			
			this.UpdateWidgetForMultilingualText ();

			widget.MultilingualEditionCalled += this.HandleTextFieldMultilingualEditionCalled;
			widget.EditionAccepted           += this.HandleEditionAccepted;
		}

		public void Attach(CheckButton widget)
		{
			this.widget = widget;

			this.UpdateWidgetForMultilingualText ();

			widget.ActiveStateChanged += this.HandleButtonChanged;
		}

		private void SetupFieldBinder()
		{
			if (this.fieldBinder == null)
			{
				this.fieldBinder = FieldBinderFactory.Create (this.GetFieldType ());
			}
		}

		private void AttachFieldBinder(MarshalerValidator validator)
		{
			if (this.fieldBinder != null)
			{
				validator.Validator = this.fieldBinder.GetValidator ();
				this.fieldBinder.Attach (this.marshaler);
			}
		}

		private void HandleEditionAccepted(object sender)
		{
			FormattedText text = this.GetWidgetText ();

			this.SetMarshalerText (text);
			
			this.UpdateWidgetForMultilingualText ();
		}

		private void HandleButtonChanged(object sender)
		{
			switch (this.widget.ActiveState)
			{
				case ActiveState.Maybe:
					this.SetMarshalerText (null);
					break;

				case ActiveState.Yes:
					this.SetMarshalerText ("true");
					break;

				case ActiveState.No:
					this.SetMarshalerText ("false");
					break;
			}
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
				var itemText = HintComparer.GetComparableText (text);
				result = Widgets.HintComparer.GetBestResult (result, Widgets.HintComparer.Compare (itemText, userText));
			}

			return result;
		}

		private FormattedText GetWidgetText()
		{
			if (this.widget is AutoCompleteTextFieldEx)
			{
				var autoCompleteTextField = this.widget as AutoCompleteTextFieldEx;

				string[] item = autoCompleteTextField.Items.GetValue (autoCompleteTextField.SelectedItemIndex) as string[];

				return new FormattedText (item[TextValueController.KeyIndex]);
			}
			else
			{
				return this.widget.FormattedText;
			}
		}

		private void SetWidgetText(FormattedText text)
		{
			if (this.widget is AutoCompleteTextFieldEx)
			{
				var auto = this.widget as AutoCompleteTextFieldEx;

				auto.SelectedItemIndex = auto.Items.FindIndexByKey (text.ToString ());
			}
			else
			{
				// Il ne faut absolument pas utiliser TextConverter.ConvertToTaggedText, car le texte peut
				// contenir des tags <br/>, <b>, etc. qui doivent être édités par le widget !

				if (this.widget.FormattedText == text)
				{
				}
				else
				{
					this.widget.FormattedText = text;

					var field = this.widget as AbstractTextField;

					if (field != null)
					{
						field.SelectAll ();
					}
				}
			}
		}

		private bool CheckIfMarshalerIsUsingFormattedText()
		{
			return this.marshaler.IsMarshaledTypeFormattedText;
		}


		private INamedType GetFieldType()
		{
			return this.namedType ?? EntityInfo.GetFieldType (this.marshaler.ValueGetterExpression);
		}

		private FormattedText GetMarshalerText()
		{
			var value = this.marshaler.GetFormattedTextValue ();

			MultilingualText multilingual = null;

			if (marshaler.MarshaledType == typeof (FormattedText))
			{
				FormattedText formattedText = value;

				//	Handle formatted text, which could be stored as a multilingual text : the UI
				//	can only display and handle one language at any given time.
				
				if (MultilingualText.IsMultilingual (formattedText))
				{
					multilingual = new MultilingualText (formattedText);

					if (multilingual.ContainsLocalizations)
					{
						value = multilingual.GetTextOrDefault (this.TwoLetterISOLanguageName).ToString ();
					}
				}
			}

			this.UpdateWidgetAfterTextChanged (multilingual);

			return this.ConvertToUI (value);
		}

		private void SetMarshalerText(FormattedText text)
		{
			try
			{
				text = this.ConvertFromUI (text);
				MultilingualText multilingual = null;

				if (this.marshaler.MarshaledType == typeof (FormattedText))
				{
					var originalFormattedText    = this.marshaler.GetFormattedTextValue ();
					var twoLetterISOLanguageName = this.TwoLetterISOLanguageName;

					//	Handle formatted text, which could be stored as a multilingual text : the UI
					//	can only display and handle one language at any given time.

					multilingual = new MultilingualText (originalFormattedText);

					if (Library.UI.Services.Settings.CultureForData.IsDefaultLanguage (twoLetterISOLanguageName))
					{
						multilingual.SetDefaultText (text);
						multilingual.ClearText (twoLetterISOLanguageName);
					}
					else
					{
						multilingual.SetText (twoLetterISOLanguageName, text);
					}

					if (multilingual.ContainsLocalizations)
					{
						text = multilingual.ToString ();
					}
					else
					{
						multilingual = null;
					}
				}

				this.marshaler.SetFormattedTextValue (text);
				this.UpdateWidgetAfterTextChanged (multilingual);
			}
			catch
			{
				//	TODO: we should notify the user that the value was rejected by the system
				//	Dismiss the error...
			}
		}

		private FormattedText ConvertToUI(FormattedText value)
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

		private FormattedText ConvertFromUI(FormattedText value)
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
			//	Appelé lorsque le commande 'MultilingualEdition' est exécutée, par exemple
			//	depuis le menu contextuel de AbstractTextField.
			var textField = sender as AbstractTextField;

			var value = this.marshaler.GetFormattedTextValue ();

			if (marshaler.MarshaledType == typeof (FormattedText))
			{
				FormattedText formattedText = value;
				MultilingualText multilingual = new MultilingualText (formattedText);

				var dialog = new MultilingualEditionDialog (textField, multilingual);
				dialog.IsModal = true;
				dialog.OpenDialog ();

				if (dialog.Result == Common.Dialogs.DialogResult.Accept)
				{
					var text = multilingual.ToFormattedText ();
					this.marshaler.SetFormattedTextValue (text);

					this.UpdateWidgetAfterTextChanged (multilingual);
				}
			}
		}


		private void UpdateWidgetForMultilingualText()
		{
			if (this.widget != null)
			{
				var textField = this.widget as AbstractTextField;
				var button    = this.widget as CheckButton;

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

				FormattedText text = this.GetMarshalerText ();
				
				if (button != null)
				{
					switch (text.ToSimpleText ().ToLowerInvariant ())
					{
						case "true":
							button.ActiveState = ActiveState.Yes;
							break;

						case "false":
							button.ActiveState = ActiveState.No;
							break;

						default:
							button.ActiveState = ActiveState.Maybe;
							break;
					}

					return;
				}

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
						textField.SetUndefinedLanguage (Library.UI.Services.Settings.CultureForData.HasTwoLetterISOLanguageName &&
														!Library.UI.Services.Settings.CultureForData.IsDefaultLanguageActive);
					}
					else
					{
						bool undefined = false;

						if (Library.UI.Services.Settings.CultureForData.HasTwoLetterISOLanguageName)
						{
							if (Library.UI.Services.Settings.CultureForData.IsDefaultLanguageActive)
							{
								undefined = !multilingualText.ContainsLanguage (Library.UI.Services.Settings.CultureForData.TwoLetterISOLanguageName)
										 && !multilingualText.ContainsLanguage (MultilingualText.DefaultTwoLetterISOLanguageToken);
							}
							else
							{
								undefined = !multilingualText.ContainsLanguage (Library.UI.Services.Settings.CultureForData.TwoLetterISOLanguageName);
							}
						}

						textField.SetUndefinedLanguage (undefined);
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


		private const int						KeyIndex = 0;
		
		private readonly Marshaler				marshaler;
		private readonly IEnumerable<string[]>	possibleItems;
		private readonly ValueToFormattedTextConverter<string[]> getUserText;
		private readonly bool					useFormattedText;
		
		private Widget							widget;
		private string							twoLetterISOLanguageName;
		private IFieldBinder					fieldBinder;
		private INamedType						namedType;
	}
}

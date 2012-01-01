//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Library;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.ComptabilitéControllers
{
	public static class UIBuilder
	{
		#region AutoCompleteTextField pour choisir un compte
		public static void CreateAutoCompleteTextField(Widget parent, IEnumerable<ComptabilitéCompteEntity> comptes, Marshaler marshaler, out Widget container, out AbstractTextField field)
		{
			//	Crée un widget permettant de saisir un numéro de compte.
			container = new FrameBox
			{
				Parent  = parent,
				Dock    = DockStyle.Left,
				Margins = new Margins (0, 1, 0, 0),
			};

			var textField = new Widgets.AutoCompleteTextField
			{
				Parent          = container,
				MenuButtonWidth = Library.UI.Constants.ComboButtonWidth-1,
				PreferredHeight = 20,
				Dock            = DockStyle.Fill,
				Margins         = new Margins (0, 0, 0, 0),
				HintEditorMode  = Widgets.HintEditorMode.DisplayMenu,
				TabIndex        = 1,
			};

			var menuButton = new GlyphButton
			{
				Parent          = container,
				ButtonStyle     = Common.Widgets.ButtonStyle.Combo,
				GlyphShape      = GlyphShape.Menu,
				PreferredWidth  = Library.UI.Constants.ComboButtonWidth,
				PreferredHeight = 20,
				Dock            = DockStyle.Right,
				Margins         = new Margins (-1, 0, 0, 0),
				AutoFocus       = false,
			};

			foreach (var compte in comptes)
			{
				textField.Items.Add (compte);
			}

			textField.ValueToDescriptionConverter = value => UIBuilder.GetCompteText (value as ComptabilitéCompteEntity);
			textField.HintComparer                = (value, text) => UIBuilder.MatchCompteText (value as ComptabilitéCompteEntity, text);
			textField.HintComparisonConverter     = x => HintComparer.GetComparableText (x);

			UIBuilder.InitializeCompte (textField, marshaler);

			textField.EditionAccepted += delegate
			{
				UIBuilder.UpdateCompte (textField, marshaler);
			};

			menuButton.Clicked += delegate
			{
				textField.SelectAll ();
				textField.Focus ();
				textField.OpenComboMenu ();
			};

			field = textField;
		}

		private static FormattedText GetCompteText(ComptabilitéCompteEntity compte)
		{
			//	Retourne le texte complet à utiliser pour un compte donné.
			return TextFormatter.FormatText (compte.Numéro, compte.Titre);
		}

		private static HintComparerResult MatchCompteText(ComptabilitéCompteEntity compte, string userText)
		{
			//	Compare un compte avec le texte partiel entré par l'utilisateur.
			if (string.IsNullOrWhiteSpace (userText))
			{
				return Widgets.HintComparerResult.NoMatch;
			}

			var itemText = HintComparer.GetComparableText (UIBuilder.GetCompteText (compte).ToSimpleText ());
			return HintComparer.Compare (itemText, userText);
		}

		private static void InitializeCompte(Widgets.AutoCompleteTextField textField, Marshaler marshaler)
		{
			//	Initialise le texte complet du widget, en fonction du numéro de compte stocké dans le champ de l'entité.
			FormattedText value = marshaler.GetStringValue ();

			foreach (var item in textField.Items)
			{
				var compte = item as ComptabilitéCompteEntity;

				if (compte.Numéro == value)
				{
					value = UIBuilder.GetCompteText (compte);
					break;
				}
			}

			textField.FormattedText = value;
		}

		private static void UpdateCompte(Widgets.AutoCompleteTextField textField, Marshaler marshaler)
		{
			//	Met à jour le champ de l'entité (numéro de compte seul), en fonction de l'état du widget.
			if (string.IsNullOrEmpty (textField.Text) || textField.SelectedItemIndex == -1)
			{
				marshaler.SetStringValue ("");
			}
			else
			{
				var compte = textField.Items.GetValue (textField.SelectedItemIndex) as ComptabilitéCompteEntity;
				marshaler.SetStringValue (compte.Numéro.ToSimpleText ());
			}
		}
		#endregion


		public static void CreateAutoCompleteTextField<T>(Widget parent, Marshaler marshaler, IEnumerable<EnumKeyValues<T>> possibleItems, out Widget container, out AbstractTextField field)
		{
			//	possibleItems.Item1 est la 'key' !
			container = new FrameBox
			{
				Parent  = parent,
				Dock    = DockStyle.Left,
				Margins = new Margins (0, 1, 0, 0),
			};

			var textField = new Widgets.AutoCompleteTextField
			{
				Parent          = container,
				MenuButtonWidth = Library.UI.Constants.ComboButtonWidth-1,
				PreferredHeight = 20,
				Dock            = DockStyle.Fill,
				Margins         = new Margins (0, 0, 0, 0),
				HintEditorMode  = Widgets.HintEditorMode.DisplayMenu,
				TabIndex        = 1,
			};

			var menuButton = new GlyphButton
			{
				Parent          = container,
				ButtonStyle     = Common.Widgets.ButtonStyle.Combo,
				GlyphShape      = GlyphShape.Menu,
				PreferredWidth  = Library.UI.Constants.ComboButtonWidth,
				PreferredHeight = 20,
				Dock            = DockStyle.Right,
				Margins         = new Margins (-1, 0, 0, 0),
				AutoFocus       = false,
			};

			menuButton.Clicked += delegate
			{
				textField.OpenComboMenu ();
			};

			var valueController = new EnumValueController<T> (marshaler, possibleItems, x => TextFormatter.FormatText (x));
			valueController.Attach (textField);

			field = textField;
		}


		public static void CreateInfoCompte(FrameBox box)
		{
			box.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;

			new StaticText
			{
				Parent           = box,
				ContentAlignment = ContentAlignment.MiddleCenter,
				TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				PreferredHeight  = 19,
				Dock             = DockStyle.Top,
				Margins          = new Margins (5, 5, 0, 0),
			};

			new Separator
			{
				Parent           = box,
				IsHorizontalLine = true,
				PreferredHeight  = 1,
				Dock             = DockStyle.Top,
			};

			new StaticText
			{
				Parent           = box,
				ContentAlignment = ContentAlignment.MiddleRight,
				TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				PreferredHeight  = 19,
				Dock             = DockStyle.Top,
				Margins          = new Margins (5, 5, 0, 0),
			};
		}

		public static void UpdateInfoCompte(FrameBox box, FormattedText titre, decimal? solde)
		{
			var s1 = box.Children[0] as StaticText;
			s1.FormattedText = titre;

			var s2 = box.Children[2] as StaticText;
			if (solde.HasValue)
			{
				s2.FormattedText = solde.Value.ToString ("0.00");
			}
			else
			{
				s2.FormattedText = null;
			}
		}


		public static readonly FormattedText							leftIndentText = "●  ";
		public static readonly FormattedText							rightIndentText = "  ";
	}
}

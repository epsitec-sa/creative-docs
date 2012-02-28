﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Widgets;
using Epsitec.Cresus.Compta.Fields.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Helpers
{
	public static class UIBuilder
	{
		public static FrameBox CreateMiniToolbar(Widget parent, double height=0)
		{
			var toolbar = new FrameBox
			{
				Parent        = parent,
				DrawFullFrame = true,
				BackColor     = UIBuilder.MiniToolbarColor,
				Padding       = new Margins (2),
				Dock          = DockStyle.Top,
				Margins       = new Margins (0, 0, 0, -1),
			};

			if (height != 0)
			{
				toolbar.PreferredHeight = height;
			}

			return toolbar;
		}

	
		public static Button CreateButton(Widget parent, Command command, FormattedText description)
		{
			//	Crée un gros bouton contenant à gauche l'icône de la commande, suivi d'un texte libre indépendant de la commande.
			string icon = string.Format (@"<img src=""{0}"" voff=""-10"" dx=""32"" dy=""32""/>  ", command.Icon);

			return new IconButton
			{
				Parent           = parent,
				CommandObject    = command,
				FormattedText    = "  " + icon + "  " + description,
				ContentAlignment = ContentAlignment.MiddleLeft,
				TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				PreferredHeight  = 42,
				Dock             = DockStyle.Top,
			};
		}

		public static IconButton CreateButton(Widget parent, Command command, double buttonWidth, double iconWidth, bool isActivable = false)
		{
			//	Crée un bouton icône standard lié à une commande.
			if (isActivable)
			{
				return new BackIconButton
				{
					Parent              = parent,
					BackColor           = UIBuilder.SelectionColor,
					CommandObject       = command,
					PreferredIconSize   = new Size (iconWidth, iconWidth),
					PreferredSize       = new Size (buttonWidth, buttonWidth),
					Dock                = DockStyle.StackBegin,
					Name                = (command == null) ? null : command.Name,
					VerticalAlignment   = VerticalAlignment.Top,
					HorizontalAlignment = HorizontalAlignment.Center,
					AutoFocus           = false,
				};
			}
			else
			{
				return new IconButton
				{
					Parent              = parent,
					CommandObject       = command,
					PreferredIconSize   = new Size (iconWidth, iconWidth),
					PreferredSize       = new Size (buttonWidth, buttonWidth),
					Dock                = DockStyle.StackBegin,
					Name                = (command == null) ? null : command.Name,
					VerticalAlignment   = VerticalAlignment.Top,
					HorizontalAlignment = HorizontalAlignment.Center,
					AutoFocus           = false,
				};
			}
		}


		public static DateFieldController CreateDateField(AbstractController controller, Widget parent, FormattedText initialDate, FormattedText tooltip, System.Action<EditionData> validateAction, System.Action changedAction)
		{
			//	Crée un contrôleur permettant de saisir une date.
			var fieldController = new DateFieldController (controller, 0, new ColumnMapper (tooltip), null, changedAction);

			fieldController.CreateUI (parent);
			fieldController.Box.PreferredWidth = 70;
			fieldController.EditionData = new EditionData (controller, validateAction);
			fieldController.EditionData.Text = initialDate;
			fieldController.EditionDataToWidget ();
			fieldController.Validate ();

			return fieldController;
		}


		public static FrameBox CreatePseudoCombo(Widget parent, out StaticText field, out GlyphButton button)
		{
			//	Crée un widget inerte qui ressemble à un TextFieldCombo.
			var frame = new FrameBox
			{
				Parent         = parent,
				PreferredWidth = 100,
				Dock           = DockStyle.Left,
				Margins        = new Margins (1, 0, 0, 0),
			};

			var frameField = new FrameBox
			{
				Parent          = frame,
				DrawFullFrame   = true,
				BackColor       = Color.FromBrightness (0.96),
				PreferredHeight = 20,
				Dock            = DockStyle.Fill,
			};

			field = new StaticText
			{
				Parent           = frameField,
				ContentAlignment = ContentAlignment.MiddleLeft,
				TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				PreferredHeight  = 20,
				Dock             = DockStyle.Fill,
				Margins          = new Margins (3, 0, 0, 1),
			};

			button = new GlyphButton
			{
				Parent          = frame,
				GlyphShape      = GlyphShape.Menu,
				PreferredWidth  = UIBuilder.ComboButtonWidth,
				PreferredHeight = 20,
				Dock            = DockStyle.Right,
				Margins         = new Margins (-1, 0, 0, 0),
			};

			return frame;
		}

	
		public static void CreateAutoCompleteTextField(Widget parent, out FrameBox container, out AbstractTextField field)
		{
			//	Crée un widget permettant de saisir un numéro de compte.
			container = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 20,
				Dock            = DockStyle.Fill,
				TabIndex        = 1,
			};

			var textField = new AutoCompleteTextField
			{
				Parent          = container,
				MenuButtonWidth = UIBuilder.ComboButtonWidth,
				PreferredHeight = 20,
				Dock            = DockStyle.Fill,
				Margins         = new Margins (0, 0, 0, 0),
				TabIndex        = 1,
			};

			var menuButton = new GlyphButton
			{
				Parent          = container,
				ButtonStyle     = Common.Widgets.ButtonStyle.Combo,
				GlyphShape      = GlyphShape.Menu,
				PreferredWidth  = UIBuilder.ComboButtonWidth,
				PreferredHeight = 20,
				Dock            = DockStyle.Right,
				Margins         = new Margins (-1, 0, 0, 0),
				AutoFocus       = false,
			};

			menuButton.Clicked += delegate
			{
				textField.OpenComboMenu ();
			};

			field = textField;
		}

		public static void UpdateAutoCompleteTextField(AbstractTextField field, IEnumerable<ComptaCompteEntity> comptes)
		{
			var auto = field as AutoCompleteTextField;
			System.Diagnostics.Debug.Assert (auto != null);

			auto.PrimaryTexts.Clear ();
			auto.SecondaryTexts.Clear ();

			foreach (var compte in comptes)
			{
				auto.PrimaryTexts.Add (compte.Numéro.ToSimpleText ());
				auto.SecondaryTexts.Add (compte.Titre.ToSimpleText ());
			}
		}

		public static void UpdateAutoCompleteTextField(AbstractTextField field, char separator, params FormattedText[] texts)
		{
			var auto = field as AutoCompleteTextField;
			System.Diagnostics.Debug.Assert (auto != null);

			auto.PrimaryTexts.Clear ();
			auto.SecondaryTexts.Clear ();

			foreach (var text in texts)
			{
				var words = text.ToString ().Split (separator);

				if (words.Length == 2)
				{
					auto.PrimaryTexts.Add (words[0]);
					auto.SecondaryTexts.Add (words[1]);
				}
			}
		}

		public static void UpdateAutoCompleteTextField(AbstractTextField field, params FormattedText[] texts)
		{
			var auto = field as AutoCompleteTextField;
			System.Diagnostics.Debug.Assert (auto != null);

			auto.PrimaryTexts.Clear ();
			auto.SecondaryTexts.Clear ();

			foreach (var text in texts)
			{
				auto.PrimaryTexts.Add (text);
			}
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
			s2.FormattedText = Converters.MontantToString (solde);
		}


		public static string GetTextIconUri(string icon, double verticalOffset = -6, double? iconSize = null)
		{
			if (iconSize.HasValue)
			{
				return string.Format (@"<img src=""{0}"" voff=""{1}"" dx=""{2}"" dy=""{2}""/>", UIBuilder.GetResourceIconUri (icon), verticalOffset.ToString (System.Globalization.CultureInfo.InvariantCulture), iconSize.Value.ToString (System.Globalization.CultureInfo.InvariantCulture));
			}
			else
			{
				return string.Format (@"<img src=""{0}"" voff=""{1}""/>", UIBuilder.GetResourceIconUri (icon), verticalOffset.ToString (System.Globalization.CultureInfo.InvariantCulture));
			}
		}

		public static string GetCheckStateIconUri(bool state)
		{
			return UIBuilder.GetResourceIconUri (state ? "Button.CheckYes" : "Button.CheckNo");
		}

		public static string GetRadioStateIconUri(bool state)
		{
			return UIBuilder.GetResourceIconUri (state ? "Button.RadioYes" : "Button.RadioNo");
		}

		public static string GetResourceIconUri(string icon)
		{
			if (icon.Contains (':'))
			{
				return FormattedText.Escape (icon);
			}
			else
			{
				return string.Format ("manifest:Epsitec.Cresus.Compta.Images.{0}.icon", FormattedText.Escape (icon));
			}
		}


		public static readonly Color CreationBackColor		= Color.FromHexa ("e5f4ff");  // bleu pastel
		public static readonly Color ModificationBackColor	= Color.FromHexa ("fff8d5");  // orange pastel

		public static readonly Color ViewSettingsBackColor	= Color.FromHexa ("ccffcc");  // vert pastel
		public static readonly Color SearchBackColor		= Color.FromHexa ("ffffcc");  // jaune pastel
		public static readonly Color FilterBackColor		= Color.FromHexa ("ffeecc");  // orange pastel
		public static readonly Color OptionsBackColor		= Color.FromHexa ("d2f0ff");  // bleu pastel

		public static readonly Color SelectionColor			= Color.FromHexa ("ffd700");  // orange "gold"
		public static readonly Color JustCreatedColor		= Color.FromHexa ("b3d7ff");  // bleu pastel

		public static readonly Color InfoColor				= Color.FromAlphaColor (0.5, Color.FromHexa ("eeeeee"));   // gris transparent
		public static readonly Color ErrorColor				= Color.FromHexa ("ffb1b1");  // rouge pâle
		public static readonly Color MiniToolbarColor		= Color.FromHexa ("f4f9ff");  // bleu très léger

		public static readonly Color TextInsideSearchColor  = Color.FromBrightness (0);    // noir
		public static readonly Color TextOutsideSearchColor = Color.FromBrightness (0.6);  // gris
		public static readonly Color BackInsideSearchColor  = Color.FromHexa ("fff000");   // jaune pétant
		public static readonly Color BackOutsideSearchColor = Color.FromAlphaColor (0.1, Color.FromHexa ("fff000"));   // jaune très transparent

		public static readonly Color GraphicGreenColor		= Color.FromHexa ("00bb00");  // vert
		public static readonly Color GraphicRedColor		= Color.FromHexa ("ff0000");  // rouge

		public static readonly double LeftLabelWidth		= 64;
		public static readonly double ComboButtonWidth		= 14;

		public static readonly FormattedText leftIndentText  = "●  ";
		public static readonly FormattedText rightIndentText = "  ";
	}
}

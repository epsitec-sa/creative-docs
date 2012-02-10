//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Library;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Helpers
{
	public static class UIBuilder
	{
		public static DateFieldController CreateDateField(AbstractController controller, Widget parent, FormattedText initialDate, FormattedText tooltip, System.Action<EditionData> validateAction, System.Action changedAction)
		{
			//	Crée un contrôleur permettant de saisir une date.
			var fieldController = new DateFieldController (controller, 0, new ColumnMapper (tooltip), null, changedAction);

			fieldController.CreateUI (parent);
			fieldController.Box.PreferredWidth = 70;
			fieldController.EditionData = new EditionData (controller, validateAction);
			fieldController.EditionData.Text = initialDate;
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


		public static string GetTextIconUri(string icon)
		{
			return string.Format (@"<img src=""{0}""/>", UIBuilder.GetResourceIconUri (icon));
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


		public static readonly double LeftLabelWidth	= 64;
		public static readonly double ComboButtonWidth	= 14;

		public static readonly FormattedText							leftIndentText = "●  ";
		public static readonly FormattedText							rightIndentText = "  ";
	}
}

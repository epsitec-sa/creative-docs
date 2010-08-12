//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Helpers;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.ArticleParameterControllers
{
	/// <summary>
	/// Ce contrôleur gère la toolbar pour éditer les paramètres dans la désignation d'un article.
	/// </summary>
	public class ArticleParameterToolbarController
	{
		public ArticleParameterToolbarController(TileContainer tileContainer, ArticleDefinitionEntity articleDefinition)
		{
			this.tileContainer = tileContainer;
			this.articleDefinition = articleDefinition;
		}


		public void CreateUI(FrameBox parent, string label)
		{
			if (!string.IsNullOrEmpty (label))
			{
				var staticText = new StaticText
				{
					Parent = parent,
					Text = string.Concat (label, " :"),
					TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
					Dock = DockStyle.Top,
					Margins = new Margins (0, UIBuilder.RightMargin, 0, UIBuilder.MarginUnderLabel),
				};
			}

			this.toolbar = new FrameBox
			{
				Parent = parent,
				PreferredHeight = UIBuilder.TinyButtonSize,
				Margins = new Margins (0, UIBuilder.RightMargin, 0, 1),
				Dock = DockStyle.Top,
			};
		}

		public void UpdateUI(TextFieldMultiEx textField)
		{
			this.toolbar.Children.Clear ();

			foreach (var parameter in this.articleDefinition.ArticleParameterDefinitions)
			{
				var button = new Button
				{
					Parent = this.toolbar,
					ButtonStyle = Common.Widgets.ButtonStyle.Icon,
					Text = parameter.Code,
					Name = parameter.Code,
					PreferredHeight = UIBuilder.TinyButtonSize,
					Margins = new Margins (0, 1, 0, 0),
					Dock = DockStyle.Left,
				};

				button.PreferredWidth = GetButtonRequiredWidth (button);

				button.Clicked += delegate
				{
					InsertText (textField, GetTag (button.Name));
				};
			}

			this.toolbar.Visibility = this.articleDefinition.ArticleParameterDefinitions.Count != 0;
		}


		private static double GetButtonRequiredWidth(Button button)
		{
			//	Retourne la largeur requise pour un bouton, selon le texte contenu.
			//	Au minimum, il sera carré.
			var size = button.TextLayout.SingleLineSize;
			return System.Math.Max (button.PreferredHeight, (int) (size.Width+10));
		}

		private static string GetTag(string code)
		{
			//	Retourne le tag à insérer dans le texte pour un paramètre.
			return string.Format ("<param code=\"{0}\"/>", code);
		}

		private static void InsertText(TextFieldMultiEx textField, string text)
		{
			//	Insère un texte comme s'il avait été frappé oar l'utilisateur.
			textField.Selection = text;
			textField.Focus ();
		}


		private readonly TileContainer tileContainer;
		private readonly ArticleDefinitionEntity articleDefinition;

		private FrameBox toolbar;
	}
}

//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

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
	/// Chaque paramètre correspond à un bouton avec le code du paramètre.
	/// </summary>
	public class ArticleParameterToolbarController
	{
		public ArticleParameterToolbarController(TileContainer tileContainer)
		{
			this.tileContainer = tileContainer;
		}


		public FrameBox CreateUI(FrameBox parent, string label)
		{
			//	Crée l'interface, c'est-à-dire la toolbar vide.
			if (!string.IsNullOrEmpty (label))
			{
				var staticText = new StaticText
				{
					Parent = parent,
					Text = string.Concat (label, " :"),
					TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
					Dock = DockStyle.Stacked,
					Margins = new Margins (0, Library.UI.Constants.RightMargin, 0, Library.UI.Constants.MarginUnderLabel),
				};
			}

			this.toolbar = UIBuilder.CreateMiniToolbar (parent, Library.UI.Constants.TinyButtonSize);
			this.toolbar.Dock = DockStyle.Stacked;
			this.toolbar.Margins = new Margins (0, Library.UI.Constants.RightMargin, 0, -1);

			return this.toolbar;
		}

		public void UpdateUI(ArticleDefinitionEntity articleDefinition, AbstractTextField textField)
		{
			this.UpdateUI (articleDefinition, null, textField);
		}

		public void UpdateUI(ArticleDocumentItemEntity articleDocumentItem, AbstractTextField textField)
		{
			this.UpdateUI (articleDocumentItem.ArticleDefinition, articleDocumentItem, textField);
		}

		private void UpdateUI(ArticleDefinitionEntity articleDefinition, ArticleDocumentItemEntity articleDocumentItem, AbstractTextField textField)
		{
			//	Met à jour l'interface en créant les boutons pour chaque paramètre.
			//	Si articleDocumentItem == null, on ne spécifie pas les valeurs dans le texte éditable.
			this.toolbar.Children.Clear ();

			foreach (var parameter in articleDefinition.ArticleParameterDefinitions)
			{
				if (parameter.Name.IsNullOrEmpty)
				{
					continue;
				}

				var button = new Button
				{
					Parent = this.toolbar,
					ButtonStyle = Common.Widgets.ButtonStyle.Icon,
					AutoFocus = false,
					Name = parameter.Name.ToString (),
					Text = parameter.Name.ToString (),
					PreferredHeight = Library.UI.Constants.TinyButtonSize,
					Margins = new Margins (0, 1, 0, 0),
					Dock = DockStyle.Left,
				};

				button.PreferredWidth = ArticleParameterToolbarController.GetButtonRequiredWidth (button);
				ToolTip.Default.SetToolTip (button, TextFormatter.ConvertToText (parameter.Description));

				button.Clicked += delegate
				{
					ArticleParameterToolbarController.InsertText (textField, ArticleParameterToolbarController.GetTag (button.Name));
				};
			}

			//	La toolbar est invisible s'il n'y a aucun paramètre.
			this.toolbar.Visibility = articleDefinition.ArticleParameterDefinitions.Count != 0;

			ArticleParameterToolbarController.UpdateTextFieldParameter (articleDocumentItem, textField);
		}


		public static void UpdateTextFieldParameter(ArticleDocumentItemEntity articleDocumentItem, AbstractTextField textField)
		{
			//	Met à jour les paramètres dans le widget qui contient la désignation de l'article.
			//	Ainsi, les valeurs affichées pour les paramètres sont mises à jour.
			var dico = ArticleParameterHelper.GetArticleParametersValues (articleDocumentItem, useNameAsDictionaryKey: true);

			foreach (var pair in dico)
			{
				textField.TextLayout.SetParameter (pair.Key, pair.Value);
			}
		}



		private static double GetButtonRequiredWidth(Button button)
		{
			//	Retourne la largeur requise pour un bouton, selon le texte contenu.
			//	Au minimum, il sera carré.
			double width;

			if (button.TextLayout == null)
			{
				width = 0;
			}
			else
			{
				width = button.TextLayout.GetSingleLineSize ().Width;
			}

			return System.Math.Max (button.PreferredHeight, (int) (width+10));
		}

		private static FormattedText GetTag(string name)
		{
			//	Retourne le tag à insérer dans le texte pour un paramètre.
			return FormattedText.Concat (ArticleParameterHelper.startParameterTag, name, ArticleParameterHelper.endParameterTag);
		}

		private static void InsertText(AbstractTextField textField, FormattedText text)
		{
			//	Insère un texte comme s'il avait été frappé par l'utilisateur.
			textField.Focus ();  // il faut mettre le focus AVANT, à cause de la gestion des boutons v/x dans les widgets '*Ex' !
			textField.Selection = text.ToString ();
		}


		private readonly TileContainer tileContainer;

		private FrameBox toolbar;
	}
}

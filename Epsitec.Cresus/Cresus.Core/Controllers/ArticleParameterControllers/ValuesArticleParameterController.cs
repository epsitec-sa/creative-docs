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
	/// Ce contrôleur gère l'ensemble de la saisie des valeurs des articles paramétrés (donc pour tous
	/// les paramètres de l'article concerné).
	/// </summary>
	public class ValuesArticleParameterController
	{
		public ValuesArticleParameterController(TileContainer tileContainer, EditionTile editionTile)
		{
			this.tileContainer = tileContainer;
			this.editionTile = editionTile;
		}


		public void CreateUI(FrameBox parent)
		{
			this.frameBox = new FrameBox ()
			{
				Parent = parent,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 0, 0),
			};
		}

		public void UpdateUI(ArticleDocumentItemEntity article)
		{
			this.frameBox.Children.Clear ();

			for (int index = 0; index < article.ArticleDefinition.ArticleParameterDefinitions.Count; index++)
			{
				this.CreateParameterUI (this.frameBox, article, index);
			}

			// Montre ou cache la tuile parente.
			this.editionTile.Visibility = article.ArticleDefinition.ArticleParameterDefinitions.Count != 0;
		}

		private void CreateParameterUI(FrameBox parent, ArticleDocumentItemEntity article, int index)
		{
			var box = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 1),
				TabIndex = index+1,  // 1..n
			};

			var label = new StaticText
			{
				Parent = box,
				Text = article.ArticleDefinition.ArticleParameterDefinitions[index].Name,
				PreferredWidth = 80,
				Dock = DockStyle.Left,
			};

			AbstractArticleParameterDefinitionEntity parameter = article.ArticleDefinition.ArticleParameterDefinitions[index];

			if (parameter is NumericValueArticleParameterDefinitionEntity)
			{
				var controller = new NumericValueArticleParameterController (article, index);
				controller.CreateUI (box);
			}

			if (parameter is EnumValueArticleParameterDefinitionEntity)
			{
				var controller = new EnumValueArticleParameterController (article, index);
				controller.CreateUI (box);
			}
		}


		private readonly TileContainer tileContainer;
		private readonly EditionTile editionTile;

		private FrameBox frameBox;
	}
}

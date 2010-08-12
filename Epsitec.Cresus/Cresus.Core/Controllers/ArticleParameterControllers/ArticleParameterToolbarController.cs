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
		public ArticleParameterToolbarController(TileContainer tileContainer, ArticleDocumentItemEntity articleItem)
		{
			this.tileContainer = tileContainer;
			this.articleItem = articleItem;
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
				PreferredHeight = 20,
				Margins = new Margins (0, UIBuilder.RightMargin, 0, 1),
				Dock = DockStyle.Top,
			};
		}

		public void UpdateUI()
		{
			this.toolbar.Children.Clear ();

			foreach (var parameter in this.articleItem.ArticleDefinition.ArticleParameterDefinitions)
			{
				var button = new Button
				{
					Parent = this.toolbar,
					ButtonStyle = Common.Widgets.ButtonStyle.Normal,
					Text = parameter.Code,
					PreferredWidth = 50,
					PreferredHeight = 20,
					Margins = new Margins (0, 1, 0, 0),
					Dock = DockStyle.Left,
				};
			}

			this.toolbar.Visibility = this.articleItem.ArticleDefinition.ArticleParameterDefinitions.Count != 0;
		}


		private readonly TileContainer tileContainer;
		private readonly ArticleDocumentItemEntity articleItem;

		private FrameBox toolbar;
	}
}

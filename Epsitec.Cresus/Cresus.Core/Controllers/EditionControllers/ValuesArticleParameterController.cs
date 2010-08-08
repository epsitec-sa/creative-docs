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

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
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

		public void UpdateUI(ArticleDefinitionEntity article)
		{
			this.frameBox.Children.Clear ();

			int tabIndex = 0;
			foreach (var parameter in article.ArticleParameters)
			{
				this.CreateParameterUI (this.frameBox, parameter, ++tabIndex);
			}

			// Montre ou cache la tuile parente.
			this.editionTile.Visibility = article.ArticleParameters.Count != 0;
		}

		private void CreateParameterUI(FrameBox parent, AbstractArticleParameterDefinitionEntity parameter, int tabIndex)
		{
			var box = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 1),
				TabIndex = tabIndex,
			};

			var label = new StaticText
			{
				Parent = box,
				Text = parameter.Name,
				PreferredWidth = 100,
				Dock = DockStyle.Left,
			};

			var field = new AutoCompleteTextField
			{
				Parent = box,
				Dock = DockStyle.Fill,
				TabIndex = 1,
			};
		}


		private readonly TileContainer tileContainer;
		private readonly EditionTile editionTile;
		private FrameBox frameBox;
	}
}

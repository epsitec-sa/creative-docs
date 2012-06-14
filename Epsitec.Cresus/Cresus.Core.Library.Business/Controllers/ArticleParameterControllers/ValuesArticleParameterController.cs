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
		public ValuesArticleParameterController(TileContainer tileContainer, FrameBox parent)
		{
			this.tileContainer = tileContainer;
			this.parent        = parent;
		}


		public System.Action<AbstractArticleParameterDefinitionEntity> CallbackParameterChanged
		{
			//	Définition de la méthode qui sera appelée lorsqu'un paramètre sera changé.
			get;
			set;
		}


		public FrameBox CreateUI(FrameBox parent, int labelWidth = 80, bool labelToRight = false)
		{
			this.labelWidth   = labelWidth;
			this.labelToRight = labelToRight;

			this.frameBox = new FrameBox ()
			{
				Parent  = parent,
				Dock    = DockStyle.Top,
				Margins = new Margins (0, 10, 1, 0),
			};

			return this.frameBox;
		}

		public void UpdateUI(IArticleDefinitionParameters article)
		{
			this.frameBox.Children.Clear ();

			for (int index = 0; index < article.ArticleDefinition.ArticleParameterDefinitions.Count; index++)
			{
				this.CreateParameterUI (this.frameBox, article, index);
			}

			// Montre ou cache la tuile parente.
			if (this.parent != null)
			{
				this.parent.Visibility = article.ArticleDefinition.ArticleParameterDefinitions.Count != 0;
			}
		}

		private void CreateParameterUI(FrameBox parent, IArticleDefinitionParameters article, int index)
		{
			AbstractArticleParameterDefinitionEntity parameter = article.ArticleDefinition.ArticleParameterDefinitions[index];

			if (parameter.Name.IsNullOrEmpty () && parameter.Description.IsNullOrEmpty ())
			{
				return;
			}

			var box = new FrameBox
			{
				Parent   = parent,
				Dock     = DockStyle.Top,
				Margins  = new Margins (0, 0, 0, 1),
				TabIndex = index+1,  // 1..n
			};

			var label = new StaticText
			{
				Parent         = box,
				FormattedText  = parameter.Description.IsNullOrEmpty () ? parameter.Name : parameter.Description,
				PreferredWidth = this.labelWidth,
				Dock           = DockStyle.Left,
			};

			if (this.labelToRight)
			{
				label.ContentAlignment = Common.Drawing.ContentAlignment.MiddleRight;
				label.Margins = new Margins (0, 5, 0, 0);
			}

			if (parameter is NumericValueArticleParameterDefinitionEntity)
			{
				var controller = new NumericValueArticleParameterController (article, index, this.tileContainer);

				controller.CallbackParameterChanged = this.CallbackParameterChanged;
				controller.CreateUI (box);
			}

			if (parameter is EnumValueArticleParameterDefinitionEntity)
			{
				var controller = new EnumValueArticleParameterController (article, index, this.tileContainer);

				controller.CallbackParameterChanged = this.CallbackParameterChanged;
				controller.CreateUI (box);
			}

			if (parameter is OptionValueArticleParameterDefinitionEntity)
			{
				var controller = new OptionValueArticleParameterController (article, index, this.tileContainer);

				controller.CallbackParameterChanged = this.CallbackParameterChanged;
				controller.CreateUI (box);
			}

			if (parameter is FreeTextValueArticleParameterDefinitionEntity)
			{
				var controller = new FreeTextValueArticleParameterController (article, index, this.tileContainer);

				controller.CallbackParameterChanged = this.CallbackParameterChanged;
				controller.CreateUI (box);
			}
		}


		private readonly TileContainer		tileContainer;
		private readonly FrameBox			parent;

		private FrameBox					frameBox;
		private int							labelWidth;
		private bool						labelToRight;
	}
}

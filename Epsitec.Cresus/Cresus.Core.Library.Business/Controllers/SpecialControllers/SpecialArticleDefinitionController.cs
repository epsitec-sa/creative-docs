//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

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
using Epsitec.Cresus.Core.Factories;

namespace Epsitec.Cresus.Core.Controllers.SpecialControllers
{
	/// <summary>
	/// Ce contrôleur gère la définition de la description courte ou longue d'un article.
	/// </summary>
	public class SpecialArticleDefinitionController : IEntitySpecialController
	{
		public SpecialArticleDefinitionController(TileContainer tileContainer, ArticleDefinitionEntity articleEntity, int mode)
		{
			this.tileContainer = tileContainer;
			this.articleEntity = articleEntity;
			this.mode = mode;
		}


		public void CreateUI(Widget parent, UIBuilder builder, bool isReadOnly)
		{
			this.isReadOnly = isReadOnly;

			var frameBox = parent as FrameBox;
			System.Diagnostics.Debug.Assert (frameBox != null);

			if (this.mode == 0)  // description courte ?
			{
				var toolbarController = new ArticleParameterControllers.ArticleParameterToolbarController (this.tileContainer);
				toolbarController.CreateUI (frameBox, "Description courte");

				var textField = builder.CreateTextField (frameBox, 0, null, Marshaler.Create (() => this.articleEntity.Name, x => this.articleEntity.Name = x));
				toolbarController.UpdateUI (this.articleEntity, textField);
			}

			if (this.mode == 1)  // description longue ?
			{
				var toolbarController = new ArticleParameterControllers.ArticleParameterToolbarController (this.tileContainer);
				toolbarController.CreateUI (frameBox, "Description longue");

				var textField = builder.CreateTextFieldMulti (frameBox, 68, null, Marshaler.Create (() => this.articleEntity.Description, x => this.articleEntity.Description = x));
				toolbarController.UpdateUI (this.articleEntity, textField);
			}
		}


		private class Factory : DefaultEntitySpecialControllerFactory<ArticleDefinitionEntity>
		{
			protected override IEntitySpecialController Create(TileContainer container, ArticleDefinitionEntity entity, int mode)
			{
				return new SpecialArticleDefinitionController (container, entity, mode);
			}
		}

	
		private readonly TileContainer tileContainer;
		private readonly ArticleDefinitionEntity articleEntity;
		private readonly int mode;

		private bool isReadOnly;
	}
}

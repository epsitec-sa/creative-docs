//	Copyright © 2011-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types.Converters;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Widgets;

namespace Epsitec.Cresus.Core.Controllers.SpecialControllers
{
	/// <summary>
	/// Ce contrôleur gère la définition de la description courte ou longue d'un article.
	/// </summary>
	public class SpecialArticleDefinitionController : IEntitySpecialController
	{
		public SpecialArticleDefinitionController(TileContainer tileContainer, ArticleDefinitionEntity articleEntity, ViewId mode)
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

			if (this.mode.Id == 0)  // description courte ?
			{
				var toolbarController = new ArticleParameterControllers.ArticleParameterToolbarController (this.tileContainer);
				toolbarController.CreateUI (frameBox, "Description courte");

				var textField = builder.CreateTextField (frameBox, 0, false, null, Marshaler.Create (() => this.articleEntity.Name, x => this.articleEntity.Name = x));
				toolbarController.UpdateUI (this.articleEntity, textField);
			}

			if (this.mode.Id == 1)  // description longue ?
			{
				var toolbarController = new ArticleParameterControllers.ArticleParameterToolbarController (this.tileContainer);
				toolbarController.CreateUI (frameBox, "Description longue");

				var textField = builder.CreateTextFieldMulti (frameBox, 68, false, null, Marshaler.Create (() => this.articleEntity.Description, x => this.articleEntity.Description = x));
				toolbarController.UpdateUI (this.articleEntity, textField);
			}
		}


		private class Factory : DefaultEntitySpecialControllerFactory<ArticleDefinitionEntity>
		{
			protected override IEntitySpecialController Create(TileContainer container, ArticleDefinitionEntity entity, ViewId mode)
			{
				return new SpecialArticleDefinitionController (container, entity, mode);
			}
		}

	
		private readonly TileContainer tileContainer;
		private readonly ArticleDefinitionEntity articleEntity;
		private readonly ViewId mode;

		private bool isReadOnly;
	}
}

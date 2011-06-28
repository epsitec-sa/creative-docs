//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	public class ArticleDefinitionController : IEntitySpecialController
	{
		public ArticleDefinitionController(TileContainer tileContainer, ArticleDefinitionEntity articleEntity, int mode)
		{
			this.tileContainer = tileContainer;
			this.articleEntity = articleEntity;
			this.mode = mode;
		}


		public void CreateUI(Widget parent, UIBuilder builder, bool isReadOnly)
		{
			System.Diagnostics.Debug.Assert (parent is FrameBox);
			this.isReadOnly = isReadOnly;

			if (this.mode == 0)  // description courte ?
			{
				var toolbarController = new ArticleParameterControllers.ArticleParameterToolbarController (this.tileContainer);
				toolbarController.CreateUI (parent as FrameBox, "Description courte");

				var textField = builder.CreateTextField (parent as FrameBox, 0, null, Marshaler.Create (() => this.articleEntity.Name, x => this.articleEntity.Name = x));
				toolbarController.UpdateUI (this.articleEntity, textField);
			}

			if (this.mode == 1)  // description longue ?
			{
				var toolbarController = new ArticleParameterControllers.ArticleParameterToolbarController (this.tileContainer);
				toolbarController.CreateUI (parent as FrameBox, "Description longue");

				var textField = builder.CreateTextFieldMulti (parent as FrameBox, 68, null, Marshaler.Create (() => this.articleEntity.Description, x => this.articleEntity.Description = x));
				toolbarController.UpdateUI (this.articleEntity, textField);
			}
		}


		private class Factory : DefaultEntitySpecialControllerFactory<ArticleDefinitionEntity>
		{
			protected override IEntitySpecialController Create(TileContainer container, ArticleDefinitionEntity entity, int mode)
			{
				return new ArticleDefinitionController (container, entity, mode);
			}
		}

	
		private readonly TileContainer tileContainer;
		private readonly ArticleDefinitionEntity articleEntity;
		private readonly int mode;

		private bool isReadOnly;
	}
}

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
using Epsitec.Cresus.Core.Factories;

namespace Epsitec.Cresus.Core.Controllers.SpecialControllers
{
	/// <summary>
	/// Ce contrôleur permet d'éditer une tablle de prix.
	/// </summary>
	public class SpecialTableDesignerController : IEntitySpecialController
	{
		public SpecialTableDesignerController(TileContainer tileContainer, PriceCalculatorEntity priceCalculatorEntity)
		{
			this.tileContainer = tileContainer;
			this.priceCalculatorEntity = priceCalculatorEntity;
		}


		public void CreateUI(Widget parent, UIBuilder builder, bool isReadOnly)
		{
			this.isReadOnly = isReadOnly;

			var box = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
			};

			var controller = this.tileContainer.Controller as EntityViewController;
			var businessContext = controller.BusinessContext;
			var orchestrator = controller.Orchestrator;

			var articleDefinition = businessContext.GetMasterEntity<ArticleDefinitionEntity> ();
			System.Diagnostics.Debug.Assert (articleDefinition != null);

			var c = new Epsitec.Cresus.Core.TableDesigner.TableDesignerController (orchestrator, businessContext, this.priceCalculatorEntity, articleDefinition);
			c.CreateUI (box);
		}


		private class Factory : DefaultEntitySpecialControllerFactory<PriceCalculatorEntity>
		{
			protected override IEntitySpecialController Create(TileContainer container, PriceCalculatorEntity entity, int mode)
			{
				return new SpecialTableDesignerController (container, entity);
			}
		}

	
		private readonly TileContainer tileContainer;
		private readonly PriceCalculatorEntity priceCalculatorEntity;

		private bool isReadOnly;
	}
}

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
	public class SpecialDocumentCategoryController : IEntitySpecialController
	{
		public SpecialDocumentCategoryController(TileContainer tileContainer, DocumentCategoryEntity documentCategoryEntity)
		{
			this.tileContainer = tileContainer;
			this.documentCategoryEntity = documentCategoryEntity;
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

			var c = new Cresus.Core.DocumentCategoryController.DocumentCategoryController (businessContext, this.documentCategoryEntity);
			c.CreateUI (box);
		}


		private class Factory : DefaultEntitySpecialControllerFactory<DocumentCategoryEntity>
		{
			protected override IEntitySpecialController Create(TileContainer container, DocumentCategoryEntity entity, int mode)
			{
				return new SpecialDocumentCategoryController (container, entity);
			}
		}

	
		private readonly TileContainer tileContainer;
		private readonly DocumentCategoryEntity documentCategoryEntity;

		private bool isReadOnly;
	}
}

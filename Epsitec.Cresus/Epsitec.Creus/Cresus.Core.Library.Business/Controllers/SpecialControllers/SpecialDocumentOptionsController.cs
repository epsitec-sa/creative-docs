//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Business;
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
	/// Ce contrôleur permet de choisir les options d'impression à éditer, et de les éditer.
	/// </summary>
	public class SpecialDocumentOptionsController : IEntitySpecialController
	{
		public SpecialDocumentOptionsController(TileContainer tileContainer, DocumentOptionsEntity documentOptionsEntity)
		{
			this.tileContainer = tileContainer;
			this.documentOptionsEntity = documentOptionsEntity;
		}


		public void CreateUI(Widget parent, UIBuilder builder, bool isReadOnly)
		{
			this.isReadOnly = isReadOnly;

			var box = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
			};

			var controller = this.tileContainer.EntityViewController;
			var businessContext = controller.BusinessContext;

			var c = new Cresus.Core.DocumentOptionsController.DocumentOptionsController (businessContext, this.documentOptionsEntity);
			c.CreateUI (box);
		}


		private class Factory : DefaultEntitySpecialControllerFactory<DocumentOptionsEntity>
		{
			protected override IEntitySpecialController Create(TileContainer container, DocumentOptionsEntity entity, int mode)
			{
				return new SpecialDocumentOptionsController (container, entity);
			}
		}

	
		private readonly TileContainer tileContainer;
		private readonly DocumentOptionsEntity documentOptionsEntity;

		private bool isReadOnly;
	}
}

//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Dialogs;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Helpers;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Data;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SpecialControllers
{
	/// <summary>
	/// Ce contrôleur gère la définition de la description courte ou longue d'un article.
	/// </summary>
	public class BusinessDocumentLinesController : IEntitySpecialController, System.IDisposable, IWidgetUpdater
	{
		public BusinessDocumentLinesController(TileContainer tileContainer, BusinessDocumentEntity businessDocumentEntity, int mode)
		{
			this.tileContainer = tileContainer;
			this.businessDocumentEntity = businessDocumentEntity;
			this.mode = mode;
		}


		public void CreateUI(Widget parent, UIBuilder builder, bool isReadOnly)
		{
			this.isReadOnly = isReadOnly;

			var controller = this.tileContainer.Controller as EntityViewController;
			this.businessContext = controller.BusinessContext;
			this.dataContext = controller.DataContext;
			this.coreData = controller.Data;

			var documentMetadataEntity = this.businessContext.GetMasterEntity<DocumentMetadataEntity> ();
			System.Diagnostics.Debug.Assert (documentMetadataEntity != null);
	
			var frameBox = parent as FrameBox;
			System.Diagnostics.Debug.Assert (frameBox != null);

			var box = new FrameBox
			{
				Parent = parent,
				Padding = new Margins (0, 0, 10, 10),
				Dock = DockStyle.Fill,
				Margins = Widgets.Tiles.TileArrow.GetContainerPadding (Direction.Right),
			};

			var access = new BusinessDocumentControllers.AccessData ();
			access.CoreViewController     = this.tileContainer.Controller;
			access.UIBuilder              = builder;
			access.BusinessContext        = this.businessContext;
			access.DataContext            = this.dataContext;
			access.CoreData               = this.coreData;
			access.DocumentMetadataEntity = documentMetadataEntity;
			access.BusinessDocumentEntity = this.businessDocumentEntity;

			this.controller = new BusinessDocumentControllers.BusinessDocumentLinesController (access);
			this.controller.CreateUI (box);
			this.controller.UpdateUI ();
		}

		#region IDisposable Members

		void System.IDisposable.Dispose()
		{
			if (this.controller != null)
			{
				this.controller.Dispose ();
				this.controller = null;
			}
		}

		#endregion
		
		#region IWidgetUpdater Members

		void IWidgetUpdater.Update()
		{
			this.controller.UpdateLineInformations ();
			this.controller.UpdateUI ();
		}

		#endregion

		private class Factory : DefaultEntitySpecialControllerFactory<BusinessDocumentEntity>
		{
			protected override IEntitySpecialController Create(TileContainer container, BusinessDocumentEntity entity, int mode)
			{
				return new BusinessDocumentLinesController (container, entity, mode);
			}
		}

	
		private readonly TileContainer tileContainer;
		private readonly BusinessDocumentEntity businessDocumentEntity;
		private readonly int mode;

		private bool isReadOnly;
		private BusinessContext businessContext;
		private DataContext dataContext;
		private CoreData coreData;
		private BusinessDocumentControllers.BusinessDocumentLinesController controller;
	}
}

//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Dialogs;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers;
using Epsitec.Cresus.Core.Controllers.ComplexControllers;
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
	/// The <c>SpecialBusinessDocumentLinesController</c> class manages the edition of a business
	/// document's lines.
	/// </summary>
	public sealed class SpecialBusinessDocumentController : IEntitySpecialController, System.IDisposable, IWidgetUpdater
	{
		private SpecialBusinessDocumentController(TileContainer tileContainer, BusinessDocumentEntity businessDocument, int mode)
		{
			this.tileContainer    = tileContainer;
			this.businessDocument = businessDocument;
			this.mode             = mode;
			this.viewController   = this.tileContainer.Controller as EntityViewController;
			this.businessContext  = this.viewController.BusinessContext;
			this.dataContext      = this.viewController.DataContext;
			this.coreData         = this.viewController.Data;
		}

		public void CreateUI(Widget parent, UIBuilder builder, bool isReadOnly)
		{
			this.isReadOnly = isReadOnly;

			switch (this.mode)
			{
				case 0:
					this.CreateDocumentLinesUI (parent, builder);
					break;

				case 1:
					this.CreateFooterTextUI (parent, builder);
					break;
			}
		}

		private void CreateDocumentLinesUI(Widget parent, UIBuilder builder)
		{
			var documentMetadata = this.businessContext.GetMasterEntity<DocumentMetadataEntity> ();
			System.Diagnostics.Debug.Assert (documentMetadata.IsNotNull ());

			var frame = new FrameBox
			{
				Parent  = parent,
				Padding = new Margins (0, 0, 10, 10),
				Dock    = DockStyle.Fill,
				Margins = TileArrow.GetContainerPadding (Direction.Right),
			};

			this.documentLogic = new DocumentLogic (this.businessContext, documentMetadata);
			this.access        = new AccessData (this.viewController, this.documentLogic);

			this.documentLinesController = new BusinessDocumentLinesController (this.access);
			this.documentLinesController.CreateUI (frame);
			this.documentLinesController.UpdateUI ();
		}

		private void CreateFooterTextUI(Widget parent, UIBuilder builder)
		{
			var documentMetadata = this.businessContext.GetMasterEntity<DocumentMetadataEntity> ();
			System.Diagnostics.Debug.Assert (documentMetadata.IsNotNull ());

			var frame = new FrameBox
			{
				Parent  = parent,
				Padding = new Margins (0, 2, 10, 0),
				Dock    = DockStyle.Fill,
				Margins = TileArrow.GetContainerPadding (Direction.Right),
			};

			this.access = new AccessData (this.viewController, this.documentLogic);

			this.footerTextController = new FooterTextController (this.businessContext, documentMetadata, documentMetadata.BusinessDocument as BusinessDocumentEntity, builder.ReadOnly);
			this.footerTextController.CreateUI (frame);
			this.footerTextController.UpdateUI ();
		}


		#region IDisposable Members

		void System.IDisposable.Dispose()
		{
			if (this.documentLinesController != null)
			{
				this.documentLinesController.Dispose ();
				this.documentLinesController = null;
			}

			if (this.footerTextController != null)
			{
				this.footerTextController.Dispose ();
				this.footerTextController = null;
			}
		}

		#endregion
		
		#region IWidgetUpdater Members

		void IWidgetUpdater.Update()
		{
			if (this.documentLinesController != null)
			{
				this.documentLinesController.UpdateUI ();
			}

			if (this.footerTextController != null)
			{
				this.footerTextController.UpdateUI ();
			}
		}

		#endregion

		#region Factory Class

		private class Factory : DefaultEntitySpecialControllerFactory<BusinessDocumentEntity>
		{
			protected override IEntitySpecialController Create(TileContainer container, BusinessDocumentEntity entity, int mode)
			{
				return new SpecialBusinessDocumentController (container, entity, mode);
			}
		}

		#endregion


		private readonly TileContainer				tileContainer;
		private readonly BusinessDocumentEntity		businessDocument;
		private readonly int						mode;
		private readonly EntityViewController		viewController;
		private readonly BusinessContext			businessContext;
		private readonly DataContext				dataContext;
		private readonly CoreData					coreData;
		
		private bool								isReadOnly;
		private BusinessDocumentLinesController		documentLinesController;
		private FooterTextController				footerTextController;
		private DocumentLogic						documentLogic;
		private AccessData							access;
	}
}

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
	/// document's lines (mode = 0) and ship ti mail contacts (mode = 1).
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

			if (this.mode == 0)
			{
				this.CreateLinesUI (parent, builder);
			}
			else if (this.mode == 1)
			{
				this.CreateShipToMailContactUI (parent, builder);
			}
		}


		#region Lines controller
		private void CreateLinesUI(Widget parent, UIBuilder builder)
		{
			var documentMetadata = this.businessContext.GetMasterEntity<DocumentMetadataEntity> ();
			var frameBox         = parent as FrameBox;

			System.Diagnostics.Debug.Assert (documentMetadata.IsNotNull ());
			System.Diagnostics.Debug.Assert (frameBox != null);

			var frame = new FrameBox
			{
				Parent  = parent,
				Padding = new Margins (0, 0, 10, 10),
				Dock    = DockStyle.Fill,
				Margins = TileArrow.GetContainerPadding (Direction.Right),
			};

			this.documentLogic = new DocumentLogic (this.businessContext, documentMetadata);
			this.access        = new AccessData (this.viewController, this.documentLogic);

			this.controller = new BusinessDocumentLinesController (this.access);
			this.controller.CreateUI (frame);
			this.controller.UpdateUI ();
		}
		#endregion

		#region ShipToMailContact controller
		private void CreateShipToMailContactUI(Widget parent, UIBuilder builder)
		{
			this.siteMailContacts = this.SiteMailContacts;
			bool isSiteMailContact = this.IsSiteMailContact;

			var staticText = new StaticText
			{
				Parent        = parent,
				Text          =  "Adresse de livraison :",
				TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
				Dock          = DockStyle.Stacked,
				Margins       = new Margins (0, Library.UI.Constants.RightMargin, 0, Library.UI.Constants.MarginUnderLabel),
			};

			this.siteShipToMailContactButton = new CheckButton
			{
				Parent      = parent,
				Text        = "Adresse d'un chantier",
				ActiveState = isSiteMailContact ? ActiveState.Yes : ActiveState.No,
				Dock        = DockStyle.Stacked,
				Margins     = new Margins (0, Library.UI.Constants.RightMargin, 1, 2),
			};

			{
				this.shippingBaseFrame = new FrameBox
				{
					Parent  = parent,
					Dock    = DockStyle.Stacked,
					Margins = new Margins (0, 0, 0, 0),
					Visibility = !isSiteMailContact,
				};

				var controller = new SelectionController<MailContactEntity> (this.businessContext)
				{
					ValueGetter         = () => this.businessDocument.ShipToMailContact,
					ValueSetter         = x => this.businessDocument.ShipToMailContact = x,
					ReferenceController = new ReferenceController (() => this.businessDocument.ShipToMailContact, creator: this.CreateNewMailContact),
					PossibleItemsGetter = () => this.businessContext.Data.GetAllEntities<MailContactEntity> (dataContext: this.businessContext.DataContext),
				};

				builder.CreateCompactAutoCompleteTextField (this.shippingBaseFrame, null, controller);
			}

			{
				this.shippingSiteFrame = new FrameBox
				{
					Parent     = parent,
					Dock       = DockStyle.Stacked,
					Margins    = new Margins (0, 0, 0, 0),
					Visibility = isSiteMailContact,
				};

				var controller = new SelectionController<MailContactEntity> (this.businessContext)
				{
					ValueGetter         = () => this.businessDocument.ShipToMailContact,
					ValueSetter         = x => this.businessDocument.ShipToMailContact = x,
					PossibleItemsGetter = () => this.SiteMailContacts,
				};

				builder.CreateCompactAutoCompleteTextField (this.shippingSiteFrame, null, controller);
			}

			if (this.siteMailContacts == null || !this.siteMailContacts.Any ())
			{
				this.siteShipToMailContactButton.Visibility = false;
			}

			this.siteShipToMailContactButton.ActiveStateChanged += delegate
			{
				this.shippingBaseFrame.Visibility = (this.siteShipToMailContactButton.ActiveState == ActiveState.No);
				this.shippingSiteFrame.Visibility = (this.siteShipToMailContactButton.ActiveState == ActiveState.Yes);
			};
		}

		private NewEntityReference CreateNewMailContact(DataContext context)
		{
			return context.CreateEntityAndRegisterAsEmpty<MailContactEntity> ();
		}

		private bool IsSiteMailContact
		{
			get
			{
				if (siteMailContacts != null)
				{
					return this.siteMailContacts.Contains (this.businessDocument.ShipToMailContact);
				}

				return false;
			}
		}

		private IEnumerable<MailContactEntity> SiteMailContacts
		{
			get
			{
				var affair = this.businessDocument.GetAffair (this.businessContext);

				if (affair != null && affair.AssociatedSite.Person.IsNotNull ())
				{
					var person = affair.AssociatedSite.Person;
					return person.Contacts.OfType<MailContactEntity> ();
				}

				return null;
			}
		}
		#endregion


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
			if (this.controller != null)
			{
				this.controller.UpdateUI ();
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


		private readonly TileContainer			tileContainer;
		private readonly BusinessDocumentEntity	businessDocument;
		private readonly int					mode;
		private readonly EntityViewController	viewController;
		private readonly BusinessContext		businessContext;
		private readonly DataContext			dataContext;
		private readonly CoreData				coreData;
		
		private bool							isReadOnly;
		private BusinessDocumentLinesController	controller;
		private DocumentLogic					documentLogic;
		private AccessData						access;

		private IEnumerable<MailContactEntity>	siteMailContacts;
		private CheckButton						siteShipToMailContactButton;
		private FrameBox						shippingBaseFrame;
		private FrameBox						shippingSiteFrame;
	}
}

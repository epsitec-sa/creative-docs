//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Helpers;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionBusinessDocumentViewController : EditionViewController<BusinessDocumentEntity>
	{
		protected override void CreateBricks(BrickWall<BusinessDocumentEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.BillToMailContact).PickFromCollection (this.GetMailContacts (false))
				  .Field (x => x.ShipToMailContact).PickFromCollection (this.GetMailContacts (true))
				  .Field (x => x.OtherPartyRelation)

				  .Field (x => x).WithSpecialController (1)  // FooterText
				  .Title ("").Field (x => x.FooterText)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.BillingDate)
				  .Field (x => x.PriceRefDate)
				  .Field (x => x.CurrencyCode)
				  .Field (x => x.PriceGroup)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.DebtorBookAccount)
				.End ()
				;
		}

		private IEnumerable<MailContactEntity> GetMailContacts(bool includeSites)
		{
			var affair = this.Entity.GetAffair (this.BusinessContext);

			if (affair != null)
			{
				//	Cherche toutes les adresses du client de l'affaire.
				var list = affair.Customer.MainRelation.Person.Contacts.OfType<MailContactEntity> ().ToList ();

				//	Ajoute les adresses du chantier de l'affaire.
				if (includeSites && affair.AssociatedSite.IsNotNull ())
				{
					list = list.Union (affair.AssociatedSite.Person.Contacts.OfType<MailContactEntity> ()).ToList ();
				}

				return list;
			}

			return null;
		}


#if false
		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.InvoiceDocument", "Document");

				this.CreateUIBillingMail  (builder);
				this.CreateUIShippingMail (builder);
				this.CreateUIMain2        (builder);

				builder.CreateFooterEditorTile ();
			}
		}


		private void CreateUIBillingMail(UIBuilder builder)
		{
			var controller = new SelectionController<MailContactEntity> (this.BusinessContext)
			{
				ValueGetter = () => this.Entity.BillToMailContact,
				ValueSetter = x => this.Entity.BillToMailContact = x,
				ReferenceController = new ReferenceController (() => this.Entity.BillToMailContact, creator: this.CreateNewMailContact),
				PossibleItemsGetter = () => this.Data.GetAllEntities<MailContactEntity> (dataContext: this.BusinessContext.DataContext),
			};

			builder.CreateAutoCompleteTextField ("Adresse de facturation", controller);
		}

		private void CreateUIShippingMail(UIBuilder builder)
		{
			var controller = new SelectionController<MailContactEntity> (this.BusinessContext)
			{
				ValueGetter = () => this.Entity.ShipToMailContact,
				ValueSetter = x => this.Entity.ShipToMailContact = x,
				ReferenceController = new ReferenceController (() => this.Entity.ShipToMailContact, creator: this.CreateNewMailContact),
				PossibleItemsGetter = () => this.Data.GetAllEntities<MailContactEntity> (dataContext: this.BusinessContext.DataContext),
			};

			builder.CreateAutoCompleteTextField ("Adresse de livraison", controller);
		}

		private void CreateUIMain2(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateMargin (tile, horizontalSeparator: true);
//-			builder.CreateTextFieldMulti (tile, 36, "Texte <i>Concerne</i> imprimé", Marshaler.Create (() => this.Entity.DocumentTitle, x => this.Entity.DocumentTitle = x));
//-			builder.CreateTextField      (tile,  0, "Description interne",           Marshaler.Create (() => this.Entity.Description,   x => this.Entity.Description = x));

#if true
			// TODO: à supprimer un jour...
			builder.CreateMargin (tile, horizontalSeparator: false);
			builder.CreateMargin (tile, horizontalSeparator: false);
			var button = builder.CreateButton (tile, 0, "Action", "Recalculer les totaux (provisoire)");

			button.Clicked += delegate
			{
				using (var calculator = new Epsitec.Cresus.Core.Business.Finance.PriceCalculators.DocumentPriceCalculator (this.BusinessContext, this.Entity))
				{
					Epsitec.Cresus.Core.Business.Finance.PriceCalculator.UpdatePrices (calculator);
				}
				this.TileContainer.UpdateAllWidgets ();
			};
#endif
		}

	
		private NewEntityReference CreateNewMailContact(DataContext context)
		{
			var country = context.CreateEntityAndRegisterAsEmpty<MailContactEntity> ();

			return country;
		}
#endif
	}
}

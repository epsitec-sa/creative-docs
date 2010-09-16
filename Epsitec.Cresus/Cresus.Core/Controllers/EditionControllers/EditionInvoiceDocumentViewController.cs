//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Widgets;

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
	public class EditionInvoiceDocumentViewController : EditionViewController<Entities.InvoiceDocumentEntity>
	{
		public EditionInvoiceDocumentViewController(string name, Entities.InvoiceDocumentEntity entity)
			: base (name, entity)
		{
		}


		protected override EditionStatus GetEditionStatus()
		{
			if (string.IsNullOrEmpty (this.Entity.IdA) &&
				string.IsNullOrEmpty (this.Entity.IdB) &&
				string.IsNullOrEmpty (this.Entity.IdC))
			{
				return EditionStatus.Empty;
			}

			if (string.IsNullOrEmpty (this.Entity.IdA) &&
				(!string.IsNullOrEmpty (this.Entity.IdB) ||
				 !string.IsNullOrEmpty (this.Entity.IdC)))
			{
				return EditionStatus.Invalid;
			}

			return EditionStatus.Valid;
		}

	
		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.InvoiceDocument", "Document");

				this.CreateUIMain1        (builder);
				this.CreateUIBillingMail  (builder);
				this.CreateUIShippingMail (builder);
				this.CreateUIMain2        (builder);

				builder.CreateFooterEditorTile ();
			}
		}


		private void CreateUIMain1(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			FrameBox group = builder.CreateGroup (tile, "N° du document (principal, externe et interne)");
			builder.CreateTextField (group, DockStyle.Left, 74, Marshaler.Create (() => this.Entity.IdA, x => this.Entity.IdA = x));
			builder.CreateTextField (group, DockStyle.Left, 74, Marshaler.Create (() => this.Entity.IdB, x => this.Entity.IdB = x));
			builder.CreateTextField (group, DockStyle.Left, 74, Marshaler.Create (() => this.Entity.IdC, x => this.Entity.IdC = x));

			builder.CreateMargin (tile, horizontalSeparator: true);
		}

		private void CreateUIBillingMail(UIBuilder builder)
		{
			builder.CreateAutoCompleteTextField ("Adresse de facturation",
				new SelectionController<MailContactEntity> (this.BusinessContext)
				{
					ValueGetter = () => this.Entity.BillingMailContact,
					ValueSetter = x => this.Entity.BillingMailContact = x,
					ReferenceController = new ReferenceController (() => this.Entity.BillingMailContact, creator: this.CreateNewMailContact),
					PossibleItemsGetter = () => CoreProgram.Application.Data.GetMailContacts (),

					ToTextArrayConverter     = x => GetMailTexts (x),
					ToFormattedTextConverter = x => GetMailText (x),
				});
		}

		private void CreateUIShippingMail(UIBuilder builder)
		{
			builder.CreateAutoCompleteTextField ("Adresse de livraison",
				new SelectionController<MailContactEntity> (this.BusinessContext)
				{
					ValueGetter = () => this.Entity.ShippingMailContact,
					ValueSetter = x => this.Entity.ShippingMailContact = x,
					ReferenceController = new ReferenceController (() => this.Entity.ShippingMailContact, creator: this.CreateNewMailContact),
					PossibleItemsGetter = () => CoreProgram.Application.Data.GetMailContacts (),

					ToTextArrayConverter     = x => GetMailTexts (x),
					ToFormattedTextConverter = x => GetMailText (x),
				});
		}

		private void CreateUIMain2(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateMargin (tile, horizontalSeparator: true);
			builder.CreateTextFieldMulti (tile, 36, "Texte <i>Concerne</i> imprimé", Marshaler.Create (() => this.Entity.DocumentTitle, x => this.Entity.DocumentTitle = x));
			builder.CreateTextField      (tile,  0, "Description interne",           Marshaler.Create (() => this.Entity.Description,   x => this.Entity.Description = x));

#if true
			// TODO: à supprimer un jour...
			builder.CreateMargin (tile, horizontalSeparator: false);
			builder.CreateMargin (tile, horizontalSeparator: false);
			var button = builder.CreateButton (tile, 0, "Action", "Recalculer les totaux (provisoire)");

			button.Clicked += delegate
			{
				InvoiceDocumentHelper.UpdatePrices (this.Entity, this.DataContext);
				this.TileContainer.UpdateAllWidgets ();
			};
#endif
		}


	
		private NewEntityReference CreateNewMailContact(DataContext context)
		{
			var country = context.CreateEntityAndRegisterAsEmpty<MailContactEntity> ();

			return country;
		}


		private static string[] GetMailTexts(MailContactEntity x)
		{
			//	Retourne la liste des textes pour les recherches 'AutoComplete'.
			return new string[]
			{
				TextFormatter.FormatText (x.LegalPerson.Name).ToSimpleText (),
				TextFormatter.FormatText (x.NaturalPerson.Firstname).ToSimpleText (),
				TextFormatter.FormatText (x.NaturalPerson.Lastname).ToSimpleText (),
				TextFormatter.FormatText (x.Address.Street.StreetName).ToSimpleText (),
				TextFormatter.FormatText (x.Address.Location.PostalCode).ToSimpleText (),
				TextFormatter.FormatText (x.Address.Location.Name).ToSimpleText (),
			};
		}

		private static FormattedText GetMailText(MailContactEntity x)
		{
			//	Retourne le texte compact d'une ligne pour peupler le widget AutoCompleteTextField.
			return TextFormatter.FormatText (x.LegalPerson.Name, "~,",
										 string.Join (" ", x.NaturalPerson.Firstname, x.NaturalPerson.Lastname), "~,",
										 x.Address.Street.StreetName, "~,",
										 x.Address.Location.PostalCode, x.Address.Location.Name);
		}
	}
}

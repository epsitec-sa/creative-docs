//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Orchestrators.Navigation;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.DataLayer.Context;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionAffairViewController : EditionViewController<Entities.AffairEntity>
	{
		public EditionAffairViewController(string name, Entities.AffairEntity entity)
			: base (name, entity)
		{
		}


		protected override EditionStatus GetEditionStatus()
		{
			if (string.IsNullOrEmpty (this.Entity.IdA) &&
				string.IsNullOrEmpty (this.Entity.IdB) &&
				string.IsNullOrEmpty (this.Entity.IdC) &&
				string.IsNullOrEmpty (this.Entity.DefaultDebtorBookAccount) &&
				this.Entity.Events.Count == 0)
			{
				return EditionStatus.Empty;
			}

			if (string.IsNullOrEmpty (this.Entity.IdA))
			{
				return EditionStatus.Invalid;
			}

			// TODO: Comment implémenter un vraie validation ? Est-ce que le Marshaler sait faire cela ?

			return EditionStatus.Valid;
		}

		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.Affair", "Affaire");

				this.CreateUIMain (builder);
				this.CreateUIActionButtons (builder);

				builder.CreateFooterEditorTile ();

				using (var data = TileContainerController.Setup (builder))
				{
					this.CreateUICaseEvents (data);
				}
			}
		}

		protected override void AboutToCloseUI()
		{
			base.AboutToCloseUI ();

			this.CloseUIActionButtons ();
		}

		private void CreateUIActionButtons(UIBuilder builder)
		{
#if false
			var mainViewController   = this.Orchestrator.MainViewController;
			var actionViewController = mainViewController.ActionViewController;

			actionViewController.AddButton ("NewOffer", "Nouvelle offre", "Crée une nouvelle offre (vide)", this.ExecuteNewOffer);
			
				
			mainViewController.SetActionVisibility (true);
#else
			builder.CreateMargin ();
			builder.CreateButton ("NewOffer", "Nouvelle offre", "Crée une nouvelle offre (vide)", this.ExecuteNewOffer);
			builder.CreateMargin ();
#endif
		}

		private void CloseUIActionButtons()
		{
#if false
			var mainViewController   = this.Orchestrator.MainViewController;
			var actionViewController = mainViewController.ActionViewController;

			actionViewController.RemoveButton ("NewOffer");
			
			this.Orchestrator.MainViewController.SetActionVisibility (false);
#endif
		}

		private void ExecuteNewOffer()
		{
			var businessEvent = this.DataContext.CreateEntity<BusinessEventEntity> ();
			var document = this.DataContext.CreateEntity<InvoiceDocumentEntity> ();
			var now = System.DateTime.Now;

			//	TODO: définir le n° IdA plus proprement (business rule ?)
			document.IdA                   = string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0}-{1}", this.Entity.IdA, this.Entity.Events.Select (x => x.Documents.FirstOrDefault ()).Distinct ().Count () + 1);
			document.OtherPartyBillingMode = Business.Finance.BillingMode.IncludingTax;
			document.OtherPartyTaxMode     = Business.Finance.TaxMode.LiableForVat;
			document.CurrencyCode          = Business.Finance.CurrencyCode.Chf;
			document.BillingStatus         = Business.Finance.BillingStatus.None;
			document.CreationDate          = now;
			document.LastModificationDate  = now;

			var relation = this.Orchestrator.MainViewController.GetVisibleEntities ().Select (x => x as RelationEntity).Where (x => x.IsNull () == false).FirstOrDefault ();

			if (relation.DefaultAddress.IsNull () == false)
            {
				var mailContact = relation.Person.Contacts.Where (x => x is Entities.MailContactEntity).Cast<Entities.MailContactEntity> ().Where (x => x.Address == relation.DefaultAddress).FirstOrDefault ();
				
				//	TODO: sélectionner l'adresse de facturation et l'adresse de livraison selon le type d'adresse !

				document.BillingMailContact  = mailContact;
				document.ShippingMailContact = mailContact;
            }

			businessEvent.EventType = CoreProgram.Application.Data.GetCaseEventTypes ().Where (x => x.Code.Contains ("offre")).First ();
			businessEvent.Date      = now;
			businessEvent.Documents.Add (document);

			this.Entity.Events.Add (businessEvent);

			this.ReopenSubView (new TileNavigationPathElement (this.GetOfferTileName (document) + ".0"));
		}


		private void CreateUIMain(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			FrameBox group = builder.CreateGroup (tile, "N° de l'affaire (principal, externe et interne)");
			builder.CreateTextField (group, DockStyle.Left, 74, Marshaler.Create (() => this.Entity.IdA, x => this.Entity.IdA = x));
			builder.CreateTextField (group, DockStyle.Left, 74, Marshaler.Create (() => this.Entity.IdB, x => this.Entity.IdB = x));
			builder.CreateTextField (group, DockStyle.Left, 74, Marshaler.Create (() => this.Entity.IdC, x => this.Entity.IdC = x));

			builder.CreateMargin    (tile, horizontalSeparator: true);
			builder.CreateTextField (tile, 150, "Compte débiteur (comptabilité)",  Marshaler.Create (() => this.Entity.DefaultDebtorBookAccount, x => this.Entity.DefaultDebtorBookAccount = x));

			int counter = 0;

			foreach (var doc in this.GetDocumentEntities (this.Entity))
			{
				builder.CreateEditionTitleTile ("Data.Document", "Offre");
				builder.CreateSummaryTile (EditionAffairViewController.GetOfferTileName (counter++), doc, TextFormatter.FormatText ("N°", doc.IdA ?? doc.IdB ?? doc.IdC ?? "?", "créée le", doc.CreationDate));
			}
		}

		private static string GetOfferTileName(int index)
		{

			if (index < 0)
			{
				return null;
			}
			else
			{
				return string.Format (System.Globalization.CultureInfo.InstalledUICulture, "Offer.{0}", index);
			}
		}

		private string GetOfferTileName(InvoiceDocumentEntity doc)
		{
			return EditionAffairViewController.GetOfferTileName (this.GetDocumentEntities (this.Entity).IndexOf (doc));
		}

		private void CreateUICaseEvents(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "BusinessEvent",
					IconUri		 = "Data.BusinessEvent",
					Title		 = TextFormatter.FormatText ("Evénements"),
					CompactTitle = TextFormatter.FormatText ("Evénements"),
					Text		 = CollectionTemplate.DefaultEmptyText
				});

			var template = new CollectionTemplate<BusinessEventEntity> ("BusinessEvent", data.Controller, this.DataContext);

			template.DefineText        (x => TextFormatter.FormatText (GetCaseEventsSummary (x)));
			template.DefineCompactText (x => TextFormatter.FormatText (Misc.GetDateTimeShortDescription (x.Date), x.EventType.Code));

			data.Add (CollectionAccessor.Create (this.EntityGetter, x => x.Events, template));
		}

		private IList<InvoiceDocumentEntity> GetDocumentEntities(AffairEntity affair)
		{
			var documents = new HashSet<InvoiceDocumentEntity> ();

			foreach (var businessEvent in affair.Events)
			{
				documents.AddRange (businessEvent.Documents.Select (doc => doc as InvoiceDocumentEntity).Where (doc => doc != null));
			}

			return documents.OrderBy (doc => doc.CreationDate).ToArray ();
		}

		private static string GetCaseEventsSummary(BusinessEventEntity caseEventEntity)
		{
			string date = Misc.GetDateTimeShortDescription (caseEventEntity.Date);
			int count = caseEventEntity.Documents.Count;

			if (count < 2)
			{
				return string.Format ("{0} {1}", date, caseEventEntity.EventType.Code);
			}
			else
			{
				return string.Format ("{0} {1} ({2} documents)", date, caseEventEntity.EventType.Code, count);
			}
		}
	}
}

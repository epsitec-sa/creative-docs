//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

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
				string.IsNullOrEmpty (this.Entity.DefaultDebtorBookAccount))
			{
				return EditionStatus.Empty;
			}

			if (string.IsNullOrEmpty (this.Entity.IdA) &&
				(!string.IsNullOrEmpty (this.Entity.IdB) ||
				 !string.IsNullOrEmpty (this.Entity.IdC) ||
				 !string.IsNullOrEmpty (this.Entity.DefaultDebtorBookAccount)))
			{
				return EditionStatus.Invalid;
			}

			// TODO: Comment implémenter un vraie validation ? Est-ce que le Marshaler sait faire cela ?

			return EditionStatus.Valid;
		}

		protected override void UpdateEmptyEntityStatus(DataLayer.Context.DataContext context, bool isEmpty)
		{
			var entity = this.Entity;
			context.UpdateEmptyEntityStatus (entity, isEmpty);
		}

	
		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.Affair", "Affaire");

				this.CreateUIMain (builder);

				builder.CreateFooterEditorTile ();
			}
			
			//	Summary:
			using (var data = TileContainerController.Setup (this))
			{
				this.CreateUICaseEvents (data);
				this.CreateUIDocuments (data);
			}

			this.CreateUIActionButtons ();
		}

		protected override void AboutToCloseUI()
		{
			base.AboutToCloseUI ();

			this.CloseUIActionButtons ();
		}

		private void CreateUIActionButtons()
		{
			var mainViewController   = this.Orchestrator.MainViewController;
			var actionViewController = mainViewController.ActionViewController;

			actionViewController.AddButton ("NewOffer", "Nouvelle offre", "Crée une nouvelle offre (vide)", this.ExecuteNewOffer);
			
				
			mainViewController.SetActionVisibility (true);
		}

		private void CloseUIActionButtons()
		{
			var mainViewController   = this.Orchestrator.MainViewController;
			var actionViewController = mainViewController.ActionViewController;

			actionViewController.RemoveButton ("NewOffer");
			
			this.Orchestrator.MainViewController.SetActionVisibility (false);
		}

		private void ExecuteNewOffer()
		{
			var businessEvent = this.DataContext.CreateEntity<BusinessEventEntity> ();
			var document = this.DataContext.CreateEntity<InvoiceDocumentEntity> ();
			var now = System.DateTime.Now;

			document.OtherPartyBillingMode = BusinessLogic.Finance.BillingMode.IncludingTax;
			document.OtherPartyTaxMode     = BusinessLogic.Finance.TaxMode.LiableForVat;
			document.CurrencyCode          = BusinessLogic.Finance.CurrencyCode.Chf;
			document.BillingStatus         = BusinessLogic.Finance.BillingStatus.None;
			document.CreationDate          = now;
			document.LastModificationDate  = now;

			businessEvent.EventType = CoreProgram.Application.Data.GetCaseEventTypes ().Where (x => x.Code.Contains ("offre")).First ();
			businessEvent.Date      = now;
			businessEvent.Documents.Add (document);

			this.Entity.Events.Add (businessEvent);
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

		private void CreateUIDocuments(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					AutoGroup    = false,
					Name		 = "Document",
					IconUri		 = "Data.Document",
					Title		 = TextFormatter.FormatText ("Documents"),
					CompactTitle = TextFormatter.FormatText ("Documents"),
					Text		 = CollectionTemplate.DefaultEmptyText
				});

			var template = new CollectionTemplate<InvoiceDocumentEntity> ("Document", data.Controller, this.DataContext);

			template.DefineText (x => TextFormatter.FormatText (GetDocumentSummary (x)));
			template.DefineCompactText (x => TextFormatter.FormatText (GetDocumentSummary (x)));

			data.Add (CollectionAccessor.Create (this.EntityGetter, x => this.GetDocumentEntities (x), template));
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

		private static string GetDocumentSummary(InvoiceDocumentEntity x)
		{
			return "Offre";
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

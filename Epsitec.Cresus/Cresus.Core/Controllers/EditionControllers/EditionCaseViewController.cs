//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionCaseViewController : EditionViewController<Entities.CaseEntity>
	{
		public EditionCaseViewController(string name, Entities.CaseEntity entity)
			: base (name, entity)
		{
		}


		protected override EditionStatus GetEditionStatus()
		{
			if (string.IsNullOrEmpty (this.Entity.Id) &&
				string.IsNullOrEmpty (this.Entity.External) &&
				string.IsNullOrEmpty (this.Entity.Internal) &&
				string.IsNullOrEmpty (this.Entity.DefaultDebtorBookAccount))
			{
				return EditionStatus.Empty;
			}

			if (string.IsNullOrEmpty (this.Entity.Id) &&
				(!string.IsNullOrEmpty (this.Entity.External) ||
				 !string.IsNullOrEmpty (this.Entity.Internal) ||
				 !string.IsNullOrEmpty (this.Entity.DefaultDebtorBookAccount)))
			{
				return EditionStatus.Invalid;
			}

			// TODO: Comment implémenter un vraie validation ? Est-ce que le Marshaler sait faire cela ?

			return EditionStatus.Valid;
		}

		protected override void UpdateEmptyEntityStatus(DataLayer.DataContext context, bool isEmpty)
		{
			var entity = this.Entity;
			context.UpdateEmptyEntityStatus (entity, isEmpty);
		}


		protected override void CreateUI(TileContainer container)
		{
			using (var builder = new UIBuilder (container, this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.Case", "Cas");

				this.CreateUIMain                (builder);
				this.CreateUISalesRepresentative (builder);
				this.CreateUIOwner               (builder);

				builder.CreateFooterEditorTile ();
			}

			//	Summary:
			var containerController = new TileContainerController (this, container);
			var data = containerController.DataItems;

			this.CreateUICaseEvents (data);

			containerController.GenerateTiles ();
		}


		private void CreateUIMain(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField (tile, 150, "Numéro du cas",  Marshaler.Create (() => this.Entity.Id,       x => this.Entity.Id = x));
			builder.CreateTextField (tile, 150, "Numéro externe", Marshaler.Create (() => this.Entity.External, x => this.Entity.External = x));
			builder.CreateTextField (tile, 150, "Numéro interne", Marshaler.Create (() => this.Entity.Internal, x => this.Entity.Internal = x));
			builder.CreateMargin    (tile, horizontalSeparator: true);
			builder.CreateTextField (tile, 150, "Numéro de compte à débiter",  Marshaler.Create (() => this.Entity.DefaultDebtorBookAccount, x => this.Entity.DefaultDebtorBookAccount = x));
			builder.CreateMargin    (tile, horizontalSeparator: true);
		}

		private void CreateUISalesRepresentative(UIBuilder builder)
		{
			var textField = builder.CreateAutoCompleteTextField ("Représentant (personne physique)",
				new SelectionController<NaturalPersonEntity>
				{
					ValueGetter         = () => this.Entity.SalesRepresentative,
					ValueSetter         = x => this.Entity.SalesRepresentative = x.WrapNullEntity (),
					ReferenceController = this.GetSalesRepresentativeReferenceController (),
					PossibleItemsGetter = () => CoreProgram.Application.Data.GetNaturalPersons (),

					ToTextArrayConverter     = x => new string[] { x.Firstname, x.Lastname },
					ToFormattedTextConverter = x => UIBuilder.FormatText (x.Firstname, x.Lastname),
				});
		}

		private void CreateUIOwner(UIBuilder builder)
		{
			var textField = builder.CreateAutoCompleteTextField ("Propriétaire (personne physique)",
				new SelectionController<NaturalPersonEntity>
				{
					ValueGetter         = () => this.Entity.Owner,
					ValueSetter         = x => this.Entity.Owner = x.WrapNullEntity (),
					ReferenceController = this.GetOwnerReferenceController (),
					PossibleItemsGetter = () => CoreProgram.Application.Data.GetNaturalPersons (),

					ToTextArrayConverter     = x => new string[] { x.Firstname, x.Lastname },
					ToFormattedTextConverter = x => UIBuilder.FormatText (x.Firstname, x.Lastname),
				});
		}

		private ReferenceController GetSalesRepresentativeReferenceController()
		{
			return ReferenceController.Create (
				this.EntityGetter,
				entity => entity.SalesRepresentative,
				entity => CoreProgram.Application.Data.GetCustomers (entity.SalesRepresentative).FirstOrDefault (),
				creator: this.CreateNewNaturalPerson);
		}

		private ReferenceController GetOwnerReferenceController()
		{
			return ReferenceController.Create (
				this.EntityGetter,
				entity => entity.Owner,
				entity => CoreProgram.Application.Data.GetCustomers (entity.Owner).FirstOrDefault (),
				creator: this.CreateNewNaturalPerson);
		}

		private NewEntityReference CreateNewNaturalPerson(DataContext context)
		{
			var customer = context.CreateRegisteredEmptyEntity<RelationEntity> ();
			var person   = context.CreateRegisteredEmptyEntity<NaturalPersonEntity> ();

			customer.Person = person;
			customer.FirstContactDate = Date.Today;

			return new NewEntityReference (person, customer);
		}


		private void CreateUICaseEvents(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "CaseEvent",
					IconUri		 = "Data.CaseEvent",
					Title		 = UIBuilder.FormatText ("Evénements"),
					CompactTitle = UIBuilder.FormatText ("Evénements"),
					Text		 = CollectionTemplate.DefaultEmptyText
				});

			var template = new CollectionTemplate<CaseEventEntity> ("CaseEvent", data.Controller)
				.DefineText        (x => UIBuilder.FormatText (GetCaseEventsSummary (x)))
				.DefineCompactText (x => UIBuilder.FormatText (Misc.GetDateTimeShortDescription (x.Date), x.EventType.Code));

			data.Add (CollectionAccessor.Create (this.EntityGetter, x => x.Events, template));
		}

		private static string GetCaseEventsSummary(CaseEventEntity caseEventEntity)
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

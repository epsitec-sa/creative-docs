//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

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
				builder.CreateEditionTitleTile ("Data.Affair", "Affaire");

				this.CreateUIMain (builder);

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

			builder.CreateTextField (tile, 150, "Numéro de l'affaire", Marshaler.Create (() => this.Entity.IdA, x => this.Entity.IdA = x));
			builder.CreateTextField (tile, 150, "Numéro externe",      Marshaler.Create (() => this.Entity.IdB, x => this.Entity.IdB = x));
			builder.CreateTextField (tile, 150, "Numéro interne",      Marshaler.Create (() => this.Entity.IdC, x => this.Entity.IdC = x));
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
					Title		 = UIBuilder.FormatText ("Evénements"),
					CompactTitle = UIBuilder.FormatText ("Evénements"),
					Text		 = CollectionTemplate.DefaultEmptyText
				});

			var template = new CollectionTemplate<AbstractBusinessEventEntity> ("BusinessEvent", data.Controller)
				.DefineText (x => UIBuilder.FormatText (GetCaseEventsSummary (x)))
				.DefineCompactText (x => UIBuilder.FormatText (Misc.GetDateTimeShortDescription (x.Date), x.EventType.Code));

			data.Add (CollectionAccessor.Create (this.EntityGetter, x => x.Events, template));
		}
		
		private static string GetCaseEventsSummary(AbstractBusinessEventEntity caseEventEntity)
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

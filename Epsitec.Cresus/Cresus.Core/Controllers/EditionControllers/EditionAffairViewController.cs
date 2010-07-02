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
				builder.CreateEditionTitleTile ("Data.Affair", "Affaire");

				this.CreateUIMain (builder);

				builder.CreateFooterEditorTile ();
			}

			//	Summary:
			var containerController = new TileContainerController (this, container);
			var data = containerController.DataItems;

			this.CreateUICases (data);

			containerController.GenerateTiles ();
		}


		private void CreateUIMain(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField (tile, 150, "Numéro de l'affaire", Marshaler.Create (() => this.Entity.Id,       x => this.Entity.Id = x));
			builder.CreateTextField (tile, 150, "Numéro externe",      Marshaler.Create (() => this.Entity.External, x => this.Entity.External = x));
			builder.CreateTextField (tile, 150, "Numéro interne",      Marshaler.Create (() => this.Entity.Internal, x => this.Entity.Internal = x));
			builder.CreateMargin    (tile, horizontalSeparator: true);
			builder.CreateTextField (tile, 150, "Numéro de compte à débiter",  Marshaler.Create (() => this.Entity.DefaultDebtorBookAccount, x => this.Entity.DefaultDebtorBookAccount = x));
		}

		private void CreateUICases(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "Case",
					IconUri		 = "Data.Case",
					Title		 = UIBuilder.FormatText ("Cas"),
					CompactTitle = UIBuilder.FormatText ("Cas"),
					Text		 = CollectionTemplate.DefaultEmptyText
				});

			var template = new CollectionTemplate<CaseEntity> ("Case", data.Controller)
				.DefineText        (x => UIBuilder.FormatText (GetCasesSummary (x)))
				.DefineCompactText (x => UIBuilder.FormatText (x.Id));

			data.Add (CollectionAccessor.Create (this.EntityGetter, x => x.Cases, template));
		}

		private static string GetCasesSummary(CaseEntity caseEntity)
		{
			int count = caseEntity.Events.Count;

			if (count == 0)
			{
				return caseEntity.Id;
			}
			else
			{
				string date = Misc.GetDateTimeShortDescription (caseEntity.Events[0].Date);  // date du premier événement
				return string.Format ("{0} {1} ({2} événement{3})", date, caseEntity.Id, count, count>1?"s":"");
			}
		}
	}
}

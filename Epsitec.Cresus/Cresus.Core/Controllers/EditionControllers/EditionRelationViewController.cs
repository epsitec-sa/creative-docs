//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionRelationViewController : EditionViewController<RelationEntity>
	{
		public EditionRelationViewController(string name, Entities.RelationEntity entity)
			: base (name, entity)
		{
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			if (this.personController != null)
			{
				yield return this.personController;
			}
		}

		protected override void CreateUI(TileContainer container)
		{
			using (var builder = new UIBuilder (container, this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.Customer", "Client");

				this.CreateUIMain (builder);

				this.personController = EntityViewController.CreateEntityViewController (this.Name + "Person", this.Entity.Person, ViewControllerMode.Edition, this.Orchestrator);
				this.personController.DataContext = this.DataContext;
				this.personController.CreateUI (container);

				builder.CreateFooterEditorTile ();
			}
		}

		protected override EditionStatus GetEditionStatus()
		{
			var entity = this.Entity;
			return entity.IsEmpty () ? EditionStatus.Empty : EditionStatus.Valid;
		}

		protected override void UpdateEmptyEntityStatus(DataLayer.DataContext context, bool isEmpty)
		{
			var entity = this.Entity;
			context.UpdateEmptyEntityStatus (entity, isEmpty);
		}

		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField (tile, 150, "Numéro de client",           Marshaler.Create (this.Entity, x => x.Id,                       (x, v) => x.Id = v));
			builder.CreateTextField (tile, 150, "Numéro externe",             Marshaler.Create (this.Entity, x => x.External,                 (x, v) => x.External = v));
			builder.CreateTextField (tile, 150, "Numéro interne",             Marshaler.Create (this.Entity, x => x.Internal,                 (x, v) => x.Internal = v));
			builder.CreateMargin    (tile, horizontalSeparator: false);
			builder.CreateTextField (tile,  90, "Client depuis le",           Marshaler.Create (this.Entity, x => x.FirstContactDate,         (x, v) => x.FirstContactDate = v));
			builder.CreateMargin    (tile, horizontalSeparator: true);
			builder.CreateTextField (tile, 120, "Numéro de TVA",              Marshaler.Create (this.Entity, x => x.VatNumber,                (x, v) => x.VatNumber = v));
			builder.CreateTextField (tile,  90, "Mode de TVA",                Marshaler.Create (this.Entity, x => x.VatCalculationMode,       (x, v) => x.VatCalculationMode = v));
			builder.CreateTextField (tile,  90, "Numéro de compte à débiter", Marshaler.Create (this.Entity, x => x.DefaultDebtorBookAccount, (x, v) => x.DefaultDebtorBookAccount = v));
			builder.CreateTextField (tile,  60, "Monnaie standard",           Marshaler.Create (this.Entity, x => x.DefaultCurrencyCode,      (x, v) => x.DefaultCurrencyCode = v));
		}


		private EntityViewController personController;
	}
}

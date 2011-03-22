//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Library;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionRelationViewController : EditionViewController<RelationEntity>
	{
		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield return this.personController;
		}

		protected override void CreateSubControllers()
		{
			this.personController = EntityViewControllerFactory.Create (this.Name + "Person", this.Entity.Person, ViewControllerMode.Edition, this.Orchestrator);
		}

		protected override void CreateBricks(Bricks.BrickWall<RelationEntity> wall)
		{
			wall.AddBrick ()
				.Name ("Customer")
				.Icon ("Data.Customer")
				.Title ("Client")
				.Input ()
				  .Field (x => x.FirstContactDate).Width (90)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.VatNumber)
				  .Field (x => x.TaxMode)
				  .Field (x => x.DefaultDebtorBookAccount)
				  .Field (x => x.DefaultCurrencyCode)
				.End ();
		}

#if false
		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.Customer", "Client");

				this.CreateUIMain (builder);

				this.personController = EntityViewControllerFactory.Create (this.Name + "Person", this.Entity.Person, ViewControllerMode.Edition, this.Orchestrator);
				this.personController.CreateUI (this.TileContainer);

				builder.CreateFooterEditorTile ();
			}
		}
#endif


		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateMargin                (tile, horizontalSeparator: false);
			builder.CreateTextField             (tile,  90,                              "Client depuis le",                         Marshaler.Create (() => this.Entity.FirstContactDate,         x => this.Entity.FirstContactDate = x));
			builder.CreateMargin                (tile, horizontalSeparator: true);
			builder.CreateTextField             (tile, 150,                              "Numéro de TVA",                            Marshaler.Create (() => this.Entity.VatNumber,                x => this.Entity.VatNumber = x));
			builder.CreateAutoCompleteTextField (tile, 150-UIBuilder.ComboButtonWidth+1, "Mode d'assujetissement à la TVA",          Marshaler.Create (() => this.Entity.TaxMode,                  x => this.Entity.TaxMode = x), EnumKeyValues.FromEnum<TaxMode> (), x => TextFormatter.FormatText (x));
			builder.CreateAccountEditor         (tile,                                   "Compte débiteur pour la comptabilisation", Marshaler.Create (() => this.Entity.DefaultDebtorBookAccount, x => this.Entity.DefaultDebtorBookAccount = x));
			builder.CreateAutoCompleteTextField (tile, 150-UIBuilder.ComboButtonWidth+1, "Monnaie utilisée",                         Marshaler.Create (() => this.Entity.DefaultCurrencyCode,      x => this.Entity.DefaultCurrencyCode = x), EnumKeyValues.FromEnum<CurrencyCode> (), x => TextFormatter.FormatText (x));
			
			
			
			builder.CreateAutoCompleteTextField (tile,
				150-UIBuilder.ComboButtonWidth+1,
				"Monnaie utilisée",
				Marshaler.Create (() => this.Entity.DefaultCurrencyCode, x => this.Entity.DefaultCurrencyCode = x),
				EnumKeyValues.FromEnum<CurrencyCode> (),
				x => TextFormatter.FormatText (x));
		}

		private EntityViewController personController;
	}
}

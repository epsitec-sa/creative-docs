//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Widgets;

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
			this.InitializeDefaultValues ();
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			if (this.personController != null)
			{
				yield return this.personController;
			}
		}

		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.Customer", "Client");

				this.CreateUIMain (builder);

				this.personController = EntityViewController.CreateEntityViewController (this.Name + "Person", this.Entity.Person, ViewControllerMode.Edition, this.Orchestrator);
				this.personController.DataContext = this.DataContext;
				this.personController.CreateUI (this.TileContainer);

				builder.CreateFooterEditorTile ();
			}
		}

		protected override EditionStatus GetEditionStatus()
		{
			var entity = this.Entity;
			return entity.IsEmpty () ? EditionStatus.Empty : EditionStatus.Valid;
		}

		protected override void UpdateEmptyEntityStatus(DataLayer.Context.DataContext context, bool isEmpty)
		{
			var entity = this.Entity;
			context.UpdateEmptyEntityStatus (entity, isEmpty);
		}


		private void InitializeDefaultValues()
		{
			if (!this.Entity.TaxMode.HasValue)
			{
				this.Entity.TaxMode = BusinessLogic.Finance.TaxMode.None;
			}

			if (!this.Entity.DefaultCurrencyCode.HasValue)
			{
				this.Entity.DefaultCurrencyCode = BusinessLogic.Finance.CurrencyCode.Chf;
			}
		}


		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			FrameBox group = builder.CreateGroup (tile, "N° de client (principal, externe et interne)");
			builder.CreateTextField (group, DockStyle.Left, 74, Marshaler.Create (this.Entity, x => x.IdA, (x, v) => x.IdA = v));
			builder.CreateTextField (group, DockStyle.Left, 74, Marshaler.Create (this.Entity, x => x.IdB, (x, v) => x.IdB = v));
			builder.CreateTextField (group, DockStyle.Left, 74, Marshaler.Create (this.Entity, x => x.IdC, (x, v) => x.IdC = v));

			builder.CreateMargin                (tile, horizontalSeparator: false);
			builder.CreateTextField             (tile,  90, "Client depuis le",                Marshaler.Create (this.Entity, x => x.FirstContactDate,         (x, v) => x.FirstContactDate = v));
			builder.CreateMargin                (tile, horizontalSeparator: true);
			builder.CreateTextField             (tile, 150, "Numéro de TVA",                   Marshaler.Create (this.Entity, x => x.VatNumber,                (x, v) => x.VatNumber = v));
			builder.CreateAutoCompleteTextField (tile, 137, "Mode d'assujetissement à la TVA", Marshaler.Create (this.Entity, x => x.TaxMode,                  (x, v) => x.TaxMode = v),                   BusinessLogic.Enumerations.GetAllPossibleTaxModes (),  x => TextFormatter.FormatText (x.Values[0]));
			builder.CreateTextField             (tile, 150, "Compte débiteur (comptabilité)",  Marshaler.Create (this.Entity, x => x.DefaultDebtorBookAccount, (x, v) => x.DefaultDebtorBookAccount = v));
			builder.CreateAutoCompleteTextField (tile, 137, "Monnaie utilisée",                Marshaler.Create (this.Entity, x => x.DefaultCurrencyCode,      (x, v) => x.DefaultCurrencyCode = v),       BusinessLogic.Enumerations.GetAllPossibleCurrencyCodes (), x => TextFormatter.FormatText (x.Values[0], "-", x.Values[1]));
		}


		private EntityViewController personController;
	}
}

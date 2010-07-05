//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
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
			this.InitializeDefaultValues ();
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


		private void InitializeDefaultValues()
		{
			if (string.IsNullOrWhiteSpace (this.Entity.TaxMode))
			{
//-				this.Entity.TaxMode = "1";
			}

			if (string.IsNullOrWhiteSpace (this.Entity.DefaultCurrencyCode))
			{
				this.Entity.DefaultCurrencyCode = "CHF";
			}
		}


		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField             (tile, 150, "Numéro de client",           Marshaler.Create (this.Entity, x => x.Id,                       (x, v) => x.Id = v));
			builder.CreateTextField             (tile, 150, "Numéro externe",             Marshaler.Create (this.Entity, x => x.External,                 (x, v) => x.External = v));
			builder.CreateTextField             (tile, 150, "Numéro interne",             Marshaler.Create (this.Entity, x => x.Internal,                 (x, v) => x.Internal = v));
			builder.CreateMargin                (tile, horizontalSeparator: false);
			builder.CreateTextField             (tile,  90, "Client depuis le",           Marshaler.Create (this.Entity, x => x.FirstContactDate,         (x, v) => x.FirstContactDate = v));
			builder.CreateMargin                (tile, horizontalSeparator: true);
			builder.CreateTextField             (tile, 150, "Numéro de TVA",                   Marshaler.Create (this.Entity, x => x.VatNumber,           (x, v) => x.VatNumber = v));
			builder.CreateAutoCompleteTextField (tile, 137, "Mode d'assujetissement à la TVA", Marshaler.Create (this.Entity, x => x.TaxMode,             (x, v) => x.TaxMode = v), this.GetAllPossibleItemsTaxModes (), this.GetUserTextTaxMode);
			builder.CreateTextField             (tile, 150, "Compte débiteur (comptabilité)",  Marshaler.Create (this.Entity, x => x.DefaultDebtorBookAccount, (x, v) => x.DefaultDebtorBookAccount = v));
			builder.CreateAutoCompleteTextField (tile, 137, "Monnaie utilisée",                Marshaler.Create (this.Entity, x => x.DefaultCurrencyCode, (x, v) => x.DefaultCurrencyCode = v), this.GetGetAllPossibleItemsDefaultCurrencyCodes (), this.GetUserTextDefaultCurrencyCode);
		}

		private IEnumerable<string[]> GetAllPossibleItemsTaxModes()
		{
			//	possibleItems[0] doit obligatoirement être la 'key' !
			var list = new List<string[]> ()
			{
				new string[] { "1", "Assujetti à la TVA" },
				new string[] { "2", "Non-assujetti à la TVA" },
				new string[] { "3", "Exonéré" },
			};

			return list;
		}

		private FormattedText GetUserTextTaxMode(string[] value)
		{
			return UIBuilder.FormatText (value[1]);  // par exemple "Assujetti à la TVA"
		}


		private IEnumerable<string[]> GetGetAllPossibleItemsDefaultCurrencyCodes()
		{
			//	possibleItems[0] doit obligatoirement être la 'key' !
			var list = new List<string[]>
			{
				new string[] { "CHF", "Franc suisse" },
				new string[] { "EUR", "Euro" },
				new string[] { "USD", "Dollar américain" },
				new string[] { "GBP", "Livre anglaise" },
				new string[] { "JPY", "Yen japonais" },
				new string[] { "CNY", "Yuan chinois" }
			};
			
			return list;
		}

		private FormattedText GetUserTextDefaultCurrencyCode(string[] value)
		{
			return UIBuilder.FormatText (value[0], "-", value[1]);  // par exemple "CHF - Franc suisse"
		}


		private EntityViewController personController;
	}
}

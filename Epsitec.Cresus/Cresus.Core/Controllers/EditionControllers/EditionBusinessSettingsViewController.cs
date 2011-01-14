﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionBusinessSettingsViewController : EditionViewController<BusinessSettingsEntity>
	{
		public EditionBusinessSettingsViewController(string name, BusinessSettingsEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.BusinessSettings", "Réglages de l'entreprise");

				this.CreateUIRelation  (builder);
				this.CreateUITax       (builder);
				this.CreateUISeparator (builder);
				this.CreateUILogo      (builder);
				this.CreateUISeparator (builder);
				this.CreateUICharts    (builder);

				builder.CreateFooterEditorTile ();
			}
		}

		private void CreateUIRelation(UIBuilder builder)
		{
			var controller = new SelectionController<RelationEntity> (this.BusinessContext)
			{
				ValueGetter         = () => this.Entity.Company,
				ValueSetter         = x => this.Entity.Company = x,
				ReferenceController = new ReferenceController (() => this.Entity.Company),

				ToTextArrayConverter     = x => x.GetEntityKeywords (),
				ToFormattedTextConverter = x => x.GetCompactSummary ()
			};

			builder.CreateAutoCompleteTextField ("Entreprise", controller);
		}

		private void CreateUITax(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateMargin                (tile, horizontalSeparator: true);
			builder.CreateTextField             (tile, 150,                              "Numéro de TVA",                   Marshaler.Create (() => this.Entity.Tax.VatNumber, x => this.Entity.Tax.VatNumber = x));
			builder.CreateAutoCompleteTextField (tile, 150-UIBuilder.ComboButtonWidth+1, "Mode d'assujetissement à la TVA", Marshaler.Create (() => this.Entity.Tax.TaxMode, x => this.Entity.Tax.TaxMode = x), Business.Enumerations.GetAllPossibleTaxModes (), x => TextFormatter.FormatText (x.Values[0]));
		}

		private void CreateUISeparator(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateMargin (tile, horizontalSeparator: true);
		}

		private void CreateUILogo(UIBuilder builder)
		{
			var controller = new SelectionController<ImageEntity> (this.BusinessContext)
			{
				ValueGetter         = () => this.Entity.CompanyLogo,
				ValueSetter         = x => this.Entity.CompanyLogo = x,
				ReferenceController = new ReferenceController (() => this.Entity.CompanyLogo),

				ToTextArrayConverter     = x => x.GetEntityKeywords (),
				ToFormattedTextConverter = x => x.GetCompactSummary ()
			};

			builder.CreateAutoCompleteTextField ("Logo pour les documents imprimés", controller);
		}

		private void CreateUICharts(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			this.chartsOfAccountsController = new ChartsOfAccountsControllers.ChartsOfAccountsController (this.BusinessContext, this.Entity.Finance);
			this.chartsOfAccountsController.CreateUI (tile.Container);
		}


		private ChartsOfAccountsControllers.ChartsOfAccountsController chartsOfAccountsController;
	}
}

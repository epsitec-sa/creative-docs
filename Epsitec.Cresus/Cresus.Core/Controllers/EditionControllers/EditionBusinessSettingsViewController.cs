//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

				this.CreateUILegalPerson (builder);
				this.CreateUITax (builder);

				builder.CreateFooterEditorTile ();
			}
		}

		protected override EditionStatus GetEditionStatus()
		{
			return EditionStatus.Valid;
		}

		private void CreateUITax(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateMargin                (tile, horizontalSeparator: true);
			builder.CreateTextField             (tile, 150,                              "Numéro de TVA",                   Marshaler.Create (() => this.Entity.TaxSettings.VatNumber, x => this.Entity.TaxSettings.VatNumber = x));
			builder.CreateAutoCompleteTextField (tile, 150-UIBuilder.ComboButtonWidth+1, "Mode d'assujetissement à la TVA", Marshaler.Create (() => this.Entity.TaxSettings.TaxMode, x => this.Entity.TaxSettings.TaxMode = x), Business.Enumerations.GetAllPossibleTaxModes (), x => TextFormatter.FormatText (x.Values[0]));
		}

		private void CreateUILegalPerson(UIBuilder builder)
		{
			// TODO: Pourquoi on n'affiche aucune entreprise ?
			var controller = new SelectionController<LegalPersonEntity> (this.BusinessContext)
			{
				ValueGetter         = () => this.Entity.LegalPerson,
				ValueSetter         = x => this.Entity.LegalPerson = x,
				ReferenceController = new ReferenceController (() => this.Entity.LegalPerson, creator: this.CreateNewLegalPerson),
				PossibleItemsGetter = () => this.Data.GetAllEntities<LegalPersonEntity> (),

				ToTextArrayConverter     = x => new string[] { TextFormatter.FormatText (x.Name).ToSimpleText () },
				ToFormattedTextConverter = x => TextFormatter.FormatText (x.Name)
			};

			builder.CreateAutoCompleteTextField ("Personne morale", controller);
		}

		private NewEntityReference CreateNewLegalPerson(DataContext context)
		{
			var person = context.CreateEntityAndRegisterAsEmpty<LegalPersonEntity> ();

			return person;
		}
	}
}

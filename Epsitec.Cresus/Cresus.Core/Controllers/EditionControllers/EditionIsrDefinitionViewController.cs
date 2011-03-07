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
using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Library;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionIsrDefinitionViewController : EditionViewController<Entities.IsrDefinitionEntity>
	{
		public EditionIsrDefinitionViewController(string name, Entities.IsrDefinitionEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.IsrDefinition", "Contrat BVR");

				this.CreateUIMain (builder);

				builder.CreateFooterEditorTile ();
			}
		}


		private void CreateUIMain(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField             (tile, 150,                              "Numéro d'adhérent",  Marshaler.Create (() => this.Entity.SubscriberNumber, x => this.Entity.SubscriberNumber = x));
			builder.CreateTextFieldMulti        (tile,  52,                              "Adresse d'adhérent", Marshaler.Create (() => this.Entity.SubscriberAddress, x => this.Entity.SubscriberAddress = x));
			builder.CreateAutoCompleteTextField (tile, 150-UIBuilder.ComboButtonWidth+1, "Monnaie",            Marshaler.Create (() => this.Entity.Currency, x => this.Entity.Currency = x), EnumKeyValues.FromEnum<CurrencyCode> (), x => TextFormatter.FormatText (x));

			builder.CreateMargin (tile, horizontalSeparator: true);

			builder.CreateTextField (tile, 150, "Préfixe du numéro de référence bancaire", Marshaler.Create (() => this.Entity.BankReferenceNumberPrefix, x => this.Entity.BankReferenceNumberPrefix = x));
			builder.CreateTextField (tile,   0, "Adresse de la banque, première ligne",    Marshaler.Create (() => this.Entity.BankAddressLine1, x => this.Entity.BankAddressLine1 = x));
			builder.CreateTextField (tile,   0, "Adresse de la banque, deuxième ligne",    Marshaler.Create (() => this.Entity.BankAddressLine2, x => this.Entity.BankAddressLine2 = x));
			builder.CreateTextField (tile, 150, "Numéro de compte bancaire",               Marshaler.Create (() => this.Entity.BankAccount, x => this.Entity.BankAccount = x));

			builder.CreateMargin (tile, horizontalSeparator: true);

			builder.CreateAccountEditor (tile, "Compte entrant pour la comptabilisation", Marshaler.Create (() => this.Entity.IncomingBookAccount, x => this.Entity.IncomingBookAccount = x));
		}
	}
}

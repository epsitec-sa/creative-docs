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
				builder.CreateEditionTitleTile ("Data.IsrDefinition", "Compte BVR");

				this.CreateUIMain (builder);

				builder.CreateFooterEditorTile ();
			}
		}


		private void CreateUIMain(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField      (tile,  0, "Numéro d'adhérent", Marshaler.Create (() => this.Entity.SubscriberNumber, x => this.Entity.SubscriberNumber = x));
			builder.CreateTextFieldMulti (tile, 52, "Adresse d'adhérent", Marshaler.Create (() => this.Entity.SubscriberAddress, x => this.Entity.SubscriberAddress = x));

			builder.CreateMargin (tile, horizontalSeparator: true);

			builder.CreateTextField (tile, 0, "Numéro de référence bancaire", Marshaler.Create (() => this.Entity.BankReferenceNumberPrefix, x => this.Entity.BankReferenceNumberPrefix = x));
			builder.CreateTextField (tile, 0, "Adresse de la banque, première ligne", Marshaler.Create (() => this.Entity.BankAddressLine1, x => this.Entity.BankAddressLine1 = x));
			builder.CreateTextField (tile, 0, "Adresse de la banque, deuxième ligne", Marshaler.Create (() => this.Entity.BankAddressLine2, x => this.Entity.BankAddressLine2 = x));
			builder.CreateTextField (tile, 0, "Numéro de compte", Marshaler.Create (() => this.Entity.BankAccount, x => this.Entity.BankAccount = x));
		}


		protected override EditionStatus GetEditionStatus()
		{
			if (string.IsNullOrWhiteSpace (this.Entity.SubscriberNumber))
			{
				return EditionStatus.Empty;
			}

			return EditionStatus.Valid;
		}
	}
}

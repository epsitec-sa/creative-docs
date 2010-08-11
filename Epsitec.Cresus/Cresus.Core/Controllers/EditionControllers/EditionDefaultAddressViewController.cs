//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionDefaultAddressViewController : EditionViewController<Entities.AddressEntity>
	{
		public EditionDefaultAddressViewController(string name, Entities.AddressEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI(TileContainer container)
		{
			using (var builder = new UIBuilder (container, this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.Mail", "Adresse par défaut du client");

				this.CreateUIMargin (builder);
				this.CreateUICountry (builder);
				this.CreateUIMain (builder);
				this.CreateUILocation (builder);

				builder.CreateFooterEditorTile ();
			}
		}


		private void CreateUIMargin(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateMargin (tile, horizontalSeparator: true);
		}

		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateMargin         (tile, horizontalSeparator: true);
			builder.CreateTextField      (tile,  0, "Rue",                     Marshaler.Create (() => this.Entity.Street.StreetName, x => this.Entity.Street.StreetName = x));
			builder.CreateTextFieldMulti (tile, 52, "Complément de l'adresse", Marshaler.Create (() => this.Entity.Street.Complement, x => this.Entity.Street.Complement = x));
			builder.CreateTextField      (tile,  0, "Boîte postale",           Marshaler.Create (() => this.Entity.PostBox.Number,    x => this.Entity.PostBox.Number = x));
			builder.CreateMargin         (tile, horizontalSeparator: true);
		}

		private void CreateUICountry(UIBuilder builder)
		{
			builder.CreateAutoCompleteTextField ("Nom et code du pays",
				new SelectionController<Entities.CountryEntity>
				{
					ValueGetter = () => this.Entity.Location.Country,
					ValueSetter = x => this.Entity.Location.Country = x.WrapNullEntity (),
					PossibleItemsGetter = () => CoreProgram.Application.Data.GetCountries (),

					ToTextArrayConverter     = x => new string[] { x.Code, x.Name },
					ToFormattedTextConverter = x => TextFormater.FormatText (x.Name, "(", x.Code, ")"),
				});
		}

		private void CreateUILocation(UIBuilder builder)
		{
			builder.CreateAutoCompleteTextField ("Numéro postal et ville",
				new SelectionController<Entities.LocationEntity>
				{
					ValueGetter = () => this.Entity.Location,
					ValueSetter = x => this.Entity.Location = x.WrapNullEntity (),
					PossibleItemsGetter = () => CoreProgram.Application.Data.GetLocations (),

					ToTextArrayConverter     = x => new string[] { x.PostalCode, x.Name },
					ToFormattedTextConverter = x => TextFormater.FormatText (x.PostalCode, x.Name),
				});
		}
	}
}

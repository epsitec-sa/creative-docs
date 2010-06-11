//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionMailContactViewController : EditionViewController<Entities.MailContactEntity>
	{
		public EditionMailContactViewController(string name, Entities.MailContactEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI(TileContainer container)
		{
			var builder = new UIBuilder (container, this);

			builder.CreateHeaderEditorTile ();
			builder.CreateEditionGroupingTile ("Data.Mail", "Adresse");

			this.CreateUIRoles (builder);
			this.CreateUIMargin (builder);
			this.CreateUICountry (builder);
			this.CreateUIMain (builder);
			this.CreateUILocation (builder);

			builder.CreateFooterEditorTile ();

			UI.SetInitialFocus (container);
		}


		private void CreateUIRoles(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var controller = new SelectionController<Entities.ContactRoleEntity>
			{
				CollectionValueGetter    = () => this.Entity.Roles,
				PossibleItemsGetter      = () => CoreProgram.Application.Data.GetRoles (),
				ToFormattedTextConverter = x => UIBuilder.FormatText (x.Name)
			};

			builder.CreateEditionDetailedCheck (0, "Choix du ou des rôles souhaités", controller);
		}

		private void CreateUIMargin(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateMargin (tile, true);
		}

		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateMargin         (tile, true);
			builder.CreateTextField      (tile,  0, "Rue",                     Marshaler.Create (() => this.Entity.Address.Street.StreetName, x => this.Entity.Address.Street.StreetName = x));
			builder.CreateTextFieldMulti (tile, 52, "Complément de l'adresse", Marshaler.Create (() => this.Entity.Address.Street.Complement, x => this.Entity.Address.Street.Complement = x));
			builder.CreateTextField      (tile,  0, "Boîte postale",           Marshaler.Create (() => this.Entity.Address.PostBox.Number,    x => this.Entity.Address.PostBox.Number = x));
			builder.CreateMargin         (tile, true);
		}

		private void CreateUICountry(UIBuilder builder)
		{
			builder.CreateAutoCompleteTextField ("Nom et code du pays",
				new SelectionController<Entities.CountryEntity>
				{
					ValueGetter = () => this.Entity.Address.Location.Country,
					ValueSetter = x => this.Entity.Address.Location.Country = x,
					PossibleItemsGetter = () => CoreProgram.Application.Data.GetCountries (),

					ToTextArrayConverter     = x => new string[] { x.Code, x.Name },
					ToFormattedTextConverter = x => UIBuilder.FormatText (x.Name, "(", x.Code, ")"),
				});
		}

		private void CreateUILocation(UIBuilder builder)
		{
			builder.CreateAutoCompleteTextField ("Numéro postal et ville",
				new SelectionController<Entities.LocationEntity>
				{
					ValueGetter = () => this.Entity.Address.Location,
					ValueSetter = x => this.Entity.Address.Location = x,
					PossibleItemsGetter = () => CoreProgram.Application.Data.GetLocations (),

					ToTextArrayConverter     = x => new string[] { x.PostalCode, x.Name },
					ToFormattedTextConverter = x => UIBuilder.FormatText (x.PostalCode, x.Name),
				});
		}
	}
}

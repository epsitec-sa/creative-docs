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
			using (var builder = new UIBuilder (container, this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.Mail", "Adresse");

				this.CreateUIRoles (builder);

				//?this.CreateTabBook (container, builder);

				this.CreateUILegalPerson (builder);
				this.CreateUIMargin      (builder);
				this.CreateUICountry     (builder);
				this.CreateUIMain        (builder);
				this.CreateUILocation    (builder);

				builder.CreateFooterEditorTile ();
			}
		}


		private void CreateUIRoles(UIBuilder builder)
		{
			var controller = new SelectionController<Entities.ContactRoleEntity>
			{
				CollectionValueGetter    = () => this.Entity.Roles,
				PossibleItemsGetter      = () => CoreProgram.Application.Data.GetRoles (),
				ToFormattedTextConverter = x => UIBuilder.FormatText (x.Name)
			};

			builder.CreateEditionDetailedCheck (0, "Choix du ou des rôles souhaités", controller);
		}

		private void CreateTabBook(TileContainer container, UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();
			var tabBook = builder.CreateTabBook (tile);

			var page1 = new Common.Widgets.TabPage ()
			{
				TabTitle = "Adresse spécifique",
				Name = "Local",
			};

			var page2 = new Common.Widgets.TabPage ()
			{
				TabTitle = "Adresse existante",
				Name = "Global",
			};

			tabBook.Items.Add (page1);
			tabBook.Items.Add (page2);
			tabBook.ActivePage = page1;
		}

		private void CreateUILegalPerson(UIBuilder builder)
		{
			builder.CreateAutoCompleteTextField ("Entreprise",
				new SelectionController<Entities.LegalPersonEntity>
				{
					ValueGetter = () => this.Entity.LegalPerson,
					ValueSetter = x => this.Entity.LegalPerson = x,
					PossibleItemsGetter = () => CoreProgram.Application.Data.GetLegalPersons (),

					ToTextArrayConverter     = x => new string[] { x.Name },
					ToFormattedTextConverter = x => UIBuilder.FormatText (x.Name),
				});
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

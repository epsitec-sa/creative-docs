//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionMailContactViewController : EntityViewController<Entities.MailContactEntity>
	{
		public EditionMailContactViewController(string name, Entities.MailContactEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI(TileContainer container)
		{
#if false
			UIBuilder builder = new UIBuilder (container, this);
			TitleTile group;

			var mailAccessor = new Accessors.MailContactAccessor (null, this.Entity, false);
			var locationAccessor = new Accessors.LocationAccessor (null, this.Entity.Address.Location, false);
			var countryAccessor = new Accessors.CountryAccessor (null, this.Entity.Address.Location.Country, false);

			//	Crée les tuiles.
			builder.CreateHeaderEditorTile ();

			//	Crée le contenu de la tuile d'édition.
			group = builder.CreateSummaryGroupingTile ("Data.Roles", "Rôles");

			var roleAccessor = new Accessors.RolesContactAccessor (null, mailAccessor.Entity, false)
			{
				ViewControllerMode = ViewControllerMode.RolesEdition
			};

			builder.CreateSummaryTile (group, roleAccessor);

			//	Crée le contenu de la tuile d'édition.
			group = builder.CreateEditionGroupingTile ("Data.Mail", "Adresse");
			var tile = builder.CreateEditionTile (group, mailAccessor);

			builder.CreateLinkButtons (tile.Container);

			builder.CreateHintEditor (tile.Container, "Nom et code du pays", this.Entity.Address.Location.Country, countryAccessor, x => this.Entity.Address.Location.Country = x as Entities.CountryEntity);
			builder.CreateMargin (tile.Container, true);

			builder.CreateTextField (tile.Container, 0, "Rue", mailAccessor.StreetName, x => mailAccessor.StreetName = x, Validators.StringValidator.Validate);
			builder.CreateTextFieldMulti (tile.Container, 52, "Complément de l'adresse", mailAccessor.StreetComplement, x => mailAccessor.StreetComplement = x, null);
			builder.CreateTextField (tile.Container, 0, "Boîte postale", mailAccessor.PostBoxNumber, x => mailAccessor.PostBoxNumber = x, Validators.StringValidator.Validate);

			//?builder.CreateHintEditor (tile.Container, "Code et nom de la région", mailAccessor.RegionCode, mailAccessor.RegionName, x => mailAccessor.RegionCode = x, x => mailAccessor.RegionName = x, Accessors.MailContactAccessor.regionConverter);

			builder.CreateHintEditor (tile.Container, "Numéro postal et ville", this.Entity.Address.Location, locationAccessor, x => this.Entity.Address.Location = x as Entities.LocationEntity);

			UI.SetInitialFocus (container);
#else
			var builder = new UIBuilder (container, this);

			builder.CreateHeaderEditorTile ();

			var mail = this.Entity;
			var group = builder.CreateEditionGroupingTile ("Data.Mail", "Adresse");
			var tile1 = builder.CreateEditionTile (group, this.Entity);
			var tile2 = builder.CreateEditionTile (group, this.Entity);
			var tile3 = builder.CreateEditionTile (group, this.Entity);

			builder.CreateFooterEditorTile ();

			var countryHint = builder.CreateHintEditor (tile1.Container, "Nom et code du pays", this.Entity.Address.Location.Country, null, x => this.Entity.Address.Location.Country = x as Entities.CountryEntity);
			var countryCtrl = new HintEditorController<Entities.CountryEntity>
			{
				ValueGetter = () => this.Entity.Address.Location.Country,
				ValueSetter = x => this.Entity.Address.Location.Country = x,
				Items = CoreProgram.Application.Data.GetCountries (),
				ToTextArrayConverter = x => new string[] { x.Code, x.Name },
				ToFormattedTextConverter = x => UIBuilder.FormatText (x.Name, "(", x.Code, ")")
			};
			countryCtrl.Attach (countryHint);
			builder.CreateMargin (tile2.Container, true);

			builder.CreateTextField (tile2.Container, 0, "Rue", this.Entity.Address.Street.StreetName, x => this.Entity.Address.Street.StreetName = x, Validators.StringValidator.Validate);
			builder.CreateTextFieldMulti (tile2.Container, 52, "Complément de l'adresse", this.Entity.Address.Street.Complement, x => this.Entity.Address.Street.Complement = x, Validators.StringValidator.Validate);
			builder.CreateTextField (tile2.Container, 0, "Boîte postale", this.Entity.Address.PostBox.Number, x => this.Entity.Address.PostBox.Number = x, Validators.StringValidator.Validate);
			builder.CreateMargin (tile2.Container, true);

			var locationHint = builder.CreateHintEditor (tile3.Container, "Numéro postal et ville", this.Entity.Address.Location, null, x => this.Entity.Address.Location = x as Entities.LocationEntity);
			var locationCtrl = new HintEditorController<Entities.LocationEntity>
			{
				ValueGetter = () => this.Entity.Address.Location,
				ValueSetter = x => this.Entity.Address.Location = x,
				Items = CoreProgram.Application.Data.GetLocations (),
				ToTextArrayConverter = x => new string[] { x.PostalCode, x.Name },
				ToFormattedTextConverter = x => UIBuilder.FormatText (x.PostalCode, x.Name)
			};
			locationCtrl.Attach (locationHint);

			UI.SetInitialFocus (container);
#endif
		}
	}
}

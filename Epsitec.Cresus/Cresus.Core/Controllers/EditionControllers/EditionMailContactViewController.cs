//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionMailContactViewController : EntityViewController<Entities.MailContactEntity>
	{
		public EditionMailContactViewController(string name, Entities.MailContactEntity entity)
			: base (name, entity)
		{
		}

		public override void CreateUI(Widget container)
		{
			UIBuilder builder = new UIBuilder (container, this);
			Widgets.GroupingTile group;

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
		}
	}
}

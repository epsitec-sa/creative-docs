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

			var accessor = new EntitiesAccessors.MailContactAccessor (null, this.Entity, false);

			//	Crée les tuiles.
			builder.CreateHeaderEditorTile ();

			//	Crée le contenu de la tuile d'édition.
			group = builder.CreateGroupingTile ("Data.Roles", "Rôles", false);

			var roleAccessor = new EntitiesAccessors.RolesContactAccessor (null, accessor.MailContact, false)
			{
				ViewControllerMode = ViewControllerMode.RolesEdition
			};

			builder.CreateSummaryTile (group, roleAccessor);

			//	Crée le contenu de la tuile d'édition.
			group = builder.CreateGroupingTile ("Data.Mail", "Adresse", true);
			var tile = builder.CreateEditionTile (group, accessor);

			builder.CreateLinkButtons (tile.Container);

			builder.CreateHintEditor (tile.Container, "Code et nom du pays", accessor.CountryCode, accessor.CountryName, x => accessor.CountryCode = x, x => accessor.CountryName = x, EntitiesAccessors.MailContactAccessor.countryConverter);

			builder.CreateMargin (tile.Container, true);

			builder.CreateTextField (tile.Container, 0, "Rue", accessor.StreetName, x => accessor.StreetName = x, Validators.StringValidator.Validate);
			builder.CreateTextFieldMulti (tile.Container, 52, "Complément de l'adresse", accessor.StreetComplement, x => accessor.StreetComplement = x, null);
			builder.CreateTextField (tile.Container, 0, "Boîte postale", accessor.PostBoxNumber, x => accessor.PostBoxNumber = x, Validators.StringValidator.Validate);

			builder.CreateHintEditor (tile.Container, "Code et nom de la région", accessor.RegionCode, accessor.RegionName, x => accessor.RegionCode = x, x => accessor.RegionName = x, EntitiesAccessors.MailContactAccessor.regionConverter);
			builder.CreateHintEditor (tile.Container, "Numéro postal et ville", accessor.LocationPostalCode, accessor.LocationName, x => accessor.LocationPostalCode = x, x => accessor.LocationName = x, EntitiesAccessors.MailContactAccessor.locationConverter);

			UI.SetInitialFocus (container);
		}
	}
}

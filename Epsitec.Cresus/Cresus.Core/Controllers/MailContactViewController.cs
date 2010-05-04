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

namespace Epsitec.Cresus.Core.Controllers
{
	public class MailContactViewController : EntityViewController
	{
		public MailContactViewController(string name, AbstractEntity entity, ViewControllerMode mode)
			: base (name, entity, mode)
		{
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public override void CreateUI(Widget container)
		{
			this.container = container;
			Widgets.TileGrouping group;

			System.Diagnostics.Debug.Assert (this.Entity != null);
			var accessor = new EntitiesAccessors.MailContactAccessor (null, this.Entity as Entities.MailContactEntity, false);

			//	Crée les tuiles.
			this.CreateHeaderEditorTile ();

			//	Crée le contenu de la tuile d'édition.
			group = this.CreateTileGrouping (this.container, "Data.Roles", "Rôles", false);

			var roleAccessor = new EntitiesAccessors.RolesContactAccessor (null, accessor.MailContact, false);
			this.CreateSummaryTile (group, roleAccessor, false, ViewControllerMode.RolesEdition);

			//	Crée le contenu de la tuile d'édition.
			group = this.CreateTileGrouping (this.container, "Data.Mail", "Adresse", true);
			var tile = this.CreateEditionTile (group, accessor, ViewControllerMode.None);

			this.CreateLinkButtons (tile.Container);

			this.CreateTextFieldPair (tile.Container, 70, "Code et nom du pays", accessor.CountryCode, accessor.CountryName, x => accessor.CountryCode = x, x => accessor.CountryName = x, Validators.StringValidator.Validate, Validators.StringValidator.Validate, EntitiesAccessors.MailContactAccessor.countryConverter);

			this.CreateMargin (tile.Container, true);

			this.CreateTextField (tile.Container, 0, "Rue", accessor.StreetName, x => accessor.StreetName = x, Validators.StringValidator.Validate);
			this.CreateTextFieldMulti (tile.Container, 52, "Complément de l'adresse", accessor.StreetComplement, x => accessor.StreetComplement = x, null);
			this.CreateTextField (tile.Container, 0, "Boîte postale", accessor.PostBoxNumber, x => accessor.PostBoxNumber = x, Validators.StringValidator.Validate);

			//?this.CreateTextFieldPair (tile.Container.Container, 70, "Code et nom de la région", accessor.RegionCode, accessor.RegionName, x => accessor.RegionCode = x, x => accessor.RegionName = x, Validators.StringValidator.Validate, Validators.StringValidator.Validate, EntitiesAccessors.MailContactAccessor.regionConverter);
			this.CreateTextFieldPair (tile.Container, 70, "Numéro postal et ville", accessor.LocationPostalCode, accessor.LocationName, x => accessor.LocationPostalCode = x, x => accessor.LocationName = x, Validators.PostalCodeValidator.Validate, Validators.StringValidator.Validate, EntitiesAccessors.MailContactAccessor.locationConverter);

			this.AdjustVisualForGroups ();
			this.SetInitialFocus ();
		}
	}
}

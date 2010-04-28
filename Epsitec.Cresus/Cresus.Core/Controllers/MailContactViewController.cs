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
		public MailContactViewController(string name)
			: base (name)
		{
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public override void CreateUI(Widget container)
		{
			this.container = container;

			System.Diagnostics.Debug.Assert (this.Entity != null);
			var accessor = new EntitiesAccessors.MailContactAccessor (null, this.Entity as Entities.MailContactEntity, false);

			Widgets.AbstractTile tile = this.CreateEditionTile (accessor, ViewControllerMode.None);
			FrameBox group;

			this.CreateCombo (tile.Container, 0, "Roles", accessor.RoleList, true, true, accessor.Roles, x => accessor.Roles = x, null);
			this.CreateMargin (tile.Container, true);

			this.CreateTextField (tile.Container, 0, "Rue", accessor.StreetName, x => accessor.StreetName = x, Validators.StringValidator.Validate);
			this.CreateTextFieldMulti (tile.Container, 52, "Complément de l'adresse", accessor.StreetComplement, x => accessor.StreetComplement = x, null);
			this.CreateTextField (tile.Container, 0, "Boîte postale", accessor.PostBoxNumber, x => accessor.PostBoxNumber = x, Validators.StringValidator.Validate);

			group = this.CreateGroup (tile.Container, "Numéro postal et ville");
			this.CreateTextField (group, 50, accessor.LocationPostalCode, x => accessor.LocationPostalCode = x, Validators.StringValidator.Validate);
			this.CreateTextField (group, 0, accessor.LocationName, x => accessor.LocationName = x, Validators.StringValidator.Validate);

			group = this.CreateGroup (tile.Container, "Code et nom du pays");
			this.CreateTextField (group, 50, accessor.CountryCode, x => accessor.CountryCode = x, Validators.StringValidator.Validate);
			this.CreateTextField (group, 0, accessor.CountryName, x => accessor.CountryName = x, Validators.StringValidator.Validate);

			group = this.CreateGroup (tile.Container, "Code et nom de la région");
			this.CreateTextField (group, 50, accessor.RegionCode, x => accessor.RegionCode = x, Validators.StringValidator.Validate);
			this.CreateTextField (group, 0, accessor.RegionName, x => accessor.RegionName = x, Validators.StringValidator.Validate);

			this.SetInitialFocus ();
		}
	}
}

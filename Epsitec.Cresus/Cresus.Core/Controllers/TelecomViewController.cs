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
	public class TelecomViewController : EntityViewController
	{
		public TelecomViewController(string name)
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
			var accessor = new EntitiesAccessors.TelecomContactAccessor (null, this.Entity as Entities.TelecomContactEntity, false);

			Widgets.AbstractTile tile = this.CreateEditionTile (accessor, ViewControllerMode.None);

			//?this.CreateTextField (tile.Container, 0, "Roles", accessor.Roles, x => accessor.Roles = x, Validators.StringValidator.Validate);
			this.CreateCombo (tile.Container, 0, "Roles", accessor.RoleList, true, accessor.Roles, x => accessor.Roles = x, null);
			this.CreateMargin (tile.Container, true);

			//?this.CreateTextField (tile.Container, 150, "Type du numéro", accessor.TelecomType, x => accessor.TelecomType = x, Validators.StringValidator.Validate);
			this.CreateCombo (tile.Container, 150, "Type du numéro", accessor.TelecomTypeList, false, accessor.TelecomType, x => accessor.TelecomType = x, null);
			this.CreateMargin (tile.Container, false);

			this.CreateTextField (tile.Container, 150, "Numéro de téléphone", accessor.TelecomContact.Number, x => accessor.TelecomContact.Number = x, Validators.StringValidator.Validate);
			this.CreateTextField (tile.Container, 100, "Numéro interne", accessor.TelecomContact.Extension, x => accessor.TelecomContact.Extension = x, Validators.StringValidator.Validate);

			this.SetInitialFocus ();
		}
	}
}

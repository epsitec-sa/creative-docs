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
	public class TelecomContactViewController : EntityViewController
	{
		public TelecomContactViewController(string name, AbstractEntity entity, ViewControllerMode mode)
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

			int groupIndex = 0;

			System.Diagnostics.Debug.Assert (this.Entity != null);
			var accessor = new EntitiesAccessors.TelecomContactAccessor (null, this.Entity as Entities.TelecomContactEntity, false);

			//	Crée les tuiles.
			this.CreateHeaderEditorTile ();
			Widgets.AbstractTile tile1 = this.CreateEditionTile (accessor, ViewControllerMode.None);

			var roleAccessor = new EntitiesAccessors.RolesContactAccessor (null, accessor.TelecomContact, false);
			this.CreateSummaryTile (roleAccessor, groupIndex, false, false, true, ViewControllerMode.RolesEdition);

			var telecomTypeAccessor = new EntitiesAccessors.TelecomTypeAccessor (null, accessor.TelecomContact, false);
			this.CreateSummaryTile (telecomTypeAccessor, groupIndex, false, false, true, ViewControllerMode.TelecomTypeEdition);

			Widgets.AbstractTile tile2 = this.CreateEditionTile ();
			this.CreateFooterEditorTile ();

			//	Crée le contenu de la tuile d'édition.
			this.CreateLinkButtons (tile1.Container);

			this.CreateTextField (tile2.Container, 150, "Numéro de téléphone", accessor.TelecomContact.Number, x => accessor.TelecomContact.Number = x, Validators.StringValidator.Validate);
			this.CreateTextField (tile2.Container, 100, "Numéro interne", accessor.TelecomContact.Extension, x => accessor.TelecomContact.Extension = x, Validators.StringValidator.Validate);

			this.AdjustVisualForGroups ();
			this.SetInitialFocus ();
		}
	}
}

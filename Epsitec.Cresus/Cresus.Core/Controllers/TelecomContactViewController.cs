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
			Widgets.GroupingTile group;
			Widgets.EditionTile tile;

			System.Diagnostics.Debug.Assert (this.Entity != null);
			var accessor = new EntitiesAccessors.TelecomContactAccessor (null, this.Entity as Entities.TelecomContactEntity, false);

			//	Crée les tuiles.
			this.CreateHeaderEditorTile ();

			//	Crée le contenu de la tuile d'édition.
			group = EntityViewController.CreateGroupingTile (this.container, "Data.Roles", "Rôles", false);

			var roleAccessor = new EntitiesAccessors.RolesContactAccessor (null, accessor.TelecomContact, false);
			this.CreateSummaryTile (group, roleAccessor, false, ViewControllerMode.RolesEdition);

			//	Crée le contenu de la tuile d'édition.
			group = EntityViewController.CreateGroupingTile (this.container, "Data.Type", "Type", false);

			var telecomTypeAccessor = new EntitiesAccessors.TelecomTypeAccessor (null, accessor.TelecomContact, false);
			this.CreateSummaryTile (group, telecomTypeAccessor, false, ViewControllerMode.TelecomTypeEdition);

			//	Crée le contenu de la tuile d'édition.
			group = EntityViewController.CreateGroupingTile (this.container, "Data.Telecom", "Téléphone", true);
			tile = this.CreateEditionTile (group, accessor, ViewControllerMode.None);

			this.CreateLinkButtons (tile.Container);

			this.CreateTextField (tile.Container, 150, "Numéro de téléphone", accessor.TelecomContact.Number, x => accessor.TelecomContact.Number = x, Validators.StringValidator.Validate);
			this.CreateTextField (tile.Container, 100, "Numéro interne", accessor.TelecomContact.Extension, x => accessor.TelecomContact.Extension = x, Validators.StringValidator.Validate);

			UI.SetInitialFocus (container);
		}
	}
}

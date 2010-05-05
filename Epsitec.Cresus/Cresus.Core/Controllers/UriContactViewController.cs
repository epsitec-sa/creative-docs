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
	public class UriContactViewController : EntityViewController
	{
		public UriContactViewController(string name, AbstractEntity entity, ViewControllerMode mode)
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

			System.Diagnostics.Debug.Assert (this.Entity != null);
			var accessor = new EntitiesAccessors.UriContactAccessor (null, this.Entity as Entities.UriContactEntity, false);

			//	Crée les tuiles.
			this.CreateHeaderEditorTile ();

			//	Crée le contenu de la tuile d'édition.
			group = EntityViewController.CreateGroupingTile (this.container, "Data.Roles", "Rôles", false);

			var roleAccessor = new EntitiesAccessors.RolesContactAccessor (null, accessor.UriContact, false);
			this.CreateSummaryTile (group, roleAccessor, false, ViewControllerMode.RolesEdition);

			//	Crée le contenu de la tuile d'édition.
			group = EntityViewController.CreateGroupingTile (this.container, "Data.Type", "Type", false);

			var uriSchemeAccessor = new EntitiesAccessors.UriSchemeAccessor (null, accessor.UriContact, false);
			this.CreateSummaryTile (group, uriSchemeAccessor, false, ViewControllerMode.UriSchemeEdition);
	
			//	Crée le contenu de la tuile d'édition.
			group = EntityViewController.CreateGroupingTile (this.container, "Data.Uri", "Mail", true);
			var tile = this.CreateEditionTile (group, accessor, ViewControllerMode.None);

			this.CreateLinkButtons (tile.Container);

			this.CreateTextField (tile.Container, 0, "Adresse mail", accessor.UriContact.Uri, x => accessor.UriContact.Uri = x, Validators.StringValidator.Validate);

			UI.SetInitialFocus (container);
		}
	}
}

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

			int groupIndex = 0;

			System.Diagnostics.Debug.Assert (this.Entity != null);
			var accessor = new EntitiesAccessors.UriContactAccessor (null, this.Entity as Entities.UriContactEntity, false);

			//	Crée les tuiles.
			this.CreateHeaderEditorTile ();
			Widgets.AbstractTile tile1 = this.CreateEditionTile (accessor, ViewControllerMode.None);

			var roleAccessor = new EntitiesAccessors.RolesContactAccessor (null, accessor.UriContact, false);
			this.CreateSummaryTile (roleAccessor, groupIndex, false, false, true, ViewControllerMode.RolesEdition);

			var uriSchemeAccessor = new EntitiesAccessors.UriSchemeAccessor (null, accessor.UriContact, false);
			this.CreateSummaryTile (uriSchemeAccessor, groupIndex, false, false, true, ViewControllerMode.UriSchemeEdition);

			Widgets.AbstractTile tile2 = this.CreateEditionTile ();
			this.CreateFooterEditorTile ();

			//	Crée le contenu de la tuile d'édition.
			this.CreateLinkButtons (tile1.Container);

			this.CreateTextField (tile2.Container, 0, "Adresse mail", accessor.UriContact.Uri, x => accessor.UriContact.Uri = x, Validators.StringValidator.Validate);

			this.AdjustVisualForGroups ();
			this.SetInitialFocus ();
		}
	}
}

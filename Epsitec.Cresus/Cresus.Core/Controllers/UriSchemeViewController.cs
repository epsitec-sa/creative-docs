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
	public class UriSchemeViewController : EntityViewController
	{
		public UriSchemeViewController(string name)
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
			var contact = this.Entity as Entities.AbstractContactEntity;
			System.Diagnostics.Debug.Assert (contact != null);

			var accessor = new EntitiesAccessors.UriSchemeAccessor (null, contact, false);

			//	Crée les tuiles.
			Widgets.AbstractTile tile = this.CreateEditionTile (accessor, ViewControllerMode.None);
			this.CreateFooterEditorTile ();

			//?this.CreateLinkButtons (tile.Container);

			//	Crée le contenu de la tuile d'édition.
			this.CreateCombo (tile.Container, 100, "Type du moyen de contact", accessor.UriSchemeInitializer, true, false, true, accessor.UriScheme, x => accessor.UriScheme = x, null);

			this.AdjustVisualForGroups ();
			this.SetInitialFocus ();
		}
	}
}

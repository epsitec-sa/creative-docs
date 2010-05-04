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
	public class TelecomTypeViewController : EntityViewController
	{
		public TelecomTypeViewController(string name, AbstractEntity entity, ViewControllerMode mode)
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
			var contact = this.Entity as Entities.AbstractContactEntity;
			System.Diagnostics.Debug.Assert (contact != null);

			var accessor = new EntitiesAccessors.TelecomTypeAccessor (null, contact, false);

			//	Crée les tuiles.
			this.CreateHeaderEditorTile ();

			group = this.CreateTileGrouping (this.container, "Data.Type", "Type", true);
			var tile = this.CreateEditionTile (group, accessor, ViewControllerMode.None);

			this.CreateFooterEditorTile ();

			//	Crée le contenu de la tuile d'édition.
			this.CreateCombo (tile.Container, 150, "Type du numéro de téléphone", accessor.TelecomTypeInitializer, true, false, true, accessor.TelecomType, x => accessor.TelecomType = x, null);

			this.SetInitialFocus ();
		}
	}
}

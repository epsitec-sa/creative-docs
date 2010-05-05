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
	public class EditionTelecomTypeViewController : EntityViewController<Entities.AbstractContactEntity>
	{
		public EditionTelecomTypeViewController(string name, Entities.AbstractContactEntity entity)
			: base (name, entity)
		{
		}

		public override void CreateUI(Widget container)
		{
			UIBuilder builder = new UIBuilder (container);
			Widgets.GroupingTile group;

			var contact = this.Entity;
			var accessor = new EntitiesAccessors.TelecomTypeAccessor (null, contact, false);

			//	Crée les tuiles.
			builder.CreateHeaderEditorTile ();

			group = builder.CreateGroupingTile ("Data.Type", "Type", true);
			var tile = builder.CreateEditionTile (group, accessor, ViewControllerMode.None);

			builder.CreateFooterEditorTile ();

			//	Crée le contenu de la tuile d'édition.
			builder.CreateCombo (tile.Container, 150, "Type du numéro de téléphone", accessor.TelecomTypeInitializer, true, false, true, accessor.TelecomType, x => accessor.TelecomType = x, null);

			UI.SetInitialFocus (container);
		}
	}
}

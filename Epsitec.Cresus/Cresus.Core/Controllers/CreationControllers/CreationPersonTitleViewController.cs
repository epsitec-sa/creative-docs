//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.CreationControllers
{
	public class CreationPersonTitleViewController : CreationViewController<PersonTitleEntity>
	{
		public CreationPersonTitleViewController(string name, Entities.PersonTitleEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI(Widgets.TileContainer container)
		{
			using (var builder = new UIBuilder (container, this))
			{
				builder.CreatePanelTitleTile ("Data.PersonTitle", "Titre à créer...");

				
				builder.EndPanelTitleTile ();
			}
		}


		
		protected override CreationStatus GetCreationStatus()
		{
			if (string.IsNullOrWhiteSpace (this.Entity.Name) ||
				string.IsNullOrWhiteSpace (this.Entity.ShortName))
			{
				return CreationStatus.Empty;
			}
			else
			{
				return CreationStatus.Ready;
			}
		}
	}
}

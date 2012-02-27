//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Controllers.CreationControllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Controllers.CreationControllers
{
	public sealed class CreationAiderHouseholdViewController : CreationViewController<AiderHouseholdEntity>
	{
		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreatePanelTitleTile ("Data.AiderHousehold", "Ménage à créer...");
				builder.CreateCreationButton<AiderHouseholdEntity, AiderHouseholdEntity> (this, "Nouveau ménage", "Crée un nouveau ménage, propre à cette personne");
				builder.EndPanelTitleTile ();
			}

			this.RegisterSimpleCreator (this.CreateHousehold);
		}

		private AiderHouseholdEntity CreateHousehold()
		{
			return this.BusinessContext.CreateEntity<AiderHouseholdEntity> ();
		}
	}
}
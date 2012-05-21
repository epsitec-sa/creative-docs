//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers.CreationControllers;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Controllers.CreationControllers
{
	public sealed class CreationAiderHouseholdViewController : CreationViewController<AiderHouseholdEntity>
	{
		protected override void CreateBricks(BrickWall<AiderHouseholdEntity> wall)
		{
			wall.AddBrick ()
				.Attribute (BrickMode.FullWidthPanel)
				.Input ()
					.Button ("Nouveau ménage", "Crée un nouveau ménage, propre à cette personne", this.HandleButtonCreateClicked)
					.SearchPanel ("Lier à un ménage existant", "Lier", this.HandleButtonAssociateClicked)
				.End ();
		}


		private void HandleButtonCreateClicked()
		{
			var businessContext = this.BusinessContext;
			var activePerson    = businessContext.GetMasterEntity<AiderPersonEntity> ();
			var dummyHousehold  = this.Entity;
			var finalHousehold  = businessContext.CreateEntity<AiderHouseholdEntity> ();

			System.Diagnostics.Debug.Assert (activePerson.IsNotNull ());
			System.Diagnostics.Debug.Assert (activePerson.Households.Contains (dummyHousehold));

			businessContext.ReplaceDummyEntity (dummyHousehold, finalHousehold);

			System.Diagnostics.Debug.Assert (activePerson.Households.Contains (finalHousehold));
			
			this.ReopenSubView ();
		}

		private void HandleButtonAssociateClicked(AbstractEntity entity)
		{
			var businessContext = this.BusinessContext;
			var activePerson    = businessContext.GetMasterEntity<AiderPersonEntity> ();
			var dummyHousehold  = this.Entity;
			var finalHousehold  = businessContext.GetLocalEntity (entity as AiderHouseholdEntity);

			System.Diagnostics.Debug.Assert (activePerson.IsNotNull ());
			System.Diagnostics.Debug.Assert (activePerson.Households.Contains (dummyHousehold));

			businessContext.ReplaceDummyEntity (dummyHousehold, finalHousehold);

			System.Diagnostics.Debug.Assert (activePerson.Households.Contains (finalHousehold));

			this.ReopenSubView ();
		}
	}
}
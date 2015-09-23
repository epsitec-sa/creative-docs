//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Aider.Override;
using Epsitec.Aider.Controllers.SetControllers;
using Epsitec.Aider.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderEventParticipantViewController : SummaryViewController<AiderEventParticipantEntity>
	{
		protected override void CreateBricks(BrickWall<AiderEventParticipantEntity> wall)
		{
			wall.AddBrick (p => p.Event)
				.Attribute (BrickMode.DefaultToSummarySubView);

			if (this.Entity.IsExternal == false )
			{
				if (this.Entity.Person.MainContact.IsNotNull ())
				{
					wall.AddBrick (p => p.Person.MainContact)
						.Attribute (BrickMode.DefaultToSummarySubView);	
				}
			}
			
		}
	}
}
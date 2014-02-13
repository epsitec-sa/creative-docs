//	Copyright © 2013-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;

using Epsitec.Cresus.Core.Controllers.SummaryControllers;

using System;


namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderSubscriptionRefusalViewController : SummaryViewController<AiderSubscriptionRefusalEntity>
	{
		protected override void CreateBricks(BrickWall<AiderSubscriptionRefusalEntity> wall)
		{
			this.Entity.RefreshCache ();

			switch (this.Entity.RefusalType)
			{
				case SubscriptionType.Household:
					wall.AddBrick (x => x.Household)
						.Attribute (BrickMode.DefaultToSummarySubView);
					break;

				case SubscriptionType.LegalPerson:
					wall.AddBrick (x => x.LegalPersonContact)
						.Attribute (BrickMode.DefaultToSummarySubView);
					break;

				default:
					throw new NotImplementedException ();
			}
		}
	}
}

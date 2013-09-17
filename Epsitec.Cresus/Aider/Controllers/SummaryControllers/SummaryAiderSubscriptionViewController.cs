//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;

using Epsitec.Cresus.Core.Controllers.SummaryControllers;


namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderSubscriptionViewController : SummaryViewController<AiderSubscriptionEntity>
	{
		protected override void CreateBricks(BrickWall<AiderSubscriptionEntity> wall)
		{
			wall.AddBrick ()
				.EnableActionButton<ActionAiderSubscriptionViewController0FlagVerificationRequired> ();
			
			switch (this.Entity.SubscriptionType)
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
					throw new System.NotImplementedException ();
			}
		}
	}
}

//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Controllers.EditionControllers;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Entities;

using System.Linq;


namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderPersonWarningViewController : SummaryViewController<AiderPersonWarningEntity>
	{
		public override EntityViewController GetController()
		{
			switch (this.Entity.WarningType)
			{
				case WarningType.EChPersonDataChanged:
					return new SpecializedSummaryAiderPersonWarningViewController_EChPersonDataChanged ();

				case WarningType.EChProcessDeparture:
					return new SpecializedSummaryAiderPersonWarningViewController_EChProcessDeparture ();

				case WarningType.EChProcessArrival:
					return new SpecializedSummaryAiderPersonWarningViewController_EChProcessArrival ();

				case WarningType.EChHouseholdAdded:
					return new SpecializedSummaryAiderPersonWarningViewController_EChHouseholdAdded ();

				case WarningType.EChHouseholdChanged:
					return new SpecializedSummaryAiderPersonWarningViewController_EChHouseholdChanged ();

				case WarningType.EChAddressChanged:
					return new SpecializedSummaryAiderPersonWarningViewController_EChAddressChanged ();

				case WarningType.ParishArrival:
					return new SpecializedSummaryAiderPersonWarningViewController_ParishArrival ();

				case WarningType.ParishDeparture:
					return new SpecializedSummaryAiderPersonWarningViewController_ParishDeparture ();

				case WarningType.HouseholdWithoutSubscription:
					return new SpecializedSummaryAiderPersonWarningViewController_HouseholdWithoutSubscription ();

				case WarningType.EChHouseholdMissing:
				case WarningType.MissingHousehold:
					return new SpecializedSummaryAiderPersonWarningViewController_MissingHousehold ();

				case WarningType.ParishMismatch://JokeInCode: it's like Paris Match ?! 
					return new SpecializedSummaryAiderPersonWarningViewController_ParishMismatch (); 
			}
			
			return base.GetController ();
		}


		protected override void CreateBricks(BrickWall<AiderPersonWarningEntity> wall)
		{
			System.Diagnostics.Trace.WriteLine ("Unhandled warning: " + this.Entity.WarningType.GetQualifiedName ());

			wall.AddBrick ()
				.Attribute (BrickMode.DefaultToNoSubView);
		}
	}
}

//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Aider.Controllers.ActionControllers;

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Aider.Override;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SpecializedSummaryAiderPersonWarningViewController_EChProcessDeparture : SpecializedSummaryAiderPersonWarningViewController
	{
		protected override void CreateBricks(BrickWall<AiderPersonWarningEntity> wall)
		{
			var warning     = this.Entity;
			var person      = warning.Person;
			var currentUser	= AiderUserManager.Current.AuthenticatedUser;
			bool canProcess = currentUser.IsOfficeManager ();
			bool alive      = person.IsAlive;

			wall.AddBrick ()
				.Title (x => x.WarningType)
				.Attribute (BrickMode.DefaultToSummarySubView)
				.WithSpecialController (typeof (SummaryAiderPersonWarningViewController1Details))
				.EnableActionButton<ActionAiderPersonWarningViewController61ProcessDepartureDeceased> ()
				.EnableActionButton<ActionAiderPersonWarningViewController64ProcessDeparture> ().IfTrue (alive && canProcess);

		}
	}
}

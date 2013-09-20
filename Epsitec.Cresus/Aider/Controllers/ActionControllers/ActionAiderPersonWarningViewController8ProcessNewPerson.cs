//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (8)]
	public sealed class ActionAiderPersonWarningViewController8ProcessNewPerson : ActionAiderPersonWarningViewControllerPassive
	{
		protected override void Execute()
		{
			this.ClearWarning ();
		}
	}
}

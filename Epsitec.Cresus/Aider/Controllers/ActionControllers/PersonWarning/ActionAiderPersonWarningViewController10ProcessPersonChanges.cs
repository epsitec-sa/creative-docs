//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Cresus.Core.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (10)]
	public sealed class ActionAiderPersonWarningViewController10ProcessPersonChanges : ActionAiderPersonWarningViewControllerPassive
	{
		protected override void Execute()
		{
			this.ClearWarningAndRefreshCaches ();
		}
	}
}

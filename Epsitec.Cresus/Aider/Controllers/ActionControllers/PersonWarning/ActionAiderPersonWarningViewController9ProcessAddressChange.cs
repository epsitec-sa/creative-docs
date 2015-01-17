//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Enumerations;
using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (9)]
	public sealed class ActionAiderPersonWarningViewController9ProcessAddressChange : ActionAiderPersonWarningViewControllerPassive
	{
		protected override void Execute()
		{
			this.ClearWarningAndRefreshCaches ();
		}
	}
}
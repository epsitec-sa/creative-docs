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
	[ControllerSubType (91)]
	public sealed class ActionAiderPersonWarningViewController91ProcessAddressChange : ActionAiderPersonWarningViewControllerPassive
	{
		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Marquer comme lu<br/>pour tout le ménage");
		}

		protected override void Execute()
		{
			this.ClearWarningAndRefreshCaches ();
			this.ClearWarningAndRefreshCachesForAll (WarningType.EChAddressChanged);
		}
	}
}


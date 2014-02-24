//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (0)]
	public sealed class ActionAiderOfficeSettingsViewController0ActivateSettings : ActionViewController<AiderOfficeSettingsEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Activer");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		private void Execute()
		{
			this.Entity.Office.ActivateSettings (this.Entity);
		}
	}
}

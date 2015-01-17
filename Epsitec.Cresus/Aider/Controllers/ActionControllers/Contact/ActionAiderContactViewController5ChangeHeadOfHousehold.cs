//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (5)]
	public sealed class ActionAiderContactViewController5ChangeHeadOfHousehold : TemplateActionViewController<AiderContactEntity, AiderPersonEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Modifier le statut de chef du ménage");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		private void Execute()
		{
			var person    = this.AdditionalEntity;
			var household = this.Entity.Household;

			person.ToggleHouseholdRole (household);
		}
	}
}
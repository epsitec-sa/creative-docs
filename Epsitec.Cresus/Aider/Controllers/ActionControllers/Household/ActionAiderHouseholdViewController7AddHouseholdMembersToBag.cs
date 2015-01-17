//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Aider.Override;
using Epsitec.Cresus.Core.Library;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (7)]
	public sealed class ActionAiderHouseholdViewController7AddHouseholdMembersToBag : ActionViewController<AiderHouseholdEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Ajouter les membres au panier");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		private void Execute()
		{
			foreach (var person in this.Entity.Members)
			{
				var contact = person.MainContact;
				EntityBag.Add (contact, "Contact");
			}
		}
	}
}

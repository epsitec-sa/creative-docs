//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Cresus.Core.Controllers;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Aider.Entities;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Common.Types;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Bricks;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (100)]
	public sealed class ActionAiderPersonWarningViewController100AddToBag : ActionViewController<AiderPersonWarningEntity>
	{

		public override FormattedText GetTitle()
		{
			return "Ajouter au panier";
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		private void Execute()
		{
			var contact = this.Entity.Person.MainContact;
			if (contact.IsNotNull ())
			{
				EntityBag.Add (contact, "Contact");
			}
		}
	}
}

//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (3)]
	public sealed class ActionAiderHouseholdViewController3ChangeHeadOfHousehold : TemplateActionViewController<AiderHouseholdEntity, AiderPersonEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Modifier le statut de chef du ménage");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		public override bool RequiresAdditionalEntity()
		{
			return true;
		}

		private void Execute()
		{
			var person    = this.AdditionalEntity;
			var household = this.Entity;
			var contacts  = person.Contacts;
			var contact   = contacts.FirstOrDefault (x => x.Household == household);

			if (contact.IsNotNull ())
			{
				if (contact.HouseholdRole == Enumerations.HouseholdRole.Head)
				{
					contact.HouseholdRole = Enumerations.HouseholdRole.None;
				}
				else
				{
					contact.HouseholdRole = Enumerations.HouseholdRole.Head;
				}
			}
		}

		protected override void GetForm(ActionBrick<AiderHouseholdEntity, SimpleBrick<AiderHouseholdEntity>> form)
		{
			var household  = this.Entity;
			var person     = this.AdditionalEntity;
			var contacts   = household.Contacts;
			int headCount  = contacts.Count (x => x.HouseholdRole == Enumerations.HouseholdRole.Head);

			FormattedText message;

			if (household.IsHead (person) == false)
			{
				string variable;
				
				if (headCount == 0)
				{
					variable = "ne contient aucun chef de ménage.";
				}
				else if (headCount == 1)
				{
					variable = "contient un chef de ménage.";
				}
				else
				{
					variable = "contient plusieurs chefs de ménage.";
				}

				message = TextFormatter.FormatText (
					"Souhaitez-vous rendre", person.GetFullName (), "chef de ce ménage ?",
					"\n \n",
					"Actuellement, ce ménage", variable);
			}
			else
			{
				if (headCount < 2)
				{
					message = TextFormatter.FormatText (
						"Souhaitez-vous vraiment que", person.GetFullName (), "ne soit plus chef de ce ménage ?",
						"\n \n",
						"Il n'y aura alors plus aucun chef de ménage.");
				}
				else
				{
					var variable = headCount > 1 ? "un chef de ménage." : "plusieurs chefs de ménage.";
					message = TextFormatter.FormatText (
						"Souhaitez-vous que", person.GetFullName (), "ne soit plus chef de ce ménage ?",
						"\n \n",
						"Il y aura encore", variable);
				}
			}
			
			form
				.Title ("Modifier le statut de chef du ménage ?")
				.Text (message)
				.End ();
		}
	}
}

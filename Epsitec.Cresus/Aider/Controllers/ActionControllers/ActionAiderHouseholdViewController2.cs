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
	[ControllerSubType (2)]
	public sealed class ActionAiderHouseholdViewController2 : TemplateActionViewController<AiderHouseholdEntity, AiderPersonEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Retirer le membre sélectionné de ce ménage");
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
			var context = this.BusinessContext;
			
			var person    = this.AdditionalEntity;
			var household = this.Entity;
			var contacts  = person.Contacts;
			var contact   = contacts.FirstOrDefault (x => x.Household == household);

			if (contacts.Count == 1)
			{
				var newHousehold = this.BusinessContext.CreateAndRegisterEntity<AiderHouseholdEntity> ();
				AiderContactEntity.Create (this.BusinessContext, person, newHousehold, true);
			}

			if (household.Members.Count == 1)
			{
				context.DeleteEntity (household);
			}
			if (contact.IsNotNull ())
			{
				AiderContactEntity.Delete (this.BusinessContext, contact);
			}
		}

		protected override void GetForm(ActionBrick<AiderHouseholdEntity, SimpleBrick<AiderHouseholdEntity>> form)
		{
			var person = this.AdditionalEntity;
			var households = person.Households;
			int count = households.Count - 1;

			FormattedText message;

			if (count == 0)
			{
				message = TextFormatter.FormatText (
					"Souhaitez-vous vraiment retirer", person.CallName, person.eCH_Person.PersonOfficialName, "de ce ménage ?",
					"\n \n",
					"La personne sera déplacée dans un nouveau ménage vide.");
			}
			else
			{
				var variable = count > 1 ? "de plusieurs ménages." : "d'un ménage.";
				message = TextFormatter.FormatText (
					"Souhaitez-vous vraiment retirer", person.CallName, person.eCH_Person.PersonOfficialName, "de ce ménage ?",
					"\n \n",
					"La personne fera encore partie", variable);
			}
			
			form
				.Title ("Retirer le membre du ménage ?")
				.Text (message)
				.End ();
		}
	}
}

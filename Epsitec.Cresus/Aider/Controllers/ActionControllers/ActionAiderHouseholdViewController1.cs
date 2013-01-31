using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;


namespace Epsitec.Aider.Controllers.ActionControllers
{


	[ControllerSubType (1)]
	public sealed class ActionAiderHouseholdViewController1 : ActionViewController<AiderHouseholdEntity>
	{


		public override FormattedText GetTitle()
		{
			return "Ajouter un membre";
		}


		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<AiderPersonEntity, bool> (this.Execute);
		}


		private void Execute(AiderPersonEntity person, bool isPersonHead)
		{
			var household = this.Entity;

			if (person.IsNull ())
			{
				throw new BusinessRuleException (household, "Aucune personne n'a été sélectionnée.");
			}

			if (household.Members.Contains (person))
			{
				throw new BusinessRuleException (household, "La personne sélectionnée appartient déjà au ménage.");
			}

			var newContact = this.BusinessContext.CreateAndRegisterEntity<AiderContactEntity> ();

			newContact.Person = person;
			newContact.Household = this.Entity;
			newContact.ContactType = Enumerations.ContactType.PersonHousehold;
			newContact.HouseholdRole = isPersonHead ? Enumerations.HouseholdRole.Head : Enumerations.HouseholdRole.None;
		}


		protected override void GetForm(ActionBrick<AiderHouseholdEntity, SimpleBrick<AiderHouseholdEntity>> form)
		{
			form
				.Title ("Ajouter un membre")
				.Field<AiderPersonEntity> ()
					.Title ("Nouveau membre")
				.End ()
				.Field<bool> ()
					.Title ("Le nouveau membre est un chef du ménage")
					.InitialValue (true)
				.End ()
			.End ();
		}
	}


}

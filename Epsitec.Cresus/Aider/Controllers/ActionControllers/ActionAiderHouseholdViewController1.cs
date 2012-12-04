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
			return ActionExecutor.Create<AiderHouseholdEntity, AiderPersonEntity, bool> (this.Execute);
		}


		private void Execute(AiderHouseholdEntity household, AiderPersonEntity person, bool isMainHousehold)
		{
			if (person.IsNull ())
			{
				throw new BusinessRuleException (household, "Aucune persone sélectionnée.");
			}

			if (household.Members.Contains (person))
			{
				throw new BusinessRuleException (household, "La personne sélectionnée appartien déjà au ménage.");
			}

			person.SetHousehold (this.BusinessContext, household, isMainHousehold);
		}


		protected override void GetForm(ActionBrick<AiderHouseholdEntity, SimpleBrick<AiderHouseholdEntity>> form)
		{
			form
				.Title ("Ajouter un membre")
				.Field<AiderPersonEntity> ()
					.Title ("Nouveau membre")
				.End ()
				.Field<bool> ()
					.Title ("Ménage principal")
					.InitialValue (true)
				.End ()
			.End ();
		}
	}


}

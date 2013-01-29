using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

using System.Collections.Generic;


namespace Epsitec.Aider.Controllers.ActionControllers
{


	[ControllerSubType (0)]
	public sealed class ActionAiderHouseholdViewController0 : ActionViewController<AiderHouseholdEntity>
	{


		public override FormattedText GetTitle()
		{
			return "Créer un membre";
		}


		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<string, string> (this.Execute);
		}


		private void Execute(string firstname, string lastname)
		{
			var household = this.Entity;

			var newPerson = AiderPersonEntity.Create (this.BusinessContext);

			newPerson.eCH_Person.PersonFirstNames = firstname;
			newPerson.eCH_Person.PersonOfficialName = lastname;

			newPerson.SetHousehold1 (this.BusinessContext, household);
		}


		protected override void GetForm(ActionBrick<AiderHouseholdEntity, SimpleBrick<AiderHouseholdEntity>> form)
		{
			form
				.Title ("Créer un membre")
				.Field<string> ()
					.Title ("Prénom")
				.End ()
				.Field<string> ()
					.Title ("Nom")
					.InitialValue (h => h.GetDefaultLastname ())
				.End ()
			.End ();
		}
	}


}

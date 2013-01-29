using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

using System.Collections.Generic;


namespace Epsitec.Aider.Controllers.ActionControllers
{


	[ControllerSubType (0)]
	public sealed class ActionAiderPersonViewController0 : ActionViewController<AiderPersonEntity>
	{


		public override FormattedText GetTitle()
		{
			return "Cr�er un nouveau m�nage";
		}


		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<bool> (this.Execute);
		}


		private void Execute(bool isMainHousehold)
		{
			var person = this.Entity;

			var newHousehold = this.BusinessContext.CreateAndRegisterEntity<AiderHouseholdEntity> ();

			person.SetHousehold (this.BusinessContext, newHousehold, isMainHousehold);
		}
		

		protected override void GetForm(ActionBrick<AiderPersonEntity, SimpleBrick<AiderPersonEntity>> form)
		{
			form
				.Title ("Cr�er un nouveau m�nage")
				.Text ("Vous allez cr�er un nouveau m�nage pour cette personne. Choisissez s'il s'agit du m�nage principal ou secondaire.")
				.Field<bool> ()
					.Title ("M�nage principal")
					.InitialValue (true)
				.End ()
			.End ();
		}
	}


}

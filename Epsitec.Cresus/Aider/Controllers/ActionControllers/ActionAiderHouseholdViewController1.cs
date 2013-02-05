//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (1)]
	public sealed class ActionAiderHouseholdViewController1 : ActionViewController<AiderHouseholdEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Ajouter un membre");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<AiderContactEntity, bool> (this.Execute);
		}


		private void Execute(AiderContactEntity contact, bool isPersonHead)
		{
			var household = this.Entity;
			var person    = contact.Person;

			ActionAiderHouseholdViewController1.ValidatePerson (household, person);			
			ActionAiderHouseholdViewController1.ValidateHouseholdComposition (household, person);
			ActionAiderHouseholdViewController1.ValidatePersonAge (household, person);

			AiderContactEntity.Create (this.BusinessContext, person, household, isPersonHead);
		}


		private static void ValidatePerson(AiderHouseholdEntity household, AiderPersonEntity person)
		{
			if (person.IsNull ())
			{
				Logic.BusinessRuleException (household, Resources.Text ("Aucune personne n'a été sélectionnée, ou le contact choisi ne correspond pas à une personne physique."));
			}
			if (person.IsDeceased)
			{
				Logic.BusinessRuleException (household, Resources.Text ("Il n'est pas possible d'associer une personne décédée à un ménage."));
			}
		}

		private static void ValidateHouseholdComposition(AiderHouseholdEntity household, AiderPersonEntity person)
		{
			if (household.Members.Contains (person))
			{
				Logic.BusinessRuleException (household, "La personne sélectionnée appartient déjà au ménage.");
			}
		}
		
		private static void ValidatePersonAge(AiderHouseholdEntity household, AiderPersonEntity person)
		{
			var age = person.ComputeAge ();

			if ((age.HasValue) &&
				(age.Value < 18))
			{
				Logic.BusinessRuleException (household, string.Format (Resources.Text ("Un enfant de {0} ans ne peut pas être un chef de ménage."), age.Value));
			}
		}

		
		protected override void GetForm(ActionBrick<AiderHouseholdEntity, SimpleBrick<AiderHouseholdEntity>> form)
		{
			form
				.Title ("Ajouter un membre")
				.Field<AiderContactEntity> ()
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

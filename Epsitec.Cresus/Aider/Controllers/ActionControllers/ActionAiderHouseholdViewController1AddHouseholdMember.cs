//	Copyright � 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	public sealed class ActionAiderHouseholdViewController1AddHouseholdMember : ActionViewController<AiderHouseholdEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Ajouter un membre au m�nage");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<AiderContactEntity, bool> (this.Execute);
		}

		private void Execute(AiderContactEntity contact, bool isPersonHead)
		{
			var household = this.Entity;
			var person    = contact.Person;

			ActionAiderHouseholdViewController1AddHouseholdMember.ValidatePerson (household, person);
			ActionAiderHouseholdViewController1AddHouseholdMember.ValidateHouseholdComposition (household, person);
			ActionAiderHouseholdViewController1AddHouseholdMember.ValidatePersonAge (household, person, isPersonHead);

			AiderContactEntity.Create (this.BusinessContext, person, household, isPersonHead);
		}

		internal static void ValidatePerson(AiderHouseholdEntity household, AiderPersonEntity person)
		{
			if (person.IsNull ())
			{
				Logic.BusinessRuleException (household, Resources.Text ("Aucune personne n'a �t� s�lectionn�e, ou le contact choisi ne correspond pas � une personne physique."));
			}
			if (person.IsDeceased)
			{
				Logic.BusinessRuleException (household, Resources.Text ("Il n'est pas possible d'associer une personne d�c�d�e � un m�nage."));
			}
			var sex = person.eCH_Person.PersonSex;

			if ((sex != Enumerations.PersonSex.Female) &&
				(sex != Enumerations.PersonSex.Male))
			{
				Logic.BusinessRuleException (household, Resources.Text ("Il n'est pas possible d'associer une personne sans sexe connu � un m�nage."));
			}
		}

		internal static void ValidateHouseholdComposition(AiderHouseholdEntity household, AiderPersonEntity person)
		{
			if (household.Members.Contains (person))
			{
				Logic.BusinessRuleException (household, "La personne s�lectionn�e appartient d�j� au m�nage.");
			}
		}

		internal static void ValidatePersonAge(AiderHouseholdEntity household, AiderPersonEntity person, bool isPersonHead)
		{
			if (isPersonHead == false)
			{
				return;
			}

			var age = person.Age;

			if ((age.HasValue) &&
				(age.Value < 18))
			{
				Logic.BusinessRuleException (household, string.Format (Resources.Text ("Un enfant de {0} ans ne peut pas �tre un chef de m�nage."), age.Value));
			}
		}

		protected override void GetForm(ActionBrick<AiderHouseholdEntity, SimpleBrick<AiderHouseholdEntity>> form)
		{
			form
				.Title ("Ajouter un membre au m�nage")
				.Text ("S�lectionnez dans la liste des contacts la personne � ajouter � ce m�nage.")
				.Field<AiderContactEntity> ()
					.Title ("Nouveau membre")
				.End ()
				.Field<bool> ()
					.Title ("Le nouveau membre est un chef du m�nage")
					.InitialValue (false)
				.End ()
			.End ();
		}
	}
}

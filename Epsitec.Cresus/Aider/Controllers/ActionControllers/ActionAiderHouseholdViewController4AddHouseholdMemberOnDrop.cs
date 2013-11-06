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
	[ControllerSubType (4)]
	public sealed class ActionAiderHouseholdViewController4AddHouseholdMemberOnDrop : TemplateActionViewController<AiderHouseholdEntity,AiderContactEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Ajouter un contact au ménage");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create <bool>(this.Execute);
		}

		private void Execute(bool isPersonHead)
		{
			var household = this.Entity;
			var person    = this.AdditionalEntity.Person;

			ActionAiderHouseholdViewController1AddHouseholdMember.ValidatePerson (household, person);
			ActionAiderHouseholdViewController1AddHouseholdMember.ValidateHouseholdComposition (household, person);
			ActionAiderHouseholdViewController1AddHouseholdMember.ValidatePersonAge (household, person, isPersonHead);

			AiderContactEntity.Create (this.BusinessContext, person, household, isPersonHead);

			household.RefreshCache ();
		}

		internal static void ValidatePerson(AiderHouseholdEntity household, AiderPersonEntity person)
		{
			if (person.IsNull ())
			{
				Logic.BusinessRuleException (household, Resources.Text ("Aucune personne n'a été sélectionnée, ou le contact choisi ne correspond pas à une personne physique."));
			}
			if (person.IsDeceased)
			{
				Logic.BusinessRuleException (household, Resources.Text ("Il n'est pas possible d'associer une personne décédée à un ménage."));
			}
			var sex = person.eCH_Person.PersonSex;

			if ((sex != Enumerations.PersonSex.Female) &&
				(sex != Enumerations.PersonSex.Male))
			{
				Logic.BusinessRuleException (household, Resources.Text ("Il n'est pas possible d'associer une personne sans sexe connu à un ménage."));
			}
		}

		internal static void ValidateHouseholdComposition(AiderHouseholdEntity household, AiderPersonEntity person)
		{
			if (household.Members.Contains (person))
			{
				Logic.BusinessRuleException (household, "La personne sélectionnée appartient déjà au ménage.");
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
				Logic.BusinessRuleException (household, string.Format (Resources.Text ("Un enfant de {0} ans ne peut pas être un chef de ménage."), age.Value));
			}
		}

		protected override void GetForm(ActionBrick<AiderHouseholdEntity, SimpleBrick<AiderHouseholdEntity>> form)
		{
			form
				.Title (TextFormatter.FormatText ("Ajouter", this.AdditionalEntity.DisplayName, "au ménage ?"))
				.Field<bool> ()
					.Title ("chef du ménage")
					.InitialValue (false)
				.End ()
			.End ();
		}
	}
}

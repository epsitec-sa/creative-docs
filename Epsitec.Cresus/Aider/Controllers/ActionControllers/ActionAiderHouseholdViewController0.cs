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
	[ControllerSubType (0)]
	internal sealed class ActionAiderHouseholdViewController0 : ActionViewController<AiderHouseholdEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Créer un nouveau membre et l'ajouter au ménage");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<string, string, Enumerations.PersonSex, Date?, bool, bool> (this.Execute);
		}

		
		private void Execute(string firstname, string lastname, Enumerations.PersonSex sex, Date? birthdate, bool isPersonHead, bool isUserSure)
		{
			var household = this.Entity;
			var context   = this.BusinessContext;

			if (isUserSure == false)
			{
				Logic.BusinessRuleException (household, Resources.Text ("Avez-vous bien vérifié que cette personne n'existait pas déjà dans AIDER?"));
			}

			var person = context.CreateAndRegisterEntity<AiderPersonEntity> ();

			ActionAiderHouseholdViewController0.AssignPersonValues (person, firstname, lastname, sex, birthdate);
			ActionAiderHouseholdViewController0.AssignPersonMrMrs (person, sex, birthdate);

			try
			{
				ActionAiderHouseholdViewController1.ValidatePerson (household, person);
				ActionAiderHouseholdViewController1.ValidateHouseholdComposition (household, person);
				ActionAiderHouseholdViewController1.ValidatePersonAge (household, person, isPersonHead);
			}
			catch
			{
				context.DeleteEntity (person.eCH_Person);
				context.DeleteEntity (person);

				throw;
			}

			AiderContactEntity.Create (context, person, household, isPersonHead);
		}

		
		private static void AssignPersonValues(AiderPersonEntity person, string firstname, string lastname, Enumerations.PersonSex sex, Date? birthdate)
		{
			person.Confession = Enumerations.PersonConfession.Protestant;

			person.eCH_Person.PersonFirstNames       = firstname;
			person.eCH_Person.PersonOfficialName     = lastname;
			person.eCH_Person.PersonDateOfBirth      = birthdate;
			person.eCH_Person.DataSource             = Enumerations.DataSource.Undefined;
			person.eCH_Person.DeclarationStatus      = Enumerations.PersonDeclarationStatus.Undefined;
			person.eCH_Person.PersonSex              = sex;
			person.eCH_Person.NationalityCountryCode = "CH";
			person.eCH_Person.NationalityStatus      = Enumerations.PersonNationalityStatus.Defined;
			person.eCH_Person.AdultMaritalStatus     = Enumerations.PersonMaritalStatus.Single;
		}

		private static void AssignPersonMrMrs(AiderPersonEntity person, Enumerations.PersonSex sex, Date? birthdate)
		{
			if (sex == Enumerations.PersonSex.Male)
			{
				person.MrMrs = Enumerations.PersonMrMrs.Monsieur;
			}
			else if (birthdate.HasValue)
			{
				var age = birthdate.Value.ComputeAge ();

				if ((age.HasValue) &&
						(age.Value < 20))
				{
					person.MrMrs = Enumerations.PersonMrMrs.Mademoiselle;
				}
				else
				{
					person.MrMrs = Enumerations.PersonMrMrs.Madame;
				}
			}
			else
			{
				person.MrMrs = Enumerations.PersonMrMrs.Madame;
			}
		}
		
		protected override void GetForm(ActionBrick<AiderHouseholdEntity, SimpleBrick<AiderHouseholdEntity>> form)
		{
			var household = this.Entity;
			var example = new AiderContactEntity ()
			{
				Household = household,
			};

			var contacts = this.BusinessContext.DataContext.GetByExample (example);
			var contact  = contacts.FirstOrDefault ();

			string lastname = "";

			if ((contact != null) &&
				(contact.Person.IsNotNull ()) &&
				(contact.Person.eCH_Person.IsNotNull ()))
			{
				lastname = contact.Person.eCH_Person.PersonOfficialName;
			}

			form
				.Title ("Créer un nouveau membre et l'ajouter au ménage")
				
				.Field<string> ()
					.Title ("Prénom")
				.End ()
				.Field<string> ()
					.Title ("Nom")
					.InitialValue (lastname)
				.End ()
				.Field<Enumerations.PersonSex> ()
					.Title ("Sexe de la personne")
					.InitialValue (Enumerations.PersonSex.Male)
				.End ()
				.Field<Date?> ()
					.Title ("Date de naissance")
				.End ()
				.Field<bool> ()
					.Title ("Le nouveau membre est un chef du ménage")
					.InitialValue (false)
				.End ()
				.Field<bool> ()
					.Title ("J'ai vérifié que cette personne n'existait pas déjà")
					.InitialValue (false)
				.End ()
			.End ();
		}
	}
}

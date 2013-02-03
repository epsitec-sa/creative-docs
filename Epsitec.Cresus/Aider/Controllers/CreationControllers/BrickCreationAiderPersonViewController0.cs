using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.CreationControllers;


namespace Epsitec.Aider.Controllers.CreationControllers
{
	[ControllerSubType (0)]
	public sealed class BrickCreationAiderPersonViewController0 : BrickCreationViewController<AiderPersonEntity>
	{
		protected override void GetForm(ActionBrick<AiderPersonEntity, SimpleBrick<AiderPersonEntity>> action)
		{
			action
				.Title ("Créer une nouvelle personne")
				.Field<string> ()
					.Title ("Prénom")
				.End ()
				.Field<string> ()
					.Title ("Nom")
				.End ()
				.Field<PersonSex> ()
					.Title ("Sex")
					.InitialValue (PersonSex.Unknown)
				.End ()
				.Field<Date?> ()
					.Title ("Date de naissance")
				.End ();
		}

		public override FunctionExecutor GetExecutor()
		{
			return FunctionExecutor.Create<string, string, PersonSex, Date, AiderPersonEntity> (this.Execute);
		}

		private AiderPersonEntity Execute(string firstname, string lastname, PersonSex sex, Date dateOfBirth)
		{
			if (string.IsNullOrEmpty (firstname))
			{
				throw new BusinessRuleException ("Le prénom ne peut pas être vide");
			}

			if (string.IsNullOrEmpty (lastname))
			{
				throw new BusinessRuleException ("Le nom ne peut pas être vide.");
			}

			var person = this.BusinessContext.CreateAndRegisterEntity<AiderPersonEntity> ();

			person.eCH_Person.PersonFirstNames = firstname;
			person.eCH_Person.PersonOfficialName = lastname;
			person.eCH_Person.PersonSex = sex;
			person.eCH_Person.PersonDateOfBirth = dateOfBirth;
		

			return person;
		}
	}
}

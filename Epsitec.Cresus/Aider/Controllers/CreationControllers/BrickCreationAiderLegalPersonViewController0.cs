//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.CreationControllers;

using Epsitec.Cresus.Core.Entities;

namespace Epsitec.Aider.Controllers.CreationControllers
{
	[ControllerSubType (0)]
	public sealed class BrickCreationAiderLegalPersonViewController0 : BrickCreationViewController<AiderLegalPersonEntity>
	{
		protected override void GetForm(ActionBrick<AiderLegalPersonEntity, SimpleBrick<AiderLegalPersonEntity>> action)
		{
			action
				.Title ("Créer une nouvelle personne morale")
				.Field<string> ()
					.Title ("Nom de la société/entreprise/association/...")
				.End ()
				.Field<ContactRole> ()
					.Title ("Role de la personne de contact (optionnel)")
					.InitialValue (ContactRole.None)
				.End ()
				.Field<PersonMrMrs> ()
					.Title ("Appellation de la personne de contact (optionnel)")
					.InitialValue (PersonMrMrs.None)
				.End ()
				.Field<string> ()
					.Title ("Nom de la personne de contact (optionnel)")
				.End ()
				.Field<AiderTownEntity> ()
					.Title ("Localité")
				.End ()
				.Field<string> ()
					.Title ("Rue avec numéro de maison")
				.End ()
				.Field<string> ()
					.Title ("Case postale")
				.End ();
		}

		public override FunctionExecutor GetExecutor()
		{
			return FunctionExecutor.Create<string, ContactRole, PersonMrMrs, string, AiderTownEntity, string, string, AiderLegalPersonEntity> (this.Execute);
		}

		private AiderLegalPersonEntity Execute(string name, ContactRole personRole, PersonMrMrs personMrMrs, string personName, AiderTownEntity town, string street, string postBox)
		{
			if (string.IsNullOrEmpty (name))
			{
				throw new BusinessRuleException ("Le nom de la société est obligatoire");
			}

			if (town.IsNull ())
			{
				throw new BusinessRuleException ("La localité est obligatoire");
			}

			if (string.IsNullOrEmpty (street))
			{
				throw new BusinessRuleException ("La rue est obligatoire");
			}

			var legalPerson = this.BusinessContext.CreateAndRegisterEntity<AiderLegalPersonEntity> ();
			legalPerson.Name = name;

			var address = legalPerson.Address;
			address.Town = town;
			address.StreetHouseNumberAndComplement = street;
			address.PostBox = postBox;

			AiderContactEntity.Create (this.BusinessContext, legalPerson, personMrMrs, personName, personRole);

			return legalPerson;
		}
	}
}

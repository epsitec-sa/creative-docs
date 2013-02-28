using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.CreationControllers;

namespace Epsitec.Aider.Controllers.CreationControllers
{
	[ControllerSubType (0)]
	public sealed class BrickCreationAiderCountryViewController0 : BrickCreationViewController<AiderCountryEntity>
	{
		protected override void GetForm(ActionBrick<AiderCountryEntity, SimpleBrick<AiderCountryEntity>> action)
		{
			action
				.Title ("Créer un nouveau pays")
				.Field<string> ()
					.Title ("Nom")
				.End ()
				.Field<string> ()
					.Title ("Code ISO")
				.End ();
		}

		public override FunctionExecutor GetExecutor()
		{
			return FunctionExecutor.Create<string, string, AiderCountryEntity> (this.Execute);
		}

		private AiderCountryEntity Execute(string name, string isoCode)
		{
			if (string.IsNullOrEmpty (name))
			{
				throw new BusinessRuleException ("Le nom est obligatoire");
			}

			if (string.IsNullOrEmpty (isoCode))
			{
				throw new BusinessRuleException ("Le code ISO est obligatoire");
			}

			if (!AiderCountryEntity.IsValidIsoCode (isoCode))
			{
				throw new BusinessRuleException ("Le code ISO est invalide");
			}

			if (AiderCountryEntity.Find (this.BusinessContext, isoCode).IsNotNull ())
			{
				throw new BusinessRuleException ("Un pays avec le même code ISO existe déjà.");
			}

			return AiderCountryEntity.Create (this.BusinessContext, isoCode, name, Mutability.Customizable);
		}
	}
}

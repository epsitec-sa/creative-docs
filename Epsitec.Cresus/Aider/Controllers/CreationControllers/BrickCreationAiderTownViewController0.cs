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
	public sealed class BrickCreationAiderTownViewController0 : BrickCreationViewController<AiderTownEntity>
	{
		protected override void GetForm(ActionBrick<AiderTownEntity, SimpleBrick<AiderTownEntity>> action)
		{
			var favorites = AiderCountryEntity.GetCountryFavorites (this.BusinessContext);

			action
				.Title ("Créer une nouvelle localité")
				.Field<string> ()
					.Title ("Nom")
				.End ()
				.Field<string> ()
					.Title ("Numéro postal")
				.End ()
				.Field<AiderCountryEntity> ()
					.Title ("Pays")
					.WithFavorites (favorites)
				.End ();
		}

		public override FunctionExecutor GetExecutor()
		{
			return FunctionExecutor.Create<string, string, AiderCountryEntity, AiderTownEntity> (this.Execute);
		}

		private AiderTownEntity Execute(string name, string zipCode, AiderCountryEntity country)
		{
			if (string.IsNullOrEmpty (name))
			{
				throw new BusinessRuleException ("Le nom est obligatoire");
			}

			if (string.IsNullOrEmpty (zipCode))
			{
				throw new BusinessRuleException ("Le code postal est obligatoire");
			}

			if (country.IsNull ())
			{
				throw new BusinessRuleException ("Le pays est obligatoire");
			}

			return AiderTownEntity.Create (this.BusinessContext, country, zipCode, name, Mutability.Customizable);
		}
	}
}

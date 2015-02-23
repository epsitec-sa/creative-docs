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
	public sealed class BrickCreationAiderEventPlaceViewController0 : BrickCreationViewController<AiderEventPlaceEntity>
	{
		protected override void GetForm(ActionBrick<AiderEventPlaceEntity, SimpleBrick<AiderEventPlaceEntity>> action)
		{
			action
				.Title ("Créer un nouveau lieu de célébration")
				.Field<string> ()
					.Title ("Nom")
				.End ()
				.Field<AiderOfficeManagementEntity> ()
					.Title ("Gestion")
				.End ()
				.Field<bool> ()
					.Title ("Lieu partagé ?")
				.End ();
		}

		public override FunctionExecutor GetExecutor()
		{
			return FunctionExecutor.Create<string, AiderOfficeManagementEntity, bool, AiderEventPlaceEntity> (this.Execute);
		}

		private AiderEventPlaceEntity Execute(string name, AiderOfficeManagementEntity office, bool shared)
		{
			if (string.IsNullOrEmpty (name))
			{
				throw new BusinessRuleException ("Le nom est obligatoire");
			}

			if (office != null)
			{
				if (!office.ParishGroup.IsParish ())
				{
					throw new BusinessRuleException ("La gestion n'est pas de type paroisse");
				}
			}
			

			return AiderEventPlaceEntity.Create (this.BusinessContext, name, shared, office);
		}
	}
}

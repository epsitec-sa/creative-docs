using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.CreationControllers;
using Epsitec.Aider.Override;

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
                .Field<AiderTownEntity> ()
                    .Title ("Localité")
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
			return FunctionExecutor.Create<string, AiderTownEntity, AiderOfficeManagementEntity, bool, AiderEventPlaceEntity> (this.Execute);
		}

		private AiderEventPlaceEntity Execute(string name, AiderTownEntity town, AiderOfficeManagementEntity office, bool shared)
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

			var user = AiderUserManager.Current.AuthenticatedUser;
			if (user.CanViewOfficeDetails ())
			{
				return AiderEventPlaceEntity.Create (this.BusinessContext, name, town, shared, office);
			}
			else
			{
				if (office == null)
				{
					throw new BusinessRuleException ("Veuillez choisir une gestion!");
				}
				else
				{
					if(user.Office.OfficeName != office.OfficeName)
					{
						throw new BusinessRuleException ("Vous n'êtes pas gestionnaire de cette gestion");
					}
					else
					{
						return AiderEventPlaceEntity.Create (this.BusinessContext, name, town, shared, office);
					}
				}
			}
			
		}
	}
}

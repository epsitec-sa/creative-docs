//	Copyright © 2013-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.CreationControllers;
using Epsitec.Aider.Controllers.SpecialFieldControllers;

namespace Epsitec.Aider.Controllers.CreationControllers
{
	[ControllerSubType (0)]
	public sealed class BrickCreationAiderOfficeManagementViewController0 : BrickCreationViewController<AiderOfficeManagementEntity>
	{
		protected override void GetForm(ActionBrick<AiderOfficeManagementEntity, SimpleBrick<AiderOfficeManagementEntity>> action)
		{
			action
				.Title ("Ajouter une gestion")
				.Field<AiderGroupEntity> ()
					.Title ("Paroisse de référence")
					.WithSpecialField<AiderGroupSpecialField<AiderOfficeManagementEntity>> ()
				.End ();
		}

		public override FunctionExecutor GetExecutor()
		{
			return FunctionExecutor.Create<AiderGroupEntity, AiderOfficeManagementEntity> (this.Execute);
		}

		private AiderOfficeManagementEntity Execute(AiderGroupEntity group)
		{
			if (group.IsNull ())
			{
				throw new BusinessRuleException ("Un groupe de référence est obligatoire");
			}
			else
			{
				if(!group.IsParish ())
				{
					throw new BusinessRuleException ("Ce groupe ne correspond pas à une paroisse");
				}
			}

			return AiderOfficeManagementEntity.Create (this.BusinessContext, group.Name, group);
		}
	}
}

using Epsitec.Aider.Data.Common;
using Epsitec.Common.Types;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.CreationControllers;
using Epsitec.Cresus.Core.Entities;

using System.Linq;

namespace Epsitec.Aider.Controllers.CreationControllers
{
	[ControllerSubType (0)]
	public sealed class BrickCreationAiderMailingCategoryViewController0 : BrickCreationViewController<AiderMailingCategoryEntity>
	{
		protected override void GetForm(ActionBrick<AiderMailingCategoryEntity, SimpleBrick<AiderMailingCategoryEntity>> action)
		{
			action
				.Title ("Création d'une catégorie de publipostage")
				.Field<string> ()
					.Title ("Nom de la catégorie")
					.InitialValue ("Nouvelle catégorie")
				.End ();
		}

		public override FunctionExecutor GetExecutor()
		{
			return FunctionExecutor.Create<string,AiderMailingCategoryEntity> (this.Execute);
		}

		private AiderMailingCategoryEntity Execute(string name)
		{
			return AiderMailingCategoryEntity.Create (this.BusinessContext,name);
		}
	}
}

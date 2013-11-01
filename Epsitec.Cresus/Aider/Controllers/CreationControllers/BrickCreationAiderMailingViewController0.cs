using Epsitec.Aider.Data.Common;
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
	public sealed class BrickCreationAiderMailingViewController0 : BrickCreationViewController<AiderMailingEntity>
	{
		protected override void GetForm(ActionBrick<AiderMailingEntity, SimpleBrick<AiderMailingEntity>> action)
		{
			action
				.Title ("Création d'un publipostage")
				.Field<string> ()
					.Title ("Intitulé du publipostage")
					.InitialValue ("Nouveau publipostage")
				.End ()
				.Field<bool> ()
					.Title ("Activé?")
					.InitialValue (false)
				.End ();
		}

		public override FunctionExecutor GetExecutor()
		{
			return FunctionExecutor.Create<string, bool,AiderMailingEntity> (this.Execute);
		}

		private AiderMailingEntity Execute(string name,bool ready)
		{
			var currentUser = UserManager.Current.AuthenticatedUser;

			if (string.IsNullOrEmpty(name))
			{
				throw new BusinessRuleException ("L'intitulé est obligatoire");
			}

			return AiderMailingEntity.Create (this.BusinessContext, currentUser, name, ready);
		}
	}
}

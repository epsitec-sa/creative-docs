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
	public sealed class BrickCreationAiderMailingViewController0 : BrickCreationViewController<AiderMailingEntity>
	{
		protected override void GetForm(ActionBrick<AiderMailingEntity, SimpleBrick<AiderMailingEntity>> action)
		{
			action
				.Title ("Création d'un publipostage")
				.Field<string> ()
					.Title ("Intitulé du publipostage")
					.InitialValue ("Publipostage du " + Date.Today.Day + "." + Date.Today.Month)
				.End ()
				.Field<string> ()
					.Title ("Description")
					.InitialValue ("Nouveau publipostage")
				.End ()
				.Field<bool> ()
					.Title ("Prêt pour l'envoi?")
					.InitialValue (false)
				.End ();
		}

		public override FunctionExecutor GetExecutor()
		{
			return FunctionExecutor.Create<string, string, bool,AiderMailingEntity> (this.Execute);
		}

		private AiderMailingEntity Execute(string name,string desc,bool ready)
		{
			var currentUser = UserManager.Current.AuthenticatedUser;

			if (string.IsNullOrEmpty(name))
			{
				throw new BusinessRuleException ("L'intitulé est obligatoire");
			}

			return AiderMailingEntity.Create (this.BusinessContext, currentUser, name, desc,ready);
		}
	}
}

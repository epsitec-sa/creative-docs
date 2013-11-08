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
using Epsitec.Aider.Override;
using Epsitec.Cresus.DataLayer.Loader;

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
				.Field<AiderMailingCategoryEntity> ()
					.Title ("Catégorie")
				.End ()
				.Field<bool> ()
					.Title ("Prêt pour l'envoi?")
					.InitialValue (false)
				.End ();
		}

		public override FunctionExecutor GetExecutor()
		{
			return FunctionExecutor.Create<string, string,AiderMailingCategoryEntity, bool,AiderMailingEntity> (this.Execute);
		}

		private AiderMailingEntity Execute(string name, string desc, AiderMailingCategoryEntity cat, bool ready)
		{
			var currentUser = AiderUserManager.Current.AuthenticatedUser;
			var userKey = AiderUserManager.Current.BusinessContext.DataContext.GetNormalizedEntityKey (currentUser);
			var aiderUser = this.DataContext.GetByRequest<AiderUserEntity> (Request.Create (new AiderUserEntity(), userKey.Value.RowKey)).First ();

			if (string.IsNullOrEmpty(name))
			{
				throw new BusinessRuleException ("L'intitulé est obligatoire");
			}

			return AiderMailingEntity.Create (this.BusinessContext, aiderUser, name, desc, cat, ready);
		}
	}
}

//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;
using System.Linq;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DeletionControllers;
using Epsitec.Aider.Override;
using Epsitec.Cresus.DataLayer.Loader;

namespace Epsitec.Aider.Controllers.DeletionControllers
{
	[ControllerSubType (0)]
	public sealed class BrickDeletionAiderMailingController0 : BrickDeletionViewController<AiderMailingEntity>
	{
		protected override void GetForm(ActionBrick<AiderMailingEntity, SimpleBrick<AiderMailingEntity>> action)
		{
			action
				.Title ("Détruire le publipostage")
				.Text ("Êtes vous sûr de vouloir détruire ce publipostage ?")
				.Field<string> ()
					.Title ("Intitulé")
					.InitialValue (x => x.Name)
					.ReadOnly ()
				.End ()
			.End ();
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<string> (this.Execute);
		}

		private void Execute(string _1)
		{

			var currentUser = AiderUserManager.Current.AuthenticatedUser;
			var userKey = AiderUserManager.Current.BusinessContext.DataContext.GetNormalizedEntityKey (currentUser);
			var aiderUser = this.DataContext.GetByRequest<AiderUserEntity> (Request.Create (new AiderUserEntity (), userKey.Value.RowKey)).First ();
			
			if (this.Entity.CreatedBy != aiderUser.DisplayName && !aiderUser.CanRemoveMailing())
			{
				var message = "Vous ne pouvez pas détruire le publipostage d'un autre utilisateur";

				throw new BusinessRuleException (this.Entity, message);
			}


			AiderMailingEntity.Delete (this.BusinessContext, this.Entity);
		}
	}
}

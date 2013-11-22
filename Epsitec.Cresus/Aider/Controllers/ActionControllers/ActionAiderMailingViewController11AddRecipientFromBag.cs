//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Library;
using Epsitec.Aider.Override;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer.Context;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (11)]
	public sealed class ActionAiderMailingViewController11AddRecipientFromBag : ActionViewController<AiderMailingEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Remplir avec l'arche");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		private void Execute()
		{
			var aiderUser = this.BusinessContext.GetLocalEntity (AiderUserManager.Current.AuthenticatedUser);

			var entitiesId = EntityBagManager.GetCurrentEntityBagManager ().GetUserBagEntitiesId (aiderUser.LoginName).ToList ();

			foreach (var entityId in entitiesId)
			{
				var entity = this.BusinessContext.DataContext.GetPersistedEntity (entityId);

				if (entity is AiderContactEntity)
				{
					EntityBagManager.GetCurrentEntityBagManager ().SetLoading (aiderUser.LoginName, true);

					this.Entity.AddContact (this.BusinessContext, (AiderContactEntity) entity);
					//Remove entity from the bag
					this.ConfirmByRemoveFromBag (entity,aiderUser.LoginName);
				}

				if (entity is AiderPersonEntity)
				{
					var aiderPerson = (AiderPersonEntity) entity;
					EntityBagManager.GetCurrentEntityBagManager ().SetLoading (aiderUser.LoginName, true);

					this.Entity.AddContact (this.BusinessContext, aiderPerson.MainContact);
					//Remove entity from the bag
					this.ConfirmByRemoveFromBag (entity, aiderUser.LoginName);
				}

				if (entity is AiderGroupEntity)
				{
					EntityBagManager.GetCurrentEntityBagManager ().SetLoading (aiderUser.LoginName, true);

					this.Entity.AddGroup (this.BusinessContext, (AiderGroupEntity) entity);
					//Remove entity from the bag
					this.ConfirmByRemoveFromBag (entity, aiderUser.LoginName);
				}

				if (entity is AiderHouseholdEntity)
				{
					EntityBagManager.GetCurrentEntityBagManager ().SetLoading (aiderUser.LoginName, true);

					this.Entity.AddHousehold (this.BusinessContext, (AiderHouseholdEntity) entity);
					//Remove entity from the bag
					this.ConfirmByRemoveFromBag (entity, aiderUser.LoginName);
				}

				if (entity is AiderLegalPersonEntity)
				{
					var legalPerson = (AiderLegalPersonEntity) entity;
					EntityBagManager.GetCurrentEntityBagManager ().SetLoading (aiderUser.LoginName, true);

					this.Entity.AddContact (this.BusinessContext, legalPerson.GetMainContact ());
					//Remove entity from the bag
					this.ConfirmByRemoveFromBag (entity, aiderUser.LoginName);
				}	
			}	
		}

		private void ConfirmByRemoveFromBag(AbstractEntity entity,string aiderUser)
		{
			var id = this.BusinessContext.DataContext.GetNormalizedEntityKey (entity).Value.ToString ().Replace ('/', '-');
			EntityBagManager.GetCurrentEntityBagManager ().RemoveFromBag (aiderUser, id, When.Now);
			EntityBagManager.GetCurrentEntityBagManager ().SetLoading (aiderUser, false);
		}
	}
}

//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Controllers.SpecialFieldControllers;
using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Entities;

using System.Linq;
using Epsitec.Aider.Override;
using Epsitec.Cresus.Core.Library;
using System.Collections.Generic;
using Epsitec.Common.Support.EntityEngine;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (7)]
	public sealed class ActionAiderGroupViewController7AddMembersFromBag : ActionViewController<AiderGroupEntity>
	{
		public override FormattedText GetTitle()
		{
			return "Ajouter des membres depuis l'arche";
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<Date?, FormattedText> (this.Execute);
		}

		private void Execute(Date? startDate, FormattedText comment)
		{
			if (!this.Entity.CanBeEditedByCurrentUser ())
			{
				var message = "Vous n'avez pas le droit d'éditer ce groupe";

				throw new BusinessRuleException (message);
			}

			var contacts = new List<AiderContactEntity> ();
			
			var aiderUser = this.BusinessContext.GetLocalEntity (AiderUserManager.Current.AuthenticatedUser);

			var entitiesId = EntityBagManager.GetCurrentEntityBagManager ().GetUserBagEntitiesId (aiderUser.LoginName).ToList ();

			foreach (var entityId in entitiesId)
			{
				var entity = this.BusinessContext.DataContext.GetPersistedEntity (entityId);

				if (entity is AiderContactEntity)
				{
					contacts.Add ((AiderContactEntity) entity);
					//Remove entity from the bag
					this.ConfirmByRemoveFromBag (entity, aiderUser.LoginName);
				}
			}

			if (contacts.Count > 0)
			{
				this.Entity.ImportContactsMembers (this.BusinessContext, contacts, startDate, comment);
			}
		}

		private void ConfirmByRemoveFromBag(AbstractEntity entity, string aiderUser)
		{
			var id = this.BusinessContext.DataContext.GetNormalizedEntityKey (entity).Value.ToString ().Replace ('/', '-');
			EntityBagManager.GetCurrentEntityBagManager ().RemoveFromBag (aiderUser, id, When.Now);
			EntityBagManager.GetCurrentEntityBagManager ().SetLoading (aiderUser, false);
		}

		protected override void GetForm(ActionBrick<AiderGroupEntity, SimpleBrick<AiderGroupEntity>> form)
		{
			form
				.Title (this.GetTitle ())
				.Field<Date> ()
					.Title ("Date d'entrée dans le groupe")
					.InitialValue (Date.Today)
				.End ()
				.Field<FormattedText> ()
					.Title ("Commentaire")
					.Multiline ()
				.End ()
			.End ();
		}
	}
}

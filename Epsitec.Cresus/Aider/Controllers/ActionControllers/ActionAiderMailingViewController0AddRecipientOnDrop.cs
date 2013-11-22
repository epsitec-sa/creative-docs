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

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (0)]
	public sealed class ActionAiderMailingViewController0AddRecipientOnDrop : AbstractTemplateActionViewController<AiderMailingEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Ajouter aux destinataires");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		private void Execute()
		{
			var aiderUser = this.BusinessContext.GetLocalEntity (AiderUserManager.Current.AuthenticatedUser);

			if (this.AdditionalEntity is AiderPersonEntity)
			{
				var aiderPerson = (AiderPersonEntity) this.AdditionalEntity;
				EntityBagManager.GetCurrentEntityBagManager ().SetLoading (aiderUser.LoginName, true);

				this.Entity.AddContact (this.BusinessContext, aiderPerson.MainContact);
				//Remove entity from the bag
				this.ConfirmByRemoveFromBag (aiderUser.LoginName);
			}

			if (this.AdditionalEntity is AiderContactEntity)
			{
				EntityBagManager.GetCurrentEntityBagManager ().SetLoading (aiderUser.LoginName, true);

				this.Entity.AddContact (this.BusinessContext, (AiderContactEntity) this.AdditionalEntity);
				//Remove entity from the bag
				this.ConfirmByRemoveFromBag (aiderUser.LoginName);
			}

			if (this.AdditionalEntity is AiderGroupEntity)
			{
				EntityBagManager.GetCurrentEntityBagManager ().SetLoading (aiderUser.LoginName, true);

				this.Entity.AddGroup (this.BusinessContext, (AiderGroupEntity) this.AdditionalEntity);
				//Remove entity from the bag
				this.ConfirmByRemoveFromBag (aiderUser.LoginName);
			}

			if (this.AdditionalEntity is AiderGroupExtractionEntity)
			{
				EntityBagManager.GetCurrentEntityBagManager ().SetLoading (aiderUser.LoginName, true);

				this.Entity.AddGroupExtraction (this.BusinessContext, (AiderGroupExtractionEntity) this.AdditionalEntity);
				//Remove entity from the bag
				this.ConfirmByRemoveFromBag (aiderUser.LoginName);
			}

			if (this.AdditionalEntity is AiderHouseholdEntity)
			{
				EntityBagManager.GetCurrentEntityBagManager ().SetLoading (aiderUser.LoginName, true);

				this.Entity.AddHousehold (this.BusinessContext, (AiderHouseholdEntity) this.AdditionalEntity);
				//Remove entity from the bag
				this.ConfirmByRemoveFromBag (aiderUser.LoginName);
			}

			if (this.AdditionalEntity is AiderLegalPersonEntity)
			{
				var legalPerson = (AiderLegalPersonEntity) this.AdditionalEntity;
				EntityBagManager.GetCurrentEntityBagManager ().SetLoading (aiderUser.LoginName, true);

				this.Entity.AddContact (this.BusinessContext, legalPerson.GetMainContact ());
				//Remove entity from the bag
				this.ConfirmByRemoveFromBag (aiderUser.LoginName);
			}	
		}

		private void ConfirmByRemoveFromBag(string aiderUser)
		{
			
			var id = this.BusinessContext.DataContext.GetNormalizedEntityKey (this.AdditionalEntity).Value.ToString ().Replace ('/', '-');
			EntityBagManager.GetCurrentEntityBagManager ().RemoveFromBag (aiderUser, id, When.Now);
			EntityBagManager.GetCurrentEntityBagManager ().SetLoading (aiderUser, false);
		}
	}
}

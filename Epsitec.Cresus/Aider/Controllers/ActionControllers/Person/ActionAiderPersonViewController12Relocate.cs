//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Aider.Controllers.SpecialFieldControllers;
using Epsitec.Aider.Override;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Library;
using Epsitec.Aider.Reporting;
using Epsitec.Aider.BusinessCases;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (12)]
	public sealed class ActionAiderPersonViewController12Relocate : ActionViewController<AiderPersonEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Placer la personne dans un autre ménage...");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create <AiderHouseholdEntity,bool>(this.Execute);
		}

		protected override void GetForm(ActionBrick<AiderPersonEntity, SimpleBrick<AiderPersonEntity>> form)
		{
			form
				.Title ("Changement de ménage")
				.Field<AiderHouseholdEntity> ()
					.Title ("Choix du nouveau ménage")
				.End ()
				.Field<bool> ()
					.Title ("En tant que chef de ménage ?")
					.InitialValue (false)
				.End ()
			.End ();
		}

		private void Execute(AiderHouseholdEntity newHousehold,bool isHead)
		{
			var currentHousehold = this.Entity.MainContact.Household;
			if (currentHousehold.Members.Where (m => currentHousehold.IsHead (m)).Count () > 1 && currentHousehold.IsHead (this.Entity))
			{
				this.Entity.MainContact.Household.RemoveContactInternal (this.Entity.MainContact);
			}
			else
			{
				throw new BusinessRuleException ("Impossible de déplacer cette persone, la configuration actuelle du ménage ne le permet pas.");
			}
			
			this.Entity.MainContact = AiderContactEntity.Create (this.BusinessContext, this.Entity, newHousehold, isHead);	
		}
	}
}

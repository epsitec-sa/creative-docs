//	Copyright � 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
			return Resources.Text ("Placer la personne dans un autre m�nage...");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create <AiderHouseholdEntity,bool,bool>(this.Execute);
		}

		protected override void GetForm(ActionBrick<AiderPersonEntity, SimpleBrick<AiderPersonEntity>> form)
		{
			form
				.Title ("Changement de m�nage")
				.Field<AiderHouseholdEntity> ()
					.Title ("Choix du nouveau m�nage")
				.End ()
				.Field<bool> ()
					.Title ("En tant que chef de m�nage ?")
					.InitialValue (false)
				.End ()
				.Field<bool> ()
					.Title ("Conserver la pr�sence dans le m�nage actuel ?")
					.InitialValue (false)
				.End ()
			.End ();
		}

		private void Execute(AiderHouseholdEntity newHousehold,bool isHead,bool stayInPlace)
		{
			var currentHousehold = this.Entity.MainContact.Household;
			if (!stayInPlace)
			{
				AiderContactEntity.Delete (this.BusinessContext, this.Entity.MainContact, true);
				currentHousehold.RefreshCache ();
				var currentSubscription = AiderSubscriptionEntity.FindSubscription (this.BusinessContext, currentHousehold);
				if (currentSubscription.IsNotNull ())
				{
					currentSubscription.RefreshCache ();
				}
			}
	
			AiderContactEntity.Create (this.BusinessContext, this.Entity, newHousehold, isHead);			
			newHousehold.RefreshCache ();
			var subscription = AiderSubscriptionEntity.FindSubscription (this.BusinessContext, newHousehold);
			if (subscription.IsNotNull ())
			{
				subscription.RefreshCache ();
			}
		}
	}
}
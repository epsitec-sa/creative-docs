//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

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
using Epsitec.Aider.Enumerations;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (1)]
    public sealed class ActionAiderPersonWarningViewController1ProcessPersonMissing : ActionViewController<AiderPersonWarningEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Traiter");
		}

		public override ActionExecutor GetExecutor()
		{
            return ActionExecutor.Create<bool,bool>(this.Execute);
		}

        private void Execute(bool setInvisible, bool isDecease)
		{
			if (isDecease && !this.Entity.Person.IsDeceased)
			{
				var message = "Merci d'entrer une date de décès et recommencez l'opération";

				throw new BusinessRuleException (message);
			}

            if (setInvisible)
            {
                this.Entity.Person.Visibility = PersonVisibilityStatus.Hidden;
            }

            foreach (var household in this.Entity.Person.Households)
            {
                var subscription = AiderSubscriptionEntity.FindSubscription(this.BusinessContext, household);
                if (subscription.IsNotNull())
                {
                    subscription.RefreshCache();
                }
            }

            this.Entity.Person.RemoveWarningInternal(this.Entity);
            this.BusinessContext.DeleteEntity(this.Entity);
		}

        protected override void GetForm(ActionBrick<AiderPersonWarningEntity, SimpleBrick<AiderPersonWarningEntity>> form)
        {
            form
                .Title(this.GetTitle())
                .Field<bool>()
                    .Title("Je veux rendre invisible cette personne")
                    .InitialValue(false)
                .End()
                .Field<bool>()
                    .Title("Cette personne est décédée")
                    .InitialValue(false)
                .End()
            .End();
        }
	}
}

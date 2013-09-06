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
	[ControllerSubType (10)]
    public sealed class ActionAiderPersonWarningViewController10ProcessPersonChanges : ActionViewController<AiderPersonWarningEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Traiter");
		}

		public override ActionExecutor GetExecutor()
		{
            return ActionExecutor.Create<bool>(this.Execute);
		}

		private void Execute(bool confirmed)
		{
            if (confirmed)
            {
                foreach (var contact in this.Entity.Person.Contacts)
                {
                    contact.RefreshCache();
                }
                this.Entity.Person.RemoveWarningInternal(this.Entity);
                this.BusinessContext.DeleteEntity(this.Entity);
            }       
		}

        protected override void GetForm(ActionBrick<AiderPersonWarningEntity, SimpleBrick<AiderPersonWarningEntity>> form)
        {
            form
                .Title(this.GetTitle())
                .Field<bool>()
                    .Title("Mettre à jour les contacts si besoin")
                    .InitialValue(true)
                .End()
            .End();
        }
	}
}

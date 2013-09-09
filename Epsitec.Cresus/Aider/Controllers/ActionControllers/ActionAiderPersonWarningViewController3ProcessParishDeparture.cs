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
using Epsitec.Cresus.DataLayer.Loader;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (3)]
    public sealed class ActionAiderPersonWarningViewController3ProcessParishDeparture : ActionViewController<AiderPersonWarningEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Marquer comme lu");
		}

		public override ActionExecutor GetExecutor()
		{
            return ActionExecutor.Create<bool>(this.Execute);
		}

		private void Execute(bool appliForAll)
		{

            this.Entity.Person.RemoveWarningInternal(this.Entity);
            this.BusinessContext.DeleteEntity(this.Entity);

            if (appliForAll)
            {
                foreach (var member in this.Entity.Person.Contacts.Where(c => c.Household.Address.IsNotNull()).First().Household.Members)
                {
                    foreach (var warn in member.Warnings)
                    {
                        if(warn.WarningType.Equals(WarningType.ParishDeparture))
                        {
                            member.RemoveWarningInternal(warn);
                            this.BusinessContext.DeleteEntity(warn);
                        }
                    }
                }
                
            }
		}

        protected override void GetForm(ActionBrick<AiderPersonWarningEntity, SimpleBrick<AiderPersonWarningEntity>> form)
        {
            form
                .Title(this.GetTitle())
                .Field<bool>()
					.Title ("Appliquer à tous les membres du ménage")
                    .InitialValue(true)
            .End();
        }
	}
}

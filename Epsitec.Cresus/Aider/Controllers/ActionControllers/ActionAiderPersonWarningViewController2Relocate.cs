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
	[ControllerSubType (2)]
    public sealed class ActionAiderPersonWarningViewController2Relocate : ActionViewController<AiderPersonWarningEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Traiter");
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
                var contactExample = new AiderContactEntity();
                var householdExample = new AiderHouseholdEntity();
                contactExample.Person = this.Entity.Person;
                contactExample.Household = householdExample;
                var request = new Request()
                {
                    RootEntity = contactExample,
                    RequestedEntity = householdExample
                };

                var houshold = this.BusinessContext.DataContext.GetByRequest<AiderHouseholdEntity>(request).FirstOrDefault();
                foreach (var member in houshold.Members)
                {
                    foreach (var warn in member.Warnings)
                    {
                        if(warn.WarningType.Equals(WarningType.EChAddressChanged))
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
                    .Title("Appliquer a tout le ménage")
                    .InitialValue(true)
            .End();
        }
	}
}

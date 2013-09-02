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
	[ControllerSubType (8)]
	public sealed class ActionAiderPersonWarningViewController8ProcessNewPerson : ActionViewController<AiderPersonWarningEntity>
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
				var eChHousehold = this.Entity.Person.eCH_Person.ReportedPerson1;

				if (eChHousehold.IsNotNull())
				{
					var aiderHousehold = this.GetAiderHousehold (eChHousehold);
					AiderContactEntity.Create (this.BusinessContext, this.Entity.Person, aiderHousehold, false);
				}

				this.Entity.Person.RemoveWarningInternal (this.Entity);
				this.BusinessContext.DeleteEntity (this.Entity);
            }        
		}

		private AiderHouseholdEntity GetAiderHousehold(eCH_ReportedPersonEntity eChHousehold)
		{
			var aiderPersonExample = new AiderPersonEntity ();
			var contactExample = new AiderContactEntity ();
			var householdExample = new AiderHouseholdEntity ();

			aiderPersonExample.eCH_Person.ReportedPerson1 = eChHousehold;
			contactExample.Person = aiderPersonExample;
			contactExample.Household = householdExample;
			var request = new Request ()
			{
				RootEntity = contactExample,
				RequestedEntity = householdExample
			};

			return this.BusinessContext.DataContext.GetByRequest<AiderHouseholdEntity> (request).FirstOrDefault ();
		}

        protected override void GetForm(ActionBrick<AiderPersonWarningEntity, SimpleBrick<AiderPersonWarningEntity>> form)
        {
            form
                .Title(this.GetTitle())
                .Field<bool>()
                    .Title("Confirmer")
                    .InitialValue(true)
                .End()
            .End();
        }
	}
}

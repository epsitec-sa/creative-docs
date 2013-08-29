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
using Epsitec.Data.Platform;
using Epsitec.Aider.Data.ECh;
using Epsitec.Cresus.Core.Business.UserManagement;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (6)]
	public sealed class ActionAiderPersonWarningViewController6ProcessDeparture : ActionViewController<AiderPersonWarningEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Traiter");
		}

		public override ActionExecutor GetExecutor()
		{
            return ActionExecutor.Create<bool,bool>(this.Execute);
		}

		private void Execute(bool setInvisible,bool appliForAll)
		{
            this.Entity.Person.RemoveWarningInternal(this.Entity);
            this.BusinessContext.DeleteEntity(this.Entity);

			if (appliForAll)
			{
				foreach (var member in this.Entity.Person.Contacts.First ().Household.Members)
				{
					foreach (var warn in member.Warnings)
					{
						if (warn.WarningType.Equals (WarningType.EChProcessDeparture))
						{
							member.RemoveWarningInternal (warn);
							this.BusinessContext.DeleteEntity (warn);
						}
					}
					if (setInvisible)
					{
						member.Visibility = PersonVisibilityStatus.Hidden;
					}
				}
			}
			else
			{
				if (setInvisible)
				{
					this.Entity.Person.Visibility = PersonVisibilityStatus.Hidden;
				}
			}
		}

        private AiderTownEntity GetAiderTownEntity(eCH_AddressEntity address)
        {
            var townExample = new AiderTownEntity()
            {
                SwissZipCodeId = address.SwissZipCodeId
            };

            return this.BusinessContext.DataContext.GetByExample<AiderTownEntity>(townExample).FirstOrDefault();
        }

        private eCH_ReportedPersonEntity GetNewHousehold()
        {
            var echHouseholdExample = new eCH_ReportedPersonEntity()
            {
                Adult1 = this.Entity.Person.eCH_Person
            };
            return this.BusinessContext.DataContext.GetByExample<eCH_ReportedPersonEntity>(echHouseholdExample).FirstOrDefault();
        }

		private eCH_ReportedPersonEntity GetEChHousehold(eCH_PersonEntity person)
		{
			var echHouseholdExample = new eCH_ReportedPersonEntity ()
			{
				Adult1 = person
			};
			return this.BusinessContext.DataContext.GetByExample<eCH_ReportedPersonEntity> (echHouseholdExample).FirstOrDefault ();
		}

        protected override void GetForm(ActionBrick<AiderPersonWarningEntity, SimpleBrick<AiderPersonWarningEntity>> form)
        {
			form
				.Title (this.GetTitle ())
				.Field<bool> ()
					.Title ("Changer la visibilité")
					.InitialValue (false)
				.End ()
				.Field<bool> ()
					.Title ("Appliquer à tout le ménage")
					.InitialValue (true)
				.End ()
			.End ();           
        }
	}
}

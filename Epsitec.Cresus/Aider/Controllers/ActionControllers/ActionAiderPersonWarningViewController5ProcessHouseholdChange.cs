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

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (5)]
    public sealed class ActionAiderPersonWarningViewController5ProcessHouseholdChange : ActionViewController<AiderPersonWarningEntity>
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
                var householdMembers = this.Entity.Person.Contacts.Where(c => c.Household.Address.IsNotNull()).First().Household.Members;
                foreach (var member in householdMembers)
                {
                    var warnCount = member.Warnings.Where(w => w.WarningType.Equals(WarningType.EChPersonMissing) || w.WarningType.Equals(WarningType.EChHouseholdAdded)).ToList().Count;
                    if (warnCount > 0)
                    {
                        var message = "Il faut d'abord traiter l'avertissement sur ce member: " + member.GetDisplayName();

                        throw new BusinessRuleException(message);
                    }
                }

                this.Entity.Person.RemoveWarningInternal(this.Entity);
                this.BusinessContext.DeleteEntity(this.Entity);
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
            var householdMembers = this.Entity.Person.Contacts.Where(c => c.Household.Address.IsNotNull()).First().Household.Members;
            var newHousehold = this.GetNewHousehold();
            var analyse = TextFormatter.FormatText("Résultat de l'analyse:\n");
            if (householdMembers.Count < newHousehold.GetMembersCount())
            {
                analyse = analyse.AppendLine(TextFormatter.FormatText("Le ménage ECh contient plus de membres"));
            }
            if (householdMembers.Count > newHousehold.GetMembersCount())
            {
                analyse = analyse.AppendLine(TextFormatter.FormatText("Le ménage ECh contient moins de membres"));
            }
			form
			.Title (this.GetTitle ())
            .Text (analyse)
			.Field<bool> ()
				.Title ("Je confirme")
				.InitialValue (true)
			.End ();
            
        }
	}
}

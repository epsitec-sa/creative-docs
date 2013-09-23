//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (1)]
	public sealed class ActionAiderContactViewController1FusionOnDrag : TemplateActionViewController<AiderContactEntity, AiderContactEntity>
	{
		public override bool RequiresAdditionalEntity
		{
			get
			{
				return true;
			}
		}
		
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Fusionner avec un contact non-ECh");
		}

        private string GetText()
        {
            return "Cette action va supprimer le contact non-ECh a ajouter les données sur le contact ECh";
        }

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<bool>(this.Execute);
		}

		private void Execute(bool doFusion)
		{
            if (doFusion)
            {
                //group fusion
                var groupToAdd = this.AdditionalEntity.Person.Groups.Where(g => !this.Entity.Person.Groups.Select(gs => gs.Group).Contains(g.Group));
                foreach (var group in groupToAdd)
                {
                    this.Entity.Person.AddParticipationInternal(group);
                    var participationData = new ParticipationData(this.Entity.Person);
                    AiderGroupParticipantEntity.StartParticipation(this.BusinessContext, group.Group, participationData, group.StartDate, FormattedText.FromSimpleText("Fusion"));
                }

                //household fusion
                var householdMemberToAdd = this.AdditionalEntity.Household.Members.Where(m => !this.Entity.Household.Members.Contains(m));
                foreach (var member in householdMemberToAdd)
                {
                    //Remap contact
                    var contact = member.Contacts.FirstOrDefault(c => c.Household == this.AdditionalEntity.Household);
                    contact.Household = this.Entity.Household;
                }

                this.BusinessContext.DeleteEntity(this.AdditionalEntity);
            }
		}

		protected override void GetForm(ActionBrick<AiderContactEntity, SimpleBrick<AiderContactEntity>> form)
		{
            if (this.Entity.Person.IsGovernmentDefined)
            {
                if (!this.AdditionalEntity.Person.IsGovernmentDefined)
                {
                    form
                    .Title("Fusion de données")
                    .Text(this.GetText())
                    .Field<bool>()
                        .Title("Fusionner")
                    .End()
                    .End();
                }
            }
		}
	}
}

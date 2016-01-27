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
	[ControllerSubType (7)]
	public sealed class ActionAiderOfficeManagementViewController7DuplicateEvent : TemplateActionViewController<AiderOfficeManagementEntity, AiderEventEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Dupliquer l'acte");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<Date> (this.Execute);
		}

		protected override void GetForm(ActionBrick<AiderOfficeManagementEntity, SimpleBrick<AiderOfficeManagementEntity>> form)
		{
			form
				.Field<Date> ()
					.Title ("Date de la célébration")
					.InitialValue (Date.Today)
				.End ()
			.End ();
		}

		private void Execute(Date celebrationDate)
		{
            if (this.AdditionalEntity.Place.Town.IsNull ())
            {
                throw new BusinessRuleException ("Impossible de dupliquer l'acte, le lieu de célébration n'a pas de localité renseignée");
            }

			var duplicat = AiderEventEntity.Create (
				this.BusinessContext, 
				this.AdditionalEntity.Type, 
				this.AdditionalEntity.Kind, 
				this.AdditionalEntity.Office,
				this.AdditionalEntity.Place, 
				celebrationDate
			);
			
			foreach (var participant in this.AdditionalEntity.Participants)
			{
				if (participant.IsExternal)
				{
					AiderEventParticipantEntity.CreateForExternal (
						this.BusinessContext, 
						duplicat, 
						participant.FirstName, 
						participant.LastName,
 						participant.BirthDate,
						participant.Town, 
						participant.ParishName,
						participant.Confession,
						participant.Role
					);
				}
				else
				{
					AiderEventParticipantEntity.Create (
						this.BusinessContext, 
						duplicat, 
						participant.Person, 
						participant.Role
					);
				}
				
			}
		}
	}
}

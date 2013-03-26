using Epsitec.Aider.Entities;

using Epsitec.Common.Support;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.SetControllers;

using Epsitec.Cresus.Core.Data;

using Epsitec.Cresus.Core.Entities;

using System;

using System.Collections.Generic;


namespace Epsitec.Aider.Controllers.SetControllers
{
	[ControllerSubType (0)]
	internal sealed class SetAiderGroupViewController0 : SetViewController<AiderGroupEntity, AiderGroupParticipantEntity, AiderContactEntity>
	{
		public override string GetIcon()
		{
			return Res.Commands.Base.ShowAiderGroupMembersDisplay.Caption.Icon;
		}

		public override FormattedText GetTitle()
		{
			return Res.Commands.Base.ShowAiderGroupMembersDisplay.Caption.DefaultLabel;
		}

		public override Druid GetDisplayDataSetId()
		{
			return Res.CommandIds.Base.ShowAiderGroupMembersDisplay;
		}

		public override Druid GetPickDataSetId()
		{
			return Res.CommandIds.Base.ShowAiderGroupMembersPick;
		}

		public override bool? GetOverrideEnableCreate()
		{
			return this.EnableButtons ();
		}

		public override bool? GetOverrideEnableDelete()
		{
			return this.EnableButtons ();
		}

		private bool EnableButtons()
		{
			return this.Entity.CanBeEditedByCurrentUser ();
		}

		protected override void SetupDisplayDataSetAccessor(AiderGroupEntity entity, DataSetAccessor dataSetAccessor)
		{
			// Here we add a condition that ensures that we will display only the
			// AiderGroupParticipantEntity whose group is the current entity and that are valid
			// today.

			dataSetAccessor.Customizer = (dataContext, request, example) =>
			{
				var participation = (AiderGroupParticipantEntity) example;

				request.AddCondition (dataContext, participation, x => x.Group == entity);
				AiderGroupParticipantEntity.AddCurrentCondition (dataContext, request, participation);
			};
		}

		protected override void SetupPickDataSetAccessor(AiderGroupEntity entity, DataSetAccessor dataSetAccessor)
		{
			// Here it would be nice not to display the contacts that are already in the group but
			// we can't do it for now because of the limitations in the DataLayer requests. We would
			// require requests with multiple roots and we don't have them yet.
		}

		protected override void AddItems(IEnumerable<AiderContactEntity> entitiesToAdd)
		{
			var context = this.BusinessContext;
			var group   = this.Entity;

			foreach (var contact in entitiesToAdd)
			{
				var member = contact.Person;

				//	@PA: handle legal persons too...

				if ((member.IsNotNull ()) && 
					(member.IsNotMemberOf (group)))
				{
					var what = new Participation
					{
						Group = group,
						Person = member,
						LegalPerson = contact.LegalPerson,
						Contact = contact,
					};

					AiderGroupParticipantEntity.StartParticipation (context, what, Date.Today, FormattedText.Empty);
				}
			}
		}

		protected override void RemoveItems(IEnumerable<AiderGroupParticipantEntity> entitiesToRemove)
		{
			foreach (var entity in entitiesToRemove)
			{
				AiderGroupParticipantEntity.StopParticipation (entity, Date.Today);
			}
		}
	}
}

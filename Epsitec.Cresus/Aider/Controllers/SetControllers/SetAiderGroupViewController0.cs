using Epsitec.Aider.Entities;

using Epsitec.Common.Support;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.SetControllers;

using Epsitec.Cresus.Core.Data;

using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Loader;

using System;

using System.Collections.Generic;


namespace Epsitec.Aider.Controllers.SetControllers
{
	[ControllerSubType (0)]
	internal sealed class SetAiderGroupViewController0 : SetViewController<AiderGroupEntity, AiderGroupParticipantEntity, AiderPersonEntity>
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
			// Here we a a condition that ensures that we will only display the persons that are not
			// yet in the current group.

			dataSetAccessor.Customizer = (dataContext, request, example) =>
			{
				var participantExample = new AiderGroupParticipantEntity ();
				var personExample = new AiderPersonEntity ();

				participantExample.Person = personExample;

				var subRequest = new Request ()
				{
					RequestedEntity = personExample,
					RootEntity = participantExample,
				};

				subRequest.AddCondition (dataContext, participantExample, x => x.Group == entity);
				request.AddCondition (dataContext, (AiderPersonEntity) example, p => SqlMethods.IsNotInSet (p, subRequest));
			};
		}

		protected override void AddItems(IEnumerable<AiderPersonEntity> entitiesToAdd)
		{
			foreach (var entity in entitiesToAdd)
			{
				AiderGroupParticipantEntity.StartParticipation (this.BusinessContext, entity, this.Entity, Date.Today, "");
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

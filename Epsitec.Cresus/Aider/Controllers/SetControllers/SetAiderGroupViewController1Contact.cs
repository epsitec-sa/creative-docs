//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.SetControllers;

using Epsitec.Cresus.Core.Data;

using System.Collections.Generic;


namespace Epsitec.Aider.Controllers.SetControllers
{
	[ControllerSubType (1)]
	internal sealed class SetAiderGroupViewController1Contact : SetViewController<AiderGroupEntity, AiderContactEntity, AiderContactEntity>
	{
		public override string GetIcon()
		{
			return Res.Commands.Base.ShowAiderGroupAndSubGroupMembersDisplay.Caption.Icon;
		}

		public override FormattedText GetTitle()
		{
			return Res.Commands.Base.ShowAiderGroupAndSubGroupMembersDisplay.Caption.DefaultLabel;
		}

		public override Druid GetDisplayDataSetId()
		{
			return Res.CommandIds.Base.ShowAiderGroupAndSubGroupMembersDisplay;
		}

		public override Druid GetPickDataSetId()
		{
			// We return a dummy value as it will never be used.

			return Res.CommandIds.Base.ShowAiderGroupAndSubGroupMembersDisplay;
		}

		protected override void SetupDisplayDataSetAccessor(AiderGroupEntity entity, DataSetAccessor dataSetAccessor)
		{
			dataSetAccessor.Customizers.Add ((dataContext, request, example) =>
			{
				var participation = new AiderGroupParticipantEntity ()
				{
				};

				AiderGroupParticipantEntity.AddGroupAndSubGroupMemberCondition (dataContext, request, participation, entity);
				AiderGroupParticipantEntity.AddCurrentCondition (dataContext, request, participation);

				request.Distinct = true;

				request.RootEntity = participation;
				request.RequestedEntity = participation;
			});
		}

		protected override void SetupPickDataSetAccessor(AiderGroupEntity entity, DataSetAccessor dataSetAccessor)
		{
			throw new System.NotImplementedException ();
		}

		protected override void AddItems(IEnumerable<AiderContactEntity> entitiesToAdd)
		{
			throw new System.NotImplementedException ();
		}

		protected override void RemoveItems(IEnumerable<AiderContactEntity> entitiesToRemove)
		{
			throw new System.NotImplementedException ();
		}
	}
}

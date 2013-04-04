﻿using Epsitec.Aider.Entities;

using Epsitec.Common.Support;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.SetControllers;

using Epsitec.Cresus.Core.Data;

using System;

using System.Collections.Generic;


namespace Epsitec.Aider.Controllers.SetControllers
{
	[ControllerSubType (1)]
	internal sealed class SetAiderGroupViewController1 : SetViewController<AiderGroupEntity, AiderGroupParticipantEntity, AiderGroupParticipantEntity>
	{
		public override string GetIcon()
		{
			return Res.Commands.Base.ShowAiderGroupFunctionMembersDisplay.Caption.Icon;
		}

		public override FormattedText GetTitle()
		{
			return Res.Commands.Base.ShowAiderGroupFunctionMembersDisplay.Caption.DefaultLabel;
		}

		public override Druid GetDisplayDataSetId()
		{
			return Res.CommandIds.Base.ShowAiderGroupFunctionMembersDisplay;
		}

		public override Druid GetPickDataSetId()
		{
			// We return a dummy value as it will never be used.

			return Res.CommandIds.Base.ShowAiderGroupFunctionMembersDisplay;
		}

		protected override void SetupDisplayDataSetAccessor(AiderGroupEntity entity, DataSetAccessor dataSetAccessor)
		{
			dataSetAccessor.Customizers.Add ((dataContext, request, example) =>
			{
				var participation = (AiderGroupParticipantEntity) example;

				AiderGroupParticipantEntity.AddFunctionMemberCondition (dataContext, request, participation, entity);
				AiderGroupParticipantEntity.AddCurrentCondition (dataContext, request, participation);
			});
		}

		protected override void SetupPickDataSetAccessor(AiderGroupEntity entity, DataSetAccessor dataSetAccessor)
		{
			throw new NotImplementedException ();
		}

		protected override void AddItems(IEnumerable<AiderGroupParticipantEntity> entitiesToAdd)
		{
			throw new NotImplementedException ();
		}

		protected override void RemoveItems(IEnumerable<AiderGroupParticipantEntity> entitiesToRemove)
		{
			throw new NotImplementedException ();
		}
	}
}

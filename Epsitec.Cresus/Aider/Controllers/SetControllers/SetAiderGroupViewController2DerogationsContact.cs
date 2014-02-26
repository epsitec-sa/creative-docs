﻿//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	[ControllerSubType (2)]
	internal sealed class SetAiderGroupViewController2DerogationsContact : SetViewController<AiderGroupEntity, AiderContactEntity, AiderContactEntity>
	{
		public override string GetIcon()
		{
			return Res.Commands.Base.ShowAiderGroupDerogationsMembersDisplay.Caption.Icon;
		}

		public override FormattedText GetTitle()
		{
			return Res.Commands.Base.ShowAiderGroupDerogationsMembersDisplay.Caption.DefaultLabel;
		}

		public override Druid GetDisplayDataSetId()
		{
			return Res.CommandIds.Base.ShowAiderGroupDerogationsMembersDisplay;
		}

		public override Druid GetPickDataSetId()
		{
			// We return a dummy value as it will never be used.

			return Res.CommandIds.Base.ShowAiderGroupDerogationsMembersDisplay;
		}

		protected override void SetupDisplayDataSetAccessor(AiderGroupEntity entity, DataSetAccessor dataSetAccessor)
		{
			dataSetAccessor.Customizers.Add ((dataContext, request, example) =>
			{
				var contact = (AiderContactEntity) example;
				var participation = new AiderGroupParticipantEntity ()
				{
					Contact = contact,
				};

				AiderGroupParticipantEntity.AddGroupAndSubGroupMemberCondition (dataContext, request, participation, entity);
				AiderGroupParticipantEntity.AddCurrentCondition (dataContext, request, participation);

				request.Distinct = true;

				request.RootEntity = participation;
				request.RequestedEntity = contact;
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
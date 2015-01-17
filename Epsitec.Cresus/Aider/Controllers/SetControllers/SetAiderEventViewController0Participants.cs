//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Enumerations;
using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.SetControllers;
using Epsitec.Cresus.Core.Data;

using Epsitec.Cresus.DataLayer.Expressions;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Metadata;
using Epsitec.Cresus.Core.Library.Settings;


namespace Epsitec.Aider.Controllers.SetControllers
{
	[ControllerSubType (0)]
	internal sealed class SetAiderEventViewController0Participants : SetViewController<AiderEventEntity, AiderEventParticipantEntity, AiderPersonEntity>
	{
		public override string GetIcon()
		{
			return Res.Commands.Base.ShowAiderEventParticipants.Caption.Icon;
		}

		public override FormattedText GetTitle()
		{
			return Res.Commands.Base.ShowAiderEventParticipants.Caption.DefaultLabel;
		}

		public override Druid GetDisplayDataSetId()
		{
			return Res.CommandIds.Base.ShowAiderEventParticipants;
		}

		public override Druid GetPickDataSetId()
		{
			return Res.CommandIds.Base.ShowAiderPerson;
		}

		protected override void SetupDisplayDataSetAccessor(AiderEventEntity entity, DataSetAccessor dataSetAccessor)
		{
			dataSetAccessor.Customizers.Add ((dataContext, request, example) =>
			{
				var participation = (AiderEventParticipantEntity) example;
			
				request.AddCondition (dataContext, participation, x => x.Event == entity);	
			});
		}

		protected override void SetupPickDataSetAccessor(AiderEventEntity entity, DataSetAccessor dataSetAccessor)
		{
			
		}

		protected override void AddItems(IEnumerable<AiderPersonEntity> entitiesToAdd)
		{
	
		}

		protected override void RemoveItems(IEnumerable<AiderEventParticipantEntity> entitiesToRemove)
		{
		
		}
	}
}

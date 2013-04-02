using Epsitec.Aider.Entities;

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
	internal sealed class SetAiderGroupViewController1 : SetViewController<AiderGroupEntity, AiderPersonEntity, AiderPersonEntity>
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
			// Here we do something quite nasty, as we change the requested entity and the root
			// entity from the request. For now it works, so let's hope it will still work in the
			// future.

			dataSetAccessor.Customizer = (dataContext, request, example) =>
			{
				var person = (AiderPersonEntity) example;
				var participation = new AiderGroupParticipantEntity ()
				{
					Person = person,
				};

				AiderGroupParticipantEntity.AddFunctionMemberCondition (dataContext, request, participation, entity);
				AiderGroupParticipantEntity.AddCurrentCondition (dataContext, request, participation);

				request.RootEntity = participation;
				request.RequestedEntity = person;
			};
		}

		protected override void SetupPickDataSetAccessor(AiderGroupEntity entity, DataSetAccessor dataSetAccessor)
		{
			throw new NotImplementedException ();
		}

		protected override void AddItems(IEnumerable<AiderPersonEntity> entitiesToAdd)
		{
			throw new NotImplementedException ();
		}

		protected override void RemoveItems(IEnumerable<AiderPersonEntity> entitiesToRemove)
		{
			throw new NotImplementedException ();
		}
	}
}

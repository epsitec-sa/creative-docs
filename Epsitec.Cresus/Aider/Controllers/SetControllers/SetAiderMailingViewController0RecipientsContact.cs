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


namespace Epsitec.Aider.Controllers.SetControllers
{
	[ControllerSubType (0)]
	internal sealed class SetAiderMailingViewController0RecipientsContact : SetViewController<AiderMailingEntity, AiderMailingParticipantEntity, AiderContactEntity>
	{
		public override string GetIcon()
		{
			return Res.Commands.Base.ShowAiderMailingParticipants.Caption.Icon;
		}

		public override FormattedText GetTitle()
		{
			return Res.Commands.Base.ShowAiderMailingParticipants.Caption.DefaultLabel;
		}

		public override Druid GetDisplayDataSetId()
		{
			return Res.CommandIds.Base.ShowAiderMailingParticipants;
		}

		public override Druid GetPickDataSetId()
		{
			return Res.CommandIds.Base.ShowAiderContact;
		}

		protected override void SetupDisplayDataSetAccessor(AiderMailingEntity entity, DataSetAccessor dataSetAccessor)
		{
			dataSetAccessor.Customizers.Add ((dataContext, request, example) =>
			{
				var participation = (AiderMailingParticipantEntity) example;
				request.AddCondition (dataContext, participation, x => x.Mailing== entity);
			});
		}

		protected override void SetupPickDataSetAccessor(AiderMailingEntity entity, DataSetAccessor dataSetAccessor)
		{
			var excludedContacts = this.Entity.Exclusions;
			var ids = new List<Constant> ();
			foreach (var contact in excludedContacts)
			{
				ids.Add (new Constant (this.BusinessContext.DataContext.GetNormalizedEntityKey (contact).Value.RowKey.Id.Value));
			}

			if (ids.Count () == 0)
			{
				ids.Add (new Constant (0));
			}

			dataSetAccessor.Customizers.Add ((dataContext, request, example) =>
			{
				request.Conditions.Add (
					new ValueSetComparison
					(
						InternalField.CreateId (example),
						SetComparator.NotIn,
						ids
					)
				);
			});
		}

		protected override void AddItems(IEnumerable<AiderContactEntity> entitiesToAdd)
		{
			this.Entity.AddContacts (this.BusinessContext, entitiesToAdd);
		}

		protected override void RemoveItems(IEnumerable<AiderMailingParticipantEntity> entitiesToRemove)
		{
			foreach (var participant in entitiesToRemove)
			{
				switch (participant.ParticipantType)
				{
					case MailingParticipantType.Group:
					case MailingParticipantType.GroupExtraction:
						this.Entity.ExcludeContact (this.BusinessContext, participant.Contact);
						break;

					case MailingParticipantType.Contact:
						this.Entity.RemoveContact (this.BusinessContext, participant.Contact);
						break;

					case MailingParticipantType.Household:
						this.Entity.RemoveHousehold (this.BusinessContext, participant.Contact, participant.Houshold);
						break;
				}
			}
		}
	}
}

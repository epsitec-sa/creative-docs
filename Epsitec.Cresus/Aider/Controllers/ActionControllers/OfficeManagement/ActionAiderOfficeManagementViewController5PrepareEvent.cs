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
	[ControllerSubType (5)]
	public sealed class ActionAiderOfficeManagementViewController5PrepareEvent : ActionViewController<AiderOfficeManagementEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Préparer un acte ecclésiastique");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<AiderOfficeManagementEntity, EventType, AiderTownEntity,EventPlaceType, string, Date> (this.Execute);
		}

		protected override void GetForm(ActionBrick<AiderOfficeManagementEntity, SimpleBrick<AiderOfficeManagementEntity>> form)
		{
			form
				.Title ("Préparation d'un acte")
				.Field<AiderOfficeManagementEntity> ()
					.Title ("Paroisse")
					.InitialValue (this.Entity)
				.End ()
				.Field<EventType> ()
					.Title ("Type de célébration")
				.End ()
				.Field<AiderTownEntity> ()
					.Title ("Lieu de la célébration")
					.InitialValue (this.Entity.OfficeMainContact.Address.Town)
				.End ()
				.Field<EventPlaceType> ()
					.Title ("Précision sur le lieu")
					.InitialValue (EventPlaceType.Church)
				.End ()
				.Field<string> ()
					.Title ("Désignation du lieu")
				.End ()
				.Field<Date> ()
					.Title ("Date de la célébration")
					.InitialValue (Date.Today)
				.End ()
			.End ();
		}

		private void Execute(
			AiderOfficeManagementEntity office, 
			EventType type,
			AiderTownEntity town,
			EventPlaceType placeType,
			string placeDescription,
			Date celebrationDate)
		{
			AiderEventEntity.Create (
				this.BusinessContext, 
				type,
				office,
				town,
				placeType,
				placeDescription,
				celebrationDate);
		}
	}
}

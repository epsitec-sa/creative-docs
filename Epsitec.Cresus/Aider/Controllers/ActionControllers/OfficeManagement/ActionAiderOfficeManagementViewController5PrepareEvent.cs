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
using Epsitec.Cresus.Core.Business.UserManagement;

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
			return ActionExecutor.Create<EventType, EventKind, AiderEventPlaceEntity, AiderTownEntity, Date> (this.Execute);
		}

		protected override void GetForm(ActionBrick<AiderOfficeManagementEntity, SimpleBrick<AiderOfficeManagementEntity>> form)
		{
			var currentUser     = UserManager.Current.AuthenticatedUser;
			var favorites       = AiderTownEntity.GetTownFavoritesByUserScope (this.BusinessContext, currentUser as AiderUserEntity);
			var favoritesPlaces = AiderOfficeManagementEntity.GetOfficeEventPlaces (this.BusinessContext, this.Entity);

			form
				.Title ("Préparation d'un acte")
				.Field<EventType> ()
					.Title ("Registre")
				.End ()
				.Field<EventKind> ()
					.Title ("Type d'événement")
					.InitialValue (EventKind.None)
				.End ()
				.Field<AiderEventPlaceEntity> ()
					.Title ("Lieu de la célébration")
					.WithFavorites (favoritesPlaces)
				.End ()
				.Field<AiderTownEntity> ()
					.Title ("Localité")
					.WithFavorites (favorites)
				.End ()
				.Field<Date> ()
					.Title ("Date de la célébration")
					.InitialValue (Date.Today)
				.End ()
			.End ();
		}

		private void Execute(
			EventType type,
			EventKind kind,
			AiderEventPlaceEntity place,
			AiderTownEntity town,
			Date celebrationDate)
		{
			if (type == null)
			{
				throw new BusinessRuleException ("Il faut choisir un registre");
			}

			if (place.IsNull ())
			{
				throw new BusinessRuleException ("Il faut choisir un lieu de célébration");
			}

			if (town.IsNull ())
			{
				throw new BusinessRuleException ("Il faut choisir la localité");
			}

			AiderEventEntity.Create (
				this.BusinessContext, 
				type,
				kind,
				this.Entity,
				town,
				place,
				celebrationDate);
		}
	}
}

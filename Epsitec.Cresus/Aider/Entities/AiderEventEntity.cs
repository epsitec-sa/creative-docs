//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP,

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using System.Linq;
using System.Collections.Generic;
using Epsitec.Cresus.DataLayer.Context;

namespace Epsitec.Aider.Entities
{
	public partial class AiderEventEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Type);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Type);
		}
		
		public static AiderEventEntity Create(
			BusinessContext context,
			Enumerations.EventType type,
			AiderOfficeManagementEntity office,
			AiderTownEntity town,
			Enumerations.EventPlaceType placeType,
			string placeDescription,
			Date celebrationDate)
		{
			var newEvent = context.CreateAndRegisterEntity<AiderEventEntity> ();
			newEvent.Type = type;
			newEvent.State = Enumerations.EventState.InPreparation;
					
			newEvent.Office = office;
			newEvent.Town = town;
			newEvent.PlaceType = placeType;
			newEvent.PlaceName = placeDescription;

			newEvent.Date = celebrationDate;
			return newEvent;
		}

		public void Delete(BusinessContext context)
		{
			context.DeleteEntity (this);
		}
	}
}


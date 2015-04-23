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
			var type = this.GetTypeCaption ();
			var place = this.GetPlaceText ();
			var actors = this.GetMainActors ().Select (p => p.GetDisplayName ()).Join ("\n");
			return TextFormatter.FormatText (type + "\n"+ actors + "\n" + place + "\n" + this.Date + "\n" + this.Description);
		}

		public override FormattedText GetCompactSummary()
		{
			var type = this.GetTypeCaption (); 
			return TextFormatter.FormatText (type + "\n" + this.Date);
		}

		public FormattedText GetParticipantsSummary()
		{
			var lines = this.GetParticipations ().Select (
				p => p.Person.GetDisplayName () + "\n"
			);
			return TextFormatter.FormatText (lines);
		}

		public AiderEventParticipantEntity GetParticipantByRole (Enumerations.EventParticipantRole role)
		{
			return this.GetParticipations ()
				.Where (p => p.Role == role)
				.FirstOrDefault ();
		}
		
		public static AiderEventEntity Create(
			BusinessContext context,
			Enumerations.EventType type,
			AiderOfficeManagementEntity office,
			AiderTownEntity town,
			AiderEventPlaceEntity place,
			Date celebrationDate)
		{
			var newEvent = context.CreateAndRegisterEntity<AiderEventEntity> ();
			newEvent.Type = type;
			newEvent.State = Enumerations.EventState.InPreparation;
					
			newEvent.Office = office;
			newEvent.Town = town;
			newEvent.Place = place;

			newEvent.Date = celebrationDate;
			return newEvent;
		}

		public void Delete(BusinessContext context)
		{
			foreach (var participant in this.Participants)
			{
				participant.Delete (context);
			}

			context.DeleteEntity (this);
		}

		public List<AiderPersonEntity> GetMainActors ()
		{
			List<AiderPersonEntity> actors = new List<AiderPersonEntity> ();
			switch (this.Type)
			{
			case Enumerations.EventType.Baptism:
				this.TryAddActorsWithRole (actors, Enumerations.EventParticipantRole.ChildBatise);
				break;
			case Enumerations.EventType.Blessing:
				this.TryAddActorsWithRole (actors, Enumerations.EventParticipantRole.BlessedChild);
				break;
			case Enumerations.EventType.CelebrationRegisteredPartners:
				this.TryAddActorsWithRole (actors, Enumerations.EventParticipantRole.Husband);
				this.TryAddActorsWithRole (actors, Enumerations.EventParticipantRole.Spouse);
				break;

			case Enumerations.EventType.Confirmation:
				this.TryAddActorsWithRole (actors, Enumerations.EventParticipantRole.Confirmant);
				break;
			case Enumerations.EventType.EndOfCatechism:
				this.TryAddActorsWithRole (actors, Enumerations.EventParticipantRole.Catechumen);
				break;
			case Enumerations.EventType.FuneralService:
				this.TryAddActorsWithRole (actors, Enumerations.EventParticipantRole.DeceasedPerson);
				break;
			case Enumerations.EventType.Marriage:
				this.TryAddActorsWithRole (actors, Enumerations.EventParticipantRole.Husband);
				this.TryAddActorsWithRole (actors, Enumerations.EventParticipantRole.Spouse);
				break;
			case Enumerations.EventType.None:
				break;
			}

			return actors;
		}

		public int CountRole (Enumerations.EventParticipantRole role)
		{
			var result = this.Participants.Count (p => p.Role == role);
			return result;
		}

		public void RemoveParticipant (BusinessContext context, AiderEventParticipantEntity participant)
		{
			participant.Delete (context);
		}

		partial void GetParticipants(ref IList<AiderEventParticipantEntity> value)
		{
			value = this.GetParticipations ().AsReadOnlyCollection ();
		}

		private void TryAddActorWithRole(List<AiderPersonEntity> actors, Enumerations.EventParticipantRole role)
		{
			var participant = this.Participants
										.SingleOrDefault (p => p.Role == role);
			if (participant.IsNotNull ()) {
				actors.Add (participant.Person);
			}
		}

		private void TryAddActorsWithRole(List<AiderPersonEntity> actors, Enumerations.EventParticipantRole role)
		{
			var participants = this.Participants
										.Where (p => p.Role == role).Select (p => p.Person);
			if (participants.Any ())
			{
				actors.AddRange (participants);
			}
		}

		private string GetTypeCaption ()
		{
			return Res.Types.Enum.EventType.FindValueFromEnumValue (this.Type).Caption.DefaultLabel; 
		}

		private string GetPlaceText()
		{
			var placeType = Res.Types.Enum.EventPlaceType.FindValueFromEnumValue (this.PlaceType).Caption.DefaultLabel;
			return placeType + " " + this.PlaceName;
		}

		private IList<AiderEventParticipantEntity> GetParticipations()
		{
			if (this.participants == null)
			{
				this.participants = this.ExecuteWithDataContext (d => this.FindParticipations (d), () => new List<AiderEventParticipantEntity> ());
			}

			return this.participants;
		}

		private IList<AiderEventParticipantEntity> FindParticipations(DataContext dataContext)
		{
			var example = new AiderEventParticipantEntity
			{
				Event = this
			};

			return dataContext.GetByExample (example)
							  .ToList ();
		}

		private IList<AiderEventParticipantEntity>			participants;
	}
}


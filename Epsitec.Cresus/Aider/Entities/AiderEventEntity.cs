//	Copyright � 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
			var lines = this.GetParticipations ()
				.Where (p => p.IsExternal == false)
				.Select (
				p => p.Person.GetDisplayName () + "\n"
			);
			var others = this.GetParticipations ()
				.Where (p => p.IsExternal == true)
				.Select (
				p => p.FirstNameEx + " " + p.LastNameEx + "\n"
			);
			lines.Concat (others);
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

		public static string FindNextNumber (BusinessContext context, Enumerations.EventType type)
		{
			var nextEvent  = 1;
			var eventStyle = new AiderEventEntity ()
			{
				Type = type,
				State = Enumerations.EventState.Validated
			};
			var example   = new AiderEventOfficeReportEntity ()
			{
				Event = eventStyle
			};

			var reports        = context.GetByExample <AiderEventOfficeReportEntity> (example);
			var thisYearEvents = reports.Where (r => r.Event.Date.Value.Year == System.DateTime.Now.Year);
			if (thisYearEvents.Any ())
			{
				nextEvent = thisYearEvents.Max (r => System.Convert.ToInt32 (r.EventNumber.Split ('/')[1])) + 1;
			}

			return System.DateTime.Now.Year.ToString () + "/" + nextEvent.ToString ();
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
			var actors = new List<AiderPersonEntity> ();
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

		public string GetRegitryName()
		{
			switch (this.Type)
			{
				case Enumerations.EventType.Baptism:
					return "Registre des bapt�mes";
				case Enumerations.EventType.Blessing:
					return "Registre des b�n�dictions";
				case Enumerations.EventType.CelebrationRegisteredPartners:
					return "Registre des c�l�brations pour partenaires enregistr�s";
				case Enumerations.EventType.Confirmation:
					return "Registre des confirmations";
				case Enumerations.EventType.EndOfCatechism:
					return "Registre des fins de cat�chisme";
				case Enumerations.EventType.FuneralService:
					return "Registre des services fun�bres";
				case Enumerations.EventType.Marriage:
					return "Registre des b�n�dictions de mariage";
				case Enumerations.EventType.None:
				default:
					return "";
			}
		}

		public string GetRegitryActName()
		{
			switch (this.Type)
			{
				case Enumerations.EventType.Baptism:
					return "Bapt�me";
				case Enumerations.EventType.Blessing:
					return "B�n�diction";
				case Enumerations.EventType.CelebrationRegisteredPartners:
					return "Partenariat enregistr�";
				case Enumerations.EventType.Confirmation:
					return "Confirmation";
				case Enumerations.EventType.EndOfCatechism:
					return "Fin de cat�chisme";
				case Enumerations.EventType.FuneralService:
					return "Service fun�bre";
				case Enumerations.EventType.Marriage:
					return "Mariage";
				case Enumerations.EventType.None:
				default:
					return "";
			}
		}

		public AiderPersonEntity GetMinister ()
		{
			AiderPersonEntity minister;
			this.TryAddActorWithRole (out minister, Enumerations.EventParticipantRole.Minister);
			return minister;
		}

		public AiderPersonEntity GetActor (Enumerations.EventParticipantRole role)
		{
			AiderPersonEntity actor;
			this.TryAddActorWithRole (out actor, role);
			return actor;
		}

		public bool IsCurrentEventValid(out string error)
		{
			error = "";

			if (this.Office.ParishGroup.IsNoParish ())
			{
				error = "Cette gestion n'est pas li�e � une paroisse, aucun acte ne peut y �tre g�n�r�";
				return false;
			}

			if (this.Place.IsNull ())
			{
				error = "La lieu de c�l�bration n'est pas renseign�";
				return false;
			}

			if (this.Town.IsNull ())
			{
				error = "La localit� de c�l�bration n'est pas renseign�e";
				return false;
			}

			if (this.Date.Value == null)
			{
				error = "La date de c�l�bration n'est pas renseign�e";
				return false;
			}

			var minister   = new AiderPersonEntity ();
			if (!this.TryAddActorWithRole (out minister, Enumerations.EventParticipantRole.Minister))
			{
				error = "Il manque le ministre officiant";
				return false;
			}

			var main   = new AiderPersonEntity ();
			var actors = new List<AiderPersonEntity> ();
			switch (this.Type)
			{
				case Enumerations.EventType.Baptism:
					if (!this.TryAddActorWithRole (out main, Enumerations.EventParticipantRole.ChildBatise))
					{
						error = "Aucun enfant � baptis� dans les participants";
						return false;
					}
					if(!this.CheckActorValidity (main, out error))
					{
						return false;
					}
					break;
				case Enumerations.EventType.Blessing:
					if (!this.TryAddActorWithRole (out main, Enumerations.EventParticipantRole.BlessedChild))
					{
						error = "Aucune personne � b�nir";
						return false;
					}
					if(!this.CheckActorValidity (main, out error))
					{
						return false;
					}
					break;
				case Enumerations.EventType.Confirmation:
					if (this.Kind == null)
					{
						error = "Type de c�l�bration non renseign� (Rameaux etc.)";
						return false;
					}
					if (!this.TryAddActorsWithRole (actors, Enumerations.EventParticipantRole.Confirmant))
					{
						error = "Aucunes personnes � confirmer";
						return false;
					}
					foreach (var actor in actors)
					{
						if (!this.CheckActorValidity (actor, out error))
						{
							return false;
						}
					}
					break;
				case Enumerations.EventType.EndOfCatechism:
					if (!this.TryAddActorsWithRole (actors, Enumerations.EventParticipantRole.Catechumen))
					{
						error = "Aucunes personnes pour la fin de cat�.";
						return false;
					}
					foreach (var actor in actors)
					{
						if (!this.CheckActorValidity (actor, out error))
						{
							return false;
						}
					}
					break;
				case Enumerations.EventType.FuneralService:
					if (!this.TryAddActorWithRole (out main, Enumerations.EventParticipantRole.DeceasedPerson))
					{
						error = "Aucun d�funt";
						return false;
					}
					if (main.IsAlive)
					{
						error = "La personne est encore en vie dans AIDER";
						return false;
					}
					if(!this.CheckActorValidity (main, out error))
					{
						return false;
					}
					break;
				case Enumerations.EventType.CelebrationRegisteredPartners:
				case Enumerations.EventType.Marriage:
					if (!(this.TryAddActorsWithRole (actors, Enumerations.EventParticipantRole.Husband)
						&& this.TryAddActorsWithRole (actors, Enumerations.EventParticipantRole.Spouse))
					)
					{
						error = "Aucune personnes � c�l�brer";
					}
					foreach (var actor in actors)
					{
						if (!this.CheckActorValidity (actor, out error))
						{
							return false;
						}
					}	
					break;
				case Enumerations.EventType.None:
					break;
			}

			return true;
		}

		public bool CheckActorValidity(AiderPersonEntity main, out string error)
		{
			error = "";

			if (main.Age == null)
			{
				error = main.GetShortFullName () + ": date de naissance non renseign�e";
				return false;
			}

			if (main.IsDeceased)
			{
				if (!main.Contacts.Where (c => c.AddressType == Enumerations.AddressType.LastKnow).Any ())
				{
					error = main.GetShortFullName () + ": adresse non renseign�e";
					return false;
				}
			}
			else
			{
				if (main.IsGovernmentDefined)
				{
					if (main.eCH_Person.GetAddress ().IsNull ())
					{
						error = main.GetShortFullName () + ": adresse non renseign�e";
						return false;
					}
				}
				else
				{
					if (main.MainContact.GetAddress ().IsNull ())
					{
						error = main.GetShortFullName () + ": adresse non renseign�e";
						return false;
					}

					if (main.MainContact.GetAddress ().IsNull ())
					{
						error = main.GetShortFullName () + ": domicile de l'adresse non renseign�";
						return false;
					}
				}
			}

			return true;
		}

		public int CountRole(Enumerations.EventParticipantRole role)
		{
			var result = this.Participants.Count (p => p.Role == role);
			return result;
		}

		public void RemoveParticipant(BusinessContext context, AiderEventParticipantEntity participant)
		{
			participant.Delete (context);
		}

		partial void GetParticipants(ref IList<AiderEventParticipantEntity> value)
		{
			value = this.GetParticipations ().AsReadOnlyCollection ();
		}

		private bool TryAddActorWithRole(out AiderPersonEntity actor, Enumerations.EventParticipantRole role)
		{
			actor = null;
			var participant = this.Participants
										.SingleOrDefault (p => p.Role == role);
			if (participant.IsNotNull ()) {
				if (participant.IsExternal == false)
				{
					actor = participant.Person;
				}
				return true;
			}
			else
			{
				return false;
			}
		}

		private bool TryAddActorsWithRole(List<AiderPersonEntity> actors, Enumerations.EventParticipantRole role)
		{
			var participants = this.Participants
										.Where (p => p.Role == role);
			if (participants.Any ())
			{
				actors.AddRange (participants.Where (p => p.IsExternal == false).Select (p => p.Person));
				return true;
			}
			else
			{
				return false;
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


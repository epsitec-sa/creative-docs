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
using Epsitec.Common.Support;

namespace Epsitec.Aider.Entities
{
	public partial class AiderEventEntity
	{
		public override FormattedText GetSummary()
		{
			var type = this.GetTypeCaption ();
			var place = this.GetPlaceText ();
			var actors = this.GetMainActors ().Select (p => p.GetFullName ()).Join ("\n");
			return TextFormatter.FormatText (type + "\n"+ actors + "\n" + place + "\n" + this.Date + "\n" + this.Description);
		}

		public FormattedText GetTypeSummary()
		{
			var type = this.GetTypeCaption ();
			return TextFormatter.FormatText (type);
		}

		public FormattedText GetActSummary()
		{
			var when  = "le " + this.Date.Value.ToDateTime ().ToString ("dd MMMM yyyy") + "\n";
			var where = this.Place.Name + "\n"; 
			return TextFormatter.FormatText (when + where);
		}

		public override FormattedText GetCompactSummary()
		{
			var type = this.GetTypeCaption (); 
			return TextFormatter.FormatText (type + "\n" + this.Date);
		}

		public FormattedText GetParticipantsSummary()
		{
			var lines = this.GetParticipations ()
				.Select (
				p => p.GetFullName () + "\n"
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
			Enumerations.EventKind? kind,
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
			newEvent.Kind  = kind != null ? kind : Enumerations.EventKind.None;
			newEvent.Date = celebrationDate;
			newEvent.ParishGroupPathCache = office.ParishGroupPathCache;
			return newEvent;
		}

		public void Delete(BusinessContext context)
		{
			if (this.State == Enumerations.EventState.Validated)
			{
				throw new BusinessRuleException ("Impossible de supprimer un acte validé");
			}

			foreach (var participant in this.Participants)
			{
				participant.Delete (context);
			}

			if (this.Report.IsNotNull ())
			{
				context.DeleteEntity (this.Report);
			}

			context.DeleteEntity (this);
		}

		public List<AiderEventParticipantEntity> GetMainActors ()
		{
			var actors = new List<AiderEventParticipantEntity> ();
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
					return Res.Commands.Base.ShowAiderEventBaptims.Caption.DefaultLabel;
				case Enumerations.EventType.Blessing:
					return Res.Commands.Base.ShowAiderEventBlessing.Caption.DefaultLabel;
				case Enumerations.EventType.CelebrationRegisteredPartners:
					return Res.Commands.Base.ShowAiderEventCelebrationRegisteredPartners.Caption.DefaultLabel;
				case Enumerations.EventType.Confirmation:
					return Res.Commands.Base.ShowAiderEventConfirmation.Caption.DefaultLabel;
				case Enumerations.EventType.EndOfCatechism:
					return Res.Commands.Base.ShowAiderEventEndOfCatechism.Caption.DefaultLabel;
				case Enumerations.EventType.FuneralService:
					return Res.Commands.Base.ShowAiderEventFuneralService.Caption.DefaultLabel;
				case Enumerations.EventType.Marriage:
					return Res.Commands.Base.ShowAiderEventMarriage.Caption.DefaultLabel;
				case Enumerations.EventType.None:
				default:
					return "";
			}
		}

		public static string ResolveRegitryName(Enumerations.EventType type)
		{
			switch (type)
			{
				case Enumerations.EventType.Baptism:
					return Res.Commands.Base.ShowAiderEventBaptims.Caption.DefaultLabel;
				case Enumerations.EventType.Blessing:
					return Res.Commands.Base.ShowAiderEventBlessing.Caption.DefaultLabel;
				case Enumerations.EventType.CelebrationRegisteredPartners:
					return Res.Commands.Base.ShowAiderEventCelebrationRegisteredPartners.Caption.DefaultLabel;
				case Enumerations.EventType.Confirmation:
					return Res.Commands.Base.ShowAiderEventConfirmation.Caption.DefaultLabel;
				case Enumerations.EventType.EndOfCatechism:
					return Res.Commands.Base.ShowAiderEventEndOfCatechism.Caption.DefaultLabel;
				case Enumerations.EventType.FuneralService:
					return Res.Commands.Base.ShowAiderEventFuneralService.Caption.DefaultLabel;
				case Enumerations.EventType.Marriage:
					return Res.Commands.Base.ShowAiderEventMarriage.Caption.DefaultLabel;
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
					return "Baptême";
				case Enumerations.EventType.Blessing:
					return "Bénédiction";
				case Enumerations.EventType.CelebrationRegisteredPartners:
					return "Partenariat enregistré";
				case Enumerations.EventType.Confirmation:
					return "Confirmation";
				case Enumerations.EventType.EndOfCatechism:
					return "Bénédictions des catéchumènes ";
				case Enumerations.EventType.FuneralService:
					return "Service funèbre";
				case Enumerations.EventType.Marriage:
					return "Mariage";
				case Enumerations.EventType.None:
				default:
					return "";
			}
		}

		public List<AiderEventParticipantEntity> GetMinisters()
		{
			var ministers = new List<AiderEventParticipantEntity> ();
			this.TryAddActorsWithRole (ministers, Enumerations.EventParticipantRole.Minister);
			return ministers;
		}

		public AiderEventParticipantEntity GetActor (Enumerations.EventParticipantRole role)
		{
			AiderEventParticipantEntity actor;
			this.TryAddActorWithRole (out actor, role);
			return actor;
		}

		public string GetActorFullName(Enumerations.EventParticipantRole role)
		{
			var participant = this.Participants.Where (p => p.Role == role).FirstOrDefault ();
			if (participant.IsNotNull ())
			{
				return participant.GetFullName ();
			}
			else
			{
				return null;
			}
		}

		public Enumerations.PersonSex GetActorSex(Enumerations.EventParticipantRole role)
		{
			var participant = this.Participants.Where (p => p.Role == role).FirstOrDefault ();
			if (participant.IsNotNull ())
			{
				return participant.GetSex ();
			}
			else
			{
				return Enumerations.PersonSex.Unknown;
			}
		}

		public bool IsCurrentEventValid(out string error)
		{
			error = "";

			if (this.Participants.Any (p => p.Role == Enumerations.EventParticipantRole.None))
			{
				error = "Il manque un rôle pour un des participants";
				return false;
			}

			if (this.Office.ParishGroup.IsNoParish ())
			{
				error = "Cette gestion n'est pas liée à une paroisse, aucun acte ne peut y être généré";
				return false;
			}

			if (this.Place.IsNull ())
			{
				error = "La lieu de célébration n'est pas renseigné";
				return false;
			}

			if (this.Town.IsNull ())
			{
				error = "La localité de célébration n'est pas renseignée";
				return false;
			}

			if (this.Date.Value == null)
			{
				error = "La date de célébration n'est pas renseignée";
				return false;
			}

			var ministers   = new List<AiderEventParticipantEntity> ();
			if (!this.TryAddActorsWithRole (ministers, Enumerations.EventParticipantRole.Minister))
			{
				error = "Il manque un ministre officiant";
				return false;
			}

			var main   = new AiderEventParticipantEntity ();
			var actors = new List<AiderEventParticipantEntity> ();
			switch (this.Type)
			{
				case Enumerations.EventType.Baptism:
					if (!this.TryAddActorWithRole (out main, Enumerations.EventParticipantRole.ChildBatise))
					{
						error = "Aucun enfant à baptisé dans les participants";
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
						error = "Aucune personne à bénir";
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
						error = "Type de célébration non renseigné (Rameaux etc.)";
						return false;
					}
					if (!this.TryAddActorsWithRole (actors, Enumerations.EventParticipantRole.Confirmant))
					{
						error = "Aucunes personnes à confirmer";
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
						error = "Aucunes personnes pour la fin de caté.";
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
						error = "Aucun défunt";
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
						error = "Aucune personnes à célébrer";
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

		public bool CheckActorValidity(AiderEventParticipantEntity participant, out string error)
		{
			error = "";
			var main = participant.Person;

			if (participant.GetBirthDate ()== null)
			{
				error = main.GetFullName () + ": date de naissance non renseignée";
				return false;
			}

			if (participant.GetTown ().IsNullOrWhiteSpace ())
			{
				error = main.GetFullName () + ": adresse non renseignée";
				return false;
			}

			if (participant.GetSex () == Enumerations.PersonSex.Unknown)
			{
				error = main.GetFullName () + ": sexe non renseigné";
				return false;
			}

			if (participant.GetParishName ().IsNullOrWhiteSpace ())
			{
				error = main.GetFullName () + ": paroisse non renseignée";
				return false;
			}

			return true;
		}

		public int CountRole(Enumerations.EventParticipantRole role)
		{
			var result = this.Participants.Count (p => p.Role == role);
			return result;
		}

		/// <summary>
		/// Keep important data of participants on the participation
		/// </summary>
		public void ApplyParticipantsInfo()
		{
			foreach(var participant in this.participants)
			{
				participant.UpdateActDataFromModel ();
			}
		}

		public void RemoveParticipant(BusinessContext context, AiderEventParticipantEntity participant)
		{
			participant.Delete (context);
		}

		partial void GetParticipants(ref IList<AiderEventParticipantEntity> value)
		{
			value = this.GetParticipations ().AsReadOnlyCollection ();
		}

		public void BuildMainActorsSummary()
		{
			this.MainActorsSummary = this.GetMainActors ().Select (p => p.LastName + ", " + p.FirstName.Split (' ')[0]).Join (" / ");
		}

		private bool TryAddActorWithRole(out AiderEventParticipantEntity actor, Enumerations.EventParticipantRole role)
		{
			actor = null;
			var participant = this.Participants
										.SingleOrDefault (p => p.Role == role);
			if (participant.IsNotNull ()) 
			{
				actor = participant;
				return true;
			}
			else
			{
				return false;
			}
		}

		private bool TryAddActorsWithRole(List<AiderEventParticipantEntity> actors, Enumerations.EventParticipantRole role)
		{
			var participants = this.Participants
										.Where (p => p.Role == role);
			if (participants.Any ())
			{
				actors.AddRange (participants);
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
			if (this.Place.IsNotNull ())
			{
				return this.Place.Name;
			}
			else
			{
				return "Lieu de célébration non renseigné";
			}		
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


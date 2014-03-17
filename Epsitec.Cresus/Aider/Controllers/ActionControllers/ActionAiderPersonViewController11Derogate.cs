//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;

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

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (11)]
	public sealed class ActionAiderPersonViewController11Derogate : ActionViewController<AiderPersonEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Déroger vers...");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<AiderGroupEntity,Date> (this.Execute);
		}

		private void Execute(AiderGroupEntity destParish, Date date)
		{
			var person = this.Entity;
			var parishGroup = person.ParishGroup;

			if (destParish.IsNull ())
			{
				var message = "Vous n'avez pas sélectionné de paroisse.";
				throw new BusinessRuleException (message);
			}
			if (!destParish.IsParish ())
			{
				var message = "Le groupe sélectionné n'est pas une paroisse.";
				throw new BusinessRuleException (message);
			}

			if (parishGroup == destParish)
			{
				var message = "La personne est déjà associée à cette paroisse.";
				throw new BusinessRuleException (message);
			}

			System.Diagnostics.Trace.WriteLine ("Derogating from " + parishGroup.Name);

			var derogationInGroup = destParish.Subgroups.Single (g => g.GroupDef.Classification == Enumerations.GroupClassification.DerogationIn);		
			var derogationOutGroup = parishGroup.Subgroups.Single (g => g.GroupDef.Classification == Enumerations.GroupClassification.DerogationOut);

			var participationData = new List<ParticipationData> ();
			participationData.Add (new ParticipationData (person.MainContact));

			//Check if a previous derogation is in place ?
			if (person.HasDerogation)
			{
				//Yes, existing derogation in place:
				//Remove old derogation in
				var oldDerogationInGroup = parishGroup.Subgroups.Single (g => g.GroupDef.Classification == Enumerations.GroupClassification.DerogationIn);
				oldDerogationInGroup.RemoveParticipations (this.BusinessContext, oldDerogationInGroup.FindParticipations (this.BusinessContext, person.MainContact));

				//Check for a "return to home"
				if (person.GeoParishGroupPathCache == destParish.Path)
				{
					//Reset state
					person.ClearDerogation ();

					//Remove old derogation out
					var oldDerogationOutGroup = destParish.Subgroups.Single (g => g.GroupDef.Classification == Enumerations.GroupClassification.DerogationOut);
					oldDerogationOutGroup.RemoveParticipations (this.BusinessContext, oldDerogationOutGroup.FindParticipations (this.BusinessContext, person.MainContact));

					//Warn old derogated parish
					AiderPersonWarningEntity.Create (this.BusinessContext, person, person.ParishGroupPathCache,
						Enumerations.WarningType.ParishDeparture, "Fin de dérogation.");
					//Warn NewParish for return
					AiderPersonWarningEntity.Create (this.BusinessContext, person, destParish.Path,
						Enumerations.WarningType.ParishArrival, "Fin de dérogation.");
				}
				else
				{
					//Add derogation in participation
					derogationInGroup.AddParticipations (this.BusinessContext, participationData, date, new FormattedText ("Dérogation entrante"));

					//Warn old derogated parish
					AiderPersonWarningEntity.Create (this.BusinessContext, person, person.ParishGroupPathCache,
						Enumerations.WarningType.ParishDeparture, "Fin de dérogation.");
					
					//Warn NewParish
					AiderPersonWarningEntity.Create (this.BusinessContext, person, destParish.Path,
						Enumerations.WarningType.ParishArrival, "Personne dérogée en provenance de la\n" + person.ParishGroup.Name + ".");
					
					//Warn GeoParish for a Derogation Change
					AiderPersonWarningEntity.Create (this.BusinessContext, person, person.GeoParishGroupPathCache,
						Enumerations.WarningType.DerogationChange, "Changement de dérogation vers la\n" + destParish.Name + ".");
					
					this.CreateDerogationLetter (this.BusinessContext, destParish, parishGroup);
				}
			}
			else
			{
				//No, first derogation:
				//Backup initial parish cache
				person.GeoParishGroupPathCache = person.ParishGroupPathCache;

				//Add derogation out participation
				derogationOutGroup.AddParticipations (this.BusinessContext, participationData, date, new FormattedText ("Dérogation sortante"));
				//Warn GeoParish
				AiderPersonWarningEntity.Create (this.BusinessContext, person, person.GeoParishGroupPathCache,
					Enumerations.WarningType.ParishDeparture, "Personne dérogée vers la\n" + destParish.Name + ".");

				//Add derogation in participation
				derogationInGroup.AddParticipations (this.BusinessContext, participationData, date, new FormattedText ("Dérogation entrante"));
				
				//Warn NewParish
				AiderPersonWarningEntity.Create (this.BusinessContext, person, destParish.Path,
					Enumerations.WarningType.ParishArrival, "Personne dérogée en provenance de la\n" + person.ParishGroup.Name + ".");

				this.CreateDerogationLetter (this.BusinessContext, destParish, parishGroup);
				
			}

			//Remove parish participations
			parishGroup.RemoveParticipations (this.BusinessContext, parishGroup.FindParticipations (this.BusinessContext, person.MainContact));

			//Add participation to the destination parish
			destParish.AddParticipations (this.BusinessContext, participationData, date, null);
			
			//!Trigg business rules!
			person.ParishGroup = destParish;

			System.Diagnostics.Trace.WriteLine ("Derogated to " + destParish.Name);
		}

		protected override void GetForm(ActionBrick<AiderPersonEntity, SimpleBrick<AiderPersonEntity>> form)
		{
			form
				.Title ("Déroger vers...")
				.Field<AiderGroupEntity> ()
					.Title ("Paroisse")
					.WithSpecialField<AiderGroupSpecialField<AiderPersonEntity>> ()
					.InitialValue (this.Entity.GetGeoParishGroup (this.BusinessContext) ?? this.Entity.ParishGroup)
				.End ()
				.Field<Date> ()
					.Title ("Date de la dérogation")
					.InitialValue (Date.Today)
				.End ()
			.End ();
		}


		private void CreateDerogationLetter(BusinessContext businessContext, AiderGroupEntity destParish,AiderGroupEntity origineParish)
		{
			
			var office			= AiderOfficeManagementEntity.Find (businessContext, destParish);
			var recipient		= this.Entity.MainContact;
			var documentName	= "Confirmation dérogation " + recipient.DisplayName;

			var userManager		= AiderUserManager.Current;
			var aiderUser       = userManager.AuthenticatedUser;
			var sender		    = businessContext.GetLocalEntity (aiderUser.OfficeSender);

			if(sender.IsNull ())
			{
				var message = "Vous devez d'abord associer votre utilisateur à un secrétariat; la création de la dérogation est impossible.";
				throw new BusinessRuleException (message);
			}


			var greetings = "";
			
			if (this.Entity.eCH_Person.PersonSex == Enumerations.PersonSex.Male)
			{
				greetings = "Cher Monsieur,";
			}
			else
			{
				greetings = "Chère Madame,";
			}

			var content = string.Format (LetterTemplate, 
								greetings,
								destParish.Name,
								origineParish.Name,
								destParish.Name,
								aiderUser.Contact.Person.GetFullName ()
							);

			var letter = AiderOfficeLetterReportEntity.Create (businessContext, recipient, sender, documentName, content);

			this.BusinessContext.SaveChanges (LockingPolicy.ReleaseLock);
			letter.ProcessorUrl		= letter.BuildProcessorUrlForSender (this.BusinessContext, "officeletter", sender);
			this.BusinessContext.SaveChanges (LockingPolicy.ReleaseLock);

			var notificationManager = NotificationManager.GetCurrentNotificationManager ();
			notificationManager.Notify (aiderUser.LoginName, new NotificationMessage
			{
				Title	= "Dérogation effectuée",
				Body	= "La lettre à été ajoutée dans la gestion des documents de la " + destParish.Name,
				Dataset = Res.CommandIds.Base.ShowAiderOfficeManagement,
				EntityKey = this.BusinessContext.DataContext.GetNormalizedEntityKey(sender.Office).Value
			}
			, When.Now);

			EntityBag.Add (letter, "Document PDF");

			
		}

		private readonly string LetterTemplate = new System.Text.StringBuilder ()
												.Append ("<b>Votre dérogation</b><br/><br/>")
												.Append ("{0}<br/>")
												.Append ("Votre dérogation paroissiale a été bien enregistrée. Elle entre désormais en vigueur.<br/>")
												.Append ("Votre nouvelle paroisse officielle où vous bénéficiez du droit de vote et d'éligibilité ")
												.Append ("(= possibilité de délibérer en assemblée paroissiable, de voter, d'élire ou d'être élu) est désormais la")
												.Append ("<br/><br/><b>{1}</b><br/><br/>")
												.Append ("Vous avez perdu vos droits de vote et d'éligibilité dans la paroisse standard de votre domicile, à savoir la ")
												.Append ("{2}.<br/><br/>")
												.Append ("Au cas où vous viendrez à déménager, vous seriez automatiquement rattaché à la paroisse de votre <b>nouveau</b> domicile.")
												.Append ("La dérogation actuelle perdrait son effet. Vous auriez la possibilité de demander une nouvelle dérogation si vous l'estimiez important.")
												.Append ("<br/><br/>Nous vous souhaitons de riches expériences et un fructueux engagement dans votre nouvelle paroisse officielle ")
												.Append ("et vous adressons, nos fraternelles salutations.<br/><br/>")
												.Append ("le secrétariat de la {3}<br/><br/>")
												.Append ("signé : {4}")
												.ToString ();
		
	}
}

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
			var needDerogationLetter = false;
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

					needDerogationLetter = true;
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

				needDerogationLetter = true;			
			}

			//Remove parish participations
			parishGroup.RemoveParticipations (this.BusinessContext, parishGroup.FindParticipations (this.BusinessContext, person.MainContact));

			//Add participation to the destination parish
			destParish.AddParticipations (this.BusinessContext, participationData, date, FormattedText.Null);
			
			//!Trigg business rules!
			person.ParishGroup = destParish;

			System.Diagnostics.Trace.WriteLine ("Derogated to " + destParish.Name);

			if (needDerogationLetter)
			{
				var userManager		= AiderUserManager.Current;
				var aiderUser       = userManager.AuthenticatedUser;
				var sender		    = this.BusinessContext.GetLocalEntity (aiderUser.OfficeSender);

				var letter = this.CreateDerogationLetter (this.BusinessContext, sender, destParish, parishGroup);
				//SaveChanges for ID purpose: BuildProcessorUrl need the entity ID
				this.BusinessContext.SaveChanges (LockingPolicy.ReleaseLock);
				letter.ProcessorUrl		= letter.BuildProcessorUrlForSender (this.BusinessContext, "officeletter", sender);
				this.BusinessContext.SaveChanges (LockingPolicy.ReleaseLock);

				EntityBag.Add (letter, "Document PDF");	
			}

		}

		protected override void GetForm(ActionBrick<AiderPersonEntity, SimpleBrick<AiderPersonEntity>> form)
		{
			var userManager			= AiderUserManager.Current;
			var aiderUser			= userManager.AuthenticatedUser;
			var defaultDestParish	= this.BusinessContext.GetLocalEntity (aiderUser.Office.ParishGroup);

			form
				.Title ("Déroger vers...")
				.Field<AiderGroupEntity> ()
					.Title ("Paroisse")
					.WithSpecialField<AiderGroupSpecialField<AiderPersonEntity>> ()
					.InitialValue (this.Entity.GetGeoParishGroup (this.BusinessContext) ?? defaultDestParish)
				.End ()
				.Field<Date> ()
					.Title ("Date de la dérogation")
					.InitialValue (Date.Today)
				.End ()
			.End ();
		}


		private AiderOfficeLetterReportEntity CreateDerogationLetter(BusinessContext businessContext, AiderOfficeSenderEntity sender, AiderGroupEntity destParish, AiderGroupEntity origineParish)
		{
			
			var office			= AiderOfficeManagementEntity.Find (businessContext, destParish);
			var recipient		= this.Entity.MainContact;
			var documentName	= "Confirmation dérogation " + recipient.DisplayName;

			if(sender.IsNull ())
			{
				var message = "Vous devez d'abord associer votre utilisateur à un secrétariat; la création de la dérogation est impossible.";
				throw new BusinessRuleException (message);
			}

			if (CoreContext.HasExperimentalFeature ("OfficeManagement") == false)
			{
				throw new BusinessRuleException ("Cette fonction n'est pas encore disponible.");
			}

			var greetings = "";
			
			if (this.Entity.eCH_Person.PersonSex == Enumerations.PersonSex.Male)
			{
				greetings = "Monsieur";
			}
			else
			{
				greetings = "Madame";
			}

			string template = System.IO.File.ReadAllText (CoreContext.GetFileDepotPath ("assets", "template-letter-derogation.txt"), System.Text.Encoding.UTF8);

			var content = string.Format (template, 
								greetings,
								destParish.Name,
								origineParish.Name,
								destParish.Name,
								sender.OfficialContact.Person.GetFullName ()
							);

			return AiderOfficeLetterReportEntity.Create (businessContext, recipient, sender, documentName, content);	
		}
	}
}

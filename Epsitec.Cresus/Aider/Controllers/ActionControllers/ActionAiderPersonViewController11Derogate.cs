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
	[ControllerSubType (11)]
	public sealed class ActionAiderPersonViewController11Derogate : ActionViewController<AiderPersonEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Déroger vers...");
		}

		public override bool IsEnabled
		{
			get
			{
				var userManager = AiderUserManager.Current;
				var aiderUser   = userManager.AuthenticatedUser;

				return (aiderUser.Office.IsNotNull ())
					&& (aiderUser.OfficeSender.IsNotNull ());
			}
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<AiderGroupEntity,Date> (this.Execute);
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

		
		private void Execute(AiderGroupEntity derogationParishGroup, Date date)
		{
			var needDerogationLetter = false;
			var person = this.Entity;
			var currentParishGroup = person.ParishGroup;

			AiderDerogations.CheckPrerequisiteBeforeDerogate (person, currentParishGroup, derogationParishGroup);

			System.Diagnostics.Trace.WriteLine ("Derogating from " + currentParishGroup.Name);

			var derogationInGroup	= AiderDerogations.GetDerogationInForParishGroup (this.BusinessContext, derogationParishGroup);
			var derogationOutGroup	= AiderDerogations.GetDerogationOutForParishGroup(this.BusinessContext, currentParishGroup);

			//Check if a previous derogation is in place ?
			if (person.HasDerogation)
			{
				//Yes, existing derogation in place:
				//Remove old derogation in
				AiderDerogations.RemoveDerogationInParticipations (this.BusinessContext, currentParishGroup, person);

				//Check for a "return to home"
				if (person.GeoParishGroupPathCache == derogationParishGroup.Path)
				{
					//Reset state
					person.ClearDerogation ();

					//Remove old derogation out
					AiderDerogations.RemoveDerogationOutParticipations (this.BusinessContext, derogationParishGroup, person);

					AiderDerogations.WarnEndOfDerogation (this.BusinessContext, person, person.ParishGroupPathCache);
					AiderDerogations.WarnReturnToOriginalParish (this.BusinessContext, person, derogationParishGroup);
				}
				else
				{
					//Add derogation in participation
					AiderDerogations.AddParticipationToDerogationIn (this.BusinessContext, person, derogationInGroup, date);

					AiderDerogations.WarnEndOfDerogation (this.BusinessContext, person, person.ParishGroupPathCache);
					AiderDerogations.WarnArrivalInParish (this.BusinessContext, person, derogationParishGroup);
					//Warn GeoParish for a Derogation Change
					AiderPersonWarningEntity.Create (this.BusinessContext, person, person.GeoParishGroupPathCache,
						WarningType.DerogationChange, "Changement de dérogation vers la\n" + derogationParishGroup.Name + ".");

					needDerogationLetter = true;
				}
			}
			else
			{
				//No, first derogation:
				//Backup initial parish cache
				person.GeoParishGroupPathCache = person.ParishGroupPathCache;

				//	Add derogation out participation, but not if the person was in the
				//	'no parish' group before...

				if (derogationOutGroup != null)
				{
					AiderDerogations.AddParticipationToDerogationOut (this.BusinessContext, person, derogationOutGroup, date);

					AiderDerogations.WarnGeoParishAboutChange (this.BusinessContext, person, derogationParishGroup);
				}

				//Add derogation in participation
				AiderDerogations.AddParticipationToDerogationIn (this.BusinessContext, person, derogationInGroup, date);

				AiderDerogations.WarnArrivalInParish (this.BusinessContext, person, derogationParishGroup);

				needDerogationLetter = true;
			}

			//Remove parish participations
			AiderDerogations.RemoveParishGroupPartiticipations (this.BusinessContext, person, currentParishGroup);

			//Add participation to the destination parish
			AiderDerogations.AddParishGroupParticipations (this.BusinessContext, person, derogationParishGroup, date);

			//!Trigg business rules!
			person.ParishGroup = derogationParishGroup;

			System.Diagnostics.Trace.WriteLine ("Derogated to " + derogationParishGroup.Name);

			if (needDerogationLetter)
			{
				var userManager		= AiderUserManager.Current;
				var aiderUser       = userManager.AuthenticatedUser;
				var sender		    = this.BusinessContext.GetLocalEntity (aiderUser.OfficeSender);

				var letter = AiderDerogations.CreateDerogationLetter (this.BusinessContext,this.Entity, sender, derogationParishGroup, currentParishGroup);
				//SaveChanges for ID purpose: BuildProcessorUrl need the entity ID
				this.BusinessContext.SaveChanges (LockingPolicy.ReleaseLock);
				letter.ProcessorUrl		= letter.GetProcessorUrlForSender (this.BusinessContext, "officeletter", sender);
				this.BusinessContext.SaveChanges (LockingPolicy.ReleaseLock);

				EntityBag.Add (letter, "Document PDF");
			}

		}
	}
}

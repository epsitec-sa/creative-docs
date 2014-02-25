//	Copyright © 2013-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Aider.Controllers.SpecialFieldControllers;
using Epsitec.Cresus.Core.Business;

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
			if (!destParish.IsParish ())
			{
				var message = "Le groupe sélectionné n'est pas une paroisse";

				throw new BusinessRuleException (message);
			}


			var person = this.Entity;
			var parishGroup = person.ParishGroup;
			var derogationInGroup = destParish.Subgroups.Where (g => g.GroupDef.Classification == Enumerations.GroupClassification.DerogationIn).First ();		
			var derogationOutGroup = parishGroup.Subgroups.Where (g => g.GroupDef.Classification == Enumerations.GroupClassification.DerogationOut).First ();
			

			var contactAsList = new List<AiderContactEntity> ();
			contactAsList.Add (person.MainContact);

			//Check if a previous derogation is in place ?
			if (string.IsNullOrEmpty (person.GeoParishGroupPathCache))
			{			
				//No, first derogation:
				//Backup initial parish cache
				person.GeoParishGroupPathCache = person.ParishGroupPathCache;

				//Add derogation out participation
				derogationOutGroup.AddParticipations (this.BusinessContext, contactAsList.Select (c => new ParticipationData (c)), date, new FormattedText ("Dérogation sortante"));
				
				//Add derogation in participation
				derogationInGroup.AddParticipations (this.BusinessContext, contactAsList.Select (c => new ParticipationData (c)), date, new FormattedText ("Dérogation entrante"));
			}
			else
			{
				//Yes, existing derogation in place:
				//Remove old derogation in
				var oldDerogationInGroup = parishGroup.Subgroups.Where (g => g.GroupDef.Classification == Enumerations.GroupClassification.DerogationIn).First ();
				oldDerogationInGroup.RemoveParticipations (this.BusinessContext, oldDerogationInGroup.FindParticipations (this.BusinessContext).Where (p => p.Contact == person.MainContact));

				//Check for a "return to home"
				if (person.GeoParishGroupPathCache == destParish.Path)
				{
					//Reset state
					person.GeoParishGroupPathCache = "";

					//Remove old derogation out
					var oldDerogationOutGroup = destParish.Subgroups.Where (g => g.GroupDef.Classification == Enumerations.GroupClassification.DerogationOut).First ();
					oldDerogationOutGroup.RemoveParticipations (this.BusinessContext, oldDerogationOutGroup.FindParticipations (this.BusinessContext).Where (p => p.Contact == person.MainContact));
				}
				else
				{
					//Add derogation in participation
					derogationInGroup.AddParticipations (this.BusinessContext, contactAsList.Select (c => new ParticipationData (c)), date, new FormattedText ("Dérogation entrante"));
				}
			}

			//Remove parish participations
			parishGroup.RemoveParticipations (this.BusinessContext, parishGroup.FindParticipations (this.BusinessContext).Where (p => p.Contact == person.MainContact));

			//Add participation to the destination parish
			destParish.AddParticipations(this.BusinessContext,contactAsList.Select( c => new ParticipationData(c)),date,new FormattedText ("Dérogation"));
			
			//!Trigg business rules!
			person.ParishGroup = destParish;
		}

		protected override void GetForm(ActionBrick<AiderPersonEntity, SimpleBrick<AiderPersonEntity>> form)
		{
			form
				.Title ("Déroger vers...")
				.Field<AiderGroupEntity> ()
					.Title ("Paroisse")
					.WithSpecialField<AiderGroupSpecialField<AiderPersonEntity>> ()
				.End ()
				.Field<Date> ()
					.Title ("Date de validité de la dérogation")
					.InitialValue (Date.Today)
				.End ()
			.End ();
		}
	}
}


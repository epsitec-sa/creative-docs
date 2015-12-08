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
					.InitialValue (this.Entity.GetDerogationGeoParishGroup (this.BusinessContext) ?? defaultDestParish)
				.End ()
				.Field<Date> ()
					.Title ("Date de la dérogation")
					.InitialValue (Date.Today)
				.End ()
			.End ();
		}

		
		private void Execute(AiderGroupEntity derogationParishGroup, Date date)
		{		
			var person = this.Entity;
			var currentParishGroup = person.ParishGroup;
			var user   = AiderUserManager.Current.AuthenticatedUser;

			if (!user.CanDerogateTo (derogationParishGroup))
			{
				throw new BusinessRuleException ("Vos droits ne vous permettent pas de déroger vers cette paroisse");
			}

			System.Diagnostics.Trace.WriteLine ("Derogating from " + currentParishGroup.Name);

			var needDerogationLetter = AiderDerogations.DerogatePerson (this.BusinessContext, person, currentParishGroup, derogationParishGroup, date);

			System.Diagnostics.Trace.WriteLine ("Derogated to " + derogationParishGroup.Name);

			if (needDerogationLetter)
			{
				var userManager		= AiderUserManager.Current;
				var aiderUser       = userManager.AuthenticatedUser;
				var sender		    = this.BusinessContext.GetLocalEntity (aiderUser.OfficeSender);

				var letter = AiderDerogations.CreateDerogationLetter (this.BusinessContext, this.Entity, sender, aiderUser, derogationParishGroup, currentParishGroup);
				//SaveChanges for ID purpose: BuildProcessorUrl need the entity ID
				this.BusinessContext.SaveChanges (LockingPolicy.ReleaseLock);
				letter.ProcessorUrl		= letter.GetProcessorUrlForSender (this.BusinessContext, "officeletter", sender);
				this.BusinessContext.SaveChanges (LockingPolicy.ReleaseLock);

				EntityBag.Add (letter, "Document PDF");
			}

		}
	}
}

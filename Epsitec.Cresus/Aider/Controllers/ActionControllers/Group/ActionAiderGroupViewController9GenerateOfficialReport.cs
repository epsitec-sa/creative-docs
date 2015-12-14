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
using Epsitec.Cresus.Core.Library;

using Epsitec.Aider.Override;
using Epsitec.Aider.Reporting;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (9)]
	public sealed class ActionAiderGroupViewController9GenerateOfficialReport : ActionViewController<AiderGroupEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Générer un extrait officiel");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<string, Date, AiderTownEntity> (this.Execute);
		}

		private void Execute(string title, Date date, AiderTownEntity place)
		{
			var documentName	= title;
			var group			= this.Entity;
			var userManager		= AiderUserManager.Current;
			var aiderUser       = userManager.AuthenticatedUser;
			var sender		    = this.BusinessContext.GetLocalEntity (aiderUser.OfficeSender);

			if(sender.IsNull ())
			{
				throw new BusinessRuleException ("Vous n'êtes pas définit en tant que expéditeur");
			}

			var title1 = TextFormatter.FormatText ("Assemblée paroissiale du", date.ToShortDateString (), "à~", place.IsNull () ? null : place.Name);
			var title2 = TextFormatter.FormatText (title);

			var content = new FormattedContent (title1, title2);
			
			var report = AiderOfficeGroupParticipantReportEntity.Create (this.BusinessContext, group, sender, documentName, title, "template-group-report", content);

			//SaveChanges for ID purpose: BuildProcessorUrl need the entity ID
			this.BusinessContext.SaveChanges (LockingPolicy.ReleaseLock);
			report.ProcessorUrl		= report.GetProcessorUrlForSender (this.BusinessContext, "officegroup", sender);
			this.BusinessContext.SaveChanges (LockingPolicy.ReleaseLock);

			EntityBag.Add (report, "Document PDF");
		}

		protected override void GetForm(ActionBrick<AiderGroupEntity, SimpleBrick<AiderGroupEntity>> form)
		{
			var currentUser = this.BusinessContext.GetLocalEntity(AiderUserManager.Current.AuthenticatedUser);
			var favorites   = AiderTownEntity.GetTownFavoritesByUserScope (this.BusinessContext, currentUser as AiderUserEntity);
			form
				.Title ("Production d'un extrait officiel")
				.Field<string> ()
					.Title ("Titre")
					.InitialValue (this.Entity.Name)
				.End ()
				.Field<Date> ()
					.Title ("Date de l'assemblée")
					.InitialValue (Date.Today)
				.End ()
				.Field<AiderTownEntity> ()
					.Title ("Lieu de l'assemblée")
					.InitialValue (currentUser.Office.OfficeMainContact.Address.Town)
					.WithFavorites (favorites)
				.End ()
			.End ();
		}
	}
}

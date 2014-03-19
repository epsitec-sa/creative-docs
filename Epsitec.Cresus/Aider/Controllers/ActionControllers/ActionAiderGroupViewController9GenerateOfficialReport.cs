//	Copyright � 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Aider.Override;
using System.Text;
using Epsitec.Cresus.Core.Library;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (9)]
	public sealed class ActionAiderGroupViewController9GenerateOfficialReport : ActionViewController<AiderGroupEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("G�n�rer un extrait officiel");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<string, System.DateTime, AiderTownEntity> (this.Execute);
		}

		private void Execute(string title,System.DateTime date, AiderTownEntity place)
		{
			var documentName	= title;
			var group			= this.Entity;
			var userManager		= AiderUserManager.Current;
			var aiderUser       = userManager.AuthenticatedUser;
			var sender		    = this.BusinessContext.GetLocalEntity (aiderUser.OfficeSender);

			var content			= new StringBuilder ()
										.Append ("<br/><br/>")
										.Append ("Assembl�e paroissiale du ")
										.Append (date.ToString("d MMM yyyy"))
										.Append (" � ")
										.Append (place.Name)
										.Append ("<br/><br/>")
										.Append ("<tab/><b>" + title + "</b>")
										.Append ("<br/><br/>")
										.ToString ();

			var report = AiderOfficeGroupParticipantReportEntity.Create (this.BusinessContext, group, sender, documentName, title, content);

			//SaveChanges for ID purpose: BuildProcessorUrl need the entity ID
			this.BusinessContext.SaveChanges (LockingPolicy.ReleaseLock);
			report.ProcessorUrl		= report.GetProcessorUrlForSender (this.BusinessContext, "officegroup", sender);
			this.BusinessContext.SaveChanges (LockingPolicy.ReleaseLock);

			EntityBag.Add (report, "Document PDF");
		}

		protected override void GetForm(ActionBrick<AiderGroupEntity, SimpleBrick<AiderGroupEntity>> form)
		{
			var currentUser = this.BusinessContext.GetLocalEntity(AiderUserManager.Current.AuthenticatedUser);

			form
				.Title ("Production d'un extrait officiel")
				.Field<string> ()
					.Title ("Titre")
					.InitialValue (this.Entity.Name)
				.End ()
				.Field<System.DateTime> ()
					.Title ("Date de l'assembl�e")
					.InitialValue (System.DateTime.Now)
				.End ()
				.Field<AiderTownEntity> ()
					.Title ("Lieu de l'assembl�e")
					.InitialValue (currentUser.Office.OfficeMainContact.Address.Town)
				.End ()
			.End ();
		}
	}
}

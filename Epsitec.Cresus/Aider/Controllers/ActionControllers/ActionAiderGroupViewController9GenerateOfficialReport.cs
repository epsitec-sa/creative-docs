//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
			return Resources.Text ("Générer un extrait officiel");
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
										.Append ("Assemblée paroissiale du ")
										.Append (date.ToString("d MMM yyyy"))
										.Append (" à ")
										.Append (place.Name)
										.ToString ();

			var report = AiderOfficeGroupParticipantReportEntity.Create (this.BusinessContext, group, sender, documentName, title, content);

			this.BusinessContext.SaveChanges (LockingPolicy.ReleaseLock);
			report.ProcessorUrl		= report.GetProcessorUrl (this.BusinessContext, "officederogation", sender);
			this.BusinessContext.SaveChanges (LockingPolicy.ReleaseLock);

			EntityBag.Add (report, title);
		}

		protected override void GetForm(ActionBrick<AiderGroupEntity, SimpleBrick<AiderGroupEntity>> form)
		{
			form
				.Title ("Production d'un extrait officiel")
				.Field<string> ()
					.Title ("Titre")
					.InitialValue ("Liste des dérogations")
				.End ()
				.Field<System.DateTime> ()
					.Title ("Date de l'assemblée")
				.End ()
				.Field<AiderTownEntity> ()
					.Title ("Lieu de l'assemblée")
				.End ()
			.End ();
		}
	}
}

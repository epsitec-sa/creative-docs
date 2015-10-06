//	Copyright © 2013-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Enumerations;
using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (65)]
	public sealed class ActionAiderPersonWarningViewController65ProcessDepartureAndHide : ActionAiderPersonWarningViewControllerInteractive
	{
		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Confirmer la sortie");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<string,string> (this.Execute);
		}

		private void Execute(string globalKeys, string localKeys)
		{
			var warning = this.Entity;
			var person  = warning.Person;

			localKeys = localKeys.Trim ();
			localKeys = localKeys.Replace ('.', ',');
			var localParticipations = person.GetLocalParticipationsOrderedByName ();
			person.DeleteNumberedParticipationsNotInKeys (this.BusinessContext, localParticipations, localKeys);

			globalKeys = globalKeys.Trim ();
			globalKeys = globalKeys.Replace ('.', ',');
			var globalParticipations = person.GetGlobalParticipationsOrderedByName ();
			person.DeleteNumberedParticipationsNotInKeys (this.BusinessContext, globalParticipations, globalKeys);

			person.HidePerson (this.BusinessContext);
			
			this.ClearWarningAndRefreshCaches ();
			this.ClearWarningAndRefreshCachesForAll (warning.WarningType);
		}

		protected override void GetForm(ActionBrick<AiderPersonWarningEntity, SimpleBrick<AiderPersonWarningEntity>> form)
		{
			var content = this.Entity.Person.GetNonParishGroupParticipationsNumberedSummary (this.BusinessContext);
			content += "<br/>";
			content += this.Entity.Person.GetParishGroupParticipationsNumberedSummary (this.BusinessContext);
			form
				.Title (this.GetTitle ())
				.Text (content)
				.Field<string> ()
					.Title ("N° des participations globales à conserver ? Ex. (1,3,4)")
				.End ()
				.Field<string> ()
					.Title ("N° des participations locales à conserver ?")
				.End ()
			.End ();
		}
	}
}

//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Common.Support.Extensions;
using System.Collections.Generic;
using System.Linq;
using Epsitec.Aider.Override;
using Epsitec.Cresus.Core.Library;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (11)]
	public sealed class ActionAiderEventViewController11Delete : ActionViewController<AiderEventEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Supprimer l'acte");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create(this.Execute);
		}

		private void Execute()
		{
			var user = AiderUserManager.Current.AuthenticatedUser;

			if (this.Entity.State != Enumerations.EventState.Validated)
			{
				Logic.BusinessRuleException ("Cette action fonctionne uniquement pour supprimer un act validé par erreur");
			}

			if (user.IsAdmin ())
			{
				// remove act from person view
				this.Entity.GetMainActors ().ForEach ((a) =>
				{
					if (a.IsExternal == false)
					{
						a.Person.Events.Remove (this.Entity);
					}
				});

				// remove act document from office
				var act = this.Entity.Report;
				act.Office.RemoveDocumentInternal (act);
				
				// office next acts of year must be renumbered and renamed
				AiderEventOfficeReportEntity.GetNextOfficeActFromEvent (this.BusinessContext, this.Entity, act.EventNumberByYearAndRegistry)
				.ForEach (a =>
				{
					var newNumber = a.EventNumberByYearAndRegistry -1;
					a.EventNumberByYearAndRegistry = newNumber;
					a.Name = AiderEventOfficeReportEntity.GetReportName (this.Entity, a);
				});

				// delete report & event
				this.BusinessContext.DeleteEntity (act);
				this.BusinessContext.DeleteEntity (this.Entity);
			}
			else
			{
				Logic.BusinessRuleException ("Vous n'avez pas le droit de supprimer un acte");
			}
		}
	}
}
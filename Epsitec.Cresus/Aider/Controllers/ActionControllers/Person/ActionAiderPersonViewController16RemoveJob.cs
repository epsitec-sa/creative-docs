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
	[ControllerSubType (16)]
	public sealed class ActionAiderPersonViewController16RemoveJob : ActionViewController<AiderPersonEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Supprimer un poste...");
		}

		public FormattedText GetText()
		{
			return "Voulez-vous vraiment supprimer ce poste ?";
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<AiderEmployeeJobEntity> (this.Execute);
		}

		protected override void GetForm(ActionBrick<AiderPersonEntity, SimpleBrick<AiderPersonEntity>> form)
		{
			var jobs = this.Entity.Employee.EmployeeJobs;
			form
				.Title (this.GetTitle ())
				.Text (this.GetText ())
				.Field <AiderEmployeeJobEntity> ()
					.Title ("Poste à supprimer")
					.WithFavorites (jobs, true)
				.End ()
			.End ();
		}

		private void Execute(AiderEmployeeJobEntity job)
		{
			if(job.IsNull ())
			{
				throw new BusinessRuleException ("Veuillez séléctionner un poste dans la liste");
			}

			job.Delete (this.BusinessContext);
		}
	}
}

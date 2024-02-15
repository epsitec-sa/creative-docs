//	Copyright © 2014-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Pierre ARNAUD

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
	[ControllerSubType (4)]
	public sealed class ActionAiderOfficeManagementViewController4DeleteEmployee : TemplateActionViewController<AiderOfficeManagementEntity, AiderEmployeeEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Dissocier le collaborateur...");
		}

		public FormattedText GetText()
		{
            var num = this.AdditionalEntity.EmployeeJobs.Where (x => x.Office == this.Entity).Count ();

            if (num > 1)
            {
                return $"Voulez-vous vraiment supprimer les {num} postes qui lient ce collaborateur à cette gestion ?";
            }
            else
            {
                return "Voulez-vous vraiment supprimer le poste qui lie ce collaborateur à cette gestion ?";
            }
        }

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		protected override void GetForm(ActionBrick<AiderOfficeManagementEntity, SimpleBrick<AiderOfficeManagementEntity>> form)
		{
			form
				.Title (this.GetTitle ())
				.Text (this.GetText ())
			.End ();
		}

		private void Execute()
		{
            var jobs = this.AdditionalEntity
                .EmployeeJobs
                .Where (x => x.Office == this.Entity)
                .ToList ();

            foreach (var job in jobs)
            {
                AiderEmployeeEntity.Delete (this.BusinessContext, this.AdditionalEntity, job);
            }
		}
	}
}

//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
	[ControllerSubType (3)]
	public sealed class ActionAiderEmployeeViewController03RemoveJob : TemplateActionViewController<AiderEmployeeEntity, AiderEmployeeJobEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Supprimer un poste...");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		protected override void GetForm(ActionBrick<AiderEmployeeEntity, SimpleBrick<AiderEmployeeEntity>> form)
		{
			form
				.Title ("Supprimer un poste")
				.Text ("Faut-il vraiment supprimer ce poste ?")
				.End ();
		}

		private void Execute()
		{
			var context  = this.BusinessContext;
			var job      = this.AdditionalEntity;

			job.Delete (context);
		}
	}
}


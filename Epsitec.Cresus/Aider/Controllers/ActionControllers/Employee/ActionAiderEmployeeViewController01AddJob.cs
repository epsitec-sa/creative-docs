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

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (1)]
	public sealed class ActionAiderEmployeeViewController01AddJob : ActionViewController<AiderEmployeeEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Ajouter un poste...");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<AiderOfficeManagementEntity, string, EmployeeJobFunction, EmployeeEmployer> (this.Execute);
		}

		protected override void GetForm(ActionBrick<AiderEmployeeEntity, SimpleBrick<AiderEmployeeEntity>> form)
		{
			form
				.Title ("Définir un nouveau poste")
				.Field<AiderOfficeManagementEntity> ()
					.Title ("Lieu d'église")
				.End ()
				.Field<string> ()
					.Title ("Site ou secteur")
				.End ()
				.Field<EmployeeJobFunction> ()
					.Title ("Fonction")
				.End ()
				.Field<EmployeeEmployer> ()
					.Title ("Employeur")
					.InitialValue (EmployeeEmployer.CS)
				.End ()
			.End ();
		}

		private void Execute(AiderOfficeManagementEntity office, string detail, EmployeeJobFunction function, EmployeeEmployer employer)
		{
			if (office.IsNull ())
			{
				throw new BusinessRuleException ("Le lieu d'église manque.");
			}

			AiderEmployeeJobEntity.Create (this.BusinessContext, this.Entity, function, employer, office, detail);
		}
	}
}
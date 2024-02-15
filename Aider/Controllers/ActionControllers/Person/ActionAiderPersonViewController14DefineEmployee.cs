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
	[ControllerSubType (14)]
	public sealed class ActionAiderPersonViewController14DefineEmployee : ActionViewController<AiderPersonEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Définir en tant que collaborateur...");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<EmployeeType, string, EmployeeActivity, string, AiderUserEntity> (this.Execute);
		}

		protected override void GetForm(ActionBrick<AiderPersonEntity, SimpleBrick<AiderPersonEntity>> form)
		{
			form
				.Title ("Définir en tant que collaborateur")
				.Field<EmployeeType> ()
					.Title ("Type de collaborateur")
					.InitialValue (EmployeeType.Pasteur)
				.End ()
				.Field<string> ()
					.Title ("Fonction")
				.End ()
				.Field<EmployeeActivity> ()
					.Title ("Degré d'activité")
					.InitialValue (EmployeeActivity.Active)
				.End ()
				.Field<string> ()
					.Title ("Numéro AVS")
					.InitialValue ("756.")
				.End ()
				.Field<AiderUserEntity> ()
					.Title ("Utilisateur associé")
				.End ()
			.End ();
		}

		private void Execute(EmployeeType employeeType, string function, EmployeeActivity employeeActivity, string navs13, AiderUserEntity user)
		{
			AiderEmployeeEntity.Create (this.BusinessContext, this.Entity, user, employeeType, function, employeeActivity, navs13);
		}
	}
}

//	Copyright � 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	[ControllerSubType (4)]
	public sealed class ActionAiderOfficeManagementViewController4DeleteEmployee : TemplateActionViewController<AiderOfficeManagementEntity, AiderEmployeeEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Supprimer l'employ�...");
		}

		public FormattedText GetText()
		{
			return "Voulez-vous vraiment supprimer cet employ� ?";
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
			AiderEmployeeEntity.Delete (this.BusinessContext, this.AdditionalEntity);
		}
	}
}
//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (1)]
	public sealed class ActionAiderOfficeManagementViewController1CreateDocument : ActionViewController<AiderOfficeManagementEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Créer un document");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<AiderContactEntity, string> (this.Execute);
		}

		private void Execute(AiderContactEntity recipient,string content)
		{
			var settings = this.Entity.Settings.Single(s => s.IsCurrentSettings == true);
			AiderOfficeManagementEntity.CreateDocument (this.BusinessContext,this.Entity, settings, recipient, content);
		}

		protected override void GetForm(ActionBrick<AiderOfficeManagementEntity, SimpleBrick<AiderOfficeManagementEntity>> form)
		{
			form
				.Title ("Document")
				.Field<AiderContactEntity> ()
					.Title ("Destinataire")
				.End ()
				.Field<string> ()
					.Title ("Contenu")
				.End ()
			.End ();
		}
	}
}

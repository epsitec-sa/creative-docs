//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (6)]
	public sealed class ActionAiderPersonViewController6 : TemplateActionViewController<AiderPersonEntity, AiderContactEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Supprimer l'adresse alternative");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		public override bool RequiresAdditionalEntity()
		{
			return true;
		}


		private void Execute()
		{
			this.BusinessContext.DeleteEntity (this.AdditionalEntity);
		}

		protected override void GetForm(ActionBrick<AiderPersonEntity, SimpleBrick<AiderPersonEntity>> form)
		{
			form
				.Title ("Supprimer l'adresse alternative ?")
					.Text (TextFormatter.FormatText ("Souhaitez-vous vraiment supprimer l'adresse alternative suivante:\n \n", this.AdditionalEntity.Address.GetSummary ()))
				.End ();
		}
	}
}
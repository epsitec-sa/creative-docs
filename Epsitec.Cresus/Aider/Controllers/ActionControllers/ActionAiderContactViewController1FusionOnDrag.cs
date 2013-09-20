//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (1)]
	public sealed class ActionAiderContactViewController1FusionOnDrag : TemplateActionViewController<AiderContactEntity, AiderContactEntity>
	{
		public override bool RequiresAdditionalEntity
		{
			get
			{
				return true;
			}
		}
		
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Fusionner avec un autre contact");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<bool>(this.Execute);
		}

		private void Execute(bool doFusion)
		{
			
		}

		protected override void GetForm(ActionBrick<AiderContactEntity, SimpleBrick<AiderContactEntity>> form)
		{
			if (this.Entity.Person.IsGovernmentDefined)
			{
				if (this.AdditionalEntity.Person.IsGovernmentDefined)
				{

				}
			}

			form
				.Title ("Options de Fusion")
				.Text("Analyse...")
				.Field<bool> ()
					.Title ("Fusionner")
				.End ()
				.End ();
		}
	}
}

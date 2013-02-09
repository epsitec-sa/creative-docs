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
	[ControllerSubType (0)]
	public sealed class ActionAiderContactViewController0 : ActionViewController<AiderContactEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Créer une nouvelle personne");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<string, string> (this.Execute);
		}

		private void Execute(string firstName, string lastName)
		{
			//	TODO: ...

			throw new System.NotImplementedException ();
		}

		protected override void GetForm(ActionBrick<AiderContactEntity, SimpleBrick<AiderContactEntity>> form)
		{
			form
				.Title ("Créer une nouvelle personne")
				.Field<string> ()
					.Title ("Prénom")
				.End ()
				.Field<string> ()
					.Title ("Nom de famille")
				.End ()
				.End ();
		}
	}
}

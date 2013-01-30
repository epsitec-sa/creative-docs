//	Copyright � 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using System.Collections.Generic;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (0)]
	public sealed class ActionAiderContactViewController0 : ActionViewController<AiderContactEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Cr�er une nouvelle personne");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<string, string> (this.Execute);
		}

		private void Execute(string firstName, string lastName)
		{
			//	TODO: ...
		}

		protected override void GetForm(ActionBrick<AiderContactEntity, SimpleBrick<AiderContactEntity>> form)
		{
			form
				.Title ("Cr�er une nouvelle personne")
				.Field<string> ()
					.Title ("Pr�nom")
				.End ()
				.Field<string> ()
					.Title ("Nom de famille")
				.End ()
				.End ();
		}
	}
}

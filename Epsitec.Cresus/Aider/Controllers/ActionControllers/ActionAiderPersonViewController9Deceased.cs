//	Copyright � 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	[ControllerSubType (9)]
	public sealed class ActionAiderPersonViewController9Deceased : ActionViewController<AiderPersonEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("La personne est d�c�d�e");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<Date> (this.Execute);
		}

		private void Execute(Date date)
		{
			var person = this.Entity;

			AiderPersonEntity.KillPerson (this.BusinessContext, person, date);
		}

		protected override void GetForm(ActionBrick<AiderPersonEntity, SimpleBrick<AiderPersonEntity>> form)
		{
			form.Title ("Marque la personne comme d�c�d�e")
				.Text ("Attention: cette op�ration est irr�versible.")
				.Field<Date> ()
					.Title ("Date du d�c�s")
					.InitialValue (Date.Today)
				.End ();
		}
	}
}


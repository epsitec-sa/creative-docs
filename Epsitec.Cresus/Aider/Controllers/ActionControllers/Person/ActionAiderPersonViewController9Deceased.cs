//	Copyright © 2013-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	[ControllerSubType (9)]
	public sealed class ActionAiderPersonViewController9Deceased : ActionViewController<AiderPersonEntity>
	{
		public override bool IsEnabled
		{
			get
			{
				return this.Entity.IsAlive;
			}
		}

		public override FormattedText GetTitle()
		{
			return Resources.Text ("La personne est décédée");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<Date, bool> (this.Execute);
		}

		private void Execute(Date date, bool uncertain)
		{
			var person = this.Entity;

			person.KillPerson (this.BusinessContext, date, uncertain);
		}

		protected override void GetForm(ActionBrick<AiderPersonEntity, SimpleBrick<AiderPersonEntity>> form)
		{
			var person    = this.Entity;
			var date      = person.eCH_Person.PersonDateOfDeath ?? Date.Today;
			var uncertain = true;

			form
				.Title ("Marque la personne comme décédée")
				.Text ("Attention: cette opération est irréversible.")
				.Field<Date> ()
					.Title ("Date du décès")
					.InitialValue (date)
				.End ()
				.Field<bool> ()
					.Title ("Date du décès incertaine")
					.InitialValue (uncertain)
				.End ()
			.End ();
		}
	}
}


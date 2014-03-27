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
	[ControllerSubType (5)]
	public sealed class ActionAiderPersonViewController5AddHousehold : ActionViewController<AiderPersonEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Associer à un nouveau ménage");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<bool> (this.Execute);
		}

		protected override void GetForm(ActionBrick<AiderPersonEntity, SimpleBrick<AiderPersonEntity>> form)
		{
			var person = this.Entity;
			var householdSummary = this.GetHouseholdSummary ();

			form
				.Title ("Associer à un nouveau ménage")
				.Text (TextFormatter.FormatText ("La personne", person.DisplayName, "(", person.Age, ")",
					"est actuellement associée", householdSummary, "."))
				.Field<bool> ()
					.Title ("Déplacer la personne dans son propre ménage")
					.InitialValue (true)
				.End ()
			.End ();
		}

		private FormattedText GetHouseholdSummary()
		{
			var households = this.Entity.Households;

			if (households.Count == 0)
			{
				return Resources.FormattedText ("à aucun ménage");
			}
			if (households.Count == 1)
			{
				return TextFormatter.FormatText ("au ménage", households[0].GetCompactSummary ());
			}
			else
			{
				return Resources.FormattedText ("à plusieurs ménages");
			}

		}

		private void Execute(bool move)
		{
			var context = this.BusinessContext;
			var person  = this.Entity;
			
			person.AssignNewHousehold (context, move);
		}
	}
}

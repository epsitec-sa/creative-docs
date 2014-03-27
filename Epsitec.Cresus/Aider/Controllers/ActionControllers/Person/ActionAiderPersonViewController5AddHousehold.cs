//	Copyright � 2013-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
			return Resources.Text ("Associer � un nouveau m�nage");
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
				.Title ("Associer � un nouveau m�nage")
				.Text (TextFormatter.FormatText ("La personne", person.DisplayName, "(", person.Age, ")",
					"est actuellement associ�e", householdSummary, "."))
				.Field<bool> ()
					.Title ("D�placer la personne dans son propre m�nage")
					.InitialValue (true)
				.End ()
			.End ();
		}

		private FormattedText GetHouseholdSummary()
		{
			var households = this.Entity.Households;

			if (households.Count == 0)
			{
				return Resources.FormattedText ("� aucun m�nage");
			}
			if (households.Count == 1)
			{
				return TextFormatter.FormatText ("au m�nage", households[0].GetCompactSummary ());
			}
			else
			{
				return Resources.FormattedText ("� plusieurs m�nages");
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

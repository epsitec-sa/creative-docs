//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	[ControllerSubType (4)]
	public sealed class ActionAiderContactViewController4RemoveMemberFromHousehold : TemplateActionViewController<AiderContactEntity, AiderPersonEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Retirer le membre sélectionné de ce ménage");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}


		protected override void GetForm(ActionBrick<AiderContactEntity, SimpleBrick<AiderContactEntity>> form)
		{
			var person = this.AdditionalEntity;
			var households = person.Households;
			int count = households.Count - 1;

			FormattedText message;

			if (count == 0)
			{
				message = TextFormatter.FormatText (
					"Souhaitez-vous vraiment retirer", person.GetFullName (), "de ce ménage ?",
					"\n \n",
					"La personne sera déplacée dans un nouveau ménage vide.");
			}
			else
			{
				var variable = count > 1 ? "de plusieurs ménages." : "d'un ménage.";
				message = TextFormatter.FormatText (
					"Souhaitez-vous vraiment retirer", person.GetFullName (), "de ce ménage ?",
					"\n \n",
					"La personne fera encore partie", variable);
			}

			form
				.Title ("Retirer le membre du ménage ?")
				.Text (message)
				.End ();
		}


		private void Execute()
		{
			var context   = this.BusinessContext;
			var person    = this.AdditionalEntity;
			var household = this.Entity.Household;

			person.RemoveFromThisHousehold (context, household);
		}
	}
}


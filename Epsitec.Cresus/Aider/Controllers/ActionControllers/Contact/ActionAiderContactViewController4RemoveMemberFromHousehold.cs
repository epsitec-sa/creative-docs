//	Copyright � 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
			return Resources.FormattedText ("Retirer le membre s�lectionn� de ce m�nage");
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
					"Souhaitez-vous vraiment retirer", person.GetFullName (), "de ce m�nage ?",
					"\n \n",
					"La personne sera d�plac�e dans un nouveau m�nage vide.");
			}
			else
			{
				var variable = count > 1 ? "de plusieurs m�nages." : "d'un m�nage.";
				message = TextFormatter.FormatText (
					"Souhaitez-vous vraiment retirer", person.GetFullName (), "de ce m�nage ?",
					"\n \n",
					"La personne fera encore partie", variable);
			}

			form
				.Title ("Retirer le membre du m�nage ?")
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


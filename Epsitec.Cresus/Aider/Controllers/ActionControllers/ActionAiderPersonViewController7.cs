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
	[ControllerSubType (7)]
	public sealed class ActionAiderPersonViewController7 : TemplateActionViewController<AiderPersonEntity, AiderHouseholdEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Retirer le ménage sélectionné");
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
			var context = this.BusinessContext;

			var person    = this.Entity;
			var household = this.AdditionalEntity;
			var contacts  = person.Contacts;
			var contact   = contacts.FirstOrDefault (x => x.Household == household);

			if (contacts.Count == 1)
			{
				var newHousehold = this.BusinessContext.CreateAndRegisterEntity<AiderHouseholdEntity> ();
				AiderContactEntity.Create (this.BusinessContext, person, newHousehold, true);
			}

			if (household.Members.Count == 1)
			{
				context.DeleteEntity (household);
			}
			if (contact.IsNotNull ())
			{
				AiderContactEntity.Delete (context, contact);
			}
		}

		protected override void GetForm(ActionBrick<AiderPersonEntity, SimpleBrick<AiderPersonEntity>> form)
		{
			form
				.Title ("Retirer le ménage sélectionné ?")
					.Text (TextFormatter.FormatText ("Souhaitez-vous vraiment retirer le ménage", this.AdditionalEntity.DisplayName, "associé à cette personne ?"))
				.End ();
		}
	}
}

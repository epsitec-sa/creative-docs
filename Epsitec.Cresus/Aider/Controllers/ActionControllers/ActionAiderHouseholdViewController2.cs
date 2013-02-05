//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (2)]
	public sealed class ActionAiderHouseholdViewController2 : TemplateActionViewController<AiderHouseholdEntity, AiderPersonEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Retirer le membre sélectionné de ce ménage");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}


		private void Execute()
		{
			var context = this.BusinessContext;
			
			var person     = this.AdditionalEntity;
			var houhsehold = this.Entity;

			var example = new AiderContactEntity ()
			{
				Person      = person,
				ContactType = Enumerations.ContactType.PersonHousehold,
			};

			var results = context.DataContext.GetByExample (example);
			var contact = results.FirstOrDefault (x => x.Household == houhsehold);

			if (results.Count == 1)
			{
				var newHousehold = this.BusinessContext.CreateAndRegisterEntity<AiderHouseholdEntity> ();

				AiderContactEntity.Create (this.BusinessContext, person, newHousehold, true);
			}

			if (contact.IsNotNull ())
			{
				context.DeleteEntity (contact);
			}
		}

		protected override void GetForm(ActionBrick<AiderHouseholdEntity, SimpleBrick<AiderHouseholdEntity>> form)
		{
			form
				.Title ("Retirer le membre du ménage ?")
				.Text (TextFormatter.FormatText ("Souhaitez-vous vraiment retirer le membre suivant de ce ménage:\n \n", this.AdditionalEntity.GetSummary ()))
				.End ();
		}

		public override bool RequiresAdditionalEntity()
		{
			return true;
		}
	}
}

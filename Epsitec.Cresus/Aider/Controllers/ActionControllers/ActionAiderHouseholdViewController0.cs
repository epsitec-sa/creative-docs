//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

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
	internal sealed class ActionAiderHouseholdViewController0 : ActionViewController<AiderHouseholdEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Créer un membre");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<string, string, bool> (this.Execute);
		}

		protected override void GetForm(ActionBrick<AiderHouseholdEntity, SimpleBrick<AiderHouseholdEntity>> form)
		{
			form
				.Title ("Créer un membre")
				.Field<string> ()
					.Title ("Prénom")
				.End ()
				.Field<string> ()
					.Title ("Nom")
					.InitialValue (h => h.GetDefaultLastname ())
				.End ()
				.Field<bool> ()
					.Title ("Le nouveau membre est un chef du ménage")
					.InitialValue (false)
				.End ()
			.End ();
		}
		
		private void Execute(string firstname, string lastname, bool isPersonHead)
		{
			var household = this.Entity;

			var member = this.BusinessContext.CreateAndRegisterEntity<AiderPersonEntity> ();

			member.eCH_Person.PersonFirstNames = firstname;
			member.eCH_Person.PersonOfficialName = lastname;

			AiderContactEntity.Create(this.BusinessContext, member, household, isPersonHead);
		}
	}
}

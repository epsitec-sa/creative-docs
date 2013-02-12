using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (2)]
	public sealed class ActionAiderUserViewController2 : ActionViewController<AiderUserEntity>
	{

		public override FormattedText GetTitle()
		{
			return Resources.Text ("Associer à une personne");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<AiderContactEntity> (this.Execute);
		}

		private void Execute(AiderContactEntity contact)
		{
			var user = this.Entity;

			var person = contact.IsNull()
				? null
				: contact.Person;

			user.Person = person;
		}

		protected override void GetForm(ActionBrick<AiderUserEntity, SimpleBrick<AiderUserEntity>> form)
		{
			form
				.Title (Resources.Text ("Associer l'utilisateur à une personne"))
				.Field<AiderContactEntity> ()
					.Title (Resources.Text ("Personne"))
				.End ()
			.End ();
		}
	}
}

//	Copyright © 2013-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Common.Support.Extensions;
using System.Collections.Generic;
using System.Linq;
using Epsitec.Aider.Override;
using Epsitec.Cresus.Core.Library;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (11)]
	public sealed class ActionAiderEventViewController11Delete : ActionViewController<AiderEventEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Supprimer l'acte/radier le document");
		}

        protected override void GetForm(ActionBrick<AiderEventEntity, SimpleBrick<AiderEventEntity>> form)
        {
            form
                .Title (this.GetTitle ())
                .Text ("Voulez-vous vraiment supprimer l'acte en radiant ce document ?")
                .Field<bool> ()
                    .Title ("Supprime définitivement le document (ceci renumérotera les actes)")
                .End ()
            .End ();
        }

        public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<bool>(this.Execute);
		}

		private void Execute(bool deleteDocument)
		{
			var user = AiderUserManager.Current.AuthenticatedUser;

			if (this.Entity.State != Enumerations.EventState.Validated)
			{
				Logic.BusinessRuleException ("Cette action sert uniquement à supprimer un acte validé par erreur.");
			}

			if ((user.IsSysAdmin ()) ||
                (user.IsAdmin () && !deleteDocument))
			{
                this.Entity.DeleteImmutableEntity (this.BusinessContext, deleteDocument);
			}
			else
			{
				Logic.BusinessRuleException ("Vous n'avez pas le droit de supprimer un acte");
			}
		}
	}
}
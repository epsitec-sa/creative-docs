//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DeletionControllers;

using System.Linq;
using Epsitec.Aider.Enumerations;
using Epsitec.Cresus.Core.Business;

namespace Epsitec.Aider.Controllers.CreationControllers
{
	[ControllerSubType (0)]
	public sealed class BrickDeletionAiderEventPlaceViewController0 : BrickDeletionViewController<AiderEventPlaceEntity>
	{
        protected override void GetForm(ActionBrick<AiderEventPlaceEntity, SimpleBrick<AiderEventPlaceEntity>> action)
        {
            action
                .Title ("Détruire le lieu")
                .Text ("Êtes vous sûr de vouloir détruire ce lieu ?")
                .Field<string> ()
                    .Title ("Intitulé")
                    .InitialValue (x => x.Name)
                    .ReadOnly ()
                .End ()
            .End ();
        }

        public override ActionExecutor GetExecutor()
        {
            return ActionExecutor.Create<string> (this.Execute);
        }

		private void Execute(string _1)
        {
			var place = this.Entity;

            var eventsExample = new AiderEventEntity ()
            {
                Place = place
            };

            var blockingEvents = this.BusinessContext.GetByExample (eventsExample);
            if (blockingEvents.Any ())
            {
                throw new BusinessRuleException (string.Format ("Impossible ce lieu est utilisé dans {0} acte(s)", blockingEvents.Count ()));
            }

            this.BusinessContext.DeleteEntity (place);
		}
	}
}

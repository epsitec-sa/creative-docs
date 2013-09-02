//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

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
using Epsitec.Aider.Enumerations;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (9)]
	public sealed class ActionAiderPersonWarningViewController9ProcessAddressChange : ActionViewController<AiderPersonWarningEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Marquer comme lu");
		}

		public override ActionExecutor GetExecutor()
		{
            return ActionExecutor.Create<bool>(this.Execute);
		}

		private void Execute(bool appliForAll)
		{

			if (appliForAll)
			{
				foreach (var household in this.Entity.Person.Households)
				{
					foreach (var member in household.Members)
					{
						foreach (var warn in member.Warnings)
						{
							if (warn.WarningType.Equals (WarningType.EChAddressChanged))
							{
								member.RemoveWarningInternal (warn);
								this.BusinessContext.DeleteEntity (warn);
							}
						}
					}
				}
			}

            this.Entity.Person.RemoveWarningInternal(this.Entity);
            this.BusinessContext.DeleteEntity(this.Entity);
                   
		}

        protected override void GetForm(ActionBrick<AiderPersonWarningEntity, SimpleBrick<AiderPersonWarningEntity>> form)
        {
            form
                .Title(this.GetTitle())
				.Field<bool> ()
					.Title ("Appliquer à tout les membres du ménage")
					.InitialValue (false)
				.End ()
            .End();
        }
	}
}


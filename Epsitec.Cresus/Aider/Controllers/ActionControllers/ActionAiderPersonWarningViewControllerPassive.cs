//	Copyright � 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	public abstract class ActionAiderPersonWarningViewControllerPassive : ActionAiderPersonWarningViewController
	{
		protected override bool					NeedsInteraction
		{
			get
			{
				return false;
			}
		}


		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Marquer comme lu");
		}

		public sealed override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		
		protected abstract void Execute();
	}
}

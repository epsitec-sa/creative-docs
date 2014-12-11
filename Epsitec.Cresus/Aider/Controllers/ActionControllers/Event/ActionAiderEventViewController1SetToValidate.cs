//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

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
using Epsitec.Aider.Override;
using Epsitec.Cresus.Core.Library;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (1)]
	public sealed class ActionAiderEventViewController1SetToValidate : ActionViewController<AiderEventEntity>
	{
		public override bool IsEnabled
		{
			get
			{
				return this.Entity.State == Enumerations.EventState.InPreparation;
			}
		}

		public override FormattedText GetTitle()
		{
			return Resources.Text ("Mettre en validation");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create(this.Execute);
		}

		private void Execute()
		{
			//TODO IMPLEMENT BUSINESS RULES
			this.Entity.State = Enumerations.EventState.ToValidate;
		}
	}
}
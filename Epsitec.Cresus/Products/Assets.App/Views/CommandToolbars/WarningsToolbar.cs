//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.CommandToolbars
{
	public class WarningsToolbar : AbstractTreeTableToolbar
	{
		public WarningsToolbar(DataAccessor accessor, CommandContext commandContext)
			: base (accessor, commandContext)
		{
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.CreateButton (Res.Commands.Warnings.First, 2);
			this.CreateButton (Res.Commands.Warnings.Prev, 1);
			this.CreateButton (Res.Commands.Warnings.Next, 1);
			this.CreateButton (Res.Commands.Warnings.Last, 2);

			this.CreateSeparator (1);

			this.CreateButton (Res.Commands.Warnings.Deselect, 3);

			this.CreateSeparator (3);

			this.CreateButton (Res.Commands.Warnings.Goto, 0);
		}
	}
}

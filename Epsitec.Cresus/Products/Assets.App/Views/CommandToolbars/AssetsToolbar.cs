//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.CommandToolbars
{
	public class AssetsToolbar : AbstractTreeTableToolbar
	{
		public AssetsToolbar(DataAccessor accessor, CommandContext commandContext)
			: base (accessor, commandContext)
		{
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.CreateButton (Res.Commands.Assets.Filter, 1);
			this.CreateButton (Res.Commands.Assets.Graphic, 1);

			this.CreateButton (Res.Commands.Assets.First, 3);
			this.CreateButton (Res.Commands.Assets.Prev, 2);
			this.CreateButton (Res.Commands.Assets.Next, 2);
			this.CreateButton (Res.Commands.Assets.Last, 3);

			this.CreateSeparator (1);

			this.CreateButton (Res.Commands.Assets.CompactAll, 4);
			this.CreateButton (Res.Commands.Assets.CompactOne, 5);
			this.CreateButton (Res.Commands.Assets.ExpandOne, 5);
			this.CreateButton (Res.Commands.Assets.ExpandAll, 4);
			
			this.CreateSeparator (4);

			this.CreateButton (Res.Commands.Assets.New, 0);
			this.CreateButton (Res.Commands.Assets.Delete, 0);
			this.CreateButton (Res.Commands.Assets.Deselect, 7);

			this.CreateSeparator (0);

			this.CreateButton (Res.Commands.Assets.Copy, 6);
			this.CreateButton (Res.Commands.Assets.Paste, 6);
			this.CreateButton (Res.Commands.Assets.Export, 0);
		}
	}
}

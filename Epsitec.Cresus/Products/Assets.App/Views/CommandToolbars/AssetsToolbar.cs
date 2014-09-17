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

			this.buttonFilter     = this.CreateCommandButton (DockStyle.None, Res.Commands.Assets.Filter);
			this.buttonGraphic    = this.CreateCommandButton (DockStyle.None, Res.Commands.Assets.Graphic);

			this.buttonFirst      = this.CreateCommandButton (DockStyle.None, Res.Commands.Assets.First);
			this.buttonPrev       = this.CreateCommandButton (DockStyle.None, Res.Commands.Assets.Prev);
			this.buttonNext       = this.CreateCommandButton (DockStyle.None, Res.Commands.Assets.Next);
			this.buttonLast       = this.CreateCommandButton (DockStyle.None, Res.Commands.Assets.Last);

			this.separator1       = this.CreateSeparator     (DockStyle.None);

			this.buttonCompactAll = this.CreateCommandButton (DockStyle.None, Res.Commands.Assets.CompactAll);
			this.buttonCompactOne = this.CreateCommandButton (DockStyle.None, Res.Commands.Assets.CompactOne);
			this.buttonExpandOne  = this.CreateCommandButton (DockStyle.None, Res.Commands.Assets.ExpandOne);
			this.buttonExpandAll  = this.CreateCommandButton (DockStyle.None, Res.Commands.Assets.ExpandAll);
			
			this.separator2       = this.CreateSeparator     (DockStyle.None);
			this.separator3       = this.CreateSeparator     (DockStyle.None);

			this.buttonNew        = this.CreateCommandButton (DockStyle.None, Res.Commands.Assets.New);
			this.buttonDelete     = this.CreateCommandButton (DockStyle.None, Res.Commands.Assets.Delete);
			this.buttonDeselect   = this.CreateCommandButton (DockStyle.None, Res.Commands.Assets.Deselect);

			this.separator4       = this.CreateSeparator     (DockStyle.None);

			this.buttonCopy       = this.CreateCommandButton (DockStyle.None, Res.Commands.Assets.Copy);
			this.buttonPaste      = this.CreateCommandButton (DockStyle.None, Res.Commands.Assets.Paste);
			this.buttonExport     = this.CreateCommandButton (DockStyle.None, Res.Commands.Assets.Export);
		}
	}
}

//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.CommandToolbars
{
	public class EventsToolbar : AbstractTreeTableToolbar
	{
		public EventsToolbar(DataAccessor accessor, CommandContext commandContext)
			: base (accessor, commandContext)
		{
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.buttonFirst      = this.CreateCommandButton (DockStyle.None, Res.Commands.Events.First);
			this.buttonPrev       = this.CreateCommandButton (DockStyle.None, Res.Commands.Events.Prev);
			this.buttonNext       = this.CreateCommandButton (DockStyle.None, Res.Commands.Events.Next);
			this.buttonLast       = this.CreateCommandButton (DockStyle.None, Res.Commands.Events.Last);

			this.separator1       = this.CreateSeparator     (DockStyle.None);
			this.separator2       = this.CreateSeparator     (DockStyle.None);
			this.separator3       = this.CreateSeparator     (DockStyle.None);

			this.buttonNew        = this.CreateCommandButton (DockStyle.None, Res.Commands.Events.New);
			this.buttonDelete     = this.CreateCommandButton (DockStyle.None, Res.Commands.Events.Delete);
			this.buttonDeselect   = this.CreateCommandButton (DockStyle.None, Res.Commands.Events.Deselect);

			this.separator4       = this.CreateSeparator     (DockStyle.None);

			this.buttonCopy       = this.CreateCommandButton (DockStyle.None, Res.Commands.Events.Copy);
			this.buttonPaste      = this.CreateCommandButton (DockStyle.None, Res.Commands.Events.Paste);
			this.buttonExport     = this.CreateCommandButton (DockStyle.None, Res.Commands.Events.Export);
		}
	}
}

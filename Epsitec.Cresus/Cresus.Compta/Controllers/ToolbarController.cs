//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Widgets;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	public class ToolbarController
	{
		public ToolbarController(MainWindowController mainWindowController)
		{
			this.mainWindowController = mainWindowController;
		}


		public void CreateUI(Widget parent)
		{
			var frame = new FrameBox
			{
				Parent              = parent,
				PreferredHeight     = 24,
				//?BackColor           = UIBuilder.WindowBackColor1,
				BackColor           = UIBuilder.WindowBackColor2,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				Dock                = DockStyle.Top,
				Padding             = new Margins (5, 5, 1, 1),
			};

			new Separator
			{
				Parent              = parent,
				PreferredHeight     = 1,
				IsVerticalLine      = false,
				Dock                = DockStyle.Top,
			};

			this.CreateButton (frame, Res.Commands.Navigator.Prev);
			this.CreateButton (frame, Res.Commands.Navigator.Next, 10);

			this.CreateButton (frame, Res.Commands.Edit.Undo);
			this.CreateButton (frame, Res.Commands.Edit.Redo, 10);

			this.CreateButton (frame, Res.Commands.Select.Up);
			this.CreateButton (frame, Res.Commands.Select.Down);
			this.CreateButton (frame, Res.Commands.Select.Home);
		}

		private void CreateButton(Widget parent, Command cmd, double rightMargin = 0)
		{
			var button = UIBuilder.CreateButton (parent, cmd, 24, 20);

			if (rightMargin != 0)
			{
				button.Margins = new Margins (0, rightMargin, 0, 0);
			}
		}


		private readonly MainWindowController	mainWindowController;
	}
}

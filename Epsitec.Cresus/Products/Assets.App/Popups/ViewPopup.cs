//	Copyright © 2013-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Views.CommandToolbars;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class ViewPopup : AbstractPopup
	{
		public Command[]						ViewCommands;

		protected override Size DialogSize
		{
			get
			{
				return new Size (ViewPopup.margins*2 + ViewPopup.buttonSize*this.ViewCommands.Length, ViewPopup.margins*2 + ViewPopup.buttonSize);
			}
		}

		public override void CreateUI()
		{
			var frame = this.CreateFrame (ViewPopup.margins, ViewPopup.margins, ViewPopup.buttonSize*this.ViewCommands.Length, ViewPopup.buttonSize);

			foreach (var command in this.ViewCommands)
			{
				this.CreateButton (frame, command);
			}
		}

		protected IconButton CreateButton(Widget parent, Command command)
		{
			var button = new IconButton
			{
				Parent        = parent,
				AutoFocus     = false,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (ViewPopup.buttonSize, ViewPopup.buttonSize),
				CommandId     = command.Caption.Id,
			};

			button.Clicked += delegate
			{
				//	On ferme le popup plus tard, une fois que tout le reste aura été exécuté...
				Application.QueueAsyncCallback (() => this.ClosePopup ());
			};

			return button;
		}


		private const int margins    = 5;
		private const int buttonSize = AbstractCommandToolbar.primaryToolbarHeight;

	}
}
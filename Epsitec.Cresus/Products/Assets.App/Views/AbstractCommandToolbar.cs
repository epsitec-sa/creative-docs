﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App.Views
{
	public abstract class AbstractCommandToolbar
	{
		public AbstractCommandToolbar()
		{
			this.commandStates = new Dictionary<ToolbarCommand, ToolbarCommandState> ();
		}

		public virtual void CreateUI(Widget parent)
		{
		}


		public void SetCommandState(ToolbarCommand command, ToolbarCommandState state)
		{
			this.commandStates[command] = state;
			this.UpdateCommandButtons ();
		}

		public ToolbarCommandState GetCommandState(ToolbarCommand command)
		{
			if (this.commandStates.ContainsKey (command))
			{
				return this.commandStates[command];
			}
			else
			{
				return ToolbarCommandState.Hide;
			}
		}


		protected virtual void UpdateCommandButtons()
		{
		}

		protected void UpdateCommandButton(IconButton button, ToolbarCommand command)
		{
			button.Visibility = this.GetCommandVisibility (command);
			button.Enable     = this.GetCommandEnable     (command);
		}

		private bool GetCommandVisibility(ToolbarCommand command)
		{
			var state = this.GetCommandState (command);
			return state != ToolbarCommandState.Hide;
		}

		private bool GetCommandEnable(ToolbarCommand command)
		{
			var state = this.GetCommandState (command);
			return state == ToolbarCommandState.Enable;
		}


		protected virtual void CreateToolbar(Widget parent, int size)
		{
		}

		protected void SetActiveState(IconButton button, bool state)
		{
			button.ActiveState = state ? ActiveState.Yes : ActiveState.No;
		}

		protected IconButton CreateCommandButton(HToolBar toolbar, DockStyle dock, ToolbarCommand command, string icon, string tooltip)
		{
			var size = toolbar.PreferredHeight;

			var button = new IconButton
			{
				Parent        = toolbar,
				AutoFocus     = false,
				Dock          = dock,
				IconUri       = AbstractCommandToolbar.GetResourceIconUri (icon),
				PreferredSize = new Size (size, size),
			};

			ToolTip.Default.SetToolTip (button, tooltip);

			button.Clicked += delegate
			{
				this.OnCommandClicked (command);
			};

			return button;
		}


		protected static string GetResourceIconUri(string icon)
		{
			if (string.IsNullOrEmpty (icon))
			{
				return null;
			}
			else if (icon.Contains (':'))
			{
				return FormattedText.Escape (icon);
			}
			else
			{
				return string.Format ("manifest:Epsitec.Cresus.Assets.App.Images.{0}.icon", FormattedText.Escape (icon));
			}
		}


		#region Events handler
		private void OnCommandClicked(ToolbarCommand command)
		{
			if (this.CommandClicked != null)
			{
				this.CommandClicked (this, command);
			}
		}

		public delegate void CommandClickedEventHandler(object sender, ToolbarCommand command);
		public event CommandClickedEventHandler CommandClicked;
		#endregion


		private readonly Dictionary<ToolbarCommand, ToolbarCommandState> commandStates;
	}
}
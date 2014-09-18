//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.CommandToolbars
{
	public abstract class AbstractCommandToolbar : System.IDisposable
	{
		public AbstractCommandToolbar(DataAccessor accessor, CommandContext commandContext)
		{
			this.accessor       = accessor;
			this.commandContext = commandContext;

			this.commandDescriptions = new Dictionary<ToolbarCommand, CommandDescription> ();
			this.commandStates       = new Dictionary<ToolbarCommand, ToolbarCommandState> ();
			this.commandWidgets      = new Dictionary<ToolbarCommand, Widget> ();
			this.commandRedDotCounts = new Dictionary<ToolbarCommand, int> ();

			this.commandDispatcher = new CommandDispatcher (this.GetType ().FullName, CommandDispatcherLevel.Primary, CommandDispatcherOptions.AutoForwardCommands);
			this.commandDispatcher.RegisterController (this);  // nécesaire pour [Command (Res.CommandIds...)]
		}

		public void Dispose()
		{
			this.commandDispatcher.Dispose ();
		}

		public void Close()
		{
		}


		public bool								Visibility
		{
			get
			{
				if (this.toolbar == null)
				{
					return false;
				}
				else
				{
					return this.toolbar.Visibility;
				}
			}
			set
			{
				if (this.toolbar != null)
				{
					this.toolbar.Visibility = value;
				}
			}
		}

		public bool								IsParentVisible
		{
			get
			{
				if (this.toolbar == null)
				{
					return false;
				}
				else
				{
					Widget x = this.toolbar;

					while (x != null)
					{
						if (!x.Visibility)
						{
							return false;
						}

						x = x.Parent;
					}

					return true;
				}
			}
		}


		public virtual void CreateUI(Widget parent)
		{
			this.toolbar = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = AbstractCommandToolbar.secondaryToolbarHeight,
				BackColor       = ColorManager.ToolbarBackgroundColor,
			};

			CommandDispatcher.SetDispatcher (this.toolbar, this.commandDispatcher);  // nécesaire pour [Command (Res.CommandIds...)]
		}


		public void SetVisibility(Command command, bool visibility)
		{
			this.commandContext.GetCommandState (command).Visibility = visibility;

			if (!visibility)
			{
				this.SetEnable (command, false);
			}
		}

		public void SetEnable(Command command, bool enable)
		{
			this.commandContext.GetCommandState (command).Enable = enable;
		}

		public void SetActiveState(Command command, bool active)
		{
			this.commandContext.GetCommandState (command).ActiveState = active ? ActiveState.Yes : ActiveState.No;
		}



		protected ButtonWithRedDot CreateButton(DockStyle dock, Command command)
		{
			var size = this.toolbar.PreferredHeight;

			return new ButtonWithRedDot
			{
				Parent        = this.toolbar,
				AutoFocus     = false,
				Dock          = dock,
				PreferredSize = new Size (size, size),
				CommandObject = command,
			};
		}


		public Widget GetTarget(CommandEventArgs e)
		{
			//	Cherche le widget ayant la plus grande surface.
			var targets = this.commandDispatcher.FindVisuals (e.Command)
				.OrderByDescending (x => x.PreferredHeight * x.PreferredWidth)
				.ToArray ();

			return targets.FirstOrDefault () as Widget ?? e.Source as Widget;
		}


		public const int primaryToolbarHeight   = 32 + 10;
		public const int secondaryToolbarHeight = 24 + 2;
		public const int separatorWidth         = 11;


		protected readonly DataAccessor			accessor;
		protected readonly CommandDispatcher	commandDispatcher;
		protected readonly CommandContext		commandContext;

		private readonly Dictionary<ToolbarCommand, CommandDescription>		commandDescriptions;
		private readonly Dictionary<ToolbarCommand, ToolbarCommandState>	commandStates;
		private readonly Dictionary<ToolbarCommand, int>					commandRedDotCounts;
		private readonly Dictionary<ToolbarCommand, Widget>					commandWidgets;

		protected FrameBox						toolbar;
	}
}

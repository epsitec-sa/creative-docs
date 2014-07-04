﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Widgets;

namespace Epsitec.Cresus.Assets.App.Views
{
	public abstract class AbstractCommandToolbar
	{
		public AbstractCommandToolbar()
		{
			this.commandDescriptions = new Dictionary<ToolbarCommand, CommandDescription> ();
			this.commandStates       = new Dictionary<ToolbarCommand, ToolbarCommandState> ();
			this.commandWidgets      = new Dictionary<ToolbarCommand, Widget> ();

			this.CreateCommands ();
		}

		protected abstract void CreateCommands();

		public abstract FrameBox CreateUI(Widget parent);


		public void SetCommandDescription(ToolbarCommand command, string icon, string tooltip)
		{
			//	Modifie la description d'une commande. On peut modifier ainsi une
			//	commande déjà définie. Si 'icon' ou 'tooltip' sont null, cela
			//	signifie qu'on conserve les anciennes valeurs.
			var current = this.GetCommandDescription (command);

			if (string.IsNullOrEmpty (icon) && !current.IsEmpty)
			{
				icon = current.Icon;
			}

			if (string.IsNullOrEmpty (tooltip) && !current.IsEmpty)
			{
				tooltip = current.Tooltip;
			}

			this.SetCommandDescription (command, new CommandDescription (icon, tooltip));
		}

		public void SetCommandDescription(ToolbarCommand command, CommandDescription desc)
		{
			this.commandDescriptions[command] = desc;
		}

		public CommandDescription GetCommandDescription(ToolbarCommand command)
		{
			//	Retourne la description d'une commande.
			CommandDescription desc;
			if (this.commandDescriptions.TryGetValue (command, out desc))
			{
				return desc;
			}
			else
			{
				return CommandDescription.Empty;
			}
		}


		public void SetCommandEnable(ToolbarCommand command, bool enable)
		{
			if (command == ToolbarCommand.Edit)
			{
			}
			//?

			if (enable)
			{
				this.SetCommandState (command, ToolbarCommandState.Enable);
			}
			else
			{
				this.SetCommandState (command, ToolbarCommandState.Disable);
			}
		}

		public void SetCommandActivate(ToolbarCommand command, bool activate)
		{
			if (activate)
			{
				this.SetCommandState (command, ToolbarCommandState.Activate);
			}
			else
			{
				this.SetCommandState (command, ToolbarCommandState.Enable);
			}
		}

		public void SetCommandState(ToolbarCommand command, ToolbarCommandState state)
		{
			this.commandStates[command] = state;
			this.UpdateWidget (command);
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


		public Widget GetTarget(ToolbarCommand command)
		{
			//	Retourne le widget à l'origine d'une commande. On n'utilise jamais
			//	ceci pour modifier le widget, mais uniquement pour connaître sa
			//	position, en vue de l'affichage de la queue des popups.
			Widget widget;
			if (this.commandWidgets.TryGetValue (command, out widget))
			{
				return widget;
			}
			else
			{
				throw new System.InvalidOperationException (string.Format ("Target not found for command {0}", command));
			}
		}


		protected IconButton CreateCommandButton(DockStyle dock, ToolbarCommand command)
		{
			var desc = this.GetCommandDescription (command);

			if (desc.IsEmpty)
			{
				return null;
			}

			var size = this.toolbar.PreferredHeight;

			var button = new IconButton
			{
				Parent        = this.toolbar,
				AutoFocus     = false,
				Dock          = dock,
				IconUri       = Misc.GetResourceIconUri (desc.Icon),
				PreferredSize = new Size (size, size),
			};

			ToolTip.Default.SetToolTip (button, desc.Tooltip);

			button.Clicked += delegate
			{
				this.OnCommandClicked (command);
			};

			this.commandWidgets.Add (command, button);
			this.UpdateWidget (command);

			return button;
		}

		protected FrameBox CreateSeparator(DockStyle dock)
		{
			var size = this.toolbar.PreferredHeight;

			var sep = new FrameBox
			{
				Parent        = this.toolbar,
				Dock          = dock,
				PreferredSize = new Size (1, size),
				Margins       = new Margins (AbstractCommandToolbar.separatorWidth/2, AbstractCommandToolbar.separatorWidth/2, 0, 0),
				BackColor     = ColorManager.SeparatorColor,
			};

			return sep;
		}


		private void UpdateWidget(ToolbarCommand command)
		{
			Widget widget;
			if (this.commandWidgets.TryGetValue (command, out widget))
			{
				if (widget is IconButton)
				{
					this.UpdateButton (widget as IconButton, command);
				}

				//	Il pourrait y avoir des widgets autres que des IconButton dans le futur !
			}
		}

		private void UpdateButton(IconButton button, ToolbarCommand command)
		{
			//	Un bouton placé avec SetManualBounds gère différemment la visibilité.
			if (button.Dock != DockStyle.None)
			{
				button.Visibility = this.GetCommandVisibility (command);
			}

			button.Enable      = this.GetCommandEnable (command);
			button.ActiveState = this.GetCommandActivateState (command);
		}

		private bool GetCommandVisibility(ToolbarCommand command)
		{
			var state = this.GetCommandState (command);
			return state != ToolbarCommandState.Hide;
		}

		private bool GetCommandEnable(ToolbarCommand command)
		{
			var state = this.GetCommandState (command);
			return state == ToolbarCommandState.Enable ||
				   state == ToolbarCommandState.Activate;
		}

		private ActiveState GetCommandActivateState(ToolbarCommand command)
		{
			var state = this.GetCommandState (command);
			return Misc.GetActiveState (state == ToolbarCommandState.Activate);
		}


		#region Events handler
		protected void OnCommandClicked(ToolbarCommand command)
		{
			this.CommandClicked.Raise (this, command);
		}

		public event EventHandler<ToolbarCommand> CommandClicked;
		#endregion


		public const int primaryToolbarHeight   = 32 + 10;
		public const int secondaryToolbarHeight = 24 + 2;
		public const int separatorWidth         = 11;


		private readonly Dictionary<ToolbarCommand, CommandDescription>		commandDescriptions;
		private readonly Dictionary<ToolbarCommand, ToolbarCommandState>	commandStates;
		private readonly Dictionary<ToolbarCommand, Widget>					commandWidgets;

		protected FrameBox						toolbar;
	}
}

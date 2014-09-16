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

			this.CreateCommands ();
		}

		public void Dispose()
		{
			this.commandDispatcher.Dispose ();
		}

		public void Close()
		{
			this.DetachShortcuts ();
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


		protected abstract void CreateCommands();

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


		public void SetActiveState(Command command, bool active)
		{
			this.commandContext.GetCommandState (command).ActiveState = active ? ActiveState.Yes : ActiveState.No;
		}


		public void SetCommandDescription(ToolbarCommand command, string icon, string tooltip, Shortcut shortcut = null)
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

			if (shortcut != null)
			{
				if (string.IsNullOrEmpty (tooltip))
				{
					tooltip = string.Concat ("(", shortcut.ToString (), ")");
				}
				else
				{
					tooltip = string.Concat (tooltip, "<br/>(", shortcut.ToString (), ")");
				}
			}

			this.SetCommandDescription (command, new CommandDescription (icon, tooltip, shortcut));
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


		public void SetCommandEnable(Command command, bool enable)
		{
			this.commandContext.GetCommandState (command).Enable = enable;
		}

		public void SetCommandEnable(ToolbarCommand command, bool enable)
		{
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
			ToolbarCommandState state;
			if (this.commandStates.TryGetValue (command, out state))
			{
				return state;
			}
			else
			{
				return ToolbarCommandState.Hide;
			}
		}

		public void SetCommandRedDotCount(ToolbarCommand command, int count)
		{
			this.commandRedDotCounts[command] = count;
			this.UpdateRedDot (command);
		}

		public int GetCommandRedDotCount(ToolbarCommand command)
		{
			int count;
			if (this.commandRedDotCounts.TryGetValue (command, out count))
			{
				return count;
			}
			else
			{
				return 0;
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


		public void ActivateCommand(ToolbarCommand command)
		{
			//	Appelé par le ShortcutCatcher pour exécuter une commande, lorsqu'un
			//	raccourci clavier a été activé. L'effet est le même que si l'utilisateur
			//	avait cliqué sur le bouton.

			if (this.commandStates[command] == ToolbarCommandState.Enable ||
				this.commandStates[command] == ToolbarCommandState.Activate)
			{
				this.OnCommandClicked (command);
			}
		}

		protected void AttachShortcuts()
		{
			//	Attache toutes les commandes ayant un raccourci clavier au ShortcutCatcher.
			var shortcutCatcher = this.toolbar.GetShortcutCatcher ();

			foreach (var cd in this.commandDescriptions.Where (x => x.Value.Shortcut != null))
			{
				shortcutCatcher.Attach (this, cd.Key, cd.Value.Shortcut);
			}
		}

		private void DetachShortcuts()
		{
			//	Détache toutes les commandes de cette toolbar.
			var shortcutCatcher = this.toolbar.GetShortcutCatcher ();
			shortcutCatcher.Detach (this);
		}


		protected void CreateSajex(int width)
		{
			new FrameBox
			{
				Parent        = this.toolbar,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (width, this.toolbar.PreferredHeight),
			};
		}

		protected ButtonWithRedDot CreateCommandButton(DockStyle dock, Command command)
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

		protected ButtonWithRedDot CreateCommandButton(DockStyle dock, ToolbarCommand command, bool activable = false)
		{
			var desc = this.GetCommandDescription (command);

			if (desc.IsEmpty)
			{
				return null;
			}

			var size = this.toolbar.PreferredHeight;

			var button = new ButtonWithRedDot
			{
				Parent        = this.toolbar,
				AutoFocus     = false,
				Dock          = dock,
				IconUri       = Misc.GetResourceIconUri (desc.Icon),
				PreferredSize = new Size (size, size),
			};

			if (activable)
			{
				button.ButtonStyle = ButtonStyle.ActivableIcon;
			}

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


		private void UpdateRedDot(ToolbarCommand command)
		{
			Widget widget;
			if (this.commandWidgets.TryGetValue (command, out widget))
			{
				if (widget is ButtonWithRedDot)
				{
					var button = widget as ButtonWithRedDot;
					button.RedDotCount = this.GetCommandRedDotCount (command);
				}

				//	Il pourrait y avoir des widgets autres que des ButtonWithRedDot dans le futur !
			}
		}

		private void UpdateWidget(ToolbarCommand command)
		{
			Widget widget;
			if (this.commandWidgets.TryGetValue (command, out widget))
			{
				if (widget is ButtonWithRedDot)
				{
					this.UpdateButton (widget as ButtonWithRedDot, command);
				}

				//	Il pourrait y avoir des widgets autres que des ButtonWithRedDot dans le futur !
			}
		}

		private void UpdateButton(ButtonWithRedDot button, ToolbarCommand command)
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


		public static Widget GetTarget(CommandDispatcher commandDispatcher, CommandEventArgs e)
		{
			//	Cherche le widget ayant la plus grande surface.
			var targets = commandDispatcher.FindVisuals (e.Command)
				.OrderByDescending (x => x.PreferredHeight * x.PreferredWidth)
				.ToArray ();

			return targets.FirstOrDefault () as Widget ?? e.Source as Widget;
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

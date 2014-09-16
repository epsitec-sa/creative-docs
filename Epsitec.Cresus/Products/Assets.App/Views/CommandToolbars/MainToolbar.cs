//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;
using Epsitec.Common.Support;

namespace Epsitec.Cresus.Assets.App.Views.CommandToolbars
{
	public class MainToolbar : AbstractCommandToolbar
	{
		public MainToolbar(DataAccessor accessor, CommandContext commandContext)
			: base (accessor, commandContext)
		{
		}

		protected override void CreateCommands()
		{
		}


		public ViewType							ViewType
		{
			get
			{
				return this.viewType;
			}
			set
			{
				if (this.viewType != value)
				{
					this.viewType = value;
					this.UpdateViewTypeCommands ();
					this.UpdateViewModeCommands ();
				}
			}
		}

		public ViewMode							ViewMode
		{
			get
			{
				return this.viewMode;
			}
			set
			{
				if (this.viewMode != value)
				{
					this.viewMode = value;
					this.UpdateViewModeCommands ();
				}
			}
		}

		public int								Simulation
		{
			get
			{
				return this.simulation;
			}
			set
			{
				if (this.simulation != value)
				{
					this.simulation = value;
					this.UpdateSimulation ();
				}
			}
		}

		public int								WarningsRedDotCount
		{
			set
			{
				if (this.buttonWarnings != null)
				{
					this.buttonWarnings.RedDotCount = value;
				}
			}
		}


		public override FrameBox CreateUI(Widget parent)
		{
			this.viewType = ViewType.Assets;
			this.viewMode = ViewMode.Single;
			this.simulation = 0;

			this.toolbar = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = AbstractCommandToolbar.primaryToolbarHeight,
				BackColor       = ColorManager.ToolbarBackgroundColor,
			};

			this.CreateCommandButton (DockStyle.Left, Res.Commands.Main.New);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Main.Navigate.Back);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Main.Navigate.Forward);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Main.Navigate.Menu);

			this.CreateCommandButton (DockStyle.Left, Res.Commands.View.Assets);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.View.Amortizations);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.View.Entries);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.View.Categories);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.View.Groups);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.View.Persons);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.View.Reports);
			this.buttonWarnings =
			this.CreateCommandButton (DockStyle.Left, Res.Commands.View.Warnings);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.View.Settings);

			this.CreateSajex (10);

			this.CreateCommandButton (DockStyle.Left, Res.Commands.ViewMode.Single);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.ViewMode.Event);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.ViewMode.Multiple);

			this.CreateSajex (40);

			this.CreateCommandButton (DockStyle.Left, Res.Commands.Main.Edit);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Main.Locked);
			this.buttonSimulation =
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Main.Simulation);

			this.CreateCommandButton (DockStyle.Right, Res.Commands.Edit.Cancel);
			this.CreateCommandButton (DockStyle.Right, Res.Commands.Edit.Accept);

			this.UpdateViewTypeCommands ();
			this.UpdateViewModeCommands ();
			this.UpdateSimulation ();

			this.AttachShortcuts ();

			return this.toolbar;
		}


		//?[Command (Res.CommandIds.View.Settings)]
		//?void CommandToto(CommandDispatcher dispatcher, CommandEventArgs e)
		//?{
		//?	var target = e.Source as Widget;
		//?	var pos = e.CommandMessage.Cursor;  // position absolue dans la fenêtre
		//?
		//?	CreateAssetPopup.Show (target, this.accessor, null);
		//?
		//?	var cs = this.commandContext.GetCommandState (Res.Commands.View.Settings);
		//?	//?cs.Enable = false;
		//?
		//?	if (cs.ActiveState == ActiveState.Yes)
		//?	{
		//?		cs.ActiveState = ActiveState.No;
		//?	}
		//?	else
		//?	{
		//?		cs.ActiveState = ActiveState.Yes;
		//?	}
		//?}

		[Command (Res.CommandIds.View.Assets)]
		[Command (Res.CommandIds.View.Amortizations)]
		[Command (Res.CommandIds.View.Entries)]
		[Command (Res.CommandIds.View.Categories)]
		[Command (Res.CommandIds.View.Groups)]
		[Command (Res.CommandIds.View.Persons)]
		[Command (Res.CommandIds.View.Reports)]
		[Command (Res.CommandIds.View.Warnings)]
		[Command (Res.CommandIds.View.AssetsSettings)]
		[Command (Res.CommandIds.View.PersonsSettings)]
		[Command (Res.CommandIds.View.Accounts)]
		private void CommandView(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.ViewType = ViewType.FromDefaultKind (this.accessor, MainToolbar.GetViewKind (e.Command));
			this.OnChangeView ();
		}

		[Command (Res.CommandIds.View.Settings)]
		private void CommandViewSettings(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = AbstractCommandToolbar.GetTarget (this.commandDispatcher, e);
			this.ShowViewPopup (target);
		}

		[Command (Res.CommandIds.ViewMode.Single)]
		[Command (Res.CommandIds.ViewMode.Event)]
		[Command (Res.CommandIds.ViewMode.Multiple)]
		private void CommandViewMode(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.ViewMode = MainToolbar.GetViewMode (e.Command);
			this.OnChangeView ();
		}

		[Command (Res.CommandIds.Main.New)]
		private void CommandNew(CommandDispatcher dispatcher, CommandEventArgs e)
		{
		}

		[Command (Res.CommandIds.Main.Navigate.Back)]
		private void CommandNavigateBack(CommandDispatcher dispatcher, CommandEventArgs e)
		{
		}

		[Command (Res.CommandIds.Main.Navigate.Forward)]
		private void CommandNavigateForward(CommandDispatcher dispatcher, CommandEventArgs e)
		{
		}

		[Command (Res.CommandIds.Main.Navigate.Menu)]
		private void CommandNavigateMenu(CommandDispatcher dispatcher, CommandEventArgs e)
		{
		}

		[Command (Res.CommandIds.Main.Locked)]
		private void CommandMainLocked(CommandDispatcher dispatcher, CommandEventArgs e)
		{
		}

		//?[Command (Res.CommandIds.Main.Edit)]
		//?void CommandMainEdit(CommandDispatcher dispatcher, CommandEventArgs e)
		//?{
		//?}


		private void UpdateViewTypeCommands()
		{
			//	"Allume" le bouton correspondant à la vue sélectionnée.
			foreach (var kind in MainToolbar.ViewTypeKinds)
			{
				var command = MainToolbar.GetViewCommand (kind);
				var cs = this.commandContext.GetCommandState (command);

				cs.ActiveState = (this.viewType.Kind == kind) ? ActiveState.Yes : ActiveState.No;
			}

			//	"Allume" l'engrenage si la vue sélectionnée a été choisie dans le PopUp.
			{
				var cs = this.commandContext.GetCommandState (Res.Commands.View.Settings);
				
				bool ap = MainToolbar.PopupViewTypeKinds.Contains (this.viewType.Kind);
				cs.ActiveState = ap ? ActiveState.Yes : ActiveState.No;
			}
		}

		private void UpdateViewModeCommands()
		{
			bool enable = (this.viewType.Kind == ViewTypeKind.Assets);

			foreach (var mode in MainToolbar.ViewTypeModes)
			{
				var command = MainToolbar.GetViewCommand (mode);
				var cs = this.commandContext.GetCommandState (command);

				if (enable)
				{
					cs.Enable      = true;
					cs.ActiveState = (this.viewMode == mode) ? ActiveState.Yes : ActiveState.No;
				}
				else
				{
					cs.Enable      = false;
					cs.ActiveState = ActiveState.No;
				}
			}
		}

		private void UpdateSimulation()
		{
			string icon = "Main.Simulation";
			
			if (this.simulation >= 0)
			{
				icon += this.simulation.ToString (System.Globalization.CultureInfo.InvariantCulture);
			}
			
			this.buttonSimulation.IconUri = Misc.GetResourceIconUri (icon);
		}

		private void ShowViewPopup(Widget target)
		{
			var commands = MainToolbar.PopupViewTypeKinds
				.Select (x => MainToolbar.GetViewCommand (x))
				.ToArray ();

			var popup = new ViewPopup ()
			{
				ViewCommands = commands,
			};

			popup.Create (target, leftOrRight: false);
		}


		#region View
		public static ViewTypeKind GetViewKind(Command command)
		{
			foreach (var kind in MainToolbar.ViewTypeKinds)
			{
				if (command == MainToolbar.GetViewCommand (kind))
				{
					return kind;
				}
			}

			return ViewTypeKind.Unknown;
		}

		private static Command GetViewCommand(ViewTypeKind kind)
		{
			switch (kind)
			{
				case ViewTypeKind.Assets:
					return Res.Commands.View.Assets;

				case ViewTypeKind.Amortizations:
					return Res.Commands.View.Amortizations;

				case ViewTypeKind.Entries:
					return Res.Commands.View.Entries;

				case ViewTypeKind.Categories:
					return Res.Commands.View.Categories;

				case ViewTypeKind.Groups:
					return Res.Commands.View.Groups;

				case ViewTypeKind.Persons:
					return Res.Commands.View.Persons;

				case ViewTypeKind.Reports:
					return Res.Commands.View.Reports;

				case ViewTypeKind.Warnings:
					return Res.Commands.View.Warnings;

				case ViewTypeKind.AssetsSettings:
					return Res.Commands.View.AssetsSettings;

				case ViewTypeKind.PersonsSettings:
					return Res.Commands.View.PersonsSettings;

				case ViewTypeKind.Accounts:
					return Res.Commands.View.Accounts;

				default:
					throw new System.InvalidOperationException (string.Format ("Unsupported ViewType {0}", kind.ToString ()));
			}
		}

		private static IEnumerable<ViewTypeKind> PopupViewTypeKinds
		{
			//	Enumère uniquement les vues choisies via le ViewPopup.
			get
			{
				yield return ViewTypeKind.AssetsSettings;
				yield return ViewTypeKind.PersonsSettings;
				yield return ViewTypeKind.Accounts;
			}
		}

		private static IEnumerable<ViewTypeKind> ViewTypeKinds
		{
			//	Enumère toutes les vues possibles.
			get
			{
				yield return ViewTypeKind.Assets;
				yield return ViewTypeKind.Amortizations;
				yield return ViewTypeKind.Entries;
				yield return ViewTypeKind.Categories;
				yield return ViewTypeKind.Groups;
				yield return ViewTypeKind.Persons;
				yield return ViewTypeKind.Reports;
				yield return ViewTypeKind.Warnings;
				yield return ViewTypeKind.AssetsSettings;
				yield return ViewTypeKind.PersonsSettings;
				yield return ViewTypeKind.Accounts;
			}
		}
		#endregion


		#region ViewMode
		public static ViewMode GetViewMode(Command command)
		{
			foreach (var mode in MainToolbar.ViewTypeModes)
			{
				if (command == MainToolbar.GetViewCommand (mode))
				{
					return mode;
				}
			}

			return ViewMode.Unknown;
		}

		private static Command GetViewCommand(ViewMode mode)
		{
			switch (mode)
			{
				case ViewMode.Single:
					return Res.Commands.ViewMode.Single;

				case ViewMode.Event:
					return Res.Commands.ViewMode.Event;

				case ViewMode.Multiple:
					return Res.Commands.ViewMode.Multiple;

				default:
					throw new System.InvalidOperationException (string.Format ("Unsupported ViewMode {0}", mode.ToString ()));
			}
		}

		private static IEnumerable<ViewMode> ViewTypeModes
		{
			get
			{
				yield return ViewMode.Single;
				yield return ViewMode.Event;
				yield return ViewMode.Multiple;
			}
		}
		#endregion


		#region Events handler
		private void OnChangeView()
		{
			this.ChangeView.Raise (this);
		}

		public event EventHandler ChangeView;
		#endregion


		private ButtonWithRedDot				buttonWarnings;
		private ButtonWithRedDot				buttonSimulation;

		private ViewType						viewType;
		private ViewMode						viewMode;
		private int								simulation;
	}
}

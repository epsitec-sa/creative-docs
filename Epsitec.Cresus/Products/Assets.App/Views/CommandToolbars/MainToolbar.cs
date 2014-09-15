//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.Server.SimpleEngine;
using Epsitec.Common.Support;

namespace Epsitec.Cresus.Assets.App.Views.CommandToolbars
{
	public class MainToolbar : AbstractCommandToolbar
	{
		public MainToolbar(DataAccessor accessor, CommandDispatcher commandDispatcher, CommandContext commandContext)
			: base (accessor, commandDispatcher, commandContext)
		{
		}

		protected override void CreateCommands()
		{
			this.SetCommandDescription (ToolbarCommand.NewMandat,        "Main.New",              Res.Strings.Toolbar.Main.New.ToString ());
			this.SetCommandDescription (ToolbarCommand.OpenMandat,       "Main.Open",             Res.Strings.Toolbar.Main.Open.ToString ());
			this.SetCommandDescription (ToolbarCommand.SaveMandat,       "Main.Save",             Res.Strings.Toolbar.Main.Save.ToString ());
			this.SetCommandDescription (ToolbarCommand.NavigateBack,     "Navigate.Back",         Res.Strings.Toolbar.Main.Navigate.Back.ToString (),    new Shortcut (KeyCode.ArrowLeft  | KeyCode.ModifierAlt));
			this.SetCommandDescription (ToolbarCommand.NavigateForward,  "Navigate.Forward",      Res.Strings.Toolbar.Main.Navigate.Forward.ToString (), new Shortcut (KeyCode.ArrowRight | KeyCode.ModifierAlt));
			this.SetCommandDescription (ToolbarCommand.NavigateMenu,     "Navigate.Menu",         Res.Strings.Toolbar.Main.Navigate.Menu.ToString ());
			this.SetCommandDescription (ToolbarCommand.ViewModeSingle,   "Show.TimelineSingle",   Res.Strings.Toolbar.Main.Show.TimelineSingle.ToString ());
			this.SetCommandDescription (ToolbarCommand.ViewModeEvent,    "Show.TimelineEvent",    Res.Strings.Toolbar.Main.Show.TimelineEvent.ToString ());
			this.SetCommandDescription (ToolbarCommand.ViewModeMultiple, "Show.TimelineMultiple", Res.Strings.Toolbar.Main.Show.TimelineMultiple.ToString ());
			this.SetCommandDescription (ToolbarCommand.Edit,             "Main.Edit",             Res.Strings.Toolbar.Main.Edit.ToString (), new Shortcut (KeyCode.FuncF11));
			this.SetCommandDescription (ToolbarCommand.Locked,           "Main.Locked",           Res.Strings.Toolbar.Main.Locked.ToString ());
			this.SetCommandDescription (ToolbarCommand.Simulation,       "Main.Simulation",       Res.Strings.Toolbar.Main.Simulation.ToString ());
			this.SetCommandDescription (ToolbarCommand.Cancel,           "Edit.Cancel",           Res.Strings.Toolbar.Main.Cancel.ToString (), new Shortcut (KeyCode.Escape));
			this.SetCommandDescription (ToolbarCommand.Accept,           "Edit.Accept",           Res.Strings.Toolbar.Main.Accept.ToString (), new Shortcut (KeyCode.FuncF12));
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

			this.buttonNew             = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.NewMandat);
			this.buttonOpen            = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.OpenMandat);
			this.buttonSave            = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.SaveMandat);
			this.buttonNavigateBack    = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.NavigateBack);
			this.buttonNavigateForward = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.NavigateForward);
			this.buttonNavigateMenu    = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.NavigateMenu);

			this.CreateCommandButton (DockStyle.Left, Res.Commands.View.Assets);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.View.Amortizations);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.View.Entries);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.View.Categories);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.View.Groups);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.View.Persons);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.View.Reports);
			this.buttonWarnings = this.CreateCommandButton (DockStyle.Left, Res.Commands.View.Warnings);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.View.Settings);

			this.buttonSingle          = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.ViewModeSingle,   activable: true);
			this.buttonEvent           = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.ViewModeEvent,    activable: true);
			this.buttonMultiple        = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.ViewModeMultiple, activable: true);
									   											   				     
			this.buttonEdit            = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.Edit);
			this.buttonLocked          = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.Locked);
			this.buttonSimulation      = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.Simulation);
									  				 							   				     
			this.buttonCancel          = this.CreateCommandButton (DockStyle.Right, ToolbarCommand.Cancel);
			this.buttonAccept          = this.CreateCommandButton (DockStyle.Right, ToolbarCommand.Accept);

			this.buttonSave    .Margins = new Margins (0, 10, 0, 0);
			//?this.buttonPopup   .Margins = new Margins (0, 10, 0, 0);
			this.buttonMultiple.Margins = new Margins (0, 40, 0, 0);

			this.UpdateViewTypeCommands ();
			this.UpdateViewModeCommands ();
			this.UpdateSimulation ();

			this.AttachShortcuts ();

			return this.toolbar;
		}


		//?[Epsitec.Common.Support.Command (Res.CommandIds.View.Settings)]
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

		[Epsitec.Common.Support.Command (Res.CommandIds.View.Assets)]
		[Epsitec.Common.Support.Command (Res.CommandIds.View.Amortizations)]
		[Epsitec.Common.Support.Command (Res.CommandIds.View.Entries)]
		[Epsitec.Common.Support.Command (Res.CommandIds.View.Categories)]
		[Epsitec.Common.Support.Command (Res.CommandIds.View.Groups)]
		[Epsitec.Common.Support.Command (Res.CommandIds.View.Persons)]
		[Epsitec.Common.Support.Command (Res.CommandIds.View.Reports)]
		[Epsitec.Common.Support.Command (Res.CommandIds.View.Warnings)]
		[Epsitec.Common.Support.Command (Res.CommandIds.View.AssetsSettings)]
		[Epsitec.Common.Support.Command (Res.CommandIds.View.PersonsSettings)]
		[Epsitec.Common.Support.Command (Res.CommandIds.View.Accounts)]
		void CommandView(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.ViewType = ViewType.FromDefaultKind (this.accessor, MainToolbar.GetViewKind (e.Command));
			this.OnChangeView ();
		}

		[Epsitec.Common.Support.Command (Res.CommandIds.View.Settings)]
		void CommandViewSettings(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			//	**PA**
			var targets = commandDispatcher.FindVisuals (e.Command)
				.OrderByDescending (x => x.PreferredHeight * x.PreferredWidth)
				.ToArray ();

			var target = targets.FirstOrDefault () as Widget ?? e.Source as Widget;
			this.ShowViewPopup (target);
		}


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
			if (this.viewType == ViewType.Assets)
			{
				this.SetCommandActivate (ToolbarCommand.ViewModeSingle,   this.viewMode == ViewMode.Single  );
				this.SetCommandActivate (ToolbarCommand.ViewModeEvent,    this.viewMode == ViewMode.Event   );
				this.SetCommandActivate (ToolbarCommand.ViewModeMultiple, this.viewMode == ViewMode.Multiple);
			}
			else
			{
				this.SetCommandState (ToolbarCommand.ViewModeSingle,   ToolbarCommandState.Hide);
				this.SetCommandState (ToolbarCommand.ViewModeEvent,    ToolbarCommandState.Hide);
				this.SetCommandState (ToolbarCommand.ViewModeMultiple, ToolbarCommandState.Hide);
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


		public static ViewTypeKind GetViewKind(Command command)
		{
			foreach (var kind in MainToolbar.ViewTypeKinds.Union (MainToolbar.PopupViewTypeKinds))
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


		#region Events handler
		private void OnChangeView()
		{
			this.ChangeView.Raise (this);
		}

		public event EventHandler ChangeView;
		#endregion


		private IconButton						buttonNew;
		private IconButton						buttonOpen;
		private IconButton						buttonSave;

		private IconButton						buttonNavigateBack;
		private IconButton						buttonNavigateForward;
		private IconButton						buttonNavigateMenu;

		private ButtonWithRedDot				buttonWarnings;

		private IconButton						buttonSingle;
		private IconButton						buttonEvent;
		private IconButton						buttonMultiple;

		private IconButton						buttonEdit;
		private IconButton						buttonSimulation;
		private IconButton						buttonLocked;

		private IconButton						buttonAccept;
		private IconButton						buttonCancel;

		private ViewType						viewType;
		private ViewMode						viewMode;
		private int								simulation;
	}
}

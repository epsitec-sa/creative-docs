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

			foreach (var kind in MainToolbar.ViewTypeKinds)
			{
				var command = MainToolbar.GetViewCommand (kind);
				this.SetCommandDescription (command, StaticDescriptions.GetViewTypeIcon (kind), StaticDescriptions.GetViewTypeDescription (kind), MainToolbar.GetViewShortcut (kind));
			}
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
				var command = MainToolbar.GetViewCommand (ViewTypeKind.Warnings);
				this.SetCommandRedDotCount (command, value);
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

			this.CreateViewTypeButtons ();							   									     
			this.buttonPopup           = this.CreatePopupButton ();

			this.buttonSingle          = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.ViewModeSingle,   activable: true);
			this.buttonEvent           = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.ViewModeEvent,    activable: true);
			this.buttonMultiple        = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.ViewModeMultiple, activable: true);
									   											   				     
			this.buttonEdit            = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.Edit);
			this.buttonLocked          = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.Locked);
			this.buttonSimulation      = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.Simulation);
									  				 							   				     
			this.buttonCancel          = this.CreateCommandButton (DockStyle.Right, ToolbarCommand.Cancel);
			this.buttonAccept          = this.CreateCommandButton (DockStyle.Right, ToolbarCommand.Accept);

			{
				var size = this.toolbar.PreferredHeight;

				var button = new ButtonWithRedDot
				{
					Parent        = this.toolbar,
					AutoFocus     = false,
					Dock          = DockStyle.Left,
					PreferredSize = new Size (size, size),
					CommandId     = Res.CommandIds.View.Settings,
				};

				// [-> PA] feedback "target" ok, contrairement à Ctrl+X défini dans les ressources
				// [-> PA] comment afficher Ctrl+Y dans le tooltip ?
				button.Shortcuts.Define (new Shortcut (KeyCode.AlphaY | KeyCode.ModifierControl));

				var cs = this.commandContext.GetCommandState (Res.Commands.View.Settings);
				cs.ActiveState = ActiveState.Yes;
			}

			this.buttonSave    .Margins = new Margins (0, 10, 0, 0);
			this.buttonPopup   .Margins = new Margins (0, 10, 0, 0);
			this.buttonMultiple.Margins = new Margins (0, 40, 0, 0);

			this.UpdateViewTypeCommands ();
			this.UpdateViewModeCommands ();
			this.UpdateSimulation ();

			this.AttachShortcuts ();

			return this.toolbar;
		}


		//?
		
		void CommandToto(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = e.Source as Widget;
			var pos = e.CommandMessage.Cursor;  // position absolue dans la fenêtre

			CreateAssetPopup.Show (target, this.accessor, null);

			var cs = this.commandContext.GetCommandState (Res.Commands.View.Settings);
			//?cs.Enable = false;

			if (cs.ActiveState == ActiveState.Yes)
			{
				cs.ActiveState = ActiveState.No;
			}
			else
			{
				cs.ActiveState = ActiveState.Yes;
			}
		}
		//?


		private void CreateViewTypeButtons()
		{
			foreach (var kind in MainToolbar.ViewTypeKinds)
			{
				if (!MainToolbar.PopupViewTypeKinds.Contains (kind))
				{
					this.CreateViewTypeButton (kind);
				}
			}
		}

		private IconButton CreateViewTypeButton(ViewTypeKind kind)
		{
			var command = MainToolbar.GetViewCommand (kind);
			return this.CreateCommandButton (DockStyle.Left, command, activable: true);
		}


		private IconButton CreatePopupButton()
		{
			var size = this.toolbar.PreferredHeight;

			var button = new IconButton
			{
				Parent        = this.toolbar,
				AutoFocus     = false,
				Dock          = DockStyle.Left,
				IconUri       = Misc.GetResourceIconUri ("View.Settings"),
				ButtonStyle   = ButtonStyle.ActivableIcon,
				PreferredSize = new Size (size, size),
			};

			ToolTip.Default.SetToolTip (button, Res.Strings.Toolbar.Main.ChoiceSettings.ToString ());

			button.Clicked += delegate
			{
				this.ShowViewPopup (button);
			};

			return button;
		}


		private void UpdateViewTypeCommands()
		{
			foreach (var kind in MainToolbar.ViewTypeKinds)
			{
				var command = MainToolbar.GetViewCommand (kind);
				this.SetCommandActivate (command, this.viewType.Kind == kind);
			}

			bool ap = MainToolbar.PopupViewTypeKinds.Contains (this.viewType.Kind);
			this.buttonPopup.ActiveState = ap ? ActiveState.Yes : ActiveState.No;
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
			var popup = new ViewPopup ()
			{
				ViewTypeKinds    = MainToolbar.PopupViewTypeKinds.ToList (),
				SelectedViewType = this.viewType.Kind,
			};

			popup.Create (target, leftOrRight: false);

			popup.ViewTypeClicked += delegate (object sender, ViewTypeKind kind)
			{
				this.OnCommandClicked (MainToolbar.GetViewCommand (kind));
			};
		}


		public static ViewTypeKind GetViewKind(ToolbarCommand command)
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

		private static ToolbarCommand GetViewCommand(ViewTypeKind kind)
		{
			switch (kind)
			{
				case ViewTypeKind.Assets:
					return ToolbarCommand.ViewTypeAssets;

				case ViewTypeKind.Amortizations:
					return ToolbarCommand.ViewTypeAmortizations;

				case ViewTypeKind.Entries:
					return ToolbarCommand.ViewTypeEcritures;

				case ViewTypeKind.Categories:
					return ToolbarCommand.ViewTypeCategories;

				case ViewTypeKind.Groups:
					return ToolbarCommand.ViewTypeGroups;

				case ViewTypeKind.Persons:
					return ToolbarCommand.ViewTypePersons;

				case ViewTypeKind.Reports:
					return ToolbarCommand.ViewTypeReports;

				case ViewTypeKind.Warnings:
					return ToolbarCommand.ViewTypeWarnings;

				case ViewTypeKind.AssetsSettings:
					return ToolbarCommand.ViewTypeAssetsSettings;

				case ViewTypeKind.PersonsSettings:
					return ToolbarCommand.ViewTypePersonsSettings;

				case ViewTypeKind.Accounts:
					return ToolbarCommand.ViewTypeAccounts;

				default:
					throw new System.InvalidOperationException (string.Format ("Unsupported ViewType {0}", kind.ToString ()));
			}
		}

		private static IEnumerable<ViewTypeKind> PopupViewTypeKinds
		{
			get
			{
				yield return ViewTypeKind.AssetsSettings;
				yield return ViewTypeKind.PersonsSettings;
				yield return ViewTypeKind.Accounts;
			}
		}

		private static IEnumerable<ViewTypeKind> ViewTypeKinds
		{
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

		private static Shortcut GetViewShortcut(ViewTypeKind kind)
		{
			switch (kind)
			{
				case ViewTypeKind.Assets:
					return new Shortcut (KeyCode.FuncF2);

				case ViewTypeKind.Amortizations:
					return new Shortcut (KeyCode.FuncF3);

				case ViewTypeKind.Categories:
					return new Shortcut (KeyCode.FuncF4);

				case ViewTypeKind.Groups:
					return new Shortcut (KeyCode.FuncF5);

				case ViewTypeKind.Persons:
					return new Shortcut (KeyCode.FuncF6);

				case ViewTypeKind.Reports:
					return new Shortcut (KeyCode.FuncF7);

				case ViewTypeKind.Warnings:
					return new Shortcut (KeyCode.FuncF8);

				default:
					return null;
			}
		}


		private IconButton						buttonNew;
		private IconButton						buttonOpen;
		private IconButton						buttonSave;

		private IconButton						buttonNavigateBack;
		private IconButton						buttonNavigateForward;
		private IconButton						buttonNavigateMenu;

		private IconButton						buttonPopup;

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

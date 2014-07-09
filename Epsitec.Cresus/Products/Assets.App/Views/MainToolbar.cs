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

namespace Epsitec.Cresus.Assets.App.Views
{
	public class MainToolbar : AbstractCommandToolbar
	{
		public MainToolbar(DataAccessor accessor)
			: base (accessor)
		{
		}

		protected override void CreateCommands()
		{
			this.SetCommandDescription (ToolbarCommand.NewMandat,        "Main.New",              "Nouveau mandat");
			this.SetCommandDescription (ToolbarCommand.OpenMandat,       "Main.Open",             "Ouvrir un mandat");
			this.SetCommandDescription (ToolbarCommand.SaveMandat,       "Main.Save",             "Enregistrer le mandat");
			this.SetCommandDescription (ToolbarCommand.NavigateBack,     "Navigate.Back",         "Retourner à la vue précédente");
			this.SetCommandDescription (ToolbarCommand.NavigateForward,  "Navigate.Forward",      "Avancer à la vue suivante");
			this.SetCommandDescription (ToolbarCommand.NavigateMenu,     "Navigate.Menu",         "Dernières vues");
			this.SetCommandDescription (ToolbarCommand.ViewModeSingle,   "Show.TimelineSingle",   "Axe du temps de l'objet sélectionné");
			this.SetCommandDescription (ToolbarCommand.ViewModeEvent,    "Show.TimelineEvent",    "Tableau des événements");
			this.SetCommandDescription (ToolbarCommand.ViewModeMultiple, "Show.TimelineMultiple", "Axe du temps pour tous les objets");
			this.SetCommandDescription (ToolbarCommand.Edit,             "Main.Edit",             "Edition");
			this.SetCommandDescription (ToolbarCommand.Locked,           "Main.Locked",           "Gestion des verrous");
			this.SetCommandDescription (ToolbarCommand.Simulation,       "Main.Simulation",       "Simulation");
			this.SetCommandDescription (ToolbarCommand.Cancel,           "Edit.Cancel",           "Annuler les modifications");
			this.SetCommandDescription (ToolbarCommand.Accept,           "Edit.Accept",           "Accepter les modifications");

			foreach (var kind in MainToolbar.ViewTypeKinds)
			{
				var command = MainToolbar.GetViewCommand (kind);
				this.SetCommandDescription (command, StaticDescriptions.GetViewTypeIcon (kind), StaticDescriptions.GetViewTypeDescription (kind));
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

			this.buttonNew             = this.CreateCommandButton  (DockStyle.Left, ToolbarCommand.NewMandat);
			this.buttonOpen            = this.CreateCommandButton  (DockStyle.Left, ToolbarCommand.OpenMandat);
			this.buttonSave            = this.CreateCommandButton  (DockStyle.Left, ToolbarCommand.SaveMandat);
			this.buttonNavigateBack    = this.CreateCommandButton  (DockStyle.Left, ToolbarCommand.NavigateBack);
			this.buttonNavigateForward = this.CreateCommandButton  (DockStyle.Left, ToolbarCommand.NavigateForward);
			this.buttonNavigateMenu    = this.CreateCommandButton  (DockStyle.Left, ToolbarCommand.NavigateMenu);

			this.CreateViewTypeButtons ();							   									     
			this.buttonPopup           = this.CreatePopupButton ();
								       													   									     
			this.buttonSingle          = this.CreateViewModeButton (ViewMode.Single,   ToolbarCommand.ViewModeSingle);
			this.buttonEvent           = this.CreateViewModeButton (ViewMode.Event,    ToolbarCommand.ViewModeEvent);
			this.buttonMultiple        = this.CreateViewModeButton (ViewMode.Multiple, ToolbarCommand.ViewModeMultiple);
									   											   				     
			this.buttonEdit            = this.CreateCommandButton  (DockStyle.Left,    ToolbarCommand.Edit);
			this.buttonLocked          = this.CreateCommandButton  (DockStyle.Left,    ToolbarCommand.Locked);
			this.buttonSimulation      = this.CreateCommandButton  (DockStyle.Left,    ToolbarCommand.Simulation);
									  				 							   				     
			this.buttonCancel          = this.CreateCommandButton  (DockStyle.Right,   ToolbarCommand.Cancel);
			this.buttonAccept          = this.CreateCommandButton  (DockStyle.Right,   ToolbarCommand.Accept);

			this.buttonSave    .Margins = new Margins (0, 10, 0, 0);
			this.buttonPopup   .Margins = new Margins (0, 10, 0, 0);
			this.buttonMultiple.Margins = new Margins (0, 40, 0, 0);

			this.UpdateViewTypeCommands ();
			this.UpdateViewModeCommands ();
			this.UpdateSimulation ();

			return this.toolbar;
		}


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
			return this.CreateViewTypeButton (kind, command);
		}

		private IconButton CreateViewTypeButton(ViewTypeKind kind, ToolbarCommand command)
		{
			var button = this.CreateCommandButton (DockStyle.Left, command);
			button.ButtonStyle = ButtonStyle.ActivableIcon;

			button.Clicked += delegate
			{
				this.ViewType = ViewType.FromDefaultKind (this.accessor, kind);
				this.OnViewChanged (this.viewType);
			};

			return button;
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

			ToolTip.Default.SetToolTip (button, "Choix du réglage");

			button.Clicked += delegate
			{
				this.ShowViewPopup (button);
			};

			return button;
		}


		private IconButton CreateViewModeButton(ViewMode view, ToolbarCommand command)
		{
			var button = this.CreateCommandButton (DockStyle.Left, command);
			button.ButtonStyle = ButtonStyle.ActivableIcon;

			button.Clicked += delegate
			{
				this.ViewMode = view;
				this.OnCommandClicked (command);
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
				this.ViewType = ViewType.FromDefaultKind (this.accessor, kind);
				this.OnViewChanged (this.viewType);
			};
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
				yield return ViewTypeKind.AssetsSettings;
				yield return ViewTypeKind.PersonsSettings;
				yield return ViewTypeKind.Accounts;
			}
		}


		#region Events handler
		private void OnViewChanged(ViewType viewType)
		{
			this.ViewChanged.Raise (this, viewType);
		}

		public event EventHandler<ViewType> ViewChanged;
		#endregion


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

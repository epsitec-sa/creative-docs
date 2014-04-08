//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.App.Popups;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class MainToolbar : AbstractCommandToolbar
	{
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

			this.buttonNew             = this.CreateCommandButton  (DockStyle.Left, ToolbarCommand.NewMandat,       "Main.New",         "Nouveau mandat");
			this.buttonOpen            = this.CreateCommandButton  (DockStyle.Left, ToolbarCommand.OpenMandat,      "Main.Open",        "Ouvrir un mandat");
			this.buttonSave            = this.CreateCommandButton  (DockStyle.Left, ToolbarCommand.SaveMandat,      "Main.Save",        "Enregistrer le mandat");
			this.buttonNavigateBack    = this.CreateCommandButton  (DockStyle.Left, ToolbarCommand.NavigateBack,    "Navigate.Back",    "Retourner à la vue précédente");
			this.buttonNavigateForward = this.CreateCommandButton  (DockStyle.Left, ToolbarCommand.NavigateForward, "Navigate.Forward", "Avancer à la vue suivante");
			this.buttonNavigateMenu    = this.CreateCommandButton  (DockStyle.Left, ToolbarCommand.NavigateMenu,    "Navigate.Menu",    "Dernières vues");

			this.CreateViewTypeButtons ();							   									     
			this.buttonPopup           = this.CreatePopupButton ();
								       													   									     
			this.buttonSingle          = this.CreateViewModeButton (ViewMode.Single,   ToolbarCommand.ViewModeSingle,   "Show.TimelineSingle",   "Axe du temps de l'objet sélectionné");
			this.buttonEvent           = this.CreateViewModeButton (ViewMode.Event,    ToolbarCommand.ViewModeEvent,    "Show.TimelineEvent",    "Tableau des événements");
			this.buttonMultiple        = this.CreateViewModeButton (ViewMode.Multiple, ToolbarCommand.ViewModeMultiple, "Show.TimelineMultiple", "Axe du temps pour tous les objets");
									   											   				     
			this.buttonEdit            = this.CreateCommandButton  (DockStyle.Left,    ToolbarCommand.Edit,             "Main.Edit",             "Edition");
			this.buttonSimulation      = this.CreateCommandButton  (DockStyle.Left,    ToolbarCommand.Simulation,       "Main.Simulation",       "Simulation");
									  				 							   				     
			this.buttonCancel          = this.CreateCommandButton  (DockStyle.Right,   ToolbarCommand.Cancel,           "Edit.Cancel",           "Annuler les modifications");
			this.buttonAccept          = this.CreateCommandButton  (DockStyle.Right,   ToolbarCommand.Accept,           "Edit.Accept",           "Accepter les modifications");

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
			foreach (var viewType in MainToolbar.ViewTypes)
			{
				if (!MainToolbar.PopupViewTypes.Contains (viewType))
				{
					this.CreateViewTypeButton (viewType);
				}
			}
		}

		private IconButton CreateViewTypeButton(ViewType viewType)
		{
			var command = MainToolbar.GetViewCommand (viewType);
			return this.CreateViewTypeButton (viewType, command, StaticDescriptions.GetViewTypeIcon (viewType), StaticDescriptions.GetViewTypeDescription (viewType));
		}

		private IconButton CreateViewTypeButton(ViewType view, ToolbarCommand command, string icon, string tooltip)
		{
			var button = this.CreateCommandButton (DockStyle.Left, command, icon, tooltip);
			button.ButtonStyle = ButtonStyle.ActivableIcon;

			button.Clicked += delegate
			{
				this.ViewType = view;
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


		private IconButton CreateViewModeButton(ViewMode view, ToolbarCommand command, string icon, string tooltip)
		{
			var button = this.CreateCommandButton (DockStyle.Left, command, icon, tooltip);
			button.ButtonStyle = ButtonStyle.ActivableIcon;

			ToolTip.Default.SetToolTip (button, tooltip);

			button.Clicked += delegate
			{
				this.ViewMode = view;
				this.OnCommandClicked (command);
			};

			return button;
		}

		private void UpdateViewTypeCommands()
		{
			foreach (var viewType in MainToolbar.ViewTypes)
			{
				var command = MainToolbar.GetViewCommand (viewType);
				this.SetCommandActivate (command, this.viewType == viewType);
			}

			bool ap = MainToolbar.PopupViewTypes.Contains (this.viewType);
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
				ViewTypes        = MainToolbar.PopupViewTypes.ToList (),
				SelectedViewType = this.viewType,
			};

			popup.Create (target, leftOrRight: false);

			popup.ViewTypeClicked += delegate (object sender, ViewType viewType)
			{
				this.ViewType = viewType;
				this.OnViewChanged (this.viewType);
			};
		}


		private static ToolbarCommand GetViewCommand(ViewType viewType)
		{
			switch (viewType)
			{
				case ViewType.Assets:
					return ToolbarCommand.ViewTypeAssets;

				case ViewType.Amortizations:
					return ToolbarCommand.ViewTypeAmortizations;

				case ViewType.Entries:
					return ToolbarCommand.ViewTypeEcritures;

				case ViewType.Categories:
					return ToolbarCommand.ViewTypeCategories;

				case ViewType.Groups:
					return ToolbarCommand.ViewTypeGroups;

				case ViewType.Persons:
					return ToolbarCommand.ViewTypePersons;

				case ViewType.Reports:
					return ToolbarCommand.ViewTypeReports;

				case ViewType.AssetsSettings:
					return ToolbarCommand.ViewTypeAssetsSettings;

				case ViewType.PersonsSettings:
					return ToolbarCommand.ViewTypePersonsSettings;

				case ViewType.AccountsSettings:
					return ToolbarCommand.ViewTypeAccountsSettings;

				default:
					throw new System.InvalidOperationException (string.Format ("Unsupported ViewType {0}", viewType.ToString ()));
			}
		}

		private static IEnumerable<ViewType> PopupViewTypes
		{
			get
			{
				yield return ViewType.AssetsSettings;
				yield return ViewType.PersonsSettings;
				yield return ViewType.AccountsSettings;
			}
		}

		private static IEnumerable<ViewType> ViewTypes
		{
			get
			{
				yield return ViewType.Assets;
				yield return ViewType.Amortizations;
				yield return ViewType.Entries;
				yield return ViewType.Categories;
				yield return ViewType.Groups;
				yield return ViewType.Persons;
				yield return ViewType.Reports;
				yield return ViewType.AssetsSettings;
				yield return ViewType.PersonsSettings;
				yield return ViewType.AccountsSettings;
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

		private IconButton						buttonAccept;
		private IconButton						buttonCancel;

		private ViewType						viewType;
		private ViewMode						viewMode;
		private int								simulation;
	}
}

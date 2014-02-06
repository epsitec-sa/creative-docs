//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Widgets;

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
			this.viewType = ViewType.Objects;
			this.viewMode = ViewMode.Single;
			this.simulation = 0;

			this.toolbar = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = AbstractCommandToolbar.primaryToolbarHeight,
				BackColor       = ColorManager.ToolbarBackgroundColor,
			};

			this.buttonOpen            = this.CreateCommandButton  (DockStyle.Left,      ToolbarCommand.Open,               "Main.Open",             "Ouvrir");
			this.buttonNavigateBack    = this.CreateCommandButton  (DockStyle.Left,      ToolbarCommand.NavigateBack,       "Navigate.Back",         "Retourner");
			this.buttonNavigateForward = this.CreateCommandButton  (DockStyle.Left,      ToolbarCommand.NavigateForward,    "Navigate.Forward",      "Avancer");
			this.buttonNavigateMenu    = this.CreateCommandButton  (DockStyle.Left,      ToolbarCommand.NavigateMenu,       "Navigate.Menu",         "Historique");
								     
			this.buttonObjects         = this.CreateViewTypeButton (ViewType.Objects,    ToolbarCommand.ViewTypeObjects,    "View.Objects",          StaticDescriptions.GetViewTypeDescription (ViewType.Objects));
			this.buttonCategories      = this.CreateViewTypeButton (ViewType.Categories, ToolbarCommand.ViewTypeCategories, "View.Categories",       StaticDescriptions.GetViewTypeDescription (ViewType.Categories));
			this.buttonGroups          = this.CreateViewTypeButton (ViewType.Groups,     ToolbarCommand.ViewTypeGroups,     "View.Groups",           StaticDescriptions.GetViewTypeDescription (ViewType.Groups));
			this.buttonPersons         = this.CreateViewTypeButton (ViewType.Persons,    ToolbarCommand.ViewTypePersons,    "View.Persons",          StaticDescriptions.GetViewTypeDescription (ViewType.Persons));
			this.buttonEvents          = this.CreateViewTypeButton (ViewType.Events,     ToolbarCommand.ViewTypeEvents,     "View.Events",           StaticDescriptions.GetViewTypeDescription (ViewType.Events));
			this.buttonReports         = this.CreateViewTypeButton (ViewType.Reports,    ToolbarCommand.ViewTypeReports,    "View.Reports",          StaticDescriptions.GetViewTypeDescription (ViewType.Reports));
			this.buttonSettings        = this.CreateViewTypeButton (ViewType.Settings,   ToolbarCommand.ViewTypeSettings,   "View.Settings",         StaticDescriptions.GetViewTypeDescription (ViewType.Settings));
								       
			this.buttonSingle          = this.CreateViewModeButton (ViewMode.Single,     ToolbarCommand.ViewModeSingle,     "Show.TimelineSingle",   "Axe du temps de l'objet sélectionné");
			this.buttonEvent           = this.CreateViewModeButton (ViewMode.Event,      ToolbarCommand.ViewModeEvent,      "Show.TimelineEvent",    "Tableau des événements");
			this.buttonMultiple        = this.CreateViewModeButton (ViewMode.Multiple,   ToolbarCommand.ViewModeMultiple,   "Show.TimelineMultiple", "Axe du temps pour tous les objets");
									   
			this.buttonEdit            = this.CreateCommandButton  (DockStyle.Left,      ToolbarCommand.Edit,               "Main.Edit",             "Edition");
			this.buttonAmortissement   = this.CreateCommandButton  (DockStyle.Left,      ToolbarCommand.Amortissement,      "Main.Amortissement",    "Amortissements");
			this.buttonSimulation      = this.CreateCommandButton  (DockStyle.Left,      ToolbarCommand.Simulation,         "Main.Simulation",       "Simulation");
									  							 
			this.buttonCancel          = this.CreateCommandButton  (DockStyle.Right,     ToolbarCommand.Cancel,             "Edit.Cancel",           "Annuler les modifications");
			this.buttonAccept          = this.CreateCommandButton  (DockStyle.Right,     ToolbarCommand.Accept,             "Edit.Accept",           "Accepter les modifications");

			this.buttonOpen    .Margins = new Margins (0, 10, 0, 0);
			this.buttonSettings.Margins = new Margins (0, 10, 0, 0);
			this.buttonMultiple.Margins = new Margins (0, 40, 0, 0);

			this.UpdateViewTypeCommands ();
			this.UpdateViewModeCommands ();
			this.UpdateSimulation ();

			return this.toolbar;
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
			this.SetCommandActivate (ToolbarCommand.ViewTypeObjects,    this.viewType == ViewType.Objects   );
			this.SetCommandActivate (ToolbarCommand.ViewTypeCategories, this.viewType == ViewType.Categories);
			this.SetCommandActivate (ToolbarCommand.ViewTypeGroups,     this.viewType == ViewType.Groups    );
			this.SetCommandActivate (ToolbarCommand.ViewTypePersons,    this.viewType == ViewType.Persons   );
			this.SetCommandActivate (ToolbarCommand.ViewTypeEvents,     this.viewType == ViewType.Events    );
			this.SetCommandActivate (ToolbarCommand.ViewTypeReports,    this.viewType == ViewType.Reports   );
			this.SetCommandActivate (ToolbarCommand.ViewTypeSettings,   this.viewType == ViewType.Settings  );
		}

		private void UpdateViewModeCommands()
		{
			if (this.viewType == ViewType.Objects)
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


		#region Events handler
		private void OnViewChanged(ViewType viewType)
		{
			this.ViewChanged.Raise (this, viewType);
		}

		public event EventHandler<ViewType> ViewChanged;
		#endregion


		private IconButton						buttonOpen;

		private IconButton						buttonNavigateBack;
		private IconButton						buttonNavigateForward;
		private IconButton						buttonNavigateMenu;

		private IconButton						buttonObjects;
		private IconButton						buttonCategories;
		private IconButton						buttonGroups;
		private IconButton						buttonPersons;
		private IconButton						buttonEvents;
		private IconButton						buttonReports;
		private IconButton						buttonSettings;

		private IconButton						buttonSingle;
		private IconButton						buttonEvent;
		private IconButton						buttonMultiple;

		private IconButton						buttonEdit;
		private IconButton						buttonAmortissement;
		private IconButton						buttonSimulation;

		private IconButton						buttonAccept;
		private IconButton						buttonCancel;

		private ViewType						viewType;
		private ViewMode						viewMode;
		private int								simulation;
	}
}

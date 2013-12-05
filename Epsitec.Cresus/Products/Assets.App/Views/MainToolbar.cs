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

			this.buttonOpen          = this.CreateCommandButton  (DockStyle.Left,      ToolbarCommand.Open,               "Main.Open",             "Ouvrir");
								     
			this.buttonObjects       = this.CreateViewTypeButton (ViewType.Objects,    ToolbarCommand.ViewTypeObjects,    "View.Objects",          "Objets d'immobilisation");
			this.buttonCategories    = this.CreateViewTypeButton (ViewType.Categories, ToolbarCommand.ViewTypeCategories, "View.Categories",       "Catégories d'immobilisations");
			this.buttonGroups        = this.CreateViewTypeButton (ViewType.Groups,     ToolbarCommand.ViewTypeGroups,     "View.Groups",           "Groupes");
			this.buttonEvents        = this.CreateViewTypeButton (ViewType.Events,     ToolbarCommand.ViewTypeEvents,     "View.Events",           "Evénements");
			this.buttonReports       = this.CreateViewTypeButton (ViewType.Reports,    ToolbarCommand.ViewTypeReports,    "View.Reports",          "Rapports et statistiques");
			this.buttonSettings      = this.CreateViewTypeButton (ViewType.Settings,   ToolbarCommand.ViewTypeSettings,   "View.Settings",         "Réglages");
								     
			this.buttonSingle        = this.CreateViewModeButton (ViewMode.Single,     ToolbarCommand.ViewModeSingle,     "Show.TimelineSingle",   "Axe du temps de l'objet sélectionné");
			this.buttonEvent         = this.CreateViewModeButton (ViewMode.Event,      ToolbarCommand.ViewModeEvent,      "Show.TimelineEvent",    "Tableau des événements");
			this.buttonMultiple      = this.CreateViewModeButton (ViewMode.Multiple,   ToolbarCommand.ViewModeMultiple,   "Show.TimelineMultiple", "Axe du temps pour tous les objets");

			this.buttonEdit          = this.CreateCommandButton  (DockStyle.Left,      ToolbarCommand.Edit,               "Main.Edit",             "Edition");
			this.buttonAmortissement = this.CreateCommandButton  (DockStyle.Left,      ToolbarCommand.Amortissement,      "Main.Amortissement",    "Amortissements");
			this.buttonSimulation    = this.CreateCommandButton  (DockStyle.Left,      ToolbarCommand.Simulation,         "Main.Simulation",       "Simulation");
																 
			this.buttonCancel        = this.CreateCommandButton  (DockStyle.Right,     ToolbarCommand.Cancel,             "Edit.Cancel",           "Annuler les modifications");
			this.buttonAccept        = this.CreateCommandButton  (DockStyle.Right,     ToolbarCommand.Accept,             "Edit.Accept",           "Accepter les modifications");

			this.buttonOpen    .Margins = new Margins (0, 10, 0, 0);
			this.buttonSettings.Margins = new Margins (0, 10, 0, 0);
			this.buttonMultiple.Margins = new Margins (0, 40, 0, 0);

			this.UpdateViewTypeCommands ();
			this.UpdateViewModeCommands ();
			this.UpdateSimulation ();

			return toolbar;
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
			this.SetCommandState (ToolbarCommand.ViewTypeObjects,    this.viewType == ViewType.Objects    ? ToolbarCommandState.Activate : ToolbarCommandState.Enable);
			this.SetCommandState (ToolbarCommand.ViewTypeCategories, this.viewType == ViewType.Categories ? ToolbarCommandState.Activate : ToolbarCommandState.Enable);
			this.SetCommandState (ToolbarCommand.ViewTypeGroups,     this.viewType == ViewType.Groups     ? ToolbarCommandState.Activate : ToolbarCommandState.Enable);
			this.SetCommandState (ToolbarCommand.ViewTypeEvents,     this.viewType == ViewType.Events     ? ToolbarCommandState.Activate : ToolbarCommandState.Enable);
			this.SetCommandState (ToolbarCommand.ViewTypeReports,    this.viewType == ViewType.Reports    ? ToolbarCommandState.Activate : ToolbarCommandState.Enable);
			this.SetCommandState (ToolbarCommand.ViewTypeSettings,   this.viewType == ViewType.Settings   ? ToolbarCommandState.Activate : ToolbarCommandState.Enable);

			this.SetCommandState (ToolbarCommand.Open,          ToolbarCommandState.Enable);
			this.SetCommandState (ToolbarCommand.Amortissement, ToolbarCommandState.Enable);
			this.SetCommandState (ToolbarCommand.Simulation,    ToolbarCommandState.Enable);
		}

		private void UpdateViewModeCommands()
		{
			if (this.viewType == ViewType.Objects)
			{
				this.SetCommandState (ToolbarCommand.ViewModeSingle,   this.viewMode == ViewMode.Single   ? ToolbarCommandState.Activate : ToolbarCommandState.Enable);
				this.SetCommandState (ToolbarCommand.ViewModeEvent,    this.viewMode == ViewMode.Event    ? ToolbarCommandState.Activate : ToolbarCommandState.Enable);
				this.SetCommandState (ToolbarCommand.ViewModeMultiple, this.viewMode == ViewMode.Multiple ? ToolbarCommandState.Activate : ToolbarCommandState.Enable);
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


		private IconButton buttonOpen;

		private IconButton buttonObjects;
		private IconButton buttonCategories;
		private IconButton buttonGroups;
		private IconButton buttonEvents;
		private IconButton buttonReports;
		private IconButton buttonSettings;

		private IconButton buttonSingle;
		private IconButton buttonEvent;
		private IconButton buttonMultiple;

		private IconButton buttonEdit;
		private IconButton buttonAmortissement;
		private IconButton buttonSimulation;

		private IconButton buttonAccept;
		private IconButton buttonCancel;

		private ViewType viewType;
		private ViewMode viewMode;
		private int simulation;
	}
}

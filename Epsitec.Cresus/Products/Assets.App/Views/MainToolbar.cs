//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.App.Widgets;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class MainToolbar : AbstractCommandToolbar
	{
		public override void CreateUI(Widget parent)
		{
			this.viewType = ViewType.Objects;
			this.viewMode = ViewMode.Single;
			this.simulation = 0;

			this.CreateToolbar (parent, AbstractCommandToolbar.PrimaryToolbarHeight);
			this.UpdateCommandButtons ();
		}


		public ViewType ViewType
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
					this.UpdateViewTypeButtons ();
				}
			}
		}

		public ViewMode ViewMode
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
					this.UpdateViewModeButtons ();
				}
			}
		}

		public int Simulation
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


		protected override void UpdateCommandButtons()
		{
			this.UpdateCommandButton (this.buttonEdit,          ToolbarCommand.Edit);
			this.UpdateCommandButton (this.buttonAmortissement, ToolbarCommand.Amortissement);
			this.UpdateCommandButton (this.buttonSimulation,    ToolbarCommand.Simulation);

			this.UpdateCommandButton (this.buttonAccept,        ToolbarCommand.Accept);
			this.UpdateCommandButton (this.buttonCancel,        ToolbarCommand.Cancel);
		}


		protected override void CreateToolbar(Widget parent, int size)
		{
			var toolbar = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = size,
				BackColor       = ColorManager.ToolbarBackgroundColor,
			};

			this.buttonObjects    = this.CreateViewTypeButton (toolbar, ViewType.Objects,    "View.Objects",    "Objets d'immobilisation");
			this.buttonCategories = this.CreateViewTypeButton (toolbar, ViewType.Categories, "View.Categories", "Catégories d'immobilisations");
			this.buttonGroups     = this.CreateViewTypeButton (toolbar, ViewType.Groups,     "View.Groups",     "Sections");
			this.buttonEvents     = this.CreateViewTypeButton (toolbar, ViewType.Events,     "View.Events",     "Evénements");
			this.buttonReports    = this.CreateViewTypeButton (toolbar, ViewType.Reports,    "View.Reports",    "Rapports et statistiques");
			this.buttonSettings   = this.CreateViewTypeButton (toolbar, ViewType.Settings,   "View.Settings",   "Réglages");

			this.buttonSingle   = this.CreateViewModeButton (toolbar, ViewMode.Single,   ToolbarCommand.ViewModeSingle,   "Show.TimelineSingle",   "Axe du temps de l'objet sélectionné");
			this.buttonEvent    = this.CreateViewModeButton (toolbar, ViewMode.Event,    ToolbarCommand.ViewModeEvent,    "Show.TimelineEvent",    "Tableau des événements");
			this.buttonMultiple = this.CreateViewModeButton (toolbar, ViewMode.Multiple, ToolbarCommand.ViewModeMultiple, "Show.TimelineMultiple", "Axe du temps pour tous les objets");

			this.buttonEdit          = this.CreateCommandButton (toolbar, DockStyle.Left, ToolbarCommand.Edit,          "Main.Edit",          "Edition");
			this.buttonAmortissement = this.CreateCommandButton (toolbar, DockStyle.Left, ToolbarCommand.Amortissement, "Main.Amortissement", "Amortissements");
			this.buttonSimulation    = this.CreateCommandButton (toolbar, DockStyle.Left, ToolbarCommand.Simulation,    "Main.Simulation",    "Simulation");

			this.buttonCancel = this.CreateCommandButton (toolbar, DockStyle.Right, ToolbarCommand.Cancel, "Edit.Cancel", "Annuler les modifications");
			this.buttonAccept = this.CreateCommandButton (toolbar, DockStyle.Right, ToolbarCommand.Accept, "Edit.Accept", "Accepter les modifications");

			this.buttonSettings.Margins = new Margins (0, 40, 0, 0);
			this.buttonMultiple.Margins = new Margins (0, 40, 0, 0);

			this.UpdateViewTypeButtons ();
			this.UpdateViewModeButtons ();
			this.UpdateSimulation ();
		}

		private IconButton CreateViewTypeButton(FrameBox toolbar, ViewType view, string icon, string tooltip)
		{
			var size = toolbar.PreferredHeight;

			var button = new IconButton
			{
				Parent        = toolbar,
				ButtonStyle   = ButtonStyle.ActivableIcon,
				AutoFocus     = false,
				Dock          = DockStyle.Left,
				IconUri       = MainToolbar.GetResourceIconUri (icon),
				PreferredSize = new Size (size, size),
			};

			ToolTip.Default.SetToolTip (button, tooltip);

			button.Clicked += delegate
			{
				this.viewType = view;
				this.UpdateViewTypeButtons ();
				this.OnViewChanged (this.viewType);
			};

			return button;
		}

		private IconButton CreateViewModeButton(FrameBox toolbar, ViewMode view, ToolbarCommand command, string icon, string tooltip)
		{
			var size = toolbar.PreferredHeight;

			var button = new IconButton
			{
				Parent        = toolbar,
				ButtonStyle   = ButtonStyle.ActivableIcon,
				AutoFocus     = false,
				Dock          = DockStyle.Left,
				IconUri       = MainToolbar.GetResourceIconUri (icon),
				PreferredSize = new Size (size, size),
			};

			ToolTip.Default.SetToolTip (button, tooltip);

			button.Clicked += delegate
			{
				this.viewMode = view;
				this.UpdateViewModeButtons ();
				this.OnCommandClicked (command);
			};

			return button;
		}

		private void UpdateViewTypeButtons()
		{
			this.SetActiveState (this.buttonObjects,    this.viewType == ViewType.Objects);
			this.SetActiveState (this.buttonCategories, this.viewType == ViewType.Categories);
			this.SetActiveState (this.buttonGroups,     this.viewType == ViewType.Groups);
			this.SetActiveState (this.buttonEvents,     this.viewType == ViewType.Events);
			this.SetActiveState (this.buttonReports,    this.viewType == ViewType.Reports);
			this.SetActiveState (this.buttonSettings,   this.viewType == ViewType.Settings);

			this.SetCommandState (ToolbarCommand.Amortissement, ToolbarCommandState.Enable);
			this.SetCommandState (ToolbarCommand.Simulation,    ToolbarCommandState.Enable);
		}

		private void UpdateViewModeButtons()
		{
			this.SetActiveState (this.buttonSingle,   this.viewMode == ViewMode.Single);
			this.SetActiveState (this.buttonEvent,    this.viewMode == ViewMode.Event);
			this.SetActiveState (this.buttonMultiple, this.viewMode == ViewMode.Multiple);
		}

		private void UpdateSimulation()
		{
			string icon = "Main.Simulation";

			if (this.simulation >= 0)
			{
				icon += this.simulation.ToString (System.Globalization.CultureInfo.InvariantCulture);
			}

			this.buttonSimulation.IconUri = AbstractCommandToolbar.GetResourceIconUri (icon);
		}


		#region Events handler
		private void OnViewChanged(ViewType viewType)
		{
			if (this.ViewChanged != null)
			{
				this.ViewChanged (this, viewType);
			}
		}

		public delegate void ViewChangedEventHandler(object sender, ViewType viewType);
		public event ViewChangedEventHandler ViewChanged;
		#endregion


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

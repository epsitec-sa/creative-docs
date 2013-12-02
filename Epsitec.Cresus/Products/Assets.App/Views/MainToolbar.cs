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
		public override FrameBox CreateUI(Widget parent)
		{
			this.viewType = ViewType.Objects;
			this.viewMode = ViewMode.Single;
			this.simulation = 0;

			var toolbar = this.CreateToolbar (parent, AbstractCommandToolbar.PrimaryToolbarHeight);
			this.UpdateCommandButtons ();

			return toolbar;
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
					this.UpdateViewModeButtons ();
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
			this.UpdateCommandButton (this.buttonSingle,        ToolbarCommand.ViewModeSingle);
			this.UpdateCommandButton (this.buttonEvent,         ToolbarCommand.ViewModeEvent);
			this.UpdateCommandButton (this.buttonMultiple,      ToolbarCommand.ViewModeMultiple);

			this.UpdateCommandButton (this.buttonOpen,          ToolbarCommand.Open);
			this.UpdateCommandButton (this.buttonEdit,          ToolbarCommand.Edit);
			this.UpdateCommandButton (this.buttonAmortissement, ToolbarCommand.Amortissement);
			this.UpdateCommandButton (this.buttonSimulation,    ToolbarCommand.Simulation);

			this.UpdateCommandButton (this.buttonAccept,        ToolbarCommand.Accept);
			this.UpdateCommandButton (this.buttonCancel,        ToolbarCommand.Cancel);
		}


		protected override FrameBox CreateToolbar(Widget parent, int size)
		{
			var toolbar = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = size,
				BackColor       = ColorManager.ToolbarBackgroundColor,
			};

			this.buttonOpen = this.CreateCommandButton (toolbar, DockStyle.Left, ToolbarCommand.Open, "Main.Open", "Ouvrir");

			this.buttonObjects    = this.CreateViewTypeButton (toolbar, ViewType.Objects,    "View.Objects",    "Objets d'immobilisation");
			this.buttonCategories = this.CreateViewTypeButton (toolbar, ViewType.Categories, "View.Categories", "Catégories d'immobilisations");
			this.buttonGroups     = this.CreateViewTypeButton (toolbar, ViewType.Groups,     "View.Groups",     "Groupes");
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

			this.buttonOpen    .Margins = new Margins (0, 10, 0, 0);
			this.buttonSettings.Margins = new Margins (0, 10, 0, 0);
			this.buttonMultiple.Margins = new Margins (0, 40, 0, 0);

			this.UpdateViewTypeButtons ();
			this.UpdateViewModeButtons ();
			this.UpdateSimulation ();

			return toolbar;
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
				this.ViewType = view;
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
				this.ViewMode = view;
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

			this.SetCommandState (ToolbarCommand.Open,          ToolbarCommandState.Enable);
			this.SetCommandState (ToolbarCommand.Amortissement, ToolbarCommandState.Enable);
			this.SetCommandState (ToolbarCommand.Simulation,    ToolbarCommandState.Enable);
		}

		private void UpdateViewModeButtons()
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

			this.buttonSimulation.IconUri = AbstractCommandToolbar.GetResourceIconUri (icon);
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

//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class MainToolbar
	{
		public MainToolbar()
		{
			this.commandStates = new Dictionary<ToolbarCommand, ToolbarCommandState> ();
		}

		public void CreateUI(Widget parent)
		{
			this.viewType = ViewType.Objects;

			this.CreateToolbar (parent, 32+8);  // les icônes actuelles font 32
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
					this.UpdateViewButtons ();
				}
			}
		}


		public void SetCommandState(ToolbarCommand command, ToolbarCommandState state)
		{
			this.commandStates[command] = state;
			this.UpdateCommandButtons ();
		}

		public ToolbarCommandState GetCommandState(ToolbarCommand command)
		{
			if (this.commandStates.ContainsKey (command))
			{
				return this.commandStates[command];
			}
			else
			{
				return ToolbarCommandState.Hide;
			}
		}


		private void UpdateCommandButtons()
		{
			this.UpdateCommandButton (this.buttonNew,    ToolbarCommand.New);
			this.UpdateCommandButton (this.buttonDelete, ToolbarCommand.Delete);
			this.UpdateCommandButton (this.buttonEdit,   ToolbarCommand.Edit);
			this.UpdateCommandButton (this.buttonAccept, ToolbarCommand.Accept);
			this.UpdateCommandButton (this.buttonCancel, ToolbarCommand.Cancel);
		}

		private void UpdateCommandButton(IconButton button, ToolbarCommand command)
		{
			button.Visibility = this.GetCommandVisibility (command);
			button.Enable     = this.GetCommandEnable     (command);
		}

		private bool GetCommandVisibility(ToolbarCommand command)
		{
			var state = this.GetCommandState (command);
			return state != ToolbarCommandState.Hide;
		}

		private bool GetCommandEnable(ToolbarCommand command)
		{
			var state = this.GetCommandState (command);
			return state == ToolbarCommandState.Enable;
		}


		private void CreateToolbar(Widget parent, int size)
		{
			var toolbar = new HToolBar
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = size,
				Padding         = new Margins (0),
			};

			this.buttonObjects    = this.CreateViewButton (toolbar, ViewType.Objects,    "View.Objects",    "Objets d'immobilisation");
			this.buttonCategories = this.CreateViewButton (toolbar, ViewType.Categories, "View.Categories", "Catégories d'immobilisations");
			this.buttonGroups     = this.CreateViewButton (toolbar, ViewType.Groups,     "View.Groups",     "Sections");
			this.buttonEvents     = this.CreateViewButton (toolbar, ViewType.Events,     "View.Events",     "Evénements");
			this.buttonReports    = this.CreateViewButton (toolbar, ViewType.Reports,    "View.Reports",    "Rapports et statistiques");
			this.buttonSettings   = this.CreateViewButton (toolbar, ViewType.Settings,   "View.Settings",   "Réglages");

			this.buttonNew        = this.CreateCommandButton (toolbar, DockStyle.Left,  ToolbarCommand.New ,    "Object.New",    "Nouveau");
			this.buttonDelete     = this.CreateCommandButton (toolbar, DockStyle.Left,  ToolbarCommand.Delete , "Object.Delete", "Supprimer");
			this.buttonEdit       = this.CreateCommandButton (toolbar, DockStyle.Left,  ToolbarCommand.Edit ,   "Object.Edit",   "Modifier");

			this.buttonCancel     = this.CreateCommandButton (toolbar, DockStyle.Right, ToolbarCommand.Cancel , "Edit.Cancel",   "Annuler les modifications");
			this.buttonAccept     = this.CreateCommandButton (toolbar, DockStyle.Right, ToolbarCommand.Accept , "Edit.Accept",   "Accepter les modifications");

			this.buttonNew.Margins = new Margins (20, 0, 0, 0);

			this.UpdateViewButtons ();
		}

		private IconButton CreateViewButton(HToolBar toolbar, ViewType view, string icon, string tooltip)
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
				this.UpdateViewButtons ();
				this.OnViewChanged (this.viewType);
			};

			return button;
		}

		private void UpdateViewButtons()
		{
			this.SetActiveState (this.buttonObjects,    this.viewType == ViewType.Objects);
			this.SetActiveState (this.buttonCategories, this.viewType == ViewType.Categories);
			this.SetActiveState (this.buttonGroups,     this.viewType == ViewType.Groups);
			this.SetActiveState (this.buttonEvents,     this.viewType == ViewType.Events);
			this.SetActiveState (this.buttonReports,    this.viewType == ViewType.Reports);
			this.SetActiveState (this.buttonSettings,   this.viewType == ViewType.Settings);
		}

		private void SetActiveState(IconButton button, bool state)
		{
			button.ActiveState = state ? ActiveState.Yes : ActiveState.No;
		}

		private IconButton CreateCommandButton(HToolBar toolbar, DockStyle dock, ToolbarCommand command, string icon, string tooltip)
		{
			var size = toolbar.PreferredHeight;

			var button = new IconButton
			{
				Parent        = toolbar,
				AutoFocus     = false,
				Dock          = dock,
				IconUri       = MainToolbar.GetResourceIconUri (icon),
				PreferredSize = new Size (size, size),
			};

			ToolTip.Default.SetToolTip (button, tooltip);

			button.Clicked += delegate
			{
				this.OnCommandClicked (command);
			};

			return button;
		}


		private static string GetResourceIconUri(string icon)
		{
			if (string.IsNullOrEmpty (icon))
			{
				return null;
			}
			else if (icon.Contains (':'))
			{
				return FormattedText.Escape (icon);
			}
			else
			{
				return string.Format ("manifest:Epsitec.Cresus.Assets.App.Images.{0}.icon", FormattedText.Escape (icon));
			}
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


		private void OnCommandClicked(ToolbarCommand command)
		{
			if (this.CommandClicked != null)
			{
				this.CommandClicked (this, command);
			}
		}

		public delegate void CommandClickedEventHandler(object sender, ToolbarCommand command);
		public event CommandClickedEventHandler CommandClicked;
		#endregion


		private readonly Dictionary<ToolbarCommand, ToolbarCommandState> commandStates;

		private IconButton buttonObjects;
		private IconButton buttonCategories;
		private IconButton buttonGroups;
		private IconButton buttonEvents;
		private IconButton buttonReports;
		private IconButton buttonSettings;

		private IconButton buttonNew;
		private IconButton buttonDelete;

		private IconButton buttonEdit;
		private IconButton buttonAccept;
		private IconButton buttonCancel;

		private ViewType viewType;
	}
}

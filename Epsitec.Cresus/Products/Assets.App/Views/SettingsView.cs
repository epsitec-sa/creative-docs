//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class SettingsView : AbstractView
	{
		public SettingsView(DataAccessor accessor, MainToolbar toolbar)
			: base (accessor, toolbar)
		{
			this.selectedCommand = ToolbarCommand.SettingsGeneral;
		}

		public override void CreateUI(Widget parent)
		{
			this.topTitle = new TopTitle
			{
				Parent = parent,
			};

			this.topTitle.SetTitle (StaticDescriptions.GetViewTypeDescription (ViewType.Settings));

			this.toolbar = new SettingsToolbar ();
			this.toolbar.CreateUI (parent);

			this.settingsViewFrame = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};

			//	Connexion des événements de la toolbar.
			this.toolbar.CommandClicked += delegate (object sender, ToolbarCommand command)
			{
				this.selectedCommand = command;
				this.DeepUpdateUI ();
				this.OnViewStateChanged (this.ViewState);
			};

			this.DeepUpdateUI ();
		}


		public override void DeepUpdateUI()
		{
			this.DataChanged ();
			this.UpdateUI ();
		}

		public override void UpdateUI()
		{
			this.CreateSettingsView ();
			this.UpdateToolbar ();

			this.OnViewStateChanged (this.ViewState);
		}


		public override AbstractViewState ViewState
		{
			get
			{
				return new SettingsViewState
				{
					ViewType        = ViewType.Settings,
					SelectedCommand = this.selectedCommand,
				};
			}
			set
			{
				var viewState = value as SettingsViewState;
				System.Diagnostics.Debug.Assert (viewState != null);

				this.selectedCommand = viewState.SelectedCommand;

				this.UpdateUI ();
			}
		}


		private void CreateSettingsView()
		{
			this.settingsViewFrame.Children.Clear ();

			this.settingsView = AbstractSettingsView.CreateView (this.accessor, this.mainToolbar, this.selectedCommand);
			this.settingsView.CreateUI (this.settingsViewFrame);
		}

		private void UpdateToolbar()
		{
			this.toolbar.UpdateToolbar (this.selectedCommand);
		}


		private TopTitle						topTitle;
		private SettingsToolbar					toolbar;
		private ToolbarCommand					selectedCommand;
		private FrameBox						settingsViewFrame;
		private AbstractSettingsView			settingsView;
	}
}

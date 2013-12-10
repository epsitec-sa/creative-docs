//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class MainView
	{
		public MainView(DataAccessor accessor)
		{
			this.accessor = accessor;

			this.historyViewStates = new List<ViewState> ();
			this.historyPosition = -1;
		}

		public void CreateUI(Widget parent)
		{
			this.parent = parent;

			MouseCursorManager.SetWindow (parent.Window);

			this.toolbar = new MainToolbar ();
			this.toolbar.CreateUI (parent);

			this.viewBox = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};

			this.toolbar.ViewChanged += delegate (object sender, ViewType viewType)
			{
				this.UpdateView ();
			};

			this.toolbar.CommandClicked += delegate (object sender, ToolbarCommand command)
			{
				switch (command)
				{
					case ToolbarCommand.Open:
						this.OnOpen ();
						break;

					case ToolbarCommand.NavigateBack:
						this.OnNavigateBack ();
						break;

					case ToolbarCommand.NavigateForward:
						this.OnNavigateForward ();
						break;

					default:
						if (this.view != null)
						{
							this.view.OnCommand (command);
						}
						break;
				}
			};

			this.UpdateView ();
			this.UpdateToolbar ();
		}


		private void UpdateView()
		{
			this.viewBox.Children.Clear ();

			if (this.view != null)
			{
				this.view.Goto -= this.HandleViewGoto;
				this.view.SaveViewState -= HandleViewSave;
				this.view.Dispose ();
				this.view = null;
			}

			this.view = AbstractView.CreateView (this.toolbar.ViewType, this.accessor, this.toolbar);

			if (this.view != null)
			{
				this.view.CreateUI (this.viewBox);
				this.view.Goto += this.HandleViewGoto;
				this.view.SaveViewState += HandleViewSave;
			}
		}

		private void HandleViewGoto(object sender, ViewState viewState)
		{
			this.RestoreViewState (viewState);
		}

		private void HandleViewSave(object sender, ViewState viewState)
		{
			this.SaveViewState (viewState);
		}


		private void OnOpen()
		{
			this.ShowPopup ();
		}

		private void OnNavigateBack()
		{
			this.GoHistoryBack ();
		}

		private void OnNavigateForward()
		{
			this.GoHistoryForward ();
		}


		private void ShowPopup()
		{
			var target = this.toolbar.GetTarget (ToolbarCommand.Open);

			var popup = new SimplePopup ()
			{
				SelectedItem = AssetsApplication.SelectedMandat,
			};

			for (int i=0; i<AssetsApplication.MandatCount; i++)
			{
				var mandat = AssetsApplication.GetMandat (i);
				popup.Items.Add ("Ouvrir le mandat \"" + mandat.Name + "\"");
			}

			popup.Create (target, leftOrRight: false);

			popup.ItemClicked += delegate (object sender, int rank)
			{
				this.OpenMandat (rank);
			};
		}

		private void OpenMandat(int rank)
		{
			AssetsApplication.SelectedMandat = rank;
			this.accessor.Mandat = AssetsApplication.GetMandat (rank);

			this.UpdateView ();
		}


		private void UpdateToolbar()
		{
			this.toolbar.SetCommandState (ToolbarCommand.Open,             ToolbarCommandState.Enable);

			this.toolbar.SetCommandEnable (ToolbarCommand.NavigateBack,    this.NavigateBackEnable);
			this.toolbar.SetCommandEnable (ToolbarCommand.NavigateForward, this.NavigateForwardEnable);

			this.toolbar.SetCommandState (ToolbarCommand.Amortissement,    ToolbarCommandState.Enable);
			this.toolbar.SetCommandState (ToolbarCommand.Simulation,       ToolbarCommandState.Enable);
		}


		private void SaveViewState(ViewState viewState)
		{
			if (this.NavigateBackEnable && viewState == this.historyViewStates[this.historyPosition])
			{
				return;
			}

			while (this.historyPosition < this.historyViewStates.Count-1)
			{
				this.historyViewStates.RemoveAt (this.historyViewStates.Count-1);
			}

			this.historyViewStates.Add (viewState);
			this.historyPosition = this.historyViewStates.Count-1;

			this.UpdateToolbar ();
		}

		private void GoHistoryBack()
		{
			if (this.NavigateBackEnable)
			{
				this.RestoreViewState (this.historyViewStates[--this.historyPosition]);
			}
		}

		private void GoHistoryForward()
		{
			if (this.NavigateForwardEnable)
			{
				this.RestoreViewState (this.historyViewStates[++this.historyPosition]);
			}
		}

		private void RestoreViewState(ViewState viewState)
		{
			this.toolbar.ViewType = viewState.ViewType;
			this.UpdateView ();

			this.view.ViewState = viewState;
			this.UpdateToolbar ();
		}

		private bool NavigateBackEnable
		{
			get
			{
				return this.historyPosition >= 0;
			}
		}

		private bool NavigateForwardEnable
		{
			get
			{
				return this.historyPosition < this.historyViewStates.Count-1;
			}
		}


		private readonly DataAccessor			accessor;
		private readonly List<ViewState>		historyViewStates;

		private Widget							parent;
		private MainToolbar						toolbar;
		private FrameBox						viewBox;
		private AbstractView					view;
		private int								historyPosition;
	}
}

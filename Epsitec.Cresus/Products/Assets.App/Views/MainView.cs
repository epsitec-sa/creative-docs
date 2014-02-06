﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;
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

			this.currentViewStates = new List<AbstractViewState> ();
			this.historyViewStates = new List<AbstractViewState> ();
			this.lastViewStates    = new List<AbstractViewState> ();
			this.historyPosition = -1;

			this.ignoreChanges = new SafeCounter ();
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
				this.CreateView (viewType);
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

					case ToolbarCommand.NavigateMenu:
						this.OnNavigateMenu ();
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

			this.CreateView (ViewType.Objects);
			this.UpdateToolbar ();
		}


		private void CreateView(ViewType viewType, bool pushViewState = true)
		{
			this.viewBox.Children.Clear ();

			if (this.view != null)
			{
				this.SaveCurrentViewState ();

				this.view.Goto -= this.HandleViewGoto;
				this.view.ViewStateChanged -= this.HandleViewStateChanged;
				this.view.Dispose ();
				this.view = null;
			}

			this.view = AbstractView.CreateView (viewType, this.accessor, this.toolbar);

			if (this.view != null)
			{
				this.view.CreateUI (this.viewBox);
				this.view.Goto += this.HandleViewGoto;
				this.view.ViewStateChanged += this.HandleViewStateChanged;

				this.RestoreCurrentViewState ();

				if (pushViewState)
				{
					this.PushViewState (this.view.ViewState);
				}
			}

			this.toolbar.ViewType = viewType;
		}

		private void HandleViewGoto(object sender, AbstractViewState viewState)
		{
			this.RestoreViewState (viewState);
		}

		private void HandleViewStateChanged(object sender, AbstractViewState viewState)
		{
			this.PushViewState (viewState);
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

		private void OnNavigateMenu()
		{
			this.ShowLastViewsPopup ();
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

			this.CreateView (ViewType.Objects);
		}


		private void UpdateToolbar()
		{
			this.toolbar.SetCommandState (ToolbarCommand.Open,             ToolbarCommandState.Enable);

			this.toolbar.SetCommandEnable (ToolbarCommand.NavigateBack,    this.NavigateBackEnable);
			this.toolbar.SetCommandEnable (ToolbarCommand.NavigateMenu,    this.NavigateMenuEnable);
			this.toolbar.SetCommandEnable (ToolbarCommand.NavigateForward, this.NavigateForwardEnable);

			this.toolbar.SetCommandState (ToolbarCommand.Amortissement,    ToolbarCommandState.Enable);
			this.toolbar.SetCommandState (ToolbarCommand.Simulation,       ToolbarCommandState.Enable);
		}


		private void SaveCurrentViewState()
		{
			//	Sauvegarde le ViewState actuellement utilisé.
			if (this.view.ViewState != null)
			{
				var last = this.currentViewStates.Where (x => x.ViewType == this.view.ViewState.ViewType).FirstOrDefault ();
				if (last != null)
				{
					this.currentViewStates.Remove (last);
				}

				this.currentViewStates.Add (this.view.ViewState);
			}
		}

		private void RestoreCurrentViewState()
		{
			//	Restitue le dernier ViewState utilisé par la vue.
			if (this.view.ViewState != null)
			{
				var last = this.currentViewStates.Where (x => x.ViewType == this.view.ViewState.ViewType).FirstOrDefault ();
				if (last != null)
				{
					using (this.ignoreChanges.Enter ())
					{
						this.view.ViewState = last;
					}
				}
			}
		}


		private void SaveLastViewState(AbstractViewState viewState)
		{
			//	Sauvegarde si nécessaire le ViewState utilisé par la vue dans
			//	la liste des 100 derniers. Les derniers utilisés sont placés
			//	en tête de liste.
			if (viewState == null)
			{
				return;
			}

			int index = this.lastViewStates.FindIndex (x => x.Equals (viewState));
			if (index != -1)
			{
				this.lastViewStates.RemoveAt (index);
			}

			this.lastViewStates.Insert (0, viewState);

			while (this.lastViewStates.Count > 100)
			{
				this.lastViewStates.RemoveAt (this.lastViewStates.Count-1);
			}
		}


		private void PushViewState(AbstractViewState viewState)
		{
			if (viewState == null)
			{
				return;
			}

			if (this.ignoreChanges.IsZero)
			{
				if (this.historyPosition >= 0 &&
					viewState.Equals (this.historyViewStates[this.historyPosition]))
				{
					return;
				}

				this.SaveLastViewState (viewState);

				while (this.historyPosition < this.historyViewStates.Count-1)
				{
					this.historyViewStates.RemoveAt (this.historyViewStates.Count-1);
				}

				this.historyViewStates.Add (viewState);
				this.historyPosition = this.historyViewStates.Count-1;

				this.UpdateToolbar ();
			}
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

		private void ShowLastViewsPopup()
		{
			var navigationGuid = this.lastViewStates
				.Where (x => x.Equals (this.view.ViewState))
				.Select (x => x.Guid)
				.FirstOrDefault ();

			var target = this.toolbar.GetTarget (ToolbarCommand.NavigateMenu);
			var popup = new LastViewsPopup (this.accessor, this.lastViewStates, navigationGuid);

			popup.Create (target);

			popup.Navigate += delegate (object sender, Guid guid)
			{
				if (!guid.IsEmpty)
				{
					var viewState = this.lastViewStates.Where (x => x.Guid == guid).FirstOrDefault ();
					this.RestoreViewState (viewState);
				}
			};
		}

		private void RestoreViewState(AbstractViewState viewState)
		{
			this.CreateView (viewState.ViewType, pushViewState: false);
			this.view.ViewState = viewState;

			this.UpdateToolbar ();
		}

		private bool NavigateBackEnable
		{
			get
			{
				return this.historyPosition > 0;
			}
		}

		private bool NavigateForwardEnable
		{
			get
			{
				return this.historyPosition < this.historyViewStates.Count-1;
			}
		}

		private bool NavigateMenuEnable
		{
			get
			{
				return this.lastViewStates.Count > 1;
			}
		}


		private readonly DataAccessor			accessor;
		private readonly List<AbstractViewState> currentViewStates;
		private readonly List<AbstractViewState> historyViewStates;
		private readonly List<AbstractViewState> lastViewStates;
		private readonly SafeCounter			ignoreChanges;

		private Widget							parent;
		private MainToolbar						toolbar;
		private FrameBox						viewBox;
		private AbstractView					view;
		private int								historyPosition;
	}
}

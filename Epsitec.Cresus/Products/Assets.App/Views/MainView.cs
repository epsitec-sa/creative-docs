//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Settings;
using Epsitec.Cresus.Assets.App.Views.CommandToolbars;
using Epsitec.Cresus.Assets.App.Views.ViewStates;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.Engine;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class MainView
	{
		public MainView(DataAccessor accessor, CommandDispatcher commandDispatcher, CommandContext commandContext)
		{
			this.accessor          = accessor;
			this.commandDispatcher = commandDispatcher;
			this.commandContext    = commandContext;

			this.currentViewStates = new List<AbstractViewState> ();
			this.historyViewStates = new List<AbstractViewState> ();
			this.lastViewStates    = new List<AbstractViewState> ();
			this.historyPosition   = -1;

			this.ignoreChanges = new SafeCounter ();

			this.commandDispatcher.RegisterController (this);  // nécesaire pour [Command (Res.CommandIds...)]
		}

		public void CreateUI(Widget parent)
		{
			CommandDispatcher.SetDispatcher (parent, this.commandDispatcher);  // nécesaire pour [Command (Res.CommandIds...)]
			this.parent = parent;

			MouseCursorManager.SetWindow (parent.Window);

			this.toolbar = new MainToolbar (this.accessor, this.commandContext);
			this.toolbar.CreateUI (parent);

			this.viewBox = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};

			this.toolbar.ChangeView += delegate
			{
				this.UpdateViewState ();
				this.CreateView (this.toolbar.ViewType);
			};

			this.CreateView (ViewType.Assets);
			this.UpdateToolbar ();
		}


		private void CreateView(ViewType viewType, bool pushViewState = true)
		{
			this.viewBox.Children.Clear ();

			if (this.view != null)
			{
				using (this.ignoreChanges.Enter ())
				{
					//?this.view.UpdateUI ();  // il faut effectuer SaveObjectEdition !
					this.view.CloseUI ();
				}

				this.SaveCurrentViewState ();
				this.DeleteView ();
			}

			this.view = AbstractView.CreateView (viewType, this.accessor, this.commandContext, this.toolbar, this.historyViewStates);

			if (this.view != null)
			{
				this.view.CreateUI (this.viewBox);
				this.view.Goto             += this.HandleViewGoto;
				this.view.ViewStateChanged += this.HandleViewStateChanged;
				this.view.ChangeView       += this.HandleChangeView;

				this.RestoreCurrentViewState ();

				if (pushViewState)
				{
					this.PushViewState (this.view.ViewState);
				}
			}

			this.toolbar.ViewType = viewType;
		}

		private void DeleteView()
		{
			if (this.view != null)
			{
				this.view.Goto             -= this.HandleViewGoto;
				this.view.ViewStateChanged -= this.HandleViewStateChanged;
				this.view.ChangeView       -= this.HandleChangeView;
				this.view.Dispose ();
				this.view = null;
			}
		}

		private void HandleViewGoto(object sender, AbstractViewState viewState)
		{
			this.UpdateViewState ();
			this.RestoreViewState (viewState);
		}

		private void HandleViewStateChanged(object sender, AbstractViewState viewState)
		{
			this.PushViewState (viewState);
		}

		private void HandleChangeView(object sender, ViewType viewType)
		{
			this.UpdateViewState ();
			this.CreateView (viewType);
		}


		[Command (Res.CommandIds.Main.New)]
		private void OnNew(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.toolbar.GetTarget (e);
			this.ShowCreateMandatPopup (target);
		}

		[Command (Res.CommandIds.Main.Navigate.Back)]
		private void OnNavigateBack(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.GoHistoryBack ();
		}

		[Command (Res.CommandIds.Main.Navigate.Forward)]
		private void OnNavigateForward(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.GoHistoryForward ();
		}

		[Command (Res.CommandIds.Main.Navigate.Menu)]
		private void OnNavigateMenu(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.toolbar.GetTarget (e);
			this.ShowLastViewsPopup (target);
		}


		private void ShowCreateMandatPopup(Widget target)
		{
			var popup = new NewMandatPopup (this.accessor)
			{
				MandatFactoryName = MandatFactory.Factories.Where (x => x.IsDefault).FirstOrDefault ().Name,
				MandatWithSamples = false,
				MandatStartDate   = LocalSettings.CreateMandatDate,
			};

			popup.Create (target, leftOrRight: false);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					LocalSettings.CreateMandatDate = popup.MandatStartDate;
					this.CreateMandat (popup.MandatFactoryName, popup.MandatName, popup.MandatStartDate, popup.MandatWithSamples);
				}
			};
		}

		private void CreateMandat(string factoryName, string name, System.DateTime startDate, bool withSamples)
		{
			this.currentViewStates.Clear ();
			this.historyViewStates.Clear ();
			this.lastViewStates.Clear ();
			this.historyPosition = -1;

			var factory = MandatFactory.Factories.Where (x => x.Name == factoryName).FirstOrDefault ();
			System.Diagnostics.Debug.Assert (factory != null);
			factory.Create (this.accessor, name, startDate, withSamples);

			this.DeleteView ();
			this.CreateView (ViewType.Assets);
		}


		private void UpdateToolbar()
		{
			this.commandContext.GetCommandState (Res.Commands.Main.New             ).Enable = true;
			this.commandContext.GetCommandState (Res.Commands.Main.Navigate.Back   ).Enable = this.NavigateBackEnable;
			this.commandContext.GetCommandState (Res.Commands.Main.Navigate.Forward).Enable = this.NavigateForwardEnable;
			this.commandContext.GetCommandState (Res.Commands.Main.Navigate.Menu   ).Enable = this.NavigateMenuEnable;
			this.commandContext.GetCommandState (Res.Commands.Main.Locked          ).Enable = true;
			this.commandContext.GetCommandState (Res.Commands.Main.Simulation      ).Enable = true;
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

			//	Si le ViewState est déjà dans la liste, on le supprime.
			int index = this.lastViewStates.FindIndex (x => x.ApproximatelyEquals (viewState));
			if (index != -1)
			{
				viewState.Pin |= this.lastViewStates[index].Pin;  // conserve toujours la punaise
				this.lastViewStates.RemoveAt (index);
			}

			//	Insère le ViewState au début de la liste, dans le bon groupe "pin/unpin".
			index = 0;
			if (!viewState.Pin)
			{
				index = this.lastViewStates.FindIndex (x => !x.Pin);
				if (index == -1)
				{
					index = 0;
				}
			}

			this.lastViewStates.Insert (index, viewState);

			//	Si la liste dépasse le maximum utile, on la tronque.
			while (this.lastViewStates.Count > MainView.maxLastViewState)
			{
				this.lastViewStates.RemoveAt (this.lastViewStates.Count-1);
			}
		}

		private void SortLastViewStates()
		{
			//	Met toutes les vues punaisées en tête de liste.
			var orderedList = new List<AbstractViewState> ();

			orderedList.AddRange (this.lastViewStates.Where (x => x.Pin));
			orderedList.AddRange (this.lastViewStates.Where (x => !x.Pin));

			this.lastViewStates.Clear ();
			this.lastViewStates.AddRange (orderedList);
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
					viewState.StrictlyEquals (this.historyViewStates[this.historyPosition]))
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

		private void UpdateViewState()
		{
			var viewState = this.view.ViewState;

			if (viewState == null)
			{
				return;
			}

			this.SaveLastViewState (viewState);
			this.historyViewStates[this.historyPosition] = viewState;

			this.UpdateToolbar ();
		}

		private void GoHistoryBack()
		{
			if (this.NavigateBackEnable)
			{
				this.UpdateViewState ();
				this.RestoreViewState (this.historyViewStates[--this.historyPosition]);
			}
		}

		private void GoHistoryForward()
		{
			if (this.NavigateForwardEnable)
			{
				this.UpdateViewState ();
				this.RestoreViewState (this.historyViewStates[++this.historyPosition]);
			}
		}

		private void ShowLastViewsPopup(Widget target)
		{
			var navigationGuid = this.lastViewStates
				.Where (x => x.ApproximatelyEquals (this.view.ViewState))
				.Select (x => x.Guid)
				.FirstOrDefault ();

			var popup = new LastViewsPopup (this.accessor, this.lastViewStates, navigationGuid);

			popup.Create (target);

			popup.Navigate += delegate (object sender, Guid guid)
			{
				if (!guid.IsEmpty)
				{
					this.UpdateViewState ();

					var viewState = this.lastViewStates.Where (x => x.Guid == guid).FirstOrDefault ();
					this.RestoreViewState (viewState);
				}
			};

			popup.Closed += delegate
			{
				this.SortLastViewStates ();
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


		private const int maxLastViewState = 100;


		private readonly DataAccessor				accessor;
		private readonly CommandDispatcher			commandDispatcher;
		private readonly CommandContext				commandContext;
		private readonly List<AbstractViewState>	currentViewStates;  // pour retrouver une vue à l'identique
		private readonly List<AbstractViewState>	historyViewStates;  // pour les commandes back/forward
		private readonly List<AbstractViewState>	lastViewStates;     // pour le menu des dernières vues
		private readonly SafeCounter				ignoreChanges;

		private Widget								parent;
		private MainToolbar							toolbar;
		private FrameBox							viewBox;
		private AbstractView						view;
		private int									historyPosition;
	}
}

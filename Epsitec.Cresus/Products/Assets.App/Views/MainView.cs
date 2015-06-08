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
using Epsitec.Cresus.Assets.Server.Export;
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

			var cleaner = new CleanerAgent (this.accessor, this);
			this.accessor.CleanerAgents.Add (cleaner);
		}

		public void CreateUI(Widget parent)
		{
			CommandDispatcher.SetDispatcher (parent, this.commandDispatcher);  // nécesaire pour [Command (Res.CommandIds...)]
			this.parent = parent;

			MouseCursorManager.SetWindow (this.parent.Window);

			this.toolbar = new MainToolbar (this.accessor, this.commandContext);
			this.CreateBaseUI ();

			this.CreateFirstView ();
			this.UpdateToolbar ();
		}

		private void CreateBaseUI()
		{
			this.toolbar.CreateUI (this.parent);

			this.viewBox = new FrameBox
			{
				Parent = this.parent,
				Dock   = DockStyle.Fill,
			};

			this.toolbar.ChangeView += delegate
			{
				this.UpdateViewState ();
				this.CreateView (this.toolbar.ViewType);
			};

			this.toolbar.RebuildUI += delegate
			{
				var viewType = this.toolbar.ViewType;
				this.parent.Children.Clear ();
				this.accessor.WarningsDirty = true;
				this.CreateBaseUI ();
				this.UpdateViewState ();
				this.CreateView (viewType);
			};
		}


		private void CreateFirstView()
		{
			var list = LocalSettings.GetVisibleWarnings (this.accessor);
			this.CreateView (list.Any () ? ViewType.Warnings : ViewType.Assets);
			this.InitializeUndo ();
		}

		private void CreateView(ViewType viewType, bool pushViewState = true)
		{
			this.viewBox.Children.Clear ();

			if (this.view != null)
			{
				using (this.ignoreChanges.Enter ())
				{
					this.view.CloseUI ();  // il faut effectuer SaveObjectEdition !
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

		[Command (Res.CommandIds.Main.Open)]
		private void OnOpen(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.toolbar.GetTarget (e);
			this.ShowOpenMandatPopup (target);
		}

		[Command (Res.CommandIds.Main.Save)]
		private void OnSave(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.toolbar.GetTarget (e);

			if (string.IsNullOrEmpty (this.accessor.ComputerSettings.MandatDirectory) ||
				string.IsNullOrEmpty (this.accessor.ComputerSettings.MandatFilename))
			{
				//	Si le nom du fichier n'est pas connu, on effectue un 'SaveAs'.
				this.ShowSaveMandatPopup (target);
			}
			else
			{
				//	Effectue un enregistrement non interactif, sauf s'il y a une erreur.
				var path = System.IO.Path.Combine (this.accessor.ComputerSettings.MandatDirectory, this.accessor.ComputerSettings.MandatFilename);
				var mode = this.accessor.GlobalSettings.SaveMandatMode;

				this.SaveMandat (target, path, mode);
			}
		}

		[Command (Res.CommandIds.Main.SaveAs)]
		private void OnSaveAs(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.toolbar.GetTarget (e);
			this.ShowSaveMandatPopup (target);
		}

		[Command (Res.CommandIds.Main.ExportEntries)]
		private void OnExportEntries(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.toolbar.GetTarget (e);

			using (var ee = new ExportEntries (this.accessor))
			{
				try
				{
					var message = ee.ExportFiles ();
					int linesCount = ((message.Length - message.Replace ("<br/>", "").Length) / 5) + 1;
					MessagePopup.ShowMessage (target, Res.Commands.Main.ExportEntries.Description, message, 450, 60 + linesCount*15);
				}
				catch (System.Exception ex)
				{
					string message = TextLayout.ConvertToTaggedText (ex.Message);
					MessagePopup.ShowMessage (target, Res.Commands.Main.ExportEntries.Description, message);
					return;
				}
			}
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

		[Command (Res.CommandIds.Main.Undo)]
		private void OnUndo(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.PrepareForUndo ();

			var viewState = this.accessor.UndoManager.Undo () as AbstractViewState;
			this.RestoreUndoViewState (viewState);

			this.view.DeepUpdateUI ();
			this.UpdateToolbar ();
		}

		[Command (Res.CommandIds.Main.UndoHistory)]
		private void OnUndoHistory(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.toolbar.GetTarget (e);
			UndoListPopup.Show (target, this.accessor, this.accessor.UndoManager.UndoHistory, true, delegate (int count)
			{
				this.PrepareForUndo ();

				for (int i=0; i<count; i++)
				{
					var viewState = this.accessor.UndoManager.Undo () as AbstractViewState;

					if (i == count-1)  // dernier ?
					{
						this.RestoreUndoViewState (viewState);
					}
				}

				this.view.DeepUpdateUI ();
				this.UpdateToolbar ();
			});
		}

		[Command (Res.CommandIds.Main.Redo)]
		private void OnRedo(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.PrepareForUndo ();

			var viewState = this.accessor.UndoManager.Redo () as AbstractViewState;
			this.RestoreUndoViewState (viewState);

			this.view.DeepUpdateUI ();
			this.UpdateToolbar ();
		}

		[Command (Res.CommandIds.Main.RedoHistory)]
		private void OnRedoHistory(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.toolbar.GetTarget (e);
			UndoListPopup.Show (target, this.accessor, this.accessor.UndoManager.RedoHistory, false, delegate (int count)
			{
				this.PrepareForUndo ();

				for (int i=0; i<count; i++)
				{
					var viewState = this.accessor.UndoManager.Redo () as AbstractViewState;

					if (i == count-1)  // dernier ?
					{
						this.RestoreUndoViewState (viewState);
					}
				}

				this.view.DeepUpdateUI ();
				this.UpdateToolbar ();
			});
		}


		private void PrepareForUndo()
		{
			this.accessor.EditionAccessor.SaveObjectEdition ();
			this.accessor.EditionAccessor.CancelObjectEdition ();
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
					this.accessor.ComputerSettings.MandatFilename = null;
				}
			};
		}

		private void ShowOpenMandatPopup(Widget target)
		{
			OpenMandatPopup.Show (this.accessor, target,
				this.accessor.ComputerSettings.MandatDirectory,
				this.accessor.ComputerSettings.MandatFilename,
				delegate (string path)
			{
				this.accessor.ComputerSettings.MandatDirectory = System.IO.Path.GetDirectoryName(path);
				this.accessor.ComputerSettings.MandatFilename  = System.IO.Path.GetFileName (path);

				var err = this.OpenMandat (path);
				if (!string.IsNullOrEmpty (err))
				{
					err = TextLayout.ConvertToTaggedText (err);
					MessagePopup.ShowError (target, err);
				}
			});
		}

		private void ShowSaveMandatPopup(Widget target)
		{
			SaveMandatPopup.Show (this.accessor, target,
				this.accessor.ComputerSettings.MandatDirectory,
				this.accessor.ComputerSettings.MandatFilename,
				this.accessor.GlobalSettings.SaveMandatMode,
				delegate (string path, SaveMandatMode mode)
			{
				this.accessor.ComputerSettings.MandatDirectory = System.IO.Path.GetDirectoryName (path);
				this.accessor.ComputerSettings.MandatFilename  = System.IO.Path.GetFileName (path);
				this.accessor.GlobalSettings.SaveMandatMode = mode;

				this.SaveMandat (target, path, mode);
			});
		}

		private void SaveMandat(Widget target, string path, SaveMandatMode mode)
		{
			var err = this.SaveMandat (path, mode);
			if (!string.IsNullOrEmpty (err))
			{
				err = TextLayout.ConvertToTaggedText (err);
				MessagePopup.ShowError (target, err);
			}
		}

		private void CreateMandat(string factoryName, string name, System.DateTime startDate, bool withSamples)
		{
			this.currentViewStates.Clear ();
			this.historyViewStates.Clear ();
			this.lastViewStates.Clear ();
			this.historyPosition = -1;

			LocalSettings.Initialize (Timestamp.Now.Date);

			var factory = MandatFactory.Factories.Where (x => x.Name == factoryName).FirstOrDefault ();
			System.Diagnostics.Debug.Assert (factory != null);
			factory.Create (this.accessor, name, startDate, withSamples);

			this.DeleteView ();
			this.CreateFirstView ();
		}

		private string OpenMandat(string filename)
		{
			this.currentViewStates.Clear ();
			this.historyViewStates.Clear ();
			this.lastViewStates.Clear ();
			this.historyPosition = -1;

			var err = AssetsApplication.OpenMandat (this.accessor, filename);

			if (!string.IsNullOrEmpty (err))  // erreur ?
			{
				return err;
			}

			this.DeleteView ();
			this.CreateFirstView ();

			return null;  // ok
		}

		private string SaveMandat(string filename, SaveMandatMode mode)
		{
			return AssetsApplication.SaveMandat (this.accessor, filename, mode);
		}


		private void InitializeUndo()
		{
			this.accessor.UndoManager.SetViewStateGetter (delegate
			{
				return this.view.ViewState;
			});

			this.accessor.UndoManager.Changed += delegate
			{
				this.accessor.WarningsDirty = true;
				this.UpdateToolbarUndoRedo ();
			};

			this.UpdateToolbarUndoRedo ();
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

		private void UpdateToolbarUndoRedo()
		{
			var undo = this.accessor.UndoManager.IsUndoEnable;
			var redo = this.accessor.UndoManager.IsRedoEnable;

			this.commandContext.GetCommandState (Res.Commands.Main.Undo       ).Enable = undo;
			this.commandContext.GetCommandState (Res.Commands.Main.UndoHistory).Enable = undo;
			this.commandContext.GetCommandState (Res.Commands.Main.Redo       ).Enable = redo;
			this.commandContext.GetCommandState (Res.Commands.Main.RedoHistory).Enable = redo;

			var undoButton = this.toolbar.GetTarget (Res.Commands.Main.Undo);
			var redoButton = this.toolbar.GetTarget (Res.Commands.Main.Redo);

			if (undo)
			{
				ToolTip.Default.SetToolTip (undoButton, this.accessor.UndoManager.CurrentUndoDescription);
			}
			else
			{
				ToolTip.Default.ClearToolTip (undoButton);
			}

			if (redo)
			{
				ToolTip.Default.SetToolTip (redoButton, this.accessor.UndoManager.CurrentRedoDescription);
			}
			else
			{
				ToolTip.Default.ClearToolTip (redoButton);
			}
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
			if (this.view == null)
			{
				return;
			}

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

		private void RestoreUndoViewState(AbstractViewState viewState)
		{
			//	Restaure la vue après un undo/redo.
			if (viewState.GetType () == this.view.ViewState.GetType () &&
				!(viewState is AccountsViewState))  // voir (*)
			{
				//	Si la vue est toujours du même type, il n'est pas nécessaire de
				//	la recréer.
				this.view.ViewState = viewState;
			}
			else
			{
				//	Crée la vue du nouveau type.
				this.RestoreViewState (viewState);
			}

			// (*)	Comme la vue du plan comptable dépend de la période comptable,
			//		il est nécessaire de recréer cette vue à chaque fois !
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


		/// <summary>
		/// Cet agent de nettoyage s'occupe d'enlever un objet supprimé qui serait référencé
		/// par un AbstractViewState. Par exemple, si vous sélectionnez un objet d'immobilisation,
		/// il apparaît dans le menu des dernières vues utilisées (lastViewStates). Il sera
		/// également possible d'y revenir avec la commande Navigation.Back. S'il est supprimé,
		/// il faut impérativement empêcher ces opérations.
		/// </summary>
		private class CleanerAgent : AbstractCleanerAgent
		{
			public CleanerAgent(DataAccessor accessor, MainView mainView)
				: base (accessor)
			{
				this.mainView = mainView;
			}

			public override void Clean(BaseType baseType, Guid guid)
			{
				this.Clean (baseType, guid, this.mainView.currentViewStates);
				this.Clean (baseType, guid, this.mainView.lastViewStates);

				if (this.mainView.historyPosition == -1)
				{
					this.Clean (baseType, guid, this.mainView.historyViewStates);
				}
				else
				{
					this.Clean (baseType, guid, this.mainView.historyViewStates);

					this.mainView.historyPosition--;
					this.mainView.historyPosition = System.Math.Max (this.mainView.historyPosition, 0);
					this.mainView.historyPosition = System.Math.Min (this.mainView.historyPosition, this.mainView.historyViewStates.Count-1);
				}
			}

			private void Clean(BaseType baseType, Guid guid, List<AbstractViewState> list)
			{
				var toDelete = list.Where (x => x.IsReferenced (baseType, guid)).ToArray ();

				foreach (var delete in toDelete)
				{
					list.Remove (delete);
				}
			}

			private readonly MainView mainView;
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

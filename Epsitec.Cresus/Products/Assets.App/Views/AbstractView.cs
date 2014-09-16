//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Settings;
using Epsitec.Cresus.Assets.App.Views.CommandToolbars;
using Epsitec.Cresus.Assets.App.Views.FieldControllers;
using Epsitec.Cresus.Assets.App.Views.ViewStates;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public abstract class AbstractView : System.IDisposable
	{
		public AbstractView(DataAccessor accessor, CommandDispatcher commandDispatcher, CommandContext commandContext, MainToolbar toolbar, ViewType viewType)
		{
			this.accessor          = accessor;
			this.commandDispatcher = new CommandDispatcher (this.GetType ().FullName, CommandDispatcherLevel.Secondary, CommandDispatcherOptions.AutoForwardCommands);
			this.commandContext    = commandContext;
			this.mainToolbar       = toolbar;
			this.viewType          = viewType;

			this.ignoreChanges = new SafeCounter ();

			this.commandDispatcher.RegisterController (this);  // utile si écoute avec [Command (Res.CommandIds...)]
		}

		public virtual void Dispose()
		{
			this.commandDispatcher.Dispose ();
		}

		public virtual void Close()
		{
		}


		public virtual void CreateUI(Widget parent)
		{
			CommandDispatcher.SetDispatcher (parent, this.commandDispatcher);
		}

		public virtual void DataChanged()
		{
		}

		public virtual void DeepUpdateUI()
		{
			//	Met à jour la vue après un changement des données.
		}

		public virtual void UpdateUI()
		{
			//	Met à jour la vue.
		}


		public void CloseUI()
		{
			this.accessor.EditionAccessor.CancelObjectEdition ();
		}

		public virtual AbstractViewState		ViewState
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		protected virtual Guid					SelectedGuid
		{
			get
			{
				return Guid.Empty;
			}
		}

		public virtual void OnCommand(ToolbarCommand command)
		{
			switch (command)
			{
				case ToolbarCommand.Simulation:
					this.OnMainSimulation ();
					break;

				case ToolbarCommand.Locked:
					this.OnMainLocked ();
					break;
			}
		}

		[Command (Res.CommandIds.Main.Edit)]
		void CommandMainEdit(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = AbstractCommandToolbar.GetTarget (this.commandDispatcher, e);
			this.CommandMainEdit (target);
		}

		[Command (Res.CommandIds.Edit.Accept)]
		void CommandEditAccept(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = AbstractCommandToolbar.GetTarget (this.commandDispatcher, e);
			this.CommandEditAccept (target);
		}

		[Command (Res.CommandIds.Edit.Cancel)]
		void CommandEditCancel(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = AbstractCommandToolbar.GetTarget (this.commandDispatcher, e);
			this.CommandEditCancel (target);
		}

		protected virtual void CommandMainEdit(Widget target)
		{
		}

		protected virtual void CommandEditAccept(Widget target)
		{
		}

		protected virtual void CommandEditCancel(Widget target)
		{
		}


		private void OnMainSimulation()
		{
			var target = this.mainToolbar.GetTarget (ToolbarCommand.Simulation);

#if false
			var popup = new SimulationPopup
			{
				Simulation = DataAccessor.Simulation,
			};

			popup.Create (target);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name.StartsWith ("use-"))
				{
					DataAccessor.Simulation = popup.Simulation;
					this.mainToolbar.Simulation = popup.Simulation;
					this.OnCommand (ToolbarCommand.Accept);
				}
				else if (name.StartsWith ("clear-"))
				{
				}
			};
#else
			var popup = new StackedTestPopup (this.accessor)
			{
				DateFrom  = new System.DateTime (2014, 3, 31),
				DateTo    = new System.DateTime (2014, 4, 1),
				Operation = 1,
				FieldName = "Coucou",
				Quantity  = 99,
				Color     = 0,
				Samples   = true,
			};

			popup.Create (target);
#endif
		}

		private void OnMainLocked()
		{
			var target = this.mainToolbar.GetTarget (ToolbarCommand.Locked);

			var popup = new LockedPopup (this.accessor)
			{
				IsDelete            = false,
				IsAll               =  this.SelectedGuid.IsEmpty,
				OneSelectionAllowed = !this.SelectedGuid.IsEmpty,
				Date                = LocalSettings.LockedDate,
			};

			popup.Create (target);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					if (popup.Date.HasValue)
					{
						LocalSettings.LockedDate = popup.Date.Value;
					}

					var guid = popup.IsAll ? Guid.Empty : this.SelectedGuid;
					var createDate = popup.Date.GetValueOrDefault ();

					if (!popup.IsDelete)  // verrouiller ?
					{
						if (!AssetCalculator.IsLockable (this.accessor, guid, createDate))
						{
							MessagePopup.ShowAssetsPreviewEventWarning (target);
							return;
						}
					}

					AssetCalculator.Locked (this.accessor, guid, popup.IsDelete, createDate);
					this.DeepUpdateUI ();
				}
			};
		}


		protected Timestamp? GetLastTimestamp(Guid guid)
		{
			var obj = this.accessor.GetObject (this.baseType, guid);
			if (obj != null)
			{
				return AssetCalculator.GetLastTimestamp (obj);
			}

			return null;
		}


		public static AbstractView CreateView(ViewType viewType, DataAccessor accessor,
			CommandDispatcher commandDispatcher, CommandContext commandContext,
			MainToolbar toolbar, List<AbstractViewState> historyViewStates)
		{
			switch (viewType.Kind)
			{
				case ViewTypeKind.Assets:
					return new AssetsView (accessor, commandDispatcher, commandContext, toolbar, viewType);

				case ViewTypeKind.Amortizations:
					return new AmortizationsView (accessor, commandDispatcher, commandContext, toolbar, viewType);

				case ViewTypeKind.Categories:
					return new CategoriesView (accessor, commandDispatcher, commandContext, toolbar, viewType);

				case ViewTypeKind.Groups:
					return new GroupsView (accessor, commandDispatcher, commandContext, toolbar, viewType);

				case ViewTypeKind.Persons:
					return new PersonsView (accessor, commandDispatcher, commandContext, toolbar, viewType);

				case ViewTypeKind.Reports:
					return new ReportsView (accessor, commandDispatcher, commandContext, toolbar, viewType, historyViewStates);

				case ViewTypeKind.Warnings:
					return new WarningsView (accessor, commandDispatcher, commandContext, toolbar, viewType);

				case ViewTypeKind.AssetsSettings:
					return new UserFieldsSettingsView (accessor, commandDispatcher, commandContext, toolbar, viewType, BaseType.Assets);

				case ViewTypeKind.PersonsSettings:
					return new UserFieldsSettingsView (accessor, commandDispatcher, commandContext, toolbar, viewType, BaseType.Persons);

				case ViewTypeKind.Entries:
					return new EntriesView (accessor, commandDispatcher, commandContext, toolbar, viewType);

				case ViewTypeKind.Accounts:
					var baseType = new BaseType (BaseTypeKind.Accounts, viewType.AccountsDateRange);
					return new AccountsView (accessor, commandDispatcher, commandContext, toolbar, viewType, baseType);

				default:
					return null;
			}
		}

		public string GetViewTitle(ViewType viewType)
		{
			//	Retourne le titre à utiliser pour une vue, incluant le nom du mandat.
			return AbstractView.GetViewTitle (this.accessor, viewType);
		}

		public static string GetViewTitle(DataAccessor accessor, ViewType viewType)
		{
			//	Retourne le titre à utiliser pour une vue, incluant le nom du mandat.
			return AbstractView.GetViewTitle (accessor, StaticDescriptions.GetViewTypeDescription (viewType.Kind));
		}

		public static string GetViewTitle(DataAccessor accessor, string title)
		{
			//	Retourne le titre à utiliser pour une vue, incluant le nom du mandat.
			if (string.IsNullOrEmpty (accessor.Mandat.Name))
			{
				return title;
			}
			else
			{
				return string.Concat (accessor.Mandat.Name, " — ", title);
			}
		}


		protected void UpdateWarningsRedDot()
		{
			//	Met à jour le nombre d'avertissements dans la pastille rouge sur le
			//	bouton de la vue des avertissements.
			//	ATTENTION: Il faut construire la liste complète des avertissements,
			//	ce qui peut prendre du temps !
			//	TODO: Rendre cela asynchrone !?
			if (this.accessor.WarningsDirty)
			{
				var list = new List<Warning> ();
				WarningsLogic.GetWarnings (list, this.accessor);

				this.mainToolbar.WarningsRedDotCount = list.Count;

				this.accessor.WarningsDirty = false;
			}
		}


		#region Events handler
		protected void OnGoto(AbstractViewState viewState)
		{
			this.Goto.Raise (this, viewState);
		}

		public event EventHandler<AbstractViewState> Goto;


		protected void OnViewStateChanged(AbstractViewState viewState)
		{
			this.ViewStateChanged.Raise (this, viewState);
		}

		public event EventHandler<AbstractViewState> ViewStateChanged;


		protected void OnChangeView(ViewType viewType)
		{
			this.ChangeView.Raise (this, viewType);
		}

		public event EventHandler<ViewType> ChangeView;
		#endregion


		public const int scrollerDefaultBreadth = 17;  // AbstractScroller.DefaultBreadth pas const !
		public const int editionWidth = AbstractFieldController.labelWidth + AbstractFieldController.maxWidth + 70 + AbstractView.scrollerDefaultBreadth;

		protected readonly DataAccessor			accessor;
		protected readonly CommandDispatcher	commandDispatcher;
		protected readonly CommandContext		commandContext;
		protected readonly MainToolbar			mainToolbar;
		protected readonly ViewType				viewType;
		protected readonly SafeCounter			ignoreChanges;

		protected BaseType						baseType;
	}
}

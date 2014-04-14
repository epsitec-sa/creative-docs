//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public abstract class AbstractView
	{
		public AbstractView(DataAccessor accessor, MainToolbar toolbar)
		{
			this.accessor = accessor;
			this.mainToolbar = toolbar;

			this.amortizations = new Amortizations (this.accessor);

			this.ignoreChanges = new SafeCounter ();
		}


		public virtual void Dispose()
		{
		}


		public virtual void CreateUI(Widget parent)
		{
		}

		public virtual void DataChanged()
		{
		}

		public virtual void DeepUpdateUI()
		{
		}

		public virtual void UpdateUI()
		{
		}

		public virtual AbstractViewState ViewState
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		protected virtual Guid SelectedGuid
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


		private void OnMainSimulation()
		{
			var target = this.mainToolbar.GetTarget (ToolbarCommand.Simulation);

			if (target != null)
			{
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
			}
		}

		private void OnMainLocked()
		{
			var target = this.mainToolbar.GetTarget (ToolbarCommand.Locked);

			if (target != null)
			{
				var popup = new LockedPopup (this.accessor) 
				{
					OneSelectionAllowed = !this.SelectedGuid.IsEmpty,
					Date                = Timestamp.Now.Date,
				};

				popup.Create (target);

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (name == "ok")
					{
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


		public static AbstractView CreateView(ViewType viewType, DataAccessor accessor, MainToolbar toolbar)
		{
			switch (viewType)
			{
				case ViewType.Assets:
					return new AssetsView (accessor, toolbar);

				case ViewType.Amortizations:
					return new AmortizationsView (accessor, toolbar);

				case ViewType.Categories:
					return new CategoriesView (accessor, toolbar);

				case ViewType.Groups:
					return new GroupsView (accessor, toolbar);

				case ViewType.Persons:
					return new PersonsView (accessor, toolbar);

				case ViewType.Reports:
					return new ReportsView (accessor, toolbar);

				case ViewType.AssetsSettings:
					return new UserFieldsSettingsView (accessor, toolbar, BaseType.Assets);

				case ViewType.PersonsSettings:
					return new UserFieldsSettingsView (accessor, toolbar, BaseType.Persons);

				case ViewType.AccountsSettings:
					return new AccountsSettingsView (accessor, toolbar);

				case ViewType.Entries:
					return new EntriesView (accessor, toolbar);

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
			if (string.IsNullOrEmpty (accessor.Mandat.Name))
			{
				return StaticDescriptions.GetViewTypeDescription (viewType);
			}
			else
			{
				return string.Concat (accessor.Mandat.Name, " — ", StaticDescriptions.GetViewTypeDescription (viewType));
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
		#endregion


		public const int scrollerDefaultBreadth = 17;  // AbstractScroller.DefaultBreadth pas const !
		public const int editionWidth = AbstractFieldController.labelWidth + AbstractFieldController.maxWidth + 70 + AbstractView.scrollerDefaultBreadth;

		protected readonly DataAccessor			accessor;
		protected readonly MainToolbar			mainToolbar;
		protected readonly Amortizations		amortizations;
		protected readonly SafeCounter			ignoreChanges;

		protected BaseType						baseType;
	}
}

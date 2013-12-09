//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
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

			this.amortissements = new Amortissements (this.accessor);
		}


		public virtual void Dispose()
		{
		}


		public virtual void CreateUI(Widget parent)
		{
		}

		protected virtual Guid SelectedObjectGuid
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
				case ToolbarCommand.Amortissement:
					this.OnMainAmortissement ();
					break;

				case ToolbarCommand.Simulation:
					this.OnMainSimulation ();
					break;
			}
		}


		private void OnMainAmortissement()
		{
			var target = this.mainToolbar.GetTarget (ToolbarCommand.Amortissement);

			var now = System.DateTime.Now;

			var popup = new AmortissementsPopup (this.accessor)
			{
				OneSelectionAllowed = !this.SelectedObjectGuid.IsEmpty,
				DateFrom            = new System.DateTime (now.Year, 1, 1),
				DateTo              = new System.DateTime (now.Year, 12, 31),
			};

			popup.Create (target);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					System.Diagnostics.Debug.Assert (popup.DateFrom.HasValue);
					System.Diagnostics.Debug.Assert (popup.DateTo.HasValue);
					var range = new DateRange (popup.DateFrom.Value, popup.DateTo.Value);

					this.Amortissements (target, popup.IsCreate, popup.IsAll, range);
				}
			};
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


		private void Amortissements(Widget target, bool isCreate, bool isAll, DateRange range)
		{
			//	Effectue les opérations d'amortissement selon AmortissementsPopup,
			//	et affiche les résultats avec ErrorsPopup.

			if (isCreate)  // génère les amortissements ?
			{
				if (isAll)
				{
					var errors = this.amortissements.GeneratesAmortissementsAuto (range);
					this.ShowErrorPopup (target, errors);
				}
				else
				{
					System.Diagnostics.Debug.Assert (!this.SelectedObjectGuid.IsEmpty);
					var errors = this.amortissements.GeneratesAmortissementsAuto (range, this.SelectedObjectGuid);
					this.ShowErrorPopup (target, errors);
				}
			}
			else  // supprime les amortissements ?
			{
				if (isAll)
				{
					var errors = this.amortissements.RemovesAmortissementsAuto (range);
					this.ShowErrorPopup (target, errors);
				}
				else
				{
					System.Diagnostics.Debug.Assert (!this.SelectedObjectGuid.IsEmpty);
					var errors = this.amortissements.RemovesAmortissementsAuto (range, this.SelectedObjectGuid);
					this.ShowErrorPopup (target, errors);
				}
			}

			this.Update (dataChanged: true);
		}

		private void ShowErrorPopup(Widget target, List<Error> errors)
		{
			var popup = new ErrorsPopup (this.accessor, errors);
			popup.Create (target);
		}


		protected virtual void Update(bool dataChanged = false)
		{
		}


		protected Timestamp? GetLastTimestamp(Guid guid)
		{
			var obj = this.accessor.GetObject (this.baseType, guid);
			if (obj != null)
			{
				return ObjectCalculator.GetLastTimestamp (obj);
			}

			return null;
		}


		public static AbstractView CreateView(ViewType viewType, DataAccessor accessor, MainToolbar toolbar)
		{
			switch (viewType)
			{
				case ViewType.Objects:
					return new ObjectsView (accessor, toolbar);

				case ViewType.Categories:
					return new CategoriesView (accessor, toolbar);

				case ViewType.Groups:
					return new GroupsView (accessor, toolbar);

				case ViewType.Persons:
					return new PersonsView (accessor, toolbar);

				case ViewType.Events:
					//return new EventsView (accessor, toolbar);

				case ViewType.Reports:
					return new ReportsView (accessor, toolbar);

				case ViewType.Settings:
					return new SettingsView (accessor, toolbar);

				default:
					return null;
			}
		}


		#region Events handler
		protected void OnGoto(BaseType baseType, Guid guid, PageType pageType)
		{
			this.Goto.Raise (this, baseType, guid, pageType);
		}

		public event EventHandler<BaseType, Guid, PageType> Goto;
		#endregion


		protected readonly DataAccessor			accessor;
		protected readonly MainToolbar			mainToolbar;
		protected readonly Amortissements		amortissements;

		protected BaseType						baseType;
	}
}

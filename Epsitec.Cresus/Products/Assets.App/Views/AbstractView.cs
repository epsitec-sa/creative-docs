//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public abstract class AbstractView
	{
		public AbstractView(DataAccessor accessor, MainToolbar toolbar)
		{
			this.accessor = accessor;
			this.mainToolbar = toolbar;

			this.businessLogic = new BusinessLogic (this.accessor);
		}


		public virtual void Dispose()
		{
		}


		public virtual void CreateUI(Widget parent)
		{
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
			var target = this.mainToolbar.GetCommandWidget (ToolbarCommand.Amortissement);

			if (target != null)
			{
				var popup = new DeletePopup
				{
					Question = "Voulez-vous générer les amortissements ?",
				};

				popup.Create (target);

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (name == "yes")
					{
						this.businessLogic.GénèreAmortissementsAuto ();
						this.Update ();
					}
				};
			}
		}

		private void OnMainSimulation()
		{
			var target = this.mainToolbar.GetCommandWidget (ToolbarCommand.Simulation);

			if (target != null)
			{
				var popup = new DeletePopup
				{
					Question = "Voulez-vous débuter une simulation ?",
				};

				popup.Create (target);

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (name == "yes")
					{
					}
				};
			}
		}


		protected virtual void Update()
		{
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

				case ViewType.Events:
				//?return new ObjectsTreeTableView (accessor, toolbar);

				case ViewType.Reports:
					return new ReportsView (accessor, toolbar);

				case ViewType.Settings:
					return new SettingsView (accessor, toolbar);

				default:
					return null;
			}
		}


		protected readonly DataAccessor			accessor;
		protected readonly MainToolbar			mainToolbar;
		protected readonly BusinessLogic		businessLogic;
	}
}

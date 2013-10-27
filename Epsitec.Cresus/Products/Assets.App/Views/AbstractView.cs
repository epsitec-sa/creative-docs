//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
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
				case ToolbarCommand.Simulation:
					this.OnMainSimulation ();
					break;
			}
		}


		private void OnMainSimulation()
		{
			var target = this.mainToolbar.GetCommandWidget (ToolbarCommand.Simulation);

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

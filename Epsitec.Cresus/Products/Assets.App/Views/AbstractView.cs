//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public abstract class AbstractView
	{
		public AbstractView(DataAccessor accessor)
		{
			this.accessor = accessor;
		}

		public virtual void CreateUI(Widget parent, MainToolbar toolbar)
		{
			this.mainToolbar = toolbar;
		}


		protected virtual void Update()
		{
		}


		public static AbstractView CreateView(ViewType viewType, DataAccessor accessor)
		{
			switch (viewType)
			{
				case ViewType.Objects:
					return new ObjectsView (accessor);

				case ViewType.Categories:
					return new CategoriesView (accessor);

				case ViewType.Groups:
					return new GroupsView (accessor);

				case ViewType.Events:
					return new EventsView (accessor);

				case ViewType.Reports:
					return new ReportsView (accessor);

				case ViewType.Settings:
					return new SettingsView (accessor);

				default:
					return null;
			}
		}


		protected readonly DataAccessor			accessor;

		protected MainToolbar					mainToolbar;
	}
}

//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Views
{
	public abstract class AbstractView
	{
		public virtual void CreateUI(Widget parent, MainToolbar toolbar)
		{
			this.toolbar = toolbar;
		}


		public virtual string Title
		{
			get
			{
				return null;
			}
		}


		public static AbstractView CreateView(ViewType viewType)
		{
			switch (viewType)
			{
				case ViewType.Objects:
					return new ObjectsView ();

				case ViewType.Categories:
					return new CategoriesView ();

				case ViewType.Groups:
					return new GroupsView ();

				case ViewType.Events:
					return new EventsView ();

				case ViewType.Reports:
					return new ReportsView ();

				case ViewType.Settings:
					return new SettingsView ();

				default:
					return null;
			}
		}


		protected MainToolbar toolbar;
	}
}

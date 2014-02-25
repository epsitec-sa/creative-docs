//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public abstract class AbstractSettingsView
	{
		public AbstractSettingsView(DataAccessor accessor, MainToolbar mainToolbar)
		{
			this.accessor    = accessor;
			this.mainToolbar = mainToolbar;
		}


		public virtual void CreateUI(Widget parent)
		{
		}


		public static AbstractSettingsView CreateView(DataAccessor accessor, MainToolbar mainToolbar, ToolbarCommand command)
		{
			switch (command)
			{
				case ToolbarCommand.SettingsGeneral:
					return new GeneralSettingsView (accessor, mainToolbar);

				case ToolbarCommand.SettingsAssetsView:
					return new UserFieldsSettingsView (accessor, mainToolbar, BaseType.Assets);

				case ToolbarCommand.SettingsPersonsView:
					return new UserFieldsSettingsView (accessor, mainToolbar, BaseType.Persons);

				case ToolbarCommand.SettingsAccounts:
					return new AccountsSettingsView (accessor, mainToolbar);

				default:
					throw new System.InvalidOperationException (string.Format ("Unknown SettingsView {0}", command.ToString ()));
			}
		}


		protected readonly DataAccessor			accessor;
		protected readonly MainToolbar			mainToolbar;
	}
}

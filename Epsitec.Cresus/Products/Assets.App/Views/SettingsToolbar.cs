//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class SettingsToolbar : AbstractCommandToolbar
	{
		public override FrameBox CreateUI(Widget parent)
		{
			this.toolbar = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = AbstractCommandToolbar.secondaryToolbarHeight,
				BackColor       = ColorManager.SettingsToolbarBackgroundColor,
			};

			this.CreateButton (ToolbarCommand.SettingsGeneral,     "Settings.General");
			this.CreateButton (ToolbarCommand.SettingsAssetsView,  "Settings.AssetsView");
			this.CreateButton (ToolbarCommand.SettingsPersonsView, "Settings.PersonsView");
			this.CreateButton (ToolbarCommand.SettingsAccounts,    "Settings.Accounts");

			return this.toolbar;
		}

		public void UpdateToolbar(ToolbarCommand selectedCommand)
		{
			foreach (var command in SettingsToolbar.Commands)
			{
				this.SetCommandActivate (command, command == selectedCommand);
			}
		}


		private void CreateButton(ToolbarCommand command, string icon)
		{
			string tooltip = SettingsToolbar.GetCommandDescription (command);
			var button = this.CreateCommandButton (DockStyle.Left, command, icon, tooltip);
			button.ButtonStyle = ButtonStyle.ActivableIcon;

			this.SetCommandEnable (command, true);
		}

		public static string GetCommandDescription(ToolbarCommand command)
		{
			switch (command)
			{
				case ToolbarCommand.SettingsGeneral:
					return "Réglages généraux";

				case ToolbarCommand.SettingsAssetsView:
					return "Champs des objets d'immobilisation";

				case ToolbarCommand.SettingsPersonsView:
					return "Champs des personnes";

				case ToolbarCommand.SettingsAccounts:
					return "Plan comptable";

				default:
					return null;

			}
		}

		private static IEnumerable<ToolbarCommand> Commands
		{
			get
			{
				yield return ToolbarCommand.SettingsGeneral;
				yield return ToolbarCommand.SettingsAssetsView;
				yield return ToolbarCommand.SettingsPersonsView;
				yield return ToolbarCommand.SettingsAccounts;
			}
		}
	}
}

//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BrowserControllers
{
	/// <summary>
	/// The <c>BrowserSettingsController</c> class manages the settings associated
	/// with a <see cref="BrowserViewController"/>, such as the search criteria.
	/// </summary>
	class BrowserSettingsController : CoreViewController
	{
		public BrowserSettingsController(BrowserViewController browser)
			: base ("BrowserSettings", browser.Orchestrator)
		{
			this.browser = browser;
			this.browser.DataSetSelected += this.HandleBrowserDataSetSelected;
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public override void CreateUI(Widget container)
		{
			base.CreateUI (container);

			this.CreateCommandInfrastructure (container);
			this.CreateUINewItemIconButton (container);
		}

		private void CreateCommandInfrastructure(Widget container)
		{
			this.commandDispatcher = new CommandDispatcher ("BrowserSettings", CommandDispatcherLevel.Primary);
			this.commandDispatcher.AutoForwardCommands = true;

			this.commandContext    = new CommandContext ("BrowserSettings");

			CommandDispatcher.SetDispatcher (container, this.commandDispatcher);
			CommandContext.SetContext (container, this.commandContext);

			this.RegisterCommandHandlers ();
		}

		private void RegisterCommandHandlers()
		{
			this.commandDispatcher.Register (ApplicationCommands.New, this.ExecuteNewCommand);
		}

		private void CreateUINewItemIconButton(Widget container)
		{
			this.newItemIconButton = new LayeredIconButton
			{
				CommandObject = ApplicationCommands.New,
				Name = "CreateNewItem",
				Parent = container,
				PreferredSize = new Size (28, 28),
				PreferredIconSize = new Size (24, 24),
				Dock = DockStyle.Left,
				IconUri = Misc.GetResourceIconUri ("Edition.NewRecord"),
			};
		}

		
		private void HandleBrowserDataSetSelected(object sender)
		{
			this.UpdateNewItemIconButton ();
		}

		private void ExecuteNewCommand(object sender, CommandEventArgs e)
		{
			this.browser.AddNewEntity ();
		}

		private void UpdateNewItemIconButton()
		{
			string name = "Base." + this.browser.DataSetName;

			this.newItemIconButton.ClearOverlays ();
			this.newItemIconButton.AddOverlay (Misc.GetResourceIconUri (name), new Size (20, 20));
		}

		private readonly BrowserViewController browser;

		private LayeredIconButton newItemIconButton;
		private CommandDispatcher commandDispatcher;
		private CommandContext    commandContext;
	}
}

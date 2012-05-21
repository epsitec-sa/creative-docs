//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	public sealed class BrowserSettingsController : CoreViewController
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

			this.CreateUIButtonNewCommand (container);
			this.CreateUIButtonDeleteCommand (container);
		}

		internal void NotifyBrowserSettingsModeChanged(BrowserSettingsMode mode)
		{
		}

		private void CreateCommandInfrastructure(Widget container)
		{
			this.commandDispatcher = new CommandDispatcher ("BrowserSettings", CommandDispatcherLevel.Primary, CommandDispatcherOptions.AutoForwardCommands);
			this.commandContext    = new CommandContext ("BrowserSettings");

			CommandDispatcher.SetDispatcher (container, this.commandDispatcher);
			CommandContext.SetContext (container, this.commandContext);

			this.RegisterCommandHandlers ();
		}

		private void RegisterCommandHandlers()
		{
			this.commandDispatcher.Register (ApplicationCommands.New, this.ExecuteNewCommand);
			this.commandDispatcher.Register (ApplicationCommands.Delete, this.ExecuteDeleteCommand);
		}

		private void CreateUIButtonNewCommand(Widget container)
		{
			this.newItemIconButton = new LayeredIconButton
			{
				CommandObject = ApplicationCommands.New,
				Name = "CreateNewItem",
				Parent = container,
				PreferredSize = new Size (40, 28),
				PreferredIconSize = new Size (32, 24),
				Dock = DockStyle.Left,
				IconUri = Misc.GetResourceIconUri ("Edition.NewRecord"),
			};
		}

		private void CreateUIButtonDeleteCommand(Widget container)
		{
			this.deleteItemIconButton = new LayeredIconButton
			{
				CommandObject = ApplicationCommands.Delete,
				Name = "DeleteItem",
				Parent = container,
				PreferredSize = new Size (40, 28),
				PreferredIconSize = new Size (32, 24),
				Dock = DockStyle.Left,
				IconUri = Misc.GetResourceIconUri ("Edition.DeleteRecord"),
			};
		}

		
		private void HandleBrowserDataSetSelected(object sender)
		{
			this.UpdateNewItemIconButton ();
		}

		private void ExecuteNewCommand()
		{
			this.browser.AddNewEntity ();
		}

		private void ExecuteDeleteCommand()
		{
			this.browser.DeleteActiveEntity ();
		}

		private void UpdateNewItemIconButton()
		{
			var type = this.browser.DataSetEntityType;
			var name = "Base." + this.browser.DataSetName;

			this.newItemIconButton.ClearOverlays ();
			this.newItemIconButton.AddOverlay (Misc.GetResourceIconUri (name, type), new Size (20, 20));
			this.newItemIconButton.AddOverlay (Misc.GetResourceIconUri ("Edition.NewRecord.Overlay"), new Size (32, 24));

			this.deleteItemIconButton.ClearOverlays ();
			this.deleteItemIconButton.AddOverlay (Misc.GetResourceIconUri (name, type), new Size (20, 20));
			this.deleteItemIconButton.AddOverlay (Misc.GetResourceIconUri ("Edition.DeleteRecord.Overlay"), new Size (32, 24));
		}

		private readonly BrowserViewController browser;

		private LayeredIconButton	newItemIconButton;
		private LayeredIconButton	deleteItemIconButton;
		private CommandDispatcher	commandDispatcher;
		private CommandContext		commandContext;
	}
}

//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.BrowserControllers;
using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.CommandHandlers
{
	public class NavigationCommandHandler : ICommandHandler
	{
		public NavigationCommandHandler(CoreCommandDispatcher commandDispatcher)
		{
			this.commandDispatcher = commandDispatcher;
		}

		[Command (Core.Res.CommandIds.History.NavigateBackward)]
		public void ProcessNavigateBackward(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var navigator = this.GetNavigator (e);
			navigator.History.NavigateBackward ();
		}

		[Command (Core.Res.CommandIds.History.NavigateForward)]
		public void ProcessNavigateForward(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var navigator = this.GetNavigator (e);
			navigator.History.NavigateForward ();
		}

		private Epsitec.Cresus.Core.Orchestrators.NavigationOrchestrator GetNavigator(CommandEventArgs e)
		{
			var context    = e.CommandContext;
			var controller = this.commandDispatcher.GetApplicationComponent<MainViewController> ();
			var navigator  = controller.Navigator;
			
			return navigator;
		}

		#region ICommandHandler Members

		void ICommandHandler.UpdateCommandStates(object sender)
		{
		}

		#endregion

		private readonly CoreCommandDispatcher commandDispatcher;
	}
}

//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.BrowserControllers;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Library.Internal;

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

		[Command (Core.Library.Res.CommandIds.History.NavigateBackward)]
		public void ProcessNavigateBackward(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var navigator = this.GetNavigator (e);
			navigator.History.NavigateBackward ();
		}

		[Command (Core.Library.Res.CommandIds.History.NavigateForward)]
		public void ProcessNavigateForward(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var navigator = this.GetNavigator (e);
			navigator.History.NavigateForward ();
		}


		[Command (Core.Library.Res.CommandIds.Focus.CloseView)]
		public void ProcessCloseView(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var navigator = this.GetNavigator (e);
			navigator.CloseLeafSubview ();
		}

		[Command (Core.Library.Res.CommandIds.Focus.ToggleView1)]
		[Command (Core.Library.Res.CommandIds.Focus.ToggleView2)]
		[Command (Core.Library.Res.CommandIds.Focus.ToggleView3)]
		[Command (Core.Library.Res.CommandIds.Focus.ToggleView4)]
		[Command (Core.Library.Res.CommandIds.Focus.ToggleView5)]
		[Command (Core.Library.Res.CommandIds.Focus.ToggleView6)]
		[Command (Core.Library.Res.CommandIds.Focus.ToggleView7)]
		[Command (Core.Library.Res.CommandIds.Focus.ToggleView8)]
		[Command (Core.Library.Res.CommandIds.Focus.ToggleView9)]
		public void ProcessToggleView(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var navigator = this.GetNavigator (e);

			int view = e.Command.Name.LastCharacter () - '1';

			navigator.ToggleView (view);
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

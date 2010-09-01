//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.CommandHandlers
{
	public class CoreCommandHandler : ICommandHandler
	{
		public CoreCommandHandler(CoreCommandDispatcher commandDispatcher)
		{
			this.commandDispatcher = commandDispatcher;
		}


		[Command (Res.CommandIds.Edition.SaveRecord)]
		public void ProcessEditionSaveRecord(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.commandDispatcher.Dispatch (dispatcher, e);
		}

		[Command (Res.CommandIds.Edition.DiscardRecord)]
		public void ProcessEditionDiscardRecord(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var orchestrator = CoreCommandDispatcher.GetOrchestrator (e);
			var navigator    = orchestrator.Navigator;
			var history      = navigator.History;
			var path         = navigator.GetLeafNavigationPath ();

			using (history.SuspendRecording ())
			{
				this.commandDispatcher.Dispatch (dispatcher, e);
				
				history.NavigateInPlace (path);
			}
		}

		[Command (Res.CommandIds.Edition.Print)]
		public void ProcessEditionPrint(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var orchestrator = CoreCommandDispatcher.GetOrchestrator (e);
			var mainViewController = orchestrator.MainViewController;

			mainViewController.Print ();
		}

		[Command (Res.CommandIds.Edition.Preview)]
		public void ProcessEditionPreview(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var orchestrator = CoreCommandDispatcher.GetOrchestrator (e);
			var mainViewController = orchestrator.MainViewController;

			mainViewController.Preview ();
		}

		[Command (Res.CommandIds.File.ImportV11)]
		public void ProcessFileImportV11(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var orchestrator = CoreCommandDispatcher.GetOrchestrator (e);
			var mainViewController = orchestrator.MainViewController;

			mainViewController.FileImportV11 ();
		}

		[Command (Res.CommandIds.Global.Settings)]
		public void ProcessGlobalSettings(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.commandDispatcher.Dispatch (dispatcher, e,
				delegate
				{
					using (var dialog = new Dialogs.PrinterListDialog (CoreProgram.Application))
					{
						dialog.OpenDialog ();
					}
				});
		}


		[Command (Core.Res.CommandIds.Test.Crash)]
		public void ProcessTestCrash()
		{
			throw new System.Exception ("Crashing the application on purpose");
		}

		private readonly CoreCommandDispatcher commandDispatcher;
	}

}

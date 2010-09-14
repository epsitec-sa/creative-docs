﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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


		[Command (ApplicationCommands.Id.Quit)]
		public void ProcessQuit(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			CoreProgram.Application.Shutdown ();
		}

		[Command (Res.CommandIds.Edition.SaveRecord)]
		public void ProcessEditionSaveRecord(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var orchestrator = CoreProgram.Application.MainWindowOrchestrator;
			var navigator    = orchestrator.Navigator;

			navigator.PreserveNavigation (
				delegate
				{
					orchestrator.ClearActiveEntity ();
				});

			e.Executed = true;
		}

		[Command (Res.CommandIds.Edition.DiscardRecord)]
		public void ProcessEditionDiscardRecord(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var orchestrator = CoreProgram.Application.MainWindowOrchestrator;
			var navigator    = orchestrator.Navigator;

			navigator.PreserveNavigation (
				delegate
				{
					orchestrator.DefaultBusinessContext.Discard ();
					orchestrator.ClearActiveEntity ();
				});
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
			ImportV11.Import (CoreProgram.Application);
		}

		[Command (Res.CommandIds.Global.Settings)]
		public void ProcessGlobalSettings(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.commandDispatcher.Dispatch (dispatcher, e,
				delegate
				{
					using (var dialog = new Dialogs.PrinterUnitListDialog (CoreProgram.Application))
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

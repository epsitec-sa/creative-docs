//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Orchestrators;

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


		public DataViewOrchestrator Orchestrator
		{
			get
			{
				return this.commandDispatcher.Host.FindActiveComponent<DataViewOrchestrator> ();
			}
		}

		[Command (ApplicationCommands.Id.Quit)]
		public void ProcessQuit(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			CoreProgram.Application.Shutdown ();
		}


		[Command (Library.Res.CommandIds.Edition.SaveRecord)]
		public void ProcessEditionSaveRecord(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var orchestrator = this.Orchestrator;
			var navigator    = orchestrator.Navigator;

			navigator.PreserveNavigation (
				delegate
				{
					orchestrator.ClearActiveEntity ();
				});

			e.Executed = true;
		}

		[Command (Library.Res.CommandIds.Edition.DiscardRecord)]
		public void ProcessEditionDiscardRecord(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var orchestrator = this.Orchestrator;
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
			var orchestrator = this.Orchestrator;
			var mainViewController = orchestrator.MainViewController;

			mainViewController.Print ();
		}

		[Command (Res.CommandIds.Edition.Preview)]
		public void ProcessEditionPreview(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var orchestrator = this.Orchestrator;
			var mainViewController = orchestrator.MainViewController;

			mainViewController.Preview ();
		}

		[Command (Res.CommandIds.File.ImportV11)]
		public void ProcessFileImportV11(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var v11 = new V11.ImportFile ();
			v11.Import (CoreProgram.Application);
		}

		[Command (Res.CommandIds.Global.ShowSettings)]
		public void ProcessGlobalShowSettings(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.commandDispatcher.Dispatch (dispatcher, e,
				delegate
				{
					using (var dialog = new Dialogs.SettingsDialog (CoreProgram.Application, this.Orchestrator.DefaultBusinessContext))
					{
						dialog.OpenDialog ();
					}
				});
		}

		[Command (Res.CommandIds.Global.ShowDebug)]
		public void ProcessGlobalShowDebug(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.commandDispatcher.Dispatch (dispatcher, e,
				delegate
				{
					using (var dialog = new Dialogs.DebugDialog (CoreProgram.Application))
					{
						dialog.OpenDialog ();
					}
				});
		}

		[Command (Res.CommandIds.Global.ShowUserManager)]
		public void ProcessGlobalShowUserManager(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.commandDispatcher.Dispatch (dispatcher, e,
				delegate
				{
					var app = CoreProgram.Application;
					var manager = app.UserManager;
					manager.Authenticate (app, app.Data, manager.AuthenticatedUser, softwareStartup: false);
				});
		}


		[Command (Core.Res.CommandIds.Test.Crash)]
		public void ProcessTestCrash()
		{
			throw new System.Exception ("Crashing the application on purpose");
		}

		#region ICommandHandler Members

		void ICommandHandler.UpdateCommandStates(object sender)
		{
		}

		#endregion

		private readonly CoreCommandDispatcher commandDispatcher;
	}

}
